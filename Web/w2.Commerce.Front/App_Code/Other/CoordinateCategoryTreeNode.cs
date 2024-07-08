/*
=========================================================================================================
  Module      : コーディネートカテゴリツリーノードクラス(CoordinateCategoryTreeNode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using w2.Domain.CoordinateCategory;

/// <summary>
/// コーディネートカテゴリツリーノードクラス
/// </summary>
[Serializable]
public class CoordinateCategoryTreeNode
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="category">カテゴリデータ</param>
	public CoordinateCategoryTreeNode(CoordinateCategoryModel category)
	{

		this.CategoryId = category.CoordinateCategoryId;
		this.CategoryName = category.CoordinateCategoryName;
		this.Childs = new List<CoordinateCategoryTreeNode>();
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="drv">カテゴリデータ</param>
	public CoordinateCategoryTreeNode(DataRowView drv)
	{
		this.CategoryId = (string)drv[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID];
		this.CategoryName = (string)drv[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME]; 
		this.Childs = new List<CoordinateCategoryTreeNode>();
	}

	/// <summary>店舗ID</summary>
	public string ShopId { get; set; }
	/// <summary>カテゴリID</summary>
	public string CategoryId { get; set; }
	/// <summary>カテゴリ名</summary>
	public string CategoryName { get; set; }
	/// <summary>閲覧可能会員ランクID</summary>
	public string MemberRankId { get; set; }
	/// <summary>子カテゴリノードリスト</summary>
	public List<CoordinateCategoryTreeNode> Childs { get; set; }
}
