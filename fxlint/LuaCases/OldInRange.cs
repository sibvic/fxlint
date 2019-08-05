using System.Text.RegularExpressions;

namespace fxlint.LuaCases
{
    public class OldInRange : ILintCheck
    {
        static Regex _functionPattern = new Regex("function InRange\\(now\\)[\r\t\n ]*"
            + "(-- from, to[\r\t\n ]*)?"
            + "if OpenTime[\r\t\n ]*<[\r\t\n ]*CloseTime then[\r\t\n ]*"
            + "return now[\r\t\n ]*>=[\r\t\n ]*OpenTime and now[\r\t\n ]*<=[\r\t\n ]*CloseTime;?[\r\t\n ]*"
            + "end[\r\t\n ]*"
            + "if OpenTime[\r\t\n ]*>[\r\t\n ]*CloseTime then[\r\t\n ]*"
            + "return now[\r\t\n ]*>[\r\t\n ]*OpenTime or now[\r\t\n ]*<[\r\t\n ]*CloseTime;?[\r\t\n ]*"
            + "end[\r\t\n ]*"
            + "return now[\r\t\n ]*==[\r\t\n ]*OpenTime;?[\r\t\n ]*"
            + "end"
          );
        static Regex _usePattern = new Regex("InRange\\(now\\)");

        public string Fix(string code, string name)
        {
            var fixedCode = _functionPattern.Replace(code, @"function InRange(now, openTime, closeTime)
    if openTime == closeTime then
        return true;
    end
    if openTime < closeTime then
        return now >= openTime and now <= closeTime;
    end
    if openTime > closeTime then
        return now > openTime or now < closeTime;
    end

    return now == openTime;
end");

            return _usePattern.Replace(fixedCode, "InRange(now, OpenTime, CloseTime)");
        }

        public string[] GetWarnings(string code, string name)
        {
            if (_functionPattern.IsMatch(code))
                return new string[] { "Old version of InRange" };
            return new string[] { };
        }
    }
}
