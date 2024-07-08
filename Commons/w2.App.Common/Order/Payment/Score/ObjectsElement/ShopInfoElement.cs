/*
=========================================================================================================
  Module      : 認証情報要素(ShopInfoElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 認証情報要素
	/// </summary>
	public class ShopInfoElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShopInfoElement()
		{
			this.ShopCode = Constants.PAYMENT_SCORE_AFTER_PAY_SHOP_CODE;
			this.ShopPassword = Constants.PAYMENT_SCORE_AFTER_PAY_SHOP_PASSWORD;
			this.TerminalId = Constants.PAYMENT_SCORE_AFTER_PAY_TERMINAL_ID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopCode">加盟店コード</param>
		/// <param name="shopPassword">ダイレクトパスワード</param>
		/// <param name="terminalId">接続元ID</param>
		public ShopInfoElement(string shopPassword, string shopCode, string terminalId)
		{
			this.ShopCode = shopCode;
			this.ShopPassword = shopPassword;
			this.TerminalId = terminalId;
		}

		/// <summary>加盟店コード</summary>
		[XmlElement("shopCode")]
		public string ShopCode { get; set; }
		/// <summary>ダイレクトパスワード</summary>
		[XmlElement("shopPassword")]
		public string ShopPassword { get; set; }
		/// <summary>接続元ID</summary>
		[XmlElement("terminalId")]
		public string TerminalId { get; set; }
	}
}
