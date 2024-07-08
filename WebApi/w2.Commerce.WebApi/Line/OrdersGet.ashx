<%--
=========================================================================================================
  Module      : 受注情報一覧取得API (OrdersGet.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="OrdersGet" %>

using System;
using System.Linq;
using System.Web;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;

/// <summary>
/// 受注情報一覧取得API
/// </summary>
public class OrdersGet : LineBasePage, IHttpHandler {

	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">Context</param>
	public override void ProcessRequest (HttpContext context)
	{
		GetRequest(context);
		WriteResponse(context);
	}

	/// <summary>
	/// リクエスト取得
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <returns>リクエスト文字列</returns>
	protected override void GetRequest(HttpContext context)
	{
		this.IdType = (string)context.Request[Constants.REQUEST_KEY_ID_TYPE];
		this.UserId = (string)context.Request[Constants.REQUEST_KEY_USER_ID];
		this.FixedPurchaseId = (string)context.Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID];
		this.Limit = (string)context.Request[Constants.REQUEST_KEY_LIMIT];
		this.Offset = (string)context.Request[Constants.REQUEST_KEY_OFFSET];
		this.UpdateAtString = (string)context.Request[Constants.REQUEST_KEY_UPDATE_AT];
	}

	/// <summary>
	/// ソーシャルプラスオプションチェック抽象メソッド
	/// </summary>
	/// <returns>チェック結果</returns>
	protected override bool IsErrorSocialProviderIdLine()
	{
		var result = ((this.IdType == Constants.ID_TYPE_LINE_USER_ID)
			&& string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE));
		return result;
	}

	/// <summary>
	/// Is valid authorization
	/// </summary>
	/// <returns>True if parameter is valid, otherwise false</returns>
	protected override bool IsValidParameters()
	{
		DateTime updateAt;
		if (IsDateTimeFormatISO8601(this.UpdateAtString, out updateAt) == false)
		{
			return false;
		}
		this.UpdateAt = updateAt;

		var isValid =
			(((string.IsNullOrEmpty(this.UserId) == false)
					&& ((this.IdType == Constants.ID_TYPE_EC_USER_ID) || (this.IdType == Constants.ID_TYPE_LINE_USER_ID)))
				|| (string.IsNullOrEmpty(this.FixedPurchaseId) == false))
			&& Validator.IsHalfwidthNumber(this.Limit)
			&& Validator.IsHalfwidthNumber(this.Offset);
		return isValid;
	}

	/// <summary>
	/// Get response data
	/// </summary>
	/// <returns>A response object</returns>
	protected override object GetResponseData()
	{
		// Get orders
		var orders = GetOrdersData();
		var responseData = orders
			.Select(item => ConvertOrderData(item))
			.ToArray();
		var response = new LineOrdersGetResponse
		{
			Offset = int.Parse(this.Offset),
			Limit = int.Parse(this.Limit),
			Orders = responseData.ToArray(),
			Status = STATUS_CODE_SUCCESS,
		};
		return response;
	}

	/// <summary>
	/// Get orders data
	/// </summary>
	/// <returns>Order models</returns>
	private OrderModel[] GetOrdersData()
	{
		// Fill user id
		var userId = string.Empty;
		switch (this.IdType)
		{
			case Constants.ID_TYPE_LINE_USER_ID:
				var userModel = new UserService().GetByExtendColumn(
					Constants.SOCIAL_PROVIDER_ID_LINE,
					this.UserId);
				userId = (userModel != null)
					? userModel.UserId
					: string.Empty;
				break;

			case Constants.ID_TYPE_EC_USER_ID:
				userId = this.UserId;
				break;
		}
		// Get data order
		var data = new OrderService().GetOrdersForLine(
			userId,
			this.FixedPurchaseId,
			int.Parse(this.Offset),
			int.Parse(this.Limit),
			this.UpdateAt);
		return data;
	}

	/// <summary>
	/// Convert order data
	/// </summary>
	/// <param name="order">Order model</param>
	/// <returns>Order data for response</returns>
	private LineLastestOrderGetResponse ConvertOrderData(OrderModel order)
	{
		// Set item detail
		var detailProduct = order.Items
			.Select(item => new DetailProduct
			{
				OrderItemNo = item.OrderItemNo,
				OrderShippingNo = item.OrderShippingNo,
				ProductId = item.ProductId,
				VariationId = item.VariationId,
				ProductName = item.ProductName,
				ProductPrice = item.ProductPrice,
				ProductPricePretax = item.ProductPricePretax,
				ItemPriceTax = item.ItemPriceTax,
				ItemQuantity = item.ItemQuantity,
				ItemPrice = item.ItemPrice,
				FixedPurchaseProductFlg = item.FixedPurchaseProductFlg,
			}).ToArray();
		order.OrderId = (order.OrderId.Length < 13)
			? order.OrderId.PadRight(13, '0')
			: order.OrderId;

		// 配送情報配列
		var detailShipping = order.Shippings
			.Select(item => new DetailShipping
			{
				OrderId = item.OrderId,
				OrderShippingNo = item.OrderShippingNo,
				ShippingMethod = item.ShippingMethod,
				DeliveryCompanyId = item.DeliveryCompanyId,
			}).ToArray();

		// Set order
		var orderData = new LineLastestOrderGetResponse
		{
			OrderId = order.OrderId,
			UserId = order.UserId,
			OrderKbn = order.OrderKbn,
			OrderPaymentKbn = order.OrderPaymentKbn,
			OrderStatus = order.OrderStatus,
			OrderDate = StringUtility.ToEmpty(order.OrderDate),
			OrderShippedDate = StringUtility.ToEmpty(order.OrderShippedDate),
			OrderPaymentStatus = order.OrderPaymentStatus,
			OrderItemCount = order.OrderItemCount,
			OrderProductCount = order.OrderProductCount,
			OrderPriceSubtotal = order.OrderPriceSubtotal,
			OrderPriceTax = order.OrderPriceTax,
			OrderPriceShipping = order.OrderPriceShipping,
			OrderPriceExchange = order.OrderPriceExchange,
			OrderPriceRegulation = order.OrderPriceRegulation,
			OrderCouponUse = order.OrderCouponUse,
			OrderPointUse = order.OrderPointUse,
			OrderPointUseYen = order.OrderPointUseYen,
			OrderDiscountSetPrice = order.OrderDiscountSetPrice,
			MemberRankDiscountPrice = order.MemberRankDiscountPrice,
			OrderPriceTotal = order.OrderPriceTotal,
			OrderPointAdd = order.OrderPointAdd,
			FixedPurchaseId = order.FixedPurchaseId,
			MemberRankId = order.MemberRankId,
			SetpromotionProductDiscountAmount = order.SetpromotionProductDiscountAmount,
			SetpromotionShippingChargeDiscountAmount = order.SetpromotionShippingChargeDiscountAmount,
			SetpromotionPaymentChargeDiscountAmount = order.SetpromotionPaymentChargeDiscountAmount,
			FixedPurchaseOrderCount = (order.FixedPurchaseOrderCount != null)
				? order.FixedPurchaseOrderCount.Value
				: 0,
			FixedPurchaseShippedCount = (order.FixedPurchaseShippedCount != null)
				? order.FixedPurchaseShippedCount.Value
				: 0,
			FixedPurchaseDiscountPrice = order.FixedPurchaseDiscountPrice,
			FixedPurchaseMemberDiscountAmount = order.FixedPurchaseMemberDiscountAmount,
			OrderPriceSubtotalTax = order.OrderPriceSubtotalTax,
			OrderPriceShippingTax = order.OrderPriceShippingTax,
			OrderPriceExchangeTax = order.OrderPriceExchangeTax,
			ScheduledShippingDate = StringUtility.ToEmpty(order.Shippings[0].ScheduledShippingDate),
			ShippingDate = StringUtility.ToEmpty(order.Shippings[0].ShippingDate),
			ShippingTime = order.Shippings[0].ShippingTime,
			ShippingCheckNo = order.Shippings[0].ShippingCheckNo,
			DateCreated = StringUtility.ToEmpty(order.DateCreated),
			DateChanged = StringUtility.ToEmpty(order.DateChanged),
			DetailProduct = detailProduct,
			DetailShipping = detailShipping,
		};
		return orderData;
	}

	/// <summary>
	/// Get error response
	/// </summary>
	/// <param name="parameters">A parameters</param>
	/// <returns>An error response object</returns>
	protected override object GetErrorResponse(params object[] parameters)
	{
		var errorResponse = new LineErrorResponse()
		{
			Status = (int)parameters[0],
			Reason = StringUtility.ToEmpty(parameters[1])
		};
		return errorResponse;
	}

	/// <summary>Request Key: Id Type</summary>
	public string IdType { get; set; }
	/// <summary>Request Key: User Id</summary>
	public string UserId { get; set; }
	/// <summary>定期台帳ID</summary>
	public string FixedPurchaseId { get; set; }
	/// <summary>Request Key: Limit</summary>
	public string Limit { get; set; }
	/// <summary>Request Key: Offset</summary>
	public string Offset { get; set; }
	/// <summary>指定時刻_文字列</summary>
	public string UpdateAtString { set; get; }
	/// <summary>Request Key: Update At</summary>
	public DateTime UpdateAt { set; private get; }
	/// <summary>IsReusable</summary>
	public bool IsReusable { get { return false; } }
}