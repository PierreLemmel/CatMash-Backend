namespace CatMash.Api.Models
{
    public record VoteModel(string ProposalId, string CatId, string? userId);
}