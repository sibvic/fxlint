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
            string[] missingIndicatorChecks = GetMissingIndicatorChecks(code);
            foreach (var missingIndicatorCheck in missingIndicatorChecks)
            {
                warnings.Add("Missing indicator assert for " + missingIndicatorCheck);
            }
            return warnings.ToArray();
        }

        static Regex indicatorCreatePattern = new Regex("indicators:create\\((?<indiName>[^,]+),");
        private static string[] GetMissingIndicatorChecks(string code)
        {
            List<string> missingChecks = new List<string>();
            var matches = indicatorCreatePattern.Matches(code);
            foreach (Match match in matches)
            {
                var indicatorName = match.Groups["indiName"].Value;
                if (IsStandardIndicator(indicatorName.Trim().Trim('"')))
                    continue;
                Regex indicatorAssertPattern = new Regex("assert\\(core\\.indicators:findIndicator\\(" + indicatorName + "\\) ~= nil");
                if (!indicatorAssertPattern.IsMatch(code))
                    missingChecks.Add(indicatorName);
            }
            return missingChecks.ToArray();
        }

        private static bool IsStandardIndicator(string indicatorName)
        {
            switch (indicatorName)
            {
                case "MVA":
                    return true;
            }
            return false;
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