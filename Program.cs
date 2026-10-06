using StockQuoteAlert.Mail;
using StockQuoteAlert.QuoteApi;
using System;
using System.Collections.Specialized;
using System.Threading;
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

            Console.WriteLine("Monitoring stock quote for asset {0} with thresholds {1} - {2}", 
                asset, lowThreshold, highThreshold);

            NameValueCollection mailInfoConfig = System.Configuration.
                ConfigurationManager.GetSection("mailInfo") as NameValueCollection;

            AlertMail message = new AlertMail();
            message.SetFrom(
                mailInfoConfig["fromName"],
                mailInfoConfig["fromAddress"]
            );
            message.SetTo(
                mailInfoConfig["toName"],
                mailInfoConfig["toAddress"]
            );

            MailClient mailClient = new MailClient();

            QuoteApiClient client = new QuoteApiClient();
            decimal? currentQuote = null;

            while (true)
            {
                StockQuote newQuote = await client.GetStockQuoteAsync(asset);
                if (newQuote.MarketPrice != currentQuote)
                {
                    Console.WriteLine("{0}: {1}", newQuote.MarkedAt, newQuote.MarketPrice);

                    if (newQuote.MarketPrice <= lowThreshold && !(currentQuote <= lowThreshold))
                    {
                        message.SetLowPriceAlert(newQuote, lowThreshold);
                        mailClient.SendEmail(message);
                    }
                    else if (newQuote.MarketPrice >= highThreshold && !(currentQuote >= highThreshold))
                    {
                        message.SetHighPriceAlert(newQuote, highThreshold);
                        mailClient.SendEmail(message);
                    }

                    currentQuote = newQuote.MarketPrice;
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
