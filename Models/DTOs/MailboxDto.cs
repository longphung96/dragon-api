using DragonAPI.IntegrationEvent.Events;

namespace DragonAPI.Models.DTOs
{
    public class MailDto
    {
        public string Id { get; set; }
        [Obsolete("Will be removed")]
        public string WalletId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public MailboxType Type { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
}