using FirstInFirstAid.Models.User;
using System.Data.Entity;

namespace FirstInFirstAid.DAL
{
    public class FirstInFirstAidDBInitializer : DropCreateDatabaseIfModelChanges<FirstInFirstAidDBContext>
    {
        protected override void Seed(FirstInFirstAidDBContext context)
        {
            base.Seed(context);
            var loginUser = new AppUser { UserName="admin", Password="admin"};
            context.LoginUsers.Add(loginUser);
            context.SaveChanges();
            
        }
    }
}