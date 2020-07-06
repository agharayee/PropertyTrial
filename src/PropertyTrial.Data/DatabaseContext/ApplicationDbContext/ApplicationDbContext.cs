using Microsoft.EntityFrameworkCore;
using PropertyTrial.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyTrial.Data.DatabaseContext.ApplicationDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Property> Propperties { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
