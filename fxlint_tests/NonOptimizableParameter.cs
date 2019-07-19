using fxlint.Cases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace fxlint_tests
{
    [TestClass]
    public class NonOptimizableParameter
    {
        private const string InitSnippetNigative = @"function Init()
    strategy:name(""MACD Strategy"");
    strategy:description("""");
    strategy.parameters:addGroup(""Price"");
    strategy.parameters:addBoolean(""type"", ""Price type"", """", true);
    strategy.parameters:setFlag(""type"", core.FLAG_BIDASK);
    strategy.parameters:addBoolean(""ShowAlert"", ""ShowAlert"", """", true);
    strategy.parameters:addBoolean(""PlaySound"", ""Play Sound"", """", false);
    strategy.parameters:addFile(""SoundFile"", ""Sound File"", """", """");
    strategy.parameters:setFlag(""SoundFile"", core.FLAG_SOUND);
    strategy.parameters:addBoolean(""RecurrentSound"", ""Recurrent Sound"", """", true);
    strategy.parameters:addBoolean(""SendEmail"", ""Send Email"", """", false);
    strategy.parameters:addString(""Email"", ""Email"", """", """");
    strategy.parameters:setFlag(""Email"", core.FLAG_EMAIL);
end";

        private const string InitSnippetPositive = @"function Init()
    strategy:name(""MACD Strategy"");
    strategy:description("""");
    strategy:setTag(""NonOptimizableParameters"", ""Email,SendEmail,SoundFile,RecurrentSound,PlaySound,ShowAlert"");
    strategy.parameters:addGroup(""Price"");
    strategy.parameters:addBoolean(""type"", ""Price type"", """", true);
    strategy.parameters:setFlag(""type"", core.FLAG_BIDASK);
    strategy.parameters:addBoolean(""ShowAlert"", ""ShowAlert"", """", true);
    strategy.parameters:addBoolean(""PlaySound"", ""Play Sound"", """", false);
    strategy.parameters:addFile(""SoundFile"", ""Sound File"", """", """");
    strategy.parameters:setFlag(""SoundFile"", core.FLAG_SOUND);
    strategy.parameters:addBoolean(""RecurrentSound"", ""Recurrent Sound"", """", true);
    strategy.parameters:addBoolean(""SendEmail"", ""Send Email"", """", false);
    strategy.parameters:addString(""Email"", ""Email"", """", """");
    strategy.parameters:setFlag(""Email"", core.FLAG_EMAIL);
end";

        [TestMethod]
        public void FixNonOptimizableParams()
        {
            NoNonOptimizableParameters check = new NoNonOptimizableParameters();
            var newCode = check.Fix(InitSnippetNigative);
            Assert.AreEqual(0, check.GetWarnings(newCode).Length);
        }

        [TestMethod]
        public void WithoutNonOptimizableParams()
        {
            NoNonOptimizableParameters check = new NoNonOptimizableParameters();
            var warnings = check.GetWarnings(InitSnippetNigative);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void WithNonOptimizableParams()
        {
            NoNonOptimizableParameters check = new NoNonOptimizableParameters();
            var warnings = check.GetWarnings(InitSnippetPositive);
            Assert.AreEqual(0, warnings.Length);
        }
    }
}
