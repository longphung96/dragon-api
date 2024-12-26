using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DragonAPI.Configurations;
using DragonAPI.Enums;
using DragonAPI.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Confluent.Kafka;

namespace DragonAPI.Models.DAOs
{
    public interface IDragonBase
    {
        long Id { get; set; }
        ClassType Class { get; set; }
        ushort Level { get; set; }
        int Rarity { get; set; }
        List<Bodypart> Bodyparts { get; set; }
    }
    public class DragonDAO : BaseDAO, IDragonBase
    {

        [Column("wallet_id")]
        public string WalletId { get; set; }
        [Column("nft_id")]
        public ulong NftId { get; set; }
        [Column("class")]
        public ClassType Class { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("level")]
        public UInt16 Level { get; set; } = 1;
        [Column("rarity")]
        public int Rarity { get; set; } = 0;
        [Column("exp")]
        public double Exp { get; set; } = 0;
        [Column("birthday")]
        public DateTime Birthday { get; set; }
        [Column("bodyparts")]
        public List<Bodypart> Bodyparts { get; set; }
        [Column("cooldown_end_at")]
        public DateTime? CooldownEndAt { get; set; } = DateTime.UtcNow;
        [Column("dragon_farm_level")]
        public long DragonFarmLevel { get; set; } = 0;
        [Column("sale_status")]
        public SaleStatus SaleStatus { get; set; } = SaleStatus.NotSale;
        [Column("placed_order_info")]
        public PlacedOrderInfo? PlacedOrderInfo { get; set; }
        [Column("asset_generate_at")]
        public long AssetGeneratedAt { get; set; } = 0;
        [Column("is_off_chain")]
        public bool IsOffChain { get; set; }
        [Column("is_f2p")]
        public bool IsF2P { get; set; } = false;
        public void Reset(GameConfigs configs)
        {
            Level = 1;
            Exp = 0;
            Bodyparts.ForEach(bp =>
            {
                bp.Level = 1;
                // // TODO: Must be removed because of blockchain does not expose feature to unlock skill for onchain dragons
                // bp.SkillUnlocked = false;
            });
            // RefreshIsAbleToMate(configs);
        }

    }

    public class PlacedOrderInfo
    {
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal PriceStart { get; set; } = -1;
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal PriceEnd { get; set; } = -1;
        public DateTime ListedAt { get; set; } = DateTime.UtcNow;
        public DateTime ListedTo { get; set; } = DateTime.UtcNow;
    }

    public class LendingInfo
    {
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Rss { get; set; } = -1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime EndAt { get; set; } = DateTime.UtcNow;
        public long BorrowDuration { get; set; } = 0;
        public DateTime? BorrowedAt { get; set; } = null;
        public string Borrower { get; set; } = string.Empty;
    }

}