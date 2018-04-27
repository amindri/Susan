using Microsoft.AspNetCore.Identity;
using System;

namespace FirstInFirstAid.Models.User
{
    public class AppUser : IdentityUser
    {
        public int Id { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
    }
}