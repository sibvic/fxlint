using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace fxlint.LuaCases
{
    public class InRangeUse : ILintCheck
    {
        private const string OutdatedInRangeCode = @"function InRange(now, openTime, closeTime)
    if openTime < closeTime then
        return now >= openTime and now <= closeTime;
    end
    if openTime > closeTime then
        return now > openTime or now < closeTime;
    end

    return now == openTime;
end";

        private const string InRangeCode = @"function InRange(now, openTime, closeTime)
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
end

";

        public string[] GetWarnings(string code)
        {
            if (code.Contains(" InRange(") && !code.Contains("function InRange"))
                return new string[] { "No InRange method" };
            if (!code.Contains("if openTime == closeTime then"))
                return new string[] { "Outdated InRange method" };
            return new string[] { };
        }

        public string Fix(string code)
        {
            if (!code.Contains(" InRange("))
                return code;
            if (code.Contains("function InRange"))
            {
                if (code.Contains(OutdatedInRangeCode))
                    return code.Replace(OutdatedInRangeCode, InRangeCode);
                return code;
            }

            var index = code.IndexOf("function Parse");
            if (index < 0)
                index = code.IndexOf("function Update");
            return code.Insert(index, InRangeCode);
        }
    }
}
