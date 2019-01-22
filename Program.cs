using System;
using System.Collections.Generic;
using System.IO;

namespace fxlint
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(".", "*.lua", SearchOption.AllDirectories);
            List<string> log = new List<string>();
            foreach (var file in files)
            {
                string[] warnings = GetWarnings(file);
                if (warnings.Length > 0)
                {
                    PrintWarnings(file, warnings);
                    log.Add("\n=== " + file + " ===");
                    log.AddRange(warnings);
                }
            }
            File.WriteAllLines("fxlint_log.txt", log.ToArray());
        }

        private static void PrintWarnings(string file, string[] warnings)
        {
            Console.WriteLine("\n=== " + file + " ===");
            foreach (var warning in warnings)
            {
                Console.WriteLine("  - " + warning);
            }
        }

        private static string[] GetWarnings(string file)
        {
            string code;
            try
            {
                code = File.ReadAllText(file);
            }
            catch (Exception ex)
            {
                return new string[] { ex.Message };
            }
            switch (Path.GetExtension(file).ToUpper())
            {
                case ".LUA":
                    return LuaLint.GetWarnings(code);
            }
            return new string[0];
        }
    }
}
