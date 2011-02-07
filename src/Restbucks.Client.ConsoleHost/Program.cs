using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restbucks.Client.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = new Start();
            start.Go();
            Console.ReadLine();
        }
    }
}
