/*
=========================================================================================================
  Module      : レコメンドアイテム入力クラス (RecommendItemInput.cs)
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
/// レコメンドアイテム入力クラス
/// </summary>
[Serializable]
public class RecommendItemInput
{
	/// <summary>データソース</summary>
	Hashtable DataSource { get; set; }

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public RecommendItemInput()
	{
		this.DataSource = new Hashtable();
		this.RecommendItemType = Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_NORMAL;
		this.RecommendItemAddQuantityType = Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SPECIFY_QUANTITY;
		this.RecommendItemAddQuantity = "1";
		this.RecommendItemSortNo = "1";
		this.IsValidateFixedPurchaseShippingPattern = true;
		this.FixedPurchaseKbn = string.Empty;
		this.FixedPurchaseSetting1 = string.Empty;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="container">レコメンドアイテム</param>
	public RecommendItemInput(RecommendItemContainer container)
		: this()
	{
		this.ShopId = container.ShopId;
		this.RecommendId = container.RecommendId;
		this.RecommendItemType = container.RecommendItemType;
		this.RecommendItemProductId = container.RecommendItemProductId;
		this.RecommendItemVariationId = container.RecommendItemVariationId;
		this.RecommendItemAddQuantityType = container.RecommendItemAddQuantityType;
		this.RecommendItemAddQuantity = container.RecommendItemAddQuantity.ToString();
		this.RecommendItemSortNo = container.RecommendItemSortNo.ToString();
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		this.ProductName = container.ProductName;
		this.VariationName1 = container.VariationName1;
		this.VariationName2 = container.VariationName2;
		this.VariationName3 = container.VariationName3;
		this.ShippingType = container.ShippingType;
		this.FixedPurchaseFlg = container.FixedPurchaseFlg;
		this.FixedPurchaseKbn = container.FixedPurchaseKbn;
		this.FixedPurchaseSetting1 = container.FixedPurchaseSetting1;
		this.SubscriptionBoxFlg = container.SubscriptionBoxFlg;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <param name="recommend">レコメンド設定</param>
	/// <returns>モデル</returns>
	public RecommendItemModel CreateModel(RecommendModel recommend)
	{
		var model = new RecommendItemModel
		{
			ShopId = recommend.ShopId,
			RecommendId = recommend.RecommendId,
			RecommendItemType = this.RecommendItemType,
			RecommendItemProductId = this.RecommendItemProductId,
			RecommendItemVariationId = this.RecommendItemVariationId,
			RecommendItemAddQuantityType = this.RecommendItemAddQuantityType,
			RecommendItemAddQuantity = int.Parse(this.RecommendItemAddQuantity),
			RecommendItemSortNo = int.Parse(this.RecommendItemSortNo),
			LastChanged = recommend.LastChanged,
			FixedPurchaseKbn = this.FixedPurchaseKbn,
			FixedPurchaseSetting1 = this.FixedPurchaseSetting1
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

		// 更新対象と同じ数？の場合はチェックしない
		if (this.IsRecommendItemAddQuantityTypeSameQuantity) return;

		// 入力チェック
		var input = new Hashtable
		{
			{Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY, this.IsRecommendItemAddQuantityTypeSpecifyQuantity ? this.RecommendItemAddQuantity : "1"}
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
		// 定期商品の配送パターンを設定しているか？チェック(注文完了画面向けのみ)
		if ((this.IsFixedPurchase ||this.IsSubscriptionBox)
			&& this.IsValidateFixedPurchaseShippingPattern
			&& (string.IsNullOrEmpty(this.FixedPurchaseKbn)
				|| string.IsNullOrEmpty(this.FixedPurchaseSetting1)))
		{
			this.ErrorMessages +=
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_FIXED_PURCHASE_SETTING_NOT_SET).Replace("@@ 1 @@", this.CreateProductJointName());
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
	/// 定期配送パターン設定表示文字列取得
	/// </summary>
	/// <returns>定期配送パターン</returns>
	public string CreateFixedPurchaseSettingMessage()
	{
		var message = OrderCommon.CreateFixedPurchaseSettingMessage(this.FixedPurchaseKbn, this.FixedPurchaseSetting1);
		return message;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_SHOP_ID] = value; }
	}
	/// <summary>レコメンドID</summary>
	public string RecommendId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ID] = value; }
	}
	/// <summary>レコメンドアイテム種別</summary>
	public string RecommendItemType
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_TYPE]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_TYPE] = value; }
	}
	/// <summary>レコメンドアイテム商品ID</summary>
	public string RecommendItemProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_PRODUCT_ID] = value; }
	}
	/// <summary>レコメンドアイテム商品バリエーションID</summary>
	public string RecommendItemVariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_VARIATION_ID] = value; }
	}
	/// <summary>レコメンドアイテム投入数種別</summary>
	public string RecommendItemAddQuantityType
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE] = value; }
	}
	/// <summary>レコメンドアイテム投入数</summary>
	public string RecommendItemAddQuantity
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY] = value; }
	}
	/// <summary>レコメンドアイテム並び順</summary>
	public string RecommendItemSortNo
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_SORT_NO]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_RECOMMEND_ITEM_SORT_NO] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_LAST_CHANGED] = value; }
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
		get { return this.RecommendItemType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_NORMAL; }
	}
	/// <summary>定期商品？</summary>
	public bool IsFixedPurchase
	{
		get { return this.RecommendItemType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_FIXED_PURCHASE; }
	}
	/// <summary>指定した数？</summary>
	public bool IsRecommendItemAddQuantityTypeSpecifyQuantity
	{
		get { return this.RecommendItemAddQuantityType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SPECIFY_QUANTITY; }
	}
	/// <summary>更新対象と同じ数？</summary>
	public bool IsRecommendItemAddQuantityTypeSameQuantity
	{
		get { return this.RecommendItemAddQuantityType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SAME_QUANTITY; }
	}
	/// <summary>エラーメッセージ</summary>
	public string ErrorMessages
	{
		get { return StringUtility.ToEmpty(this.DataSource["ErrorMessages"]); }
		set { this.DataSource["ErrorMessages"] = value; }
	}
	/// <summary>定期配送パターン入力チェックを行うか？</summary>
	public bool IsValidateFixedPurchaseShippingPattern
	{
		get { return (bool)this.DataSource["FixedPurchaseShippingPattern"]; }
		set { this.DataSource["FixedPurchaseShippingPattern"] = value; }
	}
	/// <summary>定期購入区分</summary>
	public string FixedPurchaseKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_KBN]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_KBN] = value; }
	}
	/// <summary>定期購入設定1</summary>
	public string FixedPurchaseSetting1
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_SETTING1]; }
		set { this.DataSource[Constants.FIELD_RECOMMENDITEM_FIXED_PURCHASE_SETTING1] = value; }
	}
	/// <summary>頒布会商品？</summary>
	public bool IsSubscriptionBox
	{
		get { return this.RecommendItemType == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_SUBSCRIPTION_BOX; }
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
	#endregion
}
