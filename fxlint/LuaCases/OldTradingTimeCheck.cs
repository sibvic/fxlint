using System.Text.RegularExpressions;

namespace fxlint.LuaCases
{
    public class OldTradingTimeCheck : ILintCheck
    {
        static Regex _pattern = new Regex(" not\\(now ?>= ?OpenTime(\n\r)?[^a]*and now ?<= ?CloseTime\\)[\t\r\n ]");
        public string[] GetWarnings(string code, string name)
        {
            if (_pattern.IsMatch(code))
                return new string[] { "Old version of trading time check (not via InRange)" };
            return new string[] { };
        }

        public string Fix(string code)
        {
            return _pattern.Replace(code, " not InRange(now, OpenTime, CloseTime) ");
        }
    }
}
