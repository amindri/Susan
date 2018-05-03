using FirstInFirstAid.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.DAL
{
    public class FirstInFirstAidDBContext : IdentityDbContext<ApplicationUser>
    {
        public FirstInFirstAidDBContext() : base("FirstInFirstAidDBContext", throwIfV1Schema: false)
        {
            //Database.SetInitializer<FirstInFirstAidDBContext>(null); // Remove default initializer
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
        }

        public static FirstInFirstAidDBContext Create()
        {
            return new FirstInFirstAidDBContext();
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
     //   public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // IMPORTANT: we are mapping the entity User to the same table as the entity ApplicationUser
            //modelBuilder.Entity<User>().ToTable("User");
        }

        

        public System.Data.Entity.DbSet<FirstInFirstAid.Models.ClientContact> ClientContacts { get; set; }
    }
}