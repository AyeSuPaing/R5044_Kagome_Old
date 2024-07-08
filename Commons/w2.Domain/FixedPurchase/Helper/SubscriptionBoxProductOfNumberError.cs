/*
=========================================================================================================
  Module      : 頒布会商品種類数エラー判定 (SubscriptionBoxProductUpdateResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.FixedPurchase.Helper
{
	/// <summary>
	/// 頒布会商品種類数エラー判定クラス
	/// </summary>
	public class SubscriptionBoxProductOfNumberError
	{
		/// <summary>エラー内容</summary>
		public enum ErrorTypes
		{
			/// <summary>最低数量制限エラー</summary>
			MinLimit,
			/// <summary>最大数量制限エラー</summary>
			MaxLimit,
			/// <summary>エラーなし</summary>
			NonError
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SubscriptionBoxProductOfNumberError(
			string displayName,
			string minimumNumberOfProducts,
			string maximumNumberOfProducts)
		{
			this.ErrorType = ErrorTypes.NonError;
			this.MinimumNumberOfProducts = minimumNumberOfProducts;
			this.MaximumNumberOfProducts = maximumNumberOfProducts;
			this.DisplayName = displayName;
		}

		/// <summary>エラー内容</summary>
		public ErrorTypes ErrorType { get; set; }
		/// <summary>頒布会表示名</summary>
		public string DisplayName { get; set; }
		/// <summary>最低購入種類数</summary>
		public string MinimumNumberOfProducts { get; set; }
		/// <summary>最大購入種類数</summary>
		public string MaximumNumberOfProducts { get; set; }
	}
}