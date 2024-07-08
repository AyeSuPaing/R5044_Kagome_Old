/*
=========================================================================================================
  Module      : ヤマトKWC クレジット情報削除APIレスポンスデータ(PaymentYamatoKwcCreditInfoDeleteResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジット情報削除APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcCreditInfoDeleteResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcCreditInfoDeleteResponseData(string responseString)
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
					case "cardUnit":
						this.CardUnit = int.Parse(element.Value);
						break;

					case "cardData":
						this.CardDatas.Add(new CardData(element));
						break;
				}
			}
		}

		/// <summary>補完件数</summary>
		public int CardUnit { get; set; }

		/// <summary>補完件数</summary>
		public List<CardData> CardDatas
		{
			get { return m_cardDatas; }
		}
		private readonly List<CardData> m_cardDatas = new List<CardData>();

		/// <summary>
		/// カードデータ
		/// </summary>
		public class CardData
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="cardDataElement"></param>
			public CardData(XElement cardDataElement)
			{
				// 固有の値をセット
				foreach (var element in cardDataElement.Elements())
				{
					switch (element.Name.ToString())
					{
						case "cardKey":
							this.CardKey = int.Parse(element.Value);
							break;

						case "maskingCardNo":
							this.MaskingCardNo = element.Value;
							break;

						case "cardExp":
							this.CardExp = element.Value;
							break;

						case "cardOwner":
							this.CardOwner = element.Value;
							break;

						case "cardCodeApi":
							this.CardCodeApi = element.Value;
							break;

						case "lastCreditDate":
							this.LastCreditDate = DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
							break;

						case "subscriptionFlg":
							this.SubscriptionFlg = element.Value;
							break;

						case "regularFlg":
							this.RegularFlg = element.Value;
							break;
					}
				}
			}

			/// <summary>カード識別キー</summary>
			public int CardKey { get; set; }
			/// <summary>カード番号（マスク）</summary>
			public string MaskingCardNo { get; set; }
			/// <summary>カード有効期限(MMYY)</summary>
			public string CardExp { get; set; }
			/// <summary>カード名義人</summary>
			public string CardOwner { get; set; }
			/// <summary>カード会社コード(API用)</summary>
			public string CardCodeApi { get; set; }
			/// <summary>最終利用日時</summary>
			public DateTime LastCreditDate { get; set; }
			/// <summary>予約利用有無</summary>
			public string SubscriptionFlg { get; set; }
			/// <summary>継続課金利用有無</summary>
			public string RegularFlg { get; set; }
		}
	}
}
