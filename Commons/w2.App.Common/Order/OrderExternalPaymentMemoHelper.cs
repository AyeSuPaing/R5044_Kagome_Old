/*
=========================================================================================================
  Module      : 外部決済メモ系ヘルパクラス (OrderExternalPaymentMemoHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 外部決済メモ系ヘルパクラス
	/// </summary>
	public class OrderExternalPaymentMemoHelper
	{
		/// <summary>
		/// 決済連携メモ作成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID／注文ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="cardTranId">決済連携ID</param>
		/// <param name="actionNameWithoutPaymentName">決済種別名抜きアクション名</param>
		/// <param name="lastBilledAmount">最終請求金額（キャンセルはnull）</param>
		/// <returns>決済連携メモ</returns>
		public static string CreateOrderPaymentMemo(
			string paymentOrderId,
			string paymentId,
			string cardTranId,
			string actionNameWithoutPaymentName,
			decimal? lastBilledAmount)
		{
			var pamyentName = GetOrderPaymentName(paymentId);
			var memo = CreateOrderPaymentMemo(
				paymentId,
				paymentOrderId,
				cardTranId,
				pamyentName + actionNameWithoutPaymentName,
				lastBilledAmount,
				false);
			return memo;
		}
		/// <summary>
		/// 決済連携メモ作成
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="paymentOrderId">決済注文ID／注文ID</param>
		/// <param name="cardTranId">決済連携ID</param>
		/// <param name="actionName">アクション名</param>
		/// <param name="lastBilledAmount">最終請求金額（キャンセルはnull）</param>
		/// <param name="isReauth">再与信の処理であるかどうか</param>
		/// <returns>決済連携メモ</returns>
		public static string CreateOrderPaymentMemo(
			string paymentId,
			string paymentOrderId,
			string cardTranId,
			string actionName,
			decimal? lastBilledAmount,
			bool isReauth)
		{
			var settlementCurrency = CurrencyManager.GetSettlementCurrency(paymentId);
			var priceString = lastBilledAmount.HasValue
				? Constants.GLOBAL_OPTION_ENABLE
					? CurrencyManager.ToSettlementCurrencyNotation(
						lastBilledAmount.Value,
						settlementCurrency,
						true)
					: string.Format(" {0}円", lastBilledAmount.ToPriceDecimal())
				: "";

			var logFormat = "{0:yyyy/MM/dd HH:mm} 決済取引ID：{1}・{2}・{3}{4} {5}{6}";
			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				logFormat = "{0:yyyy/MM/dd HH:mm:ss} 決済取引ID：{2}・決済注文ID：{1}・{3} {5}";
			}
			var paymentMemo = string.Format(
				logFormat,
				DateTime.Now,
				paymentOrderId,
				cardTranId,
				priceString,
				(isReauth ? " 再与信：" : string.Empty),
				actionName,
				(Constants.GLOBAL_OPTION_ENABLE && (Regex.IsMatch(actionName, Constants.ACTION_NAME_SHIPPING_REPORT + "$") == false)
					? "・決済レート："
						+ DecimalUtility.DecimalRound(CurrencyManager.GetSettlementRate(settlementCurrency), DecimalUtility.Format.RoundDown, 6).ToString()
					: string.Empty));
			return paymentMemo;
		}

		/// <summary>
		/// 決済種別名取得
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>決済種別名</returns>
		private static string GetOrderPaymentName(string paymentId)
		{
			switch (paymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					return "クレジットカード決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT:
					return "代金引換";

				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE:
					return "コンビニ前払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
					return "コンビニ後払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE:
					return "銀行振込前払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF:
					return "銀行振込後払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE:
					return "郵便振込前払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF:
					return "郵便振込後払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG:
				case Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG:
				case Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG:
				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
				case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
				case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
				case Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS:
					return "キャリア決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
					return "リクルートかんたん支払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
					return " 楽天ペイ";

				case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
					return "Amazon Pay決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
					return "Amazon Pay(CV2)決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS:
					return "PayPal決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
					return "後付款（TriLink後払い）";

				case Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT:
					return "決済なし";

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
					return "Paidy決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
					return "atone翌月払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					return "aftee翌月払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
					return "LINEPay決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
					return "NP後払い";

				case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
					return "ECPay決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF:
					return "DSKコンビニ（後払い）";

				case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
					return "藍新Pay決済";

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					return "PayPay";

				case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
					return "Boku";

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
					return "GMO掛け払い（都度払い）";

				case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
					return "GMO掛け払い（枠保証）";

				case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
					return "GMOアトカラ";
			}

			return paymentId;
		}

		/// <summary>
		/// キャリア決済かどうか判定
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>判定結果</returns>
		public static bool IsPaymentCareer(string paymentId)
		{
			return GetOrderPaymentName(paymentId) == "キャリア決済";
		}
	}
}
