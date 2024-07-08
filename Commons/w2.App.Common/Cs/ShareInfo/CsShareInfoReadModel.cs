/*
=========================================================================================================
  Module      : 共有情報既読管理モデル(CsShareInfoReadModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報既読管理モデル
	/// </summary>
	[Serializable]
	public partial class CsShareInfoReadModel : ModelBase<CsShareInfoReadModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsShareInfoReadModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">共有情報既読管理データ</param>
		public CsShareInfoReadModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">共有情報既読管理データ</param>
		public CsShareInfoReadModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DEPT_ID] = value; }
		}
		/// <summary>共有情報NO</summary>
		public long InfoNo
		{
			get { return (long)this.DataSource[Constants.FIELD_CSSHAREINFOREAD_INFO_NO]; }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_INFO_NO] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_OPERATOR_ID] = value; }
		}
		/// <summary>既読フラグ</summary>
		public string ReadFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_READ_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_READ_FLG] = value; }
		}
		/// <summary>ピン止めフラグ</summary>
		public string PinnedFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_PINNED_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_PINNED_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSSHAREINFOREAD_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
