using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace fxlint.Cases
{
    public class MissingIndicatorCheck : ILintCheck
    {
        static Regex indicatorCreatePattern = new Regex("indicators:create\\((?<indiName>[^,]+),");

        bool IsCheckPresent(string code, string indicatorName)
        {
            if (IsStandardIndicator(indicatorName.Trim('"')))
                return true;
            var name = indicatorName.Replace("[", "\\[").Replace("(", "\\(").Replace(")", "\\)");
            Regex indicatorAssertPattern = new Regex("core\\.indicators:findIndicator\\( *" + name + "\\) ~= nil");
            if (indicatorAssertPattern.IsMatch(code))
                return true;
            Regex indicatorAssertPattern2 = new Regex("EnsureIndicatorInstalled\\( *" + name + "\\)");
            if (indicatorAssertPattern2.IsMatch(code))
                return true;
            return false;
        }

        public string Fix(string code)
        {
            var lines = new List<string>();
            lines.AddRange(code.Split('\n'));

            var matches = indicatorCreatePattern.Matches(code);
            var fixedIndicators = new List<string>();
            foreach (Match match in matches)
            {
                var indicatorName = match.Groups["indiName"].Value.Trim();
                if (IsCheckPresent(code, indicatorName))
                    continue;
                if (fixedIndicators.Contains(indicatorName))
                    continue;
                for (int i = 0; i < lines.Count; ++i)
                {
                    var indicatorCreateMatch = indicatorCreatePattern.Match(lines[i]);
                    if (indicatorCreateMatch.Success && indicatorCreateMatch.Groups["indiName"].Value == indicatorName)
                    {
                        lines.Insert(i, string.Format("    assert(core.indicators:findIndicator({0}) ~= nil, {0} .. \" indicator must be installed\");", indicatorName));
                        fixedIndicators.Add(indicatorName);
                        break;
                    }
                }
            }
            return string.Join('\n', lines);
        }

        private static bool IsStandardIndicator(string indicatorName)
        {
            switch (indicatorName)
            {
                case "AC":
                case "AD":
                case "ADX":
                case "ALLIGATOR":
                case "AO":
                case "AROON":
                case "ARSI":
                case "ASI":
                case "ATR":
                case "BB":
                case "CCI":
                case "CHO":
                case "CMF":
                case "CMO":
                case "DIRECTIONAL_REAL_VOLUME":
                case "DMI":
                case "EMA":
                case "EQUITYANDBALANCEVIEW":
                case "EW":
                case "EWN":
                case "EWO":
                case "FRACTAL":
                case "GATOR":
                case "GSI":
                case "HA":
                case "ICH":
                case "KAGI":
                case "KAMA":
                case "KRI":
                case "LWMA":
                case "MACD":
                case "MAE":
                case "MARKET_MOVERS_INDEX":
                case "MD":
                case "MVA":
                case "OBOS":
                case "OBV":
                case "ON_BALANCE_REAL_VOLUME":
                case "OSC":
                case "PIVOT":
                case "POINT_AND_FIGURE":
                case "PPMA":
                case "REAL VOLUME":
                case "REGRESSION":
                case "RENKO_CANDLES":
                case "RLW":
                case "ROC":
                case "RSI":
                case "SAR":
                case "SFK":
                case "SHOWTIMETOEND":
                case "SMMA":
                case "SSD":
                case "STOCHASTIC":
                case "TMA":
                case "TMACD":
                case "TRANSACTIONS":
                case "TSI":
                case "VIDYA":
                case "WMA":
                case "ZIGZAG":
                    return true;
            }
            return false;
        }

        public string[] GetWarnings(string code)
        {
            List<string> missingChecks = new List<string>();
            var matches = indicatorCreatePattern.Matches(code);
            foreach (Match match in matches)
            {
                var indicatorName = match.Groups["indiName"].Value.Trim();
                if (IsCheckPresent(code, indicatorName))
                    continue;
                if (!missingChecks.Contains(indicatorName))
                    missingChecks.Add(indicatorName);
            }
            return missingChecks.Select(s => "Missing indicator assert for " + s).ToArray();
        }
    }
}
