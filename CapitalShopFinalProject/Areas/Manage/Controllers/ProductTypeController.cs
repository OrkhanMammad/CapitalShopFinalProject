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
    public class ProductTypeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;


        public ProductTypeController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }


        public async Task<IActionResult>  Index()
        {
            IEnumerable<ProductType> productTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();

            return View(productTypes);
        }

        public async Task<IActionResult> Create()
        {
            ProductType productType = new ProductType();

            return View(productType);

        }

        [HttpPost]

        public async Task<IActionResult> Create(ProductType productType)
        {
            if (string.IsNullOrEmpty(productType.Name))
            {
                ModelState.AddModelError("", " Product Type Can Not Be Empty");
                return View(productType);
            }

            IEnumerable<ProductType> productTypes = await _context.ProductTypes.Where(c => c.IsDeleted == false).ToListAsync();
            if (productTypes.Any(c => c.Name.ToString().Trim().ToLower() == productType.Name.ToString().Trim().ToLower()))
            {
                ModelState.AddModelError("", "You have this category already");
                return View(productType);

            }

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            productType.CreatedAt = DateTime.UtcNow.AddHours(4);
            productType.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            productType.Name = productType.Name.ToString().Trim();
            productType.IsDeleted = false;

            await _context.ProductTypes.AddAsync(productType);
            await _context.SaveChangesAsync();





            return View();

        }

        public async Task<IActionResult> DeleteDetail(int? productTypeId)
        {
            IEnumerable<ProductType> productTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();

            if (productTypeId == null)
            {
                return BadRequest();
            }
            if (!productTypes.Any(pt => pt.ID == productTypeId))
            {
                return NotFound();
            }

            ProductType productType = productTypes.FirstOrDefault(pt => pt.ID == productTypeId);


            return View(productType);

        }


        public async Task<IActionResult> Delete(int? productTypeId)
        {
            IEnumerable<ProductType> productTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();

            if (productTypeId == null)
            {
                return BadRequest();
            }
            if (!productTypes.Any(c => c.ID == productTypeId))
            {
                return NotFound();
            }

            _context.ProductTypes.FirstOrDefault(pt => pt.ID == productTypeId).IsDeleted = true;

            IEnumerable<Product> Products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();
            if (Products.Any(p => p.ProductTypeId == productTypeId))
            {
                foreach(Product product in Products)
                {
                    if (product.ProductTypeId == productTypeId)
                    {
                        product.IsDeleted = true;
                    }
                }

            }


            await _context.SaveChangesAsync();

            return RedirectToAction("index");

        }

        public async Task<IActionResult> Detail(int? productTypeId)
        {
            IEnumerable<ProductType> productTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();

            if (productTypeId == null)
            {
                return BadRequest();
            }
            if (!productTypes.Any(c => c.ID == productTypeId))
            {
                return NotFound();
            }

            ProductType productType = productTypes.FirstOrDefault(c => c.ID == productTypeId);


            return View(productType);


        }



        [HttpGet]
        public async Task<IActionResult> Update(int? productTypeId)
        {
            if (productTypeId == null)
            {
                return BadRequest();
            }

            ProductType productType = await _context.ProductTypes.FirstOrDefaultAsync(pt => pt.ID == productTypeId);

            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);






        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductType productType)
        {

            if (string.IsNullOrEmpty(productType.Name))
            {
                ModelState.AddModelError("", "Product Type Name Can Not Be Empty");
                return View(productType);
            }


            ProductType changedProductType = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).FirstOrDefaultAsync(pt => pt.ID == productType.ID);

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
          
            changedProductType.UpdatedAt = DateTime.UtcNow.AddHours(4);
            changedProductType.UpdatedBy = $"{appUser.Name} {appUser.SurName}";
            changedProductType.Name = productType.Name.ToString().Trim();
            changedProductType.IsDeleted = false;

            await _context.SaveChangesAsync();

            return View();




        }


    }
}
