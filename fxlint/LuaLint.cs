using fxlint.Cases;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            _cases.Add(new OldTradingTimeCheck());
            _cases.Add(new OldPraseTime());
        }

        public static string[] GetWarnings(string code)
        {
            List<string> warnings = new List<string>();
            foreach (var lintCase in _cases)
            {
                var lintWarnings = lintCase.GetWarnings(code);
                warnings.AddRange(lintWarnings);
            }
            if (ContainsNoPrecisionForOscullator(code))
                warnings.Add("No precision for oscillator stream");
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

        static Regex streamCreatePattern = new Regex("(?<streamName>[^ =]+) *= *instance:create\\(,");
        static bool ContainsNoPrecisionForOscullator(string code)
        {
            if (!code.Contains("core.Oscillator"))
                return false;

            var matches = indicatorCreatePattern.Matches(code);
            foreach (Match match in matches)
            {
                var streamName = match.Groups["streamName"].Value;
                if (!code.Contains(streamName + ":setPrecision"))
                    return true;
            }

            return false;
        }

        static Regex indicatorCreatePattern = new Regex("indicators:create\\((?<indiName>[^,]+),");

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