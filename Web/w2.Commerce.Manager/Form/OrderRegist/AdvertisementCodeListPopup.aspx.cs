/*
=========================================================================================================
  Module      : 広告コード一覧ページ処理(AdvertisementCodeListPopup.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;

public partial class Form_AdvertisementCode_AdvertisementCodeList : BasePage
{
	protected int m_pageNumber = 1;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			// 検索ワード
			string advertisementCode = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE]);
			string mediaName = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME]);

			// ページ番号（ページャ動作時のみもちまわる）
			if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out m_pageNumber) == false)
			{
				m_pageNumber = 1;
			}
			ViewState[Constants.REQUEST_KEY_PAGE_NO] = m_pageNumber;

			//------------------------------------------------------
			// 一覧取得
			//------------------------------------------------------
			int totalCounts = 0;	// ページング可能総商品数
			var advCodeService = new AdvCodeService();

			// Get list Advertisement code media type
			var advCodeMediaType = advCodeService.GetAdvCodeMediaTypeListAll();

			ddlAdvCodeMediaType.Items.Add(new ListItem("", ""));
			foreach (AdvCodeMediaTypeModel item in advCodeMediaType)
			{
				ddlAdvCodeMediaType.Items.Add(new ListItem(item.AdvcodeMediaTypeName, item.AdvcodeMediaTypeId));
			}

			foreach (ListItem item in ddlAdvCodeMediaType.Items)
			{
				item.Selected = (item.Value == (string)Request[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]);
			}

			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]) != string.Empty)
			{
				foreach (ListItem item in ddlSearchSortKbn.Items)
				{
					item.Selected = (item.Value == StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]));
				}
			}

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable parameters = GetParameters(Request);

			var searchCondition = CreateSearchCondition(parameters);
			var listAdv = advCodeService.SearchAdvCode(searchCondition);
			totalCounts = advCodeService.GetAdvCodeSearchHitCount(searchCondition);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			// 値セット
			tbAdvCode.Text = advertisementCode;
			tbMediaName.Text = mediaName;

			// 画面制御
			if (listAdv.Length != 0)
			{
				trListError.Visible = false;
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

				totalCounts = 0;
			}

			// データセット
			rEditList.DataSource = listAdv;
			rEditList.DataBind();

			divEdit.Visible = true;

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string nextUrl = CreateListUrl(advertisementCode, mediaName, ddlAdvCodeMediaType.SelectedValue, ddlSearchSortKbn.SelectedValue);
			lbPager1.Text = WebPager.CreateDefaultListPager(totalCounts, m_pageNumber, nextUrl);
		}
		else
		{
			m_pageNumber = (int)ViewState[Constants.REQUEST_KEY_PAGE_NO];
		}
	}

	/// <summary>
	/// Get Parameters
	/// </summary>
	/// <param name="requestParams">Request Params</param>
	/// <returns>Request params</returns>
	protected Hashtable GetParameters(HttpRequest requestParams)
	{
		Hashtable resultData = new Hashtable();
		int currentPageNumber;

		//------------------------------------------------------
		// Get parameters
		//------------------------------------------------------
		resultData.Add(Constants.FIELD_ADVCODE_DEPT_ID, this.LoginOperatorDeptId);
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE]));
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME]));
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]));
		resultData.Add(Constants.FIELD_ADVCODE_VALID_FLG, Constants.FLG_ADVCODE_VALID_FLG_VALID);
		resultData.Add(Constants.REQUEST_KEY_SORT_KBN, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_SORT_KBN]));

		// Set current page number
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false)
		{
			currentPageNumber = 1;
		}
		this.m_pageNumber = currentPageNumber;
		resultData.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);

		return resultData;
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>広告コード検索条件</returns>
	private AdvCodeListSearchCondition CreateSearchCondition(Hashtable parameters)
	{
		// 条件作成
		var condition = new AdvCodeListSearchCondition
		{
			DeptId = (string)parameters[Constants.FIELD_ADVCODE_DEPT_ID],
			AdvertisementCode = (string)parameters[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE],
			MediaName = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME],
			ValidFlg = StringUtility.ToEmpty(parameters[Constants.FIELD_ADVCODE_VALID_FLG]),
			AdvcodeMediaTypeId = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID],
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.m_pageNumber - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.m_pageNumber,
			SortKbn = (string)parameters[Constants.REQUEST_KEY_SORT_KBN],
		};

		return condition;
	}	

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl(tbAdvCode.Text, tbMediaName.Text, ddlAdvCodeMediaType.SelectedValue, ddlSearchSortKbn.SelectedValue));
	}

	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <param name="advertisementCode">広告コード</param>
	/// <param name="mediaName">媒体名</param>
	/// <param name="mediaType">Media Type</param>
	/// <returns>一覧URL</returns>
	protected string CreateListUrl(string advertisementCode, string mediaName, string mediaType, string sortKbn)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_REGIST_ADVPOPUP);
		url.Append("?").Append(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE).Append("=").Append(HttpUtility.UrlEncode(advertisementCode));
		url.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME).Append("=").Append(HttpUtility.UrlEncode(mediaName));
		url.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID).Append("=").Append(HttpUtility.UrlEncode(mediaType));
		url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(sortKbn));

		return url.ToString();
	}
}