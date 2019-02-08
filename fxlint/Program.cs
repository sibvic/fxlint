using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fxlint
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = Options.Parse(args);

            IEnumerable<string> files = Directory.GetFiles(".", "*.lua", SearchOption.AllDirectories);
            if (options.File != null)
                files = files.Where(f => f == options.File);
            if (options.Mode == Mode.Fix)
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
            try
            {

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
            catch (Exception)
            {
                Console.WriteLine(file);
                throw;
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
