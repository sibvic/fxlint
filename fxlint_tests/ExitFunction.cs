using Microsoft.VisualStudio.TestTools.UnitTesting;
using fxlint.LuaCases;

namespace fxlint_tests
{
    [TestClass]
    public class ExitFunction
    {
        private const string normilizedOldExitFunction = @"function exit(BuySell)
    if not (AllowTrade) then
        return true
    end

    local valuemap, success, msg

    if tradesCount(BuySell) > 0 then
        valuemap = core.valuemap()

        -- switch the direction since the order must be in oppsite direction
        if BuySell == ""B"" then
            BuySell = ""S""
        else
            BuySell = ""B""
        end
        valuemap.OrderType = ""CM""
        valuemap.OfferID = Offer
        valuemap.AcctID = Account
        valuemap.NetQtyFlag = ""Y""
        valuemap.BuySell = BuySell
        success, msg = terminal:execute(201, valuemap)

        if not (success) then
            terminal:alertMessage(
                instance.bid:instrument(),
                instance.bid[instance.bid:size() - 1],
                ""Open order failed"" .. msg,
                instance.bid:date(instance.bid:size() - 1)
            )
            return false
        end
        return true
    end
    return false
end";
        private const string oldExitFunction = @"function exit(BuySell)
    if not(AllowTrade) then
        return true;
    end

    local valuemap, success, msg;

    if tradesCount(BuySell) > 0 then
        valuemap = core.valuemap();

        -- switch the direction since the order must be in oppsite direction
        if BuySell == ""B"" then
            BuySell = ""S"";
        else
            BuySell = ""B"";
        end
        valuemap.OrderType = ""CM"";
        valuemap.OfferID = Offer;
        valuemap.AcctID = Account;
        valuemap.NetQtyFlag = ""Y"";
        valuemap.BuySell = BuySell;
        success, msg = terminal:execute(101, valuemap);

        if not(success) then
            terminal:alertMessage(instance.bid:instrument(), instance.bid[instance.bid:size() - 1], ""Open order failed"" .. msg, instance.bid:date(instance.bid:size() - 1));
            return false;
        end
        return true;
    end
    return false;
end";
        [TestMethod]
        public void Detect()
        {
            OldExitFunction check = new OldExitFunction();
            var warnings = check.GetWarnings(oldExitFunction);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void DetectNomilized()
        {
            OldExitFunction check = new OldExitFunction();
            var warnings = check.GetWarnings(normilizedOldExitFunction);
            Assert.AreEqual(1, warnings.Length);
        }

        [TestMethod]
        public void Fix()
        {
            OldExitFunction check = new OldExitFunction();
            var code = check.Fix(oldExitFunction);
            Assert.AreEqual(true, code.Contains("function exit(BuySell, use_net)"));
        }

    }
}
