/*
=========================================================================================================
  Module      : Lpページブロックデザインモデル (LandingPageDesignBlockModel.cs)
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
	/// Lpページブロックデザインモデル
	/// </summary>
	[Serializable]
	public partial class LandingPageDesignBlockModel : ModelBase<LandingPageDesignBlockModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LandingPageDesignBlockModel()
		{
			this.PageId = "";
			this.DesignType = LandingPageConst.PAGE_DESIGN_TYPE_PC;
			this.BlockIndex = 0;
			this.BlockClassName = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageDesignBlockModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageDesignBlockModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID] = value; }
		}
		/// <summary>デザインタイプ</summary>
		public string DesignType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DESIGN_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DESIGN_TYPE] = value; }
		}
		/// <summary>ブロックインデックス</summary>
		public int BlockIndex
		{
			get { return int.Parse(this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_BLOCK_INDEX].ToString()); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_BLOCK_INDEX] = value; }
		}
		/// <summary>ブロッククラス名</summary>
		public string BlockClassName
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_BLOCK_CLASS_NAME]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_BLOCK_CLASS_NAME] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEDESIGNBLOCK_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
