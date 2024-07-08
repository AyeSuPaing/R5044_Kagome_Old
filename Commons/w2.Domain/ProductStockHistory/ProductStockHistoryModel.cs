/*
=========================================================================================================
  Module      : 商品在庫履歴マスタモデル (ProductStockHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductStockHistory
{
	/// <summary>
	/// 商品在庫履歴マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductStockHistoryModel : ModelBase<ProductStockHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductStockHistoryModel()
		{
			this.AddStock = 0;
			this.AddRealstock = 0;
			this.AddRealstockB = 0;
			this.AddRealstockC = 0;
			this.AddRealstockReserved = 0;
			this.UpdateStock = null;
			this.UpdateRealstock = null;
			this.UpdateRealstockB = null;
			this.UpdateRealstockC = null;
			this.UpdateRealstockReserved = null;
			this.SyncFlg = 0;
			this.OrderId = string.Empty;
			this.UpdateMemo = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>履歴NO</summary>
		public long HistoryNo
		{
			get { return (long)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_HISTORY_NO]; }
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID] = value; }
		}
		/// <summary>アクションステータス</summary>
		public string ActionStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS] = value; }
		}
		/// <summary>商品在庫増減</summary>
		public int AddStock
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK] = value; }
		}
		/// <summary>実在庫数増減</summary>
		public int AddRealstock
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK] = value; }
		}
		/// <summary>実在庫数B増減</summary>
		public int AddRealstockB
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B] = value; }
		}
		/// <summary>実在庫数C増減</summary>
		public int AddRealstockC
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C] = value; }
		}
		/// <summary>引当済実在庫数増減</summary>
		public int AddRealstockReserved
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED] = value; }
		}
		/// <summary>商品在庫更新</summary>
		public int? UpdateStock
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK] = value; }
		}
		/// <summary>実在庫数更新</summary>
		public int? UpdateRealstock
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK] = value; }
		}
		/// <summary>実在庫数B更新</summary>
		public int? UpdateRealstockB
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_B] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_B];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_B] = value; }
		}
		/// <summary>実在庫数C更新</summary>
		public int? UpdateRealstockC
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_C] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_C];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_C] = value; }
		}
		/// <summary>引当済実在庫数更新</summary>
		public int? UpdateRealstockReserved
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED] = value; }
		}
		/// <summary>在庫更新メモ</summary>
		public string UpdateMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED] = value; }
		}
		/// <summary>同期フラグ</summary>
		public object SyncFlg
		{
			get { return (object)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG] = value; }
		}
		#endregion
	}
}
