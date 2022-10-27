using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pustok.DAL;
using Pustok.Models;
using Pustok.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options => 
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            services.AddDbContext<AppDbContext>(options => 
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });

            services.AddSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });

            services.AddScoped<LayoutService>();

            services.AddIdentity<AppUser, IdentityRole>(identityOptions => 
            {
                identityOptions.Password.RequireDigit = true;
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireLowercase = true;
                identityOptions.Password.RequireNonAlphanumeric = true;
                identityOptions.Password.RequireUppercase = true;
                identityOptions.Password.RequiredUniqueChars = 1;

                identityOptions.User.RequireUniqueEmail = true;

                identityOptions.Lockout.MaxFailedAccessAttempts = 3;
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                identityOptions.Lockout.AllowedForNewUsers = true;
            }).AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppDbContext>();

            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Dashboard/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "areas",
                        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                    );

                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}"
                    );

                
            });
        }
    }
}
