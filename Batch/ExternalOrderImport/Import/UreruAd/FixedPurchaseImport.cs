/*
=========================================================================================================
  Module      : つくーるAPI連携：定期注文情報登録 (FixedPurchaseImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Order.Register;
using w2.Commerce.Batch.ExternalOrderImport.Entity;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalOrderImport.Import.UreruAd
{
	/// <summary>
	/// つくーるAPI連携：定期注文情報登録
	/// </summary>
	public class FixedPurchaseImport : UreruAdImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseData">レスポンスデータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public FixedPurchaseImport(UreruAdResponseDataItem responseData, SqlAccessor accessor)
			: base(responseData, accessor)
		{
		}

		/// <summary>
		/// 登録
		/// </summary>
		public override void Import()
		{
			var fixedPurchase = CreateImportData();

			// Register And Update Fixed Purchase Info For Import Order Ureru
			new FixedPurchaseRegister()
				.RegisterAndUpdateFixedPurchaseInfoForImportOrderUreru(
					fixedPurchase,
					this.ResponseData.OrderId,
					Constants.FLG_LASTCHANGED_BATCH,
					this.Accessor);

			var paymentKbn = ValueText.GetValueText(Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_ORDER_PAYMENT_KBN,
				this.ResponseData.PaymentMethod);

			//通常注文が仮注文の場合、定期台帳も仮登録のまま維持
			if (((this.ResponseData.ExternalImportStatus == Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_SUCCESS)
					&& (paymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& string.IsNullOrEmpty(this.ResponseData.NpTransactionId)))
			{
				// Update Fixed Purchase Status Temp To Normal
				new FixedPurchaseService()
					.UpdateFixedPurchaseStatusTempToNormal(
						this.ResponseData.OrderId,
						fixedPurchase.FixedPurchaseId,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.DoNotInsert,
						this.Accessor);
			}

			// 頒布会商品切り替え
			if (fixedPurchase.IsSubsctriptionBox)
			{
				var container = new FixedPurchaseService().GetContainer(
					fixedPurchase.FixedPurchaseId,
					false,
					this.Accessor);

				var updateResult = new SubscriptionBoxService().GetFixedPurchaseNextProduct(
					container.SubscriptionBoxCourseId,
					container.FixedPurchaseId,
					container.MemberRankId,
					container.NextShippingDate.Value,
					container.SubscriptionBoxOrderCount + 1,
					container.Shippings[0],
					this.Accessor);

				new FixedPurchaseService().UpdateNextDeliveryForSubscriptionBox(
					fixedPurchase.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_BATCH,
					Constants.W2MP_POINT_OPTION_ENABLED,
					updateResult,
					UpdateHistoryAction.DoNotInsert,
					this.Accessor);
			}

			new UpdateHistoryService().InsertForFixedPurchase(
				fixedPurchase.FixedPurchaseId,
				Constants.FLG_LASTCHANGED_BATCH,
				this.Accessor);
		}

		/// <summary>
		/// インポートデータ生成
		/// </summary>
		/// <returns>定期注文情報</returns>
		private FixedPurchaseModel CreateImportData()
		{
			var orderKbn = ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_ORDER_KBN, this.ResponseData.Type.ToUpper());
			var paymentKbn = ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, this.ResponseData.PaymentMethod);
			var nextShippingDate = GetNextShippingDate();
			var intervalDays = (nextShippingDate - DateTime.Now.Date).Days;
			var subscriptionBoxCourseId = this.ResponseData.GetSubscriptionBoxCourseId();
			var subscriptionBoxFixedAmount = 0m;
			if (string.IsNullOrEmpty(subscriptionBoxCourseId) == false)
			{
				var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);
				subscriptionBoxFixedAmount = (subscriptionBox != null) ? (subscriptionBox.FixedAmount ?? 0m) : 0m;
			}

			var fixedPurchase = new FixedPurchaseModel
			{
				FixedPurchaseId = this.ResponseData.FixedPurchaseId,
				FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS,
				FixedPurchaseSetting1 = intervalDays.ToString(),
				LastOrderDate = this.ResponseData.Created,
				OrderCount = 1,
				UserId = this.ResponseData.User.UserId,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				OrderKbn = string.IsNullOrEmpty(orderKbn)
					? Constants.FLG_FIXEDPURCHASE_ORDER_KBN_PC
					: orderKbn,
				OrderPaymentKbn = paymentKbn,
				FixedPurchaseDateBgn = this.ResponseData.Created.Value,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH,
				CreditBranchNo = (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					? this.ResponseData.CreditBranchNo
					: (int?)null,
				ExternalPaymentAgreementId = ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
					|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
						? this.ResponseData.AmazonPaymentsId
						: string.Empty,
				NextShippingDate = nextShippingDate,
				NextNextShippingDate = nextShippingDate.AddDays((double)intervalDays),
				FixedPurchaseManagementMemo = this.ResponseData.Note,
				CardInstallmentsCode = OrderCommon.GetCreditInstallmentsDefaultValue(),
				SubscriptionBoxCourseId = subscriptionBoxCourseId,
				SubscriptionBoxFixedAmount = subscriptionBoxFixedAmount,
				SubscriptionBoxOrderCount = string.IsNullOrEmpty(subscriptionBoxCourseId)
					? 0
					: 1,
				Shippings = CreateFixedPurchaseShipping()
			};

			// Update setting fixed purchase next shipping item
			UpdateSettingFixedPurchaseNextShippingItem(fixedPurchase);

			return fixedPurchase;
		}

		/// <summary>
		/// 定期購入配送先情報生成
		/// </summary>
		/// <returns>定期購入配送先情報</returns>
		private FixedPurchaseShippingModel[] CreateFixedPurchaseShipping()
		{
			var shipping = new FixedPurchaseShippingModel
			{
				FixedPurchaseId = this.ResponseData.FixedPurchaseId,
				ShippingName = this.ResponseData.User.Name,
				ShippingName1 = this.ResponseData.User.Name1,
				ShippingName2 = this.ResponseData.User.Name2,
				ShippingZip = this.ResponseData.User.Zip,
				ShippingAddr1 = this.ResponseData.User.Addr1,
				ShippingAddr2 = this.ResponseData.User.Addr2,
				ShippingAddr3 = this.ResponseData.User.Addr3,
				ShippingAddr4 = this.ResponseData.User.Addr4,
				ShippingTel1 = this.ResponseData.User.Tel1,
				ShippingTel2 = this.ResponseData.User.Tel2,
				ShippingTel3 = this.ResponseData.User.Tel3,
				ShippingNameKana = this.ResponseData.User.NameKana,
				ShippingNameKana1 = this.ResponseData.User.NameKana1,
				ShippingNameKana2 = this.ResponseData.User.NameKana2,
				ShippingTime = string.Empty,
				ShippingCompanyName = string.Empty,
				ShippingCompanyPostName = string.Empty,
				ShippingMethod = StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().ShippingMethod),
				DeliveryCompanyId = StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().DeliveryCompanyId),
				ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF,
				ShippingReceivingStoreId = string.Empty,
				ShippingReceivingStoreType = string.Empty,
			};
			shipping.Items = CreateFixedPurchaseItem(shipping.FixedPurchaseShippingNo);
			return new [] { shipping };
		}

		/// <summary>
		/// 定期購入商品情報生成
		/// </summary>
		/// <param name="fixedPurchaseShippingNo">定期購入配送先枝番</param>
		/// <returns>定期購入商品情報</returns>
		private FixedPurchaseItemModel[] CreateFixedPurchaseItem(int fixedPurchaseShippingNo)
		{
			var items = this.ResponseData.FixedPurchaseItemList.Select((item, index) =>
				new FixedPurchaseItemModel
				{
					FixedPurchaseId = this.ResponseData.FixedPurchaseId,
					FixedPurchaseItemNo = index + 1,
					FixedPurchaseShippingNo = fixedPurchaseShippingNo,
					ShopId = Constants.CONST_DEFAULT_SHOP_ID,
					ProductId = item.ProductId,
					VariationId = item.VariationId,
					SupplierId = item.SupplierId,
					ItemQuantity = item.ItemQuantity,
					ItemQuantitySingle = item.ItemQuantitySingle,
					ProductOptionTexts = string.Empty,
					ItemOrderCount = 1
				}).ToArray();
			return items;
		}

		/// <summary>
		/// 次回配送日取得
		/// </summary>
		/// <returns>次回配送日(配送種別の最低配送間隔＋配送種別の配送指定可能範囲From＋配送種別の休日を考慮した出荷所要営業日数)</returns>
		private DateTime GetNextShippingDate()
		{
			var shopShipping = GetShopShipping();
			var addDay = 0;

			if (shopShipping != null)
			{
				addDay = (shopShipping.ShippingDateSetFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID)
					? shopShipping.BusinessDaysForShipping + shopShipping.ShippingDateSetBegin.GetValueOrDefault(0) + shopShipping.FixedPurchaseMinimumShippingSpan
					: shopShipping.FixedPurchaseMinimumShippingSpan;
			}

			var nextShippingDate = DateTime.Now.Date.AddDays((double)addDay);
			return nextShippingDate;
		}

		/// <summary>
		/// Get Shop Shipping
		/// </summary>
		/// <returns>Shop Shipping Model</returns>
		private ShopShippingModel GetShopShipping()
		{
			var shopShipping = new ShopShippingService().Get(Constants.CONST_DEFAULT_SHOP_ID, this.ResponseData.ShippingId);
			return shopShipping;
		}

		/// <summary>
		/// Update Setting Fixed Purchase Next Shipping Item
		/// </summary>
		/// <param name="fixedPurchase">Fixed Purchase</param>
		private void UpdateSettingFixedPurchaseNextShippingItem(FixedPurchaseModel fixedPurchase)
		{
			if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED == false) return;

			var cartFixedPurchaseNextShippingProduct = this.ResponseData.Cart.Items
				.Where(item => item.IsFixedPurchase)
				.FirstOrDefault(cartProductInfo => cartProductInfo.CanSwitchProductFixedPurchaseNextShippingSecondTime());
			if (cartFixedPurchaseNextShippingProduct != null)
			{
				this.ResponseData.Cart.GetShipping().CanSwitchProductFixedPurchaseNextShippingSecondTime = true;
				var shopShipping = GetShopShipping();
				var fixedPurchaseMinSpan = (shopShipping != null)
					? shopShipping.FixedPurchaseMinimumShippingSpan
					: 0;
				this.ResponseData.Cart.GetShipping().UpdateFixedPurchaseSetting(
					string.Empty,
					string.Empty,
					0,
					fixedPurchaseMinSpan);
				this.ResponseData.Cart.GetShipping().UpdateNextShippingDates(
					fixedPurchase.NextShippingDate.Value,
					fixedPurchase.NextNextShippingDate.Value);
				this.ResponseData.Cart.GetShipping().UpdateNextShippingItemFixedPurchaseInfos(
					cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn,
					cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting);
				this.ResponseData.Cart.GetShipping().CalculateNextShippingItemNextNextShippingDate();
				fixedPurchase.NextNextShippingDate = this.ResponseData.Cart.GetShipping().NextNextShippingDate;
				fixedPurchase.FixedPurchaseKbn = cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn;
				fixedPurchase.FixedPurchaseSetting1 = cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting;
			}
		}
	}
}
