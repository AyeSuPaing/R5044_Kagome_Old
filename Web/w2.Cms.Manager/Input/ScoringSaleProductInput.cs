/*
=========================================================================================================
  Module      : Scoring Sale Product Input (ScoringSaleProductInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale product input
/// </summary>
public class ScoringSaleProductInput : InputBase<ScoringSaleProductModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleProductInput()
	{
		this.BranchNo = Constants.CONST_SCORINGSALE_BRANCH_NO_DEFAULT;
		this.ProductId = string.Empty;
		this.Quantity = Constants.CONST_SCORINGSALE_QUANTITY_DEFAULT;
	}
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale product model</param>
	public ScoringSaleProductInput(ScoringSaleProductModel model)
		: this()
	{
		this.ScoringSaleResultCondition = new ScoringSaleResultConditionInput[] { };
		this.DataSource = new Hashtable();
		this.ScoringSaleId = model.ScoringSaleId;
		this.BranchNo = model.BranchNo.ToString();
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.VariationId = model.VariationId;
		this.Quantity = model.Quantity.ToString();
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>Scoring sale product model</returns>
	public override ScoringSaleProductModel CreateModel()
	{
		var model = new ScoringSaleProductModel
		{
			ScoringSaleId = this.ScoringSaleId,
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			VariationId = this.VariationId,
			BranchNo = int.Parse(this.BranchNo),
			Quantity = int.Parse(this.Quantity),
			ScoringSaleResultConditions = (this.ScoringSaleResultCondition != null)
				? this.ScoringSaleResultCondition
					.Select(item =>
					{
						item.ScoringSaleId = this.ScoringSaleId;
						var itemModel = item.CreateModel();
						return itemModel;
					})
					.ToArray()
				: new ScoringSaleResultConditionModel[0],
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <param name="isRegister">Is register</param>
	/// <returns>Error messages</returns>
	public List<string> Validate(bool isRegister)
	{
		var checkKbn = isRegister ? "ScoringSaleProductRegister" : "ScoringSaleProductModify";
		var errorMessageList = Validator.Validate(checkKbn, this.DataSource)
			.Select(keyValue => keyValue.Value)
			.ToList();
		return errorMessageList;
	}
	#endregion

	#region +Properties
	/// <summary>スコアリング販売ID</summary>
	public string ScoringSaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SCORING_SALE_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SCORING_SALE_ID] = value; }
	}
	/// <summary>枝番</summary>
	public string BranchNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_BRANCH_NO] = value; }
	}
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_PRODUCT_ID] = value; }
	}
	/// <summary>商品バリエーションID</summary>
	public string VariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_VARIATION_ID] = value; }
	}
	/// <summary>個数</summary>
	public string Quantity
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_QUANTITY]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_QUANTITY] = value; }
	}
	/// <summary>Scoring sale result condition</summary>
	public ScoringSaleResultConditionInput[] ScoringSaleResultCondition { get; set; }
	#endregion
}
