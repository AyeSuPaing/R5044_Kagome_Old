/*
=========================================================================================================
  Module      : 定期売上予測集計(FixedPurchaseForecastReport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.FixedPurchase;

namespace w2.Commerce.Batch.FixedPurchaseBatch.FixedPurchaseForecastReport
{
	/// <summary>
	/// 定期売上予測集計
	/// </summary>
	public class FixedPurchaseForecastReport
	{
		/// <summary>
		/// 集計実行
		/// </summary>
		public void Aggregate()
		{
			new FixedPurchaseHelper().Aggregate(
				LastExecDate.GetLastExecDate(),
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE,
				Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
				Constants.MANAGEMENT_INCLUDED_TAX_FLAG,
				Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId,
				Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.CurrencyDecimalDigits);
		}
	}
}
