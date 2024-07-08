/*
=========================================================================================================
  Module      : 名称翻訳設定一覧ページ処理(NameTranslationSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_NameTranslationSetting_NameTranslationSettingList : NameTranslationSettingPage
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
			int currentPageNumber;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false) currentPageNumber = 1;
			this.CurrentPageNo = currentPageNumber;

			InitializeComponents();

			this.SearchInfo = new SearchValues(
				this.RequestDataKbn,
				this.RequestTranslationTargetColumn,
				this.RequestMasterId1,
				this.RequestMasterId2,
				this.RequestLanguage,
				this.CurrentPageNo);

			Search();
		}
	}

	/// <summary>
	/// 検索クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 一覧のリピータにバインド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rNameTranslationSettingList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var rLanguages = (Repeater)e.Item.FindControl("rLanguages");
		var languageData = ((NameTranslationSettingContainer)e.Item.DataItem);

		rLanguages.DataSource = languageData.Languages;
		rLanguages.DataBind();
	}

	/// <summary>
	/// 対象データ区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlConditionDataKbn_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedValue = ((DropDownList)sender).SelectedValue;
		ddlConditionTranslationTargetColumn.DataSource = m_translationColumnProducts[selectedValue];
		ddlConditionTranslationTargetColumn.DataBind();
	}

	/// <summary>
	/// 新規登録クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		ddlConditionDataKbn.DataSource = m_dataKbnListItemCollection;
		ddlConditionDataKbn.DataBind();
		ddlConditionDataKbn.SelectedValue = this.RequestDataKbn;

		ddlConditionTranslationTargetColumn.DataSource = m_translationColumnProducts[ddlConditionDataKbn.SelectedValue];
		ddlConditionTranslationTargetColumn.DataBind();
		ddlConditionTranslationTargetColumn.SelectedValue = this.RequestTranslationTargetColumn;

		tbConditionMasterId1.Text = this.RequestMasterId1;
		tbConditionMasterId2.Text = this.RequestMasterId2;

		ddlConditionLanguages.DataSource = m_languageListItemCollection;
		ddlConditionLanguages.DataBind();
		ddlConditionLanguages.SelectedValue = this.RequestLanguage;
	}

	/// <summary>
	/// 検索処理
	/// </summary>
	private void Search()
	{
		var language = ddlConditionLanguages.SelectedValue.Split(SEPARATOR_UNDERSCORE);
		var languageCode = (language.Length == 2) ? language[0] : string.Empty;
		var languageLocaleId = (language.Length == 2) ? language[1] : string.Empty;

		var condition = new NameTranslationSettingListSearchCondition
		{
			DataKbn = ddlConditionDataKbn.SelectedValue,
			TranslationTargetColumn = ddlConditionTranslationTargetColumn.SelectedValue,
			MasterId1 = tbConditionMasterId1.Text.Trim(),
			MasterId2 = tbConditionMasterId2.Text.Trim(),
			LanguageCode = languageCode,
			LanguageLocaleId = languageLocaleId,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo,
		};

		var service = new NameTranslationSettingService();
		var count = service.GetCountNameTranslationSettingForListSearch(condition);

		if (count != 0)
		{
			var listData = service.GetNameTranslationSettingForListSearch(condition);
			var complementedListData = listData.ToList();
			complementedListData.ForEach(siteInformation => SetSiteInformationFromXml(siteInformation, SiteInformationUtility.SiteInformation));
			rNameTranslationSettingList.DataSource = complementedListData;
			rNameTranslationSettingList.DataBind();
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		var nextUrl = CreateListUrl();
		lbPager.Text = WebPager.CreateDefaultListPager(count, this.CurrentPageNo, nextUrl);
	}

	/// <summary>
	/// 一覧URL生成
	/// </summary>
	/// <returns>URL</returns>
	private string CreateListUrl()
	{
		var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_LIST)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_DATA_KBN, ddlConditionDataKbn.SelectedValue)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_TRANSLATION_TARGET_COLUMN, ddlConditionTranslationTargetColumn.SelectedValue)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID1, tbConditionMasterId1.Text.Trim())
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID2, tbConditionMasterId2.Text.Trim())
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_LANGUAGE, ddlConditionLanguages.SelectedValue)
			.CreateUrl();
		return nextUrl;
	}

	/// <summary>
	/// Xmlファイルからサイト基本情報を設定
	/// </summary>
	/// <param name="container">名称翻訳設定一覧表示用コンテナ</param>
	/// <param name="siteInformationList">サイト設定情報一覧</param>
	/// <returns></returns>
	private new NameTranslationSettingContainer SetSiteInformationFromXml(
		NameTranslationSettingContainer container,
		IEnumerable<SiteInformationUtility.SiteInformationModel> siteInformationList)
	{
		switch (container.TranslationTargetColumn)
		{
			case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_SHOP_NAME:
				container.BeforeTranslationalName = 
					siteInformationList.FirstOrDefault(x => (x.NodeName == SiteInformationUtility.SiteInformationType.ShopName)).Text;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_COMPANY_NAME:
				container.BeforeTranslationalName = 
					siteInformationList.FirstOrDefault(x => (x.NodeName == SiteInformationUtility.SiteInformationType.CompanyName)).Text;
				break;

			case Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_CONTACT_CENTER_INFO:
				container.BeforeTranslationalName = 
					siteInformationList.FirstOrDefault(x => (x.NodeName == SiteInformationUtility.SiteInformationType.ContactCenterInfo)).Text;
				break;
		}
		return container;
	}

	/// <summary>ページ番号</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState["CurrentPageNo"]; }
		set { ViewState["CurrentPageNo"] = value; }
	}
}