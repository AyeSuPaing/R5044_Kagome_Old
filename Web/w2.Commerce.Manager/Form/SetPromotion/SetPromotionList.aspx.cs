/*
=========================================================================================================
  Module      : セットプロモーション情報一覧ページ処理(SetPromotionList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
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
using w2.Domain.SetPromotion;
using w2.App.Common.Util;
using w2.Domain.SetPromotion.Helper;

public partial class Form_SetPromotion_SetPromotionList : SetPromotionPage
{
	protected const string FIELD_SETPROMOTION_STATUS = "status";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponents();

			// 検索パラメータ取得、セッションに格納
			Hashtable parameters = GetParameters();
			Session[Constants.SESSIONPARAM_KEY_SETPROMOTION_SEARCH_INFO] = parameters;

			// 検索値を画面にセット
			SetParameters(parameters);

			// 一覧＆件数取得
			var service = new SetPromotionService();
			var searchCondition = CreateSearchCondition(parameters);
			var totalCount = service.GetSearchHitCount(searchCondition);
			var searchResults = service.Search(searchCondition);

			if (searchResults.Length != 0)
			{
				// データバインドsearchResults
				rList.DataSource = searchResults;
				rList.DataBind();

				// ページャセット
				string pageUrl = CreateListUrl(parameters);
				lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.CurrentPageNo, pageUrl);

				trListError.Visible = false;
			}
			else
			{
				CheckRedirectToLastPage(
					searchResults.Length,
					totalCount,
					CreateListUrl(GetSearchInfoFromControl()));

				// 0件だったらエラーメッセージ表示
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				trListError.Visible = true;
			}

			this.SetPromotionIdListOfDisplayedData = searchResults.Select(r => r.SetpromotionId).ToArray();
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl(GetSearchInfoFromControl()));
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateDetailUrl("", Constants.ACTION_STATUS_INSERT));
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 並び順
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_SETPROMOTION, Constants.REQUEST_KEY_SORT_KBN))
		{
			ddlSortKbn.Items.Add(li);
			if (li.Value == Constants.KBN_SORT_SETPROMOTION_LIST_DEFAULT) li.Selected = true;
		}

		// 状態
		ddlStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_SETPROMOTION, FIELD_SETPROMOTION_STATUS))
		{
			ddlStatus.Items.Add(li);
		}

		// 商品カテゴリオプションがONの場合
		if (Constants.PRODUCT_CTEGORY_OPTION_ENABLE)
		{
			tdCategoryId.Visible = true;
			tdCategoryIdTextBox.Visible = true;
		}
	}

	/// <summary>
	/// 検索パラメータ取得
	/// </summary>
	/// <returns>検索パラメータ</returns>
	private Hashtable GetParameters()
	{
		Hashtable parameters = new Hashtable();

		// セットプロモーションID
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID]));
		// セットプロモーション名
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME]));
		// 商品ID
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_PRODUCT_ID]));
		// カテゴリID
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_CATEGORY_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_CATEGORY_ID]));
		// 開催状態
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_STATUS]));
		
		// 開始日時(from,to)
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM]));
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO]));
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM]));
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO]));

		// 終了日時(from,to)
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_END_DATE_FROM,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_FROM]));
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_END_DATE_TO,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_TO]));
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_END_TIME_FROM,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_FROM]));
		parameters.Add(Constants.REQUEST_KEY_SETPROMOTION_END_TIME_TO,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_TO]));

		// 並び順
		switch (StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_SETPROMOTION_LIST_STATUS_ASC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_ID_ASC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_ID_DESC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_NAME_ASC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_NAME_DESC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_BEGIN_DATE_ASC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_BEGIN_DATE_DESC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_END_DATE_ASC:
			case Constants.KBN_SORT_SETPROMOTION_LIST_END_DATE_DESC:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
				break;

			default:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_SETPROMOTION_LIST_DEFAULT);
				break;
		}

		// ページ番号
		int pageNo;
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageNo) == false) 
		{
			pageNo = 1;
		}
		parameters.Add(Constants.REQUEST_KEY_PAGE_NO, pageNo);
		this.CurrentPageNo = pageNo;

		return parameters;
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>セットプロモーション検索条件</returns>
	private SetPromotionListSearchCondition CreateSearchCondition(Hashtable parameters)
	{
		// 開始日時(from)
		var beginDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM]),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM]));

		// 開始日時(to)
		var beginDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO]),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO]));

		// 終了日時(from)
		var endDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_FROM]),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_FROM]));


		// 終了日時(to)
		var endDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_TO]),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_TO]));

		// 条件作成
		var condition = new SetPromotionListSearchCondition
		{
			SetpromotionId = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID],
			SetpromotionName = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME],
			ProductId = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_PRODUCT_ID],
			CategoryId = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_CATEGORY_ID],
			Status = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_STATUS],
			BeginDateFrom = Validator.IsDate(beginDateFrom) ? beginDateFrom : null,
			BeginDateTo = Validator.IsDate(beginDateTo)
				? DateTime.Parse(beginDateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT)
				: null,
			EndDateFrom = Validator.IsDate(endDateFrom) ? endDateFrom : null,
			EndDateTo = Validator.IsDate(endDateTo)
				? DateTime.Parse(endDateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT)
				: null,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo,
			SortKbn = (string)parameters[Constants.REQUEST_KEY_SORT_KBN],
		};
		return condition;
	}

	/// <summary>
	/// 画面に検索値をセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetParameters(Hashtable parameters)
	{
		// セットプロモーションID
		tbSetPromotionId.Text = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID];
		// セットプロモーション名
		tbSetPromotionName.Text = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME];
		// 商品ID
		tbProductId.Text = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_PRODUCT_ID];
		// カテゴリID
		tbCategoryId.Text = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_CATEGORY_ID];
		// 開催状態
		ddlStatus.SelectedValue = (string)parameters[Constants.REQUEST_KEY_SETPROMOTION_STATUS];

		// 開始日時(from,to)
		var ucBeginDateStart = string.Format("{0}{1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM]).Replace("/", string.Empty),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM]).Replace(":", string.Empty));

		var ucBeginDateEnd = string.Format("{0}{1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO]).Replace("/", string.Empty),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO]).Replace(":", string.Empty));

		ucBeginDatePeriod.SetPeriodDate(ucBeginDateStart, ucBeginDateEnd);

		// 終了日時(from,to)
		var ucEndDateStart = string.Format("{0}{1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_FROM]).Replace("/", string.Empty),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_FROM]).Replace(":", string.Empty));

		var ucEndDateEnd = string.Format("{0}{1}",
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_TO]).Replace("/", string.Empty),
			StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_TO]).Replace(":", string.Empty));

		ucEndDatePeriod.SetPeriodDate(ucEndDateStart, ucEndDateEnd);

		// 並び順
		ddlSortKbn.SelectedValue = (string)parameters[Constants.REQUEST_KEY_SORT_KBN];
	}

	/// <summary>
	/// 検索情報を画面から取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		Hashtable searchInfo = new Hashtable();

		// セットプロモーションID
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID, tbSetPromotionId.Text.Trim());
		// セットプロモーション名
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME, tbSetPromotionName.Text.Trim());
		// 商品ID
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_PRODUCT_ID, tbProductId.Text.Trim());
		// カテゴリID
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_CATEGORY_ID, tbCategoryId.Text.Trim());
		// 開催状態
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_STATUS, ddlStatus.SelectedValue);

		// 開始日時(from,to)
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM, ucBeginDatePeriod.HfStartDate.Value);
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO, ucBeginDatePeriod.HfEndDate.Value);
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM, ucBeginDatePeriod.HfStartTime.Value);
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO, ucBeginDatePeriod.HfEndTime.Value);

		// End date period (from,to)
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_END_DATE_FROM, ucEndDatePeriod.HfStartDate.Value);
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_END_DATE_TO, ucEndDatePeriod.HfEndDate.Value);
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_END_TIME_FROM, ucEndDatePeriod.HfStartTime.Value);
		searchInfo.Add(Constants.REQUEST_KEY_SETPROMOTION_END_TIME_TO, ucEndDatePeriod.HfEndTime.Value);

		// 並び順
		searchInfo.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);

		return searchInfo;
	}

	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = this.SetPromotionIdListOfDisplayedData;
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}

	/// <summary>カレントページ番号</summary>
	private int CurrentPageNo { get; set; }
	/// <summary>画面表示されているセットプロモーションIDリスト</summary>
	private string[] SetPromotionIdListOfDisplayedData
	{
		get { return (string[])ViewState["setpromotionid_list_of_displayed_data"]; }
		set { ViewState["setpromotionid_list_of_displayed_data"] = value; }
	}
}