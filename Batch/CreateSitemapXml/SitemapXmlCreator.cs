/*
=========================================================================================================
  Module      : サイトマップXML作成処理(SitemapXmlCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.Common.Sql;
using System.Text.RegularExpressions;
using w2.App.Common.ProductDefaultSetting;
using w2.Common.Web;
using w2.Domain.Coordinate;
using w2.Domain.CoordinateCategory;
using w2.Domain.ProductBrand;

namespace w2.Commerce.Batch.CreateSitemapXml
{
	/// <summary>
	/// サイトマップXML作成処理
	/// </summary>
	public class SitemapXmlCreator
	{
		private int iFileNo = 1;								// ファイル番号
		private int iUrlCount = 0;								// 書き込みしたURLの件数
		private bool blNewFileFlg = true;						// ファイル作成フラグ
		XmlTextWriter xwSitemapXml = null;						// 出力先ファイル
		List<string> lstrSitemapXmls = new List<string>();		// 作成したXMLファイル名格納用
		private string strEntryNodeTemp = null;					// ノード一時格納用

		// 更新頻度に設定できる値
		private static readonly string[] m_changeFreqItems = { "always", "hourly", "daily", "weekly", "monthly", "yearly", "never" };

		/// <summary>
		/// サイトマップXML作成処理
		/// </summary>
		public void CreateSitemapXml()
		{
			this.AlartMessage = new StringBuilder();

			//------------------------------------------------------
			// サイト別の設定取得
			//------------------------------------------------------
			GetSitemapSetting();

			//------------------------------------------------------
			// サイトマップXMLの一時出力先フォルダがなければ作成
			//-----------------------------------------------------
			if (Directory.Exists(this.PhysicalDirPathTemp) == false)
			{
				Directory.CreateDirectory(this.PhysicalDirPathTemp);
			}

			//------------------------------------------------------
			// 個別指定ページXML作成
			//------------------------------------------------------
			CreateSpecificUrlXmls();

			//------------------------------------------------------
			// 商品一覧ページXML作成
			//------------------------------------------------------
			if (this.ShouldGenerateProductListUrl) CreateProductListXml();

			//------------------------------------------------------
			// 商品詳細ページXML作成
			//------------------------------------------------------
			if (this.ShouldGenerateProductDetailUrl) CreateProductDetailXmls();

			// コーディネート詳細ページXML作成
			if (this.ShouldGenerateCoordinateDetailUrl) CreateCoordinateDetailXmls();

			// コーディネート一覧ページXML作成
			if (this.ShouldGenerateCoordinateListUrl) CreateCoordinateListXmls();

			// コーディネートトップページXML作成
			if (this.ShouldGenerateCoordinateTopUrl) CreateCoordinateTopXml();

			//------------------------------------------------------
			// 書き込み終了
			//------------------------------------------------------
			if (xwSitemapXml != null)
			{
				xwSitemapXml.WriteString("\r\n");
				// ノード終了
				xwSitemapXml.WriteEndElement();
				// ストリームを閉じる
				xwSitemapXml.Close();
			}

			//------------------------------------------------------
			// 複数ファイル作成されていたらインデックスファイルを作成
			//------------------------------------------------------
			if (iFileNo > 1)
			{
				CreateSitemapIndexXml();
			}

			//------------------------------------------------------
			// 既存のファイルを削除し、テンポラリフォルダからファイルを移動
			//------------------------------------------------------
			CommitSitemapXml();
		}

		/// <summary>
		/// サイト別設定取得
		/// </summary>
		private void GetSitemapSetting()
		{
			this.PhysicalDirPath = Constants.PHYSICALDIRPATH_FRONT_PC;
			this.PhysicalDirPathTemp = Constants.PHYSICALDIRPATH_TEMP + @"\Pc\";
			this.SiteRootPath = Constants.URL_FRONT_PC;
			this.SitemapSettingFile = Constants.FILE_SITEMAPSETTING_PC;

			//------------------------------------------------------
			// 設定XMLファイル読み込み
			//------------------------------------------------------
			XmlDocument xdSitemapSetting = new XmlDocument();
			try
			{
				xdSitemapSetting.Load(this.SitemapSettingFile);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("SitemapSetting.xmlファイルの読み込みに失敗しました。" + ex);
			}

			// 商品一覧ページの設定取得
			XmlNode xnProductList = xdSitemapSetting.SelectSingleNode("SitemapSetting/ProductList");
			this.ShouldGenerateProductListUrl = (xnProductList != null);
			if (this.ShouldGenerateProductListUrl)
			{
				this.ProductListChangeFreq = GetChangeFreq(xnProductList.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "商品一覧");
				this.ProductListPriority = GetPriority(xnProductList.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "商品一覧");
			}

			// 商品詳細ページの設定取得
			XmlNode xnProductDetail = xdSitemapSetting.SelectSingleNode("SitemapSetting/ProductDetail");
			this.ShouldGenerateProductDetailUrl = (xnProductDetail != null);
			if (this.ShouldGenerateProductDetailUrl)
			{
				this.ProductDetailChangeFreq = GetChangeFreq(xnProductDetail.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "商品詳細");
				this.ProductDetailPriority = GetPriority(xnProductDetail.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "商品詳細");
			}

			// コーディネート一覧ページの設定取得
			var coordinateListNode = xdSitemapSetting.SelectSingleNode("SitemapSetting/CoordinateList");
			this.ShouldGenerateCoordinateListUrl = (coordinateListNode != null);
			if (this.ShouldGenerateCoordinateListUrl)
			{
				this.CoordinateListChangeFreq = GetChangeFreq(coordinateListNode.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "コーディネート一覧");
				this.CoordinateListPriority = GetPriority(coordinateListNode.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "コーディネート一覧");
			}

			// コーディネート詳細ページの設定取得
			var coordinateDetailNode = xdSitemapSetting.SelectSingleNode("SitemapSetting/CoordinateDetail");
			this.ShouldGenerateCoordinateDetailUrl = (coordinateDetailNode != null);
			if (this.ShouldGenerateCoordinateDetailUrl)
			{
				this.CoordinateDetailChangeFreq = GetChangeFreq(coordinateDetailNode.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "コーディネート詳細");
				this.CoordinateDetailPriority = GetPriority(coordinateDetailNode.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "コーディネート詳細");
			}

			// コーディネートトップページの設定取得
			var coordinateTopNode = xdSitemapSetting.SelectSingleNode("SitemapSetting/CoordinateTop");
			this.ShouldGenerateCoordinateTopUrl = (coordinateTopNode != null);
			if (this.ShouldGenerateCoordinateTopUrl)
			{
				this.CoordinateTopChangeFreq = GetChangeFreq(coordinateTopNode.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "コーディネートトップ");
				this.CoordinateTopPriority = GetPriority(coordinateTopNode.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "コーディネートトップ");
			}

			// 記事一覧ページの設定取得
			var articleListNode = xdSitemapSetting.SelectSingleNode("SitemapSetting/ArticleList");
			this.ShouldGenerateArticleListUrl = (articleListNode != null);
			if (this.ShouldGenerateArticleListUrl)
			{
				this.ArticleListChangeFreq = GetChangeFreq(articleListNode.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "記事一覧");
				this.ArticleListPriority = GetPriority(articleListNode.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "記事一覧");
			}

			// 記事詳細ページの設定取得
			var articleDetailNode = xdSitemapSetting.SelectSingleNode("SitemapSetting/ArticleDetail");
			this.ShouldGenerateArticleDetailUrl = (articleDetailNode != null);
			if (this.ShouldGenerateArticleDetailUrl)
			{
				this.ArticleDetailChangeFreq = GetChangeFreq(articleDetailNode.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "記事詳細");
				this.ArticleDetailPriority = GetPriority(articleDetailNode.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "記事詳細");
			}

			// 記事トップページの設定取得
			var articleTopNode = xdSitemapSetting.SelectSingleNode("SitemapSetting/ArticleTop");
			this.ShouldGenerateArticleTopUrl = (articleTopNode != null);
			if (this.ShouldGenerateArticleTopUrl)
			{
				this.ArticleTopChangeFreq = GetChangeFreq(articleTopNode.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "記事トップ");
				this.ArticleTopPriority = GetPriority(articleTopNode.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "記事トップ");
			}
		}

		#region "個別指定URL用XML作成関連"
		/// <summary>
		/// 個別指定URL用XML作成
		/// </summary>
		private void CreateSpecificUrlXmls()
		{
			// 設定ファイル読み込み
			XmlDocument xdSitemapSetting = new XmlDocument();
			try
			{
				xdSitemapSetting.Load(this.SitemapSettingFile);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("SitemapSetting.xmlファイルの読み込みに失敗しました。" + ex);
			}

			// urlタグあるだけループ
			int iLoopCount = 1;
			XmlNodeList xnlURLs = xdSitemapSetting.SelectNodes("SitemapSetting/SpecificUrls/url");
			foreach (XmlNode xnURL in xnlURLs)
			{
				DateTime dtLastMod = new DateTime();
				if (xnURL.SelectSingleNode(Constants.SITEMAPXML_ELE_LOCATION) != null)
				{
					string strUrl = xnURL.SelectSingleNode(Constants.SITEMAPXML_ELE_LOCATION).InnerText;

					// ファイルの更新日を取得(ファイルが存在しない場合は今日の日付)
					string strFile = (strUrl.Replace(Constants.URL_FRONT_PC, Constants.PHYSICALDIRPATH_FRONT_PC)).Replace('/', '\\');
					if (File.Exists(strFile))
					{
						dtLastMod = File.GetLastWriteTime(strFile);
					}
					else
					{
						dtLastMod = DateTime.Now;
					}

					// エントリノード作成
					var changeFreq = GetChangeFreq(xnURL.SelectSingleNode(Constants.SITEMAPXML_ELE_CHANGE_FREQ), "個別ページ" + iLoopCount + "件目");
					var priority = GetPriority(xnURL.SelectSingleNode(Constants.SITEMAPXML_ELE_PRIORITY), "個別ページ" + iLoopCount + "件目");

					if (Constants.PRODUCT_BRAND_ENABLED && strUrl.Contains(Constants.PAGE_FRONT_DEFAULT_BRAND_TOP))
					{
						var urlList = CreateBrandXmls(strUrl);
						foreach (var url in urlList)
						{
							var brandEntryNode = CreateUrlElement(
								url,
								dtLastMod,
								changeFreq,
								priority);

							// 書き込み
							WriteXml(brandEntryNode);
						}
					}
					else
					{
						// エントリノード作成
						var entryNode = CreateUrlElement(
							strUrl,
							dtLastMod,
							changeFreq,
							priority);

						// 書き込み
						WriteXml(entryNode);
					}
				}
				else
				{
					this.AlartMessage.Append("個別ページ").Append(iLoopCount.ToString()).Append("件目のURLが不正です。ノードは出力されません。\r\n");
				}

				iLoopCount++;
			}
		}
		#endregion

		#region "ブランドXML作成関連"
		/// <summary>
		/// ブランド詳細XML作成
		/// </summary>
		/// <param name="url">URL</param>
		/// <returns>ブランドIDつけたURLリスト</returns>
		private List<string> CreateBrandXmls(string url)
		{
			var brandList = new ProductBrandService().GetValidBrandList();
			var urlList = new List<string>();
			if (brandList.Length > 0)
			{
				foreach (var brand in brandList)
				{
					var newUrl = new UrlCreator(url);
					var brandId = brand.BrandId;
					newUrl.AddParam(Constants.REQUEST_KEY_BRAND_ID, brandId);
					urlList.Add(newUrl.CreateUrl());
				}
			}
			return urlList;
		}
		#endregion

		#region "商品一覧XML作成関連"
		/// <summary>
		/// 商品一覧XML作成
		/// </summary>
		private void CreateProductListXml()
		{
			// 商品カテゴリー情報取得
			DataView dvProductCategoryList = GetProductCategoryList();

			// 無効カテゴリ以下のカテゴリ削除
			int iCategoryCount = dvProductCategoryList.Count;
			for (int iLoop = 0; iLoop < iCategoryCount; iLoop++)
			{
				if ((string)dvProductCategoryList[iLoop][Constants.FIELD_PRODUCTCATEGORY_VALID_FLG] == Constants.FLG_PRODUCTCATEGORY_VALID_FLG_INVALID)
				{
					string strUnvalidCategoryId = (string)dvProductCategoryList[iLoop][Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID];

					for (int iLoop2 = 0; iLoop2 < iCategoryCount; iLoop2++)
					{
						if (((string)dvProductCategoryList[iLoop2][Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]).IndexOf(strUnvalidCategoryId) == 0)
						{
							dvProductCategoryList.Delete(iLoop2);
							iCategoryCount--;
							iLoop2--;
						}
					}

					iLoop = 0;	// わからなくなったのでリセット
				}
			}

			// ブランド使用時
			if (Constants.PRODUCT_BRAND_ENABLED)
			{
				DataView dvBrand = ProductBrandUtility.GetProductBrandList();

				foreach (DataRowView drv in dvBrand)
				{
					string strBrandId = (string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID];
					string strBrandName = (string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_NAME];

					CreateProductCategory(dvProductCategoryList, strBrandId, strBrandName);
				}
			}
			// ブランド未使用
			else
			{
				CreateProductCategory(dvProductCategoryList);
			}
		}

		/// <summary>
		/// 商品カテゴリリンクの作成
		/// </summary>
		/// <param name="dvProductCategoryList"></param>
		/// <param name="strBrandId"></param>
		/// <param name="strBrandName"></param>
		private void CreateProductCategory(DataView dvProductCategoryList, string strBrandId = "", string strBrandName = "")
		{
			foreach (DataRowView drvProductCategory in dvProductCategoryList)
			{
				// 許可されていないブランドIDの場合は次へ
				if (Constants.PRODUCT_BRAND_ENABLED)
				{
					var permittedBrandIds =
						drvProductCategory[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS].ToString().Split(',');
					if ((permittedBrandIds.Any(x => (string.IsNullOrEmpty(x)) == false)) && (permittedBrandIds.Contains(strBrandId) == false)) continue;
				}

				var	strProductListUrl = CreateProductListUrlForPc(drvProductCategory, strBrandId, strBrandName);

				// ノード作成
				string strEntryNode = CreateUrlElement(
					strProductListUrl,
					(DateTime)drvProductCategory[Constants.FIELD_PRODUCTCATEGORY_DATE_CHANGED],
					this.ProductListChangeFreq,
					this.ProductListPriority);

				// XML書き込み
				WriteXml(strEntryNode);
			}
		}

		/// <summary>
		/// 商品カテゴリー情報取得
		/// </summary>
		/// <returns>商品カテゴリー情報</returns>
		private DataView GetProductCategoryList()
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("CreateSitemapXml", "GetProductCategory"))
			{
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}
		}

		/// <summary>
		/// PC用商品一覧URL取得
		/// </summary>
		/// <param name="drvProductCategory">商品カテゴリ情報</param>
		/// <param name="strBrandId">ブランドID</param>
		/// <param name="strBrandName">ブランド名</param>
		/// <returns>PC用商品一覧URL</returns>
		private string CreateProductListUrlForPc(DataRowView drvProductCategory, string strBrandId, string strBrandName)
		{
			StringBuilder sbProductListUrl = new StringBuilder();
			sbProductListUrl.Append(Constants.URL_FRONT_PC);
			sbProductListUrl.Append(ProductCommon.CreateProductListUrl(
										drvProductCategory[Constants.FIELD_PRODUCTCATEGORY_SHOP_ID],
										drvProductCategory[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID],
										"",
										"",
										"",
										"",
										"",
										ProductListDispSettingUtility.SortDefault,
										strBrandId,
										ProductListDispSettingUtility.DispImgKbnDefault,
										"",
										(string)drvProductCategory[Constants.FIELD_PRODUCTCATEGORY_NAME],
										strBrandName,
										Constants.UNDISPLAY_NOSTOCK_PRODUCT_ENABLED ? Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_UNDISPLAY_NOSTOCK : Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK,
										"",
										-1));

			return sbProductListUrl.ToString();
		}
		#endregion

		#region "商品詳細XML作成関連"
		/// <summary>
		/// 商品詳細XML作成
		/// </summary>
		private void CreateProductDetailXmls()
		{
			// 商品情報取得
			DataView dvProductList = GetProductList();

			if (dvProductList.Count > 0)
			{
				foreach (DataRowView drvProduct in dvProductList)
				{
					// 商品詳細ページURL取得
					var	strProductDetailUrl = CreateProductDetailUrlForPc(drvProduct);
					// ノード作成
					string strEntryNode = CreateUrlElement(
						strProductDetailUrl,
						(DateTime)drvProduct[Constants.FIELD_PRODUCTCATEGORY_DATE_CHANGED],
						this.ProductDetailChangeFreq,
						this.ProductDetailPriority);

					// XML書き込み
					WriteXml(strEntryNode);
				}
			}
		}

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <returns>商品情報</returns>
		private DataView GetProductList()
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("CreateSitemapXml", "GetProduct"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_SHOP_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID);			// デフォルト店舗ID

				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// PC用商品詳細URL取得
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <returns>PC用商品詳細URL</returns>
		private string CreateProductDetailUrlForPc(DataRowView drvProduct)
		{
			string strBrandId = (string)drvProduct[Constants.FIELD_PRODUCT_BRAND_ID1];
			string strBrandName = ProductBrandUtility.GetProductBrandName(strBrandId);

			StringBuilder sbProductListUrl = new StringBuilder();
			sbProductListUrl.Append(Constants.URL_FRONT_PC);
			sbProductListUrl.Append(ProductCommon.CreateProductDetailUrl(
										drvProduct[Constants.FIELD_PRODUCT_SHOP_ID],
										"",
										strBrandId,
										"",
										drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID],
										(string)drvProduct[Constants.FIELD_PRODUCT_NAME],
										strBrandName));

			return sbProductListUrl.ToString();
		}
		#endregion

		#region "コーディネートXML作成関連"
		/// <summary>
		/// コーディネート詳細XML作成
		/// </summary>
		private void CreateCoordinateDetailXmls()
		{
			// コーディネート情報取得
			var coordinateList = new CoordinateService().GetAll().Where(
				c => (c.DisplayKbn == Constants.FLG_COORDINATE_DISPLAY_KBN_PUBLIC)
					&& (c.DisplayDate <= DateTime.Now)).ToArray();

			foreach (var coordinate in coordinateList)
			{
				var param = new Tuple<string, string>(Constants.REQUEST_KEY_COORDINATE_ID, coordinate.CoordinateId);
				var url = CreateUrl(Constants.PAGE_FRONT_COORDINATE_DETAIL, param);

				// ノード作成
				var entryNode = CreateUrlElement(
					url,
					coordinate.DateChanged,
					this.CoordinateDetailChangeFreq,
					this.CoordinateDetailPriority);

				// XML書き込み
				WriteXml(entryNode);
			}
		}

		/// <summary>
		/// コーディネート一覧XML作成
		/// </summary>
		private void CreateCoordinateListXmls()
		{
			// コーディネートカテゴリー情報取得
			var coordinateCategoryList = new CoordinateCategoryService().GetAll().ToList();

			// 無効カテゴリ以下のカテゴリ削除
			var categoryCountTmp = coordinateCategoryList.Count;
			for (var loop = 0; loop < categoryCountTmp; loop++)
			{
				if (coordinateCategoryList[loop].ValidFlg 
					!= Constants.FLG_COORDINATECATEGORY_VALID_FLG_INVALID) continue;
				var unvalidCategoryId = coordinateCategoryList[loop].CoordinateCategoryId;

				for (var loop2 = 0; loop2 < categoryCountTmp; loop2++)
				{
					if ((coordinateCategoryList[loop2].CoordinateCategoryId).IndexOf(
						unvalidCategoryId,
						StringComparison.CurrentCulture) == -1) continue;
					coordinateCategoryList.RemoveAt(loop2);
					categoryCountTmp--;
					loop2--;
				}
				loop = 0;
			}

			CreateCoordinateCategoryLink(coordinateCategoryList);
		}

		/// <summary>
		/// コーディネートカテゴリリンクの作成
		/// </summary>
		/// <param name="coordinateCategoryList">コーディネートカテゴリリスト</param>
		private void CreateCoordinateCategoryLink(List<CoordinateCategoryModel> coordinateCategoryList)
		{
			foreach (var category in coordinateCategoryList)
			{
				var param = new Tuple<string, string>(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, category.CoordinateCategoryId);
				var url = CreateUrl(Constants.PAGE_FRONT_COORDINATE_LIST, param);

				// ノード作成
				var entryNode = CreateUrlElement(
					url,
					category.DateChanged,
					this.CoordinateListChangeFreq,
					this.CoordinateListPriority);

				// XML書き込み
				WriteXml(entryNode);
			}
		}

		/// <summary>
		/// コーディネートトップXML作成
		/// </summary>
		private void CreateCoordinateTopXml()
		{
			var url = CreateUrl(Constants.PAGE_FRONT_COORDINATE_TOP);
			// ノード作成
			var entryNode = CreateUrlElement(
				url,
				DateTime.Now,
				this.CoordinateTopChangeFreq,
				this.CoordinateTopPriority);

			// XML書き込み
			WriteXml(entryNode);
		}
		#endregion

		/// <summary>
		/// sitemap.xml書き込み
		/// </summary>
		/// <param name="strEntryNode">エントリノード</param>
		private void WriteXml(string strEntryNode)
		{
			//------------------------------------------------------
			// ストリーム初期化
			//------------------------------------------------------
			// 新規ファイルフラグがtrueの場合は、ストリームを初期化
			if (blNewFileFlg)
			{
				xwSitemapXml = new XmlTextWriter(this.PhysicalDirPathTemp + "sitemap" + iFileNo.ToString() + ".xml", Encoding.GetEncoding("UTF-8"));
				// ファイルを作成したらファイル名をリストに追加
				lstrSitemapXmls.Add("sitemap" + iFileNo.ToString() + ".xml");

				// 出力書式設定
				xwSitemapXml.Formatting = Formatting.Indented;
				// インデント設定
				xwSitemapXml.Indentation = 2;

				xwSitemapXml.WriteStartDocument();
				xwSitemapXml.WriteStartElement(Constants.SITEMAPXML_ELE_URLSET);
				xwSitemapXml.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

				// フラグ、カウントリセット
				blNewFileFlg = false;
				iUrlCount = 0;

				// 複数ファイル目の場合は、一時格納しておいた商品エントリーをストリームに書き込み
				if (iFileNo > 1)
				{
					xwSitemapXml.WriteString("\r\n");
					xwSitemapXml.WriteRaw(strEntryNodeTemp);
					iUrlCount++;
				}
			}

			xwSitemapXml.WriteString("\r\n");

			// ファイル最大ストリーム長を超える場合、または最大URL数に達している場合、ファイルを区切る
			if ((xwSitemapXml.BaseStream.Length + Encoding.UTF8.GetByteCount(strEntryNode) >= Constants.SITEMAPXML_MAX_STREAM_LENGTH)
				|| (iUrlCount == Constants.SITEMAPXML_MAX_URL_COUNT))
			{
				// ノード終了
				xwSitemapXml.WriteEndElement();

				// ストリームを閉じる
				xwSitemapXml.Close();

				// ファイルNoをカウントアップ
				iFileNo++;
				// 新規ファイルフラグをtrueに設定
				blNewFileFlg = true;
				// 現在のループの商品エントリーを一時格納
				strEntryNodeTemp = strEntryNode;
			}
			// ストリームに商品エントリーを書き込み
			else
			{
				xwSitemapXml.WriteRaw(strEntryNode);
				iUrlCount++;
			}
		}

		/// <summary>
		/// sitemap.xmlのURL要素作成
		/// </summary>
		/// <param name="strLocation">URL</param>
		/// <param name="dtLastmod">最終更新日</param>
		/// <param name="strChangeFreq">更新頻度</param>
		/// <param name="strPriority">優先度</param>
		private string CreateUrlElement(string strLocation, DateTime dtLastmod, string strChangeFreq, string strPriority)
		{
			using (StringWriter sw = new StringWriter())
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(sw))
			{
				// xml出力開始
				xmlTextWriter.WriteStartElement(Constants.SITEMAPXML_ELE_URL);
				// URL
				xmlTextWriter.WriteStartElement(Constants.SITEMAPXML_ELE_LOCATION);
				xmlTextWriter.WriteString(strLocation);
				xmlTextWriter.WriteEndElement();
				// 更新日
				xmlTextWriter.WriteStartElement(Constants.SITEMAPXML_ELE_LAST_MOD);
				xmlTextWriter.WriteString(dtLastmod.ToString("yyyy-MM-dd"));
				xmlTextWriter.WriteEndElement();
				// 更新頻度
				if (string.IsNullOrEmpty(strChangeFreq) == false)
				{
					xmlTextWriter.WriteStartElement(Constants.SITEMAPXML_ELE_CHANGE_FREQ);
					xmlTextWriter.WriteString(strChangeFreq);
					xmlTextWriter.WriteEndElement();
				}
				// 優先度
				if (string.IsNullOrEmpty(strPriority) == false)
				{
					xmlTextWriter.WriteStartElement(Constants.SITEMAPXML_ELE_PRIORITY);
					xmlTextWriter.WriteString(strPriority);
					xmlTextWriter.WriteEndElement();
				}
				// xml出力終了
				xmlTextWriter.WriteEndElement();

				return sw.ToString();
			}
		}

		/// <summary>
		/// サイトマップインデックスファイル作成
		/// </summary>
		private void CreateSitemapIndexXml()
		{
			//------------------------------------------------------
			// xmlファイル作成
			//------------------------------------------------------
			XmlTextWriter xwSitemapIndexXml
				= new XmlTextWriter(this.PhysicalDirPathTemp + "sitemap_index.xml", Encoding.GetEncoding("UTF-8"));	// 出力先ファイル
			xwSitemapIndexXml.Formatting = Formatting.Indented;

			// リストにファイル名格納
			lstrSitemapXmls.Add("sitemap_index.xml");

			//------------------------------------------------------
			// xml書き込み開始
			//------------------------------------------------------
			xwSitemapIndexXml.WriteStartDocument();
			xwSitemapIndexXml.WriteStartElement(Constants.SITEMAPINDEXXML_ELE_SITEMAPINDEX);
			xwSitemapIndexXml.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

			//------------------------------------------------------
			// ノード作成
			//------------------------------------------------------
			for (int iLoop = 1; iLoop <= iFileNo; iLoop++)
			{
				// xml出力開始
				xwSitemapIndexXml.WriteStartElement(Constants.SITEMAPINDEXXML_ELE_SITEMAP);
				// ファイルの場所
				xwSitemapIndexXml.WriteStartElement(Constants.SITEMAPINDEXXML_ELE_LOCATION);
				xwSitemapIndexXml.WriteString(this.SiteRootPath + "sitemap" + iLoop.ToString() + ".xml");
				xwSitemapIndexXml.WriteEndElement();
				// 更新日
				xwSitemapIndexXml.WriteStartElement(Constants.SITEMAPINDEXXML_ELE_LAST_MOD);
				xwSitemapIndexXml.WriteString(DateTime.Now.ToString("yyyy-MM-dd"));
				xwSitemapIndexXml.WriteEndElement();
				// xml出力終了
				xwSitemapIndexXml.WriteEndElement();
			}

			//------------------------------------------------------
			// xml書き込み終了
			//------------------------------------------------------
			xwSitemapIndexXml.WriteEndElement();
			xwSitemapIndexXml.WriteEndDocument();

			//------------------------------------------------------
			// sitemap.xml出力
			//------------------------------------------------------
			xwSitemapIndexXml.Flush();
			xwSitemapIndexXml.Close();
		}

		/// <summary>
		/// 更新頻度取得
		/// </summary>
		/// <param name="changeFreqNode">更新頻度ノード</param>
		/// <param name="pageName">ページ名</param>
		/// <returns>出力する更新頻度</returns>
		private string GetChangeFreq(XmlNode changeFreqNode, string pageName)
		{
			if (changeFreqNode == null) return string.Empty;

			// 更新頻度として不正な値の場合は空に補正
			var changeFreqString = changeFreqNode.InnerText;
			if (m_changeFreqItems.Contains(changeFreqString) == false)
			{
				this.AlartMessage.Append(pageName).Append("の更新頻度が不正です。ノードは出力されません。");
				return string.Empty;
			}

			return changeFreqString;
		}

		/// <summary>
		/// 優先度取得
		/// </summary>
		/// <param name="priorityNode">優先度ノード</param>
		/// <param name="pageName">ページ名</param>
		/// <returns>出力する優先度</returns>
		private string GetPriority(XmlNode priorityNode, string pageName)
		{
			if (priorityNode == null) return string.Empty;

			var priorityString = priorityNode.InnerText;
			double priority;
			if (double.TryParse(priorityString, out priority))
			{
				// 値が範囲外の場合は空に補正
				if (((0.1 <= priority) && (priority <= 1.0)) == false)
				{
					priorityString = string.Empty;
				}
			}
			else
			{
				// 値が数値ではない場合空に補正
				priorityString = string.Empty;
			}

			if (string.IsNullOrEmpty(priorityString))
			{
				this.AlartMessage.Append(pageName).Append("の優先度が不正です。ノードは出力されません。");
			}

			return priorityString;
		}

		/// <summary>
		/// 既存のファイル削除し、テンポラリフォルダからファイルを移動
		/// </summary>
		private void CommitSitemapXml()
		{
			// 既存のファイルを取得
			string[] strXmls = Directory.GetFiles(this.PhysicalDirPath, "sitemap*.xml");

			foreach (string strFileName in strXmls)
			{
				// 以下のパターンをマッチするファイルを削除
				// ・sitemap_index.xml
				// ・「sitemap」固定文字列で始まって、最小1桁の数値から最大3桁の数値まで継続して、「.xml」固定文字列で終わる
				if (Regex.Match(strFileName, @"\\sitemap(\d{1,3}|_index)\.xml$", RegexOptions.IgnoreCase).Success)
				{
					// ファイル削除
					File.Delete(strFileName);
				}
			}

			// テンポラリからファイルを移動
			foreach (string strFileName in lstrSitemapXmls)
			{
				File.Copy(this.PhysicalDirPathTemp + strFileName, this.PhysicalDirPath + strFileName, true);
			}
		}

		/// <summary>
		/// URL作成（フロント）
		/// </summary>
		/// <param name="path">ルート以下のパス</param>
		/// <param name="param">パラメーター
		/// item1:リクエストキー item2:値</param>
		/// <returns>URL</returns>
		public static string CreateUrl(string path, Tuple<string, string> param = null)
		{
			// URL作成
			var targetPageUrl = string.Empty;
			if (Constants.PATH_ROOT_FRONT_PC.StartsWith(Uri.UriSchemeHttp) == false)
			{
				targetPageUrl = targetPageUrl + Constants.PROTOCOL_HTTP + Constants.SITE_DOMAIN;
			}
			targetPageUrl = targetPageUrl + Constants.PATH_ROOT_FRONT_PC;

			targetPageUrl = targetPageUrl + path;

			var urlCreator = new UrlCreator(targetPageUrl);
			if (param != null) urlCreator.AddParam(param.Item1, param.Item2);

			return urlCreator.CreateUrl();
		}

		/// <summary>ルート物理ディレクトリ</summary>
		protected string PhysicalDirPath { get; private set; }
		/// <summary>テンポラリ物理ディレクトリ</summary>
		protected string PhysicalDirPathTemp { get; private set; }
		/// <summary>サイトルート</summary>
		protected string SiteRootPath { get; private set; }
		/// <summary>サイトマップ設定ファイル</summary>
		protected string SitemapSettingFile { get; private set; }
		/// <summary>商品一覧：更新頻度</summary>
		protected string ProductListChangeFreq { get; private set; }
		/// <summary>商品一覧：優先度</summary>
		protected string ProductListPriority { get; private set; }
		/// <summary>商品詳細：更新頻度</summary>
		protected string ProductDetailChangeFreq { get; private set; }
		/// <summary>商品一覧：優先度</summary>
		protected string ProductDetailPriority { get; private set; }
		/// <summary>商品一覧URLを生成するか？</summary>
		private bool ShouldGenerateProductListUrl { get; set; }
		/// <summary>商品詳細URLを生成するか？</summary>
		private bool ShouldGenerateProductDetailUrl { get; set; }
		/// <summary>コーディネート一覧：更新頻度</summary>
		protected string CoordinateListChangeFreq { get; private set; }
		/// <summary>コーディネート一覧：優先度</summary>
		protected string CoordinateListPriority { get; private set; }
		/// <summary>コーディネート詳細：更新頻度</summary>
		protected string CoordinateDetailChangeFreq { get; private set; }
		/// <summary>コーディネート詳細：優先度</summary>
		protected string CoordinateDetailPriority { get; private set; }
		/// <summary>コーディネートトップ：更新頻度</summary>
		protected string CoordinateTopChangeFreq { get; private set; }
		/// <summary>コーディネートトップ：優先度</summary>
		protected string CoordinateTopPriority { get; private set; }
		/// <summary>コーディネート一覧URLを生成するか？</summary>
		private bool ShouldGenerateCoordinateListUrl { get; set; }
		/// <summary>コーディネート詳細URLを生成するか？</summary>
		private bool ShouldGenerateCoordinateDetailUrl { get; set; }
		/// <summary>コーディネートトップURLを生成するか？</summary>
		private bool ShouldGenerateCoordinateTopUrl { get; set; }
		/// <summary>記事一覧：更新頻度</summary>
		protected string ArticleListChangeFreq { get; private set; }
		/// <summary>記事一覧：優先度</summary>
		protected string ArticleListPriority { get; private set; }
		/// <summary>記事詳細：更新頻度</summary>
		protected string ArticleDetailChangeFreq { get; private set; }
		/// <summary>記事詳細：優先度</summary>
		protected string ArticleDetailPriority { get; private set; }
		/// <summary>記事トップ：更新頻度</summary>
		protected string ArticleTopChangeFreq { get; private set; }
		/// <summary>記事トップ：優先度</summary>
		protected string ArticleTopPriority { get; private set; }
		/// <summary>記事一覧URLを生成するか？</summary>
		private bool ShouldGenerateArticleListUrl { get; set; }
		/// <summary>記事詳細URLを生成するか？</summary>
		private bool ShouldGenerateArticleDetailUrl { get; set; }
		/// <summary>記事トップURLを生成するか？</summary>
		private bool ShouldGenerateArticleTopUrl { get; set; }
		/// <summary>アラートメッセージ</summary>
		public StringBuilder AlartMessage { get; private set; }
	}
}
