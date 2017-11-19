using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsersApplication.Models
{
    public class UsersApplicationManager: UserManager<MyUser>
    {
        public UsersApplicationManager(IUserStore<MyUser> store)  : base(store) 
        {
        }

        public static UsersApplicationManager Create(IdentityFactoryOptions<UsersApplicationManager> options, IOwinContext context) 
        {
            AppContext db = context.Get<AppContext>();
            UsersApplicationManager manager = new UsersApplicationManager(new UserStore<MyUser>(db));
            manager.UserValidator = new UserValidator<MyUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true 
            };
            return manager;
        }
    }
}