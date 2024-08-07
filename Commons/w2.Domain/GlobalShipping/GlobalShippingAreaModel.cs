/*
=========================================================================================================
  Module      : 海外配送エリアモデル (GlobalShippingAreaModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.GlobalShipping
{
	/// <summary>
	/// 海外配送エリアモデル
	/// </summary>
	[Serializable]
	public partial class GlobalShippingAreaModel : ModelBase<GlobalShippingAreaModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public GlobalShippingAreaModel()
		{
			this.ValidFlg = VALID_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalShippingAreaModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalShippingAreaModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>海外配送エリアID</summary>
		public string GlobalShippingAreaId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_ID] = value; }
		}
		/// <summary>海外配送エリア名</summary>
		public string GlobalShippingAreaName
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_NAME]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_NAME] = value; }
		}
		/// <summary>並び順</summary>
		public int SortNo
		{
			get { return (int)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_SORT_NO]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_SORT_NO] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
