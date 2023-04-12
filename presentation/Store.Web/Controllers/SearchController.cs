using Microsoft.AspNetCore.Mvc;
using System;
namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookService bookService;
        
        //Внедрение зависимостей через конструктор
        public SearchController(BookService bookService)
        {
            this.bookService = bookService;
        }

        public IActionResult Index(string query)
        {
            var books = bookService.GetAllByQuery(query) ;
            return View("Index", books);
        }
    }
}
