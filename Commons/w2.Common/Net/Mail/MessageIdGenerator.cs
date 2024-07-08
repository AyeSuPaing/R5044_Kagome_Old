/*
=========================================================================================================
  Module      : メッセージID生成クラス(MessageIdGenerator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace w2.Common.Net.Mail
{
	public class MessageIdGenerator
	{
		#region コンストラクタ
		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static MessageIdGenerator()
		{
			try
			{
				FQDN = GetFQDN();
			}
			catch
			{
				FQDN = "localhost";
			}
			PID = Process.GetCurrentProcess().Id;
			RANDOM = new Random().Next() & 0xFFFF;
			Serial = 0;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// メッセージIDを生成
		/// </summary>
		/// <returns>メッセージID</returns>
		public static string Generate()
		{
			var unixTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			var serial = Interlocked.Increment(ref Serial);

			// UNIXTime(最大64bit).PID(最大32bit).乱数値(最大16bit).連続値(最大32bit)@FQDN
			// PID, 乱数値, FQDN は不変
			return string.Format("{0}.{1}.{2}.{3}@{4}",
				FormatNumber(unixTime),
				FormatNumber(PID),
				FormatNumber(RANDOM),
				FormatNumber(serial),
				FQDN);
		}

		/// <summary>
		/// 数値をフォーマット
		/// </summary>
		/// <param name="num">数値</param>
		/// <returns>フォーマット後文字列</returns>
		private static string FormatNumber(long num)
		{
			return num.ToString("X");
		}

		/// <summary>
		/// FQDNを取得します
		/// </summary>
		/// <returns>FQDN</returns>
		private static string GetFQDN()
		{
			var domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
			var host = Dns.GetHostName();

			var dotDomain = string.IsNullOrEmpty(domain) ? "" : "." + domain;
			if (host.EndsWith(dotDomain) == false) host += dotDomain;

			return host;
		}
		#endregion

		#region プロパティ
		/// <summary>FQDN</summary>
		private static readonly string FQDN;
		/// <summary>プロセスID</summary>
		private static readonly int PID;
		/// <summary>乱数値</summary>
		private static readonly int RANDOM;
		/// <summary>シリアル</summary>
		private static int Serial;
		#endregion
	}
}