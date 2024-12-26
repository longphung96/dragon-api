using DotNetCore.CAP;
using MediatR;
using DragonAPI.IntegrationEvent.Events;
namespace DragonAPI.Application.Commands
{
    public class RequestGenerateAssetCommand : IRequest
    {
        public string Id { get; set; }
        public string NftId { get; set; }
        public string Entity { get; set; }
        public long TypeId { get; set; }
        public object EntityData { get; set; }
    }

    public class RequestGenerateAssetCommandHandler : IRequestHandler<RequestGenerateAssetCommand>
    {
        private readonly ICapPublisher eventBus;
        public RequestGenerateAssetCommandHandler(ICapPublisher eventBus)
        {
            this.eventBus = eventBus;
        }

        public async Task<Unit> Handle(RequestGenerateAssetCommand request, CancellationToken cancellationToken)
        {
            await eventBus.PublishAsync(EventNameConst.GenerateNftAssetsRequestEvent, new AssetsGenerationIntegrationEvent
            {
                Id = request.Id,
                NftId = request.NftId,
                Entity = request.Entity,
                TypeId = request.TypeId,
                DetailedData = request.EntityData,
            });
            return Unit.Value;
        }
    }
}