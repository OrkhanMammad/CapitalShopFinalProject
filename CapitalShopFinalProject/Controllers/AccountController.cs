using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.AccountVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CapitalShopFinalProject.Controllers
{
    


    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }




        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
            {
                return View(registerVM);
            }

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName=registerVM.Surname,
                Email = registerVM.Email,
                UserName= registerVM.UserName
            };

          IdentityResult identityResult =  await _userManager.CreateAsync(appUser, "Orkhan6991");
            if(!identityResult.Succeeded)           
            {
            foreach(IdentityError identityError in identityResult.Errors)
                {

                    ModelState.AddModelError("", identityError.Description);

                }
            return View(registerVM);
            
            
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

           
                return RedirectToAction("Index", "Home");
            
        }

        [HttpGet]

        public async Task<IActionResult> SignIn()
        {

            return View();

        }






    }
}
