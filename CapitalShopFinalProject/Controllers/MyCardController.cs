using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Controllers
{
    public class MyCardController : Controller
    {
        private readonly AppDbContext _context;
        public MyCardController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string Basket = HttpContext.Request.Cookies["basket"];
            if (string.IsNullOrEmpty(Basket))
            {
                List<BasketVM> BasketList=new List<BasketVM>();
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



            return PartialView("_MyCardPartialView", BasketList);
        }
        public IActionResult CardDecreaseCount(int? cardItemId)
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



            return PartialView("_MyCardPartialView", BasketList);
        }

        public IActionResult CardDelete(int? cardItemId)
        {
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



            return PartialView("_MyCardPartialView", BasketList);
        }


    }
}
