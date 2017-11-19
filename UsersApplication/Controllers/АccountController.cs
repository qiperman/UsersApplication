using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UsersApplication.Models;
using Microsoft.AspNet.Identity.Owin;

namespace UsersApplication.Controllers
{
    public class АccountController : Controller
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

	}
}