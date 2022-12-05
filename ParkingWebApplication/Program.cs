using Arch.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Parking.Application;
using ParkingWebApplication.Midddleware.DbInitializer;
using System.Drawing.Printing;

namespace ParkingWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IServiceCollection services = builder.Services;
            var connetctionString = builder.Configuration.GetConnectionString("SqlServer");
            ConfigurateServices(services, connetctionString);
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<DbInitializerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void ConfigurateServices(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ParkingContext>(options => options.UseSqlServer(connectionString));
            services.AddSession();
            services.AddControllersWithViews(options =>
            {
                options.CacheProfiles.Add("AllCaching", new Microsoft.AspNetCore.Mvc.CacheProfile()
                {
                    Duration = 254
                });
            });
            services.AddControllers();
        }
    }
}