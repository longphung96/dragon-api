namespace DragonAPI.IntegrationEvent.Events
{
    public class AssetsGenerationIntegrationEvent
    {
        public string Id { get; set; }
        public string NftId { get; set; }
        public string Entity { get; set; }
        public long? TypeId { get; set; }
        public object DetailedData { get; set; }
        public long RenderedTime { get; set; }
    }
}
