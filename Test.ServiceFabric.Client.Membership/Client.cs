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
        public static void Test()
        {
            Test_QueryMembershipConcurrent();
            Test_QueryMembershipSequential().Wait();
            //Test_QueryMembershipSequentialInfinite().Wait();

            Console.ReadKey();
        }

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

        static async Task Test_QueryMembershipSequentialInfinite()
        {
            try
            {
                Console.WriteLine("\r\nInfinite...");

                var proxy = GetProxy();
                ConsoleKeyInfo? input = null;
                while (!input.HasValue)
                {
                    string response = await proxy.RegisterMemberAsync(GetRegisterRequest(Guid.NewGuid().ToString()));
                    Console.WriteLine(response);
                    Task.Factory.StartNew(() => input = Console.ReadKey()).Wait(TimeSpan.FromSeconds(0.5));
                }

                Console.WriteLine("\r\nFinished...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test Exception: " + ex.Message);
            }
        }

        static IMembershipManager GetProxy()
        {
            return TrackingProxy.ForMicroservice<IMembershipManager>();
        }

        static RegisterRequest GetRegisterRequest(string name)
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
