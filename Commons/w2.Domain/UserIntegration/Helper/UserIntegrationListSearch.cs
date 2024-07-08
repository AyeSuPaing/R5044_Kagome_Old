/*
=========================================================================================================
  Module      : ユーザー統合一覧検索のためのヘルパクラス (UserIntegrationListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;
using w2.Domain.User;

namespace w2.Domain.UserIntegration.Helper
{
	#region +ユーザー統合一覧検索条件クラス
	/// <summary>
	/// ユーザー統合一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class UserIntegrationListSearchCondition : BaseDbMapModel
	{
		/// <summary>ステータス</summary>
		[DbMapName("status")]
		public string Status { get; set; }
		/// <summary>ユーザー統合No</summary>
		[DbMapName("user_integration_no")]
		public long? UserIntegrationNo { get; set; }
		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>ユーザID（SQL LIKEエスケープ済）</summary>
		[DbMapName("user_id_like_escaped")]
		public string UserIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.UserId); }
		}
		/// <summary>氏名</summary>
		public string Name { get; set; }
		/// <summary>氏名（SQL LIKEエスケープ済）</summary>
		[DbMapName("name_like_escaped")]
		public string NameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.Name); }
		}
		/// <summary>氏名かな</summary>
		public string NameKana { get; set; }
		/// <summary>氏名かな（SQL LIKEエスケープ済）</summary>
		[DbMapName("name_kana_like_escaped")]
		public string NameKanaLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.NameKana); }
		}
		/// <summary>電話番号</summary>
		public string Tel { get; set; }
		/// <summary>電話番号（SQL LIKEエスケープ済）</summary>
		[DbMapName("tel_like_escaped")]
		public string TelLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.Tel); }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr { get; set; }
		/// <summary>メールアドレス（SQL LIKEエスケープ済）</summary>
		[DbMapName("mail_addr_like_escaped")]
		public string MailAddrLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.MailAddr); }
		}
		/// <summary>郵便番号</summary>
		public string Zip { get; set; }
		/// <summary>郵便番号（SQL LIKEエスケープ済）</summary>
		[DbMapName("zip_like_escaped")]
		public string ZipLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.Zip); }
		}
		/// <summary>住所</summary>
		public string Addr { get; set; }
		/// <summary>住所（SQL LIKEエスケープ済）</summary>
		[DbMapName("addr_like_escaped")]
		public string AddrLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.Addr); }
		}
		/// <summary>
		/// 並び順区分
		/// 0：ユーザー統合No/昇順
		/// 1：ユーザー統合No/降順
		/// 2：作成日/昇順
		/// 3：作成日/降順
		/// 4：更新日/昇順
		/// 5：更新日/降順
		/// </summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
	}
	#endregion

	#region +ユーザー統合一覧検索結果クラス
	/// <summary>
	/// ユーザー統合一覧検索結果クラス
	/// ※UserIntegrationModelを拡張
	/// </summary>
	[Serializable]
	public class UserIntegrationListSearchResult : UserIntegrationModel
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationListSearchResult(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationListSearchResult(Hashtable source)
			: base()
		{
			this.DataSource = source;
		}
		#endregion

		#region +プロパティ
		/// <summary>ユーザーリスト</summary>
		public new UserIntegrationUserListSearchResult[] Users
		{
			get { return (UserIntegrationUserListSearchResult[])this.DataSource["Users"]; }
			set { this.DataSource["Users"] = value; }
		}
		#endregion
	}
	#endregion

	#region +ユーザー統合一覧検索結果クラス（ユーザー統合ユーザー情報）
	/// <summary>
	/// ユーザー統合一覧検索結果クラス（ユーザー統合ユーザー情報）
	/// ※UserIntegrationUserModelを拡張
	/// </summary>
	[Serializable]
	public class UserIntegrationUserListSearchResult : UserIntegrationUserModel
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationUserListSearchResult(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationUserListSearchResult(Hashtable source)
			: base()
		{
			this.DataSource = source;
		}
		#endregion

		#region +プロパティ
		/// <summary>顧客区分</summary>
		public string UserKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_KBN]; }
		}
		/// <summary>顧客区分テキスト</summary>
		public string UserKbnText
		{
			get { return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, this.UserKbn); }
		}
		/// <summary>モールID</summary>
		public string MallId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MALL_ID]; }
		}
		/// <summary>モール名</summary>
		public string MallName
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME] == DBNull.Value) return "";
				return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME];
			}
		}
		/// <summary>氏名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
		}
		/// <summary>氏名かな</summary>
		public string NameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA]; }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR]; }
		}
		/// <summary>メールアドレス2</summary>
		public string MailAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MAIL_ADDR2]; }
		}
		/// <summary>郵便番号</summary>
		public string Zip
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ZIP]; }
		}
		/// <summary>住所</summary>
		public string Addr
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR]; }
		}
		/// <summary>電話番号1</summary>
		public string Tel1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL1]; }
		}
		/// <summary>電話番号2</summary>
		public string Tel2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_TEL2]; }
		}
		/// <summary>性別</summary>
		public string Sex
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_SEX]; }
		}

		/// <summary>住所1</summary>
		public string Addr1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR1]; }
		}
		/// <summary>住所2</summary>
		public string Addr2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR2]; }
		}
		/// <summary>住所3</summary>
		public string Addr3
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR3]; }
		}
		/// <summary>住所4</summary>
		public string Addr4
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR4]; }
		}
		/// <summary>住所5</summary>
		public string Addr5
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR5]; }
		}
		/// <summary>住所国名</summary>
		public string AddrCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_NAME]; }
		}
		/// <summary>住所国ISOコード</summary>
		public string AddrCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]; }
		}
		/// <summary>海外住所</summary>
		public string AddrGlobal
		{
			get
			{
				return (string.Format("{0} {1} {2} {3} {4} {5}"
					,this.Addr2
					,this.Addr3
					,this.Addr4
					,this.Addr5
					,this.Zip
					, this.AddrCountryName));
			}
		}
		/// <summary>
		/// 性別テキスト
		/// </summary>
		public string SexText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, this.Sex);
			}
		}
		/// <summary>生年月日</summary>
		public DateTime? Birth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USER_BIRTH] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USER_BIRTH];
			}
		}
		/// <summary>作成日</summary>
		public DateTime UserDateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CREATED + "_" + Constants.TABLE_USER]; }
		}
		/// <summary>更新日</summary>
		public DateTime UserDateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USER_DATE_CHANGED + "_" + Constants.TABLE_USER]; }
		}
		/// <summary>カード連携情報が存在する?</summary>
		public bool HasCreditCards
		{
			get { return (int)this.DataSource["credit_card_count"] > 0; }
		}
		/// <summary>履歴リスト</summary>
		public new UserIntegrationHistoryListSearchResult[] Histories
		{
			get { return (UserIntegrationHistoryListSearchResult[])this.DataSource["Histories"]; }
			set { this.DataSource["Histories"] = value; }
		}
		#endregion
	}
	#endregion

	#region +ユーザー統合一覧検索結果クラス（ユーザー統合履歴情報）
	/// <summary>
	/// ユーザー統合一覧検索結果クラス（ユーザー統合履歴情報）
	/// ※UserIntegrationHistoryModelを拡張
	/// </summary>
	[Serializable]
	public class UserIntegrationHistoryListSearchResult : UserIntegrationHistoryModel
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationHistoryListSearchResult(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationHistoryListSearchResult(Hashtable source)
			: base()
		{
			this.DataSource = source;
		}
		#endregion
	}
	#endregion
}
