using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LocalShowsOnly.Models;

namespace LocalShowsOnly.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<RSVP> RSVP { get; set; }
        public DbSet<Band> Band { get; set; }
        public DbSet<Venue> Venue { get; set; }
    }
}
