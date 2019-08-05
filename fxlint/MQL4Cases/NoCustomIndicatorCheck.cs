using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace fxlint.MQL4Cases
{
    public class NoCustomIndicatorCheck : ILintCheck
    {
        Regex _usePattern = new Regex("iCustom\\([^,]+,[^,]+,([^,]+)");
        
        public string Fix(string code)
        {
            throw new NotImplementedException();
        }

        public string[] GetWarnings(string code, string name)
        {
            var added = new List<string>();

            List<string> warnings = new List<string>();
            var matches = _usePattern.Matches(code);
            foreach (Match match in matches)
            {
                if (match.Groups[1].Value == name)
                    continue;

                Regex _checkPattern = new Regex("temp ?= ?iCustom([^,]+,[^,]+, ?" + match.Groups[1].Value + ",[^,]+0,[^,]+0);\n[^i]*if ?(GetLastError() ?== ?ERR_INDICATOR_CANNOT_LOAD)");
                if (!_checkPattern.IsMatch(code) && !added.Contains(match.Groups[1].Value))
                {
                    warnings.Add("No check for existance of custom indicator: " + match.Groups[1].Value);
                    added.Add(match.Groups[1].Value);
                }
            }
            return warnings.ToArray();
        }
    }
}
