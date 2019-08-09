using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace fxlint.MQL4Cases
{
    public class NoCustomIndicatorCheck : ILintCheck
    {
        Regex _usePattern = new Regex("iCustom\\([^,]+,[^,]+,([^,]+)");
        Regex _initFunctionPattern = new Regex("(int init\\(\\)[\r\n\t ]*{[\r\n\t ]*)");
        Regex _initFunctionPattern2 = new Regex("(int OnInit\\(\\)[\r\n\t ]*{[\r\n\t ]*)");

        string FormatCheck(string name)
        {
            return "temp = iCustom(NULL, 0, " + name + ", 0, 0);\n"
                + "   if (GetLastError() == ERR_INDICATOR_CANNOT_LOAD)\n"
                + "   {\n"
                + "       Alert(\"Please, install the '" + name.Trim('"') + "' indicator\");\n"
                + "       return INIT_FAILED;\n"
                + "   }\n   ";
        }

        int GetInitFunctionPosition(string code)
        {
            var initMatch = _initFunctionPattern.Match(code);
            if (initMatch.Success)
            {
                var init = initMatch.Groups[1].Value;
                return code.IndexOf(init) + init.Length;
            }
            initMatch = _initFunctionPattern2.Match(code);
            if (initMatch.Success)
            {
                var init = initMatch.Groups[1].Value;
                return code.IndexOf(init) + init.Length;
            }
            return -1;
        }

        public string Fix(string code, string name)
        {
            var checks = GetMissingChecks(code, name);
            if (!checks.Any())
                return code;
            var position = GetInitFunctionPosition(code);
            if (position == -1)
                return code;

            var checksCode = string.Join("    \n", checks.Select(c => FormatCheck(c)));
            return code.Insert(position, "    double " + checksCode);
        }

        public string[] GetWarnings(string code, string name)
        {
            return GetMissingChecks(code, name)
                .Select(mc => "No check for existance of custom indicator: " + mc)
                .ToArray();
        }

        List<string> GetMissingChecks(string code, string name)
        {
            var added = new List<string>();
            var matches = _usePattern.Matches(code);
            foreach (Match match in matches)
            {
                var indicatorName = match.Groups[1].Value.Trim();
                if (indicatorName.Trim('"') == name || indicatorName == "indicatorFileName" || indicatorName == "IndicatorName")
                    continue;

                Regex _checkPattern = new Regex("temp ?= ?iCustom\\([^,]+,[^,]+, ?" + indicatorName + ",[^)]+\\);[\r\n\t ]*if ?\\(GetLastError");
                if (!_checkPattern.IsMatch(code) && !added.Contains(indicatorName))
                    added.Add(indicatorName);
            }
            return added;
        }
    }
}
