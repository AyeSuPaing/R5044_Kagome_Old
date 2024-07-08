/*
=========================================================================================================
  Module      : リージョン変更選択肢のコントローラ処理(GlobalChangeIcon.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Global.Region;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// リージョン変更選択肢のコントローラ処理
/// </summary>
public partial class Form_Common_Global_GlobalChangeList : GlobalChangeMenuUserControl
{
	#region ラップ済みコントロール宣言
	WrappedRepeater WrRegionMenuList { get { return GetWrappedControl<WrappedRepeater>("rRegionMenuList"); } }
	#endregion

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
				this.WrRegionMenuList.DataSource = Constants.GLOBAL_CONFIGS.GlobalSettings.FrontLanguageCurrencies
					.Select(i => new RegionMenuViewModel
					{
						Url = UrlRegionParamManager.CreateUrl(languageCode: i.LanguageCode, currencyCode: i.CurrencyCode),
						SelectName = i.SelectName,
						Image = NationalFlagImageUtil.GetNationalFlagPath(i.LanguageCode)
					}
					).ToList();
				break;

			case MENU_TYPE_LANGUAGE:
				this.WrRegionMenuList.DataSource = Constants.GLOBAL_CONFIGS.GlobalSettings.FrontLanguages
					.Select(i => new RegionMenuViewModel
					{
						Url = UrlRegionParamManager.CreateUrl(languageCode: i.Code),
						SelectName = i.SelectName,
						Image = NationalFlagImageUtil.GetNationalFlagPath(i.Code)
					}
					).ToList();
				break;

			case MENU_TYPE_CURRENCY:
				this.WrRegionMenuList.DataSource = Constants.GLOBAL_CONFIGS.GlobalSettings.FrontCurrencies
					.Select(i => new RegionMenuViewModel
					{
						Url = UrlRegionParamManager.CreateUrl(currencyCode: i.Code),
						SelectName = i.SelectName,
						Image = NationalFlagImageUtil.GetNationalFlagPath(i.Code)
					}
					).ToList();
				break;

			default:
				return;
		};

		this.WrRegionMenuList.DataBind();
	}
}

/// <summary>
/// リージョン選択肢のビューモデル
/// </summary>
public class RegionMenuViewModel
{
	/// <summary></summary>
	public string Url { get; set; }
	/// <summary></summary>
	public string SelectName { get; set; }
	/// <summary></summary>
	public string Image { get; set; }
}