using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DragonAPI.Common;
using DragonAPI.Extensions;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.DTOs;
using DragonAPI.Services;
// using TrueSight.Lite.Common;

namespace DragonAPI.Controllers
{
    public class DragonController : BaseController<DragonController>
    {
        private readonly DragonService rongosService;
        private readonly IdentityService identityService;
        public DragonController(
            DragonService rongosService,
            IdentityService identityService,
            ILogger<DragonController> logger
            ) : base(logger)
        {
            this.rongosService = rongosService;
            this.identityService = identityService;
        }

        [AllowAnonymous]
        [HttpGet("/api/rongos-service/rongoses")]
        public async Task<ActionResult<ApiResultDTO<OffsetPagniationData<DragonDTO>>>> List([FromQuery] DragonFilter request)
        {
            // if (!ModelState.IsValid)
            //     throw new BindException(ModelState);

            var result = await rongosService.List(request);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("/api/rongos-service/rongoses/my_rongos")]
        public async Task<ActionResult<ApiResultDTO<OffsetPagniationData<DragonDTO>>>> GetMyDragons([FromQuery] MyDragonFilter request)
        {
            var response = await rongosService.GetMyDragons(request);
            return Ok(response);
        }

        // [HttpGet("/api/rongos-service/rongoses/ownedClasses")]
        // public async Task<ActionResult<ApiResultDTO<List<OwnClassMapDto>>>> GetUniqueClasses([FromQuery] bool isOffchain = true)
        // {
        //     var response = await rongosService.GetUniqueClassGroups(isOffchain);
        //     return Ok(response);
        // }

        [AllowAnonymous]
        [HttpGet("/api/rongos-service/rongoses/count")]
        public async Task<ActionResult<ApiResultDTO<OffsetPagniationData<DragonDTO>>>> CountDragones([FromQuery] DragonFilter request)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new Exception(message);
            }

            var result = await rongosService.Count(request);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }


        [AllowAnonymous]
        [HttpGet("/api/rongos-service/rongoses/{id}")]
        public async Task<ActionResult<ApiResultDTO<DragonDTO>>> Get([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new Exception(message);
            }

            var result = await rongosService.Get(id);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }


        [HttpGet("/api/rongos-service/rongoses/created")]
        public async Task<ActionResult<ApiResultDTO<DragonDTO>>> Created([FromQuery] DragonCreateRequestDto param)
        {
            var birth = new BirthCreateETO
            {
                Owner = param.WalletId,
                TypeId = param.Class
            };
            await rongosService.AdminCreatedDragon(birth);
            return Ok(true);
        }

        [HttpPut("/api/rongos-service/rongoses/{id}/rongos-level-up")]
        public async Task<ActionResult<ApiResultDTO<int>>> DragonLevelUp([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new Exception(message);
            }

            var result = await rongosService.DragonLevelUp(id);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("/api/rongos-service/rongoses/{id}/upgrade-farm-level")]
        public async Task<ActionResult<ApiResultDTO<int>>> UpgradeFarmLevel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new Exception(message);
            }

            var result = await rongosService.DragonFarmLevelUp(id);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("/api/rongos-service/rongoses/rongos-open-package")]
        public async Task<ActionResult<ApiResultDTO<int>>> DragonMergeFragment([FromBody] DragonOpenRequestDto param)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new Exception(message);
            }

            var result = await rongosService.DragonOpenPackage(param);
            if (result.IsSucceed)
                return Ok(result);
            return BadRequest(result);
        }

        // [HttpPut("/api/rongos-service/rongoses/{id}/unlockBodypart")]
        // public async Task<ActionResult<ApiResultDTO<string>>> UnlockBodypart([FromRoute] string id, [FromBody] DragonUnlockBodypartRequestDTO request)
        // {
        //     if (!ModelState.IsValid)
        //         throw new BindException(ModelState);

        //     var result = await rongosService.UnlockBodypart(id, request.Bodypart);
        //     if (result.IsSucceed)
        //         return Ok(result);
        //     return BadRequest(result);
        // }

        // [HttpPut("/api/rongos-service/rongoses/{id}/unlockBodypartSkill")]
        // public async Task<ActionResult<ApiResultDTO<string>>> UnlockBodypartSkill([FromRoute] string id, [FromBody] DragonUnlockBodypartRequestDTO request)
        // {
        //     if (!ModelState.IsValid)
        //         throw new BindException(ModelState);

        //     var result = await rongosService.UnlockBodypartSkill(id, request.Bodypart);
        //     if (result.IsSucceed)
        //         return Ok(result);
        //     return BadRequest(result);
        // }

        [HttpPost("/api/rongos-service/rongoses/{id}/change-name")]
        public async Task<ActionResult<ApiResultDTO<bool>>> ChangeDragonName([FromRoute] string id, [FromBody] ChangeDragonNameRequest request)
        {
            var response = await rongosService.ChangeName(id, request.Name);
            return Ok(response);
        }

        // [HttpPost("/api/rongos-service/rongoses/{id}/worklplaceCreating")]
        // public async Task<ActionResult<ApiResultDTO<bool>>> WorkplaceCreate([FromRoute] string id, [FromBody] WorkplaceDragonCreate request)
        // {
        //     var response = await rongosService.PutToWorkplace(id, request.ListingDuration, request.LeasingDuration, request.Rss);
        //     return Ok(response);
        // }

        // [HttpPost("/api/rongos-service/rongoses/{id}/worklplaceLeasing")]
        // public async Task<ActionResult<ApiResultDTO<bool>>> WorkplaceLease([FromRoute] string id)
        // {
        //     var response = await rongosService.LeaseInWorkplace(id);
        //     return Ok(response);
        // }
    }
}