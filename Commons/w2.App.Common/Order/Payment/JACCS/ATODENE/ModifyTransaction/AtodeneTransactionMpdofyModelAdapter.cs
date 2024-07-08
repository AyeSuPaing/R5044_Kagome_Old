/*
=========================================================================================================
  Module      : Atodene取引変更・キャンセルモデルアダプタ(AtodeneTransactionMpdofyModelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction
{
	/// <summary>
	/// Atodene取引変更・キャンセルモデルアダプタ
	/// </summary>
	public class AtodeneTransactionMpdofyModelAdapter : BaseAtodeneModifyTransactionAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AtodeneTransactionMpdofyModelAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル</param>
		public AtodeneTransactionMpdofyModelAdapter(OrderModel order)
			: this(order, null)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文も出る</param>
		/// <param name="apiSetting">API設定</param>
		public AtodeneTransactionMpdofyModelAdapter(OrderModel order, AtodeneApiSetting apiSetting)
			: base(apiSetting)
		{
			this.Order = order;
		}

		/// <summary>
		/// 受注明細の商品情報を取得
		/// </summary>
		/// <returns>明細要素</returns>
		protected override AtodeneModifyTransactionRequest.DetailElement[] GetProductInfoOfOrderDetails()
		{
			if (Constants.PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION && (this.Order.Items != null))
			{
				var detailElements = CreateDetailElements(this.Order.Items.Length);

				var purchasedItemEtcPrice = 0m;
				this.Order.Items.Select(
					(item, i) =>
					{
						var productName = CreateCooperationProductname(item.ProductName, item.ProductNameKana);

						var cooperationId = GetCooperationId(item.ShopId, item.ProductId);

						if (i < AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS)
						{
							detailElements[i] = CreateDetailElement(
								productName + " " + cooperationId,
								item.ProductPrice.ToPriceDecimal().ToString(),
								i + 1,
								item.ItemQuantity);
						}
						else
						{
							purchasedItemEtcPrice += item.ProductPrice * item.ItemQuantity;
						}

						return detailElements;
					}).ToList();

				// 商品項目数が12項目以上ある場合は、12項目からそれ以降の金額をまとめて連携する
				if (this.Order.Items.Length > AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS)
				{
					detailElements[AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS - 1] =
						CreateDetailElement(
							Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_PURCHASED_ITEM_ETC,
							((detailElements[AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS - 1].GoodsPrice.ToPriceDecimal()
								* detailElements[AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS - 1].GoodsAmount.ToPriceDecimal())
								+ purchasedItemEtcPrice.ToPriceDecimal()).ToString(),
							AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS);
				}
				return detailElements;
			}

			return null;
		}

		/// <summary>
		/// お問い合わせ番号取得
		/// </summary>
		/// <returns>お問い合わせ番号</returns>
		protected override string GetTransactionId()
		{
			return this.Order.CardTranId;
		}

		/// <summary>
		/// 注文ID取得
		/// </summary>
		/// <returns>注文ID</returns>
		protected override string GetShopOrderId()
		{
			return this.Order.OrderId;
		}

		/// <summary>
		/// 注文日（yyyy/mm/dd形式）取得
		/// </summary>
		/// <returns>注文日（yyyy/mm/dd形式）</returns>
		protected override string GetShopOrderDate()
		{
			return this.Order.OrderDate.HasValue
				? this.Order.OrderDate.Value.ToString("yyyy/MM/dd")
				: base.GetShopOrderDate();
		}

		/// <summary>
		/// 購入者氏名取得
		/// </summary>
		/// <returns>購入者氏名</returns>
		protected override string GetName()
		{
			return this.Order.Owner.OwnerName;
		}

		/// <summary>
		/// 購入者氏名かな取得
		/// </summary>
		/// <returns>購入者氏名かな</returns>
		protected override string GetKanaName()
		{
			return this.Order.Owner.OwnerNameKana;
		}

		/// <summary>
		/// 購入者郵便番号取得
		/// </summary>
		/// <returns>購入者郵便番号</returns>
		protected override string GetZip()
		{
			return this.Order.Owner.OwnerZip;
		}

		/// <summary>
		/// 購入者住所取得
		/// </summary>
		/// <returns>購入者住所</returns>
		protected override string GetAddress()
		{
			return this.Order.Owner.OwnerAddr1 + "　" + this.Order.Owner.OwnerAddr2 + "　" + this.Order.Owner.OwnerAddr3 + "　" + this.Order.Owner.OwnerAddr4;
		}

		/// <summary>
		/// 購入者会社名取得
		/// </summary>
		/// <returns>購入者会社名</returns>
		protected override string GetCompanyName()
		{
			return this.Order.Owner.OwnerCompanyName;
		}

		/// <summary>
		/// 購入者部署名取得
		/// </summary>
		/// <returns>購入者部署名</returns>
		protected override string GetSectionName()
		{
			return this.Order.Owner.OwnerCompanyPostName;
		}

		/// <summary>
		/// 購入者電話番号取得
		/// </summary>
		/// <returns>購入者電話番号</returns>
		protected override string GetTel()
		{
			return this.Order.Owner.OwnerTel1;
		}

		/// <summary>
		/// 購入者メールアドレス取得
		/// </summary>
		/// <returns>購入者メールアドレス</returns>
		protected override string GetEmail()
		{
			return string.IsNullOrEmpty(this.Order.Owner.OwnerMailAddr)
				? this.Order.Owner.OwnerMailAddr2
				: this.Order.Owner.OwnerMailAddr;
		}

		/// <summary>
		/// 顧客請求金額取得
		/// </summary>
		/// <returns>顧客請求金額</returns>
		protected override decimal GetBilledAmount()
		{
			return this.Order.LastBilledAmount;
		}

		/// <summary>
		/// 配送先氏名取得
		/// </summary>
		/// <returns>配送先氏名</returns>
		protected override string GetShipName()
		{
			return this.Order.Shippings.First().ShippingName;
		}

		/// <summary>
		/// 配送先氏名かな取得
		/// </summary>
		/// <returns>配送先氏名かな</returns>
		protected override string GetShipKananame()
		{
			return this.Order.Shippings.First().ShippingNameKana;
		}

		/// <summary>
		/// 配送先郵便番号取得
		/// </summary>
		/// <returns>配送先郵便番号</returns>
		protected override string GetShipZip()
		{
			return this.Order.Shippings.First().ShippingZip;
		}

		/// <summary>
		/// 配送先住所取得
		/// </summary>
		/// <returns>配送先住所</returns>
		protected override string GetShipAddress()
		{
			return this.Order.Shippings.First().ShippingAddr1 + "　"
				+ this.Order.Shippings.First().ShippingAddr2 + "　"
				+ this.Order.Shippings.First().ShippingAddr3 + "　"
				+ this.Order.Shippings.First().ShippingAddr4;
		}

		/// <summary>
		/// 配送先会社名取得
		/// </summary>
		/// <returns>配送先会社名</returns>
		protected override string GetShipCompanyName()
		{
			return this.Order.Shippings.First().ShippingCompanyName;
		}

		/// <summary>
		/// 配送先部署名取得
		/// </summary>
		/// <returns>配送先部署名</returns>
		protected override string GetShipSectionName()
		{
			return this.Order.Shippings.First().ShippingCompanyPostName;
		}

		/// <summary>
		/// 配送先電話番号取得
		/// </summary>
		/// <returns>配送先電話番号</returns>
		protected override string GetShipTel()
		{
			return this.Order.Shippings.First().ShippingTel1;
		}

		/// <summary>
		/// 商品小計取得
		/// </summary>
		/// <returns>商品小計</returns>
		protected override decimal GetSubtotalPrice()
		{
			return this.Order.OrderPriceSubtotal;
		}

		/// <summary>
		/// 送料取得
		/// </summary>
		/// <returns>送料</returns>
		protected override decimal GetShippingPrice()
		{
			return this.Order.OrderPriceShipping;
		}

		/// <summary>
		/// 決済手数料取得
		/// </summary>
		/// <returns>決済手数料</returns>
		protected override decimal GetExchangePrice()
		{
			return this.Order.OrderPriceExchange;
		}

		/// <summary>
		/// 請求書送付方法取得
		/// </summary>
		/// <returns>請求書送付方法</returns>
		protected override string GetService()
		{
			if (Constants.PAYMENT_SETTING_ATODENE_USE_INVOICE_BUNDLE_SERVICE == false)
			{
				return AtodeneConst.INVOICE_SEND_SERVICE_FLG_SEPARATE;
			}

			// 返品注文は「別送」で固定
			// 通常注文、交換注文は通常通り判定
			var bundleFlg = this.Order.IsReturnOrder
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF
				: this.Order.JudgmentInvoiceBundleFlg();

			switch (bundleFlg)
			{
				case Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON:
					return AtodeneConst.INVOICE_SEND_SERVICE_FLG_INCLUDE;

				case Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF:
					return AtodeneConst.INVOICE_SEND_SERVICE_FLG_SEPARATE;

				default:
					throw new Exception("識別できない同梱フラグ：" + bundleFlg);
			}
		}

		/// <summary>
		/// 注文モデル
		/// </summary>
		public OrderModel Order { get; set; }
	}
}
