/*
=========================================================================================================
  Module      : ページャモジュール(WebPager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;

///*********************************************************************************************
/// <summary>
/// WEBページャクラス
/// </summary>
///*********************************************************************************************
public class WebPager : w2.Common.Web.PagerUtility
{
	/// <summary>ページャXMLファイルパス</summary>
	private readonly static string m_pagerXMlFilePath = AppDomain.CurrentDomain.BaseDirectory + "Contents/Pager.xml";

	/// <summary>
	/// デフォルト一覧ページャ作成
	/// </summary>
	/// <param name="total">総件数</param>
	/// <param name="currentPage">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <returns>デフォルトページャ</returns>
	public static string CreateDefaultListPager(int total, int currentPage, string pageUrl)
	{
		return CreatePagerSwitchSetting(total, currentPage, pageUrl, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST, 2);
	}

	/// <summary>
	/// 商品一覧ページャ作成
	/// </summary>
	/// <param name="total">総件数</param>
	/// <param name="currentPage">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <param name="dispCount">１ページ表示件数</param>
	/// <returns>ページャ</returns>
	public static string CreateProductListPager(int total, int currentPage, string pageUrl, int dispCount)
	{
		return CreatePagerSwitchSetting(total, currentPage, pageUrl, dispCount, 2);
	}

	/// <summary>
	/// デフォルトとスマートフォン用のページャ設定を切り替えてページャ作成
	/// </summary>
	/// <param name="total">総件数</param>
	/// <param name="currentPageNo">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <param name="dispContents">１ページ表示件数</param>
	/// <param name="dispLinkPages">リンク表示数</param>
	/// <returns>ページャ</returns>
	private static string CreatePagerSwitchSetting(int total, int currentPageNo, string pageUrl, int dispContents, int dispLinkPages)
	{
		// スマートフォンサイトの場合は、スマートフォンサイト用のページャを出力する
		if (Constants.SMARTPHONE_OPTION_ENABLED && SmartPhoneUtility.CheckSmartPhoneSite(HttpContext.Current.Request.Path))
		{
			return CreatePagerWrapper("SmartPhonePagerSetting", total, currentPageNo, pageUrl, dispContents, 2);
		}
		else
		{
			return CreatePagerWrapper("DefaultListPagerSetting", total, currentPageNo, pageUrl, dispContents, 2);
		}
	}

	/// <summary>
	/// Awoo商品一覧画面ページャ作成
	/// </summary>
	/// <param name="total">総件数</param>
	/// <param name="currentPageNo">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <param name="dispContents">１ページ表示件数</param>
	/// <returns>ページャ</returns>
	public static string CreateRecommendProductsListPager(int total, int currentPageNo, string pageUrl, int dispContents)
	{
		// スマートフォンサイトの場合は、スマートフォンサイト用のページャを出力する
		if (Constants.SMARTPHONE_OPTION_ENABLED && SmartPhoneUtility.CheckSmartPhoneSite(HttpContext.Current.Request.Path))
		{
			return CreatePagerWrapper("RecommendProductsListSpPagerSetting", total, currentPageNo, pageUrl, dispContents, 2);
		}
		else
		{
			return CreatePagerWrapper("RecommendProductsListPagerSetting", total, currentPageNo, pageUrl, dispContents, 2);
		}
	}

	/// <summary>
	/// 商品バリエーション一覧ページャ作成
	/// </summary>
	/// <param name="total">総件数</param>
	/// <param name="currentPage">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <returns>商品バリエーション一覧ページャ</returns>
	public static string CreateProductVariationListPager(int total, int currentPage, string pageUrl)
	{
		return CreateProductListPager(total, currentPage, pageUrl, Constants.CONST_DISP_CONTENTS_PRODUCTVARIATION_LIST);
	}

	/// <summary>
	/// 商品レビューページャ作成
	/// </summary>
	/// <param name="total">総件数</param>
	/// <param name="currentPage">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <returns>商品レビューページャ</returns>
	public static string CreateProductReviewPager(int total, int currentPage, string pageUrl)
	{
		return CreatePagerWrapper("ProductReviewPagerSetting", total, currentPage, pageUrl, Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST, 2);
	}

	/// <summary>
	/// 特集ページページャ作成
	/// </summary>
	/// <param name="total">総件数</param>
	/// <param name="currentPage">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <param name="dispNum">Disp num</param>
	/// <returns>特集ページページャ</returns>
	public static string CreateFeaturePagePager(int total, int currentPage, string pageUrl, int dispNum)
	{
		return CreatePagerWrapper("FeaturePagePagerSetting", total, currentPage, pageUrl, dispNum, 2);
	}

	/// <summary>
	/// ページャ作成ラッパークラス
	/// </summary>
	/// <param name="pagerSetting">ページャ定義設定ノード名</param>
	/// <param name="total">総件数</param>
	/// <param name="currentPageNo">現在のページ番号</param>
	/// <param name="pageUrl">ページURL</param>
	/// <param name="dispContents">１ページ表示件数</param>
	/// <param name="dispLinkPages">リンク表示数</param>
	/// <returns>ページャHTML</returns>
	private static string CreatePagerWrapper(string pagerSetting, int total, int currentPageNo, string pageUrl, int dispContents, int dispLinkPages)
	{
		// ページャ定義XMLの設定ミスでシステムエラーとなるのを防ぐため、例外発生時はログを出力して空文字を返却する
		try
		{
			return CreatePager(GetPagerSetting(m_pagerXMlFilePath, pagerSetting), total, currentPageNo, pageUrl, dispContents, dispLinkPages);
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);

			return "";
		}
	}
}
