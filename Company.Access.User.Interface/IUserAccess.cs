using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Company.Access.User.Interface
{
    public interface IUserAccess
        : IService
    {
        Task<bool> CheckUserExistsAsync(string name);

        Task<string> CreateUserAsync(string name);
    }
}
