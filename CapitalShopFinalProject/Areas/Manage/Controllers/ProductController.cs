using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CapitalShopFinalProject.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        public async Task<IActionResult>  Index(int pageIndex=1)
        {
            IEnumerable<Product> Products = await _context.Products
                .Include(p => p.ProductImages
                .Where(pi => pi.IsDeleted == false)).Where(p => p.IsDeleted == false).ToListAsync();

            ViewBag.pageCount = (int) Math.Ceiling((decimal)Products.Count() / 15) ;

            Products = Products.Skip((pageIndex - 1) * 15).Take(15).ToList();

            ViewBag.pageIndex = pageIndex;
            

            return View(Products);
        }


        public async Task<IActionResult> DeleteDetail(int? productId)
        {
            if (productId == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false)).FirstOrDefaultAsync(p => p.IsDeleted == false);
            if (product == null)
            {
                return NotFound();
            }
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.ID == product.CategoryId);
            ProductType productType = await _context.ProductTypes.FirstOrDefaultAsync(p => p.ID == product.ProductTypeId);
            ViewBag.Category=category.Name;
            ViewBag.ProductType = productType.Name;
            return View(product);



        }

        public async Task<IActionResult> Detail(int? productId)
        {
            if(productId == null)
            {
                return BadRequest();
            }


            Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false)).FirstOrDefaultAsync(p => p.IsDeleted == false);
            if (product == null)
            {
                return NotFound();
            }
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.ID == product.CategoryId);
            ProductType productType = await _context.ProductTypes.FirstOrDefaultAsync(p => p.ID == product.ProductTypeId);
            ViewBag.Category = category.Name;
            ViewBag.ProductType = productType.Name;
            return View(product);

           

        }

        public async Task<IActionResult> Delete(int? productId)
        {
            if(productId == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.FirstOrDefault(p =>p.ID==productId).IsDeleted=true;

            await _context.SaveChangesAsync();

            return RedirectToAction("index");

        }

    }
}
