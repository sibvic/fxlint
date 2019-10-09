using System.Text.RegularExpressions;

namespace fxlint.LuaCases
{
    public class OldPraseTime : ILintCheck
    {
        static Regex _pattern = new Regex("function ParseTime\\(time\\)[\r\t\n ]*"
            + "local Pos[\r\t\n ]*=[\r\t\n ]*string.find\\(time,[\r\t\n ]*\":\"\\);?[\r\t\n ]*"
            + "local h[\r\t\n ]*=[\r\t\n ]*tonumber\\(string.sub\\(time,[\r\t\n ]*1,[\r\t\n ]*Pos[\r\t\n ]*-[\r\t\n ]*1\\)\\);?[\r\t\n ]*"
            + "time[\r\t\n ]*=[\r\t\n ]*string.sub\\(time,[\r\t\n ]*Pos[\r\t\n ]*\\+[\r\t\n ]*1\\);?[\r\t\n ]*"
            + "Pos[\r\t\n ]*=[\r\t\n ]*string.find\\(time,[\r\t\n ]*\":\"\\);?[\r\t\n ]*"
            + "local m[\r\t\n ]*=[\r\t\n ]*tonumber\\(string.sub\\(time,[\r\t\n ]*1,[\r\t\n ]*Pos[\r\t\n ]*-[\r\t\n ]*1\\)\\);?[\r\t\n ]*"
            + "local s[\r\t\n ]*=[\r\t\n ]*tonumber\\(string.sub\\(time,[\r\t\n ]*Pos[\r\t\n ]*\\+[\r\t\n ]*1\\)\\);?[\r\t\n ]*"
            + "return \\(h[\r\t\n ]*/[\r\t\n ]*24.0[\r\t\n ]*\\+[\r\t\n ]*m[\r\t\n ]*/[\r\t\n ]*1440.0[\r\t\n ]*\\+[\r\t\n ]*s[\r\t\n ]*/[\r\t\n ]*86400.0\\),[\r\t\n ]*-- time in ole format[\r\t\n ]*"
            + "\\(\\(h[\r\t\n ]*>=[\r\t\n ]*0[\r\t\n ]*and[\r\t\n ]*h[\r\t\n ]*<[\r\t\n ]*24[\r\t\n ]*and[\r\t\n ]*m[\r\t\n ]*>=[\r\t\n ]*0[\r\t\n ]*and[\r\t\n ]*m[\r\t\n ]*<[\r\t\n ]*60[\r\t\n ]*and[\r\t\n ]*s[\r\t\n ]*>=[\r\t\n ]*0[\r\t\n ]*and[\r\t\n ]*s[\r\t\n ]*<[\r\t\n ]*60\\)[\r\t\n ]*or[\r\t\n ]*\\(h[\r\t\n ]*==[\r\t\n ]*24[\r\t\n ]*and[\r\t\n ]*m[\r\t\n ]*==[\r\t\n ]*0[\r\t\n ]*and[\r\t\n ]*s[\r\t\n ]*==[\r\t\n ]*0\\)\\);?[\r\t\n ]*-- validity flag[\r\t\n ]*"
            + "end"
          );

        public string Fix(string code, string name)
        {
            return _pattern.Replace(code, @"function ParseTime(time)
    local pos = string.find(time, "":"");
    if pos == nil then
        return nil, false;
    end
    local h = tonumber(string.sub(time, 1, pos - 1));
    time = string.sub(time, pos + 1);
    pos = string.find(time, "":"");
    if pos == nil then
        return nil, false;
    end
    local m = tonumber(string.sub(time, 1, pos - 1));
    local s = tonumber(string.sub(time, pos + 1));
    return (h / 24.0 + m / 1440.0 + s / 86400.0),                          --time in ole format
            ((h >= 0 and h< 24 and m >= 0 and m< 60 and s >= 0 and s< 60) or(h == 24 and m == 0 and s == 0)); --validity flag
end");
        }

        public string[] GetWarnings(string code, string name)
        {
            if (_pattern.IsMatch(code))
                return new string[] { "Old version of ParseTime" };
            return new string[] { };
        }

        public string Id => "OldPraseTime";
    }
}
