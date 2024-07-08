/*
=========================================================================================================
  Module      : カート注文者情報クラス(CartOwner.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.Common.Util;
using w2.Domain.User;
using w2.Domain.Order;
using w2.Domain.User.Helper;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カート注文者情報クラス
	/// </summary>
	[Serializable]
	public class CartOwner
	{
		public const string FIELD_ORDEROWNER_OWNER_ZIP_1 = Constants.FIELD_ORDEROWNER_OWNER_ZIP + "_1";
		public const string FIELD_ORDEROWNER_OWNER_ZIP_2 = Constants.FIELD_ORDEROWNER_OWNER_ZIP + "_2";
		public const string FIELD_ORDEROWNER_OWNER_TEL1_1 = Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_1";
		public const string FIELD_ORDEROWNER_OWNER_TEL1_2 = Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_2";
		public const string FIELD_ORDEROWNER_OWNER_TEL1_3 = Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_3";
		public const string FIELD_ORDEROWNER_OWNER_TEL2_1 = Constants.FIELD_ORDEROWNER_OWNER_TEL2 + "_1";
		public const string FIELD_ORDEROWNER_OWNER_TEL2_2 = Constants.FIELD_ORDEROWNER_OWNER_TEL2 + "_2";
		public const string FIELD_ORDEROWNER_OWNER_TEL2_3 = Constants.FIELD_ORDEROWNER_OWNER_TEL2 + "_3";
		public const string FIELD_ORDEROWNER_OWNER_BIRTH_YEAR = Constants.FIELD_ORDEROWNER_OWNER_BIRTH + "_year";
		public const string FIELD_ORDEROWNER_OWNER_BIRTH_MONTH = Constants.FIELD_ORDEROWNER_OWNER_BIRTH + "_month";
		public const string FIELD_ORDEROWNER_OWNER_BIRTH_DAY = Constants.FIELD_ORDEROWNER_OWNER_BIRTH + "_day";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CartOwner()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		public CartOwner(UserModel user)
		{
			this.OwnerKbn = user.UserKbn;
			this.Name = user.Name;
			this.Name1 = user.Name1;
			this.Name2 = user.Name2;
			this.NameKana = user.NameKana;
			this.NameKana1 = user.NameKana1;
			this.NameKana2 = user.NameKana2;
			this.MailAddr = user.MailAddr;
			this.MailAddr2 = user.MailAddr2;
			this.Zip1 = user.Zip1;
			this.Zip2 = user.Zip2;
			this.Addr1 = user.Addr1;
			this.Addr2 = user.Addr2;
			this.Addr3 = user.Addr3;
			this.Addr4 = user.Addr4;
			this.CompanyName = user.CompanyName;
			this.CompanyPostName = user.CompanyPostName;
			this.Tel1_1 = user.Tel1_1;
			this.Tel1_2 = user.Tel1_2;
			this.Tel1_3 = user.Tel1_3;
			this.Tel2_1 = user.Tel2_1;
			this.Tel2_2 = user.Tel2_2;
			this.Tel2_3 = user.Tel2_3;
			this.MailFlg = (user.MailFlg == Constants.FLG_USER_MAILFLG_OK);
			this.Sex = user.Sex;
			this.Birth = user.Birth;
			this.AccessCountryIsoCode = user.AccessCountryIsoCode;
			this.DispLanguageCode = user.DispLanguageCode;
			this.DispLanguageLocaleId = user.DispLanguageLocaleId;
			this.DispCurrencyCode = user.DispCurrencyCode;
			this.DispCurrencyLocaleId = user.DispCurrencyLocaleId;
			this.AddrCountryIsoCode = user.AddrCountryIsoCode;
			this.AddrCountryName = user.AddrCountryName;
			this.Zip = user.Zip;
			this.Tel1 = user.Tel1;
			this.Tel2 = user.Tel2;
			this.Addr5 = user.Addr5;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ownerKbn">注文者区分</param>
		/// <param name="name">氏名</param>
		/// <param name="name1">氏名1</param>
		/// <param name="name2">氏名2</param>
		/// <param name="nameKana">氏名かな</param>
		/// <param name="nameKana1">氏名かな1</param>
		/// <param name="nameKana2">氏名かな2</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="mailAddr2">メールアドレス2</param>
		/// <param name="zip">郵便番号</param>
		/// <param name="zip1">郵便番号1</param>
		/// <param name="zip2">郵便番号2</param>
		/// <param name="addr1">住所1</param>
		/// <param name="addr2">住所2</param>
		/// <param name="addr3">住所3</param>
		/// <param name="addr4">住所4</param>
		/// <param name="addr5">住所5</param>
		/// <param name="addrCountryIsoCode">住所国ISOコード</param>
		/// <param name="addrCountryName">住所国名</param>
		/// <param name="companyName">企業名</param>
		/// <param name="companyPostName">部署名</param>
		/// <param name="tel1">電話番号1</param>
		/// <param name="tel1_1">電話番号1-1</param>
		/// <param name="tel1_2">電話番号1-2</param>
		/// <param name="tel1_3">電話番号1-3</param>
		/// <param name="tel2">電話番号2</param>
		/// <param name="tel2_1">電話番号2-1</param>
		/// <param name="tel2_2">電話番号2-2</param>
		/// <param name="tel2_3">電話番号2-3</param>
		/// <param name="mailFlg">メール配信フラグ</param>
		/// <param name="sex">性別</param>
		/// <param name="birth">生年月日</param>
		///	<param name="accessCountryIsoCode">アクセス国ISOコード</param>
		///	<param name="dispLanguageCode">表示言語コード</param>
		///	<param name="dispLanguageLocaleId">表示言語ロケールID</param>
		///	<param name="dispCurrencyCode">表示通貨コード</param>
		///	<param name="dispCurrencyLocaleId">表示通貨ロケールID</param>
		public CartOwner(
			string ownerKbn,
			string name,
			string name1,
			string name2,
			string nameKana,
			string nameKana1,
			string nameKana2,
			string mailAddr,
			string mailAddr2,
			string zip,
			string zip1,
			string zip2,
			string addr1,
			string addr2,
			string addr3,
			string addr4,
			string addr5,
			string addrCountryIsoCode,
			string addrCountryName,
			string companyName,
			string companyPostName,
			string tel1,
			string tel1_1,
			string tel1_2,
			string tel1_3,
			string tel2,
			string tel2_1,
			string tel2_2,
			string tel2_3,
			bool mailFlg,
			string sex,
			DateTime? birth,
			string accessCountryIsoCode,
			string dispLanguageCode,
			string dispLanguageLocaleId,
			string dispCurrencyCode,
			string dispCurrencyLocaleId)
			: this()
		{
			UpdateOrderOwner(
				ownerKbn,
				name,
				name1,
				name2,
				nameKana,
				nameKana1,
				nameKana2,
				mailAddr,
				mailAddr2,
				zip,
				zip1,
				zip2,
				addr1,
				addr2,
				addr3,
				addr4,
				addr5,
				addrCountryIsoCode,
				addrCountryName,
				companyName,
				companyPostName,
				tel1,
				tel1_1,
				tel1_2,
				tel1_3,
				tel2,
				tel2_1,
				tel2_2,
				tel2_3,
				mailFlg,
				sex,
				birth,
				accessCountryIsoCode,
				dispLanguageCode,
				dispLanguageLocaleId,
				dispCurrencyCode,
				dispCurrencyLocaleId);
		}

		/// <summary>
		/// 注文者情報更新
		/// </summary>
		/// <param name="ownerKbn">注文者区分</param>
		/// <param name="name">氏名</param>
		/// <param name="name1">氏名1</param>
		/// <param name="name2">氏名2</param>
		/// <param name="nameKana">氏名かな</param>
		/// <param name="nameKana1">氏名かな1</param>
		/// <param name="nameKana2">氏名かな2</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="mailAddr2">メールアドレス2</param>
		/// <param name="zip">郵便番号</param>
		/// <param name="zip1">郵便番号1</param>
		/// <param name="zip2">郵便番号2</param>
		/// <param name="addr1">住所1</param>
		/// <param name="addr2">住所2</param>
		/// <param name="addr3">住所3</param>
		/// <param name="addr4">住所4</param>
		/// <param name="addr5">住所5</param>
		/// <param name="addrCountryIsoCode">住所国ISOコード</param>
		/// <param name="addrCountryName">住所国名</param>
		/// <param name="companyName">企業名</param>
		/// <param name="companyPostName">部署名</param>
		/// <param name="tel1">電話番号1</param>
		/// <param name="tel1_1">電話番号1-1</param>
		/// <param name="tel1_2">電話番号1-2</param>
		/// <param name="tel1_3">電話番号1-3</param>
		/// <param name="tel2">電話番号2</param>
		/// <param name="tel2_1">電話番号2-1</param>
		/// <param name="tel2_2">電話番号2-2</param>
		/// <param name="tel2_3">電話番号2-3</param>
		/// <param name="mailFlg">メール配信フラグ</param>
		/// <param name="sex">性別</param>
		/// <param name="birth">生年月日</param>
		///	<param name="accessCountryIsoCode">国ISOコード</param>
		///	<param name="dispLanguageCode">言語コード</param>
		///	<param name="dispLanguageLocaleId">言語ロケールID</param>
		///	<param name="dispCurrencyCode">通貨コード</param>
		///	<param name="dispCurrencyLocaleId">通貨ロケールID</param>
		public void UpdateOrderOwner(
			string ownerKbn,
			string name,
			string name1,
			string name2,
			string nameKana,
			string nameKana1,
			string nameKana2,
			string mailAddr,
			string mailAddr2,
			string zip,
			string zip1,
			string zip2,
			string addr1,
			string addr2,
			string addr3,
			string addr4,
			string addr5,
			string addrCountryIsoCode,
			string addrCountryName,
			string companyName,
			string companyPostName,
			string tel1,
			string tel1_1,
			string tel1_2,
			string tel1_3,
			string tel2,
			string tel2_1,
			string tel2_2,
			string tel2_3,
			bool mailFlg,
			string sex,
			DateTime? birth,
			string accessCountryIsoCode,
			string dispLanguageCode,
			string dispLanguageLocaleId,
			string dispCurrencyCode,
			string dispCurrencyLocaleId)
		{
			this.OwnerKbn = ownerKbn;
			this.Name = name;
			this.Name1 = name1;
			this.Name2 = name2;
			this.NameKana = nameKana;
			this.NameKana1 = nameKana1;
			this.NameKana2 = nameKana2;
			this.MailAddr = mailAddr;
			this.MailAddr2 = mailAddr2;
			this.Zip = zip;
			this.Zip1 = zip1;
			this.Zip2 = zip2;
			this.Addr1 = addr1;
			this.Addr2 = addr2;
			this.Addr3 = addr3;
			this.Addr4 = addr4;
			this.Addr5 = addr5;
			this.AddrCountryIsoCode = addrCountryIsoCode;
			this.AddrCountryName = addrCountryName;
			this.CompanyName = companyName;
			this.CompanyPostName = companyPostName;
			this.Tel1 = tel1;
			this.Tel1_1 = tel1_1;
			this.Tel1_2 = tel1_2;
			this.Tel1_3 = tel1_3;
			this.Tel2 = tel2;
			this.Tel2_1 = tel2_1;
			this.Tel2_2 = tel2_2;
			this.Tel2_3 = tel2_3;
			this.MailFlg = mailFlg;
			this.Sex = sex;
			this.Birth = birth;
			this.AccessCountryIsoCode = accessCountryIsoCode;
			this.DispLanguageCode = dispLanguageCode;
			this.DispLanguageLocaleId = dispLanguageLocaleId;
			this.DispCurrencyCode = dispCurrencyCode;
			this.DispCurrencyLocaleId = dispCurrencyLocaleId;
		}

		/// <summary>
		/// 注文者情報をDictionaryで取得
		/// </summary>
		/// <returns>注文者情報</returns>
		public Dictionary<string, object> GetDictionary()
		{
			var value = new Dictionary<string, object>();
			//value.Add(Constants.FIELD_ORDEROWNER_ORDER_ID, cartObject.OrderId);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, this.OwnerKbn);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN + "_text", ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_KBN, this.OwnerKbn));
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME, this.Name);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME1, this.Name1);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME2, this.Name2);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA, this.NameKana);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1, this.NameKana1);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2, this.NameKana2);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, this.MailAddr);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2, this.MailAddr2);
			value.Add(FIELD_ORDEROWNER_OWNER_ZIP_1, this.Zip1);
			value.Add(FIELD_ORDEROWNER_OWNER_ZIP_2, this.Zip2);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP, this.Zip);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, this.Addr1);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, this.Addr2);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR3, this.Addr3);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR4, this.Addr4);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR5, this.Addr5);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME, this.AddrCountryName);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME, this.CompanyName);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME, this.CompanyPostName);
			//value.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_EXECTIVE_NAME, "");
			value.Add(FIELD_ORDEROWNER_OWNER_TEL1_1, this.Tel1_1);
			value.Add(FIELD_ORDEROWNER_OWNER_TEL1_2, this.Tel1_2);
			value.Add(FIELD_ORDEROWNER_OWNER_TEL1_3, this.Tel1_3);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1, this.Tel1);
			value.Add(FIELD_ORDEROWNER_OWNER_TEL2_1, this.Tel2_1);
			value.Add(FIELD_ORDEROWNER_OWNER_TEL2_2, this.Tel2_2);
			value.Add(FIELD_ORDEROWNER_OWNER_TEL2_3, this.Tel2_3);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL2, this.Tel2);
			//value.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL3, "");
			//value.Add(Constants.FIELD_ORDEROWNER_OWNER_FAX, "");
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_SEX, this.Sex);
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_SEX + "_text", ValueText.GetValueText(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_SEX, this.Sex));
			value.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH, this.Birth);
			value.Add(Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE, this.AccessCountryIsoCode);
			value.Add(Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE, this.DispLanguageCode);
			value.Add(Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID, this.DispLanguageLocaleId);
			value.Add(Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE, this.DispCurrencyCode);
			value.Add(Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID, this.DispCurrencyLocaleId);

			return value;
		}

		/// <summary>
		/// リージョンデータの更新
		/// </summary>
		/// <param name="model">更新内容を含むリージョンモデル</param>
		public void UpdateRegion(RegionModel model)
		{
			this.AccessCountryIsoCode = model.CountryIsoCode;
			this.DispLanguageCode = model.LanguageCode;
			this.DispLanguageLocaleId = model.LanguageLocaleId;
			this.DispCurrencyCode = model.CurrencyCode;
			this.DispCurrencyLocaleId = model.CurrencyLocaleId;
		}

		/// <summary>
		/// 注文者情報モデル生成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文者情報</returns>
		public OrderOwnerModel CreateModel(string orderId)
		{
			var owner = new OrderOwnerModel
			{
				OrderId = orderId,
				OwnerKbn = this.OwnerKbn,
				OwnerName = this.Name,
				OwnerName1 = this.Name1,
				OwnerName2 = this.Name2,
				OwnerNameKana = this.NameKana,
				OwnerNameKana1 = this.NameKana1,
				OwnerNameKana2 = this.NameKana2,
				OwnerMailAddr = this.MailAddr,
				OwnerMailAddr2 = this.MailAddr2,
				OwnerZip = this.Zip,
				OwnerAddr1 = this.Addr1,
				OwnerAddr2 = this.Addr2,
				OwnerAddr3 = this.Addr3,
				OwnerAddr4 = this.Addr4,
				OwnerCompanyName = this.CompanyName,
				OwnerCompanyPostName = this.CompanyPostName,
				OwnerTel1 = this.Tel1,
				OwnerSex = this.Sex,
				OwnerBirth = this.Birth,
				DateChanged = DateTime.Now,
				DateCreated = DateTime.Now,
				AccessCountryIsoCode = this.AccessCountryIsoCode,
				DispLanguageCode = this.DispLanguageCode,
				DispLanguageLocaleId = this.DispLanguageLocaleId,
				DispCurrencyCode = this.DispCurrencyCode,
				DispCurrencyLocaleId = this.DispCurrencyLocaleId
			};

			return owner;
		}

		/// <summary>
		/// 住所項目結合
		/// </summary>
		/// <returns>結合した住所</returns>
		public string ConcatenateAddress()
		{
			string address = string.Empty;
			if (this.IsAddrJp)
			{
				address = ConcatenateAddressWithoutCountryName() + this.AddrCountryName;
			}
			else
			{
				address = this.Addr2
					+ ((string.IsNullOrEmpty(this.Addr3) == false) ? " " : "")
					+ this.Addr3
					+ " "
					+ this.Addr4
					+ ((string.IsNullOrEmpty(this.Addr5) == false) ? " " : "")
					+ this.Addr5
					+ " "
					+ this.AddrCountryName;
			}
			return address;
		}

		/// <summary>
		/// 住所項目結合（国名は含めない）
		/// </summary>
		/// <returns>結合した住所</returns>
		public string ConcatenateAddressWithoutCountryName()
		{
			var address = AddressHelper.ConcatenateAddressWithoutCountryName(
				this.Addr1,
				this.Addr2,
				this.Addr3,
				this.Addr4);

			return address;
		}

		/// <summary>注文者区分</summary>
		public string OwnerKbn { get; set; }
		/// <summary>注文者氏名</summary>
		public string Name { get; set; }
		/// <summary>注文者氏名1</summary>
		public string Name1 { get; set; }
		/// <summary>注文者氏名2</summary>
		public string Name2 { get; set; }
		/// <summary>注文者氏名かな</summary>
		public string NameKana { get; set; }
		/// <summary>注文者氏名かな1</summary>
		public string NameKana1 { get; set; }
		/// <summary>注文者氏名かな2</summary>
		public string NameKana2 { get; set; }
		/// <summary>メールアドレス</summary>
		public string MailAddr { get; set; }
		/// <summary>モバイルメールアドレス</summary>
		public string MailAddr2 { get; set; }
		/// <summary>郵便番号</summary>
		public string Zip
		{
			get
			{
				if ((string.IsNullOrEmpty(this.Zip1) == false) && (string.IsNullOrEmpty(this.Zip2) == false))
				{
					return this.Zip1 + "-" + this.Zip2;
				}
				return StringUtility.ToEmpty(m_zip);
			}
			private set { m_zip = value; }
		}
		private string m_zip;
		/// <summary>郵便番号1</summary>
		public string Zip1 { get; set; }
		/// <summary>郵便番号2</summary>
		public string Zip2 { get; set; }
		/// <summary>都道府県</summary>
		public string Addr1 { get; set; }
		/// <summary>市区町村</summary>
		public string Addr2 { get; set; }
		/// <summary>番地</summary>
		public string Addr3 { get; set; }
		/// <summary>ビル・マンション名</summary>
		public string Addr4 { get; set; }
		/// <summary>州</summary>
		public string Addr5 { get; set; }
		/// <summary>住所国名</summary>
		public string AddrCountryName { get; set; }
		/// <summary>住所国ISOコード</summary>
		public string AddrCountryIsoCode { get; set; }
		/// <summary>企業名</summary>
		public string CompanyName { get; set; }
		/// <summary>部署名</summary>
		public string CompanyPostName { get; set; }
		/// <summary>電話番号</summary>
		public string Tel1
		{
			get
			{
				if ((string.IsNullOrEmpty(this.Tel1_1) == false)
					&& (string.IsNullOrEmpty(this.Tel1_2) == false)
					&& (string.IsNullOrEmpty(this.Tel1_3) == false))
				{
					return UserService.CreatePhoneNo(this.Tel1_1, this.Tel1_2, this.Tel1_3);
				}
				return StringUtility.ToEmpty(m_tel1);
			}
			private set { m_tel1 = value; }
		}
		private string m_tel1;
		/// <summary>電話番号1-1</summary>
		public string Tel1_1 { get; set; }
		/// <summary>電話番号1-2</summary>
		public string Tel1_2 { get; set; }
		/// <summary>電話番号1-3</summary>
		public string Tel1_3 { get; set; }
		/// <summary>Order owner tel2</summary>
		public string Tel2
		{
			get
			{
				if ((string.IsNullOrEmpty(this.Tel2_1) == false)
					&& (string.IsNullOrEmpty(this.Tel2_2) == false)
					&& (string.IsNullOrEmpty(this.Tel2_3) == false))
				{
					return UserService.CreatePhoneNo(this.Tel2_1, this.Tel2_2, this.Tel2_3);
				}
				return StringUtility.ToEmpty(m_tel2);
			}
			private set { m_tel2 = value; }
		}
		private string m_tel2;
		/// <summary>Order owner tel2_1</summary>
		public string Tel2_1 { get; set; }
		/// <summary>Order owner tel2_2</summary>
		public string Tel2_2 { get; set; }
		/// <summary>Order owner tel2_3</summary>
		public string Tel2_3 { get; set; }
		/// <summary>お知らせメールの配信希望</summary>
		public bool MailFlg { get; set; }
		/// <summary>性別</summary>
		public string Sex { get; set; }
		/// <summary>生年月日</summary>
		public DateTime? Birth { get; set; }
		/// <summary>アクセス国ISOコード</summary>
		public string AccessCountryIsoCode { get; set; }
		/// <summary>表示言語コード</summary>
		public string DispLanguageCode { get; set; }
		/// <summary>表示言語ロケールID</summary>
		public string DispLanguageLocaleId { get; set; }
		/// <summary>表示通貨コード</summary>
		public string DispCurrencyCode { get; set; }
		/// <summary>表示通貨ロケールID</summary>
		public string DispCurrencyLocaleId { get; set; }
		/// <summary>生年月日（年）</summary>
		public string BirthYear
		{
			get { return (this.Birth.HasValue) ? this.Birth.Value.Year.ToString() : ""; }
		}
		/// <summary>生年月日（月）</summary>
		public string BirthMonth
		{
			get { return (this.Birth.HasValue) ? this.Birth.Value.Month.ToString() : ""; }
		}
		/// <summary>生年月日（日）</summary>
		public string BirthDay
		{
			get { return (this.Birth.HasValue) ? this.Birth.Value.Day.ToString() : ""; }
		}
		/// <summary>住所は日本か</summary>
		public bool IsAddrJp
		{
			get { return GlobalAddressUtil.IsCountryJp(this.AddrCountryIsoCode); }
		}
		/// <summary>住所は台湾か</summary>
		public bool IsAddrTw
		{
			get { return GlobalAddressUtil.IsCountryTw(this.AddrCountryIsoCode); }
		}
		/// <summary>住所はアメリカか</summary>
		public bool IsAddrUs
		{
			get { return GlobalAddressUtil.IsCountryUs(this.AddrCountryIsoCode); }
		}
		/// <summary>Status request delivery of notification mail</summary>
		public string StatusRequestDeliveryOfNotificationMail
		{
			get
			{
				var result = this.MailFlg
					? Constants.FLG_USER_MAILFLG_OK
					: Constants.FLG_USER_MAILFLG_NG;
				return result;
			}
		}
	}
}
