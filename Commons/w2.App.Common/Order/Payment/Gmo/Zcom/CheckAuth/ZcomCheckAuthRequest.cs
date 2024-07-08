/*
=========================================================================================================
  Module      : Zcom Check Auth Request (ZcomCheckAuthRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth
{
	/// <summary>
	/// Zcom check order request
	/// </summary>
	public class ZcomCheckAuthRequest : BaseZcomRequest
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public ZcomCheckAuthRequest()
			: base()
		{
			base.m_data.Add(ZcomConst.PARAM_CONTRACT_CODE, string.Empty);
			base.m_data.Add(ZcomConst.PARAM_ORDER_NUMBER, string.Empty);
		}
		#endregion

		#region +Properties
		/// <summary>Contract code</summary>
		public string ContractCode
		{
			get { return base.m_data[ZcomConst.PARAM_CONTRACT_CODE]; }
			set { base.m_data[ZcomConst.PARAM_CONTRACT_CODE] = value; }
		}
		/// <summary>Order number</summary>
		public string OrderNumber
		{
			get { return base.m_data[ZcomConst.PARAM_ORDER_NUMBER]; }
			set { base.m_data[ZcomConst.PARAM_ORDER_NUMBER] = value; }
		}
		#endregion
	}
}
