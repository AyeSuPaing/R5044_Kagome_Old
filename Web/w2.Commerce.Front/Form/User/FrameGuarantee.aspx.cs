/*
=========================================================================================================
  Module      : 枠保証登録情報(FrameGuarantee.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.FrameGuarantee;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserBusinessOwner;

public partial class Form_User_FrameGuarantee : UserPage
{
	#region Wrapped control
	protected WrappedTextBox wtbOwnerName1 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerName1"); } }
	protected WrappedTextBox wtbOwnerName2 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerName2"); } }
	protected WrappedTextBox wtbOwnerNameKana1 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerNameKana1"); } }
	protected WrappedTextBox wtbOwnerNameKana2 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerNameKana2"); } }
	protected WrappedDropDownList WddlOwnerBirthYear { get { return GetWrappedControl<WrappedDropDownList>("ddlOwnerBirthYear"); } }
	protected WrappedDropDownList WddlOwnerBirthMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlOwnerBirthMonth"); } }
	protected WrappedDropDownList WddlOwnerBirthDay { get { return GetWrappedControl<WrappedDropDownList>("ddlOwnerBirthDay"); } }
	protected WrappedTextBox WtbRequestBudget { get { return GetWrappedControl<WrappedTextBox>("tbRequestBudget"); } }
	#endregion

	/// <summary>エラーメッセージ</summary>
	public List<string> errorMessage = new List<string>();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		CheckGmoPaymentEnabled();

		if (!IsPostBack)
		{
			if (Constants.TWOCLICK_OPTION_ENABLE == false)
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
				var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR).CreateUrl();
				this.Response.Redirect(url);
			}
			this.WddlOwnerBirthYear.Items.Add(string.Empty);
			this.WddlOwnerBirthYear.AddItems(DateTimeUtility.GetBirthYearListItem());
			this.WddlOwnerBirthMonth.Items.Add(string.Empty);
			this.WddlOwnerBirthMonth.AddItems(DateTimeUtility.GetMonthListItem());
			this.WddlOwnerBirthDay.Items.Add(string.Empty);
			this.WddlOwnerBirthDay.AddItems(DateTimeUtility.GetDayListItem());

			var user = new UserBusinessOwnerService().GetByUserId(this.LoginUserId);
			if (user != null)
			{
				this.DisplayUserInfo(user);
			}
		}
	}

	/// <summary>
	/// GMO掛け払いの設定がオンかオフか確認する
	/// </summary>
	private void CheckGmoPaymentEnabled()
	{
		if (Constants.PAYMENT_GMO_POST_ENABLED == false)
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			urlCreator.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP);
			this.Response.Redirect(urlCreator.CreateUrl());
		}
	}

	/// <summary>
	/// ユーザー情報を画面に表示する
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	private void DisplayUserInfo(UserBusinessOwnerModel user)
	{
		this.wtbOwnerName1.Text = user.OwnerName1;
		this.wtbOwnerName2.Text = user.OwnerName2;
		this.wtbOwnerNameKana1.Text = user.OwnerNameKana1;
		this.wtbOwnerNameKana2.Text = user.OwnerNameKana2;
		if (user.Birth != null) {
			var birth = DateTime.Parse(user.Birth.ToString());
			this.WddlOwnerBirthDay.SelectedValue = birth.Day.ToString();
			this.WddlOwnerBirthMonth.SelectedValue = birth.Month.ToString();
			this.WddlOwnerBirthYear.SelectedValue = birth.Year.ToString();
		}

		this.lcreditStatus.Text = ValueText.GetValueText(Constants.TABLE_USER_BUSINESS_OWNER, Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS, user.CreditStatus);
		this.WtbRequestBudget.Text = user.RequestBudget.ToString();
	}

	/// <summary>
	/// 確認するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e) 
	{
		this.errorMessage = new List<string>();
		UserBusinessOwnerInput userInfo = CreateInputData();
		var userService = new UserService();
		var updateUser = userService.Get(this.LoginUserId);
		var excludeList = new List<string>();
		userInfo.DataSource[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE] = updateUser.AccessCountryIsoCode;
		var errorMessages = userInfo.Validate(UserBusinessOwnerInput.EnumUserInputValidationKbn.Modify, excludeList);
		
		// エラーが無い場合
		if (errorMessages.Count == 0)
		{
			var userBusinessOwnerService = new UserBusinessOwnerService();
			var userBusinessOwnerInput = userInfo.CreateModel();
			userBusinessOwnerInput.UserId = this.LoginUserId;

			if (userBusinessOwnerInput.RequestBudget > 0)
			{
				userBusinessOwnerInput.CreditStatus = Constants.FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW_EN;
			}
			userBusinessOwnerInput.ShopCustomerId = string.Format("{0}-F", this.LoginUserId);

			// 履歴とともに更新
			var userBusinessOwner = userBusinessOwnerService.GetByUserId(this.LoginUserId);
			var gmo = new GmoTransactionApi();
			var result = new GmoResponseFrameGuarantee();
			if (userBusinessOwner == null)
			{
				//Frame Gurantee Register
				var request = new GmoRequestFrameGuaranteeRegister(updateUser, userBusinessOwnerInput);
				result = gmo.FrameGuaranteeRegister(request);
				if (result.IsResultOk)
				{
					lcreditStatus.Text = ValueText.GetValueText(
						Constants.TABLE_USER_BUSINESS_OWNER,
						Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS,
						userBusinessOwnerInput.CreditStatus);
					userBusinessOwnerService.Insert(userBusinessOwnerInput);
				}
			}
			else
			{
				if (userBusinessOwner.CheckChange(userBusinessOwnerInput))
				{
					userBusinessOwnerInput.ShopCustomerId = userBusinessOwner.ShopCustomerId;
					var request = new GmoRequestFrameGuaranteeUpdate(updateUser, userBusinessOwnerInput);
					result = gmo.FrameGuaranteeUpdate(request);
					if (result.IsResultOk)
					{
						lcreditStatus.Text = ValueText.GetValueText(
							Constants.TABLE_USER_BUSINESS_OWNER,
							Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS,
							userBusinessOwnerInput.CreditStatus);
						userBusinessOwnerService.Update(userBusinessOwnerInput);
					}
				}
			}
			if (result.Errors != null && result.Errors.Error != null)
			{
				foreach (var item in result.Errors.Error)
				{
					var message = this.errorMessage.Find(x => x == item.ErrorMessage);
					if (string.IsNullOrWhiteSpace(message))
					{
						this.errorMessage.Add(item.ErrorMessage);
					}
				}
			}
		}

		GetErrMsgAndFocusToCV(errorMessages);
	}

	/// <summary>
	///  エラーメッセージをカスタムバリデータにセットしてフォーカス
	/// </summary>
	/// <param name="errorMsg">エラーメッセージ一覧</param>
	private void GetErrMsgAndFocusToCV(Dictionary<string, string> errorMsg)
	{
		// カスタムバリデータ取得
		List<CustomValidator> lCustomValidators = new List<CustomValidator>();
		CreateCustomValidators(this, lCustomValidators);

		// エラーをカスタムバリデータへ
		SetControlViewsForError("UserBusinessOwner", errorMsg, lCustomValidators);

		// エラーフォーカス
		// HACK:JSでおｋ（その方がお客さんも幸せでは？）
		foreach (CustomValidator cvCustomValidator in lCustomValidators)
		{
			WebControl wcTarget = (WebControl)cvCustomValidator.Parent.FindControl(cvCustomValidator.ControlToValidate);
			if (wcTarget != null)
			{
				if (cvCustomValidator.IsValid == false)
				{
					switch (wcTarget.ID)
					{
						// ターゲットにフォーカス
						default:
							wcTarget.Focus();
							break;
					}
					break;
				}
				else
				{
					cvCustomValidator.ErrorMessage = "";
					wcTarget.CssClass = wcTarget.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, " ");
				}
			}
		}
	}

	/// <summary>
	/// ビジネスオーナーのデータを作成する
	/// </summary>
	/// <returns>ビジネスオーナーの入力<returns>
	private UserBusinessOwnerInput CreateInputData()
	{
		UserBusinessOwnerInput userInput = new UserBusinessOwnerInput(new UserBusinessOwnerModel());
		userInput.OwnerName1 = DataInputUtility.ConvertToFullWidthBySetting(this.wtbOwnerName1.Text);
		userInput.OwnerName2 = DataInputUtility.ConvertToFullWidthBySetting(this.wtbOwnerName2.Text);
		userInput.OwnerNameKana1 = StringUtility.ToZenkaku(this.wtbOwnerNameKana1.Text);
		userInput.OwnerNameKana2 = StringUtility.ToZenkaku(this.wtbOwnerNameKana2.Text);
		if (string.IsNullOrWhiteSpace(this.WtbRequestBudget.Text) == false)
		{
			userInput.RequestBudget = this.WtbRequestBudget.Text;
		}
		userInput.BirthYear = this.WddlOwnerBirthYear.SelectedValue;
		userInput.BirthMonth = this.WddlOwnerBirthMonth.SelectedValue;
		userInput.BirthDay = this.WddlOwnerBirthDay.SelectedValue;

		if ((string.IsNullOrWhiteSpace(userInput.BirthYear) == false) ||
			(string.IsNullOrWhiteSpace(userInput.BirthMonth) == false) ||
			(string.IsNullOrWhiteSpace(userInput.BirthDay) == false))
		{
			userInput.Birth = String.Format("{0}/{1}/{2}", userInput.BirthYear, userInput.BirthMonth, userInput.BirthDay);
		}
		else
		{
			userInput.Birth = null;
		}

		return userInput;
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }
}