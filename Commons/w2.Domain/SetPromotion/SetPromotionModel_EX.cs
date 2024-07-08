/*
=========================================================================================================
  Module      : セットプロモーションマスタモデル (SetPromotionModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Linq;
using w2.Domain.Product;

namespace w2.Domain.SetPromotion
{
	/// <summary>
	/// セットプロモーションモデル
	/// </summary>
	public partial class SetPromotionModel
	{
		#region 列挙体
		/// <summary>割引き種別</summary>
		public enum DiscountType
		{
			/// <summary>商品割引</summary>
			ProductDiscount,
			/// <summary>配送料無料</summary>
			ShippingChargeFree,
			/// <summary>決済手数料無料</summary>
			PaymentChargeFree
		}
		/// <summary>ステータス種別</summary>
		public enum StatusType
		{
			/// <summary>準備中</summary>
			Preparing,
			/// <summary>開催中</summary>
			OnGoing,
			/// <summary>一時停止</summary>
			Suspended,
			/// <summary>終了済</summary>
			Finished,
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 対象商品かどうかを判定
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="isVariation">バリエーションを考慮するかどうか</param>
		/// <returns>対象商品かどうか</returns>
		public bool ContainsProduct(DataRowView product, bool isVariation)
		{
			return ContainsProduct(
				(string)product[Constants.FIELD_PRODUCT_PRODUCT_ID],
				(isVariation ? (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] : ""),
				(string)product[Constants.FIELD_PRODUCT_CATEGORY_ID1],
				(string)product[Constants.FIELD_PRODUCT_CATEGORY_ID2],
				(string)product[Constants.FIELD_PRODUCT_CATEGORY_ID3],
				(string)product[Constants.FIELD_PRODUCT_CATEGORY_ID4],
				(string)product[Constants.FIELD_PRODUCT_CATEGORY_ID5]);
		}
		/// <summary>
		/// 対象商品かどうかを判定
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="isVariation">バリエーションを考慮するかどうか</param>
		/// <returns>対象商品かどうか</returns>
		public bool ContainsProduct(ProductModel product, bool isVariation)
		{
			return ContainsProduct(
				product.ProductId,
				(isVariation ? product.VariationId : string.Empty),
				product.CategoryId1,
				product.CategoryId2,
				product.CategoryId3,
				product.CategoryId4,
				product.CategoryId5);
		}
		/// <summary>
		/// 対象商品かどうかを判定
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="categoryIds">カテゴリIDリスト</param>
		/// <returns>対象商品かどうか</returns>
		public bool ContainsProduct(
			string productId,
			string variationId,
			params string[] categoryIds)
		{
			foreach (var setPromotionItem in this.Items)
			{
				switch (setPromotionItem.SetpromotionItemKbn)
				{
					// 商品ID指定
					case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT:
						if (setPromotionItem.ItemsList.Contains(productId))
						{
							return true;
						}
						break;

					// バリエーションID指定
					case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION:
						if ((variationId != "") && (setPromotionItem.ItemsList.Any(item => ((item.Split(',')[0] == productId) && (item.Split(',')[1] == variationId)))))
						{
							return true;
						}
						else if ((variationId == "") && (setPromotionItem.ItemsList.Any(item => (item.Split(',')[0] == productId))))
						{
							return true;
						}
						break;

					// カテゴリID指定
					case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY:
						if (setPromotionItem.ItemsList.Any(item =>
							categoryIds.Any(id => id.StartsWith(item))))
						{
							return true;
						}
						break;

					// 全商品
					case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL:
						return true;
				}
			}

			return false;
		}
		#endregion
		
		#region 拡張プロパティ
		/// <summary>開催状況</summary>
		public StatusType Status
		{
			get
			{
				// 日付範囲外は準備中・終了
				if (this.BeginDate > DateTime.Now) return StatusType.Preparing;
				if ((this.BeginDate <= DateTime.Now)
					&& ((this.EndDate.HasValue == false) || (this.EndDate >= DateTime.Now)))
				{
					return (this.ValidFlg == Constants.FLG_SETPROMOTION_VALID_FLG_VALID) ? StatusType.OnGoing : StatusType.Suspended;
				}
				return StatusType.Finished; ;
			}
		}
		/// <summary>商品金額割引か</summary>
		public bool IsDiscountTypeProductDiscount
		{
			get { return ((string)this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG] == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON); }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG] = (value ? Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON : Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF); }
		}
		/// <summary>配送料無料か</summary>
		public bool IsDiscountTypeShippingChargeFree
		{
			get { return ((string)this.DataSource[Constants.FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON); }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG] = (value ? Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF); }
		}
		/// <summary>決済手数料無料か</summary>
		public bool IsDiscountTypePaymentChargeFree
		{
			get { return ((string)this.DataSource[Constants.FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON); }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG] = (value ? Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF); }
		}

		/// <summary>商品割引区分が割引後価格指定か</summary>
		public bool IsProductDiscountKbnDiscountedPrice
		{
			get { return this.ProductDiscountKbn == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNTED_PRICE; }
		}
		/// <summary>商品割引区分が割引額指定か</summary>
		public bool IsProductDiscountKbnDiscountPrice
		{
			get { return this.ProductDiscountKbn == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE; }
		}
		/// <summary>商品割引区分が割引率指定か</summary>
		public bool IsProductDiscountKbnDiscountRate
		{
			get { return this.ProductDiscountKbn == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_RATE; }
		}

		/// <summary>セットプロモーションアイテム</summary>
		public SetPromotionItemModel[] Items
		{
			get { return (SetPromotionItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>セットプロモーション翻訳前設定情報</summary>
		public SetPromotionBeforeTranslationModel BeforeTranslationData
		{
			get { return (SetPromotionBeforeTranslationModel)this.DataSource["before_translation_data"]; }
			set { this.DataSource["before_translation_data"] = value; }
		}
		#endregion
	}

	/// <summary>
	/// セットプロモーション翻訳前設定情報モデルクラス
	/// </summary>
	/// <remarks>グローバルOP：ON時、表示名称の切り替えに使用</remarks>
	[Serializable]
	public class SetPromotionBeforeTranslationModel : ModelBase<SetPromotionBeforeTranslationModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SetPromotionBeforeTranslationModel() { }
		#endregion

		#region プロパティ
		/// <summary>表示用セットプロモーション名</summary>
		public string SetPromotionDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME] = value; }
		}
		/// <summary>表示用文言</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION] = value; }
		}
		/// <summary>表示用文言HTML区分</summary>
		public string DescriptionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN] = value; }
		}
		#endregion
	}
}
