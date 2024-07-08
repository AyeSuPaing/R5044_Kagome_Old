/*
=========================================================================================================
  Module      : シリアルキー認証画面処理(SerialKeyAuthInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.SerialKey;

public partial class Form_User_SerialKeyAuthInput : BasePage
{
	#region ラップ済コントロール宣言
	protected WrappedTextBox WtbOrderId { get { return GetWrappedControl<WrappedTextBox>("tbOrderId"); } }
	protected WrappedTextBox WtbProductId { get { return GetWrappedControl<WrappedTextBox>("tbProductId"); } }
	protected WrappedTextBox WtbSerialKey01 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey01"); } }
	protected WrappedTextBox WtbSerialKey02 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey02"); } }
	protected WrappedTextBox WtbSerialKey03 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey03"); } }
	protected WrappedTextBox WtbSerialKey04 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey04"); } }
	protected WrappedTextBox WtbSerialKey05 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey05"); } }
	protected WrappedTextBox WtbSerialKey06 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey06"); } }
	protected WrappedTextBox WtbSerialKey07 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey07"); } }
	protected WrappedTextBox WtbSerialKey08 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey08"); } }
	protected WrappedTextBox WtbSerialKey09 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey09"); } }
	protected WrappedTextBox WtbSerialKey10 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey10"); } }
	protected WrappedTextBox WtbSerialKey11 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey11"); } }
	protected WrappedTextBox WtbSerialKey12 { get { return GetWrappedControl<WrappedTextBox>("tbSerialKey12"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
    {
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、HTTPSで再読込）
		//------------------------------------------------------
		CheckHttps(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_SERIAL_KEY_AUTH_INPUT);

		if (!IsPostBack)
		{
		}
	}

	/// <summary>
	/// 送信するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSend_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		Hashtable inputValues = new Hashtable();
		inputValues.Add(Constants.FIELD_SERIALKEY_ORDER_ID, tbOrderId.Text);
		inputValues.Add(Constants.FIELD_SERIALKEY_PRODUCT_ID, tbProductId.Text);
		inputValues.Add("serial_key_01", WtbSerialKey01.Text);
		inputValues.Add("serial_key_02", WtbSerialKey02.Text);
		inputValues.Add("serial_key_03", WtbSerialKey03.Text);
		inputValues.Add("serial_key_04", WtbSerialKey04.Text);
		inputValues.Add("serial_key_05", WtbSerialKey05.Text);
		inputValues.Add("serial_key_06", WtbSerialKey06.Text);
		inputValues.Add("serial_key_07", WtbSerialKey07.Text);
		inputValues.Add("serial_key_08", WtbSerialKey08.Text);
		inputValues.Add("serial_key_09", WtbSerialKey09.Text);
		inputValues.Add("serial_key_10", WtbSerialKey10.Text);
		inputValues.Add("serial_key_11", WtbSerialKey11.Text);
		inputValues.Add("serial_key_12", WtbSerialKey12.Text);

		Dictionary<string, string> errorMessages = Validator.ValidateAndGetErrorContainer("SerialKeyAuthInput", inputValues);

		// 入力エラー?
		if (errorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			List<CustomValidator> customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("SerialKeyAuthInput", errorMessages, customValidators);

			return;
		}

		//------------------------------------------------------
		// シリアルキーの検索
		//------------------------------------------------------
		StringBuilder keyStringBuilder = new StringBuilder();
		keyStringBuilder.Append(WtbSerialKey01.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey02.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey03.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey04.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey05.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey06.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey07.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey08.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey09.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey10.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey11.Text.Trim());
		keyStringBuilder.Append(WtbSerialKey12.Text.Trim());

		var serialKeyModel =  new SerialKeyService().Authenticate(
			this.WtbOrderId.Text.Trim(),
			this.WtbProductId.Text.Trim(),
			keyStringBuilder.ToString());

		if (serialKeyModel == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		switch (serialKeyModel.Status)
		{
			case Constants.FLG_SERIALKEY_STATUS_NOT_RESERVED: // 未引当
			case Constants.FLG_SERIALKEY_STATUS_RESERVED: // 予約済み
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				break;

			case Constants.FLG_SERIALKEY_STATUS_DELIVERED: // 引渡済
				if ((Constants.DIGITAL_CONTENTS_SERIAL_KEY_VALID_DAYS != 0)
					&& (serialKeyModel.DateDelivered != null)
					&& (DateTime.Today > ((DateTime)serialKeyModel.DateDelivered).AddDays(Constants.DIGITAL_CONTENTS_SERIAL_KEY_VALID_DAYS)))
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SERIALKEY_KEY_EXPIRED);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
				break;

			case Constants.FLG_SERIALKEY_STATUS_CANCELLED: // キャンセル済
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SERIALKEY_KEY_CANCELLED);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				break;
		}

		if (string.IsNullOrEmpty(serialKeyModel.DownloadUrl))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE_EMPTY_URL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//------------------------------------------------------
		// 認証成功：ダウンロードURL表示ページに遷移
		//------------------------------------------------------

		// 必要なデータをセッションへ格納（完了ページ表示＆ログ出力用）
		Dictionary<string, string> sessionData = new Dictionary<string, string>();
		sessionData.Add("order_id", serialKeyModel.OrderId);
		sessionData.Add("product_id", serialKeyModel.ProductId);
		sessionData.Add("product_name", serialKeyModel.ProductName);
		sessionData.Add("download_url", serialKeyModel.DownloadUrl);
		sessionData.Add("serial_key", serialKeyModel.SerialKey);
		sessionData.Add("user_id", serialKeyModel.UserId);

		Session[Constants.SESSION_KEY_PARAM] = sessionData;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_SERIAL_KEY_AUTH_COMPLETE);
	}

}
