using System;

namespace StockQuoteAlert.QuoteApi
{
    public class StockQuoteSnapshot
    {
        public decimal RegularMarketPrice { get; set; }
        public DateTime RegularMarketTime { get; set; }
    }

    public class StockQuoteSeries
    {
        public StockQuoteSnapshot Data { get; set; }
        public string Symbol { get; set; }
    }

    public class StockQuoteResponse
    {
        public StockQuoteSeries[] Results { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
