using fxlint.Cases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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

        [TestMethod]
        public void MissingCheckPresent()
        {
            MissingIndicatorCheck check = new MissingIndicatorCheck();
            var warnings = check.GetWarnings(presentSnippet);
            Assert.AreEqual(0, warnings.Length);

            var warnings2 = check.GetWarnings(presentSnippet2);
            Assert.AreEqual(0, warnings2.Length);

            var warnings3 = check.GetWarnings(presentSnippet3);
            Assert.AreEqual(0, warnings3.Length);
        }

        [TestMethod]
        public void MissingFixPresent()
        {
            MissingIndicatorCheck check = new MissingIndicatorCheck();
            var fixedCode = check.Fix(presentSnippet);
            Assert.AreEqual(presentSnippet, fixedCode);

            var fixedCode2 = check.Fix(presentSnippet2);
            Assert.AreEqual(presentSnippet2, fixedCode2);

            var fixedCode3 = check.Fix(presentSnippet3);
            Assert.AreEqual(presentSnippet3, fixedCode3);
        }
    }
}
