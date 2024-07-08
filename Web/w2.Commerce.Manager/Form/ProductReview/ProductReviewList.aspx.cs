/*
=========================================================================================================
  Module      : 商品レビュー一覧ページ処理(ProductReviewList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Option;
using w2.Common.Logger;
using w2.Domain.ProductReview;

public partial class Form_ProductReview_ProductReviewList : BasePage
{
	private Hashtable m_htUpdatedList = new Hashtable();						// 処理結果

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
			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParameters(Request);

			//------------------------------------------------------
			// 検索情報保持(一覧画面へ戻ってくる際に利用)
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_PRODUCTREVIEW_INFO] = htParam;

			//------------------------------------------------------
			// 検索情報取得
			//------------------------------------------------------
			Hashtable htSearch = GetSearchSqlInfo(htParam);
			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

			//------------------------------------------------------
			// レビュー一覧表示
			//------------------------------------------------------
			int iTotalProductReviewCounts = 0;	// ページング可能総商品数

			// 商品情報取得
			DataView dvReview = GetProductReviewDataView(htSearch, iCurrentPageNumber);

			// 商品情報が存在する場合
			if (dvReview.Count != 0)
			{
				iTotalProductReviewCounts = int.Parse(dvReview[0].Row["row_count"].ToString());

				// エラー非表示制御
				trListError.Visible = false;
			}
			else
			{
				iTotalProductReviewCounts = 0;

				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// データソースセット＆データバインド
			rReviewList.DataSource = dvReview;
			rReviewList.DataBind();

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string strNextUrl = CreateProductReviewListUrl(htParam);
			lbPager1.Text = WebPager.CreateProductReviewPager(iTotalProductReviewCounts, iCurrentPageNumber, strNextUrl);
		}
	}

	/// <summary>
	/// 商品一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();

		// 商品ID
		tbReviewProductId.Text = hrRequest[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID];
		htResult.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID, tbReviewProductId.Text);

		// 商品名
		tbProductName.Text = hrRequest[Constants.REQUEST_KEY_PRODUCT_NAME];
		htResult.Add(Constants.REQUEST_KEY_PRODUCT_NAME, tbProductName.Text);

		// ニックネーム
		tbNickname.Text = hrRequest[Constants.REQUEST_KEY_PRODUCTREVIEW_NICK_NAME];
		htResult.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_NICK_NAME, tbNickname.Text);
		
		// タイトル
		tbReviewTitle.Text = hrRequest[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_TITLE];
		htResult.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_TITLE, tbReviewTitle.Text);
		
		// コメント
		tbReviewComment.Text = hrRequest[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_COMMENT];
		htResult.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_COMMENT, tbReviewComment.Text);

		// 公開フラグ
		foreach (ListItem li in rblOpenFlg.Items)
		{
			li.Selected = (li.Value == StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCTREVIEW_OPEN_FLG]));
		}
		htResult.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_OPEN_FLG, rblOpenFlg.SelectedValue);
		
		// チェックフラグ
		foreach (ListItem li in rblCheckFlg.Items)
		{
			li.Selected = (li.Value == StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCTREVIEW_CHECKED_FLG]));
		}
		htResult.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_CHECKED_FLG, rblCheckFlg.SelectedValue);

		// ソート順
		switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_PRODUCTREVIEW_LIST_DATE_CREATED_ASC:		// 投稿日/昇順
			case Constants.KBN_SORT_PRODUCTREVIEW_LIST_DATE_CREATED_DESC:		// 投稿日/降順
			case Constants.KBN_SORT_PRODUCTREVIEW_LIST_DATE_OPENED_ASC:		// 公開日/昇順
			case Constants.KBN_SORT_PRODUCTREVIEW_LIST_DATE_OPENED_DESC:	// 公開日/降順
			case Constants.KBN_SORT_PRODUCTREVIEW_LIST_DATE_CHECKED_ASC:	// チェック日/昇順
			case Constants.KBN_SORT_PRODUCTREVIEW_LIST_DATE_CHECKED_DESC:	// チェック日/降順
				foreach (ListItem li in ddlSortKbn.Items)
				{
					li.Selected = (li.Value == hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString());
				}
				break;
			default:
				foreach (ListItem li in ddlSortKbn.Items)
				{
					li.Selected = (li.Value == Constants.KBN_SORT_PRODUCTREVIEW_LIST_DEFAULT);	// 公開日/昇順がデフォルト
				}
				break;
		}
		htResult.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);

		// ページ番号（ページャ動作時のみもちまわる）
		int iCurrentPageNumber;
		if (int.TryParse(hrRequest[Constants.REQUEST_KEY_PAGE_NO], out iCurrentPageNumber) == false)
		{
			iCurrentPageNumber = 1;
		}
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);

		return htResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htSearch)
	{
		Hashtable htResult = new Hashtable();
		// 店舗ID
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_SHOP_ID, this.LoginOperatorShopId);
		// 商品ID
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID]));
		// 商品名
		htResult.Add(Constants.FIELD_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_PRODUCT_NAME]));
		// ユーザID
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_USER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_PRODUCTREVIEW_USER_ID]));
		// ニックネーム
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_NICK_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_PRODUCTREVIEW_NICK_NAME]));
		// タイトル
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_TITLE]));
		// コメント
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_COMMENT]));
		// 公開フラグ
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_OPEN_FLG, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_PRODUCTREVIEW_OPEN_FLG]));
		// チェックフラグ
		htResult.Add(Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_PRODUCTREVIEW_CHECKED_FLG]));
		// ソート区分
		htResult.Add("sort_kbn", (string)htSearch[Constants.REQUEST_KEY_SORT_KBN]);

		return htResult;
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Hashtable htParam = new Hashtable();
		htParam.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID, tbReviewProductId.Text.Trim());		// 商品ID
		htParam.Add(Constants.REQUEST_KEY_PRODUCT_NAME, tbProductName.Text.Trim());						// 商品名
		htParam.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_NICK_NAME, tbNickname.Text.Trim());				// ニックネーム
		htParam.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_TITLE, tbReviewTitle.Text.Trim());		// タイトル
		htParam.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_COMMENT, tbReviewComment.Text.Trim());	// 本文
		htParam.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_OPEN_FLG, rblOpenFlg.SelectedValue);			// 公開フラグ
		htParam.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_CHECKED_FLG, rblCheckFlg.SelectedValue);		// チェックフラグ
		htParam.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);							// ソート区分

		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateProductReviewListUrl(htParam, 1));
	}

	/// <summary>
	/// 商品レビュー一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <returns>商品レビュー一覧遷移URL</returns>
	private static string CreateProductReviewListUrl(Hashtable htParam)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTREVIEW_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCT_NAME]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_NICK_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTREVIEW_NICK_NAME]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_TITLE).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_TITLE]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_COMMENT).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_COMMENT]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_OPEN_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTREVIEW_OPEN_FLG]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_CHECKED_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTREVIEW_CHECKED_FLG]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_SORT_KBN]));

		return sbResult.ToString();
	}

	/// <summary>
	/// 商品レビュー一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品レビュー一覧遷移URL</returns>
	public static string CreateProductReviewListUrl(Hashtable htParam, int iPageNumber)
	{
		return CreateProductReviewListUrl(htParam) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();
	}

	/// <summary>
	/// 商品レビューデータビューを表示分だけ取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品レビューデータビュー</returns>
	private DataView GetProductReviewDataView(Hashtable htSearch, int iPageNumber)
	{
		DataView dvResult = null;
		
		// ステートメントからカテゴリデータ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "GetProductReview"))
		{
			htSearch.Add("bgn_row_num",  Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST * (iPageNumber - 1) + 1);																						// 表示開始記事番号
			htSearch.Add("end_row_num", Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST * iPageNumber);																						// 表示開始記事番号

			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSearch);
		}

		return dvResult;
	}

	/// <summary>
	/// データバインド用商品レビュー詳細URL作成
	/// </summary>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strReviewNo">商品ID</param>
	/// <returns>ユーザ詳細URL</returns>
	public static string CreateProductReviewDetailUrl(string strProductId, string strReviewNo)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTREVIEW_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(strProductId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO).Append("=").Append(HttpUtility.UrlEncode(strReviewNo));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbUrl.ToString();
	}

	/// <summary>
	/// データバインド用ユーザ詳細URL作成
	/// </summary>
	/// <param name="strUserId">ユーザID</param>
	/// <returns>ユーザ詳細URL</returns>
	public static string CreateUserDetailUrl(string strUserId)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_CONFIRM_POPUP);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(strUserId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbUrl.ToString();
	}

	/// <summary>
	/// 状態更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateStatus_Click(object sender, EventArgs e)
	{
		// 入力チェック
		// 状態変更指定未選択の場合エラーページへ
		if ((ddlUpdateOpenFlg.SelectedValue == "") && (ddlUpdateCheckFlg.SelectedValue == ""))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTREVIEW_CHECK_UPDATE_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// チェックボックス未選択の場合エラーページへ
		int iUpdateStatusCount = 0;
		foreach (RepeaterItem ri in rReviewList.Items)
		{
			iUpdateStatusCount += (((CheckBox)ri.FindControl("cbCheckedList")).Checked) ? 1 : 0;
		}
		if (iUpdateStatusCount == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTREVIEW_CHECK_UPDATE_NO_CHECKED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// ステータス更新
		StringBuilder sbUpdateListWhere = new StringBuilder();

		// 表示件数分ループ
		foreach (RepeaterItem ri in rReviewList.Items)
		{
			var productId = ((HiddenField)ri.FindControl("hfProductReviewProductId")).Value;
			var reviewNo = ((HiddenField)ri.FindControl("hfProductReviewReviewNo")).Value;

			// チェックボックス判定
			if (((CheckBox)(ri.FindControl("cbCheckedList"))).Checked)
			{
				using (var sqlAccessor = new SqlAccessor())
				{
					try
					{
						sqlAccessor.OpenConnection();
						sqlAccessor.BeginTransaction();

						var model = GetInputProductReviewModel(productId, reviewNo, sqlAccessor);
						var iResult = new ProductReviewService().Update(model, sqlAccessor);

						// where条件用文字列を作成
						m_htUpdatedList.Add(model.ShopId + " " + model.ProductId + " " + model.ReivewNo, (iResult > 0) ? "1" : "0");
						
						// レビュー投稿ポイント付与
						if (PointOptionUtility.CanAddReviewPoint(model))
						{
							var errorMessages = PointOptionUtility.AddReviewPoint(model, this.LoginOperatorId, sqlAccessor);
							if (string.IsNullOrEmpty(errorMessages) == false)
							{
								sqlAccessor.RollbackTransaction();

								Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
								Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
							}
						}

						sqlAccessor.CommitTransaction();
					}
					catch (Exception ex)
					{
						FileLogger.WriteError(ex);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
				}

				// 初回のみAND条件
				sbUpdateListWhere.Append((sbUpdateListWhere.Length == 0) ? "AND (" : " OR ");
				sbUpdateListWhere.Append("(");
				sbUpdateListWhere.Append(Constants.TABLE_PRODUCTREVIEW).Append(".").Append(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID).Append("='").Append(productId);
				sbUpdateListWhere.Append("' AND ");
				sbUpdateListWhere.Append(Constants.TABLE_PRODUCTREVIEW).Append(".").Append(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO).Append("='").Append(reviewNo);
				sbUpdateListWhere.Append("')");
			}
		}

		// 条件式終了
		sbUpdateListWhere.Append((sbUpdateListWhere.Length == 0) ? "" :")");

		// 更新されたレビュー一覧表示
		DataView dvUpdateList;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "GetProductReviewComplete"))
		{
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where @@", sbUpdateListWhere.ToString());

			Hashtable htParam = new Hashtable();
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_SHOP_ID, this.LoginOperatorShopId);
			htParam.Add("sort_kbn", ((Hashtable)(Session[Constants.SESSIONPARAM_KEY_PRODUCTREVIEW_INFO]))[Constants.REQUEST_KEY_SORT_KBN]);

			dvUpdateList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htParam);
		}

		// データソースセット＆データバインド
		rReviewListComplete.DataSource = dvUpdateList;
		rReviewListComplete.DataBind();
		
		// 表示制御
		trListTitle.Visible = false;
		divProductReview.Visible = false;
		divProductReviewComplete.Visible = true;
	}

	/// <summary>
	/// 商品レビューモデル作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="reviewNo">レビュー番号</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>商品レビューモデル</returns>
	private ProductReviewModel GetInputProductReviewModel(string productId, string reviewNo, SqlAccessor accessor)
	{
		var model = new ProductReviewService().Get(this.LoginOperatorShopId, productId, reviewNo, accessor);

		model.LastChanged = this.LoginOperatorName;
		model.OpenFlg = ddlUpdateOpenFlg.SelectedValue;
		model.DateOpened = (ddlUpdateOpenFlg.SelectedValue == Constants.FLG_PRODUCTREVIEW_OPEN_FLG_VALID)
			? DateTime.Now
			: (DateTime?)null;
		model.CheckedFlg = ddlUpdateCheckFlg.SelectedValue;
		model.DateChecked = (ddlUpdateCheckFlg.SelectedValue == Constants.FLG_PRODUCTREVIEW_CHECK_FLG_VALID)
			? DateTime.Now
			: (DateTime?)null;

		return model;
	}

	/// <summary>
	/// 状態更新結果取得
	/// </summary>
	/// <param name="iResultIndex">更新行番号</param>
	/// <returns>状態更新結果</returns>
	protected string GetUpdateStatusResult(string strShopId, string strProductId, int iReviewNo)
	{
		return ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, "status_change_result", m_htUpdatedList[strShopId + " " + strProductId + " " + iReviewNo]);
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRedirectReview_Click(object sender, EventArgs e)
	{
		// 同じ画面にリダイレクト
		Response.Redirect(CreateProductReviewListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTREVIEW_INFO], 1));
	}

	/// <summary>
	/// 選択行削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteReview_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		// チェックボックス未選択の場合エラーページへ
		int iUpdateStatusCount = 0;
		foreach (RepeaterItem ri in rReviewList.Items)
		{
			iUpdateStatusCount += (((CheckBox)ri.FindControl("cbCheckedList")).Checked) ? 1 : 0;
		}
		if (iUpdateStatusCount == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTREVIEW_CHECK_UPDATE_NO_CHECKED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 選択行削除処理
		//------------------------------------------------------
		foreach (RepeaterItem ri in rReviewList.Items)
		{
			if (((CheckBox)(ri.FindControl("cbCheckedList"))).Checked)
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "DeleteProductReview"))
				{
					Hashtable htParam = new Hashtable();
					htParam.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, (((HiddenField)ri.FindControl("hfProductReviewProductId")).Value));
					htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, (((HiddenField)ri.FindControl("hfProductReviewReviewNo")).Value));

					int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htParam);
				}
			}
		}

		// 画面再表示
		Response.Redirect(CreateProductReviewListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTREVIEW_INFO], 1));
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParameters(Request)); ;
	}
}
