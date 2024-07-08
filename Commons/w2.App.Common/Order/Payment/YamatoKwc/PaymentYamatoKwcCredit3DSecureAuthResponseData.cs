/*
=========================================================================================================
  Module      : ヤマトKWC クレジット3Dセキュア認証APIレスポンスデータ(PaymentYamatoKwcCredit3DSecureAuthResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	public class PaymentYamatoKwcCredit3DSecureAuthResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcCredit3DSecureAuthResponseData(string responseString)
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
					case PaymentYamatoKwcConstants.PARAM_RETURN_CODE:
						this.ReturnCode = element.Value;
						break;
					case PaymentYamatoKwcConstants.PARAM_ERROR_CODE:
						this.ErrorCode = element.Value;
						break;
					case PaymentYamatoKwcConstants.PARAM_RETURN_DATE:
						this.ReturnDate = element.Value;
						break;
					case PaymentYamatoKwcConstants.PARAM_CRD_C_RES_CD:
						this.CrdCResCd = element.Value;
						break;
				}
			}
		}

		/// <summary>与信承認番号</summary>
		public string CrdCResCd { get; private set; }
		/// <summary>与信結果エラーコード</summary>
		public string CreditErrorCode { get; private set; }
		/// <summary>送信日時</summary>
		public new string ReturnDate { get; private set; }
	}
}
