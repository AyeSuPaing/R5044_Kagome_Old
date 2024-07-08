/*
=========================================================================================================
  Module      : アフィリエイト商品タグ検索のためのヘルパクラス (AffiliateProductTagSettingListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Affiliate.Helper
{
	/// <summary>
	/// アフィリエイト商品タグ検索のためのヘルパクラス
	/// </summary>
	[Serializable]
	public class AffiliateProductTagSettingListSearchCondition : BaseDbMapModel
	{
		/*
		* 検索条件となるものをプロパティで持つ
		* 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		*/

		#region プロパティ
		/// <summary>アフィリエイト商品タグ名 部分一致</summary>
		[DbMapName(Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME + "_like_escaped")]
		public string AffiliateProductTagNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.AffiliateProductTagName); }
		}

		/// <summary>アフィリエイト商品タグ名</summary>
		[DbMapName(Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME)]
		public string AffiliateProductTagName { get; set; }

		/// <summary>開始 行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }

		/// <summary>終了 行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		#endregion
	}

	/// <summary>
	/// アフィリエイト商品タグ検索 結果
	/// </summary>
	[Serializable]
	public class AffiliateProductTagSettingListSearchResult : AffiliateProductTagSettingModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AffiliateProductTagSettingListSearchResult(DataRowView source) : base(source)
		{
		}
		#endregion

		#region プロパティ(Modelに実装している以外）
		#endregion
	}
}