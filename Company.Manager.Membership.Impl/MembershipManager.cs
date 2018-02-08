using Company.Common.Data;
using Company.Engine.Registration.Interface;
using Company.Manager.Membership.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Company.Manager.Membership.Impl
{
    public class MembershipManager
        : IMembershipManager
    {
        private readonly IRegistrationEngine _RegistrationEngine;
        private readonly ILogger<IMembershipManager> _Logger;

        public MembershipManager(
            IRegistrationEngine registrationEngine,
            ILogger<IMembershipManager> logger)
        {
            _RegistrationEngine = registrationEngine ?? throw new ArgumentNullException(nameof(registrationEngine));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> RegisterMemberAsync(RegisterRequest request)
        {
            _Logger.LogInformation($"{nameof(RegisterMemberAsync)} invoked");
            string result = await _RegistrationEngine.RegisterMemberAsync(request);
            return $"\r\nMembershipManager.RegisterMemberAsync -> {result}";
        }
    }
}
