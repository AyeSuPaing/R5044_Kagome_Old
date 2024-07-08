/*
=========================================================================================================
  Module      : GMOリクエストデータ用の基底クラス(BaseRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// リクエストデータ用の基底クラス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public abstract class BaseGmoRequest : IHttpApiRequestData
	{
		/// <summary>コンストラクタ</summary>
		public BaseGmoRequest()
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

		/// <summary>認証情報</summary>
		[XmlElement("shopInfo")]
		public ShopInfoElement ShopInfo;

	}

	#region ShopInfoElement
	/// <summary>
	/// 認証情報要素
	/// </summary>
	public class ShopInfoElement
	{
		/// <summary>コンストラクタ</summary>
		public ShopInfoElement()
		{
			this.AuthenticateId = Constants.PAYMENT_SETTING_GMO_DEFERRED_AUTHENTICATIONID;
			this.ShopCode = Constants.PAYMENT_SETTING_GMO_DEFERRED_SHOPCODE;
			this.ConnectPassword = Constants.PAYMENT_SETTING_GMO_DEFERRED_CONNECTPASSWORD;
		}

		/// <summary>コンストラクタ</summary>
		/// <param name="AuthenticateId">GMOに接続するための認証ID</param>
		/// <param name="ShopCode">店舗コード</param>
		/// <param name="ConnectPassword">接続パスワード</param>
		public ShopInfoElement(string AuthenticateId, string ShopCode, string ConnectPassword)
		{
			this.AuthenticateId = AuthenticateId;
			this.ShopCode = ShopCode;
			this.ConnectPassword = ConnectPassword;
		}

		/// <summary>認証ID</summary>
		/// <remarks>接続先を一位にするID</remarks>
		[XmlElement("authenticationId")]
		public string AuthenticateId;

		/// <summary>加盟店コード</summary>
		/// <remarks>GMO 後払いより個別に付加する加盟店様を一意にするコード。半角英数字以外にハイフンが登録可。</remarks>
		[XmlElement("shopCode")]
		public string ShopCode;

		/// <summary>接続パスワード</summary>
		/// <remarks>自動連携を利用する際のパスワード。</remarks>
		[XmlElement("connectPassword")]
		public string ConnectPassword;
	}
	#endregion

}
