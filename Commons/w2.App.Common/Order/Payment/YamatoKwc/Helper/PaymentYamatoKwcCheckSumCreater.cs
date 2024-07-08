/*
=========================================================================================================
  Module      : ヤマトKWC チェックサムクリエータ(PaymentYamatoKwcCheckSumCreater.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWC チェックサムクリエータ
	/// </summary>
	public static class PaymentYamatoKwcCheckSumCreater
	{
		/// <summary>
		/// 生成（トークン向け）
		/// </summary>
		/// <param name="memberId">会員ID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <returns>ハッシュ文字列</returns>
		public static string CreateForToken(string memberId, string authenticationKey)
		{
			// 認証区分
			// 0：3D セキュアなし　セキュリティコード認証なし
			// 2：3D セキュアなし　セキュリティコード認証あり
			// 1：3D セキュアあり　セキュリティコード認証なし
			// 3：3D セキュアあり　セキュリティコード認証あり
			var rawUrl = WebUtility.GetRawUrl(HttpContext.Current.Request);
			AuthDiv = Constants.PAYMENT_SETTING_YAMATO_KWC_3DSECURE
				&& (rawUrl.IndexOf(Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL) == -1)
				&& (rawUrl.IndexOf(Constants.PAGE_FRONT_FIXED_PURCHASE_DETAIL) == -1)
				&& (rawUrl.IndexOf(Constants.PAGE_FRONT_USER_CREDITCARD_INPUT) == -1)
					? Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_SECURITYCODE ? "3" : "1"
					: Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_SECURITYCODE ? "2" : "0";

			var source = memberId + authenticationKey + Constants.PAYMENT_SETTING_YAMATO_KWC_ACCESS_KEY + AuthDiv;
			var hashString = CreateHashString(source);
			return hashString;
		}

		/// <summary>
		/// 生成（トークン以外）
		/// </summary>
		/// <param name="memberId">会員ID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <returns>ハッシュ文字列</returns>
		public static string CreateForAuth(string memberId, string authenticationKey)
		{
			var source = memberId + authenticationKey + Constants.PAYMENT_SETTING_YAMATO_KWC_ACCESS_KEY;
			var hashString = CreateHashString(source);
			return hashString;
		}

		/// <summary>
		/// ハッシュ文字列作成
		/// </summary>
		/// <param name="source">エンコード</param>
		/// <returns>ハッシュ文字列</returns>
		private static string CreateHashString(string source)
		{
			var hash = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(source));

			var hashString = string.Join("", hash.Select(b => b.ToString("x2")));
			return hashString;
		}

		/// <summary>与信区分</summary>
		public static string AuthDiv { get; set; }
	}
}
