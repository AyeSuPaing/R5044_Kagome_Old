/*
=========================================================================================================
  Module      : Atodene取引変更・キャンセル基底アダプタ(BaseAtodeneModifyTransactionAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Product;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction
{
	/// <summary>
	/// Atodene取引変更・キャンセル基底アダプタ
	/// </summary>
	public abstract class BaseAtodeneModifyTransactionAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseAtodeneModifyTransactionAdapter()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">API設定</param>
		protected BaseAtodeneModifyTransactionAdapter(AtodeneApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// 変更用リクエスト生成
		/// </summary>
		/// <returns>リクエスト</returns>
		public virtual AtodeneModifyTransactionRequest CreateModifyRequest()
		{
			var request = new AtodeneModifyTransactionRequest();

			// 取引情報
			request.TransactionInfo = new AtodeneModifyTransactionRequest.TransactionInfoElement();
			request.TransactionInfo.UpdateTypeFlag = AtodeneConst.UPDATE_TYPE_FLAG_CHANGE;
			request.TransactionInfo.TransactionId = GetTransactionId();

			// ご購入情報
			request.Customer = new AtodeneModifyTransactionRequest.CustomerElement();
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
			request.Customer.BilledAmount = GetBilledAmount().ToPriceDecimal().ToString();
			request.Customer.Expand1 = GetExpand1();
			request.Customer.Service = GetService();

			// 配送先情報
			request.Ship = new AtodeneModifyTransactionRequest.ShipElement();
			request.Ship.ShipName = GetShipName();
			request.Ship.ShipKananame = GetShipKananame();
			request.Ship.ShipZip = GetShipZip();
			request.Ship.ShipAddress = GetShipAddress();
			request.Ship.ShipCompanyName = GetShipCompanyName();
			request.Ship.ShipSectionName = GetShipSectionName();
			request.Ship.ShipTel = GetShipTel();

			// 明細
			request.Details = new AtodeneModifyTransactionRequest.DetailsElement();
			request.Details.Detail = GetDetail();

			return request;
		}

		/// <summary>
		/// 取消し用リクエスト生成
		/// </summary>
		/// <returns>リクエスト</returns>
		public virtual AtodeneModifyTransactionRequest CreateCancelRequest()
		{
			var request = new AtodeneModifyTransactionRequest();

			// 取引情報
			request.TransactionInfo = new AtodeneModifyTransactionRequest.TransactionInfoElement();
			request.TransactionInfo.UpdateTypeFlag = AtodeneConst.UPDATE_TYPE_FLAG_CANCEL;
			request.TransactionInfo.TransactionId = GetTransactionId();

			return request;
		}

		/// <summary>
		/// お問合せ番号取得
		/// </summary>
		/// <returns>お問い合わせ番号</returns>
		protected abstract string GetTransactionId();

		/// <summary>
		/// 注文ID取得
		/// </summary>
		/// <returns>注文ID</returns>
		protected abstract string GetShopOrderId();

		/// <summary>
		/// 注文日（yyyy/mm/dd形式）取得
		/// </summary>
		/// <returns>注文日（yyyy/mm/dd形式）</returns>
		protected virtual string GetShopOrderDate()
		{
			return DateTime.Now.ToString("yyyy/MM/dd");
		}

		/// <summary>
		/// 購入者氏名取得
		/// </summary>
		/// <returns>購入者氏名</returns>
		protected abstract string GetName();

		/// <summary>
		/// 購入者氏名かな取得
		/// </summary>
		/// <returns>購入者氏名かな</returns>
		protected abstract string GetKanaName();

		/// <summary>
		/// 購入者郵便番号取得
		/// </summary>
		/// <returns>購入者郵便番号</returns>
		protected abstract string GetZip();

		/// <summary>
		/// 購入者住所取得
		/// </summary>
		/// <returns>購入者住所</returns>
		protected abstract string GetAddress();

		/// <summary>
		/// 購入者会社名取得
		/// </summary>
		/// <returns>購入者会社名</returns>
		protected abstract string GetCompanyName();

		/// <summary>
		/// 購入者部署名取得
		/// </summary>
		/// <returns>購入者部署名</returns>
		protected abstract string GetSectionName();

		/// <summary>
		/// 購入者電話番号取得
		/// </summary>
		/// <returns>購入者電話番号</returns>
		protected abstract string GetTel();

		/// <summary>
		/// 購入者メールアドレス取得
		/// </summary>
		/// <returns>購入者メールアドレス</returns>
		protected abstract string GetEmail();

		/// <summary>
		/// 顧客請求金額取得
		/// </summary>
		/// <returns>顧客請求金額</returns>
		protected abstract decimal GetBilledAmount();

		/// <summary>
		/// 拡張審査パラメータ取得
		/// </summary>
		/// <returns>拡張審査パラメータ</returns>
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
		/// 明細詳細情報取得
		/// </summary>
		/// <returns>明細詳細情報</returns>
		protected virtual AtodeneModifyTransactionRequest.DetailElement[] GetDetail()
		{
			AtodeneModifyTransactionRequest.DetailElement[] detailElements;

			if (GetProductInfoOfOrderDetails() != null)
			{
				detailElements = GetProductInfoOfOrderDetails();
			}
			else
			{
				detailElements = new AtodeneModifyTransactionRequest.DetailElement[4];

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
		protected abstract AtodeneModifyTransactionRequest.DetailElement[] GetProductInfoOfOrderDetails();

		/// <summary>
		/// 商品小計取得
		/// </summary>
		/// <returns>商品小計</returns>
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
		/// 変更実行
		/// </summary>
		/// <returns>変更・取消しレスポンス</returns>
		public AtodeneModifyTransactionResponse ExecuteModify()
		{
			var request = CreateModifyRequest();
			return Execute(request);
		}

		/// <summary>
		/// 取消し実行
		/// </summary>
		/// <returns>変更・取消しレスポンス</returns>
		public AtodeneModifyTransactionResponse ExecuteCancel()
		{
			var request = CreateCancelRequest();
			return Execute(request);
		}

		/// <summary>
		/// 変更・取消し実行
		/// </summary>
		/// <param name="request">変更・取消しリクエスト</param>
		/// <returns>変更・取消しレスポンス</returns>
		public AtodeneModifyTransactionResponse Execute(AtodeneModifyTransactionRequest request)
		{
			var facade = (this.ApiSetting == null) ? new AtodeneApiFacade() : new AtodeneApiFacade(this.ApiSetting);
			var res = facade.OrderModifyCancel(request);
			return res;
		}

		/// <summary>
		/// 要素作成
		/// </summary>
		/// <param name="itemCount">連携商品数</param>
		protected AtodeneModifyTransactionRequest.DetailElement[] CreateDetailElements(int itemCount)
		{
			var detailElements = (itemCount + 3 > AtodeneConst.MAXIMUM_NUMBER_OF_COOPERATION_ROWS)
				? new AtodeneModifyTransactionRequest.DetailElement[AtodeneConst.MAXIMUM_NUMBER_OF_COOPERATION_ROWS]
				: new AtodeneModifyTransactionRequest.DetailElement[itemCount + 3];

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
		protected AtodeneModifyTransactionRequest.DetailElement CreateDetailElement(string goods, string price, int expand2, int amount = 1)
		{
			var detailElement = new AtodeneModifyTransactionRequest.DetailElement
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
		/// API設定
		/// </summary>
		public AtodeneApiSetting ApiSetting { get; set; }
	}

	/// <summary>
	/// Atodene取消しアダプタ
	/// </summary>
	public class AtodeneCancelTransactionAdapter : BaseAtodeneModifyTransactionAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AtodeneCancelTransactionAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tranId">お問合せ番号取得</param>
		public AtodeneCancelTransactionAdapter(string tranId)
			: this(tranId, null)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tranId">お問合せ番号取得</param>
		/// <param name="apiSetting">API接続設定</param>
		public AtodeneCancelTransactionAdapter(string tranId, AtodeneApiSetting apiSetting)
			: base(apiSetting)
		{
			this.TransactionId = tranId;
		}

		/// <summary>
		/// 受注明細の商品情報を取得
		/// </summary>
		/// <returns>明細要素</returns>
		protected override AtodeneModifyTransactionRequest.DetailElement[] GetProductInfoOfOrderDetails()
		{
			return null;
		}

		/// <summary>
		/// お問合せ番号取得
		/// </summary>
		/// <returns>お問合せ番号取得</returns>
		protected override string GetTransactionId()
		{
			return this.TransactionId;
		}

		#region 取消し時は未使用のため空で返すように実装
		/// <summary>
		/// 注文ID取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShopOrderId()
		{
			return string.Empty;
		}

		/// <summary>
		/// 氏名取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetName()
		{
			return string.Empty;
		}

		/// <summary>
		/// 氏名かな取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetKanaName()
		{
			return string.Empty;
		}

		/// <summary>
		/// 郵便番号取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetZip()
		{
			return string.Empty;
		}

		/// <summary>
		/// 住所取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetAddress()
		{
			return string.Empty;
		}

		/// <summary>
		/// 会社名取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetCompanyName()
		{
			return string.Empty;
		}

		/// <summary>
		/// 部署名取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetSectionName()
		{
			return string.Empty;
		}

		/// <summary>
		/// 電話番号取得k
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetTel()
		{
			return string.Empty;
		}

		/// <summary>
		/// メールアドレス取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetEmail()
		{
			return string.Empty;
		}

		/// <summary>
		/// 顧客請求金額取得
		/// </summary>
		/// <returns>0</returns>
		protected override decimal GetBilledAmount()
		{
			return 0;
		}

		/// <summary>
		/// 配送先氏名取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShipName()
		{
			return string.Empty;
		}

		/// <summary>
		/// 配送先氏名かな取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShipKananame()
		{
			return string.Empty;
		}

		/// <summary>
		/// 配送先郵便番号取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShipZip()
		{
			return string.Empty;
		}

		/// <summary>
		/// 配送先住所取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShipAddress()
		{
			return string.Empty;
		}

		/// <summary>
		/// 配送先会社名取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShipCompanyName()
		{
			return string.Empty;
		}

		/// <summary>
		/// 配送先部署名取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShipSectionName()
		{
			return string.Empty;
		}

		/// <summary>
		/// 配送先電話番号取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetShipTel()
		{
			return string.Empty;
		}

		/// <summary>
		/// 小計取得
		/// </summary>
		/// <returns>0</returns>
		protected override decimal GetSubtotalPrice()
		{
			return 0;
		}

		/// <summary>
		/// 送料取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override decimal GetShippingPrice()
		{
			return 0;
		}

		/// <summary>
		/// 手数料取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override decimal GetExchangePrice()
		{
			return 0;
		}

		/// <summary>
		/// 手数料取得
		/// </summary>
		/// <returns>Empty</returns>
		protected override string GetService()
		{
			return string.Empty;
		}

		public string TransactionId { get; set; }
		#endregion
	}
}

