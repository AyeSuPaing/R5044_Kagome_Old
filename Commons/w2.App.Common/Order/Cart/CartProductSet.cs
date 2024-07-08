/*
=========================================================================================================
  Module      : カート商品セット情報クラス(CartProductSet.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;
using w2.Domain;

namespace w2.App.Common.Order
{
	//
	// 商品3,4,5がセット商品の場合、以下のようになる（CartProductSetは3,4,5へのリンクを持つ。）
	//
	// [CartObject1]
	//    -[CartProduct1]
	//    -[CartProduct2]
	//    -[CartProduct3]
	//           +-----------+---[CartProductSet]
	//                       |     +---[CartProduct3] *
	//    -[CartProduct4]    |     +---[CartProduct4] *
	//           +-----------+     +---[CartProduct5] *
	//    -[CartProduct5]    |
	//           +-----------+
	//    -[CartProduct6]
	//    -[CartProduct7]
	//

	///*********************************************************************************************
	/// <summary>
	/// カート商品セット情報クラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class CartProductSet
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvProductSet">商品セット情報</param>
		/// <param name="iProductSetCount">商品セット数</param>
		/// <param name="iProductSetNo">商品セット枝番</param>
		/// <param name="this.UpdateCartDb">DB保存有無</param>
		/// <param name="cartId">カートID</param>
		public CartProductSet(DataRowView drvProductSet, int iProductSetCount, int iProductSetNo, bool blUpdateCartDb, string cartId = "")
		{
			this.ShopId = (string)drvProductSet[Constants.FIELD_PRODUCTSET_SHOP_ID];
			this.ProductSetId = (string)drvProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID];
			this.ProductSetName = (string)drvProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME];
			this.MaxSellQuantity = (int)drvProductSet[Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY];
			this.ProductSetCount = iProductSetCount;
			this.ProductSetNo = iProductSetNo;
			this.UpdateCartDb = blUpdateCartDb;
			this.CartId = cartId;

			this.Items = new List<CartProduct>();
		}

		/// <summary>
		/// セット商品追加
		/// </summary>
		/// <param name="drvProduct"></param>
		/// <param name="iProductCount"></param>
		/// <returns></returns>
		public CartProduct AddProductVirtual(string strShopId, string strProductId, string strVariationId, int iProductCount)
		{
			DataView dvProduct = ProductCommon.GetProductVariationInfo(strShopId, strProductId, strVariationId, null);
			if (dvProduct.Count == 0)
			{
				return null;
			}

			return AddProductVirtual(dvProduct[0], iProductCount);
		}
		/// <summary>
		/// セット商品追加
		/// </summary>
		/// <param name="drvProduct"></param>
		/// <param name="iProductCount"></param>
		/// <returns></returns>
		public CartProduct AddProductVirtual(DataRowView drvProduct, int iProductCount)
		{
			//------------------------------------------------------
			// セット商品情報取得
			//------------------------------------------------------
			var cartProductSetItem = DomainFacade.Instance.ProductSetService.GetProductSetItem(
				(string)drvProduct[Constants.FIELD_PRODUCTVARIATION_SHOP_ID],
				this.ProductSetId,
				(string)drvProduct[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID],
				(string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);

			if (cartProductSetItem == null)	return null;

			//------------------------------------------------------
			// セット化可能チェック・・・はカートの再計算で行うことにする
			//------------------------------------------------------

			//------------------------------------------------------
			// セット用商品作成＆紐付け
			//------------------------------------------------------
			CartProduct cpProductItem = new CartProduct(drvProduct, Constants.AddCartKbn.Normal, "", iProductCount, this.UpdateCartDb, new Product.ProductOptionSettingList());	// ★とりあえずセット商品は定期購入不可・タイムセールなし

			var setItemPrice = cartProductSetItem.SetitemPrice;

			this.Items.Add(cpProductItem);
			cpProductItem.SetProductSet(this, this.Items.IndexOf(cpProductItem) + 1, setItemPrice);

			//------------------------------------------------------
			// 再計算
			//------------------------------------------------------
			Calculate();

			return cpProductItem;
		}

		/// <summary>
		/// 商品セット数更新
		/// </summary
		/// <param name="strCartId"></param>
		/// <param name="iSetCount"></param>
		public void SetCount(string strCartId, int iSetCount)
		{
			if (this.UpdateCartDb)
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("Cart", "UpdateProductSetCount"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_CART_CART_ID, strCartId);
					htInput.Add(Constants.FIELD_CART_SHOP_ID, this.ShopId);
					htInput.Add(Constants.FIELD_CART_PRODUCT_SET_ID, this.ProductSetId);
					htInput.Add(Constants.FIELD_CART_PRODUCT_SET_NO, this.ProductSetNo);
					htInput.Add(Constants.FIELD_CART_PRODUCT_SET_COUNT, iSetCount);

					sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}
			}

			// プロパティセット
			this.ProductSetCount = iSetCount;

			// 小計再計算
			Calculate();
		}

		/// <summary>
		/// 同じ内容のセット商品か比較
		/// </summary>
		/// <param name="cpsTarget"></param>
		/// <returns></returns>
		public bool IsSameAs(CartProductSet cpsTarget)
		{
			// 同じ商品セットかつ、アイテム数も一緒？
			if ((this.ProductSetId == cpsTarget.ProductSetId)
				&& (this.Items.Count == cpsTarget.Items.Count))
			{
				for (int iLoop = 0; iLoop < cpsTarget.Items.Count; iLoop++)
				{
					if ((this.Items[iLoop].ShopId != cpsTarget.Items[iLoop].ShopId)
						|| (this.Items[iLoop].ProductId != cpsTarget.Items[iLoop].ProductId)
						|| (this.Items[iLoop].VariationId != cpsTarget.Items[iLoop].VariationId)
						|| (this.Items[iLoop].CountSingle != cpsTarget.Items[iLoop].CountSingle))
					{
						return false;
					}
				}
			}
			else
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// アイテム再計算
		/// </summary>
		public void Calculate()
		{
			decimal dProductSetPrice = 0;
			foreach (CartProduct cp in this.Items)
			{
				dProductSetPrice += cp.PriceSubtotalSingle;
			}

			// セット価格
			this.ProductSetPrice = dProductSetPrice;

			// セット価格小計
			this.ProductSetPriceSubtotal = this.ProductSetPrice * this.ProductSetCount;
		}

		/// <summary>店舗ID</summary>
		public string ShopId { get; private set; }
		/// <summary>カートID</summary>
		public string CartId { get; private set; }
		/// <summary>商品セットID</summary>
		public string ProductSetId { get; private set; }
		/// <summary>商品セット番号（カートにセットした際に付与）</summary>
		public int ProductSetNo { get; set; }
		/// <summary>商品セット名</summary>
		public string ProductSetName { get; private set; }
		/// <summary>商品セット価格</summary>
		public decimal ProductSetPrice { get; private set; }
		/// <summary>商品セット数</summary>
		public int ProductSetCount { get; private set; }
		/// <summary>商品セット価格小計</summary>
		public decimal ProductSetPriceSubtotal { get; private set; }
		/// <summary>商品セットアイテム ※追加したら再計算が必要</summary>
		public List<CartProduct> Items { get; private set; }
		/// <summary>販売可能数量</summary>
		public int MaxSellQuantity { get; private set; }

		/// <summary>DB更新設定</summary>
		public bool UpdateCartDb { get; set; }
	}

}
