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
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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

           
                return RedirectToAction(nameof(SignIn));
            
        }

        [HttpGet]

        public async Task<IActionResult> SignIn()
        {

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInVM signInVM)
        {
            if (!ModelState.IsValid)
            {
                return View(signInVM);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(signInVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is not correct");
                return View(signInVM);
            }



            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, signInVM.Password, true, true);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is not correct");
                return View(signInVM);

            }



            return RedirectToAction("Index", "Home");



        }
    }
}
