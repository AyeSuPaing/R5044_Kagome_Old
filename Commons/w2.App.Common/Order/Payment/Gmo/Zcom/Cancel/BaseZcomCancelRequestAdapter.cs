/*
=========================================================================================================
  Module      : Zcomキャンセル基底アダプタ (BaseZcomCancelRequestAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.Cancel
{
	/// <summary>
	/// Zcomキャンセル基底アダプタ
	/// </summary>
	public abstract class BaseZcomCancelRequestAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseZcomCancelRequestAdapter()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">Api設定</param>
		protected BaseZcomCancelRequestAdapter(ZcomApiSetting apiSetting)
			: base()
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public ZcomCancelResponse Execute()
		{
			var request = CreateRequest();
			return Execute(request);
		}
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public ZcomCancelResponse Execute(ZcomCancelRequest request)
		{
			var factory = ExternalApiFacade.Instance.ZcomApiFacadeFactory;
			var facade = factory.CreateFacade(this.ApiSetting);
			var res = facade.CancelPayment(request);
			return res;
		}

		/// <summary>
		/// リクエストデータ生成
		/// </summary>
		/// <returns>リクエストデータ</returns>
		public virtual ZcomCancelRequest CreateRequest()
		{
			var req = new ZcomCancelRequest();
			req.ContractCode = this.GetConstractCode();
			req.OrderNumber = this.GetOrderNumber();
			req.RefundAmount = this.GetRefundAmount();
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

		/// <summary>
		/// 返金金額取得
		/// </summary>
		/// <returns>返金金額</returns>
		public virtual string GetRefundAmount()
		{
			// 全部取消しさせるので空
			return "";
		}

		/// <summary>/// API設定</summary>
		public ZcomApiSetting ApiSetting { get; set; }
	}
}
