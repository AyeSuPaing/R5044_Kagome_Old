/*
=========================================================================================================
  Module      : ベリトランスクレジット会員管理マネージャー(AccountManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using jp.veritrans.tercerog.mdk;
using jp.veritrans.tercerog.mdk.dto;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Payment.Veritrans
{
	/// <summary>
	/// ベリトランス会員管理マネージャー
	/// </summary>
	public class AccountManager : PaymentVeriTransBase
	{
		/// <summary>ベリトランス決済種別</summary>
		protected override VeriTransConst.VeritransPaymentKbn VeritransPaymentType { get { return VeriTransConst.VeritransPaymentKbn.Credit; } }

		/// <summary>
		/// 会員登録 かつ カード登録
		/// </summary>
		/// <param name="accountId">会員番号</param>
		/// <param name="token">トークンID</param>
		/// <returns>API戻りDTO</returns>
		public AccountAddResponseDto AddCardByUser(string accountId, string token)
		{
			var requestData = new AccountAddRequestDto
			{
				AccountId = accountId,
				CreateDate = DateTime.Now.ToString("yyyyMMdd"),
				Token = token,
			};

			var transaction = new Transaction();
			var responseData = (AccountAddResponseDto)transaction.Execute(requestData);
			return responseData;
		}

		/// <summary>
		/// ベリトランス 会員復元 定期解約時
		/// </summary>
		/// <param name="container">定期購入情報</param>
		/// <remarks>
		/// 1カード情報=1会員として管理しているため、本来退会は必要としていない
		/// 古いバージョンにて定期解約時に退会となっているため複会のために本機能を実装する
		/// 定期解約してもベリトランス側の会員情報は退会しない仕様となっている
		/// </remarks>
		public static void RestoreForFixedPurchaseCancel(FixedPurchaseContainer container)
		{
			if (container == null) return;

			if (((container.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
				&& container.CreditBranchNo.HasValue) == false) return;

			var userCreditCard = new UserCreditCardService().Get(
				container.UserId,
				container.CreditBranchNo.Value);

			if (userCreditCard != null)
			{
				// 不具合修正にて古いバージョンで退会された定期を復会させるた本処理を導入している
				// 現在の仕様では解約しても退会しないとしているため、通常フローであれば復会API連携ではエラーで返される
				// そのためエラー時の処理は実装せず、エラーで退会状態となった場合は管理画面にて手動対応となる
				// 退会状態で止まった際の検知は注文作成時
				new AccountManager().Restore(userCreditCard.CooperationId);
			}
		}

		/// <summary>
		/// 会員復元
		/// </summary>
		/// <param name="accountId">会員ID</param>
		/// <returns>API戻りDTO</returns>
		private AccountRestoreResponseDto Restore(string accountId)
		{
			var requestData = new AccountRestoreRequestDto()
			{
				AccountId = accountId,
			};

			var transaction = new Transaction();
			var responseData = (AccountRestoreResponseDto)transaction.Execute(requestData);
			return responseData;
		}
	}
}
