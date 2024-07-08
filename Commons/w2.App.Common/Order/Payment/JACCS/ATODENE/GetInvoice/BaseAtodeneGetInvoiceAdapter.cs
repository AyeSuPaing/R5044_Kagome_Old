/*
=========================================================================================================
  Module      : Atodene請求書印字データ取得基底アダプタ(BaseAtodeneGetInvoiceAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice
{
	/// <summary>
	/// Atodene請求書印字データ取得基底アダプタ
	/// </summary>
	public abstract class BaseAtodeneGetInvoiceAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseAtodeneGetInvoiceAdapter()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting"></param>
		protected BaseAtodeneGetInvoiceAdapter(AtodeneApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// リクエスト生成
		/// </summary>
		/// <returns>生成したリクエスト</returns>
		public virtual AtodeneGetInvoiceRequest CreateRequest()
		{
			var request = new AtodeneGetInvoiceRequest();
			request.TransactionInfo.TransactionId = GetTransactionId();
			return request;
		}

		/// <summary>
		/// トランザクションID取得
		/// </summary>
		/// <returns></returns>
		public abstract string GetTransactionId();

		/// <summary>
		/// 請求書印字データ取得実行
		/// </summary>
		/// <returns>請求書印字データ取得レスポンス</returns>
		public AtodeneGetInvoiceResponse Execute()
		{
			var request = CreateRequest();
			return Execute(request);
		}

		/// <summary>
		/// 請求書印字データ取得実行
		/// </summary>
		/// <param name="request">請求書印字データ取得リクエスト</param>
		/// <returns>請求書印字データ取得レスポンス</returns>
		public AtodeneGetInvoiceResponse Execute(AtodeneGetInvoiceRequest request)
		{
			var facade = (this.ApiSetting == null) ? new AtodeneApiFacade() : new AtodeneApiFacade(this.ApiSetting);
			var res = facade.GetInvoiceData(request);
			return res;
		}

		/// <summary>
		/// API設定
		/// </summary>
		public AtodeneApiSetting ApiSetting { get; set; }
	}
}

