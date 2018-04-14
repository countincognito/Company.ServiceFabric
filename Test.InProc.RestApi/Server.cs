using Company.Access.User.Impl;
using Company.Access.User.Interface;
using Company.Api.Rest.Impl;
using Company.Engine.Registration.Impl;
using Company.Engine.Registration.Interface;
using Company.Manager.Membership.Impl;
using Company.Manager.Membership.Interface;
using Company.Utility.Logging.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using System.Net;

namespace Test.InProc.RestApi
{
    public class Server
    {
        public static void Test()
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLoggingProxy()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
            Log.Logger = serilog;

            BuildWebHost(serilog).Run();
        }

        public static IWebHost BuildWebHost(ILogger serilog)
        {
            //var userAccess = new UserAccess(new NullLogger<IUserAccess>());

            //var patientAccess = new PatientAccess(new NullLogger<IPatientAccess>());

            //var registrationEngine = new RegistrationEngine(userAccess, patientAccess, new NullLogger<IRegistrationEngine>());

            //var membershipManager = new MembershipManager(registrationEngine, new NullLogger<IMembershipManager>());

            var userAccess = LoggingProxy.Create<IUserAccess>(new UserAccess(serilog), serilog);
            var registrationEngine = LoggingProxy.Create<IRegistrationEngine>(new RegistrationEngine(userAccess, serilog), serilog);
            var membershipManager = LoggingProxy.Create<IMembershipManager>(new MembershipManager(registrationEngine, serilog), serilog);

            var restApiLogger = serilog;

            return new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 80);
                })
                //WebHost.CreateDefaultBuilder()
                .ConfigureServices(
                    services => services
                        .AddSingleton(membershipManager)
                        .AddSingleton(restApiLogger))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
        }
    }
}