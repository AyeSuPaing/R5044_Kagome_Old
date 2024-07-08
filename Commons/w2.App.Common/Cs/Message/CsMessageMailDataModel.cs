/*
=========================================================================================================
  Module      : メッセージメールデータモデル(CsMessageMailDataModel.cs)
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
	/// メッセージメールデータモデル
	/// </summary>
	[Serializable]
	public partial class CsMessageMailDataModel : ModelBase<CsMessageMailDataModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMessageMailDataModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データ</param>
		public CsMessageMailDataModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データ</param>
		public CsMessageMailDataModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAILDATA_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILDATA_DEPT_ID] = value; }
		}
		/// <summary>メールID</summary>
		public string MailId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAILDATA_MAIL_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILDATA_MAIL_ID] = value; }
		}
		/// <summary>メールデータ</summary>
		public string MailData
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAILDATA_MAIL_DATA]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAILDATA_MAIL_DATA] = value; }
		}
		#endregion


	}
}
