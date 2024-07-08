/*
=========================================================================================================
  Module      : Scoring Sale Product Input (ScoringSaleProductInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
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
		this.Conditions = new ScoringSaleResultConditionInput[0];
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale product model</param>
	public ScoringSaleProductInput(ScoringSaleProductModel model)
		: this()
	{
		this.ScoringSaleId = model.ScoringSaleId;
		this.BranchNo = model.BranchNo;
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.VariationId = model.VariationId;
		this.Quantity = model.Quantity;
	}
	#endregion

	#region +Method
	/// <summary>
	/// CreateModel
	/// </summary>
	/// <returns>Scoring sale product model</returns>
	public override ScoringSaleProductModel CreateModel()
	{
		var model = new ScoringSaleProductModel
		{
			ScoringSaleId = this.ScoringSaleId,
			BranchNo = this.BranchNo,
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			VariationId = this.VariationId,
			Quantity = this.Quantity,
		};
		return model;
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
	public int BranchNo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_BRANCH_NO]; }
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
	public int Quantity
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_QUANTITY]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_QUANTITY] = value; }
	}
	/// <summary>Conditions</summary>
	public ScoringSaleResultConditionInput[] Conditions { get; set; }
	#endregion
}
