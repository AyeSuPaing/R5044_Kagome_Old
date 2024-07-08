/*
=========================================================================================================
  Module      : 注文共通ページ 表示制御部分(OrderPage_Display.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input.Order;
using w2.Domain.ShopShipping;
using w2.Domain.DeliveryCompany;

/// <summary>
/// OrderPage_Display の概要の説明です
/// </summary>
public partial class OrderPage : ProductPage
{
	/// <summary>
	/// 商品セットアイテムRowSpan取得（データバインド用）
	/// </summary>
	/// <param name="orderItem">対象受注商品</param>
	/// <param name="orderItems">全受注商品リスト</param>
	/// <returns>商品セットアイテムRowSpan</returns>
	public static int GetProductSetRowspan(OrderItemInput orderItem, OrderItemInput[] orderItems)
	{
		// 商品セットではない？
		if (orderItem.IsProductSet == false)
		{
			return 2;
		}

		return orderItems.Count(oi => ((orderItem.ProductSetId == oi.ProductSetId) && (orderItem.ProductSetNo == oi.ProductSetNo))) * 2;
	}

	/// <summary>
	/// 商品セットのアイテムTOPか否か取得（データバインド用）
	/// </summary>
	/// <param name="orderItem">対象受注商品</param>
	/// <param name="orderItems">全受注商品リスト</param>
	/// <returns>商品セットのアイテムTOPか否か</returns>
	public static bool IsProductSetItemTop(OrderItemInput orderItem, OrderItemInput[] orderItems)
	{
		// 商品セットではない？
		if (orderItem.IsProductSet == false)
		{
			return false;
		}

		OrderItemInput beforeOrderItem = null;
		foreach (var oi in orderItems)
		{
			if (orderItem == oi)
			{
				if (beforeOrderItem == null)
				{
					return true;
				}
				else if ((orderItem.ProductSetId != beforeOrderItem.ProductSetId)
					|| (orderItem.ProductSetNo != beforeOrderItem.ProductSetNo))
				{
					return true;
				}
				break;
			}

			beforeOrderItem = oi;
		}

		return false;
	}

	/// <summary>
	/// 商品セット価格小計取得（セット価格ｘ個数 データバインド用）
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <param name="orderItems">注文商品リスト</param>
	/// <returns>商品セット価格小計</returns>
	public static decimal CreateSetPriceSubtotal(OrderItemInput orderItem, OrderItemInput[] orderItems)
	{
		decimal setPriceSubtotal = 0;
		foreach (var oi in orderItems)
		{
			// 商品セット？
			if (orderItem.IsProductSet)
			{
				if ((orderItem.ProductSetId == oi.ProductSetId)
					&& (orderItem.ProductSetNo == oi.ProductSetNo))
				{
					setPriceSubtotal += decimal.Parse(oi.ProductPrice) * int.Parse(oi.ItemQuantity);
				}
			}
		}

		return setPriceSubtotal;
	}

	/// <summary>
	/// 商品セット編集可否
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <param name="orderItems">注文商品リスト</param>
	/// <returns>商品セット編集可？</returns>
	protected bool IsSetEditable(OrderItemInput orderItem, OrderItemInput[] orderItems)
	{
		foreach (var oi in orderItems)
		{
			if (oi.ProductSetId == orderItem.ProductSetId)
			{
				if (oi.IsRealStockReserved)
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// 引当情報表示
	/// </summary>
	/// <param name="oiItem">注文商品情報</param>
	/// <returns>引当情報</returns>
	public static string GetRealStockReservedDisplayString(OrderItemInput orderItem)
	{
		return GetRealStockReservedDisplayString(
			int.Parse(orderItem.ItemQuantity),
			int.Parse(orderItem.ItemRealstockReserved));
	}
	/// <summary>
	/// 引当情報表示
	/// </summary>
	/// <param name="drvOrderItem">注文商品情報</param>
	/// <returns>引当情報</returns>
	public static string GetRealStockReservedDisplayString(DataRowView drvOrderItem)
	{
		return GetRealStockReservedDisplayString(
			(int)drvOrderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
			(int)drvOrderItem[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED]);
	}
	/// <summary>
	/// 引当情報表示
	/// </summary>
	/// <param name="iItemQuantity">商品数</param>
	/// <param name="iItemRealStockReserved">引当済商品数</param>
	/// <returns>引当情報</returns>
	public static string GetRealStockReservedDisplayString(
		int iItemQuantity,
		int iItemRealStockReserved)
	{
		StringBuilder sbDisplayString = new StringBuilder();
		if (iItemQuantity < 0)
		{
			sbDisplayString.Append("-");
		}
		else
		{
			sbDisplayString.Append("[");
			sbDisplayString.Append(WebSanitizer.HtmlEncode(StringUtility.ToNumeric(iItemRealStockReserved)));
			sbDisplayString.Append("/");
			sbDisplayString.Append(WebSanitizer.HtmlEncode(StringUtility.ToNumeric(iItemQuantity)));
			sbDisplayString.Append("]");
		}

		return sbDisplayString.ToString();
	}

	/// <summary>
	/// マイナス場合、赤文字表示
	/// </summary>
	/// <param name="src">値</param>
	/// <param name="priceFlg">金額フラグ</param>
	/// <returns>HTML要素</returns>
	public static string GetMinusNumberNoticeHtml(object src, bool priceFlg)
	{
		decimal value;
		if (decimal.TryParse(src.ToString(), out value))
		{
			if (value < 0)
			{
				var html = (priceFlg) ? "-" + (value * -1).ToPriceString(true) : StringUtility.ToNumeric(value);
				return GetNoticeHtml(html);
			}

			var result = (priceFlg) ? value.ToPriceString(true) : StringUtility.ToNumeric(value);
			return WebSanitizer.HtmlEncode(result);
		}

		return WebSanitizer.HtmlEncode(src.ToString());
	}

	/// <summary>
	/// 赤文字HTML取得
	/// </summary>
	/// <param name="html">表示対象</param>
	/// <returns>HTML要素</returns>
	private static string GetNoticeHtml(string html)
	{
		return "<span class=\"notice\">" + WebSanitizer.HtmlEncode(html) + "</span>";
	}

	/// <summary>
	/// 金額表示用カラー取得
	/// </summary>
	/// <param name="objPrice">金額</param>
	/// <returns>金額表示用カラー</returns>
	/// <remarks>金額が0以下の場合はColor.Red、それ以外はColor.Emptyを返します</remarks>
	public static System.Drawing.Color GetPriceDisplayColor(object objPrice)
	{
		if (objPrice is decimal)
		{
			if ((decimal)objPrice < 0)
			{
				return System.Drawing.Color.Red;
			}
		}

		return System.Drawing.Color.Empty;
	}

	/// <summary>
	/// 配送希望時間帯ドロップダウンリスト作成
	/// </summary>
	/// <param name="deliveryCompany">配送会社情報</param>
	/// <returns>配送希望時間帯ドロップダウンリスト</returns>
	protected ListItemCollection GetShippingTimeList(DeliveryCompanyModel deliveryCompany)
	{
		var result = new ListItemCollection();

		// 配送希望時間帯設定可能フラグが有効?
		if (deliveryCompany.IsValidShippingTimeSetFlg)
		{
			result.AddRange(deliveryCompany.GetShippingTimeList().Select(kvp => new ListItem(kvp.Value, kvp.Key)).ToArray());
		}

		return result;
	}

	/// <summary>
	/// 配送会社名取得
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>配送会社名</returns>
	public string GetDeliveryCompanyName(string deliveryCompanyId)
	{
		var deliveryCompany = this.DeliveryCompanyList.FirstOrDefault(i => i.DeliveryCompanyId == deliveryCompanyId);
		return deliveryCompany != null ? deliveryCompany.DeliveryCompanyName : "";
	}

	/// <summary>配送会社リスト</summary>
	public DeliveryCompanyModel[] DeliveryCompanyList
	{
		get
		{
			if (m_DeliveryComapnyList == null)
			{
				var service = new DeliveryCompanyService();
				this.m_DeliveryComapnyList = service.GetAll();
			}
			return m_DeliveryComapnyList;
		}
	}
	private DeliveryCompanyModel[] m_DeliveryComapnyList;
}
