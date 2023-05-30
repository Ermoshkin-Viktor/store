using Microsoft.AspNetCore.Mvc;

namespace Store.YandexKassa.Areas.YandexKassa.Controllers
{
    public class HomeController : Controller
    {
        [Area("YandexKassa")]
        public IActionResult Index()
        {
            return View();
        }
        [Area("YandexKassa")]
        // /YandexKassa/Home/Callback
        public IActionResult Callback()
        {
            return View();
        }
    }
}
