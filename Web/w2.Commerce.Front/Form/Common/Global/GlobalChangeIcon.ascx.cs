/*
=========================================================================================================
  Module      : リージョンアイコンのコントローラ処理(GlobalChangeIcon.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Global.Region;

/// <summary>
/// リージョンアイコンのコントローラ処理
/// </summary>
public partial class Form_Common_Global_GlobalChangeIcon : GlobalChangeMenuUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Constants.GLOBAL_OPTION_ENABLE == false) return;

		switch (Constants.GLOBAL_CONFIGS.GlobalSettings.FrontChangeType)
		{
			case MENU_TYPE_LANGUAGE_CURRENCY:
				this.UserNationanFlagPath = NationalFlagImageUtil.GetNationalFlagPath(RegionManager.GetInstance().Region.LanguageCode);
				break;

			case MENU_TYPE_LANGUAGE:
				this.UserNationanFlagPath = NationalFlagImageUtil.GetNationalFlagPath(RegionManager.GetInstance().Region.LanguageCode);
				break;

			case MENU_TYPE_CURRENCY:
				this.UserNationanFlagPath = NationalFlagImageUtil.GetNationalFlagPath(RegionManager.GetInstance().Region.CurrencyCode);
				break;

			default:
				return;
		};
	}
	/// <summary>現在のユーザリージョンの国旗画像パス</summary>
	protected string UserNationanFlagPath { get; private set; }
}