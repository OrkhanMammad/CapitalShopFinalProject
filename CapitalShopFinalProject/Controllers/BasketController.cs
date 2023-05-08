using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;

namespace CapitalShopFinalProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? productId)
        {
            if(productId == null)
            {
                return BadRequest();
            }

            Product newBasketItem= await _context.Products.FirstOrDefaultAsync(p=>p.IsDeleted==false && p.ID==productId);

            if (newBasketItem == null)
            {
                return NotFound();
            }

            else
            {
                string Basket = HttpContext.Request.Cookies["basket"];
                if (string.IsNullOrEmpty(Basket))
                {
                    List<BasketVM> BasketList=new List<BasketVM>();

                    

                    BasketVM basketVM = new BasketVM
                    {
                        Id=newBasketItem.ID,
                        Image=newBasketItem.MainImage,
                        Title=newBasketItem.Title,
                        DiscountedPrice=newBasketItem.DiscountedPrice,
                        Count=1,                        
                    };
                    BasketList.Add(basketVM);

                    string srzdProducts = JsonConvert.SerializeObject(BasketList);
                    HttpContext.Response.Cookies.Append("basket", srzdProducts);
                    return Ok();

                }
                else
                {
                    List<BasketVM> BasketList = JsonConvert.DeserializeObject<List<BasketVM>>(Basket);

                    if(BasketList.Any(bl=>bl.Id==productId))
                    {
                        if(BasketList.Find(bl => bl.Id == productId).Count < newBasketItem.Count)
                        {
                            BasketList.Find(bl => bl.Id == productId).Count += 1;
                        }
                        
                    }

                    else 
                    {
                        BasketVM basketVM = new BasketVM
                        {
                            Id = newBasketItem.ID,
                            Image = newBasketItem.MainImage,
                            Title = newBasketItem.Title,
                            DiscountedPrice = newBasketItem.DiscountedPrice,
                            Count = 1,
                            
                        };
                        BasketList.Add(basketVM);

                    }

                    

                    string srzdProducts = JsonConvert.SerializeObject(BasketList);
                    HttpContext.Response.Cookies.Append("basket", srzdProducts);
                    return Ok();
                }

            }
            
        }



        

    }
}
