using System;
using System.Collections.Generic;

using DragonAPI.Enums;

namespace DragonAPI.Models.DAOs
{



    public class CurrencyTransactionDAO : BaseDAO
    {

        public string? MainUserId { get; set; }

        public string? WalletId { get; set; }

        public TransactionType TransactionType { get; set; }

        public long ItemId { get; set; }

        public decimal Amount { get; set; }
        public string? TxHash { get; set; }

        public string? TxId { get; set; }

        public string? Description { get; set; }

        public Status Status { get; set; }
    }
    public class WithDrawVerificationDAO : BaseDAO
    {

        public string? CurrencyTransactionId { get; set; }

        public string? ReviewerId { get; set; }

        public string? TxId { get; set; }

        public string? Description { get; set; }

        public StatusVerify Status { get; set; }
    }

}