/*
=========================================================================================================
  Module      : 再与信アクション結果(ReauthActionResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.GMO.TransactionRegister;
namespace w2.App.Common.Order.Reauth
{
	/// <summary>再与信アクション結果</summary>
	public class ReauthActionResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">再与信結果</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentMemo">決済連携メモ</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranIdForLog">決済カード取引ID（ログ出力用）</param>
		/// <param name="apiErrorMessage">APIエラーメッセージ</param>
		/// <param name="authLostForError">エラーによって与信が消失するかどうか</param>
		/// <param name="transactionResult">取引登録結果</param>
		public ReauthActionResult(
			bool result,
			string orderId,
			string paymentMemo,
			string cardTranId = "",
			string paymentOrderId = "",
			string cardTranIdForLog = "",
			string apiErrorMessage = "",
			bool authLostForError = false,
			TransactionResultElement transactionResult = null)
		{
			this.Result = result;
			this.OrderId = orderId;
			this.PaymentMemo = paymentMemo;
			this.CardTranId = cardTranId;
			this.PaymentOrderId = paymentOrderId;
			this.CardTranIdForLog = cardTranIdForLog;
			this.ApiErrorMessage = apiErrorMessage;
			this.AuthLostForError = authLostForError; 
			this.IsAuthResultHold = false;
			this.TransactionResult = transactionResult;
		}
		#endregion

		#region プロパティ
		/// <summary>結果</summary>
		public bool Result { get; private set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; private set; }
		/// <summary>決済連携メモ</summary>
		public string PaymentMemo { get; private set; }
		/// <summary>決済カード取引ID</summary>
		/// <remarks>与信成功時のみ格納</remarks>
		public string CardTranId { get; private set; }
		/// <summary>決済注文ID</summary>
		public string PaymentOrderId { get; private set; }
		/// <summary>決済カード取引ID（ログ出力用）</summary>
		/// <remarks>
		/// 与信時は取得した決済取引ID
		/// 　キャンセル時は利用した決済取引ID
		/// </remarks>
		public string CardTranIdForLog { get; private set; }
		/// <summary>APIエラーメッセージ</summary>
		public string ApiErrorMessage { get; private set; }
		/// <summary>エラー時に与信が失われるかどうか</summary>
		public bool AuthLostForError { get; set; }
		/// <summary>与信結果がHOLDか(現在はコンビニ後払い「DSK」のみ利用)</summary>
		public bool IsAuthResultHold { get; set; }
		/// <summary>取引登録結果</summary>
		public TransactionResultElement TransactionResult { get; set; }
		#endregion
	}
}