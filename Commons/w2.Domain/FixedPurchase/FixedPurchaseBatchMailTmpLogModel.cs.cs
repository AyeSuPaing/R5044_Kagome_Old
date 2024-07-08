/*
=========================================================================================================
  Module      : 定期購入バッチメールの一時ログモデル (FixedPurchaseBatchMailTmpLogModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入バッチメールの一時ログモデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseBatchMailTmpLogModel : ModelBase<FixedPurchaseBatchMailTmpLogModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseBatchMailTmpLogModel()
		{
			this.MasterType = Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseBatchMailTmpLogModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseBatchMailTmpLogModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>一時ログID</summary>
		public int TmpLogId
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_TMP_LOG_ID]; }
		}
		/// <summary>マスタ種別</summary>
		public string MasterType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE] = value; }
		}
		/// <summary>マスタID</summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_ID] = value; }
		}
		/// <summary>実行マスタID</summary>
		public string ActionMasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_ACTION_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_ACTION_MASTER_ID] = value; }
		}
		#endregion
	}
}
