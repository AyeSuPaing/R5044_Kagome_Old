using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain.Order;

namespace w2.App.CommonTests.Order
{
	/// <summary>
	/// 注文テストヘルパー
	/// </summary>
	public class OrderTestHelper
	{
		/// <summary>
		/// <see cref="OrderPriceByTaxRateModel"/>の日付型を除いた値比較
		/// </summary>
		/// <returns></returns>
		public static Func<OrderPriceByTaxRateModel, OrderPriceByTaxRateModel, bool> EqualOrderPriceByTaxRateModel()
		{
			return (x, y) => 
			{
				return ((x.OrderId == y.OrderId)
					&& (x.KeyTaxRate == y.KeyTaxRate)
					&& (x.PriceSubtotalByRate == y.PriceSubtotalByRate)
					&& (x.PriceShippingByRate == y.PriceShippingByRate)
					&& (x.PricePaymentByRate == y.PricePaymentByRate)
					&& (x.PriceTotalByRate == y.PriceTotalByRate)
					&& (x.TaxPriceByRate == y.TaxPriceByRate)
					&& (x.ReturnPriceCorrectionByRate == y.ReturnPriceCorrectionByRate));
			};
		}
	}
}