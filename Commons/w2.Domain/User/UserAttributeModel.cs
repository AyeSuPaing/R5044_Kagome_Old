/*
=========================================================================================================
  Module      : ユーザー属性マスタモデル (UserAttributeModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.User
{
	/// <summary>
	/// ユーザー属性マスタモデル
	/// </summary>
	[Serializable]
	public partial class UserAttributeModel : ModelBase<UserAttributeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserAttributeModel()
		{
			this.FirstOrderDate = null;
			this.SecondOrderDate = null;
			this.LastOrderDate = null;
			this.OrderAmountOrderAll = 0;
			this.OrderAmountOrderFp = 0;
			this.OrderCountOrderAll = 0;
			this.OrderCountOrderFp = 0;
			this.OrderAmountShipAll = 0;
			this.OrderAmountShipFp = 0;
			this.OrderCountShipAll = 0;
			this.OrderCountShipFp = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserAttributeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserAttributeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERATTRIBUTE_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_USER_ID] = value; }
		}
		/// <summary>初回購入日</summary>
		public DateTime? FirstOrderDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERATTRIBUTE_FIRST_ORDER_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERATTRIBUTE_FIRST_ORDER_DATE];
			}
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_FIRST_ORDER_DATE] = value; }
		}
		/// <summary>２回目購入日</summary>
		public DateTime? SecondOrderDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERATTRIBUTE_SECOND_ORDER_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERATTRIBUTE_SECOND_ORDER_DATE];
			}
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_SECOND_ORDER_DATE] = value; }
		}
		/// <summary>最終購入日</summary>
		public DateTime? LastOrderDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERATTRIBUTE_LAST_ORDER_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERATTRIBUTE_LAST_ORDER_DATE];
			}
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_LAST_ORDER_DATE] = value; }
		}
		/// <summary>在籍期間(日)</summary>
		public int? EnrollmentDays
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERATTRIBUTE_ENROLLMENT_DAYS] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_USERATTRIBUTE_ENROLLMENT_DAYS];
			}
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ENROLLMENT_DAYS] = value; }
		}
		/// <summary>離脱期間(日)</summary>
		public int? AwayDays
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERATTRIBUTE_AWAY_DAYS] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_USERATTRIBUTE_AWAY_DAYS];
			}
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_AWAY_DAYS] = value; }
		}
		/// <summary>累計購入金額（注文基準・全体）</summary>
		public decimal OrderAmountOrderAll
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_ORDER_ALL]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_ORDER_ALL] = value; }
		}
		/// <summary>累計購入金額（注文基準・定期のみ）</summary>
		public decimal OrderAmountOrderFp
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_ORDER_FP]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_ORDER_FP] = value; }
		}
		/// <summary>累計購入回数（注文基準・全体）</summary>
		public int OrderCountOrderAll
		{
			get { return (int)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_ORDER_ALL]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_ORDER_ALL] = value; }
		}
		/// <summary>累計購入回数（注文基準・定期のみ）</summary>
		public int OrderCountOrderFp
		{
			get { return (int)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_ORDER_FP]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_ORDER_FP] = value; }
		}
		/// <summary>累計購入金額（出荷基準・全体）</summary>
		public decimal OrderAmountShipAll
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_SHIP_ALL]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_SHIP_ALL] = value; }
		}
		/// <summary>累計購入金額（出荷基準・定期のみ）</summary>
		public decimal OrderAmountShipFp
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_SHIP_FP]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_AMOUNT_SHIP_FP] = value; }
		}
		/// <summary>累計購入回数（出荷基準・全体）</summary>
		public int OrderCountShipAll
		{
			get { return (int)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_SHIP_ALL]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_SHIP_ALL] = value; }
		}
		/// <summary>累計購入回数（出荷基準・定期のみ）</summary>
		public int OrderCountShipFp
		{
			get { return (int)this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_SHIP_FP]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_ORDER_COUNT_SHIP_FP] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERATTRIBUTE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERATTRIBUTE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERATTRIBUTE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_LAST_CHANGED] = value; }
		}
		/// <summary>CPMクラスタID</summary>
		public string CpmClusterId
		{
			get
			{
				return StringUtility.ToEmpty(CpmClusterAttribute1)
				       + StringUtility.ToEmpty(CpmClusterAttribute2);
			}
		}
		/// <summary>CPMクラスタ属性1</summary>
		public string CpmClusterAttribute1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE1]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE1] = value; }
		}
		/// <summary>CPMクラスタ属性2</summary>
		public string CpmClusterAttribute2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE2]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE2] = value; }
		}
		/// <summary>以前のCPMクラスタ属性1</summary>
		public string CpmClusterAttribute1Before
		{
			get { return (string)this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE1_BEFORE]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE1_BEFORE] = value; }
		}
		/// <summary>以前のCPMクラスタ属性2</summary>
		public string CpmClusterAttribute2Before
		{
			get { return (string)this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE2_BEFORE]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE2_BEFORE] = value; }
		}
		/// <summary>以前のCPMクラスタID</summary>
		public string CpmClusterIdBefore
		{
			get
			{
				return StringUtility.ToEmpty(CpmClusterAttribute1Before)
					   + StringUtility.ToEmpty(CpmClusterAttribute2Before);
			}
		}
		/// <summary>CPMクラスタ変更日</summary>
		public DateTime? CpmClusterChangedDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_CHANGED_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_CHANGED_DATE];
			}
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_CHANGED_DATE] = value; }
		}
		#endregion
	}
}
