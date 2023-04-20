

using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
namespace Store.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;

        public CartController(IBookRepository bookRepository, IOrderRepository orderRepository)
        {
            this.bookRepository = bookRepository;
            this.orderRepository = orderRepository;
        }
        //добавляем книгу в корзину
        public IActionResult Add(int id)
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
