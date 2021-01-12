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

        [HttpPost("require-proposal")]
        public async Task<ActionResult<ProposalModel>> RequireProposal(RequireProposalModel requireProposal)
        {
            ProposalModel proposal = await catService.RequireNewPorposal(requireProposal.UserId);
            return Ok(proposal);
        }

        [HttpPost("submit-vote")]
        public async Task<ActionResult> SubmitVote(VoteModel vote)
        {
            await catService.SubmitVote(vote);
            return Ok();
        }
    }
}