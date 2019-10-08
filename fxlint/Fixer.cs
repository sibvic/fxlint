using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fxlint
{
    class Fixer
    {
        FixOptions _options;
        LuaLint _lua;

        public Task<int> StartAsync(FixOptions options)
        {
            _options = options;
            _lua = new LuaLint(_options.IndicoreRootPath, new List<string>());

            IEnumerable<string> files = Directory
                .GetFiles(_options.Path, "*.*", SearchOption.AllDirectories)
                .Where(file => IsValidExtension(Path.GetExtension(file)))
                .ToList();
            if (_options.File != null)
                files = files.Where(f => Path.GetFileName(f) == _options.File);
            foreach (var file in files)
            {
                FixFile(file);
            }
            return Task.FromResult(1);
        }

        void FixFile(string file)
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
                            var newCode = _lua.FixCode(code, fileName);
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

        private bool IsValidExtension(string extension)
        {
            return _options.Extensions.Contains(extension.ToLower());
        }
    }
}
