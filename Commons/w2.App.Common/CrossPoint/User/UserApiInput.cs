/*
=========================================================================================================
  Module      : CrossPoint API ユーザー入力モデル (UserApiInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.CrossPoint.User.Helper;
using w2.Common.Util;
using w2.Domain.MemberRank;
using w2.Domain.User;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// ユーザー入力モデル
	/// </summary>
	public class UserApiInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserApiInput()
		{
			this.NetShopMemberId = string.Empty;
			this.LastName = string.Empty;
			this.FirstName = string.Empty;
			this.LastNamePhonetic = string.Empty;
			this.FirstNamePhonetic = string.Empty;
			this.Sex = string.Empty;
			this.Birthday = string.Empty;
			this.Postcode = string.Empty;
			this.PrefName = string.Empty;
			this.City = string.Empty;
			this.Town = string.Empty;
			this.Address = string.Empty;
			this.Building = string.Empty;
			this.Tel = string.Empty;
			this.MbTel = string.Empty;
			this.PcMail = string.Empty;
			this.MbMail = string.Empty;
			this.PostcardDmUnnecessaryFlg = string.Empty;
			this.EmailDmUnnecessaryFlg = string.Empty;
			this.Password = string.Empty;
			this.Remarks1 = string.Empty;
			this.IsDeleted = false;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ユーザーモデル</param>
		public UserApiInput(UserModel model)
			: this()
		{
			this.NetShopMemberId = model.UserId;
			this.LastName = model.Name1;
			this.FirstName = model.Name2;
			this.LastNamePhonetic = model.NameKana1;
			this.FirstNamePhonetic = model.NameKana2;
			this.MemberRankName = (string.IsNullOrEmpty(model.MemberRankId) == false)
				? MemberRankService.Get(model.MemberRankId).MemberRankName
				: string.Empty;
			this.Sex = UserConversion.GetApiSex(model.Sex);
			this.Birthday = StringUtility.ToDateString(model.Birth, "yyyy/MM/dd");
			this.Postcode = string.Format("{0}{1}", model.Zip1, model.Zip2);
			this.PrefName = model.Addr1;
			this.City = model.Addr2;
			this.Address = model.Addr3;
			this.Building = model.Addr4;
			this.Tel = string.Format("{0}{1}{2}", model.Tel1_1, model.Tel1_2, model.Tel1_3);
			this.MbTel = string.Format("{0}{1}{2}", model.Tel2_1, model.Tel2_2, model.Tel2_3);
			this.PcMail = model.MailAddr;
			this.MbMail = model.MailAddr2;
			this.EmailDmUnnecessaryFlg = UserConversion.GetApiMailFlg(model.MailFlg);
			this.Password = UserConversion.GetHash(model.PasswordDecrypted).ToLower();
			this.Remarks1 = model.UserMemo;
			this.UserExtend = model.UserExtend;
			this.IsDeleted = model.IsDeleted;
			// ユーザ拡張項目設定、ユーザに拡張項目が存在しない場合は空で連携する
			this.PostcardDmUnnecessaryFlg
				= (model.UserExtend != null)
					&& model.UserExtend.UserExtendColumns.Any(colum => colum == Constants.CROSS_POINT_USREX_DM)
						? model.UserExtend.UserExtendDataValue.CrossPointDm
						: string.Empty;
		}

		/// <summary>
		/// パラメータ取得
		/// </summary>
		/// <returns>パラメータ</returns>
		public Dictionary<string, string> GetParam()
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_NET_SHOP_MEMBER_ID, this.NetShopMemberId },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_LAST_NAME, this.LastName },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_FIRST_NAME, this.FirstName },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_LAST_NAME_PHONETIC, this.LastNamePhonetic },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_FIRST_NAME_PHONETIC, this.FirstNamePhonetic },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_SEX, this.Sex },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BIRTHDAY, this.Birthday },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_POSTCODE, this.Postcode },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PREF_NAME, this.PrefName },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_CITY, this.City },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_TOWN, this.Town },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_ADDRESS, this.Address },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_BUILDING, this.Building },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_TEL, this.Tel },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_MB_TEL, this.MbTel },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_POSTCARD_DM_UNNECESSARY_FLG, this.PostcardDmUnnecessaryFlg },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_EMAIL_DM_UNNECESSARY_FLG, this.EmailDmUnnecessaryFlg },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PASSWORD, this.Password },
				{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_REMARKS_1, this.Remarks1 },
			};

			if (string.IsNullOrEmpty(this.MemberRankName) == false)
			{
				param[Constants.CROSS_POINT_PARAM_MEMBER_INFO_MEMBER_NAME] = this.MemberRankName;
			}
			// 空の場合クロスポイント側に連携をしない
			if (string.IsNullOrEmpty(this.PcMail) == false)
			{
				param[Constants.CROSS_POINT_PARAM_MEMBER_INFO_PC_MAIL] = this.PcMail;
			}
			if (string.IsNullOrEmpty(this.MbMail) == false)
			{
				param[Constants.CROSS_POINT_PARAM_MEMBER_INFO_MB_MAIL] = this.MbMail;
			}

			if (this.IsDeleted)
			{
				// 退会ユーザー情報をクリアする時は、DB上の「***」をAPI側で受けるとことができる文字列に変換
				param = param.ToDictionary(item => item.Key, item => ConvertToWithdrawParam(item.Value));
			}
			return param;
		}

		/// <summary>
		/// パラメータ文字列を退会連携用パラメータ文字列に変換
		/// </summary>
		/// <param name="param">変換対象</param>
		/// <returns>変換後パラメータ文字列</returns>
		private string ConvertToWithdrawParam(string param)
		{
			var result = param.ToCharArray().All(item => item.Equals('*'))
				? string.Empty
				: param;
			return result;
		}

		/// <summary>ネットショップ会員ID</summary>
		public string NetShopMemberId { get; set; }
		/// <summary>姓</summary>
		public string LastName { get; set; }
		/// <summary>名</summary>
		public string FirstName { get; set; }
		/// <summary>姓(カナ)</summary>
		public string LastNamePhonetic { get; set; }
		/// <summary>名(カナ)</summary>
		public string FirstNamePhonetic { get; set; }
		/// <summary>会員ランク名</summary>
		public string MemberRankName { get; set; }
		/// <summary>性別</summary>
		public string Sex { get; set; }
		/// <summary>生年月日</summary>
		public string Birthday { get; set; }
		/// <summary>郵便番号</summary>
		public string Postcode { get; set; }
		/// <summary>都道府県</summary>
		public string PrefName { get; set; }
		/// <summary>市区町村</summary>
		public string City { get; set; }
		/// <summary>町域</summary>
		public string Town { get; set; }
		/// <summary>番地</summary>
		public string Address { get; set; }
		/// <summary>ビル等</summary>
		public string Building { get; set; }
		/// <summary>電話番号</summary>
		public string Tel { get; set; }
		/// <summary>携帯電話番号</summary>
		public string MbTel { get; set; }
		/// <summary>PCメールアドレス</summary>
		public string PcMail { get; set; }
		/// <summary>モバイルメールアドレス</summary>
		public string MbMail { get; set; }
		/// <summary>郵便DM不要フラグ</summary>
		public string PostcardDmUnnecessaryFlg { get; set; }
		/// <summary>メールDM不要フラグ</summary>
		public string EmailDmUnnecessaryFlg { get; set; }
		/// <summary>パスワード</summary>
		public string Password { get; set; }
		/// <summary>備考1</summary>
		public string Remarks1 { get; set; }
		/// <summary>ユーザー拡張項目</summary>
		public UserExtendModel UserExtend { get; set; }
		/// <summary>退会ユーザーか</summary>
		public bool IsDeleted { get; private set; }
	}
}
