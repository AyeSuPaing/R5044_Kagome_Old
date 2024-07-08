/*
=========================================================================================================
  Module      : 楽天IDConnect楽天ID会員情報取得レスポンスデータクラス(RakutenIDConnectUserInfoResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Util;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnect楽天ID会員情報取得レスポンスデータクラス
	/// </summary>
	[Serializable]
	public class RakutenIDConnectUserInfoResponseData : RakutenIDConnectBaseResponseData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal RakutenIDConnectUserInfoResponseData(string responseString)
			: base(responseString)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザー識別子</summary>
		public string Sub
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("sub")); }
		}
		/// <summary>ユーザー識別子</summary>
		/// <remarks>URL形式 OpenID</remarks>
		public string OpenIdId
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("openid_id")); }
		}
		/// <summary>メールアドレス</summary>
		public string Email
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("email")); }
		}
		/// <summary>氏名（名）</summary>
		public string GivenName
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("given_name")); }
		}
		/// <summary>氏名（姓）</summary>
		public string FamilyName
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("family_name")); }
		}
		/// <summary>氏名カナ（メイ）</summary>
		public string GivenNameKana
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("given_name#ja-Kana-JP")); }
		}
		/// <summary>氏名カナ（セイ）</summary>
		public string FamilyNameKana
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("family_name#ja-Kana-JP")); }
		}
		/// <summary>ニックネーム</summary>
		public string NickName
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("nickname")); }
		}
		/// <summary>性別</summary>
		/// <remarks>未指定：返却しない、男性：male、女性：female</remarks>
		public string Gender
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("gender")); }
		}
		/// <summary>誕生日</summary>
		/// <remarks>yyy-mmdd形式</remarks>
		public string BirthDate
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("birthdate")); }
		}
		/// <summary>電話番号</summary>
		/// <remarks>ハイフン含む</remarks>
		public string PhoneNumber
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("phone_number")); }
		}
		/// <summary>住所情報</summary>
		private Dictionary<string, object> Address
		{
			get
			{
				var key = "address";
				if (GetResponseDataValue(key) == null) return new Dictionary<string, object>();
				return (Dictionary<string, object>)GetResponseDataValue(key);
			}
		}
		/// <summary>住所（全て）</summary>
		public string Formatted
		{
			get
			{
				var key = "formatted";
				if (this.Address.ContainsKey(key) == false) return "";
				return StringUtility.ToEmpty(this.Address[key]);
			}
		}
		/// <summary>住所（群市区以降の住所）</summary>
		public string StreetAddress
		{
			get
			{
				var key = "street_address";
				if (this.Address.ContainsKey(key) == false) return "";
				return StringUtility.ToEmpty(this.Address[key]);
			}
		}
		/// <summary>住所（群市区）</summary>
		public string Locality
		{
			get
			{
				var key = "locality";
				if (this.Address.ContainsKey(key) == false) return "";
				return StringUtility.ToEmpty(this.Address[key]);
			}
		}
		/// <summary>住所（都道府県）</summary>
		public string Region
		{
			get
			{
				var key = "region";
				if (this.Address.ContainsKey(key) == false) return "";
				return StringUtility.ToEmpty(this.Address[key]);
			}
		}
		/// <summary>郵便番号</summary>
		/// <remarks>ハイフン含む</remarks>
		public string PostalCode
		{
			get
			{
				var key = "postal_code";
				if (this.Address.ContainsKey(key) == false) return "";
				return StringUtility.ToEmpty(this.Address[key]);
			}
		}
		/// <summary>国名</summary>
		public string Country
		{
			get
			{
				var key = "country";
				if (this.Address.ContainsKey(key) == false) return "";
				return StringUtility.ToEmpty(this.Address[key]);
			}
		}
		#endregion
	}
}