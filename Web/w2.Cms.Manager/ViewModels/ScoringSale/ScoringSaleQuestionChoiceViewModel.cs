/*
=========================================================================================================
  Module      : Scoring Sale Question Choice View Model (ScoringSaleQuestionChoiceViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.Codes;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale question choice view model
	/// </summary>
	public class ScoringSaleQuestionChoiceViewModel
	{
		#region Property
		/// <summary>質問ID</summary>
		public string QuestionId { get; set; }
		/// <summary>枝番</summary>
		public string BranchNo { get; set; }
		/// <summary>選択肢文</summary>
		public string QuestionChoiceStatement { get; set; }
		/// <summary>選択肢画像</summary>
		public string QuestionChoiceStatementImgPath { get; set; }
		/// <summary>軸加算値１</summary>
		public string AxisAdditional1 { get; set; }
		/// <summary>軸加算値２</summary>
		public string AxisAdditional2 { get; set; }
		/// <summary>軸加算値３</summary>
		public string AxisAdditional3 { get; set; }
		/// <summary>軸加算値４</summary>
		public string AxisAdditional4 { get; set; }
		/// <summary>軸加算値５</summary>
		public string AxisAdditional5 { get; set; }
		/// <summary>軸加算値６</summary>
		public string AxisAdditional6 { get; set; }
		/// <summary>軸加算値７</summary>
		public string AxisAdditional7 { get; set; }
		/// <summary>軸加算値８</summary>
		public string AxisAdditional8 { get; set; }
		/// <summary>軸加算値９</summary>
		public string AxisAdditional9 { get; set; }
		/// <summary>軸加算値１０</summary>
		public string AxisAdditional10 { get; set; }
		/// <summary>軸加算値１１</summary>
		public string AxisAdditional11 { get; set; }
		/// <summary>軸加算値１２</summary>
		public string AxisAdditional12 { get; set; }
		/// <summary>軸加算値１３</summary>
		public string AxisAdditional13 { get; set; }
		/// <summary>軸加算値１４</summary>
		public string AxisAdditional14 { get; set; }
		/// <summary>軸加算値１５</summary>
		public string AxisAdditional15 { get; set; }
		/// <summary>Image source</summary>
		public string ImageSrc
		{
			get
			{
				var imageSrc = (string.IsNullOrEmpty(this.QuestionChoiceStatementImgPath) == false)
					? string.Format(
						"{0}{1}",
						Constants.PATH_ROOT_FRONT_PC,
						this.QuestionChoiceStatementImgPath)
					: string.Empty;
				return imageSrc;
			}
		}
		#endregion
	}
}
