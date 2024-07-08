/*
=========================================================================================================
  Module      : 楽天IDConnect楽天ID会員情報ヘルパークラス(RakutenIDConnectUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.App.Common.Auth.RakutenIDConnect.Helper
{
	/// <summary>
	/// 楽天IDConnect楽天ID会員情報ヘルパークラス
	/// </summary>
	public class RakutenIDConnectUser
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="action">アクション情報</param>
		public RakutenIDConnectUser(RakutenIDConnectActionInfo action)
		{
			this.Response = (action != null)
				? action.RakutenIdConnectUserInfoResponseData
				: new RakutenIDConnectUserInfoResponseData(string.Empty);
		}

		/// <summary>
		/// 分割した文字列を取得
		/// </summary>
		/// <param name="value">分割する文字列</param>
		/// <param name="separator">区切り文字</param>
		/// <param name="index">取得対象の順序数</param>
		/// <returns>分割された文字列</returns>
		private static string GetSplitPiece(string value, char separator ,int index)
		{
			var split = value.Split(separator);
			var result = (split.Length > index) ? split[index] : string.Empty;
			return result;
		}

		/// <summary>姓</summary>
		public string FamilyName
		{
			get
			{
				var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name1.type@@");
				var result = (type == Validator.STRTYPE_FULLWIDTH)
					? StringUtility.ToZenkaku(this.Response.FamilyName)
					: this.Response.FamilyNameKana;
				return result;
			}
		}
		/// <summary>名</summary>
		public string GivenName
		{
			get
			{
				var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name2.type@@");
				var result = (type == Validator.STRTYPE_FULLWIDTH)
					? StringUtility.ToZenkaku(this.Response.GivenName)
					: this.Response.GivenName;
				return result;
			}
		}
		/// <summary>姓（かな）</summary>
		public string FamilyNameKana
		{
			get
			{
				var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana.type@@");
				var result = (type == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? this.Response.FamilyNameKana
					: StringUtility.ToZenkakuHiragana(this.Response.FamilyNameKana);
				return result;
			}
		}
		/// <summary>名（かな）</summary>
		public string GivenNameKana {
			get
			{
				var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana2.type@@");
				var result = (type == Validator.STRTYPE_FULLWIDTH_KATAKANA)
					? this.Response.GivenNameKana
					: StringUtility.ToZenkakuHiragana(this.Response.GivenNameKana);
				return result;
			}
		}
		/// <summary>ニックネーム</summary>
		public string NickName
		{
			get { return this.Response.NickName; }
		}
		/// <summary>生年月日</summary>
		public DateTime? Birth
		{
			get
			{
				DateTime result;
				if (DateTime.TryParse(this.Response.BirthDate, out result) == false)
				{
					return null;
				}
				return result;
			}
		}
		/// <summary>生年月日（年）</summary>
		public string BirthYear
		{
			get
			{
				var result = StringUtility.ToDateString(this.Response.BirthDate, "yyyy");
				return result;
			}
		}
		/// <summary>生年月日（月）</summary>
		public string BirthMonth
		{
			get
			{
				var result = StringUtility.ToDateString(this.Response.BirthDate, "%M");
				return result;
			}
		}
		/// <summary>生年月日（日）</summary>
		public string BirthDay
		{
			get
			{
				var result = StringUtility.ToDateString(this.Response.BirthDate, "%d");
				return result;
			}
		}
		/// <summary>性別</summary>
		public string Gender
		{
			get
			{
				switch (this.Response.Gender)
				{
					case "male":
						return Constants.FLG_USER_SEX_MALE;

					case "female":
						return Constants.FLG_USER_SEX_FEMALE;

					default:
						return Constants.FLG_USER_SEX_UNKNOWN;
				}
			}
		}
		/// <summary>メールアドレス</summary>
		public string Email
		{
			get { return this.Response.Email; }
		}
		/// <summary>国</summary>
		public string Country
		{
			get
			{
				var result = (this.Response.Country == "JP") ? "Japan" : this.Response.Country;
				return result;
			}
		}
		/// <summary>郵便番号1</summary>
		public string PostalCode1
		{
			get
			{
				var result = GetSplitPiece(this.Response.PostalCode, '-', 0);
				return result;
			}
		}
		/// <summary>郵便番号2</summary>
		public string PostalCode2
		{
			get
			{
				var result = GetSplitPiece(this.Response.PostalCode, '-', 1);
				return result;
			}
		}
		/// <summary>住所（都道府県）</summary>
		public string Address1
		{
			get
			{
				var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.addr1.type@@");
				var result = (type == Validator.STRTYPE_FULLWIDTH)
					? StringUtility.ToZenkaku(this.Response.Region)
					: this.Response.Region;
				return result;
			}
		}
		/// <summary>住所（群市区）</summary>
		public string Address2
		{
			get
			{
				var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.addr2.type@@");
				var address = (string.IsNullOrEmpty(this.Response.Locality) == false)
					? this.Response.Locality
					: this.Response.StreetAddress;
				var result = (type == Validator.STRTYPE_FULLWIDTH) ? StringUtility.ToZenkaku(address) : address;
				return result;
			}
		}
		/// <summary>住所（群市区以降の住所）</summary>
		public string Address3
		{
			get
			{
				var type = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.addr3.type@@");
				var address = (string.IsNullOrEmpty(this.Response.Locality) == false)
					? this.Response.StreetAddress
					: string.Empty;
				var result = (type == Validator.STRTYPE_FULLWIDTH) ? StringUtility.ToZenkaku(address) : address;
				return result;
			}
		}
		/// <summary>電話番号1</summary>
		public string PhoneNumber1
		{
			get
			{
				var result = GetSplitPiece(this.Response.PhoneNumber, '-', 0);
				return result;
			}
		}
		/// <summary>電話番号2</summary>
		public string PhoneNumber2
		{
			get
			{
				var result = GetSplitPiece(this.Response.PhoneNumber, '-', 1);
				return result;
			}
		}
		/// <summary>電話番号3</summary>
		public string PhoneNumber3
		{
			get
			{
				var result = GetSplitPiece(this.Response.PhoneNumber, '-', 2);
				return result;
			}
		}
		/// <summary>電話番号</summary>
		public string PhoneNumber
		{
			get { return this.Response.PhoneNumber; }
		}
		/// <summary>レスポンスデータ</summary>
		public RakutenIDConnectUserInfoResponseData Response { get; set; }
	}
}
