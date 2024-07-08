/*
=========================================================================================================
  Module      : 履歴詳細画面処理(HistoryPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Domain.Payment;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

/// <summary>
/// 履歴詳細画面処理
/// </summary>
public class HistoryPage : OrderCartPage
{
	/// <summary>
	/// クレジットトークン取得＆フォームセットJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected new string CreateGetCreditTokenAndSetToFormJsScript()
	{
		var script = CreateGetCreditTokenAndSetToFormJsScriptInner(this.CreditRepeaterItem);
		return script;
	}

	/// <summary>
	/// クレジットトークン向け フォームマスキングJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected new string CreateMaskFormsForCreditTokenJsScript()
	{
		var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner(this.CreditRepeaterItem);
		return maskingScripts;
	}

	# region ラップ済みコントロール宣言
	/// <summary>支払い方法リピーター</summary>
	protected WrappedRepeater WrPayment
	{
		get { return GetWrappedControl<WrappedRepeater>("rPayment"); }
	}
	/// <summary>支払い方法エラーメッセージ</summary>
	protected WrappedHtmlGenericControl WsErrorMessagePayment
	{
		get { return GetWrappedControl<WrappedHtmlGenericControl>("sErrorMessagePayment"); }
	}
	/// <summary>クレジットカード入力クラス</summary>
	protected CommonPageProcess.WrappedCreditCardInputs WciCardInputs
	{
		get { return m_wciCardInputs ?? (m_wciCardInputs = new CommonPageProcess.WrappedCreditCardInputs(this.Process, this, this.CreditRepeaterItem)); }
	}
	private CommonPageProcess.WrappedCreditCardInputs m_wciCardInputs;
	/// <summary>クレジットカードのリピーターアイテム</summary>
	protected RepeaterItem CreditRepeaterItem
	{
		get { return m_creditRepeaterItem ?? (m_creditRepeaterItem = GetCreditRepeaterItem()); }
	}
	private RepeaterItem m_creditRepeaterItem;
	/// <summary>Paidy token hidden field control</summary>
	protected new WrappedHiddenField WhfPaidyTokenId
	{ 
		get { return GetWrappedControl<WrappedHiddenField>("hfPaidyTokenId"); }
	}
	/// <summary>Paidy pay seleced hidden field control</summary>
	protected new WrappedHiddenField WhfPaidyPaySelected
	{
		get { return GetWrappedControl<WrappedHiddenField>("hfPaidyPaySelected"); }
	}
	/// <summary>Atone Repeater Item</summary>
	protected RepeaterItem AtoneRepeaterItem
	{
		get { return m_atoneRepeaterItem ?? (m_atoneRepeaterItem = GetAtoneRepeaterItem()); }
	}
	private RepeaterItem m_atoneRepeaterItem;
	/// <summary>Aftee Repeater Item</summary>
	protected RepeaterItem AfteeRepeaterItem
	{
		get { return m_afteeRepeaterItem ?? (m_afteeRepeaterItem = GetAfteeRepeaterItem()); }
	}
	private RepeaterItem m_afteeRepeaterItem;
	/// <summary>Np After Pay Repeater Item</summary>
	protected RepeaterItem NPAfterPayRepeaterItem
	{
		get { return m_npAfterPayRepeaterItem ?? (m_npAfterPayRepeaterItem = GetNPAfterPayRepeaterItem()); }
	}
	private RepeaterItem m_npAfterPayRepeaterItem;
	/// <summary>Payment Repeater Item</summary>
	protected RepeaterItem PaymentRepeaterItem
	{
		get { return this.AfteeRepeaterItem
				?? this.AtoneRepeaterItem
				?? this.NPAfterPayRepeaterItem; }
	}
	/// <summary>楽天クレカユーザーコントロール</summary>
	protected WrappedControl RakutenCreditCardControll
	{
		get { return GetWrappedControl<WrappedControl>(this.CreditRepeaterItem, "ucRakutenCreditCard"); }
	}
	#endregion

	/// <summary>
	/// クレジットカードフォームのコンポーネントをセット
	/// </summary>
	/// <param name="orderPaymentKbn">支払い区分</param>
	/// <param name="creditBranchNo">クレジットカード枝番</param>
	/// <param name="cardInstallmentsCode">カード支払い回数コード</param>
	protected void SetCreditCardComponents(string orderPaymentKbn, string creditBranchNo, string cardInstallmentsCode)
	{
		// 有効期限(月)ドロップダウン作成
		this.WciCardInputs.WddlExpireMonth.AddItems(DateTimeUtility.GetCreditMonthListItem());
		this.WciCardInputs.WddlExpireMonth.SelectedValue = DateTime.Now.Month.ToString("00");
		// 有効期限(年)ドロップダウン作成
		this.WciCardInputs.WddlExpireYear.AddItems(DateTimeUtility.GetCreditYearListItem());
		this.WciCardInputs.WddlExpireYear.SelectedValue = DateTime.Now.Year.ToString("00").Substring(2);

		// 支払回数ドロップダウン作成
		this.WciCardInputs.WddlInstallments.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditInstallmentsValueTextFieldName));
		this.WciCardInputs.WdllCreditInstallments2.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditInstallmentsValueTextFieldName));
		var installments = new ListItemCollection();
		installments.AddRange(ValueText.GetValueItemArray(
			Constants.TABLE_ORDER,
			OrderCommon.CreditInstallmentsValueTextFieldName));
		this.CreditInstallmentsList = installments;
		this.RakutenCreditCardControll.DataBind();

		// カード会社リスト作成
		if (OrderCommon.CreditCompanySelectable)
		{
			this.WciCardInputs.WddlCardCompany.AddItems(
			ValueText.GetValueItemList(
				Constants.TABLE_ORDER,
				OrderCommon.CreditCompanyValueTextFieldName).Cast<ListItem>().ToArray());
		}

		// 登録クレジットカードドロップダウン作成（登録可能な場合、ユーザカード種別取得）
		if (Constants.MAX_NUM_REGIST_CREDITCARD > 0)
		{
			var userCreditCards = UserCreditCard.GetUsable(this.LoginUserId);
			foreach (var userCreditCard in userCreditCards)
			{
				this.WciCardInputs.WddlUserCreditCard.AddItem(new ListItem(userCreditCard.CardDispName, userCreditCard.BranchNo.ToString()));
			}
			var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(this.CreditRepeaterItem, "cbRegistCreditCard");
			wcbRegistCreditCard.Visible = OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, userCreditCards.Length);
		}

		this.WciCardInputs.WddlUserCreditCard.AddItem(
			new ListItem(
				ReplaceTag("@@DispText.credit_card_list.new@@"),
				CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW));

		// 利用しているカードの選択
		if (StringUtility.ToEmpty(orderPaymentKbn) == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT
			&& string.IsNullOrEmpty(StringUtility.ToEmpty(creditBranchNo)) == false)
		{
			var model = new UserCreditCardService().Get(this.LoginUserId, int.Parse(creditBranchNo));
			this.WciCardInputs.WddlUserCreditCard.SelectedValue = (model.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON)
				? creditBranchNo
				: CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
		}

		// カード支払い回数
		var wtrInstallments = GetWrappedControl<WrappedHtmlGenericControl>(this.CreditRepeaterItem, "trInstallments");
		wtrInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
		this.WciCardInputs.WddlInstallments.SelectedValue = GetListItemValue(this.WciCardInputs.WddlInstallments.Items, cardInstallmentsCode);

		// セキュリティコードの表示・非表示
		var wtrSecurityCode = GetWrappedControl<WrappedHtmlGenericControl>(this.CreditRepeaterItem, "trSecurityCode");
		wtrSecurityCode.Visible = OrderCommon.CreditSecurityCodeEnable;
	}

	/// <summary>
	/// 新規クレジットか
	/// </summary>
	/// <returns>新規クレジット：true、既存クレジット：false</returns>
	protected bool IsNewCreditCard()
	{
		return (this.WciCardInputs.WddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW);
	}

	/// <summary>
	/// （トークン決済向け）カード情報編集リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, System.EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm(this.CreditRepeaterItem, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING);
		RefreshCreditForm();
	}

	/// <summary>
	/// クレジットフォームをリフレッシュ
	/// </summary>
	protected void RefreshCreditForm()
	{
		var isRakutenCredit = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten;
		var wdivRakutenCredit = GetWrappedControl<WrappedHtmlGenericControl>(
			this.CreditRepeaterItem,
			"divRakutenCredit");
		var wdivCreditCardNoToken = GetWrappedControl<WrappedHtmlGenericControl>(
			this.CreditRepeaterItem,
			"divCreditCardNoToken");
		var wdivCreditCardForTokenAcquired = GetWrappedControl<WrappedHtmlGenericControl>(
			this.CreditRepeaterItem,
			"divCreditCardForTokenAcquired");
		var wspanErrorMessageForCreditCard = GetWrappedControl<WrappedHtmlGenericControl>(
			this.CreditRepeaterItem,
			"spanErrorMessageForCreditCard");
		wdivRakutenCredit.Visible = isRakutenCredit;
		wdivCreditCardNoToken.Visible = (isRakutenCredit == false) && (HasCreditToken(this.CreditRepeaterItem) == false);
		wdivCreditCardForTokenAcquired.Visible = (isRakutenCredit == false) && HasCreditToken(this.CreditRepeaterItem);
		wspanErrorMessageForCreditCard.InnerHtml = "";
	}

	/// <summary>
	/// クレジットカードの登録処理
	/// </summary>
	/// <param name="creditCardName">登録リストに追加しない場合のクレジットカード名</param>
	/// <param name="errorMessage">out エラーメッセージ</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="apiErrorMessage">APIエラーメッセージ</param>
	/// <returns>登録成功:クレジットカード情報 登録失敗:null</returns>
	protected UserCreditCardInput CreditCardProcessing(
		string creditCardName,
		out string errorMessage,
		out string apiErrorMessage,
		UpdateHistoryAction updateHistoryAction)
	{
		var success = true;
		UserCreditCardInput creditCardInput = null;
		errorMessage = "";
		apiErrorMessage = "";
		// 登録済みカードを使用する?
		if (IsNewCreditCard() == false)
		{
			// クレジットカード枝番をセット
			var model = new UserCreditCardService().Get(
				this.LoginUserId,
				int.Parse(this.WciCardInputs.WddlUserCreditCard.SelectedValue));
			creditCardInput = new UserCreditCardInput(model);
		}
		// カード情報更新?
		else
		{
			var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(this.CreditRepeaterItem, "cbRegistCreditCard");
			// パラメタ格納
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
			{
				this.WciCardInputs.WhfMyCardMount.Value = GetWrappedControl<WrappedHiddenField>(this.RakutenCreditCardControll, "hfMyCardMount").Value;
				this.WciCardInputs.WhfMyCvvMount.Value = GetWrappedControl<WrappedHiddenField>(this.RakutenCreditCardControll ,"hfMyCvvMount").Value;
				var year = GetWrappedControl<WrappedHiddenField>(this.RakutenCreditCardControll, "hfMyExpirationYearMount").Value;
				this.WciCardInputs.WhfMyExpirationYearMount.Value = year.Substring(year.Length - 2, 2);
				this.WciCardInputs.WhfMyExpirationMonthMount.Value = GetWrappedControl<WrappedHiddenField>(this.RakutenCreditCardControll, "hfMyExpirationMonthMount").Value;
				this.WciCardInputs.WhfAuthorCardName.Text = GetWrappedControl<WrappedHiddenField>(this.RakutenCreditCardControll, "hfAuthorNameCard").Value;
				this.WciCardInputs.WhfCreditCardCompany.Value = GetWrappedControl<WrappedHiddenField>(this.RakutenCreditCardControll, "hfCreditCardCompany").Value;

				creditCardInput = new UserCreditCardInput
				{
					UserId = this.LoginUserId,
					CardDispName =
						(wcbRegistCreditCard.InnerControl != null && wcbRegistCreditCard.Checked == false)
							? creditCardName
							: this.WciCardInputs.WtbUserCreditCardName.Text.Trim(),
					CompanyCode = this.WciCardInputs.WhfCreditCardCompany.Value,
					BranchNo = "0",
					CardNo = this.WciCardInputs.WhfMyCardMount.Value,
					CardNo1 = this.WciCardInputs.WhfMyCardMount.Value,
					CardNo2 = this.WciCardInputs.WhfMyCardMount.Value,
					CardNo3 = this.WciCardInputs.WhfMyCardMount.Value,
					CardNo4 = this.WciCardInputs.WhfMyCardMount.Value,
					ExpirationMonth = this.WciCardInputs.WhfMyExpirationMonthMount.Value,
					ExpirationYear = this.WciCardInputs.WhfMyExpirationYearMount.Value,
					AuthorName = string.IsNullOrEmpty(this.WciCardInputs.WhfAuthorCardName.Text)
						? this.WciCardInputs.WhfAuthorCardNameRegis.Text
						: this.WciCardInputs.WhfAuthorCardName.Text,
					CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.WciCardInputs.WhfCreditToken.Value),
					CreditCvvToken = OrderCommon.CreditSecurityCodeEnable
						? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.WciCardInputs.WhfMyCvvMount.Value)
						: null,
				};
			}
			else
			{
				creditCardInput = new UserCreditCardInput
				{
					UserId = this.LoginUserId,
					CardDispName =
						(wcbRegistCreditCard.InnerControl != null && wcbRegistCreditCard.Checked == false)
							? creditCardName
							: this.WciCardInputs.WtbUserCreditCardName.Text.Trim(),
					CompanyCode = OrderCommon.CreditCompanySelectable ? this.WciCardInputs.WddlCardCompany.SelectedValue : "",
					BranchNo = "0",
					CardNo =
						this.WciCardInputs.WtbCard1.Text.Trim() + this.WciCardInputs.WtbCard2.Text.Trim()
							+ this.WciCardInputs.WtbCard3.Text.Trim() + this.WciCardInputs.WtbCard4.Text.Trim(),
					CardNo1 = this.WciCardInputs.WtbCard1.Text.Trim(),
					CardNo2 = this.WciCardInputs.WtbCard2.Text.Trim(),
					CardNo3 = this.WciCardInputs.WtbCard3.Text.Trim(),
					CardNo4 = this.WciCardInputs.WtbCard4.Text.Trim(),
					ExpirationMonth = this.WciCardInputs.WddlExpireMonth.Text,
					ExpirationYear = this.WciCardInputs.WddlExpireYear.Text,
					AuthorName = this.WciCardInputs.WtbAuthorName.Text.Trim(),
					SecurityCode = OrderCommon.CreditSecurityCodeEnable ? this.WciCardInputs.WtbSecurityCode.Text : null,
					CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.WciCardInputs.WhfCreditToken.Value),
				};
			}
			if (OrderCommon.CreditTokenUse && string.IsNullOrEmpty(this.WciCardInputs.WhfCreditToken.Value) && (this.IsCreditCardLinkPayment() == false))
			{
				errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
				success = false;
			}

			if (success)
			{
				var dicErrorMessages = creditCardInput.ValidateForFrontUserCreditCardRegist();
				if (dicErrorMessages.Count != 0)
				{
					// カスタムバリデータ取得
					var lCustomValidators = new List<CustomValidator>();
					CreateCustomValidators(this, lCustomValidators);

					// エラーをカスタムバリデータへ
					SetControlViewsForError("UserCreditCardRegist", dicErrorMessages, lCustomValidators);

					// カード番号桁数エラーをカスタムバリデータへ
					if (this.WciCardInputs.WcvCreditCardNo1.IsValid)
					{
						ChangeControlLooksForValidator(
							dicErrorMessages,
							CartPayment.FIELD_CREDIT_CARD_NO + "_length",
							this.WciCardInputs.WcvCreditCardNo1,
							this.WciCardInputs.WtbCard1,
							this.WciCardInputs.WtbCard2,
							this.WciCardInputs.WtbCard3,
							this.WciCardInputs.WtbCard4);
					}
					errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
					success = false;
				}
			}

			if (success)
			{
				// クレジット与信ロックされていればエラーページへ
				if (CreditAuthAttackBlocker.Instance.IsLocked(this.LoginUserId, Request.UserHostAddress))
				{
					errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CREDIT_AUTH_LOCK);
					success = false;
				}
				else if (this.IsCreditCardLinkPayment())
				{
					var result = new UserCreditCardRegister().ExecOnlySave(
						creditCardInput,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.Insert);

					creditCardInput.BranchNo = result.BranchNo.ToString();
				}
				else
				{
					// カード登録
					try
					{
						var result = new UserCreditCardRegister().Exec(
							creditCardInput,
							this.IsSmartPhone ? SiteKbn.SmartPhone : SiteKbn.Pc,
							false,
							Constants.FLG_LASTCHANGED_USER,
							updateHistoryAction);
						if (result.Success)
						{
							creditCardInput.BranchNo = result.BranchNo.ToString();
						}
						else
						{
							apiErrorMessage = result.ErrorMessage;

							// クレジット与信試行カウント-1
							CreditAuthAttackBlocker.Instance.DecreasePossibleTrialCount(this.LoginUserId, Request.UserHostAddress);
							errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
							success = false;
						}
					}
					catch (Exception ex)
					{
						FileLogger.WriteError(ex);
						errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
						success = false;
					}
				}
			}
		}
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm(this.CreditRepeaterItem, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, true);

		var creditResult = (success) ? creditCardInput : null;

		return creditResult;
	}

	/// <summary>
	/// 入力した支払い情報の取得
	/// </summary>
	/// <param name="cartObject">カート情報</param>
	/// <param name="errorMessages">out エラーメッセージ</param>
	/// <returns>成功:入力した支払い情報 失敗:null</returns>
	protected PaymentModel GetAndValidatePaymentInput(CartObject cartObject, out StringBuilder errorMessages)
	{
		errorMessages = new StringBuilder();
		foreach (RepeaterItem riPaymentItem in this.WrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfPaymentId", string.Empty);

			bool isSelected = false;
			if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
			{
				var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPaymentItem, "rbgPayment");
				isSelected = wrbgPayment.InnerControl != null && wrbgPayment.Checked;
			}
			else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
			{
				var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPaymentItem.Parent.Parent, "ddlPayment");
				isSelected = wddlPayment.InnerControl != null && (wddlPayment.SelectedValue == whfPaymentId.Value);
			}

			if (isSelected)
			{
				if (ECPayUtility.GetIsCollection(cartObject.Shippings[0].ShippingReceivingStoreType)
					== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON)
				{
					whfPaymentId.Value = Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE;
				}

				var paymentModel = DataCacheControllerFacade.GetPaymentCacheController().Get((string)whfPaymentId.Value);

				// 支払方法関連入力チェック
				if (paymentModel == null)
				{
					errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_UNSELECTED_ERROR));
				}
				else
				{
					paymentModel.PaymentPrice = OrderCommon.GetPaymentPrice(
						paymentModel.ShopId,
						paymentModel.PaymentId,
						cartObject.PriceSubtotal,
						cartObject.PriceCartTotalWithoutPaymentPrice);

					// 決済手数料設定チェック
					var errorCode = OrderCommon.CheckSetUpPaymentPrice(
						cartObject.ShopId,
						paymentModel.PaymentId,
						cartObject.PriceSubtotal,
						cartObject.PriceCartTotalWithoutPaymentPrice);
					if (errorCode != OrderErrorcode.NoError)
					{
						errorMessages.Append(OrderCommon.GetErrorMessage(errorCode).Replace("@@ 1 @@", paymentModel.PaymentId));
					}
				}
				return paymentModel;
			}
		}
		errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_UNSELECTED_ERROR));
		return null;
	}

	/// <summary>
	/// 入力した支払い情報の取得
	/// </summary>
	/// <param name="cartObject">カート情報</param>
	/// <param name="selectedPaymentId">Selected Payment Id</param>
	/// <param name="errorMessages">out エラーメッセージ</param>
	/// <returns>成功:入力した支払い情報 失敗:null</returns>
	public static PaymentModel GetAndValidatePaymentInput(
		CartObject cartObject,
		string selectedPaymentId,
		out StringBuilder errorMessages)
	{
		errorMessages = new StringBuilder();
		var paymentModel = DataCacheControllerFacade.GetPaymentCacheController().Get(selectedPaymentId);

		// 支払方法関連入力チェック
		if (paymentModel == null)
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_UNSELECTED_ERROR));
		}
		else
		{
			paymentModel.PaymentPrice = OrderCommon.GetPaymentPrice(
				paymentModel.ShopId,
				paymentModel.PaymentId,
				cartObject.PriceSubtotal,
				cartObject.PriceCartTotalWithoutPaymentPrice);

			// 決済手数料設定チェック
			var errorCode = OrderCommon.CheckSetUpPaymentPrice(
				cartObject.ShopId,
				paymentModel.PaymentId,
				cartObject.PriceSubtotal,
				cartObject.PriceCartTotalWithoutPaymentPrice);
			if (errorCode != OrderErrorcode.NoError)
			{
				errorMessages.Append(OrderCommon.GetErrorMessage(errorCode).Replace("@@ 1 @@", paymentModel.PaymentId));
			}
		}
		return paymentModel;
	}

	/// <summary>
	/// クレジットカードのリピーターアイテムを取得
	/// </summary>
	/// <returns>クレジットカードのリピーター 存在しない場合はnull</returns>
	private RepeaterItem GetCreditRepeaterItem()
	{
		foreach (RepeaterItem riPaymentItem in this.WrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfPaymentId", string.Empty);
			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return riPaymentItem;
		}
		return null;
	}

	/// <summary>
	/// Get Atone Repeater Item
	/// </summary>
	/// <returns>Repeater Item</returns>
	private RepeaterItem GetAtoneRepeaterItem()
	{
		foreach (RepeaterItem paymentItem in this.WrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(paymentItem, "hfPaymentId", string.Empty);

			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) return paymentItem;
		}
		return null;
	}

	/// <summary>
	/// Get Aftee Repeater Item
	/// </summary>
	/// <returns>Repeater Item</returns>
	private RepeaterItem GetAfteeRepeaterItem()
	{
		foreach (RepeaterItem paymentItem in this.WrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(paymentItem, "hfPaymentId", string.Empty);

			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) return paymentItem;
		}
		return null;
	}

	/// <summary>
	/// Get Np After Pay Repeater Item
	/// </summary>
	/// <returns>Repeater Item</returns>
	private RepeaterItem GetNPAfterPayRepeaterItem()
	{
		foreach (RepeaterItem paymentItem in this.WrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(paymentItem, "hfPaymentId", string.Empty);
			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY) return paymentItem;
		}
		return null;
	}

	/// <summary>
	/// 選択されたカード情報セット
	/// </summary>
	/// <param name="userCreditCardsUsable">ユーザの持っている利用可能なクレジットカード群</param>
	protected void SetUserCreditCard(UserCreditCard[] userCreditCardsUsable)
	{
		if (userCreditCardsUsable == null) return;

		foreach (var userCreditCard in userCreditCardsUsable)
		{
			// 選択されたカード情報をセット
			if (this.WciCardInputs.WddlUserCreditCard.SelectedValue == userCreditCard.BranchNo.ToString())
			{
				this.CreditCardCompanyName = userCreditCard.CompanyName;
				this.LastFourDigit = userCreditCard.LastFourDigit;
				this.ExpirationMonth = userCreditCard.ExpirationMonth;
				this.ExpirationYear = userCreditCard.ExpirationYear;
				this.CreditAuthorName = userCreditCard.AuthorName;
				break;
			}
		}
	}

	# region ラップ済みコントロール宣言
	/// <summary>AmazonPayのリピーターアイテム</summary>
	protected RepeaterItem AmazonPayRepeaterItem
	{
		get { return m_amazonPayRepeaterItem ?? (m_amazonPayRepeaterItem = GetAmazonPayRepeaterItem()); }
	}
	private RepeaterItem m_amazonPayRepeaterItem;
	/// <summary>AmazonPay(CV2)のリピーターアイテム</summary>
	protected RepeaterItem AmazonPayCv2RepeaterItem
	{
		get { return m_amazonPayCv2RepeaterItem ?? (m_amazonPayCv2RepeaterItem = GetAmazonPayCv2RepeaterItem()); }
	}
	private RepeaterItem m_amazonPayCv2RepeaterItem;
	#endregion

	/// <summary>
	/// AmazonPayのリピーターアイテムを取得
	/// </summary>
	/// <returns>AmazonPayのリピーター 存在しない場合はnull</returns>
	protected RepeaterItem GetAmazonPayRepeaterItem()
	{
		foreach (RepeaterItem riPaymentItem in this.WrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfPaymentId", string.Empty);
			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) return riPaymentItem;
		}
		return null;
	}

	/// <summary>
	/// AmazonPay(CV2)のリピーターアイテムを取得
	/// </summary>
	/// <returns>AmazonPayのリピーター 存在しない場合はnull</returns>
	protected RepeaterItem GetAmazonPayCv2RepeaterItem()
	{
		foreach (RepeaterItem riPaymentItem in this.WrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfPaymentId", string.Empty);
			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) return riPaymentItem;
		}
		return null;
	}

	/// <summary>
	/// 注文拡張項目の変更尾ボタンは表示可能か
	/// </summary>
	/// <returns>表示可能かどうか</returns>
	protected bool IsDisplayOrderExtendModifyButton()
	{
		return (Constants.ORDER_EXTEND_OPTION_ENABLED
			&& DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModelsForFront.Any(m => m.CanUseModify));
	}

	/// <summary>
	/// 購入可能な商品購入数か
	/// </summary>
	/// <param name="maxSellQuantity">購入限度数</param>
	/// <param name="quantity">数量</param>
	/// <returns>購入可能な商品購入数か</returns>
	protected bool CanSalableMaxSellQuantityLimit(int maxSellQuantity, int quantity)
	{
		var result = maxSellQuantity >= quantity;
		return result;
	}

	/// <summary>
	/// 利用可能金額の範囲内であるか
	/// </summary>
	/// <param name="orderInput">注文入力情報</param>
	/// <returns>利用可能金額の範囲内であるか</returns>
	protected string IsPaymentPriceInRange(OrderInput orderInput)
	{
		decimal orderPriceTotal;
		orderPriceTotal = decimal.TryParse(orderInput.OrderPriceTotal, out orderPriceTotal)
			? orderPriceTotal
			: 0;
		var paymentErrorMessage = IsPaymentPriceInRange(
			orderPriceTotal,
			orderInput.ShopId,
			orderInput.OrderPaymentKbn);
		return paymentErrorMessage;
	}
	/// <summary>
	/// 利用可能金額の範囲内であるか
	/// </summary>
	/// <param name="orderPriceTotal">合計金額</param>
	/// <param name="shopId">ショップID</param>
	/// <param name="paymentId">決済種別ID</param>
	/// <returns>利用可能金額の範囲内であるか</returns>
	protected string IsPaymentPriceInRange(decimal orderPriceTotal, string shopId, string paymentId)
	{
		var payment = new PaymentService().Get(shopId, paymentId);
		var errorMessage = (IsPriceInMaxRange(payment, orderPriceTotal)
			&& IsPriceInMinRange(payment, orderPriceTotal))
			? string.Empty
			: OrderCommon.GetErrorMessage(
				OrderErrorcode.PaymentUsablePriceOutOfRangeError,
				CurrencyManager.ToPrice(orderPriceTotal),
				CurrencyManager.ToPrice(payment.UsablePriceMin),
				CurrencyManager.ToPrice(payment.UsablePriceMax));
		return errorMessage;
	}

	/// <summary>
	/// 利用可能金額の範囲内であるか（最小値）
	/// </summary>
	/// <param name="payment">決済種別情報</param>
	/// <param name="orderPriceTotal">合計金額</param>
	/// <returns>利用可能金額の範囲内であるか</returns>
	protected bool IsPriceInMinRange(PaymentModel payment, decimal orderPriceTotal)
	{
		var result = ((payment.UsablePriceMin.HasValue == false)
			|| (orderPriceTotal >= payment.UsablePriceMin));
		return result;
	}

	/// <summary>
	/// 利用可能金額の範囲内であるか（最大値）
	/// </summary>
	/// <param name="payment">決済種別情報</param>
	/// <param name="orderPriceTotal">合計金額</param>
	/// <returns>利用可能金額の範囲内であるか</returns>
	protected bool IsPriceInMaxRange(PaymentModel payment, decimal orderPriceTotal)
	{
		var result = ((payment.UsablePriceMax.HasValue == false)
			|| (orderPriceTotal <= payment.UsablePriceMax));
		return result;
	}

	/// <summary>カード会社名(登録済みカード)</summary>
	protected string CreditCardCompanyName { get; set; }
	/// <summary>カード番号(登録済みカード)</summary>
	protected string LastFourDigit { get; set; }
	/// <summary>有効期限（月）(登録済みカード)</summary>
	protected string ExpirationMonth { get; set; }
	/// <summary>有効期限（年）(登録済みカード)</summary>
	protected string ExpirationYear { get; set; }
	/// <summary>カード名義人(登録済みカード)</summary>
	protected string CreditAuthorName { get; set; }
}
