/*
=========================================================================================================
  Module      : Atodene出荷報告基底アダプタ(BaseAtodeneShippingAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Shipping
{
	/// <summary>
	/// Atodene出荷報告基底アダプタ
	/// </summary>
	public abstract class BaseAtodeneShippingAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseAtodeneShippingAdapter()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">API接続設定</param>
		protected BaseAtodeneShippingAdapter(AtodeneApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// リクエストデータ生成
		/// </summary>
		/// <returns>リクエストデータ</returns>
		public virtual AtodeneShippingRequest CreateRequest()
		{
			var request = new AtodeneShippingRequest();
			request.TransactionInfo = new AtodeneShippingRequest.TransactionInfoElement();
			request.TransactionInfo.TransactionId = GetTransactionId();
			request.TransactionInfo.DeliveryCompanyCode = GetDeliveryCompanyCode();
			request.TransactionInfo.DeliverySlipNo = GetDeliverySlipNo();
			request.TransactionInfo.DeliveryType = AtodeneConst.DELIVERY_TYPE_SHIPMENT;

			return request;
		}

		/// <summary>
		/// 変更用リクエストデータ作成
		/// </summary>
		/// <returns>変更用のリクエストデータ</returns>
		public virtual AtodeneShippingRequest CreateModifyRequest()
		{
			var request = new AtodeneShippingRequest();
			request.TransactionInfo = new AtodeneShippingRequest.TransactionInfoElement();
			request.TransactionInfo.TransactionId = GetTransactionId();
			request.TransactionInfo.DeliveryType = AtodeneConst.DELIVERY_TYPE_CHANGE;

			return request;
		}

		/// <summary>
		/// 取消し用リクエストデータ作成
		/// </summary>
		/// <returns>取消し用のリクエストデータ</returns>
		public virtual AtodeneShippingRequest CreateCancelRequest()
		{
			var request = new AtodeneShippingRequest();
			request.TransactionInfo = new AtodeneShippingRequest.TransactionInfoElement();
			request.TransactionInfo.TransactionId = GetTransactionId();
			request.TransactionInfo.DeliveryType = AtodeneConst.DELIVERY_TYPE_CANCEL;

			return request;
		}

		/// <summary>
		/// お問い合わせ番号取得
		/// </summary>
		/// <returns>お問い合わせ番号</returns>
		public abstract string GetTransactionId();

		/// <summary>
		/// 運送会社コード取得
		/// </summary>
		/// <returns>運送会社コード</returns>
		public abstract string GetDeliveryCompanyCode();

		/// <summary>
		/// 配送伝票番号取得
		/// </summary>
		/// <returns>配送伝票番号</returns>
		public abstract string GetDeliverySlipNo();

		/// <summary>
		/// 出荷報告実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public AtodeneShippingResponse Execute()
		{
			var request = CreateRequest();
			return Execute(request);
		}

		/// <summary>
		/// 出荷報告取消し実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public AtodeneShippingResponse ExecuteCancel()
		{
			var request = CreateCancelRequest();
			return Execute(request);
		}

		/// <summary>
		/// 出荷豪国変更実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public AtodeneShippingResponse ExecuteModdify()
		{
			var request = CreateModifyRequest();
			return Execute(request);
		}

		/// <summary>
		/// 出荷報告実行
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public AtodeneShippingResponse Execute(AtodeneShippingRequest request)
		{
			var facade = (this.ApiSetting == null) ? new AtodeneApiFacade() : new AtodeneApiFacade(this.ApiSetting);
			var res = facade.Shipment(request);
			return res;
		}

		/// <summary>API接続設定</summary>
		public AtodeneApiSetting ApiSetting { get; set; }

	}

	/// <summary>
	/// Atodene出荷報告アダプタ
	/// </summary>
	public class AtodeneShippingAdapter : BaseAtodeneShippingAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AtodeneShippingAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="transactionId">お問い合わせ番号</param>
		/// <param name="deliverySlipNo">配送伝票番号</param>
		/// <param name="deliveryServiceCode">出荷連携配送会社コード</param>
		public AtodeneShippingAdapter(string transactionId, string deliverySlipNo, string deliveryServiceCode)
			: this(transactionId, deliverySlipNo, deliveryServiceCode, null)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="transactionId">お問い合わせ番号</param>
		/// <param name="deliverySlipNo">配送伝票番号</param>
		/// <param name="deliveryServiceCode">出荷連携配送会社コード</param>
		/// <param name="apiSetting">API接続設定</param>
		public AtodeneShippingAdapter(string transactionId, string deliverySlipNo, string deliveryServiceCode, AtodeneApiSetting apiSetting)
			: base(apiSetting)
		{
			this.TransactionId = transactionId;
			this.DeliverySlipNo = deliverySlipNo;
			this.DeliveryServiceCode = deliveryServiceCode;
		}

		/// <summary>
		/// お問い合わせ番号取得
		/// </summary>
		/// <returns>お問い合わせ番号</returns>
		public override string GetTransactionId()
		{
			return this.TransactionId;
		}

		/// <summary>
		/// 配送伝票番号取得
		/// </summary>
		/// <returns></returns>
		public override string GetDeliverySlipNo()
		{
			return this.DeliverySlipNo;
		}

		/// <summary>
		/// 運送会社コード取得
		/// </summary>
		/// <returns>運送会社コード</returns>
		public override string GetDeliveryCompanyCode()
		{
			return this.DeliveryServiceCode;
		}

		/// <summary>/// お問い合わせ番号</summary>
		public string TransactionId { get; set; }
		/// <summary>/// 配送伝票番号</summary>
		public string DeliverySlipNo { get; set; }
		/// <summary>出荷連携配送会社コード</summary>
		public string DeliveryServiceCode { get; set; }
	}
}

