/*
=========================================================================================================
  Module      : ユーザー統合表示のためのヘルパクラス (UserIntegrationContainer.cs)
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
using w2.Domain.Point;

namespace w2.Domain.UserIntegration.Helper
{
	#region +ユーザー統合表示クラス
	/// <summary>
	/// ユーザー統合表示クラス
	/// ※UserIntegrationModelを拡張
	/// </summary>
	[Serializable]
	public class UserIntegrationContainer : UserIntegrationModel
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationContainer(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationContainer(Hashtable source)
			: base()
		{
			this.DataSource = source;
		}
		#endregion

		#region +プロパティ
		/// <summary>ユーザーリスト</summary>
		public new UserIntegrationUserContainer[] Users
		{
			get { return (UserIntegrationUserContainer[])this.DataSource["Users"]; }
			set { this.DataSource["Users"] = value; }
		}
		#endregion
	}
	#endregion

	#region +ユーザー統合表示クラス（ユーザー統合ユーザー情報）
	/// <summary>
	/// ユーザー統合表示クラス（ユーザー統合ユーザー情報）
	/// ※UserIntegrationUserModelを拡張
	/// </summary>
	[Serializable]
	public class UserIntegrationUserContainer : UserIntegrationUserModel
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationUserContainer(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationUserContainer(Hashtable source)
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
		/// <summary>氏名1</summary>
		public string Name1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME1]; }
		}
		/// <summary>氏名2</summary>
		public string Name2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME2]; }
		}
		/// <summary>氏名かな</summary>
		public string NameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA]; }
		}
		/// <summary>氏名かな1</summary>
		public string NameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA1]; }
		}
		/// <summary>氏名かな2</summary>
		public string NameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME_KANA2]; }
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
		/// <summary>最終ログイン日時</summary>
		public DateTime? DateLastLoggedin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USER_DATE_LAST_LOGGEDIN];
			}
		}
		/// <summary>カード連携情報が存在する?</summary>
		public bool HasCreditCards
		{
			get { return (int)this.DataSource["credit_card_count"] > 0; }
		}
		/// <summary>履歴リスト</summary>
		public new UserIntegrationHistoryContainer[] Histories
		{
			get { return (UserIntegrationHistoryContainer[])this.DataSource["Histories"]; }
			set { this.DataSource["Histories"] = value; }
		}
		#endregion
	}
	#endregion

	#region +ユーザー統合表示クラスクラス（ユーザー統合履歴情報）
	/// <summary>
	/// ユーザー統合表示クラスクラス（ユーザー統合履歴情報）
	/// ※UserIntegrationHistoryModelを拡張
	/// </summary>
	[Serializable]
	public class UserIntegrationHistoryContainer : UserIntegrationHistoryModel
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationHistoryContainer(DataRowView source)
			: base(source)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserIntegrationHistoryContainer(Hashtable source)
			: base()
		{
			this.DataSource = source;
		}
		#endregion

		#region +プロパティ
		/// <summary>ユーザーポイント履歴情報</summary>
		public UserPointHistoryModel UserPointHistory
		{
			get { return (UserPointHistoryModel)this.DataSource["UserPointHistory"]; }
			set { this.DataSource["UserPointHistory"] = value; }
		}
		#endregion
	}
	#endregion
}