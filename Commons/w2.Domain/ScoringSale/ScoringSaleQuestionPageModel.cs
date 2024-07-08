/*
=========================================================================================================
  Module      : ScoringSaleQuestionPageモデル (ScoringSaleQuestionPageModel.cs)
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
	/// ScoringSaleQuestionPageモデル
	/// </summary>
	[Serializable]
	public partial class ScoringSaleQuestionPageModel : ModelBase<ScoringSaleQuestionPageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoringSaleQuestionPageModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionPageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionPageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スコアリング販売ID</summary>
		public string ScoringSaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID] = value; }
		}
		/// <summary>ページNo</summary>
		public int PageNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PAGE_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PAGE_NO] = value; }
		}
		/// <summary>前ページボタン文言</summary>
		public string PreviousPageBtnCaption
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PREVIOUS_PAGE_BTN_CAPTION]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PREVIOUS_PAGE_BTN_CAPTION] = value; }
		}
		/// <summary>次ページボタン文言</summary>
		public string NextPageBtnCaption
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_NEXT_PAGE_BTN_CAPTION]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_NEXT_PAGE_BTN_CAPTION] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
