/*
=========================================================================================================
  Module      : ヤマト決済(後払い) SMS認証番号判レスポンスデータクラス(PaymentYamatoKaSmsAuthResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using w2.App.Common.Order.Payment;
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) SMS認証番号判レスポンスデータクラス
	/// </summary>
	public class PaymentYamatoKaSmsAuthResponseData : PaymentYamatoKaBaseResponseData
	{
		/// <summary>
		/// 判定結果（値）
		/// </summary>
		public enum ResultValue
		{
			/// <summary>判定OK</summary>
			[EnumTextName("判定OK")]
			Ok = 1,
			/// <summary>コード不一致</summary>
			[EnumTextName("コード不一致")]
			Mismatch = 2,
			/// <summary>有効期限切れ</summary>
			[EnumTextName("有効期限切れ")]
			Expired = 3,
			/// <summary>認証SMS送信NG</summary>
			[EnumTextName("認証SMS送信NG")]
			Ng = 4,
			/// <summary>認証結果の不正</summary>
			[EnumTextName("認証結果の不正")]
			Invalid = 5,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKaSmsAuthResponseData(string responseString)
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
				}
			}
		}

		/// <summary>判定結果</summary>
		public string Result { get; private set; }
		/// <summary>判定結果説明</summary>
		public string ResultDescription
		{
			get
			{
				ResultValue value;
				var isConvertionSuccess = Enum.TryParse(this.Result, true, out value);

				// ログを残す
				PaymentFileLogger.WritePaymentLog(
					null,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentType.Yamatokwc,
					PaymentFileLogger.PaymentProcessingType.Examination,
					isConvertionSuccess
						? string.Format("審査結果のコード「{0}」がResultValueのタイプに定義されてない値です。", this.Result)
						: string.Format("審査結果のコード「{0}」がResultValueのタイプに変換できませんでした。", this.Result));

				return this.Result;
			}
		}
	}
}
