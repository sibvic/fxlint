namespace fxlint
{
    public interface ILintCheck
    {
        string Fix(string code, string name);
        string[] GetWarnings(string code, string name);

        string Id { get; }
    }
}