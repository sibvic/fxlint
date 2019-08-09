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

            IEnumerable<string> files = Directory
                .GetFiles(options.Path, "*.*", SearchOption.AllDirectories)
                .Where(file => IsValidExtension(options, Path.GetExtension(file)))
                .ToList();
            if (options.File != null)
                files = files.Where(f => Path.GetFileName(f) == options.File);
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
                File.WriteAllLines(Path.Combine(options.Path, "fxlint_log.txt"), log.ToArray());
            }
        }

        private static bool IsValidExtension(Options options, string extension)
        {
            return options.Extensions.Contains(extension.ToLower());
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
                var fileName = Path.GetFileNameWithoutExtension(file);
                switch (Path.GetExtension(file).ToUpper())
                {
                    case ".LUA":
                        {
                            var newCode = LuaLint.FixCode(code, fileName);
                            if (newCode != code)
                                File.WriteAllText(file, newCode);
                        }
                        break;
                    case ".MQ4":
                        {
                            var newCode = MQL4Lint.FixCode(code, fileName);
                            if (newCode != code)
                                File.WriteAllText(file, newCode);
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
            var fileName = Path.GetFileNameWithoutExtension(file);
            switch (Path.GetExtension(file).ToUpper())
            {
                case ".LUA":
                    return LuaLint.GetWarnings(code, fileName);
                case ".MQ4":
                    return MQL4Lint.GetWarnings(code, fileName);
            }
            return new string[0];
        }
    }
}
