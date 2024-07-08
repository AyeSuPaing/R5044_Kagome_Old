/*
=========================================================================================================
 Module      : サイトマップ共通定義(Commons.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
 */

using System.Xml.Serialization;

namespace w2.Cms.Manager.Codes.Sitemap
{
	/// <summary>
	/// ページ種別
	/// </summary>
	public enum PageTypeEnum
	{
		/// <summary>指定なし</summary>
		None = Constants.SITEMAP_PAGE_TYPE_NONE,
		/// <summary>標準ページ</summary>
		StandardPage = Constants.SITEMAP_PAGE_TYPE_STANDARD,
		/// <summary>カスタムページ</summary>
		CustomPage = Constants.SITEMAP_PAGE_TYPE_CUSTOM,
		/// <summary>ランディングページ</summary>
		LandingPage = Constants.SITEMAP_PAGE_TYPE_LANDING,
		/// <summary>コーディネート</summary>
		Coordinate = Constants.SITEMAP_PAGE_TYPE_COORDINATE,
		/// <summary>特集ページ</summary>
		FeaturePage = Constants.SITEMAP_PAGE_TYPE_FEATURE
	}

	/// <summary>
	/// 端末種別
	/// </summary>
	public enum DeviceTypeEnum
	{
		/// <summary>PC</summary>
		Pc,
		/// <summary>SP</summary>
		Sp,
		/// <summary>PCとSP</summary>
		PcAndSp
	}

	/// <summary>
	/// 更新頻度
	/// </summary>
	public enum ChangeFrequencyEnum
	{
		/// <summary>表示する度</summary>
		[XmlEnum("always")]
		Always = Constants.SITEMAP_CHANGE_FREQ_ALWAYS,
		/// <summary>１時間毎</summary>
		[XmlEnum("hourly")]
		Hourly = Constants.SITEMAP_CHANGE_FREQ_HOURLY,
		/// <summary>毎日</summary>
		[XmlEnum("daily")]
		Daily = Constants.SITEMAP_CHANGE_FREQ_DAILY,
		/// <summary>１週間毎</summary>
		[XmlEnum("weekly")]
		Weekly = Constants.SITEMAP_CHANGE_FREQ_WEEKLY,
		/// <summary>１ヶ月毎</summary>
		[XmlEnum("monthly")]
		Monthly = Constants.SITEMAP_CHANGE_FREQ_MONTHLY,
		/// <summary>１年毎</summary>
		[XmlEnum("yearly")]
		Yearly = Constants.SITEMAP_CHANGE_FREQ_YEARLY,
		/// <summary>更新されない</summary>
		[XmlEnum("never")]
		Never = Constants.SITEMAP_CHANGE_FREQ_NEVER,
	}
}