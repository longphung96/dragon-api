using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DragonAPI.Common;
using DragonAPI.Extensions;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;

namespace DragonAPI.Controllers
{
    [Authorize]
    public class UserController : BaseController<UserController>
    {
        private readonly UserService userService;
        private readonly MasterService masterService;
        private readonly IdentityService identityService;

        public UserController(IdentityService identityService, UserService userService, MasterService masterService, ILogger<UserController> logger)
        : base(logger)
        {
            this.userService = userService;
            this.masterService = masterService;
            this.identityService = identityService;
        }

        [HttpGet("/api/users")]
        public async Task<ActionResult<ApiResultDTO<OffsetPagniationData<UserDTO>>>> GetUsers([FromQuery] ListUserFilteringRequestDto request)
        {
            var result = await userService.GetUsersAsync(request);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("/api/users/search")]
        public async Task<ActionResult<ApiResultDTO<List<UserDTO>>>> SearchUsers([FromQuery] SearchUserRequestDto request)
        {
            var result = await userService.SearchUsersAsync(request);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("/api/users/me")]
        public async Task<ActionResult<ApiResultDTO<MeDTO>>> GetMe()
        {
            var result = await userService.GetMeHandler();
            if (result.IsSucceed)
            {
                masterService.CheckFarmPoolInitJob(identityService.CurrentUserId);
                return Ok(result);
            }
            return BadRequest(result);
        }
        [AllowAnonymous]
        [HttpGet("/api/users/create-bot")]
        public async Task<ActionResult<ApiResultDTO<bool>>> CreateBot([FromQuery] CreateBotRequest request)
        {
            var result = new ApiResultDTO<bool>();
            await userService.CreateUserBot(request.Tier, request.TotalBot);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }

        // [HttpGet("/api/users/link-wallet")]
        // public async Task<ActionResult<ApiResultDTO<MeDTO>>> LinkWallet()
        // {
        //     var accessToken = Request.Headers["Authorization"];
        //     var result = await userService.LinkWallet(accessToken);
        //     if (result.IsSucceed)
        //         return Ok(result);
        //     return BadRequest(result);
        // }
    }
}