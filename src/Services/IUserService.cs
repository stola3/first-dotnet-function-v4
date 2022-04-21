using System.Threading.Tasks;
using Unico.Admin.Api.Models;

namespace Unico.Admin.Api.Services
{
    public interface IUserService
    {
        public Task<UserDto> CreateUser(UserDto input);
    }
}
