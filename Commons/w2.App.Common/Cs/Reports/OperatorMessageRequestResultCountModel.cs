/*
=========================================================================================================
  Module      : オペレータ別メッセージリクエスト結果件数モデル(OperatorMessageRequestResultCountModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// オペレータ別メッセージリクエスト結果件数モデル
	/// </summary>
	[Serializable]
	public class OperatorMessageRequestResultCountModel : ModelBase<OperatorMessageRequestResultCountModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OperatorMessageRequestResultCountModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OperatorMessageRequestResultCountModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OperatorMessageRequestResultCountModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>名称</summary>
		public string Name
		{
			get { return StringUtility.ToEmpty(this.DataSource["Name"]); }
			set { this.DataSource["Name"] = value; }
		}
		/// <summary>有効状態</summary>
		public bool Valid
		{
			get { return StringUtility.ToEmpty(this.DataSource["valid_flg"]) == "1"; }
			set { this.DataSource["valid_flg"] = value ? "1" : "0"; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource["OperatorId"]); }
			set { this.DataSource["OperatorId"] = value; }
		}
		/// <summary>結果ステータス</summary>
		public string ResultStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_RESULT_STATUS]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_RESULT_STATUS] = value; }
		}
		/// <summary>依頼種別</summary>
		public string RequestType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_TYPE]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_TYPE] = value; }
		}
		/// <summary>件数</summary>
		public int Count
		{
			get { return (int)this.DataSource["Count"]; }
			set { this.DataSource["Count"] = value; }
		}
		#endregion
	}
}
