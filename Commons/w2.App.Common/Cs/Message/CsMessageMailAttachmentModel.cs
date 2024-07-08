/*
=========================================================================================================
  Module      : メッセージメール添付ファイルモデル(CsMessageMailAttachmentModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージメール添付ファイルモデル
	/// </summary>
	[Serializable]
	public partial class CsMessageMailAttachmentModel : ModelBase<CsMessageMailAttachmentModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMessageMailAttachmentModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データ</param>
		public CsMessageMailAttachmentModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データ</param>
		public CsMessageMailAttachmentModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID] = value; }
		}
		/// <summary>メールID</summary>
		public string MailId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID] = value; }
		}
		/// <summary>ファイル枝番</summary>
		public int FileNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_NO]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_NO] = value; }
		}
		/// <summary>ファイル名</summary>
		public string FileName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_NAME]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_NAME] = value; }
		}
		/// <summary>ファイルデータ</summary>
		public byte[] FileData
		{
			get { return (byte[])this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_DATA]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_DATA] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILATTACHMENT_DATE_CREATED] = value; }
		}
		#endregion

	}
}
