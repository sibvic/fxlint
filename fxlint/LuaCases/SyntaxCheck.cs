using System.Collections.Generic;
using ProfitRobots.FXTS2LuaExecuter;

namespace fxlint.LuaCases
{
    class SyntaxCheck : ILintCheck
    {
        private readonly string _indicoreRootPath;
        public SyntaxCheck(string indicoreRootPath)
        {
            _indicoreRootPath = indicoreRootPath ?? "";
        }

        public string Fix(string code, string name)
        {
            return code;
        }

        public string[] GetWarnings(string code, string name)
        {
            List<string> errors = new List<string>();
            try
            {
                var profile = ExtensionProfileLoader.LoadFromCode(code, name, _indicoreRootPath);
                profile.Init();
            }
            catch (FXTS2ExtensionException ex)
            {
                errors.Add("Syntax error: " + ex.Message);
            }
            return errors.ToArray();
        }

        public string Id => "SyntaxCheck";
    }
}
