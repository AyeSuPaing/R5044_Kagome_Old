/*
=========================================================================================================
  Module      : Advertisement Code Media Type (AdvertisementCodeMediaType.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using w2.App.Common.Input;
using w2.Common.Logger;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;
using Input.AdvCode;

public partial class Form_AdvertisementCode_AdvertisementCodeMediaType : BasePage
{
	protected int m_pageNumber = 1;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			if (Constants.W2MP_AFFILIATE_OPTION_ENABLED == false)
			{
				GoToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));
			}

			// ページ番号（ページャ動作時のみもちまわる）
			if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out m_pageNumber) == false)
			{
				m_pageNumber = 1;
			}
			ViewState[Constants.REQUEST_KEY_PAGE_NO] = m_pageNumber;

			// 検索パラメータ取得
			Hashtable parameters = GetParameters();

			// 検索値を画面にセット
			SetParameters(parameters);

			var service = new AdvCodeService();
			var searchCondition = CreateSearchCondition(parameters);
			var dataList = service.SearchAdvCodeMediaType(searchCondition);
			var totalCounts = service.GetAdvCodeMediaTypeSearchHitCount(searchCondition);

			// 画面制御
			if (dataList.Length != 0)
			{
				trListError.Visible = false;
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

				totalCounts = 0;

				// 更新できないようにする
				btnUpdateTop.Visible = false;
				btnUpdateBottom.Visible = false;
			}

			// 完了表示
			if ((string)Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COMPLETE)
			{
				// 処理結果取得
				List<AdvCodeMediaTypeModel> updateResult = null;
				if (Session[Constants.SESSION_KEY_PARAM] is List<AdvCodeMediaTypeModel>)
				{
					updateResult = (List<AdvCodeMediaTypeModel>)Session[Constants.SESSION_KEY_PARAM];
				}
				else
				{
					updateResult = new List<AdvCodeMediaTypeModel>();
				}

				// データセット
				rComplete.DataSource = updateResult;
				rComplete.DataBind();

				divEdit.Visible = false;
				divComplete.Visible = true;
				trList.Visible = false;
				tbdyAddShortUrl.Visible = false;
			}
			// 編集表示
			else
			{
				// データセット
				rEditList.DataSource = dataList;
				rEditList.DataBind();

				divEdit.Visible = true;
				divComplete.Visible = false;
				tbdyAddShortUrl.Visible = true;
			}

			string nextUrl = CreateListUrl((string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID], (string)parameters[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME], (string)parameters[Constants.REQUEST_KEY_SORT_KBN]);
			lbPager.Text = WebPager.CreateDefaultListPager(totalCounts, m_pageNumber, nextUrl);

			//------------------------------------------------------
			// エンターキーでのSubmitを無効とする領域を設定する
			// ※RepeaterがBindされていないと正常に動作しない
			//------------------------------------------------------
			List<Control> targetControls = new List<Control>();
			targetControls.Add(divEdit);		// 編集部分のdiv
			targetControls.Add(divAddNew);		// 追加部分のdiv

			// KeyEventをキャンセルするスクリプトを設定
			new InnerTextBoxList(targetControls).SetKeyPressEventCancelEnterKey();
		}
		else
		{
			m_pageNumber = (int)ViewState[Constants.REQUEST_KEY_PAGE_NO];
		}
	}

	/// <summary>
	/// 検索パラメータ取得
	/// </summary>
	/// <returns>検索パラメータ</returns>
	private Hashtable GetParameters()
	{
		Hashtable parameters = new Hashtable();

		// 区分ID
		parameters.Add(Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]));
		// 媒体区分名
		parameters.Add(Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME]));
		// 並び順
		switch (StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DATE_CREATED_ASC:				// 登録日/昇順
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DATE_CREATED_DESC:				// 登録日/降順
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID_ASC:		// 区分ID/昇順
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID_DESC:		// 区分ID/降順
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
				break;

			default:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DEFAULT);
				break;
		}

		// ページ番号
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out m_pageNumber) == false)
		{
			m_pageNumber = 1;
		}
		parameters.Add(Constants.REQUEST_KEY_PAGE_NO, m_pageNumber);

		return parameters;
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>広告コード検索条件</returns>
	private AdvCodeMediaTypeListSearchCondition CreateSearchCondition(Hashtable parameters)
	{
		// 条件作成
		var condition = new AdvCodeMediaTypeListSearchCondition
		{
			AdvcodeMediaTypeId = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID],
			AdvcodeMediaTypeName = (string)parameters[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME],
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.m_pageNumber - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.m_pageNumber,
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
		//------------------------------------------------------
		// 画面制御
		//------------------------------------------------------
		// 値セット
		tbSearchAdvCodeMediaTypeId.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID];
		tbSearchAdvCodeMediaTypeName.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME];
		foreach (ListItem item in ddlSearchSortKbn.Items)
		{
			item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_SORT_KBN]);
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl(tbSearchAdvCodeMediaTypeId.Text, tbSearchAdvCodeMediaTypeName.Text, ddlSearchSortKbn.SelectedValue));
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		int targetIndex = int.Parse(((Button)sender).CommandArgument);
		string advCodeMediaTypeId = ((HiddenField)rEditList.Items[targetIndex].FindControl("hfAdvCodeMediaTypeId")).Value;
		int numOfAdvCode = new AdvCodeService().GetAdvCodeFromAdvcodeMediaTypeId(advCodeMediaTypeId);

		// Check if AdvCode Media Type is set to AdvCode then it can't be deleted
		if (numOfAdvCode > 0)
		{
			GoToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_DELETE_ERROR).Replace("@@ 1 @@", advCodeMediaTypeId).Replace("@@ 2 @@", numOfAdvCode.ToString()));
		}

		//------------------------------------------------------
		// 削除
		//------------------------------------------------------
		new AdvCodeService().DeleteAdvCodeMediaType(advCodeMediaTypeId);

		Response.Redirect(CreateListUrl(tbSearchAdvCodeMediaTypeId.Text,
										tbSearchAdvCodeMediaTypeName.Text,
										ddlSearchSortKbn.SelectedValue,
										Constants.ACTION_STATUS_UPDATE,
										m_pageNumber,
										true));
	}

	/// <summary>
	/// 一括更新ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTop_Click(object sender, EventArgs e)
	{
		var errorMessages = new StringBuilder();
		var listModels = new List<AdvCodeMediaTypeModel>();

		// リピータアイテムに対して繰り返し
		foreach (RepeaterItem item in rEditList.Items)
		{
			string advCodeMediaTypeName = ((TextBox)item.FindControl("tbAdvCodeMediaTypeName")).Text.Trim();
			string advCodeMediaTypeNameBackup = ((HiddenField)item.FindControl("hfAdvCodeMediaTypeName")).Value;
			string advCodeDisplayOrder = ((TextBox)item.FindControl("tbAdvCodeMediaTypeDisplayOrder")).Text.Trim();
			string advCodeDisplayOrderBackup = ((HiddenField)item.FindControl("hfAdvCodeMediaTypeDisplayOrder")).Value;

			if ((advCodeMediaTypeName != advCodeMediaTypeNameBackup)
				|| (advCodeDisplayOrder != advCodeDisplayOrderBackup))
			{
				var advMediaTypeId = ((HiddenField)item.FindControl("hfAdvCodeMediaTypeId")).Value;
				var input = GetInputData(advMediaTypeId, advCodeMediaTypeName, advCodeDisplayOrder);
				var validateMessages = input.Validate(Constants.ACTION_STATUS_UPDATE);

				if (string.IsNullOrEmpty(validateMessages)) listModels.Add(input.CreateModel());
				errorMessages.Append(validateMessages.Replace("@@ 1 @@", advMediaTypeId));
			}
		}

		// エラーページへ
		if (errorMessages.Length != 0) GoToErrorPage(errorMessages.ToString());

		if ((listModels.Count == 0) || (UpdateAdvertisementCodeMediaType(listModels) == false))
		{
			// エラーページへ
			GoToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_SELECTED_ERROR));
		}

		// 結果
		Session[Constants.SESSION_KEY_PARAM] = listModels;

		// To Finish state
		Response.Redirect(CreateListUrl(tbSearchAdvCodeMediaTypeId.Text,
										tbSearchAdvCodeMediaTypeName.Text,
										ddlSearchSortKbn.SelectedValue,
										Constants.ACTION_STATUS_COMPLETE,
										1,
										true));
	}

	/// <summary>
	/// 編集画面トップへ遷移
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btRedirectEditTop_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl(tbSearchAdvCodeMediaTypeId.Text,
										tbSearchAdvCodeMediaTypeName.Text,
										ddlSearchSortKbn.SelectedValue,
										Constants.ACTION_STATUS_UPDATE,
										m_pageNumber,
										false));
	}

	/// <summary>
	/// Create List Url
	/// </summary>
	/// <param name="advCodeMediaTypeId">MediaTypeId</param>
	/// <param name="advCodeMediaTypeName">MediaTypeName</param>
	/// <param name="sortKbn">Sort type</param>
	/// <param name="actionStatus">Action status</param>
	/// <param name="pageNumber">page no</param>
	/// <param name="isReloadParent">if set to <c>true</c> [is reload parent].</param>
	///  <returns>List Url</returns>
	private string CreateListUrl(string advCodeMediaTypeId, string advCodeMediaTypeName, string sortKbn, string actionStatus, int pageNumber, bool isReloadParent)
	{
		var listPageUrl = CreateListUrl(advCodeMediaTypeId, advCodeMediaTypeName, sortKbn);
		listPageUrl += string.Format("&{0}={1}&{2}={3}", Constants.REQUEST_KEY_ACTION_STATUS, actionStatus, Constants.REQUEST_KEY_PAGE_NO, pageNumber);
		if (isReloadParent) listPageUrl += string.Format("&{0}={1}", Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW, Constants.KBN_RELOAD_PARENT_WINDOW_ON);

		return listPageUrl;
	}
	
	/// <summary>
	/// Create List Url
	/// </summary>
	/// <param name="advCodeMediaTypeId">MediaTypeId</param>
	/// <param name="advCodeMediaTypeName">MediaTypeName</param>
	/// <param name="sortKbn">Sort type</param>
	/// <returns>List Url</returns>
	protected string CreateListUrl(string advCodeMediaTypeId, string advCodeMediaTypeName, string sortKbn)
	{
		StringBuilder listUrl = new StringBuilder();
		listUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_MEDIA_TYPE);
		listUrl.Append("?").Append(Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID).Append("=").Append(HttpUtility.UrlEncode(advCodeMediaTypeId));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME).Append("=").Append(HttpUtility.UrlEncode(advCodeMediaTypeName));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(sortKbn));

		return listUrl.ToString();
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		var advCodeMediaTypeInput = GetInputData(tbAddAdvCodeMediaTypeId.Text.Trim(),
												tbAddAdvCodeMediaTypeName.Text.Trim(),
												tbAddAdvCodeMediaTypeDisplayOrder.Text);

		string errorMessage = advCodeMediaTypeInput.Validate(Constants.ACTION_STATUS_INSERT);

		// エラーページへ
		if (errorMessage.Length != 0)
		{
			if (string.IsNullOrEmpty(advCodeMediaTypeInput.AdvcodeMediaTypeId)
				&& string.IsNullOrEmpty(advCodeMediaTypeInput.AdvcodeMediaTypeName))
			{
				trErrorInsert.Visible = true;
				lErrorInsertMessage.Text = errorMessage;

				return;
			}

			GoToErrorPage(errorMessage);
		}

		//------------------------------------------------------
		// インサート
		//------------------------------------------------------
		var model = advCodeMediaTypeInput.CreateModel();

		//------------------------------------------------------
		// 登録
		//------------------------------------------------------
		new AdvCodeService().InsertAdvCodeMediaType(model);

		var listAdd = new List<AdvCodeMediaTypeModel> { model };
		Session[Constants.SESSION_KEY_PARAM] = listAdd;

		// To finish state
		Response.Redirect(CreateListUrl(tbSearchAdvCodeMediaTypeId.Text,
										tbSearchAdvCodeMediaTypeName.Text,
										ddlSearchSortKbn.SelectedValue,
										Constants.ACTION_STATUS_COMPLETE,
										1,
										true));
	}

	/// <summary>
	/// Get the input data.
	/// </summary>
	/// <param name="mediaTypeId">The media type identifier.</param>
	/// <param name="mediaName">Name of the media.</param>
	/// <param name="displayOrder">The display order.</param>
	/// <returns>The AdvCode media type input</returns>
	private AdvCodeMediaTypeInput GetInputData(string mediaTypeId, string mediaName, string displayOrder)
	{
		var input = new AdvCodeMediaTypeInput();

		input.AdvcodeMediaTypeId = mediaTypeId;
		input.AdvcodeMediaTypeName = mediaName;
		input.DisplayOrder = displayOrder;
		input.LastChanged = this.LoginOperatorName;

		return input;
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return CreateSearchCondition(GetParameters()).CreateHashtableParams();
	}

	/// <summary>
	/// DB更新
	/// </summary>
	private bool UpdateAdvertisementCodeMediaType(List<AdvCodeMediaTypeModel> models)
	{
		using (SqlAccessor accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var advCodeService = new AdvCodeService();

			try
			{
				foreach (AdvCodeMediaTypeModel model in models)
				{
					advCodeService.UpdateAdvCodeMediaType(model, accessor);
				}

				accessor.CommitTransaction();
			}
			catch (Exception exception)
			{
				accessor.RollbackTransaction();
				FileLogger.WriteError(exception);

				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Go to error page.
	/// </summary>
	/// <param name="errorMessages">The error messages.</param>
	private void GoToErrorPage(string errorMessages)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
		var errorPage = string.Format("{0}{1}?{2}={3}", Constants.PATH_ROOT, Constants.PAGE_W2MP_MANAGER_ERROR, Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);

		Response.Redirect(errorPage);
	}	
}
	
