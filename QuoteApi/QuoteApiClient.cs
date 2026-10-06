using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockQuoteAlert.QuoteApi
{
    public class QuoteApiClient
    {
        private readonly HttpClient httpClient;
        private readonly string endpoint;

        public QuoteApiClient()
        {
            NameValueCollection quoteApiConfig = (NameValueCollection)ConfigurationManager.GetSection("quoteApi");
            string baseUrl = quoteApiConfig["baseUrl"];
            string token = quoteApiConfig["token"];

            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            this.endpoint = quoteApiConfig["endpoint"];
        }

        public async Task<StockQuote> GetStockQuoteAsync(string asset)
        {
            string path = $"{this.endpoint}?symbols={asset}";
            HttpResponseMessage response = await this.httpClient.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                StockQuoteResponse result = await response.Content.ReadFromJsonAsync<StockQuoteResponse>();
                return StockQuote.FromResult(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"Asset {asset} not found.");
            }
            else
            {
                throw new Exception($"Error retrieving stock quote for {asset}.");
            }
        }
    }
}
