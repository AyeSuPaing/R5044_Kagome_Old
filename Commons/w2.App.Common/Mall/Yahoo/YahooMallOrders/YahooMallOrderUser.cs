/*
=========================================================================================================
  Module      : YAHOOモール注文ユーザー (YahooMallOrderUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Mall.Yahoo.Dto;
using w2.App.Common.Mall.Yahoo.YahooMallOrders.ValueObjects;
using w2.Common.Logger;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders
{
	/// <summary>
	/// YAHOOモール注文ユーザー
	/// </summary>
	public class YahooMallOrderUser
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="ownerName1">注文者名1</param>
		/// <param name="ownerNameKana1">注文者名かな1</param>
		/// <param name="ownerNameKana2">注文者名2</param>
		/// <param name="ownerName2">注文者かな2</param>
		/// <param name="ownerName">注文者名</param>
		/// <param name="ownerNameKana">注文者名かな</param>
		/// <param name="ownerZip">郵便番号</param>
		/// <param name="ownerAddr1">住所1</param>
		/// <param name="ownerAddr2">住所2</param>
		/// <param name="ownerAddr3">住所3</param>
		/// <param name="ownerAddr4">住所4</param>
		/// <param name="ownerTel">電話番号</param>
		/// <param name="mailAddr">Eメールアドレス</param>
		public YahooMallOrderUser(
			OrderInfo order,
			string ownerName1,
			string ownerNameKana1,
			string ownerNameKana2,
			string ownerName2,
			string ownerName,
			string ownerNameKana,
			YahooMallOrderZipValueObject ownerZip,
			string ownerAddr1,
			string ownerAddr2,
			string ownerAddr3,
			string ownerAddr4,
			YahooMallOrderTelValueObject ownerTel,
			string mailAddr)
		{
			this.UserName1 = ownerName1;
			this.UserNameKana1 = ownerNameKana1;
			this.UserName2 = ownerName2;
			this.UserNameKana2 = ownerNameKana2;
			this.UserName = ownerName;
			this.UserNameKana = ownerNameKana;
			this.UserZip = ownerZip;
			this.UserAddr = ownerAddr1 + ownerAddr2 + ownerAddr3 + ownerAddr4;
			this.UserAddr1 = ownerAddr1;
			this.UserAddr2 = ownerAddr2;
			this.UserAddr3 = ownerAddr3;
			this.UserAddr4 = ownerAddr4;
			this.UserTel = ownerTel;

			if (Enum.TryParse<DeviceType>(order.DeviceType, out var deviceType) == false)
			{
				FileLogger.WriteWarn($"想定外の値です。DeviceType={order.DeviceType}");
			}
			switch (deviceType)
			{
				case DeviceType.Pc:
				case DeviceType.Tablet:
				default:
					this.UserKbn = Constants.FLG_USER_USER_KBN_PC_USER;
					this.UserMailAddr = mailAddr;
					break;

				case DeviceType.Mb:
					this.UserKbn = Constants.FLG_USER_USER_KBN_MOBILE_USER;
					this.UserMailAddr2 = mailAddr;
					break;

				case DeviceType.Sp:
					this.UserKbn = Constants.FLG_USER_USER_KBN_SMARTPHONE_USER;
					this.UserMailAddr = mailAddr;
					break;
			}
		}

		/// <summary>ユーザー区分</summary>
		public string UserKbn { get; } = "";
		/// <summary>ユーザーメールアドレス</summary>
		public string UserMailAddr { get; } = "";
		/// <summary>ユーザーメールアドレス2</summary>
		public string UserMailAddr2 { get; } = "";
		/// <summary>ユーザー氏名</summary>
		public string UserName { get; } = "";
		/// <summary>ユーザー氏名</summary>
		public string UserNameKana { get; } = "";
		/// <summary>ユーザー名字</summary>
		public string UserName1 { get; } = "";
		/// <summary>ユーザー名字カナ</summary>
		public string UserNameKana1 { get; } = "";
		/// <summary>ユーザー名前</summary>
		public string UserName2 { get; } = "";
		/// <summary>ユーザー名前カナ</summary>
		public string UserNameKana2 { get; } = "";
		/// <summary>ユーザー郵便番号</summary>
		public YahooMallOrderZipValueObject UserZip { get; }
		/// <summary>ユーザー住所</summary>
		public string UserAddr { get; } = "";
		/// <summary>ユーザー都道府県</summary>
		public string UserAddr1 { get; } = "";
		/// <summary>ユーザー市区郡</summary>
		public string UserAddr2 { get; } = "";
		/// <summary>ユーザー住所1</summary>
		public string UserAddr3 { get; } = "";
		/// <summary>ユーザー住所2</summary>
		public string UserAddr4 { get; } = "";
		/// <summary>ユーザー電話番号</summary>
		public YahooMallOrderTelValueObject UserTel { get; }
	}
}
