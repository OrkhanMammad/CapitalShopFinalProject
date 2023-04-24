using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapitalShopFinalProject.Controllers
{
    public class DetailController : Controller
    {
        AppDbContext _context;
        public DetailController(AppDbContext context)
        {
            _context=context;
        }

        public async Task<IActionResult>  Index(int? productId)
        {
            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();
            Product product= products.FirstOrDefault(p=>p.ID==productId);

            return View(product);
        }
    }
}
