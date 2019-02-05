using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace fxlint.Cases
{
    public class InRangeUse : ILintCheck
    {
        public string[] GetWarnings(string code)
        {
            if (code.Contains(" InRange(") && !code.Contains("function InRange"))
                return new string[] { "No InRange method" };
            return new string[] { };
        }

        public string Fix(string code)
        {
            if (!code.Contains(" InRange(") || code.Contains("function InRange"))
                return code;

            var index = code.IndexOf("function Parse");
            if (index < 0)
                index = code.IndexOf("function Update");
            return code.Insert(index, @"function InRange(now, openTime, closeTime)
    if openTime < closeTime then
        return now >= openTime and now <= closeTime;
    end
    if openTime > closeTime then
        return now > openTime or now < closeTime;
    end

    return now == openTime;
end

");
        }
    }
}
