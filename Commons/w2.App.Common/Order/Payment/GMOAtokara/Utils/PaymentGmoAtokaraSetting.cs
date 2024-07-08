/*
=========================================================================================================
  Module      : GMOアトカラ設定クラス(PaymentGmoAtokaraSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMOAtokara.Utils
{
	/// <summary>
	/// 設定
	/// </summary>
	public class PaymentGmoAtokaraSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopCode">加盟店コード</param>
		/// <param name="authenticationId">認証ID</param>
		/// <param name="connectPassword">接続パスワード</param>
		/// <param name="smsAuthenticationPassword">SMS認証接続パスワード</param>
		public PaymentGmoAtokaraSetting(
			string shopCode,
			string authenticationId,
			string connectPassword,
			string smsAuthenticationPassword)
		{
			this.ShopCode = shopCode;
			this.AuthenticationId = authenticationId;
			this.ConnectPassword = connectPassword;
			this.SmsAuthenticationPassword = smsAuthenticationPassword;
		}

		/// <summary>
		/// デフォルト設定取得
		/// </summary>
		/// <returns>設定</returns>
		internal static PaymentGmoAtokaraSetting GetDefaultSetting()
		{
			return new PaymentGmoAtokaraSetting(
				Constants.PAYMENT_GMOATOKARA_DEFERRED_SHOPCODE,
				Constants.PAYMENT_GMOATOKARA_DEFERRED_AUTHENTICATIONID,
				Constants.PAYMENT_GMOATOKARA_DEFERRED_CONNECTPASSWORD,
				Constants.PAYMENT_GMOATOKARA_DEFERRED_SMSAUTHENTICATIONPASSWORD);
		}

		/// <summary>加盟店コード</summary>
		public string ShopCode { get; private set; }
		/// <summary>認証ID</summary>
		public string AuthenticationId { get; private set; }
		/// <summary>接続パスワード</summary>
		public string ConnectPassword { get; private set; }
		/// <summary>SMS認証接続パスワード</summary>
		public string SmsAuthenticationPassword { get; private set; }
	}
}
