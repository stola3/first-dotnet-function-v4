using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using Unico.Admin.Api.Models;
using unit_tests;
using Xunit;

namespace Unico.Admin.Api.Services
{
    public class UserServiceTest
    {
        private readonly ILogger<UserService> logger = new LoggerFactory().CreateLogger<UserService>(); // NullLoggerFactory.Instance.CreateLogger("Test");


        [Fact]
        public async void testCreateUser_shouldWork()
        {
            // ARRANGE
            var handler = new Mock<HttpMessageHandler>();
            var client = handler.CreateClient();
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            UserDto user = TestUserUtil.CreateTestUserSuperMario();

            var jsonRespone = JsonConvert.SerializeObject(user);
            handler.SetupAnyRequest().ReturnsResponse(jsonRespone);

            // ACT
            UserService us = new UserService(logger, client);
            var result = await us.CreateUser(user);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(result, user);
        }


        [Fact]
        public async void testCreateUser_backend_500()
        {
            // ARRANGE
            var handler = new Mock<HttpMessageHandler>();
            var client = handler.CreateClient();
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.InternalServerError);

            // ACT
            UserService us = new UserService(logger, client);
            try
            {
                var result = await us.CreateUser(TestUserUtil.CreateTestUserSuperMario());
            } // ASSERT
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError)
            { // expected
                Assert.Equal(StatusCodes.Status500InternalServerError, (int)ex.StatusCode);
                return;
                
            }
            Assert.Equal("", "500 exptected");
        }

        [Fact]
        public async void testCreateUser_backend_409()
        {
            // ARRANGE
            var handler = new Mock<HttpMessageHandler>();
            var client = handler.CreateClient();
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.Conflict);

            // ACT
            UserService us = new UserService(logger, client);
            try
            {
                var result = await us.CreateUser(TestUserUtil.CreateTestUserSuperMario());
            } // ASSERT
            catch (HttpRequestException ex) 
            { // expected
                Assert.Equal(StatusCodes.Status409Conflict, (int) ex.StatusCode);
                return;
            }
            Assert.Equal("", "500 exptected");
        }
    }
}