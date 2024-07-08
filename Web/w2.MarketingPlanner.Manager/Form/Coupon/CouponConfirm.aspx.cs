/*
=========================================================================================================
  Module      : クーポン設定確認ページ処理(CouponConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using Input.Coupon;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.Domain.Coupon;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_Coupon_CouponConfirm : CouponPage
{
	// 入力情報
	protected CouponInput m_couponInput = new CouponInput();
	// アクションフラグ
	protected string m_actionStatus = null;

	/// <summary>セッションキー:登録/更新完了メッセージ</summary>
	const string SESSION_KEY_DISP_COMP_MESSAGE = "dispcompmessage";

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
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			m_actionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_actionStatus);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();
			this.CouponTranslationData = new NameTranslationSettingModel[0];

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・コピー登録・更新画面確認？
			if (m_actionStatus == Constants.ACTION_STATUS_INSERT || m_actionStatus == Constants.ACTION_STATUS_COPY_INSERT || m_actionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

				// クーポン情報取得
				m_couponInput = (CouponInput)Session[Constants.SESSIONPARAM_KEY_COUPON_INFO];
			}
			// 詳細表示？
			else if (m_actionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// クーポンID取得
				string strCouponId = Request[Constants.REQUEST_KEY_COUPON_COUPON_ID];

				// クーポン情報取得
				var couponInfo = new CouponService().GetCoupon(this.LoginOperatorDeptId, strCouponId);

				// 該当データが有りの場合
				if (couponInfo != null)
				{
					m_couponInput = new CouponInput(couponInfo);

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						this.CouponTranslationData = GetCouponTranslationData(strCouponId);
					}
				}
				else
				{
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
				}
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}

			// ビューステート格納
			ViewState.Add(Constants.SESSIONPARAM_KEY_COUPON_INFO, m_couponInput);

			trBrandId.Visible = (Constants.PRODUCT_BRAND_ENABLED
				&& (m_couponInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND));
			trCategoryId.Visible = (m_couponInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND);

			// 登録/更新完了メッセージ表示制御
			if (Session[SESSION_KEY_DISP_COMP_MESSAGE] != null)
			{
				Session[SESSION_KEY_DISP_COMP_MESSAGE] = null;

				divComp.Visible = true;
			}
			else
			{
				divComp.Visible = false;
			}

			// データバインド
			DataBind();
		}
	}
	#endregion

	#region -InitializeComponents コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 新規・コピー新規？
		if (m_actionStatus == Constants.ACTION_STATUS_INSERT || m_actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			btnGoBackTop.Visible = true;
			btnGoBackBottom.Visible = true;
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 更新？
		else if (m_actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnGoBackTop.Visible = true;
			btnGoBackBottom.Visible = true;
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 詳細
		else if (m_actionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnBackTop.Visible = true;
			btnBackBottom.Visible = true;
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetail.Visible = true;
		}
	}
	#endregion

	#region #DisplayDiscount クーポン割引(割引額 OR 割引率)取得
	/// <summary>
	/// クーポン割引(割引額 OR 割引率)取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン割引</returns>
	protected string DisplayDiscount(CouponInput coupon)
	{
		// 配送料無料
		var freeShippingMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING);
		var discountPriceMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE);

		// クーポン割引額に値がある場合
		if (coupon.DiscountPrice != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + coupon.DiscountPrice.ToPriceString(true);
			return coupon.DiscountPrice.ToPriceString(true);
		}
		// クーポン割引率に値がある場合
		if (coupon.DiscountRate != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(coupon.DiscountRate) + "%";
			return StringUtility.ToNumeric(coupon.DiscountRate) + "%";
		}
		if (CouponOptionUtility.IsFreeShipping(coupon.CouponType) || (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
		{
			return freeShippingMessage;
		}
		return "-";
	}
	#endregion

	#region #DisplayExpire クーポン有効期限・期間取得
	/// <summary>
	/// クーポン有効期限・期間取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン有効期限・期間</returns>
	protected string DisplayExpire(CouponInput coupon)
	{
		string result = null;

		// クーポン有効期限に値がある場合
		if (coupon.ExpireDay != null)
		{
			result = string.Format(
				// 「発行から{0}日」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_COUPON_CONFIRM,
					Constants.VALUETEXT_PARAM_COUPON_CONFIRM_TITLE,
					Constants.VALUETEXT_PARAM_COUPON_EXPIRE_DATE),
				coupon.ExpireDay);
		}
		// クーポン有効期間に値がある場合
		else if (coupon.ExpireDateBgn != null)
		{
			result = string.Format(
				"{0}～{1}",
				DateTimeUtility.ToStringForManager(
					coupon.ExpireDateBgn,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter),
				DateTimeUtility.ToStringForManager(
					coupon.ExpireDateEnd,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		}

		return StringUtility.ToEmpty(result);
	}
	#endregion

	#region #DisplayExceptionalIcon クーポン例外商品アイコン取得
	/// <summary>
	/// クーポン例外商品アイコン取得
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン例外商品アイコン</returns>
	protected string DisplayExceptionalIcon(CouponInput coupon)
	{
		string result = String.Empty;
		int exceptionalIcon = 0;

		exceptionalIcon = int.Parse(StringUtility.ToEmpty(coupon.ExceptionalIcon));

		var specified = ReplaceTag("@@DispText.common_message.specified@@");
		var unspecified = ReplaceTag("@@DispText.common_message.unspecified@@");

		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON1) == Constants.FLG_COUPON_EXCEPTIONAL_ICON1
			? string.Format("1：{0}", specified)
			: string.Format("1：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON2) == Constants.FLG_COUPON_EXCEPTIONAL_ICON2
			? string.Format("2：{0}", specified)
			: string.Format("2：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON3) == Constants.FLG_COUPON_EXCEPTIONAL_ICON3
			? string.Format("3：{0}", specified)
			: string.Format("3：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON4) == Constants.FLG_COUPON_EXCEPTIONAL_ICON4
			? string.Format("4：{0}", specified)
			: string.Format("4：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON5) == Constants.FLG_COUPON_EXCEPTIONAL_ICON5
			? string.Format("5：{0}", specified)
			: string.Format("5：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON6) == Constants.FLG_COUPON_EXCEPTIONAL_ICON6
			? string.Format("6：{0}", specified)
			: string.Format("6：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON7) == Constants.FLG_COUPON_EXCEPTIONAL_ICON7
			? string.Format("7：{0}", specified)
			: string.Format("7：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON8) == Constants.FLG_COUPON_EXCEPTIONAL_ICON8
			? string.Format("8：{0}", specified)
			: string.Format("8：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON9) == Constants.FLG_COUPON_EXCEPTIONAL_ICON9
			? string.Format("9：{0}", specified)
			: string.Format("9：{0}", unspecified);
		result += result.Length > 0 ? "　" : "";
		result += (exceptionalIcon & Constants.FLG_COUPON_EXCEPTIONAL_ICON10) == Constants.FLG_COUPON_EXCEPTIONAL_ICON10
			? string.Format("10：{0}", specified)
			: string.Format("10：{0}", unspecified);

		return StringUtility.ToEmpty(result);
	}
	#endregion

	#region #btnEditTop_Click 編集ボタンクリック
	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEditTop_Click(object sender, System.EventArgs e)
	{
		// クーポン情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_COUPON_INFO] = ViewState[Constants.SESSIONPARAM_KEY_COUPON_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}
	#endregion

	#region #btnCopyInsertTop_Click
	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsertTop_Click(object sender, System.EventArgs e)
	{
		// クーポン情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_COUPON_INFO] = ViewState[Constants.SESSIONPARAM_KEY_COUPON_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}
	#endregion

	#region #btnDeleteTop_Click 削除するボタンクリック
	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteTop_Click(object sender, System.EventArgs e)
	{
		m_couponInput = ((CouponInput)ViewState[Constants.SESSIONPARAM_KEY_COUPON_INFO]);
		CouponService couponService = new CouponService();
		//------------------------------------------------------
		// ユーザクーポン情報に設定されていないかチェック
		//------------------------------------------------------
		var userCoupon = couponService.GetUserCouponFromCouponId(this.LoginOperatorDeptId, m_couponInput.CouponId);

		// 設定されていたら削除させない、エラーページへ
		if (userCoupon != null)
		{
			// エラーメッセージに、設定されているユーザーIDを表示させる
			StringBuilder errMsg = new StringBuilder();
			errMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCOUPON_DELETE_IMPOSSIBLE_ERROR));
			Session[Constants.SESSION_KEY_ERROR_MSG] = errMsg.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//クーポン発行スケジュールに設定されていないかチェック
		var count = couponService.GetCouponScheduleCountFromCouponId(m_couponInput.CouponId);
		// 設定されていたら削除させない、エラーページへ
		if (count > 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCOUPON_DELETE_IMPOSSIBLE_ERROR_FOR_SCHEDULE_USED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// クーポン情報削除
		couponService.DeleteCoupon(m_couponInput.DeptId, m_couponInput.CouponId);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPONT_LIST);
	}
	#endregion

	#region #btnInsertTop_Click 登録するボタンクリック
	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertTop_Click(object sender, System.EventArgs e)
	{
		m_couponInput = ((CouponInput)ViewState[Constants.SESSIONPARAM_KEY_COUPON_INFO]);

		// クーポンID作成
		var couponId = CouponOptionUtility.CreateNewCouponId();
		m_couponInput.CouponId = couponId;

		// クーポン情報登録
		new CouponService().InsertCoupon(m_couponInput.CreateModel());

		// 完了メッセージ表示準備＆画面遷移
		Session[SESSION_KEY_DISP_COMP_MESSAGE] = "1";
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

		// 詳細画面へ
		Response.Redirect(CreateCouponDetailUrl(m_couponInput.CouponId));
	}
	#endregion

	#region #btnUpdateTop_Click 更新ボタンクリック
	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTop_Click(object sender, System.EventArgs e)
	{
		// クーポン情報更新
		m_couponInput = ((CouponInput)ViewState[Constants.SESSIONPARAM_KEY_COUPON_INFO]);
		new CouponService().UpdateCoupon(m_couponInput.CreateModel());

		// 完了メッセージ表示準備＆画面遷移
		Session[SESSION_KEY_DISP_COMP_MESSAGE] = "1";
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

		// 詳細画面へ
		Response.Redirect(CreateCouponDetailUrl(m_couponInput.CouponId));
	}
	#endregion

	#region #btnBack_Click 一覧へ戻るボタンクリック
	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, System.EventArgs e)
	{
		// クーポン一覧へ
		var url = CreateCouponListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_COUPON_SEARCH_INFO]);
		Response.Redirect(url);
	}
	#endregion

	#region #DisplayCouponCount クーポン利用可能回数の表示
	/// <summary>
	/// クーポン利用可能回数の表示
	/// </summary>
	/// <param name="coupon">クーポン情報</param>
	/// <returns>クーポン利用可能回数</returns>
	protected string DisplayCouponCount(CouponInput coupon)
	{
		string result = null;

		// 回数制限のある場合
		if (CouponOptionUtility.IsCouponAllLimit(coupon.CouponType))
		{
			result = string.Format(
				ReplaceTag("@@DispText.common_message.times@@"),
				StringUtility.ToNumeric(coupon.CouponCount));
		}
		else
		{
			result = "－";
		}

		return result;
	}
	#endregion

	/// <summary>
	/// クーポンユーザ表示フラグの表示制御
	/// </summary>
	/// <param name="couponType">クーポンタイプ</param>
	/// <returns>表示有無、表示：true、非表示：false</returns>
	protected bool IsDisplayUserDispFlg(string couponType)
	{
		if ((couponType == Constants.FLG_COUPONCOUPON_TYPE_UNLIMIT)
			|| (couponType == Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT)
			|| (couponType == Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT)
			|| (couponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING)
			|| (CouponOptionUtility.IsBlacklistCoupon(couponType)))
		{
			return true;
		}

		return false;
	}

	#region -GetCouponTranslationData クーポン翻訳設定情報取得
	/// <summary>
	/// クーポン翻訳設定情報取得
	/// </summary>
	/// <param name="couponId">クーポンID</param>
	/// <returns>クーポン翻訳設定情報</returns>
	private NameTranslationSettingModel[] GetCouponTranslationData(string couponId)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON,
			MasterId1 = couponId,
		};
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
		return translationData;
	}
	#endregion

	/// <summary>クーポン翻訳設定情報</summary>
	protected NameTranslationSettingModel[] CouponTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["coupon_translation_data"]; }
		set { ViewState["coupon_translation_data"] = value; }
	}

}
