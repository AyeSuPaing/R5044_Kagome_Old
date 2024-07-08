/*
=========================================================================================================
  Module      : ユーザマスタモデル (UserModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザマスタモデル
	/// </summary>
	[Serializable]
	public partial class UserModel : ModelBase<UserModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserModel()
		{
			this.UserKbn = Constants.FLG_USER_USER_KBN_PC_USER;
			this.MallId = Constants.FLG_USER_MALL_ID_OWN_SITE;
			this.Sex = Constants.FLG_USER_SEX_UNKNOWN;
			this.Name = string.Empty;
			this.Name1 = string.Empty;
			this.Name2 = string.Empty;
			this.NameKana = string.Empty;
			this.NameKana1 = string.Empty;
			this.NameKana2 = string.Empty;
			this.NickName = string.Empty;
			this.MailAddr = string.Empty;
			this.MailAddr2 = string.Empty;
			this.Zip = string.Empty;
			this.Zip1 = string.Empty;
			this.Zip2 = string.Empty;
			this.Addr = string.Empty;
			this.Addr1 = string.Empty;
			this.Addr2 = string.Empty;
			this.Addr3 = string.Empty;
			this.Addr4 = string.Empty;
			this.Tel1 = string.Empty;
			this.Tel1_1 = string.Empty;
			this.Tel1_2 = string.Empty;
			this.Tel1_3 = string.Empty;
			this.Tel2 = string.Empty;
			this.Tel2_1 = string.Empty;
			this.Tel2_2 = string.Empty;
			this.Tel2_3 = string.Empty;
			this.Tel3 = string.Empty;
			this.Tel3_1 = string.Empty;
			this.Tel3_2 = string.Empty;
			this.Tel3_3 = string.Empty;
			this.Fax = string.Empty;
			this.Fax_1 = string.Empty;
			this.Fax_2 = string.Empty;
			this.Fax_3 = string.Empty;
			this.Birth = null;
			this.BirthYear = string.Empty;
			this.BirthMonth = string.Empty;
			this.BirthDay = string.Empty;
			this.CompanyName = string.Empty;
			this.CompanyPostName = string.Empty;
			this.CompanyExectiveName = string.Empty;
			this.AdvcodeFirst = string.Empty;
			this.Attribute1 = string.Empty;
			this.Attribute2 = string.Empty;
			this.Attribute3 = string.Empty;
			this.Attribute4 = string.Empty;
			this.Attribute5 = string.Empty;
			this.Attribute6 = string.Empty;
			this.Attribute7 = string.Empty;
			this.Attribute8 = string.Empty;
			this.Attribute9 = string.Empty;
			this.Attribute10 = string.Empty;
			this.LoginId = string.Empty;
			this.Password = string.Empty;
			this.Question = string.Empty;
			this.Answer = string.Empty;
			this.CareerId = string.Empty;
			this.MobileUid = string.Empty;
			this.RemoteAddr = string.Empty;
			this.MailFlg = Constants.FLG_USER_MAILFLG_UNKNOWN;
			this.UserMemo = string.Empty;
			this.DelFlg = Constants.FLG_USER_DELFLG_UNDELETED;
			this.LastChanged = string.Empty;
			this.MemberRankId = string.Empty;
			this.RecommendUid = string.Empty;
			this.DateLastLoggedin = null;
			this.UserManagementLevelId = Constants.FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL;
			this.IntegratedFlg = Constants.FLG_USER_INTEGRATED_FLG_NONE;
			this.EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL;
			this.FixedPurchaseMemberFlg = Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF;
			this.AccessCountryIsoCode = "";
			this.DispLanguageCode = "";
			this.DispLanguageLocaleId = "";
			this.DispCurrencyCode = "";
			this.DispCurrencyLocaleId = "";
			this.LastBirthdayPointAddYear = "";
			this.AddrCountryIsoCode = "";
			this.AddrCountryName = "";
			this.Addr5 = "";
			this.LastBirthdayCouponPublishYear = "";

			this.IsRegisted = false;
			this.OrderCountOrderRealtime = 0;
			this.OrderCountOld = 0;
			this.ReferralCode = string.Empty;
			this.ReferredUserId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		[UpdateData(1, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USER_USER_ID] = value; }
		}
		/// <summary>顧客区分</summary>
		[UpdateData(2, "user_kbn")]
		public string UserKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_KBN]; }
			set { this.DataSource[Constants.FIELD_USER_USER_KBN] = value; }
		}
		/// <summary>モールID</summary>
		[UpdateData(3, "mall_id")]
		public string MallId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MALL_ID]; }
			set { this.DataSource[Constants.FIELD_USER_MALL_ID] = value; }
		}
		/// <summary>氏名</summary>
		[UpdateData(4, "name")]
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_NAME] = value; }
		}
		/// <summary>氏名1</summary>
		[UpdateData(5, "name1")]
		public string Name1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME1]; }
			set { this.DataSource[Constants.FIELD_USER_NAME1] = value; }
		}
		/// <summary>氏名2</summary>
		[UpdateData(6, "name2")]
		public string Name2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME2]; }
			set { this.DataSource[Constants.FIELD_USER_NAME2] = value; }
		}
		/// <summary>氏名かな</summary>
		[UpdateData(7, "name_kana")]
		public string NameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_USER_NAME_KANA] = value; }
		}
		/// <summary>氏名かな1</summary>
		[UpdateData(8, "name_kana1")]
		public string NameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_USER_NAME_KANA1] = value; }
		}
		/// <summary>氏名かな2</summary>
		[UpdateData(9, "name_kana2")]
		public string NameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_USER_NAME_KANA2] = value; }
		}
		/// <summary>ニックネーム</summary>
		[UpdateData(10, "nick_name")]
		public string NickName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NICK_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_NICK_NAME] = value; }
		}
		/// <summary>メールアドレス</summary>
		[UpdateData(11, "mail_addr")]
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_USER_MAIL_ADDR] = value; }
		}
		/// <summary>メールアドレス2</summary>
		[UpdateData(12, "mail_addr2")]
		public string MailAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR2]; }
			set { this.DataSource[Constants.FIELD_USER_MAIL_ADDR2] = value; }
		}
		/// <summary>郵便番号</summary>
		[UpdateData(13, "zip")]
		public string Zip
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ZIP]; }
			set { this.DataSource[Constants.FIELD_USER_ZIP] = value; }
		}
		/// <summary>郵便番号1</summary>
		[UpdateData(14, "zip1")]
		public string Zip1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ZIP1]; }
			set { this.DataSource[Constants.FIELD_USER_ZIP1] = value; }
		}
		/// <summary>郵便番号2</summary>
		[UpdateData(15, "zip2")]
		public string Zip2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ZIP2]; }
			set { this.DataSource[Constants.FIELD_USER_ZIP2] = value; }
		}
		/// <summary>住所</summary>
		[UpdateData(16, "addr")]
		public string Addr
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR] = value; }
		}
		/// <summary>住所1</summary>
		[UpdateData(17, "addr1")]
		public string Addr1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR1]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		[UpdateData(18, "addr2")]
		public string Addr2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR2]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		[UpdateData(19, "addr3")]
		public string Addr3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR3]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR3] = value; }
		}
		/// <summary>住所4</summary>
		[UpdateData(20, "addr4")]
		public string Addr4
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR4]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR4] = value; }
		}
		/// <summary>電話番号1</summary>
		[UpdateData(21, "tel1")]
		public string Tel1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL1]; }
			set { this.DataSource[Constants.FIELD_USER_TEL1] = value; }
		}
		/// <summary>電話番号1-1</summary>
		[UpdateData(22, "tel1_1")]
		public string Tel1_1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL1_1]; }
			set { this.DataSource[Constants.FIELD_USER_TEL1_1] = value; }
		}
		/// <summary>電話番号1-2</summary>
		[UpdateData(23, "tel1_2")]
		public string Tel1_2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL1_2]; }
			set { this.DataSource[Constants.FIELD_USER_TEL1_2] = value; }
		}
		/// <summary>電話番号1-3</summary>
		[UpdateData(24, "tel1_3")]
		public string Tel1_3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL1_3]; }
			set { this.DataSource[Constants.FIELD_USER_TEL1_3] = value; }
		}
		/// <summary>電話番号2</summary>
		[UpdateData(25, "tel2")]
		public string Tel2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL2]; }
			set { this.DataSource[Constants.FIELD_USER_TEL2] = value; }
		}
		/// <summary>電話番号2-1</summary>
		[UpdateData(26, "tel2_1")]
		public string Tel2_1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL2_1]; }
			set { this.DataSource[Constants.FIELD_USER_TEL2_1] = value; }
		}
		/// <summary>電話番号2-2</summary>
		[UpdateData(27, "tel2_2")]
		public string Tel2_2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL2_2]; }
			set { this.DataSource[Constants.FIELD_USER_TEL2_2] = value; }
		}
		/// <summary>電話番号2-3</summary>
		[UpdateData(28, "tel2_3")]
		public string Tel2_3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL2_3]; }
			set { this.DataSource[Constants.FIELD_USER_TEL2_3] = value; }
		}
		/// <summary>電話番号3</summary>
		[UpdateData(29, "tel3")]
		public string Tel3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL3]; }
			set { this.DataSource[Constants.FIELD_USER_TEL3] = value; }
		}
		/// <summary>電話番号3-1</summary>
		[UpdateData(30, "tel3_1")]
		public string Tel3_1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL3_1]; }
			set { this.DataSource[Constants.FIELD_USER_TEL3_1] = value; }
		}
		/// <summary>電話番号3-2</summary>
		[UpdateData(31, "tel3_2")]
		public string Tel3_2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL3_2]; }
			set { this.DataSource[Constants.FIELD_USER_TEL3_2] = value; }
		}
		/// <summary>電話番号3-3</summary>
		[UpdateData(32, "tel3_3")]
		public string Tel3_3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL3_3]; }
			set { this.DataSource[Constants.FIELD_USER_TEL3_3] = value; }
		}
		/// <summary>ＦＡＸ</summary>
		[UpdateData(33, "fax")]
		public string Fax
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_FAX]; }
			set { this.DataSource[Constants.FIELD_USER_FAX] = value; }
		}
		/// <summary>ＦＡＸ1</summary>
		[UpdateData(34, "fax_1")]
		public string Fax_1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_FAX_1]; }
			set { this.DataSource[Constants.FIELD_USER_FAX_1] = value; }
		}
		/// <summary>ＦＡＸ2</summary>
		[UpdateData(35, "fax_2")]
		public string Fax_2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_FAX_2]; }
			set { this.DataSource[Constants.FIELD_USER_FAX_2] = value; }
		}
		/// <summary>ＦＡＸ3</summary>
		[UpdateData(36, "fax_3")]
		public string Fax_3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_FAX_3]; }
			set { this.DataSource[Constants.FIELD_USER_FAX_3] = value; }
		}
		/// <summary>性別</summary>
		[UpdateData(37, "sex")]
		public string Sex
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_SEX]; }
			set { this.DataSource[Constants.FIELD_USER_SEX] = value; }
		}
		/// <summary>生年月日</summary>
		[UpdateData(38, "birth")]
		public DateTime? Birth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USER_BIRTH] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USER_BIRTH];
			}
			set { this.DataSource[Constants.FIELD_USER_BIRTH] = value; }
		}
		/// <summary>生年月日（年）</summary>
		[UpdateData(39, "birth_year")]
		public string BirthYear
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BIRTH_YEAR]; }
			set { this.DataSource[Constants.FIELD_USER_BIRTH_YEAR] = value; }
		}
		/// <summary>生年月日（月）</summary>
		[UpdateData(40, "birth_month")]
		public string BirthMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BIRTH_MONTH]; }
			set { this.DataSource[Constants.FIELD_USER_BIRTH_MONTH] = value; }
		}
		/// <summary>生年月日（日）</summary>
		[UpdateData(41, "birth_day")]
		public string BirthDay
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_BIRTH_DAY]; }
			set { this.DataSource[Constants.FIELD_USER_BIRTH_DAY] = value; }
		}
		/// <summary>企業名</summary>
		[UpdateData(42, "company_name")]
		public string CompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_COMPANY_NAME] = value; }
		}
		/// <summary>部署名</summary>
		[UpdateData(43, "company_post_name")]
		public string CompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_COMPANY_POST_NAME] = value; }
		}
		/// <summary>役職名</summary>
		[UpdateData(44, "company_exective_name")]
		public string CompanyExectiveName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_COMPANY_EXECTIVE_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_COMPANY_EXECTIVE_NAME] = value; }
		}
		/// <summary>初回広告コード</summary>
		[UpdateData(45, "advcode_first")]
		public string AdvcodeFirst
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADVCODE_FIRST]; }
			set { this.DataSource[Constants.FIELD_USER_ADVCODE_FIRST] = value; }
		}
		/// <summary>属性1</summary>
		[UpdateData(46, "attribute1")]
		public string Attribute1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE1]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE1] = value; }
		}
		/// <summary>属性2</summary>
		[UpdateData(47, "attribute2")]
		public string Attribute2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE2]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE2] = value; }
		}
		/// <summary>属性3</summary>
		[UpdateData(48, "attribute3")]
		public string Attribute3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE3]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE3] = value; }
		}
		/// <summary>属性4</summary>
		[UpdateData(49, "attribute4")]
		public string Attribute4
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE4]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE4] = value; }
		}
		/// <summary>属性5</summary>
		[UpdateData(50, "attribute5")]
		public string Attribute5
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE5]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE5] = value; }
		}
		/// <summary>属性6</summary>
		[UpdateData(51, "attribute6")]
		public string Attribute6
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE6]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE6] = value; }
		}
		/// <summary>属性7</summary>
		[UpdateData(52, "attribute7")]
		public string Attribute7
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE7]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE7] = value; }
		}
		/// <summary>属性8</summary>
		[UpdateData(53, "attribute8")]
		public string Attribute8
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE8]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE8] = value; }
		}
		/// <summary>属性9</summary>
		[UpdateData(54, "attribute9")]
		public string Attribute9
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE9]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE9] = value; }
		}
		/// <summary>属性10</summary>
		[UpdateData(55, "attribute10")]
		public string Attribute10
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ATTRIBUTE10]; }
			set { this.DataSource[Constants.FIELD_USER_ATTRIBUTE10] = value; }
		}
		/// <summary>ログインＩＤ</summary>
		[UpdateData(56, "login_id")]
		public string LoginId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_LOGIN_ID]; }
			set { this.DataSource[Constants.FIELD_USER_LOGIN_ID] = value; }
		}
		/// <summary>暗号化されているパスワード</summary>
		[UpdateData(57, "password")]
		public string Password
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_PASSWORD]; }
			set { this.DataSource[Constants.FIELD_USER_PASSWORD] = value; }
		}
		/// <summary>質問</summary>
		[UpdateData(58, "question")]
		public string Question
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_QUESTION]; }
			set { this.DataSource[Constants.FIELD_USER_QUESTION] = value; }
		}
		/// <summary>回答</summary>
		[UpdateData(59, "answer")]
		public string Answer
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ANSWER]; }
			set { this.DataSource[Constants.FIELD_USER_ANSWER] = value; }
		}
		/// <summary>キャリアID</summary>
		[UpdateData(60, "career_id")]
		public string CareerId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_CAREER_ID]; }
			set { this.DataSource[Constants.FIELD_USER_CAREER_ID] = value; }
		}
		/// <summary>モバイルUID</summary>
		[UpdateData(61, "mobile_uid")]
		public string MobileUid
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MOBILE_UID]; }
			set { this.DataSource[Constants.FIELD_USER_MOBILE_UID] = value; }
		}
		/// <summary>リモートIPアドレス</summary>
		[UpdateData(62, "remote_addr")]
		public string RemoteAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_REMOTE_ADDR]; }
			set { this.DataSource[Constants.FIELD_USER_REMOTE_ADDR] = value; }
		}
		/// <summary>メール配信フラグ</summary>
		[UpdateData(63, "mail_flg")]
		public string MailFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_FLG]; }
			set { this.DataSource[Constants.FIELD_USER_MAIL_FLG] = value; }
		}
		/// <summary>ユーザメモ</summary>
		[UpdateData(64, "user_memo")]
		public string UserMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_MEMO]; }
			set { this.DataSource[Constants.FIELD_USER_USER_MEMO] = value; }
		}
		/// <summary>削除フラグ</summary>
		[UpdateData(65, "del_flg")]
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_USER_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(66, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(67, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USER_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[UpdateData(68, "last_changed")]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USER_LAST_CHANGED] = value; }
		}
		/// <summary>会員ランクID</summary>
		[UpdateData(69, "member_rank_id")]
		public string MemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_USER_MEMBER_RANK_ID] = value; }
		}
		/// <summary>外部レコメンド連携用ユーザID</summary>
		[UpdateData(70, "recommend_uid")]
		public string RecommendUid
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USER_RECOMMEND_UID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_USER_RECOMMEND_UID];
			}
			set { this.DataSource[Constants.FIELD_USER_RECOMMEND_UID] = value; }
		}
		/// <summary>最終ログイン日時</summary>
		[UpdateData(71, "date_last_loggedin")]
		public DateTime? DateLastLoggedin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN];
			}
			set { this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN] = value; }
		}
		/// <summary>ユーザー管理レベルID</summary>
		[UpdateData(72, "user_management_level_id")]
		public string UserManagementLevelId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID]; }
			set { this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID] = value; }
		}
		/// <summary>ユーザー統合フラグ</summary>
		[UpdateData(73, "integrated_flg")]
		public string IntegratedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_INTEGRATED_FLG]; }
			set { this.DataSource[Constants.FIELD_USER_INTEGRATED_FLG] = value; }
		}
		/// <summary>かんたん会員フラグ</summary>
		[UpdateData(74, "easy_register_flg")]
		public string EasyRegisterFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_EASY_REGISTER_FLG]; }
			set { this.DataSource[Constants.FIELD_USER_EASY_REGISTER_FLG] = value; }
		}
		/// <summary>定期会員フラグ</summary>
		[UpdateData(75, "fixed_purchase_member_flg")]
		public string FixedPurchaseMemberFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG]; }
			set { this.DataSource[Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG] = value; }
		}
		/// <summary>アクセス国ISOコード</summary>
		[UpdateDataAttribute(76, "access_country_iso_code")]
		public string AccessCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>表示言語コード</summary>
		[UpdateDataAttribute(77, "disp_language_code")]
		public string DispLanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_CODE] = value; }
		}
		/// <summary>表示言語ロケールID</summary>
		[UpdateDataAttribute(78, "disp_language_locale_id")]
		public string DispLanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>表示通貨コード</summary>
		[UpdateDataAttribute(79, "disp_currency_code")]
		public string DispCurrencyCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_DISP_CURRENCY_CODE]; }
			set { this.DataSource[Constants.FIELD_USER_DISP_CURRENCY_CODE] = value; }
		}
		/// <summary>表示通貨ロケールID</summary>
		[UpdateDataAttribute(80, "disp_currency_locale_id")]
		public string DispCurrencyLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID] = value; }
		}
		/// <summary>最終誕生日ポイント付与年</summary>
		[UpdateDataAttribute(81, "last_birthday_point_add_year")]
		public string LastBirthdayPointAddYear
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR]; }
			set { this.DataSource[Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR] = value; }
		}
		/// <summary>住所国ISOコード</summary>
		[UpdateDataAttribute(82, "addr_country_iso_code")]
		public string AddrCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>住所国名</summary>
		[UpdateDataAttribute(83, "addr_country_name")]
		public string AddrCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_NAME] = value; }
		}
		/// <summary>住所5</summary>
		[UpdateDataAttribute(84, "addr5")]
		public string Addr5
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR5]; }
			set { this.DataSource[Constants.FIELD_USER_ADDR5] = value; }
		}
		/// <summary>最終誕生日クーポン付与年</summary>
		[UpdateDataAttribute(85, "last_birthday_coupon_publish_year")]
		public string LastBirthdayCouponPublishYear
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR]; }
			set { this.DataSource[Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR] = value; }
		}
		/// <summary>リアルタイム購入回数（注文基準）</summary>
		[UpdateData(86, "order_count_order_realtime")]
		public int OrderCountOrderRealtime
		{
			get { return (int)this.DataSource[Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME]; }
			set { this.DataSource[Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME] = value; }
		}
		/// <summary>過去累計購入回数</summary>
		[UpdateData(87, "order_count_old")]
		public int OrderCountOld
		{
			get { return (int)this.DataSource[Constants.FIELD_USER_ORDER_COUNT_OLD]; }
			set { this.DataSource[Constants.FIELD_USER_ORDER_COUNT_OLD] = value; }
		}
		/// <summary>紹介コード</summary>
		[UpdateData(88, "referral_code")]
		public string ReferralCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_REFERRAL_CODE]; }
			set { this.DataSource[Constants.FIELD_USER_REFERRAL_CODE] = value; }
		}
		/// <summary>紹介元ユーザーID</summary>
		[UpdateData(89, "referred_user_id")]
		public string ReferredUserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_REFERRED_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USER_REFERRED_USER_ID] = value; }
		}
		#endregion
	}
}
