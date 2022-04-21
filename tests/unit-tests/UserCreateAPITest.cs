using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Unico.Admin.Api.Models;
using Unico.Admin.Api.Services;
using unit_tests;
using Xunit;

namespace Unico.Admin.Api
{
    public class UserCreateAPITest
    {

        [Fact]
        public async void TestCreateUser_ok_expectSucess()
        {
            // ARRANGE
            UserDto user = TestUserUtil.CreateTestUserSuperMario();

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(c => c.CreateUser(It.IsAny<UserDto>())).ReturnsAsync(user);

            Mock<HttpRequest> mockRequest = CreateMockRequest(user);
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            UserController uc = new UserController(new NullLogger<UserController>(), userServiceMock.Object);

            // ACT
            var result = await uc.CreateUser(mockRequest.Object);
 

            // ASSERT
            Assert.IsType<ObjectResult>(result);
            var okResult = result as ObjectResult;
            Assert.Equal(StatusCodes.Status201Created, okResult.StatusCode);
            var actualUser = okResult.Value as EnvelopedResult<UserDto> ;
            Assert.Equal(user, actualUser.data);
        }

        [Fact]
        public async void TestCreateUser_emptyUser_expectBadRequest()
        {
            // ARRANGE
            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            //userServiceMock.Setup(c => c.CreateUser(It.IsAny<UserDto>())).ReturnsAsync(user);

            Mock<HttpRequest> mockRequest = CreateMockRequest(null);
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            UserController uc = new UserController(new NullLogger<UserController>(), userServiceMock.Object);

            // ACT
            var result = await uc.CreateUser(mockRequest.Object);


            // ASSERT
            Assert.IsType<ObjectResult>(result);
            var badResult = result as ObjectResult;
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
            string errorMsg = (badResult.Value as EnvelopedResult<UserDto>).errors[0].message;
            Assert.Equal(UserController.ERROR_INPUT_NULL, errorMsg);
        }

        [Fact]
        public async void TestCreateUser_notUserObject_expectBadRequest()
        {
            // ARRANGE
            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            //userServiceMock.Setup(c => c.CreateUser(It.IsAny<UserDto>())).ReturnsAsync(user);

            Mock<HttpRequest> mockRequest = CreateMockRequest("not a user");
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            UserController uc = new UserController(new NullLogger<UserController>(), userServiceMock.Object);

            // ACT
            var result = await uc.CreateUser(mockRequest.Object);


            // ASSERT
            Assert.IsType<ObjectResult>(result);
            var badResult = result as ObjectResult;
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
            string errorMsg = (badResult.Value as EnvelopedResult<UserDto>).errors[0].message;
            Assert.Equal(UserController.ERROR_INPUT_NOT_PARSABLE, errorMsg);
        }

        [Fact]
        public async void TestCreateUser_NOK_backendNotAvaialbe()
        {
            // ARRANGE
            UserDto user = TestUserUtil.CreateTestUserSuperMario();

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            var mockEx = new HttpRequestException(UserController.ERROR_BACKEND, null, HttpStatusCode.ServiceUnavailable);
            userServiceMock.Setup(c => c.CreateUser(It.IsAny<UserDto>())).Throws(mockEx); ;

            Mock<HttpRequest> mockRequest = CreateMockRequest(user);
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            UserController uc = new UserController(new NullLogger<UserController>(), userServiceMock.Object);

            // ACT
            var result = await uc.CreateUser(mockRequest.Object);


            // ASSERT
            Assert.IsType<ObjectResult>(result);
            var nokResult = result as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, nokResult.StatusCode);
            var actualUser = nokResult.Value as EnvelopedResult<UserDto>;
            string errorMsg = (nokResult.Value as EnvelopedResult<UserDto>).errors[0].message;
            Assert.Contains(UserController.ERROR_BACKEND, errorMsg);
        }

        [Fact]
        public async void TestCreateUser_duplicaterUser_expect409()
        {
            // ARRANGE
            UserDto user = TestUserUtil.CreateTestUserSuperMario();

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            var mockEx = new HttpRequestException(UserController.ERROR_BACKEND, null, HttpStatusCode.Conflict);
            userServiceMock.Setup(c => c.CreateUser(It.IsAny<UserDto>())).Throws(mockEx); ;

            Mock<HttpRequest> mockRequest = CreateMockRequest(user);
            Environment.SetEnvironmentVariable("FunPS-CreateUser", "http://localhost/UnitTest123");

            UserController uc = new UserController(new NullLogger<UserController>(), userServiceMock.Object);

            // ACT
            var result = await uc.CreateUser(mockRequest.Object);

            // ASSERT
            Assert.IsType<ObjectResult>(result);
            var nokResult = result as ObjectResult;
            Assert.Equal(StatusCodes.Status409Conflict, nokResult.StatusCode);
        }


        private static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }
    }
  
}