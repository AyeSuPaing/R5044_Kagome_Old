/*
=========================================================================================================
  Module      : Zcomキャンセルリクエストデータ (ZcomCancelRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.Cancel
{
	/// <summary>
	/// Zcomキャンセルリクエストデータ
	/// </summary>
	public class ZcomCancelRequest : BaseZcomRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZcomCancelRequest() 
			: base()
		{
			base.m_data.Add("contract_code", "");
			base.m_data.Add("order_number", "");
			base.m_data.Add("refund_amount", "");
		}

		/// <summary>契約コード</summary>
		public string ContractCode
		{
			get { return base.m_data["contract_code"]; }
			set { base.m_data["contract_code"] = value; }
		}
		/// <summary>オーダー番号</summary>
		public string OrderNumber
		{
			get { return base.m_data["order_number"]; }
			set { base.m_data["order_number"] = value; }
		}
		/// <summary>返金金額</summary>
		public string RefundAmount
		{
			get { return base.m_data["refund_amount"]; }
			set { base.m_data["refund_amount"] = value; }
		}
	}
}
