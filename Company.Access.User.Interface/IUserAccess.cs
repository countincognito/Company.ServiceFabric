using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Company.Access.User.Interface
{
    public interface IUserAccess
        : IService
    {
        Task<bool> CheckUserExistsAsync(string email);

        Task<string> CreateUserAsync(string email);
    }
}
