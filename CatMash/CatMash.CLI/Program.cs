using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatMash.CLI
{
    public static partial class Program
    {
        public static async Task Main(string[] args) => await Parser.Default
            .ParseArguments<SeedDbOptions>(args)
            .MapResult(
                RunSeedProgram,
                RunErrorProgram
            );

        private static async Task RunSeedProgram(SeedDbOptions options) => await Services.Get<IDbSeeder>().SeedCatsCollection(options.DataUrl);
        private static Task RunErrorProgram(IEnumerable<Error> errors)
        {
            errors.ForEach(Console.WriteLine);
            return Task.CompletedTask;
        }
    }
}
