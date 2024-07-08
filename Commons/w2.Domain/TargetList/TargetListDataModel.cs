/*
=========================================================================================================
  Module      : ターゲットリストデータモデル (TargetListDataModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TargetList
{
	/// <summary>
	/// ターゲットリストデータモデル
	/// </summary>
	[Serializable]
	public partial class TargetListDataModel : ModelBase<TargetListDataModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TargetListDataModel()
		{
			this.DispLanguageCode = "";
			this.DispLanguageLocaleId = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TargetListDataModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TargetListDataModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLISTDATA_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_TARGETLISTDATA_DEPT_ID] = value; }
		}
		/// <summary>ターゲットデータ区分</summary>
		public string TargetKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLISTDATA_TARGET_KBN]; }
			set { this.DataSource[Constants.FIELD_TARGETLISTDATA_TARGET_KBN] = value; }
		}
		/// <summary>マスタID</summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLISTDATA_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_TARGETLISTDATA_MASTER_ID] = value; }
		}
		/// <summary>枝番</summary>
		public long DataNo
		{
			get { return (long)this.DataSource[Constants.FIELD_TARGETLISTDATA_DATA_NO]; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLISTDATA_USER_ID]; }
			set { this.DataSource[Constants.FIELD_TARGETLISTDATA_USER_ID] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR] = value; }
		}
		/// <summary>メールアドレス区分</summary>
		public string MailAddrKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR_KBN]; }
			set { this.DataSource[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR_KBN] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TARGETLISTDATA_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TARGETLISTDATA_DATE_CREATED] = value; }
		}
		#endregion
	}
}
