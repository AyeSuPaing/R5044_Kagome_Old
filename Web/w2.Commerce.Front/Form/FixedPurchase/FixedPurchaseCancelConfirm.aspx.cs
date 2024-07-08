/*
=========================================================================================================
  Module      : 定期購入情報解約確認画面処理(FixedPurchaseCancelConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.SendMail;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_FixedPurchase_FixedPurchaseCancelConfirm : FixedPurchasePage
{
	# region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WspFixedPurchaseStatus { get { return GetWrappedControl<WrappedHtmlGenericControl>("spFixedPurchaseStatus"); } }
	WrappedHtmlGenericControl WspPaymentStatus { get { return GetWrappedControl<WrappedHtmlGenericControl>("spPaymentStatus"); } }
	WrappedRepeater WrItem { get { return GetWrappedControl<WrappedRepeater>("rItem"); } }
	protected WrappedLiteral WlCancelPaypayNotification { get { return GetWrappedControl<WrappedLiteral>("lCancelPaypayNotification"); } }
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

			// 既にキャンセル済み/休止済みの場合はエラー
			if (((this.FixedPurchaseNextPageKbn == Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL)
					&& this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus)
				|| ((this.FixedPurchaseNextPageKbn == Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_SUSPEND)
					&& this.FixedPurchaseContainer.IsSuspendFixedPurchaseStatus))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXEDPURCHASE_UNDISP);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 画面にセット
			SetValues();

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// キャンセルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdate_Click(object sender, EventArgs e)
	{
		if (Constants.CROSS_POINT_OPTION_ENABLED
			&& (this.FixedPurchaseContainer.NextShippingUsePoint > 0))
		{
			var errorMessage = FixedPurchaseHelper.UpdateCrossPointApiUserPoint(
				this.FixedPurchaseContainer,
				this.LoginUser);

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(
					PageUrlCreatorUtility.CreateErrorPageUrl(
						PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(
							this.RequestFixedPurchaseId)));
			}
		}

		// 継続課金の解約を行う
		string apiError;
		var success = FixedPurchaseHelper.CancelPaymentContinuous(
			this.FixedPurchaseContainer.FixedPurchaseId,
			this.FixedPurchaseContainer.OrderPaymentKbn,
			this.FixedPurchaseContainer.ExternalPaymentAgreementId,
			Constants.FLG_LASTCHANGED_USER,
			out apiError);

		// 定期購入を解約する
		success = success && new FixedPurchaseService().CancelFixedPurchase(
			this.FixedPurchaseContainer,
			this.CancelReasonInput.CancelReasonId ?? string.Empty,
			this.CancelReasonInput.CancelMemo ?? string.Empty,
			Constants.FLG_LASTCHANGED_USER,
			Constants.CONST_DEFAULT_DEPT_ID,
			Constants.W2MP_POINT_OPTION_ENABLED,
			UpdateHistoryAction.Insert);

		if (success)
		{
			if (Constants.CROSS_POINT_OPTION_ENABLED
				&& (this.FixedPurchaseContainer.NextShippingUsePoint > 0))
			{
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
			}

			// 最新のユーザポイントを取得
			this.LoginUserPoint = PointOptionUtility.GetUserPoint((string)this.LoginUserId);
			// メール送信
			SendMailCommon.SendModifyFixedPurchaseMail(this.FixedPurchaseContainer.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.OrderCancell);
			// 解約完了画面へ
			Response.Redirect(CreateFixedPurchaseCancelReasonCompleteUrl(
				this.RequestFixedPurchaseId,
				Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL));
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.IsNullOrEmpty(apiError)
				? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_CANCEL_ALERT)
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CANCEL_FP);
			Response.Redirect(PageUrlCreatorUtility.CreateErrorPageUrl(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.RequestFixedPurchaseId)));
		}
	}

	/// <summary>
	/// 休止ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSuspend_Click(object sender, EventArgs e)
	{
		var service = new FixedPurchaseService();

		// 定期再開予定日を文字列から日付型に変換
		DateTime date;
		DateTime? resumeDate = null;
		if (DateTime.TryParse(this.SuspendReasonInput.ResumeDate, out date)) resumeDate = date;

		// 次回配送日を文字列から日付型に変換
		DateTime? nextShippingDate = null;
		if (DateTime.TryParse(this.SuspendReasonInput.NextShippingDate, out date)) nextShippingDate = date;

		// 次々回配送日を計算
		DateTime? nextNextShippingDate = null;
		if (nextShippingDate != null)
		{
			var calculateMode = service.GetCalculationMode(
				this.FixedPurchaseContainer.FixedPurchaseKbn,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			nextNextShippingDate = service.CalculateNextShippingDate(
				this.FixedPurchaseContainer.FixedPurchaseKbn,
				this.FixedPurchaseContainer.FixedPurchaseSetting1,
				nextShippingDate,
				this.ShopShipping.FixedPurchaseShippingDaysRequired,
				this.ShopShipping.FixedPurchaseMinimumShippingSpan,
				calculateMode);
		}

		// 定期休止処理を行う
		var success = service.SuspendFixedPurchase(
			this.FixedPurchaseContainer,
			resumeDate,
			nextShippingDate,
			nextNextShippingDate,
			this.SuspendReasonInput.SuspendReason,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		if (success)
		{
			// メール送信
			SendMailCommon.SendModifyFixedPurchaseMail(
				this.FixedPurchaseContainer.FixedPurchaseId,
				SendMailCommon.FixedPurchaseModify.Suspend);
			// 休止・解約完了画面へ
			Response.Redirect(CreateFixedPurchaseCancelReasonCompleteUrl(
				this.RequestFixedPurchaseId,
				Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_SUSPEND));
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_SUSPEND_ALERT);
			Response.Redirect(
				PageUrlCreatorUtility.CreateErrorPageUrl(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.RequestFixedPurchaseId)));
		}
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

		// 解約理由区分の名称切り替え
		if (this.FixedPurchaseNextPageKbn.Equals(Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL)
			&& Constants.GLOBAL_OPTION_ENABLE)
		{
			var cancelReason = DataCacheControllerFacade.GetFixedPurchaseCancelReasonCacheController().GetCancelReason(this.CancelReasonInput.CancelReasonId);
			this.CancelReasonInput.CancelReasonName = (string.IsNullOrEmpty(this.CancelReasonInput.CancelReasonId) == false)
				? NameTranslationCommon.GetTranslationName(
					this.CancelReasonInput.CancelReasonId,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON,
					Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME,
					cancelReason.CancelReasonName)
				: string.Empty;
		}

		if (this.IsInvalidResumePaypay)
		{
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case w2.App.Common.Constants.PaymentPayPayKbn.GMO:
					WlCancelPaypayNotification.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_CANCEL_PAYPAY_GMO_MESSAGE);
					break;

				case w2.App.Common.Constants.PaymentPayPayKbn.VeriTrans:
					WlCancelPaypayNotification.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PURCHASE_CANCEL_PAYMENT_PAPAY_VERITRANS_MESSAGE);
					break;
			}
		}
	}

	/// <summary>
	/// 定期購入キャンセル理由が入力されているか
	/// </summary>
	protected bool IsSetCancelReasonInput
	{
		get
		{
			return ((string.IsNullOrEmpty(this.CancelReasonInput.CancelReasonId) == false)
				&& (string.IsNullOrEmpty(this.CancelReasonInput.CancelReasonName) == false)
				&& (string.IsNullOrEmpty(this.CancelReasonInput.CancelMemo) == false));
		}
	}
	/// <summary>再開不可のPaypay決済か</summary>
	public bool IsInvalidResumePaypay
	{
		get
		{
			return ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& ((Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
					|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans)));
		}
	}
}
