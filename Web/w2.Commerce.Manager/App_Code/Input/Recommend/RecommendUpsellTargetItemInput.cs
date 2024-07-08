/*
=========================================================================================================
  Module      : レコメンドアップセル対象アイテム入力クラス (RecommendUpsellTargetItemInput.cs)
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
/// レコメンドアップセル対象アイテム入力クラス
/// </summary>
[Serializable]
public class RecommendUpsellTargetItemInput
{
	/// <summary>データソース</summary>
	Hashtable DataSource { get; set; }

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public RecommendUpsellTargetItemInput()
	{
		this.DataSource = new Hashtable();
		this.RecommendUpsellTargetItemType = Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_NORMAL;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="container">レコメンドアップセル対象アイテム</param>
	public RecommendUpsellTargetItemInput(RecommendUpsellTargetItemContainer container)
		: this()
	{
		this.ShopId = container.ShopId;
		this.RecommendId = container.RecommendId;
		this.RecommendUpsellTargetItemType = container.RecommendUpsellTargetItemType;
		this.RecommendUpsellTargetItemProductId = container.RecommendUpsellTargetItemProductId;
		this.RecommendUpsellTargetItemVariationId = container.RecommendUpsellTargetItemVariationId;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		this.ProductName = container.ProductName;
		this.VariationName1 = container.VariationName1;
		this.VariationName2 = container.VariationName2;
		this.VariationName3 = container.VariationName3;
		this.ShippingType = container.ShippingType;
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
	public RecommendUpsellTargetItemModel CreateModel(RecommendModel recommend)
	{
		var model = new RecommendUpsellTargetItemModel
		{
			ShopId = recommend.ShopId,
			RecommendId = recommend.RecommendId,
			RecommendUpsellTargetItemType = this.RecommendUpsellTargetItemType,
			RecommendUpsellTargetItemProductId = this.RecommendUpsellTargetItemProductId,
			RecommendUpsellTargetItemVariationId = this.RecommendUpsellTargetItemVariationId,
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

		// 入力チェック
		var input = new Hashtable
		{
			{"exists_upsell_target_item", this.RecommendUpsellTargetItemProductId}
		};
		this.ErrorMessages = Validator.Validate("Recommend", input);

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
		return ProductCommon.CreateProductJointName(this.ProductName, this.VariationName1, this.VariationName2, this.VariationName3);
	}

	/// <summary>
	/// 商品＋バリエーション名（ID：バリエーションID）のフォーマットで作成
	/// </summary>
	/// <returns>商品＋バリエーション名 （ID：バリエーションID）</returns>
	public string CreateProductJointNameAndVariationId()
	{
		var productName = ProductCommon.CreateProductJointName(this.ProductName, this.VariationName1, this.VariationName2, this.VariationName3);
		return string.Format("{0} （ID：{1}）", productName, this.RecommendUpsellTargetItemVariationId);
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID] = value; }
	}
	/// <summary>レコメンドID</summary>
	public string RecommendId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID] = value; }
	}
	/// <summary>レコメンドアップセル対象アイテム種別</summary>
	public string RecommendUpsellTargetItemType
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE] = value; }
	}
	/// <summary>レコメンドアップセル対象アイテム商品ID</summary>
	public string RecommendUpsellTargetItemProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_PRODUCT_ID] = value; }
	}
	/// <summary>レコメンドアップセル対象アイテム商品バリエーションID</summary>
	public string RecommendUpsellTargetItemVariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_VARIATION_ID] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDUPSELLTARGETITEM_LAST_CHANGED] = value; }
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
	/// <summary>配送種別</summary>
	public string ShippingType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE] = value; }
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
		get { return (this.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY); }
	}
		/// <summary>定期購入不可？</summary>
	public bool IsFixedPurchaseFlgInvalid
	{
		get { return (this.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID); }
	}
	/// <summary>通常商品？</summary>
	public bool IsNormal
	{
		get { return this.RecommendUpsellTargetItemType == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_NORMAL; }
	}
	/// <summary>定期商品？</summary>
	public bool IsFixedPurchase
	{
		get { return this.RecommendUpsellTargetItemType == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_FIXED_PURCHASE; }
	}
	/// <summary>頒布会商品？</summary>
	public bool IsSubscriptionBox
	{
		get { return this.RecommendUpsellTargetItemType == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_SUBSCRIPTION_BOX; }
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
