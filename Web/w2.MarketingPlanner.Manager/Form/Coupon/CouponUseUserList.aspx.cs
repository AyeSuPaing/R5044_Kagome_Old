/*
=========================================================================================================
  Module      : クーポン利用ユーザー一覧画面(CouponUseUserList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Manager;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.Order;

/// <summary>
/// クーポン利用ユーザー一覧画面
/// </summary>
public partial class Form_Coupon_CouponUseUserList : BasePage
{
	/// <summary>ページ番号既定値</summary>
	private const int DEFAULT_PAGE_NO = 1;

	#region イベントハンドラ
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		uMasterDownload.OnCreateSearchInputParams += CreateSearchParameter;

		if (!IsPostBack)
		{
			var parameters = GetParameters();
			SetParameters(parameters);

			this.CouponId = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID];
			SetCouponCodeAndCouponName();
			this.DisplayKbn = (string)parameters[Constants.REQUEST_KEY_DISPLAY_KBN];

			if (this.DisplayKbn == Constants.KBN_COUPONUSEUSER_DISPLAY_LIST)
			{
				ViewCouponUseUserList(parameters);
			}
			else
			{
				ViewCouponUseUserComplete();
			}

			Session[Constants.SESSION_KEY_ORIGIN_PAGE] = Constants.PAGE_W2MP_MANAGER_COUPON_COUPONUSEUSERLIST;
		}
	}
	#endregion

	#region #btnBack_Click 一覧へ戻るボタンクリック
	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_COUPONT_LIST);
	}
	#endregion

	#region #btnSearch_Click 検索ボタンクリック
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateCouponUseUserListSearchUrl(GetSearchInfo()));
	}
	#endregion

	#region #btnMassUpdate_Click 一括更新ボタンクリック
	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnMassUpdate_Click(object sender, EventArgs e)
	{
		var updateCouponUseUsers = rCouponUseUserList.Items
			.OfType<RepeaterItem>()
			.Where(item => ((CheckBox)item.FindControl("cbCancelFlag")).Checked)
			.Select(item => this.CouponUseUserList[item.ItemIndex])
			.ToList();
		if (updateCouponUseUsers.Any() == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPONUSEUSER_NOT_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
		updateCouponUseUsers.ForEach(couponUseUser => new CouponService().DeleteCouponUseUser(this.CouponId, couponUseUser.CouponUseUser));

		this.DisplayKbn = Constants.KBN_COUPONUSEUSER_DISPLAY_COMPLETE;
		this.UpdatedCouponUseUserList = updateCouponUseUsers.ToArray();
		Response.Redirect(CreateCouponUseUserListSearchUrl(GetSearchInfo()));
	}
	#endregion

	#region #btnAdd_Click 追加ボタンクリック
	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		var input = CreateInputData();
		var errorMessage = input.Validate();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		var model = input.CreateModel();
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();
			
			try
			{
				var service = new CouponService();
				service.InsertCouponUseUser(model, accessor);

				var userId = (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS) 
					? (string.IsNullOrEmpty(model.OrderId) && string.IsNullOrEmpty(model.FixedPurchaseId))
						? Constants.COUPONUSEUSER_DEFAULT_BLACKLISTCOUPON_USER
						: (string.IsNullOrEmpty(model.OrderId) == false)
							? new OrderService().GetOrderByOrderIdAndCouponUseUser(
								model.OrderId,
								model.CouponUseUser,
								Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS).UserId
							: new FixedPurchaseService().GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser(
								model.FixedPurchaseId,
								model.CouponUseUser,
								Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS).UserId
					: model.CouponUseUser;
				service.InsertUserCouponHistory(
					userId,
					model.OrderId,
					this.LoginOperatorDeptId,
					this.CouponId,
					this.CouponCode,
					(string.IsNullOrEmpty(model.OrderId) && (string.IsNullOrEmpty(model.FixedPurchaseId) == false))
						? Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE
						: Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE,
					Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_OPERATOR,
					-1,
					0,
					this.LoginOperatorName,
					accessor,
					model.FixedPurchaseId);

				accessor.CommitTransaction();
			}
			catch
			{
				accessor.RollbackTransaction();

				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPONUSEUSER_INSERT_NOT_SUCCESS);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}

		this.DisplayKbn = Constants.KBN_COUPONUSEUSER_DISPLAY_LIST;
		Response.Redirect(CreateCouponUseUserListSearchUrl(GetSearchInfo()));
	}
	#endregion

	#region #btnRedirectEdit_Click 続けて処理をするボタンクリック
	/// <summary>
	/// 続けて処理をするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRedirectEdit_Click(object sender, EventArgs e)
	{
		this.DisplayKbn = Constants.KBN_COUPONUSEUSER_DISPLAY_LIST;
		Response.Redirect(CreateCouponUseUserListSearchUrl(GetSearchInfo()));
	}
	#endregion
	#endregion

	#region -GetParameters パラメタ取得
	/// <summary>
	/// パラメタ取得
	/// </summary>
	private Hashtable GetParameters()
	{
		var parameters = new Hashtable
		{
			{ Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID]) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_USER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONUSEUSER_USER_ID]) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS]) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_USER_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPONUSEUSER_USER_NAME]) },
			{ Constants.REQUEST_KEY_USER_DEL_FLG, string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USER_DEL_FLG])
				? Constants.FLG_USER_DELFLG_UNDELETED
				: Request[Constants.REQUEST_KEY_USER_DEL_FLG] },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM,
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM], ucUseDateFrom.Year) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM,
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM], ucUseDateFrom.Month) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM,
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM], ucUseDateFrom.Day) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO,
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO], ucUseDateTo.Year) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO,
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO], ucUseDateTo.Month) },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO,
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO], ucUseDateTo.Day) },
			{ Constants.REQUEST_KEY_PAGE_NO, this.CurrentPageNo }
		};
		var displayKbn = new[]
			{
				Constants.KBN_COUPONUSEUSER_DISPLAY_LIST,
				Constants.KBN_COUPONUSEUSER_DISPLAY_COMPLETE
			};
		parameters.Add(Constants.REQUEST_KEY_DISPLAY_KBN,
			displayKbn.Any(kbn => kbn == StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DISPLAY_KBN]))
				? Request[Constants.REQUEST_KEY_DISPLAY_KBN]
				: Constants.KBN_COUPONUSEUSER_DISPLAY_DEFAULT);

		return parameters;
	}
	#endregion

	#region -SetParameters 検索パラメタを画面にセット
	/// <summary>
	/// 検索パラメタを画面にセット
	/// </summary>
	/// <param name="parameters">検索パラメタ</param>
	private void SetParameters(Hashtable parameters)
	{
		tbUserId.Text = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_USER_ID];
		tbMailAddress.Text = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS];
		tbUserName.Text = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_USER_NAME];
		cbDeleteFlag.Checked = ((string)parameters[Constants.REQUEST_KEY_USER_DEL_FLG] == Constants.FLG_USER_DELFLG_DELETED);
		ucUseDateFrom.Year = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM];
		ucUseDateFrom.Month = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM];
		ucUseDateFrom.Day = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM];
		ucUseDateTo.Year = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO];
		ucUseDateTo.Month = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO];
		ucUseDateTo.Day = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO];
	}
	#endregion

	#region -SetCouponCodeAndCouponName クーポンコード、クーポン名の取得
	/// <summary>
	/// クーポンコード、クーポン名の取得
	/// </summary>
	private void SetCouponCodeAndCouponName()
	{
		var coupon = new CouponService().GetCoupon(this.LoginOperatorDeptId, this.CouponId);
		if (coupon != null)
		{
			this.CouponCode = coupon.CouponCode;
			this.CouponName = coupon.CouponName;
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCOUPON_NO_EXISTS_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}
	#endregion

	#region -ViewCouponUseUserList クーポン利用ユーザー一覧の表示
	/// <summary>
	/// クーポン利用ユーザー一覧の表示
	/// </summary>
	/// <param name="parameters">検索パラメタ</param>
	private void ViewCouponUseUserList(Hashtable parameters)
	{
		trComplete.Visible = false;

		var service = new CouponService();
		var searchCondition = CreateRequestParameter(parameters);
		var totalCount = service.GetCouponUseUserSearchHitCount(searchCondition);

		if (totalCount > 0)
		{
			var couponUseUser = service.SearchCouponUseUser(searchCondition);
			rCouponUseUserList.DataSource = couponUseUser;
			rCouponUseUserList.DataBind();

			this.CouponUseUserList = couponUseUser;

			lbPager.Text = WebPager.CreateDefaultListPager(totalCount, this.CurrentPageNo, CreateCouponUseUserListSearchUrl(parameters));
		}
		else
		{
			btnMassUpdateTop.Enabled = btnMassUpdateBottom.Enabled = false;
			trListError.Visible = true;
			cbSelectAll.Disabled = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
	}
	#endregion

	#region -ViewCouponUseUserComplete クーポン利用ユーザー処理完了一覧の表示
	/// <summary>
	/// クーポン利用ユーザー処理完了一覧の表示
	/// </summary>
	private void ViewCouponUseUserComplete()
	{
		trComplete.Visible = true;
		trSubTitle.Visible = false;
		trList.Visible = false;
		trEdit.Visible = false;
		trSpace.Visible = false;

		rCouponUseUserListComplete.DataSource = this.UpdatedCouponUseUserList;
		rCouponUseUserListComplete.DataBind();

		this.DisplayKbn = Constants.KBN_COUPONUSEUSER_DISPLAY_LIST;
		this.UpdatedCouponUseUserList = null;
	}
	#endregion

	#region -CreateSearchParameter マスタ出力機能向けDB問い合わせ用パラメタ生成
	/// <summary>
	/// マスタ出力機能向けDB問い合わせ用パラメタ生成
	/// </summary>
	/// <returns></returns>
	private Hashtable CreateSearchParameter()
	{
		var parameter = CreateRequestParameter(GetParameters()).CreateHashtableParams();
		return parameter;
	}
	#endregion

	#region -CreateRequestParameter DB問い合わせ用パラメタ生成
	/// <summary>
	/// DB問い合わせ用パラメタ生成
	/// </summary>
	/// <param name="parameters">検索パラメタ</param>
	/// <returns>検索クラス</returns>
	private CouponUseUserListSearchCondition CreateRequestParameter(Hashtable parameters)
	{
		var condition = new CouponUseUserListSearchCondition
		{
			CouponId = this.CouponId,
			UserId = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_USER_ID],
			UserName = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_USER_NAME],
			MailAddress = (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS],
			DelFlg = (string)parameters[Constants.REQUEST_KEY_USER_DEL_FLG],
			DateCreatedFrom = DateCreatedToDatetime(
				(string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM],
				(string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM],
				(string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM]),
			DateCreatedTo = DateCreatedToDatetime(
				(string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO],
				(string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO],
				(string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO]),
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo,
			UsedUserJudgeType = Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
		};
		return condition;
	}
	#endregion

	#region -DateCreatedToDatetime 利用日時をDatetime?型に変換
	/// <summary>
	/// 利用日時をDatetime?型に変換
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// <param name="day">日</param>
	/// <returns>利用日</returns>
	private DateTime? DateCreatedToDatetime(string year, string month, string day)
	{
		var date = string.Format("{0}/{1}/{2}", year, month, day);
		var toDate = Validator.IsDate(date) ? DateTime.Parse(date) : (DateTime?)null;
		return toDate;
	}
	#endregion

	#region -GetSearchInfo 検索条件を画面から取得
	/// <summary>
	/// 検索条件を画面から取得
	/// </summary>
	/// <returns>検索条件</returns>
	private Hashtable GetSearchInfo()
	{
		var searchInfo = new Hashtable
		{
			{ Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID, this.CouponId },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_USER_ID, tbUserId.Text.Trim() },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_USER_NAME, tbUserName.Text.Trim() },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS, tbMailAddress.Text.Trim() },
			{ Constants.REQUEST_KEY_USER_DEL_FLG, cbDeleteFlag.Checked ? Constants.FLG_USER_DELFLG_DELETED : Constants.FLG_USER_DELFLG_UNDELETED },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM, ucUseDateFrom.Year },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM, ucUseDateFrom.Month },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM, ucUseDateFrom.Day },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO, ucUseDateTo.Year },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO, ucUseDateTo.Month },
			{ Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO, ucUseDateTo.Day },
			{ Constants.REQUEST_KEY_DISPLAY_KBN, this.DisplayKbn }
		};
		return searchInfo;
	}
	#endregion

	#region -CreateInputData 入力クラス生成
	/// <summary>
	/// 入力クラス生成
	/// </summary>
	/// <returns>入力クラス</returns>
	private CouponUseUserInput CreateInputData()
	{
		var input = new CouponUseUserInput
		{
			CouponId = this.CouponId,
			OrderId = tbOrderId.Text.Trim(),
			CouponUseUser = tbCouponUseUser.Text.Trim(),
			FixedPurchaseId = tbFixedPurchaseId.Text.Trim(),
			LastChanged = this.LoginOperatorName
		};
		return input;
	}
	#endregion

	#region -CreateCouponUseUserListSearchUrl ユーザークーポン情報一覧検索URL生成
	/// <summary>
	/// ユーザークーポン情報一覧検索URL生成
	/// </summary>
	/// <param name="parameters">検索条件</param>
	/// <returns></returns>
	private string CreateCouponUseUserListSearchUrl(Hashtable parameters)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_COUPONUSEUSERLIST)
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_USER_ID, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_USER_ID])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_USER_NAME, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_USER_NAME])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID])
			.AddParam(Constants.REQUEST_KEY_USER_DEL_FLG, (string)parameters[Constants.REQUEST_KEY_USER_DEL_FLG])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO])
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO, (string)parameters[Constants.REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO])
			.AddParam(Constants.REQUEST_KEY_DISPLAY_KBN, this.DisplayKbn)
			.CreateUrl();
		return url;
	}
	#endregion

	#region #CreateResetUrl 検索条件クリア用URL生成
	/// <summary>
	/// 検索条件クリア用URL生成
	/// </summary>
	/// <returns>検索条件クリア用URL</returns>
	protected string CreateResetUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_COUPONUSEUSERLIST)
			.AddParam(Constants.REQUEST_KEY_COUPONUSEUSER_COUPON_ID, this.CouponId)
			.CreateUrl();
		return url;
	}
	#endregion

	#region #CreateOrderConfirmUrl 受注確認画面URL生成
	/// <summary>
	/// 受注確認画面URL生成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>受注確認画面URL</returns>
	protected string CreateOrderConfirmUrl(string orderId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();
		var singleSignUrl = WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, url));
		var popupLink = string.Format("javascript:open_window('{0}', 'ordercontact', 'width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');", singleSignUrl);
		return popupLink;
	}
	#endregion

	#region #CreateUserCouponListUrl ユーザークーポン情報一覧画面URL生成
	/// <summary>
	/// ユーザークーポン情報一覧画面URL生成
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>ユーザークーポン情報一覧画面URL</returns>
	protected string CreateUserCouponListUrl(string userId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONLIST)
			.AddParam(Constants.REQUEST_KEY_USERID, userId)
			.CreateUrl();
		return url;
	}
	#endregion

	#region #CreateFixedPurchaseDetailUrl 定期台帳詳細画面URL生成
	/// <summary>
	/// 定期台帳詳細画面URL生成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <returns>定期台帳詳細画面URL</returns>
	protected string CreateFixedPurchaseDetailUrl(string fixedPurchaseId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();
		var singleSignUrl = WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, url));
		var popupLink = string.Format("javascript:open_window('{0}', 'ordercontact', 'width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');", singleSignUrl);
		return popupLink;
	}
	#endregion

	#region プロパティ
	/// <summary>クーポン利用ユーザー一覧検索結果</summary>
	private CouponUseUserListSearchResult[] CouponUseUserList
	{
		get { return (CouponUseUserListSearchResult[])ViewState[Constants.TABLE_COUPONUSEUSER]; }
		set { ViewState[Constants.TABLE_COUPONUSEUSER] = value; }
	}
	/// <summary>クーポンID</summary>
	protected string CouponId
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_COUPON_COUPON_ID]); }
		set { ViewState[Constants.REQUEST_KEY_COUPON_COUPON_ID] = value; }
	}
	/// <summary>クーポンコード</summary>
	protected string CouponCode
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_COUPON_COUPON_CODE]); }
		set { ViewState[Constants.REQUEST_KEY_COUPON_COUPON_CODE] = value; }
	}
	/// <summary>クーポン名</summary>
	protected string CouponName
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_COUPON_COUPON_NAME]); }
		set { ViewState[Constants.REQUEST_KEY_COUPON_COUPON_NAME] = value; }
	}
	/// <summary>現在のページ番号</summary>
	private int CurrentPageNo
	{
		get
		{
			int pageNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) ? pageNo : DEFAULT_PAGE_NO;
		}
	}
	/// <summary>画面表示区分</summary>
	private string DisplayKbn
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_DISPLAY_KBN]); }
		set { ViewState[Constants.REQUEST_KEY_DISPLAY_KBN] = value; }
	}
	/// <summary>更新済みクーポン利用ユーザー情報</summary>
	private CouponUseUserListSearchResult[] UpdatedCouponUseUserList
	{
		get { return (CouponUseUserListSearchResult[])Session[Constants.SESSION_KEY_PARAM]; }
		set { Session[Constants.SESSION_KEY_PARAM] = value; }
	}
	#endregion
}