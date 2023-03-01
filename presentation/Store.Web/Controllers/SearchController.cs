using Microsoft.AspNetCore.Mvc;
using System;
namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IBookRepository bookrepository;
        //Внедрение зависимостей через конструктор
        public SearchController(IBookRepository bookrepository)
        {
            this.bookrepository = bookrepository ;
        }

        public IActionResult Index(string query)
        {
            var books = bookrepository.GetAllByTitle(query) ;
            return View(books);
        }
    }
}
