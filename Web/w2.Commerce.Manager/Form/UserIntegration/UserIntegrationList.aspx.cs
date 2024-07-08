/*
=========================================================================================================
  Module      : ユーザー統合一覧ページ処理(UserIntegrationList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using w2.App.Common.Util;
using w2.Domain.UserIntegration;
using w2.Domain.UserIntegration.Setting;
using w2.Domain.UserIntegration.Helper;

public partial class Form_UserIntegration_UserIntegrationList : UserIntegrationPage
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
			// 設定ファイル読込
			UserIntegrationSetting.UpdateSetting();

			// 初期化
			Initialize();

			// ユーザー統合一覧表示
			DisplayUserIntegrationList();
		}
    }

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		var searchValues = new SearchValues(rblStatus.SelectedValue,
			tbUserIntegrationNo.Text,
			tbUserId.Text,
			tbName.Text,
			tbNameKana.Text,
			tbTel.Text,
			tbMailAddr.Text,
			tbZip.Text,
			tbAddr.Text,
			ddlSortKbn.SelectedValue,
			1);
		Response.Redirect(searchValues.CreateUserIntegrationListUrl());
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		Response.Redirect(CreateUserIntegrationRegisterlUrl("", Constants.ACTION_STATUS_INSERT));
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// ステータス
		var statusCountList = new UserIntegrationService().GetStatusCount();
		foreach (ListItem listItem in
			ValueText.GetValueItemArray(
				Constants.TABLE_USERINTEGRATION,
				Constants.FIELD_USERINTEGRATION_STATUS))
		{
			var count = 0;
			var statusCount = statusCountList.Where(sc => sc.Status == listItem.Value).ToArray();
			if (statusCount.Length != 0)
			{
				count = statusCount.First().Count;
			}
			listItem.Text += WebSanitizer.HtmlEncode(
				string.Format(
					"({0})",
					string.Format(
						ReplaceTag("@@DispText.common_message.unit_of_quantity@@"),
						count)));
			rblStatus.Items.Add(listItem);
		}
		rblStatus.SelectedIndex = 0;

		// 並び順
		ddlSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_USERINTEGRATION, Constants.REQUEST_KEY_SORT_KBN));
	}

	/// <summary>
	/// ユーザー統合一覧表示
	/// </summary>
	private void DisplayUserIntegrationList()
	{
		// 検索フォームにパラメータをセット
		rblStatus.SelectedValue = this.RequestStatus;
		tbUserIntegrationNo.Text = this.RequestUserIntegrationNo;
		tbUserId.Text = this.RequestUserId;
		tbName.Text = this.RequestName;
		tbNameKana.Text = this.RequestNameKana;
		tbTel.Text = this.RequestTel;
		tbMailAddr.Text = this.RequestMailAddr;
		tbZip.Text = this.RequestZip;
		tbAddr.Text = this.RequestAddr;
		ddlSortKbn.SelectedValue = this.RequestSortKbn;

		// ユーザー統合情報一覧取得
		var searchCondition = CreateSearchCondition();
		var service = new UserIntegrationService();
		int totalCount = service.GetSearchHitCount(searchCondition);
		var result = service.Search(searchCondition);
		rList.DataSource = result;
		rList.DataBind();

		// 件数取得、エラー表示制御
		if (totalCount != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ユーザー統合情報一覧検索情報を格納
		this.SearchInfo = new SearchValues(this.RequestStatus,
			this.RequestUserIntegrationNo,
			this.RequestUserId,
			this.RequestName,
			this.RequestNameKana,
			this.RequestTel,
			this.RequestMailAddr,
			this.RequestZip,
			this.RequestAddr,
			this.RequestSortKbn,
			this.RequestPageNum);

		// ページャ作成
		string nextUrl = this.SearchInfo.CreateUserIntegrationListUrl(false);
		lbPager.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <returns>検索条件</returns>
	private UserIntegrationListSearchCondition CreateSearchCondition()
	{
		var result = new UserIntegrationListSearchCondition
		{
			Status = rblStatus.SelectedValue,
			UserId = tbUserId.Text.Trim(),
			Name = tbName.Text.Trim(),
			NameKana = tbNameKana.Text.Trim(),
			Tel = tbTel.Text.Trim(),
			MailAddr = tbMailAddr.Text.Trim(),
			Zip = tbZip.Text.Trim(),
			Addr = tbAddr.Text.Trim(),
			SortKbn = ddlSortKbn.SelectedValue,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum
		};
		// 指定あり？（指定なければ、nullで条件なしとなる）
		if (tbUserIntegrationNo.Text.Trim().Length != 0)
		{
			// パースエラーの場合は0になるため該当データなしとなる
			long userIntegrationNo;
			long.TryParse(tbUserIntegrationNo.Text, out userIntegrationNo);
			result.UserIntegrationNo = userIntegrationNo;
		}

		return result;
	}

	#region +プロパティ
	/// <summary>リクエスト：ステータス</summary>
	private string RequestStatus
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_STATUS]).Trim(); }
	}
	/// <summary>リクエスト：ユーザID</summary>
	private string RequestUserId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_USER_ID]).Trim(); }
	}
	/// <summary>リクエスト：氏名</summary>
	private string RequestName
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_NAME]).Trim(); }
	}
	/// <summary>リクエスト：氏名かな</summary>
	private string RequestNameKana
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_NAME_KANA]).Trim(); }
	}
	/// <summary>リクエスト：電話番号</summary>
	private string RequestTel
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_TEL]).Trim(); }
	}
	/// <summary>リクエスト：メールアドレス</summary>
	private string RequestMailAddr
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_MAIL_ADDR]).Trim(); }
	}
	/// <summary>リクエスト：郵便番号</summary>
	private string RequestZip
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_ZIP]).Trim(); }
	}
	/// <summary>リクエスト：住所</summary>
	private string RequestAddr
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_ADDR]).Trim(); }
	}
		/// <summary>リクエスト：並び順</summary>
	private string RequestSortKbn
	{
		get
		{
			var sortKbn = Constants.KBN_SORT_USERINTEGRATION_LIST_DEFAULT;
			switch (Request[Constants.REQUEST_KEY_SORT_KBN])
			{
				case Constants.KBN_SORT_USERINTEGRATION_LIST_USER_INTEGRATION_NO_ASC:
				case Constants.KBN_SORT_USERINTEGRATION_LIST_USER_INTEGRATION_NO_DESC:
				case Constants.KBN_SORT_USERINTEGRATION_LIST_DATE_CREATED_ASC:
				case Constants.KBN_SORT_USERINTEGRATION_LIST_DATE_CREATED_DESC:
				case Constants.KBN_SORT_USERINTEGRATION_LIST_DATE_CHANGED_ASC:
				case Constants.KBN_SORT_USERINTEGRATION_LIST_DATE_CHANGED_DESC:
					sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN];
					break;
			}
			return sortKbn;
		}
	}
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	#endregion
}