using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fxlint
{
    class List
    {
        ListOptions _options;
        LuaLint _lua;
        public Task<int> StartAsync(ListOptions options)
        {
            _options = options;
            _lua = new LuaLint("", new List<string>());

            foreach (var check in _lua.Checks)
            {
                Console.WriteLine(check);
            }

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
    }
}
