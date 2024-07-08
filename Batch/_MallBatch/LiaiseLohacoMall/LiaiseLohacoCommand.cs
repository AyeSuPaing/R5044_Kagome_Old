/*
=========================================================================================================
  Module      : Lohaco連携コマンドクラス(LiaiseLohacoCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.App.Common.LohacoCreatorWebApi.OrderChange;
using w2.App.Common.LohacoCreatorWebApi.OrderInfo;
using w2.App.Common.LohacoCreatorWebApi.OrderList;
using w2.App.Common.MallCooperation;
using w2.App.Common.LohacoCreatorWebApi;
using w2.App.Common.Order;
using w2.Domain.MallCooperationSetting;
using w2.Domain.Order;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Commerce.MallBatch.LiaiseLohacoMall.Helper;

namespace w2.Commerce.MallBatch.LiaiseLohacoMall
{
	/// <summary>
	/// Lohaco連携コマンドクラス
	/// </summary>
	public class LiaiseLohacoCommand
	{
		#region +OnStart 開始時処理
		/// <summary>
		/// 開始時処理
		/// </summary>
		public void OnStart()
		{
			// 実行開始時間設定
			FileLogger.WriteInfo(string.Format("Lohaco連携処理開始[実行開始時間：{0}]", DateTime.Now.ToString()));
		}
		#endregion

		#region +Exec 実行
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			// Lohacoモール連携設定取得
			var lohacoMallSettings
				= new MallCooperationSettingService().GetValidByMallKbn(Constants.CONST_DEFAULT_SHOP_ID, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO);

			foreach (var lohacoMallSetting in lohacoMallSettings)
			{
				// メンテナンス時間帯中はログ出力してスキップする
				if (IsMaintenanceTime(lohacoMallSetting)) continue;

				try
				{
					// 予約中注文から通常注文になる注文の取得
					CheckReserveOrderToOrderStatus(lohacoMallSetting);
					// 新規予約注文の取得
					ImportOrderList(lohacoMallSetting, true, Constants.WRITE_DEBUG_LOG_ENABLED, Constants.MASK_PERSONAL_INFO_ENABLED);
					// 新規注文の取得
					ImportOrderList(lohacoMallSetting, false, Constants.WRITE_DEBUG_LOG_ENABLED, Constants.MASK_PERSONAL_INFO_ENABLED);
				}
				catch (Exception ex)
				{
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_LOHACO_MALL,
						lohacoMallSetting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						"ロハコ連携が失敗しました。",
						ex.Message);
					FileLogger.WriteError(ex);
					continue;
				}
			}
		}
		#endregion

		#region +OnComplete 完了時処理
		/// <summary>
		/// 正常完了時処理
		/// </summary>
		public void OnComplete()
		{
			FileLogger.WriteInfo(string.Format("Lohaco連携処理完了[実行終了時間：{0}]", DateTime.Now.ToString()));
		}
		#endregion

		#region +OnError 異常時処理
		/// <summary>
		/// 異常時処理
		/// </summary>
		public void OnError()
		{
			FileLogger.WriteInfo("Lohaco連携処理でエラー発生しました。エラーの詳細はエラーログをご確認ください。");
		}
		#endregion

		#region -CheckReserveOrderToOrderStatus 予約注文のステタース変更チェック
		/// <summary>
		/// 予約注文のステタース変更チェック
		/// </summary>
		/// <param name="setting">ロハコモール設定情報</param>
		private void CheckReserveOrderToOrderStatus(MallCooperationSettingModel setting)
		{
			var service = new OrderService();
			var orderList = service.GetLohacoReserveOrder(setting.MallId, Constants.ORDER_EXTEND_STATUS_NO_LOHACO_RESERVE_ORDER);
			var command = new OrderInfoCommand();
			foreach (var order in orderList)
			{
				var request = new OrderInfoRequest(setting.MallId)
				{
					LohacoPrivateKey = setting.LohacoPrivateKey,
					Target = new OrderInfoRequest.OrderInfoTarget
					{
						OrderId = order.OrderId,
						Fields = new List<LohacoConstants.OrderField>
						{
							LohacoConstants.OrderField.OrderStatus,
						},
					},
					SellerId = setting.MallId,
				};
				BaseErrorResponse errorResponse;
				var response = command.OnExecute(request, setting.MallId, setting.LohacoPrivateKey, out errorResponse, Constants.WRITE_DEBUG_LOG_ENABLED, Constants.MASK_PERSONAL_INFO_ENABLED);
				if ((response != null) && (response.Result.Status == LohacoConstants.Status.OK))
				{
					// 予約中以外ステータスの場合、w2Commerce側に注文の予約ステータスをOFFに更新
					if (response.Result.OrderInfo.OrderStatus != LohacoConstants.OrderStatus.Reserving)
					{
						service.UpdateOrderExtendStatus(
							order.OrderId,
							Constants.ORDER_EXTEND_STATUS_NO_LOHACO_RESERVE_ORDER,
							Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert);
					}
				}
				else
				{
					var errorMessage = string.Format(
						"予約注文ID'{0}'の注文情報取得時、{1}",
						order.OrderId,
						((errorResponse != null) 
							|| ((response != null) && (response.Result.Status == LohacoConstants.Status.NG)))
							? string.Format("エラーコード：{0}、エラーメッセージ：{1}が発生しました。",
								(errorResponse != null) ? errorResponse.Code : response.Result.Error.Code,
								(errorResponse != null) ? errorResponse.Message : response.Result.Error.Message)
							: "予想以外エラーが発生しましたので、システム管理者にご連絡ください。");
					throw new ImportOrderException(errorMessage);
				}
			}
		}
		#endregion

		#region -CreateOrderListRequest 注文検索APIリクエストの作成
		/// <summary>
		/// 注文検索APIリクエストの作成
		/// </summary>
		/// <param name="setting">ロハコモール設定情報</param>
		/// <param name="isReserveRequest">予約注文取得かどうか（true：予約注文取得、false：通常注文取得）</param>
		/// <param name="startIndex">取得開始件数</param>
		/// <returns>注文検索APIリクエスト</returns>
		private OrderListRequest CreateOrderListRequest(MallCooperationSettingModel setting, bool isReserveRequest, int startIndex = 1)
		{
			var request = new OrderListRequest(setting.MallId)
			{
				LohacoPrivateKey = setting.LohacoPrivateKey,
				Search = new OrderListRequest.SearchTarget
				{
					Result = Constants.LOHACO_GET_ORDER_COUNT,
					Start = startIndex,
					Sort = LohacoConstants.SORT_ORDER_TIME_ASCENDING,
					Fields = new List<LohacoConstants.OrderField>() 
					{
						LohacoConstants.OrderField.OrderId,
					},
					Condition = new OrderListRequest.Condition
					{
						SellerId = setting.MallId,
						OrderTimeTo = StringUtility.ToDateString(DateTime.Now, Constants.DATE_FORMAT_LONG),
						IsSeen = false,
						OrderStatus = (isReserveRequest) ? LohacoConstants.OrderStatus.Reserving : LohacoConstants.OrderStatus.Processing,
					}
				}
			};

			return request;
		}
		#endregion

		#region -CreateOrderInfoRequest 注文詳細APIリクエストの作成
		/// <summary>
		/// 注文詳細APIリクエストの作成
		/// </summary>
		/// <param name="setting">ロハコモール設定情報</param>
		/// <param name="orderId">対象注文の注文ID</param>
		/// <returns>注文詳細APIリクエスト</returns>
		private OrderInfoRequest CreateOrderInfoRequest(MallCooperationSettingModel setting, string orderId)
		{
			var request = new OrderInfoRequest(setting.MallId)
			{
				LohacoPrivateKey = setting.LohacoPrivateKey,
				Target = new OrderInfoRequest.OrderInfoTarget
				{
					OrderId = orderId,
					Fields = new List<LohacoConstants.OrderField>
					{
						LohacoConstants.OrderField.OrderId,
						LohacoConstants.OrderField.DeviceType,
						LohacoConstants.OrderField.IsSeen,
						LohacoConstants.OrderField.OrderTime,
						LohacoConstants.OrderField.LastUpdateTime,
						LohacoConstants.OrderField.OrderStatus,
						LohacoConstants.OrderField.BuyerComments,
						LohacoConstants.OrderField.SellerComments,
						LohacoConstants.OrderField.Notes,
						LohacoConstants.OrderField.CouponType,
						LohacoConstants.OrderField.CouponCampaignCode,
						LohacoConstants.OrderField.StoreCouponType,
						LohacoConstants.OrderField.StoreCouponCode,
						LohacoConstants.OrderField.StoreCouponName,
						LohacoConstants.OrderField.PayStatus,
						LohacoConstants.OrderField.SettleStatus,
						LohacoConstants.OrderField.PayMethod,
						LohacoConstants.OrderField.PayMethodName,
						LohacoConstants.OrderField.PayDate,
						LohacoConstants.OrderField.PayNotes,
						LohacoConstants.OrderField.SettleId,
						LohacoConstants.OrderField.CardPayType,
						LohacoConstants.OrderField.CardPayCount,
						LohacoConstants.OrderField.NeedDetailedSlip,
						LohacoConstants.OrderField.BillFirstName,
						LohacoConstants.OrderField.BillLastName,
						LohacoConstants.OrderField.BillFirstNameKana,
						LohacoConstants.OrderField.BillLastNameKana,
						LohacoConstants.OrderField.BillZipCode,
						LohacoConstants.OrderField.BillPrefecture,
						LohacoConstants.OrderField.BillCity,
						LohacoConstants.OrderField.BillAddress1,
						LohacoConstants.OrderField.BillAddress2,
						LohacoConstants.OrderField.BillPhoneNumber,
						LohacoConstants.OrderField.BillMailAddress,
						LohacoConstants.OrderField.ShipStatus,
						LohacoConstants.OrderField.ShipMethod,
						LohacoConstants.OrderField.ShipMethodName,
						LohacoConstants.OrderField.ShipRequestDate,
						LohacoConstants.OrderField.ShipRequestTime,
						LohacoConstants.OrderField.ShipNotes,
						LohacoConstants.OrderField.ShipInvoiceNumber1,
						LohacoConstants.OrderField.DeliveryBoxType,
						LohacoConstants.OrderField.ShipDate,
						LohacoConstants.OrderField.ArrivalDate,
						LohacoConstants.OrderField.NeedGiftWrap,
						LohacoConstants.OrderField.GiftWrapType,
						LohacoConstants.OrderField.GiftWrapMessage,
						LohacoConstants.OrderField.NeedGiftWrapPaper,
						LohacoConstants.OrderField.GiftWrapPaperType,
						LohacoConstants.OrderField.GiftWrapName,
						LohacoConstants.OrderField.ShipFirstName,
						LohacoConstants.OrderField.ShipLastName,
						LohacoConstants.OrderField.ShipFirstNameKana,
						LohacoConstants.OrderField.ShipLastNameKana,
						LohacoConstants.OrderField.ShipZipCode,
						LohacoConstants.OrderField.ShipPrefecture,
						LohacoConstants.OrderField.ShipCity,
						LohacoConstants.OrderField.ShipAddress1,
						LohacoConstants.OrderField.ShipAddress2,
						LohacoConstants.OrderField.ShipPhoneNumber,
						LohacoConstants.OrderField.PayCharge,
						LohacoConstants.OrderField.ShipCharge,
						LohacoConstants.OrderField.GiftWrapCharge,
						LohacoConstants.OrderField.Discount,
						LohacoConstants.OrderField.TotalMallCouponDiscount,
						LohacoConstants.OrderField.StoreCouponDiscount,
						LohacoConstants.OrderField.Adjustments,
						LohacoConstants.OrderField.UsePoint,
						LohacoConstants.OrderField.UsePaypayAmount,
						LohacoConstants.OrderField.TotalPrice,
						LohacoConstants.OrderField.LineId,
						LohacoConstants.OrderField.CtgItemCd,
						LohacoConstants.OrderField.ItemCd,
						LohacoConstants.OrderField.Title,
						LohacoConstants.OrderField.UnitPrice,
						LohacoConstants.OrderField.OriginUnitPrice,
						LohacoConstants.OrderField.UnitStoreCouponDiscount,
						LohacoConstants.OrderField.Quantity,
						LohacoConstants.OrderField.UnitGetPoint,
						LohacoConstants.OrderField.AddonGetPoint,
						LohacoConstants.OrderField.AddonGetPointRatio,
						LohacoConstants.OrderField.OperationUser,
						LohacoConstants.OrderField.TaxRatio,
					},
				},
				SellerId = setting.MallId,
			};
			return request;
		}
		#endregion

		#region -CreateOrderChangeRequest 閲覧フラグを閲覧済みへの注文内容変更APIリクエストの作成
		/// <summary>
		/// 閲覧フラグを閲覧済みへの注文内容変更APIリクエストの作成
		/// </summary>
		/// <param name="setting">ロハコモール設定情報</param>
		/// <param name="orderId">対象注文の注文ID</param>
		/// <returns>注文内容変更APIリクエスト</returns>
		private OrderChangeRequest CreateOrderChangeRequest(MallCooperationSettingModel setting, string orderId)
		{
			var request = new OrderChangeRequest(setting.MallId)
			{
				Target = new OrderChangeRequest.ChangeTarget
				{
					OrderId = orderId,
					OperationUser = Constants.FLG_LASTCHANGED_BATCH,
				},
				Order = new OrderChangeRequest.ChangeOrder
				{
					IsSeen = true,
				}
			};
			return request;
		}
		#endregion

		#region -ImportOrderList 注文一覧の取込
		/// <summary>
		/// 注文一覧の取込
		/// </summary>
		/// <param name="setting">ロハコモール設定情報</param>
		/// <param name="isReserveRequest">予約注文かどうか（true：予約注文、false：通常注文）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログの出力有無</param>
		/// <param name="isMaskPersonalInfoEnabled">デバッグログ出力時個人情報のマスク有無</param>
		private void ImportOrderList(MallCooperationSettingModel setting, bool isReserveRequest, bool isWriteDebugLogEnabled, bool isMaskPersonalInfoEnabled)
		{
			// 新規予約注文・新規通常注文の取得
			var request = CreateOrderListRequest(setting, isReserveRequest);
			var command = new OrderListCommand();
			BaseErrorResponse errorResponse;
			var response = command.OnExecute(request, setting.MallId, setting.LohacoPrivateKey, out errorResponse, isWriteDebugLogEnabled, isMaskPersonalInfoEnabled);
			var orderIdList = new List<string>();

			// 予約注文・通常注文の作成
			// エラーの場合、例外に発生しましたので、対応しない
			if ((response != null) && (response.Status == LohacoConstants.Status.OK))
			{
				foreach (var order in response.SearchInfo.OrderInfo)
				{
					orderIdList.Add(order.OrderId);
				}
			}
			else
			{
				var errorMessage = string.Format("取込注文取得時、{0}",
						((errorResponse != null)
							|| ((response != null) && (response.Status == LohacoConstants.Status.NG)))
							? string.Format("エラーが発生しました。エラーコード：{0}、エラーメッセージ：{1}。",
								(errorResponse != null) ? errorResponse.Code : response.OrderListError.Code,
								(errorResponse != null) ? errorResponse.Message : response.OrderListError.Message)
							: "予想以外エラーが発生しましたので、システム管理者にご連絡ください。");
				throw new ImportOrderException(errorMessage);
			}

			foreach (var orderId in orderIdList)
			{
				ImportOrder(setting, orderId, isReserveRequest, isWriteDebugLogEnabled, isMaskPersonalInfoEnabled);
			}
		}
		#endregion

		#region -ImportOrder 注文の取込
		/// <summary>
		/// 注文の取込
		/// </summary>
		/// <param name="setting">ロハコモール設定情報</param>
		/// <param name="orderId">取込対象注文の注文ID</param>
		/// <param name="isReserveRequest">予約注文かどうか（true：予約注文、false：通常注文）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログの出力有無</param>
		/// <param name="isMaskPersonalInfoEnabled">デバッグログ出力時個人情報のマスク有無</param>
		private void ImportOrder(MallCooperationSettingModel setting, string orderId, bool isReserveRequest, bool isWriteDebugLogEnabled, bool isMaskPersonalInfoEnabled)
		{
			using (var accessor = new SqlAccessor())
			{
				try
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// 注文内容の取得
					var orderInfoRequest = CreateOrderInfoRequest(setting, orderId);
					var orderInfoCommand = new OrderInfoCommand();
					BaseErrorResponse errorResponse;
					var orderInfoResponse = orderInfoCommand.OnExecute(orderInfoRequest, setting.MallId, setting.LohacoPrivateKey, out errorResponse, isWriteDebugLogEnabled, isMaskPersonalInfoEnabled);

					// 必須な情報のチェック
					if ((orderInfoResponse == null) || (orderInfoResponse.Result == null) || (orderInfoResponse.Result.Status == LohacoConstants.Status.NG)
						|| (orderInfoResponse.Result.OrderInfo == null) || (orderInfoResponse.Result.OrderInfo.Detail == null)
						|| (orderInfoResponse.Result.OrderInfo.Item == null) || (orderInfoResponse.Result.OrderInfo.Pay == null)
						|| (orderInfoResponse.Result.OrderInfo.Ship == null))
					{
						if ((errorResponse != null)
							|| ((orderInfoResponse != null) && (orderInfoResponse.Result.Status == LohacoConstants.Status.NG)))
						{
							FileLogger.WriteError(string.Format(
								"注文ID'{0}'情報の取得に失敗しました。エラーコード：{1}、エラーメッセージ：{2}。",
								orderId,
								(errorResponse != null) ? errorResponse.Code : orderInfoResponse.Result.Error.Code,
								(errorResponse != null) ? errorResponse.Message : orderInfoResponse.Result.Error.Message));
						}
						throw new ImportOrderException(string.Format("注文ID'{0}'情報の取得に失敗しました。", orderId));
					}

					// 注文者のユーザが存在するかどうかチェック
					var userId = CheckRegisteredUser(setting.MallId, StringUtility.ToEmpty(orderInfoResponse.Result.OrderInfo.Pay.BillMailAddress), accessor);
					var warningList = new List<string>();
					if (string.IsNullOrEmpty(userId))
					{
						// 新規ユーザの登録
						var userData = ImportOrderHelper.CreateNewUserData(
							setting.MallId,
							orderInfoResponse.Result.OrderInfo.Pay,
							orderInfoResponse.Result.OrderInfo.DeviceType,
							orderInfoResponse.Result.OrderInfo.Buyer,
							ref warningList);
						userId = userData.UserId;
						RegisterUser(userData);
					}

					// 注文作成
					var orderModel = ImportOrderHelper.CreateOrderData(setting.MallId, userId, orderInfoResponse.Result, ref warningList);
					// DB登録
					var orderService = new OrderService();
					orderService.InsertOrder(orderModel, UpdateHistoryAction.Insert, accessor);
					if (isReserveRequest)
					{
						orderService.UpdateOrderExtendStatus(
							orderId,
							Constants.ORDER_EXTEND_STATUS_NO_LOHACO_RESERVE_ORDER,
							Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert,
							accessor);
					}
					// リアルタイム累計購入回数更新処理
					var order = new Hashtable
					{
						{Constants.FIELD_ORDER_USER_ID, userId},
						{Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME,  new UserService().Get(userId, accessor).OrderCountOrderRealtime},
					};
					OrderCommon.UpdateRealTimeOrderCount(order, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER, accessor);

					var isMinusStock = false;
					// 在庫情報更新、在庫履歴登録
					foreach (var orderItem in orderModel.Items)
					{
						if (orderItem.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED) continue;

						var stockUpdateResult = UpdateProductStock(orderItem, accessor);
						InsertProductHistory(orderItem, accessor);

						// 新規注文取込時にマイナス在庫が発生した場合はワーニング出力
						if (stockUpdateResult < 0)
						{
							isMinusStock = true;
							WarnMinusStock(setting.MallId, orderModel, orderItem, stockUpdateResult, accessor);
						}
					}

					//マイナス在庫がある場合、仮注文ステータスに更新
					if (isMinusStock)
					{
						new OrderService().UpdateOrderStatus(
							orderModel.OrderId,
							Constants.FLG_ORDER_ORDER_STATUS_TEMP,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert,
							accessor);
					}

					// 注文情報の閲覧済みフラグの更新
					var orderChangeRequest = CreateOrderChangeRequest(setting, orderId);
					var orderChangeCommand = new OrderChangeCommand();
					var orderChangeResponse = orderChangeCommand.OnExecute(
						orderChangeRequest,
						setting.MallId,
						setting.LohacoPrivateKey,
						out errorResponse,
						isWriteDebugLogEnabled,
						isMaskPersonalInfoEnabled);

					if ((orderChangeResponse == null) || (orderChangeResponse.Result == null)
						|| (orderChangeResponse.Result.Status == LohacoConstants.Status.NG))
					{
						string errorMesssage = null;
						if ((errorResponse != null)
							|| ((orderChangeResponse != null) && (orderChangeResponse.Result.Status == LohacoConstants.Status.NG)))
						{
							errorMesssage = string.Format("エラーコード：{0}、エラーメッセージ：{1}。",
								(errorResponse != null) ? errorResponse.Code : orderChangeResponse.Result.Error.Code,
								(errorResponse != null) ? errorResponse.Message : orderChangeResponse.Result.Error.Message);
						}
						throw new ImportOrderException(string.Format("注文ID'{0}'の閲覧済みフラグを更新時失敗しました。{1}", orderId, StringUtility.ToEmpty(errorMesssage)));
					}

					accessor.CommitTransaction();
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_LOHACO_MALL,
						setting.MallId,
						(warningList.Count > 0) ? Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING : Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
						string.Format("注文ID'{0}'取込が成功しました。", orderId),
						(warningList.Count > 0)
							? string.Format(
								"警告事項がございますので、ご確認・対応お願いいたします。{0}{1}",
								Environment.NewLine + "・",
								string.Join(Environment.NewLine + "・", warningList.ToArray()))
							: string.Empty);
				}
				catch (ImportOrderException ex)
				{
					accessor.RollbackTransaction();

					// ログ出力
					FileLogger.WriteError(string.Format("注文ID'{0}'取込時にエラー発生しました。", orderId), ex);
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_LOHACO_MALL,
						setting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						string.Format("注文ID'{0}'取込時にエラーが発生しました。", orderId),
						ex.Message);
				}
				catch (Exception ex1)
				{
					accessor.RollbackTransaction();

					// ログ出力
					FileLogger.WriteError(string.Format("注文ID'{0}'取込時にエラー発生しました。", orderId), ex1);
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_LOHACO_MALL,
						setting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						string.Format("注文ID'{0}'取込時にエラーが発生しました。システム管理者にお問い合わせください。", orderId));
				}
			}
		}
		#endregion

		#region -UpdateProductStock 在庫数更新
		/// <summary>
		/// 在庫数更新
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新後在庫数</returns>
		private int UpdateProductStock(OrderItemModel orderItem, SqlAccessor accessor)
		{
			var itemQuantity = orderItem.ItemQuantity;
			var result = new ProductStockService().UpdateProductStockAndGetStock(
				Constants.CONST_DEFAULT_SHOP_ID,
				orderItem.ProductId,
				orderItem.VariationId,
				itemQuantity,
				Constants.FLG_LASTCHANGED_BATCH,
				accessor);
			return result.Stock;
		}
		#endregion

		#region -InsertProductHistory 商品在庫履歴登録
		/// <summary>
		/// 商品在庫履歴登録
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void InsertProductHistory(OrderItemModel orderItem, SqlAccessor accessor)
		{
			var model = new ProductStockHistoryModel
			{
				OrderId = orderItem.OrderId,
				ShopId = orderItem.ShopId,
				ProductId = orderItem.ProductId,
				VariationId = orderItem.VariationId,
				ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER,
				AddStock = orderItem.ItemQuantity * (-1),
				UpdateMemo = string.Empty,
				DateCreated = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH
			};
			new ProductStockHistoryService().Insert(model, accessor);
		}
		#endregion

		#region -WarnMinusStock マイナス在庫警告出力
		/// <summary>
		/// マイナス在庫警告出力
		/// </summary>
		/// <param name="mallId">ロハコモールID</param>
		/// <param name="order">注文情報</param>
		/// <param name="orderItem">マイナス在庫になった注文商品情報</param>
		/// <param name="stock">在庫数</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void WarnMinusStock(string mallId, OrderModel order, OrderItemModel orderItem, int stock, SqlAccessor accessor)
		{
			// 注文情報のステータスと管理メモに警告出力
			var warningMessage = string.Format(
				"Lohacoからの受注情報取込処理時({0})、「バリエーションID：{1}」で在庫数が0未満({2})になりました。外部連携ステータス'異常'で取り込みました。",
				DateTime.Now,
				orderItem.VariationId,
				stock);
			order.ExternalImportStatus = Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_ERROR;
			order.ManagementMemo = string.IsNullOrWhiteSpace(order.ManagementMemo) ? warningMessage : string.Concat(order.ManagementMemo, Environment.NewLine, warningMessage);
			new OrderService().UpdateForModify(order, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert, accessor);

			// モール監視ログに出力
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_LOHACO_MALL,
				mallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
				string.Format("注文ID：{0}[{1}]", order.OrderId, warningMessage));
		}
		#endregion

		#region -CheckRegisteredUser 登録済みユーザかどうかのチェック
		/// <summary>
		/// 登録済みユーザかどうかのチェック
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録済み：最初のユーザID、未登録：空文字</returns>
		private static string CheckRegisteredUser(string mallId, string mailAddr, SqlAccessor accessor)
		{
			string userId = new UserService().GetUserId(StringUtility.ToEmpty(mallId), StringUtility.ToEmpty(mailAddr), accessor);
			// ユーザーIDが取れない場合Nullってるので、Emptyで返却
			return StringUtility.ToEmpty(userId);
		}
		#endregion

		#region -RegisterUser 新規ユーザ登録
		/// <summary>
		/// 新規ユーザ登録
		/// </summary>
		/// <param name="user">新規ユーザモデル</param>
		private static void RegisterUser(UserModel user)
		{
			// 新規ユーザ登録
			var userService = new UserService();
			var result = userService.InsertWithUserExtend(
				user,
				Constants.FLG_LASTCHANGED_BATCH,
				UpdateHistoryAction.Insert);
			if (result == false)
			{
				throw new ImportOrderException("新規ユーザ登録に失敗しました。");
			}
		}
		#endregion

		#region -IsMaintenanceTime メンテナンス時間帯か
		/// <summary>
		/// メンテナンス時間帯か
		/// </summary>
		/// <param name="lohacoMallSetting">Lohaco連携設定</param>
		/// <returns>判定結果（true:メンテナンス時間帯中、false：メンテナンス時間帯外）</returns>
		private bool IsMaintenanceTime(MallCooperationSettingModel lohacoMallSetting)
		{
			// メンテナンス時間中はスキップ
			var isMaintenance = false;
			if ((lohacoMallSetting.MaintenanceDateFrom != null) || (lohacoMallSetting.MaintenanceDateTo != null))
			{
				var maintenanceDateFrom = lohacoMallSetting.MaintenanceDateFrom ?? DateTime.MinValue;
				var maintenanceDateTo = lohacoMallSetting.MaintenanceDateTo ?? DateTime.MaxValue;

				// 「メンテナンス開始日 <= 現在 < メンテナンス終了日」の時、次のモールの処理へ飛ばす
				if ((maintenanceDateFrom <= DateTime.Now) && (DateTime.Now < maintenanceDateTo))
				{
					// モール監視ログ登録（メンテナンス期間中）
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_LOHACO_MALL,
						lohacoMallSetting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
						"メンテナンス時間帯のため処理を実行しませんでした。");
					isMaintenance = true;
				}
			}
			return isMaintenance;
		}
		#endregion
	}
}
