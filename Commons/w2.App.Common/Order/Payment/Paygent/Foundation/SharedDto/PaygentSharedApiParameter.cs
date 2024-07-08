/*
=========================================================================================================
  Module      : PaygentApi 共通項目モデル (PaygentSharedApiParameter.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Paygent.Foundation.SharedDto
{
	/// <summary>
	/// 共通項目モデル
	/// </summary>
	[Serializable]
	public class PaygentSharedApiParameter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaygentSharedApiParameter()
		{
			this.MerchantId = Constants.PAYMENT_PAYGENT_MERCHANTID;
			this.ConnectId = Constants.PAYMENT_PAYGENT_CONNECTID;
			this.ConnectPassword = Constants.PAYMENT_PAYGENT_CONNECTIDPASSWORD;
			this.TelegramVersion = Constants.PAYMENT_PAYGENT_API_VERSION;
		}

		/// <summary>マーチャントID</summary>
		public string MerchantId { get; private set; }
		/// <summary>接続ID</summary>
		public string ConnectId { get; private set; }
		/// <summary>接続パスワード</summary>
		public string ConnectPassword { get; private set; }
		/// <summary>電文バージョン番号</summary>
		public string TelegramVersion { get; private set; }
	}
}
