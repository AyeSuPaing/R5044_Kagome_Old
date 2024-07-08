/*
=========================================================================================================
  Module      : Gmail Account Setting(GmailAccountSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.IO;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// Gmail Account Setting
	/// </summary>
	public class GmailAccountSetting
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settingName">Setting Name</param>
		/// <param name="physicalDirpathGmailApi">Physical Dirpath Gmail Api</param>
		/// <param name="clientId">ClientId</param>
		/// <param name="clientSecret">ClientSecret</param>
		/// <param name="account">Account</param>
		public GmailAccountSetting(
			string settingName,
			string physicalDirpathGmailApi,
			string clientId,
			string clientSecret,
			string account)
		{
			this.SettingName = settingName.Trim();
			this.PhysicalDirpathGmailApi = physicalDirpathGmailApi.Trim();
			this.ClientId = clientId.Trim();
			this.ClientSecret = clientSecret.Trim();
			this.Account = account.Trim();
		}

		/// <summary>
		/// Gmail Account Setting
		/// </summary>
		/// <param name="settingName">Setting Name</param>
		/// <param name="physicalDirpathGmailApi">Physical Dirpath Gmail Api</param>
		/// <param name="setting">Setting</param>
		public GmailAccountSetting(string settingName, string physicalDirpathGmailApi, string[] setting)
			: this(
				settingName,
				physicalDirpathGmailApi,
				setting[0],
				setting[1],
				setting[2])
		{
		}

		#region Properties
		/// <summary>Setting Name</summary>
		public string SettingName { get; private set; }
		/// <summary>Client Id</summary>
		public string ClientId { get; private set; }
		/// <summary>Client Secret</summary>
		public string ClientSecret { get; private set; }
		/// <summary>Account</summary>
		public string Account { get; private set; }
		/// <summary>Physical Dirpath Gmail Api</summary>
		public string PhysicalDirpathGmailApi { get; private set; }
		/// <summary>Token File Name Default</summary>
		public string TokenFileNameDefault { get { return "Google.Apis.Auth.OAuth2.Responses.TokenResponse-me"; } }
		/// <summary>Token Directory</summary>
		public string TokenDirectory { get { return Path.Combine(this.PhysicalDirpathGmailApi, this.SettingName); } }
		#endregion
	}
}
