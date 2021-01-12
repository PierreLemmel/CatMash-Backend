using CommandLine;

namespace CatMash.CLI
{
    public static partial class Program
    {
        [Verb("seeddb", HelpText = "Seed the cat database with default data")]
        public class SeedDbOptions
        {
            [Option('s', "src", Required = true)]
            public string DataUrl { get; set; } = "";
        }
    }
}
