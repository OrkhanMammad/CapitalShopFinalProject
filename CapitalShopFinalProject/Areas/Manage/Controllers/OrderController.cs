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
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public OrderController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? orderid)
        {
            Order order = await _context.Orders.Include(o=>o.OrderItems).FirstOrDefaultAsync(o => o.ID == orderid);

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateStatus(Order order)
        {

            Order Order = await _context.Orders.FirstOrDefaultAsync(o => o.ID == order.ID);
            if (Order == null) { return NotFound(); }
            Order.Status = order.Status;
            Order.Comment= order.Comment;
            await _context.SaveChangesAsync();
            return RedirectToAction("index", "dashboard", new { area = "manage" });


        }
    }
}
