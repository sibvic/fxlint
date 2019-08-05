using fxlint.LuaCases;
using fxlint.MQL4Cases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fxlint_tests
{
    [TestClass]
    public class MissingIndicator
    {
        const string presentSnippet = @"    assert(core.indicators:findIndicator(M[i]) ~= nil, M[i] .. "" indicator must be installed"");
        Row[i]=core.indicators:create(M[i], source[IN[i]], F[i]);";
        const string presentSnippet2 = @"    assert(core.indicators:findIndicator(   instance.parameters:getString(""Method"" .. i)) ~= nil,    instance.parameters:getString(""Method"" .. i) .. "" indicator must be installed"");
        MA[i] = core.indicators:create(instance.parameters:getString(""Method"" .. i), Source[instance.parameters:getString(""Price""..i)],  instance.parameters:getInteger(""Period"" .. i)  );";
        const string presentSnippet3 = @"    assert(core.indicators:findIndicator(""ICHIMOKU + DAILY-CANDLE_X + HULL-MA_X + MACD"") ~= nil, ""ICHIMOKU + DAILY-CANDLE_X + HULL-MA_X + MACD"" .. "" indicator must be installed"");
    indi = core.indicators:create(""ICHIMOKU + DAILY-CANDLE_X + HULL-MA_X + MACD"", trading_logic.MainSource.close, keh);";
        const string presentSnippet4 = @"indiProfile = core.indicators:findIndicator(""CUSTOM TIME FRAME CANDLE VIEW.HAILKAYY"");
    assert(indiProfile ~= nil, ""Please download and install CUSTOM TIME FRAME CANDLE VIEW.HAILKAYY.lua indicator"");
    indi = core.indicators:create(""CUSTOM TIME FRAME CANDLE VIEW.HAILKAYY"", source);";

        [TestMethod]
        public void MissingCheckPresent()
        {
            MissingIndicatorCheck check = new MissingIndicatorCheck();
            var warnings = check.GetWarnings(presentSnippet, "");
            Assert.AreEqual(0, warnings.Length);

            var warnings2 = check.GetWarnings(presentSnippet2, "");
            Assert.AreEqual(0, warnings2.Length);

            var warnings3 = check.GetWarnings(presentSnippet3, "");
            Assert.AreEqual(0, warnings3.Length);

            var warnings4 = check.GetWarnings(presentSnippet4, "");
            Assert.AreEqual(0, warnings4.Length);
        }

        [TestMethod]
        public void MissingFixPresent()
        {
            MissingIndicatorCheck check = new MissingIndicatorCheck();
            var fixedCode = check.Fix(presentSnippet, "");
            Assert.AreEqual(presentSnippet, fixedCode);

            var fixedCode2 = check.Fix(presentSnippet2, "");
            Assert.AreEqual(presentSnippet2, fixedCode2);

            var fixedCode3 = check.Fix(presentSnippet3, "");
            Assert.AreEqual(presentSnippet3, fixedCode3);
        }

        const string no_check_mql4_code = "double nma4_current = iCustom(_symbol, _timeframe, \"NMA.4\", 0, 0 + shift);\nint init()\n{\n}";
        const string check_mql4_code2 = @"double ST = iCustom(NULL, 0, ""ST"", ST_N, ST_M, 2, period);
   double temp = iCustom(NULL, 0, ""ST"", ST_N, ST_M, 0, 0);
   if (GetLastError() == ERR_INDICATOR_CANNOT_LOAD)";
        const string check_mql4_code = @"double temp = iCustom(NULL, 0, ""2 bar supply and demand"", 0, 0);
   if (GetLastError() == ERR_INDICATOR_CANNOT_LOAD)";

        [TestMethod]
        public void MissingCheckMQL4()
        {
            var check = new NoCustomIndicatorCheck();
            var warnings = check.GetWarnings(no_check_mql4_code, "");
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void CheckMQL4()
        {
            var check = new NoCustomIndicatorCheck();
            var warnings = check.GetWarnings(check_mql4_code, "");
            Assert.AreEqual(0, warnings.Length);

            warnings = check.GetWarnings(check_mql4_code2, "");
            Assert.AreEqual(0, warnings.Length);
        }

        [TestMethod]
        public void FixMQL4()
        {
            var check = new NoCustomIndicatorCheck();
            var code = check.Fix(no_check_mql4_code, "");

            var warnings = check.GetWarnings(code, "");
            Assert.AreEqual(0, warnings.Length);
        }
    }
}
