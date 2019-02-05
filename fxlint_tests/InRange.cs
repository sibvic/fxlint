using Microsoft.VisualStudio.TestTools.UnitTesting;
using fxlint.Cases;

namespace fxlint_tests
{
    [TestClass]
    public class InRange
    {
        private const string inRangeSnippet = @"if not(now >= OpenTime
	and now <= CloseTime)
	then";
        private const string inRangeSnippet2 = @"       if not(now >= OpenTime
	  and now <= CloseTime)
	  and not ManageExit
	  then            ";
        readonly string noInrangeUseSnippet = @"some code";
        readonly string inRangeUseSnippet = @"-- NG: create a function to parse time
function ParseTime(time)
    local Pos = string.find(time, "":"");
    if Pos == nil then
        return nil, false;
            end
            local h = tonumber(string.sub(time, 1, Pos - 1));
            time = string.sub(time, Pos + 1);
            Pos = string.find(time, "":"");
            if Pos == nil then
        return nil, false;
            end
            local m = tonumber(string.sub(time, 1, Pos - 1));
            local s = tonumber(string.sub(time, Pos + 1));
            return (h / 24.0 + m / 1440.0 + s / 86400.0),                          --time in ole format
                 ((h >= 0 and h< 24 and m >= 0 and m< 60 and s >= 0 and s< 60) or(h == 24 and m == 0 and s == 0)); --validity flag
 end

      if not InRange(now, OpenTime, CloseTime) then  
";

        #region Time conversion
        private const string timeConversionSnippet = "now = core.host:execute (\"convertTime\", core.TZ_SERVER, ToTime, now);";

        [TestMethod]
        public void TimeConversionDetect()
        {
            ConvertTimeTZServer check = new ConvertTimeTZServer();
            var warnings = check.GetWarnings(timeConversionSnippet);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void TimeConversionFix()
        {
            ConvertTimeTZServer check = new ConvertTimeTZServer();
            var fixedCode = check.Fix(timeConversionSnippet);
            Assert.AreEqual("now = core.host:execute(\"convertTime\", core.TZ_EST, ToTime, now);", fixedCode);
        }
        #endregion

        [TestMethod]
        public void InRangePresentDetect()
        {
            InRangeUse check = new InRangeUse();
            var warnings = check.GetWarnings(inRangeUseSnippet);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void InRangePresentFix()
        {
            InRangeUse check = new InRangeUse();
            var fixedCode = check.Fix(inRangeUseSnippet);
            Assert.AreEqual(true, fixedCode.Contains("function InRange("));
        }

        [TestMethod]
        public void InRangePresentFixNoNeed()
        {
            InRangeUse check = new InRangeUse();
            var fixedCode = check.Fix(noInrangeUseSnippet);
            Assert.AreEqual(noInrangeUseSnippet, fixedCode);
        }

        [TestMethod]
        public void OldTradingTimeCheckDetect()
        {
            OldTradingTimeCheck check = new OldTradingTimeCheck();
            var warnings = check.GetWarnings(inRangeSnippet);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void OldTradingTimeCheckDetect2()
        {
            OldTradingTimeCheck check = new OldTradingTimeCheck();
            var warnings = check.GetWarnings(inRangeSnippet2);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void OldTradingTimeCheckFix()
        {
            OldTradingTimeCheck check = new OldTradingTimeCheck();
            var fixedCode = check.Fix(inRangeSnippet);
            Assert.AreEqual(true, fixedCode.Contains(" not InRange(now, OpenTime, CloseTime) "));
        }

        [TestMethod]
        public void OldTradingTimeCheckFix2()
        {
            OldTradingTimeCheck check = new OldTradingTimeCheck();
            var fixedCode = check.Fix(inRangeSnippet2);
            Assert.AreEqual(true, fixedCode.Contains(" not InRange(now, OpenTime, CloseTime) "));
        }
    }
}
