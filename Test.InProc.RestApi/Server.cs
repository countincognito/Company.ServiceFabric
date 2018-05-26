using Company.Access.User.Impl;
using Company.Access.User.Interface;
using Company.Api.Rest.Impl;
using Company.Engine.Registration.Impl;
using Company.Engine.Registration.Interface;
using Company.Manager.Membership.Impl;
using Company.Manager.Membership.Interface;
using Destructurama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using System.Net;
using Zametek.Utility.Logging;

namespace Test.InProc.RestApi
{
    public class Server
    {
        public static void Test()
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .Destructure.UsingAttributes()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
            Log.Logger = serilog;

            BuildWebHost(serilog).Run();
        }

        public static IWebHost BuildWebHost(ILogger serilog)
        {
            var userAccess = LogProxy.Create<IUserAccess>(new UserAccess(serilog), serilog, LogType.All);
            var registrationEngine = LogProxy.Create<IRegistrationEngine>(new RegistrationEngine(userAccess, serilog), serilog, LogType.All);
            var membershipManager = LogProxy.Create<IMembershipManager>(new MembershipManager(registrationEngine, serilog), serilog, LogType.All);

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