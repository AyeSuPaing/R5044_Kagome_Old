/*
=========================================================================================================
  Module      : Gooddeal result (GooddealResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal result
	/// </summary>
	public class GooddealResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public GooddealResult()
		{
			this.Response = new GooddealResponse();
		}

		/// <summary>
		/// Set response
		/// </summary>
		/// <param name="response">The response</param>
		public void SetResponse(GooddealResponse response)
		{
			this.Response = response;
		}

		/// <summary>
		/// Get api message
		/// </summary>
		/// <returns>Api message</returns>
		public string GetApiMessage()
		{
			var message = string.Format(
				"{0} {1}",
				this.Response.ErrorCode ?? this.Response.RegisterErrorCode,
				this.Response.ErrorMessage ?? this.Response.DeliverType);
			return message;
		}

		/// <summary>The response</summary>
		public GooddealResponse Response { get; set; }
		/// <summary>Is register succeeded</summary>
		public bool IsRegisterSucceeded
		{
			get
			{
				return ((this.Response.Status == GooddealConstants.GOODDEAL_STATUS_SUCCESS)
					&& (this.Response.ErrorCode == GooddealConstants.SHIPPING_GOODDEAL_REGISTER_ERROR_CODE_SUCCESS));
			}
		}
		/// <summary>Is cancelation succeeded</summary>
		public bool IsCancelationSucceeded
		{
			get
			{
				return ((this.Response.Status == GooddealConstants.GOODDEAL_STATUS_SUCCESS)
					&& (this.Response.ErrorCode == GooddealConstants.SHIPPING_GOODDEAL_CANCEL_ERROR_CODE_SUCCESS));
			}
		}
		/// <summary>Is get delivery slip succeeded</summary>
		public bool IsGetDeliverySlipSucceeded
		{
			get
			{
				return ((this.Response.Status == GooddealConstants.GOODDEAL_STATUS_SUCCESS)
					&& (string.IsNullOrEmpty(this.Response.DeliverNo) == false));
			}
		}
	}
}