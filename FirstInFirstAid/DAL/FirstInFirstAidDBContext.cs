using FirstInFirstAid.Models;
using FirstInFirstAid.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FirstInFirstAid.DAL
{
    public class FirstInFirstAidDBContext : IdentityDbContext<AppUser>
    {
        public FirstInFirstAidDBContext() : base("FirstInFirstAidDBContext")
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<EquipmentAllocation> EquipmentAllocations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventSegment> EventSegments { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Trainor> Trainors { get; set; }
        public DbSet<TrainorAllocationForEventSeg> TrainorEventSegAllocations { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<AppUser> LoginUsers { get; set; }
        public DbSet<ClientContact> ClientContacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        
    }
}