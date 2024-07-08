/*
=========================================================================================================
  Module      : ソフトバンクペイメント WEBコンビニ「入金通知処理」受取APIクラス(PaymentSBPSCvsPaymentApiRcv.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント WEBコンビニ「入金通知処理」受取APIクラス
	/// </summary>
	public class PaymentSBPSCvsPaymentApiRcv : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCvsPaymentApiRcv()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCvsPaymentApiRcv(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCvsPaymentApiRcvResponseData(settings))
		{
		}

		/// <summary>
		/// 決済要求
		/// </summary>
		/// <param name="responseXmlString">リクエストXML文字列</param>
		/// <returns>実行結果</returns>
		public void Receive(string responseXmlString)
		{
			// XML読み込み
			this.ResponseData.LoadXml(XDocument.Parse(responseXmlString));
		}

		/// <summary>
		/// レスポンスXML作成
		/// </summary>
		/// <param name="result">結果</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>レスポンスXML</returns>
		public string CreateResponseXml(
			bool result,
			string errorMessage)
		{
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-response", new XAttribute("id", "NT01-00103-701"),
					new XElement("res_result", result ? "OK" : "NG"),
					new XElement("res_err_msg", Convert.ToBase64String(m_encodingPost.GetBytes(errorMessage)))
			));
			return document.ToString();
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCvsPaymentApiRcvResponseData ResponseData { get { return (PaymentSBPSCvsPaymentApiRcvResponseData)this.ResponseDataInner; } }
		/// <summary>受信時例外</summary>
		public Exception ReceiveException { get; private set; }
	}
}
