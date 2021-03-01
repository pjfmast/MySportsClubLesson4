using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcSportsClub.Data;
using MvcSportsClub.Models;

namespace MvcSportsClub {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();
            services.AddDbContext<SportsClubContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Choose here between FakeRepository en EFMemberRepository:
            // whenever an IMemberRepository is needed a EFMemberRepository is created.
            services.AddTransient<IMemberRepository, EFMemberRepository>();
            services.AddTransient<IWorkoutRepository, EFWorkoutRepository>();

            // todo stap 2a: registreer services voor Identity:
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<SportsClubContext>();

            // todo stap 9a - testen registreren en check op password
            // todo stap 9b - configureren check op password
            //    (Kan ook als parameter in AddIdentity worden ingesteld.)
            services.Configure<IdentityOptions>(
                options => {
                    // voor stap 9b: Configuration check on password:
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 5;
                    options.Password.RequiredLength = 8;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // todo stap 2b. Add middleware for Authentication 
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
