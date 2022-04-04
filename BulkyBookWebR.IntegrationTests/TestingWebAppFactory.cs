using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;
using BulkyBook.DataAccess;

namespace BulkyBookWeb.IntegrationTests
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryBulkyBookTest");
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                        //InitializeDbForTests(appContext);
                        
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });

        }

        //public void InitializeDbForTests(ApplicationDbContext db)
        //{
        //    db.AddRange(new List<CoverType>
        //    {
        //        new CoverType { Name = "Hard Cover" },
        //        new CoverType { NameValueHeaderValue = "Paperback" }
        //    });
        //    db.SaveChanges();
        //}
    }
}