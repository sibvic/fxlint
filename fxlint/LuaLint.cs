using fxlint.LuaCases;
using System.Collections.Generic;
using System.Linq;

namespace fxlint
{
    class LuaLint
    {
        List<ILintCheck> _cases = new List<ILintCheck>();
        List<string> _excludedChecks;
        public LuaLint(string indicoreRootPath, List<string> excludedChecks)
        {
            _excludedChecks = excludedChecks;
            _cases.Add(new OldTradingTimeCheck());
            _cases.Add(new InRangeUse());
            _cases.Add(new MissingIndicatorCheck());
            _cases.Add(new ConvertTimeTZServer());
            _cases.Add(new OldPraseTime());
            _cases.Add(new OldInRange());
            _cases.Add(new NoPrecisionForOscillator());
            _cases.Add(new OldExitFunction());
            _cases.Add(new NoNonOptimizableParameters());
            _cases.Add(new SyntaxCheck(indicoreRootPath));
        }

        public IEnumerable<string> Checks 
        {
            get
            {
                return _cases.Select(c => c.Id);
            }
        }

        public string[] GetWarnings(string code, string name)
        {
            List<string> warnings = new List<string>();
            foreach (var lintCase in _cases.Where(c => !_excludedChecks.Contains(c.Id)))
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
            foreach (var lintCase in _cases.Where(c => !_excludedChecks.Contains(c.Id)))
            {
                fixedCode = lintCase.Fix(fixedCode, name);
            }

            return fixedCode;
        }
    }
}