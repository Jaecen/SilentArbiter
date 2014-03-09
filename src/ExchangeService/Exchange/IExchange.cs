using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExchangeService.Market;

namespace ExchangeService.Exchange
{
	[TypeConverter(typeof(ExchangeTypeConverter))]
	public interface IExchange
	{
		OrderDepth GetMarketOrderDepth(CurrencyPair pair);
	}

	public class ExchangeTypeConverter : TypeConverter
	{
		readonly Dictionary<string, Type> ExchangeMappings;

		public ExchangeTypeConverter()
		{
			var exchangeType = typeof(IExchange);
			var exchanges = System.Reflection.Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type => exchangeType.IsAssignableFrom(type))
				.ToArray();

			ExchangeMappings = exchanges
				.ToDictionary(type => type.Name, StringComparer.OrdinalIgnoreCase);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(String))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if(value is String && ExchangeMappings.ContainsKey((string)value))
			{
				var type = ExchangeMappings[(string)value];
				var instance = Activator.CreateInstance(type);

				if(instance != null)
					return instance;
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}