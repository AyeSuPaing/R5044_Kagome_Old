/*
=========================================================================================================
  Module      : メールクリックレポートページ処理(MailClickReportDetail.aspx.cs)
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

public partial class Form_MailClickReport_MailClickReportDetail : BasePage
{
	/// <summary>メール文章ID</summary>
	private string _mailTextId;
	/// <summary>メール配信ID</summary>
	private string _mailDistId;
	/// <summary>クーポン発行スケジュールによるメール送信</summary>
	private bool _isPublishCouponMail;

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
			_mailTextId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILTEXT_ID]);
			_mailDistId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILDIST_ID]);

			// クーポン発行スケジュールによるメール送信か判別
			_isPublishCouponMail = _mailDistId.Contains(Constants.MAILDIST_ID_PREFIX);

			if (_isPublishCouponMail)
			{
				_mailDistId = _mailDistId.Replace(
					Constants.MAILDIST_ID_PREFIX,
					string.Empty);
			}

			ViewState[Constants.REQUEST_KEY_MAILTEXT_ID] = _mailTextId;
			ViewState[Constants.REQUEST_KEY_MAILDIST_ID] = _mailDistId;
			ViewState[Constants.MAILDIST_ID_PREFIX_NAME] = _isPublishCouponMail;

			var actionNo = 0;
			int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_NO]), out actionNo);

			//------------------------------------------------------
			// １－１．メールクリック情報取得
			//------------------------------------------------------
			DataView dvMailClickDetail = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatament = new SqlStatement("MailClickReport", "GetMailClickDetailHead"))
			{
				 var htInput = new Hashtable
				{
					{ Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId },
					{ Constants.FIELD_MAILCLICK_MAILTEXT_ID, _mailTextId },
					{ Constants.FIELD_MAILCLICK_MAILDIST_ID, (_isPublishCouponMail ? Constants.MAILDIST_ID_PREFIX : string.Empty) + _mailDistId },
				};

				dvMailClickDetail = sqlStatament.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// １－２．メールクリック情報画面セット
			//------------------------------------------------------
			//int iActionNo = 0;
			if (dvMailClickDetail.Count > 0)
			{
				lbMailTextId.Text = (string)dvMailClickDetail[0][Constants.FIELD_MAILCLICK_MAILTEXT_ID];
				lbMailDistId.Text = (string)dvMailClickDetail[0][Constants.FIELD_MAILCLICK_MAILDIST_ID];
				lbMailTextName.Text = StringUtility.ToEmpty(dvMailClickDetail[0][Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]);
				lbMailDistName.Text = _isPublishCouponMail
					? new CouponService().GetCouponSchedule(_mailDistId).CouponScheduleName
					: StringUtility.ToEmpty(dvMailClickDetail[0][Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME]);

				// 終端が先頭
				//actionNo = (int)dvMailClickDetail[dvMailClickDetail.Count-1][Constants.FIELD_MAILCLICK_ACTION_NO];
			}

			//------------------------------------------------------
			// ２－１．メール送信情報取得
			//------------------------------------------------------
			DataView dvMailClickDist = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatament = new SqlStatement("MailClickReport", "GetMailClickDistList"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId },
					{ Constants.FIELD_MAILCLICK_MAILTEXT_ID, _mailTextId },
					{ Constants.FIELD_MAILCLICK_MAILDIST_ID, _mailDistId },
					{ Constants.FIELD_TASKSCHEDULE_ACTION_KBN, (_isPublishCouponMail ? "PublishCoupon" : "MailDist") },
					{ Constants.MAILDIST_ID_PREFIX_NAME, (_isPublishCouponMail ? Constants.MAILDIST_ID_PREFIX : string.Empty) },
				};

				dvMailClickDist = sqlStatament.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// ２－２．メール送信情報画面セット
			//------------------------------------------------------
			rListActionNo.DataSource = dvMailClickDist;
			rListActionNo.DataBind();

			if (dvMailClickDist.Count != 0)
			{
				if (actionNo != 0)
				{
					foreach (RepeaterItem ri in rListActionNo.Items)
					{
						if (((HiddenField)ri.FindControl("hfActonNo")).Value == actionNo.ToString())
						{
							((RadioButton)ri.FindControl("rbActionNoSelect")).Checked = true;
							break;
						}
					}
				}
				else
				{
					((RadioButton)rListActionNo.Items[0].FindControl("rbActionNoSelect")).Checked = true;
				}
			}

			//------------------------------------------------------
			// ３．メールクリックレポート取得・画面セット
			//------------------------------------------------------
			DisplayMailClickReportDetail();
		}
		else
		{
			_mailTextId = (string)ViewState[Constants.REQUEST_KEY_MAILTEXT_ID];
			_mailDistId = (string)ViewState[Constants.REQUEST_KEY_MAILDIST_ID];
			_isPublishCouponMail = (bool)ViewState[Constants.MAILDIST_ID_PREFIX_NAME];
		}
	}

	/// <summary>
	/// メールクリックレポート表示
	/// </summary>
	private void DisplayMailClickReportDetail()
	{
		//------------------------------------------------------
		// 選択ActionNo取得
		//------------------------------------------------------
		var actionNo = 0;
		var sendCounts = 0;
		foreach (RepeaterItem ri in rListActionNo.Items)
		{
			if (((RadioButton)ri.FindControl("rbActionNoSelect")).Checked)
			{
				actionNo = int.Parse(((HiddenField)ri.FindControl("hfActonNo")).Value);
				sendCounts = int.Parse(((HiddenField)ri.FindControl("hfSendCounts")).Value);
				break;
			}
		}

		//------------------------------------------------------
		// メールクリックレポート取得
		//------------------------------------------------------
		if (sendCounts != 0)
		{
			DataView dvMailClickReport = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatament = new SqlStatement("MailClickReport", "GetMailClickReport"))
			{
				Hashtable htInput = new Hashtable
				{
					{ Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId },
					{ Constants.FIELD_MAILCLICK_MAILTEXT_ID, _mailTextId },
					{ Constants.FIELD_MAILCLICK_MAILDIST_ID, _mailDistId },
					{ Constants.FIELD_MAILCLICK_ACTION_NO, actionNo },
					{ "send_counts", sendCounts }
				};

				dvMailClickReport = sqlStatament.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// メールクリックレポート画面セット
			//------------------------------------------------------
			rList.DataSource = dvMailClickReport;
			rList.DataBind();
			rList.Visible = true;
		}
		else
		{
			rList.Visible = false;
		}
	}

	/// <summary>
	/// ラジオボタンクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbActionNoSelect_CheckedChanged(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// クリックしたラジオボタンをアクティブに
		//------------------------------------------------------
		var riSender = (RepeaterItem)((RadioButton)sender).Parent;
		foreach (RepeaterItem ri in rListActionNo.Items)
		{
			((RadioButton)ri.FindControl("rbActionNoSelect")).Checked = (ri == riSender);
		}

		//------------------------------------------------------
		// メールクリックレポート取得・画面セット
		//------------------------------------------------------
		DisplayMailClickReportDetail();
	}

	/// <summary>
	/// 詳細ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDailyReport_Click(object sender, EventArgs e)
	{
		var strActionNo = ((HiddenField)((RepeaterItem)((Button)sender).Parent).FindControl("hfActionNo")).Value;
		var strMailClickId = ((HiddenField)((RepeaterItem)((Button)sender).Parent).FindControl("hfMailClickId")).Value;

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILCLICKREPORT_DETAIL2);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(HttpUtility.UrlEncode(_mailTextId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(
			HttpUtility.UrlEncode(
				(_isPublishCouponMail ? Constants.MAILDIST_ID_PREFIX : string.Empty)
				+ _mailDistId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_NO).Append("=").Append(HttpUtility.UrlEncode(strActionNo));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_MAILCLICK_ID).Append("=").Append(HttpUtility.UrlEncode(strMailClickId));

		Response.Redirect(sbUrl.ToString());
	}


	protected void btnBack_Click(object sender, EventArgs e)
	{
		// ★
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILCLICKREPORT_LIST);
	}
}
