/*
=========================================================================================================
  Module      : ソソフトバンクペイメント マルチ決済 クレジット向け「フリー項目」文字列作成クラス(PaymentSBPSFreeCSVCredit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチ決済 クレジット向け「フリー項目」文字列作成クラス
	/// </summary>
	public class PaymentSBPSFreeCSVCredit : IPaymentSBPSFreeCSV
	{
		/// <summary>デフォルトエンコーディング</summary>
		readonly static Encoding DEFAULT_ENCODING = Encoding.GetEncoding("Shift_JIS");

		/// <summary>トークン</summary>
		const string FIELD_TOKEN = "TOKEN";
		/// <summary>トークンキー</summary>
		const string FIELD_TOKEN_KEY = "TOKEN_KEY";
		/// <summary>取引区分</summary>
		const string FIELD_DEALINGS_TYPE = "DEALINGS_TYPE";
		/// <summary>分割回数</summary>
		const string FIELD_DIVIDE_TIMES = "DIVIDE_TIMES";
		/// <summary>ボーナス併用回数</summary>
		const string FIELD_BONUS_DETAILS_TIME = "BONUS_DETAILS_TIME";
		/// <summary>都度購入顧客登録フラグ</summary>
		const string FIELD_ONE_TIME_CHARGE_CUSTOMER_REGIST = "ONE_TIME_CHARGE_CUSTOMER_REGIST";

		/// <summary>SBPS 3DES</summary>
		private PaymentSBPSTripleDESCrypto m_tripleDES = null;
		/// <summary>トークン</summary>
		private string m_token { get; set; }
		/// <summary>トークンキー</summary>
		private string m_tokenKey { get; set; }
		/// <summary>分割情報</summary>
		private PaymentSBPSCreditDivideInfo m_divideInfo { get; set; }
		/// <summary>クレジット保存フラグ</summary>
		private bool m_saveCardInfo { get; set; }

		/// <summary>
		/// コンストラクタ（顧客IDで決済する場合）
		/// </summary>
		/// <param name="divideInfo">分割情報</param>
		public PaymentSBPSFreeCSVCredit(
			PaymentSBPSCreditDivideInfo divideInfo)
			: this("", "", divideInfo, false)
		{
		}

		/// <summary>
		/// コンストラクタ（カード番号で決済する場合）
		/// </summary>
		/// <param name="token">トークン</param>
		/// <param name="tokenKey">トークンキー</param>
		/// <param name="divideInfo">分割情報</param>
		/// <param name="saveCardInfo">クレジット保存フラグ</param>
		public PaymentSBPSFreeCSVCredit(
			string token,
			string tokenKey,
			PaymentSBPSCreditDivideInfo divideInfo,
			bool saveCardInfo)
		{
			var setting = PaymentSBPSSetting.GetDefaultSetting();
			m_tripleDES = new PaymentSBPSTripleDESCrypto(setting.TripleDESKeyAndIV.Value.Key, setting.TripleDESKeyAndIV.Value.Value);
			m_token = token;
			m_tokenKey = tokenKey;
			m_divideInfo = divideInfo;
			m_saveCardInfo = saveCardInfo;
		}

		/// <summary>
		/// 「フリー項目」文字列取得
		/// </summary>
		/// <returns>「フリー項目」文字列</returns>
		public string GetFreeCsvString()
		{
			string csvString = GetCsvString();

			return m_tripleDES.GetEncryptedData(csvString);
		}

		/// <summary>
		/// CSV文字列取得
		/// </summary>
		/// <returns>CSV文字列</returns>
		private string GetCsvString()
		{
			NameValueCollection nameValues = new NameValueCollection();
			nameValues.Add(FIELD_DEALINGS_TYPE, m_divideInfo.GetDealingsTypeString());
			if ((m_divideInfo.DivideType == PaymentSBPSCreditDivideInfo.DivideTypes.Divide)
				&& m_divideInfo.DivideTimes.HasValue)
			{
				nameValues.Add(FIELD_DIVIDE_TIMES, m_divideInfo.DivideTimes.Value.ToString());
			}
			//nameValues.Add(FIELD_BONUS_DETAILS_TIME, "");
			nameValues.Add(FIELD_ONE_TIME_CHARGE_CUSTOMER_REGIST, "on");

			if (string.IsNullOrEmpty(m_tokenKey) == false)
			{
				nameValues.Add(FIELD_TOKEN_KEY, m_tokenKey);    // ここだけtokenとtokenKeyなぜか逆だが仕様書はこの順番
			}
			if (string.IsNullOrEmpty(m_token) == false)
			{
				nameValues.Add(FIELD_TOKEN, m_token);
			}

			return GetCsvStringFromNameValueCollection(nameValues);
		}

		/// <summary>
		/// NameValueCollectionからcsv文字列作成
		/// </summary>
		/// <param name="nameValues">NameValueCollection</param>
		/// <returns>CSV文字列</returns>
		private string GetCsvStringFromNameValueCollection(NameValueCollection nameValues)
		{
			StringBuilder csv = new StringBuilder();
			foreach (string name in nameValues.Keys)
			{
				if (csv.Length != 0) csv.Append(",");
				csv.Append(name).Append("=").Append(nameValues[name]);
			}
			return csv.ToString();
		}
	}
}
