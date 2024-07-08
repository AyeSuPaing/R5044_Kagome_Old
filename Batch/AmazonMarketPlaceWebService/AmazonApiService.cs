/*
=========================================================================================================
  Module      : AmazonAPIサービスクラス(AmazonApiService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using AmazonMarketPlaceWebService.XmlSchema;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using w2.Domain.MallCooperationSetting;
using w2.Domain.Order;
using w2.Domain.Product;

namespace AmazonMarketPlaceWebService
{
	/// <summary>
	/// AmazonAPIサービスクラス
	/// </summary>
	public class AmazonApiService
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setting">モール連携設定</param>
		public AmazonApiService(MallCooperationSettingModel setting)
		{
			this.AmazonMallCooperationSetting = setting;
			var ordersConfig = new MarketplaceWebServiceOrdersConfig
			{
				ServiceURL = Constants.AMAZON_MALL_MWS_ENDPOINT
			};

			this.OrdersClient = new MarketplaceWebServiceOrdersClient(
				setting.AmazonAwsAccesskeyId,
				setting.AmazonSecretKey,
				Constants.APPLICATION_NAME,
				Constants.APPLICATION_VERSION,
				ordersConfig);

			var feedConfig = new MarketplaceWebServiceConfig
			{
				ServiceURL = Constants.AMAZON_MALL_MWS_ENDPOINT,
			};
			this.FeedClient = new MarketplaceWebServiceClient(
				setting.AmazonAwsAccesskeyId,
				setting.AmazonSecretKey,
				Constants.APPLICATION_NAME,
				Constants.APPLICATION_VERSION,
				feedConfig);
		}
		#endregion

		#region +InvokeListOrders ListOrders実行
		/// <summary>
		/// ListOrders実行
		/// </summary>
		/// <param name="latestExecuteDatetime">前回実行時間</param>
		/// <returns>レスポンス</returns>
		public ListOrdersResponse InvokeListOrders(DateTime? latestExecuteDatetime)
		{
			// 初回実行／差分実行でパラメータを変更
			DateTime lastUpdatedAfter;
			List<string> orderStatus;
			if (latestExecuteDatetime != null)
			{
				lastUpdatedAfter = (DateTime)latestExecuteDatetime;
				orderStatus = new List<string>
				{
					Constants.AMAZON_MALL_ORDER_STATUS_PENDING,
					Constants.AMAZON_MALL_ORDER_STATUS_CANCELED,
					Constants.AMAZON_MALL_ORDER_STATUS_UNSHIPPED,
					Constants.AMAZON_MALL_ORDER_STATUS_PARTIALLY_SHIPPED
				};
			}
			else
			{
				lastUpdatedAfter = (DateTime.Today).AddDays(Constants.GET_AMAZON_ORDERS_UPDATE_BEFORE_DAYS * -1);
				orderStatus = new List<string>
				{
					Constants.AMAZON_MALL_ORDER_STATUS_PENDING,
					Constants.AMAZON_MALL_ORDER_STATUS_UNSHIPPED,
					Constants.AMAZON_MALL_ORDER_STATUS_PARTIALLY_SHIPPED
				};
			}

			// 実行
			var request = new ListOrdersRequest
			{
				SellerId = this.AmazonMallCooperationSetting.AmazonMerchantId,
				MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
				LastUpdatedAfter = lastUpdatedAfter,
				OrderStatus = orderStatus,
				MarketplaceId = new List<string> { this.AmazonMallCooperationSetting.AmazonMarketplaceId },
				FulfillmentChannel = new List<string> { Constants.AMAZON_MALL_FULFILMENT_CHANNEL_MFN }
			};
			var response = this.OrdersClient.ListOrders(request);
			return response;
		}
		#endregion

		#region +InvokeListOrdersByNextToken ListOrdersByNextToken実行
		/// <summary>
		/// ListOrdersByNextToken実行
		/// </summary>
		/// <param name="nextToken">NextToken</param>
		/// <returns>レスポンス</returns>
		public ListOrdersByNextTokenResponse InvokeListOrdersByNextToken(string nextToken)
		{
			var request = new ListOrdersByNextTokenRequest
			{
				SellerId = this.AmazonMallCooperationSetting.AmazonMerchantId,
				MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
				NextToken = nextToken
			};
			var response = this.OrdersClient.ListOrdersByNextToken(request);
			return response;
		}
		#endregion

		#region +InvokeListOrderItems ListOrderItems実行
		/// <summary>
		/// ListOrderItems実行
		/// </summary>
		/// <param name="amazonOrderId">Amazon注文番号</param>
		/// <returns>レスポンス</returns>
		public ListOrderItemsResponse InvokeListOrderItems(string amazonOrderId)
		{
			var request = new ListOrderItemsRequest
			{
				SellerId = this.AmazonMallCooperationSetting.AmazonMerchantId,
				MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
				AmazonOrderId = amazonOrderId
			};
			var response = this.OrdersClient.ListOrderItems(request);
			return response;
		}
		#endregion

		#region +InvokeListOrderItemsByNextToken ListOrderItemsByNextToken実行
		/// <summary>
		/// ListOrderItemsByNextToken実行
		/// </summary>
		/// <param name="nextToken">NextToken</param>
		/// <returns>レスポンス</returns>
		public ListOrderItemsByNextTokenResponse InvokeListOrderItemsByNextToken(string nextToken)
		{
			var request = new ListOrderItemsByNextTokenRequest
			{
				SellerId = this.AmazonMallCooperationSetting.AmazonMerchantId,
				MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
				NextToken = nextToken
			};
			var response = this.OrdersClient.ListOrderItemsByNextToken(request);
			return response;
		}
		#endregion

		#region +InvokeSubmitFeedForOrder SubmitFeed実行(注文情報)
		/// <summary>
		/// SubmitFeed実行(注文情報)
		/// </summary>
		/// <param name="feedType">フィードタイプ</param>
		/// <param name="orders">注文情報</param>
		/// <returns>レスポンス</returns>
		public SubmitFeedResponse InvokeSubmitFeedForOrder(string feedType, OrderModel[] orders)
		{
			using (var stream = new MemoryStream())
			using (var writer = new StreamWriter(stream, Encoding.UTF8))
			{
				ConvertSchemaToUTF8(CreateFeedContentForOrder(feedType, orders), stream, writer);
				var request = new SubmitFeedRequest
				{
					Merchant = this.AmazonMallCooperationSetting.AmazonMerchantId,
					MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
					FeedContent = stream,
					ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(stream),
					FeedType = feedType
				};
				var response = this.FeedClient.SubmitFeed(request);
				return response;
			}
		}
		#endregion

		#region +InvokeSubmitFeedForProduct SubmitFeed実行(商品情報)
		/// <summary>
		/// SubmitFeed実行(商品情報)
		/// </summary>
		/// <param name="feedType">フィードタイプ</param>
		/// <param name="products">商品情報</param>
		/// <returns>レスポンス</returns>
		public SubmitFeedResponse InvokeSubmitFeedForProduct(string feedType, ProductModel[] products)
		{
			using (var stream = new MemoryStream())
			using (var writer = new StreamWriter(stream, Encoding.UTF8))
			{
				ConvertSchemaToUTF8(CreateFeedContentForProduct(feedType, products), stream, writer);
				var request = new SubmitFeedRequest
				{
					Merchant = this.AmazonMallCooperationSetting.AmazonMerchantId,
					MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
					FeedContent = stream,
					ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(stream),
					FeedType = feedType
				};
				var response = this.FeedClient.SubmitFeed(request);
				return response;
			}
		}
		#endregion

		#region -CreateFeedContentForOrder FeedContent作成(注文情報)
		/// <summary>
		/// FeedContent作成(注文情報)
		/// </summary>
		/// <param name="feedType">フィードタイプ</param>
		/// <param name="orders">注文情報</param>
		/// <returns>FeedContent(注文情報)</returns>
		/// <remarks>将来的に注文修正(返品など)に対応することを想定</remarks>
		private SubmitFeedRequestXmlSchema CreateFeedContentForOrder(string feedType, OrderModel[] orders)
		{
			var feedContent = new SubmitFeedRequestXmlSchema();
			switch (feedType)
			{
				// 出荷通知
				case Constants.AMAZON_MALL_FEED_TYPE_ORDER_FULFILLMENT:
					feedContent = CreateFeedContentForOrderFulfillment(feedContent, orders);
					break;
			}
			return feedContent;
		}
		#endregion

		#region -CreateFeedContentForProduct FeedContent作成(商品情報)
		/// <summary>
		/// FeedContent作成(商品情報)
		/// </summary>
		/// <param name="feedType">フィードタイプ</param>
		/// <param name="products">商品情報</param>
		/// <returns>FeedContent(商品情報)</returns>
		private SubmitFeedRequestXmlSchema CreateFeedContentForProduct(string feedType, ProductModel[] products)
		{
			var feedContent = new SubmitFeedRequestXmlSchema();
			switch (feedType)
			{
				// 在庫連携
				case Constants.AMAZON_MALL_FEED_TYPE_INVENTORY_AVAILABILITY:
					feedContent = CreateFeedContentForInventory(feedContent, products);
					break;
			}
			return feedContent;
		}
		#endregion

		#region -CreateFeedContentForOrderFulfillment 出荷通知フィードFeedContent作成
		/// <summary>
		/// 出荷通知フィードFeedContent作成
		/// </summary>
		/// <param name="schema">XMLスキーマ</param>
		/// <param name="orders">注文情報</param>
		/// <returns>出荷通知フィードFeedContent</returns>
		private SubmitFeedRequestXmlSchema CreateFeedContentForOrderFulfillment(SubmitFeedRequestXmlSchema schema, OrderModel[] orders)
		{
			var fulfilmentMessages = orders.Select((order, index) =>
				new XmlSchema.Message
				{
					MessageID = (index + 1).ToString(),
					OrderFulfillment = new OrderFulfillment
					{
						AmazonOrderID = order.OrderId,
						FulfillmentDate = order.OrderShippedDate != null
							? XmlConvert.ToString((DateTime)order.OrderShippedDate, XmlDateTimeSerializationMode.Local)
							: XmlConvert.ToString((DateTime)order.OrderDeliveringDate, XmlDateTimeSerializationMode.Local),
						FulfillmentData = new FulfillmentData()
						{
							// Amazonの注文は配送先ごとに作成されるため、w2_OrderShipping.order_shipping_no = 1しか作成されない
							CarrierName = order.Shippings[0].DeliveryCompanyName,
							ShippingMethod = order.Shippings[0].ShippingMethod,
							ShipperTrackingNumber = order.Shippings[0].ShippingCheckNo
						},
						Item = null
					}
				}).ToList();

			schema.Header = new Header
			{
				DocumentVersion = Constants.AMAZON_FEED_DOCUMENT_VERSION,
				MerchantIdentifier = this.AmazonMallCooperationSetting.AmazonMerchantId
			};
			schema.MessageType = Constants.AMAZON_FEED_MESSAGE_TYPE_FULFILLMENT;
			schema.Message = fulfilmentMessages;
			return schema;
		}
		#endregion

		#region -CreateFeedContentForInventory 在庫フィードFeedContent作成
		/// <summary>
		/// 在庫フィードFeedContent作成
		/// </summary>
		/// <param name="schema">スキーマ</param>
		/// <param name="products">商品情報</param>
		/// <returns>在庫フィードFeedContent</returns>
		private SubmitFeedRequestXmlSchema CreateFeedContentForInventory(SubmitFeedRequestXmlSchema schema, ProductModel[] products)
		{
			var inventoryMessages = products.Select((product, index) =>
				new XmlSchema.Message
				{
					MessageID = (index + 1).ToString(),
					OperationType = Constants.AMAZON_OPERATION_TYPE_UPDATE,
					Inventory = new Inventory
					{
						SKU = product.AmazonSKU,
						Quantity = product.Stock,
						FulfillmentLatency = product.FulfillmentLatency
					}
				}).ToList();

			schema.Header = new Header
			{
				DocumentVersion = Constants.AMAZON_FEED_DOCUMENT_VERSION,
				MerchantIdentifier = this.AmazonMallCooperationSetting.AmazonMerchantId
			};
			schema.MessageType = Constants.AMAZON_FEED_MESSAGE_TYPE_FULFILLMENT;
			schema.Message = inventoryMessages;
			return schema;
		}
		#endregion

		#region -ConvertSchemaToUTF8 UTF8変換
		/// <summary>
		/// UTF8変換
		/// </summary>
		/// <param name="feedSchema">XMLスキーマ</param>
		/// <param name="stream">メモリストリーム</param>
		/// <param name="writer">ストリームライター</param>
		private void ConvertSchemaToUTF8(SubmitFeedRequestXmlSchema feedSchema, MemoryStream stream, StreamWriter writer)
		{
			var serializer = new XmlSerializerNamespaces();
			serializer.Add(string.Empty, string.Empty);
			new XmlSerializer(typeof(SubmitFeedRequestXmlSchema)).Serialize(writer, feedSchema, serializer);
			stream.Seek(0, SeekOrigin.Begin);
		}
		#endregion

		#region +GetFeedSubmissionList GetFeedSubmissionList実行
		/// <summary>
		/// GetFeedSubmissionList実行
		/// </summary>
		/// <returns>レスポンス</returns>
		public GetFeedSubmissionListResponse InvokeGetFeedSubmissionList(IdList feedIdList)
		{
			var request = new GetFeedSubmissionListRequest()
			{
				Merchant = this.AmazonMallCooperationSetting.AmazonMerchantId,
				MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
				FeedProcessingStatusList = new StatusList { Status = new List<string> { Constants.AMAZON_FEED_SUBMIT_RESULT_DONE } },
				FeedTypeList = new TypeList { Type = new List<string> { Constants.AMAZON_FEED_MESSAGE_TYPE_FULFILLMENT } },
				MaxCount = Constants.AMAZON_FEED_SUBMIT_MAXCOUNT,
				FeedSubmissionIdList = feedIdList
			};
			var response = this.FeedClient.GetFeedSubmissionList(request);
			return response;
		}
		#endregion

		#region +GetFeedSubmissionListByNextToken GetFeedSubmissionListByNextToken実行
		/// <summary>
		/// GetFeedSubmissionListByNextToken実行
		/// </summary>
		/// <param name="nextToken">NextToken</param>
		/// <returns>レスポンス</returns>
		public GetFeedSubmissionListByNextTokenResponse InvokeGetFeedSubmissionListByNextToken(string nextToken)
		{
			var request = new GetFeedSubmissionListByNextTokenRequest()
			{
				Merchant = this.AmazonMallCooperationSetting.AmazonMerchantId,
				MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
				NextToken = nextToken
			};
			var response = this.FeedClient.GetFeedSubmissionListByNextToken(request);
			return response;
		}
		#endregion

		#region +InvokeGetFeedSubmissionResult GetFeedSubmissionResult実行
		/// <summary>
		/// GetFeedSubmissionResult実行
		/// </summary>
		/// <param name="feedSubmissionId">FeedSubmissionId</param>
		/// <returns>レスポンス</returns>
		public GetFeedSubmissionResultResponse InvokeGetFeedSubmissionResult(string feedSubmissionId)
		{
			var resultXml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp", Constants.XML_FILE_NAME_TMP_FEEDSUBMISSION_RESULT);
			// 事前に前回分の結果を削除しておく
			File.Delete(resultXml);
			var request = new GetFeedSubmissionResultRequest()
			{
				Merchant = this.AmazonMallCooperationSetting.AmazonMerchantId,
				MWSAuthToken = this.AmazonMallCooperationSetting.AmazonMwsAuthtoken,
				FeedSubmissionId = feedSubmissionId,
				FeedSubmissionResult = File.Open(resultXml, FileMode.OpenOrCreate, FileAccess.ReadWrite)
			};
			var response = this.FeedClient.GetFeedSubmissionResult(request);
			request.FeedSubmissionResult.Close();
			
			return response;
		}
		#endregion

		#region プロパティ
		/// <summary>注文サービスクライアント</summary>
		private MarketplaceWebServiceOrdersClient OrdersClient { get; set; }
		/// <summary>フィードサービスクライアント</summary>
		private MarketplaceWebServiceClient FeedClient { get; set; }
		/// <summary>Amazonモール連携設定</summary>
		private MallCooperationSettingModel AmazonMallCooperationSetting { get; set; }
		#endregion
	}
}
