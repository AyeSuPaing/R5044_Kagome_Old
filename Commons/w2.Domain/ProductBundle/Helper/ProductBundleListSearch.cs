/*
=========================================================================================================
  Module      : 商品同梱一覧検索クラス (ProductBundleListSearch.cs)
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

namespace w2.Domain.ProductBundle.Helper
{
	/// <summary>
	/// 商品同梱一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class ProductBundleListSearchCondition
		: BaseDbMapModel
	{
		#region プロパティ
		/// <summary>
		/// 商品同梱ID
		/// </summary>
		public string ProductBundleId { get; set; }
		/// <summary>
		/// 商品同梱ID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("product_bundle_id_like_escaped")]
		public string ProductBundleIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductBundleId); }
		}

		/// <summary>
		/// 商品同梱名
		/// </summary>
		public string ProductBundleName { get; set; }
		/// <summary>
		/// 商品同梱名（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("product_bundle_name_like_escaped")]
		public string ProductBundleNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.ProductBundleName); }
		}

		/// <summary>
		/// 対象注文種別
		/// </summary>
		[DbMapName(Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE)]
		public string TargetOrderType { get; set; }

		/// <summary>
		/// 開始行番号
		/// </summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set;}

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

		/// <summary>
		/// 対象購入商品ID
		/// </summary>
		public string TargetProductId { get; set; }
		/// <summary>
		/// 対象購入商品ID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("target_product_id_like_escaped")]
		public string TargetProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.TargetProductId); }
		}

		/// <summary>
		/// 同梱商品ID
		/// </summary>
		public string BundleProductId { get; set; }
		/// <summary>
		/// 同梱商品ID（SQL LIKEエスケープ済）
		/// </summary>
		[DbMapName("bundle_product_id_like_escaped")]
		public string BundleProductIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.BundleProductId); }
		}
		#endregion
	}

	/// <summary>
	/// 商品同梱一覧検索結果クラス
	/// </summary>
	[Serializable]
	public class ProductBundleListSearchResult
		: ProductBundleModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductBundleListSearchResult(DataRowView drv)
			: base(drv)
		{
		}
		#endregion

		#region プロパティ
		#endregion
	}
}
