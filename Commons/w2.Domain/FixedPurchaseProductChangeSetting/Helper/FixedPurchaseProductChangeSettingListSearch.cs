/*
=========================================================================================================
  Module      : 定期商品変更設定一覧検索のためのヘルパクラス (FixedPurchaseProductChangeSettingListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchaseProductChangeSetting.Helper
{
	#region 定期商品変更設定一覧検索条件クラス
	/// <summary>
	/// 定期商品変更設定一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseProductChangeSettingListSearchCondition : BaseDbMapModel
	{
		/// <summary>定期商品変更ID</summary>
		public string FixedPurchaseProductChangeId { get; set; }
		/// <summary>定期商品変更ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("fixed_purchase_product_change_id_like_escaped")]
		public string FixedPurchaseProductChangeIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.FixedPurchaseProductChangeId); }
		}
		/// <summary>定期商品変更名</summary>
		public string FixedPurchaseProductChangeName { get; set; }
		/// <summary>定期商品変更名（SQL LIKEエスケープ済）</summary>
		[DbMapName("fixed_purchase_product_change_name_like_escaped")]
		public string FixedPurchaseProductChangeNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.FixedPurchaseProductChangeName); }
		}
		/// <summary>定期変更元商品ID</summary>
		public string BeforeChangeProductId { get; set; }
		/// <summary>定期変更元商品ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("before_change_product_id_like_escaped")]
		public string BeforeChangeProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.BeforeChangeProductId); }
		}
		/// <summary>定期変更後商品ID</summary>
		public string AfterChangeProductId { get; set; }
		/// <summary>定期変更後商品ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("after_change_product_id_like_escaped")]
		public string AfterChangeProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.AfterChangeProductId); }
		}
		/// <summary>並び順区分</summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		/// <summary>有効フラグ</summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
	}
	#endregion

	#region 定期商品変更設定一覧検索結果クラス
	/// <summary>
	/// 定期商品変更設定一覧検索結果クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseProductChangeSettingListSearchResult : FixedPurchaseProductChangeSettingModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseProductChangeSettingListSearchResult(DataRowView source)
		: base(source)
		{
		}
	}
	#endregion
}
