namespace DragonAPI.Models.Settings
{
    public class TemplateItemConfig
    {

        public class CommonMetaNftMetadata
        {
            public long nftTypeId { get; set; }
        }


        public long Id { get; set; }
        public string Entity { get; set; }
        public long? TypeId { get; set; }
        //public int? SealSlot { get; set; }
        public bool IsNFT { get; set; }
        public string ImageURL { get; set; }

        public object GetMetadata()
        {
            // if (Entity == "seal")
            // {
            //     return new SealMetadata
            //     {
            //         totalSlot = (int)SealSlot,
            //     };
            // }
            if (TypeId != null)
            {
                return new CommonMetaNftMetadata
                {
                    nftTypeId = (long)TypeId,
                };
            }
            return null;
        }
    }

    public class DropItemConfig
    {
        public TemplateItemConfig Item { get; set; }
        public int Quantity { get; set; }
    }
}