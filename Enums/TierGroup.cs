using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonAPI.Enums
{
    public enum TierGroup
    {
        Undefined = -1,
        Bronze,
        Silver,
        Gold,
        Diamond,
    }

}
