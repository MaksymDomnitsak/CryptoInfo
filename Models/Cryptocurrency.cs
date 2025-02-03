using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfo.Models
{
    public class Cryptocurrency
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string MarketCapUsd { get; set; }
        public string PriceUsd { get; set; }
        public string Supply { get; set; }
    }
}
