using System;

namespace CatMash.Api.Models
{
    public record ProposalModel(string ProposalId, CatModel Cat1, CatModel Cat2);
}