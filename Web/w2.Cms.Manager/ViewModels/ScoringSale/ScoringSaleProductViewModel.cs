/*
=========================================================================================================
  Module      : Scoring Sale Product View Model(ScoringSaleProductViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Domain.ScoringSale;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale product view model
	/// </summary>
	[Serializable]
	public class ScoringSaleProductViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleProductViewModel()
		{
			this.ScoringSaleId = string.Empty;
			this.Conditions = new ScoringSaleResultConditionViewModel[] { new ScoringSaleResultConditionViewModel() };
			this.Quantity = 1;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="model">Scoring sale product model</param>
		public ScoringSaleProductViewModel(ScoringSaleProductModel model)
		{
			this.ScoringSaleId = model.ScoringSaleId;
			this.ProductName = model.ProductName;
			this.ImageUrl = model.ProductImage;
			this.Quantity = model.Quantity;
			this.ShopId = model.ShopId;
			this.ProductId = model.ProductId;
			this.VariationId = model.VariationId;
			this.FixedPurchaseFlg = model.FixedPurchaseFlg;
			this.Conditions = model.ScoringSaleResultConditions
				.Select(condition => new ScoringSaleResultConditionViewModel(condition))
				.ToArray();
		}

		/// <summary>Scoring sale id</summary>
		public string ScoringSaleId { get; set; }
		/// <summary>Product name</summary>
		public string ProductName { get; set; }
		/// <summary>Image url</summary>
		public string ImageUrl { get; set; }
		/// <summary>Quantity</summary>
		public int Quantity { get; set; }
		/// <summary>Shop id</summary>
		public string ShopId { get; set; }
		/// <summary>Product id</summary>
		public string ProductId { get; set; }
		/// <summary>Variation id</summary>
		public string VariationId { get; set; }
		/// <summary>Fixed purchase flag</summary>
		public bool FixedPurchaseFlg { get; set; }
		/// <summary>Condition type group</summary>
		public string ConditionTypeGroup
		{
			get
			{
				var conditionType = Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR;
				if (this.Conditions.Length == 0) return conditionType;
				if (this.Conditions[0].IsGroup == false)
				{
					conditionType = (this.Conditions[0].Condition == Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR)
						? Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND
						: Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR;
					return conditionType;
				}
				conditionType = this.Conditions[0].Condition;
				return conditionType;
			}
		}
		/// <summary>Condition type non group</summary>
		public string ConditionTypeNonGroup
		{
			get
			{
				var conditionType = Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND;
				if (this.Conditions.Length == 0) return conditionType;
				if (this.Conditions[0].IsGroup)
				{
					conditionType = (this.Conditions[0].Condition == Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR)
						? Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND
						: Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR;
					return conditionType;
				}
				conditionType = this.Conditions[0].Condition;
				return conditionType;
			}
		}
		/// <summary>Conditions</summary>
		public ScoringSaleResultConditionViewModel[] Conditions;
	}
}