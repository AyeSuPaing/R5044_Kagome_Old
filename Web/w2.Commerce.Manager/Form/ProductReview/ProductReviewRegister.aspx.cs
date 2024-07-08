/*
=========================================================================================================
  Module      : 商品レビュー登録ページ処理(ProductReviewRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using w2.App.Common.Util;
using w2.Common.Web;

public partial class Form_ProductReview_ProductReviewRegister : BasePage
{
	string m_strProductId = null;
	string m_strReviewNo = null;
	string m_strActionStatus = null;

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
			// マスタページのOnLoad追加
			//------------------------------------------------------
			Master.OnLoadEvents += "open_flg_checkbox_status();";
			Master.OnLoadEvents += "check_flg_checkbox_status();";
			
			//------------------------------------------------------
			// パラメタ取得
			//------------------------------------------------------
			// 商品ID
			m_strProductId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID]);
			ViewState[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID] = m_strProductId;

			// レビュー番号
			m_strReviewNo = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO]);
			ViewState[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO] = m_strReviewNo;

			// アクションステータス
			m_strActionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
			ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = m_strActionStatus;

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent();

			switch (m_strActionStatus)
			{
				// 更新
				case Constants.ACTION_STATUS_UPDATE:

					// 商品レビュー取得
					DataView dvReview = null;
					using (SqlAccessor sqlAccessor = new SqlAccessor())
					using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "GetProductReviewDetail"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, m_strProductId);		// 商品ID
						htInput.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo);		// レビュー番号
						dvReview = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					}

					// 画面セット
					if (dvReview.Count != 0)
					{
						DataRowView drvReview = dvReview[0];
						lProductId.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID]);
						lProductName.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCT_NAME]);

						// 公開日
						cbOpenFlg.Checked = (string)drvReview[Constants.FIELD_PRODUCTREVIEW_OPEN_FLG] != Constants.FLG_PRODUCTREVIEW_OPEN_FLG_INVALID;
						var format = "yyyy/MM/dd HH:mm:ss";
						var openDate = StringUtility.ToDateString(
							drvReview[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED],
							format,
							DateTime.Now.ToString(format));
						ucOpenDatePeriod.SetStartDate(DateTime.Parse(openDate));

						// チェック日
						cbCheckFlg.Checked = (string)drvReview[Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG] != Constants.FLG_PRODUCTREVIEW_CHECK_FLG_INVALID;
						var checkDate = StringUtility.ToDateString(
							drvReview[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED],
							format,
							DateTime.Now.ToString(format));
						ucCheckDatePeriod.SetStartDate(DateTime.Parse(checkDate));
						lUserId.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCTREVIEW_USER_ID]);
						tbNickname.Text = (string)drvReview[Constants.FIELD_PRODUCTREVIEW_NICK_NAME];
						ddlRating.Text = WebSanitizer.HtmlEncode(drvReview[Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING]);
						tbTitle.Text = (string)drvReview[Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE];
						tbComment.Text = (string)drvReview[Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT];
					}
					else
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					break;
			}
			if (this.IsBackFromConfirm
				&& (Session[Constants.SESSION_KEY_PARAM] != null))
			{
				var input = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				DateTime openDate;
				DateTime checkDate;
				if (DateTime.TryParse(StringUtility.ToEmpty(input[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED]), out openDate))
				{
					ucOpenDatePeriod.SetStartDate(openDate);
				}
				if (DateTime.TryParse(StringUtility.ToEmpty(input[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED]), out checkDate))
				{
					ucCheckDatePeriod.SetStartDate(checkDate);
				}
				Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
			}
		}
		else
		{
			m_strProductId = (string)ViewState[Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID];
			m_strReviewNo = (string)ViewState[Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO];
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			// ユーザー情報の更新
			trEdit.Visible = true;

			// レビュー評価
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING))
			{
				ddlRating.Items.Add(li);
			}
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		string strValidator = "ProductReviewModify";

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();

		htInput.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, lProductId.Text);
		htInput.Add(Constants.FIELD_PRODUCT_NAME, lProductName.Text);

		// 公開フラグ
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_OPEN_FLG, cbOpenFlg.Checked ? Constants.FLG_PRODUCTREVIEW_OPEN_FLG_VALID : Constants.FLG_PRODUCTREVIEW_OPEN_FLG_INVALID);

		// 公開日
		if (cbOpenFlg.Checked)
		{
			var openDate = ucOpenDatePeriod.StartDateTimeString;
			htInput.Add(
				Constants.FIELD_PRODUCTREVIEW_DATE_OPENED, openDate);
		}
		else
		{
			htInput.Add(Constants.FIELD_PRODUCTREVIEW_DATE_OPENED, null);
		}

		// チェックフラグ
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG, cbCheckFlg.Checked ? Constants.FLG_PRODUCTREVIEW_CHECK_FLG_VALID : Constants.FLG_PRODUCTREVIEW_CHECK_FLG_INVALID);
		
		// チェック日
		if (cbCheckFlg.Checked)
		{
			var checkDate = ucCheckDatePeriod.StartDateTimeString;
			htInput.Add(
				Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED, checkDate);
		}
		else
		{
			htInput.Add(Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED, null);
		}
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_USER_ID, lUserId.Text);
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_NICK_NAME, tbNickname.Text);
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING, ddlRating.Text);
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE, tbTitle.Text);
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT, tbComment.Text);
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo);
		htInput.Add(Constants.FIELD_PRODUCTREVIEW_LAST_CHANGED, this.LoginOperatorName);

		//--------------------------------------------
		// 入力チェック＆重複チェック
		//--------------------------------------------
		string strErrorMessage = Validator.Validate(strValidator, htInput);
		if (strErrorMessage.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 画面遷移
		Session[Constants.SESSION_KEY_PARAM] = htInput;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = 1;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTREVIEW_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID, m_strProductId)
			.AddParam(Constants.REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO, m_strReviewNo)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

		Response.Redirect(url.CreateUrl());
	}

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] != null); }
	}
}
