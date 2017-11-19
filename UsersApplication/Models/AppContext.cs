﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UsersApplication.Models
{
    public class AppContext: IdentityDbContext<MyUser>
    {
        public AppContext() : base("UsersDb") 
        { }
 
        public static AppContext Create()
        {
            return new AppContext();
        }
    }

}