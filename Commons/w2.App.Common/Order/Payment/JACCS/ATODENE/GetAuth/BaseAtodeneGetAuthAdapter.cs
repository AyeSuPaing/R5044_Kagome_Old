/*
=========================================================================================================
  Module      : Atodene与信取得基底アダプタ(BaseAtodeneGetAuthAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetAuth
{
	/// <summary>
	/// Atodene与信取得基底アダプタ
	/// </summary>
	public abstract class BaseAtodeneGetAuthAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseAtodeneGetAuthAdapter()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">Api設定</param>
		protected BaseAtodeneGetAuthAdapter(AtodeneApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// リクエスト生成
		/// </summary>
		/// <returns>生成したリクエスト</returns>
		public virtual AtodeneGetAuthRequest CreateRequest()
		{
			var request = new AtodeneGetAuthRequest();
			request.TransactionInfo = new AtodeneGetAuthRequest.TransactionInfoElement();
			request.TransactionInfo.TransactionId = GetTransactionId();
			return request;
		}

		/// <summary>
		/// トランザクションID取得
		/// </summary>
		/// <returns>トランザクションID</returns>
		public abstract string GetTransactionId();

		/// <summary>
		///与信取得実行
		/// </summary>
		/// <returns>与信取得レスポンス</returns>
		public AtodeneGetAuthResponse Execute()
		{
			var request = CreateRequest();
			return Execute(request);
		}
		/// <summary>
		/// 与信取得実行
		/// </summary>
		/// <param name="request">与信取得リクエスト</param>
		/// <returns>与信取得レスポンス</returns>
		public AtodeneGetAuthResponse Execute(AtodeneGetAuthRequest request)
		{
			var facade = (this.ApiSetting == null) ? new AtodeneApiFacade() : new AtodeneApiFacade(this.ApiSetting);
			var res = facade.GetAuth(request);
			return res;
		}

		/// <summary>/// API設定</summary>
		public AtodeneApiSetting ApiSetting { get; set; }
	}
}

