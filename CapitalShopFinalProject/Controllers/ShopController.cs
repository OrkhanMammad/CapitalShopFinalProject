using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.ShopVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CapitalShopFinalProject.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> Products = await _context.Products.Where(p => p.IsDeleted == false && p.Count>0).ToListAsync();
            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            IEnumerable<ProductType> ProductTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();

            ViewBag.categoryid = 0;
            ViewBag.productTypeId = 0;
            ViewBag.pageIndex = 1;
            ViewBag.pageCount = (int)Math.Ceiling((decimal)Products.Count() / 9);
            Products = Products.Skip(0).Take(9);

            ShopVM shopVM = new ShopVM
            {
                Products = Products,
                Categories = Categories,
                ProductTypes = ProductTypes
            };

           

            return View(shopVM);
        }

        public async Task<IActionResult> getFilteredProducts(int categoryid = 0, int productTypeId = 0, int pageIndex=1, string? priceRange = "" )
        {
            IEnumerable<Product> Products = await _context.Products.Where(p => p.IsDeleted == false && p.Count>0).ToListAsync();
            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            IEnumerable<ProductType> ProductTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();

            ViewBag.categoryid = categoryid;
            ViewBag.productTypeId = productTypeId;
            ViewBag.priceRange = priceRange;
            ViewBag.pageIndex=pageIndex;


            priceRange = priceRange?.Replace("$", "");
            double minValue = 20;
            double maxValue = 400;


            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                string[] arr = priceRange.Split("-");
                minValue = double.Parse(arr[0]);
                maxValue = double.Parse(arr[1]);
                Products = Products.Where(p => p.DiscountedPrice > minValue && p.DiscountedPrice < maxValue).ToList();
                
            }

            ViewBag.minPrice = minValue;
            ViewBag.maxPrice = maxValue;



            if (!Categories.Any(c => c.ID == categoryid))
            {
                if (categoryid != 0)
                {
                    return NotFound();
                }
            }

            if (Categories.Any(c => c.ID == categoryid))
            {
                Products = Products.Where(p => p.CategoryId == categoryid).ToList();
            }


            if (!ProductTypes.Any(p => p.ID == productTypeId))
            {
                if (productTypeId != 0)
                {
                    return NotFound();
                }
            }

            if (ProductTypes.Any(p => p.ID == productTypeId))
            {
                Products = Products.Where(p => p.ProductTypeId == productTypeId).ToList();
            }
            ViewBag.pageCount = (int)Math.Ceiling((decimal)Products.Count() / 9);
            Products = Products.Skip((pageIndex - 1) * 9).Take(9);





            ShopVM shopVM = new ShopVM
            {
                Products = Products,
                Categories = Categories,
                ProductTypes = ProductTypes
            };

            

            return PartialView("_ShopProductPartialView", shopVM);


        }


        


    }
}
