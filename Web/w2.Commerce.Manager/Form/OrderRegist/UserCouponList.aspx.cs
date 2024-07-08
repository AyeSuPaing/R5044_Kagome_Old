/*
=========================================================================================================
  Module      : ユーザークーポン選択ページ処理(UserCouponList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using w2.App.Common.Option;
using w2.Common.Web;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;

public partial class Form_OrderRegist_UserCouponList : BasePage
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
			// パラメタ取得
			GetParameters();

			// クーポン一覧表示
			DisplayCouponList(this.LoginOperatorDeptId, this.UserId, this.MailAddress);
		}
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	private void GetParameters()
	{
		switch (Request[Constants.REQUEST_KEY_ACTION_STATUS])
		{
			case "UserCoupons":
			case "UsableCoupons":
				rblSearchType.SelectedValue = Request[Constants.REQUEST_KEY_ACTION_STATUS];
				break;
		}

		int pageNo;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) == false) pageNo = 1;
		this.CurrentPageNo = pageNo;

		tbCouponCode.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPON_CODE]);

		tbCouponName.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPON_NAME]);
	}

	/// <summary>
	/// クーポン一覧表示
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="mailAddress">メールアドレス</param>
	private void DisplayCouponList(string deptId, string userId, string mailAddress)
	{
		// データ取得
		var condition = new CouponListSearchCondition()
		{
			DeptId = deptId,
			UserId = userId,
			MailAddress = mailAddress,
			CouponCode = StringUtility.SqlLikeStringSharpEscape(tbCouponCode.Text.Trim()),
			CouponName = StringUtility.SqlLikeStringSharpEscape(tbCouponName.Text.Trim()),
			UserCouponOnly = (rblSearchType.SelectedValue == "UserCoupons") ? "1" : "0",
			BgnRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo,
			UsedUserJudgeType = Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
		};
		var userCoupons = new CouponService().GetAllUserUsableCoupons(condition);

		// 一覧セット
		rUsableCouponList.DataSource = userCoupons;
		rUsableCouponList.DataBind();
		trListError.Visible = (rUsableCouponList.Items.Count == 0);

		// ページャ作成
		lbPager1.Text = WebPager.CreateDefaultListPager(
			(userCoupons.Length > 0) ? (int)userCoupons[0].RowCount : 0,
			this.CurrentPageNo,
			CreateCouponListUrl());
	}

	/// <summary>
	/// クーポン一覧URL作成（ページNO指定なし）
	/// </summary>
	/// <returns>クーポン一覧URL</returns>
	private string CreateCouponListUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_COUPON_LIST)
			.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId)
			.AddParam(Constants.REQUEST_KEY_USER_MAIL_ADDR, this.MailAddress)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, rblSearchType.SelectedValue)
			.AddParam(Constants.REQUEST_KEY_COUPON_CODE, tbCouponCode.Text.Trim())
			.AddParam(Constants.REQUEST_KEY_COUPON_NAME, tbCouponName.Text.Trim())
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// クーポン割引文字列取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン割引文字列</returns>
	protected string GetCouponDiscountString(UserCouponDetailInfo coupon)
	{
		return CouponOptionUtility.GetCouponDiscountString(coupon);
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateCouponListUrl());
	}

	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState["CurrentPageNo"]; }
		set { ViewState["CurrentPageNo"] = value; }
	}
	/// <summary>ユーザーID</summary>
	private string UserId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_ID]); }
	}
	/// <summary>メールアドレス</summary>
	private string MailAddress
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_MAIL_ADDR]); }
	}
}