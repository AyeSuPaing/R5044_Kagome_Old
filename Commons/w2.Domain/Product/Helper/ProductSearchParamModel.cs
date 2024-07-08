/*
=========================================================================================================
  Module      : 商品検索条件 (ProductSearchParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Product.Helper
{
	/// <summary>
	/// 商品検索条件
	/// </summary>
	public class ProductSearchParamModel : BaseDbMapModel
	{
		/// <summary>
		/// 商品ID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("product_id_like_escaped")]
		public string ProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductId); }
		}
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>
		/// 商品名（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("name_like_escaped")]
		public string NameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductName); }
		}
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>通常/定期購入可否</summary>
		[DbMapName("fixed_purchase")]
		public string FixedPurchaseFlg { get; set; }
		/// <summary>ページ番号</summary>
		public int PagerNo { get; set; }
		/// <summary>
		/// 開始行番号
		/// </summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>
		/// 終了行番号
		/// </summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
	}
}
