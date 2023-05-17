using Microsoft.AspNetCore.Mvc;

namespace CapitalShopFinalProject.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
