using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DragonAPI.Common;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;
using static DragonAPI.Services.MarketPlaceService;

namespace DragonAPI.Controllers
{
    [Authorize]
    public class MarketPlaceController : BaseController<MarketPlaceController>
    {
        private readonly IMarketPlaceService marketPlaceService;
        public MarketPlaceController(IMarketPlaceService marketPlaceService, ILogger<MarketPlaceController> logger)
        : base(logger)
        {
            this.marketPlaceService = marketPlaceService;
        }

        [HttpPost("/api/market/buy-item")]
        public async Task<ActionResult<ApiResultDTO<bool>>> BuyItem([FromBody] BuyItemRequest param)
        {
            var result = await marketPlaceService.BuyItem(param);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        [HttpPost("/api/market/buy-package")]
        public async Task<ActionResult<ApiResultDTO<bool>>> BuyPackage([FromBody] BuyPackageRequest param)
        {
            var result = await marketPlaceService.BuyPackage(param);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }

        [HttpPost("/api/market/faucetRss")]
        public async Task<ActionResult<ApiResultDTO<FaucetResultDTO>>> FaucetRss()
        {
            var result = await marketPlaceService.FaucetRss();
            return Ok(result);
        }
    }
}