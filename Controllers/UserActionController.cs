using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DragonAPI.Common;
using DragonAPI.Models.Cache;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;
// using TrueSight.Lite.Common;

namespace DragonAPI.Controllers
{
    public class UserActionController : BaseController<UserActionController>
    {
        private readonly UserActionService userActionService;
        public UserActionController(
            UserActionService userActionService,
            ILogger<UserActionController> logger
            ) : base(logger)
        {
            this.userActionService = userActionService;
        }

        [HttpGet("/api/rongos-service/actions/{id}")]
        public async Task<ActionResult<ApiResultDTO<UserActionDTO>>> Get([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new Exception(message);
            }

            var result = await userActionService.GetAction(id);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }
    }
}