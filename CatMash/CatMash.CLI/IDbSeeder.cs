using System.Threading.Tasks;

namespace CatMash.CLI
{
    internal interface IDbSeeder
    {
        Task SeedCatsCollection(string dataUrl);
    }
}