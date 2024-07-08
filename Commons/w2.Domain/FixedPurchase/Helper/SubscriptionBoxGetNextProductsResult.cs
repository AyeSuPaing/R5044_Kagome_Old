/*
=========================================================================================================
  Module      : 頒布会次回配送商品取得結果クラス (SubscriptionBoxGetNextProductResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.FixedPurchase.Helper
{
	/// <summary>
	/// 頒布会次回配送商品取得結果クラス
	/// </summary>
	public class SubscriptionBoxGetNextProductsResult
	{
		/// <summary>注文結果</summary>
		public enum ResultTypes
		{
			/// <summary>成功</summary>
			Success,
			/// <summary>失敗</summary>
			Fail,
			/// <summary>終了</summary>
			Cancel,
			///<summary>一部失敗</summary>
			PartialFailure
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SubscriptionBoxGetNextProductsResult()
		{
			this.NextCount = 1;
			this.Result = ResultTypes.Fail;
			this.NextProducts = new FixedPurchaseItemModel[] { };
			this.HasNecessaryFlg = false;
		}
		/// <summary>取得結果</summary>
		public ResultTypes Result { get; set; }
		/// <summary>次回購入回数</summary>
		public int NextCount { get; set; }
		/// <summary>次回配送商品</summary>
		public FixedPurchaseItemModel[] NextProducts { get; set; }
		/// <summary>必須商品フラグ</summary>
		public bool HasNecessaryFlg { get; set; }
	}
}