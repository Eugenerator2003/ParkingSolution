global using Parking.Domain;
using Parking.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebParking.Data;
using WebParking.Middleware;
using WebParking.Validators;
using WebParking.Application;
using WebParking.Managers;

namespace WebParking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            //customCulture.NumberFormat.NumberDecimalSeparator = ".";

            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ParkingContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddTransient<IUserValidator<IdentityUser>, CustomUserValidator>();

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
                     options.SignIn.RequireConfirmedAccount = false;
                     options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders().AddErrorDescriber<CustomIdentityErrorDescriber>();

            builder.Services.AddTransient<ParkingAccountant>();
            builder.Services.AddTransient<EmployeeManager>();
            builder.Services.AddTransient<OwnerManager>();

            builder.Services.AddTransient<CacheProvider>();
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseMiddleware<DbInitializerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}