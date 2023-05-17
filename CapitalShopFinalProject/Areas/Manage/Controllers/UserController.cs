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
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;       
        private readonly IWebHostEnvironment _env;
        public UserController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _roleManager = roleManager;
        }

       
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
      
        public async Task<IActionResult> Index(int pageIndex=1)
        {
            IEnumerable<AppUser> Users = await _context.Users.ToListAsync();
            
            //IEnumerable<IdentityRole> roles= await _context.Roles.ToListAsync();
    
            ViewBag.pageCount = (int)Math.Ceiling((decimal)Users.Count() / 15);

            Users = Users.Skip((pageIndex - 1) * 15).Take(15).ToList();

            ViewBag.pageIndex = pageIndex;


            return View(Users);


            
        }
    }
}
