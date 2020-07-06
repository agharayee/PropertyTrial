using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyTrial.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyTrial.Data.DatabaseContext.AuthenticationDbCOntext
{
    public class AuthenticationDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
        {

        }
    }
}
