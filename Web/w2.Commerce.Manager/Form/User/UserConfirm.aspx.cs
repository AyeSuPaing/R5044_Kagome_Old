/*
=========================================================================================================
  Module      : ユーザー情報確認ページ処理(UserConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.User.SocialLogin.Util;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.UpdateHistory;
using w2.Domain.User;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserBusinessOwner;
using w2.App.Common.Order.Payment.GMO.FrameGuarantee;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Global;
using System.Collections.Generic;

public partial class Form_User_UserConfirm : BasePage
{
	private const string REQUEST_KEY_UPDATE = "update";
	private const string REQUEST_KEY_REGIST = "regist";
	private const string REQUEST_KEY_USER_WITHDRAWAL = "withdrawal";
	private string m_strUserId = null;
	private string _uniqueKey;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		m_strUserId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_ID]);
		_uniqueKey = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_UNIQUE_KEY]);
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			// アクションステータス取得
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			if (Request[REQUEST_KEY_UPDATE] != null)
			{
				lMessages.Text = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_CONFIRM_CONDITION,
					Constants.VALUETEXT_PARAM_RESULT_MESSAGE,
					Constants.VALUETEXT_PARAM_USER_CONFIRM_UPDATE);
				lMessages.Visible = true;
			}
			else if (Request[REQUEST_KEY_REGIST] != null)
			{
				lMessages.Text = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_CONFIRM_CONDITION,
					Constants.VALUETEXT_PARAM_RESULT_MESSAGE,
					Constants.VALUETEXT_PARAM_USER_CONFIRM_REGIST);
				lMessages.Visible = true;
			}
			else if (Request[REQUEST_KEY_USER_WITHDRAWAL] != null)
			{
				lMessages.Text = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_CONFIRM_CONDITION,
					Constants.VALUETEXT_PARAM_RESULT_MESSAGE,
					Constants.VALUETEXT_PARAM_USER_CONFIRM_DELETE);
				lMessages.Visible = true;
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		spanUpdateHistoryConfirmTop.Visible = false;
		btnEditTop.Visible = btnEditBottom.Visible = false;
		btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
		btnRegistTop.Visible = btnRegistBottom.Visible = false;

		btnHistoryBackTop.Visible = btnHistoryBackBottom.Visible = false;
		btnBackListTop.Visible = btnBackListBottom.Visible = false;
		trDetail.Visible = false;
		trConfirm.Visible = false;

		// ユーザ情報更新可否
		bool blUpdateEnabled = MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_UPDATE);

		// 詳細
		if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnBackListTop.Visible = btnBackListBottom.Visible = true;
			trDetail.Visible = true;
			spanUpdateHistoryConfirmTop.Visible = btnEditTop.Visible = btnEditBottom.Visible = blUpdateEnabled;
			// ポップアップ表示制御
			if (this.IsPopUp)
			{
				// 一覧へ戻るを非表示へ
				btnBackListTop.Visible = btnBackListBottom.Visible = false;
			}

			// Show anchor link
			divAnchor.Visible = true;
		}
		// 新規・新規コピー
		else if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			if (this.UserInput == null)
			{
				// ユーザー登録の後にブラウザバックを行うとシステムエラーになるため、
				// ユーザー一覧ページに遷移させる
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_LIST);
			}

			btnHistoryBackTop.Visible = btnHistoryBackBottom.Visible = true;
			trConfirm.Visible = true;
			btnRegistTop.Visible = btnRegistBottom.Visible = blUpdateEnabled;
			// ポップアップ表示制御
			if (this.IsPopUp)
			{
				// 一覧へ戻るを非表示へ
				btnBackListTop.Visible = btnBackListBottom.Visible = false;
			}
		}
		// 更新？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnHistoryBackTop.Visible = btnHistoryBackBottom.Visible = true;
			trConfirm.Visible = true;
			btnUpdateTop.Visible = btnUpdateBottom.Visible = blUpdateEnabled;
		}
	}


	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(Server.UrlEncode(m_strUserId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var userInput = this.UserInput;
		var user = userInput.CreateModel();
		user.UserExtend = userInput.UserExtendInput.CreateModel();

		// パラメータのUserIDとセッション内のユーザIDの整合性を確認する(ポップアップ同時起動対策)
		if (user.UserId == Request[Constants.REQUEST_KEY_USER_ID])
		{
			var userService = new UserService();

			// 変更前のユーザ情報を取得
			var originalUser = userService.Get(user.UserId);
			var beforeUser = (UserModel)originalUser.Clone();
			beforeUser.UserExtend = new UserExtendModel(
				(Hashtable)originalUser.UserExtend.DataSource.Clone(),
				originalUser.UserExtend.UserExtendSettings);
			string beforMemberRankId = beforeUser.MemberRankId;

			beforeUser.UserKbn = user.UserKbn;
			beforeUser.Name = user.Name;
			beforeUser.Name1 = user.Name1;
			beforeUser.Name2 = user.Name2;
			beforeUser.NameKana = user.NameKana;
			beforeUser.NameKana1 = user.NameKana1;
			beforeUser.NameKana2 = user.NameKana2;
			beforeUser.NickName = user.NickName;
			beforeUser.Birth = user.Birth;
			beforeUser.BirthYear = user.BirthYear;
			beforeUser.BirthMonth = user.BirthMonth;
			beforeUser.BirthDay = user.BirthDay;
			beforeUser.Sex = user.Sex;
			beforeUser.MailAddr = user.MailAddr;
			beforeUser.MailAddr2 = user.MailAddr2;
			beforeUser.Zip = user.Zip;
			beforeUser.Zip1 = user.Zip1;
			beforeUser.Zip2 = user.Zip2;
			beforeUser.Addr = user.Addr;
			beforeUser.Addr1 = user.Addr1;
			beforeUser.Addr2 = user.Addr2;
			beforeUser.Addr3 = user.Addr3;
			beforeUser.Addr4 = user.Addr4;
			beforeUser.Addr5 = user.Addr5;
			beforeUser.AddrCountryName = user.AddrCountryName;
			beforeUser.AddrCountryIsoCode = user.AddrCountryIsoCode;
			beforeUser.CompanyName = user.CompanyName;
			beforeUser.CompanyPostName = user.CompanyPostName;
			beforeUser.Tel1 = user.Tel1;
			beforeUser.Tel1_1 = user.Tel1_1;
			beforeUser.Tel1_2 = user.Tel1_2;
			beforeUser.Tel1_3 = user.Tel1_3;
			beforeUser.Tel2 = user.Tel2;
			beforeUser.Tel2_1 = user.Tel2_1;
			beforeUser.Tel2_2 = user.Tel2_2;
			beforeUser.Tel2_3 = user.Tel2_3;
			beforeUser.Tel3 = user.Tel3;
			beforeUser.Tel3_1 = user.Tel3_1;
			beforeUser.Tel3_2 = user.Tel3_2;
			beforeUser.Tel3_3 = user.Tel3_3;
			beforeUser.MailFlg = user.MailFlg;
			beforeUser.EasyRegisterFlg = user.EasyRegisterFlg;
			beforeUser.RemoteAddr = user.RemoteAddr;
			beforeUser.LoginId = user.LoginId;
			// パスワード変更する場合 ゲストユーザー、ソーシャルログイン連携したユーザーは空文字も可
			if (user.Password != string.Empty
				|| UserService.IsGuest(user.UserKbn)
				|| (SocialLoginUtil.GetProviders(null, user.UserId) != null))
			{
				beforeUser.Password = user.Password;
			}
			// Only update member rank for user when member rank option is enable
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				beforeUser.MemberRankId = user.MemberRankId;
			}
			beforeUser.RecommendUid = user.RecommendUid;
			beforeUser.UserMemo = user.UserMemo;
			beforeUser.UserManagementLevelId = user.UserManagementLevelId;
			beforeUser.CareerId = user.CareerId;
			beforeUser.MobileUid = user.MobileUid;
			beforeUser.AdvcodeFirst = user.AdvcodeFirst;
			beforeUser.LastChanged = this.LoginOperatorName; // 最終更新者をセット

			beforeUser.UserExtend = user.UserExtend;

			beforeUser.AccessCountryIsoCode = user.AccessCountryIsoCode;
			beforeUser.DispLanguageCode = user.DispLanguageCode;
			beforeUser.DispLanguageLocaleId = user.DispLanguageLocaleId;
			beforeUser.DispCurrencyCode = user.DispCurrencyCode;
			beforeUser.DispCurrencyLocaleId = user.DispCurrencyLocaleId;
			beforeUser.LastBirthdayPointAddYear = user.LastBirthdayPointAddYear;
			beforeUser.LastBirthdayCouponPublishYear = user.LastBirthdayCouponPublishYear;

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				if ((SessionManager.UpdatedShopCardNoAndPinFlg == false)
					&& (SessionManager.MemberIdForCrossPoint != null)
					&& (SessionManager.PinCodeForCrossPoint != null))
				{
					beforeUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo = SessionManager.MemberIdForCrossPoint;
					beforeUser.UserExtend.UserExtendDataValue.CrossPointShopCardPin = SessionManager.PinCodeForCrossPoint;
				}
			}

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新（更新履歴とともに）
				userService.UpdateWithUserExtend(beforeUser, UpdateHistoryAction.Insert, accessor);

				// 会員ランクを変更したら履歴格納
				if (Constants.MEMBER_RANK_OPTION_ENABLED)
				{
					// 現在のユーザーのランクと更新(予定)ランクを比較
					var afterUser = userService.Get(user.UserId, accessor);
					if (beforMemberRankId != afterUser.MemberRankId)
					{
						// 会員ランク更新履歴へ格納
						MemberRankOptionUtility.InsertUserMemberRankHistory(
							user.UserId,
							// userIdに変更はないので、after,beforeどちらでもいいけど、一応セッションの内容そのまま
							beforMemberRankId,
							afterUser.MemberRankId,
							"",
							this.LoginOperatorId,
							accessor);
					}
				}
				accessor.CommitTransaction();
				//handle GMO
				if ((Constants.PAYMENT_GMO_POST_ENABLED) && (GlobalAddressUtil.IsCountryJp(user.AddrCountryIsoCode)))
				{
					var userBusinessOwnerService = new UserBusinessOwnerService();
					var userBusinessOwnerInput = new UserBusinessOwnerModel();
					if (userInput.BusinessOwner != null)
					{
						userBusinessOwnerInput = userInput.BusinessOwner.CreateModel();
						if (userBusinessOwnerInput.RequestBudget > 0)
						{
							userBusinessOwnerInput.CreditStatus = Constants.FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW;
						}
						userBusinessOwnerInput.ShopCustomerId = string.Format("{0}-F", user.UserId);
					}
					userBusinessOwnerInput.UserId = user.UserId;

					var userBusinessOwner = userBusinessOwnerService.GetByUserId(user.UserId);
					var gmo = new GmoTransactionApi();
					var result = new GmoResponseFrameGuarantee();
					if (userBusinessOwner == null)
					{
						if (userInput.BusinessOwner != null)
						{
							var request = new GmoRequestFrameGuaranteeRegister(user, userBusinessOwnerInput);
							result = gmo.FrameGuaranteeRegister(request);
							if (result.IsResultOk)
							{
								userBusinessOwnerService.Insert(userBusinessOwnerInput);
							}
						}
					}
					else 
					{
						userBusinessOwnerInput.ShopCustomerId = userBusinessOwner.ShopCustomerId;
						var request = new GmoRequestFrameGuaranteeUpdate(user, userBusinessOwnerInput);
						result = gmo.FrameGuaranteeUpdate(request);
						if (result.IsResultOk)
						{
							userBusinessOwnerService.Update(userBusinessOwnerInput);
						}
					}
					if ((result != null) && (result.Errors != null) && (result.Errors.Error != null))
					{
						string messages = string.Empty;
						List<string> errorMessages = new List<string>();
						foreach (var item in result.Errors.Error)
						{
							errorMessages.Add(item.ErrorMessage);
							messages += item.ErrorMessage + "<br/>";
						}
						Session[Constants.SESSION_KEY_ERROR_MSG] = messages;
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
				}
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				// Cooperation Cross Point api
				var result = new CrossPointUserApiService().Update(beforeUser);
				if (result.IsSuccess == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
						w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				if (userInput.UserExtendInput.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
					&& userInput.UserExtendInput.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN))
				{
					var cardNo = beforeUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo;
					var cardPin = beforeUser.UserExtend.UserExtendDataValue.CrossPointShopCardPin;
					if ((originalUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo != cardNo)
						|| (originalUser.UserExtend.UserExtendDataValue.CrossPointShopCardPin != cardPin))
					{
						result = new CrossPointUserApiService().Merge(beforeUser.UserId, cardNo, cardPin);
						if (result.IsSuccess == false)
						{
							Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
								w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
						}
					}
				}
			}

			Response.Redirect(Form_User_UserList.CreateUserDetailUrl(m_strUserId) + "&" + REQUEST_KEY_UPDATE + "=1");
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBuckHistoryBackTop_OnClick(object sender, EventArgs e)
	{
		var uniqueKey = CreateUniqueKeyForSaveUserInput(this.ActionStatus, m_strUserId);
		Session[Constants.SESSION_KEY_PARAM] = null;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK + uniqueKey] = this.UserInput;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK + "_extend" + uniqueKey] = this.UserInput.UserExtendInput;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_REGISTER);
		if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) url.AddParam(Constants.REQUEST_KEY_USER_ID, m_strUserId);
		url.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus);
		url.AddParam(Constants.REQUEST_KEY_UNIQUE_KEY, uniqueKey);
		Response.Redirect(url.CreateUrl());
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Hashtable htSearchParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_USER_SEARCH_INFO];

		if (htSearchParam != null)
		{
			Response.Redirect(Form_User_UserList.CreateUserListUrl(htSearchParam, (int)htSearchParam[Constants.REQUEST_KEY_PAGE_NO]));
		}
		else
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_LIST);
		}
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegist_Click(object sender, EventArgs e)
	{
		// セッション情報チェック
		if (this.UserInput == null)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_LIST);
		}

		var userInput = this.UserInput;
		var user = userInput.CreateModel();
		// ユーザID整合性チェック(ポップアップ同時起動対策)
		if (user.UserId == "")
		{
			user.UserId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			user.LastChanged = this.LoginOperatorName; // 最終更新者をセット

			user.UserExtend = userInput.UserExtendInput.CreateModel();
			user.UserExtend.UserId = user.UserId;
			user.UserExtend.LastChanged = user.LastChanged;

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				var result = new CrossPointUserApiService().Insert(user);
				if (result.IsSuccess == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = result.ErrorCodeList.Contains(
							w2.App.Common.Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
						? result.ErrorMessage
						: MessageManager.GetMessages(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				if (userInput.UserExtendInput.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
					&& userInput.UserExtendInput.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)
					&& (string.IsNullOrEmpty(userInput.UserExtendInput.CrossPointShopCardNo) == false)
					&& (string.IsNullOrEmpty(userInput.UserExtendInput.CrossPointShopCardPin) == false))
				{
					result = new CrossPointUserApiService().Merge(
						user.UserId,
						userInput.UserExtendInput.CrossPointShopCardNo,
						userInput.UserExtendInput.CrossPointShopCardPin);
					if (result.IsSuccess == false)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
							w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}

					var userApiResult = new CrossPointUserApiService().Get(user.UserId);
					if (userApiResult == null)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}

					user.UserExtend.UserExtendDataText.CrossPointShopCardNo = userApiResult.RealShopCardNo;
					user.UserExtend.UserExtendDataText.CrossPointShopCardPin = userApiResult.PinCode;
				}
				else
				{
					var userApiResult = new CrossPointUserApiService().Get(user.UserId);
					if (userApiResult == null)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}

					if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_NO) == false)
					{
						user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_NO);
					}

					if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN) == false)
					{
						user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN);
					}

					if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME) == false)
					{
						user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME);
					}

					user.UserExtend.UserExtendDataValue.CrossPointShopCardNo = userApiResult.RealShopCardNo;
					user.UserExtend.UserExtendDataValue.CrossPointShopCardPin = userApiResult.PinCode;
					user.UserExtend.UserExtendDataValue.CrossPointAddShopName = userApiResult.AdmissionShopName;
				}
			}

			new UserService().InsertWithUserExtend(
				user,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				PublishPointAtRegist(user, UpdateHistoryAction.Insert);
			}

			this.UserInput = null;
			if ((Constants.PAYMENT_GMO_POST_ENABLED) && (GlobalAddressUtil.IsCountryJp(user.AddrCountryIsoCode)))
			{
				var userBusinessOwnerService = new UserBusinessOwnerService();
				if (userInput.BusinessOwner != null)
				{
					var userBusinessOwnerInput = userInput.BusinessOwner.CreateModel();
					userBusinessOwnerInput.ShopCustomerId = string.Format("{0}-F", user.UserId);
					if (userBusinessOwnerInput.RequestBudget > 0)
					{
						userBusinessOwnerInput.CreditStatus = Constants.FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW;
					}
					userBusinessOwnerInput.UserId = user.UserId;
					var request = new GmoRequestFrameGuaranteeRegister(user, userBusinessOwnerInput);
					var gmo = new GmoTransactionApi();
					var result = gmo.FrameGuaranteeRegister(request);
					if (result.IsResultOk)
					{
						userBusinessOwnerService.Insert(userBusinessOwnerInput);
					}
				}
			}
			Response.Redirect(Form_User_UserList.CreateUserDetailUrl(user.UserId) + "&" + REQUEST_KEY_REGIST + "=1");
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// Publish point at regist
	/// </summary>
	/// <param name="user">User model</param>
	/// <param name="updateHistoryAction">Update history action</param>
	/// <returns>Points</returns>
	protected string PublishPointAtRegist(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		var pointRules = PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER);

		var totalGrantedPoint = 0;
		foreach (var pointRule in pointRules)
		{
			totalGrantedPoint += (int)pointRule.IncNum;
			new PointOptionUtility().InsertUserRegisterUserPoint(
				user.UserId,
				pointRule.PointRuleId,
				user.LastChanged,
				updateHistoryAction);

			// Update Cross Point api
			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
					user,
					pointRule.IncNum,
					CrossPointUtility.GetValue(Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID, pointRule.PointIncKbn));
			}
		}

		return totalGrantedPoint.ToString();
	}

	/// <summary>ユーザー情報</summary>
	private UserInput UserInput
	{
		get
		{
			var result = Session[Constants.SESSION_KEY_PARAM_FOR_USER_INPUT + _uniqueKey] as UserInput;
			return result;
		}
		set
		{
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_INPUT + _uniqueKey] = value;
		}
	}
}
