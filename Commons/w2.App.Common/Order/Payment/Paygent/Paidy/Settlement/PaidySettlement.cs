/*
=========================================================================================================
  Module      : Paygent API Paidy売上電文 モジュール(PaidySettlement.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.Paidy.Settlement.Dto;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Settlement
{
	/// <summary>
	/// Paygent API Paid売上電文 モジュール
	/// </summary>
	public class PaidySettlement
	{
		/// <summary>Paidy売上電文</summary>
		private const string PAYGENT_API_NAME = "Paidy売上電文";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentId">決済ID</param>
		public PaidySettlement(string paymentId)
		{
			var postBody = new PaidySettlementPostBody(paymentId);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Paidy売上電文 実行
		/// </summary>
		/// <returns>結果</returns>
		public PaidySettlementResult Settlement()
		{
			var response = new PaygentApiService(
					PAYGENT_API_NAME,
					this.PostBody.GenerateKeyValues())
				.GetResult<PaidySettlementResponseDataset>();
			var result = new PaidySettlementResult(response);
			return result;
		}

		/// <summary>リクエストボディ</summary>
		public PaidySettlementPostBody PostBody { get; private set; }
	}
}
