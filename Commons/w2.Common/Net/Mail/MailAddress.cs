/*
=========================================================================================================
  Module      : メールアドレスモジュール(MailAddress.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Util;

namespace w2.Common.Net.Mail
{
	///*********************************************************************************************
	/// <summary>
	/// メールアドレス情報を格納する
	/// </summary>
	///*********************************************************************************************
	public class MailAddress
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="address">メールアドレス</param>
		/// <param name="displayName">表示名</param>
		public MailAddress(string address, string displayName = "")
		{
			this.AddressRaw = address;
			this.Address = ConvertSendableAddress(address);
			this.DisplayName = displayName;
		}

		/// <summary>
		/// メールアドレスオブジェクト取得
		/// </summary>
		/// <param name="headerMailAddress">単一のヘッダメールアドレス（「XXX ＜a@b.c＞」形式）</param>
		/// <returns>メールアドレスオブジェクト</returns>
		public static MailAddress GetInstance(string headerMailAddress)
		{
			Match match = Regex.Match(headerMailAddress, "<.*>");
			if (match.Success)
			{
				return new MailAddress(match.Value.Replace("<", "").Replace(">", "").Trim(), headerMailAddress.Replace(match.Value, "").Trim());
			}
			else
			{
				return new MailAddress(headerMailAddress.Trim());
			}
		}

		/// <summary>
		/// メールアドレスを送信可能形式に変換
		/// </summary>
		/// <param name="strAddress">メールアドレス</param>
		/// <returns>返還後メールアドレス</returns>
		private string ConvertSendableAddress(string strAddress)
		{
			// 変換（既にエスケープされていたら変換しない）
			if (((strAddress.IndexOf(".@") != -1) || (strAddress.IndexOf("..") != -1))
				&& (strAddress.IndexOf("\"") == -1))
			{
				return "\"" + strAddress.Substring(0, strAddress.IndexOf("@")) + "\"" + strAddress.Substring(strAddress.IndexOf("@"));
			}

			return strAddress;
		}

		/// <summary>
		/// メールアドレスを "表示名 &lt;sample@address.co.jp&gt;" 形式で取得します。
		/// 表示名が空のときはメールアドレスをそのまま返します。
		/// </summary>
		/// <param name="displayName">表示名</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <returns></returns>
		public static string GetMailAddrString(string displayName, string mailAddress)
		{
			return string.IsNullOrEmpty(displayName) ? mailAddress : String.Format("{0} <{1}>", displayName, mailAddress);
		}

		/// <summary>
		/// Get address
		/// </summary>
		/// <returns>Return address</returns>
		public override string ToString()
		{
			return this.Address;
		}

		/// <summary>メールアドレス(".."エスケープ済み)</summary>
		public string Address { get; private set; }
		/// <summary>メールアドレス(".."未エスケープ)</summary>
		public string AddressRaw { get; private set; }
		/// <summary>表示名</summary>
		public string DisplayName { get; private set; }
	}
}
