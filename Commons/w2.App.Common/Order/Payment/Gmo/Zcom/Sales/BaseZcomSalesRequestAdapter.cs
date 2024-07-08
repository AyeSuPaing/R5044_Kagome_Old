/*
=========================================================================================================
  Module      : Zcom実売上基底アダプタ (BaseZcomSalesRequestAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.Sales
{
	/// <summary>
	/// Zcom実売上基底アダプタ
	/// </summary>
	public abstract class BaseZcomSalesRequestAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseZcomSalesRequestAdapter()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">Api設定</param>
		protected BaseZcomSalesRequestAdapter(ZcomApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public ZcomSalesResponse Execute()
		{
			var request = CreateRequest();
			return Execute(request);
		}
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public ZcomSalesResponse Execute(ZcomSalesRequest request)
		{
			var factory = ExternalApiFacade.Instance.ZcomApiFacadeFactory;
			var facade = factory.CreateFacade(this.ApiSetting);
			var res = facade.SalesPayment(request);
			return res;
		}

		/// <summary>
		/// リクエストデータ生成
		/// </summary>
		/// <returns>リクエストデータ</returns>
		public virtual ZcomSalesRequest CreateRequest()
		{
			var req = new ZcomSalesRequest();
			req.ContractCode = this.GetConstractCode();
			req.OrderNumber = this.GetOrderNumber();
			return req;
		}

		/// <summary>
		/// 契約コード取得
		/// </summary>
		/// <returns>契約コード</returns>
		public virtual string GetConstractCode()
		{
			return Constants.PAYMENT_CREDIT_ZCOM_APICONTACTCODE;
		}

		/// <summary>
		/// オーダー番号取得
		/// </summary>
		/// <returns>オーダー番号</returns>
		public abstract string GetOrderNumber();

		/// <summary>/// API設定</summary>
		public ZcomApiSetting ApiSetting { get; set; }
	}
}
