<%--
=========================================================================================================
  Module      : ログイン(Login.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Botchan.User.Login" %>
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

namespace Botchan.User
{
	/// <summary>
	/// ログイン
	/// </summary>
	public class Login : BotchanApiBase, IHttpHandler
	{
		/// <summary>
		/// Process Request
		/// </summary>
		/// <param name="context">Context</param>
		public void ProcessRequest(HttpContext context)
		{
			BotChanApiProcess(context, w2.App.Common.Constants.BOTCHAN_API_NAME_LOGIN);
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
			var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(requestContents);
			var response = new LoginApiFacade().CreateResponseByUser(loginRequest, ref errorList, ref memo);
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
			var errorList = BotChanUtility.ValidateRequest(new Hashtable(), apiName);
			return errorList;
		}

		/// <summary>
		/// パラメータバリエーション
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <returns>エラーリスト</returns>
		protected override Validator.ErrorMessageList ParametersValidate(string requestContents)
		{
			var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(requestContents);
			var dicParam = new Hashtable
			{
				{ Constants.REQUEST_KEY_MAIL_ADDRESS, loginRequest.MailAddress ?? "" },
				{ Constants.REQUEST_KEY_AUTH_TEXT, loginRequest.AuthText ?? "" },
				{ Constants.REQUEST_KEY_USER_IP_ADDRESS, loginRequest.UserIpAddress ?? "" },
			};
			var errorList = Validator.Validate(
				Constants.CHECK_KBN_BOTCHAN_LOGIN,
				dicParam,
				Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "",
				"");
			return errorList;
		}

		/// <summary>Is Reusable</summary>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}