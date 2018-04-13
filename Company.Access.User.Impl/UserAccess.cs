using Company.Access.User.Interface;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Company.Access.User.Impl
{
    public class UserAccess
        : IUserAccess
    {
        private readonly ILogger _Logger;

        public UserAccess(ILogger logger)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> CheckUserExistsAsync(string name)
        {
            _Logger.Information($@"{nameof(CheckUserExistsAsync)} Invoked");
            _Logger.Information($@"{nameof(CheckUserExistsAsync)} {name}");
            return Task.FromResult(false);
        }

        public Task<string> CreateUserAsync(string name)
        {
            _Logger.Information($@"{nameof(CreateUserAsync)} Invoked");
            _Logger.Information($@"{nameof(CreateUserAsync)} {name}");
            return Task.FromResult($"\r\n        UserAccess.CreateUserAsync -> {name} -> {DateTime.UtcNow}");
        }
    }
}
