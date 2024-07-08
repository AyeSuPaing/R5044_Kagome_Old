/*
=========================================================================================================
  Module      : セットプロモーション一覧検索条件クラス (SetPromotionListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Common.Extensions;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.SetPromotion.Helper
{
	/// <summary>
	/// セットプロモーション一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class SetPromotionListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// セットプロモーションID
		/// </summary>
		public string SetpromotionId { get; set; }
		/// <summary>
		/// セットプロモーションID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("setpromotion_id_like_escaped")]
		public string SetpromotionIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.SetpromotionId); }
		}

		/// <summary>
		/// セットプロモーション名
		/// </summary>
		public string SetpromotionName { get; set; }
		/// <summary>
		/// セットプロモーション名（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("setpromotion_name_like_escaped")]
		public string SetpromotionNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.SetpromotionName); }
		}

		/// <summary>
		/// 商品ID
		/// </summary>
		public string ProductId { get; set; }
		/// <summary>
		/// 商品ID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("product_id_like_escaped")]
		public string ProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductId); }
		}

		/// <summary>
		/// カテゴリID
		/// </summary>
		public string CategoryId { get; set; }
		/// <summary>
		/// カテゴリID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("category_id_like_escaped")]
		public string CategoryIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.CategoryId); }
		}

		/// <summary>
		/// ステータス
		/// </summary>
		[DbMapName("status")]
		public string Status { get; set; }

		/// <summary>
		/// 開始日（FROM）
		/// </summary>
		[DbMapName("begin_date_from")]
		public string BeginDateFrom { get; set; }

		/// <summary>
		/// 開始日（TO）
		/// </summary>
		[DbMapName("begin_date_to")]
		public string BeginDateTo { get; set; }

		/// <summary>
		/// 終了日（FROM）
		/// </summary>
		[DbMapName("end_date_from")]
		public string EndDateFrom { get; set; }

		/// <summary>
		/// 終了日（TO）
		/// </summary>
		[DbMapName("end_date_to")]
		public string EndDateTo { get; set; }

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

		/// <summary>
		/// ソート区分
		/// </summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		#endregion
	}

	/// <summary>
	///セットプロモーション一覧検索クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class SetPromotionListSearchResult : SetPromotionModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SetPromotionListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ(Modelに実装している以外）
		#endregion
	}
}
