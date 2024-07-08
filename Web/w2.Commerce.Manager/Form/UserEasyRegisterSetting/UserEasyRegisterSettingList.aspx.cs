/*
=========================================================================================================
  Module      : かんたん会員登録設定ページ処理(UserEasyRegisterSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.UserEasyRegisterSetting;

public partial class Form_UserEasyRegisterSetting_UserEasyRegisterSettingList : BasePage
{
	/// <summary>更新フラグ</summary>
	protected bool mb_flg_result = false;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			var settings = new UserService().GetUserEasyRegisterSettingList();
			var content = Page.Form.FindControl("ContentPlaceHolderBody");

			foreach (var setting in settings)
			{
				var checkbox = GetCheckBoxControl(setting.ItemId);
				if (checkbox != null) checkbox.Checked = (setting.DisplayFlg == Constants.FLG_USER_EASY_REGISTER_FLG_EASY);
			}
		}
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		this.mb_flg_result = true;

		var settings = CreateSettingModel();
		UpdateUserEasyRegisterSetting(settings);
	}

	/// <summary>
	/// かんたん会員登録設定モデルの配列を作成します。
	/// </summary>
	/// <returns>かんたん会員登録設定モデルの配列</returns>
	protected UserEasyRegisterSettingModel[] CreateSettingModel()
	{
		var settings = new UserService().GetUserEasyRegisterSettingList();
		var content = Page.Form.FindControl("ContentPlaceHolderBody");
		var now = DateTime.Now;
		foreach (var setting in settings)
		{

			// モバイルデータの表示と非表示OFF時、モバイルメールアドレスを非表示にする
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (setting.ItemId == Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_ADDR2)) continue;
			var checkbox = GetCheckBoxControl(setting.ItemId);
			setting.DisplayFlg = checkbox.Checked ? Constants.FLG_USER_EASY_REGISTER_FLG_EASY : Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL;
			setting.DateChanged = now;
			setting.LastChanged = this.LoginOperatorName;
		}
		return settings;
	}

	/// <summary>
	/// かんたん会員登録設定マスタを更新します。
	/// </summary>
	/// <param name="models">かんたん会員登録設定モデルの配列</param>
	protected void UpdateUserEasyRegisterSetting(UserEasyRegisterSettingModel[] models)
	{
		var service = new UserService();
		foreach (var model in models) service.UpdateUserEasyRegisterSetting(model);
	}

	/// <summary>
	/// 項目IDからコントロールを返却します。
	/// </summary>
	/// <param name="itemId">項目ID</param>
	/// <returns>チェックボックスのコントロール</returns>
	protected CheckBox GetCheckBoxControl(string itemId)
	{
		switch (itemId)
		{
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COUNTRY:
				return this.cbUserCountry;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR1:
				return this.cbUserAddr1;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR2:
				return this.cbUserAddr2;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR3:
				return this.cbUserAddr3;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR4:
				return this.cbUserAddr4;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_BIRTH:
				return this.cbUserBirth;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_NAME:
				return this.cbUserCompanyName;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_POST_NAME:
				return this.cbUserCompanyPostName;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_ADDR2:
				var userMailAddr2 = (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) ? this.cbUserMailAddr2 : null;
				return userMailAddr2;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_FLG:
				return this.cbUserMailFlg;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME:
				return this.cbUserName;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME_KANA:
				return this.cbUserNameKana;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NICK_NAME:
				return this.cbUserNickName;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_SEX:
				return this.cbUserSex;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL1:
				return this.cbUserTel1;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL2:
				return this.cbUserTel2;
			case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ZIP:
				return this.cbUserZip;
			default:
				return null;
		}
	}
}
