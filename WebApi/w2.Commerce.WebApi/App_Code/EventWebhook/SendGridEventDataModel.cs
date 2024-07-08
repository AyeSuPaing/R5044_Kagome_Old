/*
=========================================================================================================
 Module      : イベントデータモデル(SendGridEventDataModel.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

/// <summary>
/// Sendgridイベントデータモデル
/// </summary>
public class SendGridEventDataModel
{
	/// <summary>イベントID</summary>
	[JsonProperty("sg_event_id")]
	public string SgEventId { get; set; }
	/// <summary>メッセージID</summary>
	[JsonProperty("sg_message_id")]
	public string SgMessageId { get; set; }
	/// <summary>SMTP ID</summary>
	[JsonProperty("smtp_id")]
	public string SmtpId { get; set; }
	/// <summary>イベント種別</summary>
	[JsonProperty("event")]
	public string Event { get; set; }
	/// <summary>メールアドレス</summary>
	[JsonProperty("email")]
	public string Email { get; set; }
	/// <summary>URL</summary>
	[JsonProperty("url")]
	public string Url { get; set; }
	/// <summary>ユーザエージェント</summary>
	[JsonProperty("useragent")]
	public string UserAgent { get; set; }
	/// <summary>タイムスタンプ</summary>
	[JsonProperty("timestamp")]
	public long Timestamp { get; set; }
}