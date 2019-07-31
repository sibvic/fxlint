namespace fxlint
{
    public interface ILintCheck
    {
        string Fix(string code);
        string[] GetWarnings(string code);
    }
}