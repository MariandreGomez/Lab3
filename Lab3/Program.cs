using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lab3.Operaciones;

namespace Lab3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Json Data");
            Operaciones operaciones1 = new Operaciones();
            operaciones1.CSV();
            operaciones1.EncriptarArchivos();
            operaciones1.Decifrar();
            Console.ReadKey();
        }
    }
}
