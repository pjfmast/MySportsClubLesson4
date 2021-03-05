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

            // todo stap-16c: indien gebruik niet ingelogd is, voor geauthoriseerde acties omleiden naar Login.
            // De default redirect naar Login URL in Identity is: /Account/Login
            // Dit kun je op deze manier wijzigen:
            services.ConfigureApplicationCookie(
                options => {
                    // voor stap 16c configureren:
                    options.LoginPath = "/Users/Login";

                    // todo stap-18 configureren: acces denied
                    options.AccessDeniedPath = "/Users/AccessDenied";
                });

            // todo stap 5-1a: installeer Microsoft.AspNetCore.Authentication.Google
            //  In package manager console: Install-Package Microsoft.AspNetCore.Authentication.Google -Version 3.1.2
            // todo stap 5-1b: maak voor Google sign-in een client ID. Zie: https://developers.google.com/identity/sign-in/web/sign-in#before_you_begin
            // todo stap 5-2a: enable secret storage voor opslag ClientId en ClientSecret. Zie https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows#enable-secret-storage
            // todo stap 5-2b: store ClientId en ClientSecret. Zie https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-3.1#store-google-clientid-and-clientsecret

            // Voor les 5. Na het registreren van onze asp.net appicatie als Client bij Google Services
            // Your Client ID: 715995463169-abe2cfn259j40he7pp3c0u9jc3mse9v4.apps.googleusercontent.com
            // Your Client Secret: 8mzFpwqlY5ahWo2yRVuAvVFS

            // todo 5-1 voeg Goegle service voor authenticatie toe.
            services
                .AddAuthentication()
                .AddGoogle(options => {
                    IConfigurationSection googleAuthNSection =
                    Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];

                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
            , UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager
            ) {
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

            // todo stap 2c. Add middleware for Authorization
            app.UseAuthorization();


            // todo stap-15. Seed Identity EF store with roles and users
            UserAndRoleDataInitializer.SeedRoles(roleManager);
            UserAndRoleDataInitializer.SeedUsers(userManager);

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
