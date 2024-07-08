/*
=========================================================================================================
  Module      : LP 商品ビューモデル(LandingPageProductViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common;
using w2.Domain.LandingPage;

namespace w2.Cms.Manager.ViewModels.LandingPage
{
	/// <summary>
	/// LP 商品ビューモデル
	/// </summary>
	[Serializable]
	public class LandingPageProductViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageProductViewModel()
		{
			this.PageId = "";
			this.ProductSets = new ProductSet[] { };
			this.ProductSetTemplate = new ProductSet();
		}

		/// <summary>ページID</summary>
		public string PageId { get; set; }
		/// <summary>頒布会コース</summary>
		public SelectListItem[] SubscriptionBoxes { get; set; }
		/// <summary>Enable subscription box option in configuration</summary>
		public bool EnabledSubscriptionBox { get; set; }
		/// <summary>商品選択タイプ "一覧指定の商品のうち、複数の商品を選択可能" を選べるか</summary>
		public bool CanSelectCheckboxOfProductChooseTypes
		{
			get
			{
				return ((Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION == false)
					|| (this.ProductSets.Any(ps => (ps.HasProductsOfMultipleBuyTypes)) == false)
					|| (this.ProductSets.Any() == false));
			}
		}
		/// <summary>商品セット</summary>
		public IEnumerable<ProductSet> ProductSets { get; set; }
		/// <summary>商品セットテンプレート (新規に追加する際に利用)</summary>
		public ProductSet ProductSetTemplate { get; set; }

		/// <summary>
		/// ランディングページ 商品セット
		/// </summary>
		public class ProductSet
		{
			public ProductSet()
			{
				this.Products = new Product[] { };
				this.SetName = "";
				this.ValidFlg = true;
				this.SubscriptionBoxCourseFlg = false;
			}

			/// <summary>購入タイプに通常、定期を設定した商品がそれぞれ一つ以上あるか</summary>
			public bool HasProductsOfMultipleBuyTypes
			{
				get
				{
					var hasNormalBuyType =
						this.Products.Any(p => (p.BuyType == LandingPageConst.BUY_TYPE_NORMAL));
					var hasFixedPurchaseBuyType =
						this.Products.Any(p => (p.BuyType == LandingPageConst.BUY_TYPE_FIXEDPURCHASE));
					return (hasNormalBuyType && hasFixedPurchaseBuyType);
				}
			}
			/// <summary>商品</summary>
			public IEnumerable<Product> Products { get; set; }
			/// <summary>商品選択肢名</summary>
			public string SetName { get; set; }
			/// <summary>有効フラグ</summary>
			public bool ValidFlg { get; set; }
			/// <summary>頒布会フラグ</summary>
			public bool SubscriptionBoxCourseFlg { get; set; }
			/// <summary>頒布会コースID</summary>
			public string SubscriptionBoxCourseId { get; set; }
		}

		/// <summary>
		/// ランディングページ 商品
		/// </summary>
		public class Product
		{
			/// <summary>商品画像Url</summary>
			public string ProductImage { get; set; }
			/// <summary>ページID</summary>
			public string PageId { get; set; }
			/// <summary>店舗ID</summary>
			public string ShopId { get; set; }
			/// <summary>商品ID</summary>
			public string ProductId { get; set; }
			/// <summary>バリエーションID</summary>
			public string VariationId { get; set; }
			/// <summary>商品名</summary>
			public string ProductName { get; set; }
			/// <summary>数量</summary>
			public string Quantity { get; set; }
			/// <summary>定期可否</summary>
			public bool FixedPurchaseFlg { get; set; }
			/// <summary>バリエーション順序</summary>
			public int VariationSortNumber { get; set; }
			/// <summary>配送種別ID</summary>
			public string ShippingId { get; set; }
			/// <summary>購入タイプ</summary>
			public string BuyType { get; set; }
		}
	}
}
