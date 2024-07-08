<%--
=========================================================================================================
  Module      : Lastest Order Get (LastestOrderGet.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="LastestOrderGet" %>

using System.Linq;
using System.Web;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;

/// <summary>
/// 最新受注情報取得API
/// </summary>
public class LastestOrderGet : LineBasePage, IHttpHandler
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">Context</param>
	public override void ProcessRequest (HttpContext context) {
		GetRequest(context);
		WriteResponse(context, (response) =>
		{
			var lastestOrderResponse = response as LineLastestOrderGetResponse;
			if (lastestOrderResponse == null) return false;
			
			if (string.Equals(lastestOrderResponse.Status, STATUS_CODE_DATA_NOT_EXIST))
			{
				Write300ErrorResponse(null);
				return false;
			}
			return true;
		});
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
		var isValid = ((string.IsNullOrEmpty(this.UserId) == false)
			&& ((this.IdType == Constants.ID_TYPE_EC_USER_ID)
				|| (this.IdType == Constants.ID_TYPE_LINE_USER_ID)));
		return isValid;
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

	/// <summary>
	/// Get order data
	/// </summary>
	/// <returns>An order model</returns>
	private OrderModel GetOrderData()
	{
		var userId = string.Empty;
		switch (this.IdType)
		{
			case Constants.ID_TYPE_LINE_USER_ID:
				var userModel = new UserService().GetByExtendColumn(
					Constants.SOCIAL_PROVIDER_ID_LINE,
					this.UserId);
				userId = (userModel == null)
					? string.Empty
					: userModel.UserId;
				break;

			case Constants.ID_TYPE_EC_USER_ID:
				userId = this.UserId;
				break;
		}
		return new OrderService().GetLatestOrder(userId);
	}

	/// <summary>
	/// Get response data
	/// </summary>
	/// <returns>A response object</returns>
	protected override object GetResponseData()
	{
		var orderModel = GetOrderData();
		if (orderModel == null) return null;

		var detailProduct = orderModel.Items
			.Select(item => new DetailProduct
			{
				OrderItemNo = item.OrderItemNo,
				ProductId = item.ProductId,
				VariationId = item.VariationId,
				ProductName = item.ProductName,
				ProductPrice = item.ProductPrice,
				ProductPricePretax = item.ProductPricePretax,
				ItemPriceTax = item.ItemPriceTax,
				ItemQuantity = item.ItemQuantity,
				ItemPrice = item.ItemPrice,
				FixedPurchaseProductFlg = item.FixedPurchaseProductFlg,
			});

		// 配送情報配列
		var detailShipping = orderModel.Shippings
			.Select(item => new DetailShipping
			{
				OrderId = item.OrderId,
				OrderShippingNo = item.OrderShippingNo,
				ShippingMethod = item.ShippingMethod,
				DeliveryCompanyId = item.DeliveryCompanyId,
			});

		orderModel.OrderId = (orderModel.OrderId.Length < 13)
			? orderModel.OrderId.PadLeft(13, '0')
			: orderModel.OrderId;

		var response = new LineLastestOrderGetResponse
		{
			OrderId = orderModel.OrderId,
			UserId = orderModel.UserId,
			OrderKbn = orderModel.OrderKbn,
			OrderPaymentKbn = orderModel.OrderPaymentKbn,
			OrderStatus = orderModel.OrderStatus,
			OrderDate = StringUtility.ToEmpty(orderModel.OrderDate),
			OrderShippedDate = StringUtility.ToEmpty(orderModel.OrderShippedDate),
			OrderPaymentStatus = orderModel.OrderPaymentStatus,
			OrderItemCount = orderModel.OrderItemCount,
			OrderProductCount = orderModel.OrderProductCount,
			OrderPriceSubtotal = orderModel.OrderPriceSubtotal,
			OrderPriceTax = orderModel.OrderPriceTax,
			OrderPriceShipping = orderModel.OrderPriceShipping,
			OrderPriceExchange = orderModel.OrderPriceExchange,
			OrderPriceRegulation = orderModel.OrderPriceRegulation,
			OrderCouponUse = orderModel.OrderCouponUse,
			OrderPointUse = orderModel.OrderPointUse,
			OrderPointUseYen = orderModel.OrderPointUseYen,
			OrderDiscountSetPrice = orderModel.OrderDiscountSetPrice,
			MemberRankDiscountPrice = orderModel.MemberRankDiscountPrice,
			OrderPriceTotal = orderModel.OrderPriceTotal,
			OrderPointAdd = orderModel.OrderPointAdd,
			FixedPurchaseId = orderModel.FixedPurchaseId,
			MemberRankId = orderModel.MemberRankId,
			SetpromotionProductDiscountAmount = orderModel.SetpromotionProductDiscountAmount,
			SetpromotionShippingChargeDiscountAmount = orderModel.SetpromotionShippingChargeDiscountAmount,
			SetpromotionPaymentChargeDiscountAmount = orderModel.SetpromotionPaymentChargeDiscountAmount,
			FixedPurchaseOrderCount = (orderModel.FixedPurchaseOrderCount != null)
				? orderModel.FixedPurchaseOrderCount.Value
				: 0,
			FixedPurchaseShippedCount = (orderModel.FixedPurchaseShippedCount != null)
				? orderModel.FixedPurchaseShippedCount.Value
				: 0,
			FixedPurchaseDiscountPrice = orderModel.FixedPurchaseDiscountPrice,
			FixedPurchaseMemberDiscountAmount = orderModel.FixedPurchaseMemberDiscountAmount,
			OrderPriceSubtotalTax = orderModel.OrderPriceSubtotalTax,
			OrderPriceShippingTax = orderModel.OrderPriceShippingTax,
			OrderPriceExchangeTax = orderModel.OrderPriceExchangeTax,
			ScheduledShippingDate = StringUtility.ToEmpty(orderModel.Shippings[0].ScheduledShippingDate),
			ShippingDate = StringUtility.ToEmpty(orderModel.Shippings[0].ShippingDate),
			ShippingTime = orderModel.Shippings[0].ShippingTime,
			ShippingCheckNo = orderModel.Shippings[0].ShippingCheckNo,
			DateCreated = StringUtility.ToEmpty(orderModel.DateCreated),
			DateChanged = StringUtility.ToEmpty(orderModel.DateChanged),
			Status = STATUS_CODE_SUCCESS,
			DetailProduct = detailProduct.ToArray(),
			DetailShipping = detailShipping.ToArray(),
		};
		return response;
	}

	/// <summary>Request key: ID type</summary>
	public string IdType { get; set; }
	/// <summary>Request key: User ID</summary>
	public string UserId { get; set; }
	/// <summary>IsReusable</summary>
	public bool IsReusable { get { return false; } }
}