using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using CapitalShopFinalProject.ViewModels.OrderVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace CapitalShopFinalProject.Controllers
{
    public class OrderController : Controller
    {
      private readonly  AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;
        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context= context;
            _userManager= userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            string basket = HttpContext.Request.Cookies["basket"];
            if (string.IsNullOrWhiteSpace(basket))
            {
                return RedirectToAction("index", "shop");
            }

            List<BasketVM> basketVMs= JsonConvert.DeserializeObject<List<BasketVM>>(basket);

            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsMain && a.IsDeleted == false)).FirstOrDefaultAsync(u=>u.UserName==User.Identity.Name);

            if (appUser == null)
            {
                return BadRequest();
            }
            Address address=appUser.Addresses.FirstOrDefault();
            // fikir ver
            if (address == null)
            {
                return RedirectToAction("profile", "account");
            }

            foreach(BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.ID == basketVM.Id);
                basketVM.DiscountedPrice = product.DiscountedPrice;
                basketVM.Title= product.Title;
            }

            Order order = new Order
            { 
                Name=appUser?.Name,
                Surname=appUser?.SurName,
                Email=appUser?.Email,
                AddressLine=address?.AddressLIne,
                City=address?.City,
                PostalCode=address?.PostalCode,
                State=address?.State,
                Country=address?.Country,

            };

            OrderVM orderVM = new OrderVM
            { 
                Order=order,
                BasketVMs=basketVMs,
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Checkout( Order order)
        {
            string basket = HttpContext.Request.Cookies["basket"];
            if (string.IsNullOrWhiteSpace(basket))
            {
                return RedirectToAction("index", "shop");
            }

            AppUser appUser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .Include(u => u.Orders)
                .Include(u => u.Addresses.Where(a => a.IsMain && a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);



            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.ID == basketVM.Id);
                basketVM.DiscountedPrice = product.DiscountedPrice;
                basketVM.Title = product.Title;
            }

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs,
            };

            if(!ModelState.IsValid) 
            { 
                return View(order);
            
            }

            List<OrderItem> orderItems= new List<OrderItem>();

            foreach(BasketVM basketVm in basketVMs)
            {
                OrderItem orderItem = new OrderItem
                {
                    ProductId = basketVm.Id,
                    Count = basketVm.Count,
                    Price = basketVm.DiscountedPrice,
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}"
                };
                orderItems.Add(orderItem);


            }

            foreach(Basket basket1 in appUser.Baskets)
            {

                basket1.IsDeleted = true;

            }

            HttpContext.Response.Cookies.Append("basket", "");
            order.UserId= appUser.Id;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.OrderItems= orderItems;
            order.No = appUser.Orders != null && appUser.Orders.Count() > 0 ? appUser.Orders.Last().No + 1 : 1;
            

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "home");



        }




    }
}
