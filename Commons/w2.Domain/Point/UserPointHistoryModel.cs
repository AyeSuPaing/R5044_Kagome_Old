/*
=========================================================================================================
  Module      : ユーザポイント履歴モデル (UserPointHistoryModel.cs)
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
	/// ユーザポイント履歴モデル
	/// </summary>
	[Serializable]
	public partial class UserPointHistoryModel : ModelBase<UserPointHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserPointHistoryModel()
		{
			this.PointRuleId = string.Empty;
			this.PointRuleKbn = Constants.FLG_USERPOINTHISTORY_POINT_RULE_KBN_BASE;
			this.PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE;
			this.PointType = Constants.FLG_USERPOINT_POINT_TYPE_COMP;
			this.PointInc = 0;
			this.PointExpExtend = DEFAULT_POINT_EXP_EXTEND_STRING;
			this.Kbn1 = string.Empty;
			this.Kbn2 = string.Empty;
			this.Kbn3 = string.Empty;
			this.Kbn4 = string.Empty;
			this.Kbn5 = string.Empty;
			this.Memo = string.Empty;
			this.EffectiveDate = null;
			this.DateCreated = DateTime.Now;
			this.RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_RESTORED;
			this.HistoryGroupNo = Constants.CONST_USERPOINTHISTORY_DEFAULT_GROUP_NO;
			this.CartId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserPointHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserPointHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_USER_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int HistoryNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERPOINTHISTORY_HISTORY_NO]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_HISTORY_NO] = value; }
		}
		/// <summary>対象年</summary>
		public object TgtYear
		{
			get { return (object)this.DataSource[Constants.FIELD_USERPOINTHISTORY_TGT_YEAR]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_TGT_YEAR] = value; }
		}
		/// <summary>対象月</summary>
		public object TgtMonth
		{
			get { return (object)this.DataSource[Constants.FIELD_USERPOINTHISTORY_TGT_MONTH]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_TGT_MONTH] = value; }
		}
		/// <summary>対象日</summary>
		public object TgtDay
		{
			get { return (object)this.DataSource[Constants.FIELD_USERPOINTHISTORY_TGT_DAY]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_TGT_DAY] = value; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_DEPT_ID] = value; }
		}
		/// <summary>ポイントルールID</summary>
		public string PointRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_RULE_ID] = value; }
		}
		/// <summary>ポイントルール区分</summary>
		public string PointRuleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_RULE_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_RULE_KBN] = value; }
		}
		/// <summary>ポイント区分</summary>
		public string PointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_KBN] = value; }
		}
		/// <summary>ポイント種別</summary>
		public string PointType
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_TYPE]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_TYPE] = value; }
		}
		/// <summary>ポイント加算区分</summary>
		public string PointIncKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN] = value; }
		}
		/// <summary>ポイント加算数</summary>
		public decimal PointInc
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_INC]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_INC] = value; }
		}
		/// <summary>ポイント有効期限延長</summary>
		public string PointExpExtend
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_EXP_EXTEND]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_POINT_EXP_EXTEND] = value; }
		}
		/// <summary>ユーザ最新有効期限</summary>
		public DateTime? UserPointExp
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERPOINTHISTORY_USER_POINT_EXP] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERPOINTHISTORY_USER_POINT_EXP];
			}
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_USER_POINT_EXP] = value; }
		}
		/// <summary>予備区分1</summary>
		public string Kbn1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN1]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN1] = value; }
		}
		/// <summary>予備区分2</summary>
		public string Kbn2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN2]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN2] = value; }
		}
		/// <summary>予備区分3</summary>
		public string Kbn3
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN3]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN3] = value; }
		}
		/// <summary>予備区分4</summary>
		public string Kbn4
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN4]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN4] = value; }
		}
		/// <summary>予備区分5</summary>
		public string Kbn5
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN5]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_KBN5] = value; }
		}
		/// <summary>メモ</summary>
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_MEMO]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_MEMO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERPOINTHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_LAST_CHANGED] = value; }
		}
		/// <summary>ポイント利用可能開始日</summary>
		public DateTime? EffectiveDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERPOINTHISTORY_EFFECTIVE_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERPOINTHISTORY_EFFECTIVE_DATE];
			}
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_EFFECTIVE_DATE] = value; }
		}
		/// <summary>ポイント復元処理済フラグ</summary>
		public string RestoredFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_RESTORED_FLG]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_RESTORED_FLG] = value; }
		}
		/// <summary>履歴グループ番号</summary>
		public int HistoryGroupNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERPOINTHISTORY_HISTORY_GROUP_NO]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_HISTORY_GROUP_NO] = value; }
		}
		/// <summary>カートID</summary>
		public string CartId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERPOINTHISTORY_CART_ID]; }
			set { this.DataSource[Constants.FIELD_USERPOINTHISTORY_CART_ID] = value; }
		}
		#endregion
	}
}
