/*
=========================================================================================================
  Module      : クーポンリスト検索のためのヘルパクラス (CouponListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coupon.Helper
{
	/// <summary>
	/// クーポン情報検索条件情報
	/// </summary>
	[Serializable]
	public class CouponListSearchCondition : BaseCouponSearchCondition
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// クーポンID
		/// </summary>
		[DbMapName("coupon_id")]
		public string CouponId { get; set; }

		/// <summary>
		/// クーポンコード
		/// </summary>
		[DbMapName("coupon_code_like_escaped")]
		public string CouponCode { get; set; }

		/// <summary>
		/// クーポン名
		/// </summary>
		[DbMapName("coupon_name_like_escaped")]
		public string CouponName { get; set; }

		/// <summary>
		/// クーポンタイプ
		/// </summary>
		[DbMapName("coupon_type")]
		public string CouponType { get; set; }

		/// <summary>
		/// 発行日
		/// </summary>
		[DbMapName("publish_date")]
		public string PublishDate { get; set; }

		/// <summary>
		/// 有効フラグ
		/// </summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }

		/// <summary>
		/// ユーザーの持つクーポンのみフラグ
		/// </summary>
		[DbMapName("user_coupon_only")]
		public string UserCouponOnly { get; set; }
		#endregion
	}

	/// <summary>
	/// クーポン情報検索結果情報
	/// </summary>
	[Serializable]
	public class CouponListSearchResult : w2.Domain.Coupon.CouponModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drv">クーポン検索結果情報</param>
		public CouponListSearchResult(DataRowView drv)
			: base(drv)
		{
		}
		#endregion

		/// <summary>
		/// 検索結果の全て件数
		/// </summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
	}
}
