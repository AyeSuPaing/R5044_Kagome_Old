/*
=========================================================================================================
  Module      : ランキングユーティリティモジュール(RankingUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Xml;

public class RankingUtility
{
	// 	定義XMLノード（アクセスランキング）
	public const string XMLNODENAME_ROOT_ACSPAGERANKING = "AccessRanking";
	public const string XMLNODENAME_ACS_ACSPAGERANKING = "AccessPageRanking";
	public const string XMLNODENAME_ACS_REFDOMAINRANKING = "ReferrerDomainRanking";
	public const string XMLNODENAME_ACS_SRCHENGINERANKING = "SearchEngineRanking";
	public const string XMLNODENAME_ACS_SRCHWORDSRANKING = "SearchWordsRanking";

	// 定義XMLノード（モバイルランキング）
	public const string XMLNODENAME_ROOT_MOBPAGERANKING = "MobileRanking";
	public const string XMLNODENAME_MOB_MOBILECAREERRANKING = "MobileCareerRanking";
	public const string XMLNODENAME_MOB_MOBILENAMERANKING = "MobileNameRanking";

	// 定義XMLノード（商品ランキング）
	public const string XMLNODENAME_ROOT_PCTRANKING = "ProductRanking";
	public const string XMLNODENAME_PCT_PCTSRCHWORDRANKING = "ProductSearchWordRanking";
	public const string XMLNODENAME_PCT_PCTBUYCOUNTRANKING = "ProductBuyCountRanking";
	public const string XMLNODENAME_PCT_PCTBUYPRICERANKING = "ProductBuyPriceRanking";

	// 取得する最大値
	public const int MAX_RANK = 100;

	// 抽出条件定数
	private const string FIELD_ORDERCONDITION_AMOUNT_FIELD_NAME = "@@ amount_field @@";		// 金額フィールド

	/// <summary>
	/// アクセスページランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetAccessPageRankingReport(string strDeptId, int iBgn, int iEnd, string strAccessKbn)
	{
		// 全体ランキング
		return GetAccessPageRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetAccessPageRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd, string strAccessKbn)
	{
		// 年ランキング
		return GetAccessPageRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetAccessPageRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd, string strAccessKbn)
	{
		// 月ランキング
		return GetAccessPageRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetAccessPageRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn)
	{
		return GetAccessRankingReport(XMLNODENAME_ACS_ACSPAGERANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccessKbn);
	}

	/// <summary>
	/// リファラードメインランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetReferrerDomainRankingReport(string strDeptId, int iBgn, int iEnd, string strAccessKbn)
	{
		// 全体ランキング
		return GetReferrerDomainRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetReferrerDomainRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd, string strAccessKbn)
	{
		// 年ランキング
		return GetReferrerDomainRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetReferrerDomainRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd, string strAccessKbn)
	{
		// 月ランキング
		return GetReferrerDomainRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetReferrerDomainRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn)
	{
		return GetAccessRankingReport(XMLNODENAME_ACS_REFDOMAINRANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccessKbn);
	}

	/// <summary>
	/// 検索エンジンランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetSearchEngineRankingReport(string strDeptId, int iBgn, int iEnd, string strAccessKbn)
	{
		// 全体ランキング
		return GetSearchEngineRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetSearchEngineRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd, string strAccessKbn)
	{
		// 年ランキング
		return GetSearchEngineRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetSearchEngineRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd, string strAccessKbn)
	{
		// 月ランキング
		return GetSearchEngineRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetSearchEngineRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn)
	{
		return GetAccessRankingReport(XMLNODENAME_ACS_SRCHENGINERANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccessKbn);
	}

	/// <summary>
	/// 検索ワードランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetSearchWordRankingReport(string strDeptId, int iBgn, int iEnd, string strAccessKbn)
	{
		// 全体ランキング
		return GetSearchWordRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetSearchWordRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd, string strAccessKbn)
	{
		// 年ランキング
		return GetSearchWordRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetSearchWordRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd, string strAccessKbn)
	{
		// 月ランキング
		return GetSearchWordRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd, strAccessKbn);
	}
	public static RankingTable GetSearchWordRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn)
	{
		return GetAccessRankingReport(XMLNODENAME_ACS_SRCHWORDSRANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccessKbn);
	}

	/// <summary>
	/// モバイルキャリアランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetMobileCareerRankingReport(string strDeptId, int iBgn, int iEnd)
	{
		// 全体ランキング
		return GetMobileCareerRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd);
	}
	public static RankingTable GetMobileCareerRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd)
	{
		// 年ランキング
		return GetMobileCareerRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd);
	}
	public static RankingTable GetMobileCareerRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd)
	{
		// 月ランキング
		return GetMobileCareerRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd);
	}
	public static RankingTable GetMobileCareerRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd)
	{
		return GetMobileRankingReport(XMLNODENAME_MOB_MOBILECAREERRANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd);
	}

	/// <summary>
	/// モバイル機種名キャリアランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetMobileNameRankingReport(string strDeptId, int iBgn, int iEnd)
	{
		// 全体ランキング
		return GetMobileNameRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd);
	}
	public static RankingTable GetMobileNameRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd)
	{
		// 年ランキング
		return GetMobileNameRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd);
	}
	public static RankingTable GetMobileNameRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd)
	{
		// 月ランキング
		return GetMobileNameRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd);
	}
	public static RankingTable GetMobileNameRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd)
	{
		return GetMobileRankingReport(XMLNODENAME_MOB_MOBILENAMERANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd);
	}

	/// <summary>
	/// 商品検索ワードランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetProductSearchWordRankingReport(string strDeptId, int iBgn, int iEnd, string strAccesskbn)
	{
		// 全体ランキング
		return GetProductSearchWordRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd, strAccesskbn);
	}
	public static RankingTable GetProductSearchWordRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd, string strAccesskbn)
	{
		// 年ランキング
		return GetProductSearchWordRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd, strAccesskbn);
	}
	public static RankingTable GetProductSearchWordRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd, string strAccesskbn)
	{
		// 月ランキング
		return GetProductSearchWordRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd, strAccesskbn);
	}
	public static RankingTable GetProductSearchWordRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn)
	{
		return GetProductRankingReport(XMLNODENAME_PCT_PCTSRCHWORDRANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccessKbn, "");
	}

	/// <summary>
	/// 商品販売個数ランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetProductBuyCountRankingReport(string strDeptId, int iBgn, int iEnd, string strAccesskbn)
	{
		// 全体ランキング
		return GetProductBuyCountRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd, strAccesskbn);
	}
	public static RankingTable GetProductBuyCountRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd, string strAccesskbn)
	{
		// 年ランキング
		return GetProductBuyCountRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd, strAccesskbn);
	}
	public static RankingTable GetProductBuyCountRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd, string strAccesskbn)
	{
		// 月ランキング
		return GetProductBuyCountRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd, strAccesskbn);
	}
	public static RankingTable GetProductBuyCountRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccesskbn)
	{
		return GetProductRankingReport(XMLNODENAME_PCT_PCTBUYCOUNTRANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccesskbn, "");
	}

	/// <summary>
	/// 商品販売金額ランキングレポート取得
	/// </summary>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <param name="amountFieldName">税込フラグ（税込："0",税抜："1"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetProductBuyPriceRankingReport(string strDeptId, int iBgn, int iEnd, string strAccesskbn, string amountFieldName)
	{
		// 全体ランキング
		return GetProductBuyPriceRankingReport(strDeptId, 0, 0, 0, iBgn, iEnd, strAccesskbn, amountFieldName);
	}
	public static RankingTable GetProductBuyPriceRankingReport(string strDeptId, int iTgtYear, int iBgn, int iEnd, string strAccesskbn, string amountFieldName)
	{
		// 年ランキング
		return GetProductBuyPriceRankingReport(strDeptId, iTgtYear, 0, 0, iBgn, iEnd, strAccesskbn, amountFieldName);
	}
	public static RankingTable GetProductBuyPriceRankingReport(string strDeptId, int iTgtYear, int iMonth, int iBgn, int iEnd, string strAccesskbn, string amountFieldName)
	{
		// 月ランキング
		return GetProductBuyPriceRankingReport(strDeptId, iTgtYear, iMonth, 0, iBgn, iEnd, strAccesskbn, amountFieldName);
	}
	public static RankingTable GetProductBuyPriceRankingReport(string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccesskbn, string amountFieldName)
	{
		return GetProductRankingReport(XMLNODENAME_PCT_PCTBUYPRICERANKING, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccesskbn, amountFieldName);
	}

	/// <summary>
	/// アクセスランキングレポート取得
	/// </summary>
	/// <param name="strXmlNodePath">定義ノードパス</param>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetAccessRankingReport(string strXmlNodePath, string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn)
	{
		return GetRankingReport(XMLNODENAME_ROOT_ACSPAGERANKING, strXmlNodePath, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccessKbn, null);
	}
	/// <summary>
	/// モバイルランキングレポート取得
	/// </summary>
	/// <param name="strXmlNodePath">定義ノードパス</param>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetMobileRankingReport(string strXmlNodePath, string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd)
	{
		return GetRankingReport(XMLNODENAME_ROOT_MOBPAGERANKING, strXmlNodePath, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, "", null);
	}
	/// <summary>
	/// 商品ランキングレポート取得
	/// </summary>
	/// <param name="strXmlNodePath">定義ノードパス</param>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2"）</param>
	/// <param name="amountFieldName">金額フィールド</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetProductRankingReport(string strXmlNodePath, string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn, string amountFieldName)
	{
		return GetRankingReport(XMLNODENAME_ROOT_PCTRANKING, strXmlNodePath, strDeptId, iTgtYear, iMonth, iTgtDay, iBgn, iEnd, strAccessKbn, amountFieldName);
	}

	/// <summary>
	/// ランキングレポート取得
	/// </summary>
	/// <param name="strXmlRootNode">定義XML</param>
	/// <param name="strXmlNode">定義ノード名</param>
	/// <param name="strDeptId">識別ID</param>
	/// <param name="iTgtYear">対象年(未指定：0)</param>
	/// <param name="iMonth">対象月(未指定：0)</param>
	/// <param name="iTgtDay">対象日(未指定：0)</param>
	/// <param name="iBgn">表示開始記事番号</param>
	/// <param name="iEnd">表示終了記事番号</param>
	/// <param name="strAccessKbn">アクセス区分（全体："",PC："1",モバイル："2",スマートフォン："3"）</param>
	/// <param name="amountFieldName">金額フィールド</param>
	/// <returns>ランキングテーブルデータ</returns>
	public static RankingTable GetRankingReport(string strXmlRootNode, string strXmlNode, string strDeptId, int iTgtYear, int iMonth, int iTgtDay, int iBgn, int iEnd, string strAccessKbn, string amountFieldName)
	{
		string strPhysicalXmlDirPath = AppDomain.CurrentDomain.BaseDirectory + Constants.PATH_MANAGER_RANKING;

		XmlDocument xmldoc = new XmlDocument();
		xmldoc.Load(strPhysicalXmlDirPath + strXmlRootNode + ".xml");
		XmlNode xmlNode = xmldoc.SelectSingleNode(strXmlRootNode).SelectSingleNode(strXmlNode);

		//------------------------------------------------------
		// SQL発行
		//------------------------------------------------------
		DataView dvResult = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement(strPhysicalXmlDirPath, strXmlRootNode, strXmlNode))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, strDeptId);
			htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR, iTgtYear.ToString().PadLeft(4, '0'));
			htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH, iMonth.ToString().PadLeft(2, '0'));
			htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_DAY, iTgtDay.ToString().PadLeft(2, '0'));
			htInput.Add("bgn_row_num", iBgn);
			htInput.Add("end_row_num", iEnd);
			htInput.Add("access_kbn", strAccessKbn);

			// 金額項目名置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_AMOUNT_FIELD_NAME, amountFieldName);
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		// トータル
		int iTotalRows = 0;
		if (dvResult.Count != 0)
		{
			iTotalRows = int.Parse(dvResult[0]["row_count"].ToString());
		}

		RankingTable rtRestlt = new RankingTable(
			xmlNode.SelectSingleNode("Title").InnerText,
			xmlNode.SelectSingleNode("Unit").InnerText,
			xmlNode.SelectSingleNode("Value").InnerText,
			(xmlNode.SelectSingleNode("Extra") != null) ? xmlNode.SelectSingleNode("Extra").InnerText : "",
			iTotalRows);

		//------------------------------------------------------
		// 表示用データ作成
		//------------------------------------------------------
		foreach (DataRowView drv in dvResult)
		{
			// アクセスページランキングであればURLデコード（フレンドリURL対応）
			string strValueName = (string)drv[Constants.FIELD_DISPSUMMARYANALYSIS_VALUE_NAME];
			if (strXmlRootNode == XMLNODENAME_ROOT_ACSPAGERANKING)
			{
				strValueName = HttpUtility.UrlDecode(strValueName);
			}

			RankingRow rr = new RankingRow(
				strValueName,
				(int)drv["rank"],
				(drv.DataView.Table.Columns.IndexOf("extra") != -1) ? drv["extra"] : null,
				(drv.DataView.Table.Columns.IndexOf(Constants.FIELD_DISPSUMMARYANALYSIS_COUNTS) != -1) ? (int)drv[Constants.FIELD_DISPSUMMARYANALYSIS_COUNTS] : 0,
				(drv.DataView.Table.Columns.IndexOf("total_counts") != -1) ? (int)drv["total_counts"] : 0,
				(drv.DataView.Table.Columns.IndexOf("price") != -1) ? (decimal)drv["price"] : 0m,
				(drv.DataView.Table.Columns.IndexOf("total_price") != -1) ? (decimal)drv["total_price"] : 0m);

			rtRestlt.Rows.Add(rr);
		}

		return rtRestlt;
	}


	///*********************************************************************************************
	/// <summary>
	/// ランキングテーブルクラス
	/// </summary>
	///*********************************************************************************************
	public class RankingTable
	{
		private string m_strName = null;
		private string m_strUnit = null;
		private string m_strValueName = null;
		private string m_strExtraName = null;
		private int m_iTotalRows = 0;

		private List<RankingRow> m_lRows = new List<RankingRow>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strUnit">単位</param>
		/// <param name="strValueName">値名称</param>
		/// <param name="strExtraName">付加データ名称</param>
		/// <param name="lTotal">合計</param>
		public RankingTable(string strName, string strUnit, string strValueName, string strExtraName, int iTotalRows)
		{
			m_strName = strName;
			m_strUnit = strUnit;
			m_strValueName = strValueName;
			m_strExtraName = strExtraName;
			m_iTotalRows = iTotalRows;
		}

		public string Name
		{
			get { return m_strName; }
		}

		public string Unit
		{
			get { return m_strUnit; }
		}

		public string ValueName
		{
			get { return m_strValueName; }
		}

		public string ExtraName
		{
			get { return m_strExtraName; }
		}

		public int TotalRows
		{
			get { return m_iTotalRows; }
		}

		public List<RankingRow> Rows
		{
			get { return m_lRows; }
		}
	}

	///*********************************************************************************************
	/// <summary>
	/// ランキング行クラス
	/// </summary>
	///*********************************************************************************************
	public class RankingRow
	{
		private string m_strRowName = null;
		private int m_iCount = 0;
		private int m_iTotalCount = 0;
		private int m_iRank = 0;
		private object m_objExtra = null;
		private decimal m_price = 0m;
		private decimal m_totalPrice = 0m;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strRowName">行名</param>
		/// <param name="iRank">ランク</param>
		/// <param name="objExtra">付加データ</param>
		/// <param name="iCount">カウント数</param>
		/// <param name="iTotalCount">トータルカウント数</param>
		/// <param name="price">単価</param>
		/// <param name="totalPrice">合計金額</param>
		public RankingRow(string strRowName, int iRank, object objExtra, int iCount = 0, int iTotalCount = 0, decimal price = 0m, decimal totalPrice = 0m)
		{
			m_strRowName = strRowName;
			m_iCount = iCount;
			m_iTotalCount = iTotalCount;
			m_iRank = iRank;
			m_objExtra = objExtra;
			m_price = price;
			m_totalPrice = totalPrice;
		}

		public string RowName
		{
			get { return m_strRowName; }
		}

		public int Count
		{
			get { return m_iCount; }
		}

		public int TotalCount
		{
			get { return m_iTotalCount; }
		}

		public int Rank
		{
			get { return m_iRank; }
		}

		public object Extra
		{
			get { return m_objExtra; }
		}

		public decimal Price
		{
			get { return m_price; }
		}

		public decimal TotalPrice
		{
			get { return m_totalPrice; }
		}
	}
}

