/*
=========================================================================================================
  Module      : 後付款(TriLink後払い)ヘルパ(TriLinkAfterPayHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Helper
{
	/// <summary>
	/// 後付款(TriLink後払い)ヘルパ
	/// </summary>
	public class TriLinkAfterPayHelper
	{
		#region #CheckUsedPaymentForTriLinkAfterPay 注文者の住所国と配送先国から、後付款(TriLink後払い)が利用可能かを判定
		/// <summary>
		///  注文者の住所国と配送先国から、後付款(TriLink後払い)が利用可能かを判定
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="ownerShippingCountryIsoCodes">注文者・配送先の国コードリスト</param>
		/// <returns>不可能:True 可能:False</returns>
		public static bool CheckUsedPaymentForTriLinkAfterPay(
			string paymentId,
			params string[] ownerShippingCountryIsoCodes)
		{
			var result = ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				&& ownerShippingCountryIsoCodes.Any(code => code != Constants.COUNTRY_ISO_CODE_TW));
			return result;
		}
		#endregion
	}
}
