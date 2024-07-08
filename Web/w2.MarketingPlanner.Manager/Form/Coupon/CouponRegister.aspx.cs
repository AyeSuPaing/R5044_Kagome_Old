/*
=========================================================================================================
  Module      : クーポン設定登録ページ処理(CouponRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using Input.Coupon;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.Domain.ProductBrand;
using w2.Domain.ProductCategory;

public partial class Form_Coupon_CouponRegister : BasePage
{
	// 定数値
	protected string FIELD_COUPON_EXPIRE = "expire";	// クーポン有効期限・期間

	protected CouponInput m_couponInput = new CouponInput();
	protected string m_actionStatus = null;

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
			// OnLoad設定
			//------------------------------------------------------
			this.Master.OnLoadEvents += "initialize();";
			
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			m_actionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_actionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			// 新規かつセッション情報あり
			if ((m_actionStatus == Constants.ACTION_STATUS_INSERT) && (Session[Constants.SESSIONPARAM_KEY_COUPON_INFO] != null))
			{
				trRegister.Visible = true;

				// セッションよりクーポン情報取得
				m_couponInput = (CouponInput)Session[Constants.SESSIONPARAM_KEY_COUPON_INFO];
				ViewState.Add(Constants.FIELD_COUPON_COUPON_ID, m_couponInput.CouponId);
				ViewState.Add(Constants.FIELD_COUPON_COUPON_CODE, m_couponInput.CouponCode);

				// クーポン情報設定
				SetCouponInfo();
			}
			// 新規？
			else if (m_actionStatus == Constants.ACTION_STATUS_INSERT)
			{
				// 処理無し
				trRegister.Visible = true;
			}
			// コピー新規・編集？
			else if (m_actionStatus == Constants.ACTION_STATUS_COPY_INSERT || m_actionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				if (m_actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				{
					trRegister.Visible = true;
				}
				else if (m_actionStatus == Constants.ACTION_STATUS_UPDATE)
				{
					trEdit.Visible = true;

					// 新規作成後は発行パターンを選択不可とする(発行パターン変更によるユーザークーポン不整合防止のため)
					rbCouponTypeUserRegist.Enabled = false;
					rbCouponTypeBuy.Enabled = false;
					rbCouponTypeFirstBuy.Enabled = false;
					rbCouponTypeCreateLimitedgForRegisteredUser.Enabled = false;
					rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.Enabled = false;
					rbCouponTypeCreateLimitedBirthdayForRegisteredUser.Enabled = false;
					rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.Enabled = false;
					rbCouponTypeCreateUnLimit.Enabled = false;
					rbCouponTypeCreateAllUnLimit.Enabled = false;
					rbCouponTypeCreateLimit.Enabled = false;
					rbCouponTypeLimitedFreeShipping.Enabled = false;
					rbCouponTypeBlacklistForRegisteredUser.Enabled = false;
					rbCouponTypeBlacklistFreeShippingForRegisteredUser.Enabled = false;
					rbCouponTypeBlacklistForAll.Enabled = false;
					rbCouponTypeBlacklistFreeShippingForAll.Enabled = false;
					rbCouponTypeThanksForIntroducer.Enabled = false;
					rbCouponTypePurchaseGiveToIntroducer.Enabled = false;
					rbCouponTypeRegisterGiveToIntroducer.Enabled = false;
				}

				// セッションよりクーポン情報取得
				m_couponInput = (CouponInput)Session[Constants.SESSIONPARAM_KEY_COUPON_INFO];
				ViewState.Add(Constants.FIELD_COUPON_COUPON_ID, m_couponInput.CouponId);
				ViewState.Add(Constants.FIELD_COUPON_COUPON_CODE, m_couponInput.CouponCode);
				ViewState.Add(Constants.FIELD_COUPON_COUPON_CODE + "_old", m_couponInput.OldCouponCode);

				// クーポン情報設定
				SetCouponInfo();
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}
	}
	#endregion

	#region -InitializeComponents コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 期間に設定
		if (m_actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			ucPublishDatePeriod.SetPeriodDate(DateTime.Today, DateTime.Today.AddYears(1));
			ucExpireDatePeriod.SetPeriodDate(DateTime.Today, DateTime.Today.AddYears(1));
		}

		// 発行パターン
		rbCouponTypeUserRegist.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeBuy.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeFirstBuy.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeCreateLimitedgForRegisteredUser.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeCreateLimitedBirthdayForRegisteredUser.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeCreateUnLimit.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeCreateAllUnLimit.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeCreateLimit.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeLimitedFreeShipping.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeBlacklistForRegisteredUser.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeBlacklistFreeShippingForRegisteredUser.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeBlacklistForAll.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeBlacklistFreeShippingForAll.Attributes.Add("onclick", "change_discount_and_dispflg();");

		// 有効期間・期限設定
		rbExpireDay.Attributes.Add("onclick", "change_expire();");
		rbExpireDate.Attributes.Add("onclick", "change_expire();");

		// 対象商品
		rbProductKbnTarget.Attributes.Add("onclick", "change_exceptional();");
		rbProductKbnUnTarget.Attributes.Add("onclick", "change_exceptional();");
		rbProductKbnUnTargetByLogicalAnd.Attributes.Add("onclick", "change_exceptional();");

		// 利用時の最低購入金額指定
		cbUsablePrice.Attributes.Add("onclick", "change_usableprice();");

		// Coupon introduction
		rbCouponTypeThanksForIntroducer.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypePurchaseGiveToIntroducer.Attributes.Add("onclick", "change_discount_and_dispflg();");
		rbCouponTypeRegisterGiveToIntroducer.Attributes.Add("onclick", "change_discount_and_dispflg();");
	}
	#endregion

	#region -SetCouponInfo クーポン情報設定
	/// <summary>
	/// クーポン情報設定
	/// </summary>
	private void SetCouponInfo()
	{
		var dateTimeStart = new DateTime();
		var dateTimeEnd = new DateTime();

		// クーポンコード
		tbCouponCode.Text = StringUtility.ToEmpty(m_couponInput.CouponCode);
		// クーポン名(管理用)
		tbCouponName.Text = StringUtility.ToEmpty(m_couponInput.CouponName);
		// クーポン名(ユーザ表示用)
		tbCouponDispName.Text = StringUtility.ToEmpty(m_couponInput.CouponDispName);
		// クーポン説明(管理用)
		tbCouponDiscription.Text = StringUtility.ToEmpty(m_couponInput.CouponDiscription);
		// クーポン説明(ユーザ表示用)
		tbCouponDispDiscription.Text = StringUtility.ToEmpty(m_couponInput.CouponDispDiscription);
		// 有効フラグ
		cbValidFlg.Checked = m_couponInput.ValidFlg == Constants.FLG_COUPON_VALID_FLG_VALID;
		// ユーザ表示フラグ
		cbDispFlg.Checked = (m_couponInput.DispFlg == Constants.FLG_COUPON_DISP_FLG_ON);
		// クーポン種別
		rbCouponTypeUserRegist.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_USERREGIST;
		rbCouponTypeBuy.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BUY;
		rbCouponTypeFirstBuy.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY;
		rbCouponTypeCreateLimitedgForRegisteredUser.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER;
		rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER;
		rbCouponTypeCreateLimitedBirthdayForRegisteredUser.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER;
		rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER;
		rbCouponTypeCreateLimit.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT;
		rbCouponTypeCreateUnLimit.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_UNLIMIT;
		rbCouponTypeCreateAllUnLimit.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT;
		rbCouponTypeLimitedFreeShipping.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING;
		rbCouponTypeBlacklistForRegisteredUser.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER;
		rbCouponTypeBlacklistFreeShippingForRegisteredUser.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER;
		rbCouponTypeBlacklistForAll.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL;
		rbCouponTypeBlacklistFreeShippingForAll.Checked = m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL;
		rbCouponTypeThanksForIntroducer.Checked =
			(m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON);
		rbCouponTypePurchaseGiveToIntroducer.Checked =
			(m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON);
		rbCouponTypeRegisterGiveToIntroducer.Checked =
			(m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION);

		// 有効回数
		if (m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT)
		{
			tbCouponTypeCreateLimitCount.Text = StringUtility.ToEmpty(m_couponInput.CouponCount);
		}
		else if (m_couponInput.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING)
		{
			tbCouponLimitedFreeShipping.Text = StringUtility.ToEmpty(m_couponInput.CouponCount);
		}

		// 発行期間(開始)(終了)
		if ((DateTime.TryParse(StringUtility.ToEmpty(m_couponInput.PublishDateBgn), out dateTimeStart))
			&& (DateTime.TryParse(StringUtility.ToEmpty(m_couponInput.PublishDateEnd), out dateTimeEnd)))
		{
			ucPublishDatePeriod.SetPeriodDate(dateTimeStart, dateTimeEnd);
		}
		// クーポン割引額・割引率
		rbDiscountPrice.Checked = m_couponInput.DiscountPrice != null;
		rbDiscountRate.Checked = m_couponInput.DiscountRate != null;
		tbDiscountPrice.Text = rbDiscountPrice.Checked ? m_couponInput.DiscountPrice.ToPriceString() : "";
		tbDiscountRate.Text = rbDiscountRate.Checked ? m_couponInput.DiscountRate : "";
		// 有効期限・期間
		rbExpireDay.Checked = m_couponInput.ExpireDay != null;
		rbExpireDate.Checked = m_couponInput.ExpireDateBgn != null && m_couponInput.ExpireDateEnd != null;
		// 有効期限指定の場合
		if (rbExpireDay.Checked)
		{
			// 有効期限
			tbExpireDay.Text = m_couponInput.ExpireDay;

			// 有効期間を現在日時～現時日時 + 1年で初期設定
			ucExpireDatePeriod.SetPeriodDate(DateTime.Now.Date, DateTime.Now.Date.AddYears(1));
		}
		// 有効期間指定の場合
		else if (rbExpireDate.Checked)
		{
			// 有効期間(開始)(終了)
			if ((DateTime.TryParse(StringUtility.ToEmpty(m_couponInput.ExpireDateBgn), out dateTimeStart))
				&& (DateTime.TryParse(StringUtility.ToEmpty(m_couponInput.ExpireDateEnd), out dateTimeEnd)))
			{
				ucExpireDatePeriod.SetPeriodDate(dateTimeStart, dateTimeEnd);
			}
		}

		// クーポン対象商品区分
		rbProductKbnTarget.Checked = (m_couponInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_TARGET);
		rbProductKbnUnTarget.Checked = (m_couponInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET);
		rbProductKbnUnTargetByLogicalAnd.Checked = (m_couponInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND);

		//クーポン対象商品区分が「対象商品を限定する(かつ)」の場合
		if (rbProductKbnUnTargetByLogicalAnd.Checked)
		{
			tbProductBrand.Text = m_couponInput.ExceptionalBrandIds;
			tbProductCategory.Text = m_couponInput.ExceptionalProductCategoryIds;
		}
		// クーポン例外商品
		tbExceptionalProduct.Text = m_couponInput.ExceptionalProduct;
		// クーポン例外商品アイコン
		cbExceptionalIco1.Checked = int.Parse(m_couponInput.ExceptionalIcon1) == Constants.FLG_COUPON_EXCEPTIONAL_ICON1;
		cbExceptionalIco2.Checked = int.Parse(m_couponInput.ExceptionalIcon2) == Constants.FLG_COUPON_EXCEPTIONAL_ICON2;
		cbExceptionalIco3.Checked = int.Parse(m_couponInput.ExceptionalIcon3) == Constants.FLG_COUPON_EXCEPTIONAL_ICON3;
		cbExceptionalIco4.Checked = int.Parse(m_couponInput.ExceptionalIcon4) == Constants.FLG_COUPON_EXCEPTIONAL_ICON4;
		cbExceptionalIco5.Checked = int.Parse(m_couponInput.ExceptionalIcon5) == Constants.FLG_COUPON_EXCEPTIONAL_ICON5;
		cbExceptionalIco6.Checked = int.Parse(m_couponInput.ExceptionalIcon6) == Constants.FLG_COUPON_EXCEPTIONAL_ICON6;
		cbExceptionalIco7.Checked = int.Parse(m_couponInput.ExceptionalIcon7) == Constants.FLG_COUPON_EXCEPTIONAL_ICON7;
		cbExceptionalIco8.Checked = int.Parse(m_couponInput.ExceptionalIcon8) == Constants.FLG_COUPON_EXCEPTIONAL_ICON8;
		cbExceptionalIco9.Checked = int.Parse(m_couponInput.ExceptionalIcon9) == Constants.FLG_COUPON_EXCEPTIONAL_ICON9;
		cbExceptionalIco10.Checked = int.Parse(m_couponInput.ExceptionalIcon10) == Constants.FLG_COUPON_EXCEPTIONAL_ICON10;

		cbFreeShipping.Checked = m_couponInput.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID;
		// クーポン利用最低購入金額
		cbUsablePrice.Checked = m_couponInput.UsablePrice != null;
		tbUsablePrice.Text = m_couponInput.UsablePrice.ToPriceString();
	}
	#endregion

	#region #btnConfirmTop_Click 確認するボタンクリック
	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirmTop_Click(object sender, System.EventArgs e)
	{
		string validator = null;
		string couponId = null;
		string couponCode = null;
		string couponType = null;
		string couponCount = null;
		string expire = FIELD_COUPON_EXPIRE;

		//------------------------------------------------------
		// 処理ステータス
		//------------------------------------------------------
		// 新規・コピー新規
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT ||
			(string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT)
		{
			validator = "CouponRegist";
			couponCode = tbCouponCode.Text;
		}
		// 変更
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			validator = "CouponModity";
			couponId = (string)ViewState[Constants.FIELD_COUPON_COUPON_ID];
			couponCode = (string)ViewState[Constants.FIELD_COUPON_COUPON_CODE];
		}
		var oldCouponCode = (string)ViewState[Constants.FIELD_COUPON_COUPON_CODE + "_old"] ?? couponCode;

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		var dataInput = new CouponInput()
		{
			DeptId = this.LoginOperatorDeptId,
			CouponId = StringUtility.ToEmpty(couponId),
			CouponCode = tbCouponCode.Text.Trim(),
			OldCouponCode = oldCouponCode.Trim(),
			CouponName = tbCouponName.Text,
			CouponDispName = tbCouponDispName.Text,
			CouponDiscription = tbCouponDiscription.Text,
			ValidFlg = cbValidFlg.Checked ? Constants.FLG_COUPON_VALID_FLG_VALID : Constants.FLG_COUPON_VALID_FLG_INVALID,
			DispFlg = this.IsDispFlg ? Constants.FLG_COUPON_DISP_FLG_ON : Constants.FLG_COUPON_DISP_FLG_OFF,
			CouponDispDiscription = tbCouponDispDiscription.Text,
			FreeShippingFlg = Constants.FLG_COUPON_FREE_SHIPPING_INVALID
		};

		if (rbCouponTypeUserRegist.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_USERREGIST;
		}
		else if (rbCouponTypeBuy.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_BUY;
		}
		else if (rbCouponTypeFirstBuy.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY;
		}
		else if (rbCouponTypeCreateLimitedgForRegisteredUser.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER;
		}
		else if (rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER;
		}
		else if (rbCouponTypeCreateLimitedBirthdayForRegisteredUser.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER;
		}
		else if (rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER;
		}
		else if (rbCouponTypeCreateLimit.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT;
			couponCount = tbCouponTypeCreateLimitCount.Text.Trim();
		}
		else if (rbCouponTypeCreateUnLimit.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_UNLIMIT;
		}
		else if (rbCouponTypeCreateAllUnLimit.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT;
		}
		else if (rbCouponTypeLimitedFreeShipping.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING;
			couponCount = tbCouponLimitedFreeShipping.Text.Trim();
		}
		else if (rbCouponTypeBlacklistForRegisteredUser.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER;
		}
		else if (rbCouponTypeBlacklistFreeShippingForRegisteredUser.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER;
		}
		else if (rbCouponTypeBlacklistForAll.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL;
		}
		else if (rbCouponTypeBlacklistFreeShippingForAll.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL;
		}
		else if (rbCouponTypeThanksForIntroducer.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON;
		}
		else if (rbCouponTypePurchaseGiveToIntroducer.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON;
		}
		else if (rbCouponTypeRegisterGiveToIntroducer.Checked)
		{
			couponType = Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION;
		}

		dataInput.CouponType = couponType;

		// 回数制限付きクーポンを管理者が発行 || 回数制限付き配送無料クーポンを管理者が発行
		if (couponCount != null)
		{
			if (couponCount.Length == 0)
			{
				couponCount = "0"; // 未入力の場合は０
			}
			// 利用可能回数を設定する処理
			dataInput.CouponCount = couponCount;
		}

		if (string.IsNullOrEmpty(ucPublishDatePeriod.HfStartDate.Value) == false)
		{
			dataInput.PublishDateBgn = string.Format("{0} {1}",
				ucPublishDatePeriod.HfStartDate.Value,
				ucPublishDatePeriod.HfStartTime.Value);
		}
		else
		{
			dataInput.PublishDateBgn = string.Empty;
		}

		if (string.IsNullOrEmpty(ucPublishDatePeriod.HfEndDate.Value) == false)
		{
			dataInput.PublishDateEnd = string.Format("{0} {1}",
				ucPublishDatePeriod.HfEndDate.Value,
				ucPublishDatePeriod.HfEndTime.Value);
		}
		else
		{
			dataInput.PublishDateEnd = string.Empty;
		}

		// 配送無料クーポン以外の場合に設定
		if ((rbCouponTypeLimitedFreeShipping.Checked == false)
			&& (rbCouponTypeBlacklistFreeShippingForRegisteredUser.Checked == false)
			&& (rbCouponTypeBlacklistFreeShippingForAll.Checked == false)
			&& (rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.Checked == false)
			&& (rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.Checked == false))
		{
			dataInput.FreeShippingFlg = cbFreeShipping.Checked
				? Constants.FLG_COUPON_FREE_SHIPPING_VALID
				: Constants.FLG_COUPON_FREE_SHIPPING_INVALID;
			if ((string.IsNullOrEmpty(tbDiscountPrice.Text) == false)
				|| (string.IsNullOrEmpty(tbDiscountRate.Text) == false))
			{
				if (rbDiscountPrice.Checked)
				{
					dataInput.DiscountPrice = tbDiscountPrice.Text;
				}
				else
				{
					dataInput.DiscountRate = tbDiscountRate.Text;
				}
			}
		}

		// 有効期限指定の場合
		if (rbExpireDay.Checked)
		{
			dataInput.ExpireDay = tbExpireDay.Text;
		}
		// 有効期間指定の場合
		else if (rbExpireDate.Checked)
		{
			if (string.IsNullOrEmpty(ucExpireDatePeriod.HfStartDate.Value) == false)
			{
				dataInput.ExpireDateBgn = string.Format("{0} {1}",
					ucExpireDatePeriod.HfStartDate.Value,
					ucExpireDatePeriod.HfStartTime.Value);
			}
			else
			{
				dataInput.ExpireDateBgn = string.Empty;
			}

			if (string.IsNullOrEmpty(ucExpireDatePeriod.HfEndDate.Value) == false)
			{
				dataInput.ExpireDateEnd = string.Format("{0} {1}",
					ucExpireDatePeriod.HfEndDate.Value,
					ucExpireDatePeriod.HfEndTime.Value);
			}
			else
			{
				dataInput.ExpireDateEnd = string.Empty;
			}
		}
		else
		{
			expire = "";
		}
		// 有効期限・期間入力チェックとして利用
		dataInput.Expire = expire;

		dataInput.ProductKbn = (rbProductKbnTarget.Checked
			? Constants.FLG_COUPON_PRODUCT_KBN_TARGET
			: (rbProductKbnUnTarget.Checked
				? Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET
				: Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND));

		dataInput.ExceptionalBrandIds = ((dataInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND)
			? tbProductBrand.Text
			: string.Empty);
		dataInput.ExceptionalProductCategoryIds = ((dataInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND)
			? tbProductCategory.Text
			: string.Empty);

		// 商品ID(改行、スペース削除)
		string exceptionalProduct = tbExceptionalProduct.Text;
		exceptionalProduct = exceptionalProduct.Replace("\r\n", "");	// 改行
		exceptionalProduct = exceptionalProduct.Replace("\r", "");	// 改行
		exceptionalProduct = exceptionalProduct.Replace("\n", "");	// 改行
		exceptionalProduct = exceptionalProduct.Replace(" ", "");		// 半角スペース
		exceptionalProduct = exceptionalProduct.Replace("　", "");	// 全角スペース
		dataInput.ExceptionalProduct = exceptionalProduct;

		// キャンペーンアイコン
		dataInput.ExceptionalIcon1 = (cbExceptionalIco1.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON1.ToString() : "0");
		dataInput.ExceptionalIcon2 = (cbExceptionalIco2.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON2.ToString() : "0");
		dataInput.ExceptionalIcon3 = (cbExceptionalIco3.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON3.ToString() : "0");
		dataInput.ExceptionalIcon4 = (cbExceptionalIco4.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON4.ToString() : "0");
		dataInput.ExceptionalIcon5 = (cbExceptionalIco5.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON5.ToString() : "0");
		dataInput.ExceptionalIcon6 = (cbExceptionalIco6.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON6.ToString() : "0");
		dataInput.ExceptionalIcon7 = (cbExceptionalIco7.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON7.ToString() : "0");
		dataInput.ExceptionalIcon8 = (cbExceptionalIco8.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON8.ToString() : "0");
		dataInput.ExceptionalIcon9 = (cbExceptionalIco9.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON9.ToString() : "0");
		dataInput.ExceptionalIcon10 = (cbExceptionalIco10.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON10.ToString() : "0");

		int exceptionalIcon = 0;
		exceptionalIcon += cbExceptionalIco1.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON1 : 0;
		exceptionalIcon += cbExceptionalIco2.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON2 : 0;
		exceptionalIcon += cbExceptionalIco3.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON3 : 0;
		exceptionalIcon += cbExceptionalIco4.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON4 : 0;
		exceptionalIcon += cbExceptionalIco5.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON5 : 0;
		exceptionalIcon += cbExceptionalIco6.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON6 : 0;
		exceptionalIcon += cbExceptionalIco7.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON7 : 0;
		exceptionalIcon += cbExceptionalIco8.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON8 : 0;
		exceptionalIcon += cbExceptionalIco9.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON9 : 0;
		exceptionalIcon += cbExceptionalIco10.Checked ? Constants.FLG_COUPON_EXCEPTIONAL_ICON10 : 0;
		dataInput.ExceptionalIcon = exceptionalIcon.ToString();
		// クーポン利用最低購入金額指定の場合
		if (cbUsablePrice.Checked)
		{
			dataInput.UsablePrice = tbUsablePrice.Text;
		}

		// パラメタをセッションへ格納
		Session[Constants.SESSIONPARAM_KEY_COUPON_INFO] = dataInput;
		if ((string.IsNullOrEmpty(dataInput.ExpireDateBgn) == false)
			|| (string.IsNullOrEmpty(dataInput.ExpireDateEnd) == false)
			|| (string.IsNullOrEmpty(dataInput.PublishDateBgn) == false)
			|| (string.IsNullOrEmpty(dataInput.PublishDateEnd) == false))
		{
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = 1;
		}

		// 入力チェック＆重複チェック
		string errorMessages = dataInput.Validate(validator);
		if (errorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// メインカテゴリのカタログをチェックしてください。
		if (string.IsNullOrEmpty(dataInput.ExceptionalProductCategoryIds.Trim()) == false)
		{
			var itemValidatorCategory = dataInput.ExceptionalProductCategoryIds.Trim().Split(',');
			errorMessages = CheckProductCategoryIds(itemValidatorCategory);
			if (errorMessages != "")
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}

		// メインブランドのブランドをチェックしてください。
		if (string.IsNullOrEmpty(dataInput.ExceptionalBrandIds.Trim()) == false)
		{
			var itemValidatorBrand = dataInput.ExceptionalBrandIds.Trim().Split(',');
			errorMessages = CheckProductBrandIds(itemValidatorBrand);
			if (errorMessages != "")
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}

		// 商品存在チェック
		var exceptionalProducts = dataInput.ExceptionalProduct.Trim();
		if (string.IsNullOrEmpty(exceptionalProducts) == false)
		{
			errorMessages = dataInput.CheckProductIds(exceptionalProducts);
			if (errorMessages != string.Empty)
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}

		if ((rbCouponTypeLimitedFreeShipping.Checked == false)
			&& (rbCouponTypeBlacklistFreeShippingForRegisteredUser.Checked == false)
			&& (rbCouponTypeBlacklistFreeShippingForAll.Checked == false)
			&& (rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.Checked == false)
			&& (rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.Checked == false))
		{
			// クーポン割引設定未選択エラー
			if (string.IsNullOrEmpty(tbDiscountPrice.Text)
				&& string.IsNullOrEmpty(tbDiscountRate.Text)
				&& (cbFreeShipping.Checked == false))
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CHECK_DISCOUNT_MONEY_OR_FREE_SHIP);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}

		//------------------------------------------------------
		// 入力チェック後行う
		//------------------------------------------------------
		// 最終更新者
		dataInput.LastChanged = this.LoginOperatorName;

		// クーポン回数指定以外の場合:0を入れておく
		if (CouponOptionUtility.IsCouponAllLimit(dataInput.CouponType) == false)
		{
			dataInput.CouponCount = "0";
		}

		// クーポン情報確認ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPON_CONFIRM +
			"?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}

	/// <summary>
	/// 有効なカテゴリID存在チェック
	/// </summary>
	/// <param name="productCategoryIdsList">カテゴリID</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckProductCategoryIds(string[] productCategoryIdsList)
	{
		if (productCategoryIdsList.Length == 0) return string.Empty;

		// 有効なカテゴリ
		var categories = new ProductCategoryService().GetByCategoryIds(productCategoryIdsList);

		var message = string.Empty;
		foreach (var categoryId in productCategoryIdsList)
		{
			if (categories.Any(category => (category.CategoryId == categoryId)) == false)
			{
				message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_PRODUCT_CATEGORY_UNFOUND)
					.Replace("@@ 1 @@", categoryId);
				break;
			}
		}

		return message;
	}

	/// <summary>
	/// 有効なブランドID存在チェック
	/// </summary>
	/// <param name="productBrandIdsList">ブランドID</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckProductBrandIds(string[] productBrandIdsList)
	{
		if (productBrandIdsList.Length == 0) return string.Empty;

		// 有効なブランド
		var brandIds = new ProductBrandService().GetByBrandIds(productBrandIdsList);

		var message = string.Empty;
		foreach (var brandId in productBrandIdsList)
		{
			if (brandIds.Any(brand => (brand.BrandId == brandId)) == false)
			{
				message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_PRODUCT_CATEGORY_UNFOUND)
					.Replace("@@ 1 @@", brandId);
				break;
			}
		}

		return message;
	}
	#endregion

	/// <summary>フロント表示するか</summary>
	private bool IsDispFlg { get { return (cbDispFlg.Checked || rbCouponTypeUserRegist.Checked || rbCouponTypeBuy.Checked || rbCouponTypeFirstBuy.Checked); } }
}
