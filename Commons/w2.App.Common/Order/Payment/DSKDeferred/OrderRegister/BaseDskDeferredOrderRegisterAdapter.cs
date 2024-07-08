/*
=========================================================================================================
  Module      : DSK後払い注文情報登録基底アダプタ(BaseDskDeferredOrderRegisterAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderRegister
{
	/// <summary>
	/// DSK後払い注文情報登録基底アダプタ
	/// </summary>
	public class BaseDskDeferredOrderRegisterAdapter : BaseDskDeferredAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">API接続設定</param>
		protected BaseDskDeferredOrderRegisterAdapter(DskDeferredApiSetting apiSetting = null)
			: base(apiSetting)
		{
		}

		/// <summary>
		/// 詳細要素作成
		/// </summary>
		/// <param name="detailName">明細名</param>
		/// <param name="price">単価</param>
		/// <param name="quantity">注文数</param>
		/// <returns>詳細要素</returns>
		protected DetailElement CreateDetailElement(string detailName, decimal price, int quantity = 1)
		{
			var detailElement = new DetailElement
			{
				DetailName = detailName,
				DetailPrice = price.ToPriceString(),
				DetailQuantity = quantity.ToString(),
			};

			return detailElement;
		}
	}
}
