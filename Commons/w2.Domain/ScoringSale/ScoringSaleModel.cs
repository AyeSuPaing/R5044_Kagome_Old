/*
=========================================================================================================
  Module      : ScoringSaleモデル (ScoringSaleModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleモデル
	/// </summary>
	[Serializable]
	public partial class ScoringSaleModel : ModelBase<ScoringSaleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoringSaleModel()
		{
			this.PublishStatus = Constants.FLG_SCORINGSALE_PUBLISH_STATUS_PUBLISHED;
			this.PublicStartDatetime = null;
			this.PublicEndDatetime = null;
			this.TopPageUseFlg = Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON;
			this.TopPageTitle = null;
			this.TopPageSubTitle = null;
			this.TopPageBody = null;
			this.TopPageImgPath = null;
			this.TopPageBtnCaption = null;
			this.RadarChartTitle = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スコアリング販売ID</summary>
		public string ScoringSaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_ID] = value; }
		}
		/// <summary>スコアリング販売タイトル</summary>
		public string ScoringSaleTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_TITLE]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_SCORING_SALE_TITLE] = value; }
		}
		/// <summary>公開状態</summary>
		public string PublishStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_PUBLISH_STATUS]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_PUBLISH_STATUS] = value; }
		}
		/// <summary>公開開始日時</summary>
		public DateTime? PublicStartDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_START_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_START_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_START_DATETIME] = value; }
		}
		/// <summary>公開終了日時</summary>
		public DateTime? PublicEndDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_END_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_END_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_PUBLIC_END_DATETIME] = value; }
		}
		/// <summary>スコア軸ID</summary>
		public string ScoreAxisId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_SCORE_AXIS_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_SCORE_AXIS_ID] = value; }
		}
		/// <summary>テーマカラー</summary>
		public string ThemeColor
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_THEME_COLOR]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_THEME_COLOR] = value; }
		}
		/// <summary>トップページ利用</summary>
		public string TopPageUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_USE_FLG] = value; }
		}
		/// <summary>トップページタイトル</summary>
		public string TopPageTitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_TITLE] = value; }
		}
		/// <summary>トップページサブタイトル</summary>
		public string TopPageSubTitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE] = value; }
		}
		/// <summary>トップページ本文</summary>
		public string TopPageBody
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BODY] = value; }
		}
		/// <summary>トップページ画像</summary>
		public string TopPageImgPath
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH] = value; }
		}
		/// <summary>トップページボタン文言</summary>
		public string TopPageBtnCaption
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION] = value; }
		}
		/// <summary>結果ページタイトル</summary>
		public string ResultPageTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_TITLE]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_TITLE] = value; }
		}
		/// <summary>結果ページ本文HTML(上)</summary>
		public string ResultPageBodyAbove
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_ABOVE]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_ABOVE] = value; }
		}
		/// <summary>結果ページ本文HTML(下)</summary>
		public string ResultPageBodyBelow
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_BELOW]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BODY_BELOW] = value; }
		}
		/// <summary>レーダーチャート利用</summary>
		public string RadarChartUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_USE_FLG] = value; }
		}
		/// <summary>レーダーチャートタイトル</summary>
		public string RadarChartTitle
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_TITLE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_TITLE];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALE_RADAR_CHART_TITLE] = value; }
		}
		/// <summary>結果ページボタン文言</summary>
		public string ResultPageBtnCaption
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BTN_CAPTION]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_RESULT_PAGE_BTN_CAPTION] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
