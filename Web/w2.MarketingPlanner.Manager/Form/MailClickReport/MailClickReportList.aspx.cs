/*
=========================================================================================================
  Module      : メールクリック一覧処理(MailClickReportList.aspx.cs)
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
using w2.Domain.Coupon;

public partial class Form_MailClick_MailClickReportList : BasePage
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
			//------------------------------------------------------
			// リクエスト取得
			//------------------------------------------------------
			var mailTextId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILTEXT_ID]);
			var mailDistId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILDIST_ID]);
			var mailTextName = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILTEXT_NAME]);
			var mailDistName = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILDIST_NAME]);
			var mailClickUrl = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILCLICK_URL]);
			var mailClickKey = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILCLICK_KEY]);
			var pageNo = 1;
			if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageNo) == false)
			{
				pageNo = 1;
			}

			//------------------------------------------------------
			// 一覧取得
			//------------------------------------------------------
			DataView dvList = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("MailClickReport", "GetMailClickList"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId },
					{ Constants.FIELD_MAILCLICK_MAILTEXT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(mailTextId) },
					{ Constants.FIELD_MAILCLICK_MAILDIST_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(mailDistId) },
					{ Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(mailTextName) },
					{ Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(mailDistName) },
					{ Constants.FIELD_MAILCLICK_MAILCLICK_URL + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(mailClickUrl) },
					{ Constants.FIELD_MAILCLICK_MAILCLICK_KEY + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(mailClickKey) },
					{ "bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNo - 1) + 1 },
					{ "end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNo },
				};

				dvList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 画面表示
			//------------------------------------------------------
			var iTotalUserCounts = 0;
			if (dvList.Count != 0)
			{
				trListError.Visible = false;
				iTotalUserCounts = (int)dvList[0].Row["row_count"];
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// クーポン発行スケジュールによるメール送信の場合、クーポン発行スケジュール名を取得
			for (var i = 0; i < dvList.Count; i++)
			{
				var gotMailDistId = (string)dvList[i][Constants.FIELD_MAILCLICK_MAILDIST_ID];

				if (gotMailDistId.Contains(Constants.MAILDIST_ID_PREFIX) == false) continue;

				var couponSchedule = new CouponService()
					.GetCouponSchedule(gotMailDistId.Replace(Constants.MAILDIST_ID_PREFIX, string.Empty));
				dvList[i][Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME] = couponSchedule.CouponScheduleName;
			}

			// 一覧セット
			rList.DataSource = dvList;
			rList.DataBind();

			// 検索ボックスセット
			tbMailTextId.Text = mailTextId;
			tbMailTextName.Text = mailTextName;
			tbMailDistId.Text = mailDistId;
			tbMailDistName.Text = mailDistName;
			tbMailClickKey.Text = mailClickKey;
			tbMailClickUrl.Text = mailClickUrl;

			// ページャセット
			var strNextUrl = CreateListUrl();
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalUserCounts, pageNo, strNextUrl);
		}
	}

	/// <summary>
	/// 一覧URL作成(ページャ用）
	/// </summary>
	/// <returns></returns>
	protected string CreateListUrl()
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILCLICKREPORT_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(HttpUtility.UrlEncode(tbMailTextId.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MAILTEXT_NAME).Append("=").Append(HttpUtility.UrlEncode(tbMailTextName.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(HttpUtility.UrlEncode(tbMailDistId.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_NAME).Append("=").Append(HttpUtility.UrlEncode(tbMailDistName.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MAILCLICK_URL).Append("=").Append(HttpUtility.UrlEncode(tbMailClickUrl.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MAILCLICK_KEY).Append("=").Append(HttpUtility.UrlEncode(tbMailClickKey.Text));

		return sbResult.ToString();
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="mailClickData"></param>
	/// <returns></returns>
	protected string CreateDetailUrl(object mailClickData)
	{
		DataRowView drvMailClickData = (DataRowView)mailClickData;

		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILCLICKREPORT_DETAIL);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(HttpUtility.UrlEncode((string)drvMailClickData[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(HttpUtility.UrlEncode((string)drvMailClickData[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]));

		return sbResult.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 文字列省略
	/// </summary>
	/// <param name="strSrc"></param>
	/// <param name="iLength"></param>
	/// <returns></returns>
	protected string Truncate(string strSrc, int iLength, string strReplace)
	{
		return (strSrc.Length > iLength) ? strSrc.Substring(0, iLength) + strReplace : strSrc;
	}
}
