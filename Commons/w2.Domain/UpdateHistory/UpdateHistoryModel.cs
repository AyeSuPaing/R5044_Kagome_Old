/*
=========================================================================================================
  Module      : 更新履歴情報モデル (UpdateHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UpdateHistory
{
	/// <summary>
	/// 更新履歴情報モデル
	/// </summary>
	[Serializable]
	public partial class UpdateHistoryModel : ModelBase<UpdateHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UpdateHistoryModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UpdateHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UpdateHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>更新履歴No</summary>
		public long UpdateHistoryNo
		{
			get { return (long)this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_NO]; }
		}
		/// <summary>更新履歴区分</summary>
		public string UpdateHistoryKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN] = value; }
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_USER_ID] = value; }
		}
		/// <summary>マスタID</summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_MASTER_ID] = value; }
		}
		/// <summary>更新履歴アクション</summary>
		public string UpdateHistoryAction
		{
			get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_ACTION]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_ACTION] = value; }
		}
		/// <summary>更新データ</summary>
		public byte[] UpdateData
		{
			get { return (byte[])this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_DATA]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_DATA] = value; }
		}
		/// <summary>更新データハッシュ</summary>
		public string UpdateDataHash
		{
			get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_DATA_HASH]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_DATA_HASH] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_UPDATEHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_UPDATEHISTORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
