/*
=========================================================================================================
  Module      : Zcom実売上リクエストデータ (ZcomSalesRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom.Sales
{
	/// <summary>
	/// Zcom実売上リクエストデータ
	/// </summary>
	public class ZcomSalesRequest : BaseZcomRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZcomSalesRequest()
			: base()
		{
			base.m_data.Add("contract_code", "");
			base.m_data.Add("order_number", "");
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
	}
}
