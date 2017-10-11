using FirstInFirstAid.Models;
using FirstInFirstAid.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.DAL
{
    public class FirstInFirstAidDBContext : DbContext
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
        public DbSet<LoginUser> LoginUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}