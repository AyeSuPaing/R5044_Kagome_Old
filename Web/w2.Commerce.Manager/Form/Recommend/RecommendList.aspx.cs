/*
=========================================================================================================
  Module      : レコメンド設定一覧ページ処理(RecommendList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.RefreshFileManager;
using w2.Domain.Recommend;
using w2.Domain.Recommend.Helper;

public partial class Form_Recommend_RecommendList : RecommendPage
{
	#region 定数
	/// <summary>開催状態</summary>
	protected const string FIELD_RECOMMEND_STATUS = "status";
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 更新完了メッセージ非表示
		divComp.Visible = false;

		if (!IsPostBack)
		{
			// 表示コンポーネント初期化
			InitializeComponents();

			// レコメンド設定一覧表示
			DisplayRecommendList();
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		var searchValues = new SearchValues(tbRecommendId.Text,
			tbRecommendName.Text,
			ddlRecommendKbn.SelectedValue,
			ddlStatus.SelectedValue,
			ddlSortKbn.SelectedValue,
			1);
		Response.Redirect(searchValues.CreateRecommendListUrl(true, this.RequestWindowKbn));
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var url = CreateRecommendRegisterUrl("", Constants.ACTION_STATUS_INSERT);
		Response.Redirect(url);
	}

	/// <summary>
	/// 一括更新（適用優先順）ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdatePriority_Click(object sender, EventArgs e)
	{
		var isUpdate = false;
		using (var accessor = new SqlAccessor())
		{
			try
			{
				// トランザクション：開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var service = new RecommendService();
				foreach (RepeaterItem item in rList.Items)
				{
					// 適用優先順取得
					var priorityText = ((TextBox)item.FindControl("tbPriority")).Text.Trim();

					// 数値に変換できない場合は更新対象外とする
					int priority;
					if (int.TryParse(priorityText, out priority) == false) continue;

					// 変更前と同じ場合は更新対象外とする
					var recommend = this.SearchResult[item.ItemIndex];
					if (priority == recommend.Priority) continue;

					// 適用優先順更新
					service.UpdatePriority(recommend.ShopId,
						recommend.RecommendId,
						priority,
						this.LoginOperatorName,
						accessor);

					isUpdate = true;
				}

				// トランザクション：コミット
				accessor.CommitTransaction();

				// 各サイトのレコメンド情報を最新の状態にする
				RefreshFileManagerProvider.GetInstance(RefreshFileType.Recommend).CreateUpdateRefreshFile();
			}
			catch (Exception ex)
			{
				// トランザクション：ロールバック
				AppLogger.WriteError(ex.ToString());
				accessor.RollbackTransaction();
			}
		}

		// 更新がなければ処理を抜ける
		if (isUpdate == false) return;

		// 一覧画面へ遷移（更新完了メッセージ表示）
		var url = this.SearchInfo.CreateRecommendListUrl()
			+ string.Format("&{0}=1", REQUEST_KEY_SUCCESS);
		Response.Redirect(url);
	}

	#region メソッド
	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// レコメンド区分
		ddlRecommendKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlRecommendKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_KBN));
		// 開催状態
		ddlStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_STATUS));
		// 並び順
		ddlSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.REQUEST_KEY_SORT_KBN));
		// 更新完了メッセージ
		divComp.Visible = this.IsDisplaySuccess;
	}

	/// <summary>
	/// レコメンド設定一覧表示
	/// </summary>
	private void DisplayRecommendList()
	{
		// 検索フォームにパラメータをセット
		tbRecommendId.Text = this.RequestRecommendId;
		tbRecommendName.Text = this.RequestRecommendName;
		ddlRecommendKbn.SelectedValue = this.RequestRecommendKbn;
		ddlStatus.SelectedValue = this.RequestStatus;
		ddlSortKbn.SelectedValue = this.RequestSortKbn;

		// レコメンド設定一覧取得
		var searchCondition = CreateSearchCondition();
		var service = new RecommendService();
		int totalCount = service.GetSearchHitCount(searchCondition);
		this.SearchResult = service.Search(searchCondition);

		// Redirect to last page when current page no don't have any data
		var searchValues = new SearchValues(
			tbRecommendId.Text,
			tbRecommendName.Text,
			ddlRecommendKbn.SelectedValue,
			ddlStatus.SelectedValue,
			ddlSortKbn.SelectedValue,
			0);

		CheckRedirectToLastPage(
			this.SearchResult.Length,
			totalCount,
			searchValues.CreateRecommendListUrl(false));

		rList.DataSource = this.SearchResult;
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

		// レコメンド設定一覧検索情報を格納
		this.SearchInfo = new SearchValues(this.RequestRecommendId,
			this.RequestRecommendName,
			this.RequestRecommendKbn,
			this.RequestStatus,
			this.RequestSortKbn,
			this.RequestPageNum);

		// ページャ作成
		var nextUrl = this.SearchInfo.CreateRecommendListUrl(false, this.RequestWindowKbn);
		lbPager.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <returns>検索条件</returns>
	private RecommendListSearchCondition CreateSearchCondition()
	{
		return new RecommendListSearchCondition
		{
			ShopId = this.LoginOperatorShopId,
			RecommendId = tbRecommendId.Text.Trim(),
			RecommendName = tbRecommendName.Text.Trim(),
			RecommendKbn = ddlRecommendKbn.SelectedValue,
			Status = ddlStatus.SelectedValue,
			SortKbn = ddlSortKbn.SelectedValue,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum
		};
	}
	#endregion

	/// <summary>
	/// レコメンドレポート（レコメンドID選択）対応：選択されたレコメンドIDを設定するパラメータ作成
	/// </summary>
	/// <param name="recommendId">レコメンドID</param>
	/// <returns>レコメンドIDを設定パラメータ</returns>
	protected string CreateJavaScriptSetRecommendId(string recommendId)
	{
		var jsParam = string.Format("'{0}'", StringUtility.ToEmpty(recommendId).Replace("'", "\\'"));
		return jsParam;
	}

	#region プロパティ
	/// <summary>レコメンド設定一覧検索結果</summary>
	private RecommendListSearchResult[] SearchResult
	{
		get { return (RecommendListSearchResult[])ViewState["SearchResult"]; }
		set { ViewState["SearchResult"] = value; }
	}
	/// <summary>リクエスト：レコメンド名（管理用）</summary>
	private string RequestRecommendName
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RECOMMEND_NAME]).Trim(); }
	}
	/// <summary>リクエスト：レコメンド区分</summary>
	private string RequestRecommendKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RECOMMEND_KBN]).Trim(); }
	}
	/// <summary>リクエスト：開催状態</summary>
	private string RequestStatus
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RECOMMEND_STATUS]).Trim(); }
	}
	/// <summary>リクエスト：並び順</summary>
	private string RequestSortKbn
	{
		get
		{
			var sortKbn = Constants.KBN_SORT_RECOMMEND_LIST_STATUS;
			switch (Request[Constants.REQUEST_KEY_SORT_KBN])
			{
				case Constants.KBN_SORT_RECOMMEND_LIST_STATUS:
				case Constants.KBN_SORT_RECOMMEND_LIST_RECOMMEND_ID_ASC:
				case Constants.KBN_SORT_RECOMMEND_LIST_RECOMMEND_ID_DESC:
				case Constants.KBN_SORT_RECOMMEND_LIST_RECOMMEND_NAME_ASC:
				case Constants.KBN_SORT_RECOMMEND_LIST_RECOMMEND_NAME_DESC:
				case Constants.KBN_SORT_RECOMMEND_LIST_PRIORITY_ASC:
				case Constants.KBN_SORT_RECOMMEND_LIST_PRIORITY_DESC:
				case Constants.KBN_SORT_RECOMMEND_LIST_DATE_BEGIN_ASC:
				case Constants.KBN_SORT_RECOMMEND_LIST_DATE_BEGIN_DESC:
				case Constants.KBN_SORT_RECOMMEND_LIST_DATE_END_ASC:
				case Constants.KBN_SORT_RECOMMEND_LIST_DATE_END_DESC:
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
	/// <summary>リクエスト：ポップアップ画面化</summary>
	private string RequestWindowKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_WINDOW_KBN]).Trim(); }
	}
	/// <summary>レコメンドレポート（レコメンドID選択）対応かどうか</summary>
	protected bool IsDisplayingForRecommendReport
	{
		get { return this.RequestWindowKbn == Constants.KBN_WINDOW_POPUP; }
	}
	#endregion
}
