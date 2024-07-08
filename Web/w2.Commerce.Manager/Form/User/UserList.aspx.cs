/*
=========================================================================================================
  Module      : ユーザー情報一覧ページ処理(UserList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Common.Web;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.UserManagementLevel;
using w2.Common.Web;

public partial class Form_User_UserList : BasePage
{
	/// <summary>ユーザーシンボルフィールド名</summary>
	protected const string FIELD_USER_SYMBOL = "symbol";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			var requestParameters = GetParameters(Request);

			SetMenuAuthority();

			if (this.IsNotSearchDefault) return;

			// 不正パラメータが存在した場合
			if ((bool)requestParameters[Constants.ERROR_REQUEST_PRAMETER])
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// 検索情報保持(編集で利用)
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_USER_SEARCH_INFO] = requestParameters;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			var sqlParameters = GetSearchSqlInfo(requestParameters);
			var currentPageNumber = (int)requestParameters[Constants.REQUEST_KEY_PAGE_NO];

			// 検索条件を生成
			var cond = new UserSearchCondition();
			cond.UserIdLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_USER_ID + "_like_escaped"];
			cond.LoginIdLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_LOGIN_ID + "_like_escaped"];
			cond.MallId = (string)sqlParameters[Constants.FIELD_USER_MALL_ID];
			cond.UserKbn = (string)sqlParameters[Constants.FIELD_USER_USER_KBN];
			cond.EasyRegisterFlg = (string)sqlParameters[Constants.FIELD_USER_EASY_REGISTER_FLG];
			cond.NameLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_NAME + "_like_escaped"];
			cond.NameKanaLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_NAME_KANA + "_like_escaped"];
			cond.MailAddrLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_MAIL_ADDR + "_like_escaped"];
			cond.Tel1LikeEscaped = (string)sqlParameters[Constants.FIELD_USER_TEL1 + "_like_escaped"];
			cond.ZipLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_ZIP + "_like_escaped"];
			cond.AddrLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_ADDR + "_like_escaped"];
			cond.CompanyNameLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_COMPANY_NAME + "_like_escaped"];
			cond.CompanyPostNameLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_COMPANY_POST_NAME + "_like_escaped"];
			cond.MailFlag = (string)sqlParameters[Constants.FIELD_USER_MAIL_FLG];
			cond.DelFlg = (string)sqlParameters[Constants.FIELD_USER_DEL_FLG];
			cond.UserManagementLevelId = (string)sqlParameters[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID];
			cond.UserManagementLevelExclude = (string)sqlParameters["user_management_level_exclude"];
			cond.FixedPurchaseMemberFlg = (string)sqlParameters[Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG];
			cond.DateCreatedFrom = (sqlParameters[Constants.FIELD_USER_DATE_CREATED + "_from"] != DBNull.Value) ? (DateTime)sqlParameters[Constants.FIELD_USER_DATE_CREATED + "_from"] : (DateTime?)null;
			cond.DateCreatedTo = (sqlParameters[Constants.FIELD_USER_DATE_CREATED + "_to"] != DBNull.Value) ? (DateTime)sqlParameters[Constants.FIELD_USER_DATE_CREATED + "_to"] : (DateTime?)null;
			cond.DateChangedFrom = (sqlParameters[Constants.FIELD_USER_DATE_CHANGED + "_from"] != DBNull.Value) ? (DateTime)sqlParameters[Constants.FIELD_USER_DATE_CHANGED + "_from"] : (DateTime?)null;
			cond.DateChangedTo = (sqlParameters[Constants.FIELD_USER_DATE_CHANGED + "_to"] != DBNull.Value) ? (DateTime)sqlParameters[Constants.FIELD_USER_DATE_CHANGED + "_to"] : (DateTime?)null;
			cond.IntegratedFlg = (string)sqlParameters[Constants.FIELD_USER_INTEGRATED_FLG];
			cond.UserMemo = (string)sqlParameters[Constants.FIELD_USER_USER_MEMO];
			cond.UserMemoLikeEscaped = (string)sqlParameters[Constants.FIELD_USER_USER_MEMO + "_like_escaped"];
			//ユーザー拡張項目
			cond.UserExtendName = (string)sqlParameters["user_extend_name"];
			cond.UserExtendFlg = (string)sqlParameters["user_extend_flg"];
			cond.UserExtendType = (string)sqlParameters["user_extend_type"];
			cond.UserExtendLikeEscaped = (string)sqlParameters["user_extend_like_escaped"];
			cond.SortKbn = int.Parse((string)sqlParameters["sort_kbn"]);
			cond.BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (currentPageNumber - 1) + 1;
			cond.EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * currentPageNumber;
			cond.AccessCountoryIsoCode = (Constants.GLOBAL_OPTION_ENABLE) ? (string)sqlParameters[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE] : "";

			int totalUserCounts = 0;	// ページング可能総商品数
			// ユーザ該当件数取得
			totalUserCounts = new UserService().GetSearchHitCount(cond);
			if (totalUserCounts > 0)
			{
				// 検索情報取得
				var sqlParams = new Hashtable
					{
						{ Constants.FIELD_TARGETLIST_TARGET_TYPE, Constants.FLG_TARGETLIST_TARGET_TYPE_USER_LIST },
						{ Constants.TABLE_USER, sqlParameters }
					};
				Session[Constants.SESSION_KEY_PARAM + "EC"] = sqlParams;
				btnImportTargetList.Enabled = true;
			}
			else
			{
				btnImportTargetList.Enabled = false;
			}

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			var displayPager = true;
			var errorMessage = new StringBuilder();
			// 上限件数より多い？
			if (totalUserCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				errorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(totalUserCounts));
				errorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				displayPager = false;
			}
			// 該当件数なし？
			else if (totalUserCounts == 0)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = errorMessage.ToString();
			tdErrorMessage.ColSpan += Constants.MALLCOOPERATION_OPTION_ENABLED ? 0 : -1;
			trListError.Visible = (errorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// ユーザ一覧情報表示
			//------------------------------------------------------
			if (trListError.Visible == false)
			{
				// データバインド
				rList.DataSource = GetUserListWithUserSymbols(cond);
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (displayPager)
			{
				lbPager1.Text = WebPager.CreateDefaultListPager(totalUserCounts, currentPageNumber, CreateUserListUrl(requestParameters));
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 顧客区分
		ddlUserKbn.Items.Add(new ListItem("", ""));
		ddlUserKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_USER, Constants.VALUE_TEXT_KEY_USER_USER_KBN_ALL));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN))
		{
			if ((Constants.CS_OPTION_ENABLED == false) && (li.Value == Constants.FLG_USER_USER_KBN_CS)) continue;	// オプションOFF時はCS区分を追加しない
			// モバイルデータの表示と非表示OFF時はMB_USERとMB_GEST区分を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& ((li.Value == Constants.FLG_USER_USER_KBN_MOBILE_USER)
					|| (li.Value == Constants.FLG_USER_USER_KBN_MOBILE_GUEST))) continue;
			ddlUserKbn.Items.Add(li);
		}

		// メール配信希望
		ddlMailFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG))
		{
			ddlMailFlg.Items.Add(li);
		}

		// かんたん会員
		ddlUserEasyRegisterFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG))
		{
			ddlUserEasyRegisterFlg.Items.Add(li);
		}

		// サイト
		ddlSiteName.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlSiteName.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.VALUETEXT_PARAM_SITENAME,
				Constants.VALUETEXT_PARAM_OWNSITENAME));
		foreach (DataRowView drvMallCooperationSetting in GetMallCooperationSettingList(this.LoginOperatorShopId))
		{
			ddlSiteName.Items.Add(
				new ListItem(
					CreateSiteNameForList(
						(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
						(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]),
					(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]));
		}
		if (Constants.GLOBAL_OPTION_ENABLE || (Constants.URERU_AD_IMPORT_ENABLED == false))
		{
			// 「つくーる」削除
			var ddlItem = ddlSiteName.Items.FindByValue(Constants.FLG_USER_MALL_ID_URERU_AD);
			ddlSiteName.Items.Remove(ddlItem);
		}

		// 定期会員フラグ
		ddlFixedPurchaseMemberFlg.Items.Add(new ListItem("", ""));
		ddlFixedPurchaseMemberFlg.Items.Add(new ListItem(
			ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON), Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON));
		ddlFixedPurchaseMemberFlg.Items.Add(new ListItem(
			ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF), Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF));

		// ユーザー管理レベルID
		ddlUserManagementLevelId.Items.Add(new ListItem("", ""));
		var models = new UserManagementLevelService().GetAllList();
		ddlUserManagementLevelId.Items.AddRange(
			models.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());

		//ユーザー メモ フラグを設定します。
		ddlUserMemoFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO))
		{
			ddlUserMemoFlg.Items.Add(li);
		}

		//ユーザー拡張項目の設定
		this.UserExtendSettings = new UserExtendSettingList(this.LoginOperatorId);
		ddlUserExtendName.Items.Add(new ListItem("", ""));
		ddlUserExtendName.Items.AddRange(this.UserExtendSettings.Items.Select(ues => new ListItem(ues.SettingName, ues.SettingId)).ToArray());
		ddlUserExtendFlg.Items.Add(new ListItem("", ""));
		ddlUserExtendFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_USER, "user_extend_flg"));

		//グローバル対応 項目作成
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlCountryIsoCode.Items.Add(new ListItem("", ""));
			ddlCountryIsoCode.Items.AddRange(Constants.GLOBAL_CONFIGS.GlobalSettings.CountryIsoCodes.Select(cic => new ListItem(cic, cic)).ToArray());
		}

		// 日本配送なしの場合
		if (this.IsShippingCountryAvailableJp == false)
		{
			// 「かな」の並び順削除
			var ddlItem = ddlSortKbn.Items.FindByValue("2");
			ddlSortKbn.Items.Remove(ddlItem);
			ddlItem = ddlSortKbn.Items.FindByValue("3");
			ddlSortKbn.Items.Remove(ddlItem);
		}

		// e-SCOTT会員削除ボタン表示・非表示
		lbDeleteEScottKaiin.Visible =
			(OrderCommon.IsPaymentCardTypeEScott
				&& MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_ESCOTT_DELETE_MEMBER_DL));
	}

	/// <summary>
	/// ユーザ一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">ユーザ一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();
		int iCurrentPageNumber = 1;
		bool blParamError = false;

		try
		{
			// ユーザーID
			htResult.Add(Constants.REQUEST_KEY_USER_USER_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_ID]));
			tbUserId.Text = hrRequest[Constants.REQUEST_KEY_USER_USER_ID];
			// 顧客区分
			htResult.Add(Constants.REQUEST_KEY_USER_USER_KBN, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_KBN]));
			ddlUserKbn.SelectedValue = hrRequest[Constants.REQUEST_KEY_USER_USER_KBN];
			// 氏名
			htResult.Add(Constants.REQUEST_KEY_USER_NAME, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_NAME]));
			tbName.Text = hrRequest[Constants.REQUEST_KEY_USER_NAME];
			// 氏名(かな)
			htResult.Add(Constants.REQUEST_KEY_USER_NAME_KANA, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_NAME_KANA]));
			tbNameKana.Text = hrRequest[Constants.REQUEST_KEY_USER_NAME_KANA];
			// メール配信希望
			htResult.Add(Constants.REQUEST_KEY_USER_MAIL_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_MAIL_FLG]));
			ddlMailFlg.SelectedValue = hrRequest[Constants.REQUEST_KEY_USER_MAIL_FLG];
			// 電話番号
			htResult.Add(Constants.REQUEST_KEY_USER_TEL1, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_TEL1]));
			tbTel.Text = hrRequest[Constants.REQUEST_KEY_USER_TEL1];
			// メールアドレス
			htResult.Add(Constants.REQUEST_KEY_USER_MAIL_ADDR, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_MAIL_ADDR]));
			tbMailAddr.Text = hrRequest[Constants.REQUEST_KEY_USER_MAIL_ADDR];
			// 郵便番号
			htResult.Add(Constants.REQUEST_KEY_USER_ZIP, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_ZIP]));
			tbZip.Text = hrRequest[Constants.REQUEST_KEY_USER_ZIP];
			// 住所
			htResult.Add(Constants.REQUEST_KEY_USER_ADDR, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_ADDR]));
			tbAddr.Text = hrRequest[Constants.REQUEST_KEY_USER_ADDR];
			// かんたん会員フラグ
			htResult.Add(Constants.REQUEST_KEY_USER_EASY_REGISTER_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_EASY_REGISTER_FLG]));
			ddlUserEasyRegisterFlg.SelectedValue = hrRequest[Constants.REQUEST_KEY_USER_EASY_REGISTER_FLG];
			// 企業名
			htResult.Add(Constants.REQUEST_KEY_USER_COMPANY_NAME, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_COMPANY_NAME]));
			thCompanyName.Text = hrRequest[Constants.REQUEST_KEY_USER_COMPANY_NAME];
			// 部署名
			htResult.Add(Constants.REQUEST_KEY_USER_COMPANY_POST_NAME, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_COMPANY_POST_NAME]));
			thCompanyPostName.Text = hrRequest[Constants.REQUEST_KEY_USER_COMPANY_POST_NAME];
			// 退会者
			htResult.Add(Constants.REQUEST_KEY_USER_DEL_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_DEL_FLG]));
			cbDelFlg.Checked = (hrRequest[Constants.REQUEST_KEY_USER_DEL_FLG] == Constants.FLG_USER_DELFLG_DELETED);
			// サイト
			htResult.Add(Constants.REQUEST_KEY_USER_MALL_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_MALL_ID]));
			ddlSiteName.SelectedValue = hrRequest[Constants.REQUEST_KEY_USER_MALL_ID];
			// ソート区分
			string strSortKbn = null;
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_USER_LIST_USER_ID_ASC:				// ユーザID/昇順
				case Constants.KBN_SORT_USER_LIST_USER_ID_DESC:				// ユーザID/降順
				case Constants.KBN_SORT_USER_LIST_NAME_ASC:					// 氏名/昇順
				case Constants.KBN_SORT_USER_LIST_NAME_DESC:				// 氏名/降順
				case Constants.KBN_SORT_USER_LIST_NAME_KANA_ASC:			// 氏名(かな)/昇順
				case Constants.KBN_SORT_USER_LIST_NAME_KANA_DESC:			// 氏名(かな)/降順
				case Constants.KBN_SORT_USER_LIST_DATE_CREATED_ASC:			// 作成日/昇順
				case Constants.KBN_SORT_USER_LIST_DATE_CREATED_DESC:		// 作成日/降順
				case Constants.KBN_SORT_USER_LIST_DATE_CHANGED_ASC:			// 更新日/昇順
				case Constants.KBN_SORT_USER_LIST_DATE_CHANGED_DESC:		// 更新日/降順
					strSortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					strSortKbn = Constants.KBN_SORT_USER_LIST_DEFAULT;		// 商品ID/昇順がデフォルト
					break;
				default:
					blParamError = true;
					break;
			}
			htResult.Add(Constants.REQUEST_KEY_SORT_KBN, strSortKbn);
			ddlSortKbn.SelectedValue = strSortKbn;
			// ユーザーログインID
			htResult.Add(Constants.REQUEST_KEY_USER_LOGIN_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_LOGIN_ID]));
			tbUserLoginId.Text = hrRequest[Constants.REQUEST_KEY_USER_LOGIN_ID];
			// ユーザー管理レベルID
			htResult.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID]));
			ddlUserManagementLevelId.SelectedValue = hrRequest[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID];
			// 選択したユーザー管理レベルを除いて検索するか、しないか
			htResult.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE]));
			cbUserManagementLevelExclude.Checked = (hrRequest[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE] == Constants.FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_ON);
			// 定期会員割引
			htResult.Add(Constants.REQUEST_KEY_USER_FIXED_PURCHASE_MEMBER_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_FIXED_PURCHASE_MEMBER_FLG]));
			ddlFixedPurchaseMemberFlg.SelectedValue = (string)htResult[Constants.REQUEST_KEY_USER_FIXED_PURCHASE_MEMBER_FLG];
			// 統合ユーザー
			htResult.Add(Constants.REQUEST_KEY_USER_INTEGRATED_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_INTEGRATED_FLG]));
			cbUserIntegrationFlg.Checked = (hrRequest[Constants.REQUEST_KEY_USER_INTEGRATED_FLG] == Constants.FLG_USER_INTEGRATED_FLG_DONE);
			if (string.IsNullOrEmpty(hrRequest[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM]))
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM,
					Request[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM]);
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM,
					Request[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM]);
				var startUserDateCreated = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM]));

				if (string.IsNullOrEmpty(startUserDateCreated) == false)
				{
					ucUserDateCreated.SetStartDate(DateTime.Parse(startUserDateCreated));
				}
			}

			if (string.IsNullOrEmpty(hrRequest[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO]))
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO,
					Request[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO]);
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO,
					Request[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO]);
				var endUserDateCreated = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO]));

				if (string.IsNullOrEmpty(endUserDateCreated) == false)
				{
					ucUserDateCreated.SetEndDate(DateTime.Parse(endUserDateCreated));
				}
			}

			if (string.IsNullOrEmpty(hrRequest[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM]))
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM,
					Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM]);
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM,
					Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM]);
				var startUserDateChanged = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM]));

				if (string.IsNullOrEmpty(startUserDateChanged) == false)
				{
					ucUserDateChanged.SetStartDate(DateTime.Parse(startUserDateChanged));
				}
			}

			if (string.IsNullOrEmpty(hrRequest[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO]))
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO,
					Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO]);
				htResult.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO,
					Request[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO]);

				var endUserDateChanged = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO]));

				if (string.IsNullOrEmpty(endUserDateChanged) == false)
				{
					ucUserDateChanged.SetEndDate(DateTime.Parse(endUserDateChanged));
				}
			}

			// ページ番号（ページャ動作時のみもちまわる）
			if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				iCurrentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
			}
			// グローバル対応 国ISOコード
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				htResult.Add(Constants.REQUEST_KEY_USER_COUNTRY_ISO_CODE, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_COUNTRY_ISO_CODE]));
				ddlCountryIsoCode.SelectedValue = hrRequest[Constants.REQUEST_KEY_USER_COUNTRY_ISO_CODE];
			}
		}
		catch
		{
			blParamError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		htResult.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);
		//ユーザメモ
		htResult.Add(Constants.REQUEST_KEY_USER_USER_MEMO, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_MEMO]));
		ddlUserMemoFlg.SelectedValue = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_MEMO]);
		//パラメーターのユーザーのメモのテキストを追加します。
		htResult.Add(Constants.REQUEST_KEY_USER_USER_MEMO_TEXT, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_MEMO_TEXT]));
		tbUserMemo.Text = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_MEMO_TEXT]);
		//ユーザ拡張項目
		htResult.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_NAME, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_NAME]));
		ddlUserExtendName.SelectedValue = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_NAME]);
		htResult.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_FLG, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_FLG]));
		ddlUserExtendFlg.SelectedValue = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_FLG]);
		htResult.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_TYPE, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_TYPE]));
		htResult.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT]));
		InitializeUserExtendText(
			StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_FLG]),
			StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT]));

		return htResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htSearch)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		// ユーザーID
		htResult.Add(Constants.FIELD_USER_USER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_USER_ID]));
		// ユーザー区分
		htResult.Add(Constants.FIELD_USER_USER_KBN, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_USER_KBN]));
		// 氏名
		htResult.Add(Constants.FIELD_USER_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_NAME]));
		// 氏名(かな)
		htResult.Add(Constants.FIELD_USER_NAME_KANA + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_NAME_KANA]));
		// メール配信希望
		htResult.Add(Constants.FIELD_USER_MAIL_FLG, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_MAIL_FLG]));
		// 電話番号
		htResult.Add(Constants.FIELD_USER_TEL1 + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_TEL1]));
		// メールアドレス
		htResult.Add(Constants.FIELD_USER_MAIL_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_MAIL_ADDR]));
		// 郵便番号
		htResult.Add(Constants.FIELD_USER_ZIP + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_ZIP]));
		// 住所
		htResult.Add(Constants.FIELD_USER_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_ADDR]));
		// かんたん会員フラグ
		htResult.Add(Constants.FIELD_USER_EASY_REGISTER_FLG, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_EASY_REGISTER_FLG]));
		// 企業名
		htResult.Add(Constants.FIELD_USER_COMPANY_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_COMPANY_NAME]));
		// 部署名
		htResult.Add(Constants.FIELD_USER_COMPANY_POST_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_COMPANY_POST_NAME]));
		// 退会者
		htResult.Add(Constants.FIELD_USER_DEL_FLG, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DEL_FLG]));
		// サイト
		htResult.Add(Constants.FIELD_USER_MALL_ID, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_MALL_ID]));
		// ソート区分
		htResult.Add("sort_kbn", StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_SORT_KBN]));
		// ユーザーログインID
		htResult.Add(Constants.FIELD_USER_LOGIN_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_LOGIN_ID]));
		// ユーザー管理レベルID
		htResult.Add(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID]));
		// 選択したユーザー管理レベルを除いて検索するか、しないか
		htResult.Add("user_management_level_exclude", StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE]));
		// 定期会員フラグ
		htResult.Add(Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_FIXED_PURCHASE_MEMBER_FLG]));
		// ユーザー統合
		htResult.Add(Constants.FIELD_USER_INTEGRATED_FLG, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_INTEGRATED_FLG]));
		// 作成日(From)
		htResult.Add(Constants.FIELD_USER_DATE_CREATED + "_from", DBNull.Value);
		var dateCreatedFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM]),
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM]));
		if (Validator.IsDate(dateCreatedFrom))
		{
			htResult[Constants.FIELD_USER_DATE_CREATED + "_from"] = DateTime.Parse(dateCreatedFrom);
		}
		// 作成日(To)
		htResult.Add(Constants.FIELD_USER_DATE_CREATED + "_to", DBNull.Value);
		var dateCreatedTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO]),
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO]));
		if (Validator.IsDate(dateCreatedTo))
		{
			htResult[Constants.FIELD_USER_DATE_CREATED + "_to"] =
				DateTime.Parse(dateCreatedTo).AddSeconds(1);
		}
		// 更新日(From)
		htResult.Add(Constants.FIELD_USER_DATE_CHANGED + "_from", DBNull.Value);
		var dateChangedFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM]),
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM]));
		if (Validator.IsDate(dateChangedFrom))
		{
			htResult[Constants.FIELD_USER_DATE_CHANGED + "_from"] = DateTime.Parse(dateChangedFrom);
		}
		// 更新日(To)
		htResult.Add(Constants.FIELD_USER_DATE_CHANGED + "_to", DBNull.Value);
		var dateChangedTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO]),
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO]));
		if (Validator.IsDate(dateChangedTo))
		{
			htResult[Constants.FIELD_USER_DATE_CHANGED + "_to"]
				= DateTime.Parse(dateChangedTo).AddSeconds(1);
		}
		// ユーザー メモ
		htResult.Add(Constants.FIELD_USER_USER_MEMO, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_USER_MEMO]));
		// ユーザー メモのテキスト
		htResult.Add(Constants.FIELD_USER_USER_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_USER_MEMO_TEXT]));

		// ユーザー拡張項目
		htResult.Add("user_extend_name", StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_NAME]));
		htResult.Add("user_extend_flg", StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_FLG]));
		htResult.Add("user_extend_type", StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_TYPE]));
		htResult.Add("user_extend_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT]));

		//グローバル対応
		htResult.Add(Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_USER_COUNTRY_ISO_CODE]));

		return htResult;
	}

	/// <summary>
	/// ユーザ一覧情報を表示分だけ取得
	/// </summary>
	/// <param name="sqlParameters">SQLパラメタ情報</param>
	/// <param name="currentPageNumber">表示開始記事番号</param>
	/// <returns>ユーザ一覧情報</returns>
	private UserSearchResult[] GetUserListWithUserSymbols(UserSearchCondition cond)
	{
		// ユーザ一覧情報取得
		var userList = new UserService().Search(cond);

		// ユーザーシンボル取得
		var userIds = userList.Select(user => user.UserId).Distinct().ToArray();
		var symbols = GetUserSymbols(userIds);

		foreach (var user in userList)
		{
			user.Symbol = symbols.Where(s => s.Key == user.UserId).FirstOrDefault().Value;
		}
		return userList;
	}

	/// <summary>
	/// ユーザー一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>ユーザー一覧遷移URL</returns>
	private static string CreateUserListUrl(Hashtable htSearch)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USER_USER_ID).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_ID]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_KBN]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_NAME]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_NAME_KANA).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_NAME_KANA]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_MAIL_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_MAIL_FLG]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_TEL1).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_TEL1]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_MAIL_ADDR).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_MAIL_ADDR]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_ZIP).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_ZIP]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_ADDR).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_ADDR]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_EASY_REGISTER_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_EASY_REGISTER_FLG]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_COMPANY_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_COMPANY_NAME]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_COMPANY_POST_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_COMPANY_POST_NAME]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DEL_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DEL_FLG]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_MALL_ID).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_MALL_ID]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_SORT_KBN]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_LOGIN_ID).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_LOGIN_ID]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_FIXED_PURCHASE_MEMBER_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_FIXED_PURCHASE_MEMBER_FLG]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_INTEGRATED_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_INTEGRATED_FLG]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM).Append("=")
			.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM)
			.Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO).Append("=")
			.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO).Append("=")
			.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM)
			.Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM)
			.Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO)
			.Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO)
			.Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_MEMO).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_MEMO]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_MEMO_TEXT).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_MEMO_TEXT]));
		//ユーザー拡張項目
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_EXTEND_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_NAME]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_EXTEND_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_FLG]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_EXTEND_TYPE).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_TYPE]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USER_COUNTRY_ISO_CODE).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_USER_COUNTRY_ISO_CODE]));

		return sbUrl.ToString();
	}

	/// <summary>
	/// ユーザー一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>ユーザー一覧遷移URL</returns>
	public static string CreateUserListUrl(Hashtable htSearch, int iPageNumber)
	{
		string strResult = CreateUserListUrl(htSearch) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();

		return strResult;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfo()
	{
		// 変数宣言
		Hashtable htSearch = new Hashtable();

		htSearch.Add(Constants.REQUEST_KEY_USER_USER_ID, tbUserId.Text.Trim());					// ユーザーID
		htSearch.Add(Constants.REQUEST_KEY_USER_USER_KBN, ddlUserKbn.SelectedValue);			// 顧客区分
		htSearch.Add(Constants.REQUEST_KEY_USER_NAME, tbName.Text.Trim());						// 氏名
		htSearch.Add(Constants.REQUEST_KEY_USER_NAME_KANA, tbNameKana.Text.Trim());				// 氏名(かな)
		htSearch.Add(Constants.REQUEST_KEY_USER_MAIL_FLG, ddlMailFlg.SelectedValue);			// メール配信希望
		htSearch.Add(Constants.REQUEST_KEY_USER_TEL1, tbTel.Text.Trim());						// 電話番号
		htSearch.Add(Constants.REQUEST_KEY_USER_MAIL_ADDR, tbMailAddr.Text.Trim());				// メールアドレス
		htSearch.Add(Constants.REQUEST_KEY_USER_ZIP, tbZip.Text.Trim());						// 郵便番号
		htSearch.Add(Constants.REQUEST_KEY_USER_ADDR, tbAddr.Text.Trim());						// 住所
		htSearch.Add(Constants.REQUEST_KEY_USER_EASY_REGISTER_FLG, ddlUserEasyRegisterFlg.SelectedValue); // かんたん会員フラグ
		htSearch.Add(Constants.REQUEST_KEY_USER_COMPANY_NAME, thCompanyName.Text.Trim());  // 企業名
		htSearch.Add(Constants.REQUEST_KEY_USER_COMPANY_POST_NAME, thCompanyPostName.Text.Trim());  // 部署名
		htSearch.Add(Constants.REQUEST_KEY_USER_DEL_FLG,
			(cbDelFlg.Checked) ? Constants.FLG_USER_DELFLG_DELETED : Constants.FLG_USER_DELFLG_UNDELETED); // 退会者
		htSearch.Add(Constants.REQUEST_KEY_USER_MALL_ID, ddlSiteName.SelectedValue);			// モールID
		htSearch.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);					// ソート区分
		htSearch.Add(Constants.REQUEST_KEY_FIXED_PURCHASE_MEMBER_FLG, ddlFixedPurchaseMemberFlg.SelectedValue);		// 定期会員フラグ
		htSearch.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID, ddlUserManagementLevelId.SelectedValue); // ユーザー管理レベルID
		htSearch.Add(Constants.REQUEST_KEY_USER_LOGIN_ID, tbUserLoginId.Text.Trim());			// ユーザーログインID
		htSearch.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE,
			(cbUserManagementLevelExclude.Checked) ? Constants.FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_ON : Constants.FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_OFF); // 選択したユーザー管理レベルを除いて検索するか、しないか
		htSearch.Add(Constants.REQUEST_KEY_USER_INTEGRATED_FLG,
			(cbUserIntegrationFlg.Checked) ? Constants.FLG_USER_INTEGRATED_FLG_DONE : string.Empty); // 統合ユーザー
		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_FROM,
			string.IsNullOrEmpty(ucUserDateCreated.HfStartDate.Value)
				? null
				: ucUserDateCreated.HfStartDate.Value);
		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_FROM,
			string.IsNullOrEmpty(ucUserDateCreated.HfStartTime.Value)
				? null
				: ucUserDateCreated.HfStartTime.Value);

		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_DATE_TO,
			string.IsNullOrEmpty(ucUserDateCreated.HfEndDate.Value)
				? null
				: ucUserDateCreated.HfEndDate.Value);

		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CREATED_TIME_TO,
			string.IsNullOrEmpty(ucUserDateCreated.HfEndTime.Value)
				? null
				: ucUserDateCreated.HfEndTime.Value);

		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM,
			string.IsNullOrEmpty(ucUserDateChanged.HfStartDate.Value)
				? null
				: ucUserDateChanged.HfStartDate.Value);
		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM,
			string.IsNullOrEmpty(ucUserDateChanged.HfStartTime.Value)
				? null
				: ucUserDateChanged.HfStartTime.Value);

		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_DATE_TO,
			string.IsNullOrEmpty(ucUserDateChanged.HfEndDate.Value)
				? null
				: ucUserDateChanged.HfEndDate.Value);
		htSearch.Add(Constants.REQUEST_KEY_USER_DATE_CHANGED_TIME_TO,
			string.IsNullOrEmpty(ucUserDateChanged.HfEndTime.Value)
				? null
				: ucUserDateChanged.HfEndTime.Value);

		htSearch.Add(Constants.REQUEST_KEY_USER_USER_MEMO, ddlUserMemoFlg.SelectedValue);                               // ユーザー メモ
		htSearch.Add(Constants.REQUEST_KEY_USER_USER_MEMO_TEXT, tbUserMemo.Text.Trim());                                // ユーザー メモのテキスト

		htSearch.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_NAME, ddlUserExtendName.SelectedValue);                               // ユーザー拡張項目
		htSearch.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_FLG, ddlUserExtendFlg.SelectedValue);
		if (string.IsNullOrEmpty(ddlUserExtendName.SelectedValue))
		{
			htSearch.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT, "");
		}
		else
		{
			var userExtendSetting =
				this.UserExtendSettings.Items.Find(
					userExtend => userExtend.SettingId == ddlUserExtendName.SelectedValue);
			htSearch.Add(
				Constants.REQUEST_KEY_USER_USER_EXTEND_TEXT,
				(userExtendSetting.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT)
					? tbUserExtendText.Text.Trim()
					: ddlUserExtendText.SelectedValue);
			htSearch.Add(Constants.REQUEST_KEY_USER_USER_EXTEND_TYPE, userExtendSetting.InputType);
		}

		htSearch.Add(Constants.REQUEST_KEY_USER_COUNTRY_ISO_CODE, (Constants.GLOBAL_OPTION_ENABLE) ? ddlCountryIsoCode.SelectedValue : "");// グローバル対応:国ISOコード

		return htSearch;
	}

	/// <summary>
	/// データバインド用ユーザ詳細URL作成
	/// </summary>
	/// <param name="strUserId">ユーザID</param>
	/// <returns>ユーザ詳細URL</returns>
	public static string CreateUserDetailUrl(string strUserId)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(strUserId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbUrl.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// ユーザー一覧へ
		Response.Redirect(CreateUserListUrl(GetSearchInfo(), 1));
	}

	/// <summary>
	/// 新規ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		//ユーザー拡張項目セッションをクリア
		Session[Constants.SESSION_KEY_PARAM + "_extend"] = null;
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// Button default setting click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDefaultSettingTop_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_DEFAULTSETTING;

		// Url to user default setting
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DEFAULTSETTING)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// ユーザー拡張項目選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserExtend_SelectedIndexChanged(object sender, EventArgs e)
	{
		InitializeUserExtendText("", "");
	}

	/// <summary>
	/// e-SCOTT会員削除出力クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDeleteEScottKaiin_Click(object sender, System.EventArgs e)
	{
		// 検索情報をセッションに保存
		Session[Constants.SESSION_KEY_PARAM] = CreateSearchParams();

		// ファイル出力ページに遷移
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USERFILEEXPORT).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// ユーザー拡張項目テキストの設定
	/// </summary>
	/// <param name="defaultFlg">ありなしフラグ</param>
	/// <param name="defaultText">検索値</param>
	private void InitializeUserExtendText(string defaultFlg, string defaultText)
	{
		ddlUserExtendText.Items.Clear();
		tbUserExtendText.Text = "";
		ddlUserExtendFlg.Visible = false;
		tbUserExtendText.Visible = false;
		ddlUserExtendText.Visible = false;
		if (string.IsNullOrEmpty(ddlUserExtendName.SelectedValue)) return;

		ddlUserExtendFlg.Visible = true;
		ddlUserExtendFlg.SelectedValue = defaultFlg;
		var userExtendSetting =
			this.UserExtendSettings.Items.Find(
			userExtend => userExtend.SettingId == ddlUserExtendName.SelectedValue);
		// 拡張項目の設定方法がテキストボックスの場合は検索用テキストボックスを表示する。
		// そうではない場合は検索用ドロップダウンリストを表示する
		if (userExtendSetting.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT)
		{
			tbUserExtendText.Visible = true;
			tbUserExtendText.Text = defaultText;
		}
		else
		{
			ddlUserExtendText.Visible = true;
			ddlUserExtendText.Items.Add(new ListItem("", ""));
			AddItemToDropDownList(
				ddlUserExtendText,
				((List<ListItem>)userExtendSetting.ListItem),
				defaultText);
		}
	}

	/// <summary>
	/// ドロップダウンリストにアイテム追加
	/// </summary>
	/// <param name="ddl">追加先</param>
	/// <param name="listItems">リストアイテム列</param>
	/// <param name="defaultValue">デフォルト値（指定無し：null）</param>
	private void AddItemToDropDownList(
		DropDownList ddl,
		IEnumerable<ListItem> listItems,
		string defaultValue)
	{
		foreach (var li in listItems)
		{
			li.Selected = (li.Value == defaultValue);
			ddl.Items.Add(li);
		}
	}

	/// <summary>
	/// ユーザー管理レベル名取得
	/// </summary>
	/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
	/// <returns>ユーザー管理レベル名</returns>
	protected string GetUserManagementLevelName(string userManagementLevelId)
	{
		return UserManagementLevelUtility.GetUserManagementLevelName(userManagementLevelId);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParameters(Request));
	}

	/// <summary>
	/// Set Menu Authority
	/// </summary>
	private void SetMenuAuthority()
	{
		var canUseDefaultSetting = MenuUtility.HasAuthority(
			this.LoginOperatorMenu,
			this.RawUrl,
			Constants.KBN_MENU_FUNCTION_USER_DEFAULT_SETTINGS_EDIT);
		btnDefaultSettingTop.Visible = canUseDefaultSetting;
	}

	/// <summary>ユーザー拡張項目リスト</summary>
	private UserExtendSettingList UserExtendSettings
	{
		get { return (UserExtendSettingList)ViewState["userExtendSettingList"]; }
		set { ViewState["userExtendSettingList"] = value; }
	}
}
