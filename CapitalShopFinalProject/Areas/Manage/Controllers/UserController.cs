using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

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

       
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
      
        public async Task<IActionResult> Index(int pageIndex=1)
        {
            IEnumerable<AppUser> Users = await _context.Users.Where(u => u.UserName != User.Identity.Name).ToListAsync();
            
            
           
    
            ViewBag.pageCount = (int)Math.Ceiling((decimal)Users.Count() / 15);

            Users = Users.Skip((pageIndex - 1) * 15).Take(15).ToList();

            ViewBag.pageIndex = pageIndex;


            return View(Users);
          
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Update(string? userid)
        {

            if(userid == null)
            {
                return BadRequest();
            }
            
            AppUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
            if (appUser == null)
            {
                return NotFound();

            }

            if(await _userManager.IsInRoleAsync(appUser, "Admin"))
            {
                ViewBag.admin = true;
            }
            if(await _userManager.IsInRoleAsync(appUser, "Member"))
            {
                ViewBag.admin = false;
            }


            return View(appUser);

        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string? userid, int? roleid)
        {
            if(userid == null)
            {
                return BadRequest();
            }

            AppUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
            if (appUser == null)
            {
                return NotFound();
            }

            if(roleid==1)
            {
                if (!await _userManager.IsInRoleAsync(appUser, "Admin"))
                {
                    await _userManager.RemoveFromRoleAsync(appUser, "Member");
                    await _userManager.AddToRoleAsync(appUser, "Admin");
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return RedirectToAction("Index", "User", new { area = "manage" });
                }
            }

            if (roleid == 2)
            {
                if (!await _userManager.IsInRoleAsync(appUser, "Member"))
                {
                    await _userManager.RemoveFromRoleAsync(appUser, "Admin");
                    await _userManager.AddToRoleAsync(appUser, "Member");
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return RedirectToAction("Index", "User", new { area = "manage" });
                }
            }

            return RedirectToAction("Index", "User", new { area = "manage" });







        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> BlockUser(string? userid) 
        {
            if(userid == null) { return BadRequest(); }
            AppUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
            if (appUser == null) { return NotFound(); }
            if(await _userManager.IsInRoleAsync(appUser, "Member"))
            {
                appUser.LockoutEnd = DateTime.UtcNow.AddMonths(1);
                appUser.BlockedBy = User.Identity.Name;
                _context.SaveChanges();
            }

            


            return RedirectToAction("Index", "User", new { area = "manage" });

        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> UnBlockUser(string? userid)
        {
            if (userid == null) { return BadRequest(); }
            AppUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
            if (appUser == null) { return NotFound(); }
            
                appUser.LockoutEnd = null;
                appUser.BlockedBy = null;
                _context.SaveChanges();
            




            return RedirectToAction("Index", "User", new { area = "manage" });

        }


    }
}
