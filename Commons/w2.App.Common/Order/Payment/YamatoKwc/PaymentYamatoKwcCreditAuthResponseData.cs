/*
=========================================================================================================
  Module      : ヤマトKWC クレジット認証APIレスポンスデータ(PaymentYamatoKwcCreditAuthResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジット認証APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcCreditAuthResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcCreditAuthResponseData(string responseString)
			: base(responseString)
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void SetPropertyFromXml(XDocument responseXml)
		{
			// 基底クラスのメソッド呼び出し
			base.SetPropertyFromXml(responseXml);

			// 固有の値をセット
			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case PaymentYamatoKwcConstants.PARAM_CRD_C_RES_CD:
						this.CrdCResCd = element.Value;
						break;

					case PaymentYamatoKwcConstants.PARAM_CREDIT_ERROR_CODE:
						this.CreditErrorCode = element.Value;
						break;

					case PaymentYamatoKwcConstants.PARAM_THREE_D_AUTH_HTML:
						this.ThreeDAuthHtml = element.Value;
						break;

					case PaymentYamatoKwcConstants.PARAM_THREE_D_TOKEN:
						this.ThreeDToken = element.Value;
						break;
				}
			}
		}

		/// <summary>与信承認番号</summary>
		public string CrdCResCd { get; private set; }
		/// <summary>与信結果エラーコード</summary>
		public string CreditErrorCode { get; private set; }
		/// <summary>与信結果エラーメッセージ</summary>
		public string CreditErrorMessage
		{
			get { return m_creditErrorMessage ?? (m_creditErrorMessage = GetCgMessage(this.CreditErrorCode)); }
		}
		private string m_creditErrorMessage = null;
		/// <summary>3DセキュアリダイレクトHTML</summary>
		public string ThreeDAuthHtml { get; private set; }
		/// <summary>3Dトークン</summary>
		public string ThreeDToken { get; private set; }
	}
}
