/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 取引状況照会レスポンスデータクラス(PaymentYamatoKaReferenceResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 取引状況照会レスポンスデータクラス
	/// </summary>
	public class PaymentYamatoKaReferenceResponseData : PaymentYamatoKaBaseResponseData
	{
		/// <summary>取引状況（値）</summary>
		public enum ResultValue
		{
			/// <summary>承認済み</summary>
			Confirmed = 1,
			/// <summary>取消済み</summary>
			Cancelled = 2,
			/// <summary>送り状番号登録済み</summary>
			InvoiceNumberRegistered = 3,
			/// <summary>配送要調査</summary>
			DeliveryRequiredSurvey = 5,
			/// <summary>売上確定</summary>
			SalesFixed = 10,
			/// <summary>請求書発行済み</summary>
			InvoiceIssued = 11,
			/// <summary>入金済み</summary>
			Paymented = 12,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal PaymentYamatoKaReferenceResponseData(string responseString)
			: base(responseString)
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void LoadXml(XDocument responseXml)
		{
			// 基底クラスのメソッド呼び出し
			base.LoadXml(responseXml);

			// 固有の値をセット (後払い) 決済依頼
			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "result":
						this.Result = element.Value;
						break;

					case "warning":
						this.Warning = element.Value;
						break;
				}
			}
		}

		/// <summary>取引状況</summary>
		public string Result { get; private set; }
		/// <summary>売上確定</summary>
		public bool IsSalesFixed { get { return this.Result == ((int)ResultValue.SalesFixed).ToString(); } }
		/// <summary>入金済み</summary>
		public bool IsPaymented { get { return this.Result == ((int)ResultValue.Paymented).ToString(); } }
		/// <summary>警報情報区分</summary>
		public string Warning { get; private set; }
	}
}
