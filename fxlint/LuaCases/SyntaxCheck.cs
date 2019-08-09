using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Text;

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
            using (Lua lua = new Lua())
            {
                dynamic env = lua.CreateEnvironment();
                try
                {
                    env.dochunk(code, "test.lua");
                }
                catch (InvalidOperationException)
                {
                }
                catch (LuaParseException ex)
                {
                    errors.Add("Syntax error: " + ex.Message);
                }
                catch (LuaRuntimeException)
                {
                }
            }
            return errors.ToArray();
        }
    }
}
