/*
=========================================================================================================
  Module      : ショートURL一覧検索のためのヘルパクラス (ShortUrlListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.ShortUrl.Helper
{
	#region +ショートURL検索条件クラス

	/// <summary>
	/// ショートURL一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class ShortUrlListSearchCondition : BaseDbMapModel
	{
		/// <summary>店舗ID</summary>
		[DbMapName("shop_id")]
		public string ShopId { get; set; }

		/// <summary>ショートURL</summary>
		public string ShortUrl { get; set; }

		/// <summary>ショートURL（SQL LIKEエスケープ済）</summary>
		[DbMapName("short_url_like_escaped")]
		public string ShortUrlLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ShortUrl); }
		}

		/// <summary>ロングURL</summary>
		public string LongUrl { get; set; }

		/// <summary>ロングURL（SQL LIKEエスケープ済）</summary>
		[DbMapName("long_url_like_escaped")]
		public string LongUrlLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.LongUrl); }
		}

		/// <summary>プロトコル＆ドメイン</summary>
		[DbMapName("protocol_and_domain")]
		public string ProtocolAndDomain { get; set; }

		/// <summary>
		/// 並び順区分
		/// 0：登録日/昇順
		/// 1：登録日/降順
		/// 2：ショートURL/昇順
		/// </summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }

		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }

		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
	}

	#endregion

	#region +ショートURL一覧検索結果クラス

	/// <summary>
	/// ショートURL一覧検索結果クラス
	/// ※ShortUrlModelを拡張
	/// </summary>
	[Serializable]
	public class ShortUrlListSearchResult : ShortUrlModel
	{
		#region コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShortUrlListSearchResult(DataRowView source)
			: base(source)
		{
		}

		#endregion コンストラクタ
	}

	#endregion
}