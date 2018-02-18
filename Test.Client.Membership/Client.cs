using Company.Common.Data;
using Company.Manager.Membership.Interface;
using Company.ServiceFabric.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.Client.Membership
{
    public class Client
    {
        static void Test_QueryMembershipConcurrent()
        {
            try
            {
                Console.WriteLine("\r\nConcurrent...");

                var proxy = GetProxy();
                var tasks = new List<Task<string>>();
                for (int i = 0; i < 10; i++)
                {
                    Task<string> response = proxy.RegisterMemberAsync(GetRegisterRequest(Guid.NewGuid().ToString()));
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

        static async Task Test_QueryMembershipSequential()
        {
            try
            {
                Console.WriteLine("\r\nSequential...");

                var proxy = GetProxy();
                for (int i = 0; i < 10; i++)
                {
                    string response = await proxy.RegisterMemberAsync(GetRegisterRequest(Guid.NewGuid().ToString()));
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
            Test_QueryMembershipConcurrent();
            Test_QueryMembershipSequential().Wait();

            Console.ReadKey();
        }

        public static IMembershipManager GetProxy()
        {
            return AuditableProxy.ForMicroservice<IMembershipManager>();
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
