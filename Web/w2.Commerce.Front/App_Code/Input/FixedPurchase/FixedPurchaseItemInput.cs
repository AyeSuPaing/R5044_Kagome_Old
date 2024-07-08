/*
=========================================================================================================
  Module      : 定期購入商品情報入力クラス (FixedPurchaseItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Input;
using w2.App.Common.Product;
using w2.Common.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.FixedPurchaseProductChangeSetting;
using w2.Domain.Product.Helper;

/// <summary>
/// 定期購入商品情報入力クラス
/// </summary>
[Serializable]
public class FixedPurchaseItemInput : InputBase<FixedPurchaseItemModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public FixedPurchaseItemInput()
	{
		this.ItemOrderCount = 0;
		this.ItemShippedCount = 0;
		this.ModifyDeleteTarget = false;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="itemContainer">表示用定期購入情報（商品）</param>
	public FixedPurchaseItemInput(FixedPurchaseItemContainer itemContainer)
		: this()
	{
		this.FixedPurchaseId = itemContainer.FixedPurchaseId;
		this.FixedPurchaseItemNo = itemContainer.FixedPurchaseItemNo.ToString();
		this.FixedPurchaseShippingNo = itemContainer.FixedPurchaseShippingNo.ToString();
		this.ShopId = itemContainer.ShopId;
		this.ProductId = itemContainer.ProductId;
		this.VariationId = itemContainer.VariationId;
		this.SupplierId = itemContainer.SupplierId;
		this.ItemQuantity = itemContainer.ItemQuantity.ToString();
		this.ItemQuantitySingle = itemContainer.ItemQuantitySingle.ToString();
		this.DateCreated = itemContainer.DateCreated.ToString();
		this.DateChanged = itemContainer.DateChanged.ToString();
		this.ProductOptionTexts = itemContainer.ProductOptionTexts;
		this.Name = itemContainer.Name;
		this.VariationName1 = itemContainer.VariationName1;
		this.VariationName2 = itemContainer.VariationName2;
		this.VariationName3 = itemContainer.VariationName3;
		this.Price = itemContainer.Price;
		this.SpecialPrice = itemContainer.SpecialPrice;
		this.MemberRankPrice = itemContainer.MemberRankPrice;
		this.FixedPurchasePrice = itemContainer.FixedPurchasePrice;
		this.ShippingType = itemContainer.ShippingType;
		this.FixedPurchaseFlg = itemContainer.FixedPurchaseFlg;
		this.LimitedFixedPurchaseKbn1Setting = itemContainer.LimitedFixedPurchaseKbn1Setting;
		this.LimitedFixedPurchaseKbn3Setting = itemContainer.LimitedFixedPurchaseKbn3Setting;
		this.LimitedFixedPurchaseKbn4Setting = itemContainer.LimitedFixedPurchaseKbn4Setting;
		this.FixedPurchaseCancelableCount = itemContainer.FixedPurchaseCancelableCount;
		this.ItemOrderCount = itemContainer.ItemOrderCount;
		this.ItemShippedCount = itemContainer.ItemShippedCount;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="container">定期購入情報</param>
	/// <param name="product">商品情報</param>
	public FixedPurchaseItemInput(FixedPurchaseContainer container, DataRowView product)
		: this()
	{
		this.FixedPurchaseId = container.FixedPurchaseId;
		this.FixedPurchaseItemNo = (container.Shippings[0].Items.Length + 1).ToString();
		this.FixedPurchaseShippingNo = "1";
		this.ShopId = container.ShopId;
		this.ProductId = (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID];
		this.VariationId = (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
		this.SupplierId = (string)product[Constants.FIELD_PRODUCT_SUPPLIER_ID];
		this.ItemQuantity = "1";
		this.ItemQuantitySingle = "1";
		this.DateCreated = DateTime.Now.ToString();
		this.DateChanged = DateTime.Now.ToString();
		this.ProductOptionTexts = string.Empty;
		this.Name = (string)product[Constants.FIELD_PRODUCT_NAME];
		this.VariationName1 = (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
		this.VariationName2 = (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
		this.VariationName3 = (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
		this.Price = (decimal)product[Constants.FIELD_PRODUCTVARIATION_PRICE];
		decimal specialPrice;
		if (decimal.TryParse(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]), out specialPrice) == false)
		{
			specialPrice = this.Price;
		}
		this.SpecialPrice = specialPrice;
		decimal memberRankPrice;
		if (decimal.TryParse(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION]), out memberRankPrice) == false)
		{
			memberRankPrice = this.Price;
		}
		this.MemberRankPrice = memberRankPrice;
		decimal fixedPurchasePrice;
		if (decimal.TryParse(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]), out fixedPurchasePrice) == false)
		{
			fixedPurchasePrice = this.Price;
		}
		this.FixedPurchasePrice = fixedPurchasePrice;
		this.ShippingType = (string)product[Constants.FIELD_PRODUCT_SHIPPING_TYPE];
		this.FixedPurchaseFlg = (string)product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
		this.LimitedFixedPurchaseKbn1Setting = (string)product[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING];
		this.LimitedFixedPurchaseKbn3Setting = (string)product[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING];
		this.LimitedFixedPurchaseKbn4Setting = (string)product[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING];
		this.FixedPurchaseCancelableCount = (int)product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT];
		this.ItemOrderCount = 0;
		this.ItemShippedCount = 0;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override FixedPurchaseItemModel CreateModel()
	{
		var model = new FixedPurchaseItemModel
		{
			FixedPurchaseId = this.FixedPurchaseId,
			FixedPurchaseItemNo = int.Parse(this.FixedPurchaseItemNo),
			FixedPurchaseShippingNo = int.Parse(this.FixedPurchaseShippingNo),
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			VariationId = this.VariationId,
			SupplierId = this.SupplierId,
			ItemQuantity = int.Parse(this.ItemQuantity),
			ItemQuantitySingle = int.Parse(this.ItemQuantitySingle),
			ProductOptionTexts = this.ProductOptionTexts,
			ItemOrderCount = this.ItemOrderCount,
			ItemShippedCount = this.ItemShippedCount
		};
		return model;
	}

	/// <summary>
	/// 商品＋バリエーション名作成
	/// </summary>
	/// <returns>商品＋バリエーション名</returns>
	public string CreateProductJointName()
	{
		return ProductCommon.CreateProductJointName(this.Name, this.VariationName1, this.VariationName2, this.VariationName3);
	}

	/// <summary>
	/// 優先順位順で値がある金額を取得
	/// </summary>
	/// <returns>優先度の高い価格</returns>
	public decimal GetValidPrice()
	{
		return FixedPurchasePriceHelper.GetValidPrice(this.FixedPurchasePrice, this.MemberRankPrice, this.SpecialPrice, this.Price);
	}

	/// <summary>
	/// 明細金額（小計）取得
	/// </summary>
	/// <returns>明細金額（小計）</returns>
	public decimal GetItemPrice()
	{
		var itemQuantity = 0;
		int.TryParse(this.ItemQuantity, out itemQuantity);

		return FixedPurchasePriceHelper.GetItemPrice(itemQuantity, GetValidPrice());
	}

	/// <summary>
	/// 表示用の商品付帯情報選択値取得
	/// </summary>
	/// <returns>表示用の商品付帯情報選択値</returns>
	public string GetDisplayProductOptionTexts()
	{
		var displayProductOptionTexts = ProductOptionSettingHelper.GetDisplayProductOptionTexts(this.ProductOptionTexts);
		return displayProductOptionTexts;
	}

	/// <summary>
	/// 同一商品か
	/// </summary>
	/// <param name="product">入力商品</param>
	/// <returns>結果</returns>
	public bool IsSameProduct(FixedPurchaseItemInput product)
	{
		var result = (this.ShopId == product.ShopId)
			&& (this.ProductId == product.ProductId)
			&& (this.VariationId == product.VariationId)
			&& (this.ProductOptionTexts == product.ProductOptionTexts);
		return result;
	}

	/// <summary>
	/// オプション価格の合計取得
	/// </summary>
	/// <returns>オプション価格</returns>
	public decimal GetItemOptionPrice()
	{
		decimal itemQuantity;
		if (decimal.TryParse(this.ItemQuantity, out itemQuantity) == false)
		{
			itemQuantity = 0m;
		}
		var result = itemQuantity * this.OptionPrice;
		return result;
	}
	#endregion

	#region プロパティ
	/// <summary>定期購入ID</summary>
	public string FixedPurchaseId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID] = value; }
	}
	/// <summary>定期購入注文商品枝番</summary>
	public string FixedPurchaseItemNo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO] = value; }
	}
	/// <summary>定期購入配送先枝番</summary>
	public string FixedPurchaseShippingNo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_SHIPPING_NO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_SHIPPING_NO] = value; }
	}
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID] = value; }
	}
	/// <summary>商品バリエーションID</summary>
	public string VariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_VARIATION_ID] = value; }
	}
	/// <summary>サプライヤID</summary>
	public string SupplierId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SUPPLIER_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SUPPLIER_ID] = value; }
	}
	/// <summary>注文数</summary>
	public string ItemQuantity
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY] = value; }
	}
	/// <summary>注文数（セット未考慮）</summary>
	public string ItemQuantitySingle
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY_SINGLE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY_SINGLE] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CHANGED] = value; }
	}
	/// <summary>商品付帯情報選択値</summary>
	public string ProductOptionTexts
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_OPTION_TEXTS]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_OPTION_TEXTS] = value; }
	}
	/// <summary>商品名</summary>
	public string Name
	{
		get { return (string)this.DataSource["product_name"]; }
		set { this.DataSource["product_name"] = value; }
	}
	/// <summary>商品バリエーションID（商品ID除く）</summary>
	public string VId
	{
		get { return this.VariationId.Substring(this.ProductId.Length); }
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
	/// <summary>価格</summary>
	public decimal Price
	{
		get { return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] = value; }
	}
	/// <summary>特別価格</summary>
	public decimal? SpecialPrice
	{
		get
		{
			if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] == DBNull.Value) return null;
			return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];
		}
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] = value; }
	}
	/// <summary>会員ランク価格</summary>
	public decimal? MemberRankPrice
	{
		get
		{
			if (this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] == DBNull.Value) return null;
			return (decimal?)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE];
		}
		set { this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] = value; }
	}
	/// <summary>定期購入価格</summary>
	public decimal? FixedPurchasePrice
	{
		get
		{
			if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] == DBNull.Value) return null;
			return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];
		}
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] = value; }
	}
	/// <summary>配送料種別</summary>
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
	/// <summary>利用不可定期購入配送間隔月</summary>
	public string LimitedFixedPurchaseKbn1Setting
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING] = value; }
	}
	/// <summary>利用不可定期購入配送間隔日</summary>
	public string LimitedFixedPurchaseKbn3Setting
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING] = value; }
	}
	/// <summary>利用不可定期購入配送間隔週</summary>
	public string LimitedFixedPurchaseKbn4Setting
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING] = value; }
	}
	/// <summary>定期購入解約可能回数</summary>
	public int FixedPurchaseCancelableCount
	{
		get { return (int)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT] = value; }
	}
	/// <summary>購入回数(注文基準)</summary>
	public int ItemOrderCount
	{
		get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT] = value; }
	}
	/// <summary>購入回数(出荷基準)</summary>
	public int ItemShippedCount
	{
		get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT] = value; }
	}
	public bool HasVariation
	{
		get { return (this.ProductId != this.VariationId); }
	}
	/// <summary>選択値を反映した商品付帯情報設定</summary>
	public ProductOptionSettingList ProductOptionSettingsWithSelectedValues
	{
		get
		{
			return ProductOptionSettingHelper.GetProductOptionSettingList(
				this.ShopId,
				this.ProductId,
				GetDisplayProductOptionTexts());
		}
	}
	/// <summary>付帯価格</summary>
	public decimal OptionPrice
	{
		get { return ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts(this.ProductOptionTexts); }
	}
	/// <summary>付帯情報の価格を含めた価格</summary>
	public decimal ItemPriceIncludedOptionPrice
	{
		get
		{
			var result = GetItemPrice() + GetItemOptionPrice();
			return result;
		}
	}
	/// <summary>付帯情報の価格を含めた商品価格</summary>
	/// <remarks>
	/// ※商品価格は下記の価格情報で優先順位が高い価格が採用される<br/>
	/// ・定期購入価格<br/>
	/// ・会員ランク価格<br/>
	/// ・特別価格<br/>
	/// ・価格<br/>
	/// </remarks>
	public decimal ProductPriceIncludedOptionPrice
	{
		get
		{
			var result = GetValidPrice() + this.OptionPrice;
			return result;
		}
	}
	/// <summary>定期商品変更設定</summary>
	public FixedPurchaseProductChangeSettingModel ProductChangeSetting { get; set; }
	/// <summary>定期商品変更設定を持つか</summary>
	public bool HasProductChangeSetting
	{
		get { return this.ProductChangeSetting != null; }
	}
	/// <summary>編集時削除対象か</summary>
	public bool ModifyDeleteTarget { get; set; }

	#endregion
}
