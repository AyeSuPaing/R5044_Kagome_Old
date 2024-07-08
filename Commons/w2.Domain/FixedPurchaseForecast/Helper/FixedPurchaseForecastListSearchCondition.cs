/*
=========================================================================================================
  Module      : 定期売上予測レポート検索条件クラス(FixedPurchaseForecastListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchaseForecast.Helper
{
	/// <summary>
	/// 定期売上予測レポート検索条件クラス
	/// </summary>
	public class FixedPurchaseForecastListSearchCondition : BaseDbMapModel
	{
		/// <summary>ショップID</summary>
		[DbMapName("shop_id")]
		public string ShopId { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>商品ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("product_id_like_escaped")]
		public string ProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductId); }
		}
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>商品名（SQL LIKEエスケープ済）</summary>
		[DbMapName("product_name_like_escaped")]
		public string ProductNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductName); }
		}
		/// <summary>配送種別</summary>
		[DbMapName("shipping_type")]
		public string ShippingType { get; set; }
		/// <summary>カテゴリID</summary>
		public string CategoryId { get; set; }
		/// <summary>カテゴリID（SQL LIKEエスケープ済</summary>
		[DbMapName("category_id_like_escaped")]
		public string CategoryIdEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.CategoryId); }
		}
		/// <summary>ページ番号</summary>
		public int PagerNo { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		/// <summary>対象年月</summary>
		[DbMapName("target_month")]
		public DateTime TargetMonth { get; set; }
	}
}
