/*
=========================================================================================================
  Module      : Order response(OrderResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Linq;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Common.Util;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Order response
	/// </summary>
	[Serializable]
	public class OrderResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="workflowType">Workflow type</param>
		public OrderResponse(DataRowView source, string workflowType)
		{
			if (workflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				var order = ConvertOrderFromDataRowView(source);
				this.OrderId = order.OrderId;
				this.MallId = CreateSiteNameForDetail(order.MallId, string.Empty);
				this.OrderKbn = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_KBN,
					order.OrderKbn);
				this.PaymentKbn = ValueText.GetValueText(
					Constants.TABLE_PAYMENT,
					Constants.VALUETEXT_PARAM_PAYMENT_TYPE,
					order.OrderPaymentKbn);
				this.OrderDate = order.OrderDate.ToString();
				this.OrderStatus = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_STATUS,
					order.OrderStatus);
				this.PriceTotal = StringUtility.ToPrice(order.OrderPriceTotal);
				this.PaymentStatus = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
					order.OrderPaymentStatus);
				this.OwnerId = order.UserId;
				this.OwnerName = GetOrderOwnerName(order.UserId, order.Owner.OwnerName);
				this.OwnerTel = order.Owner.OwnerTel1;
				this.OwnerMail = order.Owner.OwnerMailAddr;
				this.OwnerKbn = ValueText.GetValueText(
					Constants.TABLE_ORDEROWNER,
					Constants.FIELD_ORDEROWNER_OWNER_KBN,
					order.Owner.OwnerKbn);
				var shipping = order.Shippings.FirstOrDefault();
				this.ManagementLevel = GetUserManagementLevelName(order.UserManagementLevelId);
				this.ShippingAddress = shipping.ConcatenateAddressWithoutCountryName();
				this.ShippingName = shipping.ShippingName;
				this.ShippingTel = shipping.ShippingTel1;
				var shippingDate = DateTimeUtility.ToStringForManager(
					shipping.ShippingDate,
					DateTimeUtility.FormatType.LongMonthDay);
				this.ShippingDate = string.IsNullOrEmpty(shippingDate)
					? CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@")
					: shippingDate;
				var shippingTime = GetShippingTimeMessage(
					shipping.DeliveryCompanyId,
					shipping.ShippingTime);
				this.ShippingTime = string.IsNullOrEmpty(shippingTime)
					? CommonPage.ReplaceTag("@@DispText.shipping_time_list.none@@")
					: shippingTime;
				this.ManagementMemo = order.ManagementMemo;
				this.ShippingMemo = order.ShippingMemo;
				this.MemberRankId = order.MemberRankId;
				this.OrderGiftFlg = order.IsGiftOrder
					? "*"
					: string.Empty;
				this.ReturnExchangeKbn = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN,
					order.ReturnExchangeKbn);
				var returnExchangeReceiptDate = DateTimeUtility.ToStringForManager(
					order.OrderReturnExchangeReceiptDate.ToString(),
					DateTimeUtility.FormatType.ShortDate2Letter);
				this.ReturnExchangeReceiptDate = returnExchangeReceiptDate;
				this.ReturnExchangeStatus = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS,
					order.OrderReturnExchangeStatus);
				this.RepaymentStatus = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS,
					order.OrderRepaymentStatus);
				this.CardTranId = order.CardTranId;
				this.OrderMemo = order.Memo;
				this.DigitalContentsFlg = (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON)
					? "*"
					: string.Empty;
				this.ShippedStatus = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS,
					order.OrderShippedStatus);
				this.StockreservedStatus = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS,
					order.OrderStockreservedStatus);
				var scheduledShippingDate = DateTimeUtility.ToStringForManager(
					shipping.ScheduledShippingDate,
					DateTimeUtility.FormatType.LongMonthDay);
				this.ScheduledShippingDate = string.IsNullOrEmpty(scheduledShippingDate)
					? CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@")
					: scheduledShippingDate;
				this.FixedPurchaseOrderCount = (string.IsNullOrEmpty(source[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT].ToString()) == false)
					? (string.Format(
						ValueText.GetValueText(
							Constants.TABLE_ORDER,
							"purchase_count_unit",
							"count_unit"),
						source[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT]))
					: "－";
				this.FixedPurchaseShippedCount = (string.IsNullOrEmpty(source[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT].ToString()) == false)
					? (string.Format(
						ValueText.GetValueText(
							Constants.TABLE_ORDER,
							"purchase_count_unit",
							"count_unit"),
						source[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT]))
					: "－";
				this.OrderCountOrder = (string.IsNullOrEmpty(order.OrderCountOrder.ToString()) == false)
					? (string.Format(
						ValueText.GetValueText(
							Constants.TABLE_ORDER,
							"purchase_count_unit",
							"count_unit"),
						order.OrderCountOrder))
					: "－";
				this.DemandStatus = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_DEMAND_STATUS,
					order.DemandStatus);
				this.PaymentName = order.PaymentName;
				this.ShippingCompanyName = shipping.ShippingCompanyName;
				this.ShippingCompanyPostName = shipping.ShippingCompanyPostName;

			}
			else
			{
				var fixedPurchase = ConvertFixedPurchaseFromDataRowView(source);
				this.FixedPurchaseId = fixedPurchase.FixedPurchaseId;
				this.FixedPurchaseKbn = ValueText.GetValueText(
					Constants.TABLE_FIXEDPURCHASE,
					Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
					fixedPurchase.FixedPurchaseKbn);
				this.FixedPurchaseSetting = OrderCommon.CreateFixedPurchaseSettingMessage(
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1);
				this.PaymentKbn = ValueText.GetValueText(
					Constants.TABLE_PAYMENT,
					Constants.VALUETEXT_PARAM_PAYMENT_TYPE,
					StringUtility.ToEmpty(fixedPurchase.OrderPaymentKbn));
				this.FixedPurchaseDate = fixedPurchase.DateCreated.ToString();
				this.FixedPurchaseStatus = ValueText.GetValueText(
					Constants.TABLE_FIXEDPURCHASE,
					Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS,
					fixedPurchase.FixedPurchaseStatus);
				this.PaymentStatus = ValueText.GetValueText(
					Constants.TABLE_FIXEDPURCHASE,
					Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS,
					fixedPurchase.PaymentStatus);
				this.OwnerId = fixedPurchase.UserId;
				this.OwnerName = fixedPurchase.OwnerName;
				this.OwnerTel = fixedPurchase.OwnerTel1;
				this.OwnerMail = fixedPurchase.OwnerMailAddr;
				this.ManagementMemo = fixedPurchase.FixedPurchaseManagementMemo;
				this.ShippingMemo = fixedPurchase.ShippingMemo;
				this.FixedPurchaseOrderCount = fixedPurchase.OrderCount.ToString();
				this.FixedPurchaseShippingCount = fixedPurchase.ShippedCount.ToString();
				this.OwnerKbn = ValueText.GetValueText(
					Constants.TABLE_FIXEDPURCHASE,
					Constants.FIELD_FIXEDPURCHASE_ORDER_KBN,
					fixedPurchase.OwnerKbn);
				this.FixedPurchaseNextShippingDate = DateTimeUtility.ToStringForManager(
					fixedPurchase.NextShippingDate,
					DateTimeUtility.FormatType.ShortDate2Letter);
				this.FixedPurchaseNextNextShippingDate = DateTimeUtility.ToStringForManager(
					fixedPurchase.NextNextShippingDate,
					DateTimeUtility.FormatType.ShortDate2Letter);
				this.FixedPurchaseDateBgn = DateTimeUtility.ToStringForManager(
					fixedPurchase.FixedPurchaseDateBgn,
					DateTimeUtility.FormatType.ShortDate2Letter);
				this.FixedPurchaseLastOrderDate = DateTimeUtility.ToStringForManager(
					fixedPurchase.LastOrderDate,
					DateTimeUtility.FormatType.ShortDate2Letter);
				this.PaymentName = StringUtility.ToEmpty(source[Constants.FIELD_PAYMENT_PAYMENT_NAME]);
				this.SubscriptionBoxOrderCount = fixedPurchase.SubscriptionBoxOrderCount.ToString();
			}
		}

		/// <summary>
		/// Convert order from data row view
		/// </summary>
		/// <param name="drvOrder">Data row view order</param>
		/// <returns>The order model</returns>
		private OrderModel ConvertOrderFromDataRowView(DataRowView drvOrder)
		{
			var orderShippings = new []
			{
				new OrderShippingModel(drvOrder)
			};

			var orderItems = new []
			{
				new OrderItemModel(drvOrder)
			};

			var order = new OrderModel(drvOrder)
			{
				Owner = new OrderOwnerModel(drvOrder),
				Shippings = orderShippings,
				Items = orderItems
			};

			return order;
		}

		/// <summary>
		/// ユーザー名の取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="ownerName">ユーザー名</param>
		/// <returns>ユーザー名</returns>
		/// <remarks>注文を２回以上している場合はリピーターユーザーになる</remarks>
		private string GetOrderOwnerName(string userId, string ownerName)
		{
			var useList = new UserService().GetUserSymbols(userId);

			var reslut = useList
				.Where(userSymbols => (userSymbols.OrderCount > 1))
				.Aggregate(ownerName, (current, userSymbols) => (current + Constants.USERSYMBOL_REPEATER));
			return reslut;
		}

		/// <summary>
		/// Convert fixed purchase from data row view
		/// </summary>
		/// <param name="drvOrder">Data row view order</param>
		/// <returns>The fixed purchase container</returns>
		private FixedPurchaseContainer ConvertFixedPurchaseFromDataRowView(DataRowView drvOrder)
		{
			var fixedPurchaseId = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			var fixedPurchase = new FixedPurchaseService().GetContainer(fixedPurchaseId)
				?? new FixedPurchaseContainer(drvOrder);
			return fixedPurchase;
		}

		/// <summary>
		/// Get shipping time message
		/// </summary>
		/// <param name="shippingCompanyId">Shipping company id</param>
		/// <param name="shippingTimeId">Shipping time id</param>
		/// <returns>Time message</returns>
		private string GetShippingTimeMessage(string shippingCompanyId, string shippingTimeId)
		{
			var shippingCompany = new DeliveryCompanyService().Get(shippingCompanyId);
			var timeMessage = shippingCompany.GetShippingTimeMessage(shippingTimeId);
			return timeMessage;
		}

		/// <summary>
		/// Get user management level name
		/// </summary>
		/// <param name="userManagementLevelId">User management level id</param>
		/// <returns>User management level name</returns>
		private string GetUserManagementLevelName(string userManagementLevelId)
		{
			var managementLevelName =
				UserManagementLevelUtility.GetUserManagementLevelName(userManagementLevelId);
			return managementLevelName;
		}

		/// <summary>
		/// Get price
		/// </summary>
		/// <param name="fixedPuchaseId">Fixed purchase id</param>
		/// <returns>Price of order item</returns>
		private decimal GetPrice(string fixedPuchaseId)
		{
			var fixedPurchase = new FixedPurchaseService().GetContainer(fixedPuchaseId);
			var result = fixedPurchase.Shippings.Sum(shipping
				=> shipping.Items.Sum(item
					=> item.GetValidPrice() * item.ItemQuantity));
			return result;
		}

		/// <summary>
		/// 詳細表示用サイト名取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="mallName">モール名</param>
		/// <returns>サイト名（モール名＋モールID）</returns>
		public string CreateSiteNameForDetail(string mallId, string mallName)
		{
			return OrderCommon.CreateSiteNameForDetail(mallId, mallName);
		}

		/// <summary>Order id</summary>
		public string OrderId { get; set; }
		/// <summary>Mall id</summary>
		public string MallId { get; set; }
		/// <summary>Order kbn</summary>
		public string OrderKbn { get; set; }
		/// <summary>Payment kbn</summary>
		public string PaymentKbn { get; set; }
		/// <summary>Order date</summary>
		public string OrderDate { get; set; }
		/// <summary>Order status</summary>
		public string OrderStatus { get; set; }
		/// <summary>Price total</summary>
		public string PriceTotal { get; set; }
		/// <summary>Payment status</summary>
		public string PaymentStatus { get; set; }
		/// <summary>Owner id</summary>
		public string OwnerId { get; set; }
		/// <summary>Owner name</summary>
		public string OwnerName { get; set; }
		/// <summary>Owner tel</summary>
		public string OwnerTel { get; set; }
		/// <summary>Owner mail</summary>
		public string OwnerMail { get; set; }
		/// <summary>Owner kbn</summary>
		public string OwnerKbn { get; set; }
		/// <summary>Management level</summary>
		public string ManagementLevel { get; set; }
		/// <summary>Shipping address</summary>
		public string ShippingAddress { get; set; }
		/// <summary>Shipping name</summary>
		public string ShippingName { get; set; }
		/// <summary>Shipping tel</summary>
		public string ShippingTel { get; set; }
		/// <summary>Shipping date</summary>
		public string ShippingDate { get; set; }
		/// <summary>Shipping time</summary>
		public string ShippingTime { get; set; }
		/// <summary>Order memo</summary>
		public string OrderMemo { get; set; }
		/// <summary>Management memo</summary>
		public string ManagementMemo { get; set; }
		/// <summary>Shipping memo</summary>
		public string ShippingMemo { get; set; }
		/// <summary>Member rank id</summary>
		public string MemberRankId { get; set; }
		/// <summary>Order gift flg</summary>
		public string OrderGiftFlg { get; set; }
		/// <summary>Return exchange kbn</summary>
		public string ReturnExchangeKbn { get; set; }
		/// <summary>Return exchange receipt date</summary>
		public string ReturnExchangeReceiptDate { get; set; }
		/// <summary>Return exchange status</summary>
		public string ReturnExchangeStatus { get; set; }
		/// <summary>Repayment status</summary>
		public string RepaymentStatus { get; set; }
		/// <summary>Card tran id</summary>
		public string CardTranId { get; set; }
		/// <summary>Digital contents flag</summary>
		public string DigitalContentsFlg { get; set; }
		/// <summary>Shipped status</summary>
		public string ShippedStatus { get; set; }
		/// <summary>Stock reserved status</summary>
		public string StockreservedStatus { get; set; }
		/// <summary>出荷予定日</summary>
		public string ScheduledShippingDate { get; set; }
		/// <summary>定期購入回数(出荷時点)</summary>
		public string FixedPurchaseShippedCount { get; set; }
		///// <summary>ユーザー購入回数（注文基準）</summary>
		public string OrderCountOrder { get; set; }
		/// <summary>督促ステータス</summary>
		public string DemandStatus { get; set; }
		/// <summary>Fixed purchase kbn</summary>
		public string FixedPurchaseKbn { get; set; }
		/// <summary>Fixed purchase order count</summary>
		public string FixedPurchaseOrderCount { get; set; }
		/// <summary>Fixed purchase shipping count</summary>
		public string FixedPurchaseShippingCount { get; set; }
		/// <summary>Fixed purchase next shipping date</summary>
		public string FixedPurchaseNextShippingDate { get; set; }
		/// <summary>Fixed purchase next next shipping date</summary>
		public string FixedPurchaseNextNextShippingDate { get; set; }
		/// <summary>Fixed purchase date begin</summary>
		public string FixedPurchaseDateBgn { get; set; }
		/// <summary>Fixed purchase last order date</summary>
		public string FixedPurchaseLastOrderDate { get; set; }
		/// <summary>Fixed purchase id</summary>
		public string FixedPurchaseId { get; set; }
		/// <summary>Fixed purchase date</summary>
		public string FixedPurchaseDate { get; set; }
		/// <summary>Fixed purchase status</summary>
		public string FixedPurchaseStatus { get; set; }
		/// <summary>Fixed purchase setting</summary>
		public string FixedPurchaseSetting { get; set; }
		/// <summary>Order items</summary>
		public OrderItemResponse[] OrderItems { get; set; }
		/// <summary>Payment name</summary>
		public string PaymentName { get; set; }
		/// <summary>Order subscription box order count</summary>
		public string SubscriptionBoxOrderCount { get; set; }
		/// <summary>配送先の企業名 </summary>
		public string ShippingCompanyName { get; set; }
		/// <summary>配送先の部署名</summary>
		public string ShippingCompanyPostName { get; set; }
	}
}
