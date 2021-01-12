﻿using CatMash.Api.Models;
using System.Threading.Tasks;

namespace CatMash.Api.Services
{
    public interface ICatService
    {
        Task<ProposalModel> RequireNewPorposal(string? userId);
        Task SubmitVote(VoteModel vote);
    }
}