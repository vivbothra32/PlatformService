using System.Diagnostics.Eventing.Reader;
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope()){
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd){

            if(isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try{
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Could not run migrations: {ex.Message}");
                }
            }
                if(!context.Platforms.Any())
                {
                    Console.WriteLine("Seeding data...");
                    context.Platforms.AddRange(
                        new Platform{ Name="Dot Net", Cost = "Free", Publisher="Microsoft"},
                        new Platform{ Name="Docker", Cost="Free", Publisher="Cloud Native" },
                        new Platform{ Name="SQL Server", Cost="Free", Publisher="Microsoft"},
                        new Platform{ Name="Azure", Cost="Chargeable", Publisher="Microsoft"}
                    );
                    context.SaveChanges();
                }
                else{
                    Console.WriteLine("Data already exists! ");
                }
            
        }
    }
}