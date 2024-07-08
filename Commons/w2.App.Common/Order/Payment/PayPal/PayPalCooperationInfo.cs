/*
=========================================================================================================
  Module      : ペイパル連携情報(PayPalCooperationInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.User;

namespace w2.App.Common.Order.Payment.PayPal
{
	/// <summary>
	/// ペイパル連携情報
	/// </summary>
	[Serializable]
	public class PayPalCooperationInfo
	{
		/// <summary>連携情報区切り文字</summary>
		private const char COOPERATION_INFO_SEPARATE_CHAR = '\t';

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userExtend">ユーザー拡張項目</param>
		public PayPalCooperationInfo(UserExtendModel userExtend)
			: this(
				userExtend.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID],
				(userExtend.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS] + COOPERATION_INFO_SEPARATE_CHAR).Split(COOPERATION_INFO_SEPARATE_CHAR)[0],
				(userExtend.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS] + COOPERATION_INFO_SEPARATE_CHAR).Split(COOPERATION_INFO_SEPARATE_CHAR)[1])
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userCreditCard">ユーザークレジットカード</param>
		public PayPalCooperationInfo(UserCreditCard userCreditCard)
			: this(
				userCreditCard.CooperationInfo.PayPalCustomerId,
				userCreditCard.AuthorName,
				userCreditCard.CooperationInfo.PayPalDeviceData)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="payPalLoginResult">PayPalログイン結果</param>
		public PayPalCooperationInfo(PayPalLoginResult payPalLoginResult)
			: this(
				payPalLoginResult.CustomerId,
				payPalLoginResult.AccountEMail,
				payPalLoginResult.DeviceData)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="customerId">BrainTree顧客ID</param>
		/// <param name="accountEMail">アカウントEメール</param>
		/// <param name="deviceData">デバイスデータ</param>
		private PayPalCooperationInfo(
			string customerId,
			string accountEMail,
			string deviceData)
		{
			this.CustomerId = customerId;
			this.AccountEMail = accountEMail;
			this.DeviceData = deviceData;
		}

		/// <summary>
		/// 連携情報を分割する
		/// </summary>
		/// <param name="cooprationInfos">連携情報</param>
		/// <returns>分割された連携情報</returns>
		private static string[] SplitCooprationInfos(string cooprationInfos)
		{
			var results = (cooprationInfos + COOPERATION_INFO_SEPARATE_CHAR).Split(COOPERATION_INFO_SEPARATE_CHAR);
			return results;
		}

		/// <summary>BrainTree顧客ID</summary>
		public string CustomerId { get; set; }
		/// <summary>アカウントEメール</summary>
		public string CooperationInfo
		{
			get { return this.AccountEMail; }
		}
		/// <summary>アカウントEメール</summary>
		public string AccountEMail { get; set; }
		/// <summary>デバイスデータ</summary>
		public string DeviceData { get; set; }
	}
}
