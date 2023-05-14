using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.AccountVM;
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

        //public async Task<IActionResult> createUser()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        Email = "SuperAdmin@gmail.com",
        //        UserName = "SuperAdmin",
        //        Name = "Super",
        //        SurName = "Admin"

        //    };


        //    await _userManager.CreateAsync(appUser, "Super123");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");
        //    return Content("ok");
        //    AppUser appUser = new AppUser
        //    {
        //        Email = "Admin1@gmail.com",
        //        UserName = "Admin1",
        //        Name = "Admin1",
        //        SurName = "Admin1ov"

        //    };


        //    await _userManager.CreateAsync(appUser, "Admin111");
        //    await _userManager.AddToRoleAsync(appUser, "Admin");
        //    return Content("ok");


        //}





        [HttpGet]
      public async Task<IActionResult> SignIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("SuperAdmin") || User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Index", "Dashboard", "Manage");
                }


            }

            return View();


        }
        [HttpPost]

        public async Task<IActionResult> SignIn(SignInVM signInVM)
        {
            


            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "E-Mail or Password is not correct");
                return View(signInVM);
            }

            AppUser appUser=await _userManager.FindByEmailAsync(signInVM.Email);
            if (appUser == null)
            {
                ModelState.AddModelError("", "E-Mail or Password is not correct");
                return View(signInVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, signInVM.Password, true, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Account has been blocked");
                return View(signInVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "E-Mail or Password is not correct");
                return View(signInVM);
            }

            if(await _userManager.IsInRoleAsync(appUser, "SuperAdmin")==false) 
            {
                if (await _userManager.IsInRoleAsync(appUser, "Admin") == false)
                {
                    if (await _userManager.IsInRoleAsync(appUser, "Member") == true)
                    {

                        //return RedirectToAction("Index", "Home");
                        return RedirectToAction("Index", "Home", new { area = "" });

                    }
                    else{
                        ModelState.AddModelError("", "E-Mail or Password is not correct");
                        return View(signInVM);
                    }

                    
                    
                }
            }

           

            
            


            return RedirectToAction("Index", "Dashboard", "Manage");


        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIN", "Account");
        }



    }
}
