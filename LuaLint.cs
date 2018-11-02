using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace fxlint
{
    class LuaLint
    {
        public static string[] GetWarnings(string code)
        {
            List<string> warnings = new List<string>();
            if (ContainsOldParseTime(code))
                warnings.Add("Old version of ParseTime");
            if (ContainsOldTradingTimeCheck(code))
                warnings.Add("Old version of trading time check (not via InRange)");
            return warnings.ToArray();
        }

        static Regex oldTradingTimeCheckPattern = new Regex("if not\\(now ?>= ?OpenTime(\n\r)?[^a]*and now ?<= ?CloseTime\\)");
        private static bool ContainsOldTradingTimeCheck(string code)
        {
            return oldTradingTimeCheckPattern.IsMatch(code);
        }

        static Regex oldParseTimePattern = new Regex("local Pos ?= ?string\\.find\\(time, ?\":\"\\);[ ]*\r\n[ ]*local");
        private static bool ContainsOldParseTime(string code)
        {
            return oldParseTimePattern.IsMatch(code);
        }
    }
}