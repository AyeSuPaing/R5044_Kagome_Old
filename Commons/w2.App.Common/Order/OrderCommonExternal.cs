/*
=========================================================================================================
  Module      : 外部連携用注文共通処理パーシャルクラス(OrderCommonExternal.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.Mail;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.App.Common.Properties;
using w2.App.Common.Stock;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Domain.Product;
using w2.Domain.SerialKey;

namespace w2.App.Common.Order
{
	/// <summary>
	/// OrderCommonのPartialクラス
	/// </summary>
	public partial class OrderCommon
	{
		#region 定数
		public const string LAST_CHANGED_API = "API";
		public const string DEFAULT_MAILTEMPLATE_ORDER_CANCEL = "00000080";	// 注文キャンセルのデフォルトメールテンプレート
		public const string DEFAULT_MAILTEMPLATE_ORDER_SHIPPING_COMP = "00000060";	// 配送完了のデフォルトメールテンプレート
		public const string PARAMETER_ORDER_CREATED_FROM = "created_from";	// Created from of parameter order
		public const string PARAMETER_ORDER_CREATED_BEFORE = "created_before";	// Created before of parameter order
		public const string PARAMETER_ORDER_UPDATED_FROM = "updated_from";	// Updated from of parameter order
		public const string PARAMETER_ORDER_UPDATED_BEFORE = "updated_before";	// Updated before of parameter order

		#endregion

		/// <summary>
		/// 注文情報キャンセル（ステータス・外部連携以外更新）
		/// </summary>
		/// <param name="drvOrder">パラメータの説明を記述</param>
		/// <param name="blRollBackRealStock">パラメータの説明を記述</param>
		/// <param name="strProductStockHistoryActionStatus">パラメータの説明を記述</param>
		/// <param name="strLoginOperatorDeptId">パラメータの説明を記述</param>
		/// <param name="strLoginOperatorName">パラメータの説明を記述</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="updateTwInvoiceStatus">電子発票ステータス</param>
		/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
		public static void CancelOrder(
			DataRowView drvOrder,
			bool blRollBackRealStock,
			string strProductStockHistoryActionStatus,
			string strLoginOperatorDeptId,
			string strLoginOperatorName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor,
			string updateTwInvoiceStatus = "")
		{
			CancelOrderSubProcess(
				drvOrder,
				blRollBackRealStock,
				strProductStockHistoryActionStatus,
				strLoginOperatorDeptId,
				strLoginOperatorName,
				Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED,
				updateHistoryAction,
				sqlAccessor,
				updateTwInvoiceStatus);
		}

		#region +CancelOrder 注文キャンセル実行
		/// <summary>
		/// 注文情報キャンセル実行+注文キャンセルメール送信
		/// ここでキャンセルする情報は、「在庫情報」、「利用ポイント」、「利用クーポン」、「シリアルキー」、「ステータス」
		/// 本メソッド内でトランザクションの制御を行う
		/// 「在庫情報」、「利用ポイント」、「利用クーポン」、「シリアルキー」、「ステータス」のいずれかのキャンセルに失敗したらロールバック
		/// し、エラーをthrow
		/// トランザクション終了後にメールを送信する（エラった場合は送らない）
		/// </summary>
		/// <param name="orderId">キャンセル対象の注文ID</param>
		/// <param name="doesMail">キャンセルメールを送るか否か</param>
		/// <param name="apiMemo">キャンセルした注文の外部連携メモに記載する値</param>
		/// <param name="isOverwriteMemo">外部連携メモに上書きまたは追記かのフラグ
		/// True：上書き
		/// False:追記
		/// </param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void CancelOrder(
			string orderId,
			bool doesMail,
			string apiMemo,
			bool isOverwriteMemo,
			UpdateHistoryAction updateHistoryAction)
		{
			// キャンセル対象の注文情報取得
			var orderView = GetOrder(orderId);

			// キャンセル対象の注文情報とれたか？
			if ((orderView == null) || (orderView.Count == 0))
			{
				//注文取れない場合はエラー
				throw new Exception(string.Format("キャンセル対象の注文情報が取得できませんでした。order_id:'{0}'", orderId));
			}

			// 注文がキャンセル可能かチェック
			if (!ChkPermitCancelStatus(orderView[0]))
			{
				// キャンセル不可なステータスの場合
				throw new Exception(string.Format("指定された注文のキャンセルは行えません。order_id:'{0}'\r\n指定した注文のステータスが{1}です。", orderId, orderView[0][Constants.FIELD_ORDER_ORDER_STATUS]));
			}

			// トランザクション開始
			using (var sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					// 在庫戻し処理
					RollbackStock(orderView[0], LAST_CHANGED_API, sqlAccessor);

					// 利用ポイント戻し処理
					RollbackPoint(orderView[0], "0", LAST_CHANGED_API, updateHistoryAction, sqlAccessor);

					// 利用クーポン戻し処理
					RollbackCoupon(orderView[0], "0", LAST_CHANGED_API, updateHistoryAction, sqlAccessor);

					// シリアルキー戻し処理
					RollbackSerialKey(orderView[0], "0", sqlAccessor);

					// 注文のステータスをキャンセルに+Apiメモ更新
					UpdateOrderStatus(
						sqlAccessor,
						orderView[0],
						LAST_CHANGED_API,
						apiMemo,
						isOverwriteMemo,
						Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED,
						updateHistoryAction);

					// 定期購入情報：購入回数(注文基準)戻し処理
					var order = new OrderModel(orderView[0]);
					RollbackFixedPurchaseOrderCount(order, LAST_CHANGED_API, updateHistoryAction, sqlAccessor);

					sqlAccessor.CommitTransaction();
				}
				catch
				{
					// えらったらロールバック
					sqlAccessor.RollbackTransaction();
					// 上位にエラー投げる
					throw;
				}
			}

			// コミット後にメール送る
			if (doesMail)
			{
				SendOrderMail(orderView[0][Constants.FIELD_ORDER_ORDER_ID].ToString(), DEFAULT_MAILTEMPLATE_ORDER_CANCEL);
			}
		}
		#endregion

		#region +ShipOrder 注文出荷完了実行
		/// <summary>
		/// 注文出荷完了実行+出荷完了メール送信
		/// ここで更新する情報は、「仮ポイントから本ポイントへの更新」、「シリアルキー引渡し」、「ステータス」、「配送伝票番号」
		/// 本メソッド内でトランザクションの制御を行う
		/// 「仮ポイントから本ポイントへの更新」、「シリアルキー引渡し」、「ステータス」、「配送伝票番号」のいずれかの更新
		/// に失敗したらロールバック
		/// トランザクション終了後にメールを送信する（エラった場合は送らない）
		/// </summary>
		/// <param name="orderId">出荷完了対象の注文ID</param>
		/// <param name="shippingNo">配送伝票番号更新対象の配送先枝番</param>
		/// <param name="shippingCheckNo">更新する配送伝票番号、Emptyの場合は配送伝票番号の更新は行わない</param>
		/// <param name="doesMail">出荷完了メールを送るか否か</param>
		/// <param name="apiMemo">出荷完了にした注文の外部連携メモの記載する値</param>
		/// <param name="isOverwriteMemo">外部連携メモに上書きまたは追加かのフラグ</param>
		/// <param name="shippedDate">出荷完了日</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void ShipOrder(
			string orderId,
			int shippingNo,
			string shippingCheckNo,
			bool doesMail,
			string apiMemo,
			bool isOverwriteMemo,
			DateTime shippedDate,
			UpdateHistoryAction updateHistoryAction)
		{
			// 出荷完了対象の注文情報取得
			var orderView = GetOrder(orderId);

			// 出荷完了対象の注文情報とれたか？
			if ((orderView == null) || (orderView.Count == 0))
			{
				//注文取れない場合はエラー
				throw new Exception(string.Format("出荷完了対象の注文情報が取得できませんでした。order_id:'{0}'", orderId));
			}

			// トランザクション開始
			using (var sqlAccessor = new SqlAccessor())
			{

				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					// 仮ポイントを本ポイントへ
					UpdateUserPointTempToComp(
						orderView[0],
						"0",
						LAST_CHANGED_API,
						Constants.GRANT_ORDER_POINT_AUTOMATICALLY,
						updateHistoryAction,
						sqlAccessor);

					// 注文ステータス更新
					// 出荷完了にする
					UpdateOrderStatus(
						sqlAccessor,
						orderView[0],
						LAST_CHANGED_API,
						apiMemo,
						isOverwriteMemo,
						Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP,
						updateHistoryAction,
						shippedDate);

					// 注文配送伝票番号更新
					// 配送伝票番号が空の場合は更新しない
					if (!string.IsNullOrEmpty(shippingCheckNo))
					{
						// 空白、Null以外の時だけ更新
						UpdateOrderShippingCheckNo(orderId, shippingNo, shippingCheckNo, updateHistoryAction, sqlAccessor);
					}

					// シリアルキー引当
					DeliverSerialKey(
						orderId,
						LAST_CHANGED_API,
						sqlAccessor);

					// 定期購入回数(出荷時点)更新処理
					var order = new OrderModel(orderView[0]);
					UpdateFixedPurchaseShippedCount(order, LAST_CHANGED_API, updateHistoryAction, sqlAccessor);

					// トランザクションコミット
					sqlAccessor.CommitTransaction();
				}
				catch
				{
					// えらったらロールバック
					sqlAccessor.RollbackTransaction();
					// 上位にエラー投げる
					throw;
				}

				// コミット後にメール送る
				if (doesMail)
				{
					SendOrderMail(orderView[0][Constants.FIELD_ORDER_ORDER_ID].ToString(), DEFAULT_MAILTEMPLATE_ORDER_SHIPPING_COMP);
				}
			}
		}
		#endregion

		#region +CompleteShipment 配送完了通知
		/// <summary>
		/// 配送完了実行+出荷完了メール送信
		/// ここで更新する情報は、「シリアルキー引渡し」、「ステータス」、「配送伝票番号」
		/// 本メソッド内でトランザクションの制御を行う
		/// 「シリアルキー引渡し」、「ステータス」、「配送伝票番号」のいずれかの更新
		/// に失敗したらロールバック
		/// トランザクション終了後にメールを送信する（エラった場合は送らない）
		/// </summary>
		/// <param name="orderId">配送完了対象の注文ID</param>
		/// <param name="shippingNo">配送伝票更新対象の配送先枝番</param>
		/// <param name="shippingCheckNo">更新する配送伝票番号、Emptyの場合は配送伝票番号の更新は行わない</param>
		/// <param name="doesMail">出荷完了メールを送るか否か</param>
		/// <param name="apiMemo">配送完了にした注文の外部連携メモの記載する値</param>
		/// <param name="isOverwriteMemo">外部連携メモに上書きまたは追加かのフラグ</param>
		/// <param name="deliveringDate">配送完了日</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void CompleteShipment(
			string orderId,
			int shippingNo,
			string shippingCheckNo,
			bool doesMail,
			string apiMemo,
			bool isOverwriteMemo,
			DateTime deliveringDate,
			UpdateHistoryAction updateHistoryAction)
		{
			// 出荷完了対象の注文情報取得
			var orderView = GetOrder(orderId);

			// 出荷完了対象の注文情報とれたか？
			if ((orderView == null) || (orderView.Count == 0))
			{
				// 注文取れない場合はエラー
				throw new Exception(string.Format("配送完了対象の注文情報が取得できませんでした。order_id:'{0}'", orderId));
			}

			// トランザクション開始
			using (var sqlAccessor = new SqlAccessor())
			{

				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					// 注文ステータス更新
					// 配送完了にする
					UpdateOrderStatus(
						sqlAccessor,
						orderView[0],
						LAST_CHANGED_API,
						apiMemo,
						isOverwriteMemo,
						Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP,
						updateHistoryAction,
						null,
						deliveringDate);

					// 注文配送伝票番号更新
					// 配送伝票番号が空の場合は更新しない
					if (!string.IsNullOrEmpty(shippingCheckNo))
					{
						// 空白、Null以外の時だけ更新
						UpdateOrderShippingCheckNo(
							orderId,
							shippingNo,
							shippingCheckNo,
							updateHistoryAction,
							sqlAccessor);
					}

					// シリアルキー引当
					DeliverSerialKey(
						orderId,
						LAST_CHANGED_API,
						sqlAccessor);

					// 定期購入回数(出荷時点)更新処理
					var order = new OrderModel(orderView[0]);
					UpdateFixedPurchaseShippedCount(order, LAST_CHANGED_API, updateHistoryAction, sqlAccessor);

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertAllForOrder(orderId, LAST_CHANGED_API, sqlAccessor);
					}

					// トランザクションコミット
					sqlAccessor.CommitTransaction();
				}
				catch
				{
					// えらったらロールバック
					sqlAccessor.RollbackTransaction();
					// 上位にエラー投げる
					throw;
				}

				//コミット後にメール送る
				if (doesMail)
				{
					SendOrderMail(orderView[0][Constants.FIELD_ORDER_ORDER_ID].ToString(), DEFAULT_MAILTEMPLATE_ORDER_SHIPPING_COMP);
				}
			}
		}
		#endregion

		#region +UpdateOrderShippingCheckNo 配送伝票番号更新
		/// <summary>
		/// 配送伝票番号更新
		/// </summary>
		/// <param name="orderId">更新対象の注文ID</param>
		/// <param name="shippingNo">更新対象の配送先枝番</param>
		/// <param name="shippingCheckNo">更新する配送伝票番号の値</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void UpdateOrderShippingCheckNo(
			string orderId,
			int shippingNo,
			string shippingCheckNo,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			new OrderService().UpdateOrderShippingCheckNo(
				orderId,
				shippingNo,
				shippingCheckNo,
				LAST_CHANGED_API,
				updateHistoryAction,
				sqlAccessor);
		}
		#endregion

		#region +DeliverSerialKey シリアルキー引渡し
		/// <summary>
		/// シリアルキー引渡し
		/// </summary>
		/// <param name="orderId">引渡し対象の注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void DeliverSerialKey(
			string orderId,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			new SerialKeyService().DeliverSerialKey(orderId, lastChanged, sqlAccessor);
		}
		#endregion

		#region +UpdateUserPointTempToComp 仮ポイントを本ポイントへ更新

		/// <summary>
		/// 仮ポイントを本ポイントへ更新
		/// </summary>
		/// <param name="drvOrder">本ポイントへ更新する対象の注文情報をもつDataRowView</param>
		/// <param name="shopId">対象ショップID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderPointAutomaticallyFlg">本注文自動ポイント付与フラグ
		/// Falseの場合は仮ポイントを本ポイントには更新しない</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void UpdateUserPointTempToComp(
			DataRowView drvOrder,
			string shopId,
			string lastChanged,
			bool orderPointAutomaticallyFlg,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{

			// ポイントオプションが有効かどうか
			if (Constants.W2MP_POINT_OPTION_ENABLED == false)
			{
				// ポイントオプションが有効でない場合は処理しない
				return;
			}

			// 注文本ポイント自動付与か否か
			if (orderPointAutomaticallyFlg == false)
			{
				// 自動付与ではない場合は本ポイントへ更新しない
				return;
			}

			// 仮ポイントを持っているか？
			if ((HasUserTempPoint(drvOrder[Constants.FIELD_ORDER_USER_ID].ToString(),
				drvOrder[Constants.FIELD_ORDER_ORDER_ID].ToString()))
				== false)
			{
				// 仮ポイント持ってなければ更新しない
				return;
			}

			// 仮ポイント→本ポイント更新
			// 指定されたトランザクション内で行う
			var sv = new PointService();
			sv.TempToRealPoint(
				drvOrder[Constants.FIELD_ORDER_USER_ID].ToString(),
				drvOrder[Constants.FIELD_ORDER_ORDER_ID].ToString(),
				lastChanged,
				updateHistoryAction,
				sqlAccessor);

			// CROSS POINT連携(CP上の仮付与ポイントを有効ポイントに確定)
			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				var pointApiInput = new PointApiInput
				{
					MemberId = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_USER_ID]),
					OrderId = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_ORDER_ID]),
					UserCode = lastChanged
				};
				
				new CrossPointPointApiService().Grant(pointApiInput.GetParam(PointApiInput.RequestType.Grant));
			}
		}

		#endregion

		#region +HasUserTempPoint ユーザが仮ポイントを持っているか否か
		public static bool HasUserTempPoint(string userId, string orderId)
		{
			// 1件でもあればTrue
			// なければFalse
			return (GetUserPointTemp(userId, orderId).Any());
		}
		#endregion

		#region +GetUserPointTemp ユーザーポイント情報取得(仮ポイント)
		/// <summary>
		/// ユーザーポイント情報取得(仮ポイント)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>ユーザーポイント情報取得</returns>
		public static UserPointModel[] GetUserPointTemp(string userId, string orderId)
		{
			var models = new PointService().GetUserPoint(userId, string.Empty)
				.Where(p => ((p.OrderId == orderId) && p.IsPointTypeTemp))
				.ToArray();

			return models;
		}
		#endregion

		#region メール周り

		#region +PC向け各注文・入金ステータス更新連絡メールの送信
		/// <summary>
		/// PC向け各注文・入金ステータス更新連絡メールの送信
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="userId">ユーザーID</param>
		public static void SendOrderMailToPc(Hashtable order, string mailId, string userId)
		{

			using (var msMailSend = new MailSendUtility((string)order[Constants.FIELD_MAILTEMPLATE_SHOP_ID], mailId, userId, order, true, Constants.MailSendMethod.Auto, userMailAddress: (string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]))
			{
				msMailSend.AddTo((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]);

				if (msMailSend.SendMail() == false)
				{
					throw msMailSend.MailSendException;
				}
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region モバイル向け各注文・入金ステータス更新連絡メールの送信
		/// <summary>
		/// PC向け各注文・入金ステータス更新連絡メールの送信
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="orderMobile">注文情報（モバイル用）</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="userId">ユーザーID</param>
		public static void SendOrderMailToMobile(Hashtable order, Hashtable orderMobile, string mailId, string userId)
		{
			using (var msMailSend = new MailSendUtility((string)order[Constants.FIELD_MAILTEMPLATE_SHOP_ID], mailId, userId, orderMobile, false, Constants.MailSendMethod.Auto, userMailAddress: (string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]))
			{
				msMailSend.AddTo((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]);

				if (msMailSend.SendMail() == false)
				{
					throw msMailSend.MailSendException;
				}
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		#endregion

		#region +SendOrderMail 各注文・入金ステータス更新連絡メールを送信
		/// <summary>
		/// 各注文・入金ステータス更新連絡メールを送信
		/// </summary>
		/// <param name="strOrderId">注文ID</param>
		/// <param name="strMailId">メールテンプレートID</param>
		/// <returns>処理結果(成功:true 失敗:false)</returns>
		public static void SendOrderMail(string strOrderId, string strMailId)
		{
			//------------------------------------------------------
			// 注文情報取得
			//------------------------------------------------------
			var htOrder = new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(strOrderId);
			var htOrderMobile = new MailTemplateDataCreaterForOrder(false).GetOrderMailDatas(strOrderId);

			//------------------------------------------------------
			// メール送信
			//------------------------------------------------------
			if (htOrder.Count != 0)
			{
				var sendPc = false;
				var sendMobile = false;
				if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
				{
					sendPc = ((string)htOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "");
					sendMobile = ((string)htOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] != "");
				}
				else
				{
					if ((string)htOrder[Constants.FIELD_ORDER_ORDER_KBN] != Constants.FLG_ORDER_ORDER_KBN_MOBILE)
					{
						if ((string)htOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "")
						{
							sendPc = true;
						}
						else if ((string)htOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] != "")
						{
							sendMobile = true;
						}
					}
					else if ((string)htOrder[Constants.FIELD_ORDER_ORDER_KBN] == Constants.FLG_ORDER_ORDER_KBN_MOBILE)
					{
						if ((string)htOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] != "")
						{
							sendMobile = true;
						}
						else if ((string)htOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "")
						{
							sendPc = true;
						}
					}
				}

				// PCメール送信
				var userId = (string)htOrder[Constants.FIELD_ORDER_USER_ID];
				if (sendPc)
				{
					// PC向けメール送信
					SendOrderMailToPc(htOrder, strMailId, userId);
				}

				// モバイルメール送信
				if (sendMobile)
				{
					if (htOrderMobile.Count != 0)
					{
						// モバイル向けメール送信
						// PC向けメール送信
						SendOrderMailToMobile(htOrder, htOrderMobile, strMailId, userId);
					}
				}
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える


		}
		#endregion

		#region +SetOrderShippingDateTimeForOrderMailTemplete 配送希望日時セット（注文メールテンプレート用）
		/// <summary>
		/// 配送希望日時セット（注文メールテンプレート用）
		/// </summary>
		/// <param name="htInput"></param>
		/// <param name="languageLocaleId">注文者の言語ロケールID</param>
		private static void SetOrderShippingDateTimeForOrderMailTemplete(Hashtable htInput, string languageLocaleId = "")
		{
			htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]
				= (StringUtility.ToEmpty(htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]) != "")
					? DateTimeUtility.ToString(
						(string)htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE],
						DateTimeUtility.FormatType.ShortDate2Letter,
						languageLocaleId)
					: CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@");
			string strShippingTimeId = StringUtility.ToEmpty(htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME]);
			htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = "";
			for (int iLoop = 1; iLoop <= 10; iLoop++)
			{
				if (StringUtility.ToEmpty(htInput["shipping_time_id" + iLoop.ToString()]) == strShippingTimeId)
				{
					htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = htInput["shipping_time_message" + iLoop];
					break;
				}
			}
			if (StringUtility.ToEmpty(htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME]) == "")
			{
				htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = CommonPage.ReplaceTag("@@DispText.shipping_time_list.none@@");
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		#endregion

		#region +GetOrderItemsStringForOrderMailTemplete 注文メールテンプレート用に、注文商品情報の文字列を取得する。
		/// <summary>
		/// 注文メールテンプレート用に、注文商品情報の文字列を取得する。
		/// </summary>
		/// <param name="strOrderId"></param>
		/// <param name="orderItems"></param>
		/// <param name="strBorderString"></param>
		/// <returns>注文商品情報文字列</returns>
		private static string GetOrderItemsStringForOrderMailTemplete(string strOrderId, List<DataRowView> orderItems, string strBorderString)
		{
			//------------------------------------------------------
			// デジタルコンテンツ用にシリアルキー取得
			//------------------------------------------------------
			DataView dvOrder;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatements = new SqlStatement("Order", "GetSendOrderMail_SerialKey"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, strOrderId }
				};

				dvOrder = sqlStatements.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 注文商品情報文字列の組み立て
			//------------------------------------------------------
			var sbOrderItems = new StringBuilder();
			var iIndex = 0;
			var iOrderItemNo = 1;
			while (iIndex < orderItems.Count)
			{
				sbOrderItems.Append((iIndex != 0) ? "\r\n" : "");

				sbOrderItems.Append("-(").Append(iOrderItemNo).Append(")----------" + strBorderString).Append("\r\n");
				if (((string)orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length != 0)
				{
					int iLoop2 = 1;
					string strProductSetId = StringUtility.ToEmpty(orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]);
					string strProductSetNo = StringUtility.ToEmpty(orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]);

					decimal dItemPriceTotal = 0;
					while ((iIndex < orderItems.Count)
						&& (strProductSetId == StringUtility.ToEmpty(orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]))
						&& (strProductSetNo == StringUtility.ToEmpty(orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_SET_NO])))
					{
						dItemPriceTotal += (decimal)orderItems[iIndex][Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE];

						sbOrderItems.Append(" -(").Append(iOrderItemNo).Append("-").Append(iLoop2).Append(")----------" + strBorderString).Append("\r\n");
						sbOrderItems.Append(" 商品ID    ：").Append((string)orderItems[iIndex][Constants.FIELD_ORDERITEM_VARIATION_ID]).Append("\r\n");
						sbOrderItems.Append(" 商品名    ：").Append((string)orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_NAME]).Append("\r\n");
						sbOrderItems.Append(" 商品単価  ：").Append(@"¥").Append(StringUtility.ToNumeric(orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_PRICE])).Append("\r\n");
						sbOrderItems.Append(" 数量      ：").Append(StringUtility.ToNumeric(orderItems[iIndex][Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE])).Append("\r\n");

						iIndex++;
						iLoop2++;
					}

					sbOrderItems.Append(" --------------" + strBorderString).Append("\r\n");
					sbOrderItems.Append(" セット価格：").Append(@"¥").Append(StringUtility.ToNumeric(dItemPriceTotal)).Append("\r\n");
					sbOrderItems.Append(" 数量      ：").Append(StringUtility.ToNumeric(orderItems[iIndex - 1][Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT])).Append("");
				}
				else
				{
					sbOrderItems.Append("商品ID    ：").Append((string)orderItems[iIndex][Constants.FIELD_ORDERITEM_VARIATION_ID]).Append("\r\n");
					sbOrderItems.Append("商品名    ：").Append((string)orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_NAME]).Append("\r\n");
					sbOrderItems.Append("商品単価  ：").Append(@"¥").Append(StringUtility.ToNumeric(orderItems[iIndex][Constants.FIELD_ORDERITEM_PRODUCT_PRICE])).Append("\r\n");
					sbOrderItems.Append("数量      ：").Append(StringUtility.ToNumeric(orderItems[iIndex][Constants.FIELD_ORDERITEM_ITEM_QUANTITY])).Append("");

					// デジタルコンテンツ対応
					for (var i = 0; i < dvOrder.Count; i++)
					{
						if ((int)dvOrder[i][Constants.FIELD_SERIALKEY_ORDER_ITEM_NO] == iOrderItemNo)
						{
							sbOrderItems.Append("\r\n");
							sbOrderItems.Append("シリアルキー　　：");
							sbOrderItems.Append(SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey((string)dvOrder[i][Constants.FIELD_SERIALKEY_SERIAL_KEY])));
						}
					}

					iIndex++;
				}
				iOrderItemNo++;
			}
			// 注文商品情報を返却
			return sbOrderItems.ToString();

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		#endregion

		#region GetMessageForSeparateEstimate 別途見積りのためのメッセージ取得
		/// <summary>
		/// 別途見積りのためのメッセージ取得
		/// </summary>
		/// <param name="dvOrder">注文情報</param>
		/// <param name="isPc">PCか</param>
		/// <returns>PC,Mobileサイトに合わせたメッセージ</returns>
		private static string GetMessageForSeparateEstimate(DataView dvOrder, bool isPc)
		{
			var message = (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED
				&& ((string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] == Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID))
					? isPc
						? GetShopShipping((string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_ID], (string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_ID]).ShippingPriceSeparateEstimatesMessage
						: GetShopShipping((string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_ID], (string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_ID]).ShippingPriceSeparateEstimatesMessageMobile
					: string.Empty;
			return message;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#endregion

		#region -UpdateOrderStatus 注文ステータス更新(外部連携memoは更新しない）
		/// <summary>
		/// 注文ステータス更新(外部連携memoは更新しない）
		/// </summary>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		/// <param name="drvOrder">キャンセル対象のw2_orderのDatarowview</param>
		/// <param name="strLoginOperatorName">最終更新者</param>
		/// <param name="updateStatus">更新する注文ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private static void UpdateOrderStatus(
			SqlAccessor sqlAccessor,
			DataRowView drvOrder,
			string strLoginOperatorName,
			string updateStatus,
			UpdateHistoryAction updateHistoryAction)
		{
			UpdateOrderStatus(sqlAccessor, drvOrder, strLoginOperatorName, "", false, updateStatus, updateHistoryAction);
		}
		#endregion

		#region -UpdateOrderStatus 注文ステータス更新
		/// <summary>
		/// 注文ステータス更新
		/// </summary>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		/// <param name="drvOrder">更新対象のw2_orderのDatarowview</param>
		/// <param name="strLoginOperatorName">最終更新者</param>
		/// <param name="strreLationMemo">更新または追記する外部連携メモ</param>
		/// <param name="updateStatus">更新する注文ステータス</param>
		/// <param name="isOverwriteMemo">
		/// True:外部連携メモを上書き
		/// False:外部連携メモを追記
		/// </param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="shippedDate">出荷完了日</param>
		/// <param name="deliveringDate">配送完了日</param>
		private static void UpdateOrderStatus(
			SqlAccessor sqlAccessor,
			DataRowView drvOrder,
			string strLoginOperatorName,
			string strreLationMemo,
			bool isOverwriteMemo,
			string updateStatus,
			UpdateHistoryAction updateHistoryAction,
			DateTime? shippedDate = null,
			DateTime? deliveringDate = null)
		{
			string statement = isOverwriteMemo
				? "UpdateOrderStatusOverwriteMemo" // 外部連携メモ上書き
				: "UpdateOrderStatusAppendMemo"; // 外部連携メモ追加

			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "Order", "Order", statement))
			{
				var ht = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, drvOrder[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_LAST_CHANGED, strLoginOperatorName },
					{ Constants.FIELD_ORDER_RELATION_MEMO, strreLationMemo },
					{ Constants.FIELD_ORDER_ORDER_STATUS, updateStatus },
					{ Constants.FIELD_ORDER_ORDER_SHIPPED_DATE, shippedDate ?? drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE] },
					{ Constants.FIELD_ORDER_ORDER_DELIVERING_DATE, deliveringDate ?? drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE] }
				};
				sqlStatement.ExecStatement(sqlAccessor, ht);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(
					(string)drvOrder[Constants.FIELD_ORDER_ORDER_ID],
					strLoginOperatorName,
					sqlAccessor);
			}
		}
		#endregion

		#region +RollbackStock 在庫戻し処理
		/// <summary>
		/// 在庫戻し処理
		/// </summary>
		/// <param name="drvOrder">戻し処理を行うw2_orderのDatarowview</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void RollbackStock(
			DataRowView drvOrder,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// 在庫戻し処理
			//------------------------------------------------------
			var stockCommon = new StockCommon();

			// 論理在庫戻し
			// 注文した商品分
			foreach (
				DataRowView itemRow in
					GetOrderItems(
						drvOrder[Constants.FIELD_ORDER_ORDER_ID].ToString(),
						drvOrder[Constants.FIELD_ORDER_SHOP_ID].ToString(),
						sqlAccessor))
			{
				var product = new ProductService().Get(
					StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_SHOP_ID]),
					StringUtility.ToEmpty(itemRow[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
					sqlAccessor);

				if ((product == null)
					|| (product.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)) continue;

				// 在庫情報取得
				var stockView = stockCommon.GetStock(
					itemRow[Constants.FIELD_ORDERITEM_PRODUCT_ID].ToString(),
					itemRow[Constants.FIELD_ORDERITEM_VARIATION_ID].ToString(),
					sqlAccessor);

				// 在庫データがとれた場合だけ在庫戻す
				if (stockView.Count > 0)
				{
					DataRowView stockDr = stockView[0];

					stockCommon.UpdateProductStock(
						drvOrder[Constants.FIELD_ORDER_ORDER_ID].ToString(),
						drvOrder[Constants.FIELD_ORDER_SHOP_ID].ToString(),
						itemRow[Constants.FIELD_ORDERITEM_PRODUCT_ID].ToString(),
						itemRow[Constants.FIELD_ORDERITEM_VARIATION_ID].ToString(),
						StockCommon.LAST_CHANGED_API,
						Convert.ToInt32(stockDr[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]),
						Convert.ToInt32(itemRow[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]),
						0,
						0,
						0,
						0,
						Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL,
						"",
						sqlAccessor);
				}
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +RollbackPoint 利用ポイント戻し処理
		/// <summary>
		/// 利用ポイント戻し処理
		/// </summary>
		/// <param name="drvOrder">戻し処理を行うw2_orderのDatarowview</param>
		/// <param name="shopId">DeptIdとして利用するショップID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void RollbackPoint(
			DataRowView drvOrder,
			string shopId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// 利用・付与ポイント戻し処理
			//-----------------------------------------------------
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// ポイントオプションが有効な場合のみ

				// ユーザー利用ポイント戻し
				CancelUserPointUse(
					drvOrder,
					shopId,
					lastChanged,
					shouldRestoreExpiredPoint: false,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);

				// ユーザー付与ポイント戻し
				CancelUserPointAdd(
					drvOrder,
					shopId,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder((string)drvOrder[Constants.FIELD_ORDER_ORDER_ID], lastChanged, sqlAccessor);
					new UpdateHistoryService().InsertForUser((string)drvOrder[Constants.FIELD_ORDER_USER_ID], lastChanged, sqlAccessor);
				}
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +RollbackCoupon 利用クーポン戻し処理
		/// <summary>
		/// 利用クーポン戻し処理
		/// </summary>
		/// <param name="orderInfo">戻し処理を行うw2_orderのDatarowview</param>
		/// <param name="shopId">DeptIdとして利用するショップID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void RollbackCoupon(
			DataRowView orderInfo,
			string shopId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// クーポン戻し処理
			//------------------------------------------------------
			// クーポンオプションが有効 かつ 注文クーポン情報が存在する場合
			if ((Constants.W2MP_COUPON_OPTION_ENABLED) && (orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_NO] != DBNull.Value))
			{
				var couponService = new CouponService();
				var userId = StringUtility.ToEmpty(orderInfo[Constants.FIELD_ORDER_USER_ID]);
				var orderId = StringUtility.ToEmpty(orderInfo[Constants.FIELD_ORDER_ORDER_ID]);
				//------------------------------------------------------
				// 回数制限有りの利用クーポンを利用済み→未使用更新
				//------------------------------------------------------
				// 回数制限有りクーポンの場合
				var couponType = (string)orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_TYPE];
				if (CouponOptionUtility.IsCouponLimit(couponType))
				{
					//------------------------------------------------------
					// 本注文で利用したクーポンを利用済み→未使用更新
					//------------------------------------------------------
					couponService.UpdateUserCouponUseFlg(
						userId,
						StringUtility.ToEmpty(orderInfo[Constants.FIELD_ORDERCOUPON_DEPT_ID]),
						StringUtility.ToEmpty(orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_ID]),
						(int)orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_NO],
						false,
						// unuse
						DateTime.Now,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);
				}
				// 回数制限クーポンを使った場合（利用回数を戻す）
				else if (CouponOptionUtility.IsCouponAllLimit(couponType))
				{
					//------------------------------------------------------
					// 利用可能回数をプラス１する
					//------------------------------------------------------
					couponService.UpdateCouponCountUp(
						(string)orderInfo[Constants.FIELD_ORDERCOUPON_DEPT_ID],
						(string)orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_ID],
						(string)orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_CODE],
						lastChanged,
						sqlAccessor);
				}
				// ブラックリスト型クーポンを使った場合、利用済み情報を削除する
				else if (CouponOptionUtility.IsBlacklistCoupon(couponType))
				{
					var couponUseUser = (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
						== Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
						? new UserService().Get(userId, sqlAccessor).MailAddr
						: userId;
					couponService.DeleteCouponUseUser(
						(string)orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_ID],
						couponUseUser,
						sqlAccessor);
				}

				//------------------------------------------------------
				// ユーザクーポン履歴登録(利用クーポン)
				//------------------------------------------------------
				couponService.InsertUserCouponHistory(
					userId,
					orderId,
					(string)orderInfo[Constants.FIELD_ORDERCOUPON_DEPT_ID],
					(string)orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_ID],
					(string)orderInfo[Constants.FIELD_ORDERCOUPON_COUPON_CODE],
					Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL,
					Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
					1,
					(decimal)orderInfo[Constants.FIELD_ORDER_ORDER_COUPON_USE] * -1,
					lastChanged,
					sqlAccessor);

				//------------------------------------------------------
				// ユーザクーポン履歴登録(発行クーポン)
				// ※削除処理前に履歴登録を行う
				//------------------------------------------------------
				// 本注文で発行したユーザクーポン情報を取得
				var publishedCoupons = couponService.GetOrderPublishUserCoupon(shopId, userId, orderId, sqlAccessor);
				foreach (var coupon in publishedCoupons)
				{
					couponService.InsertUserCouponHistory(
						userId,
						orderId,
						shopId,
						coupon.CouponId,
						coupon.CouponCode,
						Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH_CANCEL,
						Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
						-1,
						((coupon.DiscountPrice != null) ? (coupon.DiscountPrice.GetValueOrDefault()) : 0) * -1,
						lastChanged,
						sqlAccessor);
				}

				//------------------------------------------------------
				// 本注文で発行したクーポンを削除
				//------------------------------------------------------
				couponService.DeleteUserCouponByOrderId(userId, orderId, lastChanged, updateHistoryAction, sqlAccessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(orderId, lastChanged, sqlAccessor);
					new UpdateHistoryService().InsertForUser(userId, lastChanged, sqlAccessor);
				}
			}
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +RollbackSerialKey シリアルキー戻し処理
		/// <summary>
		/// シリアルキー戻し処理
		/// </summary>
		/// <param name="drvOrder">戻し処理を行うw2_orderのDatarowview</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void RollbackSerialKey(DataRowView drvOrder, string lastChanged, SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// シリアルキー戻し処理
			//   @引当済 -> 引当リセット（再引き当て可能にする）
			//   @引渡済 -> キャンセル（使用不可にする）
			//------------------------------------------------------
			if ((string)drvOrder[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG] == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON)
			{
				new SerialKeyService().UpdateByCancelOrder(
					(string)drvOrder[Constants.FIELD_ORDER_ORDER_ID],
					lastChanged,
					sqlAccessor);
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		#endregion

		#region +RollbackFixedPurchaseOrderCount 定期購入情報：購入回数(注文基準)戻し処理
		/// <summary>
		/// 定期購入情報：購入回数(注文基準)戻し処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void RollbackFixedPurchaseOrderCount(
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 定期購入OP無効の場合は処理を抜ける
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false) return;

			// 元注文かつ定期購入注文?
			if (order.IsOriginalOrder && order.IsFixedPurchaseOrder)
			{
				if (string.IsNullOrEmpty(order.CombinedOrgOrderIds) == false)
				{
					// 商品同梱あり
					UpdateFixedPurchaseCancelCount(order, lastChanged, updateHistoryAction, accessor);
				}
				else
				{
					// 商品同梱なし
					new FixedPurchaseService().UpdateForCancelOrder(
						order.FixedPurchaseId,
						order.OrderId,
						lastChanged,
						updateHistoryAction,
						accessor);
				}
			}
		}
		#endregion

		#region +UpdateFixedPurchaseShippedCount 定期購入情報：購入回数(出荷基準)、注文情報：定期購入回数(出荷時点)更新
		/// <summary>
		/// (1)定期購入情報：購入回数(出荷基準)更新
		/// (2)注文情報：定期購入回数(出荷時点)更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">処理を実行するトランザクションを持つSqlAccessor</param>
		public static void UpdateFixedPurchaseShippedCount(
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 定期購入OP無効の場合は処理を抜ける
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false) return;

			if (HasOrderCombineFixedPurchase(order))
			{
				// 商品同梱あり出荷基準更新
				UpdateFixedPurchaseShippedCountIncludeCombined(order, lastChanged, updateHistoryAction, accessor);
			}
			else if (order.IsOriginalOrder && order.IsFixedPurchaseOrder)
			{
				// 商品同梱なし出荷基準更新
				// 定期購入情報：購入回数(出荷基準)更新
				var service = new FixedPurchaseService();
				var updatedCount = service.UpdateForShippedOrder(
					order.FixedPurchaseId,
					order.OrderId,
					lastChanged,
					updateHistoryAction,
					accessor);
				// 更新した場合（※既に更新済みの場合は、更新されない=0件）
				if (updatedCount != 0)
				{
					// 注文情報：定期購入回数(出荷時点)更新
					var fixedPurchase = service.Get(order.FixedPurchaseId, accessor);
					var orderService = new OrderService();
					orderService.UpdateFixedPurchaseShippedCount(
						order.OrderId,
						fixedPurchase.ShippedCount,
						lastChanged,
						updateHistoryAction,
						accessor);

					// 注文情報：定期商品購入回数(出荷時点)更新
					orderService.UpdateFixedPerchaseItemShippedCount(
						order.OrderId,
						fixedPurchase.FixedPurchaseId,
						lastChanged,
						updateHistoryAction,
						accessor);
				}
			}
		}

		/// <summary>
		/// Has order combine Fixed Purchase 
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Has order combine Fixed Purchase</returns>
		private static bool HasOrderCombineFixedPurchase(OrderModel order)
		{
			if (string.IsNullOrEmpty(order.CombinedOrgOrderIds)) return false;

			var orderIds = order.CombinedOrgOrderIds.Split(',');
			var orderCombines = new OrderService().GetCombinedOrders(orderIds);

			var result = ((orderCombines != null)
				&& orderCombines.Any(item => string.IsNullOrEmpty(item.FixedPurchaseId) == false));

			return result;
		}
		#endregion

		#region -UpdateFixedPurchaseShippedCountIncludeCombined 注文同梱あり定期購入情報：購入回数(出荷基準)、注文情報：定期購入回数(出荷時点)更新
		/// <summary>
		/// 注文同梱あり定期購入情報：購入回数(出荷基準)、注文情報：定期購入回数(出荷時点)更新
		/// </summary>
		/// <param name="order">定期購入情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private static void UpdateFixedPurchaseShippedCountIncludeCombined(
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var orderIds = order.CombinedOrgOrderIds.Split(',');
			var orderCombines = new OrderService().GetCombinedOrders(orderIds);
			foreach (var orderCombine in orderCombines)
			{
				if (string.IsNullOrEmpty(orderCombine.CombinedOrgOrderIds))
				{
					// 通常注文は対象外(通常注文＋定期注文同梱)
					if (string.IsNullOrEmpty(orderCombine.FixedPurchaseId)) continue;

					// 定期購入情報：購入回数(出荷基準)更新
					var service = new FixedPurchaseService();
					var updatedCount = service.UpdateForShippedOrder(
						orderCombine.FixedPurchaseId,
						order.OrderId,
						lastChanged,
						updateHistoryAction,
						accessor,
						order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP);
					// 更新した場合（※既に更新済みの場合は、更新されない=0件）
					if (updatedCount != 0)
					{
						// 注文情報：定期購入回数(出荷時点)更新
						var fixedPurchase = service.Get(orderCombine.FixedPurchaseId, accessor);
						var orderService = new OrderService();
						orderService.UpdateFixedPurchaseShippedCount(
							order.OrderId,
							fixedPurchase.ShippedCount,
							lastChanged,
							updateHistoryAction,
							accessor);

						// 注文情報：定期商品購入回数(出荷時点)更新
						if (order.FixedPurchaseId == orderCombine.FixedPurchaseId)
						{
							orderService.UpdateFixedPerchaseItemShippedCount(
							order.OrderId,
							fixedPurchase.FixedPurchaseId,
							lastChanged,
							updateHistoryAction,
							accessor);
						}
					}
				}
				else
				{
					UpdateFixedPurchaseShippedCountIncludeCombined(orderCombine, lastChanged, updateHistoryAction, accessor);
				}
			}
		}
		#endregion

		#region +GetOrderItems 注文商品情報取得
		/// <summary>
		/// 注文商品情報取得
		/// </summary>
		/// <param name="orderId">取得対象のOrderID</param>
		/// <param name="shopId">取得対象のShopIDが</param>
		/// <param name="sqlAccessor">処理を実行するトランザクションを持つSqlAccessor</param>
		/// <returns>
		/// 取得したOrder_Itemテーブルのデータ
		/// </returns>
		public static DataView GetOrderItems(string orderId, string shopId, SqlAccessor sqlAccessor)
		{
			using (var sqlStatement = new SqlStatement("Order", "GetOrderItem") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
					{ Constants.FIELD_ORDER_SHOP_ID, shopId }
				};

				return sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		#endregion

		#region +ChkPermitCancelStatus キャンセル可能なステータスかチェック
		/// <summary>
		/// 注文がキャンセル可能なステータスかチェック
		/// </summary>
		/// <param name="orderRow">
		/// 注文情報を持つDataRowView
		/// </param>
		/// <returns>
		/// True：キャンセル可能である
		/// False：キャンセル不可である
		/// </returns>
		/// <remarks>
		/// 注文ステータスが
		/// 「注文済み」、「受注承認」、「在庫引き当て済み」、「出荷手配済み」の場合はTrue,
		/// それ以外はFalseを返却
		/// </remarks>
		private static bool ChkPermitCancelStatus(DataRowView orderRow)
		{
			switch(orderRow[Constants.FIELD_ORDER_ORDER_STATUS].ToString())
			{
				case Constants.FLG_ORDER_ORDER_STATUS_ORDERED:
				case Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED:
				case Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED:
				case Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED:
					return true;
			}

			return false;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		#endregion

		#region +GetOrders 注文の基本情報を抽出、取得
		/// <summary>
		///  注文の基本情報を抽出、取得
		/// </summary>
		/// <param name="createdDate">作成日</param>
		/// <param name="updatedDate">更新日</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="orderId">注文ID</param>
		/// <returns></returns>
		public DataTable GetOrders(PastAbsoluteTimeSpan createdDate,
								   PastAbsoluteTimeSpan updatedDate,
								   string orderStatus,
								   string orderId
								   )
		{
			DataTable dataTable;

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrders") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var parameter = new Hashtable
				{
					{ PARAMETER_ORDER_CREATED_FROM, createdDate != null ? (object)createdDate.BeginTime : null },
					{ PARAMETER_ORDER_CREATED_BEFORE, createdDate != null ? (object)createdDate.EndTime : null },
					{ PARAMETER_ORDER_UPDATED_FROM, updatedDate != null ? (object)updatedDate.BeginTime : null },
					{ PARAMETER_ORDER_UPDATED_BEFORE, updatedDate != null ? (object)updatedDate.EndTime : null },
					{ Constants.FIELD_ORDER_ORDER_STATUS, orderStatus },
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) }
				};

				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		/// <summary>
		///  注文の基本情報を抽出、取得
		/// </summary>
		/// <param name="createdDate">作成日</param>
		/// <param name="updatedDate">更新日</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderPaymentStatus">決済ステータス</param>
		/// <param name="orderExtendedStatusSpecificationWhereSql">注文拡張ステータスWhere</param>
		/// <returns></returns>
		public DataTable GetOrders(PastAbsoluteTimeSpan createdDate,
								   PastAbsoluteTimeSpan updatedDate,
								   string orderStatus,
								   string orderId,
								   string orderPaymentStatus,
								   string orderExtendedStatusSpecificationWhereSql
								   )
		{
			DataTable dataTable;

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrders") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace(
					"@@extended_status_where_statement@@",
					orderExtendedStatusSpecificationWhereSql);

				var parameter = new Hashtable
				{
					{ PARAMETER_ORDER_CREATED_FROM, createdDate != null ? (object)createdDate.BeginTime : null },
					{ PARAMETER_ORDER_CREATED_BEFORE, createdDate != null ? (object)createdDate.EndTime : null },
					{ PARAMETER_ORDER_UPDATED_FROM, updatedDate != null ? (object)updatedDate.BeginTime : null },
					{ PARAMETER_ORDER_UPDATED_BEFORE, updatedDate != null ? (object)updatedDate.EndTime : null },
					{ Constants.FIELD_ORDER_ORDER_STATUS, orderStatus },
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) },
					{ Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, orderPaymentStatus }
				};

				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}

		/// <summary>
		/// Get orders by extend status
		/// </summary>
		/// <returns>Orders</returns>
		public DataTable GetOrdersByExtendStatus()
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrdersByExtendStatus") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				return statement.SelectSingleStatement(accessor).Table;
			}
		}
		#endregion

		#region +GetOrderShippings 注文の配送情報を抽出、取得
		/// <summary>
		/// 注文の配送情報を抽出、取得
		/// </summary>
		/// <param name="createdDate">作成日</param>
		/// <param name="updatedDate">更新日</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="orderId">注文ID</param>
		/// <returns></returns>
		public DataTable GetOrderShippings(
			PastAbsoluteTimeSpan createdDate,
			PastAbsoluteTimeSpan updatedDate,
			string orderStatus,
			string orderId)
		{
			DataTable dataTable;

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrderShippings") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var parameter = new Hashtable
				{
					{ PARAMETER_ORDER_CREATED_FROM, createdDate != null ? (object)createdDate.BeginTime : null },
					{ PARAMETER_ORDER_CREATED_BEFORE, createdDate != null ? (object)createdDate.EndTime : null },
					{ PARAMETER_ORDER_UPDATED_FROM, updatedDate != null ? (object)updatedDate.BeginTime : null },
					{ PARAMETER_ORDER_UPDATED_BEFORE, updatedDate != null ? (object)updatedDate.EndTime : null },
					{ Constants.FIELD_ORDER_ORDER_STATUS, orderStatus },
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) }
				};


				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		/// <summary>
		/// 注文の配送情報を抽出、取得
		/// </summary>
		/// <param name="createdDate">作成日</param>
		/// <param name="updatedDate">更新日</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderPaymentStatus">決済ステータス</param>
		/// <param name="orderExtendedStatusSpecificationWhereSql">注文拡張ステータスWhere</param>
		/// <returns></returns>
		public DataTable GetOrderShippings(
			PastAbsoluteTimeSpan createdDate,
			PastAbsoluteTimeSpan updatedDate,
			string orderStatus,
			string orderId,
			string orderPaymentStatus,
			string orderExtendedStatusSpecificationWhereSql)
		{
			DataTable dataTable;

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrderShippings") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace(
					"@@extended_status_where_statement@@",
					orderExtendedStatusSpecificationWhereSql);

				var parameter = new Hashtable
				{
					{ PARAMETER_ORDER_CREATED_FROM, createdDate != null ? (object)createdDate.BeginTime : null },
					{ PARAMETER_ORDER_CREATED_BEFORE, createdDate != null ? (object)createdDate.EndTime : null },
					{ PARAMETER_ORDER_UPDATED_FROM, updatedDate != null ? (object)updatedDate.BeginTime : null },
					{ PARAMETER_ORDER_UPDATED_BEFORE, updatedDate != null ? (object)updatedDate.EndTime : null },
					{ Constants.FIELD_ORDER_ORDER_STATUS, orderStatus },
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) },
					{ Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, orderPaymentStatus }
				};

				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える

		}
		#endregion

		#region +GetOrderItems 注文の商品情報を抽出、取得
		/// <summary>
		///  注文の商品情報を抽出、取得
		/// </summary>
		/// <param name="createdDate">作成日</param>
		/// <param name="updatedDate">更新日</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="orderId">注文ID</param>
		/// <returns></returns>
		public DataTable GetOrderItems(
			PastAbsoluteTimeSpan createdDate,
			PastAbsoluteTimeSpan updatedDate,
			string orderStatus,
			string orderId)
		{
			DataTable dataTable;

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrderItems") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var parameter = new Hashtable
				{
					{
						PARAMETER_ORDER_CREATED_FROM,
						createdDate != null ? (DateTime?)createdDate.BeginTime : (DateTime?)null
					},
					{
						PARAMETER_ORDER_CREATED_BEFORE,
						createdDate != null ? (DateTime?)createdDate.EndTime : (DateTime?)null
					},
					{
						PARAMETER_ORDER_UPDATED_FROM,
						updatedDate != null ? (DateTime?)updatedDate.BeginTime : (DateTime?)null
					},
					{
						PARAMETER_ORDER_UPDATED_BEFORE,
						updatedDate != null ? (DateTime?)updatedDate.EndTime : (DateTime?)null
					},
					{ Constants.FIELD_ORDER_ORDER_STATUS, orderStatus },
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) }
				};


				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		/// <summary>
		///  注文の商品情報を抽出、取得
		/// </summary>
		/// <param name="createdDate">作成日</param>
		/// <param name="updatedDate">更新日</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderPaymentStatus">決済ステータス</param>
		/// <param name="orderExtendedStatusSpecificationWhereSql">注文拡張ステータスWhere</param>
		/// <param name="returnExchangeKbn">Return Exchange Kbn</param>
		/// <returns></returns>
		public DataTable GetOrderItems(
			PastAbsoluteTimeSpan createdDate,
			PastAbsoluteTimeSpan updatedDate,
			string orderStatus,
			string orderId,
			string orderPaymentStatus,
			string orderExtendedStatusSpecificationWhereSql,
			string returnExchangeKbn)
		{
			DataTable dataTable;

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrderItems") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@extended_status_where_statement@@",
																		orderExtendedStatusSpecificationWhereSql);
				var parameter = new Hashtable
				{
					{
						PARAMETER_ORDER_CREATED_FROM,
						createdDate != null ? (DateTime?)createdDate.BeginTime : (DateTime?)null
					},
					{
						PARAMETER_ORDER_CREATED_BEFORE,
						createdDate != null ? (DateTime?)createdDate.EndTime : (DateTime?)null
					},
					{
						PARAMETER_ORDER_UPDATED_FROM,
						updatedDate != null ? (DateTime?)updatedDate.BeginTime : (DateTime?)null
					},
					{
						PARAMETER_ORDER_UPDATED_BEFORE,
						updatedDate != null ? (DateTime?)updatedDate.EndTime : (DateTime?)null
					},
					{ Constants.FIELD_ORDER_ORDER_STATUS, orderStatus },
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) },
					{ Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, orderPaymentStatus },
					{ Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, returnExchangeKbn }
				};

				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +UpdateOrderExtendStatus 注文拡張ステータス更新
		/// <summary>
		/// 注文拡張ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="number">ステータス項番</param>
		/// <param name="flag"><c>true</c><c>false</c>で設定する。これがあればstatusは無視する</param>
		/// <param name="status">ステータスに格納する文字列</param>
		/// <param name="updateDate">更新日付</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void UpdateOrderExtendStatus(
			string orderId,
			int number,
			bool? flag,
			string status,
			DateTime updateDate,
			UpdateHistoryAction updateHistoryAction)
		{
			new OrderService().UpdateOrderExtendStatus(
				orderId,
				number,
				flag.HasValue ? Convert.ToByte(flag.Value).ToString() : status,
				updateDate,
				LAST_CHANGED_API,
				updateHistoryAction);
		}
		#endregion

		#region +GetEntireOrderById
		/// <summary>
		///  注文情報を抽出、取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns></returns>
		public DataTable GetEntireOrderById(string shopId, string orderId)
		{
			DataTable dataTable;

			var parameter = new Hashtable
			{
				{ Constants.FIELD_ORDER_SHOP_ID, shopId },
				{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) }
			};

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "EntireOrder", "EntireOrder", "GetOrder") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +GetEntireOrderOwnerById
		/// <summary>
		///  注文者情報を抽出、取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns></returns>
		public DataTable GetEntireOrderOwnerById(string orderId)
		{
			DataTable dataTable;

			var parameter = new Hashtable
			{
				{ Constants.FIELD_ORDEROWNER_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) }
			};

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "EntireOrder", "EntireOrder", "GetOrderOwner") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +GetEntireOrderShippingById
		/// <summary>
		///  注文配送先情報を抽出、取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns></returns>
		public DataTable GetEntireOrderShippingById(string orderId)
		{
			DataTable dataTable;

			var parameter = new Hashtable
			{
				{ Constants.FIELD_ORDERSHIPPING_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) }
			};

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "EntireOrder", "EntireOrder", "GetOrderShipping") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +GetEntireOrderItemById
		/// <summary>
		///  注文商品情報を抽出、取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns></returns>
		public DataTable GetEntireOrderItemById(string shopId, string orderId)
		{
			DataTable dataTable;

			var parameter = new Hashtable
			{
				{ Constants.FIELD_ORDERITEM_SHOP_ID, shopId },
				{ Constants.FIELD_ORDERITEM_ORDER_ID, StringUtility.SqlLikeStringSharpEscape(orderId) }
			};

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement(Resources.ResourceManager, "EntireOrder", "EntireOrder", "GetOrderItem") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				dataTable = sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}

			return dataTable;

			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion
	}
}
