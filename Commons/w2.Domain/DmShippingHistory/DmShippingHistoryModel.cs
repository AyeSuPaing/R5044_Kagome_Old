/*
=========================================================================================================
  Module      : DM発送履歴モデル (DmShippingHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.DmShippingHistory
{
	/// <summary>
	/// DM発送履歴モデル
	/// </summary>
	[Serializable]
	public partial class DmShippingHistoryModel : ModelBase<DmShippingHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DmShippingHistoryModel()
		{
			this.ValidDateFrom = null;
			this.ValidDateTo = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DmShippingHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DmShippingHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_USER_ID] = value; }
		}
		/// <summary>DMコード</summary>
		public string DmCode
		{
			get { return (string)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DM_CODE]; }
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DM_CODE] = value; }
		}
		/// <summary>DM発送日</summary>
		public DateTime DmShippingDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DM_SHIPPING_DATE]; }
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DM_SHIPPING_DATE] = value; }
		}
		/// <summary>DM名</summary>
		public string DmName
		{
			get { return (string)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DM_NAME]; }
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DM_NAME] = value; }
		}
		/// <summary>有効期間(From)</summary>
		public DateTime? ValidDateFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_VALID_DATE_FROM] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_VALID_DATE_FROM];
			}
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_VALID_DATE_FROM] = value; }
		}
		/// <summary>有効期限(To)</summary>
		public DateTime? ValidDateTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_VALID_DATE_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_VALID_DATE_TO];
			}
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_VALID_DATE_TO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_DMSHIPPINGHISTORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}