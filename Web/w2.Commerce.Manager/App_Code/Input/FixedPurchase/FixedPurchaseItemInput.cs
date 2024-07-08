/*
=========================================================================================================
  Module      : 定期購入商品情報入力クラス (FixedPurchaseItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Input;
using w2.App.Common.Product;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.App.Common.Web.Process;

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
		this.ItemOrderCount = "0";
		this.ItemShippedCount = "0";
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
		this.DigitalContentsFlg = itemContainer.DigitalContentsFlg;
		this.ItemOrderCount = itemContainer.ItemOrderCount.ToString();
		this.ItemShippedCount = itemContainer.ItemShippedCount.ToString();
		this.ProductLimitedPaymentIds = itemContainer.ProductLimitedPaymentIds;
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
			ItemOrderCount = int.Parse(this.ItemOrderCount),
			ItemShippedCount = int.Parse(this.ItemShippedCount)
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		return Validator.Validate("FixedPurchaseModifyInput", this.DataSource).Replace("@@ 1 @@", this.FixedPurchaseItemNo + "番目の");
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

		return FixedPurchasePriceHelper.GetItemPrice(itemQuantity, GetValidPrice()) + GetProductOptionPrice() * itemQuantity;
	}

	/// <summary>
	/// オプション価格取得
	/// </summary>
	/// <returns>オプション価格修得</returns>
	public decimal GetProductOptionPrice()
	{
		var result = ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts(this.ProductOptionTexts);
		return result;
	}

	/// <summary>
	/// 定期購入有効商品?
	/// </summary>
	/// <returns>有効：true、無効：false</returns>
	public bool IsValidFixedPurchaseFlg()
	{
		// 有効 or 定期のみ
		if ((this.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_VALID)
			|| (this.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)) return true;

		return false;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="checkKbn">対象チェック区分</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(string checkKbn)
	{
		if (string.IsNullOrEmpty(Validator.Validate(checkKbn, this.DataSource).Replace("@@ 1 @@", this.FixedPurchaseItemNo))) return string.Empty;
		var result = string.Format(CommonPageProcess.ReplaceTag("@@DispText.common_message.location_no@@"),
			Validator.Validate(checkKbn, this.DataSource).Replace("@@ 1 @@", this.FixedPurchaseItemNo),
			string.Empty);
		return result;
	}

	/// <summary>
	/// 表示用の商品付帯情報選択値取得
	/// </summary>
	/// <returns>表示用の商品付帯情報選択値</returns>
	public string GetDisplayProductOptionTexts()
	{
		var displayProductOptionTexts = ProductOptionSettingHelper.GetDisplayProductOptionTexts(this.ProductOptionTexts);
		return string.IsNullOrEmpty(displayProductOptionTexts) ? string.Empty : displayProductOptionTexts;
	}

	/// <summary>
	/// 決済が利用できるか判定
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <returns>有効：true、無効：false</returns>
	public bool CanUsePayment(string paymentId)
	{
		var separator = new string[] { "," };
		var limitedPaymentIds = this.ProductLimitedPaymentIds.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		var result = (limitedPaymentIds.Any(id => id == paymentId) == false);
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
	/// <summary>定期購入解約可能回数 </summary>
	public int FixedPurchaseCancelableCount
	{
		get { return (int)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT] = value; }
	}
	/// <summary>デジタルコンテンツ商品フラグ</summary>
	public string DigitalContentsFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] = value; }
	}
	/// <summary>購入回数(注文基準)</summary>
	public string ItemOrderCount
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT]); }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT] = value; }
	}
	/// <summary>購入回数(出荷基準)</summary>
	public string ItemShippedCount
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT]); }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT] = value; }
	}
	/// <summary>商品付帯情報</summary>
	public ProductOptionSettingList ProductOptionSettingList
	{
		get { return (ProductOptionSettingList)this.DataSource["puroduct_option_value_settings"]; }
		set { this.DataSource["puroduct_option_value_settings"] = value; }
	}
	/// <summary>決済利用不可</summary>
	public string ProductLimitedPaymentIds
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_LIMITED_PAYMENT_IDS]; }
		set { this.DataSource[Constants.FIELD_ORDERITEM_LIMITED_PAYMENT_IDS] = value; }
	}
	#endregion
}
