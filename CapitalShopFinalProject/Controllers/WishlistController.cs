using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using CapitalShopFinalProject.ViewModels.WishlistVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;
        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string Wish = HttpContext.Request.Cookies["wishlist"];
            if (string.IsNullOrEmpty(Wish))
            {
                List<WishlistVM> Wishlist = new List<WishlistVM>();
                return View(Wishlist);
            }
            else
            {
                List<WishlistVM> Wishlist = JsonConvert.DeserializeObject<List<WishlistVM>>(Wish);



                return View(Wishlist);

            }






        }

        public async Task<IActionResult> addToWishlist(int? productId)
        {
            if (productId == null)
            {
                return BadRequest();
            }

            Product newWishItem = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.ID == productId);

            if (newWishItem == null)
            {
                return NotFound();
            }


            else
            {
                string Wish = HttpContext.Request.Cookies["wishlist"];
                if (string.IsNullOrEmpty(Wish))
                {
                    List<WishlistVM> WishList = new List<WishlistVM>();

                    WishlistVM wishlistVM = new WishlistVM
                    {
                        Id = newWishItem.ID,
                        Image = newWishItem.MainImage,
                        Title = newWishItem.Title,
                        DiscountedPrice = newWishItem.DiscountedPrice,
                        
                    };
                    WishList.Add(wishlistVM);

                    string srzdProducts = JsonConvert.SerializeObject(WishList);
                    HttpContext.Response.Cookies.Append("wishlist", srzdProducts);
                    return Ok();

                }
                else
                {
                    List<WishlistVM> WishList = JsonConvert.DeserializeObject<List<WishlistVM>>(Wish);

                    if (WishList.Any(bl => bl.Id == productId))
                    {
                        return Ok();
                    }

                    else
                    {
                        WishlistVM wishlistVM = new WishlistVM
                        {
                            Id = newWishItem.ID,
                            Image = newWishItem.MainImage,
                            Title = newWishItem.Title,
                            DiscountedPrice = newWishItem.DiscountedPrice,

                        };
                        WishList.Add(wishlistVM);

                    }



                    string srzdProducts = JsonConvert.SerializeObject(WishList);
                    HttpContext.Response.Cookies.Append("wishlist", srzdProducts);
                    return Ok();
                }

            }



        }

        

        public IActionResult RemoveFromWishlist(int? wishItemId)
        {
            string Wish = HttpContext.Request.Cookies["wishlist"];
            List<WishlistVM> Wishlist = JsonConvert.DeserializeObject<List<WishlistVM>>(Wish);
           
            if (Wishlist.Any(bl => bl.Id == wishItemId))
            {
                WishlistVM deletingWishItem = Wishlist.FirstOrDefault(bl => bl.Id == wishItemId);
                Wishlist.Remove(deletingWishItem);

                
                string srzdProducts = JsonConvert.SerializeObject(Wishlist);
                HttpContext.Response.Cookies.Append("wishlist", srzdProducts);
            }



            return PartialView("_WishlistPartialView", Wishlist);
        }


    }
}
