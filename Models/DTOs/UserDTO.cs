using AutoFilterer.Attributes;
using AutoFilterer.Types;
using Microsoft.AspNetCore.Mvc;
using DragonAPI.Common;
using DragonAPI.Models.Cache;
using DragonAPI.Models.DAOs;
using System.Text.Json.Serialization;

namespace DragonAPI.Models.DTOs
{
    public class UserDTO
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("username")]
        public string UserName { get; set; }
        // [JsonPropertyName("offchain_wallet_id")]
        // public string OffchainWalletId { get; set; }
        // [JsonPropertyName("onchain_wallet_id")]
        // public string[] OnchainWalletIds { get; set; }
    }

    public class UserFilterDTO : BaseFilter
    {
        [FromQuery(Name = "id")]
        public string[] UserId { get; set; }
    }

    public class SearchUserRequestDTO : FilterBase
    {
        [FromQuery(Name = "text")]
        [CompareTo(typeof(ToLowerContainsComparisonAttribute), /*nameof(UserDAO.OffchainWalletId), nameof(UserDAO.OnchainWalletIds),*/ nameof(UserDAO.UserName), nameof(UserDAO.UserId), nameof(UserDAO.MainUserId))]
        public string Search { get; set; }
    }



    public class UserActionDTO
    {
        public string RequestId { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public string Action { get; set; }
        public ActionState State { get; set; } = ActionState.Pending;
        public object Payload { get; set; }
    }

}
