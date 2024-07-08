/*
=========================================================================================================
  Module      : ＆mall在庫引当モデル (AndmallInventoryReserveModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.AndmallInventoryReserve
{
	/// <summary>
	/// ＆mall在庫引当モデル
	/// </summary>
	[Serializable]
	public partial class AndmallInventoryReserveModel : ModelBase<AndmallInventoryReserveModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AndmallInventoryReserveModel()
		{
			this.Quantity = 0;
			this.CancelDate = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AndmallInventoryReserveModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AndmallInventoryReserveModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別コード</summary>
		public string IdentificationCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_IDENTIFICATION_CODE]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_IDENTIFICATION_CODE] = value; }
		}
		/// <summary>ショップコード</summary>
		public string AndmallBaseStoreCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_ANDMALL_BASE_STORE_CODE]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_ANDMALL_BASE_STORE_CODE] = value; }
		}
		/// <summary>SKUコード</summary>
		public string SkuId
		{
			get { return (string)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_SKU_ID]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_SKU_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_VARIATION_ID] = value; }
		}
		/// <summary>数量</summary>
		public int Quantity
		{
			get { return (int)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_QUANTITY] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_STATUS]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_STATUS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_DATE_CHANGED] = value; }
		}
		/// <summary>キャンセル日</summary>
		public DateTime? CancelDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_CANCEL_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_CANCEL_DATE];
			}
			set { this.DataSource[Constants.FIELD_ANDMALLINVENTORYRESERVE_CANCEL_DATE] = value; }
		}
		#endregion
	}
}
