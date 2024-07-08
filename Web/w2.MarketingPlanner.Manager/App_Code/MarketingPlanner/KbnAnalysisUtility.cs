/*
=========================================================================================================
  Module      : 構成分析ユーティリティモジュール(KbnAnalysisUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Config;
using w2.Common.Sql;
using System.Text;

public class KbnAnalysisUtility
{
	/// <summary>
	/// 構成分析結果取得
	/// </summary>
	/// <param name="strAnalysisName">分析名</param>
	/// <returns>分析結果</returns>
	public static List<KbnAnalysisTable> GetKbnAnalysisData(string strAnalysisName)
	{
		return GetKbnAnalysisData(strAnalysisName, null);
	}
	/// <summary>
	/// 構成分析結果取得
	/// </summary>
	/// <param name="strAnalysisName">分析名</param>
	/// <param name="htInput">ステートメント入力パラメタ</param>
	/// <returns>分析結果</returns>
	public static List<KbnAnalysisTable> GetKbnAnalysisData(string strAnalysisName, Hashtable htInput)
	{
		List<KbnAnalysisTable> lResut = new List<KbnAnalysisTable>();

		XmlDocument xmldoc = new XmlDocument();
		xmldoc.Load(Constants.PHYSICALDIRPATH_KBN_ANALYSIS_XML + strAnalysisName + ".xml");

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			foreach (XmlNode xmlNode in xmldoc.SelectSingleNode(strAnalysisName).ChildNodes)
			{
				if (xmlNode.NodeType == System.Xml.XmlNodeType.Comment)
					continue;

				// グローバルオプションがOFFの場合は該当項目をスキップ
				if (Constants.GLOBAL_OPTION_ENABLE == false)
				{
					if ((xmlNode.Name == "CountryIsCoodeAnalysis") || (xmlNode.Name == "LanguageCodeAnalysis") || (xmlNode.Name == "CurrencyCodeAnalysis")) continue;
				}

				List<KeyValuePair<string, string>> replaceKeyValues = null;

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					switch (xmlNode.Name)
					{
						case "LanguageCodeAnalysis":
							replaceKeyValues = Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Select(l =>
								new KeyValuePair<string, string>(l.Code, string.Format("{0} [{1}]", l.Code, GlobalConfigUtil.LanguageLocaleIdDisplayFormat(l.LocaleId)))).ToList();
							break;

						case "CurrencyCodeAnalysis":
							replaceKeyValues = Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies.Select(c =>
								new KeyValuePair<string, string>(c.Code,
									string.Format("{0} [{1}]", c.Code, string.Join(",", c.CurrencyLocales.Select(cl => GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(cl.LocaleId)).ToArray())))).ToList();
							break;

						case "Addr1Analysis":
							continue;
					}
				}

				//------------------------------------------------------
				// SQL発行・データ取得
				//------------------------------------------------------
				DataView dvResult = null;
				using (SqlStatement sqlStatement = new SqlStatement(Constants.PHYSICALDIRPATH_KBN_ANALYSIS_XML, strAnalysisName, xmlNode.Name))
				{
					//------------------------------------------------------
					// 名称をValueTextから取得して置換
					//------------------------------------------------------
					switch (strAnalysisName)
					{
						case "UserKbnAnalysisHead":
						case "OrderKbnAnalysis":
							// ユーザー区分
							foreach (ListItem li in ValueText.GetValueItemList("w2_User", "user_kbn"))
							{
								sqlStatement.Statement = sqlStatement.Statement.Replace("@@ValueText:user_kbn:" + li.Value + "@@", "'" + li.Text + "'");
							}

							// 注文区分
							var setValue = CreateOrderKbnStatement();
							sqlStatement.Statement = sqlStatement.Statement.Replace("@@ValueText:order_kbn@@", setValue);

							// Amazonペイメントオプション
							sqlStatement.Statement = sqlStatement.Statement.Replace("@@amazon_payment_option@@", Constants.AMAZON_PAYMENT_OPTION_ENABLED ? "1" : "0");
							sqlStatement.Statement = sqlStatement.Statement.Replace("@@amazon_payment_id@@", Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);

							if (xmlNode.Name == "OrderPriceAnalysis")
							{
								var i = 1;
								foreach (var section in Constants.GLOBAL_CONFIGS.GlobalSettings.OrderPriceAnalysis)
								{
									sqlStatement.Statement = sqlStatement.Statement.Replace(string.Format("@@order_price_name_{0}@@", i), section.Name);
									sqlStatement.Statement = sqlStatement.Statement.Replace(string.Format("@@order_price_from_{0}@@", i), section.From);
									sqlStatement.Statement = sqlStatement.Statement.Replace(string.Format("@@order_price_to_{0}@@", i), section.To);
									i++;
								}
							}
							break;

						case "UserKbnAnalysis":
							// 性別
							foreach (ListItem li in ValueText.GetValueItemList("w2_User", "sex"))
							{
								sqlStatement.Statement = sqlStatement.Statement.Replace("@@ValueText:sex:" + li.Value + "@@", "'" + li.Text + "'");
							}
							break;

						case "PointAnalysis":
							// ポイントルール区分
							foreach (ListItem li in ValueText.GetValueItemList("w2_UserPointHistory", "point_rule_kbn"))
							{
								sqlStatement.Statement = sqlStatement.Statement.Replace("@@ValueText:point_rule_kbn:" + li.Value + "@@", "'" + li.Text + "'");
							}
							// ポイント加算区分
							foreach (ListItem li in ValueText.GetValueItemList("w2_UserPointHistory", "point_inc_kbn"))
							{
								sqlStatement.Statement = sqlStatement.Statement.Replace("@@ValueText:point_inc_kbn:" + li.Value + "@@", "'" + li.Text + "'");
							}
							break;

						case "FixedPurchaseKbnAnalysis":
							// ユーザー区分
							foreach (ListItem li in ValueText.GetValueItemList("w2_User", "user_kbn"))
							{
								sqlStatement.Statement = sqlStatement.Statement.Replace("@@ValueText:user_kbn:" + li.Value + "@@", "'" + li.Text + "'");
							}
							// 購入金額
							var j = 1;
							foreach (var section in Constants.GLOBAL_CONFIGS.GlobalSettings.OrderPriceAnalysis)
							{
								sqlStatement.Statement = sqlStatement.Statement.Replace(string.Format("@@fixed_purchase_price_name_{0}@@", j), section.Name);
								sqlStatement.Statement = sqlStatement.Statement.Replace(string.Format("@@fixed_purchase_price_from_{0}@@", j), section.From);
								sqlStatement.Statement = sqlStatement.Statement.Replace(string.Format("@@fixed_purchase_price_to_{0}@@", j), section.To);
								j++;
							}
							break;
					}

					//------------------------------------------------------
					// データ取得
					//------------------------------------------------------
					if (htInput != null)
					{
						dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					}
					else
					{
						dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
					}
				}

				//------------------------------------------------------
				// 表示用データ作成＆追加
				//------------------------------------------------------
				if (dvResult.Count != 0)
				{
					lResut.Add(CreateKbnAnalysisTable(xmlNode.SelectSingleNode("Title").InnerText, xmlNode.SelectSingleNode("Unit").InnerText, double.Parse(dvResult[0]["total"].ToString()), dvResult, replaceKeyValues));
				}
			}
		}

		// モバイルデータの表示と非表示OFF時
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
		{
			switch (strAnalysisName)
			{
				case "FixedPurchaseKbnAnalysis":
					// モバイルデータがない場合は削除不要
					if (lResut[2].Rows.Count <= 5) break;
					// 顧客区分分析のモバイル会員とモバイルゲストを非表示にする
					lResut[2].Rows.RemoveAt(5);
					lResut[2].Rows.RemoveAt(4);
					break;

				case "OrderKbnAnalysis":
					// モバイルデータがない場合は削除不要
					if ((lResut[4].Rows.Count <= 5) || (lResut[5].Rows.Count <= 2)) break;
					// 顧客区分分析のモバイル会員とモバイルゲストを非表示にする
					lResut[4].Rows.RemoveAt(5);
					lResut[4].Rows.RemoveAt(4);
					// 注文区分分析のモバイルを非表示にする
					lResut[5].Rows.RemoveAt(2);
					break;

				case "UserKbnAnalysisHead":
					// モバイルデータがない場合は削除不要
					if (lResut[1].Rows.Count <= 5) break;
					// モバイルデータの表示と非表示OFF時はモバイル会員とモバイルゲストを非表示にする
					lResut[1].Rows.RemoveAt(5);
					lResut[1].Rows.RemoveAt(4);
					break;
			}
		}
		
		return lResut;
	}

	/// <summary>
	/// 区分分析結果テーブル取得
	/// </summary>
	/// <param name="strTitle">タイトル</param>
	/// <param name="strUnit">単位</param>
	/// <param name="dbTotal">合計</param>
	/// <param name="dvAnalysisData">区分分析結果</param>
	/// <param name="replaceKeyValues">表示時内容、置換処理の指定</param>
	/// <returns>区分分析結果テーブル</returns>
	public static KbnAnalysisTable CreateKbnAnalysisTable(
		string strTitle,
		string strUnit,
		double dbTotal,
		DataView dvAnalysisData,
		List<KeyValuePair<string, string>> replaceKeyValues = null)
	{
		KbnAnalysisTable katResult = new KbnAnalysisTable(strTitle, strUnit, dbTotal);
		
		// ValueTextからクリックポイント発行名取得
		var clickPointName = ValueText.GetValueText(
			Constants.TABLE_POINTRULE,
			Constants.FIELD_POINTRULE_POINT_INC_KBN,
			Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK);

		var rewardPointName = ValueText.GetValueText(
			Constants.TABLE_POINTRULE,
			Constants.FIELD_POINTRULE_POINT_INC_KBN,
			Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT);

		// 通常データ作成
		double dbCulcTotals = 0;
		foreach (DataRowView drv in dvAnalysisData)
		{
			var columnName = ((string)drv["name"]);

			// クリックポイント発行が無効またはレビュー投稿ポイントが無効の場合はレポート表示しない
			if (((columnName == clickPointName) && (Constants.POINTRULE_OPTION_CLICKPOINT_ENABLED == false))
				|| ((columnName == rewardPointName) && (Constants.REVIEW_REWARD_POINT_ENABLED == false))) continue;

			// 置換用の指定がある場合、その内容で置換処理を実施する(動的に設定する場合に利用)
			if ((replaceKeyValues != null) && (replaceKeyValues.Count > 0))
			{
				KeyValuePair<string, string> keyValue = replaceKeyValues.Find(item => (item.Key == columnName));
				columnName = StringUtility.ToEmpty(keyValue.Value);
			}
			if (columnName.Length == 0) columnName = "空値"; // 空の場合にのみ「空値」と入れる

			KbnAnalysisRow row = new KbnAnalysisRow(columnName, double.Parse(drv["count"].ToString()));
			katResult.Rows.Add(row);

			dbCulcTotals += row.Count;
		}

		// 「上記以外」データ作成
		if (katResult.Total != dbCulcTotals)
		{
			katResult.Rows.Add(new KbnAnalysisRow("上記以外", katResult.Total - dbCulcTotals));
		}

		return katResult;
	}

	/// <summary>
	/// レート画像サイズ取得
	/// </summary>
	/// <param name="objCurrent">比較データ</param>
	/// <param name="objTarget">比較対象データ</param>
	/// <returns>レート（0～100の整数値を返す）</returns>
	public static int GetRateImgWidth(object objCurrent, object objTarget)
	{
		int iResult = 0;

		try
		{
			iResult = (int)(double.Parse(GetRateString(objCurrent, objTarget, 1)));
		}
		catch
		{
		}

		return iResult;
	}

	/// <summary>
	/// レート取得
	/// </summary>
	/// <param name="objCurrent">比較データ</param>
	/// <param name="objTarget">比較対象データ</param>
	/// <param name="iDecimalPoints">小数点以下の桁数</param>
	/// <returns>レート（小数点1桁のパーセンテージ数値)、∞の場合は「-」を返す。（</returns>
	public static string GetRateString(object objCurrent, object objTarget, int iDecimalPoints)
	{
		string strResult = null;

		double dIncRate = 0;
		double dUnderDecimalPoint = Math.Pow(10, iDecimalPoints);	// 小数点以下の桁数

		try
		{
			double dCurrent = double.Parse(objCurrent.ToString());
			double dTarget = double.Parse(objTarget.ToString());

			if (dTarget == 0)
			{
				if (dCurrent == 0)
				{
					strResult = (0 * dUnderDecimalPoint).ToString();
				}
				else
				{
					strResult = "-";
				}
			}
			else
			{
				dIncRate = ((dCurrent * 100) / dTarget);
				strResult = (((int)((dIncRate * dUnderDecimalPoint) + 0.5)) / dUnderDecimalPoint) != 0 ? (((int)((dIncRate * dUnderDecimalPoint) + 0.5)) / dUnderDecimalPoint).ToString("f" + iDecimalPoints.ToString()) : "0";
			}
		}
		catch
		{
		}

		return strResult;
	}

	/// <summary>
	/// ValueTextから絞り込みを行う注文区分取得
	/// </summary>
	/// <returns>ステートメント</returns>
	public static string CreateOrderKbnStatement()
	{
		var sbStatement = new StringBuilder();
		var index = 1;
		var valueText = ValueText.GetValueItemList("w2_Order", "order_kbn");
		foreach (ListItem li in valueText)
		{
			var baseStatement = "(\r\nSELECT  @@Index@@ AS no,\r\n@@ValueText:order_kbn:text@@ AS name,\r\nCOUNT(w2_Order.order_id) AS count\r\nFROM  w2_Order\r\nWHERE  w2_Order.order_kbn = @@ValueText:order_kbn:value@@\r\nAND  w2_Order.del_flg = '0'\r\nAND  w2_Order.return_exchange_kbn = '00' -- 元注文のみ(返品交換分は含めない)\r\n)";
			var parameter = baseStatement.Replace("@@Index@@", index.ToString())
				.Replace("@@ValueText:order_kbn:text@@", "'" + li.Text + "'")
				.Replace("@@ValueText:order_kbn:value@@", "'" + li.Value + "'");
			sbStatement.AppendLine(parameter);
			index++;
			if (valueText.Count >= index) sbStatement.AppendLine("UNION ALL");
		}
		return sbStatement.ToString();
	}

	/// <summary>
	/// 区分分析テーブルクラス
	/// </summary>
	public class KbnAnalysisTable
	{
		private string m_strTitle = null;
		private string m_strUnit = null;
		private double m_dbTotal = 0;

		private List<KbnAnalysisRow> m_lKbnAnalysisRows = new List<KbnAnalysisRow>();

		public KbnAnalysisTable(string strTitle, string strUnit, double dbTotal)
		{
			m_strTitle = strTitle;
			m_strUnit = strUnit;
			m_dbTotal = dbTotal;
		}

		public string Title
		{
			get { return m_strTitle; }
			set { m_strTitle = value; }
		}
		public string Unit
		{
			get { return m_strUnit; }
			set { m_strUnit = value; }
		}
		public double Total
		{
			get { return m_dbTotal; }
			set { m_dbTotal = value; }
		}
		public List<KbnAnalysisRow> Rows
		{
			get { return m_lKbnAnalysisRows; }
		}
	}

	/// <summary>
	/// 区分分析行クラス
	/// </summary>
	public class KbnAnalysisRow
	{
		private string m_strName;
		private double m_dbCount;

		public KbnAnalysisRow(string strName, double dbCount)
		{
			m_strName = strName;
			m_dbCount = dbCount;
		}

		public string Name
		{
			get { return m_strName; }
		}
		public double Count
		{
			get { return m_dbCount; }
		}
	}
}
