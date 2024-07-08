/*
=========================================================================================================
  Module      : 定期購入情報解約入力画面処理(FixedPurchaseCancelInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.FixedPurchase;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_FixedPurchase_FixedPurchaseCancelInput : FixedPurchasePage
{
	# region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WspFixedPurchaseStatus { get { return GetWrappedControl<WrappedHtmlGenericControl>("spFixedPurchaseStatus"); } }
	WrappedHtmlGenericControl WspPaymentStatus { get { return GetWrappedControl<WrappedHtmlGenericControl>("spPaymentStatus"); } }
	WrappedRepeater WrItem { get { return GetWrappedControl<WrappedRepeater>("rItem"); } }
	WrappedDropDownList WddlCancelReason { get { return GetWrappedControl<WrappedDropDownList>("ddlCancelReason"); } }
	WrappedTextBox WtbCancelMemo { get { return GetWrappedControl<WrappedTextBox>("tbCancelMemo"); } }
	WrappedTextBox WtbSuspendReason { get { return GetWrappedControl<WrappedTextBox>("tbSuspendReason"); } }
	WrappedDropDownList WddlResumeDateYear { get { return GetWrappedControl<WrappedDropDownList>("ddlResumeDateYear"); } }
	WrappedDropDownList WddlResumeDateMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlResumeDateMonth"); } }
	WrappedDropDownList WddlResumeDateDay { get { return GetWrappedControl<WrappedDropDownList>("ddlResumeDateDay"); } }
	WrappedDropDownList WddlNextShippingDateYear { get { return GetWrappedControl<WrappedDropDownList>("ddlNextShippingDateYear"); } }
	WrappedDropDownList WddlNextShippingDateMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlNextShippingDateMonth"); } }
	WrappedDropDownList WddlNextShippingDateDay { get { return GetWrappedControl<WrappedDropDownList>("ddlNextShippingDateDay"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ログインチェック（ログイン後は定期購入詳細から）
		CheckLoggedIn(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.RequestFixedPurchaseId));

		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		CheckHttps();

		if (!IsPostBack)
		{
			// 定期購入情報セット
			SetFixedPurchaseInfo();

			// 既にキャンセル済みの場合はエラー
			if (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXEDPURCHASE_UNDISP);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 表示コンポーネント初期化
			InitializeComponents();

			// 画面にセット
			SetValues();

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		switch (this.FixedPurchaseNextPageKbn)
		{
			case Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL:
				SetCancelReason();
				break;

			case Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_SUSPEND:
				SetSuspendReason();
				break;
		}
	}

	/// <summary>
	/// 解約理由セット
	/// </summary>
	private void SetCancelReason()
	{
		// 解約理由
		var cancelReasonList = new FixedPurchaseService().GetCancelReasonForPC();

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			cancelReasonList = SetCancelReasonTranslationData(cancelReasonList);
		}
		
		if (cancelReasonList.Length != 0)
		{
			WddlCancelReason.Items.Add(new ListItem(string.Empty, string.Empty));
			WddlCancelReason.Items.AddRange(
				cancelReasonList.Select(item => new ListItem(item.CancelReasonName, item.CancelReasonId)).ToArray());
		}
	}

	/// <summary>
	/// 休止理由セット
	/// </summary>
	private void SetSuspendReason()
	{
		// 定期再開予定日設定
		this.WddlResumeDateYear.Items.Add(string.Empty);
		this.WddlResumeDateYear.AddItems(DateTimeUtility.GetYearListItem(DateTime.Now.Year, DateTime.Now.Year + 10));
		this.WddlResumeDateYear.SelectItemByValue(DateTime.Now.Year.ToString());
		this.WddlResumeDateMonth.Items.Add(string.Empty);
		this.WddlResumeDateMonth.AddItems(DateTimeUtility.GetMonthListItem());
		this.WddlResumeDateMonth.SelectItemByValue(DateTime.Now.Month.ToString());
		this.WddlResumeDateDay.Items.Add(string.Empty);
		this.WddlResumeDateDay.AddItems(DateTimeUtility.GetDayListItem());
		this.WddlResumeDateDay.SelectItemByValue(DateTime.Now.Day.ToString());

		// 次回配送日設定
		this.WddlNextShippingDateYear.Items.Add(string.Empty);
		this.WddlNextShippingDateYear.AddItems(DateTimeUtility.GetYearListItem(DateTime.Now.Year, DateTime.Now.Year + 10));
		this.WddlNextShippingDateMonth.Items.Add(string.Empty);
		this.WddlNextShippingDateMonth.AddItems(DateTimeUtility.GetMonthListItem());
		this.WddlNextShippingDateDay.Items.Add(string.Empty);
		this.WddlNextShippingDateDay.AddItems(DateTimeUtility.GetDayListItem());
	}

	/// <summary>
	/// 画面に値をセット
	/// </summary>
	private void SetValues()
	{
		// 各ステータスCSS制御
		this.WspFixedPurchaseStatus.Attributes["class"] = "fixedPurchaseStatus_" + this.FixedPurchaseContainer.FixedPurchaseStatus;
		this.WspPaymentStatus.Attributes["class"] = "paymentStatus_" + this.FixedPurchaseContainer.PaymentStatus;

		// 定期購入商品リピーターにセット
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		this.WrItem.DataSource = input.Shippings[0].Items;
	}

	/// <summary>
	/// クリアボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnResumeDateClear_Click(object sender, EventArgs e)
	{
		this.WddlResumeDateYear.SelectItemByValue(string.Empty);
		this.WddlResumeDateMonth.SelectItemByValue(string.Empty);
		this.WddlResumeDateDay.SelectItemByValue(string.Empty);
		
	}

	/// <summary>
	/// キャンセルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e)
	{
		// 入力内容をセット
		var input = new FixedPurchaseCancelReasonInput()
		{
			CancelMemo = WtbCancelMemo.Text
		};
		if (WddlCancelReason.Items.Count != 0)
		{
			input.CancelReasonId = WddlCancelReason.SelectedValue;
			input.CancelReasonName = WddlCancelReason.SelectedItem.Text;
		}

		// エラーチェック＆カスタムバリデータへセット
		var errorMessages = input.Validate();
		if (errorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("FixedPurchaseModifyInput", errorMessages, customValidators);

			return;
		}
		
		// 入力情報をセット
		this.CancelReasonInput = input;
		this.SuspendReasonInput = new FixedPurchaseSuspendReasonInput();

		// 確認画面へ遷移
		Response.Redirect(CreateFixedPurchaseCancelReasonConfirmUrl(
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL));
	}

	/// <summary>
	/// 解約理由翻訳情報設定
	/// </summary>
	/// <param name="cancelReasonList">解約理由リスト</param>
	/// <returns>解約理由リスト</returns>
	private FixedPurchaseCancelReasonModel[] SetCancelReasonTranslationData(FixedPurchaseCancelReasonModel[] cancelReasonList)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON,
			MasterId1List = cancelReasonList.Select(cancelReason => cancelReason.CancelReasonId).ToList(),
			LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
			LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
		};
		var translationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

		if (translationSettings.Any() == false) return cancelReasonList;

		cancelReasonList = cancelReasonList.Select(cancelReason => SetCancelReasonTranslationData(cancelReason, translationSettings)).ToArray();
		return cancelReasonList;
	}
	/// <summary>
	/// 解約理由翻訳情報設定
	/// </summary>
	/// <param name="cancelReason">解約理由</param>
	/// <param name="translationSettings">翻訳設定情報</param>
	/// <returns>解約理由</returns>
	private FixedPurchaseCancelReasonModel SetCancelReasonTranslationData(FixedPurchaseCancelReasonModel cancelReason, NameTranslationSettingModel[] translationSettings)
	{
		var translationSetting = translationSettings.FirstOrDefault(setting => (setting.MasterId1 == cancelReason.CancelReasonId));
		cancelReason.CancelReasonName = (translationSetting != null)
			? translationSetting.AfterTranslationalName
			: cancelReason.CancelReasonName;

		return cancelReason;
	}

	/// <summary>
	/// 休止ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSuspendConfirm_Click(object sender, EventArgs e)
	{
		//入力内容セット
		var input = SetSuspendReasonInput();
		// エラーチェック＆カスタムバリデータへセット
		var errorMessages = input.Validate();
		if (errorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("FixedPurchaseModifyInput", errorMessages, customValidators);

			return;
		}

		// 入力情報をセット
		this.SuspendReasonInput = input;
		this.CancelReasonInput = new FixedPurchaseCancelReasonInput();

		// 確認画面へ遷移
		Response.Redirect(CreateFixedPurchaseCancelReasonConfirmUrl(
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_SUSPEND));
	}

	/// <summary>
	/// 定期休止：入力内容セット
	/// </summary>
	/// <returns>入力内容</returns>
	private FixedPurchaseSuspendReasonInput SetSuspendReasonInput()
	{
		// 入力内容をセット
		var input = new FixedPurchaseSuspendReasonInput()
		{
			ResumeDateYear = this.WddlResumeDateYear.SelectedText,
			ResumeDateMonth = this.WddlResumeDateMonth.SelectedText,
			ResumeDateDay = this.WddlResumeDateDay.SelectedText,
			NextShippingDateYear = this.WddlNextShippingDateYear.SelectedText,
			NextShippingDateMonth = this.WddlNextShippingDateMonth.SelectedText,
			NextShippingDateDay = this.WddlNextShippingDateDay.SelectedText,
			SuspendReason = this.WtbSuspendReason.Text
		};

		// 定期再開予定日をセット
		if (string.IsNullOrEmpty(
			this.WddlResumeDateYear.SelectedValue
				+ this.WddlResumeDateMonth.SelectedValue
				+ this.WddlResumeDateDay.SelectedValue) == false)
		{
			input.ResumeDate =
				this.WddlResumeDateYear.SelectedValue + "/" + this.WddlResumeDateMonth.SelectedValue + "/" + this.WddlResumeDateDay.SelectedValue;
		}
		else
		{
			input.ResumeDate = null;
		}

		// 次回配送日をセット
		if (string.IsNullOrEmpty(
			this.WddlNextShippingDateYear.SelectedValue
				+ this.WddlNextShippingDateMonth.SelectedValue
				+ this.WddlNextShippingDateDay.SelectedValue) == false)
		{
			input.NextShippingDate =
				this.WddlNextShippingDateYear.SelectedValue + "/" + this.WddlNextShippingDateMonth.SelectedValue + "/" + this.WddlNextShippingDateDay.SelectedValue;
		}
		else
		{
			input.NextShippingDate = null;
		}

		return input;
	}

	/// <summary>配送先の国が日本か</summary>
	protected bool IsShippingAddrCountryJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode);  }
	}
}