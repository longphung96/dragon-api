using System.Text.Json.Serialization;
using AutoFilterer.Types;
using DragonAPI.Enums;
namespace DragonAPI.Models.DTOs
{

    public class CurrencyTransactionDTO : BaseDTO
    {
        [JsonPropertyName("mainUserId")]
        public string MainUserId { get; set; }
        [JsonPropertyName("wallet_id")]
        public string WalletId { get; set; }
        [JsonPropertyName("transaction_type")]
        public string Type { get; set; }
        [JsonPropertyName("item_id")]
        public long ItemId { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("tx_hash")]
        public long TxHash { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("status")]
        public long Status { get; set; }
    }
    public class WithDrawVerificationDTO : BaseDTO
    {
        [JsonPropertyName("currency_transaction_id")]
        public string CurrencyTransactionId { get; set; }
        [JsonPropertyName("reviewer_id")]
        public string? ReviewerId { get; set; }
        [JsonPropertyName("tx_id")]
        public string TxId { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("status")]
        public StatusVerify Status { get; set; }
    }
    public class WithDrawVerificationFilterDto : ListOffsetPagniationFiltering
    {
        public string[]? CurrencyTransactionId { get; set; }
        public string[]? TxId { get; set; }
        public string? ReviewerId { get; set; }
        public StatusVerify Status { get; set; }
    }

    public class ETHWithdrawRequest
    {
        public string WalletId { get; set; }
        public long ItemReq { get; set; }
        public decimal Amount { get; set; }
    }
    public class ETHWithdrawVerifyRequest
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}