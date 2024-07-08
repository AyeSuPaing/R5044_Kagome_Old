/*
=========================================================================================================
  Module      : 注文拡張ステータス設定マスタモデル (OrderExtendStatusSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OrderExtendStatusSetting
{
	/// <summary>
	/// 注文拡張ステータス設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class OrderExtendStatusSettingModel : ModelBase<OrderExtendStatusSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderExtendStatusSettingModel()
		{
			this.ShopId = "";
			this.ExtendStatusNo = 1;
			this.ExtendStatusName = "";
			this.ExtendStatusDiscription = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderExtendStatusSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderExtendStatusSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID] = value; }
		}
		/// <summary>拡張ステータス番号</summary>
		public int ExtendStatusNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO] = value; }
		}
		/// <summary>拡張ステータス名称</summary>
		public string ExtendStatusName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME] = value; }
		}
		/// <summary>拡張ステータス説明</summary>
		public string ExtendStatusDiscription
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_DISCRIPTION] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}