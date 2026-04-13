using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class GoalController : Controller
    {
        GoalServices GoalServices;
        private readonly IMapper _mapper;

        public GoalController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            GoalServices = new GoalServices(unitOfWork, _mapper);
        }
        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await GoalServices.deleteAll();
            var resultWithStatusCode = ResponseStatusCode<Goal>.GetApiResponseCode(result, "HttpDelete");
            return resultWithStatusCode;
        }
        // ==============================
        // GET ALL GOALS
        // ==============================
        [HttpGet("getAll")]
        public async Task<IActionResult> GetGoalList()
        {
            var result = await GoalServices.getGoalsList();

            var resultWithStatusCode =
                ResponseStatusCode<Goal>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // ==============================
        // GET GOAL BY ID
        // ==============================
        [HttpGet("getById")]
        public async Task<IActionResult> GetGoal(int id)
        {
            var result = await GoalServices.getGoal(id);

            var resultWithStatusCode =
                ResponseStatusCode<Goal>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // ==============================
        // UPDATE GOAL
        // ==============================
        [HttpPut("updateById")]
        public async Task<IActionResult> PutGoal(int id, [FromBody] List<GoalDTO> goalUpdated)
        {
            var result = await GoalServices.updateGoal(id, goalUpdated);

            var resultWithStatusCode =
                ResponseStatusCode<Goal>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }

        // ==============================
        // ADD GOAL
        // ==============================
        [HttpPost("add")]
        public async Task<IActionResult> AddGoal([FromBody] List<GoalDTO> goalDTOs)
        {
            var result = await GoalServices.addGoal(goalDTOs);

            var resultWithStatusCode =
                ResponseStatusCode<Goal>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        // ==============================
        // DELETE GOAL
        // ==============================
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var result = await GoalServices.deleteGoal(id);

            var resultWithStatusCode =
                ResponseStatusCode<Goal>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}