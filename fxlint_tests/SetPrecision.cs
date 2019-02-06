using fxlint.Cases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace fxlint_tests
{
    [TestClass]
    public class SetPrecision
    {
        #region Set precision
        private const string _noPrecisionSnippet = @"   indicator:type(core.Oscillator);
    SIG = instance:addStream(""SIG"", core.Line, name .. "".SIG"", ""SIG"", instance.parameters.SIG_color, firstSIG);
    SIG:setWidth(instance.parameters.width2);
    SIG:setStyle(instance.parameters.style2);
";
        [TestMethod]
        public void NoPrecisionDetect()
        {
            var check = new NoPrecisionForOscillator();
            var warnings = check.GetWarnings(_noPrecisionSnippet);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void NoPrecisionFix()
        {
            var check = new NoPrecisionForOscillator();
            var fixedCode = check.Fix(_noPrecisionSnippet);
            Assert.AreEqual(true, fixedCode.Contains("SIG:setPrecision(math.max(2, instance.source:getPrecision()));"));
        }
        #endregion

    }
}
