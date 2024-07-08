/*
=========================================================================================================
  Module      : コーディネートアイテムモデル (CoordinateItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Coordinate
{
	/// <summary>
	/// コーディネートアイテムモデル
	/// </summary>
	[Serializable]
	public partial class CoordinateItemModel : ModelBase<CoordinateItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CoordinateItemModel()
		{
			this.CoordinateId = "";
			this.ItemNo = 1;
			this.ItemKbn = "";
			this.ItemId = "";
			this.ItemId2 = "";
			this.ItemName = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>コーディネートID</summary>
		public string CoordinateId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_COORDINATE_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_COORDINATE_ID] = value; }
		}
		/// <summary>アイテムNO</summary>
		public int ItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NO] = value; }
		}
		/// <summary>アイテム区分</summary>
		public string ItemKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_KBN]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_KBN] = value; }
		}
		/// <summary>アイテムID</summary>
		public string ItemId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID] = value; }
		}
		/// <summary>アイテムID2</summary>
		public string ItemId2
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID2]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID2] = value; }
		}
		/// <summary>アイテム名</summary>
		public string ItemName
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NAME]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NAME] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COORDINATEITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COORDINATEITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
