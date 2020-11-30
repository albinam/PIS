using PIS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PIS
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("aspnet-PIS-20201130063633") { }
        public DbSet<Book> Books { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<BookContract> BookContracts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<LibraryCard> LibraryCards { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Status> Statuses { get; set; }

    }
}