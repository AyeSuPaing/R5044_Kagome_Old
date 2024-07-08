/*
=========================================================================================================
  Module      : 特集エリアマスタモデル (FeatureAreaModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアマスタモデル
	/// </summary>
	[Serializable]
	public partial class FeatureAreaModel : ModelBase<FeatureAreaModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FeatureAreaModel()
		{
			this.AreaId = "";
			this.AreaName = "";
			this.AreaTypeId = "";
			this.InternalMemo = "";
			this.SideMaxCount = "";
			this.SideTurn = "";
			this.SliderCount = "";
			this.SliderScrollCount = "";
			this.SliderScrollAuto = "";
			this.SliderScrollInterval = "";
			this.SliderArrow = "";
			this.SliderDot = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>特集エリアID</summary>
		public string AreaId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_AREA_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_AREA_ID] = value; }
		}
		/// <summary>特集エリア名</summary>
		public string AreaName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_AREA_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_AREA_NAME] = value; }
		}
		/// <summary>特集エリアタイプ</summary>
		public string AreaTypeId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_AREA_TYPE_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_AREA_TYPE_ID] = value; }
		}
		/// <summary>内部用メモ</summary>
		public string InternalMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_INTERNAL_MEMO]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_INTERNAL_MEMO] = value; }
		}
		/// <summary>横並び最大数</summary>
		public string SideMaxCount
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SIDE_MAX_COUNT]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SIDE_MAX_COUNT] = value; }
		}
		/// <summary>折り返し設定</summary>
		public string SideTurn
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SIDE_TURN]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SIDE_TURN] = value; }
		}
		/// <summary>スライド数</summary>
		public string SliderCount
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_COUNT] = value; }
		}
		/// <summary>スクロールのスライド数</summary>
		public string SliderScrollCount
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_SCROLL_COUNT]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_SCROLL_COUNT] = value; }
		}
		/// <summary>自動スクロール</summary>
		public string SliderScrollAuto
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_SCROLL_AUTO]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_SCROLL_AUTO] = value; }
		}
		/// <summary>自動スクロールの間隔</summary>
		public string SliderScrollInterval
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_SCROLL_INTERVAL]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_SCROLL_INTERVAL] = value; }
		}
		/// <summary>矢印表示</summary>
		public string SliderArrow
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_ARROW]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_ARROW] = value; }
		}
		/// <summary>ドット表示</summary>
		public string SliderDot
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_DOT]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_SLIDER_DOT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREAREA_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREAREA_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREA_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREA_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
