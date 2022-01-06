using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var Services = scope.ServiceProvider;
            try
            {
                 var context = Services.GetRequiredService<DataContext>();
                 await context.Database.MigrateAsync(); 
                 await Seed.SeedUsers(context);
            }
            catch (Exception ex )
            {
                var logger = Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
                
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
