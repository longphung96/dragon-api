using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DragonAPI.Common;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;

namespace DragonAPI.Controllers
{
    [Authorize]
    public class WorldmapController : BaseController<WorldmapController>
    {
        private readonly WorldmapService worldmapService;
        public WorldmapController(WorldmapService worldmapService, ILogger<WorldmapController> logger)
        : base(logger)
        {
            this.worldmapService = worldmapService;
        }

        [HttpGet("/api/worldmap/stages")]
        public async Task<ActionResult<ApiResultDTO<IEnumerable<StageDTO>>>> GetStages([FromQuery] string mapId)
        {
            var result = await worldmapService.GetStages(mapId);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }

        [HttpGet("/api/worldmap/maps")]
        public async Task<ActionResult<ApiResultDTO<IEnumerable<MapDTO>>>> GetMaps()
        {
            var result = await worldmapService.GetMaps();
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }

        [HttpGet("/api/worldmap/open-time-last-stage")]
        public async Task<ActionResult<ApiResultDTO<IEnumerable<LastStageTimeOpenInfoDTO>>>> GetOpenTimeLastStage([FromQuery] string stageId)
        {
            var result = await worldmapService.GetOpenTimeLastStage(stageId);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
    }
}