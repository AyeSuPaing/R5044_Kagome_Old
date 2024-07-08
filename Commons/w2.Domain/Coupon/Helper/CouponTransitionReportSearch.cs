/*
=========================================================================================================
  Module      : クーポン推移レポート検索のためのヘルパクラス (CouponTransitionReportSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coupon.Helper
{
	#region 列挙体
	/// <summary>
	/// レポート種別
	/// </summary>
	public enum ReportType
	{
		/// <summary>日別</summary>
		Day,
		/// <summary>月別</summary>
		Month
	}

	/// <summary>
	/// データー表示種別
	/// </summary>
	public enum DataDisplayKbn
	{
		/// <summary>クーポン金額表示</summary>
		Price,
		/// <summary>枚数表示</summary>
		Count
	}
	#endregion

	/// <summary>
	/// クーポン推移レポート検索条件情報
	/// </summary>
	[Serializable]
	public class CouponTransitionReportCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// 対象年
		/// </summary>
		[DbMapName("year")]
		public string Year { get; set; }

		/// <summary>
		/// 対象月
		/// </summary>
		[DbMapName("month")]
		public string Month { get; set; }

		/// <summary>
		/// 識別ID
		/// </summary>
		[DbMapName("dept_id")]
		public string DeptId { get; set; }

		/// <summary>
		/// クーポンID
		/// </summary>
		[DbMapName("coupon_id")]
		public string CouponId { get; set; }

		/// <summary>
		/// クーポンコード
		/// </summary>
		[DbMapName("coupon_code_like_escaped")]
		public string CouponCodeLikeEscaped { get; set; }

		/// <summary>
		/// レポート種別（月別or日別）
		/// </summary>
		[DbMapName("report_type")]
		public ReportType ReportType { get; set; }

		/// <summary>
		/// 金額ロケールID
		/// </summary>
		public string CurrencyLocaleId { get; set; }

		/// <summary>
		/// 金額フォーマット
		/// </summary>
		public string CurrencyLocaleFormat { get; set; }
		#endregion
	}

	/// <summary>
	/// クーポン推移レポート検索結果
	/// </summary>
	[Serializable]
	public class CouponTransitionReportResult
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CouponTransitionReportResult()
		{
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// 月日単位（日／月）
		/// </summary>
		public string TimeUnit { get; set; }

		/// <summary>
		/// 発行クーポン金額
		/// </summary>
		public string AddCoupon { get; set; }

		/// <summary>
		/// 発行枚数
		/// </summary>
		public string AddCount { get; set; }

		/// <summary>
		/// 利用クーポン金額
		/// </summary>
		public string UseCoupon { get; set; }

		/// <summary>
		/// 利用枚数
		/// </summary>
		public string UseCount { get; set; }

		/// <summary>
		/// 有効期限切れクーポン金額
		/// </summary>
		public string ExpCoupon { get; set; }

		/// <summary>
		/// 有効期限切れ枚数
		/// </summary>
		public string ExpCount { get; set; }

		/// <summary>
		/// 未利用クーポン金額
		/// </summary>
		public string UnusedCoupon { get; set; }

		/// <summary>
		/// 未利用枚数
		/// </summary>
		public string UnusedCount { get; set; }
		#endregion
	}

	/// <summary>
	/// クーポン推移レポート情報
	/// </summary>
	[Serializable]
	public class CouponTransitionInfo : ModelBase<CouponTransitionInfo>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CouponTransitionInfo()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// <param name="drv">データソース</param>
		/// </summary>
		public CouponTransitionInfo(DataRowView drv)
		{
			this.DataSource = drv.ToHashtable();
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// 対象年
		/// </summary>
		public string TgtYear
		{
			get { return (string)this.DataSource["tgt_year"]; }
			set { this.DataSource["tgt_year"] = value; }
		}

		/// <summary>
		/// 対象月
		/// </summary>
		public string TgtMonth
		{
			get { return (string)this.DataSource["tgt_month"]; }
			set { this.DataSource["tgt_month"] = value; }
		}

		/// <summary>
		/// 対象日
		/// </summary>
		public string TgtDay
		{
			get { return (string)this.DataSource["tgt_day"]; }
			set { this.DataSource["tgt_day"] = value; }
		}

		/// <summary>
		/// 発行クーポン金額
		/// </summary>
		public decimal AddCoupon
		{
			get { return (decimal)this.DataSource["add_coupon"]; }
			set { this.DataSource["add_coupon"] = value; }
		}

		/// <summary>
		/// 発行枚数
		/// </summary>
		public decimal AddCount
		{
			get { return (decimal)this.DataSource["add_count"]; }
			set { this.DataSource["add_count"] = value; }
		}

		/// <summary>
		/// 利用クーポン金額
		/// </summary>
		public decimal UseCoupon
		{
			get { return (decimal)this.DataSource["use_coupon"]; }
			set { this.DataSource["use_coupon"] = value; }
		}

		/// <summary>
		/// 利用枚数
		/// </summary>
		public decimal UseCount
		{
			get { return (decimal)this.DataSource["use_count"]; }
			set { this.DataSource["use_count"] = value; }
		}

		/// <summary>
		/// 有効期限切れクーポン金額
		/// </summary>
		public decimal ExpCoupon
		{
			get { return (decimal)this.DataSource["exp_coupon"]; }
			set { this.DataSource["exp_coupon"] = value; }
		}

		/// <summary>
		/// 有効期限切れ枚数
		/// </summary>
		public decimal ExpCount
		{
			get { return (decimal)this.DataSource["exp_count"]; }
			set { this.DataSource["exp_count"] = value; }
		}

		/// <summary>
		/// 未利用クーポン金額
		/// </summary>
		public decimal UnusedPrice
		{
			get { return (decimal)this.DataSource["unused_price"]; }
			set { this.DataSource["unused_price"] = value; }
		}

		/// <summary>
		/// 未利用枚数
		/// </summary>
		public decimal UnusedCount
		{
			get { return (decimal)this.DataSource["unused_count"]; }
			set { this.DataSource["unused_count"] = value; }
		}
		#endregion
	}

	/// <summary>
	/// 未利用クーポン推移レポート情報
	/// </summary>
	[Serializable]
	public class UnusedCouponTransitionInfo : ModelBase<UnusedCouponTransitionInfo>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UnusedCouponTransitionInfo()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// <param name="drv">データソース</param>
		/// </summary>
		public UnusedCouponTransitionInfo(DataRowView drv)
		{
			this.DataSource = drv.ToHashtable();
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// 対象年
		/// </summary>
		public string TgtYear
		{
			get { return (string)this.DataSource["tgt_year"]; }
			set { this.DataSource["tgt_year"] = value; }
		}

		/// <summary>
		/// 対象月
		/// </summary>
		public string TgtMonth
		{
			get { return (string)this.DataSource["tgt_month"]; }
			set { this.DataSource["tgt_month"] = value; }
		}

		/// <summary>
		/// 対象日
		/// </summary>
		public string TgtDay
		{
			get { return (string)this.DataSource["tgt_day"]; }
			set { this.DataSource["tgt_day"] = value; }
		}

		/// <summary>
		/// 未使用クーポン金額
		/// </summary>
		public decimal UnusedPrice
		{
			get { return (decimal)this.DataSource["unused_price"]; }
			set { this.DataSource["unused_price"] = value; }
		}

		/// <summary>
		/// 未使用クーポン数
		/// </summary>
		public decimal UnusedCount
		{
			get { return (decimal)this.DataSource["unused_count"]; }
			set { this.DataSource["unused_count"] = value; }
		}
		#endregion
	}
}
