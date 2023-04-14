

using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
namespace Store.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IBookRepository bookRepository;

        public CartController(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }
        //добавляем книгу в корзину
        public IActionResult Add(int id)
        {
            var book = bookRepository.GetById(id);
            Cart cart;
            //Если корзины с таким id нет
            if (!HttpContext.Session.TryGetCart(out cart))
                cart = new Cart();//создаем новую пустую корзину
            //Если книга с таким id уже есть в корзине
            if (cart.Items.ContainsKey(id))
                //то увеличиваем к-во на единицу
                cart.Items[id]++;
            else
                //иначе к-во книг = 1
                cart.Items[id] = 1;
            //и стоимость корзины увеличиваем на цену книги
            cart.Amount += book.Price;
            //сохраняем сессию
            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Book", new { id });
        }
    }
}
