/*
=========================================================================================================
  Module      : 注文情報クレジットカード入力クラス (OrderCreditCardInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Common.Util;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 注文情報クレジットカード入力クラス
	/// </summary>
	[Serializable]
	public class OrderCreditCardInput
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderCreditCardInput()
		{
			this.DataSource = new Hashtable();
			this.DoRegister = false;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			// 新規入力の時のみチェックする
			if (this.CreditBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				var errorMessages = Util.Validator.Validate("UserCreditCardRegist", this.DataSource);
				return errorMessages;
			}
			return "";
		}
		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="isSuccessfulCardRegistration">カード登録に成功したか</param>
		/// <param name="cardUnregisteredErrorMessage">カード未登録エラーメッセージ</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(bool isSuccessfulCardRegistration, string cardUnregisteredErrorMessage)
		{
			// 新規入力の時のみチェックする
			if (this.CreditBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				var errorMessages = Util.Validator.Validate("UserCreditCardRegist", this.DataSource);
				if (isSuccessfulCardRegistration == false)
				{
					errorMessages += cardUnregisteredErrorMessage;
				}
				return errorMessages;
			}
			return "";
		}

		/// <summary>
		/// ユーザークレジットカードモデル作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>表示用ユーザークレジットカードモデル</returns>
		public UserCreditCardInput CeateUserCreditCardInput(string userId)
		{
			var creditCardInput = new UserCreditCardInput
			{
				UserId = userId,
				//BranchNo	// 採番
				CompanyCode = OrderCommon.CreditCompanySelectable ? this.CompanyCode : "",
				CardNo = this.CardNo,
				CardNo1 = this.CardNo1,
				CardNo2 = this.CardNo2,
				CardNo3 = this.CardNo3,
				CardNo4 = this.CardNo4,
				ExpirationMonth = this.ExpireMonth,
				ExpirationYear = this.ExpireYear,
				AuthorName = this.AuthorName,
				DispFlg = this.DoRegister ? Constants.FLG_USERCREDITCARD_DISP_FLG_ON : Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				CardDispName = this.RegisterCardName,
				SecurityCode = this.SecurityCode,
				CreditToken = this.CreditToken,
			};
			return creditCardInput;
		}

		/// <summary>
		/// 表示用ユーザークレジットカードモデル作成
		/// </summary>
		/// <returns>表示用ユーザークレジットカードモデル</returns>
		public UserCreditCardModel CeateUserCreditCardModelForDsip()
		{
			var model = new UserCreditCardModel
			{
				LastFourDigit = this.CardNo,
				ExpirationMonth = this.ExpireMonth,
				ExpirationYear = this.ExpireYear,
				AuthorName = this.AuthorName,
				CardDispName = this.RegisterCardName,
				DispFlg = this.DoRegister ? Constants.FLG_USERCREDITCARD_DISP_FLG_ON : Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
			};
			return model;
		}

		/// <summary>
		/// トークン有効期限切れをチェックする（切れていた場合はトークン削除）
		/// </summary>
		/// <returns>有効期限切れか</returns>
		public bool IsTokenExpired()
		{
			if (OrderCommon.CreditTokenUse == false) return false;
			if (this.CreditToken.IsExpired)
			{
				this.CreditToken = null;    // 有効期限切れであればリセット
				return true;
			}
			return false;
		}
		#endregion

		#region プロパティ
		/// <summary>カード番号</summary>
		public string CardNo
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO]; }
			set
			{
				this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO] = value;
				this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO + "_length"] = value;
			}
		}
		/// <summary>カード番号1</summary>
		public string CardNo1
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_1]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_1] = value; }
		}
		/// <summary>カード番号2</summary>
		public string CardNo2
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_2]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_2] = value; }
		}
		/// <summary>カード番号3</summary>
		public string CardNo3
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_3]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_3] = value; }
		}
		/// <summary>カード番号4</summary>
		public string CardNo4
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_4]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO_4] = value; }
		}
		/// <summary>有効期限(月)</summary>
		public string ExpireMonth
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_EXPIRE_MONTH]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_EXPIRE_MONTH] = value; }
		}
		/// <summary>有効期限(年)</summary>
		public string ExpireYear
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_EXPIRE_YEAR]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_EXPIRE_YEAR] = value; }
		}
		/// <summary>支払回数コード</summary>
		public string InstallmentsCode
		{
			get { return StringUtility.ToEmpty(this.DataSource[CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE]); }
			set { this.DataSource[CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE] = value; }
		}
		/// <summary>支払回数文言</summary>
		public string InstallmentsName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDER_CARD_INSTRUMENTS]); }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_INSTRUMENTS] = value; }
		}
		/// <summary>カード名義人</summary>
		public string AuthorName
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_AUTHOR_NAME]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_AUTHOR_NAME] = value; }
		}
		/// <summary>セキュリティコード</summary>
		public string SecurityCode
		{
			get { return StringUtility.ToEmpty(this.DataSource[CartPayment.FIELD_CREDIT_SECURITY_CODE]); }
			set { this.DataSource[CartPayment.FIELD_CREDIT_SECURITY_CODE] = value; }
		}
		/// <summary>登録する？</summary>
		public bool DoRegister
		{
			get { return (bool)this.DataSource[CartPayment.FIELD_CREDIT_CARD_REGIST_FLG]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_CARD_REGIST_FLG] = value; }
		}
		/// <summary>登録クレジットカード名</summary>
		public string RegisterCardName
		{
			get { return (string)this.DataSource[CartPayment.FIELD_REGIST_CREDIT_CARD_NAME]; }
			set { this.DataSource[CartPayment.FIELD_REGIST_CREDIT_CARD_NAME] = value; }
		}
		/// <summary>カード会社コード</summary>
		public string CompanyCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_COMPANY_CODE]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_COMPANY_CODE] = value; }
		}
		/// <summary>クレジットトークン（登録用）</summary>
		public CartPayment.CreditTokenInfoBase CreditToken
		{
			get { return (CartPayment.CreditTokenInfoBase)this.DataSource[CartPayment.FIELD_CREDIT_TOKEN_HIDDEN_VALUE]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_TOKEN_HIDDEN_VALUE] = value; }
		}
		/// <summary>クレジットカード支番</summary>
		public string CreditBranchNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = value; }
		}
		/// <summary>データソース</summary>
		public Hashtable DataSource { get; set; }
		/// <summary>Credit bincode</summary>
		public string CreditBincode
		{
			get { return StringUtility.ToEmpty(this.DataSource[CartPayment.FIELD_CREDIT_BINCODE]); }
			set { this.DataSource[CartPayment.FIELD_CREDIT_BINCODE] = value; }
		}
		#endregion
	}
}