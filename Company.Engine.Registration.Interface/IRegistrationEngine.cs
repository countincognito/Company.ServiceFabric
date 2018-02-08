using Company.Common.Data;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Company.Engine.Registration.Interface
{
    public interface IRegistrationEngine
        : IService
    {
        Task<string> RegisterMemberAsync(RegisterRequest request);
    }
}
