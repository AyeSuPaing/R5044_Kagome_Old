/*
=========================================================================================================
  Module      : GMOアトカラユーティリティ(PaymentGmoAtokaraUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Security.Cryptography;
using System.Text;

namespace w2.App.Common.Order.Payment.GMOAtokara.Utils
{
	/// <summary>
	/// GMOアトカラユーティリティ
	/// </summary>
	public class PaymentGmoAtokaraUtil
	{
		/// <summary>
		/// チェックサム作成
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="gmoShopTransactionId">加盟店取引ID</param>
		/// <param name="atokaraTel1">電話番号1</param>
		/// <param name="atokaraZipCode">郵便番号</param>
		/// <param name="atokaraBilledAmount">顧客請求金額</param>
		/// <param name="atokaraSex">性別</param>
		/// <param name="atokaraBirthday">誕生日</param>
		/// <param name="memberRegistDate">会員登録日</param>
		/// <param name="buyCount">購入回数</param>
		/// <param name="buyAmountTotal">購入金額総額</param>
		/// <param name="atokaraMemberId">会員ID</param>
		/// <returns></returns>
		public static string CreateCheckSum(
			string key,
			string gmoShopTransactionId,
			string atokaraTel1,
			string atokaraZipCode,
			string atokaraBilledAmount,
			string atokaraSex,
			string atokaraBirthday,
			string memberRegistDate,
			string buyCount,
			string buyAmountTotal,
			string atokaraMemberId
			)
		{
			var result = string.Empty;

			foreach (var request in new[]
				{
					key,
					gmoShopTransactionId,
					atokaraTel1,
					atokaraZipCode,
					atokaraBilledAmount,
					atokaraSex,
					atokaraBirthday,
					memberRegistDate,
					buyCount,
					buyAmountTotal,
					atokaraMemberId
				})
			{
				result += (result.Length > 0 ? "|" : "") + request;
			}

			return CreateHashSha256(result);
		}

		/// <summary>
		/// ハッシュ（SHA256）作成
		/// </summary>
		/// <param name="data">データ</param>
		/// <returns>ハッシュ（SHA256）</returns>
		private static string CreateHashSha256(string data)
		{
			var keyBytesForHash = Encoding.UTF8.GetBytes(data);
			using (var cryptoService = new SHA256CryptoServiceProvider())
			{
				var computeHash = cryptoService.ComputeHash(keyBytesForHash);
				var result = Convert.ToBase64String(computeHash);
				return result;
			}
		}

	}
}
