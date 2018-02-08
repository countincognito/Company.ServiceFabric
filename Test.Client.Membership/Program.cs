using System;

namespace Test.Client.Membership
{
    class Program
    {
        static void Main()
        {
            try
            {
                Client.Test();
                Console.WriteLine("Enter to close");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
