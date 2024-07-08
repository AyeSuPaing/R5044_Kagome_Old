/*
=========================================================================================================
  Module      : 注文フロー（注文決済）プロセス(OrderFlowProcess_OrderPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.Web.WebCustomControl;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Payment;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;

/// <summary>
/// OrderLpInputs の概要の説明です
/// </summary>
public partial class OrderFlowProcess
{
	#region 決済情報入力画面系処理

	/// <summary>
	/// 決済情報入力画面初期処理
	/// </summary>
	public void InitComponentsOrderPayment()
	{
		//----------------------------------------------------
		// クレジット決済系
		//----------------------------------------------------
		// 使用可能クレジットカードドロップダウン
		this.CreditCardList = new ListItemCollection();

		// 登録可能な場合、ユーザカード種別取得
		if (Constants.MAX_NUM_REGIST_CREDITCARD > 0)
		{
			// つくーる連携、EScottにて通常注文時再与信不可のカードが登録される為除く
			foreach (var cardInfo in UserCreditCard.GetUsable(this.LoginUserId)
				.Where(ccl => (ccl.CooperationId.Split(',').Length != 2) || (ccl.CooperationId.Split(',')[0] != string.Empty)).ToArray())
			{
				this.CreditCardList.Add(new ListItem(cardInfo.CardDispName, cardInfo.BranchNo.ToString()));
			}
		}

		this.CreditCardList.Add(
			new ListItem(
				ReplaceTag("@@DispText.credit_card_list.new@@"),
				CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW));

		// カード会社リスト
		if (OrderCommon.CreditCompanySelectable)
		{
			this.CreditCompanyList = ValueText.GetValueItemList(
				Constants.TABLE_ORDER,
				OrderCommon.CreditCompanyValueTextFieldName);
		}

		// 有効期限(月)ドロップダウン作成
		this.CreditExpireMonth = new ListItemCollection();
		this.CreditExpireMonth.AddRange(DateTimeUtility.GetCreditMonthListItem());
		// 有効期限(年)ドロップダウン作成
		this.CreditExpireYear = new ListItemCollection();
		var creditYearListItem = (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
			? DateTimeUtility.GetCreditYearListItem("yy", 19)
			: DateTimeUtility.GetCreditYearListItem();
		this.CreditExpireYear.AddRange(creditYearListItem);

		// 支払回数ドロップダウン作成
		this.CreditInstallmentsList = ValueText.GetValueItemList(
			Constants.TABLE_ORDER,
			OrderCommon.CreditInstallmentsValueTextFieldName);
		this.NewebPayInstallmentsList = ValueText.GetValueItemList(
			Constants.TABLE_ORDER,
			Constants.FIELD_CREDIT_INSTALLMENTS_NEWEBPAY);

		//----------------------------------------------------
		// コンビニ決済系
		//----------------------------------------------------
		switch (Constants.PAYMENT_CVS_KBN)
		{
			// 電算システムコンビニ支払先
			case Constants.PaymentCvs.Dsk:
				this.CvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, PaymentDskCvs.FIELD_CVS_TYPE);
				break;

			// SBPSコンビニ支払先
			case Constants.PaymentCvs.SBPS:
				this.CvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, "sbps_cvs_dsk_type");
				break;

			// ヤマトKWCコンビニ支払先
			case Constants.PaymentCvs.YamatoKwc:
				this.CvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, "yamatokwc_cvs_type");
				break;

			// GMO convenience store payment
			case Constants.PaymentCvs.Gmo:
				this.CvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.PAYMENT_GMO_CVS_TYPE);
				break;

			// Rakuten convenience store payment
			case Constants.PaymentCvs.Rakuten:
				this.CvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.PAYMENT_RAKUTEN_CVS_TYPE);
				break;

			// Zeus convenience store payment
			case Constants.PaymentCvs.Zeus:
				this.CvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.PAYMENT_ZEUS_CVS_TYPE);
				break;

			// Paygent convenience store payment
			case Constants.PaymentCvs.Paygent:
				this.CvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.PAYMENT_PAYGENT_CVS_TYPE);
				break;
		}
	}

	/// <summary>
	/// カート決済情報作成
	/// </summary>
	public void CreateCartPayment()
	{
		var userCards = UserCreditCard.GetUsable(this.LoginUserId);
		var defaultSelectedCard = string.Empty;

		if ((Constants.MAX_NUM_REGIST_CREDITCARD > 0) && (userCards.Length != 0))
		{
			// 登録済みカードから一番上を取得
			// HACK:仕様的な不整合。クレカ登録可能枚数を運用中に減らしたとき、登録済みのカードは利用できるのか？
			defaultSelectedCard = userCards[0].BranchNo.ToString();
		}

		// カート決済情報作成
		foreach (CartObject cart in this.CartList)
		{
			if (cart.Payment == null)
			{
				cart.Payment = new CartPayment
				{
					IsSamePaymentAsCart1 = (this.CartList.Items[0] != cart)
				};

				// 既定のお支払方法が存在する場合はdefaultSelectedCardにクレジットカードの枝番を設定する
				if (string.IsNullOrEmpty(this.LoginUserId) == false)
				{
					defaultSelectedCard = GetDefaultSelectedCard();
				}

				// 決済がどちらをデフォルト選択するかは表示順で決まる
				if (cart.Payment.IsSamePaymentAsCart1 == false)
				{
					var selectRakutenId =
						(Constants.RAKUTEN_LOGIN_ENABLED) && (SessionManager.IsRakutenIdConnectLoggedIn);
					var selectPayPal = (SessionManager.PayPalCooperationInfo != null);
					if (selectRakutenId && selectPayPal)
					{
						var min = new[]
						{
							DataCacheControllerFacade.GetPaymentCacheController()
								.Get(Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS),
							DataCacheControllerFacade.GetPaymentCacheController()
								.Get(Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL),
						}.OrderBy(p => p.DisplayOrder).First();
						selectRakutenId = (min.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS);
						selectPayPal = (min.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
					}

					if (selectRakutenId) cart.Payment.PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS;
					if (selectPayPal) cart.Payment.PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL;
				}

				if (defaultSelectedCard != string.Empty)
				{
					cart.Payment.CreditCardBranchNo = defaultSelectedCard;
				}

				// 初回のカート決済情報作成の際、領収書情報がカート１と同じにするかのフラグも初期化する
				// ※カート１の初期値＝FALSE　カート１以外の初期値＝TRUE
				cart.IsUseSameReceiptInfoAsCart1 = (this.CartList.Items[0] != cart);
			}
			else
			{
				// 登録済みカードを選択していて、そのカードが削除されている場合
				if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
					&& (userCards.Any(card => (card.BranchNo.ToString() == cart.Payment.CreditCardBranchNo)) == false))
				{
					// 他の登録済みカードあり？
					if (defaultSelectedCard != string.Empty)
					{
						// 登録済みカードの一番上を設定
						cart.Payment.CreditCardBranchNo = defaultSelectedCard;
					}
					else
					{
						// クレジットカード情報初期化
						cart.Payment.ClearCreditCardInfo();
					}
				}
				else if ((cart.Payment.PaymentId == null) && (string.IsNullOrEmpty(this.LoginUserId) == false))
				{
					// 既定のお支払方法が設定されている場合はdefaultSelectedCardにクレジットカードの枝番を設定する
					defaultSelectedCard = GetDefaultSelectedCard();

					// カートの支払情報にクレジットカードの枝番を設定する
					if (defaultSelectedCard != string.Empty)
					{
						cart.Payment.CreditCardBranchNo = defaultSelectedCard;
					}
				}
				else if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				{
					// Amazonペイメントの場合は通常フローでは選ばせないのでカートデフォルト状態にする
					cart.Payment = new CartPayment();
					cart.Payment.IsSamePaymentAsCart1 = (this.CartList.Items[0] != cart);

					if (defaultSelectedCard != string.Empty)
					{
						cart.Payment.CreditCardBranchNo = defaultSelectedCard;
					}
				}
			}
		}
	}

	/// <summary>
	/// デフォルトクレジットカードの取得
	/// </summary>
	/// <returns>カード枝番</returns>
	private string GetDefaultSelectedCard()
	{
		var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
		var creditBranchNo = (userDefaultOrderSetting ?? new UserDefaultOrderSettingModel()).CreditBranchNo;
		
		if (creditBranchNo.HasValue)
		{
			return creditBranchNo.ToString();
		}

		var creditCards = new UserCreditCardService().GetByUserId(this.LoginUserId);
		return (creditCards.Length != 0)
			? creditCards.First().BranchNo.ToString()
			: string.Empty;
	}

	/// <summary>
	/// クレジットカード番号表示設定
	/// </summary>
	public void AdjustCreditCardNo()
	{
		foreach (CartObject coCart in this.CartList)
		{
			if (coCart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				string strCreditCardNo
					= coCart.Payment.CreditCardNo1 + coCart.Payment.CreditCardNo2 + coCart.Payment.CreditCardNo3 + coCart.Payment.CreditCardNo4;

				if (strCreditCardNo.Length >= 14)
				{
					if (coCart.Payment.CreditCardNo2 + coCart.Payment.CreditCardNo3 + coCart.Payment.CreditCardNo4 == "")
					{
						coCart.Payment.CreditCardNo1 = "";
					}
					else if (coCart.Payment.CreditCardNo4 != "")
					{
						coCart.Payment.CreditCardNo2 = "";
						coCart.Payment.CreditCardNo3 = "";
					}
					else
					{
						coCart.Payment.CreditCardNo2 = "";
					}
				}
				coCart.Payment.CreditSecurityCode = "";
			}
		}
	}

	/// <summary>
	/// お支払い情報をカート情報へセット
	/// </summary>
	/// <param name="forceApplyFirstCartPaymentToAfterCarts">強制的にカート1の決済を後続のカートに適用する</param>
	/// <returns>エラー有り：true、エラーなし：false</returns>
	public bool SetPayment(bool forceApplyFirstCartPaymentToAfterCarts = false)
	{
		var cartlist = this.CartList;
		foreach (var cart in this.CartList.Items)
		{
			if ((this.IsLoggedIn
				&& this.IsOnCreditCardAutoSave
				&& ((this.CreditCardList.Count - 1) < Constants.MAX_NUM_REGIST_CREDITCARD))
				&& (((this.IsLandingPage == false) && (cart.Payment.IsBackFromConfirm == false))
					|| (this.IsLandingPage && (cart.Payment.HasUpdateDefaultCard == false))))
			{
				cart.Payment.UpdateUserCreditCardRegistSetting(this.IsOnCreditCardAutoSave, this.CreditCardDefaultNameSetting);
				cart.Payment.HasUpdateDefaultCard = true;
			}
		}
		bool blHasError = false;
		bool blCheckCreditCardNoLength = false;

		CartPayment cpCart1PaymentCart = null;

		var isCart1CreditCardInputNew = false;

		var whfPaidyTokenId = GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfPaidyTokenId");

		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// カートリピーターアイテムインデックスがカートオブジェクト数以上の場合、スキップ
			if (riCart.ItemIndex >= this.CartList.Items.Count) continue;

			// ラップ済みコントロール宣言
			var wcbUseSamePaymentAddrAsCart1 = GetWrappedControl<WrappedCheckBox>(riCart, "cbUseSamePaymentAddrAsCart1", false);
			var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");

			CartObject coCartCurrent = this.CartList.Items[riCart.ItemIndex];

			string strPaymentId = null;
			IPayment ipPayment = null;
			StringBuilder sbErrorMessages = new StringBuilder();

			var isSamePaymentAsCart1 = ((riCart.ItemIndex != 0) && (wcbUseSamePaymentAddrAsCart1.Checked));

			//------------------------------------------------------
			// お支払い方法入力情報取得
			//------------------------------------------------------
			Hashtable htInput = null;
			if ((isSamePaymentAsCart1 == false) || (forceApplyFirstCartPaymentToAfterCarts == false))
			{
				htInput = new Hashtable();

				//------------------------------------------------------
				// お支払い方法入力情報取得
				//------------------------------------------------------
				foreach (RepeaterItem riPaymentItem in wrPayment.Items)
				{
					// ラップ済みコントロール宣言
					var wCardInputs = new WrappedCreditCardInputs(this, this.Page, riPaymentItem);
					var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfPaymentId", "");
					var whfShopId = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfShopId", "0");
					var whfPaymentName = GetWrappedControl<WrappedHiddenField>(riPaymentItem, "hfPaymentName", "");
					var wddlDskCvsType = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlDskCvsType");
					var wddlSBPSCvsType = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlSBPSCvsType");
					var wddlYamatoKwcCvsType = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlYamatoKwcCvsType");
					var wddlGmoCvsType = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlGmoCvsType");
					var wddlZeusCvsType = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlZeusCvsType");
					var wddlPaygentCvsType = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlPaygentCvsType");
					var wddlEcPayment = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlEcPayment");
					var wcbEcPayCreditInstallment = GetWrappedControl<WrappedCheckBox>(riPaymentItem, "cbEcPayCreditInstallment");
					var wcbNewebPayCreditInstallment = GetWrappedControl<WrappedCheckBox>(riPaymentItem, "cbEcPayCreditInstallment");
					var wddlNewebPayment = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlNewebPayment");
					var wddlCreditInstallment = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlCreditInstallment");

					var whfCreditTokenSameAs1 = GetWrappedControl<WrappedHiddenField>(riCart, "hfCreditTokenSameAs1", "");
					var wddlRakutenCvsType = GetWrappedControl<WrappedDropDownList>(riPaymentItem, "ddlRakutenCvsType");

					bool isSelected = false;
					if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB))
					{
						isSelected = GetWrappedControl<WrappedRadioButtonGroup>(riPaymentItem, "rbgPayment").Checked;
					}
					else if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_DDL) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL))
					{
						var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPaymentItem.Parent.Parent, "ddlPayment");
						isSelected = wddlPayment.SelectedValue == whfPaymentId.Value;
					}

					if (isSelected)
					{
						strPaymentId = whfPaymentId.Value;

						htInput.Add(Constants.FIELD_PAYMENT_SHOP_ID, whfShopId.Value);
						htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_ID, strPaymentId);
						htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_NAME, whfPaymentName.Value);
						// Paypal,Paidy,ECPayでは「マイページ > 注文方法の登録」利用を除き、通常注文ではw2_UserCreditCardに利用時の情報を残すためにブランチIDは空とする
						htInput.Add(
							CartPayment.FIELD_CREDIT_CARD_BRANCH_NO,
							((strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
								? wCardInputs.WddlUserCreditCard.SelectedValue
								: string.Empty));

						// クレジット決済入力の場合
						if (strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						{
							// 新規カード利用
							if ((string)htInput[CartPayment.FIELD_CREDIT_CARD_BRANCH_NO] == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
							{
								if (riCart.ItemIndex == 0) isCart1CreditCardInputNew = true;

								if (IsCreditCardLinkPayment() == false)
								{
									htInput.Add(CartPayment.FIELD_CREDIT_COMPANY, OrderCommon.CreditCompanySelectable ? wCardInputs.WddlCardCompany.SelectedValue : "");
									if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
									{
										var ucRakutenCreditCard = riPaymentItem.FindControl("ucRakutenCreditCard");
										wCardInputs.WhfMyCardMount.Value = ((HiddenField)ucRakutenCreditCard.FindControl("hfMyCardMount")).Value;
										wCardInputs.WhfMyCvvMount.Value = ((HiddenField)ucRakutenCreditCard.FindControl("hfMyCvvMount")).Value;
										var year = ((HiddenField) ucRakutenCreditCard.FindControl("hfMyExpirationYearMount")).Value;
										wCardInputs.WhfMyExpirationYearMount.Value = (year.Length >= 2) ? year.Substring(year.Length - 2, 2) : string.Empty;
										wCardInputs.WhfMyExpirationMonthMount.Value = ((HiddenField)ucRakutenCreditCard.FindControl("hfMyExpirationMonthMount")).Value;
										wCardInputs.WhfAuthorCardName.Text = ((HiddenField)ucRakutenCreditCard.FindControl("hfAuthorNameCard")).Value;
										wCardInputs.WhfCreditCardCompany.Value = ((HiddenField)ucRakutenCreditCard.FindControl("hfCreditCardCompany")).Value;
										wCardInputs.WdllCreditInstallmentsRakuten.SelectedValue = ((DropDownList)ucRakutenCreditCard.FindControl("dllCreditInstallmentsRakuten")).SelectedValue;
										if (OrderCommon.CreditTokenUse == false)
										{
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO, StringUtility.ToHankaku(wCardInputs.WhfMyCardMount.Value));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO + "_length", htInput[CartPayment.FIELD_CREDIT_CARD_NO]);
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_1, StringUtility.ToHankaku(wCardInputs.WhfMyCardMount.Value));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_2, StringUtility.ToHankaku(wCardInputs.WtbCard2.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_3, StringUtility.ToHankaku(wCardInputs.WtbCard3.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, StringUtility.ToHankaku(wCardInputs.WtbCard4.Text));
										}
										else if(string.IsNullOrEmpty(wCardInputs.WhfCreditToken.Value) == false)
										{
											var cardNoInputTmp = "    " + StringUtility.ToHankaku(wCardInputs.WhfMyCardMount.Value + wCardInputs.WtbCard2.Text + wCardInputs.WtbCard3.Text + wCardInputs.WtbCard4.Text);
											var cardNoInputLast4 = cardNoInputTmp.Substring(cardNoInputTmp.Length - 4, 4).Replace(Constants.CHAR_MASKING_FOR_TOKEN, "").Trim();
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, cardNoInputLast4);
										}
										else
										{
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_1, StringUtility.ToHankaku(wCardInputs.WhfMyCardMount.Value));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_2, StringUtility.ToHankaku(wCardInputs.WtbCard2.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_3, StringUtility.ToHankaku(wCardInputs.WtbCard3.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, StringUtility.ToHankaku(wCardInputs.WtbCard4.Text));
										}
										htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_MONTH, wCardInputs.WhfMyExpirationMonthMount.Value);
										htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_YEAR, wCardInputs.WhfMyExpirationYearMount.Value);
										htInput.Add(CartPayment.FIELD_CREDIT_AUTHOR_NAME, StringUtility.ToHankaku(wCardInputs.WhfAuthorCardName.Text));
										htInput.Add(CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE, wCardInputs.WdllCreditInstallmentsRakuten.SelectedValue);
										htInput[CartPayment.FIELD_CREDIT_COMPANY] =  RakutenApiFacade.ConvertCompanyCode(wCardInputs.WhfCreditCardCompany.Value);
										if (OrderCommon.CreditSecurityCodeEnable)
										{
											htInput.Add(CartPayment.FIELD_CREDIT_RAKUTEN_CVV_TOKEN, StringUtility.ToHankaku(wCardInputs.WhfMyCvvMount.Value));
										}

										// Token情報セット
										if (OrderCommon.CreditTokenUse)
										{
											htInput.Add("CreditToken", StringUtility.ToEmpty(wCardInputs.WhfCreditToken.Value));
											if (isCart1CreditCardInputNew)
											{
												htInput.Add("CreditTokenSameAs1", StringUtility.ToEmpty(whfCreditTokenSameAs1.Value));
											}

											if (string.IsNullOrEmpty(wCardInputs.WhfCreditToken.Value))
											{
												blHasError = true;
												if (wCardInputs.WspanErrorMessageForCreditCard.HasInnerControl)
												{
													wCardInputs.WspanErrorMessageForCreditCard.InnerHtml
														= WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
													wCardInputs.WspanErrorMessageForCreditCard.InnerControl.Style["display"] = "block";
												}
											}
										}
									}
									else
									{
										if (OrderCommon.CreditTokenUse == false)
										{
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO, StringUtility.ToHankaku(wCardInputs.WtbCard1.Text + wCardInputs.WtbCard2.Text + wCardInputs.WtbCard3.Text + wCardInputs.WtbCard4.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO + "_length", htInput[CartPayment.FIELD_CREDIT_CARD_NO]);
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_1, StringUtility.ToHankaku(wCardInputs.WtbCard1.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_2, StringUtility.ToHankaku(wCardInputs.WtbCard2.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_3, StringUtility.ToHankaku(wCardInputs.WtbCard3.Text));
											htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, StringUtility.ToHankaku(wCardInputs.WtbCard4.Text));
										}
										else
										{
											if (string.IsNullOrEmpty(wCardInputs.WhfCreditToken.Value) == false)
											{
												var cardNoInputTmp = "    " + StringUtility.ToHankaku(wCardInputs.WtbCard1.Text + wCardInputs.WtbCard2.Text + wCardInputs.WtbCard3.Text + wCardInputs.WtbCard4.Text);
												var cardNoInputLast4 = cardNoInputTmp.Substring(cardNoInputTmp.Length - 4, 4).Replace(Constants.CHAR_MASKING_FOR_TOKEN, "").Trim();
												htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, cardNoInputLast4);
											}
											else
											{
												htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_1, StringUtility.ToHankaku(wCardInputs.WtbCard1.Text));
												htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_2, StringUtility.ToHankaku(wCardInputs.WtbCard2.Text));
												htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_3, StringUtility.ToHankaku(wCardInputs.WtbCard3.Text));
												htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, StringUtility.ToHankaku(wCardInputs.WtbCard4.Text));
											}
										}
										htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_MONTH, wCardInputs.WddlExpireMonth.SelectedValue);
										htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_YEAR, wCardInputs.WddlExpireYear.SelectedValue);
										htInput.Add(CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE, wCardInputs.WddlInstallments.SelectedValue);
										htInput.Add(CartPayment.FIELD_CREDIT_AUTHOR_NAME, StringUtility.ToHankaku(wCardInputs.WtbAuthorName.Text));
										var creditCardNo = StringUtility.ToEmpty(htInput[CartPayment.FIELD_CREDIT_CARD_NO]);
										var bincode = ((OrderCommon.CreditTokenUse == false)
												&& (creditCardNo.Length > 6))
											? creditCardNo.Substring(0, 6)
											: wCardInputs.WhfCreditBincode.Value;
										htInput.Add(CartPayment.FIELD_CREDIT_BINCODE, bincode);

										if (OrderCommon.CreditTokenUse == false)
										{
											if (OrderCommon.CreditSecurityCodeEnable)
											{
												htInput.Add(CartPayment.FIELD_CREDIT_SECURITY_CODE, StringUtility.ToHankaku(wCardInputs.WtbSecurityCode.Text.Trim()));
											}
										}

										// Token情報セット
										// 必要に応じで決済代行ごとに入れる
										if (OrderCommon.CreditTokenUse)
										{
											htInput.Add("CreditToken", StringUtility.ToEmpty(wCardInputs.WhfCreditToken.Value));
											if (isCart1CreditCardInputNew)
											{
												htInput.Add("CreditTokenSameAs1", StringUtility.ToEmpty(whfCreditTokenSameAs1.Value));
											}

											// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
											if (string.IsNullOrEmpty(wCardInputs.WhfCreditToken.Value))
											{
												blHasError = true;
												if (wCardInputs.WspanErrorMessageForCreditCard.HasInnerControl)
												{
													wCardInputs.WspanErrorMessageForCreditCard.InnerHtml
														= WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
													wCardInputs.WspanErrorMessageForCreditCard.InnerControl.Style["display"] = "block";
												}
											}
										}
									}
								}

								// 登録クレカにする場合
								if ((wCardInputs.WcbRegistCreditCard.InnerControl != null) && wCardInputs.WcbRegistCreditCard.Visible)
								{
									htInput.Add(CartPayment.FIELD_CREDIT_CARD_REGIST_FLG, wCardInputs.WcbRegistCreditCard.Checked);
									if (wCardInputs.WcbRegistCreditCard.Checked)
									{
										htInput.Add(CartPayment.FIELD_REGIST_CREDIT_CARD_NAME, wCardInputs.WtbUserCreditCardName.Text);
									}
								}
							}
							else
							{
								int cardBranchNo = int.Parse(wCardInputs.WddlUserCreditCard.SelectedValue);
								var card = UserCreditCard.Get(this.LoginUserId, cardBranchNo);

								htInput.Add(CartPayment.FIELD_CREDIT_COMPANY, card.CompanyCode);
								htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_MONTH, card.ExpirationMonth);
								htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_YEAR, card.ExpirationYear);
								htInput.Add(CartPayment.FIELD_CREDIT_COOPERATION_ID, card.CooperationId);
								// 表示用にCard_no_1に入れておく
								htInput.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, card.LastFourDigit);
								if (string.IsNullOrEmpty(card.AuthorName) == false)
								{
									// ゼウスリンク式の場合、名義は戻ってこないので空になっている。
									// 空だとバリデータに引っかかってしまう。
									htInput.Add(CartPayment.FIELD_CREDIT_AUTHOR_NAME, card.AuthorName);
								}
								htInput.Add(CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE, wCardInputs.WdllCreditInstallments2.SelectedValue);
								htInput.Add(CartPayment.FIELD_CREDIT_CARD_REGIST_FLG, false);
								// ペイジェントの場合は連携ID2に顧客カードIDが入っているので追加
								if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
								{
									htInput.Add(Constants.FIELD_USERCREDITCARD_COOPERATION_ID2, card.CooperationId2);
								}
							}
							// エラーチェック前にコントロールリセット（これをしないとクライアントチェックの値が残る）
							ResetControlViewsForError(riPaymentItem, wCardInputs.WtbSecurityCode, wCardInputs.WtbAuthorName);

							var dicErrorMessages = new Dictionary<string, string>();
							
							if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
							{
								// ベリトランス決済とき、下4桁バリデーションチェック対象外
								// 理由：payTgレスポンスクレジットカード番号は下2桁しかない
								var checkHtInput = (Hashtable)htInput.Clone();
								checkHtInput.Remove(CartPayment.FIELD_CREDIT_CARD_NO_4);
								dicErrorMessages = Validator.ValidateAndGetErrorContainer("OrderPayment", checkHtInput);
							}
							else
							{
								dicErrorMessages = Validator.ValidateAndGetErrorContainer("OrderPayment", htInput);
							}

							if (dicErrorMessages.Count != 0)
							{
								// カスタムバリデータ取得
								List<CustomValidator> lCustomValidators = new List<CustomValidator>();
								CreateCustomValidators(riPaymentItem, lCustomValidators);

								// エラーをカスタムバリデータへ
								SetControlViewsForError("OrderPayment", dicErrorMessages, lCustomValidators);

								// 楽天クレジットの場合、モダール形式で入力しているため、エラーメッセージをフォーカスする
								if ((strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
									&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
									&& (string.IsNullOrEmpty(wCardInputs.WspanErrorMessageForCreditCard.InnerHtml) == false))
								{
									SessionManager.LpValidateErrorElementClientId = wCardInputs.WspanErrorMessageForCreditCard.ClientID;
								}

								// カード番号桁数エラーをカスタムバリデータへ
								if (wCardInputs.WcvCreditCardNo1.IsValid)
								{
									ChangeControlLooksForValidator(
										dicErrorMessages,
										CartPayment.FIELD_CREDIT_CARD_NO + "_length",
										wCardInputs.WcvCreditCardNo1,
										wCardInputs.WtbCard1, wCardInputs.WtbCard2, wCardInputs.WtbCard3, wCardInputs.WtbCard4);
								}

								blHasError = true;
							}

							// ゼウスリンク式決済で新規登録の場合は、ここで情報補完
							if (((string)htInput[CartPayment.FIELD_CREDIT_CARD_BRANCH_NO] == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
								&& IsCreditCardLinkPayment())
							{
								htInput.Add(CartPayment.FIELD_CREDIT_COMPANY, "");
								htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_MONTH, wCardInputs.WddlExpireMonth.SelectedValue);
								htInput.Add(CartPayment.FIELD_CREDIT_EXPIRE_YEAR, wCardInputs.WddlExpireYear.SelectedValue);
								htInput.Add(CartPayment.FIELD_CREDIT_AUTHOR_NAME, "");
								htInput.Add(CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE, wCardInputs.WddlInstallments.SelectedValue);
							}
						}
						// コンビニ決済の場合
						else if (strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
						{
							// 電算システムコンビニ決済の場合
							if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Dsk)
							{
								ipPayment = new PaymentDskCvs(wddlDskCvsType.SelectedValue);
							}
							// SBPSコンビニ決済の場合
							else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.SBPS)
							{
								coCartCurrent.Payment.SBPSWebCvsType = (PaymentSBPSTypes.WebCvsTypes)Enum.Parse(typeof(PaymentSBPSTypes.WebCvsTypes), wddlSBPSCvsType.SelectedValue);
							}
							// ヤマトKWCコンビニ決済の場合
							else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc)
							{
								coCartCurrent.Payment.YamatoKwcCvsType = (YamatoKwcFunctionDivCvs)Enum.Parse(typeof(YamatoKwcFunctionDivCvs), wddlYamatoKwcCvsType.SelectedValue);
							}
							// Gmo convenience store payment
							else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo)
							{
								coCartCurrent.Payment.GmoCvsType = wddlGmoCvsType.SelectedValue;
							}
							// Rakuten convenience store payment
							else if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten)
							{
								coCartCurrent.Payment.RakutenCvsType = wddlRakutenCvsType.SelectedValue;
							}
							// Zeus convenience store payment
							else if (OrderCommon.IsPaymentCvsTypeZeus)
							{
								ipPayment = new PaymentZeusCvs(wddlZeusCvsType.SelectedValue);
							}
							// Paygent convenience store payment
							else if (OrderCommon.IsPaymentCvsTypePaygent)
							{
								ipPayment = new PaymentPaygentCvs(wddlPaygentCvsType.SelectedValue);
							}
						}
						// PayPal決済の場合
						else if (strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
						{
							this.CartList.PayPalCooperationInfo = SessionManager.PayPalCooperationInfo;
							if (SessionManager.PayPalCooperationInfo == null)
							{
								DispCartError(riCart, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAL_NEEDS_LOGIN_ERROR));
								blHasError = true;
							}
						}
						// Check paidy get token
						else if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct)
							&& (strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
						{
							if (string.IsNullOrEmpty(whfPaidyTokenId.Value))
							{
								DispCartError(riCart, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_GET_TOKEN_ERROR));
								blHasError = true;
							}
							else
							{
								coCartCurrent.Payment.PaidyToken = whfPaidyTokenId.Value;
							}
						}
						// Ec Pay set External Payment Type
						else if (strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
						{
							coCartCurrent.Payment.ExternalPaymentType = wddlEcPayment.SelectedValue;
							coCartCurrent.Payment.IsPaymentEcPayWithCreditInstallment = (wcbEcPayCreditInstallment.Visible && wcbEcPayCreditInstallment.Checked);
							htInput[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE] = coCartCurrent.Payment.ExternalPaymentType;
						}
						// Set NewebPay External Payment Type
						else if (strPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
						{
							coCartCurrent.Payment.ExternalPaymentType = wddlNewebPayment.SelectedValue;
							htInput[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE] = coCartCurrent.Payment.ExternalPaymentType;
							htInput.Add("neweb_" + CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE, wddlCreditInstallment.SelectedValue);
						}
						break;
					}
				}

				//------------------------------------------------------
				// お支払い方法入力チェック
				//------------------------------------------------------
				// 支払方法関連入力チェック
				if (strPaymentId == null)
				{
					sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_UNSELECTED_ERROR));
				}
				else
				{
					// 決済手数料設定チェック
					OrderErrorcode oeErrorCode = OrderCommon.CheckSetUpPaymentPrice(
						coCartCurrent.ShopId,
						(string)htInput[Constants.FIELD_PAYMENT_PAYMENT_ID],
						coCartCurrent.PriceSubtotal,
						coCartCurrent.PriceCartTotalWithoutPaymentPrice);
					if (oeErrorCode != OrderErrorcode.NoError)
					{
						sbErrorMessages.Append(OrderCommon.GetErrorMessage(oeErrorCode).Replace("@@ 1 @@", (string)htInput[Constants.FIELD_PAYMENT_PAYMENT_ID]));
					}
				}

				// 入力チェックエラー表示
				if (sbErrorMessages.Length != 0)
				{
					if (DispCartError(riCart, sbErrorMessages.ToString())) blHasError = true;
				}

				//------------------------------------------------------
				// お支払い情報をカート情報へセット
				//------------------------------------------------------
				// CartPayment更新
				coCartCurrent.Payment.UpdateCartPayment(
					(string)htInput[Constants.FIELD_PAYMENT_PAYMENT_ID],
					(string)htInput[Constants.FIELD_PAYMENT_PAYMENT_NAME],
					(string)htInput[CartPayment.FIELD_CREDIT_CARD_BRANCH_NO],
					(string)htInput[CartPayment.FIELD_CREDIT_COMPANY],
					(string)htInput[CartPayment.FIELD_CREDIT_CARD_NO_1],
					(string)htInput[CartPayment.FIELD_CREDIT_CARD_NO_2],
					(string)htInput[CartPayment.FIELD_CREDIT_CARD_NO_3],
					(string)htInput[CartPayment.FIELD_CREDIT_CARD_NO_4],
					(string)htInput[CartPayment.FIELD_CREDIT_EXPIRE_MONTH],
					(string)htInput[CartPayment.FIELD_CREDIT_EXPIRE_YEAR],
					(string)htInput[CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE],
					(string)htInput[CartPayment.FIELD_CREDIT_SECURITY_CODE],
					(string)htInput[CartPayment.FIELD_CREDIT_AUTHOR_NAME],
					ipPayment,
					false,
					(string)htInput[CartPayment.FIELD_CREDIT_RAKUTEN_CVV_TOKEN],
					StringUtility.ToEmpty(htInput["neweb_" + CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE]),
					StringUtility.ToEmpty(htInput[CartPayment.FIELD_CREDIT_BINCODE]));

				// 登録カード情報セット
				if (htInput.Contains(CartPayment.FIELD_CREDIT_CARD_REGIST_FLG))
				{
					coCartCurrent.Payment.UserCreditCardRegistable = true;
					coCartCurrent.Payment.UpdateUserCreditCardRegistSetting(
						(bool)htInput[CartPayment.FIELD_CREDIT_CARD_REGIST_FLG],
						(string)htInput[CartPayment.FIELD_REGIST_CREDIT_CARD_NAME]);
				}
				else
				{
					coCartCurrent.Payment.UserCreditCardRegistable = false;
					coCartCurrent.Payment.UpdateUserCreditCardRegistSetting(false, "");
				}

				if (OrderCommon.CreditTokenUse)
				{
					// Token情報セット
					coCartCurrent.Payment.CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(StringUtility.ToEmpty(htInput["CreditToken"]));
					// Token情報（カート１と同様） ※支払い方法変更で戻されたときのことを考え、保持しておく
					coCartCurrent.Payment.CreditTokenSameAs1 = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(StringUtility.ToEmpty(htInput["CreditTokenSameAs1"]));
				}

				//------------------------------------------------------
				// カート決済情報へセット
				//------------------------------------------------------
				// 決済手数料セット（商品合計金額から計算する（ポイント等で割引された金額から計算してはいけない））
				coCartCurrent.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
					coCartCurrent.ShopId,
					coCartCurrent.Payment.PaymentId,
					coCartCurrent.PriceSubtotal,
					coCartCurrent.PriceCartTotalWithoutPaymentPrice); 
				coCartCurrent.Calculate(false, isPaymentChanged: true);
			}
			// カート1と同じお支払い方法の場合
			else
			{
				if (cpCart1PaymentCart != null)
				{
					coCartCurrent.Payment.UpdateCartPayment(
						cpCart1PaymentCart.PaymentId,
						cpCart1PaymentCart.PaymentName,
						cpCart1PaymentCart.CreditCardBranchNo,
						cpCart1PaymentCart.CreditCardCompany,
						cpCart1PaymentCart.CreditCardNo1,
						cpCart1PaymentCart.CreditCardNo2,
						cpCart1PaymentCart.CreditCardNo3,
						cpCart1PaymentCart.CreditCardNo4,
						cpCart1PaymentCart.CreditExpireMonth,
						cpCart1PaymentCart.CreditExpireYear,
						cpCart1PaymentCart.CreditInstallmentsCode,
						cpCart1PaymentCart.CreditSecurityCode,
						cpCart1PaymentCart.CreditAuthorName,
						cpCart1PaymentCart.PaymentObject,
						true,
						cpCart1PaymentCart.RakutenCvvToken,
						cpCart1PaymentCart.NewebPayCreditInstallmentsCode,
						cpCart1PaymentCart.CreditBincode);

					if (coCartCurrent.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					{
						coCartCurrent.Payment.PaidyToken = whfPaidyTokenId.Value;
					}

					// コンビニ区分をセット
					coCartCurrent.Payment.SBPSWebCvsType = cpCart1PaymentCart.SBPSWebCvsType;
					coCartCurrent.Payment.YamatoKwcCvsType = cpCart1PaymentCart.YamatoKwcCvsType;
					coCartCurrent.Payment.GmoCvsType = cpCart1PaymentCart.GmoCvsType;
					coCartCurrent.Payment.RakutenCvsType = cpCart1PaymentCart.RakutenCvsType;

					// 登録カード情報セット(登録すると２重登録なのでfalse)
					coCartCurrent.Payment.UpdateUserCreditCardRegistSetting(
						false,
						cpCart1PaymentCart.UserCreditCardName);

					// Token情報セット
					// 必要に応じで決済代行ごとに入れる
					if ((OrderCommon.CreditTokenUse)
						&& (cpCart1PaymentCart.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT))
					{
						if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten)
						{
							// カート１のhfCreditTokenSameAs1には何も入っていないので、現カートのhfCreditTokenSameAs1を利用
							var whfCreditTokenSameAs1 = GetWrappedControl<WrappedHiddenField>(riCart, "hfCreditTokenSameAs1", "");
							coCartCurrent.Payment.CreditTokenSameAs1 = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(whfCreditTokenSameAs1.Value);
							coCartCurrent.Payment.CreditToken = coCartCurrent.Payment.CreditTokenSameAs1;
						}
						else
						{
							coCartCurrent.Payment.CreditToken = cpCart1PaymentCart.CreditToken;
							coCartCurrent.Payment.CreditTokenSameAs1 = cpCart1PaymentCart.CreditTokenSameAs1;
						}
					}
					else
					{
						coCartCurrent.Payment.CreditToken = null;
					}
				}
			}

			if ((blHasError == false) && (this.IsLoggedIn))
			{
				//------------------------------------------------------
				// クレジット登録可否チェック(サーバー側でも行う)
				//------------------------------------------------------
				int iCreditCardRegistCount = 0;
				foreach (CartObject co in this.CartList)
				{
					if (co.Payment == null) continue;
					// 新規カードを登録する場合
					if (co.Payment.UserCreditCardRegistFlg
						&& (co.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
					{
						iCreditCardRegistCount++;
					}
				}
				// 登録しようとする場合、登録可否チェック
				if ((iCreditCardRegistCount > 0)
					&& (OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, (this.CreditCardList.Count - 1), iCreditCardRegistCount) == false))
				{
					blHasError = true;
					sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDITCARD_REGIST_COUNT_OVER_ERROR));
					sbErrorMessages.Replace("@@ 1 @@", Constants.MAX_NUM_REGIST_CREDITCARD + "枚");
				}

				//------------------------------------------------------
				// 決済種別金額範囲チェック（上の処理で決済金額が設定されているはず）
				//------------------------------------------------------
				sbErrorMessages.Append(OrderCommon.CheckPaymentPriceEnabled(this.CartList.Items[riCart.ItemIndex], coCartCurrent.Payment.PaymentId));
				if (DispCartError(riCart, sbErrorMessages.ToString()))
				{
					blHasError = true;
				}
			}

			if (blHasError == false)
			{
				// エラー無しの場合、カート1であれば入力情報を保存
				if (riCart.ItemIndex == 0)
				{
					cpCart1PaymentCart = coCartCurrent.Payment;
				}
			}

			var errorMessage = string.Empty;
			foreach (CartObject currentCartObject in this.CartList)
			{
				// If Change Product From LpTemplate Then Continue
				if (currentCartObject.Shippings.Any(item => string.IsNullOrEmpty(item.ShippingCountryIsoCode))) continue;
				// Check Valid Tel No And Country
				if (currentCartObject.Payment != null)
				{
					var ownerTel = currentCartObject.Owner.Tel1;
					var paymentId = currentCartObject.Payment.PaymentId;
					var lPaymentErrorMessage = GetWrappedControl<WrappedLiteral>("lPaymentErrorMessage");

					switch(paymentId)
					{
						case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
							if (Constants.PAYMENT_ATONEOPTION_ENABLED == false) break;

							// Check Country
							var hasShippingJp = (currentCartObject.Shippings.Any(shipping => shipping.IsShippingAddrJp == false));
							if ((currentCartObject.Owner.IsAddrJp == false)
								|| hasShippingJp)
							{
								if (string.IsNullOrEmpty(errorMessage) == false) errorMessage += "</br>";
								errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE);
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
							if (Constants.PAYMENT_AFTEEOPTION_ENABLED == false) break;

							// Check Country
							var hasShippingTw = (currentCartObject.Shippings.Any(shipping => shipping.IsShippingAddrTw == false));
							if ((currentCartObject.Owner.IsAddrTw == false)
								|| hasShippingTw)
							{
								if (string.IsNullOrEmpty(errorMessage) == false) errorMessage += "</br>";
								errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE);
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
							if (Constants.ECPAY_PAYMENT_OPTION_ENABLED == false) break;
							// Check Country
							var existNotShippingTw = (currentCartObject.Shippings.Any(shipping => shipping.IsShippingAddrTw == false));
							if ((currentCartObject.Owner.IsAddrTw == false)
								|| existNotShippingTw)
							{
								if (string.IsNullOrEmpty(errorMessage) == false) errorMessage += "</br>";
								errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_ECPAY);
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
							if (Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED == false) break;
							// Check Country
							var isExistNotShippingTw = (currentCartObject.Shippings.Any(shipping => shipping.IsShippingAddrTw == false));
							if ((currentCartObject.Owner.IsAddrTw == false)
								|| isExistNotShippingTw)
							{
								if (string.IsNullOrEmpty(errorMessage) == false) errorMessage += "</br>";
								errorMessage +=
									WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_NEWEBPAY);
							}
							break;
					}
					lPaymentErrorMessage.Text = errorMessage;
				}
			}

			if (string.IsNullOrEmpty(errorMessage) == false) return true;
		}	// foreach (RepeaterItem riCart in rCartList.Items)

		// 通常の入力チェックとクレジット番号の長さチェックをエラーがあるか？
		return (blHasError || blCheckCreditCardNoLength);
	}

	/// <summary>
	/// カートエラー表示
	/// </summary>
	/// <param name="riCart">カートリピータアイテム</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <remarks>エラーありかどうか</remarks>
	protected bool DispCartError(RepeaterItem riCart, string errorMessage)
	{
		var wsErrorMessage2 = GetWrappedControl<WrappedHtmlGenericControl>(riCart, "sErrorMessage2");
		wsErrorMessage2.InnerText = this.DispCartErrorMessage = errorMessage;
		var hasError = (errorMessage != "");
		if (hasError) wsErrorMessage2.Focus();

		return hasError;
	}

	/// <summary>
	/// カート番号「１」同じお支払いを指定するチェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbUseSamePaymentAddrAsCart1_OnCheckedChanged(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ラップ済みコントロール設定
		//------------------------------------------------------
		WrappedCheckBox wcbUseSamePaymentAddrAsCart1 = null;

		if (sender is CheckBox)
		{
			wcbUseSamePaymentAddrAsCart1 = GetWrappedControl<WrappedCheckBox>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else if (sender is WrappedCheckBox)
		{
			wcbUseSamePaymentAddrAsCart1 = (WrappedCheckBox)sender;
		}
		else
		{
			return;
		}

		var rPayment = GetWrappedControl<WrappedRepeater>(wcbUseSamePaymentAddrAsCart1.Parent, "rPayment");
		var wddlPayment = GetWrappedControl<WrappedDropDownList>(wcbUseSamePaymentAddrAsCart1.Parent, "ddlPayment");

		//------------------------------------------------------
		// カート番号「１」同じお支払いを指定するチェックボックス設定
		//------------------------------------------------------
		var riCartList = GetParentRepeaterItem(wcbUseSamePaymentAddrAsCart1, "rCartList");
		rPayment.Visible = ((riCartList.ItemIndex == 0) || (wcbUseSamePaymentAddrAsCart1.Checked == false));
		wddlPayment.Visible = ((riCartList.ItemIndex == 0) || (wcbUseSamePaymentAddrAsCart1.Checked == false));

		if (sender is CheckBox)
		{
			var cart = this.CartList.Items[riCartList.ItemIndex];

			if (wcbUseSamePaymentAddrAsCart1.Checked)
			{
				cart.Payment.PaymentId = this.CartList.Items[0].Payment.PaymentId;
			}
		}

		// 領収書情報入力フォームの表示制御
		ControlDisplayReceiptInfoInputForm();

		// Set Paidy Pay Controls;
		SetPaidyPayControls();
	}

	/// <summary>
	/// 決済種別変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void rbgPayment_OnCheckedChanged(object sender, System.EventArgs e)
	{
		var isRb = this.IsLandingPage
			? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB)
			: (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB);

		//------------------------------------------------------
		// ラップ済みコントロール設定
		//------------------------------------------------------
		WrappedRadioButtonGroup wrbgPayments = null;
		WrappedDropDownList wddlPayments = null;
		if (isRb)
		{
		if (sender is RadioButtonGroup)
		{
			wrbgPayments = GetWrappedControl<WrappedRadioButtonGroup>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else if (sender is WrappedRadioButtonGroup)
		{
			wrbgPayments = (WrappedRadioButtonGroup)sender;
		}
		else
		{
			return;
		}
		}
		else
		{
			if (sender is DropDownList)
			{
				wddlPayments = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, ((Control)sender).ID);
			}
			else if (sender is WrappedDropDownList)
			{
				wddlPayments = (WrappedDropDownList)sender;
			}
			else
			{
				return;
			}
		}

		//------------------------------------------------------
		// 決済種別毎の入力項目表示制御
		//------------------------------------------------------
		WrappedRepeater wrPayments = null;
		if (isRb)
		{
			wrPayments = GetWrappedControl<WrappedRepeater>(wrbgPayments.Parent.Parent.Parent, "rPayment");
		}
		else
		{
			wrPayments = GetWrappedControl<WrappedRepeater>(wddlPayments.Parent, "rPayment");
		}

		foreach (RepeaterItem riPayment in wrPayments.Items)
		{
			var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
			var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");
			var wddCredit = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCredit");
			var wddCvsPre = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCvsPre");
			var wddCvsDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCvsDef");
			var wddSmsDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddSmsDef");
			var wddBankPre = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddBankPre");
			var wddBankDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddBankDef");
			var wddPostPre = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPostPre");
			var wddPostDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPostDef");
			var wddDocomoPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddDocomoPayment");
			var wddSMatometePayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddSMatometePayment");
			var wddAuMatometePayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAuMatometePayment");
			var wddSoftBankKeitaiSBPSPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddSoftBankKeitaiSBPSPayment");
			var wddAuKantanSBPSPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAuKantanSBPSPayment");
			var wddDocomoKeitaiSBPSPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddDocomoKeitaiSBPSPayment");
			var wddSMatometeSBPSPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddSMatometeSBPSPayment");
			var wddPaypalSBPSPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPayPalSBPSPayment");
			var wddRecruitSBPSPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddRecruitSBPSPayment");
			var wddRakutenIdSBPSPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddRakutenIdSBPSPayment");
			var wddAmazonPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAmazonPay");
			var wddTriLinkAfterPayPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddTriLinkAfterPayPayment");
			var wddCollect = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCollect");
			var wddPayPal = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPayPal");
			var wddNoPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddNoPayment");
			var wucPaidyCheckoutControl = GetWrappedControl<WrappedPaidyCheckoutControl>(riPayment, "ucPaidyCheckoutControl");
			var wddPaidy = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaidy");
			var wddAtonePayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAtonePayment");
			var wddAfteePayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAfteePayment");
			var wddNpAfterPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddNpAfterPay");
			var wddEcPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddEcPayment");
			var wddDskDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddDskDef");
			var wddNewebPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddNewebPayment");
			var wddCarrierbillingBokuPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCarrierbillingBokuPayment");
			var wddLinePay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddLinePay");
			var wddPayPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPayPay");
			var wddGmoAtokara = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddGmoAtokara");

			wddCredit.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
			wddCvsPre.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE));
			wddCvsDef.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
			wddSmsDef.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF));
			wddBankPre.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE));
			wddBankDef.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF));
			wddPostPre.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE));
			wddPostDef.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF));
			wddDocomoPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG));
			wddSMatometePayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG));
			wddAuMatometePayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG));
			wddSoftBankKeitaiSBPSPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS));
			wddAuKantanSBPSPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS));
			wddDocomoKeitaiSBPSPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS));
			wddSMatometeSBPSPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS));
			wddPaypalSBPSPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS));
			wddRecruitSBPSPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS));
			wddRakutenIdSBPSPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS));
			wddAmazonPay.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT));
			wddTriLinkAfterPayPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY));
			wddCollect.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT));
			wddPayPal.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL));
			wddNoPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
			wddPaidy.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			wucPaidyCheckoutControl.DisplayUserControl = wddPaidy.Visible;
			wddAtonePayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE));
			wddAfteePayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			wddNpAfterPay.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
			wddLinePay.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			wddPayPay.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			wddEcPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY));
			wddDskDef.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF));
			wddNewebPayment.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY));
			wddCarrierbillingBokuPayment.Visible = (wrbgPayment.Checked && (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			wddGmoAtokara.Visible = ((isRb ? wrbgPayment.Checked : (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA))
					&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA));
		}

		//------------------------------------------------------
		// カート1にて選択している決済種別がカート2以降選択不可の場合、
		// 「カート番号「１」同じお支払いを指定する」を選択不可とする
		//------------------------------------------------------
		string strPaymentIdCart1 = null;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
			var wcbUseSamePaymentAddrAsCart1 = GetWrappedControl<WrappedCheckBox>(riCart, "cbUseSamePaymentAddrAsCart1", false);
			var wddlPayment = GetWrappedControl<WrappedDropDownList>(riCart, "ddlPayment");
			var wdlPaymentList = GetWrappedControl<WrappedHtmlGenericControl>(riCart, "dlPaymentList");

			if (riCart.ItemIndex == 0)
			{
				// カート1の選択決済種別取得
				foreach (RepeaterItem riPayment in wrPayment.Items)
				{
					// ラップ済みコントロール宣言
					var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");

					bool isSelected = false;
					if (isRb)
					{
						isSelected = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment").Checked;
					}
					else
					{
						
						isSelected = (wddlPayment.SelectedValue == whfPaymentId.Value);
					}

					if (isSelected)
					{
						strPaymentIdCart1 = whfPaymentId.Value;
						break;
					}
				}
			}
			else
			{
				bool blUseSamePaymentAddrAsCart1 = false;
				foreach (RepeaterItem riPayment in wrPayment.Items)
				{
					var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", string.Empty);
					var currentCartPayment = this.CartList.Items[riCart.ItemIndex].Payment;
					if (((whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						&& this.IsLoggedIn
						&& this.IsOnCreditCardAutoSave
						&& ((this.CreditCardList.Count - 1) < Constants.MAX_NUM_REGIST_CREDITCARD))
						&& (((this.IsLandingPage == false) && (currentCartPayment.IsBackFromConfirm == false))
							|| (this.IsLandingPage && (currentCartPayment.HasUpdateDefaultCard == false))))
					{
						var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(riPayment, "cbRegistCreditCard");
						var wtbUserCreditCardName = GetWrappedControl<WrappedTextBox>(riPayment, "tbUserCreditCardName", string.Empty);
						wcbRegistCreditCard.Checked = this.IsOnCreditCardAutoSave;
						wtbUserCreditCardName.Text = this.CreditCardDefaultNameSetting;
						currentCartPayment.HasUpdateDefaultCard = true;
					}
				}

				foreach (RepeaterItem riPayment in wrPayment.Items)
				{
					// ラップ済みコントロール宣言
					var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");
					if (whfPaymentId.Value == strPaymentIdCart1)
					{
						blUseSamePaymentAddrAsCart1 = true;

						// カート1がクレカでトークンありのとき、他のカートにトークンがない場合はカート1の決済を利用させない
						if ((strPaymentIdCart1 == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
							&& OrderCommon.CreditTokenUse
							&& (this.CartList.Items[0].Payment.CreditToken != null))
						{
							var hfCreditTokenSameAs1 = GetWrappedControl<WrappedHiddenField>(riPayment, "hfCreditTokenSameAs1", "");
							if (string.IsNullOrEmpty(hfCreditTokenSameAs1.Value)) blUseSamePaymentAddrAsCart1 = false;
						}
						break;
					}
				}
				// 「カート番号「１」同じお支払いを指定する」表示制御
				if (blUseSamePaymentAddrAsCart1)
				{
					wcbUseSamePaymentAddrAsCart1.Visible = true;
				}
				else
				{
					wcbUseSamePaymentAddrAsCart1.Visible = false;
					wcbUseSamePaymentAddrAsCart1.Checked = false;
					wrPayment.Visible = true;
					wddlPayment.Visible = true;
				}
			}
		}

		// 領収書情報入力フォームの表示制御
		ControlDisplayReceiptInfoInputForm();

		// Set Paidy Pay Controls
		SetPaidyPayControls();
	}

	/// <summary>
	/// クレジットカード選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlUserCreditCard_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ラップ済みコントロール設定
		//------------------------------------------------------
		WrappedDropDownList wddlUserCreditCard = null;
		if (sender is DropDownList)
		{
			wddlUserCreditCard = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else if (sender is WrappedDropDownList)
		{
			wddlUserCreditCard = (WrappedDropDownList)sender;
		}
		else
		{
			return;
		}

		// ラップ済みコントロール宣言
		var wdivCreditCardInputForm = GetWrappedControl<WrappedHtmlGenericControl>(wddlUserCreditCard.Parent, "divCreditCardInputForm");
		var wdivCreditCardDisp = GetWrappedControl<WrappedHtmlGenericControl>(wddlUserCreditCard.Parent, "divCreditCardDisp");
		var wlCreditCardCompanyName = GetWrappedControl<WrappedLiteral>(wddlUserCreditCard.Parent, "lCreditCardCompanyName", "");
		var wlLastFourDigit = GetWrappedControl<WrappedLiteral>(wddlUserCreditCard.Parent, "lLastFourDigit", "");
		var wlExpirationMonth = GetWrappedControl<WrappedLiteral>(wddlUserCreditCard.Parent, "lExpirationMonth", "");
		var wlExpirationYear = GetWrappedControl<WrappedLiteral>(wddlUserCreditCard.Parent, "lExpirationYear", "");
		var wlCreditAuthorName = GetWrappedControl<WrappedLiteral>(wddlUserCreditCard.Parent, "lCreditAuthorName", "");
		var whfCreditCardId = GetWrappedControl<WrappedHiddenField>(wddlUserCreditCard.Parent, "hfCreditCardId", "");

		if ((this.IsPostBack == false)
			&& (this.LoginUserId != null))
		{
			var userUsable = wddlUserCreditCard.Items.Cast<ListItem>()
				.Where(c => (c.Value != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
				.Select(c => c.Value)
				.ToArray();
			if (userUsable.Any() == false)
			{
				wddlUserCreditCard.SelectedValue = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
			}
		}

		switch (wddlUserCreditCard.SelectedValue)
		{
			case CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW:
				wdivCreditCardInputForm.Visible = true;
				wdivCreditCardDisp.Visible = false;
				break;

			default:
				wdivCreditCardInputForm.Visible = false;
				wdivCreditCardDisp.Visible = true;

				// クレジットカード情報画面セット
				var branchNo = 0;
				int.TryParse(wddlUserCreditCard.SelectedValue, out branchNo);
				var userCreditCard = UserCreditCard.Get(this.LoginUserId, branchNo);
				if (userCreditCard != null)
				{
					wlCreditCardCompanyName.Text = WebSanitizer.HtmlEncode(userCreditCard.CompanyName);
					wlLastFourDigit.Text = (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
						? WebSanitizer.HtmlEncode(userCreditCard.LastFourDigit.Replace('*','X'))
						: WebSanitizer.HtmlEncode(userCreditCard.LastFourDigit);
					wlExpirationMonth.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationMonth);
					wlExpirationYear.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationYear);
					wlCreditAuthorName.Text = WebSanitizer.HtmlEncode(userCreditCard.AuthorName);
					whfCreditCardId.Value = userCreditCard.CooperationId;
				}
				break;
		}
	}

	/// <summary>
	/// クレジットカードを登録するチェックボックスをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbRegistCreditCard_OnCheckedChanged(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ラップ済みコントロール設定
		//------------------------------------------------------
		WrappedCheckBox wcbRegistCreditCard = null;
		if (sender is CheckBox)
		{
			wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else if (sender is WrappedCheckBox)
		{
			wcbRegistCreditCard = (WrappedCheckBox)sender;
		}
		else
		{
			return;
		}

		var wdivUserCreditCardName = GetWrappedControl<WrappedHtmlGenericControl>(wcbRegistCreditCard.Parent, "divUserCreditCardName");

		wdivUserCreditCardName.Visible = wcbRegistCreditCard.Checked;
	}

	/// <summary>
	/// クレジットトークン向け  カード情報取得JSスクリプト作成
	/// </summary>
	/// <param name="riParent">決済リピータアイテム</param>
	/// <returns>スクリプト</returns>
	public string CreateGetCardInfoJsScriptForCreditToken(RepeaterItem riParent)
	{
		if (riParent == null) return "";
		if (GetWrappedControl<WrappedHiddenField>(riParent, "hfPaymentId").Value != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return "";

		var userCreditCardCooperationInfoFacade = UserCreditCardCooperationInfoFacade.CreateForGetToken();

		var script = CreateGetCardInfoJsScriptForCreditTokenInner(userCreditCardCooperationInfoFacade, null, riParent);
		return script;
	}

	/// <summary>
	/// クレジットトークン向け  カード情報取得JSスクリプト作成
	/// </summary>
	/// <param name="riCart">カートリピータアイテム</param>
	/// <param name="riParent">決済リピータアイテム</param>
	/// <returns>スクリプト</returns>
	public string CreateGetCardInfoJsScriptForCreditTokenForCart(RepeaterItem riCart, RepeaterItem riParent)
	{
		if (riParent == null) return "";
		if (GetWrappedControl<WrappedHiddenField>(riParent, "hfPaymentId").Value != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return "";

		var userCreditCardCooperationInfo = UserCreditCardCooperationInfoFacade.CreateForGetToken();

		var script = CreateGetCardInfoJsScriptForCreditTokenInner(userCreditCardCooperationInfo, riCart, riParent);
		return script;
	}

	/// <summary>
	/// クレジットトークン取得＆フォームセットJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	public string CreateGetCreditTokenAndSetToFormJsScript()
	{
		var script = CreateGetCreditTokenAndSetToFormJsScriptInner(this.WrCartList);
		return script;
	}

	/// <summary>
	/// クレジットトークン向け フォームマスキングJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	public string CreateMaskFormsForCreditTokenJsScript()
	{
		var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner(this.WrCartList);
		return maskingScripts;
	}

	/// <summary>
	/// ペイパル認証完了（決済入力画面向け）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbPayPalAuthComplete_Click(object sender, EventArgs e)
	{
		// ログインしていて別ユーザーに紐づいていた場合はエラーとして認証させない
		var payPalScriptsForm = GetWrappedControl<WrappedPayPalPayScriptsFormControl>("ucPaypalScriptsForm");
		var user = PayPalUtility.Account.GetUserByPayPalCustomerId(payPalScriptsForm.PayPalPayerId);
		if (this.IsLoggedIn && (user != null) && (user.UserId != this.LoginUserId))
		{
			SessionManager.MessageForErrorPage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COOPERATE_WITH_SOMEONE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			return;
		}

		// 認証OKであれば認証情報をセッションへ格納
		SetPaypalInfoToSession(payPalScriptsForm);

		// ログイン済みであれば、新しい連携ID紐づけ
		if (this.IsLoggedIn)
		{
			PayPalUtility.Account.UpdateUserExtendForPayPal(
				this.LoginUserId,
				SessionManager.PayPalLoginResult,
				UpdateHistoryAction.Insert);
		}
		// 未ログインかつ、PayPalに紐づいたユーザーが見つかればログインさせる
		else if (user != null)
		{
			var nextUrl = this.RawUrl;
			if ((this is OrderLandingProcess) == false)
			{
				nextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_OWNER_DECISION;
				Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextUrl;
			}

			// ログイン成功処理実行＆三分岐画面へ戻る
			ExecLoginSuccessProcessAndGoNextForLogin(
				user,
				nextUrl,
				false,
				BasePage.LoginType.PayPal,
				UpdateHistoryAction.Insert);
		}

		// PayPal選択（注文決済画面向け）
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var wcbUseSamePaymentAddrAsCart1 = GetWrappedControl<WrappedCheckBox>(riCart, "cbUseSamePaymentAddrAsCart1", false);
			if (wcbUseSamePaymentAddrAsCart1.Checked) continue;

			SelectPayment(riCart, Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
		}
		// PayPal選択（購入履歴、定期変更画面向け）
		SelectPayment(null, Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);

		// PayPal連携情報更新（注文確認画面からのPayPal連携で新たにセットしなおす）
		this.CartList.PayPalCooperationInfo = SessionManager.PayPalCooperationInfo;
	}

	/// <summary>
	/// 決済選択
	/// </summary>
	/// <param name="parentControl">親コントロール</param>
	/// <param name="paymentId">選択したい決済種別ID</param>
	private void SelectPayment(Control parentControl, string paymentId)
	{
		var wrPayment = GetWrappedControl<WrappedRepeater>(parentControl, "rPayment");
		foreach (RepeaterItem riPayment in wrPayment.Items)
		{
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");
			if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB))
			{
			var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
			if (whfPaymentId.Value == paymentId)
			{
				wrbgPayment.Checked = true;
				rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
			}
			else if (wrbgPayment.Checked)
			{
				wrbgPayment.Checked = false;
				rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
			}
		}
			else if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_DDL) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL))
			{
				var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
				if (whfPaymentId.Value == paymentId)
				{
					wddlPayment.SelectedValue = paymentId;
					rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
				}
				else if (wddlPayment.SelectedValue == whfPaymentId.Value)
				{
					rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
				}
			}
		}
	}

	/// <summary>
	/// 選択した領収書希望フラグ値の取得
	/// </summary>
	/// <param name="cartIndex">カート番号</param>
	/// <param name="isSameReceiptCart1">カート１と同じにするか</param>
	/// <param name="receiptFlg">領収書希望フラグ</param>
	/// <returns>適当な領収書希望値</returns>
	public string GetSelectedValueOfReceiptFlg(int cartIndex, bool isSameReceiptCart1, string receiptFlg)
	{
		// 最初のカートは、カート情報に持っている領収書希望の値で返す
		if (cartIndex == 0) return receiptFlg;

		// カート２以降、「カート１と同じにする」フラグに応じて、適当な値を返す
		return isSameReceiptCart1 ? Constants.FLG_ORDER_RECEIPT_FLG_SAME_CART1 : receiptFlg;
	}

	/// <summary>
	/// 領収書希望有無ドロップダウンリスト変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlReceiptFlg_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		WrappedDropDownList wddlReceiptFlg;
		if (sender is WrappedDropDownList)
		{
			wddlReceiptFlg = (WrappedDropDownList)sender;
		}
		else if (sender is Control)
		{
			wddlReceiptFlg = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, ((Control)sender).ID);
		}
		else
		{
			return;
		}

		var wdivReceiptAddressProviso = GetWrappedControl<WrappedHtmlGenericControl>(
			wddlReceiptFlg.Parent,
			"divReceiptAddressProviso");
		wdivReceiptAddressProviso.Visible = (wddlReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
	}

	/// <summary>
	/// 領収書情報入力フォームの表示制御
	/// </summary>
	protected void ControlDisplayReceiptInfoInputForm()
	{
		// 領収書対応OPが無効の場合、戻す
		if (Constants.RECEIPT_OPTION_ENABLED == false) return;

		var paymentIdCart1 = string.Empty;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
			var wcbUseSamePaymentAddrAsCart1 =
				GetWrappedControl<WrappedCheckBox>(riCart, "cbUseSamePaymentAddrAsCart1", false);
			var wdivReceiptInfoInputForm =
				GetWrappedControl<WrappedHtmlGenericControl>(riCart, "divReceiptInfoInputForm");
			var wdivDisplayCanNotInputMessage =
				GetWrappedControl<WrappedHtmlGenericControl>(riCart, "divDisplayCanNotInputMessage");

			// 選択した決済ID取得
			var selectedPaymentId = string.Empty;
			foreach (RepeaterItem riPayment in wrPayment.Items)
			{
				// ラップ済みコントロール宣言
				var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");

				bool isSelected = false;
				if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB))
				{
					isSelected = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment").Checked;
				}
				else if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_DDL) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL))
				{
					var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
					isSelected = (wddlPayment.SelectedValue == whfPaymentId.Value);
				}

				if (isSelected)
				{
					selectedPaymentId = whfPaymentId.Value;
					// カート1の選択決済種別取得
					if (riCart.ItemIndex == 0) paymentIdCart1 = selectedPaymentId;
					break;
				}
			}

			// カート２以降かつカート１と同じにする場合、カート１の選択した決済IDに変更
			if ((riCart.ItemIndex > 0) && wcbUseSamePaymentAddrAsCart1.Checked)
			{
				selectedPaymentId = paymentIdCart1;
			}

			// 表示制御
			wdivReceiptInfoInputForm.Visible =
				(Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(selectedPaymentId) == false);
			wdivDisplayCanNotInputMessage.Visible = (wdivReceiptInfoInputForm.Visible == false);
		}
	}

	/// <summary>
	/// 領収書情報をカート情報へセット
	/// </summary>
	/// <returns>エラー有り：true、エラーなし：false</returns>
	public bool SetReceipt()
	{
		var hasError = false;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wddlReceiptFlg = GetWrappedControl<WrappedDropDownList>(riCart, "ddlReceiptFlg");
			var wtbReceiptAddress = GetWrappedControl<WrappedTextBox>(riCart, "tbReceiptAddress");
			var wtbReceiptProviso = GetWrappedControl<WrappedTextBox>(riCart, "tbReceiptProviso");
			var wdivReceiptInfoInputForm =
				GetWrappedControl<WrappedHtmlGenericControl>(riCart, "divReceiptInfoInputForm");

			var isSameInfoAsCart1 = ((riCart.ItemIndex != 0)
				&& wddlReceiptFlg.HasInnerControl
				&& (wddlReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_SAME_CART1));
			this.CartList.Items[riCart.ItemIndex].IsUseSameReceiptInfoAsCart1 = isSameInfoAsCart1;

			if (isSameInfoAsCart1 == false)
			{
				var hasReceipt = (wdivReceiptInfoInputForm.Visible
					&& wddlReceiptFlg.HasInnerControl
					&& (wddlReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON));
				var address = StringUtility.RemoveUnavailableControlCode(wtbReceiptAddress.Text.Trim());
				var proviso = StringUtility.RemoveUnavailableControlCode(wtbReceiptProviso.Text.Trim());

				// 領収書希望ありの時に、宛名と但し書きの入力チェックを行う
				if (hasReceipt)
				{
					// エラーチェック前にコントロールリセット（これをしないとクライアントチェックの値が残る）
					ResetControlViewsForError(riCart, wtbReceiptAddress, wtbReceiptProviso);
					// エラーチェック＆カスタムバリデータへセット
					var input = new Hashtable
					{
						{ Constants.FIELD_ORDER_RECEIPT_ADDRESS, address },
						{ Constants.FIELD_ORDER_RECEIPT_PROVISO, proviso },
					};
					var errorMessages = Validator.ValidateAndGetErrorContainer("ReceiptRegisterModify", input);
					if (errorMessages.Count != 0)
					{
						// カスタムバリデータ取得
						var customValidators = new List<CustomValidator>();
						CreateCustomValidators(riCart, customValidators);
						// エラーをカスタムバリデータへ
						SetControlViewsForError("ReceiptRegisterModify", errorMessages, customValidators);
						hasError = true;
						continue;
					}
				}

				// 領収書情報をカートにセット
				this.CartList.Items[riCart.ItemIndex].ReceiptFlg = hasReceipt
					? Constants.FLG_ORDER_RECEIPT_FLG_ON
					: Constants.FLG_ORDER_RECEIPT_FLG_OFF;
				this.CartList.Items[riCart.ItemIndex].ReceiptAddress = hasReceipt ? address : string.Empty;
				this.CartList.Items[riCart.ItemIndex].ReceiptProviso = hasReceipt ? proviso : string.Empty;
			}
			else
			{
				// カート１の領収書情報をカートにセット
				this.CartList.Items[riCart.ItemIndex].ReceiptFlg = this.CartList.Items[0].ReceiptFlg;
				this.CartList.Items[riCart.ItemIndex].ReceiptAddress = this.CartList.Items[0].ReceiptAddress;
				this.CartList.Items[riCart.ItemIndex].ReceiptProviso = this.CartList.Items[0].ReceiptProviso;
			}
		}

		return hasError;
	}

	/// <summary>
	/// Set Paidy Pay Controls
	/// </summary>
	public void SetPaidyPayControls()
	{
		if (Constants.PAYMENT_PAIDY_OPTION_ENABLED == false) return;

		var whfPaidyTokenId = GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfPaidyTokenId");
		var whfPaidyPaySelected = GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfPaidyPaySelected");
		if (string.IsNullOrEmpty(this.PaidyTokenId) == false)
		{
			whfPaidyTokenId.Value = this.PaidyTokenId;
		}

		var hasPaidyPay = false;
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
			foreach (RepeaterItem riPayment in wrPayment.Items)
			{
				var wucPaidyCheckoutControl = GetWrappedControl<WrappedPaidyCheckoutControl>(riPayment, "ucPaidyCheckoutControl");
				var wddPaidy = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaidy");

				bool isSelected = false;
				if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB))
				{
					isSelected = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment").Checked;
				}
				else if (this.IsLandingPage ? (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_DDL) : (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL))
				{
					var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
					var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");
					isSelected = (wddlPayment.SelectedValue == whfPaymentId.Value);
				}

				if (isSelected)
				{
					wucPaidyCheckoutControl.DisplayUserControl = wddPaidy.Visible;

					if (hasPaidyPay) continue;
					hasPaidyPay = wddPaidy.Visible;
				}
			}
		}

		whfPaidyPaySelected.Value = hasPaidyPay.ToString();
	}

	/// <summary>
	/// Check Paidy Token Id Exist
	/// </summary>
	/// <returns>True: token has exist, otherwise: false</returns>
	public bool CheckPaidyTokenIdExist()
	{
		var hasPaidyPay = this.CartList.Items.Any(cart => cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
		var result = hasPaidyPay
			&& PaidyUtility.IsTokenIdExist(this.PaidyTokenId);

		return result;
	}

	/// <summary>
	/// Is Display Credit Installment
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>True: If there is a Credit Installer setting and the External payment type is Credit</returns>
	public bool IsDisplayCreditInstallment(CartPayment payment)
	{
		var result = ((payment == null)
			|| string.IsNullOrEmpty(payment.ExternalPaymentType)
			|| (payment.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
			|| (payment.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT));
		return result;
	}

	/// <summary>
	/// Dropdown List Ec Payment On Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlEcPayment_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var wcbEcPayCreditInstallment = GetWrappedControl<WrappedCheckBox>(((Control)sender).Parent, "cbEcPayCreditInstallment");
		var wddlEcPayment = GetWrappedControl<WrappedDropDownList>((Control)sender, "ddlEcPayment");
		wcbEcPayCreditInstallment.Visible = ((string.IsNullOrEmpty(Constants.ECPAY_PAYMENT_CREDIT_INSTALLMENT) == false)
			&& (wddlEcPayment.SelectedValue == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT));
	}

	/// <summary>
	/// Dropdown List NewebPay On Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlNewebPayment_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var wdvCreditInstallment = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "dvCreditInstallment");
		var wddlNewebPayment = GetWrappedControl<WrappedDropDownList>((Control)sender, "ddlNewebPayment");
		wdvCreditInstallment.Visible = (wddlNewebPayment.SelectedValue == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT);
	}
	#endregion

	#region 決済選択プロパティ

	/// <summary>カードリスト</summary>
	public ListItemCollection CreditCardList { get; set; }
	/// <summary>カード会社名</summary>
	public ListItemCollection CreditCompanyList { get; set; }
	/// <summary>有効期限(月)</summary>
	public ListItemCollection CreditExpireMonth { get; set; }
	/// <summary>有効期限(年)</summary>
	public ListItemCollection CreditExpireYear { get; set; }
	/// <summary>支払回数</summary>
	public ListItemCollection CreditInstallmentsList { get; set; }
	/// <summary>コンビニ支払先</summary>
	public ListItemCollection CvsTypeList { get; set; }
	/// <summary>有効決済種別</summary>
	public List<PaymentModel[]> ValidPayments { get; set; }
	/// <summary>NewebPay Installments List</summary>
	public ListItemCollection NewebPayInstallmentsList { get; set; }
	/// <summary>領収書希望のプルダウン選択肢リスト</summary>
	public List<ListItemCollection> DdlReceiptFlgListItems
	{
		get
		{
			// 最初のエレメントを設定※「カート１と同じにする」アイテムなし
			var list = new List<ListItemCollection>
			{
				this.ReceiptFlgListItems
			};

			// 複数カートではない場合、１エレメントで返す
			if (this.CartList.Items.Count <= 1) return list;

			// 複数カートである場合、カート数に応じて、エレメントを追加
			for (var idx = 1; idx < this.CartList.Items.Count; idx++)
			{
				list.Add(ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG));
			}

			return list;
		}
	}
	/// <summary>領収書希望選択肢リスト</summary>
	public ListItemCollection ReceiptFlgListItems
	{
		get
		{
			var list = ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG);

			// 「カート１と同じにする」のアイテムがあれば、削除
			var needRemoveItem = list.FindByValue(Constants.FLG_ORDER_RECEIPT_FLG_SAME_CART1);
			if (needRemoveItem != null) list.Remove(needRemoveItem);

			return list;
		}
	}
	/// <summary>Paidy Token ID</summary>
	protected string PaidyTokenId
	{
		get
		{
			var cartHasPaidyToken = this.CartList.Items
				.FirstOrDefault(item => (item.Payment != null) && (string.IsNullOrEmpty(item.Payment.PaidyToken) == false));

			return (cartHasPaidyToken != null)
				? cartHasPaidyToken.Payment.PaidyToken
				: string.Empty;
		}
	}
	/// <summary>Is on credit card auto save</summary>
	public bool IsOnCreditCardAutoSave
	{
		get
		{
			var isCreditCardAutoSave = false;
			bool.TryParse(
				ReplaceTag("@@CreditCard.credit_card_register.default@@"),
				out isCreditCardAutoSave);
			return isCreditCardAutoSave;
		}
	}
	/// <summary> Credit card default name setting</summary>
	public string CreditCardDefaultNameSetting
	{
		get
		{
			return DateTime.Now.ToString(
				ReplaceTag("@@CreditCard.disp_name.text@@"));
		}
	}
	# endregion
}
