using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husrum_Fastigheter_AB
{
    class Program
    {
        static void Main(string[] args)
        {
            Database info = new Database();
            info.Create_Database();

            Console.WriteLine("Done");
            Console.ReadKey();
            
        }
    }
}