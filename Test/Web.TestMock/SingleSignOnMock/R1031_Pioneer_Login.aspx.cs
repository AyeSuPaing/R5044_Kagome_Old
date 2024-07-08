/*
=========================================================================================================
  Module      : シングルサインオン会員ログインテスト用モック（R1031_Pioneer用）クラス(R1031_Pioneer_Login.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Text;

/// <summary>
/// シングルサインオン会員ログインテスト用モック（R1031_Pioneer用）クラス
/// </summary>
public partial class R1031_Pioneer_Login : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.NextUrl = string.Format("{0}&{1}={2}", this.Context.Request["qUrl"], "SsnId", "abc1234567890");
		this.ServiceId = this.Context.Request["qServiceId"];
	}

	/// <summary>ログイン後遷移先URL</summary>
	public string NextUrl { get; set; }
	/// <summary>サービスId</summary>
	public string ServiceId { get; set; }
}