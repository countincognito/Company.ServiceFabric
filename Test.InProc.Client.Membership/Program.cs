using System;

namespace Test.InProc.Membership
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
