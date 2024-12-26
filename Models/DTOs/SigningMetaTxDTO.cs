using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DragonAPI.Configurations;
using DragonAPI.IntegrationEvent.Events;
using DragonAPI.Models.Entities;

namespace DragonAPI.Models.DTOs
{
    public class SigningResponseDTO
    {
        public MetaTxDTO MetaTx { get; set; }
        public string Signature { get; set; }
    }

    public class MetaTxDTO
    {
        public string Operator { get; set; }
        public string From { get; set; }
        public string Nonce { get; set; }
        public long ExpiredAt { get; set; }
        public string Data { get; set; }
    }
}