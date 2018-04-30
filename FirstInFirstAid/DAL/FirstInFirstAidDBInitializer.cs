using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

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