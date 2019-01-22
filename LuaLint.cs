using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace fxlint
{
    class LuaLint
    {
        public static string[] GetWarnings(string code)
        {
            List<string> warnings = new List<string>();
            if (ContainsOldParseTime(code))
                warnings.Add("Old version of ParseTime");
            if (ContainsOldTradingTimeCheck(code))
                warnings.Add("Old version of trading time check (not via InRange)");
            if (ContainsNoPrecisionForOscullator(code))
                warnings.Add("No precision for oscillator stream");
            if (ContainsNoAllowTrade(code))
                warnings.Add("No allow trade");
            string[] missingIndicatorChecks = GetMissingIndicatorChecks(code);
            foreach (var missingIndicatorCheck in missingIndicatorChecks)
            {
                warnings.Add("Missing indicator assert for " + missingIndicatorCheck);
            }
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
        private static string[] GetMissingIndicatorChecks(string code)
        {
            List<string> missingChecks = new List<string>();
            var matches = indicatorCreatePattern.Matches(code);
            foreach (Match match in matches)
            {
                var indicatorName = match.Groups["indiName"].Value;
                if (IsStandardIndicator(indicatorName.Trim().Trim('"')))
                    continue;
                Regex indicatorAssertPattern = new Regex("assert\\(core\\.indicators:findIndicator\\(" + indicatorName + "\\) ~= nil");
                if (!indicatorAssertPattern.IsMatch(code) && !missingChecks.Contains(indicatorName))
                    missingChecks.Add(indicatorName);
            }
            return missingChecks.ToArray();
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

        static Regex oldTradingTimeCheckPattern = new Regex("if not\\(now ?>= ?OpenTime(\n\r)?[^a]*and now ?<= ?CloseTime\\)");
        private static bool ContainsOldTradingTimeCheck(string code)
        {
            return oldTradingTimeCheckPattern.IsMatch(code);
        }

        static Regex oldParseTimePattern = new Regex("local Pos ?= ?string\\.find\\(time, ?\":\"\\);[ ]*\r\n[ ]*local");
        private static bool ContainsOldParseTime(string code)
        {
            return oldParseTimePattern.IsMatch(code);
        }
    }
}