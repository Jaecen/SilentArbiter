using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ExchangeService.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExchangeService.Exchange
{
	public class CryptoTrade : IExchange
	{
		readonly ApiUrlBuilder ApiUrl;
		readonly HttpClient HttpClient;

		public CryptoTrade()
		{
			ApiUrl = new ApiUrlBuilder();
			HttpClient = new System.Net.Http.HttpClient();
		}

		public OrderDepth GetMarketOrderDepth(CurrencyPair pair)
		{
			if(pair == null)
				throw new ArgumentNullException("pair");

			var requestUrl = ApiUrl.OrderBook(pair);

			var response = HttpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
			if(!response.Result.IsSuccessStatusCode)
				throw new PoloniexExchangeException(String.Format("API request failed with status code {0}", response.Result.StatusCode));

			JObject result;
			using(var responseStream = response.Result.Content.ReadAsStreamAsync().Result)
			using(var streamReader = new System.IO.StreamReader(responseStream))
			using(var jsonReader = new JsonTextReader(streamReader))
				result = JObject.Load(jsonReader);

			if(!String.IsNullOrEmpty((string)result["status"]))
				throw new CryptsyExchangeException("API response content returned failure");

			var buys = ParseOrderDepthFromResult(result, "bids");
			var sells = ParseOrderDepthFromResult(result, "asks");

			return new OrderDepth(pair, buys, sells);
		}

		Dictionary<decimal, decimal> ParseOrderDepthFromResult(JObject result, string tokenPath)
		{
			return result
				.SelectToken(tokenPath)
				.OfType<JArray>()
				.Select(order => order.Values<decimal>().ToArray())
				.Where(order => order.Length == 2)
				.ToDictionary(
					order => order[0],
					order => order[1]
				);
		}

		class ApiUrlBuilder
		{
			readonly Uri ApiBase;

			public ApiUrlBuilder()
			{
				ApiBase = new Uri("https://crypto-trade.com/api/1/");
			}

			public Uri OrderBook(CurrencyPair pair)
			{
				return new Uri(ApiBase, String.Format("depth/{0}_{1}", pair.Base, pair.Counter).ToLower());
			}
		}
	}
}