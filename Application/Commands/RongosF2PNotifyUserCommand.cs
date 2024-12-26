using MediatR;
using Microsoft.AspNetCore.SignalR;

using DragonAPI.Models.DTOs;
using DragonAPI.Services;

namespace DragonAPI.Application.Commands
{
    public class DragonF2PNotifyUserCommand : IRequest
    {
        public string EventName { get; set; }
        public string UserId { get; set; }
    }

    public class DragonF2PNotifyUserCommandHandler : IRequestHandler<DragonF2PNotifyUserCommand>
    {

        private readonly ILogger<DragonF2PNotifyUserCommandHandler> logger;
        private readonly DragonService rongosService;

        public DragonF2PNotifyUserCommandHandler(ILogger<DragonF2PNotifyUserCommandHandler> logger, DragonService rongosService)
        {

            this.logger = logger;
            this.rongosService = rongosService;
        }

        public async Task<Unit> Handle(DragonF2PNotifyUserCommand request, CancellationToken cancellationToken)
        {


            // Deprecated, keep logic temporary for old events
            switch (request.EventName)
            {
                case ("CreateDragon"):
                    await rongosService.ProcessCreateDragonF2P(request.UserId);
                    break;
                default:
                    logger.LogWarning("not in special old events");
                    return Unit.Value;
            }
            return Unit.Value;
        }
    }
}