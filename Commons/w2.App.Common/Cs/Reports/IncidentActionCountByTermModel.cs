/*
=========================================================================================================
  Module      : 期間内インシデントアクション件数モデル(IncidentActionCountByTermModel.cs)
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
	/// 期間内インシデントアクション件数モデル
	/// </summary>
	[Serializable]
	public class IncidentActionCountByTermModel : ModelBase<IncidentActionCountByTermModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public IncidentActionCountByTermModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public IncidentActionCountByTermModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public IncidentActionCountByTermModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>期間(日数）</summary>
		public int DaySpan
		{
			get { return (int)this.DataSource["DaySpan"]; }
			set { this.DataSource["DaySpan"] = value; }
		}
		/// <summary>発生件数</summary>
		public int Occurred
		{
			get { return (int)this.DataSource["Occurred"]; }
			set { this.DataSource["Occurred"] = value; }
		}
		/// <summary>完了件数</summary>
		public int Completed
		{
			get { return (int)this.DataSource["Completed"]; }
			set { this.DataSource["Completed"] = value; }
		}
		#endregion
	}
}
