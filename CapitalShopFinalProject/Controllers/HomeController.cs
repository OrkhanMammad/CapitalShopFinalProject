using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.HomeVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;

namespace CapitalShopFinalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context= context;
        }


        public async Task<IActionResult>  Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Testimonials = await _context.Testimonials.Where(t=>t.IsDeleted==false).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync(),
                Products=await _context.Products.Where(p=>p.IsDeleted==false).Include(p=>p.ProductImages.Where(pi=>pi.IsDeleted==false)).ToListAsync(),
                News=await _context.News.Where(n=>n.IsDeleted==false).ToListAsync()
            };



            return View(homeVM);
        }

        public async Task<IActionResult> getTrendingProducts(int categoryId)
        {
            IEnumerable<Product> Products=await _context.Products.Where(p=>p.IsDeleted==false && p.IsTrending==true && p.CategoryId==categoryId).ToListAsync();
            
            

            HomeVM homeVM = new HomeVM
            {
                Products=Products
               
                
            };

            return PartialView("_TrendingPartialView", homeVM);

        }

    }
}
