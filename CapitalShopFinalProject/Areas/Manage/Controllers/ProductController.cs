using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Extension;
using CapitalShopFinalProject.Helper;
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

            Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false)).FirstOrDefaultAsync(p => p.ID == productId);
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


            Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false)).FirstOrDefaultAsync(p => p.ID == productId);
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

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.ID == productId);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.FirstOrDefault(p =>p.ID==productId).IsDeleted=true;

            await _context.SaveChangesAsync();

            return RedirectToAction("index");

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.ProductTypes=await _context.ProductTypes.Where(pt=>pt.IsDeleted==false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();


            return View(new Product());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.ProductTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            if (product.Title == null)
            {
                ModelState.AddModelError("Title", "Title Must Be Filled");
                return View(product);

            }

            if (product.Price == null || product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be more than 0");
                return View(product);

            }

            if (product.DiscountedPrice == null || product.DiscountedPrice <= 0)
            {
                ModelState.AddModelError("DiscountedPrice", "Discounted Price must be more than 0");
                return View(product);

            }

            if (product.Count == null || product.Count < 1)
            {
                ModelState.AddModelError("Count", "Count must be more than 0");
                return View(product);

            }

            if (product.MainFile == null)
            {
                ModelState.AddModelError("MainFile", "Main Image Must Be Selected");
                return View(product);
            }
            if (product.MainFile?.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("MainFile", "Image type is not acceptable");
                return View(product);
            }

            if ((product.MainFile?.Length / 1024) > 300)
            {
                ModelState.AddModelError("MainFile", "Maximum 300KB is allowed for image");
                return View(product);
            }

            if (product.ProductTypeId == null)
            {
                ModelState.AddModelError("ProductTypeId", "Product Type Must Be Filled");
                return View(product);

            }
            ProductType productType = await _context.ProductTypes.FirstOrDefaultAsync(pt => pt.IsDeleted == false && pt.ID==product.ProductTypeId);
            if (productType == null)
            {
                ModelState.AddModelError("ProductTypeId", "Product Type Doesnt Exist");

                return View(product);
            }


            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Category Must Be Filled");

                return View(product);

            }
            Category category = await _context.Categories.FirstOrDefaultAsync(pt => pt.IsDeleted == false && pt.ID == product.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Category Doesnt Exist");
                return View(product);
            }

            if (string.IsNullOrEmpty(product.Description))
            {
                ModelState.AddModelError("Description", "Description Must Be Filled");
                return View(product);

            }


            string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-{Guid.NewGuid().ToString()}-{product.MainFile.FileName}";
            string filePath = Path.Combine(_env.WebRootPath, "assets", "Images", "products", fileName);
            product.MainImage = fileName;

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await product.MainFile.CopyToAsync(stream);
            }

            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.CreatedBy = User.Identity.Name;
            product.IsDeleted = false;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();


            return RedirectToAction("index", "product", new {area="manage"});
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? productId)
        {
            if (productId == null) return BadRequest();

            ViewBag.ProductTypes = await _context.ProductTypes
                .Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(b => b.IsDeleted == false).ToListAsync();

            Product product = await _context.Products
            .FirstOrDefaultAsync(p => p.ID == productId && p.IsDeleted == false);
            if (product == null) { return NotFound(); }

            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            ViewBag.ProductTypes = await _context.ProductTypes.Where(pt => pt.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            Product updatingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ID == product.ID);


            if (product.Title != null)
            {
                updatingProduct.Title=product.Title;
            }

            if (product.Price != null && product.Price > 0)
            {
               updatingProduct.Price = product.Price;

            }

            if (product.DiscountedPrice != null && product.DiscountedPrice > 0)
            {
               updatingProduct.DiscountedPrice = product.DiscountedPrice;

            }

            if (product.Count != null && product.Count > 1)
            {
                updatingProduct.Count = product.Count;

            }

            if (product.MainFile != null)
            {
                if (product.MainFile?.ContentType == "image/jpeg")
                {
                    if ((product.MainFile?.Length / 1024) < 300)
                    {
                        string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-{Guid.NewGuid().ToString()}-{product.MainFile.FileName}";
                        string filePath = Path.Combine(_env.WebRootPath, "assets", "Images", "products", fileName);
                        updatingProduct.MainImage = fileName;
                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.MainFile.CopyToAsync(stream);
                        }
                    }

                }
            }
        
            if (product.ProductTypeId != null)
            {
                ProductType productType = await _context.ProductTypes.FirstOrDefaultAsync(pt => pt.IsDeleted == false && pt.ID == product.ProductTypeId);
                if (productType != null)
                {
                    updatingProduct.ProductTypeId = product.ProductTypeId;
                }
            }
           


            if (product.CategoryId != null)
            {
                Category category = await _context.Categories.FirstOrDefaultAsync(pt => pt.IsDeleted == false && pt.ID == product.CategoryId);
                if (category != null)
                {
                    updatingProduct.CategoryId = product.CategoryId;
                }

            }
            

            if (!string.IsNullOrEmpty(product.Description))
            {
                updatingProduct.Description= product.Description;

            }

            updatingProduct.UpdatedAt = DateTime.UtcNow.AddHours(4);
            updatingProduct.UpdatedBy = User.Identity.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction("index", "product", new {area="manage"});
        }


    }



 }
