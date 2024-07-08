/*
=========================================================================================================
  Module      : ユーザーアクティビティモデル (UserActivityModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザーアクティビティモデル
	/// </summary>
	[Serializable]
	public partial class UserActivityModel : ModelBase<UserActivityModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserActivityModel()
		{
			this.UserId = "";
			this.MasterKbn = "";
			this.MasterId = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserActivityModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserActivityModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERACTIVITY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERACTIVITY_USER_ID] = value; }
		}
		/// <summary>マスター区分</summary>
		public string MasterKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERACTIVITY_MASTER_KBN]; }
			set { this.DataSource[Constants.FIELD_USERACTIVITY_MASTER_KBN] = value; }
		}
		/// <summary>マスターID</summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERACTIVITY_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_USERACTIVITY_MASTER_ID] = value; }
		}
		/// <summary>日付</summary>
		public DateTime Date
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERACTIVITY_DATE]; }
			set { this.DataSource[Constants.FIELD_USERACTIVITY_DATE] = value; }
		}
		/// <summary>対象年</summary>
		public string TgtYear
		{
			get { return (string)this.DataSource[Constants.FIELD_USERACTIVITY_TGT_YEAR]; }
		}
		/// <summary>対象月</summary>
		public string TgtMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_USERACTIVITY_TGT_MONTH]; }
		}
		/// <summary>対象日</summary>
		public string TgtDay
		{
			get { return (string)this.DataSource[Constants.FIELD_USERACTIVITY_TGT_DAY]; }
		}
		#endregion
	}
}
