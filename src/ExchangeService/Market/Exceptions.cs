using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExchangeService.Market
{
	[Serializable]
	public class ExchangeException : Exception
	{
		public ExchangeException() { }
		public ExchangeException(string message) : base(message) { }
		public ExchangeException(string message, Exception inner) : base(message, inner) { }
		protected ExchangeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	[Serializable]
	public class CryptsyExchangeException : ExchangeException
	{
		public CryptsyExchangeException() { }
		public CryptsyExchangeException(string message) : base(message) { }
		public CryptsyExchangeException(string message, Exception inner) : base(message, inner) { }
		protected CryptsyExchangeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	[Serializable]
	public class PoloniexExchangeException : ExchangeException
	{
		public PoloniexExchangeException() { }
		public PoloniexExchangeException(string message) : base(message) { }
		public PoloniexExchangeException(string message, Exception inner) : base(message, inner) { }
		protected PoloniexExchangeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}