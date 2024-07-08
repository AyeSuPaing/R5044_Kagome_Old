/*
=========================================================================================================
  Module      : ヤマトKWC 出荷情報登録APIレスポンスデータ(PaymentYamatoKwcShipmentEntryResponseData.cs)
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
	/// ヤマトKWC 出荷情報登録APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcShipmentEntryResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcShipmentEntryResponseData(string responseString)
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
					case "slipUrlPc":
						this.SlipUrlPc = element.Value;
						break;

					case "slipUrlMobile":
						this.SlipUrlMobile = element.Value;
						break;
				}
			}
		}

		/// <summary>荷物問い合わせURL（パソコン／スマートフォン）</summary>
		public string SlipUrlPc { get; private set; }
		/// <summary>荷物問い合わせURL（携帯電話）</summary>
		public string SlipUrlMobile { get; private set; }
	}
}
