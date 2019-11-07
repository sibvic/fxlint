using fxlint.MQL4Cases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fxlint_tests
{
    [TestClass]
    public class ObjectDeletionCheckTest
    {
        [TestMethod]
        public void WithCheck()
        {
            var snippet = System.IO.File.ReadAllText("../../../snippets/ObjectDeletionExist.mq4");
            var check = new ObjectDeletionCheck();
            var warnings = check.GetWarnings(snippet, "");
            Assert.AreEqual(0, warnings.Length);
        }
    }
}
