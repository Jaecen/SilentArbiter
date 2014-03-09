using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ExchangeService.Market;
using Newtonsoft.Json.Linq;

namespace ExchangeService.Exchange
{
	public class Cryptsy : IExchange
	{
		readonly ApiUrlBuilder ApiUrl;
		readonly HttpClient HttpClient;
		readonly Dictionary<CurrencyPair, int> MarketLookup;

		public Cryptsy()
		{
			ApiUrl = new ApiUrlBuilder();
			HttpClient = new System.Net.Http.HttpClient();
			MarketLookup = new Dictionary<CurrencyPair, int>
			{
				{ CurrencyPair.DrkBtc, 155 },
				{ CurrencyPair.UtcBtc, 163 },
			};
		}

		public OrderDepth GetMarketOrderDepth(CurrencyPair pair)
		{
			if(pair == null)
				throw new ArgumentNullException("pair");

			if(!MarketLookup.ContainsKey(pair))
				throw new CryptsyExchangeException("Unsupported currency pair");

			var marketId = MarketLookup[pair];
			var requestUrl = ApiUrl.SingleMarketData(marketId);

			var response = HttpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
			if(!response.Result.IsSuccessStatusCode)
				throw new CryptsyExchangeException(String.Format("API request failed with status code {0}", response.Result.StatusCode));

			var result = response.Result.Content.ReadAsAsync<JObject>().Result;
			if(!(bool)result["success"])
				throw new CryptsyExchangeException("API response content returned failure");

			var buys = ParseOrderDepthFromResult(result, String.Format("return.markets.{0}.buyorders", pair.Base.Code.ToUpper()));
			var sells = ParseOrderDepthFromResult(result, String.Format("return.markets.{0}.sellorders", pair.Base.Code.ToUpper()));

			return new OrderDepth(pair, buys, sells);
		}

		Dictionary<decimal, decimal> ParseOrderDepthFromResult(JObject result, string tokenPath)
		{
			return result
				.SelectToken(tokenPath)
				.Select(token => new
				{
					Price = (decimal?)token["price"],
					Quantity = (decimal?)token["quantity"],
					Total = (decimal?)token["total"],
				})
				.Where(order => order.Price.HasValue)
				.Where(order => order.Quantity.HasValue)
				.ToDictionary(
					order => order.Price.Value,
					order => order.Quantity.Value
				);
		}

		class ApiUrlBuilder
		{
			readonly Uri ApiBase;

			public ApiUrlBuilder()
			{
				ApiBase = new Uri("http://pubapi.cryptsy.com/api.php");
			}

			public Uri SingleMarketData(int marketId)
			{
				return new Uri(ApiBase, String.Format("?method=singlemarketdata&marketid={0}", marketId));
			}
		}
	}
}