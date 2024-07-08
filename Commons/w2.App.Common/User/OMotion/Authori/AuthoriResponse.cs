/*
=========================================================================================================
  Module      : Authori Response(AuthoriResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using Newtonsoft.Json;

namespace w2.App.Common.User.OMotion.Authori
{
	/// <summary>
	/// Authori response
	/// </summary>
	[Serializable]
	public class AuthoriResponse : BaseOMotionResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public AuthoriResponse()
			: base()
		{
		}

		/// <summary>Status</summary>
		[JsonProperty("status")]
		public int Status { get; set; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
		/// <summary>Authori id</summary>
		[JsonProperty("authori_id")]
		public string AuthoriId { get; set; }
		/// <summary>Event id</summary>
		[JsonProperty("event_id")]
		public string EventId { get; set; }
		/// <summary>Event name</summary>
		[JsonProperty("event_name")]
		public string EventName { get; set; }
		/// <summary>User id hashed</summary>
		[JsonProperty("user_id_hashed")]
		public string UserIdHashed { get; set; }
		/// <summary>User id encrypt</summary>
		[JsonProperty("user_id_encrypt")]
		public string UserIdEncrypt { get; set; }
		/// <summary>Final result</summary>
		[JsonProperty("final_result")]
		public string FinalResult { get; set; }
		/// <summary>Result</summary>
		[JsonProperty("result")]
		public string Result { get; set; }
		/// <summary>Reason</summary>
		[JsonProperty("reason")]
		public string Reason { get; set; }
		/// <summary>Feedback</summary>
		[JsonProperty("feedback")]
		public string Feedback { get; set; }
		/// <summary>Feedback comment</summary>
		[JsonProperty("feedback_comment")]
		public string FeedbackComment { get; set; }
		/// <summary>User device id</summary>
		[JsonProperty("user_device_id")]
		public string UserDeviceId { get; set; }
		/// <summary>Event url</summary>
		[JsonProperty("event_url")]
		public string EventUrl { get; set; }
		/// <summary>Login success</summary>
		[JsonProperty("login_success")]
		public string LoginSuccess { get; set; }
		/// <summary>Cookie</summary>
		[JsonProperty("cookie")]
		public string Cookie { get; set; }
		/// <summary>Etag</summary>
		[JsonProperty("etag")]
		public string Etag { get; set; }
		/// <summary>Local strage</summary>
		[JsonProperty("local_strage")]
		public string LocalStrage { get; set; }
		/// <summary>Did short</summary>
		[JsonProperty("did_short")]
		public string DidShort { get; set; }
		/// <summary>Did middle</summary>
		[JsonProperty("did_middle")]
		public string DidMiddle { get; set; }
		/// <summary>Source ip</summary>
		[JsonProperty("source_ip")]
		public string SourceIp { get; set; }
		/// <summary>Useragent</summary>
		[JsonProperty("useragent")]
		public string Useragent { get; set; }
		/// <summary>Device category name</summary>
		[JsonProperty("device_category_name")]
		public string DeviceCategoryName { get; set; }
		/// <summary>Os name</summary>
		[JsonProperty("os_name")]
		public string OsName { get; set; }
		/// <summary>Os version</summary>
		[JsonProperty("os_version")]
		public string OsVersion { get; set; }
		/// <summary>Browser name</summary>
		[JsonProperty("browser_name")]
		public string BrowserName { get; set; }
		/// <summary>Browser version</summary>
		[JsonProperty("browser_version")]
		public string BrowserVersion { get; set; }
		/// <summary>Browser language</summary>
		[JsonProperty("browser_language")]
		public string BrowserLanguage { get; set; }
		/// <summary>Timezone offset</summary>
		[JsonProperty("timezone_offset")]
		public int TimezoneOffset { get; set; }
		/// <summary>Referer</summary>
		[JsonProperty("referer")]
		public string Referer { get; set; }
		/// <summary>Access at</summary>
		[JsonProperty("access_at")]
		public string AccessAt { get; set; }
		/// <summary>Send at</summary>
		[JsonProperty("send_at")]
		public string SendAt { get; set; }
		/// <summary>Status</summary>
		[JsonProperty("Authori at")]
		public string AuthoriAt { get; set; }
		/// <summary>Ip version</summary>
		[JsonProperty("ip_version")]
		public string IpVersion { get; set; }
		/// <summary>Ip country code</summary>
		[JsonProperty("ip_country_code")]
		public string IpCountryCode { get; set; }
		/// <summary>Ip country name</summary>
		[JsonProperty("ip_country_name")]
		public string IpCountryName { get; set; }
		/// <summary>Ip pref code</summary>
		[JsonProperty("ip_pref_code")]
		public string IpPrefCode { get; set; }
		/// <summary>Ip pref name</summary>
		[JsonProperty("ip_pref_name")]
		public string IpPrefName { get; set; }
		/// <summary>Ip city code</summary>
		[JsonProperty("ip_city_code")]
		public string IpCityCode { get; set; }
		/// <summary>Ip city name</summary>
		[JsonProperty("ip_city_name")]
		public string IpCityName { get; set; }
		/// <summary>Ip org name</summary>
		[JsonProperty("ip_org_name")]
		public string IpOrgName { get; set; }
		/// <summary>Ip domain name</summary>
		[JsonProperty("ip_domain_name")]
		public string IpDomainName { get; set; }
		/// <summary>Ip line name</summary>
		[JsonProperty("ip_line_name")]
		public string IpLineName { get; set; }
		/// <summary>Ip proxy</summary>
		[JsonProperty("ip_proxy")]
		public string IpProxy { get; set; }
		/// <summary>Ip pref location</summary>
		[JsonProperty("ip_pref_location")]
		public string IpPrefLocation { get; set; }
		/// <summary>Ip city location</summary>
		[JsonProperty("ip_city_location")]
		public string IpCityLocation { get; set; }
		/// <summary>Input type</summary>
		[JsonProperty("input_type")]
		public string InputType { get; set; }
		/// <summary>Ip tor flag</summary>
		[JsonProperty("ip_tor_flag")]
		public bool IpTorFlag { get; set; }
		/// <summary>Ip foreign flag</summary>
		[JsonProperty("ip_foreign_flag")]
		public bool IpForeignFlag { get; set; }
		/// <summary>Ip pref cf level</summary>
		[JsonProperty("ip_pref_cf_level")]
		public int IpPrefCfLevel { get; set; }
		/// <summary>Ip city cf level</summary>
		[JsonProperty("ip_city_cf_level")]
		public int IpCityCfLevel { get; set; }
		/// <summary>Ip line cf level</summary>
		[JsonProperty("ip_line_cf_level")]
		public int IpLineCfLevel { get; set; }
		/// <summary>Bot flag</summary>
		[JsonProperty("bot_flag")]
		public bool BotFlag { get; set; }
		/// <summary>Company user id</summary>
		[JsonProperty("company_user_id")]
		public string CompanyUserId { get; set; }
		/// <summary>Connected id</summary>
		[JsonProperty("connected_id")]
		public string ConnectedId { get; set; }
	}
}
