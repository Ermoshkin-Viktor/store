

using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using System;
using System.Linq;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;

        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository)
        {
            this.bookRepository = bookRepository;
            this.orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            //если в корзине есть книги
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                //загружаем ордер(подгружаем нужные книги)
                var order = orderRepository.GetById(cart.OrderId);
                //создаем модель
                OrderModel model = Map(order);
                return View(model);
            }
            return View("Empty");
        }

        private OrderModel Map(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);
            var books = bookRepository.GetAllByIds(bookIds);
            var itemModels = from item in order.Items
                             join book in books on item.BookId equals book.Id
                             select new OrderItemModel
                             {
                                 BookId = book.Id,
                                 Title= book.Title,
                                 Author= book.Author,
                                 Price= item.Price,
                                 Count= item.Count
                             };
            return new OrderModel
            {
                Id = order.Id,             
                Items = itemModels.ToArray(),
                TotalCount= order.TotalCount,
                TotalPrice= order.TotalPrice,
            };
        }

        public IActionResult AddItem(int bookId, int count = 1)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            //получаем книгу из репозитория
            var book = bookRepository.GetById(bookId);

            order.AddOrUpdateItem(book, count);

            //обновляем в базе
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        //обновление
        public IActionResult UpdateItem(int bookId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            //получаем книгу, увеличиваем к-во          
            order.GetItem(bookId).Count = count;
            //обновляем в базе
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }

        private void SaveOrderAndCart(Order order, Cart cart)
        {
            orderRepository.Update(order);
            //обновляем к-во и цену
            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;
            //сохраняем сессию
            HttpContext.Session.Set(cart);
        }

        private (Order order, Cart cart) GetOrCreateOrderAndCart()
        {
            Order order;
            //Если корзина с таким id есть
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                //Если нет, создаем новый заказ
                order = orderRepository.Create();
                //и новую корзину
                cart = new Cart(order.Id);
            }
            return (order, cart);
        }

        public IActionResult RemoveItem(int bookid)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.RemoveItem(bookid);

            SaveOrderAndCart(order, cart);
            
            return RedirectToAction("Index", "Order");
        }
    }
}
