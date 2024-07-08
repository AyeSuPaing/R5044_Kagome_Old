/*
=========================================================================================================
  Module      : YAHOOモール注文者 (YahooMallOrderOwner.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Mall.Yahoo.Dto;
using w2.App.Common.Mall.Yahoo.YahooMallOrders.ValueObjects;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders
{
	/// <summary>
	/// YAHOOモール注文者クラス
	/// </summary>
	public class YahooMallOrderOwner
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文</param>
		public YahooMallOrderOwner(OrderInfo order)
		{
			// 苗字
			var billLastName = order.Pay.BillLastName;
			if (billLastName.Length > 20)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BillLastName={billLastName}",
					nameof(order.Pay.BillLastName));
			}
			this.OwnerName1 = billLastName;

			// 苗字かな
			var billLastNameKana = order.Pay.BillLastNameKana;
			if (billLastNameKana.Length > 30)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BillLastNameKana={billLastNameKana}",
					nameof(order.Pay.BillLastNameKana));
			}
			this.OwnerNameKana1 = billLastNameKana;

			// 名前
			var billFirstName = order.Pay.BillFirstName;
			if (billFirstName.Length > 20)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BillFirstName={billFirstName}",
					nameof(order.Pay.BillFirstName));
			}
			this.OwnerName2 = billFirstName;

			// 名前かな
			var billFirstNameKana = order.Pay.BillFirstNameKana;
			if (billLastNameKana.Length > 30)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BillFirstNameKana={billFirstNameKana}",
					nameof(order.Pay.BillFirstNameKana));
			}
			this.OwnerNameKana2 = billFirstNameKana;

			// 氏名
			var billName = billLastName + billFirstName;
			if (string.IsNullOrEmpty(billName))
			{
				throw new ArgumentException($"氏名が空です。BillLastName+BillFirstName={billName}");
			}
			this.OwnerName = billName;
			this.OwnerNameKana = billLastNameKana + billFirstNameKana;
			
			// 郵便番号
			this.OwnerZip = new YahooMallOrderZipValueObject(order.Pay.BillZipCode);

			// 県 住所
			var billPrefecture = StringUtility.ToZenkaku(order.Pay.BillPrefecture);
			if (string.IsNullOrEmpty(billPrefecture)
				|| (billPrefecture.Length > 50)
				|| (Array.Exists(Constants.STR_PREFECTURES_LIST, elm => elm == billPrefecture) == false))
			{
				throw new ArgumentException($"正しい都道府県ではありません。BillPrefecture={billPrefecture}", nameof(order.Pay.BillPrefecture));
			}
			this.OwnerAddr1 = billPrefecture;

			// 市区町村
			var billCity = StringUtility.ToZenkaku(order.Pay.BillCity);
			if (billCity.Length > 50)
			{
				throw new ArgumentException($"想定のサイズを超えています。BillCity={billCity}", nameof(order.Pay.BillCity));
			}
			this.OwnerAddr2 = billCity;

			// 住所1
			var billAddress1 = StringUtility.ToZenkaku(order.Pay.BillAddress1);
			if (billAddress1.Length > 50)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BillAddress1={billAddress1}",
					nameof(order.Pay.BillAddress1));
			}
			this.OwnerAddr3 = billAddress1;

			// 住所2
			var billAddress2 = StringUtility.ToZenkaku(order.Pay.BillAddress2);
			if (billAddress2.Length > 50)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BillAddress1={billAddress2}",
					nameof(order.Pay.BillAddress2));
			}
			this.OwnerAddr4 = billAddress2;

			// 住所
			this.OwnerAddr = billPrefecture + billCity + billAddress1 + billAddress2;

			// 電話番号
			this.OwnerTel = new YahooMallOrderTelValueObject(order.Pay.BillPhoneNumber);

			// Eメールアドレス
			var billMailAddress = order.Pay.BillMailAddress;
			if (billMailAddress.Length > 256)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BillMailAddress={billMailAddress}",
					nameof(order.Pay.BillMailAddress));
			}
			this.OwnerMailAddr = billMailAddress;

			// 注文者区分
			if (Enum.TryParse<DeviceType>(order.DeviceType, out var deviceType) == false)
			{
				FileLogger.WriteWarn($"想定外の値です。DeviceType={order.DeviceType}");
			}
			switch (deviceType)
			{
				case DeviceType.Pc:
				case DeviceType.Tablet:
				default:
					this.OwnerKbn = Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER;
					break;

				case DeviceType.Mb:
					this.OwnerKbn = Constants.FLG_ORDEROWNER_OWNER_KBN_MOBILE_USER;
					break;

				case DeviceType.Sp:
					this.OwnerKbn = Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_USER;
					break;
			}
		}

		/// <summary>
		/// ユーザーモデル生成
		/// </summary>
		/// <param name="order">注文</param>
		/// <returns>ユーザー</returns>
		public YahooMallOrderUser GenerateUser(OrderInfo order)
		{
			var result = new YahooMallOrderUser(
				order,
				this.OwnerName1,
				this.OwnerNameKana1,
				this.OwnerNameKana2,
				this.OwnerName2,
				this.OwnerName,
				this.OwnerNameKana,
				this.OwnerZip,
				this.OwnerAddr1,
				this.OwnerAddr2,
				this.OwnerAddr3,
				this.OwnerAddr4,
				this.OwnerTel,
				this.OwnerMailAddr);
			return result;
		}

		/// <summary>注文者区分</summary>
		public string OwnerKbn { get; } = "";
		/// <summary>ご請求先氏名</summary>
		public string OwnerName { get; } = "";
		/// <summary>ご請求先氏名</summary>
		public string OwnerNameKana { get; } = "";
		/// <summary>ご請求先名字</summary>
		public string OwnerName1 { get; } = "";
		/// <summary>ご請求先名字カナ</summary>
		public string OwnerNameKana1 { get; } = "";
		/// <summary>ご請求先名前</summary>
		public string OwnerName2 { get; } = "";
		/// <summary>ご請求先名前カナ</summary>
		public string OwnerNameKana2 { get; } = "";
		/// <summary>ご請求先郵便番号</summary>
		public YahooMallOrderZipValueObject OwnerZip { get; }
		/// <summary>ご請求先住所</summary>
		public string OwnerAddr { get; } = "";
		/// <summary>ご請求先都道府県</summary>
		public string OwnerAddr1 { get; } = "";
		/// <summary>ご請求先市区郡</summary>
		public string OwnerAddr2 { get; } = "";
		/// <summary>ご請求先住所1</summary>
		public string OwnerAddr3 { get; } = "";
		/// <summary>ご請求先住所2</summary>
		public string OwnerAddr4 { get; } = "";
		/// <summary>ご請求先電話番号</summary>
		public YahooMallOrderTelValueObject OwnerTel { get; }
		/// <summary>ご請求先メールアドレス</summary>
		public string OwnerMailAddr { get; } = "";
	}
}
