/*
=========================================================================================================
  Module      : SMSヘルパクラス(SMSHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Diagnostics;
using System.Linq;
using w2.App.Common.Global.Config;
using w2.App.Common.ShopMessage;
using w2.Common.Logger;
using w2.Common.Net.SMS;
using w2.Common.Util;
using w2.Domain.GlobalSMS;
using w2.Domain.TempDatas;
using w2.Domain.User;

namespace w2.App.Common.SMS
{
	/// <summary>
	/// SMSヘルパクラス
	/// </summary>
	public static class SMSHelper
	{
		/// <summary>SMSキャリアID：既定のキャリア</summary>
		public const string SMS_PHONE_CARRIER_ID_DEFAULT = "default";

		#region +GetSMSPhoneCarrier SMSキャリア取得
		/// <summary>
		/// SMSキャリア取得
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <returns>SMSキャリアID</returns>
		public static string GetSMSPhoneCarrier(string userId)
		{
			// 現在は既定のキャリアのみ
			return SMS_PHONE_CARRIER_ID_DEFAULT;
		}
		#endregion

		#region +SendSms SMS送信
		/// <summary>
		/// SMS送信
		/// </summary>
		/// <param name="userId">送信対象のユーザーID</param>
		/// <param name="smsText">送信するテキスト</param>
		public static void SendSms(string userId, string smsText)
		{
			// オプションが無効である場合はSMS送信しない
			if (Constants.GLOBAL_SMS_OPTION_ENABLED == false) { return; }

			try
			{
				var manager = new SMSManager();
				var to = GetSmsToPhoneNumber(userId);

				// 送り先の電話番号（国コード付き）が特定できた場合にSMS送信
				if (string.IsNullOrEmpty(to))
				{
					return;
				}
				// エラーポイントが閾値超えている場合はダメ
				if (CheckSmsErrorPoint(to) == false)
				{
					var msg = string.Format(
						"エラーポイントが閾値を超えているためSMS送信見送り。\r\nユーザーID：{0}\r\nTo：{1}\r\n本文：{2}",
						userId,
						to,
						smsText);
					FileLogger.Write("sms", msg);
					return;
				}

				var res = manager.SendMessage(smsText, to, Constants.GLOBAL_SMS_FROM);

				if (res.IsSucccess)
				{
					// SMSステートに依頼済みとして書き込み
					var state = new GlobalSMSStatusModel();
					state.GlobalTelNo = to;
					state.MessageId = res.SuccessMessageID;
					state.LastChanged = "api_result";
					state.SmsStatus = GlobalSMSStatusModel.SMS_STATUS_REQUEST;
					var sv = new GlobalSMSService();
					sv.RegisterState(state);
				}
				else
				{
					var msg = string.Format(
						"SMS送信に失敗。\r\nユーザーID：{0}\r\nTo：{1}\r\n本文：{2}\r\n返却内容：{3}",
						userId,
						to,
						smsText,
						res.ResultMessage);
					FileLogger.Write("sms", msg);
				}

			}
			catch (Exception ex)
			{
				// ログ書くだけにする
				var msg = string.Format(
					"SMS送信に失敗。\r\nユーザーID：{0}\r\n本文：{1}\r\nエラー内容：{2}",
					userId,
					smsText,
					ex);
				FileLogger.Write("sms", msg);
			}
		}
		#endregion

		#region +CheckSmsErrorPoint SMSエラーポイントのチェック
		/// <summary>
		/// SMSエラーポイントのチェック
		/// </summary>
		/// <param name="phoneNumber">対象電話番号</param>
		/// <returns>
		/// TRUE：閾値超えていない。OK。閾値丁度もOKとして取り扱う。
		/// FALSE：閾値超えた。NG
		/// </returns>
		public static bool CheckSmsErrorPoint(string phoneNumber)
		{
			var sv = new GlobalSMSService();
			var model = sv.GetErrorPoint(phoneNumber);

			if (model == null) { return true; }

			// 閾値未満であればOK
			if (model.ErrorPoint < Constants.GLOBAL_SMS_STOP_ERROR_POINT) { return true; }

			// 上記以外であればエラーポイント超過とする
			return false;
		}
		#endregion

		#region +GetSmsToPhoneNumber SMS用の電話番号取得
		/// <summary>
		/// SMS用の電話番号取得
		/// </summary>
		/// <param name="userId">対象ユーザー</param>
		/// <returns>SMS用の電話番号</returns>
		public static string GetSmsToPhoneNumber(string userId)
		{
			// ユーザー情報取得
			var sv = new UserService();
			var user = sv.Get(userId);
			var code = GlobalConfigs.GetInstance().GlobalSettings.InternationalTelephoneCode
				.FirstOrDefault(x => x.Iso == user.AddrCountryIsoCode);

			if (code == null)
			{
				var msg = string.Format(
					"該当ユーザーの電話番号国コードが特定できません。\r\nユーザーID：{0}\r\nISO：{1}\r\n電話番号：{2}",
					user.UserId,
					user.AddrCountryIsoCode,
					user.Tel1);
				FileLogger.Write("sms", msg);
				return "";
			}

			var rtn = GetSmsToPhoneNumber(user.Tel1, user.AddrCountryIsoCode);
			return rtn;
		}
		/// <summary>
		/// SMS用の電話番号取得
		/// </summary>
		/// <param name="telNo">電話番号</param>
		/// <param name="countryIsoCode">ISOコード</param>
		/// <returns>SMS用の電話番号</returns>
		public static string GetSmsToPhoneNumber(string telNo, string countryIsoCode)
		{
			if (string.IsNullOrEmpty(telNo))
			{
				var msg = "SMS送信用の電話番号が空です。";
				FileLogger.Write("sms", msg);
				return "";
			}

			var code = GlobalConfigs.GetInstance().GlobalSettings.InternationalTelephoneCode
				.FirstOrDefault(x => x.Iso == countryIsoCode);

			if (code == null)
			{
				var msg = string.Format(
					"ISOコードに該当する電話番号国コードが特定できません。\r\nISO：{0}\r\n電話番号：{1}",
					countryIsoCode,
					telNo);
				FileLogger.Write("sms", msg);
				return "";
			}

			// 番号を組み立て
			var to = code.Number;
			if (telNo.StartsWith("0"))
			{
				// 先頭のゼロを抜かして電話番号国コードの後ろにつけたス
				to += string.Join("", telNo.Skip(1));
			}
			else
			{
				to += telNo;
			}

			// ハイフン除去
			return to.Replace("-", "");
		}
		#endregion

		#region +SendSmsAuthenticationCode
		/// <summary>
		/// Send sms authentication code
		/// </summary>
		/// <param name="authenticationCode">Authentication code</param>
		/// <param name="phoneNumber">Phone number</param>
		/// <returns>True: if send sms success, otherwise: False</returns>
		public static bool SendSmsAuthenticationCode(string authenticationCode, string phoneNumber)
		{
			Debug.WriteLine("SMS personal identity verification code: " + authenticationCode);
			var smsTemplate = new GlobalSMSService().GetSmsTemplate(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_SEND_SMS_AUTHENTICATION,
				SMSHelper.GetSMSPhoneCarrier(string.Empty));

			var smsText = smsTemplate.SmsText
				.Replace("@@ auth_code @@", authenticationCode)
				.Replace("@@ ShopMessage.ShopName @@", ShopMessageUtil.GetMessage("ShopName"));

			var resultSendSms = new SMSManager().SendMessage(smsText, phoneNumber, Constants.GLOBAL_SMS_FROM);
			return resultSendSms.IsSucccess;
		}
		#endregion

		#region +CheckBeforeSendAuthenticationCode
		/// <summary>
		/// Check before send authentication code
		/// </summary>
		/// <param name="tempKey">Temp key</param>
		/// <param name="address">Address</param>
		/// <param name="sendFailCount">Send fail count</param>
		/// <param name="authCodeSendCount">Auth code send count</param>
		/// <returns>Error message</returns>
		public static string CheckBeforeSendAuthenticationCode(
			string tempKey,
			string address,
			ref int sendFailCount,
			ref int authCodeSendCount)
		{
			// Check if user request send sms many times
			sendFailCount = GetTempDataAuthenticationCode(
				TempDatasService.TempType.AuthCodeTrySendTel,
				tempKey);

			if (sendFailCount > Constants.PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_TRY_COUNT)
			{
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_TEL_NO_LOCKED_FAIL_AUTH_CODE_MANY_TIMES);
			}

			// Check if user entered fail authentication code many times
			var codeTryIpAddressCount = GetTempDataAuthenticationCode(
				TempDatasService.TempType.AuthCodeTryIpAddress,
				address,
				Constants.PERSONAL_AUTHENTICATION_AUTH_CODE_TRY_LOCK_MINUTES);

			if (codeTryIpAddressCount > Constants.PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_TRY_COUNT)
			{
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE_MANY_TIMES);
			}

			// Check if ip address request send sms many times
			authCodeSendCount = GetTempDataAuthenticationCode(
				TempDatasService.TempType.AuthCodeSendIpAddress,
				address,
				Constants.PERSONAL_AUTHENTICATION_AUTH_CODE_SEND_LOCK_MINUTES);

			if (authCodeSendCount > Constants.PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_SEND_COUNT)
			{
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_REQUEST_TOO_MANY_AUTH);
			}

			return string.Empty;
		}
		#endregion

		#region +CheckAuthenticationCode
		/// <summary>
		/// Check authentication code
		/// </summary>
		/// <param name="authenticationCode">Authentication code</param>
		/// <param name="tempKey">Temp key</param>
		/// <param name="userHostAddress">User host address</param>
		/// <param name="errorMessage">Error message</param>
		/// <returns>Check authentication code result</returns>
		public static string CheckAuthenticationCode(
			string authenticationCode,
			string tempKey,
			string userHostAddress,
			ref string errorMessage)
		{
			var timeStart = DateTime.Now;
			var tempDatasService = new TempDatasService();

			// Check if user entered fail auth code many times
			var codeTryIpAddressCount = GetTempDataAuthenticationCode(
				TempDatasService.TempType.AuthCodeTryIpAddress,
				userHostAddress,
				Constants.PERSONAL_AUTHENTICATION_AUTH_CODE_TRY_LOCK_MINUTES);

			if (codeTryIpAddressCount > Constants.PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_TRY_COUNT)
			{
				errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE_MANY_TIMES);
				return Constants.FLG_AUTHENTICATION_RESULT_STOP_PROCESS;
			}

			// Check user entered authentication code is incorrect
			var authenticationData = tempDatasService.Resotre(
				TempDatasService.TempType.AuthCode,
				tempKey);

			if ((authenticationData == null)
				|| (authenticationCode.Trim() != StringUtility.ToEmpty(authenticationData.TempDataDeserialized)))
			{
				tempDatasService.Save(
					TempDatasService.TempType.AuthCodeTryIpAddress,
					userHostAddress,
					(codeTryIpAddressCount + 1));

				// If user entered authentication code is incorrect and fail many times
				if (codeTryIpAddressCount >= Constants.PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_TRY_COUNT)
				{
					errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE_MANY_TIMES);
					return Constants.FLG_AUTHENTICATION_RESULT_STOP_PROCESS;
				}

				errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE);
				return Constants.FLG_AUTHENTICATION_RESULT_VERIFICATION_CODE_IS_INCORRECT;
			}

			// Check verification code has expired
			if (authenticationData.DateCreated.AddSeconds(Constants.PERSONAL_AUTHENTICATION_OF_USERR_EGISTRATION_AUTH_CODE_EXPIRATION_TIME) < timeStart)
			{
				errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_CODE_HAS_EXPIRED);
				return Constants.FLG_AUTHENTICATION_RESULT_VERIFICATION_CODE_HAS_EXPIRED;
			}

			errorMessage = string.Empty;
			return Constants.FLG_AUTHENTICATION_RESULT_SUCCSESS;
		}
		#endregion

		/// <summary>
		/// Get temp data deserialized authentication code
		/// </summary>
		/// <param name="tempType">Temp type</param>
		/// <param name="tempKey">Temp key</param>
		/// <param name="minutes">Minutes</param>
		/// <returns>Temp data deserialized</returns>
		private static int GetTempDataAuthenticationCode(
			TempDatasService.TempType tempType,
			string tempKey,
			int? minutes = null)
		{
			var tempData = new TempDatasService().Resotre(
				tempType,
				tempKey,
				minutes);

			var result = (tempData != null)
				? (int)tempData.TempDataDeserialized
				: 1;

			return result;
		}
	}
}
