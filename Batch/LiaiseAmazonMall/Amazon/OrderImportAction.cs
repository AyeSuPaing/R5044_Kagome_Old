/*
=========================================================================================================
  Module      : 注文取込アクションクラス(OrderImportAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using AmazonMarketPlaceWebService;
using MarketplaceWebServiceOrders.Model;
using w2.App.Common.MallCooperation;
using w2.Commerce.Batch.LiaiseAmazonMall.Import;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MemberRank;
using w2.Domain.Order;
using w2.Domain.ShopShipping;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Amazon
{
	/// <summary>
	/// 注文取込アクションクラス
	/// </summary>
	public class OrderImportAction : ActionBase
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="amazonApi">AmazonAPI</param>
		/// <param name="latestExecuteDatetime">前回実行時間</param>
		/// <param name="mallId">モールID</param>
		public OrderImportAction(AmazonApiService amazonApi, DateTime? latestExecuteDatetime, string mallId)
			: base(amazonApi, mallId)
		{
			this.LatestExecuteDatetime = latestExecuteDatetime;
		}
		#endregion

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
				"注文取込処理を開始しました。");
		}
		#endregion

		#region +Exec 実行
		/// <summary>
		/// 実行
		/// </summary>
		public override void Exec()
		{
			// 注文取得
			var amazonOrders = GetAmazonOrders();

			// 注文取込
			ImportOrder(amazonOrders);
		}
		#endregion

		#region +OnComplete 終了時処理
		/// <summary>
		/// 終了時処理
		/// </summary>
		public override void OnComplete()
		{
			// モール監視ログ登録
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
				this.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
				string.Format(
					"注文取込処理を完了しました。(取込件数：{0}件、エラー件数：{1}件)",
					this.ActionResult.SuccessCount.ToString(),
					this.ActionResult.ErrorCount.ToString()));
		}
		#endregion

		#region
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
				"注文取込処理時に例外エラーが発生しました。システム管理者にお問い合わせください。");
		}
		#endregion

		#region -GetAmazonOrders Amazon注文取得
		/// <summary>
		/// Amazon注文取得
		/// </summary>
		/// <returns>Amazon注文リスト</returns>
		private List<Order> GetAmazonOrders()
		{
			// ListOrdersで注文取得
			var response = this.AmazonApi.InvokeListOrders(this.LatestExecuteDatetime);
			var orders = new List<Order>();
			if ((response != null) && response.IsSetListOrdersResult() && response.ListOrdersResult.IsSetOrders())
			{
				orders.AddRange(response.ListOrdersResult.Orders);
			}
			else
			{
				return orders;
			}

			// NextTokenが存在する限り残りの注文を取得
			var nextToken = response.ListOrdersResult.NextToken;
			while (string.IsNullOrEmpty(nextToken) == false)
			{
				var nextTokenResponse = this.AmazonApi.InvokeListOrdersByNextToken(nextToken);
				if ((nextTokenResponse != null) && nextTokenResponse.IsSetListOrdersByNextTokenResult()
						&& nextTokenResponse.ListOrdersByNextTokenResult.IsSetOrders())
				{
					orders.AddRange(nextTokenResponse.ListOrdersByNextTokenResult.Orders);
					nextToken = nextTokenResponse.ListOrdersByNextTokenResult.NextToken;
				}
			}
			return orders;
		}
		#endregion

		#region -ImportOrder 注文取込
		/// <summary>
		/// 注文取込
		/// </summary>
		/// <param name="orders">Amazon注文リスト</param>
		private void ImportOrder(List<Order> orders)
		{
			if (orders.Count == 0) return;
			// 事前に取込済みAmazon注文とデフォルト設定を取得
			var importedOrders = GetImportedAmazonOrder(orders);
			var defaultDeliveryCompanyId = GetDefaultDeliveryCompanyId();
			var defaultMemberRank = MemberRankService.GetDefaultMemberRank();

			foreach (var order in orders)
			{
				// 取込用にAmazon注文モデルを作成
				var importedOrder = importedOrders.FirstOrDefault(o => o.OrderId == order.AmazonOrderId);
				var amazonOrder = CreateAmazonOrderModel(order, defaultDeliveryCompanyId, defaultMemberRank, importedOrder);

				// 未取込のキャンセル注文は取込処理対象外とする
				if ((amazonOrder.Order.OrderStatus == Constants.AMAZON_MALL_ORDER_STATUS_CANCELED) && (amazonOrder.ImportedOrder == null))
				{
					continue;
				}

				using (var accessor = new SqlAccessor())
				{
					try
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						// 注文情報取込
						var orderImport = new OrderImport(amazonOrder, this.MallId, accessor);
						orderImport.Import();

						// ユーザ情報取込
						var userImport = new UserImport(amazonOrder, this.MallId, accessor);
						userImport.Import();

						accessor.CommitTransaction();
						this.ActionResult.SuccessCount++;
					}
					catch (NoMatchItemException noMatchItemException)
					{
						// 商品みつからないエラー

						accessor.RollbackTransaction();
						this.ActionResult.ErrorCount++;

						// ログ出力
						FileLogger.WriteError(string.Format("注文ID'{0}'取込時にエラー発生。{1}", amazonOrder.Order.AmazonOrderId, noMatchItemException.Message));

						new MallWatchingLogManager().Insert(
							Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
							this.MallId,
							Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
							string.Format("注文ID'{0}'取込時にエラーが発生しました。{1}", amazonOrder.Order.AmazonOrderId, noMatchItemException.Message));
					}
					catch (Exception ex)
					{
						accessor.RollbackTransaction();
						this.ActionResult.ErrorCount++;

						// ログ出力
						FileLogger.WriteError(string.Format("注文ID'{0}'取込時にエラー発生", amazonOrder.Order.AmazonOrderId));
						AppLogger.WriteError(ex);
						new MallWatchingLogManager().Insert(
							Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
							this.MallId,
							Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
							string.Format("注文ID'{0}'取込時にエラーが発生しました。システム管理者にお問い合わせください。", amazonOrder.Order.AmazonOrderId));
					}
				}
			}
		}
		#endregion

		#region -GetExistedAmazonOrder 取込済み注文情報取得
		/// <summary>
		/// 取込済み注文情報取得
		/// </summary>
		/// <returns>取込済み注文情報リスト</returns>
		private List<OrderModel> GetImportedAmazonOrder(List<Order> amazonOrders)
		{
			var condition = string.Format(
				"w2_Order.order_id in ('{0}')",
				string.Join("','", amazonOrders.Select(item => item.AmazonOrderId)));
			var list = new OrderService().GetImportedAmazonOrder(condition);

			var importedOrders = list.Select(s => s).ToList();
			return importedOrders;
		}
		#endregion

		#region -GetDefaultDeliveryCompanyId デフォルト配送会社ID取得
		/// <summary>
		/// デフォルト配送会社ID取得
		/// </summary>
		/// <returns>デフォルト配送会社ID</returns>
		private string GetDefaultDeliveryCompanyId()
		{
			var defaultDeliveryCompany
				= new ShopShippingService().GetDefaultCompany(Constants.LIAISE_AMAZON_MALL_DEFAULT_SHIPPING_ID, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
			
			var defaultDeliveryCompanyId = (defaultDeliveryCompany != null) ? defaultDeliveryCompany.DeliveryCompanyId : string.Empty;
			return defaultDeliveryCompanyId;
		}
		#endregion

		#region -CreateAmazonOrderModel 取込用Amazon注文情報モデル作成
		/// <summary>
		/// 取込用Amazon注文情報モデル作成
		/// </summary>
		/// <param name="amazonOrder">Amazon注文情報</param>
		/// <param name="defaultDeliveryCompanyId">デフォルト配送会社ID</param>
		/// <param name="defaultMemberRankId">デフォルト会員ランクID</param>
		/// <param name="importedOrder">取込済み注文情報(新規注文の場合はNULL)</param>
		/// <returns>取込用Amazon注文情報モデル</returns>
		private AmazonOrderModel CreateAmazonOrderModel(Order amazonOrder, string defaultDeliveryCompanyId, MemberRankModel defaultMemberRank, OrderModel importedOrder)
		{
			var amazonOrderModel = new AmazonOrderModel
			{
				Order = amazonOrder,
				OrderItem = GetAmazonOrderItems(amazonOrder.AmazonOrderId),
				DefaultDeliveryCompanyId = defaultDeliveryCompanyId,
				DefaultMemberRankId = (defaultMemberRank != null) ? defaultMemberRank.MemberRankId : string.Empty,
				ImportedOrder = importedOrder
			};
			return amazonOrderModel;
		}
		#endregion

		#region -GetAmazonOrderItems Amazon注文商品取得
		/// <summary>
		/// Amazon注文商品取得
		/// </summary>
		/// <param name="amazonOrderId">Amazon注文ID</param>
		/// <returns>Amazon注文商品リスト</returns>
		private List<OrderItem> GetAmazonOrderItems(string amazonOrderId)
		{
			// ListOrderItemsで注文商品取得
			var orderItem = new List<OrderItem>();
			var response = this.AmazonApi.InvokeListOrderItems(amazonOrderId);
			if (response != null && response.IsSetListOrderItemsResult() && response.ListOrderItemsResult.IsSetOrderItems())
			{
				orderItem.AddRange(response.ListOrderItemsResult.OrderItems);
			}

			// NextTokenが存在する限り残りの注文商品を取得
			var nextToken = response.ListOrderItemsResult.NextToken;
			while (string.IsNullOrEmpty(nextToken) == false)
			{
				var nextTokenResponse = this.AmazonApi.InvokeListOrderItemsByNextToken(nextToken);
				if ((nextTokenResponse != null) && nextTokenResponse.IsSetListOrderItemsByNextTokenResult()
						&& nextTokenResponse.ListOrderItemsByNextTokenResult.IsSetOrderItems())
				{
					orderItem.AddRange(nextTokenResponse.ListOrderItemsByNextTokenResult.OrderItems);
					nextToken = nextTokenResponse.ListOrderItemsByNextTokenResult.NextToken;
				}
			}
			return orderItem;
		}
		#endregion

		#region プロパティ
		/// <summary>前回実行時間</summary>
		private DateTime? LatestExecuteDatetime { get; set; }
		#endregion
	}
}
