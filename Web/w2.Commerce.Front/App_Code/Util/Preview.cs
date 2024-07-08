/*
=========================================================================================================
  Module      : プレビュークラス(Preview.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.User;

/// <summary>
/// プレビュークラス
/// </summary>
public class Preview
{
	/// <summary>プレビューページ拡張子</summary>
	public const string PREVIEW_PAGE_EXTENSION = ".Preview.aspx";
	/// <summary>プレビューパーツ拡張子</summary>
	public const string PREVIEW_PARTS_EXTENSION = ".Preview.ascx";

	/// <summary>
	/// プレビューか?
	/// </summary>
	/// <param name="request">Httpリクエスト</param>
	/// <returns>結果</returns>
	public static bool IsPreview(HttpRequest request)
	{
		var result = request.Url.AbsolutePath.Contains(PREVIEW_PAGE_EXTENSION);
		return result;
	}

	/// <summary>
	/// キャッシュ非保持をレスポンスに設定
	/// </summary>
	/// <param name="response"></param>
	public static void SetNonCache(HttpResponse response)
	{
		response.AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate");
		response.AddHeader("Pragma", "no-cache");
		response.Cache.SetAllowResponseInBrowserHistory(false);
	}

	/// <summary>
	/// ページに対してアクションの無効化
	/// </summary>
	/// <param name="page">ページ</param>
	public static void PageInvalidateAction(Page page)
	{
		// クリック無効化
		ScriptManager.RegisterStartupScript(page, page.GetType(), "InvalidateAction", "InvalidateAction();", true);
	}

	/// <summary>
	/// プレビュー用ダミーユーザモデルの取得
	/// </summary>
	/// <returns>ユーザモデル</returns>
	public static UserModel GetDummyUserModel()
	{
		var userModel = new UserModel()
		{
			UserId = "preview",
			UserKbn = Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER,
			Name = "山田 太郎",
			Name1 = "山田",
			Name2 = "太郎",
			NameKana = "やまだ たろう",
			NameKana1 = "やまだ",
			NameKana2 = "たろう",
			MailAddr = "mail1@example.com",
			MailAddr2 = "mail2@example.com",
			Zip = "123-4567",
			Zip1 = "123",
			Zip2 = "4567",
			Addr = "東京都 市区町村 番地 ビル名号室",
			Addr1 = "東京都",
			Addr2 = "市区町村",
			Addr3 = "番地",
			Addr4 = "ビル名号室",
			Addr5 = "",
			AddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE ? Constants.COUNTRY_ISO_CODE_JP : "",
			AddrCountryName = Constants.GLOBAL_OPTION_ENABLE ? "Japan" : "",
			CompanyName = "Example株式会社",
			CompanyPostName = "Example",
			Tel1 = "123-4567-8999",
			Tel1_1 = "123",
			Tel1_2 = "4567",
			Tel1_3 = "8999",
			Tel2 = "123-4567-8999",
			Tel2_1 = "123",
			Tel2_2 = "4567",
			Tel2_3 = "8999",
			MailFlg = Constants.FLG_USER_MAILFLG_OK,
			Sex = "MALE",
			Birth = new DateTime(1977, 11, 19)
		};

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			userModel.UserExtend = new UserExtendInput().CreateModel();
			userModel.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_NO);
			userModel.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_NO] = "12345678";
		}

		return userModel;
	}

	/// <summary>
	/// プレビュー用ダミーカートリストの取得
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <returns>ダミーカートリスト</returns>
	public static CartObjectList GetDummyCart(string shopId)
	{
		// ダミー商品取得
		var cartProduct = GetDummyCartProduct(shopId);
		CartObject cartObject;
		if (cartProduct != null)
		{
			cartObject = new CartObject(null, "", shopId, cartProduct.ShippingType, false, false, "");
			cartObject.AddVirtural(cartProduct, false);
		}
		else
		{
			cartObject = new CartObject(null, "", shopId, "", false, false, "");
		}

		cartObject.OrderMemos = new List<CartOrderMemo>();

		cartObject.OrderExtend = OrderExtendCommon.CreateOrderExtend();

		// 支払い方法は決済なし
		var noPayment = DataCacheControllerFacade.GetPaymentCacheController().Get(Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
		var noPaymentMethod = new CartPayment();
		noPaymentMethod.UpdateCartPayment(
			noPayment.PaymentId,
			noPayment.PaymentName,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			null,
			false,
			string.Empty);

		cartObject.Payment = noPaymentMethod;
		cartObject.SettlementCurrency = CurrencyManager.GetSettlementCurrency(noPaymentMethod.PaymentId);
		cartObject.SettlementRate = CurrencyManager.GetSettlementRate(cartObject.SettlementCurrency);
		cartObject.SettlementAmount = CurrencyManager.GetSettlementAmount(
			cartObject.PriceTotal,
			cartObject.SettlementRate,
			cartObject.SettlementCurrency);

		var ownerTmp = new CartOwner(GetDummyUserModel());
		cartObject.Shippings.First().UpdateShippingAddr(ownerTmp, true);
		cartObject.Shippings.First().ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

		var cartList = new CartObjectList(null, "", false);
		cartList.AddCartVirtural(cartObject);
		cartList.SetOwner(ownerTmp);
		foreach (var cart in cartList.Items)
		{
			cart.Shippings[0] = new CartShipping(cart);
			cart.Shippings[0].UpdateShippingAddr(cartList.Owner, true);
			cart.Shippings[0].ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
		}
		cartList.CalculateAllCart();

		return cartList;
	}

	/// <summary>
	/// プレビュー用ダミーカート商品データの取得
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <returns>ダミーカート商品</returns>
	public static CartProduct GetDummyCartProduct(string shopId)
	{
		var pm = new ProductService().GetProductTopForPreview(shopId);
		if (pm == null) return null;

		var product = ProductCommon.GetProductInfo(shopId, pm.ProductId, "", "");
		var cartProduct = new CartProduct(product[0], Constants.AddCartKbn.Normal, "", 1, false);

		var pos = new ProductOptionSettingList();
		pos.SetProductOptionSettingAll(pm.ProductOptionSettings);
		cartProduct.ProductOptionSettingList = pos;

		return cartProduct;
	}

	/// <summary>
	/// プレビュー用ダミー注文データの取得
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <returns>ダミー注文データ</returns>
	public static DataView GetDummyOrder(string shopId)
	{
		var dummyOrderId = "dummyOrder";
		var orderModel = new OrderModel();
		var orderOwnerModel = new OrderOwnerModel();
		var orderShippingModel = new OrderShippingModel();
		var shopShippingModel = new ShopShippingModel();
		var deliveryCompanyModel = new DeliveryCompanyModel();
		var orderItem = new OrderItemModel
		{
			OrderId = dummyOrderId,
			OrderItemNo = 0,
			ShopId = shopId,
		};

		var dummyCartList = GetDummyCart(shopId);
		if (dummyCartList.Items.Count > 0)
		{
			var dummyCart = dummyCartList.Items.First();
			orderModel = dummyCart.CreateNewOrder();
			orderOwnerModel = orderModel.Owner;
			orderShippingModel = orderModel.Shippings[0];
			if (dummyCart.Items.Count > 0)
			{
				var dummyCartProduct = dummyCart.Items[0];
				orderItem = dummyCartProduct.CreateOrderItem(orderModel, 1, 1, 0, 0, null, null);
			}
		}

		orderModel.OrderId = dummyOrderId;
		orderOwnerModel.OrderId = dummyOrderId;
		orderShippingModel.OrderId = dummyOrderId;

		var dataSet = new DataSet();
		var dataTable = new DataTable();
		var row = dataTable.NewRow();
		SetOrderData(orderModel.DataSource, dataTable, row);
		SetOrderData(orderOwnerModel.DataSource, dataTable, row);
		SetOrderData(orderShippingModel.DataSource, dataTable, row);
		SetOrderData(shopShippingModel.DataSource, dataTable, row);
		SetOrderData(deliveryCompanyModel.DataSource, dataTable, row);
		SetOrderData(orderItem.DataSource, dataTable, row);

		foreach (var addInfo in new []
		{
			new Tuple<string, Type, object>("order_payment_kbn_name", typeof(string), ""),
			new Tuple<string, Type, object>("shipping_time_message", typeof(string), ""),
			new Tuple<string, Type, object>("newest_product_count", typeof(int), 1),
			new Tuple<string, Type, object>(Constants.FIELD_SERIALKEY_SERIAL_KEY, typeof(string), ""),
			new Tuple<string, Type, object>(Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE, typeof(string), ""),
			new Tuple<string, Type, object>(Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO, typeof(string), ""),
			new Tuple<string, Type, object>(Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, typeof(string), Constants.FLG_TW_UNIFORM_INVOICE_COMPANY),
			new Tuple<string, Type, object>(Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1, typeof(string), ""),
			new Tuple<string, Type, object>(Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2, typeof(string), ""),
			new Tuple<string, Type, object>(Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, typeof(string), ""),
			new Tuple<string, Type, object>(Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION, typeof(string), ""),
		})
		{
			dataTable.Columns.Add(addInfo.Item1, addInfo.Item2);
			row[addInfo.Item1] = addInfo.Item3;
		}
		dataTable.Rows.Add(row);

		dataSet.Tables.Add(dataTable);
		dataTable.TableName = "DummyOrder";

		var dataView = new DataView(dataTable);
		return dataView;
	}

	/// <summary>
	/// 注文モデルから内容をセット
	/// </summary>
	/// <param name="tableData">内容</param>
	/// <param name="dataTable">テーブル</param>
	/// <param name="dataRow">レコード</param>
	private static void SetOrderData(Hashtable tableData, DataTable dataTable, DataRow dataRow)
	{
		foreach (DictionaryEntry column in tableData)
		{
			if (dataTable.Columns.Contains(column.Key.ToString())) continue;

			dataTable.Columns.Add(
				column.Key.ToString(),
				(column.Value != null) ? column.Value.GetType() : typeof(string));
			dataRow[column.Key.ToString()] = column.Value;
		}
	}
}