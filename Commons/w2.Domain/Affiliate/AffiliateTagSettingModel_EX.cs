/*
=========================================================================================================
  Module      : アフィリエイトタグ設定マスタモデル (AffiliateTagSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトタグ設定マスタモデル
	/// </summary>
	public partial class AffiliateTagSettingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>
		/// アフィリエイト商品タグ設定
		/// </summary>
		public AffiliateProductTagSettingModel AffiliateProductTagSettingModel
		{
			get { return (AffiliateProductTagSettingModel)this.DataSource["AffiliateProductTagSettingModel"]; }
			set { this.DataSource["AffiliateProductTagSettingModel"] = value; }
		}
		/// <summary>
		/// 出力条件条件
		/// </summary>
		public AffiliateTagConditionModel[] AffiliateTagConditionList
		{
			get { return (AffiliateTagConditionModel[])this.DataSource["AffiliateTagConditionList"]; }
			set { this.DataSource["AffiliateTagConditionList"] = value; }
		}
		#endregion
	}
}
