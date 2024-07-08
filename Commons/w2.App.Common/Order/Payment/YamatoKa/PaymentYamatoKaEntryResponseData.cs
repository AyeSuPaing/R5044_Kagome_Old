/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 決済依頼レスポンスデータクラス(PaymentYamatoKaEntryResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using w2.App.Common.Order.Payment;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 決済依頼レスポンスデータクラス
	/// </summary>
	public class PaymentYamatoKaEntryResponseData : PaymentYamatoKaBaseResponseData
	{
		/// <summary>審査結果（値）</summary>
		public enum ResultValue
		{
			/// <summary>ご利用可</summary>
			[EnumTextName("ご利用可")]
			Available = 0,
			/// <summary>ご利用不可</summary>
			[EnumTextName("ご利用不可")]
			Unavailable = 1,
			/// <summary>限度額超過</summary>
			[EnumTextName("限度額超過")]
			Overlimit = 2,
			/// <summary>審査中</summary>
			[EnumTextName("審査中")]
			UnderExamination = 3,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal PaymentYamatoKaEntryResponseData(string responseString)
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

		/// <summary>審査結果</summary>
		public string Result { get; private set; }
		/// <summary> 審査結果説明 </summary>
		public string ResultDescription
		{
			get
			{
				ResultValue value;
				var isConvertionSuccess = Enum.TryParse(this.Result, true, out value);
				if (isConvertionSuccess && Enum.IsDefined(typeof(ResultValue), value)) return value.ToText();

				// ログを残す
				PaymentFileLogger.WritePaymentLog(
					null,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentType.Yamatoka,
					PaymentFileLogger.PaymentProcessingType.Examination,
					isConvertionSuccess
						? string.Format("審査結果のコード「{0}」がResultValueのタイプに定義されてない値です。", this.Result)
						: string.Format("審査結果のコード「{0}」がResultValueのタイプに変換できませんでした。", this.Result));

				return this.Result;
			}
		}
	}
}
