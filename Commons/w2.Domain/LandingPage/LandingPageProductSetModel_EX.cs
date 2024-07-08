/*
=========================================================================================================
  Module      : Lpページ商品セットモデル (LandingPageProductSetModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページ商品セットモデル
	/// </summary>
	public partial class LandingPageProductSetModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>定期用の商品が1つ以上含まれているか</summary>
		public bool HasAnyProductForFixedPurchase
		{
			get { return this.Products.Any(p => (p.BuyType == LandingPageConst.BUY_TYPE_FIXEDPURCHASE)); }
		}
		/// <summary>商品リスト</summary>
		public LandingPageProductModel[] Products { get; set; }
		#endregion
	}
}
