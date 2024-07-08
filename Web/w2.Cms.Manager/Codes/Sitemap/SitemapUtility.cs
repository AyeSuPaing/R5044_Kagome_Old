/*
=========================================================================================================
  Module      : サイトマップユーティリティ(SitemapUtility.xml)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.App.Common.Design;
using w2.App.Common.Util;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Domain.PageDesign;

namespace w2.Cms.Manager.Codes.Sitemap
{
	/// <summary>
	/// サイトマップユーティリティ
	/// </summary>
	public class SitemapUtility
	{
		#region メンバ変数定義
		// フロントルートURL
		private static readonly string m_frontRootUrl
			= (Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC);
		// SPサイトディレクトリ名
		private static readonly string m_smartPhoneDirectoryName;
		// 検索対象ファイル拡張子
		private static readonly string m_targetFileExtension = "aspx";
		// 検索対象ディレクトリ(と、ページタイプ)
		private static readonly Dictionary<string, PageTypeEnum> m_targetDirectoriesAndPageTypes;
		// 除外ページ
		private static readonly string[] m_ignorePages;
		// 標準ページ
		private static readonly SitemapPageItem[] m_standardPages;
		// コーディネートページ
		private static readonly SitemapPageItem[] m_coordinatePages;
		/// <summary>レスポンシブデザインか</summary>
		private static readonly bool m_isResponsive = false;
		#endregion

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static SitemapUtility()
		{
			var spSetting = SmartPhoneUtility.SmartPhoneSiteSettings.FirstOrDefault();
			m_smartPhoneDirectoryName = (spSetting != null)
				? spSetting.RootPath
					.Replace("~/", string.Empty)
					.Replace("/", Path.DirectorySeparatorChar.ToString())
				: string.Empty;
			m_isResponsive = string.IsNullOrEmpty(m_smartPhoneDirectoryName);

			m_targetDirectoriesAndPageTypes = new Dictionary<string, PageTypeEnum>
			{
				{ Path.Combine("Page"), PageTypeEnum.CustomPage },
				{ Path.Combine("Page", "Feature"), PageTypeEnum.FeaturePage },
				{ Path.Combine("Landing"), PageTypeEnum.LandingPage },
				{ Path.Combine("Landing", "formlp"), PageTypeEnum.LandingPage },
			};

			if (m_isResponsive == false)
			{
				m_targetDirectoriesAndPageTypes.Add(Path.Combine(m_smartPhoneDirectoryName, "Page"), PageTypeEnum.CustomPage);
				m_targetDirectoriesAndPageTypes.Add(Path.Combine(m_smartPhoneDirectoryName, "Landing"), PageTypeEnum.LandingPage);
				m_targetDirectoriesAndPageTypes.Add(Path.Combine(m_smartPhoneDirectoryName, "Landing", "formlp"), PageTypeEnum.LandingPage);
				m_targetDirectoriesAndPageTypes.Add(Path.Combine(m_smartPhoneDirectoryName, "Page", "Feature"), PageTypeEnum.FeaturePage);
			}

			m_ignorePages = new[]
			{
				Path.Combine("Landing", "LandingCartConfirm.aspx"),
				Path.Combine("Landing", "LandingLinkShare.aspx"),
				Path.Combine("Landing", "formlp", "LpPreview.aspx"),
				Path.Combine(m_smartPhoneDirectoryName, "Landing", "formlp", "LpPreview.aspx"),
			};

			m_standardPages = Enumerable.Empty<SitemapPageItem>()
				.AppendElement(
					// PCトップ
					new SitemapPageItem
					{
						Url = ToUrl(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.PAGE_FRONT_DEFAULT)),
						PageType = PageTypeEnum.StandardPage,
						DeviceType = (m_isResponsive ? DeviceTypeEnum.PcAndSp : DeviceTypeEnum.Pc)
					})
				.AppendElement(
					// SPトップ
					new SitemapPageItem
					{
						Url = ToUrl(
							Path.Combine(
								Constants.PHYSICALDIRPATH_FRONT_PC,
								m_smartPhoneDirectoryName,
								Constants.PAGE_FRONT_DEFAULT)),
						PageType = PageTypeEnum.StandardPage,
						DeviceType = DeviceTypeEnum.Sp
					})
				.AppendElement(
					// PCトップ（ブランドONの場合追加）
					Constants.PRODUCT_BRAND_ENABLED
					? new SitemapPageItem
					{
						Url = ToUrl(
							Path.Combine(
								Constants.PHYSICALDIRPATH_FRONT_PC,
								Constants.PAGE_FRONT_DEFAULT_BRAND_TOP)),
						PageType = PageTypeEnum.StandardPage,
						DeviceType = DeviceTypeEnum.Pc
					} : null)
				.AppendElement(
					// SPトップ（ブランドONの場合追加）
					Constants.PRODUCT_BRAND_ENABLED
					? new SitemapPageItem
					{
						Url = ToUrl(
							Path.Combine(
								Constants.PHYSICALDIRPATH_FRONT_PC,
								m_smartPhoneDirectoryName,
								Constants.PAGE_FRONT_DEFAULT_BRAND_TOP)),
						PageType = PageTypeEnum.StandardPage,
						DeviceType = DeviceTypeEnum.Sp
					} : null)
				.AppendElement(
					// 商品一覧
					new SitemapPageItem
					{
						Url = ToUrl(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.PAGE_FRONT_PRODUCT_LIST)),
						PageType = PageTypeEnum.StandardPage,
						DeviceType = DeviceTypeEnum.PcAndSp,
						IsProductPage = true
					})
				.AppendElement(
					// 商品詳細
					new SitemapPageItem
					{
						Url = ToUrl(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.PAGE_FRONT_PRODUCT_DETAIL)),
						PageType = PageTypeEnum.StandardPage,
						DeviceType = DeviceTypeEnum.PcAndSp,
						IsProductPage = true
					})
				.Where(item => (item != null))
				.Where(item => (File.Exists(ToPhysicalPath(item.Url)) && ((m_isResponsive == false) || (item.DeviceType != DeviceTypeEnum.Sp))))
				.ToArray();

			m_coordinatePages = new[]
			{
				// コーディネートトップ
				new SitemapPageItem
				{
					Url = ToUrl(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.PAGE_FRONT_COORDINATE_TOP)),
					PageType = PageTypeEnum.Coordinate,
					DeviceType = DeviceTypeEnum.PcAndSp,
					IsCoordinatePage = true
				},
				// コーディネート一覧
				new SitemapPageItem
				{
					Url = ToUrl(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.PAGE_FRONT_COORDINATE_LIST)),
					PageType = PageTypeEnum.Coordinate,
					DeviceType = DeviceTypeEnum.PcAndSp,
					IsCoordinatePage = true
				},
				// コーディネート詳細
				new SitemapPageItem
				{
					Url = ToUrl(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.PAGE_FRONT_COORDINATE_DETAIL)),
					PageType = PageTypeEnum.Coordinate,
					DeviceType = DeviceTypeEnum.PcAndSp,
					IsCoordinatePage = true
				}
			}.Where(item => File.Exists(ToPhysicalPath(item.Url))).ToArray();
		}

		#region ページアイテム取得
		/// <summary>
		/// ページアイテムを取得
		/// </summary>
		/// <returns>ページアイテム</returns>
		public static IEnumerable<SitemapPageItem> GetPagesByPageType(PageTypeEnum pageType)
		{
			var result = new List<SitemapPageItem>();

			// 標準ページ
			if ((pageType == PageTypeEnum.None) || (pageType == PageTypeEnum.StandardPage))
			{
				result.AddRange(
					m_standardPages
						.Select(
							p =>
							{
								var item = p.Copy();
								item.Title = DesignCommon.GetAspxTitle(
									DesignCommon.GetFileTextAll(ToPhysicalPath(p.Url)));
								return item;
							}));
			}

			// コーディネートページ
			if ((pageType == PageTypeEnum.None) || (pageType == PageTypeEnum.Coordinate))
			{
				result.AddRange(
					m_coordinatePages
						.Select(
							p =>
							{
								var item = p.Copy();
								item.Title = DesignCommon.GetAspxTitle(
									DesignCommon.GetFileTextAll(ToPhysicalPath(p.Url)));
								return item;
							}));
			}

			if ((pageType == PageTypeEnum.None) || (pageType != PageTypeEnum.StandardPage))
			{
				// 対象ディレクトリからファイル取得
				var pageDatasOnDb = new PageDesignService().GetAllManagedPagesForXmlSitemap();
				result.AddRange(
					GetTargetFilePaths(pageType)
						.Select(
							path =>
							{
								// ReSharper disable once PossibleNullReferenceException
								var directory = Path.GetDirectoryName(path).Replace(Constants.PHYSICALDIRPATH_FRONT_PC, string.Empty);
								var item = new SitemapPageItem
								{
									Url = ToUrl(path),
									PageType = m_targetDirectoriesAndPageTypes[directory],
									DeviceType =
										(m_isResponsive
											? DeviceTypeEnum.PcAndSp
											: (directory.StartsWith(m_smartPhoneDirectoryName)
												? DeviceTypeEnum.Sp
												: DeviceTypeEnum.Pc))
								};

								// タイトル取得
								// DBから取れなければページディレクティブから取得
								var foundItem = pageDatasOnDb
									.FirstOrDefault(
										x => (Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, x.FileDirPath, x.FileName)
												== ((m_isResponsive == false)
													? path.Replace(m_smartPhoneDirectoryName, string.Empty)
													: path)));
								item.Title = (foundItem != null)
									? foundItem.ManagementTitle
									: DesignCommon.GetAspxTitle(
										DesignCommon.GetFileTextAll(path));

								return item;
							}));
			}

			// SitemapSettingの内容を反映
			var sitemapSetting =
				SerializeHelper.DeserializeFromXmlFile<SitemapSetting>(Constants.FILE_SITEMAPSETTING_PC);
			foreach (var url in sitemapSetting.SpecifiedUrls)
			{
				var foundItem = result.FirstOrDefault(x => (x.Url == url.Location));
				if (foundItem == null) continue;

				foundItem.IsIncluded = true;
				foundItem.ChangeFrequency = url.ChangeFrequency;
				foundItem.Priority = url.Priority;
			}

			// 商品系ページの設定を反映
			foreach (var productPage in result.Where(r => r.IsProductPage))
			{
				var isProductList = productPage.Url.Contains(Constants.PAGE_FRONT_PRODUCT_LIST);
				var isProductDetail = productPage.Url.Contains(Constants.PAGE_FRONT_PRODUCT_DETAIL);
				var setting = isProductList
					? sitemapSetting.SettingForProductList
					: isProductDetail
						? sitemapSetting.SettingForProductDetail
						: null;

				if (setting == null) continue;

				productPage.IsIncluded = true;
				productPage.ChangeFrequency = setting.ChangeFrequency;
				productPage.Priority = setting.Priority;
			}

			// コーディネート系ページの設定を反映
			foreach (var coordinatePage in result.Where(r => r.IsCoordinatePage))
			{
				var isCoordinateList = coordinatePage.Url.Contains(Constants.PAGE_FRONT_COORDINATE_LIST);
				var isCoordinateDetail = coordinatePage.Url.Contains(Constants.PAGE_FRONT_COORDINATE_DETAIL);
				var isCoordinateTop = coordinatePage.Url.Contains(Constants.PAGE_FRONT_COORDINATE_TOP);
				var setting = isCoordinateList
					? sitemapSetting.SettingForCoordinateList
					: isCoordinateDetail
						? sitemapSetting.SettingForCoordinateDetail
						: isCoordinateTop
							? sitemapSetting.SettingForCoordinateTop
							: null;

				if (setting == null) continue;

				coordinatePage.IsIncluded = true;
				coordinatePage.ChangeFrequency = setting.ChangeFrequency;
				coordinatePage.Priority = setting.Priority;
			}

			return result;
		}
		#endregion

		#region 設定更新
		/// <summary>
		/// 設定更新
		/// </summary>
		/// <param name="pageItem">ページアイテム</param>
		public static void UpdateSetting(SitemapPageItem[] pageItem)
		{
			var setting
				= SerializeHelper.DeserializeFromXmlFile<SitemapSetting>(Constants.FILE_SITEMAPSETTING_PC);

			// 含める指定されていて現在含まれていないもの追加
			setting.SpecifiedUrls.AddRange(
				pageItem
					.Where(
						p => (p.IsIncluded
							&& (p.IsProductPage == false)
							&& (p.IsCoordinatePage == false)
							&& (setting.SpecifiedUrls.All(su => (su.Location != p.Url)))))
					.Select(
						p => new XmlSitemapUrl
						{
							Location = p.Url,
							Priority = p.Priority,
							ChangeFrequency = p.ChangeFrequency
						}));

			// 更新頻度、優先度を反映
			foreach (var includedPageItem in pageItem.Where(pi => pi.IsIncluded))
			{
				var found = setting.SpecifiedUrls.FirstOrDefault(su => (su.Location == includedPageItem.Url));
				if (found == null) continue;

				found.Priority = includedPageItem.Priority;
				found.ChangeFrequency = includedPageItem.ChangeFrequency;
			}

			// 含めない指定されていて現在含まれているもの削除
			setting.SpecifiedUrls.RemoveAll(
				su => pageItem.Any(pi => ((pi.IsIncluded == false) && (su.Location == pi.Url))));

			// 商品系ページ設定
			if (pageItem.Any(p => p.IsProductPage))
			{
				setting.SettingForProductList = ConvertToSitemapUrl(
					pageItem.FirstOrDefault(p => (p.IsIncluded && p.Url.Contains(Constants.PAGE_FRONT_PRODUCT_LIST))));
				setting.SettingForProductDetail = ConvertToSitemapUrl(
					pageItem.FirstOrDefault(p => (p.IsIncluded && p.Url.Contains(Constants.PAGE_FRONT_PRODUCT_DETAIL))));

				// 商品系ページにLocationは必要ない
				if (setting.SettingForProductList != null) setting.SettingForProductList.Location = string.Empty;
				if (setting.SettingForProductDetail != null) setting.SettingForProductDetail.Location = string.Empty;
			}

			// コーディネート系ページ設定
			if (pageItem.Any(p => p.IsCoordinatePage))
			{
				setting.SettingForCoordinateList = ConvertToSitemapUrl(
					pageItem.FirstOrDefault(p => (p.IsIncluded && p.Url.Contains(Constants.PAGE_FRONT_COORDINATE_LIST))));
				setting.SettingForCoordinateDetail = ConvertToSitemapUrl(
					pageItem.FirstOrDefault(p => (p.IsIncluded && p.Url.Contains(Constants.PAGE_FRONT_COORDINATE_DETAIL))));
				setting.SettingForCoordinateTop = ConvertToSitemapUrl(
					pageItem.FirstOrDefault(p => (p.IsIncluded && p.Url.Contains(Constants.PAGE_FRONT_COORDINATE_TOP))));

				// コーディネート系ページにLocationは必要ない
				if (setting.SettingForCoordinateList != null) setting.SettingForCoordinateList.Location = string.Empty;
				if (setting.SettingForCoordinateDetail != null) setting.SettingForCoordinateDetail.Location = string.Empty;
				if (setting.SettingForCoordinateTop != null) setting.SettingForCoordinateTop.Location = string.Empty;
			}

			// 更新ついでに既に存在しないファイルの消込
			var paths = GetTargetFilePaths().ToArray();
			setting.SpecifiedUrls.RemoveAll(
				su => (paths.All(p => (su.Location != ToUrl(p))))
					&& (m_standardPages.All(sp => (sp.Url != su.Location))));

			// ファイル保存
			SerializeHelper.SerializeToXmlFile(setting, Constants.FILE_SITEMAPSETTING_PC);
		}
		#endregion

		#region 対象ディレクトリからファイルパスの一覧取得
		/// <summary>
		/// 対象ディレクトリからページのファイルパス一覧取得
		/// </summary>
		/// <param name="pageType">ページ種別(省略時は全部)</param>
		/// <returns>ファイルパス</returns>
		private static IEnumerable<string> GetTargetFilePaths(PageTypeEnum pageType = PageTypeEnum.None)
		{
			var result = m_targetDirectoriesAndPageTypes.Keys
				.Where(k => ((pageType == PageTypeEnum.None) || (m_targetDirectoriesAndPageTypes[k] == pageType)))
				.Where(d => Directory.Exists(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, d)))
				.SelectMany(d => Directory.EnumerateFiles(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, d), "*", SearchOption.TopDirectoryOnly)) // 対象ディレクトリ検索
				.Where(path => path.ToLower().EndsWith(m_targetFileExtension)) // 拡張子で絞り込み
				.Where(p => (m_ignorePages.Any(p.Contains) == false));

			return result;
		}
		#endregion

		#region サイトマップ設定ファイルのシリアライズ試行
		/// <summary>
		/// サイトマップ設定ファイルのシリアライズ試行
		/// 失敗した場合はログの落とし込みもする
		/// </summary>
		/// <returns>シリアライズできたか</returns>
		public static bool TrySerializeSitemapSetting()
		{
			try
			{
				var attr = File.GetAttributes(Constants.FILE_SITEMAPSETTING_PC);
				//読み取り専用属性があるか調べる
				if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					//読み取り専用属性を削除する
					File.SetAttributes(Constants.FILE_SITEMAPSETTING_PC, attr & (~FileAttributes.ReadOnly));
				}

				SerializeHelper.DeserializeFromXmlFile<SitemapSetting>(Constants.FILE_SITEMAPSETTING_PC);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return false;
			}

			return true;
		}
		#endregion

		#region 変換系のメソッド
		/// <summary>
		/// ページアイテムからサイトマップURLに変換
		/// </summary>
		/// <param name="pageItem">ページアイテム</param>
		/// <returns>サイトマップURL</returns>
		private static XmlSitemapUrl ConvertToSitemapUrl(SitemapPageItem pageItem)
		{
			if (pageItem == null) return null;

			var url = new XmlSitemapUrl
			{
				Location = pageItem.Url,
				Priority = pageItem.Priority,
				ChangeFrequency = pageItem.ChangeFrequency
			};
			return url;
		}

		/// <summary>
		/// URLから物理パスに変換
		/// </summary>
		/// <param name="url">URL</param>
		/// <returns>物理パス</returns>
		private static string ToPhysicalPath(string url)
		{
			var physicalPath = url.Replace(m_frontRootUrl, Constants.PHYSICALDIRPATH_FRONT_PC).Replace("/", @"\");
			return physicalPath;
		}

		/// <summary>
		/// 物理パスからURLに変換
		/// </summary>
		/// <param name="physicalPath">物理パス</param>
		/// <returns>URL</returns>
		private static string ToUrl(string physicalPath)
		{
			var virtualPath = NormalizeUrl(physicalPath.Replace(Constants.PHYSICALDIRPATH_FRONT_PC, m_frontRootUrl));
			return virtualPath;
		}

		/// <summary>
		/// URL正常化
		/// </summary>
		/// <param name="url">URL</param>
		/// <returns>URL</returns>
		/// <remarks>単純に文字列結合するとURLに円記号が混ざることがあるため、置換処理で正常化する</remarks>
		private static string NormalizeUrl(string url)
		{
			var normalizedUrl = url.Replace(@"\", "/");
			return normalizedUrl;
		}
		#endregion
	}
}