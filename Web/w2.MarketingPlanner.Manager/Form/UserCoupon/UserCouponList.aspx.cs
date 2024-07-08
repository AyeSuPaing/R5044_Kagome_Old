/*
=========================================================================================================
  Module      : ユーザクーポン情報一覧処理(UserCouponList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.Option;

public partial class Form_UserCoupon_UserCouponList : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			this.UserId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERID]);
			//------------------------------------------------------
			// ユーザ情報取得&設定
			//------------------------------------------------------
			var userModel = new UserService().Get(this.UserId);
			SetUser(userModel);
			this.MailAddress = userModel.MailAddr;

			//------------------------------------------------------
			// 利用可能ユーザクーポン情報取得&設定
			//------------------------------------------------------
			var userCouponDetailInfo = new CouponService().GetUserUsableCoupons(
				this.LoginOperatorDeptId,
				this.UserId,
				this.MailAddress,
				Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE);
			SetUserCoupon(userCouponDetailInfo);

			// 非会員ユーザ以外の場合(ＰＣゲスト、モバイルゲスト、スマフォゲスト以外)
			bool isUser = (UserService.IsGuest(userModel.UserKbn) == false);
			SetPublishCoupon(isUser);
		}
		SetCouponQuantityDisplay();
	}
	#endregion

	#region -SetUser ユーザ情報設定
	/// <summary>
	/// ユーザ情報設定
	/// </summary>
	/// <param name="user"></param>
	private void SetUser(UserModel user)
	{
		if (user != null)
		{
			// 基本情報設定
			lUserKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, user.UserKbn));
			lName.Text = WebSanitizer.HtmlEncode(user.Name);
		}
		else
		{
			// 該当データ無しエラー
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
	}
	#endregion

	#region -SetUserCoupon 利用可能ユーザクーポン情報設定
	/// <summary>
	/// 利用可能ユーザクーポン情報設定
	/// </summary>
	/// <param name="userCouponInfo">ユーザクーポン情報</param>
	private void SetUserCoupon(UserCouponDetailInfo[] userCouponInfo)
	{
		if (userCouponInfo != null && userCouponInfo.Length != 0)
		{
			// エラー非表示制御
			trListError.Visible = false;
			// データバインド
			rList.DataSource = userCouponInfo;
			rList.DataBind();
		}
		else
		{
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
	}
	#endregion

	#region -SetPublishCoupon 発行可能クーポン情報取得
	/// <summary>
	/// 発行可能クーポン情報取得
	/// </summary>
	/// <param name="isUser">会員判断フラグ</param>
	private void SetPublishCoupon(bool isUser)
	{
		if (isUser)
		{
			// 発行可能クーポン情報取得
			this.PublishCouponList = new CouponService().GetAllPublishCoupons(this.LoginOperatorDeptId);

			if (this.PublishCouponList != null && this.PublishCouponList.Length > 0)
			{
				// 発行可能クーポン設定
				ddlCoupon.Items.Add(new ListItem("", ""));
				foreach (CouponModel couponInfo in this.PublishCouponList)
				{
					ddlCoupon.Items.Add(new ListItem(couponInfo.CouponName, couponInfo.CouponId));
				}
			}
			else
			{
				// 発行可能クーポン無しエラー
				ddlCoupon.Visible = false;
				btnPublish.Visible = false;
				lError.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCOUPON_NO_EXISTS_ERROR);
			}
		}
		// 非会員の場合
		else
		{
			// 非会員クーポン発行不可エラー
			ddlCoupon.Visible = false;
			btnPublish.Visible = false;
			lError.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCOUPON_NO_PUBLISH_ERROR);
		}
	}
	#endregion

	#region #CreateUserListUrl ユーザ情報一覧遷移URL作成
	/// <summary>
	/// ユーザ情報一覧遷移URL作成
	/// </summary>
	/// <returns>ユーザ情報一覧遷移URL</returns>
	protected string CreateUserListUrl()
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERLIST);
		// 検索条件が存在する場合
		if (Session[Constants.SESSIONPARAM_KEY_USER_SEARCH_INFO] != null)
		{
			// 一覧検索条件取得
			Hashtable searchParams = (Hashtable)Session[Constants.SESSIONPARAM_KEY_USER_SEARCH_INFO];
			url.Append("?").Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_SEARCH_KEY]));
			url.Append("&").Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_SEARCH_WORD]));
			url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_SORT_KBN]));
			url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(searchParams[Constants.REQUEST_KEY_PAGE_NO])));
		}

		return url.ToString();
	}
	#endregion

	#region #CreateCouponDetailUrl クーポン情報詳細遷移URL作成
	/// <summary>
	/// クーポン情報詳細遷移URL作成
	/// </summary>
	/// <param name="couponId">ユーザID</param>
	/// <returns>クーポン情報詳細遷移URL</returns>
	protected string CreateCouponDetailUrl(string couponId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_COUPON_CONFIRM);
		url.Append("?").Append(Constants.REQUEST_KEY_COUPON_COUPON_ID).Append("=").Append(HttpUtility.UrlEncode(couponId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return url.ToString();
	}
	#endregion

	#region #CreateUserCouponHistoryListUrl ユーザクーポン履歴情報一覧遷移URL作成
	/// <summary>
	/// ユーザクーポン履歴情報一覧遷移URL作成
	/// </summary>
	/// <returns>ユーザクーポン履歴情報一覧遷移URL</returns>
	protected string CreateUserCouponHistoryListUrl()
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONHISTORYLIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USERID).Append("=").Append(HttpUtility.UrlEncode(this.UserId));

		return url.ToString();
	}
	#endregion

	#region #DisplayDiscount クーポン割引(割引額 OR 割引率)取得
	/// <summary>
	/// クーポン割引(割引額 OR 割引率)取得
	/// </summary>
	/// <param name="userCouponInfo">クーポン情報</param>
	/// <returns>クーポン割引</returns>
	protected string DisplayDiscount(UserCouponDetailInfo userCouponInfo)
	{
		return CouponOptionUtility.GetCouponDiscountString(userCouponInfo);
	}
	#endregion

	#region #DisplayDateCreated クーポン発効日取得
	/// <summary>
	/// クーポン発効日取得
	/// </summary>
	/// <param name="userCouponInfo">クーポン情報</param>
	/// <returns>クーポン発効日</returns>
	/// <remarks>
	/// 利用制限有りクーポンの場合のみ表示
	/// </remarks>
	protected string DisplayDateCreated(UserCouponDetailInfo userCouponInfo)
	{
		string result = null;

		// 利用制限有りクーポンの場合
		if ((userCouponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_USERREGIST)
			|| (userCouponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BUY)
			|| (userCouponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY)
			|| (userCouponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT)
			|| (userCouponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON)
			|| (userCouponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION))
		{
			result = DateTimeUtility.ToStringForManager(
				userCouponInfo.DateCreated,
				DateTimeUtility.FormatType.ShortDate2Letter);
		}
		else
		{
			result = "－";
		}

		return StringUtility.ToEmpty(result);
	}
	#endregion

	#region #DisplayDeleteButton クーポン削除ボタン表示可否
	/// <summary>
	/// クーポン削除ボタン表示可否
	/// </summary>
	/// <param name="userCouponInfo">クーポン情報</param>
	/// <returns>クーポン削除ボタン表示可否</returns>
	protected bool DisplayDeleteButton(UserCouponDetailInfo userCouponInfo)
	{
		// 無制限クーポンの場合は削除ボタンは表示しない
		var isDisplay = CouponOptionUtility.IsCouponLimit(userCouponInfo.CouponType)
			|| CouponOptionUtility.IsCouponLimitedForRegisteredUser(userCouponInfo.CouponType);
		return isDisplay;
	}

	/// <summary>
	/// クーポン発行ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPublish_OnClick(object sender, System.EventArgs e)
	{
		var errorMessage = "";

		// ユーザID、クーポンID、枝番、発行枚数取得
		var couponId = ddlCoupon.SelectedValue;
		int? couponCount = null;
		if (CouponOptionUtility.IsCouponLimitedForRegisteredUserByCouponId(couponId))
		{
			var input = new Hashtable
			{
				{"publishCouponQuantity", tbCouponQuantity.Text}
			};
			errorMessage = Validator.Validate("UserCoupon", input);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				this.ErrorMessage = errorMessage;
				return;
			}

			couponCount = int.Parse(tbCouponQuantity.Text);
			tbCouponQuantity.Text = "";
		}

		// 発行クーポン情報取得
		var couponService = new CouponService();
		var couponInfo = couponService.GetPublishCouponsById(this.LoginOperatorDeptId, couponId);

		// クーポン指定有りの場合
		if (couponInfo != null)
		{
			var publishCouponInfo = new UserCouponDetailInfo
			{
				UserId = this.UserId,
				DeptId = this.LoginOperatorDeptId,
				CouponId = couponId,
				CouponCode = couponInfo.CouponCode,
				DiscountPrice = couponInfo.DiscountPrice,
				LastChanged = this.LoginOperatorName,
				UserCouponCount = couponCount
			};

			// ユーザークーポン登録（更新履歴とともに）
			couponService.InsertUserCoupon(publishCouponInfo, UpdateHistoryAction.Insert);

			// 誕生日クーポンの場合は誕生日クーポン発行年を更新
			if ((couponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER)
				|| (couponInfo.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER))
			{
				var user = new UserModel
				{
					UserId = this.UserId,
					LastBirthdayCouponPublishYear = DateTime.Now.Year.ToString(),
					LastChanged = this.LoginOperatorName
				};
				new UserService().UpdateUserBirthdayCouponPublishYear(user, UpdateHistoryAction.Insert);
			}

			//------------------------------------------------------
			// 利用可能ユーザクーポン情報取得
			//------------------------------------------------------
			var userCouponInfo = couponService.GetUserUsableCoupons(this.LoginOperatorDeptId,
				this.UserId,
				this.MailAddress,
				Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE);
			if (userCouponInfo != null && userCouponInfo.Length > 0)
			{
				// エラー非表示制御
				trListError.Visible = false;
				// データバインド
				rList.DataSource = userCouponInfo;
				rList.DataBind();
			}
			else
			{
				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// 未選択状態にする
			ddlCoupon.SelectedIndex = 0;
		}
	}
	#endregion

	#region #btnDelete_Click 削除ボタンクリック
	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// ユーザID、クーポンID、枝番取得
		RepeaterItem ri = (RepeaterItem)((Button)sender).Parent;
		string couponId = ((HiddenField)ri.FindControl("hfCouponId")).Value;
		int couponNo = int.Parse(((HiddenField)ri.FindControl("hfCouponNo")).Value);

		// ユーザクーポン情報取得
		var couponService = new CouponService();
		var userCouponInfo = couponService.GetUserCouponFromCouponNo(this.LoginOperatorDeptId, this.UserId, couponId, couponNo);
		if (userCouponInfo != null)
		{
			userCouponInfo.LastChanged = this.LoginOperatorName;
			// クーポン情報削除（更新履歴とともに）
			couponService.DeleteUserCoupon(userCouponInfo, this.LoginOperatorName, UpdateHistoryAction.Insert);

			//------------------------------------------------------
			// 利用可能ユーザクーポン情報取得
			//------------------------------------------------------
			var usableUserCouponInfo = couponService.GetUserUsableCoupons(
				this.LoginOperatorDeptId,
				this.UserId,
				this.MailAddress,
				Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE);
			if (usableUserCouponInfo != null && usableUserCouponInfo.Length > 0)
			{
				// エラー非表示制御
				trListError.Visible = false;
			}
			else
			{
				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// データバインド
			rList.DataSource = usableUserCouponInfo;
			rList.DataBind();
		}
	}
	#endregion

	#region #発行枚数表示制御
	/// <summary>
	/// クーポン選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCoupon_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetCouponQuantityDisplay();
	}

	/// <summary>
	/// クーポン発行枚数表示制御
	/// </summary>
	protected void SetCouponQuantityDisplay()
	{
		var couponId = ddlCoupon.SelectedValue;

		// 選択なしは非表示
		if(string.IsNullOrEmpty(couponId)){
			trCouponQuantity.Visible = false;
			return;
		}

		// 選択状態で表示制御
		var selectedCoupon = this.PublishCouponList.Where(coupon => coupon.CouponId == couponId).First();
		switch (selectedCoupon.CouponType)
		{
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER:
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER:
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER:
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER:
				trCouponQuantity.Visible = true;
				break;
			default:
				trCouponQuantity.Visible = false;
				break;
		}
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザID</summary>
	protected string UserId
	{
		get { return (ViewState[Constants.REQUEST_KEY_USERID] != null) ? (string)ViewState[Constants.REQUEST_KEY_USERID] : ""; }
		set { ViewState[Constants.REQUEST_KEY_USERID] = value; }
	}
	/// <summary>メールアドレス</summary>
	protected string MailAddress
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS]); }
		set { ViewState[Constants.REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS] = value; }
	}
	/// <summary>発行可能クーポン一覧</summary>
	protected CouponModel[] PublishCouponList
	{
		get { return (CouponModel[])ViewState["PublishCouponList"]; }
		set { ViewState["PublishCouponList"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; set; }
	#endregion
}