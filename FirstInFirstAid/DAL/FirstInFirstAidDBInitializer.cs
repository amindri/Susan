using FirstInFirstAid.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FirstInFirstAid.App_Start;
using System.Configuration;

namespace FirstInFirstAid.DAL
{
    public class FirstInFirstAidDBInitializer : DropCreateDatabaseIfModelChanges<FirstInFirstAidDBContext>
    {
        protected override void Seed(FirstInFirstAidDBContext context)
        {
            base.Seed(context);
        }
    }
}