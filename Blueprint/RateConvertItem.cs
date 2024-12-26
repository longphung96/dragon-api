using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragon.Blueprints
{
    public class RateConvertItemConfig
    {
        public long Id { get; set; }
        public long ExchangeItem { get; set; }
        public long ReceivedItem { get; set; }
        public decimal Rate { get; set; }
        public bool Status { get; set; } = true;


    }
}
