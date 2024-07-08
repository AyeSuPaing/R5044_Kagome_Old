/*
=========================================================================================================
  Module      : User Conversion (UserConversion.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace w2.App.Common.CrossPoint.User.Helper
{
	/// <summary>
	/// ユーザー情報変換
	/// </summary>
	public class UserConversion
	{
		/// <summary>
		/// 性別をAPI側の文字列に変換
		/// </summary>
		/// <param name="sex">性別</param>
		/// <returns>API側の性別</returns>
		internal static string GetApiSex(string sex)
		{
			switch (sex)
			{
				case Constants.FLG_USER_SEX_FEMALE:
					return Constants.CROSS_POINT_FLG_USER_SEX_FEMALE;

				case Constants.FLG_USER_SEX_MALE:
					return Constants.CROSS_POINT_FLG_USER_SEX_MALE;

				default:
					return Constants.CROSS_POINT_FLG_USER_SEX_UNKNOWN;
			}
		}

		/// <summary>
		/// メール希望フラグをAPI側の文字列に変換
		/// </summary>
		/// <param name="mailFlg">メール希望フラグ</param>
		/// <returns>API側のメール希望</returns>
		internal static string GetApiMailFlg(string mailFlg)
		{
			switch (mailFlg)
			{
				case Constants.FLG_USER_MAILFLG_OK:
					return Constants.CROSS_POINT_FLG_USER_MAIL_FLG_OK;

				case Constants.FLG_USER_MAILFLG_NG:
					return Constants.CROSS_POINT_FLG_USER_MAIL_FLG_NG;

				default:
					return Constants.CROSS_POINT_FLG_USER_MAIL_FLG_NG;
			}
		}

		/// <summary>
		/// ハッシュ値(SHA256)取得
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>ハッシュ値</returns>
		internal static string GetHash(string input)
		{
			using (var crypto = new SHA256CryptoServiceProvider())
			{
				var hash = crypto.ComputeHash(Encoding.GetEncoding("shift_jis").GetBytes(input));
				var result = string.Join(string.Empty, hash.Select(character => character.ToString("X2")));
				return result;
			}
		}
	}
}
