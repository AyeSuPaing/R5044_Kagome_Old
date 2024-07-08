/*
=========================================================================================================
  Module      : DSK後払いリクエストデータ用の基底クラス(BaseDskDeferredRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.DSKDeferred
{
	/// <summary>
	/// DSK後払いリクエスト用の基底クラス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false)]
	public class BaseDskDeferredRequest : IHttpApiRequestData
	{
		/// <summary>コンストラクタ</summary>
		public BaseDskDeferredRequest()
		{
			this.ShopInfo = new ShopInfoElement();
		}

		/// <summary>
		/// ポスト値生成
		/// </summary>
		/// <returns>生成したポスト値</returns>
		public virtual string CreatePostString()
		{
			return SerializeHelper.Serialize(this);
		}

		/// <summary>加盟店情報</summary>
		[XmlElement("shopInfo")]
		public ShopInfoElement ShopInfo;
	}

	/// <summary>
	/// 加盟店情報要素
	/// </summary>
	public class ShopInfoElement
	{
		/// <summary>コンストラクタ</summary>
		public ShopInfoElement()
		{
			this.ShopCode = Constants.PAYMENT_SETTING_DSK_DEFERRED_SHOPCODE;
			this.TerminalId = Constants.PAYMENT_SETTING_DSK_DEFERRED_TERMINAI_ID;
			this.ShopPassword = Constants.PAYMENT_SETTING_DSK_DEFERRED_SHOP_PASSWORD;
		}

		/// <summary>加盟店ID</summary>
		[XmlElement("shopCode")]
		public string ShopCode;

		/// <summary>接続元ID</summary>
		[XmlElement("terminalId")]
		public string TerminalId;

		/// <summary>接続パスワード</summary>
		[XmlElement("shopPassword")]
		public string ShopPassword;
	}
}
