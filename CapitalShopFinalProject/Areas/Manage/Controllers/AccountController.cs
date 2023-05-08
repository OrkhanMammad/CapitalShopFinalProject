using CapitalShopFinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CapitalShopFinalProject.Areas.Manage.Controllers 
{
    [Area("Manage")]

    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //[HttpGet]
        //public async Task<IActionResult> createRole() 
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));
        //    return Content("Succesfull");

            
        //}
        //[HttpGet]
        //public async Task<IActionResult> createUser()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        FullName= "Orxan Mammadli",
        //        Email="superadmin@gmail.com",
        //        UserName="SuperAdmin"
        //    };
        //    await _userManager.CreateAsync(appUser, "Orxan6991");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Content("Succesfull");


        //}

        




    }
}
