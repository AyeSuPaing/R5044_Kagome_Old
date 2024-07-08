/*
=========================================================================================================
  Module      : Base Zcom Check Auth Request Adapter (BaseZcomCheckAuthRequestAdapter.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth
{
	/// <summary>
	/// Base Zcom check auth request adapter
	/// </summary>
	public abstract class BaseZcomCheckAuthRequestAdapter
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		protected BaseZcomCheckAuthRequestAdapter()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="zcomApiSetting">Zcom api setting</param>
		protected BaseZcomCheckAuthRequestAdapter(ZcomApiSetting zcomApiSetting)
			: base()
		{
			this.ApiSetting = zcomApiSetting;
		}
		#endregion

		#region +Method
		/// <summary>
		/// Excute
		/// </summary>
		/// <returns>Zcom check auth response</returns>
		public ZcomCheckAuthResponse.Response Excute()
		{
			var zcomCheckAuthRequest = new ZcomCheckAuthRequest
			{
				ContractCode = this.GetContractCode(),
				OrderNumber = this.GetOrderNumber(),
			};
			var response = Excute(zcomCheckAuthRequest).GetResponse();
			return response;
		}

		/// <summary>
		/// Excute
		/// </summary>
		/// <param name="zcomCheckAuthRequest">Zcom check auth request</param>
		/// <returns>Zcom check auth response</returns>
		public ZcomCheckAuthResponse Excute(ZcomCheckAuthRequest zcomCheckAuthRequest)
		{
			var factory = ExternalApiFacade.Instance.ZcomApiFacadeFactory;
			var zcomApiFacade = factory.CreateFacade(this.ApiSetting);
			var zcomCheckAuthResponse = zcomApiFacade.ZcomCreditCheckAuth(zcomCheckAuthRequest);
			return zcomCheckAuthResponse;
		}

		/// <summary>
		/// Get contract code
		/// </summary>
		/// <returns>Contract code</returns>
		protected virtual string GetContractCode()
		{
			return string.Empty;
		}

		/// <summary>
		/// Get order number
		/// </summary>
		/// <returns>Order number</returns>
		protected virtual string GetOrderNumber()
		{
			return string.Empty;
		}
		#endregion

		#region +Properties
		/// <summary>Api setting</summary>
		public ZcomApiSetting ApiSetting { get; set; }
		#endregion
	}
}
