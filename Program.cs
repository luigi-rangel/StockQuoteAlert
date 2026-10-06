using StockQuoteAlert.Mail;
using StockQuoteAlert.QuoteApi;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace StockQuoteAlert
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                ParsedArguments parsedArgs = ParseArgs(args);
                string asset = parsedArgs.Asset;
                decimal lowThreshold = parsedArgs.LowThreshold;
                decimal highThreshold = parsedArgs.HighThreshold;

                Console.WriteLine("Monitoring stock quote for asset {0} with thresholds {1} - {2}",
                    asset, lowThreshold, highThreshold);

                await RunApiLoop(asset, lowThreshold, highThreshold);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task RunApiLoop(string asset, decimal lowThreshold, decimal highThreshold)
        {
            QuoteApiClient quoteApiClient = new QuoteApiClient();
            decimal? currentQuote = null;

            while (true)
            {
                StockQuote newQuote = await quoteApiClient.GetStockQuoteAsync(asset);
                Console.WriteLine("{0}: {1}", newQuote.MarkedAt, newQuote.MarketPrice);

                if (newQuote.MarketPrice <= lowThreshold && !(currentQuote <= lowThreshold))
                {
                    MailClient mailClient = new MailClient();
                    AlertMail email = SetupBuyEmail(newQuote, lowThreshold);
                    mailClient.SendEmail(email);
                }
                else if (newQuote.MarketPrice >= highThreshold && !(currentQuote >= highThreshold))
                {
                    MailClient mailClient = new MailClient();
                    AlertMail email = SetupSellEmail(newQuote, highThreshold);
                    mailClient.SendEmail(email);
                }

                currentQuote = newQuote.MarketPrice;

                double nextCheck = Math.Max(0, (newQuote.MarkedAt.AddMinutes(2) - DateTime.Now).TotalMilliseconds);
                await Task.Delay((int)nextCheck);
            }
        }

        private static AlertMail SetupSellEmail(StockQuote newQuote, decimal highThreshold)
        {
            AlertMail email = SetupEmailEssentials();

            email.SetHighPriceAlert(newQuote, highThreshold);

            return email;
        }

        private static AlertMail SetupBuyEmail(StockQuote newQuote, decimal lowThreshold)
        {
            AlertMail email = SetupEmailEssentials();

            email.SetLowPriceAlert(newQuote, lowThreshold);

            return email;
        }

        private static AlertMail SetupEmailEssentials()
        {
            NameValueCollection mailInfoConfig = System.Configuration.
                ConfigurationManager.GetSection("mailInfo") as NameValueCollection;

            AlertMail email = new AlertMail();
            email.SetFrom(
                mailInfoConfig["fromName"],
                mailInfoConfig["fromAddress"]
            );
            email.SetTo(
                mailInfoConfig["toName"],
                mailInfoConfig["toAddress"]
            );
            return email;
        }

        class ParsedArguments
        {
            public string Asset { get; set; }
            public decimal LowThreshold { get; set; }
            public decimal HighThreshold { get; set; }
        }

        private static ParsedArguments ParseArgs(string[] args)
        {
            if (args.Length < 3)
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
