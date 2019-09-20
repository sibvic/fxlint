using System.Collections.Generic;
using ProfitRobots.FXTS2LuaExecuter;

namespace fxlint.LuaCases
{
    class SyntaxCheck : ILintCheck
    {
        public string Fix(string code, string name)
        {
            return code;
        }

        public string[] GetWarnings(string code, string name)
        {
            List<string> errors = new List<string>();
            try
            {
                ExtensionProfileLoader.LoadFromCode(code, name, "");
            }
            catch (FXTS2ExtensionException ex)
            {
                errors.Add("Syntax error: " + ex.Message);
            }
            return errors.ToArray();
        }
    }
}
