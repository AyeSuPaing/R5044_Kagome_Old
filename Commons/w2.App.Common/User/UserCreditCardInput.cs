/*
=========================================================================================================
  Module      : ユーザークレジットカード入力クラス(UserCreditCardInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.User
{
	/// <summary>
	/// ユーザークレジットカード入力クラス（フロント、管理のユーザーカード＆定期画面で利用）
	/// </summary>
	[Serializable]
	public class UserCreditCardInput : InputBase<UserCreditCardModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserCreditCardInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public UserCreditCardInput(UserCreditCardModel model)
			: this()
		{
			this.UserId = model.UserId;
			this.BranchNo = model.BranchNo.ToString();
			this.CardDispName = model.CardDispName;
			this.ExpirationMonth = model.ExpirationMonth;
			this.ExpirationYear = model.ExpirationYear;
			this.AuthorName = model.AuthorName;
			this.DispFlg = model.DispFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.CompanyCode = model.CompanyCode;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="cooperationId">連携ID1</param>
		/// <param name="cooperationId2">連携ID2</param>
		/// <param name="dispFlg">表示フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="registerStatus">登録ステータス</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel CreateModel(
			string cooperationId,
			string cooperationId2,
			bool dispFlg,
			string lastChanged,
			string registerStatus = Constants.FLG_USERCREDITCARD_REGISTER_STATUS_NORMAL)
		{
			var model = CreateModel();
			model.CooperationId = cooperationId;
			model.CooperationId2 = cooperationId2;
			model.DispFlg = dispFlg ? Constants.FLG_USERCREDITCARD_DISP_FLG_ON : Constants.FLG_USERCREDITCARD_DISP_FLG_OFF;
			model.RegisterStatus = registerStatus;
			model.LastChanged = lastChanged;
			return model;
		}
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override UserCreditCardModel CreateModel()
		{
			var startIndex = this.CardNo.Length - 4;
			var lastFourDigit = ((startIndex < 0) ? this.CardNo : this.CardNo.Substring(startIndex, 4));
			var model = new UserCreditCardModel
			{
				UserId = this.UserId,
				BranchNo = string.IsNullOrEmpty(this.BranchNo) ? 0 : int.Parse(this.BranchNo),
				CardDispName = this.CardDispName,
				LastFourDigit = lastFourDigit,
				ExpirationMonth = this.ExpirationMonth,
				ExpirationYear = this.ExpirationYear,
				AuthorName = this.AuthorName,
				DispFlg = this.DispFlg,
				LastChanged = this.LastChanged,
				CompanyCode = this.CompanyCode,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="regsterUserCredtCard">クレジットカード登録するか</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(bool regsterUserCredtCard = true)
		{
			var messageList = Validator.Validate(
				"UserCreditCardRegist",
				GetInputForValidate(regsterUserCredtCard));
			return string.Join("<br />", messageList.Select(kvp => kvp.Value));
		}

		/// <summary>
		/// 検証（フロントユーザーカード登録）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public Dictionary<string, string> ValidateForFrontUserCreditCardRegist()
		{
			var messageList = Validator.Validate("UserCreditCardRegist", GetInputForValidate());
			return messageList
				.GroupBy(kvp => kvp.Key)
				.Select(group => group.First())
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		/// <summary>
		/// 検証（フロント定期登録向け）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public Dictionary<string, string> ValidateForFrontFixedPurchaseRegist()
		{
			var messageList = Validator.Validate("OrderPayment", GetInputForValidate());
			return messageList.Select(kvp => kvp.Key).Distinct()
				.ToDictionary(key =>
					key,
					key => string.Join("\r\n", messageList.Where(kvp => kvp.Key == key).Select(kvp => kvp.Value)));
		}

		/// <summary>
		/// 検証用入力情報取得
		/// </summary>
		/// <param name="regsterUserCredtCard">クレジットカード登録するか</param>
		/// <returns>検証用入力情報</returns>
		protected Hashtable GetInputForValidate(bool regsterUserCredtCard = true)
		{
			var input = new Hashtable
			{
				{ CartPayment.FIELD_REGIST_CREDIT_CARD_NAME, regsterUserCredtCard ? this.CardDispName : "DUMMY" }
			};
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) return input;

			if ((Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Zeus)
				|| (Constants.PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED == false))
			{
				if (OrderCommon.CreditTokenUse == false)
				{
					input.Add(CartPayment.FIELD_CREDIT_CARD_NO, this.CardNo);
					input.Add(CartPayment.FIELD_CREDIT_CARD_NO + "_length", this.CardNo);
					input.Add(CartPayment.FIELD_CREDIT_CARD_NO_1, this.CardNo1);
					input.Add(CartPayment.FIELD_CREDIT_CARD_NO_2, this.CardNo2);
					input.Add(CartPayment.FIELD_CREDIT_CARD_NO_3, this.CardNo3);
					input.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, this.CardNo4);
					if (OrderCommon.CreditSecurityCodeEnable && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten))
					{
						input.Add(CartPayment.FIELD_CREDIT_SECURITY_CODE, this.SecurityCode);
					}
				}
				else
				{
					if ((this.CreditToken == null) || string.IsNullOrEmpty(this.CreditToken.Token))
					{
						input.Add(CartPayment.FIELD_CREDIT_CARD_NO, this.CardNo);
						input.Add(CartPayment.FIELD_CREDIT_CARD_NO + "_length", this.CardNo);
						input.Add(CartPayment.FIELD_CREDIT_CARD_NO_1, this.CardNo1);
						if (OrderCommon.CreditSecurityCodeEnable)
						{
							input.Add(CartPayment.FIELD_CREDIT_SECURITY_CODE, this.SecurityCode);
						}
					}
					input.Add(CartPayment.FIELD_CREDIT_CARD_NO_4, StringUtility.ToHankaku(this.CardNo4).Replace(Constants.CHAR_MASKING_FOR_TOKEN, ""));
				}
				input.Add(CartPayment.FIELD_CREDIT_EXPIRE_MONTH, this.ExpirationMonth);
				input.Add(CartPayment.FIELD_CREDIT_EXPIRE_YEAR, this.ExpirationYear);
				input.Add(CartPayment.FIELD_CREDIT_AUTHOR_NAME, this.AuthorName);
			}
			return input;
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
				this.CreditToken = null;	// 有効期限切れであればリセット
				return true;
			}
			return false;
		}

		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_USER_ID] = value; }
		}
		/// <summary>カード枝番</summary>
		public string BranchNo
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_BRANCH_NO] = value; }
		}
		/// <summary>カード表示名</summary>
		public string CardDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_CARD_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_CARD_DISP_NAME] = value; }
		}
		/// <summary>カード番号</summary>
		public string CardNo
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_CARD_NO] = value; }
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
		/// <summary>有効期限（月）</summary>
		public string ExpirationMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_MONTH]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_MONTH] = value; }
		}
		/// <summary>有効期限（年）</summary>
		public string ExpirationYear
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_YEAR]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_EXPIRATION_YEAR] = value; }
		}
		/// <summary>カード名義人</summary>
		public string AuthorName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_AUTHOR_NAME]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_AUTHOR_NAME] = value; }
		}
		/// <summary>セキュリティコード</summary>
		public string SecurityCode
		{
			get { return (string)this.DataSource[CartPayment.FIELD_CREDIT_SECURITY_CODE]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_SECURITY_CODE] = value; }
		}
		/// <summary>表示フラグ</summary>
		public string DispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_DISP_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCREDITCARD_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERCREDITCARD_LAST_CHANGED] = value; }
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
		/// <summary>クレジットCVVトークン（楽天カード登録用）</summary>
		public CartPayment.CreditTokenInfoBase CreditCvvToken
		{
			get { return (CartPayment.CreditTokenInfoBase)this.DataSource[CartPayment.FIELD_CREDIT_CVV_TOKEN_HIDDEN_VALUE]; }
			set { this.DataSource[CartPayment.FIELD_CREDIT_CVV_TOKEN_HIDDEN_VALUE] = value; }
		}
		#endregion
	}
}
