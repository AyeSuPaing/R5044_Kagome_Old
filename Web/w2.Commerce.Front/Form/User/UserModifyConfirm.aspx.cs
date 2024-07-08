/*
=========================================================================================================
  Module      : 会員登録変更確認画面処理(UserModifyConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.CrossPoint;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Global;
using w2.App.Common.SendMail;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Form_User_UserModifyConfirm : UserPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ログインチェック（ログイン後は顧客変更入力ページから）
		//------------------------------------------------------
		CheckLoggedIn(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_MODIFY_INPUT);

		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// URLセッションチェック
		//------------------------------------------------------
		CheckUrlSessionForUserRegistModify();
	}

	/// <summary>
	/// 更新するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbModity_Click(object sender, EventArgs e)
	{
		// まずDB重複チェック
		Dictionary<string, string> errorMessages = UserInput.ValidateDuplication(UserInput.EnumUserInputValidationKbn.UserModify);

		if (errorMessages.Count == 0)
		{
			var user = this.UserInput.CreateModel();
			user.UserExtend = this.UserInput.UserExtendInput.CreateModel();

			var userService = new UserService();
			var originalUser = userService.Get(user.UserId);
			var updateUser = (UserModel)originalUser.Clone();
			updateUser.UserExtend = new UserExtendModel(
				(Hashtable)originalUser.UserExtend.DataSource.Clone(),
				originalUser.UserExtend.UserExtendSettings);

			updateUser.Name = user.Name;
			updateUser.Name1 = user.Name1;
			updateUser.Name2 = user.Name2;
			updateUser.NameKana = user.NameKana;
			updateUser.NameKana1 = user.NameKana1;
			updateUser.NameKana2 = user.NameKana2;
			updateUser.NickName = user.NickName;
			updateUser.Birth = user.Birth;
			updateUser.BirthYear = user.BirthYear;
			updateUser.BirthMonth = user.BirthMonth;
			updateUser.BirthDay = user.BirthDay;
			updateUser.Sex = user.Sex;
			updateUser.MailAddr = user.MailAddr;
			updateUser.MailAddr2 = user.MailAddr2;
			updateUser.Zip = user.Zip;
			updateUser.Zip1 = user.Zip1;
			updateUser.Zip2 = user.Zip2;
			updateUser.AddrCountryIsoCode = user.AddrCountryIsoCode;
			updateUser.AddrCountryName = user.AddrCountryName;
			updateUser.Addr = user.Addr;
			updateUser.Addr1 = user.Addr1;
			updateUser.Addr2 = user.Addr2;
			updateUser.Addr3 = user.Addr3;
			updateUser.Addr4 = user.Addr4;
			updateUser.Addr5 = user.Addr5;
			updateUser.CompanyName = user.CompanyName;
			updateUser.CompanyPostName = user.CompanyPostName;
			updateUser.Tel1 = user.Tel1;
			updateUser.Tel1_1 = user.Tel1_1;
			updateUser.Tel1_2 = user.Tel1_2;
			updateUser.Tel1_3 = user.Tel1_3;
			updateUser.Tel2 = user.Tel2;
			updateUser.Tel2_1 = user.Tel2_1;
			updateUser.Tel2_2 = user.Tel2_2;
			updateUser.Tel2_3 = user.Tel2_3;
			updateUser.MailFlg = user.MailFlg;
			updateUser.EasyRegisterFlg = user.EasyRegisterFlg;
			updateUser.RemoteAddr = user.RemoteAddr;
			updateUser.LoginId = user.LoginId;
			// パスワード変更する場合
			if (user.Password != string.Empty)
			{
				updateUser.Password = user.Password;
			}
			updateUser.LastChanged = Constants.FLG_LASTCHANGED_USER;
			// PC以外の拡張項目が欠損するとエラーになるため、ぐるぐる回して入力値のモデルと一致するKeyの拡張項目を設定する
			foreach (var extend in user.UserExtend.UserExtendSettings.Items.Where(x => (x.InitOnlyFlg == Constants.FLG_USEREXTENDSETTING_UPDATABLE)))
			{
				if (updateUser.UserExtend.UserExtendSettings.Items.Any(x => (x.SettingId == extend.SettingId))
					&& user.UserExtend.UserExtendDataValue.ContainsKey(extend.SettingId))
				{
					updateUser.UserExtend.UserExtendDataValue[extend.SettingId] = user.UserExtend.UserExtendDataValue[extend.SettingId];
				}
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				if ((SessionManager.UpdatedShopCardNoAndPinFlg == false)
					&& (SessionManager.MemberIdForCrossPoint != null)
					&& (SessionManager.PinCodeForCrossPoint != null))
				{
					updateUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo = SessionManager.MemberIdForCrossPoint;
					updateUser.UserExtend.UserExtendDataValue.CrossPointShopCardPin = SessionManager.PinCodeForCrossPoint;
				}

				// Cooperation Cross Point api
				var result = new CrossPointUserApiService().Update(updateUser);
				if (result.IsSuccess == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
						w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				var cardNo = updateUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo;
				var cardPin = updateUser.UserExtend.UserExtendDataValue.CrossPointShopCardPin;
				if (((string.IsNullOrEmpty(cardNo) == false)
						&& ((string.IsNullOrEmpty(cardPin) == false)))
					&& ((originalUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo != cardNo)
						|| (originalUser.UserExtend.UserExtendDataValue.CrossPointShopCardPin != cardPin)))
				{
					result = new CrossPointUserApiService().Merge(updateUser.UserId, cardNo, cardPin);
					if (result.IsSuccess == false)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
							w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}
				}
			}

			// 履歴とともに更新
			userService.UpdateWithUserExtend(
				updateUser,
				UpdateHistoryAction.DoNotInsert);

			if (Constants.USER_COOPERATION_ENABLED)
			{
				// ユーザー更新イベント実行
				var userServiceWrapper = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
				userServiceWrapper.Update(updateUser);
			}

			// 更新履歴登録
			new UpdateHistoryService().InsertForUser(updateUser.UserId, updateUser.LastChanged);

			// セッション情報を書き換え（再ログイン処理は行わない）
			this.LoginUser = updateUser;
			this.LoginUserName = user.ComplementUserName;
			this.LoginUserNickName = updateUser.NickName;
			this.LoginUserMail = updateUser.MailAddr;
			this.LoginUserMail2 = updateUser.MailAddr2;
			this.LoginUserBirth = DateTimeUtility.ToStringFromRegion(updateUser.Birth, DateTimeUtility.FormatType.ShortDate2Letter);
			this.LoginUserEasyRegisterFlg = updateUser.EasyRegisterFlg;

			// カートのユーザ情報を更新するためカートオーナーが存在すれば削除する
			GetCartObjectList().SetOwner(null);

			// ログインIDのSecureCookie作成に関する処理
			UserCookieManager.CreateCookieForLoginId(this.UserInput.LoginId, (UserCookieManager.GetLoginIdFromCookie() != ""));

			// メール送信処理
			SendMailCommon.SendModifyUserMail(updateUser.UserId);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_MODIFY_COMPLETE);
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		var user = this.UserInput.CreateModel();
		user.UserExtend = this.UserInput.UserExtendInput.CreateModel();

		if ((user.UserExtend != null)
			&& (user.UserExtend.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO))
			&& (user.UserExtend.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)
			&& (string.IsNullOrEmpty(user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_NO]) == false)
			&& (string.IsNullOrEmpty(user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN]) == false)))
		{
			SessionManager.IsBackForCrossPoint = true;
		}

			// ターゲットページ設定
		this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_MODIFY_INPUT;

		// 会員更新ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_MODIFY_INPUT);
	}

	/// <summary>ユーザー編集入力情報</summary>
	protected UserInput UserInput { get { return (UserInput)Session[Constants.SESSION_KEY_PARAM]; } }
	/// <summary>PCサイトユーザーか判定（利用メールアドレス判定用）</summary>
	protected bool IsPcSiteOrOfflineUser { get { return UserService.IsPcSiteOrOfflineUser(this.UserInput.UserKbn); } }
	/// <summary> 表示するユーザ拡張項目があるか</summary>
	protected bool ExistsUserExtend
	{
		get { return (this.UserInput.UserExtendSettingList.Items.Count > 0); }
	}
	/// <summary>ユーザーの住所が日本か</summary>
	public bool IsUserAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所国ISOコード</summary>
	public string UserAddrCountryIsoCode
	{
		get { return this.UserInput.AddrCountryIsoCode; }
	}
}