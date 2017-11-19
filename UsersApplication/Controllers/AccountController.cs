using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UsersApplication.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UsersApplication.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Аccount/
        public ActionResult Index()
        {
            return View();
        }

        private UsersApplicationManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<UsersApplicationManager>();
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        //Get: /Account/Register/
        public ActionResult Register()
        {
            return View();
        }

        //
        //Post: /Account/Register/
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                MyUser user = new MyUser { UserName = model.Login, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SendConfirmEmail(user.Id);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }


        //
        //Get: /Account/Login/
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        //
        //Post: /Account/Login/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                MyUser user = await UserManager.FindAsync(model.Login, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "Account");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        //
        //Get: /Account/Logout
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> SendConfirmEmail(string userId)
        {
            MyUser user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var provider = new DpapiDataProtectionProvider("Sample");

                var userManager = new UserManager<MyUser>(new UserStore<MyUser>());

                UserManager.UserTokenProvider = new DataProtectorTokenProvider<MyUser>(
                    provider.Create("EmailConfirmation"));

                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // создаем ссылку для подтверждения
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                           protocol: Request.Url.Scheme);
                // отправка письма
                await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты", "Для завершения регистрации перейдите по ссылке:: <a href=\"" + callbackUrl + "\">завершить регистрацию</a>");
                ViewBag.email = user.Email;
                return View();
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed()
        {
            MyUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            return RedirectToAction("Index", "Account");
        }

        public async Task<ActionResult> Edit()
        {
            MyUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                EditModel model = new EditModel { FirstName = user.FirstName, SecondName = user.SecondName, Patronymic = user.Patronymic, Email = user.Email, Login = user.UserName, EmailConfirmed = user.EmailConfirmed, UserId = user.Id};
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditModel model)
        {
            MyUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.SecondName = model.SecondName;
                user.Patronymic = model.Patronymic;
                if (user.Email != model.Email)
                {
                    user.Email = model.Email;
                    user.EmailConfirmed = false;
                }
                
                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Что-то пошло не так");
                }
            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            var provider = new DpapiDataProtectionProvider("Sample");

            UserManager.UserTokenProvider = new DataProtectorTokenProvider<MyUser>(
                provider.Create("EmailConfirmation"));
            IdentityResult result = UserManager.ConfirmEmail(userId, code);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Что-то пошло не так");
            }
            return RedirectToAction("Index", "Account");

        }

        [HttpGet]
        public ActionResult ResetPassword(string userId, string code)
        {
            ViewBag.userId = userId;
            ViewBag.code = code;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model, string userId, string code)
        {
            var provider = new DpapiDataProtectionProvider("Sample");

            var userManager = new UserManager<MyUser>(new UserStore<MyUser>());

            UserManager.UserTokenProvider = new DataProtectorTokenProvider<MyUser>(
                provider.Create("ResetPassword"));

            IdentityResult result = await UserManager.ResetPasswordAsync(userId, code, model.Password );
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Что-то пошло не так");
            }

            return RedirectToAction("Index", "Account");
        }

        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View("ForgotPassword");
                }
                var provider = new DpapiDataProtectionProvider("Sample");

                var userManager = new UserManager<MyUser>(new UserStore<MyUser>());

                UserManager.UserTokenProvider = new DataProtectorTokenProvider<MyUser>(
                    provider.Create("ResetPassword"));

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Сброс пароля",
                    "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return View(model);
        }

        
	}
}