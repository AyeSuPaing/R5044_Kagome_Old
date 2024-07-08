/*
=========================================================================================================
  Module      : 注文共通ページ アクション部分(OrderPage_Action.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.User;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;

/// <summary>
/// OrderPage_Action の概要の説明です
/// </summary>
public partial class OrderPage : ProductPage
{
	/// <summary>
	/// 注文情報キャンセル（ステータス・外部連携以外更新）
	/// </summary>
	/// <param name="drvOrder">注文情報</param>
	/// <param name="updateTwInvoiceStatus">電子発票更新ステータス</param>
	/// <param name="blRollBackRealStock">実在庫ロールバックするか</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	public void CancelOrder(
		DataRowView drvOrder,
		string updateTwInvoiceStatus,
		bool blRollBackRealStock,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		OrderCommon.CancelOrder(
			drvOrder,
			blRollBackRealStock,
			Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL,
			this.LoginOperatorDeptId,
			this.LoginOperatorName,
			updateHistoryAction,
			sqlAccessor,
			updateTwInvoiceStatus);
	}
	/// <summary>
	/// 注文情報キャンセル（ステータス・外部連携以外更新）
	/// </summary>
	/// <param name="drvOrder">注文情報</param>
	/// <param name="blRollBackRealStock">実在庫ロールバックするか</param>
	/// <param name="strProductStockHistoryActionStatus">商品在庫履歴区分</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	public void CancelOrder(
		DataRowView drvOrder,
		bool blRollBackRealStock,
		string strProductStockHistoryActionStatus,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		OrderCommon.CancelOrder(
			drvOrder,
			blRollBackRealStock,
			strProductStockHistoryActionStatus,
			this.LoginOperatorDeptId,
			this.LoginOperatorName,
			updateHistoryAction,
			sqlAccessor);
	}

	/// <summary>
	/// 実在庫の引当を行う(注文商品情報の実在庫引当済み商品数に引当、商品在庫情報の引当済実在庫数を加算する)
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>実行結果</returns>
	protected ResultKbn UpdateOrderItemRealStockReserved(
		string strOrderId,
		string strShopId,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		return UpdateOrderItemRealStockReserved(
			strOrderId,
			strShopId,
			this.LoginOperatorName,
			updateHistoryAction,
			sqlAccessor);
	}

	/// <summary>
	/// 実在庫の引当を行う(注文商品情報の実在庫引当済み商品数に引当、商品在庫情報の引当済実在庫数を加算する)
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strLoginOperatorName">ログインオペレータ名</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>実行結果</returns>
	public static ResultKbn UpdateOrderItemRealStockReserved(
		string strOrderId,
		string strShopId,
		string strLoginOperatorName,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		OrderCommon.ResultKbn result = OrderCommon.UpdateOrderItemRealStockReserved(
			strOrderId,
			strShopId,
			strLoginOperatorName,
			updateHistoryAction,
			sqlAccessor);

		return (ResultKbn)Enum.Parse(typeof(ResultKbn), result.ToString());
	}

	/// <summary>
	/// 注文商品実在庫の出荷を行う(出荷する商品数分、実在庫、引当済実在庫数を減算する)
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	protected ResultKbn UpdateOrderItemRealStockShipped(
		string strOrderId,
		string strShopId,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		return (ResultKbn)OrderCommon.UpdateOrderItemRealStockShipped(
			strOrderId,
			strShopId,
			this.LoginOperatorName,
			updateHistoryAction,
			sqlAccessor);
	}

	/// <summary>
	/// 実在庫の引当戻しを行う
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
	protected ResultKbn UpdateOrderItemRealStockCanceled(
		string strOrderId,
		string strShopId,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		return UpdateOrderItemRealStockCanceled(strOrderId, strShopId, this.LoginOperatorName, updateHistoryAction, sqlAccessor);
	}

	/// <summary>
	/// 実在庫の引当戻しを行う
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strLoginOperatorName">ログインオペレータ名</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
	protected static ResultKbn UpdateOrderItemRealStockCanceled(
		string strOrderId,
		string strShopId,
		string strLoginOperatorName,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		OrderCommon.ResultKbn result = OrderCommon.UpdateOrderItemRealStockCanceled(
			strOrderId,
			strShopId,
			strLoginOperatorName,
			updateHistoryAction,
			sqlAccessor);

		return (ResultKbn)Enum.Parse(typeof(ResultKbn), result.ToString());
	}

	/// <summary>
	/// 論理在庫のキャンセルを行う
	/// </summary>
	/// <param name="drvOrder">注文情報</param>
	/// <param name="strProductStockHistoryActionStatus"></param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	protected void UpdateProductStockCancel(DataRowView drvOrder, string strProductStockHistoryActionStatus, SqlAccessor sqlAccessor)
	{
		OrderCommon.UpdateProductStockCancel(drvOrder, strProductStockHistoryActionStatus, this.LoginOperatorName, sqlAccessor);
	}

	/// <summary>
	/// 注文画面向けクレジットカード入力情報取得（PayPalの情報も取得）
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <returns>クレジットカード入力情報</returns>
	protected OrderCreditCardInput GetOrderCreditCardInputForOrderPage(string paymentId, string userId)
	{
		// 完全再与信非対応対応（ZEUS以外）のクレジットカードは抜ける
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false)) return null;

		var cardInput = new CommonPageProcess.WrappedCreditCardInputs(this);
		var orderCreditCardInput = GetOrderCreditCardInput(userId, cardInput, cardInput.WcbRegistCreditCard.Checked);
		return orderCreditCardInput;
	}

	/// <summary>
	/// 定期画面向けクレジットカード入力情報取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>クレジットカード入力情報</returns>
	protected OrderCreditCardInput GetOrderCreditCardInputForFixedPurchasePage(string userId)
	{
		var cardInput = CreateWrappedCreditCardInputsForFixedPurchase();

		var orderCreditCardInput = GetOrderCreditCardInput(userId, cardInput, cardInput.WcbRegistCreditCard.Checked);
		return orderCreditCardInput;
	}

	/// <summary>
	/// ユーザークレジットカード登録画面向けクレジットカード入力情報取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="processMode">プロセスモード</param>
	/// <returns>クレジットカード入力情報</returns>
	protected OrderCreditCardInput GetOrderCreditCardInputForUserCreditCardPage(
		string userId,
		UserCreditCardPage.ProcessModeType processMode)
	{
		var cardInput = new CommonPageProcess.WrappedCreditCardInputs(this);
		cardInput.WddlUserCreditCard.SelectedValue = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
		if (processMode == UserCreditCardPage.ProcessModeType.UserCreditCartdRegister)
		{
			cardInput.WcbRegistCreditCard.Checked = true;
		}

		var doRegister = ((processMode == UserCreditCardPage.ProcessModeType.UserCreditCartdRegister)
			|| cardInput.WcbRegistCreditCard.Checked);

		var orderCreditCardInput = GetOrderCreditCardInput(userId, cardInput, doRegister);
		return orderCreditCardInput;
	}

	/// <summary>
	/// 定期画面向けラップ済みクレジットカード入力クラス作成
	/// </summary>
	/// <returns>ラップ済みクレジットカード入力クラス</returns>
	private CommonPageProcess.WrappedCreditCardInputs CreateWrappedCreditCardInputsForFixedPurchase()
	{
		var dvFixedPurchasePayment = GetWrappedControl<WrappedHtmlGenericControl>("dvFixedPurchasePayment");
		var cardInput = new CommonPageProcess.WrappedCreditCardInputs(this, dvFixedPurchasePayment);

		// 新クレカ
		var wrbNewCreditCard = GetWrappedControl<WrappedRadioButton>("rbNewCreditCard");
		if (wrbNewCreditCard.Checked)
		{
			cardInput.WddlUserCreditCard.SelectedValue = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
			return cardInput;
		}

		// 登録済みクレカ
		var wrbRegisteredCreditCard = GetWrappedControl<WrappedRadioButton>("rbRegisteredCreditCard");
		if (wrbRegisteredCreditCard.Checked)
		{
			var wddlRegisteredCreditCardInstallments = GetWrappedControl<WrappedDropDownList>("ddlRegisteredCreditCardInstallments");
			cardInput.WddlInstallments.SelectedValue = wddlRegisteredCreditCardInstallments.SelectedValue;
			return cardInput;
		}

		// 現在のクレカ
		var wrbUseNowCreditCard = GetWrappedControl<WrappedRadioButton>("rbUseNowCreditCard");
		if (wrbUseNowCreditCard.Checked)
		{
			var wddlUseNowCreditCardInstallments = GetWrappedControl<WrappedDropDownList>("ddlUseNowCreditCardInstallments");
			cardInput.WddlInstallments.SelectedValue = wddlUseNowCreditCardInstallments.SelectedValue;
			return cardInput;
		}

		throw new Exception("チェックボックスがチェックされていませんでした。");
	}

	/// <summary>
	/// クレジットカード入力情報取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="cardInput">ラップ済みクレジットカード入力クラス</param>
	/// <param name="doRegister">登録するか</param>
	/// <returns>クレジットカード入力情報</returns>
	private OrderCreditCardInput GetOrderCreditCardInput(string userId, CommonPageProcess.WrappedCreditCardInputs cardInput, bool doRegister)
	{
		// 完全再与信対応のクレジットカード以外は抜ける
		if (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false) return null;

		// 新しいカード登録？
		var branchNo = cardInput.WddlUserCreditCard.SelectedValue;
		var cardNoInput = string.Format(
			"{0}{1}{2}{3}",
			cardInput.WtbCard1.Text,
			cardInput.WtbCard2.Text,
			cardInput.WtbCard3.Text,
			cardInput.WtbCard4.Text);
		var bincode = ((OrderCommon.CreditTokenUse == false)
				&& (cardNoInput.Length > 6))
			? cardNoInput.Substring(0, 6)
			: cardInput.WhfCreditBincode.Value;

		if (string.IsNullOrEmpty(branchNo)) branchNo = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;	// バインドされていないことがあるため
		if (branchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
		{
			return new OrderCreditCardInput
			{
				CreditBranchNo = branchNo,
				CompanyCode = (OrderCommon.CreditCompanySelectable) ? cardInput.WddlCardCompany.SelectedValue : "",
				CardNo = StringUtility.ToHankaku(cardInput.WtbCard1.Text + cardInput.WtbCard2.Text + cardInput.WtbCard3.Text + cardInput.WtbCard4.Text),
				CardNo1 = StringUtility.ToHankaku(cardInput.WtbCard1.Text),
				CardNo2 = StringUtility.ToHankaku(cardInput.WtbCard2.Text),
				CardNo3 = StringUtility.ToHankaku(cardInput.WtbCard3.Text),
				CardNo4 = StringUtility.ToHankaku(cardInput.WtbCard4.Text),
				ExpireMonth = cardInput.WddlExpireMonth.SelectedValue,
				ExpireYear = cardInput.WddlExpireYear.SelectedValue,
				AuthorName = StringUtility.ToHankaku(cardInput.WtbAuthorName.Text.Trim()),
				SecurityCode = (OrderCommon.CreditSecurityCodeEnable) ? StringUtility.ToHankaku(cardInput.WtbSecurityCode.Text).Trim() : null,
				InstallmentsCode = (OrderCommon.CreditInstallmentsSelectable) ? cardInput.WddlInstallments.SelectedValue : string.Empty,
				InstallmentsName = (OrderCommon.CreditInstallmentsSelectable) ? cardInput.WddlInstallments.SelectedText : string.Empty,
				DoRegister = doRegister,
				RegisterCardName = cardInput.WcbRegistCreditCard.Checked ? cardInput.WtbUserCreditCardName.Text : Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME,
				CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(cardInput.WhfCreditToken.Value),
				CreditBincode = bincode,
			};
		}
		// 登録カード？
		else
		{
			var userCreditCard = new UserCreditCardService().Get(userId, int.Parse(branchNo));
			return new OrderCreditCardInput
			{
				CreditBranchNo = branchNo,
				CompanyCode = (OrderCommon.CreditCompanySelectable) ? userCreditCard.CompanyCode : "",
				CardNo = StringUtility.ToHankaku(userCreditCard.LastFourDigit),
				CardNo4 = StringUtility.ToHankaku(userCreditCard.LastFourDigit),
				ExpireMonth = userCreditCard.ExpirationMonth,
				ExpireYear = userCreditCard.ExpirationYear,
				AuthorName = StringUtility.ToHankaku(userCreditCard.AuthorName),
				SecurityCode = (OrderCommon.CreditSecurityCodeEnable) ? StringUtility.ToHankaku(cardInput.WtbSecurityCode.Text).Trim() : null,
				InstallmentsCode = (OrderCommon.CreditInstallmentsSelectable) ? cardInput.WddlInstallments.SelectedValue : string.Empty,
				InstallmentsName = (OrderCommon.CreditInstallmentsSelectable) ? cardInput.WddlInstallments.SelectedText : string.Empty,
				DoRegister = false,
				RegisterCardName = userCreditCard.CardDispName,
				CreditToken = null,
			};
		}
	}

	/// <summary>
	/// ユーザークレジットカード登録実行
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="orderCreditCardInput">クレジットカード入力情報</param>
	/// <param name="setDispFlg">表示フラグをオンにするか</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>クレジットカード登録枝番（登録しなかった場合はnull）</returns>
	protected Result RegisterUserCreditCard(
		string userId,
		OrderCreditCardInput orderCreditCardInput,
		bool setDispFlg,
		UpdateHistoryAction updateHistoryAction)
	{
		Result result = null;
		UserCreditCardInput userCreditCardInput = null;
		try
		{
			userCreditCardInput = orderCreditCardInput.CeateUserCreditCardInput(userId);
			var userCreditCardRegister = new UserCreditCardRegister();

			if (this.IsUserPayTg)
			{
				result = userCreditCardRegister.ExecForPayTg(
					userCreditCardInput,
					SiteKbn.Pc,
					setDispFlg,
					this.LoginOperatorName,
					updateHistoryAction);
			}
			else
			{
				result = userCreditCardRegister.Exec(
					userCreditCardInput,
					SiteKbn.Pc,
					setDispFlg,
					this.LoginOperatorName,
					updateHistoryAction);
			}

			if (result.Success == false)
			{
				this.ApiErrorMessage = result.ErrorMessage;
				throw new Exception(result.ErrorMessage);
			}

			return result;
		}
		catch (Exception ex)
		{
			// ログファイル書き込み処理
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Unknown,
				PaymentFileLogger.PaymentProcessingType.CreatePaypalCustomer,
				BaseLogger.CreateExceptionMessage(ex),
				new Dictionary<string, string>
				{
					{Constants.FIELD_USER_USER_ID, userId}
				});
		}
		return null;
	}

	/// <summary>
	/// トークンが入力されていたら入力画面を切り替える
	/// </summary>
	protected virtual void SwitchDisplayForCreditTokenInput()
	{
		var cardInput = new CommonPageProcess.WrappedCreditCardInputs(this);

		if (OrderCommon.CreditTokenUse == false)
		{
			cardInput.WdivCreditCardForTokenAcquired.Visible = false;
			return;
		}

		if (HasCreditToken())
		{
			var tokenInfo = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(cardInput.WhfCreditToken.Value);
			if ((tokenInfo != null) && (tokenInfo.ExpireDate < DateTime.Now))
			{
				cardInput.WhfCreditToken.Value = "";
				cardInput.WtbCard1.Text = cardInput.WtbCard2.Text = cardInput.WtbCard3.Text = "";
				cardInput.WtbSecurityCode.Text = "";
			}
		}

		// 表示切り替え
		cardInput.WdivCreditCardNoToken.Visible = (HasCreditToken() == false);
		cardInput.WdivCreditCardForTokenAcquired.Visible = HasCreditToken();

		var cardNo = (cardInput.WtbCard1.Text.Trim()
			+ cardInput.WtbCard2.Text.Trim()
			+ cardInput.WtbCard3.Text.Trim()
			+ cardInput.WtbCard4.Text.Trim());
		var lastFourDigit = (cardNo.Length < 4)
			? cardNo
			: cardNo.Substring(cardNo.Length - 4, 4);
		cardInput.WlCreditCardCompanyNameForTokenAcquired.Text = WebSanitizer.HtmlEncode(cardInput.WddlCardCompany.SelectedText);
		cardInput.WlLastFourDigitForTokenAcquired.Text = WebSanitizer.HtmlEncode(lastFourDigit);
		cardInput.WlExpirationMonthForTokenAcquired.Text = WebSanitizer.HtmlEncode(cardInput.WddlExpireMonth.SelectedValue);
		cardInput.WlExpirationYearForTokenAcquired.Text = WebSanitizer.HtmlEncode(cardInput.WddlExpireYear.SelectedValue);
		cardInput.WlCreditAuthorNameForTokenAcquired.Text = WebSanitizer.HtmlEncode(cardInput.WtbAuthorName.Text);
	}

	/// <summary>
	/// APIエラーメッセージ
	/// </summary>
	public string ApiErrorMessage { get; set; }
}
