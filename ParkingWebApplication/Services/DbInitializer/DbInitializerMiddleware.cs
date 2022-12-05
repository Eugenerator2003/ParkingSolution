using Parking.Application;

namespace ParkingWebApplication.Services.DbInitializer
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;

        public DbInitializerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, IServiceProvider serviceProvider, ParkingContext parkingContext)
        {
            DbInitializer.Initialize(parkingContext);

            return _next.Invoke(context);
        }
    }
}
