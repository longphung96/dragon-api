using System;
using System.Text.Json.Serialization;
using DragonAPI.Common;

namespace DragonAPI.Models.DTOs
{

    public class ClaimableItemDto
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
        public long Type { get; set; }
        public long Amount { get; set; } = 0;
        public string Entity { get; set; }

    }
}