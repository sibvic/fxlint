using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace fxlint.MQL4Cases
{
    public class NoCustomIndicatorCheck : ILintCheck
    {
        Regex _usePattern = new Regex("iCustom\\([^,]+,[^,]+,([^,]+)");
        
        public string Fix(string code, string name)
        {
            var checks = GetMissingChecks(code, name);
            
            return code;
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
            List<string> warnings = new List<string>();
            var matches = _usePattern.Matches(code);
            foreach (Match match in matches)
            {
                if (match.Groups[1].Value == name)
                    continue;

                Regex _checkPattern = new Regex("temp ?= ?iCustom([^,]+,[^,]+, ?" + match.Groups[1].Value + ",[^,]+,[^,]+);[\r\n\t ]*if ?\\(GetLastError");
                if (!_checkPattern.IsMatch(code) && !added.Contains(match.Groups[1].Value))
                    added.Add(match.Groups[1].Value);
            }
            return added;
        }
    }
}
