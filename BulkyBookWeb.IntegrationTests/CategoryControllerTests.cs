using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;
using Xunit;

namespace BulkyBookWeb.IntegrationTests
{
    public class CategoryControllerTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _client;

        public CategoryControllerTests(TestingWebAppFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async void Index_WhenCalled_ReturnsSomething()
        {
            var response = await _client.GetAsync("/Category");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async void Create_WhenPosted_AddsCategory()
        {
            var initResponse = await _client.GetAsync("/Category/Create");
            var responseString = await initResponse.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);
            var input = htmlDoc.DocumentNode.SelectSingleNode("//input[@name = \"__RequestVerificationToken\"]");
            var token = input.Attributes["value"].Value;

            
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Category/Create");
            var formModel = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", token },   
                { "Name", "Test" },
                { "DisplayOrder", "33" }
            };

            postRequest.Content = new FormUrlEncodedContent(formModel);

            var response = await _client.SendAsync(postRequest);
            //var responseString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

        }
    }
}