using CatMash.Api.Models;
using CatMash.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CatMash.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatMashController : ControllerBase
    {
        private readonly ICatService catService;

        public CatMashController(ICatService catService)
        {
            this.catService = catService;
        }

        [HttpGet("get-proposal")]
        public async Task<ProposalModel> GetProposal(string? userId) => await catService.RequireNewPorposal(userId);

        [HttpPost("submit-vote")]
        public async Task SubmitVote(VoteModel vote) => await catService.SubmitVote(vote);
    }
}