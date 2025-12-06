using Microsoft.AspNetCore.Mvc;

namespace StudentRegistrationApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
