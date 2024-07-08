/*
=========================================================================================================
  Module      : 出荷通知アクションクラス(OrderFulfillmentAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.IO;
using AmazonMarketPlaceWebService;
using MarketplaceWebService.Model;
using w2.App.Common.MallCooperation;
using w2.Domain.Order;
using System.Threading;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Amazon
{
	/// <summary>
	/// 出荷通知アクションクラス
	/// </summary>
	public class OrderFulfillmentAction : ActionBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="amazonApi">AmazonAPIサービス</param>
		/// <param name="mallId">モールID</param>
		public OrderFulfillmentAction(AmazonApiService amazonApi, string mallId)
			: base(amazonApi, mallId)
		{
		}

		#region +OnStart 開始時処理
		/// <summary>
		/// 開始時処理
		/// </summary>
		public override void OnStart()
		{
			// モール監視ログ登録
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
				this.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
				"出荷通知処理を開始しました。");
		}
		#endregion

		#region +Exec 実行
		/// <summary>
		/// 実行
		/// </summary>
		public override void Exec()
		{
			// 出荷通知対象取得
			var orders = GetOrderFulfilmentTargetOrders();
			if (orders.Count() == 0) return;

			// 出荷通知(SubmitFeed)
			var submitFeedResponce = this.AmazonApi.InvokeSubmitFeedForOrder(Constants.AMAZON_MALL_FEED_TYPE_ORDER_FULFILLMENT, orders);
			if (submitFeedResponce.SubmitFeedResult.FeedSubmissionInfo.FeedProcessingStatus != AmazonMarketPlaceWebService.Constants.AMAZON_FEED_SUBMIT_RESULT_SUBMITTED) return;

			// 注文情報更新(出荷通知処理中)
			UpdateShipNoteProgress(orders);

			// SubmitFeed実行結果取得(GetFeedSubmissionList)
			var resultFeedSubmissionId = submitFeedResponce.SubmitFeedResult.FeedSubmissionInfo.FeedSubmissionId;
			var feedProcessingStatus = string.Empty;
			var feedSubmissionListResponse = new GetFeedSubmissionListResponse();
			while (feedProcessingStatus != AmazonMarketPlaceWebService.Constants.AMAZON_FEED_SUBMIT_RESULT_DONE)
			{
				feedSubmissionListResponse = this.AmazonApi.InvokeGetFeedSubmissionList(new IdList{ Id = new List<string> { resultFeedSubmissionId } });
				feedProcessingStatus = feedSubmissionListResponse.GetFeedSubmissionListResult.FeedSubmissionInfo[0].FeedProcessingStatus;

				// リクエスト回復のため、次のリクエスト送信まで45秒間待機
				Thread.Sleep(45000);
			}

			ConfirmOrderFulfilmentResult(feedSubmissionListResponse.GetFeedSubmissionListResult.FeedSubmissionInfo, orders);
		}
		#endregion

		#region +OnComplete 完了時処理
		/// <summary>
		/// 完了時処理
		/// </summary>
		public override void OnComplete()
		{
			// モール監視ログ登録
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
				this.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
				"出荷通知処理を完了しました。");
		}
		#endregion

		#region +OnError 異常時処理
		/// <summary>
		/// 異常時処理
		/// </summary>
		public override void OnError()
		{
			// モール監視ログ登録
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
				this.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
				"出荷通知処理時に例外エラーが発生しました。システム管理者にお問い合わせください。");
		}
		#endregion

		#region -GetShipNoteTargetOrders 出荷通知対象注文情報取得
		/// <summary>
		/// 出荷通知対象注文情報取得
		/// </summary>
		/// <returns>出荷通知対象注文情報リスト</returns>
		private OrderModel[] GetOrderFulfilmentTargetOrders()
		{
			var dv = new OrderService().GetForAmazonOrderFulfilment(this.MallId);
			var orders = dv.Cast<DataRowView>()
				.Select(drv => 
					new OrderModel
					{
						OrderId = (string)drv[Constants.FIELD_ORDER_ORDER_ID],
						OrderShippedDate = (drv[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE] != DBNull.Value)
							? (DateTime?)drv[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE]
							: null,
						OrderDeliveringDate = (drv[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE] != DBNull.Value)
							? (DateTime?)drv[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE]
							: null,
						Shippings = new OrderShippingModel[1]
						{
							new OrderShippingModel
							{
								DeliveryCompanyId = (string)drv[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID],
								ShippingMethod = (string)drv[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD],
								ShippingCheckNo = (string)drv[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO],
								DeliveryCompanyName = (string)drv[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME]
							}
						}
					}).ToArray();
			return orders;
		}
		#endregion

		#region -UpdateShipNoteProgress 出荷通知処理中更新
		/// <summary>
		/// 出荷通知処理中更新
		/// </summary>
		/// <param name="orders">注文情報リスト</param>
		private void UpdateShipNoteProgress(OrderModel[] orders)
		{
			var condition = string.Format(
				"w2_Order.order_id in ('{0}')",
				string.Join("','",orders.Select(o => o.OrderId)));
			new OrderService().UpdateAmazonShipNoteProgress(condition);
		}
		#endregion

		#region -ConfirmOrderFulfilmentResult 出荷通知処理結果確認
		/// <summary>
		/// 出荷通知処理結果確認
		/// </summary>
		/// <param name="feedSubmissionInfo">フィード送信情報</param>
		/// <param name="orders">出荷通知処理対象注文情報</param>
		private void ConfirmOrderFulfilmentResult(List<FeedSubmissionInfo> feedSubmissionInfo, OrderModel[] orders)
		{
			foreach (var info in feedSubmissionInfo)
			{
				// SubmitFeed結果確認
				var response = this.AmazonApi.InvokeGetFeedSubmissionResult(info.FeedSubmissionId);
				if ((response == null) || (response.IsSetGetFeedSubmissionResultResult() == false)) continue;

				// 処理結果取得
				var feedSubmissionResult = GetFeedSubmissionResult();

				var message = string.Format(
					"Amazon出荷通知実行結果\r\n"
					+ "FeedSubmitId：{0}\r\n"
					+ "Submitted Date：{1}\r\n"
					+ "Processing Start：{2}\r\n"
					+ "Processing Complete：{3}\r\n"
					+ "処理件数：{4}\r\n"
					+ "成功件数：{5}\r\n"
					+ "エラー件数：{6}\r\n"
					+ "警告件数：{7}",
					info.FeedSubmissionId,
					info.SubmittedDate,
					info.StartedProcessingDate,
					info.CompletedProcessingDate,
					feedSubmissionResult.MessageProcessed,
					feedSubmissionResult.MessagesSuccessful,
					feedSubmissionResult.MessagesWithError,
					feedSubmissionResult.MessagesWithWarning);

				// モール監視ログに出力
				if ((feedSubmissionResult.MessagesWithError > 0) || (feedSubmissionResult.MessagesWithWarning > 0))
				{
					// モール監視ログ登録
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
						this.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
						string.Format("出荷通知処理でエラーが発生したレコードがあります。\r\n{0}",message.ToString()),
						feedSubmissionResult.FeedSubmissionResultXmlText);
				}
				else
				{
					// モール監視ログ登録
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
						this.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
						string.Format("出荷通知処理が正常に完了しました。\r\n{0}", message.ToString()),
						feedSubmissionResult.FeedSubmissionResultXmlText);
				}

				// 出荷通知済みに更新
				UpdateAmazonShipNoteComplete(orders);
			}
		}
		#endregion

		#region -GetFeedSubmissionResult FeedSubmissionResult処理結果取得
		/// <summary>
		/// FeedSubmissionResult処理結果取得
		/// </summary>
		/// <returns>FeedSubmissionResult処理結果</returns>
		private FeedSubmissionResultModel GetFeedSubmissionResult()
		{
			var xmlFilePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				"tmp",
				AmazonMarketPlaceWebService.Constants.XML_FILE_NAME_TMP_FEEDSUBMISSION_RESULT);
			using (var fs = new FileStream(xmlFilePath, FileMode.Open))
			using (var sr = new StreamReader(fs))
			{
				var xmlText = sr.ReadToEnd();
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xmlText);

				var feedSubmissionResult = new FeedSubmissionResultModel
				{
					MessageProcessed = int.Parse(xmlDocument.GetElementsByTagName(Constants.FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_PROCESSED)[0].InnerText),
					MessagesSuccessful = int.Parse(xmlDocument.GetElementsByTagName(Constants.FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_SUCCESSFUL)[0].InnerText),
					MessagesWithError = int.Parse(xmlDocument.GetElementsByTagName(Constants.FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_WITH_ERROR)[0].InnerText),
					MessagesWithWarning = int.Parse(xmlDocument.GetElementsByTagName(Constants.FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_WITH_WARNING)[0].InnerText),
					FeedSubmissionResultXmlText = xmlText,
				};
				return feedSubmissionResult;
			}
		}
		#endregion

		#region -UpdateAmazonShipNoteComplete Amazon出荷通知済み更新
		/// <summary>
		/// Amazon出荷通知済み更新
		/// </summary>
		/// <param name="orders">出荷通知対象注文情報</param>
		private void UpdateAmazonShipNoteComplete(OrderModel[] orders)
		{
			var condition = string.Format(
				"w2_Order.order_id in ('{0}')",
				string.Join("','",orders.Select(o => o.OrderId)));
			new OrderService().UpdateAmazonShipNoteComplete(condition);
		}
		#endregion
	}

	#region +FeedSubmissionResultModel FeedSubmissionResult実行結果格納モデルクラス
	/// <summary>
	/// FeedSubmissionResult実行結果格納モデルクラス
	/// </summary>
	public class FeedSubmissionResultModel
	{
		/// <summary>処理件数</summary>
		public int MessageProcessed { get; set; }
		/// <summary>成功件数</summary>
		public int MessagesSuccessful { get; set; }
		/// <summary>エラー件数</summary>
		public int MessagesWithError { get; set; }
		/// <summary>警告件数</summary>
		public int MessagesWithWarning { get; set; }
		/// <summary>処理結果XML内容</summary>
		public string FeedSubmissionResultXmlText { get; set; }
	}
	#endregion
}
