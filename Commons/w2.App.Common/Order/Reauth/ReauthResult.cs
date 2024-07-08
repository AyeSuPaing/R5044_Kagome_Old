/*
=========================================================================================================
  Module      : 再与信結果(ReauthResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Reauth
{
	/// <summary>再与信結果</summary>
	public class ReauthResult
	{
		/// <summary>
		/// 再与信結果詳細
		/// </summary>
		public enum ResultDetailTypes
		{
			/// <summary>成功</summary>
			Success,
			/// <summary>失敗</summary>
			Failure,
			/// <summary>失敗したが与信は成功している</summary>
			FailureButAuthSuccess,
			/// <summary>失敗したが与信＆売上確定は成功している</summary>
			FailureButAuthAndSalesSuccess,
			/// <summary>失敗したので一度キャンセルし、同額で与信を取り直している</summary>
			FailureButAuthSameAmount,
		}

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="resultDetail">再与信結果詳細</param>
		/// <param name="paymentMemoList">決済メモリスト</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="errorMessages">エラーメッセージ</param>
		/// <param name="apiErrorMessages">APIエラーメッセージ</param>
		/// <param name="isAuthResultHold">与信結果がHOLDか</param>
		public ReauthResult(
			ResultDetailTypes resultDetail,
			Dictionary<ActionTypes, string> paymentMemoList,
			string cardTranId = "",
			string paymentOrderId = "",
			string errorMessages = "",
			string apiErrorMessages = "",
			bool isAuthResultHold = false)
		{
			this.ResultDetail = resultDetail;
			this.CardTranId = cardTranId;
			this.PaymentOrderId = paymentOrderId;
			this.PaymentMemoList = paymentMemoList;
			this.ErrorMessages = errorMessages;
			this.ApiErrorMessages = apiErrorMessages;
			this.IsAuthResultHold = isAuthResultHold;
		}
		#endregion

		#region プロパティ
		/// <summary>与信＆キャンセルすべて成功したか</summary>
		public bool SuccessAll
		{
			get { return this.ResultDetail == ResultDetailTypes.Success; }
		}
		/// <summary>与信成功したか（キャンセル失敗でも成功とみなす）</summary>
		public bool AuthSuccess
		{
			get
			{
				return ((this.ResultDetail == ResultDetailTypes.Success)
					|| (this.ResultDetail == ResultDetailTypes.FailureButAuthSuccess)
					|| (this.ResultDetail == ResultDetailTypes.FailureButAuthAndSalesSuccess));
			}
		}
		/// <summary>結果詳細</summary>
		public ResultDetailTypes ResultDetail { get; private set; }
		/// <summary>決済カード取引ID</summary>
		/// <remarks>与信成功時のみ格納</remarks>
		public string CardTranId { get; private set; }
		/// <remarks>決済注文ID</remarks>
		public string PaymentOrderId { get; private set; }
		/// <summary>決済メモリスト</summary>
		public Dictionary<ActionTypes, string> PaymentMemoList { get; private set; }
		/// <summary>決済メモ</summary>
		public string PaymentMemo
		{
			get { return (string.Join("\r\n", this.PaymentMemoList.Select(p => p.Value))).Trim(); }
		}
		/// <summary>決済メモ（元注文用）</summary>
		/// <remarks>キャンセル or 返金</remarks>
		public string PaymentMemoForOrderOld
		{
			get
			{
				return (string.Join("\r\n",
					this.PaymentMemoList
					.Where(p => (p.Key == ActionTypes.Cancel) || (p.Key == ActionTypes.Refund)).Select(p => p.Value))).Trim();
			}
		}
		/// <summary>決済メモ（返品注文用）</summary>
		/// <remarks>与信 or 売上確定</remarks>
		public string PaymentMemoForReturnExchangeOrder
		{
			get
			{
				return (string.Join("\r\n",
					this.PaymentMemoList
					.Where(p => (p.Key == ActionTypes.Reauth)
						|| (p.Key == ActionTypes.Sales) || (p.Key == ActionTypes.Reprint) || (p.Key == ActionTypes.Reduce)).Select(p => p.Value))).Trim();
			}
		}
		/// <summary>Payment Memo Sales</summary>
		public string PaymentMemoSales
		{
			get
			{
				return (string.Join("\r\n",
					this.PaymentMemoList
						.Where(p => (p.Key == ActionTypes.Sales)).Select(p => p.Value))).Trim();
			}
		}
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessages { get; private set; }
		/// <summary>apiエラーメッセージ</summary>
		public string ApiErrorMessages { get; private set; }
		/// <summary>与信結果がHOLDか(現在はコンビニ後払い「DSK」のみ利用)</summary>
		public bool IsAuthResultHold { get; private set; }
		#endregion
	}
}