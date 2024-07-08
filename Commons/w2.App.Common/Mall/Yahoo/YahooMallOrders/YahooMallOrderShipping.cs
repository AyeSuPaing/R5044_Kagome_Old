/*
=========================================================================================================
  Module      : YAHOOモール注文配送 (YahooMallOrderShipping.cs)
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
	/// YAHOOモール注文配送
	/// </summary>
	public class YahooMallOrderShipping
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文</param>
		public YahooMallOrderShipping(OrderInfo order)
		{
			// 配送希望日
			DateTime? shippingDate = null;
			if (string.IsNullOrEmpty(order.Ship.ShipRequestDate) == false)
			{
				if (DateTime.TryParse(order.Ship.ShipRequestDate, out var temp) == false)
				{
					FileLogger.WriteWarn($"想定外の値です。DeviceType={order.DeviceType}");
				}
				else
				{
					shippingDate = temp;
				}
			}
			this.ShippingDate = shippingDate;

			// 配送希望時間
			this.ShipRequestTime = order.Ship.ShipRequestTime;
			this.TrimmedShipRequestTime= order.Ship.ShipRequestTime.Trim().TrimStart('0').Replace("-", "～");

			// 苗字
			var shipLastname = order.Ship.ShipLastName;
			if (shipLastname.Length > 20)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。ShipLastName={shipLastname}",
					nameof(order.Ship.ShipLastName));
			}
			this.ShippingName1 = shipLastname;
			
			// 苗字かな
			var shipLastNameKana = order.Ship.ShipLastNameKana;
			if (shipLastNameKana.Length > 30)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。ShipLastNameKana={shipLastNameKana}",
					nameof(order.Ship.ShipLastNameKana));
			}
			this.ShippingNameKana1 = shipLastNameKana;

			// 名前
			var shipFirstName = order.Ship.ShipFirstName;
			if (shipFirstName.Length > 20)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。ShipFirstName={shipFirstName}",
					nameof(order.Ship.ShipFirstName));
			}
			this.ShippingName2 = shipFirstName;

			// 名前かな
			var shipFirstNameKana = order.Ship.ShipFirstNameKana;
			if (shipFirstNameKana.Length > 30)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。ShipFirstNameKana={shipFirstNameKana}",
					nameof(order.Ship.ShipFirstNameKana));
			}
			this.ShippingNameKana2 = shipFirstNameKana;

			// 氏名
			var shipName = shipLastname + shipFirstName;
			if (string.IsNullOrEmpty(shipName))
			{
				throw new ArgumentException($"氏名が空です。ShipLastName+ShipFirstName={shipName}");
			}
			this.ShippingName = shipName;
			this.ShippingNameKana = shipLastNameKana + shipFirstNameKana;

			// 郵便番号
			this.ShippingZip = new YahooMallOrderZipValueObject(order.Ship.ShipZipCode);

			// 県 住所
			var shipPrefecture = StringUtility.ToZenkaku(order.Ship.ShipPrefecture);
			if (string.IsNullOrEmpty(shipPrefecture)
				|| shipPrefecture.Length > 50
				|| (Array.Exists(Constants.STR_PREFECTURES_LIST, elm => elm == shipPrefecture) == false))
			{
				throw new ArgumentException(
					$"正しい都道府県ではありません。ShipPrefecture={shipPrefecture}",
					nameof(order.Ship.ShipPrefecture));
			}
			this.ShippingAddr1 = shipPrefecture;

			// 市区町村
			var shipCity = StringUtility.ToZenkaku(order.Ship.ShipCity);
			if (shipCity.Length > 50)
			{
				throw new ArgumentException($"想定のサイズを超えています。ShipCity={shipCity}", nameof(order.Ship.ShipCity));
			}
			this.ShippingAddr2 = shipCity;

			// 住所1
			var shipAddress1 = StringUtility.ToZenkaku(order.Ship.ShipAddress1);
			if (shipAddress1.Length > 50)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。ShipAddress1={shipAddress1}",
					nameof(order.Ship.ShipAddress1));
			}
			this.ShippingAddr3 = shipAddress1;

			// 住所2
			var shipAddress2 = StringUtility.ToZenkaku(order.Ship.ShipAddress2);
			if (shipAddress2.Length > 50)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。ShipAddress2={shipAddress2}",
					nameof(order.Ship.ShipAddress2));
			}
			this.ShippingAddr4 = shipAddress2;
			
			// 住所
			this.ShippingAddr = shipPrefecture + shipCity + shipAddress1 + shipAddress2;

			// 電話番号
			this.ShippingTel = new YahooMallOrderTelValueObject(order.Ship.ShipPhoneNumber);
		}

		/// <summary>
		/// 配送希望時間があるかどうか
		/// </summary>
		/// <returns>配送希望時間があるかどうか</returns>
		public bool HasShipRequestTime() => string.IsNullOrEmpty(this.ShipRequestTime) == false;

		/// <summary>配送希望日</summary>
		public DateTime? ShippingDate { get; }
		/// <summary>配送希望時間</summary>
		public string ShipRequestTime { get; } = "";
		/// <summary>トリム済みの配送希望時間</summary>
		public string TrimmedShipRequestTime { get; } = "";
		/// <summary>お届け先氏名</summary>
		public string ShippingName { get; } = "";
		/// <summary>お届け先氏名かな</summary>
		public string ShippingNameKana { get; } = "";
		/// <summary>お届け先名字</summary>
		public string ShippingName1 { get; } = "";
		/// <summary>お届け先名字かな</summary>
		public string ShippingNameKana1 { get; } = "";
		/// <summary>お届け先名前</summary>
		public string ShippingName2 { get; } = "";
		/// <summary>お届け先名前かな</summary>
		public string ShippingNameKana2 { get; } = "";
		/// <summary>お届け先郵便番号</summary>
		public YahooMallOrderZipValueObject ShippingZip { get; }
		/// <summary>お届け先住所</summary>
		public string ShippingAddr { get; } = "";
		/// <summary>お届け先都道府県</summary>
		public string ShippingAddr1 { get; } = "";
		/// <summary>お届け先市区郡</summary>
		public string ShippingAddr2 { get; } = "";
		/// <summary>お届け先住所1</summary>
		public string ShippingAddr3 { get; } = "";
		/// <summary>お届け先住所2</summary>
		public string ShippingAddr4 { get; } = "";
		/// <summary>お届け先電話番号</summary>
		public YahooMallOrderTelValueObject ShippingTel { get; }
	}
}
