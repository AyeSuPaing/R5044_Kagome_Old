/*
=========================================================================================================
  Module      : ユーザーパスワード暗号化クラス (UserPassowordCryptor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Util.Security;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// ユーザーパスワード暗号化クラス
	/// </summary>
	public class UserPassowordCryptor
	{
		/// <summary>
		/// パスワード暗号化
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string PasswordEncrypt(string password)
		{
			RijndaelCrypto rcUserPassword = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			return rcUserPassword.Encrypt(password);
		}

		/// <summary>
		/// パスワード復号化
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string PasswordDecrypt(string password)
		{
			RijndaelCrypto rcUserPassword = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			return rcUserPassword.Decrypt(password);
		}
	}
}
