using Microsoft.AspNetCore.Identity;
using WebParking.Application;
using WebParking.Data;
using WebParking.Initializer;

namespace WebParking.Middleware
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;
        public DbInitializerMiddleware(RequestDelegate next)
        {
            _next = next; 
        }
        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider, ParkingContext dbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            await DbInitializer.InitializeAsync(dbContext);

            await RoleInitializer.InitializeAsync(userManager, roleManager, applicationDbContext);

            await _next.Invoke(context);
        }
    }
}
