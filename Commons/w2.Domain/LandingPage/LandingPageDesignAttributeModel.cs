/*
=========================================================================================================
  Module      : Lpページ属性デザインモデル (LandingPageDesignAttributeModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページ属性デザインモデル
	/// </summary>
	[Serializable]
	public partial class LandingPageDesignAttributeModel : ModelBase<LandingPageDesignAttributeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LandingPageDesignAttributeModel()
		{
			this.PageId = "";
			this.DesignType = LandingPageConst.PAGE_DESIGN_TYPE_PC;
			this.BlockIndex = 0;
			this.ElementIndex = 0;
			this.Attribute = "";
			this.Value = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageDesignAttributeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageDesignAttributeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_PAGE_ID] = value; }
		}
		/// <summary>デザインタイプ</summary>
		public string DesignType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_DESIGN_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_DESIGN_TYPE] = value; }
		}
		/// <summary>ブロックインデックス</summary>
		public int BlockIndex
		{
			get { return int.Parse(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_BLOCK_INDEX].ToString()); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_BLOCK_INDEX] = value; }
		}
		/// <summary>要素インデックス</summary>
		public int ElementIndex
		{
			get { return int.Parse(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_ELEMENT_INDEX].ToString()); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_ELEMENT_INDEX] = value; }
		}
		/// <summary>属性</summary>
		public string Attribute
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE] = value; }
		}
		/// <summary>値</summary>
		public string Value
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_VALUE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_VALUE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNATTRIBUTE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
