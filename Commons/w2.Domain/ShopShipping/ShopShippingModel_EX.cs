/*
=========================================================================================================
  Module      : 店舗配送種別マスタモデル (ShopShippingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.Holiday.Helper;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 店舗配送種別マスタモデル
	/// </summary>
	public partial class ShopShippingModel
	{
		#region 定数
		/// <summary>配送希望時間帯保持数</summary>
		public const int CONST_SHIPPING_TIME_COUNT = 8;
		#endregion

		#region メソッド
		/// <summary>
		/// 初期配送会社情報
		/// </summary>
		/// <param name="isExpressDelivery">宅配便か</param>
		/// <returns>配送会社</returns>
		public ShopShippingCompanyModel GetDefaultDeliveryCompany(bool isExpressDelivery)
		{
			var defaultCompany = (isExpressDelivery
				? this.CompanyListExpress
				: this.CompanyListMail)
				.First(company => company.IsDefault);
			return defaultCompany;
		}

		/// <summary>
		/// 許可された決済種別か
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>許可されているか</returns>
		public bool IsPermittedPayment(string paymentId)
		{
			var isPermitted = ((this.IsValidPaymentSelectionFlg == false)
				|| string.IsNullOrEmpty(this.PermittedPaymentIds)
				|| this.PermittedPaymentIds.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Contains(paymentId));
			return isPermitted;
		}

		/// <summary>
		/// 定期購入区分1(月間隔日付選択)設定値として有効か
		/// </summary>
		/// <param name="month">月間隔設定値</param>
		/// <returns>有効か</returns>
		public bool IsValidFixedPurchaseKbn1Setting(string month)
		{
			var isValid = this.FixedPurchaseKbn1Setting.Split(',').Contains(month);
			return isValid;
		}

		/// <summary>
		/// 定期購入区分3(指定日間隔選択)設定値として有効か
		/// </summary>
		/// <param name="intervalDay">日間隔設定値</param>
		/// <returns>有効か</returns>
		public bool IsValidFixedPurchaseKbn3Setting(string intervalDay)
		{
			var isValid = this.FixedPurchaseKbn3Setting.Split(',').Contains(intervalDay);
			return isValid;
		}

		/// <summary>
		/// 定期購入区分4(指定週間隔選択)設定値として有効か
		/// </summary>
		/// <param name="intervalWeek">週間隔設定値</param>
		/// <returns>有効か</returns>
		public bool IsValidFixedPurchaseKbn4Setting(string intervalWeek)
		{
			var isValid = this.FixedPurchaseKbn4Setting1.Split(',').Contains(intervalWeek);
			return isValid;
		}

		/// <summary>
		/// 配送希望日が配送可能日付範囲内か(休日設定考慮)
		/// </summary>
		/// <param name="orderDate">注文日</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <returns>配送可能日付範囲内か</returns>
		public bool IsValidShippingDate(DateTime orderDate, DateTime? shippingDate)
		{
			// 配送希望日設定不可or配送希望日指定なしの場合はfalse
			if ((this.IsValidShippingDateSetFlg == false) || (shippingDate.HasValue == false)) return false;

			var canShippingDateSetBegin = this.ShippingDateSetBegin.GetValueOrDefault(0) + this.BusinessDaysForShipping;
			var canShippingDateSetEnd = this.ShippingDateSetEnd.GetValueOrDefault(0) + this.BusinessDaysForShipping;
			var isValid = ((shippingDate.Value.Date >= HolidayUtil.GetDateOfBusinessDay(orderDate.Date, canShippingDateSetBegin, true))
				&& (shippingDate.Value.Date <= HolidayUtil.GetDateOfBusinessDay(orderDate.Date, canShippingDateSetEnd, true)));
			return isValid;
		}

		/// <summary>
		/// Has Convenience Store EcPay 
		/// </summary>
		/// <param name="isExpressDelivery">Is express delivery</param>
		/// <param name="deliveryCompanyId">Delivery company id</param>
		/// <returns>True: has delivery company id or False: has not delivery company id</returns>
		public bool HasConvenienceStoreEcPay(bool isExpressDelivery, string deliveryCompanyId)
		{
			var companyList = isExpressDelivery
				? this.CompanyListExpress
				: this.CompanyListMail;
			var hasConvenienceStoreEcPay = companyList
				.Any(item => (item.DeliveryCompanyId == deliveryCompanyId));
			return hasConvenienceStoreEcPay;
		}

		/// <summary>
		/// Can use fixed purchase first shipping date next month
		/// </summary>
		/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
		/// <returns>True if can use fixed purchase first shipping date next month, otherwise false</returns>
		public bool CanUseFixedPurchaseFirstShippingDateNextMonth(string fixedPurchaseKbn)
		{
			var result = (this.FixedPurchaseFirstShippingNextMonthFlg
				== Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_VALID)
					&& ((fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
						|| (fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY));
			this.IsFixedPurchaseFirstShippingDateNextMonth = result;
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>地域リスト</summary>
		public ShopShippingZoneModel[] ZoneList
		{
			get { return (ShopShippingZoneModel[])this.DataSource["EX_ZoneList"]; }
			set { this.DataSource["EX_ZoneList"] = value; }
		}
		/// <summary>配送日設定可能フラグが有効か</summary>
		public bool IsValidShippingDateSetFlg
		{
			get { return ((this.ShippingDateSetFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID) || this.IsFixedPurchaseFirstShippingDateNextMonth); }
		}
		/// <summary>のし利用フラグが有効か</summary>
		public bool IsValidWrappingPaperFlg
		{
			get { return (this.WrappingPaperFlg == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID); }
		}
		/// <summary>包装利用フラグが有効か</summary>
		public bool IsValidWrappingBagFlg
		{
			get { return (this.WrappingBagFlg == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID); }
		}
		/// <summary>定期購入区分1設定可能フラグが有効か</summary>
		public bool IsValidFixedPurchaseKbn1Flg
		{
			get { return (this.FixedPurchaseKbn1Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_VALID); }
		}
		/// <summary>定期購入区分2設定可能フラグが有効か</summary>
		public bool IsValidFixedPurchaseKbn2Flg
		{
			get { return (this.FixedPurchaseKbn2Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_VALID); }
		}
		/// <summary>定期購入区分3設定可能フラグが有効か</summary>
		public bool IsValidFixedPurchaseKbn3Flg
		{
			get { return (this.FixedPurchaseKbn3Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_VALID); }
		}
		/// <summary>定期購入区分4設定可能フラグが有効か</summary>
		public bool IsValidFixedPurchaseKbn4Flg
		{
			get { return (this.FixedPurchaseKbn4Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_VALID); }
		}
		/// <summary>配送会社リスト</summary>
		public ShopShippingCompanyModel[] CompanyList
		{
			get { return (ShopShippingCompanyModel[])this.DataSource["EX_CompanyList"]; }
			set { this.DataSource["EX_CompanyList"] = value; }
		}
		/// <summary>配送会社リスト（宅配便）</summary>
		public ShopShippingCompanyModel[] CompanyListExpress
		{
			get { return (this.CompanyList.Where(company => company.IsExpress)).ToArray(); }
		}
		/// <summary>配送会社リスト（メール便）</summary>
		public ShopShippingCompanyModel[] CompanyListMail
		{
			get { return (this.CompanyList.Where(company => company.IsMail)).ToArray(); }
		}
		/// <summary>決済選択の任意利用フラグが有効か</summary>
		public bool IsValidPaymentSelectionFlg
		{
			get { return (this.PaymentSelectionFlg == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID); }
		}
		/// <summary>次回配送日までの最短所要日数(休日設定未考慮)</summary>
		public int FixedPurchaseNextShippingShortestIntervalDays
		{
			get { return (this.ShippingDateSetBegin.GetValueOrDefault(0) + this.FixedPurchaseMinimumShippingSpan); }
		}
		/// <summary>配送パターンの指定表示/非表示</summary>
		public bool IsFixedPurchaseShippingDisplay
		{
			get
			{
				if (this.FixedPurchaseShippingNotDisplayFlg
					== Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_DEFAULT_SETTING_FLG_VALID) return false;
				return true;
			}
		}
		/// <summary>配送会社ごとの配送料設定</summary>
		public ShippingDeliveryPostageModel[] CompanyPostageSettings
		{
			get { return (ShippingDeliveryPostageModel[])this.DataSource["EX_CompanyPostageSettings"]; }
			set { this.DataSource["EX_CompanyPostageSettings"] = value; }
		}
		/// <summary>定期購入設定可能フラグが有効か</summary>
		public bool IsValidFixedPurchaseKbnFlg
		{
			get { return (this.FixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID); }
		}
		/// <summary>Is fixed purchase first shipping date next month</summary>
		public bool IsFixedPurchaseFirstShippingDateNextMonth { get; set; }
		#endregion
	}
}
