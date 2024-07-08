/*
=========================================================================================================
  Module      : 商品レビュー確認ページ処理(ProductReviewConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Text;
using w2.App.Common.Option;
using w2.Domain.ProductReview;
using w2.Common.Logger;

public partial class Form_ProductReview_ProductReviewConfirm : BasePage
{
	private const string REQUEST_KEY_UPDATE = "update";
	private const string REQUEST_KEY_REGIST = "regist";
	private string m_strProductId = null;
	private string m_strReviewNo = null;
	private bool m_blOpenFlg = false;
	private bool m_blCheckFlg = false;

	protected string m_strActionStatus = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		m_strProductId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID]);
		m_strReviewNo = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO]);

		if (!IsPostBack)
		{
			// アクションステータス取得
			string m_strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(m_strActionStatus);

			if (Request[REQUEST_KEY_UPDATE] != null)
			{
				lMessages.Visible = true;
			}

			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			m_strActionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

			m_strProductId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID]);
			ViewState.Add(Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID, m_strProductId);

			// 通常の詳細情報表示
			if (m_strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatements = new SqlStatement("ProductReview", "GetProductReviewDetail"))
				{
					// SQL発行
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, m_strProductId);	// 商品ID
					htInput.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo);		// レビュー番号
					DataView dvReview = sqlStatements.SelectSingleStatementWithOC(sqlAccessor, htInput);

					// 該当データが有りの場合
					if (dvReview.Count != 0)
					{
					    DataRowView drvReview = dvReview[0];
						lProductId.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID]);
						lProductName.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCT_NAME]);
						lOpenFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_OPEN_FLG, (string)drvReview[Constants.FIELD_PRODUCTREVIEW_OPEN_FLG]));
						lOpenDate.Text = WebSanitizer.HtmlEncode(
							DateTimeUtility.ToStringForManager(
								drvReview[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED],
								DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
						lCheckFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, (string)drvReview[Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG]));
						lCheckDate.Text = WebSanitizer.HtmlEncode(
							DateTimeUtility.ToStringForManager(
								drvReview[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED],
								DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
						lUserId.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCTREVIEW_USER_ID]);
						lNickname.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCTREVIEW_NICK_NAME]);
						lRating.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING, (object)drvReview[Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING]));
						lTitle.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE]);
						lComment.Text = WebSanitizer.HtmlEncodeChangeToBr(drvReview[Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT]);

						// 公開ボタン表示
						m_blOpenFlg = (string)drvReview[Constants.FIELD_PRODUCTREVIEW_OPEN_FLG] == Constants.FLG_PRODUCTREVIEW_OPEN_FLG_VALID;
						btnOpenValid.Visible = !m_blOpenFlg;
						btnOpenUnValid.Visible = m_blOpenFlg;

						// チェックボタン表示
						m_blCheckFlg = (string)drvReview[Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG] == Constants.FLG_PRODUCTREVIEW_CHECK_FLG_VALID;
						btnCheckValid.Visible = !m_blCheckFlg;
						btnCheckUnValid.Visible = m_blCheckFlg;
					}
					// 該当データ無しの場合
					else
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
				}
			}
			else if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// ユーザー情報編集
				Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

				lProductId.Text = WebSanitizer.HtmlEncode(htParam[Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID]);
				lProductName.Text = WebSanitizer.HtmlEncode(htParam[Constants.FIELD_PRODUCT_NAME]);
				lOpenFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_OPEN_FLG, (string)htParam[Constants.FIELD_PRODUCTREVIEW_OPEN_FLG]));
				lOpenDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						htParam[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED],
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lCheckFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, (string)htParam[Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG]));
				lCheckDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						htParam[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED],
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lUserId.Text = WebSanitizer.HtmlEncode(htParam[Constants.FIELD_PRODUCTREVIEW_USER_ID]);
				lNickname.Text =  WebSanitizer.HtmlEncode(htParam[Constants.FIELD_PRODUCTREVIEW_NICK_NAME]);
				lRating.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING, (object)htParam[Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING] ));
				lTitle.Text = WebSanitizer.HtmlEncode(htParam[Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE]);
				lComment.Text = WebSanitizer.HtmlEncodeChangeToBr(htParam[Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT]);
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		btnEditTop.Visible = btnEditBottom.Visible = false;
		btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
		btnHistoryBackTop.Visible = btnHistoryBackBottom.Visible = false;
		btnBackListTop.Visible = btnBackListBottom.Visible = false;
		btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
		btnOpenValid.Visible = false;
		btnOpenUnValid.Visible = false;
		btnCheckValid.Visible = false;
		btnCheckUnValid.Visible = false;
		trDetail.Visible = false;
		trConfirm.Visible = false;

		// ユーザ情報更新可否
		var blUpdateEnabled = MenuUtility.HasAuthority(
			this.LoginOperatorMenu,
			Constants.PATH_ROOT_EC + Constants.MENU_PATH_LARGE_USER,
			Constants.KBN_MENU_FUNCTION_USER_UPDATE);

		// 詳細
		if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnBackListTop.Visible = btnBackListBottom.Visible = true;
			trDetail.Visible = true;
			btnEditTop.Visible = btnEditBottom.Visible = blUpdateEnabled;
			btnDeleteTop.Visible = btnDeleteBottom.Visible = blUpdateEnabled;
		}
		// 更新
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnHistoryBackTop.Visible = btnHistoryBackBottom.Visible = true;
			trConfirm.Visible = true;
			btnUpdateTop.Visible = btnUpdateBottom.Visible = blUpdateEnabled;
		}
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTREVIEW_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID).Append("=").Append(Server.UrlEncode(m_strProductId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO).Append("=").Append(Server.UrlEncode(m_strReviewNo));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		using (var sqlAccessor = new SqlAccessor())
		{
			try
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				// レビュー更新
				var model = GetInputProductReviewModel((Hashtable)Session[Constants.SESSION_KEY_PARAM], sqlAccessor);
				new ProductReviewService().Update(model, sqlAccessor);

				// レビュー投稿
				AddReviewPoint(sqlAccessor);

				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTREVIEW_LIST);
	}

	/// <summary>
	/// 更新用モデルの取得
	/// </summary>
	/// <param name="input">入力データ</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns></returns>
	private ProductReviewModel GetInputProductReviewModel(Hashtable input, SqlAccessor accessor)
	{
		var model = new ProductReviewService().Get(
			this.LoginOperatorShopId,
			(string)input[Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID],
			(string)input[Constants.FIELD_PRODUCTREVIEW_REVIEW_NO],
			accessor);

		model.NickName = (string)input[Constants.FIELD_PRODUCTREVIEW_NICK_NAME];
		model.ReviewRating = (string)input[Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING];
		model.ReviewTitle = (string)input[Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE];
		model.ReviewComment = (string)input[Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT];
		model.OpenFlg = (string)input[Constants.FIELD_PRODUCTREVIEW_OPEN_FLG];
		model.CheckedFlg = (string)input[Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG];
		model.DateOpened = (string.IsNullOrEmpty((string)input[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED]) == false)
			? DateTime.Parse((string)input[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED])
			:(DateTime?)null;
		model.DateChecked = (string.IsNullOrEmpty((string)input[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED]) == false)
			? DateTime.Parse((string)input[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED])
			:(DateTime?)null;
		model.LastChanged = this.LoginOperatorName;

		return model;
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "DeleteProductReview"))
		{
			Hashtable htParam = new Hashtable();
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, m_strProductId);
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo);

			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htParam);
		}
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTREVIEW_LIST);
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Hashtable htSearchParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTREVIEW_INFO];

		if (htSearchParam != null)
		{
			Response.Redirect(Form_ProductReview_ProductReviewList.CreateProductReviewListUrl(htSearchParam, (int)htSearchParam[Constants.REQUEST_KEY_PAGE_NO]));
		}
		else
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTREVIEW_LIST);
		}
	}

	/// <summary>
	/// 公開するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOpenValid_Click(object sender, EventArgs e)
	{
		using (var sqlAccessor = new SqlAccessor())
		{
			try
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				// 公開処理
				var service = new ProductReviewService();

				var model = service.Get(this.LoginOperatorShopId, m_strProductId, m_strReviewNo, sqlAccessor);
				model.OpenFlg = Constants.FLG_PRODUCTREVIEW_OPEN_FLG_VALID;
				model.DateOpened = DateTime.Now;
				model.LastChanged = this.LoginOperatorName;

				var iResult = service.Update(model, sqlAccessor);

				// レビュー投稿
				AddReviewPoint(sqlAccessor);

				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
		Response.Redirect(Form_ProductReview_ProductReviewList.CreateProductReviewDetailUrl(m_strProductId, m_strReviewNo));
	}

	/// <summary>
	/// 非公開にするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOpenUnValid_Click(object sender, EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "OpenProductReview"))
		{
			// SQL発行
			Hashtable htParam = new Hashtable();
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, m_strProductId);								// 商品ID
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo);								// レビュー番号
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_OPEN_FLG, Constants.FLG_PRODUCTREVIEW_OPEN_FLG_INVALID);	// 公開フラグ
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_DATE_OPENED, null);										// 公開日
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_LAST_CHANGED, this.LoginOperatorName);

			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htParam);
		}
		Response.Redirect(Form_ProductReview_ProductReviewList.CreateProductReviewDetailUrl(m_strProductId, m_strReviewNo));
	}

	/// <summary>
	/// チェックするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCheckValid_Click(object sender, EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "CheckProductReview"))
		{
			// SQL発行
			Hashtable htParam = new Hashtable();
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, m_strProductId);									// 商品ID
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo);									// レビュー番号
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, Constants.FLG_PRODUCTREVIEW_CHECK_FLG_VALID);	// チェックフラグ
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED, DateTime.Now);									// チェック日
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_LAST_CHANGED, this.LoginOperatorName);

			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htParam);
		}
		Response.Redirect(Form_ProductReview_ProductReviewList.CreateProductReviewDetailUrl(m_strProductId, m_strReviewNo));
	}

	/// <summary>
	/// 未チェックにするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCheckUnValid_Click(object sender, EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "CheckProductReview"))
		{
			// SQL発行
			Hashtable htParam = new Hashtable();
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, m_strProductId);									// 商品ID
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo);									// レビュー番号
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, Constants.FLG_PRODUCTREVIEW_CHECK_FLG_INVALID);	// チェックフラグ
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED, null);											// チェック日
			htParam.Add(Constants.FIELD_PRODUCTREVIEW_LAST_CHANGED, this.LoginOperatorName);

			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htParam);
		}
		Response.Redirect(Form_ProductReview_ProductReviewList.CreateProductReviewDetailUrl(m_strProductId, m_strReviewNo));
	}

	/// <summary>
	/// レビューポイント付与
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	private void AddReviewPoint(SqlAccessor accessor)
	{
		var model = new ProductReviewService().Get(this.LoginOperatorShopId, m_strProductId, m_strReviewNo, accessor);
		if (PointOptionUtility.CanAddReviewPoint(model) == false) return;

		var errorMessages = PointOptionUtility.AddReviewPoint(model, this.LoginOperatorName, accessor);
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			accessor.RollbackTransaction();

			this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}
}
