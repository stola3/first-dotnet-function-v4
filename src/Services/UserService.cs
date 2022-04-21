using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unico.Admin.Api.Models;

namespace Unico.Admin.Api.Services
{
    public class UserService : IUserService
    {
        // TODO FIX DI, but no idea how with azure function v4
        private readonly HttpClient httpClient = new HttpClient(); 
        
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }
        
        // constructor for unit test
        public UserService(ILogger<UserService> logger, HttpClient httpClientOverwrite)
        {
            _logger = logger;
            httpClient = httpClientOverwrite;
        }

        public async Task<UserDto> CreateUser(UserDto input)
        {
            _logger.LogInformation("Trigger PS Remote to create user with SAM: " + input.samAccountName);
            var json = JsonConvert.SerializeObject(input);
            var postPayload = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string createUserApiUrl = Environment.GetEnvironmentVariable("FunPS-CreateUser");
                if (createUserApiUrl == null)
                {
                    _logger.LogCritical("Env missing, please configure 'FunPS-CreateUser'");
                    throw new Exception("Enviroment configuration incorrect");
                }
                var response = await httpClient.PostAsync(createUserApiUrl, postPayload);

                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;

                _logger.LogInformation("User sucessfully created " + input.samAccountName);

                return input;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                var errorMsg = "User already exists";
                _logger.LogInformation("User already exists", ex);
                throw new HttpRequestException(errorMsg, ex, HttpStatusCode.Conflict);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden || ex.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                var errorMsg = "Backend not available";
                _logger.LogError(errorMsg, ex);
                throw new HttpRequestException(errorMsg, ex, HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                var errorMsg = "Unable to create user";
                _logger.LogError(errorMsg, ex);
                throw new HttpRequestException(errorMsg, ex, HttpStatusCode.InternalServerError);
            }
        }
    }
}
