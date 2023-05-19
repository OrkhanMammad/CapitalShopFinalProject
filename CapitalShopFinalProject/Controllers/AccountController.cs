using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels;
using CapitalShopFinalProject.ViewModels.AccountVM;
using CapitalShopFinalProject.ViewModels.BasketVM;
using CapitalShopFinalProject.ViewModels.OrderVMs;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Controllers
{
    


    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly SmtpSetting _smtpSetting;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context, IOptions<SmtpSetting> smtpSetting)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _smtpSetting = smtpSetting.Value;
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

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            string url = Url.Action("EmailConfirm", "Account", new { id = appUser.Id, token = token },
                HttpContext.Request.Scheme, HttpContext.Request.Host.ToString());

            string fullpath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "EmailConfirmPartial.cshtml");
            string templateContent = await System.IO.File.ReadAllTextAsync(fullpath);
            templateContent = templateContent.Replace("{{url}}", url);

            MimeMessage mimeMessage = new();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Email Confirmation";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = templateContent
            };
            using (SmtpClient smtpClient = new())
            {
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }
           

            return RedirectToAction(nameof(SignIn));
            
        }


        [HttpGet]
        public async Task<IActionResult> EmailConfirm(string id, string token)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) { return NotFound(); }

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!identityResult.Succeeded)
            {

                return BadRequest();

            }
            
            await _signInManager.SignInAsync(appUser, false);
            return RedirectToAction("index", "mycard");

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
        
        public async Task<IActionResult> Profile(bool a = false)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .Include(u=>u.CreditCards)
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
                CreditCard=appUser.CreditCards.FirstOrDefault(),
                
            };
            if (a == true)
            {
                ViewBag.CardError = true;
            }
            return View(profileVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .Include(u => u.CreditCards)
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            profileVM.Orders = appUser.Orders;

            if (!ModelState.IsValid)
            {
                ViewBag.Active = true;
                return View(profileVM);
                
            }
            
           
           


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

        [HttpGet]
        public  IActionResult ForgetPassword() 
        {
            return View();
        
        }

        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVM);
            }

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == forgetPasswordVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                return RedirectToAction("ForgetPassword", "account");
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

            string url = Url.Action("ResetPassword", "Account", new { token, email = forgetPasswordVM.Email },
                HttpContext.Request.Scheme, HttpContext.Request.Host.ToString());

            string fullpath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "PasswordReset.cshtml");
            string templateContent = await System.IO.File.ReadAllTextAsync(fullpath);
            templateContent = templateContent.Replace("{{url}}", url);

            MimeMessage mimeMessage = new();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Reset Password";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = templateContent
            };
            using (SmtpClient smtpClient = new())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.Auto);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }

            
            return RedirectToAction("index", "Home");

        }


        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordVM);

            AppUser appUser = await _userManager.FindByEmailAsync(resetPasswordVM.Email);
            if (appUser == null)
                return NotFound();

            var resetPassResult = await _userManager.ResetPasswordAsync(appUser, resetPasswordVM.Token, resetPasswordVM.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View();
            }


           
            return RedirectToAction("signin", "account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> addPaymentCard(CreditCard CreditCard)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.CreditCards).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (appUser == null)
            {
                return BadRequest();
            }
            bool a = false;

            if (!ModelState.IsValid)
            {
                a = true;
                return RedirectToAction("profile", "account", new { a = a });
            }

            

            IEnumerable<CreditCard> CreditCards = await _context.CreditCards.ToListAsync();
            CreditCard newCreditCard= CreditCards.FirstOrDefault(c => c.CardHolder.ToLower() ==CreditCard.CardHolder.ToLower().Trim() &&
            c.CardNumber == CreditCard.CardNumber && c.ExpDate == CreditCard.ExpDate && c.CVV == CreditCard.CVV);
            if(newCreditCard == null)
            {
                a = true;
                return RedirectToAction("profile", "account", new { a = a });
            }

            if (newCreditCard.UserId == appUser.Id)
            {
                return RedirectToAction("profile", "account");
            }
            if(appUser.CreditCards.Count() > 0)
            {
                foreach(CreditCard creditCard in appUser.CreditCards)
                {
                    creditCard.UserId = null;
                }
            }


            newCreditCard.UserId = appUser.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction("profile", "account");
        }










    }
}
