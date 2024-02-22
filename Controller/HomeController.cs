using Microsoft.AspNetCore.Mvc;

namespace yohye
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
