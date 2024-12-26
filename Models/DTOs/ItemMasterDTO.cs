using Microsoft.AspNetCore.Mvc;
using AutoFilterer.Attributes;

namespace DragonAPI.Models.DTOs
{
    public class ItemMasterDTO : BaseDTO
    {
        public string MainUserId { get; set; }
        public string WalletId { get; set; }
        public string Name { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }
        public string Description { get; set; }
    }
    public class ListItemMasterFilteringRequestDto : ListOffsetPagniationFiltering
    {
        [IgnoreFilter]
        public string[]? WalletId { get; set; }
        public long[]? ItemId { get; set; }
        public string MainUserId { get; set; }

    }
}