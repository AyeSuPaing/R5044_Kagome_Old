/*
=========================================================================================================
  Module      : OMotion Utility (OMotionUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace w2.App.Common.User.OMotion
{
	/// <summary>
	/// O-MOTION utility
	/// </summary>
	public static class OMotionUtility
	{
		/// <summary>
		/// 認証IDのCookieキー取得
		/// </summary>
		/// <returns></returns>
		public static string GetAuthoriIdCookieKey()
		{
			return string.Format("sess_auth_{0}", Constants.OMOTION_MERCHANTID);
		}

		/// <summary>
		/// リクエストID作成
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>リクエストID</returns>
		public static string CreateOMotionReqId(string loginId)
		{
			// ソルト値付与
			var result = string.Concat(loginId, Constants.OMOTION_SALT);

			return result;
		}

		/// <summary>
		/// リクエストID作成
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>リクエストID</returns>
		public static string CreateHashOMotionReqId(string loginId)
		{
			// ソルト値付与
			var omotionReqId = CreateOMotionReqId(loginId);

			return CreateHashSha256(omotionReqId);
		}

		/// <summary>
		/// Create hash SHA256
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>Hash SHA256</returns>
		private static string CreateHashSha256(string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			using (var crypto = new SHA256CryptoServiceProvider())
			{
				var hashed = crypto.ComputeHash(bytes);
				var result = string.Join(string.Empty, hashed.Select(item => item.ToString("X2").ToLower()));
				return result;
			}
		}
	}
}
