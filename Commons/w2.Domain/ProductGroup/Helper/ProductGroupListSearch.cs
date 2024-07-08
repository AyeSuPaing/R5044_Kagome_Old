/*
=========================================================================================================
  Module      : 商品グループ一覧検索のためのヘルパクラス (ProductGroupListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.ProductGroup.Helper
{
	/// <summary>
	/// 商品グループ一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class ProductGroupListSearchCondition : BaseDbMapModel
	{
		#region プロパティ
		/// <summary>
		/// 商品グループID
		/// </summary>
		public string ProductGroupId { get; set; }
		/// <summary>
		/// 商品グループID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("product_group_id_like_escaped")]
		public string ProductGroupIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductGroupId); }
		}

		/// <summary>
		/// 商品グループ名
		/// </summary>
		public string ProductGroupName { get; set; }
		/// <summary>
		/// 商品グループ名（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("product_group_name_like_escaped")]
		public string ProductGroupNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductGroupName); }
		}

		/// <summary>
		/// 商品ID
		/// </summary>
		public string MasterId { get; set; }
		/// <summary>
		/// 商品ID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("master_id_like_escaped")]
		public string MasterIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.MasterId); }
		}

		/// <summary>
		/// 開始日時（FROM）
		/// </summary>
		[DbMapName("begin_date_from")]
		public string BeginDateFrom { get; set; }

		/// <summary>
		/// 開始日時（TO）
		/// </summary>
		[DbMapName("begin_date_to")]
		public string BeginDateTo { get; set; }

		/// <summary>
		/// 終了日時（FROM）
		/// </summary>
		[DbMapName("end_date_from")]
		public string EndDateFrom { get; set; }

		/// <summary>
		/// 終了日時（TO）
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
		/// 有効フラグ
		/// </summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }
		#endregion
	}

	/// <summary>
	/// 商品グループ一覧検索結果クラス
	/// </summary>
	[Serializable]
	public class ProductGroupListSearchResult : ProductGroupModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductGroupListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ(Modelに実装している以外）
		#endregion
	}
}
