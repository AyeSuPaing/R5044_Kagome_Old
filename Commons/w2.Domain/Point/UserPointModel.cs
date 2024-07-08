/*
=========================================================================================================
  Module      : ユーザポイントマスタモデル (UserPointModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Point
{
	/// <summary>
	/// ユーザポイントマスタモデル
	/// </summary>
	[Serializable]
	public partial class UserPointModel : ModelBase<UserPointModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserPointModel()
		{
			this.DeptId = Constants.CONST_DEFAULT_DEPT_ID;
			this.PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE;
			this.PointKbnNo = 1;
			this.PointRuleId = string.Empty;
			this.PointRuleKbn = string.Empty;
			this.PointType = Constants.FLG_USERPOINT_POINT_TYPE_COMP;
			this.PointIncKbn = string.Empty;
			this.Point = 0;
			this.PointExp = null;
			this.HistoryNo = 0;
			this.Kbn1 = string.Empty;
			this.Kbn2 = string.Empty;
			this.Kbn3 = string.Empty;
			this.Kbn4 = string.Empty;
			this.Kbn5 = string.Empty;
			this.LastChanged = string.Empty;
			this.EffectiveDate = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserPointModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserPointModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		[UpdateData(1, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_USER_ID] = value; }
		}
		/// <summary>ポイント区分</summary>
		[UpdateData(2, "point_kbn")]
		public string PointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN] = value; }
		}
		/// <summary>枝番</summary>
		[UpdateData(3, "point_kbn_no")]
		public int PointKbnNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN_NO]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_KBN_NO] = value; }
		}
		/// <summary>識別ID</summary>
		[UpdateData(4, "dept_id")]
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_DEPT_ID] = value; }
		}
		/// <summary>ポイントルールID</summary>
		[UpdateData(5, "point_rule_id")]
		public string PointRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_RULE_ID] = value; }
		}
		/// <summary>ポイントルール区分</summary>
		[UpdateData(6, "point_rule_kbn")]
		public string PointRuleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_RULE_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_RULE_KBN] = value; }
		}
		/// <summary>ポイント種別</summary>
		[UpdateData(7, "point_type")]
		public string PointType
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_TYPE]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_TYPE] = value; }
		}
		/// <summary>ポイント加算区分</summary>
		[UpdateData(8, "point_inc_kbn")]
		public string PointIncKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_POINT_INC_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_INC_KBN] = value; }
		}
		/// <summary>ポイント数</summary>
		[UpdateData(9, "point")]
		public decimal Point
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERPOINT_POINT]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT] = value; }
		}
		/// <summary>有効期限</summary>
		[UpdateData(10, "point_exp")]
		public DateTime? PointExp
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERPOINT_POINT_EXP] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERPOINT_POINT_EXP];
			}
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_EXP] = value; }
		}
		/// <summary>枝番（ユーザポイント履歴用）</summary>
		[UpdateData(11, "history_no")]
		public int? HistoryNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERPOINT_HISTORY_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_USERPOINT_HISTORY_NO];
			}
			set { this.DataSource[Constants.FIELD_USERPOINT_HISTORY_NO] = value; }
		}
		/// <summary>予備区分1</summary>
		[UpdateData(12, "kbn1")]
		public string Kbn1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_KBN1]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_KBN1] = value; }
		}
		/// <summary>予備区分2</summary>
		[UpdateData(13, "kbn2")]
		public string Kbn2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_KBN2]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_KBN2] = value; }
		}
		/// <summary>予備区分3</summary>
		[UpdateData(14, "kbn3")]
		public string Kbn3
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_KBN3]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_KBN3] = value; }
		}
		/// <summary>予備区分4</summary>
		[UpdateData(15, "kbn4")]
		public string Kbn4
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_KBN4]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_KBN4] = value; }
		}
		/// <summary>予備区分5</summary>
		[UpdateData(16, "kbn5")]
		public string Kbn5
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_KBN5]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_KBN5] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(17, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERPOINT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(18, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERPOINT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[UpdateData(19, "last_changed")]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_LAST_CHANGED] = value; }
		}
		/// <summary>ポイント利用可能開始日</summary>
		[UpdateData(20, "effective_date")]
		public DateTime? EffectiveDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERPOINT_EFFECTIVE_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERPOINT_EFFECTIVE_DATE];
			}
			set { this.DataSource[Constants.FIELD_USERPOINT_EFFECTIVE_DATE] = value; }
		}
		#endregion
	}
}
