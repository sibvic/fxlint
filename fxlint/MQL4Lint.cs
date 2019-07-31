using System;
using System.Collections.Generic;
using System.Text;

namespace fxlint
{
    class MQL4Lint
    {
        public static string[] GetWarnings(string code)
        {
            List<string> warnings = new List<string>();
            //foreach (var lintCase in _cases)
            //{
            //    var lintWarnings = lintCase.GetWarnings(code);
            //    warnings.AddRange(lintWarnings);
            //}
            return warnings.ToArray();
        }

        internal static string FixCode(string code)
        {
            var fixedCode = code;
            //foreach (var lintCase in _cases)
            //{
            //    fixedCode = lintCase.Fix(fixedCode);
            //}
            return fixedCode;
        }
    }
}
