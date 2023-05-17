using Microsoft.AspNetCore.Mvc;

namespace CapitalShopFinalProject.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
