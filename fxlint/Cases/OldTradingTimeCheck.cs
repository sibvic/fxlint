using System.Text.RegularExpressions;

namespace fxlint.Cases
{
    public class OldTradingTimeCheck : ILintCheck
    {
        static Regex oldTradingTimeCheckPattern = new Regex(" not\\(now ?>= ?OpenTime(\n\r)?[^a]*and now ?<= ?CloseTime\\)[\t\r\n ]");
        public string[] GetWarnings(string code)
        {
            if (oldTradingTimeCheckPattern.IsMatch(code))
                return new string[] { "Old version of trading time check (not via InRange)" };
            return new string[] { };
        }

        public string Fix(string code)
        {
            return oldTradingTimeCheckPattern.Replace(code, " not InRange(now, OpenTime, CloseTime) ");
        }
    }
}
