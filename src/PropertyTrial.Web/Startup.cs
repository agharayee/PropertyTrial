using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PropertyTrial.Data.DatabaseContext.ApplicationDbContext;
using PropertyTrial.Data.DatabaseContext.AuthenticationDbCOntext;
using PropertyTrial.Data.Entities;

namespace PropertyTrial.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ApplicationConnection"),
               SqlSeerverOption =>
               {
                   SqlSeerverOption.MigrationsAssembly("PropertyTrial.Data");
               }));
            services.AddDbContextPool<AuthenticationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AuthenticationConnection"),
                    sqlServerOptions =>
                    {
                        sqlServerOptions.MigrationsAssembly("PropertyTrial.Data");
                    }));

            services.AddIdentity<ApplicationUser, IdentityRole<string>>().AddEntityFrameworkStores<AuthenticationDbContext>()
                .AddDefaultTokenProviders();
            services.AddControllersWithViews();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void MigrateDataContexts(IServiceProvider svp)
        {
            var AuthenticationDbContext = svp.GetRequiredService<AuthenticationDbContext>();
            AuthenticationDbContext.Database.Migrate();

            var ApplicationDbContext = svp.GetRequiredService<ApplicationDbContext>();
            ApplicationDbContext.Database.Migrate();
        }

        public async Task CreateDefaultRolesAndUser(IServiceProvider svp)
        {
            //Create an array of string to store all the roles needed
            string[] roles = new string[]
            {
                "System Administrator",
                "Agent",
                "User"
            };
            var UserEmail = "admin@estateapp.com";
            var UserPassword = "SuperSecretPassword@2020";

            var rolesManager = svp.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in roles)
            {
                var roleExist = await rolesManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await rolesManager.CreateAsync(new IdentityRole { Name = role });

                }
            }

            //Introduce UserManager to manage users and add user when needed 
            var userManager = svp.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(UserEmail);
            if (user is null) //If user is not found then create a user
            {
                user = new ApplicationUser
                {
                    UserName = UserEmail,
                    Email = UserEmail,
                    PhoneNumber = "+2348076261518",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await userManager.CreateAsync(user, UserPassword);
                await userManager.AddToRolesAsync(user, roles);
            }


        }

    }
}
