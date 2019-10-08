using fxlint.LuaCases;
using System.Collections.Generic;

namespace fxlint
{
    class LuaLint
    {
        List<ILintCheck> _cases = new List<ILintCheck>();
        public LuaLint(string indicoreRootPath, List<string> excludedChecks)
        {
            if (!excludedChecks.Contains("OldTradingTimeCheck"))
                _cases.Add(new OldTradingTimeCheck());
            if (!excludedChecks.Contains("InRangeUse"))
                _cases.Add(new InRangeUse());
            if (!excludedChecks.Contains("MissingIndicatorCheck"))
                _cases.Add(new MissingIndicatorCheck());
            if (!excludedChecks.Contains("ConvertTimeTZServer"))
                _cases.Add(new ConvertTimeTZServer());
            if (!excludedChecks.Contains("OldPraseTime"))
                _cases.Add(new OldPraseTime());
            if (!excludedChecks.Contains("OldInRange"))
                _cases.Add(new OldInRange());
            if (!excludedChecks.Contains("NoPrecisionForOscillator"))
                _cases.Add(new NoPrecisionForOscillator());
            if (!excludedChecks.Contains("OldExitFunction"))
                _cases.Add(new OldExitFunction());
            if (!excludedChecks.Contains("NoNonOptimizableParameters"))
                _cases.Add(new NoNonOptimizableParameters());
            if (!excludedChecks.Contains("indicoreRootPath"))
                _cases.Add(new SyntaxCheck(indicoreRootPath));
        }

        public string[] GetWarnings(string code, string name)
        {
            List<string> warnings = new List<string>();
            foreach (var lintCase in _cases)
            {
                var lintWarnings = lintCase.GetWarnings(code, name);
                warnings.AddRange(lintWarnings);
            }
            if (ContainsNoAllowTrade(code))
                warnings.Add("No allow trade");
            return warnings.ToArray();
        }

        bool ContainsNoAllowTrade(string code)
        {
            if (!code.Contains("strategy:name("))
                return false;
            if (code.Contains("core.FLAG_ALLOW_TRADE"))
                return false;

            return code.Contains("terminal:execute");
        }

        string FixNoAllowTrade(string code)
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

        internal string FixCode(string code, string name)
        {
            var fixedCode = code;
            if (ContainsNoAllowTrade(fixedCode))
                fixedCode = FixNoAllowTrade(fixedCode);
            foreach (var lintCase in _cases)
            {
                fixedCode = lintCase.Fix(fixedCode, name);
            }

            return fixedCode;
        }
    }
}