/*
=========================================================================================================
  Module      : Scoring Sale Result Condition View Model (ScoringSaleResultConditionViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Cms.Manager.Codes;
using w2.Domain.ScoringSale;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale product view model
	/// </summary>
	[Serializable]
	public class ScoringSaleResultConditionViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleResultConditionViewModel()
		{
			this.IsGroup = false;
			this.IsShowSelectType = false;
			this.Condition = Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND;
			this.GroupNo = 1;
			this.IsGroupChecked = false;
			this.CanChangeConditionType = true;
			this.ValueCondition = Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="model">Scoring sale result condition model</param>
		public ScoringSaleResultConditionViewModel(ScoringSaleResultConditionModel model)
		{
			this.Condition = model.Condition;
			this.ScoreAxisAxisNo = model.ScoreAxisAxisNo.ToString();
			this.ScoreAxisAxisValueFrom = model.ScoreAxisAxisValueFrom.ToString();
			this.ScoreAxisAxisValueTo = model.ScoreAxisAxisValueTo.ToString();
			this.GroupNo = model.GroupNo;
			this.IsGroup = model.IsGroup;
			this.GroupBrandNo = model.GroupBrandNo;
			var valueCondition = ((this.ScoreAxisAxisValueFrom == Constants.SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MIN)
					&& (this.ScoreAxisAxisValueTo == Constants.SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MAX))
				? Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MAX
				: Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE;

			this.ValueCondition = valueCondition;
		}

		/// <summary>Condition</summary>
		public string Condition { get; set; }
		/// <summary>Score axis axis no</summary>
		public string ScoreAxisAxisNo { get; set; }
		/// <summary>Score axis axis value from</summary>
		public string ScoreAxisAxisValueFrom { get; set; }
		/// <summary>Score axis axis value to</summary>
		public string ScoreAxisAxisValueTo { get; set; }
		/// <summary>Is group</summary>
		public bool IsGroup { get; set; }
		/// <summary>Is show select type</summary>
		public bool IsShowSelectType { get; set; }
		/// <summary>Group no</summary>
		public int GroupNo { get; set; }
		/// <summary>Group brand no</summary>
		public int GroupBrandNo { get; set; }
		/// <summary>Is group checked</summary>
		public bool IsGroupChecked { get; set; }
		/// <summary>Can change condition type</summary>
		public bool CanChangeConditionType { get; set; }
		/// <summary>Value condition</summary>
		public string ValueCondition { get; set; }
	}
}