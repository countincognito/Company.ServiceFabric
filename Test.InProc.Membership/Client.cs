using Company.Access.User.Impl;
using Company.Access.User.Interface;
using Company.Common.Data;
using Company.Engine.Registration.Impl;
using Company.Engine.Registration.Interface;
using Company.Manager.Membership.Impl;
using Company.Manager.Membership.Interface;
using Company.Utility.Audit;
using Company.Utility.Logging.Serilog;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Test.InProc.Membership
{
    public class Client
    {
        static async Task Test_QueryMembership()
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.WithAuditContext()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
            Log.Logger = serilog;

            try
            {
                ConsoleKeyInfo? input = null;
                while (!input.HasValue)
                {
                    var proxy = GetProxy(serilog);
                    string response = await proxy.RegisterMemberAsync(
                        new RegisterRequest
                        {
                            Name = "Bob",
                            Email = "example@example.com",
                            Password = "Random",
                            DateOfBirth = new DateTime(1970, 1, 1),
                        });
                    Console.WriteLine(response);
                    Task.Factory.StartNew(() => input = Console.ReadKey()).Wait(TimeSpan.FromSeconds(0.1));
                }
                Console.WriteLine("\r\nFinished...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test Exception: " + ex.Message);
            }
            Log.CloseAndFlush();
        }

        public static void Test()
        {
            Test_QueryMembership().Wait();
        }

        public static IMembershipManager GetProxy(ILogger serilog)
        {
            var userAccessLogger = serilog.ToGeneric<IUserAccess>();
            var userAccess = AuditableWrapper.Create(new UserAccess(userAccessLogger), userAccessLogger);

            var registrationEngineLogger = serilog.ToGeneric<IRegistrationEngine>();
            var registrationEngine = AuditableWrapper.Create(new RegistrationEngine(userAccess, registrationEngineLogger), registrationEngineLogger);

            var membershipManagerLogger = serilog.ToGeneric<IMembershipManager>();
            var membershipManager = AuditableWrapper.Create(new MembershipManager(registrationEngine, membershipManagerLogger), membershipManagerLogger);

            return membershipManager;
        }
    }
}
