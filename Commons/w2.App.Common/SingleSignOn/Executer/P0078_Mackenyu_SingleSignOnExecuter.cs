/*
=========================================================================================================
  Module      : シングルサインオン実行（P0078_Mackenyu用）クラス(P0078_Mackenyu_SingleSignOnExecuter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.MemberRank;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.SingleSignOn.Executer
{
	/// <summary>
	/// シングルサインオン実行（P0078_Mackenyu用）クラス
	/// </summary>
	public class P0078_Mackenyu_SingleSignOnExecuter : BaseSingleSignOnExecuter
	{
		#region 定数
		/// <summary>mID（MackenyuFCサイト会員ID）</summary>
		private const string FIELD_USEREXTEND_USEREX_MID = "usrex_mid";
		/// <summary>MackenyuFCサイト用会員ランクID</summary>
		private const string MACKENYU_MEMBER_RANK_ID = "FamilyMember";
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="context">HTTPコンテンツ</param>
		public P0078_Mackenyu_SingleSignOnExecuter(HttpContext context)
			: base(context)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// シングルサインオン実行
		/// </summary>
		/// <returns>シングルサインオンリザルト</returns>
		protected override SingleSignOnResult OnExecute()
		{
			// リクエスト値取得
			var parameters = new CallBackParameters(this.Context.Request);

			// FamilyMemberメンバーランク存在チェック
			var memberRank = MemberRankService.Get(MACKENYU_MEMBER_RANK_ID);
			if (memberRank == null)
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CONTACT_WITH_OPERATOR));
			}

			// 検証
			var isSuccessValidate = parameters.Validate();
			if (isSuccessValidate == false)
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null);
			}

			var userService = new UserService();

			// 連携済みユーザーでログイン
			var cooperatedUser = userService.GetRegisteredUserByExtendColumn(FIELD_USEREXTEND_USEREX_MID, parameters.MId);
			if (cooperatedUser != null)
			{
				return new SingleSignOnResult(SingleSignOnDetailTypes.Success, cooperatedUser, parameters.NextUrl);
			}

			var user = userService.GetUserByMailAddr(parameters.MailAddr);
			var userExtend = (user != null)
				? userService.GetUserExtend(user.UserId)
				: null;
			var hasUserExtend = (userExtend != null);

			// 異なるFCユーザーと連携済みの場合エラー
			if (hasUserExtend
				&& (userExtend.UserExtendDataValue[FIELD_USEREXTEND_USEREX_MID] != parameters.MId)
				&& (string.IsNullOrEmpty(userExtend.UserExtendDataValue[FIELD_USEREXTEND_USEREX_MID]) == false))
			{
				return new SingleSignOnResult(
					SingleSignOnDetailTypes.Failure,
					null,
					"",
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_COOPERATION_WITH_DIFFERENT_FC_USER));
			}

			// 対象のメールアドレスを持つユーザーが未連携なら連携する
			if (hasUserExtend)
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					userExtend.UserExtendDataValue[FIELD_USEREXTEND_USEREX_MID] = parameters.MId;
					userService.Modify(
						user.UserId,
						userModel => { userModel.MemberRankId = MACKENYU_MEMBER_RANK_ID; },
						UpdateHistoryAction.Insert,
						accessor);

					userService.UpdateUserExtend(
						userExtend,
						user.UserId,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// 会員ランク更新履歴へ格納
					MemberRankOptionUtility.InsertUserMemberRankHistory(
						user.UserId,
						user.MemberRankId,
						MACKENYU_MEMBER_RANK_ID,
						"",
						user.UserId,
						accessor);

					accessor.CommitTransaction();
				}
			}
			// 対象のメールアドレスを持つユーザーが存在しない場合は新規登録して連携する
			else
			{
				userService.InsertWithUserExtend(
					CreateUser(parameters),
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
			}

			// 連携したユーザーでログイン
			var newUser = userService.GetRegisteredUserByExtendColumn(FIELD_USEREXTEND_USEREX_MID, parameters.MId);
			return new SingleSignOnResult(
				SingleSignOnDetailTypes.Success,
				newUser,
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
			var user = new UserModel
			{
				UserId = UserService.CreateNewUserId(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.NUMBER_KEY_USER_ID,
					Constants.CONST_USER_ID_HEADER,
					Constants.CONST_USER_ID_LENGTH),
				UserKbn = ((Constants.SMARTPHONE_OPTION_ENABLED)
					&& (SmartPhoneUtility.CheckSmartPhone(this.Context.Request.UserAgent)))
					? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER
					: Constants.FLG_USER_USER_KBN_PC_USER,
				AdvcodeFirst = StringUtility.ToEmpty(this.Context.Session[Constants.SESSION_KEY_ADVCODE_NOW]),
				RemoteAddr = this.Context.Request.ServerVariables["REMOTE_ADDR"],
				RecommendUid = UserCookieManager.UniqueUserId,
				MemberRankId = MACKENYU_MEMBER_RANK_ID,
				DateLastLoggedin = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_USER
			};

			// 誕生日
			user.BirthYear = BirthYearValidate(parameters.BirthYear);
			user.BirthMonth = BirthMonthAndDayValidate(parameters.BirthMonth);
			user.BirthDay = BirthMonthAndDayValidate(parameters.BirthDay);
			if ((string.IsNullOrEmpty(user.BirthYear) == false)
				&& (string.IsNullOrEmpty(user.BirthMonth) == false)
				&& (string.IsNullOrEmpty(user.BirthDay) == false))
			{
				user.Birth = DateTime.Parse(
					String.Format("{0}/{1}/{2}", user.BirthYear, user.BirthMonth, user.BirthDay));
			}

			// 性別
			switch (parameters.Sex)
			{
				case "1":
					user.Sex = Constants.FLG_USER_SEX_MALE;
					break;

				case "2":
					user.Sex = Constants.FLG_USER_SEX_FEMALE;
					break;

				default:
					user.Sex = Constants.FLG_USER_SEX_UNKNOWN;
					break;
			}

			// メアド
			user.MailAddr = parameters.MailAddr;

			// 電話番号
			if ((string.IsNullOrEmpty(parameters.Tel1) == false)
				&& (string.IsNullOrEmpty(parameters.Tel2) == false)
				&& (string.IsNullOrEmpty(parameters.Tel3) == false))
			{
				user.Tel1_1 = parameters.Tel1;
				user.Tel1_2 = parameters.Tel2;
				user.Tel1_3 = parameters.Tel3;
				user.Tel1 = String.Format("{0}-{1}-{2}", user.Tel1_1, user.Tel1_2, user.Tel1_3);
			}

			user.UserExtend = CreateUserExtend(parameters.MId);

			return user;
		}

		/// <summary>
		/// ユーザー拡張モデル作成
		/// </summary>
		/// <param name="mId">メンバID</param>
		/// <returns>ユーザー拡張モデル</returns>
		private UserExtendModel CreateUserExtend(string mId)
		{
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
						: string.Join(",", ((List<ListItem>)(extend.ListItemDefaultSelected))
							.Where(i => i.Selected).Select(i => i.Value));
			}

			// mIdに値をセット
			if (userExtend.UserExtendSettings.Items.Any(extend => extend.SettingId == FIELD_USEREXTEND_USEREX_MID) == false)
			{
				userExtend.UserExtendColumns.Add(FIELD_USEREXTEND_USEREX_MID);
			}
			userExtend.UserExtendDataValue[FIELD_USEREXTEND_USEREX_MID] = mId;

			return userExtend;
		}

		/// <summary>
		/// 誕生年のバリデーション
		/// </summary>
		/// <param name="birthYear">誕生年</param>
		/// <returns>true: 誕生年 || false ""</returns>
		private string BirthYearValidate(string birthYear)
		{
			if (birthYear.Length != 4) return "";
			int outBirthYear;
			return int.TryParse(birthYear, out outBirthYear) ? outBirthYear.ToString() : "";
		}

		/// <summary>
		/// 誕生(月、日)のバリデーション
		/// </summary>
		/// <param name="birthMonthOrDay">誕生月 || 誕生日</param>
		/// <returns>true: 誕生月(日) || false ""</returns>
		private string BirthMonthAndDayValidate(string birthMonthOrDay)
		{
			if ((birthMonthOrDay.Length != 2) && (birthMonthOrDay.Length != 1)) return "";
			var birthdayThatIsNotZeroStart = birthMonthOrDay.StartsWith("0") ? birthMonthOrDay.Substring(1) : birthMonthOrDay;
			int outBirth;
			return int.TryParse(birthdayThatIsNotZeroStart, out outBirth) ? outBirth.ToString() : "";
		}
		#endregion

		#region MackenyuFCサイト コールバックパラメータクラス
		/// <summary>
		/// MackenyuFCサイト コールバックパラメータクラス
		/// </summary>
		public class CallBackParameters
		{
			#region 定数
			/// <summary>遷移先URL</summary>
			private const string REQUEST_NURL = "nurl";
			/// <summary>メンバID</summary>
			private const string REQUEST_MID = "mid";
			/// <summary>ログイン時のタイムスタンプ</summary>
			private const string REQUEST_TIMESTAMP = "timestamp";
			/// <summary>チェックコード</summary>
			private const string REQUEST_CHECK_CODE = "checkcode";
			/// <summary>ニックネーム</summary>
			private const string REQUEST_NICKNAME = "nickname";
			/// <summary>誕生年</summary>
			private const string REQUEST_BYEAR = "byear";
			/// <summary>誕生月</summary>
			private const string REQUEST_BMONTH = "bmonth";
			/// <summary>誕生日</summary>
			private const string REQUEST_BDAY = "bday";
			/// <summary>性別</summary>
			private const string REQUEST_SEX = "sex";
			/// <summary>都道府県</summary>
			private const string REQUEST_PREF = "pref";
			/// <summary>メールアドレス</summary>
			private const string REQUEST_MAIL_ADDR = "mailaddr";
			/// <summary>電話番号1</summary>
			private const string REQUEST_TEL1 = "tel1";
			/// <summary>電話番号2</summary>
			private const string REQUEST_TEL2 = "tel2";
			/// <summary>電話番号3</summary>
			private const string REQUEST_TEL3 = "tel3";
			#endregion

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="request">リクエスト</param>
			public CallBackParameters(HttpRequest request)
			{
				this.NextUrl = StringUtility.ToEmpty(request[REQUEST_NURL]);
				this.MId = StringUtility.ToEmpty(request[REQUEST_MID]);
				this.TimeStamp = StringUtility.ToEmpty(request[REQUEST_TIMESTAMP]);
				this.CheckCode = StringUtility.ToEmpty(request[REQUEST_CHECK_CODE]);
				this.Nickname = StringUtility.ToEmpty(request[REQUEST_NICKNAME]);
				this.BirthYear = StringUtility.ToEmpty(request[REQUEST_BYEAR]);
				this.BirthMonth = StringUtility.ToEmpty(request[REQUEST_BMONTH]);
				this.BirthDay = StringUtility.ToEmpty(request[REQUEST_BDAY]);
				this.Sex = StringUtility.ToEmpty(request[REQUEST_SEX]);
				this.Pref = StringUtility.ToEmpty(request[REQUEST_PREF]);
				this.MailAddr = StringUtility.ToEmpty(request[REQUEST_MAIL_ADDR]);
				this.Tel1 = StringUtility.ToEmpty(request[REQUEST_TEL1]);
				this.Tel2 = StringUtility.ToEmpty(request[REQUEST_TEL2]);
				this.Tel3 = StringUtility.ToEmpty(request[REQUEST_TEL3]);
			}
			#endregion

			#region メソッド
			/// <summary>
			/// 検証
			/// </summary>
			/// <returns>成功：true、失敗：false</returns>
			public bool Validate()
			{
				var checkCode = CreateCheckCode(string.Join("^||^", this.NextUrl, this.MId, this.TimeStamp));
				return (checkCode.ToLower() == this.CheckCode.ToLower());
			}
			#endregion

			#region プロパティ
			/// <summary>nurl</summary>
			/// <remarks>ログイン後遷移先URL</remarks>
			public string NextUrl { get; private set; }
			/// <summary>メンバID</summary>
			public string MId { get; private set; }
			/// <summary>TimeStamp</summary>
			public string TimeStamp { get; private set; }
			/// <summary>CheckCode</summary>
			/// <remarks>Md5(loginID +”^||^”+mID +”^||^”+ timeStamp)</remarks>
			public string CheckCode { get; private set; }
			/// <summary>ニックネーム</summary>
			public string Nickname { get; private set; }
			/// <summary>誕生年</summary>
			public string BirthYear { get; private set; }
			/// <summary>誕生月</summary>
			public string BirthMonth { get; private set; }
			/// <summary>誕生日</summary>
			public string BirthDay { get; private set; }
			/// <summary>性別</summary>
			public string Sex { get; private set; }
			/// <summary>都道府県</summary>
			public string Pref { get; private set; }
			/// <summary>メールアドレス</summary>
			public string MailAddr { get; private set; }
			/// <summary>電話番号1</summary>
			public string Tel1 { get; private set; }
			/// <summary>電話番号2</summary>
			public string Tel2 { get; private set; }
			/// <summary>電話番号3</summary>
			public string Tel3 { get; private set; }
			#endregion
		}
		#endregion
	}
}