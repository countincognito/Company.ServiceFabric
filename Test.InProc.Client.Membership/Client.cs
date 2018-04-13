﻿using Company.Access.User.Impl;
using Company.Access.User.Interface;
using Company.Common.Data;
using Company.Engine.Registration.Impl;
using Company.Engine.Registration.Interface;
using Company.Manager.Membership.Impl;
using Company.Manager.Membership.Interface;
using Company.Utility;
using Company.Utility.Logging.Serilog;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.InProc.Membership
{
    public class Client
    {
        static void Test_QueryMembershipConcurrent(ILogger serilog)
        {
            try
            {
                Console.WriteLine("\r\nConcurrent...");

                var proxy = GetProxy(serilog);
                var tasks = new List<Task<string>>();
                for (int i = 0; i < 10; i++)
                {
                    // THIS IS NECESSARY FOR INPROC CALLS.
                    TrackingContext.NewCurrent();

                    Task<string> response = proxy.RegisterMemberAsync(GetRegisterRequest(TrackingContext.Current.CallChainId.ToString()));
                    tasks.Add(response);
                }
                Task.WaitAll(tasks.ToArray());

                foreach (var item in tasks)
                {
                    Console.WriteLine(item.Result);
                }

                Console.WriteLine("\r\nFinished...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test Exception: " + ex.Message);
            }
        }

        static async Task Test_QueryMembershipSequential(ILogger serilog)
        {
            try
            {
                Console.WriteLine("\r\nSequential...");

                var proxy = GetProxy(serilog);
                for (int i = 0; i < 10; i++)
                {
                    // THIS IS NECESSARY FOR INPROC CALLS.
                    TrackingContext.NewCurrent();

                    string response = await proxy.RegisterMemberAsync(GetRegisterRequest(TrackingContext.Current.CallChainId.ToString()));
                    Console.WriteLine(response);
                }

                Console.WriteLine("\r\nFinished...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test Exception: " + ex.Message);
            }
        }

        public static void Test()
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromTrackingContext()
                .Enrich.FromLoggingProxy()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
            Log.Logger = serilog;

            Test_QueryMembershipConcurrent(serilog);
            Test_QueryMembershipSequential(serilog).Wait();

            Console.ReadKey();
            Log.CloseAndFlush();
        }

        public static IMembershipManager GetProxy(ILogger serilog)
        {
            var userAccess = LoggingProxy.Create<IUserAccess>(new UserAccess(serilog), serilog);
            var registrationEngine = LoggingProxy.Create<IRegistrationEngine>(new RegistrationEngine(userAccess, serilog), serilog);
            var membershipManager = LoggingProxy.Create<IMembershipManager>(new MembershipManager(registrationEngine, serilog), serilog);
            return membershipManager;
        }

        public static RegisterRequest GetRegisterRequest(string name)
        {
            return new RegisterRequest
            {
                Name = name,
                Email = "example@example.com",
                Password = "Random",
                DateOfBirth = new DateTime(1970, 1, 1),
            };
        }
    }
}
