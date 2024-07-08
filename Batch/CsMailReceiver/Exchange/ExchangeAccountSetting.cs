/*
=========================================================================================================
  Module      : Exchange Account Setting (ExchangeAccountSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.IO;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// Exchange account setting
	/// </summary>
	public class ExchangeAccountSetting
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settingName">Setting name</param>
		/// <param name="physicalDirpathExchangeApi">Physical dirpath Exchange API</param>
		/// <param name="clientId">Client ID</param>
		/// <param name="clientSecret">Client secret</param>
		/// <param name="tenantId">Tenant ID</param>
		/// <param name="account">Account</param>
		public ExchangeAccountSetting(
			string settingName,
			string physicalDirpathExchangeApi,
			string clientId,
			string clientSecret,
			string tenantId,
			string account)
		{
			this.SettingName = settingName.Trim();
			this.PhysicalDirpathExchangeApi = physicalDirpathExchangeApi.Trim();
			this.ClientId = clientId.Trim();
			this.ClientSecret = clientSecret.Trim();
			this.TenantId = tenantId.Trim();
			this.Account = account.Trim();
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settingName">Setting name</param>
		/// <param name="physicalDirpathExchangeApi">Physical dirpath Exchange API</param>
		/// <param name="setting">Setting</param>
		public ExchangeAccountSetting(
			string settingName,
			string physicalDirpathExchangeApi,
			string[] setting)
			: this(
				settingName,
				physicalDirpathExchangeApi,
				setting[0],
				setting[1],
				setting[2],
				((setting.Length >= 4) ? setting[3] : string.Empty))
		{
		}
		#endregion

		#region Properties
		/// <summary>Setting name</summary>
		public string SettingName { get; private set; }
		/// <summary>Client ID</summary>
		public string ClientId { get; private set; }
		/// <summary>Client secret</summary>
		public string ClientSecret { get; private set; }
		/// <summary>Tenant ID</summary>
		public string TenantId { get; private set; }
		/// <summary>Account</summary>
		public string Account { get; private set; }
		/// <summary>Physical dirpath Exchange API</summary>
		public string PhysicalDirpathExchangeApi { get; private set; }
		/// <summary>Default token file name</summary>
		public string DefaultTokenFileName { get { return "Microsoft.Identity.Client.AccessToken"; } }
		/// <summary>Token directory</summary>
		public string TokenDirectory { get { return Path.Combine(this.PhysicalDirpathExchangeApi, this.SettingName); } }
		#endregion
	}
}
