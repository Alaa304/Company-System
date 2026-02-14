using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RouteG01.DAL.Models.Shared;
using RouteG01.Pl.Helper;
using RouteG01.Pl.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RouteG01.Pl.Controllers
{
    public class AccountController(UserManager<ApplicationUser>_userManager,SignInManager<ApplicationUser>_signInManager) : Controller
    {

        #region Register
        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var User = new ApplicationUser()
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                // ❌ متحطش Password هنا
            };

            // ✅ هنا تمرر الباسورد للـ UserManager وهو يتولى عمل Hash وتخزينه
            var Result = _userManager.CreateAsync(User, viewModel.Password).Result;

            if (Result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(viewModel);
            }
        }

        #endregion

        //P@$$0rd//old
        //P@$$0wd//new
        #region Login
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user is not null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            return View(viewModel);
        }

        #endregion
        #region Sign Out
        [HttpGet]
        public  async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        #endregion
        #region Forget Password
        [HttpGet]
        public IActionResult ForgetPassword() => View();
        //------------------------------------

       
     

       
        #endregion
        #region SendResetPasswordLink
        [HttpPost]
        public IActionResult SendResetPasswordLink(ForgetPasswordViewModel forgetPassword)
        {
            if (ModelState.IsValid)
            {
                var User = _userManager.FindByEmailAsync(forgetPassword.Email).Result;
                if (User is not null)
                {
                    //Send Email
                    var Token = _userManager.GeneratePasswordResetTokenAsync(User).Result;
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = forgetPassword.Email, Token }, Request.Scheme);
                    var email = new Email()
                    {
                        To = forgetPassword.Email,
                        Subject = "Reset Password Link",
                        Body = ResetPasswordLink

                    };
                    EmailSettings.SendEmail(email);
                    return RedirectToAction("CheckYourInbox");
                }

            }
            ModelState.AddModelError(string.Empty, "Invalid  Operation");
            return View("ForgetPassword", forgetPassword);
        }

        #endregion
        #region Check Your Inbox
        [HttpGet]
        public IActionResult CheckYourInbox() => View();
        #endregion
        #region ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email, string Token)
        {
            TempData["email"] = email;
            TempData["Token"] = Token;
            return View();

        }
        [HttpPost]
         public IActionResult ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (!ModelState.IsValid) return View(resetPassword);
            string email = TempData["email"]as string ?? string.Empty;
            string Token = TempData["Token"] as string ?? string.Empty;
            var User = _userManager.FindByEmailAsync(email).Result;
            if(User is not null)
            {
               var Result= _userManager.ResetPasswordAsync(User, Token, resetPassword.Password).Result;
       
            if(Result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach( var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(nameof(ResetPassword),resetPassword);
        }

            
        #endregion



    }
}
