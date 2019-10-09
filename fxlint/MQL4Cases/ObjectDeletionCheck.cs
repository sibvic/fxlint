using System;
using System.Collections.Generic;
using System.Text;

namespace fxlint.MQL4Cases
{
    class ObjectDeletionCheck : ILintCheck
    {
        public string Fix(string code, string name)
        {
            return code;
        }

        public string[] GetWarnings(string code, string name)
        {
            if (code.Contains("ObjectsDeleteAll(ChartID(), IndicatorObjPrefix);") || !code.Contains("ObjectCreate"))
                return new string[] { };

            return new string[] { "No ObjectsDeleteAll. Some object may be left undeleted." };
        }

        public string Id => "ObjectDeletionCheck";
    }
}
