/*
=========================================================================================================
  Module      : シングルサインオン実行（G3001_Funnel用）クラス(G3001_Funnel_SingleSignOnExecuter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.SingleSignOn.Executer
{
	/// <summary>
	/// シングルサインオン実行（G3001_Funnel用）クラス
	/// </summary>
	public class G3001_Funnel_SingleSignOnExecuter : BaseSingleSignOnExecuter
	{
		#region 定数
		/// <summary>mID（click108会員ID）</summary>
		private const string FIELD_USEREXTEND_USEREX_MID = "usrex_mid";
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="context">HTTPコンテンツ</param>
		public G3001_Funnel_SingleSignOnExecuter(
			HttpContext context)
			: base(context)
		{
		}
		#endregion

		#region メソッド
		/// <summary>シングルサインオン</summary>
		protected override SingleSignOnResult OnExecute()
		{
			// リクエスト値取得
			var parameters = new CallBackParameters(this.Context.Request);

			// 検証
			var result = parameters.Validate();
			if (result == false)
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					"パラメータ検証エラー");
			}

			// w2側にユーザーがなければ、ユーザー登録する
			var userService = new UserService();
			var user = userService.GetByExtendColumn(FIELD_USEREXTEND_USEREX_MID, parameters.MID);
			if (user == null)
			{
				// ユーザーモデル作成
				user = CreateUser(parameters);

				// ユーザー登録（ユーザー拡張項目含む）
				userService.InsertWithUserExtend(
					user,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);

				// 登録したユーザーを取得
				user = userService.GetByExtendColumn(FIELD_USEREXTEND_USEREX_MID, parameters.MID);
			}

			// 成功
			return new SingleSignOnResult(
				SingleSignOnDetailTypes.Success,
				user,
				parameters.NextUrl);
		}

		/// <summary>
		/// ユーザーモデル作成
		/// </summary>
		/// <param name="parameters">パラメータ</param>
		/// <returns>ユーザーモデル</returns>
		private UserModel CreateUser(CallBackParameters parameters)
		{
			// ユーザーセット
			var user = new UserModel();
			user.UserId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			user.UserKbn = ((Constants.SMARTPHONE_OPTION_ENABLED) 
				&& (SmartPhoneUtility.CheckSmartPhone(this.Context.Request.UserAgent)))
				? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER
				: Constants.FLG_USER_USER_KBN_PC_USER;
			user.AdvcodeFirst = StringUtility.ToEmpty(this.Context.Session[Constants.SESSION_KEY_ADVCODE_NOW]);
			user.RemoteAddr = this.Context.Request.ServerVariables["REMOTE_ADDR"];
			user.RecommendUid = UserCookieManager.UniqueUserId;
			user.MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank();
			user.DateLastLoggedin = DateTime.Now;
			user.LastChanged = Constants.FLG_LASTCHANGED_USER;

			// ユーザー拡張項目セット
			var cacheController = DataCacheControllerFacade.GetUserExtendSettingCacheController();
			var userExtendSettings = cacheController.GetModifyUserExtendSettingList(false, Constants.FLG_USEREXTENDSETTING_DISPLAY_PC);
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				userExtendSettings = NameTranslationCommon.SetUserExtendSettingTranslationData(
					userExtendSettings,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId);
			}
			// 会員登録時のデフォルト値をセット
			var userExtend = new UserExtendModel(userExtendSettings);
			foreach (var extend in userExtendSettings.Items)
			{
				userExtend.UserExtendColumns.Add(extend.SettingId);
				userExtend.UserExtendDataValue[extend.SettingId] =
					extend.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT
						? extend.ListItemDefaultSelected.ToString()
						: string.Join(",", ((List<ListItem>)(extend.ListItemDefaultSelected)).Where(i => i.Selected).Select(i => i.Value));
			}

			// mIDに値をセット
			if (userExtend.UserExtendSettings.Items.Any(extend => extend.SettingId == FIELD_USEREXTEND_USEREX_MID) == false)
			{
				userExtend.UserExtendColumns.Add(FIELD_USEREXTEND_USEREX_MID);
			}
			userExtend.UserExtendDataValue[FIELD_USEREXTEND_USEREX_MID] = parameters.MID;
			user.UserExtend = userExtend;

			return user;
		}
		#endregion

		#region プロパティ
		#endregion

		#region click108コールバックパラメータクラス
		/// <summary>
		/// click108コールバックパラメータクラス
		/// </summary>
		public class CallBackParameters
		{
			#region 定数
			/// <summary>Status</summary>
			private const string REQUEST_STATUS = "Status";
			/// <summary>nurl</summary>
			private const string REQUEST_NURL = "nurl";
			/// <summary>MID</summary>
			private const string REQUEST_MID = "mID";
			/// <summary>TimeStamp</summary>
			private const string REQUEST_TIMESTAMP = "timeStamp";
			/// <summary>CheckCode</summary>
			private const string REQUEST_CHECK_CODE = "CheckCode";
			#endregion

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="request">リクエスト</param>
			public CallBackParameters(HttpRequest request)
			{
				this.Status = StringUtility.ToEmpty(request[REQUEST_STATUS]);
				this.NextUrl = StringUtility.ToEmpty(request[REQUEST_NURL]);
				this.MID = StringUtility.ToEmpty(request[REQUEST_MID]);
				this.TimeStamp = StringUtility.ToEmpty(request[REQUEST_TIMESTAMP]);
				this.CheckCode = StringUtility.ToEmpty(request[REQUEST_CHECK_CODE]);
			}
			#endregion

			#region メソッド
			/// <summary>
			/// 検証
			/// </summary>
			/// <returns>成功：true、失敗：false</returns>
			public bool Validate()
			{
				// ステータス検証
				if (this.Status != "E000") return false;

#pragma warning disable 618
				// チェックコード検証
				var value = FormsAuthentication.HashPasswordForStoringInConfigFile(
#pragma warning restore 618
					string.Join("^||^", this.NextUrl, this.MID, this.TimeStamp),
					"MD5");
				if (value.ToLower() != this.CheckCode.ToLower()) return false;

				return true;
			}

			/// <summary>
			/// コールバックURL作成（ローカル検証用）
			/// </summary>
			/// <param name="status">Status</param>
			/// <param name="nextUrl">nurl</param>
			/// <param name="mID">mID</param>
			/// <returns>コールバックURL</returns>
			public static string CreateCallBackUrl(string status, string nextUrl, string mID)
			{
				var now = DateTime.UtcNow;
				long timeStamp = (long)(now - new DateTime(1970, 1, 1)).TotalSeconds;
#pragma warning disable 618
				var checkCode = FormsAuthentication.HashPasswordForStoringInConfigFile(
					string.Join("^||^", nextUrl, mID, timeStamp),
					"MD5");
#pragma warning restore 618
				return new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SINGLE_SIGN_ON)
					.AddParam(REQUEST_STATUS, status)
					.AddParam(REQUEST_NURL, nextUrl)
					.AddParam(REQUEST_MID, mID)
					.AddParam(REQUEST_TIMESTAMP, timeStamp.ToString())
					.AddParam(REQUEST_CHECK_CODE, checkCode)
					.CreateUrl();
			}

			#endregion

			#region プロパティ
			/// <summary>Status</summary>
			/// <remarks>E000:success , E001: checkCode error</remarks>
			public string Status { get; private set; }
			/// <summary>nurl</summary>
			/// <remarks>ログイン後遷移先URL</remarks>
			public string NextUrl { get; private set; }
			/// <summary>MID</summary>
			/// <remarks>Member ID generated by Click Server int(8) </remarks>
			public string MID { get; private set; }
			/// <summary>TimeStamp</summary>
			/// <remarks>Time()</remarks>
			public string TimeStamp { get; private set; }
			/// <summary>CheckCode</summary>
			/// <remarks>Md5(loginID +”^||^”+mID +”^||^”+ timeStamp)</remarks>
			public string CheckCode { get; private set; }
			#endregion
		}
		#endregion
	}
}
