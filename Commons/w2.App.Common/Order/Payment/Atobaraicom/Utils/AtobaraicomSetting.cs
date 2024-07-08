/*
=========================================================================================================
  Module      : 後払い設定 (AtobaraicomSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払い設定
	/// </summary>
	public class AtobaraicomSetting
	{
		/// <summary>
		/// 後払い設定
		/// </summary>
		/// <param name="apiUrl">API Url</param>
		/// <param name="enterpriseId">受付事業者ID</param>
		/// <param name="siteId">受付サイトID</param>
		/// <param name="apiUserId">APIユーザーID</param>
		public AtobaraicomSetting(string apiUrl, string enterpriseId, string siteId, string apiUserId)
		{
			this.ReceiptOrderDate = DateTime.Now;
			this.ApiUrl = apiUrl;
			this.EnterpriseId = enterpriseId;
			this.SiteId = siteId;
			this.ApiUserId = apiUserId;
		}

		/// <summary>
		/// デフォルト設定を取得
		/// </summary>
		/// <returns>デフォルト設定を取得</returns>
		internal static AtobaraicomSetting GetDefaultSetting()
		{
			return new AtobaraicomSetting(
				null,
				Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED,
				Constants.PAYMENT_SETTING_ATOBARAICOM_SITE,
				Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID
			);
		}

		/// <summary>API URL</summary>
		public string ApiUrl { get; private set; }
		/// <summary>注文日</summary>
		public DateTime ReceiptOrderDate { get; private set; }
		/// <summary>受付事業者ID</summary>
		public string EnterpriseId { get; private set; }
		/// <summary>受付サイトID</summary>
		public string SiteId { get; private set; }
		/// <summary>APIユーザーID</summary>
		public string ApiUserId { get; private set; }
	}
}
