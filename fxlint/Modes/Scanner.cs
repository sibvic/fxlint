using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fxlint
{
    class Scanner
    {
        ScannerOptions _options;
        LuaLint _lua;
        public Task<int> StartAsync(ScannerOptions options)
        {
            _options = options;
            _lua = new LuaLint(options.IndicoreRootPath, options.Excluded);

            IEnumerable<string> files;
            if (File.Exists(options.Path))
            {
                var list = new List<string>();
                files = list;
                list.Add(options.Path);
            }
            else
            {
                files = Directory
                    .GetFiles(options.Path, "*.*", SearchOption.AllDirectories)
                    .Where(file => IsValidExtension(Path.GetExtension(file)))
                    .ToList();
            }

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
            File.WriteAllLines(options.OutputFile, log.ToArray());
            return Task.FromResult(0);
        }

        private static void PrintWarnings(string file, string[] warnings)
        {
            Console.WriteLine("\n=== " + file + " ===");
            foreach (var warning in warnings)
            {
                Console.WriteLine("  - " + warning);
            }
        }

        private string[] GetWarnings(string file)
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
                    return _lua.GetWarnings(code, fileName);
                case ".MQ4":
                    return MQL4Lint.GetWarnings(code, fileName);
            }
            return new string[0];
        }

        private bool IsValidExtension(string extension)
        {
            return _options.Extensions.Contains(extension.ToLower());
        }
        
    }
}
