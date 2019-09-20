using System;
using System.Collections.Generic;
using CommandLine;

namespace fxlint
{
    [Verb("scan", HelpText = "Scan files.")]
    class ScannerOptions
    {
        [Option("path", Required = true, HelpText = "Path with files to fix.")]
        public string Path { get; set; }

        public List<string> Extensions { get; set; }

        [Option("extensions", Default = ".lua;.mq4", Required = false, HelpText = "Extensions to fix.")]
        public string ExtensionsString
        {
            get
            {
                return "";
            }
            set
            {
                Extensions = new List<string>();
                Extensions.AddRange(value.Split(";", StringSplitOptions.RemoveEmptyEntries));
            }
        }

        [Option("optput-file", Default = "fxlint_log.txt", Required = false, HelpText = "Output file name")]
        public string OutputFile { get; set; }
    }
}
