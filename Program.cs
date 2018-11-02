using System;
using System.IO;

namespace fxlint
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(".", "*.lua", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string[] warnings = GetWarnings(file);
                if (warnings.Length > 0)
                    PrintWarnings(file, warnings);
            }
        }

        private static void PrintWarnings(string file, string[] warnings)
        {
            Console.WriteLine("\n====\n" + file);
            foreach (var warning in warnings)
            {
                Console.WriteLine("  - " + warning);
            }
        }

        private static string[] GetWarnings(string file)
        {
            string code = File.ReadAllText(file);
            switch (Path.GetExtension(file).ToUpper())
            {
                case ".LUA":
                    return LuaLint.GetWarnings(code);
            }
            return new string[0];
        }
    }
}
