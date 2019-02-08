using System;

namespace fxlint
{
    enum Mode
    {
        Scan,
        Fix
    }

    class Options
    {
        public Mode Mode { get; set; } = Mode.Scan;

        public string File { get; set; }

        public static Options Parse(string[] args)
        {
            var options = new Options();
            var option = args.GetEnumerator();
            while (option.MoveNext())
            {
                switch (option.Current)
                {
                    case "--fix":
                        options.Mode = Mode.Fix;
                        break;
                    case "--file":
                        if (!option.MoveNext())
                            throw new ArgumentException("File need to be specified");
                        options.File = (string)option.Current;
                        break;
                }
            }
            return options;
        }
    }
}
