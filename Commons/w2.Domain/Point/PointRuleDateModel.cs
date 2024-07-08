/*
=========================================================================================================
  Module      : ポイントルール日付マスタモデル (PointRuleDateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントルール日付マスタモデル
	/// </summary>
	[Serializable]
	public partial class PointRuleDateModel : ModelBase<PointRuleDateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PointRuleDateModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PointRuleDateModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PointRuleDateModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULEDATE_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULEDATE_DEPT_ID] = value; }
		}
		/// <summary>ポイントルールID</summary>
		public string PointRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULEDATE_POINT_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULEDATE_POINT_RULE_ID] = value; }
		}
		/// <summary>対象日付</summary>
		public DateTime TgtDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_POINTRULEDATE_TGT_DATE]; }
			set { this.DataSource[Constants.FIELD_POINTRULEDATE_TGT_DATE] = value; }
		}
		#endregion
	}
}
