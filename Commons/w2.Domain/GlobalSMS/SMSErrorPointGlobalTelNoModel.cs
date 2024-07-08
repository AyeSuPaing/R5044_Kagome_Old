/*
=========================================================================================================
  Module      : SMSエラーポイントモデル (SMSErrorPointGlobalTelNoModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.GlobalSMS
{
	/// <summary>
	/// SMSエラーポイントモデル
	/// </summary>
	[Serializable]
	public partial class SMSErrorPointGlobalTelNoModel : ModelBase<SMSErrorPointGlobalTelNoModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SMSErrorPointGlobalTelNoModel()
		{
			this.GlobalTelNo = "";
			this.ErrorPoint = 0;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SMSErrorPointGlobalTelNoModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SMSErrorPointGlobalTelNoModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>グローバル電話番号</summary>
		public string GlobalTelNo
		{
			get { return (string)this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_GLOBAL_TEL_NO]; }
			set { this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_GLOBAL_TEL_NO] = value; }
		}
		/// <summary>エラーポイント</summary>
		public int ErrorPoint
		{
			get { return (int)this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_ERROR_POINT]; }
			set { this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_ERROR_POINT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SMSERRORPOINTGLOBALTELNO_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
