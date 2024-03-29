﻿using Microsoft.AspNetCore.Mvc;
using Store.Web.App;
using System;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index(string query)
        {
            var books = await bookService.GetAllByQueryAsync(query);
            return View("Index", books);
        }
    }
}
