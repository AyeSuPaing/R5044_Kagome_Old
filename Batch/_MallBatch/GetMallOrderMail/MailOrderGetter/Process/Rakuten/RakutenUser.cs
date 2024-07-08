/*
=========================================================================================================
  Module      : 楽天注文情報：注文者クラス (RakutenUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Mall.Rakuten;
using w2.App.Common.Mall.RakutenApi;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Rakuten
{
	/// <summary>
	/// 楽天注文情報：注文者クラス
	/// </summary>
	class RakutenUser : UserModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="rakutenOrderer">楽天注文者情報</param>
		/// <param name="carrierCode">利用端末</param>
		public RakutenUser(RakutenApiOrderer rakutenOrderer, int carrierCode)
		{
			this.UserKbn = GetUserKbn(carrierCode);
			this.Name = StringUtility.ToEmpty(rakutenOrderer.FamilyName) + StringUtility.ToEmpty(rakutenOrderer.FirstName);
			this.Name1 = StringUtility.ToEmpty(rakutenOrderer.FamilyName);
			this.Name2 = StringUtility.ToEmpty(rakutenOrderer.FirstName);
			SetNameKana(rakutenOrderer.FamilyNameKana, rakutenOrderer.FirstNameKana);
			SetMailAddr(rakutenOrderer.EmailAddress);
			this.Zip = string.Format(
				"{0}-{1}",
				StringUtility.ToEmpty(rakutenOrderer.ZipCode1),
				StringUtility.ToEmpty(rakutenOrderer.ZipCode2));
			this.Zip1 = StringUtility.ToEmpty(rakutenOrderer.ZipCode1);
			this.Zip2 = StringUtility.ToEmpty(rakutenOrderer.ZipCode2);
			SetAddr(rakutenOrderer.City, rakutenOrderer.SubAddress, rakutenOrderer.Prefecture);
			this.Tel1 = string.Format(
				"{0}-{1}-{2}",
				StringUtility.ToEmpty(rakutenOrderer.PhoneNumber1),
				StringUtility.ToEmpty(rakutenOrderer.PhoneNumber2),
				StringUtility.ToEmpty(rakutenOrderer.PhoneNumber3));
			this.Tel1_1 = StringUtility.ToEmpty(rakutenOrderer.PhoneNumber1);
			this.Tel1_2 = StringUtility.ToEmpty(rakutenOrderer.PhoneNumber2);
			this.Tel1_3 = StringUtility.ToEmpty(rakutenOrderer.PhoneNumber3);
			this.Sex = GetSex(rakutenOrderer.Sex);
			SetBirth(rakutenOrderer.BirthYear, rakutenOrderer.BirthMonth, rakutenOrderer.BirthDay);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// ユーザ区分取得
		/// </summary>
		/// <param name="carrierCode">利用端末</param>
		/// <returns>ユーザ区分</returns>
		private string GetUserKbn(int carrierCode)
		{
			switch ((Constants.RakutenApiCarrierCode)carrierCode)
			{
				case Constants.RakutenApiCarrierCode.Pc:
				case Constants.RakutenApiCarrierCode.TabletAndroid:
				case Constants.RakutenApiCarrierCode.TabletIpad:
				case Constants.RakutenApiCarrierCode.TabletOther:
				case Constants.RakutenApiCarrierCode.Other:
					return Constants.FLG_USER_USER_KBN_PC_USER;

				case Constants.RakutenApiCarrierCode.SmartphoneAndroid:
				case Constants.RakutenApiCarrierCode.SmartphoneIphone:
				case Constants.RakutenApiCarrierCode.SmartphoneOther:
					return Constants.FLG_USER_USER_KBN_SMARTPHONE_USER;

				case Constants.RakutenApiCarrierCode.MobileDocomo:
				case Constants.RakutenApiCarrierCode.MobileKddi:
				case Constants.RakutenApiCarrierCode.MobileSoftbank:
				case Constants.RakutenApiCarrierCode.MobileWillcom:
					return Constants.FLG_USER_USER_KBN_MOBILE_USER;

				default:
					return Constants.FLG_USER_USER_KBN_PC_USER;
			}
		}

		/// <summary>
		/// 氏名(かな)セット
		/// </summary>
		/// <param name="familyNameKana">姓カナ</param>
		/// <param name="firstNameKana">名カナ</param>
		private void SetNameKana(string familyNameKana, string firstNameKana)
		{
			// 氏名（かな）、あるいは、氏名（カナ）は設定値により自動切替
			if (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.name_kana.type@@") == Validator.STRTYPE_FULLWIDTH_KATAKANA)
			{
				this.NameKana = StringUtility.ToZenkakuKatakana(familyNameKana + firstNameKana);
				this.NameKana1 = StringUtility.ToZenkakuKatakana(familyNameKana);
				this.NameKana2 = StringUtility.ToZenkakuKatakana(firstNameKana);
			}
			else
			{
				this.NameKana = StringUtility.ToZenkakuHiragana(familyNameKana + firstNameKana);
				this.NameKana1 = StringUtility.ToZenkakuHiragana(familyNameKana);
				this.NameKana2 = StringUtility.ToZenkakuHiragana(firstNameKana);
			}
		}

		/// <summary>
		/// メールアドレスセット
		/// </summary>
		/// <param name="mailAddress">メールアドレス</param>
		private void SetMailAddr(string mailAddress)
		{
			// ユーザー区分によってメールアドレスの格納制御
			if (this.UserKbn == Constants.FLG_USER_USER_KBN_MOBILE_USER)
			{
				this.MailAddr = string.Empty;
				this.MailAddr2 = StringUtility.ToEmpty(mailAddress);
			}
			else
			{
				this.MailAddr = StringUtility.ToEmpty(mailAddress);
				this.MailAddr2 = string.Empty;
			}
		}

		/// <summary>
		/// 住所セット
		/// </summary>
		/// <param name="city">群市区</param>
		/// <param name="subAddress">それ以降の住所</param>
		/// <param name="prefecture">都道府県</param>
		private void SetAddr(string city, string subAddress, string prefecture)
		{
			var address = StringUtility.SplitAddress(city, subAddress);
			if (address == null)
			{
				Constants.DISP_ERROR_KBN = Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR;
				Constants.DISP_ERROR_MESSAGE = "注文者の住所情報の長さが規定値を超えました。";
				throw new RakutenApiCoopException("注文者の住所情報の長さが規定値を超えました");
			}

			// 海外注文の場合、Addr1とAddr2を変換
			if (Constants.RAKUTEN_OVERSEA_ORDER_MAIL_ENABLED && (Properties.Resources.sKen.Contains(prefecture) == false))
			{
				this.Addr
					= StringUtility.ToZenkaku(Constants.ORDER_ADDR1_RAKUTEN_OVERSEA
						+ prefecture
						+ city
						+ subAddress);
				this.Addr1 = Constants.ORDER_ADDR1_RAKUTEN_OVERSEA;
				this.Addr2 = StringUtility.ToEmpty(prefecture) + address[0];
			}
			else
			{
				this.Addr = StringUtility.ToZenkaku(prefecture+ city+ subAddress);
				this.Addr1 = StringUtility.ToEmpty(prefecture);
				this.Addr2 = address[0];
			}
			this.Addr3 = address[1];
			this.Addr4 = address[2];
		}

		/// <summary>
		/// 性別取得
		/// </summary>
		/// <param name="sex">性別</param>
		/// <returns>性別</returns>
		private string GetSex(string sex)
		{
			if (sex == null) return Constants.FLG_USER_SEX_UNKNOWN;

			if (sex.Contains("男"))
			{
				return Constants.FLG_USER_SEX_MALE;
			}
			else if (sex.Contains("女"))
			{
				return Constants.FLG_USER_SEX_FEMALE;
			}
			else
			{
				return Constants.FLG_USER_SEX_UNKNOWN;
			}
		}

		/// <summary>
		/// 誕生日セット
		/// </summary>
		/// <param name="birthYear">誕生日(年)</param>
		/// <param name="birthMonth">誕生日(月)</param>
		/// <param name="birthDay">誕生日(日)</param>
		private void SetBirth(string birthYear, string birthMonth, string birthDay)
		{
			try
			{
				this.Birth = new DateTime(int.Parse(birthYear), int.Parse(birthMonth), int.Parse(birthDay));
			}
			catch (Exception)
			{
				this.Birth = null;
			}
			this.BirthYear = StringUtility.ToEmpty(birthYear);
			this.BirthMonth = StringUtility.ToEmpty(birthMonth);
			this.BirthDay = StringUtility.ToEmpty(birthDay);
		}
		#endregion
	}
}
