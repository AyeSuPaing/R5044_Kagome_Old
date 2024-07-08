/*
=========================================================================================================
  Module      : 商品グループマスタモデル (ProductFeatureGroupModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.ProductGroup
{
	/// <summary>
	/// 商品グループマスタモデル
	/// </summary>
	public partial class ProductGroupModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>アイテムリスト</summary>
		public ProductGroupItemModel[] Items
		{
			get { return (ProductGroupItemModel[])this.DataSource["EX_Items"]; } 
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>拡張項目_有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_PRODUCTGROUP_VALID_FLG_VALID); }
		}
		#endregion
	}
}
