/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 送付者モデル (RakutenApiSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 送付者モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_SENDER_MODEL)]
	public class RakutenApiSender
	{
		/// <summary>郵便番号1</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_ZIP_CODE1)]
		public string ZipCode1 { get; set; }
		/// <summary>郵便番号2</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_ZIP_CODE2)]
		public string ZipCode2 { get; set; }
		/// <summary>都道府県</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_PREFECTURE)]
		public string Prefecture { get; set; }
		/// <summary>郡市区</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_CITY)]
		public string City { get; set; }
		/// <summary>それ以降の住所</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_SUBADDRESS)]
		public string SubAddress { get; set; }
		/// <summary>姓</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_FAMILY_NAME)]
		public string FamilyName { get; set; }
		/// <summary>名</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_FIRST_NAME)]
		public string FirstName { get; set; }
		/// <summary>姓カナ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_FAMILY_NAME_KANA)]
		public string FamilyNameKana { get; set; }
		/// <summary>名カナ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_FIRST_NAME_KANA)]
		public string FirstNameKana { get; set; }
		/// <summary>電話番号1</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_PHONE_NUMBER1)]
		public string PhoneNumber1 { get; set; }
		/// <summary>電話番号2</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_PHONE_NUMBER2)]
		public string PhoneNumber2 { get; set; }
		/// <summary>電話番号3</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_PHONE_NUMBER3)]
		public string PhoneNumber3 { get; set; }
		/// <summary>離島フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SENDER_MODEL_ISOLATED_ISLAND_FLAG)]
		public int? IsolatedIslandFlag { get; set; }
	}
}
