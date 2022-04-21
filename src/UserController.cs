using System.IO;
using System.Net;
using System.Threading.Tasks;
using Unico.Admin.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Unico.Admin.Api.Services;
using System;
using System.Net.Http;

namespace Unico.Admin.Api
{

    public class UserController
    {

        private readonly IUserService userService;

        private readonly ILogger<UserController> _logger;

        public readonly static string ERROR_INPUT_NULL = "Input may not be null.";
        public readonly static string ERROR_INPUT_NOT_PARSABLE = "Unable to parse input.";
        public readonly static string ERROR_BACKEND = "Backend (Function PS) Error";

        public UserController(ILogger<UserController> log, IUserService us)
        {
            _logger = log;
            userService = us;
        }

        [FunctionName("CreateUser")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "User" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(EnvelopedResult<UserDto>), Description = "Success")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableEntity, contentType: "application/json", bodyType: typeof(EnvelopedResult<UserDto>), Description = "Unprocessable Input")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "application/json", bodyType: typeof(EnvelopedResult<UserDto>), Description = "Duplicate")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(EnvelopedResult<UserDto>), Description = "Error during persisting")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserDto))]
        public async Task<IActionResult> CreateUser(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] HttpRequest req)
        {

            _logger.LogInformation("Create user triggered");
            UserDto data;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                data = JsonConvert.DeserializeObject<UserDto>(requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ERROR_INPUT_NOT_PARSABLE, ex);
                return createErrorResponse("ERROR_INPUT_NOT_PARSABLE", ERROR_INPUT_NOT_PARSABLE, StatusCodes.Status422UnprocessableEntity);
            }
            if (data == null)
            {
                _logger.LogWarning(ERROR_INPUT_NULL);
                return createErrorResponse("ERROR_INPUT_NULL", ERROR_INPUT_NULL, StatusCodes.Status422UnprocessableEntity);
            }

            //UserService userService = new UserService(log);
            try
            {
                UserDto createdUser = await userService.CreateUser(data);
                //return new ObjectResult(createdUser);
                return new ObjectResult(new EnvelopedResult<UserDto>(createdUser)) { StatusCode = StatusCodes.Status201Created };
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning(ERROR_BACKEND + " error response: " + ex.Message, ex);
                return createErrorResponse("ERROR_BACKEND", ex.Message, (int)ex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ERROR_BACKEND + " error response: " + ex.Message, ex);
                return createErrorResponse("ERROR_BACKEND", ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        private ObjectResult createErrorResponse(string errorKey, string errorMsg, int statusCode)
        {
            ErrorResponse[] errorList = { new ErrorResponse(errorKey, errorMsg) };
            var result = new EnvelopedResult<UserDto>(null, errorList);
            return new ObjectResult(result) { StatusCode = statusCode };
        }
    }
}

