/*
=========================================================================================================
  Module      : PayTg：端末状態確認モックページ処理(CheckDeviceStatusPayTgMock.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI;
using w2.App.Common.Order.Payment.PayTg;

public partial class Form_PayTgMock_CheckDeviceStatusPayTgMock : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 特になにもしない
	}

	/// <summary>
	/// ボタンクリック（デバイス状態：true、メッセージ：操作可能）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendOkOk_Click(object sender, EventArgs e)
	{
		ExecCheckDeviceStatus(
			PayTgConstants.RESPONSE_CAN_USE_PAYTG_DEVICE_VALID,
			PayTgConstants.RESPONSE_STATE_MESSAGE_PAYTG_DEVICE_VALID);
	}

	/// <summary>
	/// ボタンクリック（デバイス状態：true、メッセージ：未接続）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendOkNg_Click(object sender, EventArgs e)
	{
		ExecCheckDeviceStatus(
			PayTgConstants.RESPONSE_CAN_USE_PAYTG_DEVICE_VALID,
			PayTgConstants.RESPONSE_STATE_MESSAGE_PAYTG_DEVICE_INVALID);
	}

	/// <summary>
	/// ボタンクリック（デバイス状態：false、メッセージ：操作可能）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendNgOk_Click(object sender, EventArgs e)
	{
		ExecCheckDeviceStatus(
			PayTgConstants.RESPONSE_CAN_USE_PAYTG_DEVICE_INVALID,
			PayTgConstants.RESPONSE_STATE_MESSAGE_PAYTG_DEVICE_VALID);
	}

	/// <summary>
	/// ボタンクリック（デバイス状態：false、メッセージ：未接続）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendNgNg_Click(object sender, EventArgs e)
	{
		ExecCheckDeviceStatus(
			PayTgConstants.RESPONSE_CAN_USE_PAYTG_DEVICE_INVALID,
			PayTgConstants.RESPONSE_STATE_MESSAGE_PAYTG_DEVICE_INVALID);
	}

	/// <summary>
	/// 疎通確認実行
	/// </summary>
	/// <param name="canUseDevice"></param>
	/// <param name="stateMessage"></param>
	private void ExecCheckDeviceStatus(string canUseDevice, string stateMessage)
	{
		this.CanUseDevice = canUseDevice;
		this.StateMessage = stateMessage;

		lMessage.Text =
			string.Format(
				"デバイス状態：{0}　メッセージ：{1}",
				this.CanUseDevice,
				this.StateMessage);
		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"returnResponse",
			string.Format(
				"returnResponse('{0}', '{1}');",
				this.CanUseDevice,
				this.StateMessage),
			true);

	}

	/// <summary>デバイス状態</summary>
	private string CanUseDevice { get; set; }
	/// <summary>メッセージ（操作可能,未接続）</summary>
	private string StateMessage { get; set; }
}
