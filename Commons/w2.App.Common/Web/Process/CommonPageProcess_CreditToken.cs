/*
=========================================================================================================
  Module      : コモンページプロセス ※トークン処理(CommonPageProcess_CreditToken.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.User;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Web.Process
{
	/// <summary>
	/// コモンページプロセス
	/// </summary>
	public partial class CommonPageProcess
	{
		/// <summary>
		/// カスタムバリデータdisabled処理（トークン取得用・マスキングした後にポストバックできるようにするため）
		/// </summary>
		/// <param name="wrCartList">カートリストラップ済みリピータ</param>
		public void DisableCreditInputCustomValidatorForGetCreditToken(WrappedRepeater wrCartList)
		{
			if (OrderCommon.CreditTokenUse == false) return;

			foreach (RepeaterItem riCart in wrCartList.Items)
			{
				var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
				foreach (RepeaterItem riPayment in wrPayment.Items)
				{
					DisableCreditInputCustomValidatorForGetCreditToken(riPayment);
				}
			}
		}
		/// <summary>
		/// クレジットトークン向け カスタムバリデータdisabled処理（マスキングした後にポストバックできるようにするため）
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		public void DisableCreditInputCustomValidatorForGetCreditToken(RepeaterItem riParent = null)
		{
			if (OrderCommon.CreditTokenUse == false) return;

			var cardInput = new WrappedCreditCardInputs(this, this.Page, riParent);
			cardInput.WcvCreditCardNo1.Enabled
				= cardInput.WcvCreditCardNo2.Enabled
				= cardInput.WcvCreditCardNo3.Enabled
				= cardInput.WcvCreditCardNo4.Enabled
				= cardInput.WcvCreditSecurityCode.Enabled = false;
		}

		/// <summary>
		/// クレジットトークン向け カスタムバリデータを通す（UserCreditInput用）
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		public void EnableCreditInputCustomValidatorForGetCreditToken(RepeaterItem riParent = null)
		{
			if (OrderCommon.CreditTokenUse == false) return;

			var cardInput = new WrappedCreditCardInputs(this, this.Page, riParent);
			cardInput.WcvCreditCardNo1.Enabled
				= cardInput.WcvCreditCardNo2.Enabled
				= cardInput.WcvCreditCardNo3.Enabled
				= cardInput.WcvCreditCardNo4.Enabled
				= cardInput.WcvCreditSecurityCode.Enabled = true;
		}

		/// <summary>
		/// クレジットトークン向け  カード情報取得JSスクリプト作成（カート内ではない）
		/// </summary>
		/// <returns>
		/// カード情報取得スクリプト
		/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
		/// </returns>
		public virtual string CreateGetCardInfoJsScriptForCreditTokenInner()
		{
			var userCreditCardCooperationInfo = UserCreditCardCooperationInfoFacade.CreateForGetToken();

			var cardInfoScript = CreateGetCardInfoJsScriptForCreditTokenInner(userCreditCardCooperationInfo);
			return cardInfoScript;
		}
		/// <summary>
		/// クレジットトークン向け カード情報取得JSスクリプト作成（カート内ではない）
		/// </summary>
		/// <param name="riCart">カートリピータがある場合のリピータアイテム（カート1と同じ決済利用時のトークン取得用）</param>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		/// <param name="userCreditCardCooperationInfo">ユーザークレジットカード連携情報</param>
		/// <returns>
		/// カード情報取得スクリプト
		/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
		/// </returns>
		public virtual string CreateGetCardInfoJsScriptForCreditTokenInner(
			UserCreditCardCooperationInfoBase userCreditCardCooperationInfo,
			RepeaterItem riCart = null,
			RepeaterItem riParent = null)
		{
			// 決済IDチェック（コントロールがない場合はfalseにならない）
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riParent, "hfPaymentId");
			if (whfPaymentId.HasInnerControl && (whfPaymentId.Value != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)) return "";

			var whfCreditTokenSameAs1 = GetWrappedControl<WrappedHiddenField>(riCart, "hfCreditTokenSameAs1");

			var cardInput = new WrappedCreditCardInputs(this, this.Page, riParent);
			var cardInfoScript = CreateGetCardInfoJsScriptForCreditTokenInner(cardInput, whfCreditTokenSameAs1, userCreditCardCooperationInfo);
			return cardInfoScript;
		}

		/// <summary>
		/// クレジットトークン向け カード情報取得JSスクリプト作成（カート内ではない）
		/// </summary>
		/// <param name="cardInput">カード入力情報</param>
		/// <param name="whfCreditTokenSameAs1">カート１と同じ決済を利用するときのトークン</param>
		/// <param name="userCreditCardCooperationInfo">ユーザークレジットカード連携情報</param>
		/// <returns>スクリプト</returns>
		private string CreateGetCardInfoJsScriptForCreditTokenInner(
			WrappedCreditCardInputs cardInput,
			WrappedHiddenField whfCreditTokenSameAs1,
			UserCreditCardCooperationInfoBase userCreditCardCooperationInfo)
		{
			var cardKbnScript = "$('#" + cardInput.WddlCardCompany.ClientID + "').val()";
			var cardnoScript = string.Empty;
			var expireMonthScript = string.Empty;
			var expireYearScript = string.Empty;
			var securityCodeScript = string.Empty;
			var authornameScript = string.Empty;
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
			{
				cardnoScript = "$('#" + cardInput.WhfMyCardMount.ClientID + "').val()";
				expireMonthScript = "$('#" + cardInput.WhfMyExpirationMonthMount.ClientID + "').val()";
				expireYearScript = "$('#" + cardInput.WhfMyExpirationYearMount.ClientID + "').val()";
				securityCodeScript = "$('#" + cardInput.WhfMyCvvMount.ClientID + "').val()";
				authornameScript = "$('#" + cardInput.WhfAuthorCardName.ClientID + "').val()";
			}
			else
			{
				cardnoScript = string.Format(
				"{0} + {1} + {2} + {3}",
				(cardInput.WtbCard1.HasInnerControl) ? "$('#" + cardInput.WtbCard1.ClientID + "').val()" : "''",
				(cardInput.WtbCard2.HasInnerControl) ? "$('#" + cardInput.WtbCard2.ClientID + "').val()" : "''",
				(cardInput.WtbCard3.HasInnerControl) ? "$('#" + cardInput.WtbCard3.ClientID + "').val()" : "''",
				(cardInput.WtbCard4.HasInnerControl) ? "$('#" + cardInput.WtbCard4.ClientID + "').val()" : "''");
				expireMonthScript = "$('#" + cardInput.WddlExpireMonth.ClientID + "').val()";
				expireYearScript = "$('#" + cardInput.WddlExpireYear.ClientID + "').val()";
				securityCodeScript = string.Format(
				"{0}",
				((cardInput.WtbSecurityCode.HasInnerControl) && OrderCommon.CreditSecurityCodeEnable)
					? "$('#" + cardInput.WtbSecurityCode.ClientID + "').val()"
					: "''");
				authornameScript = string.Format(
				"{0}",
				cardInput.WtbAuthorName.HasInnerControl ? "$('#" + cardInput.WtbAuthorName.ClientID + "').val()" : "''");
			}

			var cardInfoScript =
				string.Format(
					"{{"
					+ "'CardCompany' : {0},"
					+ "'CardNo' : {1},"
					+ "'ExpMM' : {2},"
					+ "'ExpYY' : {3},"
					+ "'SecurityCode' :{4},"
					+ "'AuthorName' : {5},"
					+ "'TokenHiddenID' : '{6}',"
					+ "'TokenHiddenIDSame1' : '{7}',"
					+ "'errorMessageID' : '{8}',"
					+ "'errorMessage' : '{9}',"
					+ "'errorFocusID' : '{10}',"
					+ "'PaymentParams' : {11},"
					+ "}}",
					cardKbnScript,
					cardnoScript,
					expireMonthScript,
					expireYearScript,
					securityCodeScript,
					authornameScript,
					cardInput.WhfCreditToken.HasInnerControl ? cardInput.WhfCreditToken.ClientID : "",
					((whfCreditTokenSameAs1 != null) && whfCreditTokenSameAs1.HasInnerControl) ? whfCreditTokenSameAs1.ClientID : "",
					(cardInput.WspanErrorMessageForCreditCard.HasInnerControl) ? cardInput.WspanErrorMessageForCreditCard.ClientID : "",
					StringUtility.EncodeToUnicodeString(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR).Replace("'", "\'")),
					(cardInput.WtbCard1.HasInnerControl) ? cardInput.WtbCard1.ClientID : "",
					"[" + string.Join(",", userCreditCardCooperationInfo.CreateParams().Select(val => string.Format("'{0}'", val))) + "]");
			return cardInfoScript;
		}

		/// <summary>
		/// クレジットトークン取得＆フォームセットJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="riParent">親リピーターアイテム</param>
		/// <returns>JSスクリプト</returns>
		public string CreateGetCreditTokenAndSetToFormJsScriptInner(RepeaterItem riParent = null)
		{
			if (OrderCommon.CreditTokenUse == false) return "";

			var scripts = CreateGetCreditTokenAndSetToFormJsScriptInner(
				new string[0],
				new WrappedCreditCardInputs(this, this.Page, riParent));
			return scripts;
		}
		/// <summary>
		/// クレジットトークン取得＆フォームセットJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="owner">注文者情報</param>
		/// <param name="wrCartList">カートリストリピーター</param>
		/// <returns>JSスクリプト</returns>
		public string CreateGetCreditTokenAndSetToFormJsScriptInner(WrappedRepeater wrCartList)
		{
			if (OrderCommon.CreditTokenUse == false) return "";

			// クレジットカード入力クラス、カート１と同じ決済向けトークンフィールドIDを取得
			var cardInputs = new List<WrappedCreditCardInputs>();
			var hfCreditTokenSameAs1Ids = new List<string>();
			foreach (RepeaterItem riCart in wrCartList.Items)
			{
				var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
				var cardInputsTmp = wrPayment.Items.Cast<RepeaterItem>().Where(
					riPayment =>
					{
						var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId");
						return (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
					}).Select(
						riPayment =>
						{
							var cardInput = new WrappedCreditCardInputs(this, this.Page, riPayment);
							return cardInput;
						});
				cardInputs.AddRange(cardInputsTmp);

				var hfCreditTokenSameAs1 = GetWrappedControl<WrappedHiddenField>(riCart, "hfCreditTokenSameAs1");
				hfCreditTokenSameAs1Ids.Add(hfCreditTokenSameAs1.ClientID);
			}
			if (cardInputs.Count == 0) return "";

			// どれか１つでもクレジットカード番号フォームが存在すればトークン取得処理実行
			var scripts = CreateGetCreditTokenAndSetToFormJsScriptInner(
				hfCreditTokenSameAs1Ids.ToArray(),
				cardInputs.ToArray());
			return scripts;
		}
		/// <summary>
		/// クレジットトークン取得＆フォームセットJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="hfCreditTokenSameAs1Ids">カート1と同じ決済利用用のトークンコントロールID（不要な場合は空の配列）</param>
		/// <param name="creditCardInputs">クレジットカード入力情報</param>
		/// <returns>JSスクリプト</returns>
		private string CreateGetCreditTokenAndSetToFormJsScriptInner(
			string[] hfCreditTokenSameAs1Ids,
			params WrappedCreditCardInputs[] creditCardInputs)
		{
			if (creditCardInputs.Length == 0) return "";

			// どれか１つでもクレジットカード番号フォームが存在すればトークン取得処理実行
			var scripts = "if ("
				+ string.Join(
					"||",
					creditCardInputs.Select(cardInput => string.Format("($('#{0}')[0])", cardInput.WtbCard1.ClientID))) + "){"
				+ string.Format(
					"GetTokenAndSetToForm(Array({0}), Array({1}));",
					string.Join(
						",",
						hfCreditTokenSameAs1Ids.Select(id => string.Format("'{0}'", id))),
					string.Join(
						",",
						hfCreditTokenSameAs1Ids.Select(id =>
						"[" + string.Join(",", UserCreditCardCooperationInfoFacade.CreateForGetToken().CreateParams().Select(val => string.Format("'{0}'", val)))
							+ "]"))) + "}";
			return scripts;
		}

		/// <summary>
		/// クレジットトークン向け フォームマスキングJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="riParent">親リピーターアイテム</param>
		/// <returns>JSスクリプト</returns>
		public virtual string CreateMaskFormsForCreditTokenJsScriptInner(RepeaterItem riParent = null)
		{
			if (OrderCommon.CreditTokenUse == false) return "";

			var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner(new WrappedCreditCardInputs(this, this.Page, riParent));
			return maskingScripts;
		}
		/// <summary>
		/// クレジットトークン向け フォームマスキングJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="wrCartList">カートリストリピーター</param>
		/// <returns>JSスクリプト</returns>
		public virtual string CreateMaskFormsForCreditTokenJsScriptInner(WrappedRepeater wrCartList)
		{
			if (OrderCommon.CreditTokenUse == false) return "";

			var cardInputs = new List<WrappedCreditCardInputs>();
			foreach (var wrPayment in wrCartList.Items.Cast<RepeaterItem>()
				.Select(ri => GetWrappedControl<WrappedRepeater>(ri, "rPayment")))
			{
				cardInputs.AddRange(wrPayment.Items.Cast<RepeaterItem>().Where(ri =>
				{
					var whfPaymentId = GetWrappedControl<WrappedHiddenField>(ri, "hfPaymentId");
					return (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
				}).Select(riPayment => new WrappedCreditCardInputs(this, this.Page, riPayment)));
			}
			if (cardInputs.Count == 0) return "";

			var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner(cardInputs.ToArray());
			return maskingScripts;
		}
		/// <summary>
		/// クレジットトークン向け フォームマスキングJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="cardInputs">クレジットカード入力情報</param>
		/// <returns>フォームマスキングJSスクリプト</returns>
		private string CreateMaskFormsForCreditTokenJsScriptInner(params WrappedCreditCardInputs[] cardInputs)
		{
			var maskingScripts = new StringBuilder();
			foreach (var cardInput in cardInputs)
			{
				var cardIds = new List<string>();
				if (cardInput.WtbCard1.HasInnerControl) cardIds.Add(cardInput.WtbCard1.ClientID);
				if (cardInput.WtbCard2.HasInnerControl) cardIds.Add(cardInput.WtbCard2.ClientID);
				if (cardInput.WtbCard3.HasInnerControl) cardIds.Add(cardInput.WtbCard3.ClientID);
				if (cardInput.WtbCard4.HasInnerControl) cardIds.Add(cardInput.WtbCard4.ClientID);

				// 「もしクレジットカードの新規入力だった場合、入力フォームをマスキング」するスクリプト
				var scripts =
					string.Format(
						"if ($('#{0}')[0] && $('#{1}')[0] && ($('#{1}').val())) ",
						(cardInput.WtbCard1.HasInnerControl) ? cardInput.WtbCard1.ClientID : "",
						(cardInput.WhfCreditToken.HasInnerControl) ? cardInput.WhfCreditToken.ClientID : "")
					+ "{"
					+ ((cardInput.WhfCreditBincode.HasInnerControl
							&& cardInput.WtbCard1.HasInnerControl)
						? string.Format(
							"$('#{0}').val($('#{1}').val() ? $('#{1}').val().substring(0, 6) : '');",
							cardInput.WhfCreditBincode.ClientID,
							cardInput.WtbCard1.ClientID)
						: string.Empty)
					+ string.Format("var cardIds = [{0}];", string.Join(",", cardIds.Select(id => string.Format("'{0}'", id))))
					+ string.Format("maskingInputForToken(cardIds, '{0}', 4);maskingInputForToken(['{1}'], '{0}', 0);",
						Constants.CHAR_MASKING_FOR_TOKEN,
						(cardInput.WtbSecurityCode.HasInnerControl) ? cardInput.WtbSecurityCode.ClientID : "")
					+ "}";
				maskingScripts.Append(scripts);
			}
			return maskingScripts.ToString();
		}

		/// <summary>
		/// トークンが入力されていたら入力画面を切り替える
		/// </summary>
		/// <param name="wrCartList">カートリストラップ済みリピータ</param>
		public void SwitchDisplayForCreditTokenInput(WrappedRepeater wrCartList)
		{
			if (OrderCommon.CreditTokenUse == false) return;

			foreach (RepeaterItem riCart in wrCartList.Items)
			{
				var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
				foreach (RepeaterItem riPayment in wrPayment.Items)
				{
					// HiddenFieldにトークンが入っていたら画面切り替え
					SwitchDisplayForCreditTokenInput(riPayment);
				}
			}
		}
		/// <summary>
		/// クレジットトークンが入力されていたら入力画面を切り替える
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		public void SwitchDisplayForCreditTokenInput(RepeaterItem riParent = null)
		{
			var wdivCreditCardNoToken = GetWrappedControl<WrappedHtmlGenericControl>(riParent, "divCreditCardNoToken");
			if (HasCreditToken(riParent) && wdivCreditCardNoToken.Visible)
			{
				wdivCreditCardNoToken.Visible = false;
				var wdivCreditCardForTokenAcquired = GetWrappedControl<WrappedHtmlGenericControl>(
					riParent,
					"divCreditCardForTokenAcquired");
				wdivCreditCardForTokenAcquired.Visible = true;
			}
			var cardInput = new WrappedCreditCardInputs(this, this.Page, riParent);
			SwitchDisplayForCreditTokenInput(cardInput);
		}
		/// <summary>
		/// クレジットトークンが入力されていたら入力画面を切り替える
		/// </summary>
		/// <param name="cardInputs">カード入力情報</param>
		private void SwitchDisplayForCreditTokenInput(WrappedCreditCardInputs cardInputs)
		{
			var hasCreditToken = HasCreditToken(cardInputs.WhfCreditToken);
			cardInputs.WdivCreditCardNoToken.Visible = (hasCreditToken == false);
			cardInputs.WdivCreditCardForTokenAcquired.Visible = hasCreditToken;

			if (OrderCommon.CreditTokenUse == false) return;
			if (hasCreditToken == false) return;

			var company = string.Empty;
			var lastForDigit = string.Empty;
			var month = string.Empty;
			var year = string.Empty;

			if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten)
			{
				company = cardInputs.WddlCardCompany.SelectedText;
				var cardNo = string.Concat(
					cardInputs.WtbCard1.Text,
					cardInputs.WtbCard2.Text,
					cardInputs.WtbCard3.Text,
					cardInputs.WtbCard4.Text);
				lastForDigit = cardNo.Replace(Constants.CHAR_MASKING_FOR_TOKEN, "");
				month = cardInputs.WddlExpireMonth.SelectedValue;
				year = cardInputs.WddlExpireYear.SelectedValue;
			}
			else
			{
				var isHasSession = (SessionManager.Session[Constants.SESSION_KEY_PARAM] is UserCreditCardInput);
				var userCreditCard = (isHasSession)
					? (UserCreditCardInput)SessionManager.Session[Constants.SESSION_KEY_PARAM]
					: null;

				lastForDigit = (isHasSession) ? userCreditCard.CardNo4 : cardInputs.WtbCard4.Text;
				month = (isHasSession) ? userCreditCard.ExpirationMonth : cardInputs.WhfMyExpirationMonthMount.Value;
				year = (isHasSession) ? userCreditCard.ExpirationYear : cardInputs.WhfMyExpirationYearMount.Value;
			}

			cardInputs.WlCreditCardCompanyNameForTokenAcquired.Text = HtmlSanitizer.HtmlEncode(company);
			cardInputs.WlLastFourDigitForTokenAcquired.Text = HtmlSanitizer.HtmlEncode(lastForDigit);
			cardInputs.WlExpirationMonthForTokenAcquired.Text = HtmlSanitizer.HtmlEncode(month);
			cardInputs.WlExpirationYearForTokenAcquired.Text = HtmlSanitizer.HtmlEncode(year);
			cardInputs.WlCreditAuthorNameForTokenAcquired.Text = HtmlSanitizer.HtmlEncode(cardInputs.WtbAuthorName.Text);
		}

		/// <summary>
		/// 入力フォームからクレジットトークン情報削除（再入力ボタンで利用）
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		/// <param name="inputErrorCssClass">フォーム入力エラーCSSクラス</param>
		/// <param name="restCustomValidate">カスタムばりデータリセットするか</param>
		public void ResetCreditTokenInfoFromForm(RepeaterItem riParent, string inputErrorCssClass, bool restCustomValidate = true)
		{
			var cardInput = new WrappedCreditCardInputs(this, this.Page, riParent);
			cardInput.WtbCard1.Text = "";
			cardInput.WtbCard2.Text = "";
			cardInput.WtbCard3.Text = "";
			cardInput.WtbCard4.Text = "";
			cardInput.WddlExpireMonth.SelectedIndex = 0;
			cardInput.WddlExpireYear.SelectedValue = (DateTime.Now.Year - 2000).ToString("00");
			cardInput.WtbSecurityCode.Text = "";
			cardInput.WtbAuthorName.Text = "";
			cardInput.WhfCreditToken.Value = "";
			cardInput.WlbEditCreditCardRakuten.Visible = true;

			if (restCustomValidate)
			{
				cardInput.WcvUserCreditCardName.IsValid = true;	// クレジットカード登録名はエラー無しとして設定（赤い背景だけ残ってしまう可能性）
				cardInput.WcvUserCreditCardName.ErrorMessage = "";
				cardInput.WtbUserCreditCardName.CssClass = cardInput.WtbUserCreditCardName.CssClass.Replace(inputErrorCssClass, "");

				cardInput.WcvCreditAuthorName.IsValid = true;	// クレジットカード名義人はエラー無しとして設定（赤い背景だけ残ってしまう可能性）
				cardInput.WcvCreditAuthorName.ErrorMessage = "";
				cardInput.WtbAuthorName.CssClass = cardInput.WtbAuthorName.CssClass.Replace(inputErrorCssClass, "");
			}
		}

		/// <summary>
		/// クレジットトークンを持っているか（表示切り替えに利用）
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		/// <returns>トークンを持っているか</returns>
		public virtual bool HasCreditToken(RepeaterItem riParent = null)
		{
			// 決済IDチェック（コントロールがない場合はfalseにならない）
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riParent, "hfPaymentId");
			if (whfPaymentId.HasInnerControl && (whfPaymentId.Value != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)) return false;

			var whfCreditToken = GetWrappedControl<WrappedHiddenField>(riParent, "hfCreditToken");
			var hasCreditToken = HasCreditToken(whfCreditToken);
			return hasCreditToken;
		}
		/// <summary>
		/// クレジットトークンを持っているか（表示切り替えに利用）
		/// </summary>
		/// <param name="whfCreditToken">トークン</param>
		/// <returns>トークンを持っているか</returns>
		private bool HasCreditToken(WrappedHiddenField whfCreditToken)
		{
			var hasCreditToken = ((whfCreditToken.HasInnerControl) && (string.IsNullOrEmpty(whfCreditToken.Value) == false));
			return hasCreditToken;
		}

		#region 内部クラス
		/// <summary>
		/// ラップ済みクレジットカード入力クラス
		/// </summary>
		public class WrappedCreditCardInputs
		{
			/// <summary>ページプロセス</summary>
			private readonly Process.CommonPageProcess m_pageProcess = null;
			/// <summary>親コントロール</summary>
			private readonly Control m_cParent = null;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="page">ページ</param>
			/// <param name="cPayment">親コントロール（カート内であれば決済リピータアイテム、それ以外はContentPlaceHolder）</param>
			public WrappedCreditCardInputs(CommonPage page, Control cPayment = null)
				: this(page.Process, page, cPayment)
			{
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="pageProcess">ページプロセス</param>
			/// <param name="page">ページ</param>
			/// <param name="cPayment">親コントロール（カート内であれば決済リピータアイテム、それ以外はContentPlaceHolder）</param>
			public WrappedCreditCardInputs(CommonPageProcess pageProcess, System.Web.UI.Page page, Control cPayment = null)
			{
				m_pageProcess = pageProcess;
				m_cParent = cPayment
					?? ((page.Master != null)
						? (page.Master.FindControl("ContentPlaceHolder1") ?? page.Master.FindControl("ContentPlaceHolderBody"))
						: page);
			}

			/// <summary>ユーザークレジットカード</summary>
			public WrappedDropDownList WddlUserCreditCard
			{
				get { return m_pageProcess.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlUserCreditCard"); }
			}

			/// <summary>クレジットカード会社</summary>
			public WrappedDropDownList WddlCardCompany
			{
				get { return m_pageProcess.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlCreditCardCompany"); }
			}
			/// <summary>カード番号1</summary>
			public WrappedTextBox WtbCard1
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbCreditCardNo1"); }
			}
			/// <summary>カード番号2</summary>
			public WrappedTextBox WtbCard2
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbCreditCardNo2"); }
			}
			/// <summary>カード番号3</summary>
			public WrappedTextBox WtbCard3
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbCreditCardNo3"); }
			}
			/// <summary>カード番号4</summary>
			public WrappedTextBox WtbCard4
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbCreditCardNo4"); }
			}
			/// <summary>有効期限(月)</summary>
			public WrappedDropDownList WddlExpireMonth
			{
				get { return m_pageProcess.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlCreditExpireMonth"); }
			}
			/// <summary>有効期限(年)</summary>
			public WrappedDropDownList WddlExpireYear
			{
				get { return m_pageProcess.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlCreditExpireYear"); }
			}
			/// <summary>カード名義人</summary>
			public WrappedTextBox WtbAuthorName
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbCreditAuthorName"); }
			}
			/// <summary>セキュリティコード</summary>
			public WrappedTextBox WtbSecurityCode
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbCreditSecurityCode"); }
			}
			/// <summary>トークン</summary>
			public WrappedHiddenField WhfCreditToken
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfCreditToken"); }
			}
			/// <summary>エラーメッセージフィールド</summary>
			public WrappedHtmlGenericControl WspanErrorMessageForCreditCard
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHtmlGenericControl>(m_cParent, "spanErrorMessageForCreditCard"); }
			}
			/// <summary>カード番号1 カスタムバリデータ</summary>
			public WrappedCustomValidator WcvCreditCardNo1
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCustomValidator>(m_cParent, "cvCreditCardNo1"); }
			}
			/// <summary>カード番号2 カスタムバリデータ</summary>
			public WrappedCustomValidator WcvCreditCardNo2
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCustomValidator>(m_cParent, "cvCreditCardNo2"); }
			}
			/// <summary>カード番号3 カスタムバリデータ</summary>
			public WrappedCustomValidator WcvCreditCardNo3
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCustomValidator>(m_cParent, "cvCreditCardNo3"); }
			}
			/// <summary>カード番号4 カスタムバリデータ</summary>
			public WrappedCustomValidator WcvCreditCardNo4
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCustomValidator>(m_cParent, "cvCreditCardNo4"); }
			}
			/// <summary>カード名義人 カスタムバリデータ</summary>
			public WrappedCustomValidator WcvCreditAuthorName
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCustomValidator>(m_cParent, "cvCreditAuthorName"); }
			}
			/// <summary>カードセキュリティコード カスタムバリデータ</summary>
			public WrappedCustomValidator WcvCreditSecurityCode
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCustomValidator>(m_cParent, "cvCreditSecurityCode"); }
			}
			/// <summary>トークン決済用：入力済み カード会社桁</summary>
			public WrappedLiteral WlCreditCardCompanyNameForTokenAcquired
			{
				get { return m_pageProcess.GetWrappedControl<WrappedLiteral>(m_cParent, "lCreditCardCompanyNameForTokenAcquired"); }
			}
			/// <summary>トークン決済用：入力済み カード番号下4桁</summary>
			public WrappedLiteral WlLastFourDigitForTokenAcquired
			{
				get { return m_pageProcess.GetWrappedControl<WrappedLiteral>(m_cParent, "lLastFourDigitForTokenAcquired"); }
			}
			/// <summary>トークン決済用：入力済み 有効期限(月)</summary>
			public WrappedLiteral WlExpirationMonthForTokenAcquired
			{
				get { return m_pageProcess.GetWrappedControl<WrappedLiteral>(m_cParent, "lExpirationMonthForTokenAcquired"); }
			}
			/// <summary>トークン決済用：入力済み 有効期限(年)</summary>
			public WrappedLiteral WlExpirationYearForTokenAcquired
			{
				get { return m_pageProcess.GetWrappedControl<WrappedLiteral>(m_cParent, "lExpirationYearForTokenAcquired"); }
			}
			/// <summary>トークン決済用：入力済み カード名義人</summary>
			public WrappedLiteral WlCreditAuthorNameForTokenAcquired
			{
				get { return m_pageProcess.GetWrappedControl<WrappedLiteral>(m_cParent, "lCreditAuthorNameForTokenAcquired"); }
			}
			/// <summary>支払い回数（新規クレジットカード用。統合したい）</summary>
			public WrappedDropDownList WddlInstallments
			{
				get { return m_pageProcess.GetWrappedControl<WrappedDropDownList>(m_cParent, "dllCreditInstallments"); }
			}
			/// <summary>支払い回数２（登録済みクレジットカード用。統合したい）</summary>
			public WrappedDropDownList WdllCreditInstallments2
			{
				get { return m_pageProcess.GetWrappedControl<WrappedDropDownList>(m_cParent, "dllCreditInstallments2"); }
			}
			/// <summary>支払い回数（Rakuten）</summary>
			public WrappedDropDownList WdllCreditInstallmentsRakuten
			{
				get { return m_pageProcess.GetWrappedControl<WrappedDropDownList>(m_cParent, "dllCreditInstallmentsRakuten"); }
			}
			/// <summary>クレジットカード登録するかチェックボックス</summary>
			public WrappedCheckBox WcbRegistCreditCard
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCheckBox>(m_cParent, "cbRegistCreditCard"); }
			}
			/// <summary>クレジットカード登録名</summary>
			public WrappedTextBox WtbUserCreditCardName
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbUserCreditCardName"); }
			}
			/// <summary>クレジットカード登録名 カスタムバリデータ</summary>
			public WrappedCustomValidator WcvUserCreditCardName
			{
				get { return m_pageProcess.GetWrappedControl<WrappedCustomValidator>(m_cParent, "cvUserCreditCardName"); }
			}
			/// <summary>クレジットカード入力エリア（トークンなしのとき）</summary>
			public WrappedHtmlGenericControl WdivCreditCardNoToken
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHtmlGenericControl>(m_cParent, "divCreditCardNoToken"); }
			}
			/// <summary>クレジットカード入力エリア（トークンありのとき）</summary>	
			public WrappedHtmlGenericControl WdivCreditCardForTokenAcquired
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHtmlGenericControl>(m_cParent, "divCreditCardForTokenAcquired"); }
			}
			/// <summary>クレジットカート番号保持フィールド（楽天）</summary>
			public WrappedHiddenField WhfMyCardMount
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfMyCardMount"); }
			}
			/// <summary>セキュリティコードトークン保持フィールド（楽天）</summary>
			public WrappedHiddenField WhfMyCvvMount
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfMyCvvMount"); }
			}
			/// <summary>有効月保持フィールド（楽天）</summary>
			public WrappedHiddenField WhfMyExpirationMonthMount
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfMyExpirationMonthMount"); }
			}
			/// <summary>有効年保持フィールド（楽天）</summary>
			public WrappedHiddenField WhfMyExpirationYearMount
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfMyExpirationYearMount"); }
			}
			/// <summary>名義人保持フィールド（楽天）</summary>
			public WrappedTextBox WhfAuthorCardName
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "ucRakutenCreditCard$tbCreditAuthorNameRakuten"); }
			}
			/// <summary>名義人保持フィールド（楽天）</summary>
			public WrappedTextBox WhfAuthorCardNameRegis
			{
				get { return m_pageProcess.GetWrappedControl<WrappedTextBox>(m_cParent, "tbCreditAuthorNameRakuten"); }
			}
			/// <summary>クレカ会社保持フィールド（楽天）</summary>
			public WrappedHiddenField WhfCreditCardCompany
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHiddenField>(m_cParent, "WhfCreditCardCompany"); }
			}
			/// <summary>クレカ情報入力ボタン（楽天）</summary>
			public WrappedLinkButton WlbEditCreditCardRakuten
			{
				get { return m_pageProcess.GetWrappedControl<WrappedLinkButton>(m_cParent, "lbEditCreditCardRakuten"); }
			}
			/// <summary>Wrapped hidden field bincode</summary>
			public WrappedHiddenField WhfCreditBincode
			{
				get { return m_pageProcess.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfCreditBincode"); }
			}
		}
		#endregion
	}
}
