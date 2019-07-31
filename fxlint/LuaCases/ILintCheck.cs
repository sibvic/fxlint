namespace fxlint.Cases
{
    public interface ILintCheck
    {
        string Fix(string code);
        string[] GetWarnings(string code);
    }
}