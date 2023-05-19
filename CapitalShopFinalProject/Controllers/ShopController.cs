using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.ProductReviewVM;
using CapitalShopFinalProject.ViewModels.ShopVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CapitalShopFinalProject.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ShopController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

       

        public async Task<IActionResult> Index(int? categoryid)
        {
            IEnumerable<Product> Products = await _context.Products.Where(p => p.IsDeleted == false && p.Count>0).ToListAsync();
            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            IEnumerable<ProductType> ProductTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();
            if (categoryid != null)
            {
                Products = Products.Where(p => p.CategoryId == categoryid);
            }

            ViewBag.categoryid = categoryid;
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

        [HttpGet]
        public async Task<IActionResult> Detail(int? productId)
        {
            if (productId == null)
            {
                return BadRequest();
            }
            Product product = await _context.Products.Include(p => p.ProductImages.Where(p => p.IsDeleted == false)).Include(p => p.Reviews.Where(p => p.IsDeleted == false)).FirstOrDefaultAsync(p => p.IsDeleted == false && p.ID == productId);

            if (product == null) { return NotFound(); }

            ProductReviewVM productReviewVM = new ProductReviewVM
            {
                Product = product,
                Review = new Review { ProductId = productId },

            };



            return View(productReviewVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Review review)
        {
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(p => p.IsDeleted == false))
                .Include(p => p.Reviews.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.ID == review.ProductId);

            review.IsDeleted = false;

            ProductReviewVM productReviewVM = new ProductReviewVM { Product = product, Review = review };

            if (!ModelState.IsValid)
            {
                return View("Detail", productReviewVM);
            }
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (product.Reviews != null && product.Reviews.Count() > 0 && product.Reviews.Any(r => r.AppUserId == appUser.Id))
            {
                ModelState.AddModelError("Name", "Siz artiq fikir bildirmisiniz");
                return View("Detail", productReviewVM);

            }

            review.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            review.CreatedAt = DateTime.UtcNow.AddHours(4);
            review.AppUserId = appUser.Id;

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            ViewBag.showReview=true;

            return RedirectToAction("Detail", "Shop", new { ProductId = product.ID });

        }



    }
}
