/*
=========================================================================================================
  Module      : Atodene取引登録カートアダプタ(AtodeneTransactionCartAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction
{
	/// <summary>
	/// Atodene取引登録カートアダプタ
	/// </summary>
	public class AtodeneTransactionCartAdapter : BaseAtodeneTransactionAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AtodeneTransactionCartAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		public AtodeneTransactionCartAdapter(CartObject cart)
			: this(cart, null)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="apiSetting">API接続設定</param>
		public AtodeneTransactionCartAdapter(CartObject cart, AtodeneApiSetting apiSetting)
			: base(apiSetting)
		{
			this.Cart = cart;
		}

		/// <summary>
		/// 受注明細取得
		/// </summary>
		/// <returns>明細要素</returns>
		protected override AtodeneTransactionRequest.DetailElement[] GetProductInfoOfOrderDetails()
		{
			if (Constants.PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION && (this.Cart != null))
			{
				var detailElements = CreateDetailElements(this.Cart.Items.Count);

				var purchasedItemEtcPrice = 0m;
				this.Cart.Items.Select(
					(item, i) =>
					{
						var productName = CreateCooperationProductname(item.ProductName, item.ProductNameKana);

						var cooperationId = GetCooperationId(item.ShopId, item.ProductId);

						if (i < AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS)
						{
							detailElements[i] = CreateDetailElement(
								productName + " " + cooperationId,
								item.Price.ToPriceDecimal().ToString(),
								i + 1,
								item.Count);
						}
						else
						{
							purchasedItemEtcPrice += item.Price * item.Count;
						}

						return detailElements;
					}).ToList();

				// 商品項目数が12項目以上ある場合は、12項目からそれ以降の金額をまとめて連携する
				if (this.Cart.Items.Count > AtodeneConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS)
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
		/// 注文ID取得
		/// </summary>
		/// <returns>注文ID</returns>
		protected override string GetShopOrderId()
		{
			return OrderCommon.CreatePaymentOrderId(this.Cart.ShopId);
		}

		/// <summary>
		/// 氏名取得
		/// </summary>
		/// <returns>氏名</returns>
		protected override string GetName()
		{
			return this.Cart.Owner.Name;
		}

		/// <summary>
		/// 氏名かな取得
		/// </summary>
		/// <returns>氏名かな</returns>
		protected override string GetKanaName()
		{
			return this.Cart.Owner.NameKana;
		}

		/// <summary>
		/// 郵便番号取得
		/// </summary>
		/// <returns>郵便番号</returns>
		protected override string GetZip()
		{
			return this.Cart.Owner.Zip;
		}

		/// <summary>
		/// 住所取得
		/// </summary>
		/// <returns>住所</returns>
		protected override string GetAddress()
		{
			return this.Cart.Owner.Addr1 + "　" + this.Cart.Owner.Addr2 + "　" + this.Cart.Owner.Addr3 + "　" + this.Cart.Owner.Addr4;
		}

		/// <summary>
		/// 会社名取得
		/// </summary>
		/// <returns>会社名</returns>
		protected override string GetCompanyName()
		{
			return this.Cart.Owner.CompanyName;
		}

		/// <summary>
		/// 部署名取得
		/// </summary>
		/// <returns>部署名</returns>
		protected override string GetSectionName()
		{
			return this.Cart.Owner.CompanyPostName;
		}

		/// <summary>
		/// 電話番号取得
		/// </summary>
		/// <returns>電話番号</returns>
		protected override string GetTel()
		{
			return this.Cart.Owner.Tel1;
		}

		/// <summary>
		/// メールアドレス取得
		/// </summary>
		/// <returns>メールアドレス</returns>
		protected override string GetEmail()
		{
			return this.Cart.Owner.MailAddr;
		}

		/// <summary>
		/// 顧客請求金額（税込）取得
		/// </summary>
		/// <returns>顧客請求金額（税込）</returns>
		protected override decimal GetBilledAmount()
		{
			return this.Cart.PriceTotal;
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

			var bundleFlg = this.Cart.GetAtodenePaymentInvoiceBundleFlg();
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
		/// 配送先氏名
		/// </summary>
		/// <returns></returns>
		protected override string GetShipName()
		{
			return this.Cart.GetShipping().Name;
		}

		/// <summary>
		/// 配送先氏名かな
		/// </summary>
		/// <returns></returns>
		protected override string GetShipKananame()
		{
			return this.Cart.GetShipping().NameKana;
		}

		/// <summary>
		/// 配送先郵便番号
		/// </summary>
		/// <returns></returns>
		protected override string GetShipZip()
		{
			return this.Cart.GetShipping().Zip;
		}

		/// <summary>
		/// 配送先住所
		/// </summary>
		/// <returns></returns>
		protected override string GetShipAddress()
		{
			return this.Cart.GetShipping().Addr1 + "　" + this.Cart.GetShipping().Addr2 + "　" + this.Cart.GetShipping().Addr3 + "　" + this.Cart.GetShipping().Addr4;
		}

		/// <summary>
		/// 配送先会社名
		/// </summary>
		/// <returns></returns>
		protected override string GetShipCompanyName()
		{
			return this.Cart.GetShipping().CompanyName;
		}

		/// <summary>
		/// 配送先部署名
		/// </summary>
		/// <returns></returns>
		protected override string GetShipSectionName()
		{
			return this.Cart.GetShipping().CompanyPostName;
		}

		/// <summary>
		/// 配送先電話番号取得
		/// </summary>
		/// <returns>配送先電話番号</returns>
		protected override string GetShipTel()
		{
			return this.Cart.GetShipping().Tel1;
		}

		/// <summary>
		/// 小計取得
		/// </summary>
		/// <returns>小計</returns>
		protected override decimal GetSubtotalPrice()
		{
			return this.Cart.PriceSubtotal;
		}

		/// <summary>
		/// 送料取得
		/// </summary>
		/// <returns>送料</returns>
		protected override decimal GetShippingPrice()
		{
			return this.Cart.PriceShipping;
		}

		/// <summary>
		/// 決済手数料取得
		/// </summary>
		/// <returns>決済手数料</returns>
		protected override decimal GetExchangePrice()
		{
			return this.Cart.Payment.PriceExchange;
		}

		/// <summary>
		/// カート
		/// </summary>
		protected CartObject Cart { get; set; }
	}
}
