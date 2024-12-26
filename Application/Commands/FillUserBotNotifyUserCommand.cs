using MediatR;
using Microsoft.AspNetCore.SignalR;

using DragonAPI.Models.DTOs;
using DragonAPI.Services;

namespace DragonAPI.Application.Commands
{
    public class FillUserBotNotifyUserCommand : IRequest
    {
        public int Tier { get; set; }
        public string UserId { get; set; }
    }

    public class FillUserBotNotifyUserCommandHandler : IRequestHandler<FillUserBotNotifyUserCommand>
    {

        private readonly ILogger<FillUserBotNotifyUserCommandHandler> logger;
        private readonly UserService userService;

        public FillUserBotNotifyUserCommandHandler(ILogger<FillUserBotNotifyUserCommandHandler> logger, UserService userService)
        {

            this.logger = logger;
            this.userService = userService;
        }

        public async Task<Unit> Handle(FillUserBotNotifyUserCommand request, CancellationToken cancellationToken)
        {



            await userService.ProcessFillUserRank(request.UserId, request.Tier);
            return Unit.Value;
        }
    }
}