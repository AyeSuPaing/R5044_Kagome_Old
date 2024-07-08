/*
=========================================================================================================
  Module      : 名称翻訳設定共通ページ(NameTranslationSettingPage.cs)
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

/// <summary>
/// NameTranslationSettingPage の概要の説明です
/// </summary>
public class NameTranslationSettingPage : BasePage
{
	/// <summary>セパレータ：アンダースコア</summary>
	protected const char SEPARATOR_UNDERSCORE = '_';
	/// <summary>登録・更新完了メッセージ表示用パラメータ名</summary>
	protected const string REQUEST_SUCCESS_KEY = "success";
	/// <summary>登録・更新完了メッセージ表示用パラメータ値(成功)</summary>
	protected const string REQUEST_SUCCESS_VALUE_TRUE = "1";

	/// <summary>リストアイテムコレクション：言語</summary>
	protected static ListItemCollection m_languageListItemCollection;
	/// <summary>リストアイテムコレクション：対象データ区分</summary>
	protected static readonly ListItemCollection m_dataKbnListItemCollection = ValueText.GetValueItemList(
		Constants.TABLE_NAMETRANSLATIONSETTING,
		Constants.FIELD_NAMETRANSLATIONSETTING_DATA_KBN);
	/// <summary>対象データ区分に紐づく翻訳対象項目のリストアイテムコレクション</summary>
	protected static readonly Dictionary<string, ListItemCollection> m_translationColumnProducts = new Dictionary<string, ListItemCollection>
	{
		{
			string.Empty,
			new ListItemCollection()
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF])
		},
		{
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP,
			ValueText.GetValueItemList(Constants.TABLE_NAMETRANSLATIONSETTING,
				NameTranslationSettingService.ValueTextKeyForTranslationTargetColumn[Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP])
		},
	};

	/// <summary>
	/// 静的コンストラクタ
	/// </summary>
	static NameTranslationSettingPage()
	{
		var languageListItemCollection = new ListItemCollection
		{
			new ListItem(string.Empty, string.Empty)
		};
		languageListItemCollection.AddRange(
			Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Select(
				l => new ListItem(string.Format("{0}({1})", l.Code, l.LocaleId), string.Format("{0}{1}{2}", l.Code, SEPARATOR_UNDERSCORE, l.LocaleId))).ToArray());
		m_languageListItemCollection = languageListItemCollection;
	}

	/// <summary>
	/// Xmlファイルからサイト基本情報を設定
	/// </summary>
	/// <param name="container">名称翻訳設定一覧表示用コンテナ</param>
	/// <param name="siteInformationList">サイト設定情報一覧</param>
	/// <returns>名称翻訳設定一覧表示用コンテナ</returns>
	protected NameTranslationSettingContainer SetSiteInformationFromXml(
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

	/// <summary>
	/// 登録URL作成
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <param name="dataKbn">対象データ区分</param>
	/// <param name="translationTargetColumn">翻訳対象項目</param>
	/// <param name="masterId1">マスタID1</param>
	/// <param name="masterId2">マスタID2</param>
	/// <param name="success">成功:1</param>
	/// <returns>登録URL</returns>
	protected string CreateRegisterUrl(
		string actionStatus,
		string dataKbn,
		string translationTargetColumn,
		string masterId1,
		string masterId2,
		string success = null)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_DATA_KBN, dataKbn)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_TRANSLATION_TARGET_COLUMN, translationTargetColumn)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID1, masterId1)
			.AddParam(Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID2, masterId2);

		if (success != null) urlCreator.AddParam(REQUEST_SUCCESS_KEY, success);

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>対象データ区分</summary>
	protected string RequestDataKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_DATA_KBN]).Trim(); }
	}
	/// <summary>翻訳対象項目</summary>
	protected string RequestTranslationTargetColumn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_TRANSLATION_TARGET_COLUMN]).Trim(); }
	}
	/// <summary>マスタID1</summary>
	protected string RequestMasterId1
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID1]).Trim(); }
	}
	/// <summary>マスタID2</summary>
	protected string RequestMasterId2
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID2]).Trim(); }
	}
	/// <summary>言語</summary>
	protected string RequestLanguage
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NAME_TRANSLATION_SETTING_LANGUAGE]).Trim(); }
	}
	/// <summary>登録・更新完了メッセージ表示？</summary>
	protected bool IsRegisterSuccess
	{
		get { return StringUtility.ToEmpty(Request[REQUEST_SUCCESS_KEY]) == REQUEST_SUCCESS_VALUE_TRUE; }
	}

	/// <summary>名称翻訳設定一覧検索情報</summary>
	protected SearchValues SearchInfo
	{
		get
		{
			return (Session[Constants.SESSIONPARAM_KEY_NAME_TRANSLATION_SETTING_SEARCH_INFO] != null)
				? (SearchValues)Session[Constants.SESSIONPARAM_KEY_NAME_TRANSLATION_SETTING_SEARCH_INFO]
				: new SearchValues("", "", "", "", "", 1);
		}
		set { Session[Constants.SESSIONPARAM_KEY_NAME_TRANSLATION_SETTING_SEARCH_INFO] = value; }
	}

	/// <summary>検索値格納クラス</summary>
	[Serializable]
	protected class SearchValues
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dataKbn">対象データ区分</param>
		/// <param name="translationTargetColumn">翻訳対象項目</param>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="masterId2">マスタID2</param>
		/// <param name="language">言語</param>
		/// <param name="pageNum">ページ番号</param>
		public SearchValues(string dataKbn,
			string translationTargetColumn,
			string masterId1,
			string masterId2,
			string language,
			int pageNum)
		{
			this.DataKbn = dataKbn;
			this.TranslationTargetColumn = translationTargetColumn;
			this.MasterId1 = masterId1;
			this.MasterId2 = masterId2;
			this.Language = language;
			this.PageNum = pageNum;
		}
		/// <summary>対象データ区分</summary>
		public string DataKbn { get; set; }
		/// <summary>翻訳対象項目</summary>
		public string TranslationTargetColumn { get; set; }
		/// <summary>マスタID1</summary>
		public string MasterId1 { get; set; }
		/// <summary>マスタID2</summary>
		public string MasterId2 { get; set; }
		/// <summary>言語</summary>
		public string Language { get; set; }
		/// <summary>ページ番号</summary>
		public int PageNum { get; set; }
	}
}