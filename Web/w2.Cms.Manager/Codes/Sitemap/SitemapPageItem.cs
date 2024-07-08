/*
=========================================================================================================
  Module      : サイトマップページアイテム(SitemapPageItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Runtime.Remoting.Messaging;

namespace w2.Cms.Manager.Codes.Sitemap
{
	/// <summary>
	/// サイトマップページアイテム
	/// </summary>
	public class SitemapPageItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SitemapPageItem()
		{
			this.SequentialId = 0;
			this.IsIncluded = false;
			this.IsProductPage = false;
			this.IsCoordinatePage = false;
			this.Title = string.Empty;
			this.Url = string.Empty;
			this.ChangeFrequency = null;
			this.PageType = null;
			this.DeviceType = null;
		}

		/// <summary>
		/// コピー
		/// </summary>
		/// <returns>ページアイテム</returns>
		public SitemapPageItem Copy()
		{
			var copiedItem = new SitemapPageItem
			{
				SequentialId = this.SequentialId,
				IsIncluded = this.IsIncluded,
				IsProductPage = this.IsProductPage,
				IsCoordinatePage = this.IsCoordinatePage,
				Title = this.Title,
				Url = this.Url,
				ChangeFrequency = this.ChangeFrequency,
				Priority = this.Priority,
				PageType = this.PageType,
				DeviceType = this.DeviceType
			};

			return copiedItem;
		}

		/// <summary>連番ID</summary>
		public int SequentialId { get; set; }
		/// <summary>サイトマップに含めるか？</summary>
		public bool IsIncluded { get; set; }
		/// <summary>商品系ページ？</summary>
		public bool IsProductPage { get; set; }
		/// <summary>コーディネート系ページか</summary>
		public bool IsCoordinatePage { get; set; }
		/// <summary>タイトル</summary>
		public string Title { get; set; }
		/// <summary>URL</summary>
		public string Url { get; set; }
		/// <summary>表示URL</summary>
		public string DisplayUrl
		{
			get
			{
				var displayUrl = this.Url.Replace(
					Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC,
					"(ROOT)/");
				return displayUrl;
			}
		}
		/// <summary>更新頻度</summary>
		public ChangeFrequencyEnum? ChangeFrequency { get; set; }
		/// <summary>優先度</summary>
		public string Priority { get; set; }
		/// <summary>ページ種別</summary>
		public PageTypeEnum? PageType { get; set; }
		/// <summary>デバイス種別</summary>
		public DeviceTypeEnum? DeviceType { get; set; }
	}
}