using System;

namespace CatMash.Api.Models
{
    public record CatStatModel(string CatId, int Matches, int Votes, string Src)
    {
        public float WinRate => Matches != 0 ? (100.0f * Votes)/ Matches : 0;
    }
}