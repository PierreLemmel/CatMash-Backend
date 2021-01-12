using System;

namespace CatMash.Api.Models
{
    public record ProposalModel(string ProposalId, string? UserId, CatModel Cat1, CatModel Cat2);
}