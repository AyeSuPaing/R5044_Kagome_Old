/*
=========================================================================================================
  Module      : 電算システムコンビニ決済モジュール(PaymentDskCvs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using w2.App.Common.Util;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// 電算システムコンビニ決済モジュール
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class PaymentDskCvs : IPayment
	{
		// フロントHashtable格納用パラメータ定数
		public const string FIELD_CVS_TYPE = "dsk_cvs_type";			// コンビニタイプ

		// 送信パラメタ
		public const string PARAM_TENANTID = "tenantid";				// テナントID
		public const string PARAM_PASSWORD = "password";				// パスワード
		public const string PARAM_ORDERID = "orderid";					// お客様注文番号
		public const string PARAM_CONVENI = "conveni";					// 支払先
		public const string PARAM_CUSTOMERNAME1 = "customername1";		// 顧客名(1)
		public const string PARAM_CUSTOMERNAME2 = "customername2";		// 顧客名(2)
		public const string PARAM_CUSTOMERTEL = "customertel";			// 顧客電話番号
		public const string PARAM_AMOUNT = "amount";					// 請求金額
		public const string PARAM_EXPIRE = "expire";					// 支払期限
		public const string PARAM_EXPIRE_DATE = "expire_date";			// 支払期限(日付)

		// 支払先区分（コンビニ仕様の統廃合などあるのでコードで管理する）
		/// <summary>支払先区分"010"</summary>
		public const string CONVENI_010 = "010";
		/// <summary>支払先区分"030"</summary>
		public const string CONVENI_030 = "030";
		/// <summary>支払先区分"040"</summary>
		public const string CONVENI_040 = "040";
		/// <summary>支払先区分"060"</summary>
		public const string CONVENI_060 = "060";
		/// <summary>支払先区分"080"</summary>
		public const string CONVENI_080 = "080";

		// 決済結果パラメタ
		public const string RESULT_RESULT = "result";					// 処理結果
		public const string RESULT_RECEIPTNO = "receiptno";				// 支払先の受付番号
		public const string RESULT_URL = "url";							// 払込票等URL
		public const string RESULT_MESSAGE = "message";					// メッセージ

		// 決済結果区分
		public const string KBN_RESULT_SETTING_ERROR = "-1";			// 設定値の異常
		public const string KBN_RESULT_PAYMENTSERVER_ERROR = "-2";		// 支払先のシステム内でエラー発生
		public const string KBN_RESULT_SURROGATESERVER_ERROR = "-99";	// 代行サーバ内で予期せぬエラーが発生

		// メッセージXMLパス
		const string PATH_XML_DSK_MESSAGES = @"Xml\Message\DskCvsMessages.xml";

		string[] strResultKeys = { RESULT_RESULT, RESULT_RECEIPTNO, RESULT_URL, RESULT_MESSAGE };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strConveniType">コンビニタイプ</param>
		public PaymentDskCvs(string strConveniType)
		{
			this.ConveniType = strConveniType;
		}

		/// <summary>
		/// 決済
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <param name="blIsPc">PC・モバイル決済フラグ</param>
		/// <returns>決済結果</returns>
		public bool Auth(Hashtable htOrder, CartObject coCart, bool blIsPc)
		{
			//------------------------------------------------------
			// C・モバイル決済処理フラグ
			//------------------------------------------------------
			this.IsPC = blIsPc;

			//------------------------------------------------------
			// レスポンス取得
			//------------------------------------------------------
			// 送信パラメタ取得
			Dictionary<string, string> dicParam = GetParameter(htOrder, coCart);

			// 電文パラメタ作成
			string strParameters = CreateParameter(dicParam);

			// クライアントURLにパラメタも付加する（Streamを使用したHTTPSリクエスト送信は何故か挫折したため。）
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Constants.PAYMENT_SETTING_DSK_SERVER_URL + "?" + strParameters);

			// レスポンス取得
			string strResponse = null;
			using (WebResponse res = webRequest.GetResponse())
			using (Stream responseStream = res.GetResponseStream())
			using (StreamReader srReader = new StreamReader(responseStream, Encoding.GetEncoding("Shift_JIS")))
			{
				strResponse = srReader.ReadToEnd();
			}

			//------------------------------------------------------
			// レスポンス解析
			//------------------------------------------------------
			// HTTPリクエストが正常に終了した場合
			// ネットワークエラーの場合はパラメータが返らない
			if (strResponse.Length == 0)
			{
				return false;
			}

			// 処理結果パラメータ取得
			this.ResultParam = GetResultParam(strResponse);

			// 決済に失敗した場合はエラーとする
			if ((this.ResultParam[RESULT_RESULT] == KBN_RESULT_SETTING_ERROR)
				|| (this.ResultParam[RESULT_RESULT] == KBN_RESULT_PAYMENTSERVER_ERROR)
				|| (this.ResultParam[RESULT_RESULT] == KBN_RESULT_SURROGATESERVER_ERROR))
			{
				// ログ格納処理
				var errorMessage = "";
				if (this.ResultParam[RESULT_RESULT] == KBN_RESULT_SETTING_ERROR)
				{
					errorMessage = "設定値の異常";
				}
				else if (this.ResultParam[RESULT_RESULT] == KBN_RESULT_PAYMENTSERVER_ERROR)
				{
					errorMessage = "支払先のシステム内でエラー発生";
				}
				else if (this.ResultParam[RESULT_RESULT] == KBN_RESULT_SURROGATESERVER_ERROR)
				{
					errorMessage = "代行サーバ内で予期せぬエラーが発生";
				}

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					coCart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Dsk,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					errorMessage,
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)htOrder[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)htOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
					});

				return false;
			}

			//------------------------------------------------------
			// メッセージ取得
			//------------------------------------------------------
			var xdoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + PATH_XML_DSK_MESSAGES);
			var targetCvsMessage = xdoc.Root.Element(string.Format("CONVENI_{0}", this.ConveniType));
			var sbMessageHtml = new StringBuilder(targetCvsMessage.Element("Html").Value);
			var sbMessageText = new StringBuilder(targetCvsMessage.Element("Text").Value);

			// 支払先毎のメッセージ取得
			switch (this.ConveniType)
			{
				case CONVENI_010:
					break;

				case CONVENI_030:
					// 支払先の受付番号を企業コード-注文番号に分割する
					this.ResultParam.Add("code", this.ResultParam[RESULT_RECEIPTNO].Substring(0, 5));
					this.ResultParam.Add("no", this.ResultParam[RESULT_RECEIPTNO].Substring(6, 12));
					break;

				case CONVENI_040:
					// 確認番号を画面に表示した際に非常に見にくいので支払先の受付番号と確認番号に分割する
					this.ResultParam.Add("code", this.ResultParam[RESULT_RECEIPTNO].Substring(0, 8));
					this.ResultParam.Add("no", this.ResultParam[RESULT_RECEIPTNO].Substring(8, 9));
					break;

				case CONVENI_060:
					break;

				case CONVENI_080:
					// 支払先の受付番号を4桁-7桁に分割する
					this.ResultParam[RESULT_RECEIPTNO] = this.ResultParam[RESULT_RECEIPTNO].Substring(0, 4) + "-" + this.ResultParam[RESULT_RECEIPTNO].Substring(4, 7);

					// モバイル決済の場合はURLをモバイル用URLに置換する
					if (this.IsPC == false)
					{
						this.ResultParam[RESULT_URL] = this.ResultParam[RESULT_URL].Replace("https://link.kessai.info/JLP/JLPpca", "https://w2.kessai.info/JLM/JLMpca");		// 本番用
						this.ResultParam[RESULT_URL] = this.ResultParam[RESULT_URL].Replace("https://link.kessai.info/JLPCT/JLPpca", "https://w2.kessai.info/JLMBT/JLMpca");	// テスト用
					}
					break;
			}

			//------------------------------------------------------
			// メッセージのパラメタ置換
			//------------------------------------------------------
			// 送信パラメタ
			foreach (string strKey in dicParam.Keys)
			{
				// 請求金額の場合
				if (strKey == PARAM_AMOUNT)
				{
					sbMessageHtml.Replace("@@ " + strKey + " @@", StringUtility.ToNumeric(dicParam[strKey]));
					sbMessageText.Replace("@@ " + strKey + " @@", StringUtility.ToNumeric(dicParam[strKey]));
				}
				else
				{
					sbMessageHtml.Replace("@@ " + strKey + " @@", dicParam[strKey]);
					sbMessageText.Replace("@@ " + strKey + " @@", dicParam[strKey]);
				}
			}
			// 処理結果パラメタ
			foreach (string strKey in this.ResultParam.Keys)
			{
				sbMessageHtml.Replace("@@ " + strKey + " @@", this.ResultParam[strKey]);
				sbMessageText.Replace("@@ " + strKey + " @@", this.ResultParam[strKey]);
			}

			this.ResultMessageHtml = sbMessageHtml.ToString();
			this.ResultMessageText = sbMessageText.ToString();

			// 成功時ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				coCart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Dsk,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				this.ResultMessageText,
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, (string)htOrder[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)htOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
				});
			return true;
		}

		/// <summary>
		/// 送信パラメタ取得
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <returns>送信パラメタ</returns>
		public Dictionary<string, string> GetParameter(Hashtable htOrder, CartObject coCart)
		{
			Dictionary<string, string> dicSendParam = new Dictionary<string, string>();

			//------------------------------------------------------
			// 送信パラメタ設定
			//------------------------------------------------------
			dicSendParam.Add(PARAM_TENANTID, Constants.PAYMENT_SETTING_DSK_TENANT_ID);			// テナントID
			dicSendParam.Add(PARAM_PASSWORD, Constants.PAYMENT_SETTING_DSK_PASSWORD);			// パスワード
			dicSendParam.Add(PARAM_ORDERID, (string)htOrder[Constants.FIELD_ORDER_ORDER_ID]);	// お客様注文番号
			dicSendParam.Add(PARAM_CONVENI, this.ConveniType);	// 支払先
			// データ型がvarchar(20)のため、姓名を分けて登録
			// ※PKGでは姓が10文字、名が10文字まで登録可/
			dicSendParam.Add(PARAM_CUSTOMERNAME1, coCart.Owner.Name1.ToWithinLv2Chars());			// 顧客名(1)外字排除
			dicSendParam.Add(PARAM_CUSTOMERNAME2, coCart.Owner.Name2.ToWithinLv2Chars());			// 顧客名(2)外字排除

			dicSendParam.Add(PARAM_CUSTOMERTEL, coCart.Owner.Tel1_1 + "-" + coCart.Owner.Tel1_2 + "-" + coCart.Owner.Tel1_3);	// 顧客電話番号
			dicSendParam.Add(PARAM_AMOUNT, coCart.PriceTotal.ToPriceString());														// 請求金額
			// 支払期限
			DateTime dtExpire = DateTime.Now.AddDays(Constants.PAYMENT_SETTING_DSK_CVS_PAYMENT_LIMIT);
			dicSendParam.Add(PARAM_EXPIRE, dtExpire.ToString("yyyyMMdd"));			// 支払期限
			dicSendParam.Add(PARAM_EXPIRE_DATE,
				DateTimeUtility.ToString(
					dtExpire,
					DateTimeUtility.FormatType.ShortDate2Letter,
					coCart.Owner.DispLanguageLocaleId));	// 支払期限(日付)

			return dicSendParam;
		}

		/// <summary>
		/// 電文パラメタ作成
		/// </summary>
		/// <param name="dicParam">送信パラメタ</param>
		/// <returns>電文パラメタ作成(文字列)</returns>
		public string CreateParameter(Dictionary<string, string> dicParam)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(PARAM_TENANTID).Append("=").Append(dicParam[PARAM_TENANTID]);						// テナントID
			sb.Append("&").Append(PARAM_PASSWORD).Append("=").Append(dicParam[PARAM_PASSWORD]);			// パスワード
			sb.Append("&").Append(PARAM_ORDERID).Append("=").Append(dicParam[PARAM_ORDERID]);			// お客様注文番号
			sb.Append("&").Append(PARAM_CONVENI).Append("=").Append(dicParam[PARAM_CONVENI]);					// 支払先
			sb.Append("&").Append(PARAM_CUSTOMERNAME1).Append("=").Append(HttpUtility.UrlEncode(dicParam[PARAM_CUSTOMERNAME1], Encoding.GetEncoding("Shift_JIS")));		// 顧客名(1)
			sb.Append("&").Append(PARAM_CUSTOMERNAME2).Append("=").Append(HttpUtility.UrlEncode(dicParam[PARAM_CUSTOMERNAME2], Encoding.GetEncoding("Shift_JIS")));		// 顧客名(2)
			sb.Append("&").Append(PARAM_CUSTOMERTEL).Append("=").Append(dicParam[PARAM_CUSTOMERTEL]);	// 顧客電話番号
			sb.Append("&").Append(PARAM_AMOUNT).Append("=").Append(dicParam[PARAM_AMOUNT].ToPriceString());				// 請求金額
			sb.Append("&").Append(PARAM_EXPIRE).Append("=").Append(dicParam[PARAM_EXPIRE]);				// 支払期限

			return sb.ToString();
		}

		/// <summary>
		/// 処理結果パラメタ文字列を分割
		/// </summary>
		/// <param name="strResultParam">処理結果パラメタ文字列</param>
		/// <returns>処理結果パラメタ</returns>
		private Dictionary<string, string> GetResultParam(string strResultParam)
		{
			// 各パラメタ値を取得
			// パラメタは以下ので取得
			// result=xxxxx&receiptno=yyyyy&url=zzzzz&message=aaaaa
			// ※[url]の値に'=','&'が含まれているため、Splitで分割しない
			Dictionary<string, string> dicSendParam = new Dictionary<string, string>();
			dicSendParam.Add(RESULT_RESULT, strResultParam.Substring(strResultParam.IndexOf(RESULT_RESULT) + (RESULT_RESULT.Length + 1), strResultParam.IndexOf("&" + RESULT_RECEIPTNO) - (RESULT_RESULT.Length + 1)));
			dicSendParam.Add(RESULT_RECEIPTNO, strResultParam.Substring(strResultParam.IndexOf(RESULT_RECEIPTNO) + (RESULT_RECEIPTNO.Length + 1), strResultParam.IndexOf("&" + RESULT_URL) - (strResultParam.IndexOf(RESULT_RECEIPTNO) + (RESULT_RECEIPTNO.Length + 1))));
			dicSendParam.Add(RESULT_URL, strResultParam.Substring(strResultParam.IndexOf(RESULT_URL) + (RESULT_URL.Length + 1), strResultParam.IndexOf("&" + RESULT_MESSAGE) - (strResultParam.IndexOf(RESULT_URL) + (RESULT_URL.Length + 1))));
			dicSendParam.Add(RESULT_MESSAGE, strResultParam.Substring(strResultParam.IndexOf(RESULT_MESSAGE) + (RESULT_MESSAGE.Length + 1)));

			return dicSendParam;
		}

		/// <summary>結果パラメタ</summary>
		public Dictionary<string, string> ResultParam { get; private set; }
		/// <summary>結果メッセージHTML</summary>
		public string ResultMessageHtml { get; private set; }
		/// <summary>結果メッセージTEXT</summary>
		public string ResultMessageText { get; private set; }
		/// <summary>支払先</summary>
		public string ConveniType { get; private set; }
		/// <summary>PC・モバイル決済処理フラグ</summary>
		public bool IsPC { get; private set; }
	}
}
