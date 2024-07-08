<%--
=========================================================================================================
  Module      : 会員登録(Register.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Botchan.User.Register" %>
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using BotchanApi;
using w2.App.Common;
using w2.App.Common.Botchan;
using w2.App.Common.Global.Region;
using w2.App.Common.User.Botchan;
using w2.Common.Util;
using w2.Domain.User;

namespace Botchan.User
{
	/// <summary>
	/// 会員登録
	/// </summary>
	public class Register : BotchanApiBase, IHttpHandler
	{
		/// <summary>
		/// プロセスリクエスト
		/// </summary>
		/// <param name="context">Context</param>
		public void ProcessRequest(HttpContext context)
		{
			BotChanApiProcess(context, w2.App.Common.Constants.BOTCHAN_API_NAME_REGISTER);
		}

		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="context">httpコンテキスト</param>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモ</param>
		/// <returns>レスポンス</returns>
		protected override object GetResponseData(
			HttpContext context,
			string requestContents,
			ref List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(requestContents);
			var response = new RegisterApiFacade().CreateResponseByRegisterUser(registerRequest, ref errorList, ref memo);
			return response;
		}

		/// <summary>
		/// BOTCHAN共通チェック
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="apiName">API名</param>
		/// <returns>エラーリスト</returns>
		protected override List<BotchanMessageManager.MessagesCode> BotChanUtilityValidate(string requestContents, string apiName)
		{
			var productSearchRequest = JsonConvert.DeserializeObject<RegisterRequest>(requestContents);
			var validate = new Hashtable { { "AuthText", productSearchRequest.AuthText } };
			var errorList = BotChanUtility.ValidateRequest(validate, apiName);
			return errorList;
		}

		/// <summary>
		/// パラメータバリエーション
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <returns>エラーリスト</returns>
		protected override Validator.ErrorMessageList ParametersValidate(string requestContents)
		{
			var request = JsonConvert.DeserializeObject<RegisterRequest>(requestContents);
			var mailAddr = request.UserRegisterObject.MailAddr ?? "";
			var result = new Hashtable
			{
				{ Constants.REQUEST_KEY_USER_REGISTER_NAME1, request.UserRegisterObject.Name1 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_NAME2, request.UserRegisterObject.Name2 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_NAME_KANA1, request.UserRegisterObject.NameKana1 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_NAME_KANA2, request.UserRegisterObject.NameKana2 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_USER_KBN, request.UserRegisterObject.UserKbn ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_BIRTH, request.UserRegisterObject.Birth ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_SEX, request.UserRegisterObject.Sex ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_MAIL_ADDR, mailAddr },
				{ Constants.REQUEST_KEY_USER_REGISTER_ZIP, request.UserRegisterObject.Zip ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_ADDR1, request.UserRegisterObject.Addr1 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_ADDR2, request.UserRegisterObject.Addr2 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_ADDR3, request.UserRegisterObject.Addr3 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_ADDR4, request.UserRegisterObject.Addr4 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_TEL1, request.UserRegisterObject.Tel1 ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_PASSWORD, request.UserRegisterObject.Password ?? "" },
				{ Constants.REQUEST_KEY_USER_REGISTER_MAIL_FLG, request.UserRegisterObject.MailFlg ?? "" },
				{ Constants.REQUEST_KEY_AUTH_TEXT, request.AuthText ?? "" },
				{ Constants.REQUEST_KEY_USER_IP_ADDRESS, request.UserIpAddress ?? "" },
			};

			var errorList = Validator.Validate(
				Constants.CHECK_KBN_BOTCHAN_USER_REGISTER,
				result,
				Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "",
				"");

			var mailAddressEnabled = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "1" : "0";
			if ((string.IsNullOrEmpty(mailAddr) == false) 
				&& (new UserService().CheckDuplicationLoginId(mailAddr, mailAddressEnabled) == false))
			{
				var errorMessage = Validator.GetErrorMessage(
					"CHECK_DUPLICATION",
					Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_addr.name@@"));
				errorList.Add(Constants.REQUEST_KEY_USER_REGISTER_MAIL_ADDR, errorMessage);
			}		
	
			return errorList;
		}

		/// <summary>Is Reusable</summary>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}