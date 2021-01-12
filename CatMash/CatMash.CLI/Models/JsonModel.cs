using System.Collections.Generic;

namespace CatMash.CLI.Models
{
    internal record JsonModel(IEnumerable<CatModel> images);
}