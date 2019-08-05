using fxlint.MQL4Cases;
using System;
using System.Collections.Generic;
using System.Text;

namespace fxlint
{
    class MQL4Lint
    {
        static List<ILintCheck> _cases = new List<ILintCheck>();
        static MQL4Lint()
        {
            _cases.Add(new NoCustomIndicatorCheck());
        }

        public static string[] GetWarnings(string code, string name)
        {
            List<string> warnings = new List<string>();
            foreach (var lintCase in _cases)
            {
                var lintWarnings = lintCase.GetWarnings(code, name);
                warnings.AddRange(lintWarnings);
            }
            return warnings.ToArray();
        }

        internal static string FixCode(string code)
        {
            var fixedCode = code;
            foreach (var lintCase in _cases)
            {
                fixedCode = lintCase.Fix(fixedCode);
            }
            return fixedCode;
        }
    }
}
