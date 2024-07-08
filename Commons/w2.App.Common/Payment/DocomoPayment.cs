/*
=========================================================================================================
  Module      : ドコモケータイ払い決済モジュール(DocomoPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Payment
{
	/// <summary>
	/// PaymentZeus の概要の説明です
	/// </summary>
	public class DocomoPayment
	{
		private const string PARAM_DOCOMO_PAYMENT_SHOP_CODE = "sSpcd";			// 加盟店コード
		private const string PARAM_DOCOMO_PAYMENT_SHOP_PASSWORD = "sSppw";	// 加盟店パスワード
		private const string PARAM_DOCOMO_PAYMENT_SHOP_ORDER_NUMBER = "sSpnm";	// 加盟店注文番号
		private const string PARAM_DOCOMO_PAYMENT_PRICE = "sPric";		// 決済金額
		private const string PARAM_DOCOMO_PAYMENT_DATETIME = "sDate";		// 決済要求日時/ドコモ決済日時 (yyyy-MM-dd HH:mm:ss)
		private const string PARAM_DOCOMO_PAYMENT_RECEIPT_NO = "sSlcd";		// ケータイ払い決済番号
		private const string PARAM_DOCOMO_PAYMENT_STATUS = "sStat";		// 結果コード

		// 加盟店コード
		string m_strShopCode = null;
		// 加盟店パスワード
		string m_strShopPassword = null;
		// 確定要求先URL
		string m_strConnectUrlDecision = null;

		// エラーメッセージ
		string m_strErrorMessage = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DocomoPayment()
		{
			// エラーメッセージ
			m_strErrorMessage = "";

			// --- 加盟店側設定
			// 加盟店コード
			m_strShopCode = Constants.PAYMENT_SETTING_DOCOMOKETAI_SHOP_CODE;
			// 加盟店パスワード
			m_strShopPassword = Constants.PAYMENT_SETTING_DOCOMOKETAI_SHOP_PASSWORD;

			// --- 接続先設定
			// 確定要求先URL
			m_strConnectUrlDecision = Constants.PAYMENT_SETTING_DOCOMOKETAI_SERVER_URL_DECISION;
		}

		/// <summary>
		/// 確定要求を送信
		/// </summary>
		/// <param name="objOrder"></param>
		/// <returns></returns>
		public bool SendDecision(object objOrder)
		{
			var blResult = false;

			// パラメータの生成
			var strParameter = CreateParameter(objOrder);
			// 確定要求を送信
			var strResponse = Send(strParameter, m_strConnectUrlDecision); //

			var htResponse = ParameterAnalysis(strResponse);
			switch ((string)htResponse[PARAM_DOCOMO_PAYMENT_STATUS])
			{
				case "40": // 受付完了
					blResult = true;
					break;
				default:
					blResult = false;
					m_strErrorMessage = (string)htResponse[PARAM_DOCOMO_PAYMENT_STATUS] + " : エラーが発生しました\n - original response : " + strResponse;
					break;
			}
			return blResult;
		}

		/// <summary>
		/// パラメータ送信処理
		/// </summary>
		/// <param name="strParameters">送信するパラメータ</param>
		/// <param name="strUrl">送信先のURL</param>
		/// <returns>受信結果</returns>
		/// <remarks>
		/// Docomoケータイ払い決済サーバへパラメータを送信する
		/// </remarks>
		private string Send(string strParameters, string strUrl)
		{
			string strResult = null;

			try
			{
				Encoding enc = Encoding.GetEncoding("Shift_JIS");
				byte[] btParam = Encoding.ASCII.GetBytes(strParameters);

				// リクエストの作成
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(strUrl);
				webRequest.Method = "POST";
				webRequest.ContentType = "application/x-www-form-urlencoded";
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
				AppLogger.WriteError("Docomoサーバとの通信中にエラー発生 送信パラメータ:" + strParameters, ex);
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
			StringBuilder sb = new StringBuilder();
			sb.Append(PARAM_DOCOMO_PAYMENT_SHOP_CODE + "=" + m_strShopCode);			// 加盟店コード
			sb.Append("&");
			sb.Append(PARAM_DOCOMO_PAYMENT_SHOP_PASSWORD + "=" + m_strShopPassword);	// 加盟店パスワード
			sb.Append("&");
			sb.Append(PARAM_DOCOMO_PAYMENT_SHOP_ORDER_NUMBER + "=" + StringUtility.ToEmpty(ProductCommon.GetKeyValue(objOrder, Constants.FIELD_ORDER_ORDER_ID)));	// 加盟店注文番号
			sb.Append("&");
			sb.Append(PARAM_DOCOMO_PAYMENT_PRICE + "=" + StringUtility.ToEmpty(ProductCommon.GetKeyValue(objOrder, Constants.FIELD_ORDER_LAST_BILLED_AMOUNT)));	// 決済金額
			sb.Append("&");
			sb.Append(PARAM_DOCOMO_PAYMENT_DATETIME).Append("=").Append(StringUtility.ToDateString(ProductCommon.GetKeyValue(objOrder, Constants.FIELD_ORDER_DATE_CREATED), "yyyy-MM-dd HH:mm:ss")); // 決済要求時に使用した日時

			return sb.ToString();
		}

		/// <summary>
		/// 受信したパラメータをHashtableへ
		/// </summary>
		/// <remarks>sPnm=xx&sSlcd=xx&sDate=xx&sState=xx の形式をHashtableへ変換する</remarks>
		/// <param name="strParameter"></param>
		/// <returns></returns>
		private Hashtable ParameterAnalysis(string strParameter)
		{
			Hashtable htResult = new Hashtable();
			if (StringUtility.ToEmpty(strParameter).Length != 0)
			{
				string[] arStrParams = strParameter.Split('&');
				foreach (string strParam in arStrParams)
				{
					string[] strKeyVal = strParam.Split('=');
					if (strKeyVal.Length > 0)
					{
						htResult.Add(strKeyVal[0], strKeyVal[1]);
					}
				}
			}
			// 結果を返す
			return htResult;
		}

		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return m_strErrorMessage; }
		}
	}
}
