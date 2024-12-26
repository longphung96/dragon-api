using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DragonAPI.Common;
using DragonAPI.Extensions;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;


namespace DragonAPI.Controllers
{
    [Authorize]
    public class InventoryController : BaseController<InventoryController>
    {
        private readonly InventoryService inventoryService;
        private readonly MasterService masterService;
        private readonly IdentityService identityService;
        public InventoryController(ILogger<InventoryController> logger, InventoryService inventoryService, MasterService masterService, IdentityService identityService) : base(logger)
        {
            this.inventoryService = inventoryService;
            this.masterService = masterService;
            this.identityService = identityService;
        }
        [HttpGet("api/inventory/claims")]
        public async Task<ActionResult<ApiResultDTO<bool>>> ClaimItem([FromQuery] string requestId)
        {
            var result = new ApiResultDTO<bool>();
            result.Data = await inventoryService.ClaimItems(requestId);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        // [HttpGet("api/inventory/send")]
        // public async Task<ActionResult<ApiResultDTO<bool>>> GetMasterInfo()
        // {
        //     var result = new ApiResultDTO<bool>();
        //     await inventoryService.ProcessSendDailyRankingReward();
        //     return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        // }
        // [HttpGet("api/masters")]
        // public async Task<ActionResult<ApiResultDTO<OffsetPagniationData<MasterDTO>>>> GetListMasterInfo([FromQuery] ListMasterFilteringRequestDto request)
        // {
        //     var result = await masterService.GetMastersAsync(request);
        //     return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        // }
        // [HttpGet("api/masters/items")]
        // public async Task<ActionResult<ApiResultDTO<OffsetPagniationData<ItemMasterDTO>>>> GetListItems([FromQuery] ListItemMasterFilteringRequestDto request)
        // {
        //     var result = await masterService.GetItemMastersAsync(request);
        //     return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        // }
        // [HttpGet("api/masters/items/{id}")]
        // public async Task<ActionResult<ApiResultDTO<ItemMasterDTO>>> GetDetailItems([FromRoute] string id)
        // {
        //     var result = await masterService.GetDetailItems(id);
        //     return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        // }
        // [HttpGet("/api/masters/change-avatar")]
        // public async Task<ActionResult<ApiResultDTO<string>>> ChangeAvatar(string avatarid)
        // {
        //     return await masterService.ChangeAvatar(avatarid);
        // }

        // // [HttpGet("api/masters/get-achievementstats-ranking")]
        // // public async Task<ActionResult<ApiResultDTO<RankingTopScore>>> GetAchievementStatsRanking([FromQuery] AchievementStatsType achievementType)
        // // {
        // //     var result = await _gameService.GetAchievementStatsRanking(achievementType);
        // //     return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        // // }
        // // [HttpGet("api/masters/get-settings")]
        // // public async Task<ActionResult<ApiResultDTO<SettingsDTO>>> GetMySettings()
        // // {
        // //     var result = new ApiResultDTO<SettingsDTO>();
        // //     result.Data = await _gameService.GetMySettings();
        // //     return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        // // }

        // [HttpGet("/api/masters/idle-farming-pools")]
        // public async Task<ActionResult<ApiResultDTO<List<AFKIdleItemPoolDTO>>>> GetIdleFarmingPools()
        // {
        //     return Ok(await masterService.GetIdleFarmingPools());
        // }


        // // [HttpGet("/api/masters/get-offline-reward")]
        // // public async Task<ActionResult<ApiResultDTO<OfflineRewardDTO>>> GetOfflineReward()
        // // {
        // //     return await masterService.GetOfflineReward();
        // // }


        // // [HttpGet("api/masters/quick-claim-offline-reward")]
        // // public async Task<ActionResult<ApiResultDTO<OfflineRewardDTO>>> QuickClaimOfflineReward()
        // // {
        // //     return await masterService.QuickClaimOfflineReward();
        // // }
        // [HttpPost("api/masters/update-tutorial")]
        // public async Task<ActionResult<ApiResultDTO<List<int>>>> UpdateTutorial(int stepId)
        // {
        //     return await masterService.UpdateTutorial(stepId);
        // }
        // [HttpPost("api/masters/claim-offline-reward")]
        // public async Task<ActionResult<ApiResultDTO<bool>>> OfflineReward([FromBody] ClaimOfflineRewardRequest param)
        // {
        //     return await masterService.ClaimOfflineReward(param);
        // }
        // [HttpPost("api/masters/with-draw")]
        // public async Task<ActionResult<ApiResultDTO<string>>> MasterWithdrawETH([FromBody] ETHWithdrawRequest param)
        // {
        //     return await masterService.MasterWithdrawETH(identityService.CurrentUserId, param);
        // }
        // [HttpPost("api/masters/upgrade-vip")]
        // public async Task<ActionResult<ApiResultDTO<bool>>> UpgradeVIP([FromBody] BuyPackageRequest request)
        // {
        //     return await masterService.UpgradeVIP(request);
        // }
    }
}
