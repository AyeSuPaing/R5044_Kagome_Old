/*
=========================================================================================================
 Module      : サイトマップ設定(SitemapSetting.xml)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace w2.Cms.Manager.Codes.Sitemap
{
	/// <summary>
	/// サイトマップ設定
	/// </summary>
	[XmlRoot("SitemapSetting")]
	public class SitemapSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SitemapSetting()
		{
			this.SettingForProductList = null;
			this.SettingForProductDetail = null;
			this.SpecifiedUrls = new List<XmlSitemapUrl>();
		}

		/// <summary>
		/// SettingForProductListのシリアライズ可否
		/// </summary>
		/// <returns>結果</returns>
		public bool ShouldSerializeSettingForProductList()
		{
			return (this.SettingForProductList != null);
		}

		/// <summary>
		/// SettingForProductDetailのシリアライズ可否
		/// </summary>
		/// <returns>結果</returns>
		public bool ShouldSerializeSettingForProductDetail()
		{
			return (this.SettingForProductDetail != null);
		}

		/// <summary>商品一覧用ページ設定</summary>
		[XmlElement("ProductList")]
		public XmlSitemapUrl SettingForProductList { get; set; }
		/// <summary>商品詳細用ページ設定</summary>
		[XmlElement("ProductDetail")]
		public XmlSitemapUrl SettingForProductDetail { get; set; }
		/// <summary>コーディネート一覧用ページ設定</summary>
		[XmlElement("CoordinateList")]
		public XmlSitemapUrl SettingForCoordinateList { get; set; }
		/// <summary>コーディネート詳細用ページ設定</summary>
		[XmlElement("CoordinateDetail")]
		public XmlSitemapUrl SettingForCoordinateDetail { get; set; }
		/// <summary>コーディネートトップ用ページ設定</summary>
		[XmlElement("CoordinateTop")]
		public XmlSitemapUrl SettingForCoordinateTop { get; set; }
		/// <summary>指定URL</summary>
		[XmlArray("SpecificUrls")]
		[XmlArrayItem("url")]
		public List<XmlSitemapUrl> SpecifiedUrls { get; set; }
	}

	/// <summary>
	/// XMLサイトマップURLアイテム
	/// </summary>
	public class XmlSitemapUrl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public XmlSitemapUrl()
		{
			this.Location = string.Empty;
			this.LastModified = null;
			this.ChangeFrequency = null;
			this.Priority = null;
		}

		/// <summary>
		/// Locationのシリアライズ可否
		/// </summary>
		/// <returns>結果</returns>
		public bool ShouldSerializeLocation()
		{
			return (string.IsNullOrEmpty(this.Location) == false);
		}

		/// <summary>
		/// LastModifiedのシリアライズ可否
		/// </summary>
		/// <returns>結果</returns>
		public bool ShouldSerializeLastModified()
		{
			return this.LastModified.HasValue;
		}

		/// <summary>
		/// ChangeFrequencyのシリアライズ可否
		/// </summary>
		/// <returns>結果</returns>
		public bool ShouldSerializeChangeFrequency()
		{
			return this.ChangeFrequency.HasValue;
		}

		/// <summary>
		/// Priorityのシリアライズ可否
		/// </summary>
		/// <returns>結果</returns>
		public bool ShouldSerializePriority()
		{
			return (string.IsNullOrEmpty(this.Priority) == false);
		}

		/// <summary>パス</summary>
		[XmlElement("loc")]
		public string Location { get; set; }
		/// <summary>最終更新日</summary>
		[XmlElement("lastmod")]
		public DateTime? LastModified { get; set; }
		/// <summary>更新頻度</summary>
		[XmlElement("changefreq")]
		public ChangeFrequencyEnum? ChangeFrequency { get; set; }
		/// <summary>優先度</summary>
		[XmlElement("priority")]
		public string Priority { get; set; }
	}
}
