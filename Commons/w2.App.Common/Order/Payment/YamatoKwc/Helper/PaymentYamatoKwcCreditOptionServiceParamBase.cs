/*
=========================================================================================================
  Module      : ヤマトKWC クレジットオプションサービスパラメタ基底クラス(PaymentYamatoKwcCreditOptionServiceParamBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Web;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWCクレジットオプションサービスパラメタ基底クラス
	/// </summary>
	public abstract class PaymentYamatoKwcCreditOptionServiceParamBase
	{
		/// <summary>
		/// 文字列表現
		/// </summary>
		/// <returns>会員ID、認証キー</returns>
		public override string ToString()
		{
			return (this.MembderId + " " + this.AuthenticationKey).Trim();
		}

		/// <summary>認証区分</summary>
		/// <remarks>登録済みクレカで使用のため3Dセキュアは非対応</remarks>
		public string AuthDiv
		{
			get
			{
				return string.IsNullOrEmpty(this.SecurityCode) ? "0" : "2";
			}
		}
		/// <summary>オプションサービス区分</summary>
		public string OptionServiceDiv { get; set; }
		/// <summary>カード会社コード(API 用)</summary>
		public string CardCodeApi { get; set; }
		/// <summary>クレジットトークン</summary>
		public string Token { get; set; }

		/// <summary>セキュリティコード</summary>
		public string SecurityCode { get; set; }
		/// <summary>カード保有者を特定するID</summary>
		public string MembderId { get; set; }
		/// <summary>認証キー</summary>
		public string AuthenticationKey { get; set; }
		/// <summary>カード識別キー</summary>
		public string CardKey { get; set; }
		/// <summary>最終利用日時</summary>
		public string LastCreditDate { get; set; }
		/// <summary>チェックサム</summary>
		public string CheckSum { get; set; }
	}
}
