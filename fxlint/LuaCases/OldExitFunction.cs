using System.Text.RegularExpressions;

namespace fxlint.LuaCases
{
    public class OldExitFunction : ILintCheck
    {
        const string EOF = "[\r\t\n ]*";
        static Regex _functionPattern = new Regex("function exit\\(BuySell\\)" + EOF
            + "if not\\(AllowTrade\\) then" + EOF
            + "return true;" + EOF
            + "end" + EOF
            + "local valuemap, success, msg;" + EOF
            + "if tradesCount\\(BuySell\\) > 0 then" + EOF
            + "valuemap = core.valuemap\\(\\);" + EOF
            + "-- switch the direction since the order must be in oppsite direction" + EOF
            + "if BuySell == \"B\" then" + EOF
            + "BuySell = \"S\";?" + EOF
            + "else" + EOF
            + "BuySell = \"B\";?" + EOF
            + "end" + EOF
            + "valuemap.OrderType = \"CM\";" + EOF
            + "valuemap.OfferID = Offer;" + EOF
            + "valuemap.AcctID = Account;" + EOF
            + "valuemap.NetQtyFlag = \"Y\";" + EOF
            + "valuemap.BuySell = BuySell;" + EOF
            + "success, msg = terminal:execute\\(101, valuemap\\);" + EOF
            + "if not\\(success\\) then" + EOF
            + "terminal:alertMessage\\(instance.bid:instrument\\(\\), instance.bid\\[instance.bid:size\\(\\) - 1\\], \"Open order failed\" .. msg, instance.bid:date\\(instance.bid:size\\(\\) - 1\\)\\);" + EOF
            + "return false;" + EOF
            + "end" + EOF
            + "return true;" + EOF
            + "end" + EOF
            + "return false;" + EOF
            + "end"
        );

        static Regex _functionNormilizedPattern = new Regex("function exit\\(BuySell\\)" + EOF
            + "if not \\(AllowTrade\\) then" + EOF
            + "return true" + EOF
            + "end" + EOF
            + "local valuemap, success, msg" + EOF
            + "if tradesCount\\(BuySell\\) > 0 then" + EOF
            + "valuemap = core.valuemap\\(\\)" + EOF
            + "-- switch the direction since the order must be in oppsite direction" + EOF
            + "if BuySell == \"B\" then" + EOF
            + "BuySell = \"S\"" + EOF
            + "else" + EOF
            + "BuySell = \"B\"" + EOF
            + "end" + EOF
            + "valuemap.OrderType = \"CM\"" + EOF
            + "valuemap.OfferID = Offer" + EOF
            + "valuemap.AcctID = Account" + EOF
            + "valuemap.NetQtyFlag = \"Y\"" + EOF
            + "valuemap.BuySell = BuySell" + EOF
            + "success, msg = terminal:execute\\(\\d+, valuemap\\)" + EOF
            + "if not \\(success\\) then" + EOF
            + "terminal:alertMessage\\(" + EOF
            + "instance.bid:instrument\\(\\)," + EOF
            + "instance.bid\\[instance.bid:size\\(\\) - 1\\]," + EOF
            + "\"Open order failed\" .. msg," + EOF
            + "instance.bid:date\\(instance.bid:size\\(\\) - 1\\)" + EOF
            + "\\)" + EOF
            + "return false" + EOF
            + "end" + EOF
            + "return true" + EOF
            + "end" + EOF
            + "return false" + EOF
            + "end"
        );

        private const string _fixedCode = @"function exit(BuySell, use_net)
    if not (AllowTrade) then
        return true
    end

    if use_net == true then
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
            success, msg = terminal:execute(101, valuemap)

            if not(success) then
               terminal:alertMessage(
                   instance.bid:instrument(),
                   instance.bid[instance.bid:size() - 1],
                   ""Open order failed""..msg,
                   instance.bid:date(instance.bid:size() - 1)
               )
                return false
            end
            return true
        end
    else
        local enum = core.host:findTable(""trades""):enumerator();
        local row = enum:next();
        while row ~= nil do
            if row.BS == BuySell and row.OfferID == Offer then
                local valuemap = core.valuemap();
                valuemap.BuySell = row.BS == ""B"" and ""S"" or ""B"";
                valuemap.OrderType = ""CM"";
                valuemap.OfferID = row.OfferID;
                valuemap.AcctID = row.AccountID;
                valuemap.TradeID = row.TradeID;
                valuemap.Quantity = row.Lot;
                local success, msg = terminal:execute(101, valuemap);
            end
        row = enum: next();
        end
    end
    return false
end";
        public string Fix(string code, string name)
        {
            if (_functionPattern.IsMatch(code))
                return _functionPattern.Replace(code, _fixedCode);
            return _functionNormilizedPattern.Replace(code, _fixedCode);
        }

        public string[] GetWarnings(string code, string name)
        {
            if (_functionPattern.IsMatch(code))
                return new string[] { "Old version of Exit" };
            if (_functionNormilizedPattern.IsMatch(code))
                return new string[] { "Old version of Exit" };
            return new string[] { };
        }
    }
}
