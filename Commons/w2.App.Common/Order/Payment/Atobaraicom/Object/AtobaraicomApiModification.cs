/*
=========================================================================================================
  Module      : 後払いAPIの変更 (AtobaraicomApiModification.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いAPIの変更
	/// </summary>
	public class AtobaraicomApiModification
	{
		/// <summary>事業者ID</summary>
		public string EnterpriseId { get; set; }
		/// <summary>APIユーザーID</summary>
		public string ApiUserId { get; set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>任意注文番号</summary>
		public string O_Ent_OrderId { get; set; }
	}
}
