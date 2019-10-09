using System;
using System.Collections.Generic;
using CommandLine;

namespace fxlint
{
    [Verb("scan", HelpText = "Scan files.")]
    class ScannerOptions
    {
        [Option("path", Required = true, HelpText = "Path to the folder with files or path to file for scan.")]
        public string Path { get; set; }

        #region Extensions
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
        #endregion

        #region Excluded
        public List<string> Excluded { get; set; } = new List<string>();

        [Option("exclude", Required = false, HelpText = "List of excluded check separated by ';'.")]
        public string ExcludeString
        {
            get
            {
                return "";
            }
            set
            {
                Excluded = new List<string>();
                Excluded.AddRange(value.Split(";", StringSplitOptions.RemoveEmptyEntries));
            }
        }
        #endregion

        [Option("output-file", Default = "fxlint_log.txt", Required = false, HelpText = "Output file name")]
        public string OutputFile { get; set; }

        [Option("indicore-root", Required = false, HelpText = "Indicore root path")]
        public string IndicoreRootPath { get; set; }
    }
}
