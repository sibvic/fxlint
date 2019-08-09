using System;
using System.Collections.Generic;

namespace fxlint
{
    enum Mode
    {
        Scan,
        Fix
    }

    class Options
    {
        public Options()
        {
            Extensions.Add(".lua");
            Extensions.Add(".mq4");
        }
        public Mode Mode { get; set; } = Mode.Scan;

        public string Path { get; set; } = ".";

        public string File { get; set; }

        public List<string> Extensions { get; set; } = new List<string>();

        public static void PrintHelp()
        {
            Console.WriteLine("Available parameters:");
            Console.WriteLine("--extensions .ext1;.ext2");
            Console.WriteLine("--path path_to_scan_folder");
            Console.WriteLine("--file path_to_file");
            Console.WriteLine("--fix");
        }

        public static Options Parse(string[] args)
        {
            var options = new Options();
            var option = args.GetEnumerator();
            while (option.MoveNext())
            {
                switch (option.Current)
                {
                    case "--extensions":
                        if (!option.MoveNext())
                            throw new ArgumentException("Extensions need to be specified");
                        var extensions = (string)option.Current;
                        options.Extensions.Clear();
                        options.Extensions.AddRange(extensions.Split(";", StringSplitOptions.RemoveEmptyEntries));
                        break;
                    case "--path":
                        if (!option.MoveNext())
                            throw new ArgumentException("Path need to be specified");
                        options.Path = (string)option.Current;
                        break;
                    case "--fix":
                        options.Mode = Mode.Fix;
                        break;
                    case "--file":
                        if (!option.MoveNext())
                            throw new ArgumentException("File need to be specified");
                        options.File = (string)option.Current;
                        break;
                    case "--help":
                        PrintHelp();
                        break;
                }
            }
            return options;
        }
    }
}
