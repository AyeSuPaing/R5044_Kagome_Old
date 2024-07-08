/*
=========================================================================================================
  Module      : シリアルキー情報モデル (SerialKeyModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.SerialKey
{
	/// <summary>
	/// シリアルキー情報モデル
	/// </summary>
	[Serializable]
	public partial class SerialKeyModel : ModelBase<SerialKeyModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SerialKeyModel()
		{
			this.OrderItemNo = 0;
			this.Status = Constants.FLG_SERIALKEY_STATUS_NOT_RESERVED;
			this.ValidFlg = Constants.FLG_SERIALKEY_VALID_FLG_VALID;
			this.DateDelivered = null;
			this.DownloadCount = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SerialKeyModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SerialKeyModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>シリアルキー </summary>
		public string SerialKey
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_SERIAL_KEY]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_SERIAL_KEY] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_VARIATION_ID] = value; }
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_ORDER_ID] = value; }
		}
		/// <summary>注文商品枝番</summary>
		public int OrderItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SERIALKEY_ORDER_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_ORDER_ITEM_NO] = value; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_USER_ID] = value; }
		}
		/// <summary>状態</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_STATUS]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_STATUS] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_VALID_FLG] = value; }
		}
		/// <summary>引渡日</summary>
		public DateTime? DateDelivered
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SERIALKEY_DATE_DELIVERED] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SERIALKEY_DATE_DELIVERED];
			}
			set { this.DataSource[Constants.FIELD_SERIALKEY_DATE_DELIVERED] = value; }
		}
		/// <summary>回数</summary>
		public int DownloadCount
		{
			get { return (int)this.DataSource[Constants.FIELD_SERIALKEY_DOWNLOAD_COUNT]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_DOWNLOAD_COUNT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SERIALKEY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SERIALKEY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SERIALKEY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SERIALKEY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
