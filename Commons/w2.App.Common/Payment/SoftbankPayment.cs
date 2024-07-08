/*
=========================================================================================================
  Module      : S!まとめて支払い決済モジュール(SoftbankPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Payment
{
	/// <summary>
	/// SoftbankPayment の概要の説明です
	/// </summary>
	public class SoftbankPayment
	{
		// --- 加盟店側設定
		// 加盟店コード
		private string m_strShopId = null;

		// --- 接続設定
		// 確定要求先URL
		private string m_strConnectUrlDecision = null;
		// 接続ユーザID
		private string m_strConnectUserId = null;
		// 接続パスワード
		private string m_strConnectPassword = null;

		// エラーメッセージ
		string m_strErrorMessage = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SoftbankPayment()
		{
			// エラーメッセージ
			m_strErrorMessage = "";

			// --- 加盟店側設定
			// 加盟店コード
			m_strShopId = Constants.PAYMENT_SETTING_SMATOMETE_SHOP_ID;

			// --- 接続設定
			// 確定要求先URL
			m_strConnectUrlDecision = Constants.PAYMENT_SETTING_SMATOMETE_SERVER_URL_DECISION;
			// 接続ユーザID
			m_strConnectUserId = Constants.PAYMENT_SETTING_SMATOMETE_CONNECT_USER_ID;
			// 接続パスワード
			m_strConnectPassword = Constants.PAYMENT_SETTING_SMATOMETE_CONNECT_PASSWORD;

		}

		/// <summary>
		/// 確定要求を送信
		/// </summary>
		/// <param name="objOrder"></param>
		/// <returns></returns>
		public bool SendDecision(object objOrder)
		{
			bool blResult = false;

			// パラメータの生成
			string strXMLParameter = CreateParameter(objOrder);
			// 確定要求を送信
			string strResponse = Send(strXMLParameter, m_strConnectUrlDecision); //

			// XMLデータを解析する
			XmlDocument xmlResponse = new XmlDocument();
			xmlResponse.LoadXml(strResponse);
			XmlNodeList nlOrder = xmlResponse.SelectNodes("/cpRes/order");
			XmlNode nodeResult = xmlResponse.SelectSingleNode("/cpRes/result");

			// ログ出力
			StringBuilder sbRes = new StringBuilder();
			foreach (XmlNode node in nlOrder)
			{
				sbRes.Append("order : ").Append("approvalNo").Append(" - ").Append(StringUtility.ToEmpty(node.SelectSingleNode("approvalNo").InnerText)).Append("\n");
				sbRes.Append("order : ").Append("processResult").Append(" - ").Append(StringUtility.ToEmpty(node.SelectSingleNode("processResult").InnerText)).Append("\n");
			}
			sbRes.Append("nodeResult : ").Append(nodeResult.Name).Append(" - ").Append(StringUtility.ToEmpty(nodeResult.InnerText)).Append("\n");

			// ★★★ エラー内容によって、分かりやすくメッセージを変更した方がいいかも
			switch (nodeResult.InnerText)
			{
				case "0000": // 受付完了
					blResult = true;
					break;
				default:
					blResult = false;
					m_strErrorMessage = "[" + nodeResult.InnerText + "] : エラーが発生しました\n - original response : " + strResponse;
					break;
			}
			return blResult;
		}

		/// <summary>
		/// S!まとめて支払い パラメータ送信処理
		/// </summary>
		/// <param name="strXml">送信するXMLデータ</param>
		/// <param name="strUrl">送信先のURL</param>
		/// <returns>受信結果</returns>
		/// <remarks>
		/// S!まとめて支払い決済サーバへパラメータを送信する
		/// </remarks>
		private string Send(string strXml, string strUrl)
		{
			string strResult = null;

			try
			{
				Encoding enc = Encoding.GetEncoding("Shift_JIS");
				byte[] btParam = Encoding.ASCII.GetBytes(strXml);

				// リクエストの作成
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(strUrl);

				// 基本認証の設定 （ .NET標準の設定では認証が通らなかったため、ヘッダに直接記述）
				string strBasicAuthParam = m_strConnectUserId + ":" + m_strConnectPassword;
				byte[] byteBasicAuthData = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(strBasicAuthParam);
				strBasicAuthParam = System.Convert.ToBase64String(byteBasicAuthData);
				webRequest.Headers.Add("Authorization", "Basic " + strBasicAuthParam);

				webRequest.Method = "POST";
				webRequest.ContentType = "text/xml; charset=\"Shift_JIS\"";
				webRequest.ContentLength = btParam.Length;

				// ポスト・データの書き込み
				using (Stream reqStream = webRequest.GetRequestStream())
				{
					reqStream.Write(btParam, 0, btParam.Length);
				}

				// レスポンスの取得と読み込み
				using (WebResponse res = webRequest.GetResponse())
				using (Stream responseStream = res.GetResponseStream())
				using (StreamReader sr = new StreamReader(responseStream, enc))
				{
					strResult = sr.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				// エラー内容の確認★★
				AppLogger.WriteError("Softbankサーバとの通信中にエラー発生 送信パラメータ:" + strXml, ex);
			}

			return strResult;
		}

		/// <summary>
		/// 電文パラメータ作成
		/// </summary>
		/// <param name="objOrder">注文情報</param>
		/// <returns>パラメタ</returns>
		public string CreateParameter(object objOrder)
		{
			// 送信するXMLの作成
			StringBuilder sbOrderXml = new StringBuilder();
			sbOrderXml.Append("<?xml version=\"1.0\" encoding=\"Shift_JIS\" ?>\r\n");
			sbOrderXml.Append("<cpReq>\r\n");
			sbOrderXml.Append("<accID>").Append(Constants.PAYMENT_SETTING_SMATOMETE_SHOP_ID).Append("</accID>\r\n");
			sbOrderXml.Append("<approvalNo>").Append(StringUtility.ToEmpty(ProductCommon.GetKeyValue(objOrder, Constants.FIELD_ORDER_CARD_INSTRUMENTS))).Append("</approvalNo>\r\n"); // トランザクションIDを指定
			sbOrderXml.Append("</cpReq>\r\n");
			sbOrderXml.Append("\r\n\r\n");

			return sbOrderXml.ToString();
		}

		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return m_strErrorMessage; }
		}
	}
}
