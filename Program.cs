using StockQuoteAlert.Api;
using System;
using System.Threading.Tasks;

namespace StockQuoteAlert
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ParsedArguments parsedArgs = parseArgs(args);
            string asset = parsedArgs.Asset;
            decimal lowThreshold = parsedArgs.LowThreshold;
            decimal highThreshold = parsedArgs.HighThreshold;

            Client client = new Client();
            decimal currentQuote = decimal.MinValue;

            while (true)
            {
                decimal newQuote = await client.GetStockQuoteAsync(asset);
                if (newQuote != currentQuote)
                {
                    currentQuote = newQuote;
                    Console.WriteLine(currentQuote);
                }
            }
        }

        class ParsedArguments
        {
            public string Asset { get; set; }
            public decimal LowThreshold { get; set; }
            public decimal HighThreshold { get; set; }
        }

        private static ParsedArguments parseArgs(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("Invalid number of arguments. Usage: StockQuoteAlert <asset> <low> <high>");
            }

            decimal lowThreshold, highThreshold;

            string asset = args[0];
            try
            {
                lowThreshold = decimal.Parse(args[1]);
                highThreshold = decimal.Parse(args[2]);

                if (lowThreshold >= highThreshold)
                {
                    throw new ArgumentException("Low threshold must be less than high threshold.");
                }
            }
            catch (FormatException)
            {
                throw new ArgumentException("Low and high thresholds must be valid decimal numbers.");
            }

            return new ParsedArguments
            {
                Asset = asset,
                LowThreshold = lowThreshold,
                HighThreshold = highThreshold
            };
        }
    }
}
