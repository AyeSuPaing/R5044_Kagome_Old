/*
=========================================================================================================
  Module      : Register Event Request Adapter(RegisterEventRequestAdapter.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.OPlux.RegisterEvent
{
	/// <summary>
	/// Register event request adapter
	/// </summary>
	public class RegisterEventRequestAdapter
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="registerEventRequest">Register event request</param>
		public RegisterEventRequestAdapter(RegisterEventRequest registerEventRequest)
		{
			this.RegisterEventRequest = registerEventRequest;
		}
		#endregion

		#region +Method
		/// <summary>
		/// Execute
		/// </summary>
		/// <returns>Register event response</returns>
		public RegisterEventResponse Execute()
		{
			var response = new OPluxApiFacade().RegisterEvent(this.RegisterEventRequest);
			return response;
		}
		#endregion

		#region +Properties
		/// <summary>Register event request</summary>
		protected RegisterEventRequest RegisterEventRequest { get; set; }
		#endregion
	}
}
