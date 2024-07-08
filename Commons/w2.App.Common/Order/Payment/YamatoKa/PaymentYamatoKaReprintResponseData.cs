/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 請求書再発行レスポンスデータクラス(PaymentYamatoKaReprintResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 請求書再発行レスポンスデータクラス
	/// </summary>
	public class PaymentYamatoKaReprintResponseData : PaymentYamatoKaBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal PaymentYamatoKaReprintResponseData(string responseString)
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

			// 固有の値をセット (後払い) 請求書再発行
			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "reissueCode":
						this.ReissueCode = element.Value;
						break;
				}
			}
		}

		/// <summary>請求内容変更・請求書再発行 自動判定結果コード</summary>
		public string ReissueCode { get; private set; }
	}
}
