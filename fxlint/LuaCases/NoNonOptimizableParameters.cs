using System;
using System.Collections.Generic;
using System.Text;

namespace fxlint.LuaCases
{
    public class NoNonOptimizableParameters : ILintCheck
    {
        public string Fix(string code, string name)
        {
            if (!NeedNonOptimizableParameters(code))
                return code;

            if (code.Contains("\"NonOptimizableParameters\""))
                return code;

            var namePos = code.IndexOf("strategy:name(");
            if (namePos < 0)
                return code;
            var nextLinePos = code.IndexOf("\n", namePos);
            if (nextLinePos < 0)
                return code;
            return code.Insert(nextLinePos + 1, "	strategy:setTag(\"NonOptimizableParameters\", \"Email,SendEmail,SoundFile,RecurrentSound,PlaySound,ShowAlert\");\n");
        }

        public string[] GetWarnings(string code, string name)
        {
            if (!code.Contains("strategy:name("))
                return new string[0];

            if (!NeedNonOptimizableParameters(code))
                return new string[0];

            if (code.Contains("\"NonOptimizableParameters\""))
                return new string[0];

            return new string[] { "No NonOptimizableParameters" };
        }

        string[] _nonOptimizableParameters = {
            ":addBoolean(\"ShowAlert\"",
            ":addBoolean(\"PlaySound\"",
            ":addBoolean(\"RecurrentSound\"",
            ":addBoolean(\"SendEmail\"",
            ":addFile(\"SoundFile\"",
            ":addString(\"Email\"",
        };

        private bool NeedNonOptimizableParameters(string code)
        {
            foreach (var param in _nonOptimizableParameters)
            {
                if (code.Contains(param))
                    return true;
            }
            return false;
        }

        public string Id => "NoNonOptimizableParameters";
    }
}
