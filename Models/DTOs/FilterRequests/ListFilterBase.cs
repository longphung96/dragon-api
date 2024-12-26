using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace DragonAPI.Models.DTOs
{
    public class ListOffsetPagniationFiltering : FilterBase
    {
        [IgnoreFilter]
        public int Page { get; set; } = 1;
        [IgnoreFilter]
        public int Size { get; set; } = 20;
    }
}
