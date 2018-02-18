using Company.Access.User.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Company.Access.User.Impl
{
    public class UserAccess
        : IUserAccess
    {
        private readonly ILogger<IUserAccess> _Logger;

        public UserAccess(ILogger<IUserAccess> logger)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> CheckUserExistsAsync(string name)
        {
            _Logger.LogInformation($@"{nameof(CheckUserExistsAsync)} Invoked");
            _Logger.LogInformation($@"{nameof(CheckUserExistsAsync)} {name}");
            return Task.FromResult(false);
        }

        public Task<string> CreateUserAsync(string name)
        {
            _Logger.LogInformation($@"{nameof(CreateUserAsync)} Invoked");
            _Logger.LogInformation($@"{nameof(CreateUserAsync)} {name}");
            return Task.FromResult($"\r\n        UserAccess.CreateUserAsync -> {name} -> {DateTime.UtcNow}");
        }
    }
}
