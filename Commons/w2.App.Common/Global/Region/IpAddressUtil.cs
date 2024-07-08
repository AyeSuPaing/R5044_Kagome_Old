/*
=========================================================================================================
  Module      : IPアドレス ユーティリティクラス(IpAddressUtill.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using System.Text.RegularExpressions;
using w2.Domain.CountryIpv4;

namespace w2.App.Common.Global.Region
{
	/// <summary>
	/// IPアドレス ユーティリティクラス
	/// </summary>
	public class IpAddressUtil
	{
		/// <summary>
		/// IPアドレス(CIDR記法)からIPアドレスv4モデルを返す。
		/// </summary>
		/// <param name="ipAddressCidr">IPアドレス(CIDR記法)</param>
		/// <returns>IPアドレスv4モデルを返す。 CIDR記法でない場合はnull</returns>
		public static CountryIpv4Model ConvertByipAddressCider(string ipAddressCidr)
		{
			var parts = ipAddressCidr.Split('.', '/');

			if (parts.Length != 4) return null;

			var ipnum = (Convert.ToUInt32(parts[0]) << 24) |
				(Convert.ToUInt32(parts[1]) << 16) |
				(Convert.ToUInt32(parts[2]) << 8) |
				Convert.ToUInt32(parts[3]);

			var maskbits = Convert.ToInt32(parts[4]);
			uint mask = 0xffffffff;
			mask <<= (32 - maskbits);

			uint ipUint = ipnum & mask;
			uint ipBroadCastUint = ipnum | (mask ^ 0xffffffff);

			var ip = ConvertToIpString(ipUint);
			var ipBoradCast = ConvertToIpString(ipBroadCastUint);

			var model = new CountryIpv4Model()
			{
				Ip = ip,
				IpNumeric = ConvertToInt(ip),
				IpBroadcast = ipBoradCast,
				IpBroadcastNumeric = ConvertToInt(ipBoradCast)
			};
			return model;
		}

		/// <summary>
		/// 〇〇〇.〇〇〇.〇〇〇.〇〇〇形式のIPアドレスを数値に変換します。
		/// </summary>
		/// <param name="ipAddress">IPアドレス</param>
		/// <returns>変換後の数値</returns>
		public static int ConvertToInt(string ipAddress)
		{
			var bytes = IPAddress.Parse(ipAddress).GetAddressBytes();

			// リトルエンディアン対応
			if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

			// ToInt64用にLength8のbyte配列を用意してコピーする
			var target = new byte[8];
			Array.Copy(bytes, target, bytes.Length);

			// 最大値255.255.255.255 = 4,294,967,295
			int result = (int)BitConverter.ToInt64(target, 0);
			return result;
		}

		/// <summary>
		/// uint型のIPアドレスを〇〇〇.〇〇〇.〇〇〇.〇〇〇の形式に変換して返します。
		/// </summary>
		/// <param name="ip">uint型のIPアドレス</param>
		/// <returns>変換後のIPアドレス</returns>
		private static string ConvertToIpString(uint ip)
		{
			var result = String.Format("{0}.{1}.{2}.{3}", ip >> 24, (ip >> 16) & 0xff, (ip >> 8) & 0xff, ip & 0xff);
			return result;
		}

		/// <summary>
		/// 文字列が0.0.0.0～255.255.255.255の形式になっていることを判別します。
		/// </summary>
		/// <param name="ip">IPアドレス</param>
		/// <returns>OK:True NG:False</returns>
		public static bool CheckIp(string ip)
		{
			IPAddress address = null;
			var result = IPAddress.TryParse(ip, out address);
			return result;
		}
	}
}
