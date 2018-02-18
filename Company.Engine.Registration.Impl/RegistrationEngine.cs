using Company.Access.User.Interface;
using Company.Common.Data;
using Company.Engine.Registration.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Company.Engine.Registration.Impl
{
    public class RegistrationEngine
        : IRegistrationEngine
    {
        private readonly IUserAccess _UserAccess;
        private readonly ILogger<IRegistrationEngine> _Logger;

        public RegistrationEngine(
            IUserAccess userAccess,
            ILogger<IRegistrationEngine> logger)
        {
            _UserAccess = userAccess ?? throw new ArgumentNullException(nameof(userAccess));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> RegisterMemberAsync(RegisterRequest request)
        {
            _Logger.LogInformation($"{nameof(RegisterMemberAsync)} Invoked");
            _Logger.LogInformation($"{nameof(RegisterMemberAsync)} {request.Name}");

            // Check if user already exists or not.
            bool userExists = await _UserAccess.CheckUserExistsAsync(request.Name);

            string result = "Failed";
            if (!userExists)
            {
                result = await _UserAccess.CreateUserAsync(request.Name);
            }

            // Do other stuff.....

            return $"\r\n    RegistrationEngine.RegisterMemberAsync -> {result}";
        }
    }
}
