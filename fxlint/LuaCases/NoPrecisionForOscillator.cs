using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace fxlint.LuaCases
{
    public class NoPrecisionForOscillator : ILintCheck
    {
        static Regex _pattern = new Regex("(?<streamName>[^\t\r\n =]+)[\t\r\n ]*=[\t\r\n ]*instance:addStream\\(");
        public string Fix(string code, string fileName)
        {
            List<string> names = GetNames(code);
            var fixedCode = code;
            foreach (var name in names)
            {
                Regex pattern = new Regex(name.Replace("[", "\\[").Replace("]", "\\]") + "[\t\r\n ]*=[\t\r\n ]*instance:addStream\\(");
                var match = pattern.Match(fixedCode);
                if (match.Success)
                {
                    int index = FindIndexOfNextLine(fixedCode, match.Groups[0].Index);
                    fixedCode = fixedCode.Insert(index, "    " + name + ":setPrecision(math.max(2, instance.source:getPrecision()));\n");
                }
            }

            return fixedCode;
        }

        private int FindIndexOfNextLine(string code, int index)
        {
            while (index < code.Length - 1)
            {
                if (code[index] == '(')
                    break;
                index++;
            }
            int count = 0;
            while (index < code.Length - 1)
            {
                if (code[index] == '(')
                    count++;
                else if (code[index] == ')')
                    count--;
                if (count == 0)
                    break;
                index++;
            }
            if (count != 0)
                return -1;
            index = code.IndexOf("\n", index) + 1;
            if (code[index] == '\r')
                index++;
            return index;
        }

        public string[] GetWarnings(string code, string name)
        {
            return GetNames(code).Select(n => "No setPrecision for " + n).ToArray();
        }

        public List<string> GetNames(string code)
        {
            List<string> names = new List<string>();
            if (!code.Contains("core.Oscillator"))
                return names;
            var matches = _pattern.Matches(code);
            foreach (Match match in matches)
            {
                var streamName = match.Groups["streamName"].Value;
                if (!streamName.Trim().StartsWith("--") && !code.Contains(streamName + ":setPrecision"))
                {
                    names.Add(streamName);
                }
            }

            return names;
        }

        public string Id => "NoPrecisionForOscillator";
    }
}
