
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unico.Admin.Api.Services;

[assembly: FunctionsStartup(typeof(Unico.Admin.Api.Startup))]

namespace Unico.Admin.Api
{
    public class Startup : FunctionsStartup
    {
        private ILoggerFactory _loggerFactory;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // TODO use DI to add HttpClient... But how? 
            // v3 HowTo does not work: https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection#feedback

            _loggerFactory = new LoggerFactory();

            builder.Services.AddSingleton<IUserService>((s) => {
                return new UserService(_loggerFactory.CreateLogger<UserService>());
            });

        }
    }
}