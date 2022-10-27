using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using MimeKit;
using MimeKit.Text;
using Pustok.Models;
using Pustok.ViewModels.AccountViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        public AccountController(UserManager<AppUser> manager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment environment)
        {
            _manager = manager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _environment = environment;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return NotFound();

            AppUser appUser = await _manager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email Ve Ya Shifre Yanlisdir");
                return View(loginVM);
            }

            if (appUser.EmailConfirmed == false)
            {
                ModelState.AddModelError("", "Email confirm olmayib");
                return View(loginVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult signinResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, true, true);

            if (signinResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Email Bloklanib");
                return View(loginVM);
            }

            if (!signinResult.Succeeded)
            {
                ModelState.AddModelError("", "Email Ve Ya Shifre Yanlisdir");
                return View(loginVM);
            }
          



            if ((await _manager.GetRolesAsync(appUser))[0] == null)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "manage" });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                FatherName = registerVM.FatherName,
                Age = registerVM.Age,
                Email = registerVM.Email,
                UserName = registerVM.UserName
            };

            IdentityResult identityResult = await _manager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }



            await _manager.AddToRoleAsync(appUser, "Member");
            await _signInManager.SignInAsync(appUser, true);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Ilkin Zamanov", "izamanli97@gmail.com"));
            message.To.Add(new MailboxAddress(appUser.Name, appUser.Email));
            message.Subject = "Emaili Tesdiqleyin..";

            string emailbody = string.Empty;

            using (StreamReader stream = new StreamReader(Path.Combine(_environment.WebRootPath, "templates", "htmlpage.html")))
            {
                emailbody = stream.ReadToEnd();
            }

            string emailconfirmtoken = await _manager.GenerateEmailConfirmationTokenAsync(appUser);
            string url = Url.Action("confirmemail", "account", new { Id = appUser.Id, token = emailconfirmtoken }, Request.Scheme);
            emailbody = emailbody.Replace("{{fullname}}", $"{appUser.Name}{appUser.SurName}").Replace("{{url}}", $"{url}");
            message.Body = new TextPart(TextFormat.Html) { Text = emailbody };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("izamanli97@gmail.com", "Biznes647@");
                smtp.Send(message);
                smtp.Disconnect(true);

                return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if (string.IsNullOrWhiteSpace(Id)||string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            AppUser appUser = await _manager.FindByIdAsync(Id);

            if (appUser==null)
            {
                return NotFound();
            }

            IdentityResult identityResult = await _manager.ConfirmEmailAsync(appUser,token);

            if (!identityResult.Succeeded)
            {
                return NotFound();
            }

            return RedirectToAction("Login");

        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return NotFound();

            AppUser appUser = await _manager.FindByEmailAsync(email);

            if (appUser == null)
                return NotFound();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Ulvi Alekberov", "ulvialekberov17@gmail.com"));
            message.To.Add(new MailboxAddress(appUser.Name, appUser.Email));
            message.Subject = "Reset Password";

            string emailbody = string.Empty;

            using (StreamReader stream = new StreamReader(Path.Combine(_environment.WebRootPath, "templates", "forgetpassword.html")))
            {
                emailbody = stream.ReadToEnd();
            }

            string forgetpassword = await _manager.GeneratePasswordResetTokenAsync(appUser);
            string url = Url.Action("changepassword", "account", new { Id = appUser.Id, token = forgetpassword }, Request.Scheme);
            emailbody = emailbody.Replace("{{fullname}}", $"{appUser.Name}{appUser.SurName}").Replace("{{url}}", $"{url}");
            message.Body = new TextPart(TextFormat.Html) { Text = emailbody };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("ulvialekberov17@gmail.com", "Alekberov5859@");
            smtp.Send(message);
            smtp.Disconnect(true);

            return View();
        }

        public async Task<IActionResult> ChangePassword(string Id, string token)
        {
            if (string.IsNullOrWhiteSpace(Id)||string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            AppUser appUser = await _manager.FindByIdAsync(Id);

            if (appUser==null)
            {
                return NotFound();
            }

            ResetPasswordVM resetPasswordVM = new ResetPasswordVM
            {
                Id=Id,
                Token=token
            };

            return View(resetPasswordVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            if (string.IsNullOrWhiteSpace(resetPasswordVM.Id) || string.IsNullOrEmpty(resetPasswordVM.Token))
            {
                return NotFound();
            }

            AppUser appUser = await _manager.FindByIdAsync(resetPasswordVM.Id);

            if (appUser == null)
            {
                return NotFound();
            }
            IdentityResult identityResult = await _manager.ResetPasswordAsync(appUser, resetPasswordVM.Token, resetPasswordVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
                return View();
            }
            return RedirectToAction("Login");
        }
        #region Add Role
        //public async Task<IActionResult> AddRole()
        //{
        //    if (!await _roleManager.RoleExistsAsync("Admin"))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    }
        //    if (!await _roleManager.RoleExistsAsync("Member"))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
        //    }
        //    if (!await _roleManager.RoleExistsAsync("User"))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
        //    }

        //    return Content("Role Yarandi");
        //}
        #endregion
    }
}