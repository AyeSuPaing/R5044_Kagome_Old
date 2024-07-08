/*
=========================================================================================================
  Module      : 商品在庫マスタモデル (ProductStockModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.Domain.ProductStock
{
	/// <summary>
	/// 商品在庫マスタモデル
	/// </summary>
	public partial class ProductStockModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>在庫更新メモ</summary>
		public string UpdateMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO]); }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO] = value; }
		}
		#endregion
	}
}
