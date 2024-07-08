/*
=========================================================================================================
  Module      : Atodene取引登録基底アダプタ(BaseAtodeneTransactionAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Product;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction
{
	/// <summary>
	/// Atodene取引登録基底アダプタ
	/// </summary>
	public abstract class BaseAtodeneTransactionAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseAtodeneTransactionAdapter()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">API接続設定</param>
		protected BaseAtodeneTransactionAdapter(AtodeneApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// リクエストデータ生成
		/// </summary>
		/// <returns>リクエストデータ</returns>
		public virtual AtodeneTransactionRequest CreateRequest()
		{
			var request = new AtodeneTransactionRequest();

			// ご購入情報
			request.Customer = new AtodeneTransactionRequest.CustomerElement();
			request.Customer.ShopOrderId = GetShopOrderId();
			request.Customer.ShopOrderDate = GetShopOrderDate();
			request.Customer.Name = GetName();
			request.Customer.KanaName = GetKanaName();
			request.Customer.Zip = GetZip();
			request.Customer.Address = GetAddress();
			request.Customer.CompanyName = GetCompanyName();
			request.Customer.SectionName = GetSectionName();
			request.Customer.Tel = GetTel();
			request.Customer.Email = GetEmail();
			request.Customer.BilledAmount = (GetBilledAmount()).ToPriceDecimal().ToString();
			request.Customer.Expand1 = GetExpand1();
			request.Customer.Service = GetService();

			// 配送先情報
			request.Ship = new AtodeneTransactionRequest.ShipElement();
			request.Ship.ShipName = GetShipName();
			request.Ship.ShipKananame = GetShipKananame();
			request.Ship.ShipZip = GetShipZip();
			request.Ship.ShipAddress = GetShipAddress();
			request.Ship.ShipCompanyName = GetShipCompanyName();
			request.Ship.ShipSectionName = GetShipSectionName();
			request.Ship.ShipTel = GetShipTel();

			// 明細
			request.Details = new AtodeneTransactionRequest.DetailsElement();
			request.Details.Detail = GetDetail();

			return request;
		}

		/// <summary>
		/// 注文ID取得
		/// </summary>
		/// <returns>注文ID</returns>
		protected abstract string GetShopOrderId();

		/// <summary>
		/// 注文日取得
		/// </summary>
		/// <returns>注文日</returns>
		protected virtual string GetShopOrderDate()
		{
			return DateTime.Now.ToString("yyyy/MM/dd");
		}

		/// <summary>
		/// 氏名取得
		/// </summary>
		/// <returns>氏名</returns>
		protected abstract string GetName();

		/// <summary>
		/// 氏名かな取得
		/// </summary>
		/// <returns>氏名かな</returns>
		protected abstract string GetKanaName();

		/// <summary>
		/// 郵便番号取得
		/// </summary>
		/// <returns>郵便番号</returns>
		protected abstract string GetZip();

		/// <summary>
		/// 住所取得
		/// </summary>
		/// <returns>住所</returns>
		protected abstract string GetAddress();

		/// <summary>
		/// 会社名取得
		/// </summary>
		/// <returns>会社名</returns>
		protected abstract string GetCompanyName();

		/// <summary>
		/// 部署名取得
		/// </summary>
		/// <returns>部署名</returns>
		protected abstract string GetSectionName();

		/// <summary>
		/// 電話番号取得
		/// </summary>
		/// <returns></returns>
		protected abstract string GetTel();

		/// <summary>
		/// メールアドレス取得
		/// </summary>
		/// <returns>メールアドレス</returns>
		protected abstract string GetEmail();

		/// <summary>
		/// 顧客請求金額（税込）取得
		/// </summary>
		/// <returns>顧客請求金額（税込）</returns>
		protected abstract decimal GetBilledAmount();

		/// <summary>
		/// 拡張審査パラメータ
		/// </summary>
		/// <returns></returns>
		protected virtual string GetExpand1()
		{
			return AtodeneConst.EXTEND1_SHIPING_COMP_FLG_SKIP;
		}

		/// <summary>
		/// 請求書送付方法取得
		/// </summary>
		/// <returns>請求書送付方法</returns>
		protected abstract string GetService();

		/// <summary>
		/// 配送先氏名取得
		/// </summary>
		/// <returns>配送先氏名</returns>
		protected abstract string GetShipName();

		/// <summary>
		/// 配送先氏名かな取得
		/// </summary>
		/// <returns>配送先氏名かな</returns>
		protected abstract string GetShipKananame();

		/// <summary>
		/// 配送先郵便番号取得
		/// </summary>
		/// <returns>配送先郵便番号</returns>
		protected abstract string GetShipZip();

		/// <summary>
		/// 配送先住所取得
		/// </summary>
		/// <returns>配送先住所</returns>
		protected abstract string GetShipAddress();

		/// <summary>
		/// 配送先会社名取得
		/// </summary>
		/// <returns>配送先会社名</returns>
		protected abstract string GetShipCompanyName();

		/// <summary>
		/// 配送先部署名取得
		/// </summary>
		/// <returns>配送先部署名</returns>
		protected abstract string GetShipSectionName();

		/// <summary>
		/// 配送先電話番号取得
		/// </summary>
		/// <returns>配送先電話番号</returns>
		protected abstract string GetShipTel();

		/// <summary>
		/// 受注明細取得
		/// </summary>
		/// <returns>明細要素</returns>
		protected AtodeneTransactionRequest.DetailElement[] GetDetail()
		{
			AtodeneTransactionRequest.DetailElement[] detailElements;

			if (GetProductInfoOfOrderDetails() != null)
			{
				detailElements = GetProductInfoOfOrderDetails();
			}
			else
			{
				detailElements = new AtodeneTransactionRequest.DetailElement[4];

				detailElements[0] = CreateDetailElement(
					Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_SUBTOTAL,
					GetSubtotalPrice().ToPriceDecimal().ToString(),
					1);
			}

			detailElements[detailElements.Length - 3] = CreateDetailElement(
				Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_SHIPPING,
				(GetShippingPrice()).ToPriceDecimal().ToString(),
				detailElements.Length - 2);

			detailElements[detailElements.Length - 2] = CreateDetailElement(
				Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_PAYMENT,
				(GetExchangePrice()).ToPriceDecimal().ToString(),
				detailElements.Length - 1);

			detailElements[detailElements.Length - 1] = CreateDetailElement(
				Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_DISCOUNT_ETC,
				(GetBilledAmount() - (GetSubtotalPrice() + GetShippingPrice() + GetExchangePrice())).ToPriceDecimal().ToString(),
				detailElements.Length);

			return detailElements;
		}

		/// <summary>
		/// 受注明細の商品情報を取得
		/// </summary>
		/// <returns>明細要素</returns>
		protected abstract AtodeneTransactionRequest.DetailElement[] GetProductInfoOfOrderDetails();

		/// <summary>
		/// 小計取得
		/// </summary>
		/// <returns>小計</returns>
		protected abstract decimal GetSubtotalPrice();

		/// <summary>
		/// 送料取得
		/// </summary>
		/// <returns>送料</returns>
		protected abstract decimal GetShippingPrice();

		/// <summary>
		/// 決済手数料取得
		/// </summary>
		/// <returns>決済手数料</returns>
		protected abstract decimal GetExchangePrice();

		/// <summary>
		/// 取引登録実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public AtodeneTransactionResponse Execute()
		{
			var request = CreateRequest();
			return Execute(request);
		}

		/// <summary>
		/// 取引登録実行
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public AtodeneTransactionResponse Execute(AtodeneTransactionRequest request)
		{
			var facade = (this.ApiSetting == null) ? new AtodeneApiFacade() : new AtodeneApiFacade(this.ApiSetting);
			var res = facade.OrderRegister(request);
			return res;
		}

		/// <summary>
		/// 要素作成
		/// </summary>
		/// <param name="itemCount">連携商品数</param>
		protected AtodeneTransactionRequest.DetailElement[] CreateDetailElements(int itemCount)
		{
			var detailElements = (itemCount + 3 > AtodeneConst.MAXIMUM_NUMBER_OF_COOPERATION_ROWS)
				? new AtodeneTransactionRequest.DetailElement[AtodeneConst.MAXIMUM_NUMBER_OF_COOPERATION_ROWS]
				: new AtodeneTransactionRequest.DetailElement[itemCount + 3];

			return detailElements;
		}

		/// <summary>
		/// 詳細要素作成
		/// </summary>
		/// <param name="goods">明細内容</param>
		/// <param name="price">単価</param>
		/// <param name="expand2">明細予備項目</param>
		/// <param name="amount">注文数</param>
		/// <returns>詳細要素</returns>
		protected AtodeneTransactionRequest.DetailElement CreateDetailElement(string goods, string price, int expand2, int amount = 1)
		{
			var detailElement = new AtodeneTransactionRequest.DetailElement
			{
				Goods = goods,
				GoodsPrice = price,
				Expand2 = expand2.ToString(),
				GoodsAmount = amount.ToString(),
			};

			return detailElement;
		}

		/// <summary>
		/// 連携する商品名を作成
		/// </summary>
		/// <param name="productName">商品名</param>
		/// <param name="productNameKana">商品名(カナ)</param>
		/// <returns>商品名</returns>
		protected string CreateCooperationProductname(string productName, string productNameKana)
		{
			var cooperationProductname = ((Constants.PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION_PRODUCTNAME == AtodeneConst.FLG_COOPERATION_PRODUCTNAME_IS_PRODUCTNAME_AND_KANA)
				&& (string.IsNullOrEmpty(productNameKana) == false))
				? productName + "(" + productNameKana + ")"
				: productName;

			return cooperationProductname;
		}

		/// <summary>
		/// 連携する商品連携IDを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>商品連携ID</returns>
		protected string GetCooperationId(string shopId, string productId)
		{
			int cooperationIdNum;
			if (int.TryParse(Constants.PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION_PRODUCTID, out cooperationIdNum))
			{
				var cooperationId =
					new ProductService().GetCooperationIdByProductId(shopId, productId, cooperationIdNum);

				return cooperationId;
			}
			return null;
		}

		/// <summary>
		/// API接続設定
		/// </summary>
		public AtodeneApiSetting ApiSetting { get; set; }
	}
}

