/*
=========================================================================================================
  Module      : 項目メモ編集ページ処理(FieldMemoSettingInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI;
using w2.Domain.FieldMemoSetting;

public partial class Form_Common_FieldMemoSetting_FieldMemoSettingInput : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (this.LoginOperatorId == "")
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOP_OPERATOR_LOGIN_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// パラメタチェック
			if (Request[Constants.FIELD_FIELDMEMOSETTING_TABLE_NAME] != null
				&& Request[Constants.FIELD_FIELDMEMOSETTING_FIELD_NAME] != null)
			{
				// パラメタ取得
				hfTableName.Value = StringUtility.ToEmpty(Request[Constants.FIELD_FIELDMEMOSETTING_TABLE_NAME]);
				hfFieldName.Value = StringUtility.ToEmpty(Request[Constants.FIELD_FIELDMEMOSETTING_FIELD_NAME]);

				var fieldMemoSettingData = new FieldMemoSettingService().Get(hfTableName.Value, hfFieldName.Value);

				this.Memo = (fieldMemoSettingData != null)
					? fieldMemoSettingData.Memo
					: "";

				// 画面制御
				InitializeComponents();
			}
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		tbMemoEdit.Text = this.Memo;
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnMemoUpdate_Click(object sender, EventArgs e)
	{
		var fieldMemoSettingService = new FieldMemoSettingService();
		var fieldMemoSettingData = new FieldMemoSettingService().Get(hfTableName.Value, hfFieldName.Value);
		this.Memo = tbMemoEdit.Text;

		if (fieldMemoSettingData != null)
		{
			fieldMemoSettingData.Memo = tbMemoEdit.Text;
			fieldMemoSettingData.LastChanged = this.LoginOperatorId;
			fieldMemoSettingService.Update(fieldMemoSettingData);
		}
		else
		{
			var temp = new FieldMemoSettingModel()
			{
				TableName = hfTableName.Value,
				FieldName = hfFieldName.Value,
				Memo = tbMemoEdit.Text,
				LastChanged = this.LoginOperatorId
			};
			fieldMemoSettingService.Insert(temp);
		}

		ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAllTips", "parent.updateTooltip();", true);
	}

	/// <summary>メモ</summary>
	public string Memo { get; set; }
}