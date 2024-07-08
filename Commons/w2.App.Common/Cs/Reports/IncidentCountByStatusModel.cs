/*
=========================================================================================================
  Module      : ステータス毎のインシデント件数モデル(IncidentCountByStatusModel.cs)
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
	/// ステータス毎のインシデント件数モデル
	/// </summary>
	[Serializable]
	public class IncidentCountByStatusModel : ModelBase<IncidentCountByStatusModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public IncidentCountByStatusModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public IncidentCountByStatusModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public IncidentCountByStatusModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>未対応</summary>
		public int None
		{
			get { return (int)this.DataSource[Constants.FLG_CSINCIDENT_STATUS_NONE]; }
			set { this.DataSource[Constants.FLG_CSINCIDENT_STATUS_NONE] = value; }
		}
		/// <summary>対応中</summary>
		public int Active
		{
			get { return (int)this.DataSource[Constants.FLG_CSINCIDENT_STATUS_ACTIVE]; }
			set { this.DataSource[Constants.FLG_CSINCIDENT_STATUS_ACTIVE] = value; }
		}
		/// <summary>至急</summary>
		public int Urgent
		{
			get { return (int)this.DataSource[Constants.FLG_CSINCIDENT_STATUS_URGENT]; }
			set { this.DataSource[Constants.FLG_CSINCIDENT_STATUS_URGENT] = value; }
		}
		/// <summary>保留</summary>
		public int Suspend
		{
			get { return (int)this.DataSource[Constants.FLG_CSINCIDENT_STATUS_SUSPEND]; }
			set { this.DataSource[Constants.FLG_CSINCIDENT_STATUS_SUSPEND] = value; }
		}
		/// <summary>未完了合計</summary>
		public int UncompleteTotal
		{
			get { return this.None + this.Active + this.Urgent + this.Suspend; }
		}
		#endregion
	}
}
