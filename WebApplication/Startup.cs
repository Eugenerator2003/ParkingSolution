namespace WebApplication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

        }
        
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.Map("/Home", UseRun);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Run writting");
            });
            
            
        }

        private static void UseRun(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Use writting");
                await next.Invoke();
            }
            );

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("\nRun writting");
            });
        }
    }
}
