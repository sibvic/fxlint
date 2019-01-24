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
            if (args[0] == "--fix")
            {
                foreach (var file in files)
                {
                    FixFile(file);
                }
            }
            else
            {
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
        }

        private static void PrintWarnings(string file, string[] warnings)
        {
            Console.WriteLine("\n=== " + file + " ===");
            foreach (var warning in warnings)
            {
                Console.WriteLine("  - " + warning);
            }
        }

        public static void FixFile(string file)
        {
            string code;
            try
            {
                code = File.ReadAllText(file);
            }
            catch (Exception)
            {
                return;
            }
            switch (Path.GetExtension(file).ToUpper())
            {
                case ".LUA":
                    {
                        var newCode = LuaLint.FixCode(code);
                        if (newCode != code)
                        {
                            File.WriteAllText(file, newCode);
                        }
                    }
                    break;
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
