/*
=========================================================================================================
  Module      : LINE仮会員テーブルモデル (LineTemporaryUserModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.LineTemporaryUser
{
	/// <summary>
	/// LINE仮会員テーブルモデル
	/// </summary>
	[Serializable]
	public partial class LineTemporaryUserModel : ModelBase<LineTemporaryUserModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LineTemporaryUserModel()
		{
			this.LineUserId = string.Empty;
			this.TemporaryUserId = string.Empty;
			this.RegularUserRegistrationFlag = Constants.FLG_LINETEMPORARYUSER_REGISTRATION_FLAG_INVALID;
			this.RegularUserRegistrationDate = null;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LineTemporaryUserModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LineTemporaryUserModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>LINEユーザーID</summary>
		public string LineUserId
		{
			get { return (string)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_LINE_USER_ID]; }
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_LINE_USER_ID] = value; }
		}
		/// <summary>仮採番ユーザーID</summary>
		public string TemporaryUserId
		{
			get { return (string)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_TEMPORARY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_TEMPORARY_USER_ID] = value; }
		}
		/// <summary>仮会員登録日時</summary>
		public DateTime TemporaryUserRegistrationDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_TEMPORARY_USER_REGISTRATION_DATE]; }
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_TEMPORARY_USER_REGISTRATION_DATE] = value; }
		}
		/// <summary>本会員登録フラグ</summary>
		public string RegularUserRegistrationFlag
		{
			get { return (string)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_REGULAR_USER_REGISTRATION_FLAG]; }
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_REGULAR_USER_REGISTRATION_FLAG] = value; }
		}
		/// <summary>本会員登録日時</summary>
		public DateTime? RegularUserRegistrationDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_LINETEMPORARYUSER_REGULAR_USER_REGISTRATION_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_REGULAR_USER_REGISTRATION_DATE];
			}
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_REGULAR_USER_REGISTRATION_DATE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
