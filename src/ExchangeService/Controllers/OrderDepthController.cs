using System.Web.Http;
using ExchangeService.Exchange;
using ExchangeService.Market;

namespace ExchangeService.Controllers
{
	public class OrderDepthController : ApiController
	{
		[Route("order_depth/{exchange}/{base}/{counter}")]
		public IHttpActionResult Get(IExchange exchange, Currency @base, Currency counter)
		{
			var pair = new CurrencyPair(@base, counter);
			var orderDepth = exchange.GetMarketOrderDepth(pair);
			return Ok(orderDepth);
		}
	}
}