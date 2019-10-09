using System.Collections.Generic;
using CommandLine;

namespace fxlint
{
    [Verb("fix", HelpText = "Fix files.")]
    class FixOptions
    {
        [Option("file", Required = false, HelpText = "File to fix.")]
        public string File { get; set; }

        [Option("path", Required = true, HelpText = "Path with files to fix.")]
        public string Path { get; set; }

        [Option("extensions", Required = false, HelpText = "Extensions to fix.")]
        public List<string> Extensions { get; set; }

        [Option("indicore-root", Required = false, HelpText = "Indicore root path")]
        public string IndicoreRootPath { get; set; }
    }
}
