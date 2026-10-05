using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StockQuoteAlert
{
    class StockQuoteSnapshot
    {
        public decimal RegularMarketPrice { get; set; }
    }

    class StockQuoteSeries
    {
        public StockQuoteSnapshot Data { get; set; }
    }

    class StockQuote
    {
        public StockQuoteSeries[] Results { get; set; }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            ParsedArguments parsedArgs = parseArgs(args);
            string asset = parsedArgs.Asset;
            decimal lowThreshold = parsedArgs.LowThreshold;
            decimal highThreshold = parsedArgs.HighThreshold;

            NameValueCollection quoteApiConfig = (NameValueCollection)ConfigurationManager.GetSection("quoteApi");

            string baseUrl = quoteApiConfig["baseUrl"];
            string token = quoteApiConfig["token"];
            string endpoint = quoteApiConfig["endpoint"];

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            for (int i = 0; i < 1; i++)
            {
                try
                {
                    await GetStockQuoteAsync(client, endpoint, asset);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching stock quote: {ex.Message}");
                    return;
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

        private async static Task GetStockQuoteAsync(HttpClient client, string endpoint, string asset)
        {
            string path = $"{endpoint}?symbols={asset}";
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                StockQuote result = await response.Content.ReadFromJsonAsync<StockQuote>();
                Console.WriteLine(result.Results[0].Data.RegularMarketPrice);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"asset {asset} not found.");
            }
            else
            {
                Console.WriteLine($"{response}");
            }
        }
    }
}
