/*
=========================================================================================================
  Module      : 定期購入履歴情報モデル (FixedPurchaseHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入履歴情報モデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseHistoryModel : ModelBase<FixedPurchaseHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseHistoryModel()
		{
			this.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS;
			this.BaseTelNo = "";
			this.UpdateOrderCount = null;
			this.UpdateShippedCount = null;
			this.UpdateOrderCountResult = null;
			this.UpdateShippedCountResult = null;
			this.ExternalPaymentCooperationLog = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>定期購入注文履歴NO</summary>
		public long FixedPurchaseHistoryNo
		{
			get { return (long)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_NO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_NO] = value; }
		}
		/// <summary>定期購入履歴区分</summary>
		public string FixedPurchaseHistoryKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN] = value; }
		}
		/// <summary>元電話番号</summary>
		public string BaseTelNo
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_BASE_TEL_NO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_BASE_TEL_NO] = value; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_USER_ID] = value; }
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_ORDER_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_LAST_CHANGED] = value; }
		}
		/// <summary>購入回数(注文基準)更新</summary>
		public int? UpdateOrderCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT] = value; }
		}
		/// <summary>購入回数(出荷基準)更新</summary>
		public int? UpdateShippedCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT] = value; }
		}
		/// <summary>購入回数(注文基準)更新結果</summary>
		public int? UpdateOrderCountResult
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT_RESULT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT_RESULT];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT_RESULT] = value; }
		}
		/// <summary>購入回数(出荷基準)更新結果</summary>
		public int? UpdateShippedCountResult
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT_RESULT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT_RESULT];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT_RESULT] = value; }
		}
		/// <summary>外部決済連携ログ</summary>
		public string ExternalPaymentCooperationLog
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_EXTERNAL_PAYMENT_COOPERATION_LOG]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEHISTORY_EXTERNAL_PAYMENT_COOPERATION_LOG] = value; }
		}
		#endregion
	}
}