using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.AccountVM;
using CapitalShopFinalProject.ViewModels.BasketVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Controllers
{
    


    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
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

          IdentityResult identityResult =  await _userManager.CreateAsync(appUser, registerVM.Password);
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


            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, signInVM.Password,true, true);

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




            HttpContext.Response.Cookies.Append("basket", "");



            return RedirectToAction("Index", "Mycard");



        }



        [HttpGet]
        
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .Include(u=>u.Orders)
                .ThenInclude(o=>o.OrderItems)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Addresses= appUser.Addresses,
                Name=appUser.Name,
                Surname=appUser.SurName,
                Email=appUser.Email,
                UserName=appUser.UserName,
                Orders=appUser.Orders,
                
            };

            return View(profileVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Active = true;
                return View(profileVM);
                
            }
            
           
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);


            if(appUser.Name!=profileVM.Name)
            {
                appUser.Name = profileVM.Name;
            }
            if (appUser.SurName != profileVM.Surname)
            {
                appUser.SurName = profileVM.Surname;
            }
            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                appUser.Email = profileVM.Email;
            }
            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.UserName;
            }


            IdentityResult identityResult =  await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {

                foreach(IdentityError identityError in identityResult.Errors)
                {

                    ModelState.AddModelError("", identityError.Description);
                }
                ViewBag.Active = true;
                return View(profileVM);
                

            }

            await _signInManager.SignInAsync(appUser, true);


            if (!string.IsNullOrEmpty(profileVM.OldPassword))
            {
                if(!await _userManager.CheckPasswordAsync(appUser,profileVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old Password is not correct");
                    ViewBag.Active = true;
                    return View(profileVM);

                }
                if(string.IsNullOrEmpty(profileVM.Password)) 
                {
                    ModelState.AddModelError("Password", "New Password can not be empty");
                    ViewBag.Active = true;
                    return View(profileVM);

                }
                if (string.IsNullOrEmpty(profileVM.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "New Password can not be empty");
                    ViewBag.Active = true;
                    return View(profileVM);

                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

              IdentityResult identityResult1 = await _userManager.ResetPasswordAsync(appUser, token, profileVM.Password);

                if(!identityResult1.Succeeded)
                {
                    foreach(IdentityError identityError in identityResult1.Errors)
                    {

                        ModelState.AddModelError("", identityError.Description);

                    }
                    ViewBag.Active = true;
                    return View(profileVM);
                }


            }


            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(Address address)   
        {
            AppUser appUser = await _userManager.Users.
                    Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses
            };

            if(!ModelState.IsValid) 
            
            {

                return View(nameof(Profile), profileVM);           
            }

            if(address.IsMain && appUser.Addresses!=null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(a=>a.IsMain))
            {
                appUser.Addresses.FirstOrDefault(a=>a.IsMain).IsMain = false;
                

            }
            address.IsDeleted = false;
            address.AppUserId = appUser.Id;
            address.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            address.CreatedAt= DateTime.UtcNow.AddHours(4);
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Profile));

        }



        public async Task<IActionResult>  LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");


        }





    }
}
