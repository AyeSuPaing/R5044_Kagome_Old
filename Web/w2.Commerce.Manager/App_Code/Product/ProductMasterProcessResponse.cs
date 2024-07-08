/*
=========================================================================================================
  Module      : Product master process response(ProductMasterProcessResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

/// <summary>
/// Product master process response
/// </summary>
public class ProductMasterProcessResponse
{
	/// <summary>
	/// Constructor
	/// </summary>
	public ProductMasterProcessResponse()
	{
		this.IsSuccess = false;
		this.ErrorMessage = string.Empty;
		this.ReviewUrl = string.Empty;
		this.ConfirmUrl = string.Empty;
	}

	/// <summary>Is success</summary>
	[JsonProperty("isSuccess")]
	public bool IsSuccess { get; set; }
	/// <summary>Error message</summary>
	[JsonProperty("errorMessage")]
	public string ErrorMessage { get; set; }
	/// <summary>Review url</summary>
	[JsonProperty("reviewUrl")]
	public string ReviewUrl { get; set; }
	/// <summary>Confirm url</summary>
	[JsonProperty("confirmUrl")]
	public string ConfirmUrl { get; set; }
	/// <summary>Guid String</summary>
	[JsonProperty("guidString")]
	public string GuidString { get; set; }
}