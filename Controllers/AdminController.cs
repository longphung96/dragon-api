using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DragonAPI.Common;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;
using DragonAPI.Extensions;
using DragonAPI.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace DragonAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // [Authorize(Roles = "Admin-Dragon-Role")]
    public class AdminController : BaseController<AdminController>
    {
        private readonly MasterService masterService;
        private readonly IAdminService adminService;
        public AdminController(MasterService masterService, IAdminService adminService, ILogger<AdminController> logger)
        : base(logger)
        {
            this.masterService = masterService;
            this.adminService = adminService;
        }

        [HttpGet("/api/rongos-service/admin/list-withdraw-transaction")]
        public async Task<ActionResult<ApiResultDTO<OffsetPagniationData<WithDrawVerificationDTO>>>> GetList([FromQuery] WithDrawVerificationFilterDto request)
        {
            var response = await adminService.GetList(request);
            return Ok(response);
        }
        [HttpPost("/api/admin/verify-withdraw-transaction")]
        public async Task<ActionResult<ApiResultDTO<bool>>> VerifyWithDraw([FromBody] ETHWithdrawVerifyRequest param)
        {
            var result = await adminService.VerifyWithDraw(param);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }

    }
}