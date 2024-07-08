/*
=========================================================================================================
  Module      : Zcom Check Auth Request Adapter (ZcomCheckAuthRequestAdapter.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth
{
	/// <summary>
	/// Zcom check auth request adapter
	/// </summary>
	public class ZcomCheckAuthRequestAdapter : BaseZcomCheckAuthRequestAdapter
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public ZcomCheckAuthRequestAdapter()
			: base()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="contractCode">Contract code</param>
		/// <param name="orderNumber">Order number</param>
		public ZcomCheckAuthRequestAdapter(string contractCode, string orderNumber)
			: this(contractCode, orderNumber, null)
		{
		}

		/// <summary>
		/// Zcom check auth request adapter
		/// </summary>
		/// <param name="contractCode">Contract code</param>
		/// <param name="orderNumber">Order number</param>
		/// <param name="zcomApiSetting">Zcom api setting</param>
		public ZcomCheckAuthRequestAdapter(
			string contractCode,
			string orderNumber,
			ZcomApiSetting zcomApiSetting)
			: base(zcomApiSetting)
		{
			this.ContractCode = contractCode;
			this.OrderNumber = orderNumber;
		}
		#endregion

		#region +Method
		/// <summary>
		/// Get contract code
		/// </summary>
		/// <returns>Contract code</returns>
		protected override string GetContractCode()
		{
			return StringUtility.ToEmpty(this.ContractCode);
		}

		/// <summary>
		/// Get order number
		/// </summary>
		/// <returns>Order number</returns>
		protected override string GetOrderNumber()
		{
			return StringUtility.ToEmpty(this.OrderNumber);
		}
		#endregion

		#region +Properties
		/// <summary>Contract code</summary>
		protected string ContractCode { get; set; }
		// <summary>Order number</summary>
		protected string OrderNumber { get; set; }
		#endregion
	}
}
