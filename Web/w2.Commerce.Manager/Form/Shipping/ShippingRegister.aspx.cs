/*
=========================================================================================================
  Module      : 配送料情報登録ページ(ShippingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using w2.App.Common.ShippingBaseSettings;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.FixedPurchase;
using w2.Domain.Product;
using w2.Domain.ShopShipping;

public partial class Form_Shipping_ShippingRegister : ShopShippingPage
{
	protected Hashtable m_htParam = new Hashtable();    // 配送料情報データバインド用

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// リクエスト取得＆ビューステート格納
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus);

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
			if ((this.IsActionInsert || this.IsActionCopyInsert || this.IsActionUpdate) == false)
			{
				// エラーページへ
				RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR));
			}

			// 画面制御
			InitializeComponents();

			// 表示用値設定処理
			// 新規？
			if (this.ShippingInfoInSession == null)
			{
				// 初期日付範囲設定
				m_htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN, "1");
				m_htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END, "1");
				m_htParam.Add(Constants.FIELD_SHOPSHIPPING_BUSINESS_DAYS_FOR_SHIPPING, "0");
			}
			// コピー新規・編集または配送料設定画面から戻る？
			else
			{
				// セッションより配送料情報取得
				m_htParam = this.ShippingInfoInSession;
				ViewState.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]);   // 配送料設定ID

				// セッションより配送拠点情報を取得して選択
				var shippingBaseID = (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_BASE_ID];
				ddlShippingBase.SelectedValue = string.IsNullOrEmpty(shippingBaseID)
					? Constants.FLG_SHOPSHIPPING_SHIPPING_BASE_ID_DEFAULT
					: shippingBaseID;

				// 決済種別チェックボックス設定
				SetSearchCheckBoxValue(
					cblPaymentKbn,
					((string)m_htParam[Constants.FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS]).Split(','));

				// 曜日チェックボックス設定
				SetSearchCheckBoxValue(
					cblDayOfWeek,
					((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2]).Split(','));
			}

			// ViewStateに配送料、配送料地帯情報を格納
			this.ShippingInfoInViewState = m_htParam;

			// データバインド
			DataBind();

			// 画面制御2
			RefreshComponents();

			// 初期配送会社チェック状態保持
			var shippingId = (string)ViewState[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID];
			Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_CHECED_EXPRESS_INFO] 
				= GetDeliveryCompanyInput(rExpress, shippingId, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
			Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_CHECED_MAIL_INFO] 
				= GetDeliveryCompanyInput(rMail, shippingId, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL);
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 新規登録またはコピーして新規登録？
		if (this.IsActionInsert || this.IsActionCopyInsert)
		{
			trRegisterTop.Visible = true;
			tdShippingIdEdit.Visible = true;
		}
		// 編集？
		else if (this.IsActionUpdate)
		{
			trEditTop.Visible = true;
			tdShippingIdView.Visible = true;
		}

		// 決済種別
		var payments = GetPaymentValidList(this.LoginOperatorShopId);
		cblPaymentKbn.Items.AddRange(
			payments
				.Cast<DataRowView>()
				.Select(
					payment => new ListItem(
						WebSanitizer.HtmlEncode((string)payment[Constants.FIELD_PAYMENT_PAYMENT_NAME]),
						(string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]))
				.ToArray());

		rblFixedPurchaseFirstShippingNextMonthFlg.DataSource = GetFixedPurchaseFirstShippingNextMonthFlgKbn();

		// 配送拠点Dropdownリストデータ読み取り
		var shippingBaseSettingManager = ShippingBaseSettingsManager.GetInstance();
		foreach (var shippingBase in shippingBaseSettingManager.Settings)
		{
			ddlShippingBase.Items.Add(new ListItem(shippingBase.Name, shippingBase.Id));
		}

		ddlShippingBase.SelectedValue = Constants.FLG_SHOPSHIPPING_SHIPPING_BASE_ID_DEFAULT;
	}

	/// <summary>
	/// 表示コンポーネント初期化2
	/// </summary>
	private void RefreshComponents()
	{
		// 配送種別情報
		trShippingDateSet.Style["display"] = cbShippingDateSetFlg.Checked ? "" : "none";
		tbFixedPurchase.Style["display"] = cbFixedPurchaseFlg.Checked ? "" : "none";
		tbWrappingPaper.Style["display"] = cbWrappingPaperFlg.Checked ? "" : "none";
		tbWrappingBag.Style["display"] = cbWrappingBagFlg.Checked ? "" : "none";
		tbPayment.Style["display"] = cbPaymentFlg.Checked ? "" : "none";
		spFixedPurchaseShippingNotDisplayCheckBox.Style["display"] =
			cbFixedPurchaseKbn3Flg.Checked && Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? "" : "none";

		// 配送料の別途見積もり情報
		trShippingPriceSeparateEstimates.Style["display"] = cbShippingPriceSeparateEstimatesFlg.Checked ? "" : "none";

		// 日付指定の月末表記を置き換え
		if (this.ShippingInfoInSession != null)
		{
			var shippingDays =
				this.ShippingInfoInSession.ContainsKey(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2)
					? ((string)this.ShippingInfoInSession[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2]).Replace(
						Constants.DATE_PARAM_END_OF_MONTH_VALUE,
						CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.endOfMonth@@"))
					: ((string)this.ShippingInfoInSession[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2]);
			tbFixedPurchaseKbn1Setting2.Text = WebSanitizer.HtmlEncode(shippingDays);
		}
	}

	/// <summary>
	/// 配送料指定するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNext_Click(object sender, System.EventArgs e)
	{
		string validatorType = null;
		string shippingId = null;
		var sbErrorMessages = new StringBuilder();

		// 処理ステータス
		// 新規・コピー新規
		if (this.IsActionInsert || this.IsActionCopyInsert)
		{
			validatorType = "ShopShippingRegist";
			shippingId = tbShippingId.Text;
		}
		// 変更
		else if (this.IsActionUpdate)
		{
			validatorType = "ShopShippingModify";
			shippingId = (string)ViewState[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID];
		}

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable htParam = new Hashtable();
		#region 配送種別情報
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHOP_ID, this.LoginOperatorShopId);	// 店舗ID
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId);			// 配送料設定ID
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME, tbName.Text);		// 配送種別
		htParam.Add(Constants.FIELD_SHOPSHIPPING_LAST_CHANGED, this.LoginOperatorName);	// 最終更新者

		// 日付範囲設定の利用の場合
		htParam.Add(
			Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG,
			cbShippingDateSetFlg.Checked
				? Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID	// 配送日設定可能フラグ(1：有効)
				: Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_INVALID);// 配送日設定可能フラグ(0：無効)

		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN, ddlShippingDateSetBegin.SelectedValue);									// 配送日設定可能範囲(開始)
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END, ddlShippingDateSetTerm.SelectedValue);										// 配送日設定可能範囲（終了）
		htParam.Add(Constants.FIELD_SHOPSHIPPING_BUSINESS_DAYS_FOR_SHIPPING, ddlBusinessDaysForShipping.SelectedValue); // 出荷所要営業日数

		// 定期購入利用の場合
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG, cbFixedPurchaseFlg.Checked ?
			Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID : Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_INVALID);
		// 定期購入配送パターン設定セット
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG, cbFixedPurchaseKbn1Flg.Checked ?
			Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_VALID : Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_INVALID);
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING, tbFixedPurchaseKbn1Setting.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG, cbFixedPurchaseKbn2Flg.Checked ?
			Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_VALID : Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_INVALID);
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG, cbFixedPurchaseKbn3Flg.Checked ?
			Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_VALID : Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_INVALID);
		htParam.Add(
			Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_NOTDISPLAY_FLG,
			cbFixedPurchaseShippingNotDisplayFlg.Checked
				? Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_DEFAULT_SETTING_FLG_VALID
				: Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_DEFAULT_SETTING_FLG_INVALID);
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING, tbFixedPurchaseKbn3Setting.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG,
			cbFixedPurchaseKbn4Flg.Checked
				? Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_VALID
				: Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_INVALID);
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1, tbFixedPurchaseKbn4Setting1.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2, CreateSearchStringParts(cblDayOfWeek.Items));

		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING, tbFixedPurchaseDaysOrder.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE, tbFixedPurchaseDaysCancel.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED, tbFixedPurchaseDaysRequired.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN, tbFixedPurchaseDaysSpan.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG, cbFixedPurchaseFreeShippingFlg.Checked ?
			Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_VALID : Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_INVALID);
		htParam.Add(Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS, tbNextShippingMaxChangeDays.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2,
			tbFixedPurchaseKbn1Setting2.Text.Replace(
				CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.endOfMonth@@"),
				Constants.DATE_PARAM_END_OF_MONTH_VALUE).Trim());
		// のし設定
		htParam.Add(Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG, cbWrappingPaperFlg.Checked ?
			Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID : Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_INVALID);
		String s1 = StringUtility.CreateEscapedCsvString(tbWrappingPaperTypes.Text.Trim().Replace("\r\n", "\n").Split('\n'));
		htParam.Add(Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES, s1);
		// 包装設定
		htParam.Add(Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG, cbWrappingBagFlg.Checked ?
			Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID : Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_INVALID);
		String s2 = StringUtility.CreateEscapedCsvString(tbWrappingBagTypes.Text.Trim().Replace("\r\n", "\n").Split('\n'));
		htParam.Add(Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES, s2);
		#endregion

		// 決済種別設定
		htParam.Add(Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG, cbPaymentFlg.Checked ?
			Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID : Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_INVALID);
		htParam.Add(Constants.FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS, CreateSearchStringParts(cblPaymentKbn.Items));

		// 配送会社
		htParam.Add(Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS, GetDeliveryCompanyInput(rExpress, shippingId, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));
		htParam.Add(Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL, GetDeliveryCompanyInput(rMail, shippingId, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL));
		var companyList = new List<ShopShippingCompanyModel>();
		companyList.AddRange(((List<ShopShippingCompanyInput>)htParam[Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS]).Select(i => i.CreateModel()));
		companyList.AddRange(((List<ShopShippingCompanyInput>)htParam[Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL]).Select(i => i.CreateModel()));
		htParam.Add(Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO, companyList.ToArray());

		// 配送料の別途見積もり利用
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG, cbShippingPriceSeparateEstimatesFlg.Checked ? Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID : Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID);
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE, tbShippingPriceSeparateEstimatesMessage.Text.Trim());
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE_MOBILE, tbShippingPriceSeparateEstimatesMessageMobile.Text.Trim());

		// 定期購入初回配送翌月フラグ
		htParam.Add(
			Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG,
			Constants.FIXED_PURCHASE_FIRST_SHIPPING_DATE_NEXT_MONTH_ENABLED
				? rblFixedPurchaseFirstShippingNextMonthFlg.SelectedValue
				: Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_INVALID);

		// 配送拠点
		htParam.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_BASE_ID, ddlShippingBase.SelectedValue);

		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		#region
		// 定期購入配送パターンチェック
		if (cbFixedPurchaseFlg.Checked)
		{
			// 配送パターン選択欄チェック
			if ((cbFixedPurchaseKbn1Flg.Checked || cbFixedPurchaseKbn2Flg.Checked
				|| cbFixedPurchaseKbn3Flg.Checked || cbFixedPurchaseKbn4Flg.Checked) == false)
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_PATTERN_SETTING_ERROR));
			}
			// 月間隔選択肢欄チェック
			if ((cbFixedPurchaseKbn1Flg.Checked || cbFixedPurchaseKbn2Flg.Checked)
				&& ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING] == ""))
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_KBN1_SETTING_ERROR));
			}
			// 配送間隔選択肢欄チェック
			if (cbFixedPurchaseKbn3Flg.Checked && ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING] == ""))
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_KBN3_SETTING_ERROR));
			}
			// 配送キャンセル期限欄チェック
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE] == "")
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_DAYS_CANCEL_ERROR));
			}
			// 自動注文タイミング欄チェック
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING] == "")
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_DAYS_ORDER_ERROR));
			}
			// 配送所要日数欄チェック
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED] == "")
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_DAYS_REQUIRED_ERROR));
			}
			// 最低配送間隔欄チェック
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN] == "")
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_DAYS_SPAN_ERROR));
			}
			// 次回配送日変更範囲欄チェック
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS] == "")
			{
				sbErrorMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_NEXT_SHIPPING_MAX_CHANGE_DAYS_ERROR));
			}
			// 週間隔・曜日指定・週間隔選択肢欄チェック
			if ((cbFixedPurchaseKbn4Flg.Checked || cbFixedPurchaseKbn4Flg.Checked)
				&& (string.IsNullOrEmpty((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1])))
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_KBN4_SETTING1_ERROR));
			}
			// 週間隔・曜日指定・曜日選択肢欄チェック
			if ((cbFixedPurchaseKbn4Flg.Checked || cbFixedPurchaseKbn4Flg.Checked)
				&& (string.IsNullOrEmpty((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2])))
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_KBN4_SETTING2_ERROR));
			}
			// 日付選択肢欄チェック
			if ((cbFixedPurchaseKbn1Flg.Checked) && ValidateForDateOptions(tbFixedPurchaseKbn1Setting2.Text.Trim()))
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_KBN1_SETTING2_ERROR));
			}
		}
		else
		{
			// 最低配送間隔を設定
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN] == "")
			{
				htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN] = "1";	// 空だと 0 になってしまうので、DBのデフォルト値 1 を明示的に格納
			}

			htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG] = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_INVALID;
			htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG] = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_INVALID;
			htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG] = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_INVALID;
			htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG] = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_INVALID;

			// 次回配送日変更可能日数の値は空だと 0 になってしまうので、DBのデフォルト値(15)を明示的に格納
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS] == "")
			{
				htParam[Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS] =
					Constants.FLG_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS_DEFAULT.ToString();
			}
		}

		// のしチェック
		if (cbWrappingPaperFlg.Checked)
		{
			// のし種類チェック
			if (((string)htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES]) == ("\"\"")) // "" と比較
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPSHIPPING_WRAPPING_PAPER_ERROR));
			}
		}
		// 包装チェック
		if (cbWrappingBagFlg.Checked)
		{
			// 包装種類チェック
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES] == ("\"\"")) // "" と比較
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPSHIPPING_WRAPPING_BAG_ERROR));
			}
		}

		// 決済種別チェック
		if (cbPaymentFlg.Checked)
		{
			// 決済種別種類チェック
			if ((string)htParam[Constants.FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS] == "")
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPSHIPPING_PAYMENT_ERROR));
			}
		}

		// 入力チェック＆重複チェック
		sbErrorMessages.Insert(0, Validator.Validate(validatorType, htParam));

		// 配送会社(宅配便)チェック
		var express = (List<ShopShippingCompanyInput>)htParam[Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS];
		if (express.Count() == 0)
		{
			sbErrorMessages.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPSHIPPING_DELIVERYCOMPANY_ERROR)
				.Replace(
				"@@ 1 @@",
				ValueText.GetValueText(
					Constants.TABLE_SHOPSHIPPINGCOMPANY,
					Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN,
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)));
		}
		else if (express.Any(i => i.DefaultDeliveryCompany == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID) == false)
		{
			sbErrorMessages.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPSHIPPING_DELIVERYCOMPANY_DEFAULT_ERROR)
					.Replace(
					"@@ 1 @@",
					ValueText.GetValueText(
						Constants.TABLE_SHOPSHIPPINGCOMPANY,
						Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN,
						Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)));
		}
		// 配送会社(メール便)チェック
		var mail = (List<ShopShippingCompanyInput>)htParam[Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL];
		if (mail.Count() == 0)
		{
			sbErrorMessages.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPSHIPPING_DELIVERYCOMPANY_ERROR)
					.Replace(
					"@@ 1 @@",
					ValueText.GetValueText(
						Constants.TABLE_SHOPSHIPPINGCOMPANY,
						Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN,
						Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)));
		}
		else if (mail.Any(i => i.DefaultDeliveryCompany == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID) == false)
		{
			sbErrorMessages.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPSHIPPING_DELIVERYCOMPANY_DEFAULT_ERROR)
					.Replace(
					"@@ 1 @@",
					ValueText.GetValueText(
						Constants.TABLE_SHOPSHIPPINGCOMPANY,
						Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN,
						Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)));
		}

		// 検証成功時、定期購入配送間隔選択肢と自動受注タイミングの大小関係チェック
		if ((sbErrorMessages.Length == 0) && cbFixedPurchaseFlg.Checked && cbFixedPurchaseKbn3Flg.Checked)
		{
			foreach (string str in ((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING]).Replace("(", "").Replace(")", "").Split(','))
			{
				if (int.Parse(str) <= int.Parse((string)htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING]))
				{
					sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_KBN3_RELATION_ERROR).Replace("@@ 1 @@", str));
				}
			}
		}

		// 更新時のみ配送会社が変更可能かチェック
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			CheckDeliveryCompany(htParam, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS, sbErrorMessages);
			CheckDeliveryCompany(htParam, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL, sbErrorMessages);
		}

		// 定期購入利用する場合、指定する配送種別が設定されている定期商品があるかをチェックする
		if ((sbErrorMessages.Length == 0) && (cbFixedPurchaseFlg.Checked == false))
		{
			var isExist = new ProductService().CheckFixedPurchaseProductExistByShippingType(
				(string)htParam[Constants.FIELD_SHOPSHIPPING_SHOP_ID],
				(string)htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]);
			if (isExist)
			{
				sbErrorMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOP_SHIPPING_FIXED_PURCHASE_ERROR));
			}
		}
		#endregion

		// 配送料情報を作成し、次の画面に引き継ぐ
		var beforeInfo = this.ShippingInfoInViewState;
		var currentDistinctCompany = GetDistinctCompany(
			(ShopShippingCompanyModel[])htParam[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]);
		var beforeDistinctCompany = GetDistinctCompany(
			(ShopShippingCompanyModel[])beforeInfo[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO_BEFORE]);
		// 配送料マスタ
		htParam[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO] = CreateShippingDeliveryPostages(
				(ShippingDeliveryPostageModel[])beforeInfo[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO],
				currentDistinctCompany,
				beforeDistinctCompany,
				shippingId);
		// 配送料地帯情報
		htParam[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO] = CreateShippingZones(
				(ShopShippingZoneModel[])beforeInfo[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO],
				currentDistinctCompany,
				beforeDistinctCompany,
				shippingId);

		// 編集・コピー新規登録の場合、元の配送会社を次の画面に引き継ぐため、セッションに格納しておく
		if (this.IsActionUpdate || this.IsActionCopyInsert)
		{
			htParam[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO_BEFORE] =
				beforeInfo[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO_BEFORE];
		}

		// パラメタをセッションへ格納
		this.ShippingInfoInSession = htParam;

		// エラーページへ（値保持のため入力情報をthis.ShippingInfoInSessionに格納する処理を先に実施する）
		if (sbErrorMessages.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		// 配送料情報登録ページ２へ遷移
		var url = CreateShippingPageUrl(Constants.PAGE_MANAGER_SHIPPING_REGISTER2, this.ActionStatus);
		Response.Redirect(url);
	}

	/// <summary>
	/// 配送会社が定期台帳で使用されているか確認
	/// </summary>
	/// <param name="htParam">ハッシュテーブル</param>
	/// <param name="shippingMethod">配送方法</param>
	/// <param name="sbErrorMessages">エラーメッセージ</param>
	private void CheckDeliveryCompany(Hashtable htParam, string shippingMethod, StringBuilder sbErrorMessages)
	{
		var deliveryCompanyList = new List<ShopShippingCompanyModel>();
		deliveryCompanyList.AddRange(((List<ShopShippingCompanyInput>)htParam[shippingMethod]).Select(i => i.CreateModel()));

		List<ShopShippingCompanyInput> deliveryCompanyBeforeList = 
			(shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? (List<ShopShippingCompanyInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_CHECED_EXPRESS_INFO]
				: (List<ShopShippingCompanyInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_CHECED_MAIL_INFO];
		var deliveryCompanyDiffList = deliveryCompanyBeforeList.Select(before => before.DeliveryCompanyId).ToArray()
			.Except(deliveryCompanyList.Where(after => after.ShippingKbn == shippingMethod).Select(diff => diff.DeliveryCompanyId).ToArray());
		var deliveryCompanyIdList = new List<string>();
		foreach (var companyId in deliveryCompanyDiffList)
		{
			if (new FixedPurchaseService().CheckDeliveryCompanyFixedPurchaseItems(
				companyId,
				(string)ViewState[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID],
				shippingMethod))
			{
				deliveryCompanyIdList.Add(companyId);
			}
		}

		foreach (var id in deliveryCompanyIdList)
		{
			var deliveryCompanyName = this.DeliveryCompanyList.Where(value => value.DeliveryCompanyId == id)
				.Select(select => select.DeliveryCompanyName).ToArray()[0];
			var shippingMethodName = (shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? ValueText.GetValueText(
					Constants.TABLE_SHOPSHIPPINGCOMPANY,
					Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN,
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				: ValueText.GetValueText(
					Constants.TABLE_SHOPSHIPPINGCOMPANY,
					Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN,
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL);
			sbErrorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_DELIVERY_COMPANY_USED_ERROR)
							.Replace("@@ 1 @@", shippingMethodName).Replace("@@ 2 @@", deliveryCompanyName));
		}
	}

	/// <summary>
	/// 配送会社情報取得
	/// </summary>
	/// <param name="repeater">リピーター</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <param name="shippingKbn">配送区分</param>
	/// <returns>配送会社情報</returns>
	private List<ShopShippingCompanyInput> GetDeliveryCompanyInput(Repeater repeater, string shippingId, string shippingKbn)
	{
		var result = new List<ShopShippingCompanyInput>();
		foreach (RepeaterItem item in repeater.Items)
		{
			var checkBoxNameId = "cbUse" + shippingKbn;
			if (((CheckBox)item.FindControl(checkBoxNameId)).Checked == false) continue;

			var input = new ShopShippingCompanyInput();
			input.ShippingId = shippingId;
			input.ShippingKbn = shippingKbn;
			input.DeliveryCompanyId = ((CheckBox)item.FindControl(checkBoxNameId)).Attributes["Value"];
			var wrbgShopShippingCompanyDefaultExpress = GetWrappedControl<WrappedRadioButtonGroup>(item, "rbgShopShippingCompanyDefault" + shippingKbn);
			input.DefaultDeliveryCompany =
				(wrbgShopShippingCompanyDefaultExpress.Checked ? Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID : Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_INVALID);
			input.LastChanged = this.LoginOperatorName;

			result.Add(input);
		}

		return result;
	}

	/// <summary>
	/// 「日付範囲設定の利用の有無」クリック
	/// 「定期購入の利用の有無」クリック
	/// 「のし設定の利用の有無」クリック
	/// 「包装設定の利用の有無」クリック
	/// 「決済種別任意指定の利用の有無」クリック
	/// 「配送料の別途見積もり利用の有無」クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void RefreshComponents_OnCheckedChanged(object sender, EventArgs e)
	{
		RefreshComponents();
	}

	/// <summary>
	/// 利用可能配送会社
	/// </summary>
	/// <param name="shippingKbn">配送方法</param>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>利用可能配送会社か</returns>
	public bool IsShopShippingCompanyUse(string shippingKbn, string deliveryCompanyId)
	{
		var result = ((this.ShopShippingCompanyList != null)
			&& (this.ShopShippingCompanyList.Any(i =>
				((i.ShippingKbn == shippingKbn) && (i.DeliveryCompanyId == deliveryCompanyId)))));
		return result;
	}

	/// <summary>
	/// 初期配送会社
	/// </summary>
	/// <param name="shippingKbn">配送方法</param>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>初期利用配送会社か</returns>
	public bool IsShopShippingCompanyDefault(string shippingKbn, string deliveryCompanyId)
	{
		var result = ((this.ShopShippingCompanyList != null)
			&& (this.ShopShippingCompanyList.Any(i =>
				((i.ShippingKbn == shippingKbn) && (i.DeliveryCompanyId == deliveryCompanyId) && i.IsDefault))));
		return result;
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		string url;
		if (this.IsActionInsert)
		{
			// 一覧ページへ遷移
			url = CreateShippingPageUrl(Constants.PAGE_MANAGER_SHIPPING_LIST);
			this.Response.Redirect(url);
		}

		// 詳細ページへ遷移
		url = CreateShippingPageUrl(
			Constants.PAGE_MANAGER_SHIPPING_CONFIRM,
			Constants.ACTION_STATUS_DETAIL,
			this.OriginShippingId);
		this.Response.Redirect(url);
	}

	/// <summary>配送種別配送会社リスト</summary>
	public ShopShippingCompanyModel[] ShopShippingCompanyList
	{
		get { return (ShopShippingCompanyModel[])m_htParam[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]; }
	}

	/// <summary>
	/// 日付選択肢入力チェック
	/// </summary>
	/// <returns>（1～28、月末）以外か</returns>
	private bool ValidateForDateOptions(string input)
	{
		switch (input)
		{
			case "":
				return false;

			case null:
				return true;
		}

		var list = input.Split(',').ToList();
		// 現在の言語ロケールに合わせて「月末」を置換
		var endOfMonth = CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.endOfMonth@@");
		var result = list.Any(
			item =>
				(Regex.IsMatch(item, "(^\\(([1-9]|1[0-9]|2[0-8])\\)|^([1-9]|1[0-9]|2[0-8])|(,\\(([1-9]|1[0-9]|2[0-8])\\))|,([1-9]|1[0-9]|2[0-8]))+$") == false)
					&& (item != endOfMonth)
					&& (item != "(" + endOfMonth + ")"));
		return result;
	}

	/// <summary>
	/// Get fixed purchase first shipping next month flg kbn
	/// </summary>
	/// <returns>A fixed purchase first shipping next month flg kbn</returns>
	private ListItemCollection GetFixedPurchaseFirstShippingNextMonthFlgKbn()
	{
		var result = new ListItemCollection();
		foreach (ListItem li in ValueText.GetValueItemList(
			Constants.TABLE_SHOPSHIPPING,
			Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG))
		{
			result.Add(li);
		}

		return result;
	}
}
