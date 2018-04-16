using Company.Common.Data;
using Company.Engine.Registration.Interface;
using Company.Manager.Membership.Interface;
using Company.Utility.Logging.Serilog;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Company.Manager.Membership.Impl
{
    public class MembershipManager
        : IMembershipManager
    {
        private readonly IRegistrationEngine _RegistrationEngine;
        private readonly ILogger _Logger;

        public MembershipManager(
            IRegistrationEngine registrationEngine,
            ILogger logger)
        {
            _RegistrationEngine = registrationEngine ?? throw new ArgumentNullException(nameof(registrationEngine));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [NoDiagnosticLogging]
        public async Task<string> RegisterMemberAsync(RegisterRequest request)
        {
            _Logger.Information($"{nameof(RegisterMemberAsync)} Invoked");
            _Logger.Information($"{nameof(RegisterMemberAsync)} {request.Name}");
            string result = await _RegistrationEngine.RegisterMemberAsync(request).ConfigureAwait(false);
            return $"\r\nMembershipManager.RegisterMemberAsync -> {result}";
        }
    }
}
