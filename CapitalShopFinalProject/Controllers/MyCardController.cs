using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Controllers
{
    public class MyCardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public MyCardController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult>  Index()
        {
            string Basket = HttpContext.Request.Cookies["basket"];


            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(Basket))
            {
                AppUser appuser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                IEnumerable<Basket> BasketsDBs = appuser.Baskets.ToList();
                List<BasketVM> BasketList = new List<BasketVM>();
                ViewBag.ProductsTotal = 0;

                foreach (Basket basket in BasketsDBs)
                {
                    BasketVM basketVM = new BasketVM
                    {
                        Id = basket.ProductId,
                        Image = basket.Image,
                        Title = basket.Title,
                        DiscountedPrice = basket.DiscountedPrice,
                        Count = basket.Count,
                    };

                    ViewBag.ProductsTotal += basket.Count * basket.DiscountedPrice;
                    BasketList.Add(basketVM);
                }


                string srzdProducts = JsonConvert.SerializeObject(BasketList);
                HttpContext.Response.Cookies.Append("basket", srzdProducts);
                return View(BasketList);
            }

            else
            {
                if (string.IsNullOrEmpty(Basket))
                {
                    List<BasketVM> BasketList = new List<BasketVM>();
                    return View(BasketList);
                }
                else 
                {
                    List<BasketVM> BasketList = JsonConvert.DeserializeObject<List<BasketVM>>(Basket);
                    ViewBag.ProductsTotal = 0;
                    foreach (BasketVM basketVM in BasketList)
                    {
                        ViewBag.ProductsTotal += basketVM.Count * basketVM.DiscountedPrice;
                    }


                    return View(BasketList);

                }

            }



           
            
        }
        public async Task<IActionResult> CardIncreaseCount(int? cardItemId)
        {
           IEnumerable<Product>Products=await _context.Products.Where(p=>p.IsDeleted==false).ToListAsync();
            Product product= Products.FirstOrDefault(p=>p.ID==cardItemId);

            string Basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> BasketList = JsonConvert.DeserializeObject<List<BasketVM>>(Basket);
            ViewBag.ProductsTotal = 0;
            if (BasketList.Any(bl=>bl.Id== cardItemId))
            {
                if(product.Count > BasketList.Find(bl => bl.Id == cardItemId).Count)
                {
                    BasketList.Find(bl => bl.Id == cardItemId).Count += 1;
                }              
                foreach (BasketVM basketVM in BasketList)
                {
                    ViewBag.ProductsTotal += basketVM.Count * basketVM.DiscountedPrice;
                }
                string srzdProducts = JsonConvert.SerializeObject(BasketList);
                HttpContext.Response.Cookies.Append("basket", srzdProducts);
            }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appuser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                IEnumerable<Basket> BasketsDB = await _context.Baskets.ToListAsync();

                if (product.Count > BasketsDB.FirstOrDefault(b => b.ProductId == cardItemId && b.UserId == appuser.Id).Count)
                {
                    BasketsDB.FirstOrDefault(b => b.ProductId == cardItemId && b.UserId == appuser.Id).Count += 1;
                }


                await _context.SaveChangesAsync();
            }



            return PartialView("_MyCardPartialView", BasketList);
        }
        public async Task<IActionResult>  CardDecreaseCount(int? cardItemId)
        {

            string Basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> BasketList = JsonConvert.DeserializeObject<List<BasketVM>>(Basket);
            ViewBag.ProductsTotal = 0;
            if (BasketList.Any(bl => bl.Id == cardItemId))
            {
                if(BasketList.Find(bl => bl.Id == cardItemId).Count > 1)
                {
                    BasketList.Find(bl => bl.Id == cardItemId).Count -= 1;
                }
                
                foreach (BasketVM basketVM in BasketList)
                {
                    ViewBag.ProductsTotal += basketVM.Count * basketVM.DiscountedPrice;
                }
                string srzdProducts = JsonConvert.SerializeObject(BasketList);
                HttpContext.Response.Cookies.Append("basket", srzdProducts);
            }


            if (User.Identity.IsAuthenticated)
            {
                AppUser appuser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                IEnumerable<Basket> BasketsDB = await _context.Baskets.ToListAsync();
                if(BasketsDB.FirstOrDefault(b => b.ProductId == cardItemId && b.UserId == appuser.Id).Count > 1)
                {
                    BasketsDB.FirstOrDefault(b => b.ProductId == cardItemId && b.UserId == appuser.Id).Count -= 1;
                }
                
                await _context.SaveChangesAsync();
            }



            return PartialView("_MyCardPartialView", BasketList);
        }

        public async Task<IActionResult>  CardDelete(int? cardItemId)
        {
            if(cardItemId == null)
            {
                return BadRequest();
            }


            string Basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> BasketList = JsonConvert.DeserializeObject<List<BasketVM>>(Basket);
            ViewBag.ProductsTotal = 0;
            if (BasketList.Any(bl => bl.Id == cardItemId))
            {
               BasketVM deletingCardItem=BasketList.FirstOrDefault(bl => bl.Id == cardItemId);
                BasketList.Remove(deletingCardItem);
                
                foreach (BasketVM basketVM in BasketList)
                {
                    ViewBag.ProductsTotal += basketVM.Count * basketVM.DiscountedPrice;
                }
                string srzdProducts = JsonConvert.SerializeObject(BasketList);
                HttpContext.Response.Cookies.Append("basket", srzdProducts);
            }


            if (User.Identity.IsAuthenticated)
            {
                AppUser appuser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                IEnumerable<Basket> BasketsDB = await _context.Baskets.ToListAsync();
                Basket deletingBasketdb = BasketsDB.FirstOrDefault(b => b.ProductId == cardItemId && b.UserId==appuser.Id);


                    _context.Baskets.Remove(deletingBasketdb);

                    await _context.SaveChangesAsync();
            }



            return PartialView("_MyCardPartialView", BasketList);
        }


    }
}
