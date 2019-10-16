using System;
using System.Collections.Generic;
using CommandLine;

namespace fxlint
{
    [Verb("fix", HelpText = "Fix files.")]
    class FixOptions
    {
        [Option("path", Required = true, HelpText = "Path to folder with files or path to file for fix.")]
        public string Path { get; set; }

        public List<string> Extensions { get; set; }

        [Option("extensions", Required = false, HelpText = "Extensions to fix.")]
        public string ExtensionsString
        {
            get
            {
                return "";
            }
            set
            {
                Extensions = new List<string>();
                Extensions.AddRange(value.Split(';', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        [Option("indicore-root", Required = false, HelpText = "Indicore root path")]
        public string IndicoreRootPath { get; set; }
    }
}
