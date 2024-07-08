/*
=========================================================================================================
  Module      : YAHOOモール注文決済クラス (YahooMallOrderPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders
{
	/// <summary>
	/// YAHOOモール注文決済クラス
	/// </summary>
	public class YahooMallOrderPayment
	{
		/// <summary>Yahoo社側で定義された決済ID PAYPAY</summary>
		private const string PAYMETHOD_PAYPAY = "payment_a17";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mapper">Yahooモール注文決済マッピングクラス</param>
		/// <param name="payMethod">支払い方法</param>
		/// <param name="combinedPayMethod">併用お支払い方法</param>
		/// <param name="payMethodAmount">支払い金額</param>
		/// <param name="combinedPayMethodAmount">併用支払い金額</param>
		/// <param name="totalPrice">合計金額</param>
		/// <param name="usePoint">利用ポイント合計</param>
		/// <param name="giiftWrapCharge">ギフト包装料</param>
		/// <param name="mallCouponDiscount">モールクーポン値引き額</param>
		public YahooMallOrderPayment(
			string payMethod,
			string combinedPayMethod,
			string payMethodAmount,
			string combinedPayMethodAmount,
			string totalPrice,
			string usePoint,
			string mallCouponDiscount,
			string giiftWrapCharge,
			YahooMallOrderPaymentMapper mapper)
		{
			// PayPay支払いかどうか
			var isPaidWithPayPay = payMethod == PAYMETHOD_PAYPAY;
			this.IsPaidWithPayPay = isPaidWithPayPay;

			// PayPay以外の方法でのみ決済している => "PayMethod" をメイン決済方法とする。
			// PayPayで全て決済している => "PayMethod" をメイン決済方法とする。
			// PayPayで部分的に決済している => "CombinedPayMethod" をメイン決済方法とする。
			var isPaidOnlyWithPayPay = isPaidWithPayPay && string.IsNullOrEmpty(combinedPayMethod);
			var mainPayMethod = isPaidWithPayPay == false || isPaidOnlyWithPayPay ? payMethod : combinedPayMethod;

			this.OrderPriceRegulation = (decimal.TryParse(usePoint, out var parsedUsePoint) ? parsedUsePoint * -1 : 0)
				+ (decimal.TryParse(mallCouponDiscount, out var parsedMallCouponDiscount) ? parsedMallCouponDiscount * -1 : 0)
				+ (decimal.TryParse(giiftWrapCharge, out var parsedGiiftWrapCharge) ? parsedGiiftWrapCharge : 0);

			// 支払方法、支払いステータス
			foreach (var setting in mapper.MappingConfig.Settings)
			{
				if (setting.YahooPayMethodPhysicalName != mainPayMethod) continue;

				this.PaymentKbn = setting.PaymentKbn;
				this.PaymentStatus = setting.PaymentStatus;
				break;
			}
			if (string.IsNullOrEmpty(this.PaymentKbn) || string.IsNullOrEmpty(this.PaymentStatus))
			{
				throw new ArgumentException($"想定外の値です。PayMethod={payMethod}");
			}
			
			// PayPay以外の方法でのみ決済している場合、DBのこの値を更新しないため、そのほかのプロパティはデフォルト値でよい。
			// したがって、ここで返却する
			if (isPaidWithPayPay == false) return;

			// PayPay支払いと別決済を併用して支払っているか
			if (isPaidOnlyWithPayPay == false)
			{
				// 調整金額メモ
				if (decimal.TryParse(payMethodAmount, out var parsedPayMethodAmount) == false)
				{
					throw new ArgumentException("Detail.PayMethodAmount の値が不正です。");
				}
				this.RegulationMemo = $"[PayPay利用額]{parsedPayMethodAmount}円";

				// 合計金額
				if (decimal.TryParse(
						totalPrice,
						out var parsedTotalPrice) == false)
				{
					throw new ArgumentException("Detail.TotalPrice の値が不正です。");
				}
				this.TotalPrice = parsedTotalPrice;
			}
			// 全額PayPay払い
			else
			{
				if (decimal.TryParse(totalPrice, out var parsedTotalPrice) == false)
				{
					throw new Exception("Detail.TotalPrice の値が不正です。");
				}
				this.RegulationMemo = $"[PayPay利用額]{parsedTotalPrice}円";
				this.TotalPrice = parsedTotalPrice;
			}
		}

		/// <summary>支払い方法</summary>
		public string PaymentKbn { get; } = "";
		/// <summary>支払いステータス</summary>
		public string PaymentStatus { get; } = "";
		/// <summary>合計金額</summary>
		/// <remarks>PayPay以外の方法でのみ決済している場合、DBのこの値を更新しないため、デフォルト値でよい。</remarks>
		public decimal TotalPrice { get; }
		/// <summary>調整金額メモ</summary>
		/// <remarks>PayPay以外の方法でのみ決済している場合、DBのこの値を更新しないため、デフォルト値でよい。</remarks>
		public string RegulationMemo { get; } = "";
		/// <summary>調整金額</summary>
		/// <remarks>PayPay以外の方法でのみ決済している場合、DBのこの値を更新しないため、デフォルト値でよい。</remarks>
		public decimal OrderPriceRegulation { get; }
		/// <summary>PAYPAYで決済したかどうか</summary>
		public bool IsPaidWithPayPay { get; }
	}
}
