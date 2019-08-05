using System.Text.RegularExpressions;

namespace fxlint.LuaCases
{
    public class ConvertTimeTZServer : ILintCheck
    {
        static Regex _pattern = new Regex("now[\t\n\r ]*=[\t\n\r ]*core.host:execute[\t\n\r ]*\\([\t\n\r ]*\"convertTime\"[\t\n\r ]*,[\t\n\r ]*core\\.TZ_SERVER");

        public string Fix(string code, string name)
        {
            return _pattern.Replace(code, "now = core.host:execute(\"convertTime\", core.TZ_EST");
        }

        public string[] GetWarnings(string code, string name)
        {
            if (_pattern.IsMatch(code))
                return new string[] { "Convertion of now to TZ_SERVER" };
            return new string[] { };
        }
    }
}
