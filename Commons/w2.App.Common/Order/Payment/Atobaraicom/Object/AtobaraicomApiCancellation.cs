/*
=========================================================================================================
  Module      : 後払いAPIのキャンセル (AtobaraicomApiCancellation.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いAPIのキャンセル
	/// </summary>
	public class AtobaraicomApiCancellation
	{
		/// <summary>与信結果情報</summary>
		public string EnterpriseId { get; set; }
		/// <summary>APIユーザーID</summary>
		public string ApiUserId { get; set; }
		/// <summary>注文番号</summary>
		public string OrderNumber { get; set; }
		/// <summary>キャンセル理由</summary>
		public string Reason { get; set; }
	}
}
