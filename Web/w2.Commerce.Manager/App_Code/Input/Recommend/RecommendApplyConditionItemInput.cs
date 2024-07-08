/*
=========================================================================================================
  Module      : レコメンド適用条件アイテム入力クラス (RecommendApplyConditionItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Input;
using w2.Domain.Recommend;
using w2.Domain.Recommend.Helper;

/// <summary>
/// レコメンド適用条件アイテム入力クラス
/// </summary>
[Serializable]
public class RecommendApplyConditionItemInput
{
	/// <summary>データソース</summary>
	Hashtable DataSource { get; set; }

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public RecommendApplyConditionItemInput()
	{
		this.DataSource = new Hashtable();
		this.RecommendApplyConditionType = Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY;
		this.RecommendApplyConditionItemType = Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_NORMAL;
		this.RecommendApplyConditionItemUnitType = Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_PRODUCT;
		this.RecommendApplyConditionItemSortNo = "1";
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="container">レコメンド適用条件アイテム</param>
	public RecommendApplyConditionItemInput(RecommendApplyConditionItemContainer container)
		: this()
	{
		this.ShopId = container.ShopId;
		this.RecommendId = container.RecommendId;
		this.RecommendApplyConditionType = container.RecommendApplyConditionType;
		this.RecommendApplyConditionItemType = container.RecommendApplyConditionItemType;
		this.RecommendApplyConditionItemUnitType = container.RecommendApplyConditionItemUnitType;
		this.RecommendApplyConditionItemSortNo = container.RecommendApplyConditionItemSortNo.ToString();
		this.RecommendApplyConditionItemProductId = container.RecommendApplyConditionItemProductId;
		this.RecommendApplyConditionItemVariationId = container.RecommendApplyConditionItemVariationId;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		this.ProductName = container.ProductName;
		this.VariationName1 = container.VariationName1;
		this.VariationName2 = container.VariationName2;
		this.VariationName3 = container.VariationName3;
		this.FixedPurchaseFlg = container.FixedPurchaseFlg;
		this.SubscriptionBoxFlg = container.SubscriptionBoxFlg;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <param name="recommend">レコメンド設定</param>
	/// <returns>モデル</returns>
	public RecommendApplyConditionItemModel CreateModel(RecommendModel recommend)
	{
		var model = new RecommendApplyConditionItemModel
		{
			ShopId = recommend.ShopId,
			RecommendId = recommend.RecommendId,
			RecommendApplyConditionType = this.RecommendApplyConditionType,
			RecommendApplyConditionItemType = this.RecommendApplyConditionItemType,
			RecommendApplyConditionItemUnitType = this.RecommendApplyConditionItemUnitType,
			RecommendApplyConditionItemProductId = this.RecommendApplyConditionItemProductId,
			RecommendApplyConditionItemVariationId = this.RecommendApplyConditionItemVariationId,
			RecommendApplyConditionItemSortNo = int.Parse(this.RecommendApplyConditionItemSortNo),
			LastChanged = recommend.LastChanged
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	public void Validate()
	{
		// エラーメッセージを初期化
		this.ErrorMessages = string.Empty;

		//通常商品として選択できるか
		if (this.IsNormal && (this.IsFixedPurchaseFlgOnly || this.IsSubscriptionBoxFlgOnly))
		{
			this.ErrorMessages +=
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEMS_INVALID_NORMAL).Replace("@@ 1 @@", this.CreateProductJointName());
		}

		//定期商品として選択できるか
		if (this.IsFixedPurchase && (this.IsFixedPurchaseFlgInvalid || this.IsSubscriptionBoxFlgOnly))
		{
			this.ErrorMessages +=
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEMS_INVALID_FIXED_PURCHASE).Replace("@@ 1 @@", this.CreateProductJointName());
		}

		//頒布会商品として選択できるか
		if (this.IsSubscriptionBox && (this.IsSubscriptionBoxFlgInvalid || this.IsFixedPurchaseFlgInvalid))
		{
			this.ErrorMessages +=
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEMS_INVALID_SUBSCRIPTION_BOX).Replace("@@ 1 @@", this.CreateProductJointName());
		}
	}

	/// <summary>
	/// 商品＋バリエーション名作成
	/// </summary>
	/// <returns>商品＋バリエーション名</returns>
	public string CreateProductJointName()
	{
		if (this.IsRecommendApplyConditionItemUnitTypeProduct)
		{
			return this.ProductName;
		}

		return ProductCommon.CreateProductJointName(this.ProductName, this.VariationName1, this.VariationName2, this.VariationName3);
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID] = value; }
	}
	/// <summary>レコメンドID</summary>
	public string RecommendId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID] = value; }
	}
	/// <summary>レコメンド適用条件種別</summary>
	public string RecommendApplyConditionType
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE] = value; }
	}
	/// <summary>レコメンド適用条件アイテム種別</summary>
	public string RecommendApplyConditionItemType
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE] = value; }
	}
	/// <summary>レコメンド適用条件アイテム単位種別</summary>
	public string RecommendApplyConditionItemUnitType
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE] = value; }
	}
	/// <summary>レコメンド適用条件アイテム商品ID</summary>
	public string RecommendApplyConditionItemProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_PRODUCT_ID] = value; }
	}
	/// <summary>レコメンドアイテム商品バリエーションID</summary>
	public string RecommendApplyConditionItemVariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_VARIATION_ID] = value; }
	}
	/// <summary>レコメンド適用条件アイテム並び順</summary>
	public string RecommendApplyConditionItemSortNo
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_SORT_NO]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_SORT_NO] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_LAST_CHANGED] = value; }
	}
	/// <summary>
	/// レコメンド適用条件アイテム単位種別テキスト
	/// </summary>
	public string RecommendApplyConditionItemUnitTypeText
	{
		get
		{
			return ValueText.GetValueText(Constants.TABLE_RECOMMENDAPPLYCONDITIONITEM, Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE, this.RecommendApplyConditionItemUnitType);
		}
	}
	/// <summary>
	/// 過去注文もしくはカートで購入している？
	/// </summary>
	public bool IsRecommendApplyConditionTypeBuy
	{
		get { return (this.RecommendApplyConditionType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY); }
	}
	/// <summary>
	/// 過去注文もしくはカートで購入していない？
	/// </summary>
	public bool IsRecommendApplyConditionTypeNotBuy
	{
		get { return (this.RecommendApplyConditionType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY); }
	}
	/// <summary>通常商品？</summary>
	public bool IsNormal
	{
		get { return this.RecommendApplyConditionItemType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_NORMAL; }
	}
	/// <summary>定期商品？</summary>
	public bool IsFixedPurchase
	{
		get { return this.RecommendApplyConditionItemType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_FIXED_PURCHASE; }
	}
	/// <summary>頒布会商品？</summary>
	public bool IsSubscriptionBox
	{
		get { return this.RecommendApplyConditionItemType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_SUBSCRIPTION_BOX; }
	}
	/// <summary>
	/// 商品指定？
	/// </summary>
	public bool IsRecommendApplyConditionItemUnitTypeProduct
	{
		get { return (this.RecommendApplyConditionItemUnitType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_PRODUCT); }
	}
	/// <summary>
	/// 商品バリエーション指定？
	/// </summary>
	public bool IsRecommendApplyConditionItemUnitTypeVariation
	{
		get { return (this.RecommendApplyConditionItemUnitType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_VARIATION); }
	}
	/// <summary>商品名</summary>
	public string ProductName
	{
		get { return (string)this.DataSource["product_name"]; }
		set { this.DataSource["product_name"] = value; }
	}
	/// <summary>バリエーション名1</summary>
	public string VariationName1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = value; }
	}
	/// <summary>バリエーション名2</summary>
	public string VariationName2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = value; }
	}
	/// <summary>バリエーション名3</summary>
	public string VariationName3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = value; }
	}
	/// <summary>定期購入フラグ</summary>
	public string FixedPurchaseFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
	}
	/// <summary>定期購入のみ有効？</summary>
	public bool IsFixedPurchaseFlgOnly
	{
		get { return ((this.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY) && (this.IsSubscriptionBoxFlgOnly == false)); }
	}
	/// <summary>定期購入不可？</summary>
	public bool IsFixedPurchaseFlgInvalid
	{
		get { return (this.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID); }
	}
	/// <summary>頒布会購入フラグ</summary>
	public string SubscriptionBoxFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] = value; }
	}
	/// <summary>頒布会購入のみ有効？</summary>
	public bool IsSubscriptionBoxFlgOnly
	{
		get { return (this.SubscriptionBoxFlg == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY); }
	}
	/// <summary>頒布会購入不可？</summary>
	public bool IsSubscriptionBoxFlgInvalid
	{
		get
		{
			if (this.IsFixedPurchaseFlgInvalid) return true;
			return (this.SubscriptionBoxFlg == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID);
		}
	}
	/// <summary>エラーメッセージ</summary>
	public string ErrorMessages
	{
		get { return StringUtility.ToEmpty(this.DataSource["ErrorMessages"]); }
		set { this.DataSource["ErrorMessages"] = value; }
	}
	#endregion
}
