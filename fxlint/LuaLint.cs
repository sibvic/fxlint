using fxlint.Cases;
using System.Collections.Generic;

namespace fxlint
{
    class LuaLint
    {
        static List<ILintCheck> _cases = new List<ILintCheck>();
        static LuaLint()
        {
            _cases.Add(new OldTradingTimeCheck());
            _cases.Add(new InRangeUse());
            _cases.Add(new MissingIndicatorCheck());
            _cases.Add(new ConvertTimeTZServer());
            _cases.Add(new OldPraseTime());
            _cases.Add(new OldInRange());
            _cases.Add(new NoPrecisionForOscillator());
            _cases.Add(new OldExitFunction());
        }

        public static string[] GetWarnings(string code)
        {
            List<string> warnings = new List<string>();
            foreach (var lintCase in _cases)
            {
                var lintWarnings = lintCase.GetWarnings(code);
                warnings.AddRange(lintWarnings);
            }
            if (ContainsNoAllowTrade(code))
                warnings.Add("No allow trade");
            return warnings.ToArray();
        }

        static bool ContainsNoAllowTrade(string code)
        {
            if (!code.Contains("strategy:name("))
                return false;
            if (code.Contains("core.FLAG_ALLOW_TRADE"))
                return false;

            return code.Contains("terminal:execute");
        }

        static string FixNoAllowTrade(string code)
        {
            var lines = new List<string>();
            lines.AddRange(code.Split('\n'));
            for (int i = 0; i < lines.Count; ++i)
            {
                if (lines[i].Contains("parameters:addBoolean(\"AllowTrade\""))
                {
                    lines.Insert(i, "    strategy.parameters:setFlag(\"AllowTrade\", core.FLAG_ALLOW_TRADE);");
                    return string.Join('\n', lines);
                }
            }
            return code;
        }

        internal static string FixCode(string code)
        {
            var fixedCode = code;
            if (ContainsNoAllowTrade(fixedCode))
                fixedCode = FixNoAllowTrade(fixedCode);
            foreach (var lintCase in _cases)
            {
                fixedCode = lintCase.Fix(fixedCode);
            }

            return fixedCode;
        }
    }
}