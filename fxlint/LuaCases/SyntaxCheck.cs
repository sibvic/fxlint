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

        class Core
        {
            public string app_path()
            {
                return @"C:\Program Files (x86)\Candleworks\FXTS2";
            }
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
