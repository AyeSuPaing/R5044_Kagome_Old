/*
=========================================================================================================
  Module      : ログインユーザ置換処理クラス(LoginUserReplaceTagManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Global.Config;
using w2.Common.Web;
using w2.Domain.User;

namespace w2.App.Common.Affiliate
{
	/// <summary>
	/// ログインユーザ置換処理クラス
	/// </summary>
	public class LoginUserReplaceTagManager : ReplaceTagManager
	{
		/// <summary>
		/// 置換処理
		/// </summary>
		/// <param name="user">ユーザモデル</param>
		/// <param name="value">処理対象文字列</param>
		/// <returns>置換処理後の文字列</returns>
		public string ReplaceTag(UserModel user, string value)
		{
			var result = value;

			var userModel = user ?? new UserModel();

			result = result.Replace(this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_ID].Tag, HtmlSanitizer.HtmlEncode(userModel.UserId));
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_EMAIL].Tag,
				(string.IsNullOrEmpty(userModel.MailAddr) == false) ? HtmlSanitizer.HtmlEncode(userModel.MailAddr) : HtmlSanitizer.HtmlEncode(userModel.MailAddr2));
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_TEL1_WITH_COUNTRY_CODE].Tag,
				(string.IsNullOrEmpty(userModel.Tel1) == false) ? HtmlSanitizer.HtmlEncode(AddInternationalTelCode(userModel)) : string.Empty);
			result = result.Replace(this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_SEX].Tag, userModel.Sex);
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_FAMILY_NAME].Tag,
				HtmlSanitizer.HtmlEncode(userModel.Name1));
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_FIRST_NAME].Tag,
				HtmlSanitizer.HtmlEncode(userModel.Name2));
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_FAMILY_NAME_KANA].Tag,
				HtmlSanitizer.HtmlEncode(userModel.NameKana1));
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_FIRST_NAME_KANA].Tag,
				HtmlSanitizer.HtmlEncode(userModel.NameKana2));
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_BIRTH_DAY].Tag,
				(userModel.Birth != null) ? userModel.Birth.ToString() : "");
			result = result.Replace(
				this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_AGE].Tag,
				(userModel.Birth != null) ? UserAge(userModel.Birth.Value).ToString() : "");
			result = result.Replace(this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_ZIP].Tag, HtmlSanitizer.HtmlEncode(userModel.Zip1 + userModel.Zip2));
			result = result.Replace(this.LoginUserReplaceTags[ReplaceTagKey.LOGIN_USER_PREF].Tag, HtmlSanitizer.HtmlEncode(userModel.Addr1));

			return result;
		}

		/// <summary>
		/// 国番号付与
		/// </summary>
		/// <param name="user">ユーザーモデル</param>
		/// <returns>電話番号</returns>
		private string AddInternationalTelCode(UserModel user)
		{
			var telNumber = "";
			if (string.IsNullOrEmpty(user.Tel1)) return "";

			var code = GlobalConfigs.GetInstance().GlobalSettings.InternationalTelephoneCode
				.FirstOrDefault(x => (x.Iso == user.AddrCountryIsoCode));

			if (Constants.GLOBAL_OPTION_ENABLE == false)
			{
				var isoCode = new Regex("0");
				telNumber = isoCode.Replace(user.Tel1, "81", 1).Replace("-", "");
				return telNumber;
			}

			if (code == null)
			{
				telNumber = user.Tel1.Replace("-", "");
				return telNumber;
			}

			telNumber = code.Number;
			telNumber += user.Tel1.StartsWith("0")
				? string.Join("", user.Tel1.Skip(1))
				: user.Tel1;

			var result = telNumber.Replace("-", "");
			return result;
		}
	}
}