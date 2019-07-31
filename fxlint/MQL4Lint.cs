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
            //_cases.Add(new OldTradingTimeCheck());
            //_cases.Add(new InRangeUse());
            //_cases.Add(new MissingIndicatorCheck());
            //_cases.Add(new ConvertTimeTZServer());
            //_cases.Add(new OldPraseTime());
            //_cases.Add(new OldInRange());
            //_cases.Add(new NoPrecisionForOscillator());
            //_cases.Add(new OldExitFunction());
            //_cases.Add(new NoNonOptimizableParameters());
        }

        public static string[] GetWarnings(string code)
        {
            List<string> warnings = new List<string>();
            foreach (var lintCase in _cases)
            {
                var lintWarnings = lintCase.GetWarnings(code);
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
