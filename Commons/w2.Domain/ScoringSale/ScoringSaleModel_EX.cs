/*
=========================================================================================================
  Module      : ScoringSaleモデル (ScoringSaleModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleモデル
	/// </summary>
	public partial class ScoringSaleModel
	{
		#region プロパティ
		/// <summary>Is public status</summary>
		public bool IsPublicStatus
		{
			get
			{
				if (this.PublishStatus == Constants.FLG_SCORINGSALE_PUBLISH_STATUS_UNPUBLISHED) return false;

				if ((this.PublicStartDatetime.HasValue && (DateTime.Now < this.PublicStartDatetime.Value))
					|| (this.PublicEndDatetime.HasValue && (DateTime.Now > this.PublicEndDatetime.Value)))
				{
					return false;
				}

				return true;
			}
		}
		/// <summary>Scoring sale question page model</summary>
		public ScoringSaleQuestionPageModel[] ScoringSaleQuestionPages { get; set; }
		/// <summary>Scoring sale products</summary>
		public ScoringSaleProductModel[] ScoringSaleProducts { get; set; }
		/// <summary>Score axis title</summary>
		public string ScoreAxisTitle { get; set; }
		#endregion
	}
}
