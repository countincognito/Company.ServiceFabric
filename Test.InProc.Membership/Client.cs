using Company.Access.User.Impl;
using Company.Access.User.Interface;
using Company.Common.Data;
using Company.Engine.Registration.Impl;
using Company.Engine.Registration.Interface;
using Company.Manager.Membership.Impl;
using Company.Manager.Membership.Interface;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading.Tasks;

namespace Test.InProc.Membership
{
    public class Client
    {
        static async Task Test_QueryMembership()
        {
            try
            {
                ConsoleKeyInfo? input = null;
                while (!input.HasValue)
                {
                    var proxy = GetProxy();
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
        }

        public static void Test()
        {
            Test_QueryMembership().Wait();
        }

        public static IMembershipManager GetProxy()
        {
            return new MembershipManager(
                new RegistrationEngine(
                    new UserAccess(new NullLogger<IUserAccess>()),
                    new NullLogger<IRegistrationEngine>()),
                new NullLogger<IMembershipManager>());
        }
    }
}
