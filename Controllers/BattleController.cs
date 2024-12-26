using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DragonAPI.Common;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonAPI.Controllers
{
    [Authorize]
    public class BattleController : BaseController<BattleController>
    {
        private readonly GameService _gameService;
        public BattleController(ILogger<BattleController> logger, GameService gameService) : base(logger)
        {
            _gameService = gameService;
        }
        [HttpGet("api/battle/formation")]
        public async Task<ActionResult<ApiResultDTO<string[]>>> GetBattleTeam([FromQuery] BattleType battleTeamType)
        {
            var result = await _gameService.GetBattleTeam(battleTeamType);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }

        [HttpPost("api/battle/formation")]
        public async Task<ActionResult<ApiResultDTO<string>>> SetupTeam(SetupTeamRequest request)
        {
            var result = await _gameService.SetupTeam(request);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        [HttpGet("/api/battle/pve/join-battle")]
        public async Task<ActionResult<ApiResultDTO<bool?>>> EnterPVEBattle([FromQuery] string stageId)
        {
            var result = await _gameService.EnterPVEBattle(stageId);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        [HttpGet("/api/battle/pvp/rank")]
        public async Task<ActionResult<ApiResultDTO<RankPositionResponse>>> GetRank()
        {
            var result = await _gameService.GetTopRank();
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        [HttpGet("/api/battle/pvp/arena-rank")]
        public async Task<ActionResult<ApiResultDTO<ArenaRankResponse>>> GetArenaRank()
        {
            var result = await _gameService.GetArena();
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        [HttpGet("/api/battle/pvp/get-user-arena")]
        public async Task<ActionResult<ApiResultDTO<MyArenaRank>>> GetUserArena()
        {
            var result = await _gameService.GetUserArena();
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        [HttpGet("/api/battle/pvp/history-battle")]
        public async Task<ActionResult<ApiResultDTO<List<PvPHistoryDTO>>>> GetHistoryBattles([FromQuery] PvPHistoryRequest param)
        {
            var result = await _gameService.GetHistoryBattle(param);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }
        [HttpGet("/api/battle/pvp/challenge-battle")]
        public async Task<ActionResult<ApiResultDTO<bool?>>> EnterPVPBattle([FromQuery] string EnemyPlayerId)
        {
            var result = await _gameService.EnterPVPBattle(EnemyPlayerId);
            return result.IsSucceed == true ? Ok(result) : BadRequest(result);
        }

        // [HttpGet("/api/battle/pve/test-hub")]
        // public async Task<ActionResult<ApiResultDTO<bool?>>> TestHub()
        // {
        //     var result = new ApiResultDTO<bool?>();
        //     await _gameService.TestConnectionHub();
        //     return Ok(result);
        // }

        // [HttpGet("api/battle/battlepass-info")]
        // public async Task<ActionResult<ApiResultDTO<IEnumerable<BattlePassDTO>>>> GetBattlePassInfo()
        // {
        //     var result = new ApiResultDTO<IEnumerable<BattlePassDTO>>();
        //     result.Data = await _gameService.GetBattlePassInfo();
        //     return result.IsSucceed() ? Ok(result) : BadRequest(result);
        // }
        // [HttpGet("api/battle/leaderboards")]
        // public async Task<ActionResult<ApiResultDTO<RankingTopScore>>> GetAchievementBattlePassRanking([FromQuery] AchievementStatsType achievementType, [FromQuery] string battlePassPoint)
        // {
        //     var result = await _gameService.GetAchievementStatsRanking(achievementType, battlePassPoint);
        //     return result.IsSucceed() ? Ok(result) : BadRequest(result);
        // }
    }
}
