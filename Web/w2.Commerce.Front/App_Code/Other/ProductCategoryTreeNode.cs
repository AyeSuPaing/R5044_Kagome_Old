/*
=========================================================================================================
  Module      : 商品カテゴリツリーノードクラス(ProductCategoryTreeNode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

///*********************************************************************************************
/// <summary>
/// 商品カテゴリツリーノードクラス
/// </summary>
///*********************************************************************************************
[Serializable]
public class ProductCategoryTreeNode
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="category">カテゴリデータ</param>
	public ProductCategoryTreeNode(DataRowView category)
	{
		this.ShopId = (string)category[Constants.FIELD_PRODUCTCATEGORY_SHOP_ID];
		this.CategoryId = (string)category[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID];
		this.CategoryName = (string)category[Constants.FIELD_PRODUCTCATEGORY_NAME];
		this.CategoryUrl = (string)category[Constants.FIELD_PRODUCTCATEGORY_URL];
		this.MemberRankId = (string)category[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID];
		this.LowerMemberCanDisplayTreeFlg = (string)category[Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG];
		this.Childs = new List<ProductCategoryTreeNode>();
	}

	/// <summary>店舗ID</summary>
	public string ShopId { get; private set; }
	/// <summary>カテゴリID</summary>
	public string CategoryId { get; private set; }
	/// <summary>カテゴリ名</summary>
	public string CategoryName { get; private set; }
	/// <summary>カテゴリURL</summary>
	public string CategoryUrl { get; private set; }
	/// <summary>閲覧可能会員ランクID</summary>
	public string MemberRankId { get; private set; }
	/// <summary>閲覧可能会員ランクカテゴリツリー表示</summary>
	public string LowerMemberCanDisplayTreeFlg { get; private set; }
	/// <summary>子カテゴリノードリスト</summary>
	public List<ProductCategoryTreeNode> Childs { get; private set; }
}
