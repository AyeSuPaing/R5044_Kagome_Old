/*
=========================================================================================================
  Module      : コーディネート一覧検索条件クラス (CoordinateListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coordinate.Helper
{
	/// <summary>
	/// コーディネートカテゴリ一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class CoordinateListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */

		#region プロパティ
		/// <summary>スタッフ（SQL LIKEエスケープ済） </summary>
		[DbMapName("staff_like_escaped")]
		public string StaffLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.Staff); } }
		/// <summary>スタッフ</summary>
		[DbMapName("staff")]
		public string Staff { get; set; }
		/// <summary>スタッフID</summary>
		public string[] StaffIds { get { return this.StaffLikeEscaped.Split(',').Where(id => (string.IsNullOrEmpty(id) == false)).ToArray(); } }
		/// <summary>リアルショップ（SQL LIKEエスケープ済） </summary>
		[DbMapName("real_shop_like_escaped")]
		public string RealShopLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.RealShop); } }
		/// <summary>リアルショップ</summary>
		[DbMapName("real_shop")]
		public string RealShop { get; set; }
		/// <summary>公開日区分</summary>
		[DbMapName("display_date_kbn")]
		public string DisplayDateKbn { get; set; }
		/// <summary>公開日を考慮するか</summary>
		[DbMapName("consider_display_date")]
		public string ConsiderDisplayDate { get; set; }
		/// <summary>カテゴリ（SQL LIKEエスケープ済） </summary>
		[DbMapName("category_like_escaped")]
		public string CategoryLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.Category); } }
		/// <summary>カテゴリ</summary>
		[DbMapName("category")]
		public string Category { get; set; }
		/// <summary>表示区分（SQL LIKEエスケープ済） </summary>
		[DbMapName("display_kbn_like_escaped")]
		public string DisplayKbnEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.DisplayKbn); } }
		/// <summary>表示区分</summary>
		[DbMapName("display_kbn")]
		public string DisplayKbn { get; set; }
		/// <summary>身長下限</summary>
		[DbMapName("height_lower_limit")]
		public string HeightLowerLimit { get; set; }
		/// <summary>身長上限</summary>
		[DbMapName("height_upper_limit")]
		public string HeightUpperLimit { get; set; }
		/// <summary>キーワード０（SQL LIKEエスケープ済） </summary>
		[DbMapName("keyword0_like_escaped")]
		public string Keyword0Escaped { get { return StringUtility.SqlLikeStringSharpEscape(this.Keyword0); } }
		/// <summary>キーワード０</summary>
		[DbMapName("keyword0")]
		public string Keyword0 { get; set; }
		/// <summary>キーワード１（SQL LIKEエスケープ済） </summary>
		[DbMapName("keyword1_like_escaped")]
		public string Keyword1Escaped { get { return StringUtility.SqlLikeStringSharpEscape(this.Keyword1); } }
		/// <summary>キーワード１</summary>
		[DbMapName("keyword1")]
		public string Keyword1 { get; set; }
		/// <summary>キーワード２（SQL LIKEエスケープ済） </summary>
		[DbMapName("keyword2_like_escaped")]
		public string Keyword2Escaped { get { return StringUtility.SqlLikeStringSharpEscape(this.Keyword2); } }
		/// <summary>キーワード２</summary>
		[DbMapName("keyword2")]
		public string Keyword2 { get; set; }
		/// <summary>キーワード３（SQL LIKEエスケープ済） </summary>
		[DbMapName("keyword3_like_escaped")]
		public string Keyword3Escaped { get { return StringUtility.SqlLikeStringSharpEscape(this.Keyword3); } }
		/// <summary>キーワード３</summary>
		[DbMapName("keyword3")]
		public string Keyword3 { get; set; }
		/// <summary>キーワード４（SQL LIKEエスケープ済） </summary>
		[DbMapName("keyword4_like_escaped")]
		public string Keyword4Escaped { get { return StringUtility.SqlLikeStringSharpEscape(this.Keyword4); } }
		/// <summary>キーワード4</summary>
		[DbMapName("keyword4")]
		public string Keyword4 { get; set; }

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
		#endregion
	}

	/// <summary>
	///コーディネート一覧検索クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class CoordinateListSearchResult : CoordinateModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CoordinateListSearchResult()
		{
			this.CoordinateId = "";
			this.CoordinateTitle = "";
			this.CoordinateUrl = "";
			this.CoordinateSummary = "";
			this.InternalMemo = "";
			this.HtmlTitle = "";
			this.MetadataKeywords = "";
			this.MetadataDesc = "";
			this.StaffId = "";
			this.RealShopId = "";
			this.DisplayKbn = Constants.FLG_COORDINATE_DISPLAY_KBN_DRAFT;
			this.DisplayDate = null;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateListSearchResult(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateListSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion
	}
}
