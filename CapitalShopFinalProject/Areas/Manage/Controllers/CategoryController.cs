
using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.OrderVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace CapitalShopFinalProject.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context= context;
            _userManager= userManager;
            _env= env;
        }


        [HttpGet]
        public async Task<IActionResult>  Index()
        {
            IEnumerable<Category> Categories= await _context.Categories.Where(c=>c.IsDeleted==false).ToListAsync();
            return View(Categories);
        }

        [HttpGet]
        
        public async Task<IActionResult> Create()
        {
            Category category = new Category();
            return View(category);
        }

        [HttpPost]

        public async Task<IActionResult> Create(Category category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("", "Category Name Can Not Be Empty");
                return View(category);
            }

            if(category.File?.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("File", "Image type is not acceptable");
                return View(category);
            }

            if((category.File?.Length/1024) > 300)
            {
                ModelState.AddModelError("File", "Maximum 300KB is allowed for image");
                return View(category);
            }

       

            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            if(Categories.Any(c=>c.Name.ToString().Trim().ToLower()==category.Name.ToString().Trim().ToLower())) 
            {
                ModelState.AddModelError("", "You have this category already");
                return View(category);
            
            }

            AppUser appUser=await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (category.File != null)
            {

                string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-{Guid.NewGuid().ToString()}-{category.File.FileName}";
                string filePath = Path.Combine(_env.WebRootPath, "assets", "Images", "home", "category-banner", fileName);
                category.Image = fileName;

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await category.File.CopyToAsync(stream);
                }
            }
            category.CreatedAt= DateTime.UtcNow.AddHours(4);
            category.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            category.Name = category.Name.ToString().Trim();
            category.IsDeleted=false;

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return View();

        }

        public async Task<IActionResult> DeleteDetail(int? categoryId)
        {
            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            if(categoryId== null)
            {
                return BadRequest();
            }
            if (!Categories.Any(c => c.ID == categoryId))
            {
                return NotFound();
            }

            Category category=Categories.FirstOrDefault(c=>c.ID==categoryId);

            
            return View(category);

        }

        public async Task<IActionResult> Delete(int? categoryId)
        {
            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            if (categoryId == null)
            {
                return BadRequest();
            }
            if (!Categories.Any(c => c.ID == categoryId))
            {
                return NotFound();
            }

             _context.Categories.FirstOrDefault(c=>c.ID==categoryId).IsDeleted = true;

            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();
            IEnumerable<Basket> basketsDB = await _context.Baskets.Where(b => b.IsDeleted == false).ToListAsync();
           if(products.Any(p => p.CategoryId == categoryId))
            {
                foreach(Product product in products)
                {
                    if(product.CategoryId == categoryId)
                    {
                        product.IsDeleted = true;
                        foreach(Basket basket in basketsDB)
                        {
                            if (basket.ProductId == product.ID)
                            {
                                _context.Baskets.Remove(basket);
                            }
                        }
                        
                    }
                    
                }

            }


            await _context.SaveChangesAsync();

            return RedirectToAction("index");

        }

        public async Task<IActionResult> Detail(int? categoryId)
        {
            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            if (categoryId == null)
            {
                return BadRequest();
            }
            if (!Categories.Any(c => c.ID == categoryId))
            {
                return NotFound();
            }

            Category category = Categories.FirstOrDefault(c => c.ID == categoryId);


            return View(category);


        }

        [HttpGet]
        public async Task<IActionResult> Update(int? categoryId)
        {
            if(categoryId==null)
            {
                return BadRequest();
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.ID == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);

            




        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {

            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("", "Category Name Can Not Be Empty");
                return View(category);
            }

            if (category.File?.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("File", "Image type is not acceptable");
                return View(category);
            }

            if ((category.File?.Length / 1024) > 300)
            {
                ModelState.AddModelError("File", "Maximum 300KB is allowed for image");
                return View(category);
            }

            Category changedCategory = await _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefaultAsync(c=>c.ID==category.ID);

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (category.File != null)
            {

                string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-{Guid.NewGuid().ToString()}-{category.File.FileName}";
                string filePath = Path.Combine(_env.WebRootPath, "assets", "Images", "home", "category-banner", fileName);
                changedCategory.Image = fileName;

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await category.File.CopyToAsync(stream);
                }
            }
            changedCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            changedCategory.UpdatedBy = $"{appUser.Name} {appUser.SurName}";
            changedCategory.Name = category.Name.ToString().Trim();
            changedCategory.IsDeleted = false;

            await _context.SaveChangesAsync();



            changedCategory.Name=category.Name;
            changedCategory.Image=category.Image;


            return View();




        }




    }
}
