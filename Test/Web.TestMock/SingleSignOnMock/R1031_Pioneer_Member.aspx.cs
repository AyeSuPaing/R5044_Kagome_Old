/*
=========================================================================================================
  Module      : シングルサインオン会員情報テスト用モック（R1031_Pioneer用）クラス(R1031_Pioneer_Member.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// シングルサインオン会員情報テスト用モック（R1031_Pioneer用）クラス
/// </summary>
public partial class SingleSignOnMock_R1031_Pioneer_Member : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.ServiceId = this.Context.Request["qServiceId"];
		this.SsnId = this.Context.Request["qSsnId"];

		var xml = string.Empty;
		if ((string.IsNullOrEmpty(this.SsnId) == false)
			&& this.ServiceId == "PIO01")
		{
			xml = @"<?xml version='1.0' encoding='UTF-8'?>
					<ResultSet>
						<Status>1</Status>
						<Error/>
						<data index='1'>
							<Record>
								<kn_no>123456789012</kn_no>
								<ml>hanako.sumisho@scsk.jp</ml>
								<nm_kanji>住商　花子</nm_kanji>
								<nm_kana>スミショウ　ハナコ</nm_kana>
							</Record>
						</data>
						<LoginId>123456789012</LoginId>
						<EncryptKnNo>123456789012345678901234</EncryptKnNo>
					</ResultSet>";
		}
		else if (this.ServiceId != "PIO01")
		{
			xml = @"<?xml version='1.0' encoding='UTF-8'?>
					<ResultSet>
						<Status>0</Status>
						<Error>3</Error>
					</ResultSet>";
		}
		else
		{
			xml = @"<?xml version='1.0' encoding='UTF-8'?>
					<ResultSet>
						<Status>0</Status>
						<Error>4</Error>
					</ResultSet>";
		}

		this.Response.ClearHeaders();
		this.Response.AddHeader("content-type", "text/xml");
		this.Response.Write(xml);
		this.Response.End();
	}

	/// <summary>サービスID</summary>
	public string ServiceId { get; set; }
	/// <summary>セッションID</summary>
	public string SsnId { get; set; }
}