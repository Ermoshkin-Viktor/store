

using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
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

        //добавляем книгу к существующему заказу
        public IActionResult AddItem(int id)
        {
            Order order;
            Cart cart;
            //Если корзина с таким id есть
            if (HttpContext.Session.TryGetCart(out cart))
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
            //загружаем книгу из репозитория
            var book = bookRepository.GetById(id);
            order.AddItem(book, 1); 
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice= order.TotalPrice;
            //сохраняем сессию
            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Book", new { id });
        }
    }
}
