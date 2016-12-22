using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSNVS
{
    class Program
    {
        static void Main(string[] args)
        {
            Int64 hex = Int64.Parse("A2791717", System.Globalization.NumberStyles.HexNumber);
            string key = KeyGeneration.PKV_MakeKey(hex);
            Console.WriteLine(key);
            Console.WriteLine(KeyVerification.PKV_CheckKeyChecksum(key));
            Console.WriteLine(KeyVerification.PKV_CheckKey(key));

            string base64string = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(key));
            Console.WriteLine(base64string);
            string normalstring = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64string));
            Console.WriteLine(normalstring);
            Console.ReadKey();
        }
    }
}
