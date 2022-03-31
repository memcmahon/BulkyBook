using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using BulkyBookWeb.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

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

                //services.AddAntiforgery(options =>
                //{
                //    options.Cookie.Name = AntiForgeryTokenExtractor.AntiForgeryCookieName;
                //    options.FormFieldName = AntiForgeryTokenExtractor.AntiForgeryFieldName;
                //});

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });
            
        }
    }

    public static class AntiForgeryTokenExtractor
    {
        public static string AntiForgeryFieldName { get; }
        public static string AntiForgeryCookieName { get; }

        private static string ExtractAntiForgeryCookieValueFrom(HttpResponseMessage response)
        {
            var antiForgeryCookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains(AntiForgeryCookieName));

            if (antiForgeryCookie == null)
                throw new ArgumentException($"Cookie'{AntiForgeryCookieName}' not found in HTTP response", nameof(response));

            var antiForgeryCookieValue = SetCookieHeaderValue.Parse(antiForgeryCookie).Value.ToString();

            return antiForgeryCookieValue;
        }

        private static string ExtractAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch = Regex.Match(htmlBody, $@"\input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)""\/\>");

            if (requestVerificationTokenMatch.Success)
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;

            throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' not found in HTML", nameof(htmlBody));
        }

        public static async Task<(string fieldValue, string cookieValue)> ExtractAntiForgeryValues(HttpResponseMessage response)
        {
            var cookie = ExtractAntiForgeryCookieValueFrom(response);
            var token = ExtractAntiForgeryToken(await response.Content.ReadAsStringAsync());

            return (fieldValue: token, cookieValue: cookie);
        }
    }
}
