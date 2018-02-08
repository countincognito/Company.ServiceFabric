using Company.Common.Data;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Company.Manager.Membership.Interface
{
    public interface IMembershipManager
        : IService
    {
        Task<string> RegisterMemberAsync(RegisterRequest request);
    }
}
