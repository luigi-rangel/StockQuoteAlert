using StockQuoteAlert.QuoteApi;
using System;

namespace StockQuoteAlert
{
    public class StockQuote
    {
        public string Asset { get; set; }
        public decimal MarketPrice { get; set; }
        public DateTime MarkedAt { get; set; }
        public DateTime RequestedAt { get; set; }

        public static StockQuote FromResult(StockQuoteResponse response)
        {
            return new StockQuote
            {
                Asset = response.Results[0].Symbol,
                MarketPrice = response.Results[0].Data.RegularMarketPrice,
                MarkedAt = response.Results[0].Data.RegularMarketTime.ToLocalTime(),
                RequestedAt = response.RequestedAt.ToLocalTime()
            };
        }
    }
}
