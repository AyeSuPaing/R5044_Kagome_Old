/*
=========================================================================================================
  Module      : ユーザマスタ入力クラス(UserInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Global;
using w2.App.Common.Input;
using w2.App.Common.User;
using w2.Domain.User;
using w2.Domain.User.Helper;

/// <summary>
/// ユーザマスタ入力クラス
/// </summary>
[Serializable]
public class UserInput : InputBase<UserModel>
{
	#region 定数
	/// <summary>
	/// ユーザー入力チェック列挙体
	/// </summary>
	public enum EnumUserInputValidationKbn
	{
		UserRegist = 0,
		UserModify = 1,
		MailMagazineRegist = 2,
		MailMagazineCancel = 3,
		PasswordReminderInput = 4,
		UserEasyRegist = 5,
		UserRegistGlobal = 6,
		UserModifyGlobal = 7
	}

	protected const string FIELD_USER_MAIL_ADDR1AND2 = "mail_addr1and2";
	protected const string CHECK_DUPLICATION = "CHECK_DUPLICATION";
	#endregion

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public UserInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	public UserInput(UserModel user)
		: this()
	{
		this.UserId = user.UserId;
		this.UserKbn = user.UserKbn;
		this.MallId = user.MallId;
		this.Name = user.Name;
		this.Name1 = user.Name1;
		this.Name2 = user.Name2;
		this.NameKana = user.NameKana;
		this.NameKana1 = user.NameKana1;
		this.NameKana2 = user.NameKana2;
		this.NickName = user.NickName;
		this.MailAddr = user.MailAddr;
		this.MailAddr2 = user.MailAddr2;
		this.Zip = user.Zip;
		this.Zip1 = user.Zip1;
		this.Zip2 = user.Zip2;
		this.Addr = user.Addr;
		this.Addr1 = user.Addr1;
		this.Addr2 = user.Addr2;
		this.Addr3 = user.Addr3;
		this.Addr4 = user.Addr4;
		this.Addr5 = user.Addr5;
		this.Tel1 = user.Tel1;
		this.Tel1_1 = user.Tel1_1;
		this.Tel1_2 = user.Tel1_2;
		this.Tel1_3 = user.Tel1_3;
		this.Tel2 = user.Tel2;
		this.Tel2_1 = user.Tel2_1;
		this.Tel2_2 = user.Tel2_2;
		this.Tel2_3 = user.Tel2_3;
		this.Tel3 = user.Tel3;
		this.Tel3_1 = user.Tel3_1;
		this.Tel3_2 = user.Tel3_2;
		this.Tel3_3 = user.Tel3_3;
		this.Fax = user.Fax;
		this.Fax_1 = user.Fax_1;
		this.Fax_2 = user.Fax_2;
		this.Fax_3 = user.Fax_3;
		this.Sex = user.Sex;
		this.Birth = (user.Birth != null) ? user.Birth.ToString() : null;
		this.BirthYear = user.BirthYear;
		this.BirthMonth = user.BirthMonth;
		this.BirthDay = user.BirthDay;
		this.CompanyName = user.CompanyName;
		this.CompanyPostName = user.CompanyPostName;
		this.CompanyExectiveName = user.CompanyExectiveName;
		this.AdvcodeFirst = user.AdvcodeFirst;
		this.Attribute1 = user.Attribute1;
		this.Attribute2 = user.Attribute2;
		this.Attribute3 = user.Attribute3;
		this.Attribute4 = user.Attribute4;
		this.Attribute5 = user.Attribute5;
		this.Attribute6 = user.Attribute6;
		this.Attribute7 = user.Attribute7;
		this.Attribute8 = user.Attribute8;
		this.Attribute9 = user.Attribute9;
		this.Attribute10 = user.Attribute10;
		this.LoginId = user.LoginId;
		this.Password = user.PasswordDecrypted;
		this.Question = user.Question;
		this.Answer = user.Answer;
		this.CareerId = user.CareerId;
		this.MobileUid = user.MobileUid;
		this.RemoteAddr = user.RemoteAddr;
		this.MailFlg = user.MailFlg;
		this.UserMemo = user.UserMemo;
		this.DelFlg = user.DelFlg;
		this.LastChanged = user.LastChanged;
		this.MemberRankId = user.MemberRankId;
		this.RecommendUid = user.RecommendUid;
		this.DateLastLoggedin = (user.DateLastLoggedin != null) ? user.DateLastLoggedin.ToString() : null;
		this.UserManagementLevelId = user.UserManagementLevelId;
		this.IntegratedFlg = user.IntegratedFlg;
		this.EasyRegisterFlg = user.EasyRegisterFlg;
		this.AddrCountryIsoCode = user.AddrCountryIsoCode;
		this.AddrCountryName = user.AddrCountryName;
		this.Addr5 = user.Addr5;
		this.BusinessOwner = new UserBusinessOwnerInput();

	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override UserModel CreateModel()
	{
		var user = new UserModel
		{
			UserId = this.UserId,
			UserKbn = this.UserKbn,
			MallId = this.MallId,
			Name = this.Name,
			Name1 = this.Name1,
			Name2 = this.Name2,
			NameKana = this.NameKana,
			NameKana1 = this.NameKana1,
			NameKana2 = this.NameKana2,
			NickName = this.NickName,
			MailAddr = this.MailAddr,
			MailAddr2 = this.MailAddr2,
			Zip = this.Zip,
			Zip1 = this.Zip1,
			Zip2 = this.Zip2,
			Addr = this.Addr,
			Addr1 = this.Addr1,
			Addr2 = this.Addr2,
			Addr3 = this.Addr3,
			Addr4 = this.Addr4,
			Tel1 = this.Tel1,
			Tel1_1 = this.Tel1_1,
			Tel1_2 = this.Tel1_2,
			Tel1_3 = this.Tel1_3,
			Tel2 = this.Tel2,
			Tel2_1 = this.Tel2_1,
			Tel2_2 = this.Tel2_2,
			Tel2_3 = this.Tel2_3,
			Tel3 = this.Tel3,
			Tel3_1 = this.Tel3_1,
			Tel3_2 = this.Tel3_2,
			Tel3_3 = this.Tel3_3,
			Fax = this.Fax,
			Fax_1 = this.Fax_1,
			Fax_2 = this.Fax_2,
			Fax_3 = this.Fax_3,
			Sex = this.Sex,
			Birth = (this.Birth != null) ? DateTime.Parse(this.Birth) : (DateTime?)null,
			BirthYear = this.BirthYear,
			BirthMonth = this.BirthMonth,
			BirthDay = this.BirthDay,
			CompanyName = this.CompanyName,
			CompanyPostName = this.CompanyPostName,
			CompanyExectiveName = this.CompanyExectiveName,
			AdvcodeFirst = this.AdvcodeFirst,
			Attribute1 = this.Attribute1,
			Attribute2 = this.Attribute2,
			Attribute3 = this.Attribute3,
			Attribute4 = this.Attribute4,
			Attribute5 = this.Attribute5,
			Attribute6 = this.Attribute6,
			Attribute7 = this.Attribute7,
			Attribute8 = this.Attribute8,
			Attribute9 = this.Attribute9,
			Attribute10 = this.Attribute10,
			LoginId = this.LoginId,
			PasswordDecrypted = this.Password,
			Question = this.Question,
			Answer = this.Answer,
			CareerId = this.CareerId,
			MobileUid = this.MobileUid,
			RemoteAddr = this.RemoteAddr,
			MailFlg = this.MailFlg,
			UserMemo = this.UserMemo,
			DelFlg = this.DelFlg,
			MemberRankId = this.MemberRankId,
			RecommendUid = this.RecommendUid,
			DateLastLoggedin = (this.DateLastLoggedin != null) ? DateTime.Parse(this.DateLastLoggedin) : (DateTime?)null,
			UserManagementLevelId = this.UserManagementLevelId,
			LastChanged = this.LastChanged,
			IntegratedFlg = this.IntegratedFlg,
			EasyRegisterFlg = this.EasyRegisterFlg,
			AddrCountryIsoCode = this.AddrCountryIsoCode,
			AddrCountryName = this.AddrCountryName,
			Addr5 = this.Addr5,
			UserExtend = (this.UserExtendInput == null) ? null : this.UserExtendInput.CreateModel(),
		};

		return user;
	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <param name="validateKbn">入力チェック区分</param>
	/// <param name="excludeList">チェック除外項目</param>
	/// <returns>成功:空/失敗:エラーメッセージ</returns>
	public Dictionary<string, string> Validate(EnumUserInputValidationKbn validateKbn, List<string> excludeList = null)
	{
		var input = (Hashtable)this.DataSource.Clone();
		Dictionary<string, string> errorMessages = new Dictionary<string, string>();

		// かんたん会員登録、会員登録・更新・メルマガ登録の場合、電話番号（予備）チェック追加
		if ((validateKbn == EnumUserInputValidationKbn.UserEasyRegist)
			|| (validateKbn == EnumUserInputValidationKbn.UserRegist)
			|| (validateKbn == EnumUserInputValidationKbn.UserModify)
			|| (validateKbn == EnumUserInputValidationKbn.MailMagazineRegist))
		{
			if ((StringUtility.ToEmpty(this.Tel2_1).Length != 0)
				|| (StringUtility.ToEmpty(this.Tel2_2).Length != 0)
				|| (StringUtility.ToEmpty(this.Tel2_3).Length != 0))
			{
				input.Add(Constants.FIELD_USER_TEL2_1 + "_for_check", this.Tel2_1);
				input.Add(Constants.FIELD_USER_TEL2_2 + "_for_check", this.Tel2_2);
				input.Add(Constants.FIELD_USER_TEL2_3 + "_for_check", this.Tel2_3);
			}
		}

		// 入力チェック
		switch (validateKbn)
		{
			case EnumUserInputValidationKbn.UserEasyRegist:
				input.Add("login_id_use_mailaddress_flg", Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "1" : "0");
				input.Add(Constants.FIELD_USER_MAIL_ADDR + "_for_check", this.MailAddrForCheck);
				input.Add(Constants.FIELD_USER_MAIL_ADDR + Constants.FIELD_COMMON_CONF, this.MailAddrConf);
				input.Add(Constants.FIELD_USER_MAIL_ADDR2 + Constants.FIELD_COMMON_CONF, this.MailAddr2Conf);
				input.Add(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF, this.PasswordConf);
				input[Constants.FIELD_USER_TEL1] = this.Tel1;
				input[Constants.FIELD_USER_TEL2] = this.Tel2;
				input[Constants.FIELD_USER_ZIP1] = this.Zip1;
				input[Constants.FIELD_USER_ZIP2] = this.Zip2;

				if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false)
				{
					// メールアドレスのログインID利用無し時のログインID入力チェック用
					input.Add(Constants.FIELD_USER_LOGIN_ID + "_input_check", this.LoginId);
				}

				// 日本以外のユーザーの場合は、グローバル用の入力チェックを行う
				if (this.IsAddrJp == false)
				{
					input.Remove(Constants.FIELD_USER_NAME1);
					input.Remove(Constants.FIELD_USER_NAME2);
					input.Add(Constants.FIELD_USER_NAME1 + "_global", this.Name1);
					input.Add(Constants.FIELD_USER_NAME2 + "_global", this.Name2);
				}
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					input.Add((Constants.FIELD_USER_ADDR_COUNTRY_NAME + "_for_check"), this.AddrCountryName);
				}
				foreach (var item in excludeList)
				{
					input.Remove(item);
				}
				// かんたん会員登録時に国が非表示の場合を考慮して、住所国ISOコードがnullの場合は空変換する
				errorMessages = Validator.ValidateAndGetErrorContainer("UserEasyRegist", input, StringUtility.ToEmpty(this.AddrCountryIsoCode));
				break;

			case EnumUserInputValidationKbn.UserRegist:
			case EnumUserInputValidationKbn.UserRegistGlobal:
				input.Add("login_id_use_mailaddress_flg", Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "1" : "0");
				input.Add(Constants.FIELD_USER_MAIL_ADDR + "_for_check", this.MailAddrForCheck);
				input.Add(Constants.FIELD_USER_MAIL_ADDR + Constants.FIELD_COMMON_CONF, this.MailAddrConf);
				input.Add(Constants.FIELD_USER_MAIL_ADDR2 + Constants.FIELD_COMMON_CONF, this.MailAddr2Conf);
				input.Add(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF, this.PasswordConf);
				input[Constants.FIELD_USER_TEL1] = this.Tel1;
				input[Constants.FIELD_USER_TEL2] = this.Tel2;
				input[Constants.FIELD_USER_ZIP] = this.Zip;

				if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false)
				{
					// メールアドレスのログインID利用無し時のログインID入力チェック用
					input.Add(Constants.FIELD_USER_LOGIN_ID + "_input_check", this.LoginId);
				}
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					input.Add((Constants.FIELD_USER_ADDR_COUNTRY_NAME + "_for_check"), this.AddrCountryName);
				}
				foreach (var item in excludeList)
				{
					input.Remove(item);
				}
				errorMessages = (validateKbn == EnumUserInputValidationKbn.UserRegist)
					? Validator.ValidateAndGetErrorContainer("UserRegist", input, this.AddrCountryIsoCode)
					: Validator.ValidateAndGetErrorContainer("UserRegistGlobal", input, this.AddrCountryIsoCode);
				break;

			case EnumUserInputValidationKbn.UserModify:
			case EnumUserInputValidationKbn.UserModifyGlobal:
				input.Add("login_id_use_mailaddress_flg", Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "1" : "0");
				input.Add(Constants.FIELD_USER_MAIL_ADDR + "_for_check", this.MailAddrForCheck);
				input.Add(Constants.FIELD_USER_MAIL_ADDR + Constants.FIELD_COMMON_CONF, this.MailAddrConf);
				input.Add(Constants.FIELD_USER_MAIL_ADDR2 + "_for_check", this.MailAddr2ForCheck);
				input.Add(Constants.FIELD_USER_MAIL_ADDR2 + Constants.FIELD_COMMON_CONF, this.MailAddr2Conf);
				input[Constants.FIELD_USER_TEL1] = this.Tel1;
				input[Constants.FIELD_USER_TEL2] = this.Tel2;
				input[Constants.FIELD_USER_ZIP] = this.Zip;

				if (StringUtility.ToEmpty(input[Constants.FIELD_USER_PASSWORD]) == string.Empty)
				{
					input[Constants.FIELD_USER_PASSWORD] = null;
				}
				if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false)
				{
					// メールアドレスのログインID利用無し時のログインID入力チェック用
					input.Add(Constants.FIELD_USER_LOGIN_ID + "_input_check", this.LoginId);
				}
				input.Add(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF, this.PasswordConf);
				input.Add(Constants.FIELD_USER_PASSWORD + "_before", this.PasswordBefore);
				input.Add(Constants.FIELD_USER_PASSWORD + "_ncsry", this.PasswordNecessary);
				input.Add(Constants.FIELD_USER_PASSWORD + "_conf_ncsry", this.PasswordConfNecessary);
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					input.Add((Constants.FIELD_USER_ADDR_COUNTRY_NAME + "_for_check"), this.AddrCountryName);
				}

				if (excludeList != null)
				{
					foreach (var item in excludeList)
					{
						input.Remove(item);
					}
				}

				errorMessages = (validateKbn == EnumUserInputValidationKbn.UserModify)
					? Validator.ValidateAndGetErrorContainer("UserModify", input, this.AddrCountryIsoCode)
					: Validator.ValidateAndGetErrorContainer("UserModifyGlobal", input, this.AddrCountryIsoCode);
				break;

			case EnumUserInputValidationKbn.MailMagazineRegist:
				input.Add(Constants.FIELD_USER_MAIL_ADDR + Constants.FIELD_COMMON_CONF, this.MailAddrConf);

				// グローバルOP：ONの場合は、ふりがな項目を必須チェックしない
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					input.Remove(Constants.FIELD_USER_NAME_KANA1);
					input.Remove(Constants.FIELD_USER_NAME_KANA2);
					input.Add(Constants.FIELD_USER_NAME_KANA1 + "_global", this.NameKana1);
					input.Add(Constants.FIELD_USER_NAME_KANA2 + "_global", this.NameKana2);
				}

				errorMessages = Validator.ValidateAndGetErrorContainer("MailMagazineRegist", input, this.AddrCountryIsoCode);
				break;

			case EnumUserInputValidationKbn.MailMagazineCancel:
				errorMessages = Validator.ValidateAndGetErrorContainer("MailMagazineCancel", input, this.AddrCountryIsoCode);
				break;

			case EnumUserInputValidationKbn.PasswordReminderInput:
				if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false)
				{
					// メールアドレスのログインID利用無し時のログインID入力チェック用
					input.Add(Constants.FIELD_USER_LOGIN_ID + "_for_input_check", this.LoginId);
				}
				errorMessages = Validator.ValidateAndGetErrorContainer("PasswordReminderInput", input, this.AddrCountryIsoCode);
				break;

			default:
				break;
		}

		// 編集の場合、変更前パスワードでのログインチェック
		// パスワード変更の場合は入力チェック対象とする
		if ((validateKbn == EnumUserInputValidationKbn.UserModify)
			&& (LoginCheckAtBeforePassword(errorMessages) == false))
		{
			errorMessages.Add(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_BEFORE, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PASSWORD_BEFORE_CHECK));
		}

		if ((validateKbn == EnumUserInputValidationKbn.UserEasyRegist)
			|| (validateKbn == EnumUserInputValidationKbn.UserRegist)
			|| (validateKbn == EnumUserInputValidationKbn.UserModify)
			|| (validateKbn == EnumUserInputValidationKbn.UserRegistGlobal)
			|| (validateKbn == EnumUserInputValidationKbn.UserModifyGlobal))
		{
			// DB重複チェック
			CheckDuplicationLoginId(errorMessages, validateKbn);

			// Check authentication code
			if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED
				&& (this.HasAuthenticationCode == false))
			{
				errorMessages.Add(
					Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_VERIFICATION_CODE_IS_INCORRECT));
			}

			// CROSSPOINT入力チェック
			// オプションがONかつ、入力チェック区分が登録か更新のとき
			if (Constants.CROSS_POINT_OPTION_ENABLED
				&& (errorMessages.ContainsKey(Constants.FIELD_USER_LOGIN_ID) == false))
			{
				CheckCrossPointCardAndMailAddress(errorMessages, validateKbn);
			}
		}
		// エラーメッセージがなければ成功
		return errorMessages;
	}

	/// <summary>
	///  DB重複チェック
	/// </summary>
	/// <param name="validateKbn">重複チェック区分</param>
	/// <returns>成功:空/失敗:エラーメッセージ</returns>
	public Dictionary<string, string> ValidateDuplication(EnumUserInputValidationKbn validateKbn)
	{
		Dictionary<string, string> errorMessages = new Dictionary<string, string>();
		CheckDuplicationLoginId(errorMessages, validateKbn);
		return errorMessages;
	}

	/// <summary>
	/// ログインID重複チェック
	/// </summary>
	/// <param name="errorMessages">エラーメッセージ</param>
	/// <param name="validateKbn">入力チェック区分</param>
	public void CheckDuplicationLoginId(Dictionary<string, string> errorMessages, EnumUserInputValidationKbn validateKbn)
	{
		var errorMessage = CheckDuplicationLoginId(validateKbn);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			errorMessages.Add(Constants.FIELD_USER_LOGIN_ID, errorMessage);
		}
	}
	/// <summary>
	/// ログインID重複チェック
	/// </summary>
	/// <param name="validateKbn">入力チェック区分</param>
	public string CheckDuplicationLoginId(EnumUserInputValidationKbn validateKbn)
	{
		var userService = new UserService();
		string mailAddressEnabled = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "1" : "0";
		var replaceTagName = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
			? "@@User.mail_addr.name@@"
			: "@@User.login_id.name@@";

		switch (validateKbn)
		{
			case EnumUserInputValidationKbn.UserEasyRegist:
			case EnumUserInputValidationKbn.UserRegist:
			case EnumUserInputValidationKbn.UserRegistGlobal:
				if (userService.CheckDuplicationLoginId(this.LoginId, mailAddressEnabled)) return "";
				break;
			case EnumUserInputValidationKbn.UserModify:
			case EnumUserInputValidationKbn.UserModifyGlobal:
				if (userService.CheckDuplicationLoginId(this.LoginId, mailAddressEnabled, this.UserId)) return "";
				break;
			default:
				break;
		}
		var errorMessage = Validator.GetErrorMessage(CHECK_DUPLICATION, Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(replaceTagName, this.AddrCountryIsoCode));
		return errorMessage;
	}

	/// <summary>
	///  変更前パスワードでログインできるか確認を行う
	/// </summary>
	/// <param name="errorMessages">エラーメッセージ</param>
	/// <returns>成功:true/失敗:false</returns>
	private bool LoginCheckAtBeforePassword(Dictionary<string, string> errorMessages)
	{
		if (this.PasswordBefore == null) return true;

		if (((StringUtility.ToEmpty(this.PasswordBefore) != "")
				|| (StringUtility.ToEmpty(this.Password) != "")
				|| (StringUtility.ToEmpty(this.PasswordConf) != ""))
			&& (errorMessages.ContainsKey(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_BEFORE) == false))
		{
			var userService = new UserService();
			var user = userService.Get(this.UserId);
			var loggedUser = userService.TryLogin(user.LoginId, this.PasswordBefore, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
			return (loggedUser != null);
		}

		// 必要なけれればtrue
		return true;
	}

	/// <summary>
	/// CROSSPOINT入力チェック
	/// </summary>
	/// <param name="errorMessages">エラーメッセージ</param>
	/// <param name="validateKbn">入力チェック区分</param>
	/// <returns>エラーメッセージ</returns>
	public Dictionary<string, string> CheckCrossPointCardAndMailAddress(
		Dictionary<string, string> errorMessages,
		EnumUserInputValidationKbn validateKbn)
	{
		// 入力チェック区分が登録か更新かつ、会員カード番号とPINコードの入力欄があるか
		if (((validateKbn == EnumUserInputValidationKbn.UserRegist)
			|| (validateKbn == EnumUserInputValidationKbn.UserModify))
			&& (this.UserExtendInput.UserExtendDataText
				.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
			&& this.UserExtendInput.UserExtendDataText
				.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)))
		{
			// 会員カード番号・PIN入力チェック
			var shopCardNoAndPinCodeErrorMessages = new UserUtility().CheckShopCardNoAndPinCode(
				this.UserExtendInput.CrossPointShopCardNo,
				this.UserExtendInput.CrossPointShopCardPin,
				this.UserId,
				(validateKbn == EnumUserInputValidationKbn.UserRegist));
			if (string.IsNullOrEmpty(shopCardNoAndPinCodeErrorMessages) == false)
			{
				errorMessages.Add(
					Constants.CROSS_POINT_USREX_SHOP_CARD_NO,
					WebMessages.GetMessages(shopCardNoAndPinCodeErrorMessages));
				return errorMessages;
			}
			// PC/モバイルメールアドレス重複チェック
			if (this.MailAddr == this.MailAddr2)
			{
				errorMessages.Add(
					Constants.FIELD_USER_MAIL_ADDR,
					WebMessages.GetMessages(CommerceMessages.ERRMSG_CROSS_POINT_DUPLICATE_PC_MOBILE_MAILADDRESS));
			}
			// メールアドレス重複チェック
			var errorMessageDuplicationMailAddress = new UserUtility().CheckCrossPointDuplicationMailAddress(
				this.UserExtendInput.CrossPointShopCardNo,
				this.UserExtendInput.CrossPointShopCardPin,
				this.MailAddr,
				this.UserId,
				(validateKbn == EnumUserInputValidationKbn.UserRegist));
			if ((errorMessages.ContainsKey(Constants.FIELD_USER_MAIL_ADDR) == false)
				&& (string.IsNullOrEmpty(errorMessageDuplicationMailAddress) == false))
			{
				errorMessages.Add(
					Constants.FIELD_USER_MAIL_ADDR,
					WebMessages.GetMessages(errorMessageDuplicationMailAddress));
			}
		}
		return errorMessages;
	}

	/// <summary>
	/// 住所項目結合
	/// </summary>
	/// <returns>結合した住所</returns>
	public string ConcatenateAddress()
	{
		string address = string.Empty;
		if (this.IsAddrJp)
		{
			address = ConcatenateAddressWithoutCountryName() + this.AddrCountryName;
		}
		else
		{
			address = this.Addr2
				+ ((string.IsNullOrEmpty(this.Addr3) == false) ? " " : "")
				+ this.Addr3
				+ " "
				+ this.Addr4
				+ ((string.IsNullOrEmpty(this.Addr5) == false) ? " " : "")
				+ this.Addr5
				+ " "
				+ this.AddrCountryName;
		}
		return address;
	}

	/// <summary>
	/// 住所項目結合（国名は含めない）
	/// </summary>
	/// <returns>結合した住所</returns>
	public string ConcatenateAddressWithoutCountryName()
	{
		var address = AddressHelper.ConcatenateAddressWithoutCountryName(
			this.Addr1,
			this.Addr2,
			this.Addr3,
			this.Addr4);

		return address;
	}
	#endregion

	#region プロパテ
	/// <summary>ビジネスオーナーの入力データ</summary>
	public UserBusinessOwnerInput BusinessOwner { get; set; }
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_USER_ID]; }
		set { this.DataSource[Constants.FIELD_USER_USER_ID] = value; }
	}
	/// <summary>顧客区分</summary>
	public string UserKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_USER_KBN]; }
		set { this.DataSource[Constants.FIELD_USER_USER_KBN] = value; }
	}
	/// <summary>モールID</summary>
	public string MallId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MALL_ID]; }
		set { this.DataSource[Constants.FIELD_USER_MALL_ID] = value; }
	}
	/// <summary>氏名</summary>
	public string Name
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_NAME] = value; }
	}
	/// <summary>氏名1</summary>
	public string Name1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME1]; }
		set { this.DataSource[Constants.FIELD_USER_NAME1] = value; }
	}
	/// <summary>氏名2</summary>
	public string Name2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME2]; }
		set { this.DataSource[Constants.FIELD_USER_NAME2] = value; }
	}
	/// <summary>氏名かな</summary>
	public string NameKana
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA]; }
		set { this.DataSource[Constants.FIELD_USER_NAME_KANA] = value; }
	}
	/// <summary>氏名かな1</summary>
	public string NameKana1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA1]; }
		set { this.DataSource[Constants.FIELD_USER_NAME_KANA1] = value; }
	}
	/// <summary>氏名かな2</summary>
	public string NameKana2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA2]; }
		set { this.DataSource[Constants.FIELD_USER_NAME_KANA2] = value; }
	}
	/// <summary>ニックネーム</summary>
	public string NickName
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_NICK_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_NICK_NAME] = value; }
	}
	/// <summary>メールアドレス</summary>
	public string MailAddr
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR]; }
		set { this.DataSource[Constants.FIELD_USER_MAIL_ADDR] = value; }
	}
	/// <summary>メールアドレス確認用</summary>
	public string MailAddrConf { get; set; }
	/// <summary>メールアドレス2</summary>
	public string MailAddr2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR2]; }
		set { this.DataSource[Constants.FIELD_USER_MAIL_ADDR2] = value; }
	}
	/// <summary>メールアドレス2確認用</summary>
	public string MailAddr2Conf { get; set; }
	/// <summary>郵便番号</summary>
	public string Zip
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ZIP]; }
		set { this.DataSource[Constants.FIELD_USER_ZIP] = value; }
	}
	/// <summary>郵便番号1</summary>
	public string Zip1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ZIP1]; }
		set { this.DataSource[Constants.FIELD_USER_ZIP1] = value; }
	}
	/// <summary>郵便番号2</summary>
	public string Zip2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ZIP2]; }
		set { this.DataSource[Constants.FIELD_USER_ZIP2] = value; }
	}
	/// <summary>住所</summary>
	public string Addr
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR] = value; }
	}
	/// <summary>住所1</summary>
	public string Addr1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR1]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR1] = value; }
	}
	/// <summary>住所2</summary>
	public string Addr2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR2]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR2] = value; }
	}
	/// <summary>住所3</summary>
	public string Addr3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR3]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR3] = value; }
	}
	/// <summary>住所4</summary>
	public string Addr4
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR4]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR4] = value; }
	}
	/// <summary>電話番号1</summary>
	public string Tel1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL1]; }
		set { this.DataSource[Constants.FIELD_USER_TEL1] = value; }
	}
	/// <summary>電話番号1-1</summary>
	public string Tel1_1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL1_1]; }
		set { this.DataSource[Constants.FIELD_USER_TEL1_1] = value; }
	}
	/// <summary>電話番号1-2</summary>
	public string Tel1_2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL1_2]; }
		set { this.DataSource[Constants.FIELD_USER_TEL1_2] = value; }
	}
	/// <summary>電話番号1-3</summary>
	public string Tel1_3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL1_3]; }
		set { this.DataSource[Constants.FIELD_USER_TEL1_3] = value; }
	}
	/// <summary>電話番号2</summary>
	public string Tel2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL2]; }
		set { this.DataSource[Constants.FIELD_USER_TEL2] = value; }
	}
	/// <summary>電話番号2-1</summary>
	public string Tel2_1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL2_1]; }
		set { this.DataSource[Constants.FIELD_USER_TEL2_1] = value; }
	}
	/// <summary>電話番号2-2</summary>
	public string Tel2_2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL2_2]; }
		set { this.DataSource[Constants.FIELD_USER_TEL2_2] = value; }
	}
	/// <summary>電話番号2-3</summary>
	public string Tel2_3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL2_3]; }
		set { this.DataSource[Constants.FIELD_USER_TEL2_3] = value; }
	}
	/// <summary>電話番号3</summary>
	public string Tel3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL3]; }
		set { this.DataSource[Constants.FIELD_USER_TEL3] = value; }
	}
	/// <summary>電話番号3-1</summary>
	public string Tel3_1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL3_1]; }
		set { this.DataSource[Constants.FIELD_USER_TEL3_1] = value; }
	}
	/// <summary>電話番号3-2</summary>
	public string Tel3_2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL3_2]; }
		set { this.DataSource[Constants.FIELD_USER_TEL3_2] = value; }
	}
	/// <summary>電話番号3-3</summary>
	public string Tel3_3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_TEL3_3]; }
		set { this.DataSource[Constants.FIELD_USER_TEL3_3] = value; }
	}
	/// <summary>ＦＡＸ</summary>
	public string Fax
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_FAX]; }
		set { this.DataSource[Constants.FIELD_USER_FAX] = value; }
	}
	/// <summary>ＦＡＸ1</summary>
	public string Fax_1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_FAX_1]; }
		set { this.DataSource[Constants.FIELD_USER_FAX_1] = value; }
	}
	/// <summary>ＦＡＸ2</summary>
	public string Fax_2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_FAX_2]; }
		set { this.DataSource[Constants.FIELD_USER_FAX_2] = value; }
	}
	/// <summary>ＦＡＸ3</summary>
	public string Fax_3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_FAX_3]; }
		set { this.DataSource[Constants.FIELD_USER_FAX_3] = value; }
	}
	/// <summary>性別</summary>
	public string Sex
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_SEX]; }
		set { this.DataSource[Constants.FIELD_USER_SEX] = value; }
	}
	/// <summary>生年月日</summary>
	public string Birth
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BIRTH]; }
		set { this.DataSource[Constants.FIELD_USER_BIRTH] = value; }
	}
	/// <summary>生年月日（年）</summary>
	public string BirthYear
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BIRTH_YEAR]; }
		set { this.DataSource[Constants.FIELD_USER_BIRTH_YEAR] = value; }
	}
	/// <summary>生年月日（月）</summary>
	public string BirthMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BIRTH_MONTH]; }
		set { this.DataSource[Constants.FIELD_USER_BIRTH_MONTH] = value; }
	}
	/// <summary>生年月日（日）</summary>
	public string BirthDay
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_BIRTH_DAY]; }
		set { this.DataSource[Constants.FIELD_USER_BIRTH_DAY] = value; }
	}
	/// <summary>企業名</summary>
	public string CompanyName
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_COMPANY_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_COMPANY_NAME] = value; }
	}
	/// <summary>部署名</summary>
	public string CompanyPostName
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_COMPANY_POST_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_COMPANY_POST_NAME] = value; }
	}
	/// <summary>役職名</summary>
	public string CompanyExectiveName
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_COMPANY_EXECTIVE_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_COMPANY_EXECTIVE_NAME] = value; }
	}
	/// <summary>初回広告コード</summary>
	public string AdvcodeFirst
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADVCODE_FIRST]; }
		set { this.DataSource[Constants.FIELD_USER_ADVCODE_FIRST] = value; }
	}
	/// <summary>属性1</summary>
	public string Attribute1
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE1]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE1] = value; }
	}
	/// <summary>属性2</summary>
	public string Attribute2
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE2]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE2] = value; }
	}
	/// <summary>属性3</summary>
	public string Attribute3
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE3]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE3] = value; }
	}
	/// <summary>属性4</summary>
	public string Attribute4
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE4]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE4] = value; }
	}
	/// <summary>属性5</summary>
	public string Attribute5
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE5]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE5] = value; }
	}
	/// <summary>属性6</summary>
	public string Attribute6
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE6]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE6] = value; }
	}
	/// <summary>属性7</summary>
	public string Attribute7
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE7]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE7] = value; }
	}
	/// <summary>属性8</summary>
	public string Attribute8
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE8]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE8] = value; }
	}
	/// <summary>属性9</summary>
	public string Attribute9
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE9]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE9] = value; }
	}
	/// <summary>属性10</summary>
	public string Attribute10
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE10]; }
		set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE10] = value; }
	}
	/// <summary>ログインＩＤ</summary>
	public string LoginId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_LOGIN_ID]; }
		set { this.DataSource[Constants.FIELD_USER_LOGIN_ID] = value; }
	}
	/// <summary>パスワード</summary>
	public string Password
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_PASSWORD]; }
		set { this.DataSource[Constants.FIELD_USER_PASSWORD] = value; }
	}

	/// <summary>質問</summary>
	public string Question
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_QUESTION]; }
		set { this.DataSource[Constants.FIELD_USER_QUESTION] = value; }
	}
	/// <summary>回答</summary>
	public string Answer
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ANSWER]; }
		set { this.DataSource[Constants.FIELD_USER_ANSWER] = value; }
	}
	/// <summary>キャリアID</summary>
	public string CareerId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_CAREER_ID]; }
		set { this.DataSource[Constants.FIELD_USER_CAREER_ID] = value; }
	}
	/// <summary>モバイルUID</summary>
	public string MobileUid
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MOBILE_UID]; }
		set { this.DataSource[Constants.FIELD_USER_MOBILE_UID] = value; }
	}
	/// <summary>リモートIPアドレス</summary>
	public string RemoteAddr
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_REMOTE_ADDR]; }
		set { this.DataSource[Constants.FIELD_USER_REMOTE_ADDR] = value; }
	}
	/// <summary>メール配信フラグ</summary>
	public string MailFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_FLG]; }
		set { this.DataSource[Constants.FIELD_USER_MAIL_FLG] = value; }
	}
	/// <summary>ユーザメモ</summary>
	public string UserMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_USER_MEMO]; }
		set { this.DataSource[Constants.FIELD_USER_USER_MEMO] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_USER_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_USER_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USER_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USER_LAST_CHANGED] = value; }
	}
	/// <summary>会員ランクID</summary>
	public string MemberRankId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_MEMBER_RANK_ID]; }
		set { this.DataSource[Constants.FIELD_USER_MEMBER_RANK_ID] = value; }
	}
	/// <summary>外部レコメンド連携用ユーザID</summary>
	public string RecommendUid
	{
		get
		{
			if (this.DataSource[Constants.FIELD_USER_RECOMMEND_UID] == DBNull.Value) return string.Empty;
			return (string)this.DataSource[Constants.FIELD_USER_RECOMMEND_UID];
		}
		set { this.DataSource[Constants.FIELD_USER_RECOMMEND_UID] = value; }
	}
	/// <summary>最終ログイン日時</summary>
	public string DateLastLoggedin
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN]; }
		set { this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN] = value; }
	}
	/// <summary>ユーザー管理レベルID</summary>
	public string UserManagementLevelId
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID]; }
		set { this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID] = value; }
	}
	/// <summary>ユーザー統合フラグ</summary>
	public string IntegratedFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_INTEGRATED_FLG]; }
		set { this.DataSource[Constants.FIELD_USER_INTEGRATED_FLG] = value; }
	}
	/// <summary>かんたん会員フラグ</summary>
	public string EasyRegisterFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_EASY_REGISTER_FLG]; }
		set { this.DataSource[Constants.FIELD_USER_EASY_REGISTER_FLG] = value; }
	}
	/// <summary>ユーザ拡張項目</summary>
	public UserExtendInput UserExtendInput { get; set; }
	/// <summary>ユーザ拡張項目設定一覧</summary>
	public UserExtendSettingList UserExtendSettingList { get; set; }
	/// <summary>性別名</summary>
	public string SexValueText
	{
		get { return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, this.Sex); }
	}
	/// <summary>メール配信フラグ</summary>
	public string MailFlgValueText
	{
		get { return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, this.MailFlg); }
	}
	/// <summary>住所国ISOコード</summary>
	public string AddrCountryIsoCode
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE] = value; }
	}
	/// <summary>住所国名</summary>
	public string AddrCountryName
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_NAME]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_NAME] = value; }
	}
	/// <summary>住所5</summary>
	public string Addr5
	{
		get { return (string)this.DataSource[Constants.FIELD_USER_ADDR5]; }
		set { this.DataSource[Constants.FIELD_USER_ADDR5] = value; }
	}
	/// <summary>PCメールアドレス（必須チェック用）</summary>
	public string MailAddrForCheck { get; set; }
	/// <summary>メールアドレス2検証用</summary>
	public string MailAddr2ForCheck { get; set; }
	/// <summary>パスワード確認用</summary>
	public string PasswordConf { get; set; }
	/// <summary>パスワード変更前</summary>
	public string PasswordBefore { get; set; }
	/// <summary>ログインID検証用</summary>
	public string LoginIdMailCheck { get; set; }
	/// <summary>パスワード必須チェック用</summary>
	public string PasswordNecessary { get; set; }
	/// <summary>パスワード確認必須チェック用</summary>
	public string PasswordConfNecessary { get; set; }
	/// <summary>日本住所のユーザーか</summary>
	private bool IsAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.AddrCountryIsoCode); }
	}
	/// <summary>Authentication code</summary>
	public string AuthenticationCode { get; set; }
	/// <summary>Has authentication code</summary>
	public bool HasAuthenticationCode { get; set; }
	#endregion
}