/*
=========================================================================================================
  Module      : Amazon住所パーサー(AmazonAddressParser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Amazon.Helper;
using w2.Common.Util;
using w2.Domain.Zipcode;

namespace w2.App.Common.Amazon
{
	/// <summary>
	/// Amazonの住所情報を解析、パースするクラス
	/// </summary>
	public class AmazonAddressParser
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="input"></param>
		private AmazonAddressParser(AmazonAddressInput input)
		{
			this.Input = input;
		}

		/// <summary>
		/// パース
		/// </summary>
		/// <param name="input">Amazon入力住所情報</param>
		/// <returns>Amazon住所情報モデル</returns>
		public static AmazonAddressModel Parse(AmazonAddressInput input)
		{
			var parser = new AmazonAddressParser(input);
			return parser.CreateModel();
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>Amazon住所情報モデル</returns>
		private AmazonAddressModel CreateModel()
		{
			ParseName();
			ParseMailAddr();
			ParseZip();
			ParseAddr();
			ParseTel();

			// モデル返却
			return new AmazonAddressModel
			{
				Name = this.Name,
				Name1 = this.Name1,
				Name2 = this.Name2,
				NameKana = this.NameKana,
				NameKana1 = this.NameKana1,
				NameKana2 = this.NameKana2,
				MailAddr = this.MailAddr,
				MailAddr2 = this.MailAddr2,
				Zip = this.Zip,
				Zip1 = this.Zip1,
				Zip2 = this.Zip2,
				Addr = this.Addr,
				Addr1 = this.Addr1,
				Addr2 = this.Addr2,
				Addr3 = this.Addr3,
				Addr4 = this.Addr4,
				Tel = this.Tel,
				Tel1 = this.Tel1,
				Tel2 = this.Tel2,
				Tel3 = this.Tel3,
				CountryCode = this.CountryCode,
			};
		}

		/// <summary>
		/// 氏名パース
		/// </summary>
		private void ParseName()
		{
			this.Name = StringUtility.ToZenkaku(string.Join("", StringUtility.ToEmpty(this.Input.BuyerShippingName).Take(40))).Replace('　', ' ');
			var nameArray = this.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (nameArray.Length >= 2)
			{
				this.Name1 = nameArray[0];
				this.Name2 = nameArray[1];
			}
			else
			{
				this.Name1 = this.Name.Substring(0, nameArray[0].Length / 2);
				this.Name2 = this.Name.Substring(nameArray[0].Length / 2);
			}

			this.NameKana = Constants.PAYMENT_AMAZON_NAMEKANA1 + Constants.PAYMENT_AMAZON_NAMEKANA2;
			this.NameKana1 = Constants.PAYMENT_AMAZON_NAMEKANA1;
			this.NameKana2 = Constants.PAYMENT_AMAZON_NAMEKANA2;
		}

		/// <summary>
		/// メールアドレスパース
		/// </summary>
		private void ParseMailAddr()
		{
			this.MailAddr = StringUtility.ToHankaku(StringUtility.ToEmpty(this.Input.Email));
			this.MailAddr2 = "";
		}

		/// <summary>
		/// 郵便番号パース
		/// </summary>
		private void ParseZip()
		{
			var amazonPostalCode = StringUtility.ToHankaku(StringUtility.ToEmpty(this.Input.PostalCode));
			if (amazonPostalCode.Contains("-"))
			{
				var zipArray = amazonPostalCode.Split('-');
				this.Zip1 = zipArray[0];
				this.Zip2 = zipArray[1];
			}
			else
			{
				this.Zip1 = string.Join("", amazonPostalCode.Take(3));
				this.Zip2 = string.Join("", amazonPostalCode.Skip(3).Take(4));
			}
			this.Zip = string.Format("{0}-{1}", this.Zip1, this.Zip2);
		}

		/// <summary>
		/// 住所情報パース
		/// </summary>
		private void ParseAddr()
		{
			// 郵便番号から住所情報取得
			var zipcodeModels = new ZipcodeService().GetByZipcode(this.Zip1 + this.Zip2);

			// AddressLine1が市区町村から始まるかどうか
			var isAddressLine1StartAtCity = false;
			var addressLine1 = this.Input.AddressLine1.Replace("　", " ").Replace(" ", string.Empty);

			var townFromZip = "";
			if (zipcodeModels.Length > 0)
			{
				// 市区町村取得
				townFromZip = zipcodeModels[0].City + zipcodeModels[0].Town;

				// Amazonから取得した住所情報のAddressLine1の開始が市区町村と一致するかチェック
				var regex = new Regex("^" + townFromZip);
				isAddressLine1StartAtCity = regex.Match(addressLine1).Success;
			}

			// Amazon住所情報から都道府県を除く住所情報取得
			// AddressLine1の開始が市区町村と一致する場合は市区町村も除く
			var amazonAddrString = StringUtility.ToZenkaku(string.Format(
				"{0}{1}{2}",
				StringUtility.ToEmpty((isAddressLine1StartAtCity)
					? addressLine1.Remove(0, townFromZip.Length)
					: addressLine1),
				StringUtility.ToEmpty(this.Input.AddressLine2),
				StringUtility.ToEmpty(this.Input.AddressLine3)));

			// 1～50文字目
			var amazonAddrString1 = string.Join("", amazonAddrString.Take(50)).Replace("-", "ー");
			// 51～100文字目
			var amazonAddrString2 = string.Join("", amazonAddrString.Skip(50).Take(50)).Replace("-", "ー");
			// 101～150文字目
			var amazonAddrString3 = string.Join("", amazonAddrString.Skip(100).Take(50)).Replace("-", "ー");

			this.Addr1 = StringUtility.ToZenkaku(StringUtility.ToEmpty(this.Input.StateOrRegion));
			if (isAddressLine1StartAtCity)
			{
				this.Addr2 = StringUtility.ToZenkaku(townFromZip);
				this.Addr3 = amazonAddrString1;
				this.Addr4 = amazonAddrString2;
			}
			else
			{
				this.Addr2 = amazonAddrString1;
				this.Addr3 = amazonAddrString2;
				this.Addr4 = amazonAddrString3;
			}
			this.Addr = string.Format("{0}{1}{2} {3}", this.Addr1, this.Addr2, this.Addr3, this.Addr4);
			this.CountryCode = this.Input.CountryCode;
		}

		/// <summary>
		/// 電話番号パース
		/// </summary>
		private void ParseTel()
		{
			var amazonPhone = StringUtility.ToHankaku(StringUtility.ToEmpty(this.Input.Phone));
			if (amazonPhone.Contains("-"))
			{
				var telArray = amazonPhone.Split('-');
				if (telArray.Length == 3)
				{
					this.Tel1 = telArray[0];
					this.Tel2 = telArray[1];
					this.Tel3 = telArray[2];
					this.Tel = amazonPhone;
					return;
				}
				amazonPhone = amazonPhone.Replace("-", "");
			}

			if (amazonPhone.Length == 11)
			{
				this.Tel1 = string.Join("", amazonPhone.Take(3));
				this.Tel2 = string.Join("", amazonPhone.Skip(3).Take(4));
				this.Tel3 = string.Join("", amazonPhone.Skip(7).Take(4));
			}
			else
			{
				this.Tel1 = string.Join("", amazonPhone.Take(2));
				this.Tel2 = string.Join("", amazonPhone.Skip(2).Take(4));
				this.Tel3 = string.Join("", amazonPhone.Skip(6).Take(4));
			}
			this.Tel = string.Format("{0}-{1}-{2}", this.Tel1, this.Tel2, this.Tel3);
		}

		#region プロパティ
		/// <summary>住所入力情報</summary>
		private AmazonAddressInput Input { get; set; }
		/// <summary>姓名</summary>
		private string Name { get; set; }
		/// <summary>姓</summary>
		private string Name1 { get; set; }
		/// <summary>名</summary>
		private string Name2 { get; set; }
		/// <summary>姓名（かな</summary>
		private string NameKana { get; set; }
		/// <summary>姓（かな）</summary>
		private string NameKana1 { get; set; }
		/// <summary>名（かな）</summary>
		private string NameKana2 { get; set; }
		/// <summary>メールアドレス</summary>
		private string MailAddr { get; set; }
		/// <summary>メールアドレス2</summary>
		private string MailAddr2 { get; set; }
		/// <summary>郵便番号</summary>
		private string Zip { get; set; }
		/// <summary>郵便番号1</summary>
		private string Zip1 { get; set; }
		/// <summary>郵便番号2</summary>
		private string Zip2 { get; set; }
		/// <summary>住所</summary>
		private string Addr { get; set; }
		/// <summary>住所1</summary>
		private string Addr1 { get; set; }
		/// <summary>住所2</summary>
		private string Addr2 { get; set; }
		/// <summary>住所3</summary>
		private string Addr3 { get; set; }
		/// <summary>住所4</summary>
		private string Addr4 { get; set; }
		/// <summary>国名コード</summary>
		private string CountryCode { get; set; }
		/// <summary>電話番号</summary>
		private string Tel { get; set; }
		/// <summary>電話番号1</summary>
		private string Tel1 { get; set; }
		/// <summary>電話番号2</summary>
		private string Tel2 { get; set; }
		/// <summary>電話番号3</summary>
		private string Tel3 { get; set; }
		#endregion
	}
}