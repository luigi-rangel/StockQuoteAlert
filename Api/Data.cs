namespace StockQuoteAlert.Api
{
    public class StockQuoteSnapshot
    {
        public decimal RegularMarketPrice { get; set; }
    }

    public class StockQuoteSeries
    {
        public StockQuoteSnapshot Data { get; set; }
    }

    public class StockQuote
    {
        public StockQuoteSeries[] Results { get; set; }
    }
}
