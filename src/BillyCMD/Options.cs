using CommandLine;
using CommandLine.Text;

namespace BillyCMD
{
    public class Options
    {
        public class SchemaSuboptions
        {
            [Option('t', "type", Required = true, HelpText = "Select the type to print the schema")]
            public string Type { get; set; }
        }

        public class BuildSubOptions
        {
            [Option('v', "verbose", Required = false, DefaultValue = false, HelpText = "Display build logs")]
            public bool Verbose { get; set; }

            [Option('i', "input", Required = false, DefaultValue = ".\\Templates", HelpText = "Path to sources")]
            public string InputPath { get; set; }

            [Option('o', "output", Required = false, DefaultValue = ".\\Output", HelpText = "Output directory")]
            public string OutputPath { get; set; }
        }

        [VerbOption("schema", HelpText = "Print schema for a type")]
        public SchemaSuboptions SchemaVerb { get; set; }

        [VerbOption("build", HelpText = "Build all paperwork from the sources")]
        public BuildSubOptions BuildVerb { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}