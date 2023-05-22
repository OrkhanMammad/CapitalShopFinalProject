using CapitalShopFinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapitalShopFinalProject.DataAccessLayer;
namespace CapitalShopFinalProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        

        public DashboardController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        
        public async Task<IActionResult>  Index()
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (appUser == null)
            {
                return BadRequest();
            }

            IEnumerable<Order> Orders = await _context.Orders.Include(o=>o.OrderItems).OrderByDescending(o=>o.CreatedAt).ToListAsync();

 



            return View(Orders);
        }
    }
}
