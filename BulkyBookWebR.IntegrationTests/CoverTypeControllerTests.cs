using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess;
using Xunit;

namespace BulkyBookWeb.IntegrationTests
{
    public class CoverTypeControllerTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _client;

        public CoverTypeControllerTests(TestingWebAppFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async void CoverTypeController_Index_ReturnsAllCoverTypes()
        {
            var response = await _client.GetAsync("/covertype");
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("Hard Cover", responseString);
            Assert.Contains("Paperback", responseString);
        }

    }
}
