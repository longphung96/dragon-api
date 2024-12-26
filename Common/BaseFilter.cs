using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace DragonAPI.Common
{
    public class BaseFilter : FilterBase
    {
        [IgnoreFilter]
        public int Page { get; set; } = 1;
        [IgnoreFilter]
        public int Size { get; set; } = 20;
    }
}
