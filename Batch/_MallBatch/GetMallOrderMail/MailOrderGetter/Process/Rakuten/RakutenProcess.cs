/*
=========================================================================================================
  Module      : 楽天注文プロセスクラス (RakutenProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common;
using w2.App.Common.Mall.Rakuten;
using w2.App.Common.Mall.RakutenApi;
using w2.App.Common.Order;
using w2.Commerce.MallBatch.MailOrderGetter.Process.Base;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.ProductStock;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Rakuten
{
	/// <summary>
	/// 楽天注文プロセスクラス
	/// </summary>
	class RakutenProcess : BaseProcess
	{
		// 取り込み可能メール件名
		private static readonly string[] m_successSubjects = GetSuccessSubjects();
		// 取り込み不能メール件名
		private static readonly string[] m_errorSubjects =
		{
			"【楽天】注文内容ご確認(携帯)",
			"【楽天】入札を受付けました",
			"【楽天市場】在庫ゼロのお知らせ",
			"【楽天市場】ご注文のキャンセルについて",
			"発送手配完了のお知らせ",
			"発送完了のお知らせ",
			"本日発送のお知らせ",
			"【楽天市場】注文に関するお知らせ",
			"【楽天市場】ご注文の変更について",
			"【楽天】落札手続が完了しました",
			"【楽天市場】最高入札価格で入札されました",
			"【楽天市場】注文内容に関するお知らせ（自動配信メール）",
		};

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RakutenProcess()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="filePath">対象のファイルパス</param>
		/// <param name="mailMessage">メール文章</param>
		/// <param name="mallSetting">モール設定情報</param>
		public RakutenProcess(string shopId, string filePath, MailAnalyze.MailMessage mailMessage, DataRowView mallSetting)
			: this()
		{
			this.ShopId = shopId;
			this.FilePath = filePath;
			this.MailMessage = mailMessage.Body;
			this.OrderId = GetOrderIdFromMail();
			this.MallSetting = mallSetting;
			this.Subject = BaseProcess.GetMailHeaderString(((List<string>)mailMessage.Header.HeaderParts["subject"])[0]);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 取り込み可能メール件名取得
		/// </summary>
		/// <returns>取り込み可能メール件名</returns>
		private static string[] GetSuccessSubjects()
		{
			var successSubjects = new List<string>();
			successSubjects.Add(Constants.RAKUTEN_ORDER_MAIL_SUBJECT);	// 注文の通知メール
			successSubjects.Add(Constants.RAKUTEN_CANCEL_ORDER_MAIL_SUBJECT);// 注文キャンセルの通知メール
			if (Constants.RAKUTEN_RESERVATION_ORDER_MAIL_ENABLED)
			{
				successSubjects.Add(Constants.RAKUTEN_RESERVATION_ORDER_MAIL_SUBJECT);	// 予約購入の受注確定時メール
			}
			if (Constants.RAKUTEN_OVERSEA_ORDER_MAIL_ENABLED)
			{
				successSubjects.Add(Constants.RAKUTEN_GLOBAL_MARKET_ORDER_MAIL_SUBJECT);	// 注文の通知メール(RakutenGlobalMarket)
			}

			return successSubjects.ToArray();
		}

		/// <summary>
		/// 取込可能メールを判断する
		/// </summary>
		/// <param name="subject">メール件名</param>
		/// <param name="filePath">対象のファイルパス</param>
		/// <returns>判定結果</returns>
		/// <remarks>不要メールフォルダへの移動も行う</remarks>
		public static bool CheckMailImportPossible(string subject, string filePath)
		{
			return JudgmentGetMail(subject, m_errorSubjects, m_successSubjects, filePath);
		}

		/// <summary>
		/// 受信した注文メール本文から受注番号を取得
		/// </summary>
		/// <returns>受注番号</returns>
		private string GetOrderIdFromMail()
		{
			// 受注番号が含まれる行を正規表現で取得
			var regexForLine = new Regex(Constants.RAKUTEN_REGULAR_EXPRESSION_FOR_LINE);
			var matchForLine = regexForLine.Match(this.MailMessage);
			if (matchForLine.Success == false)
			{
				throw new MailParsingException("[" + this.FilePath + "] 受注番号が取得できませんでした。");
			}

			// 取得した行から受注番号にあたる部分を正規表現で取得
			var regexForOrderNo = new Regex(Constants.RAKUTEN_REGULAR_EXPRESSION_FOR_ORDER_NO);
			var matchForOrderNo = regexForOrderNo.Match(matchForLine.Value);
			if ((matchForOrderNo.Success == false) || string.IsNullOrEmpty(matchForOrderNo.Value))
			{
				throw new MailParsingException("[" + this.FilePath + "] 受注番号が取得できませんでした。");
			}

			return matchForOrderNo.Value;
		}

		/// <summary>
		/// 楽天注文の登録
		/// </summary>
		/// <param name="accessor">SQLアクセサー</param>
		public void InsertRakutenOrder(SqlAccessor accessor)
		{
			if (StringUtility.ToEmpty(this.MallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN]) != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN) return;

			// 楽天ペイ受注APIから受注情報を取得
			var rakutenOrderApiResponse = new RakutenPayOrderApi(this.MallSetting).GetRakutenOrderInfo(new[] { this.OrderId });

			SendErrorMail(this.OrderId, rakutenOrderApiResponse, this.ShopId);

			// 受注情報を返却
			if (rakutenOrderApiResponse.OrderModelList.Length <= 0)
			{
				throw new RakutenApiCoopException(RakutenApiCoopException.ORDER_GET_ERROR);
			}

			var rakutenOrder = rakutenOrderApiResponse.OrderModelList[0];
			var isRetryImportOrder =
				Constants.RAKUTEN_RETRY_IMPORT_ORDER_PROGRESS.Contains(StringUtility.ToEmpty(rakutenOrder.OrderProgress));

			if ((isRetryImportOrder == false)
				&& ((Constants.RAKUTEN_IMPORT_ORDER_PROGRESS.Contains(StringUtility.ToEmpty(rakutenOrder.OrderProgress)) == false)
					&& (Constants.RAKUTEN_CANCEL_ORDER_PROGRESS.Contains(StringUtility.ToEmpty(rakutenOrder.OrderProgress)) == false)))
			{
				throw new RakutenApiCoopException(RakutenApiCoopException.ORDER_GET_PROGRESS_ERROR);
			}

			var mallId = (string)this.MallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID];
			// 登録済みユーザーチェック
			var userId = CheckRegisteredUser(accessor, mallId, rakutenOrder.OrdererModel.EmailAddress);

			// 新規ユーザー⇒情報登録、登録済みユーザー⇒ユーザー情報更新
			var user = (string.IsNullOrEmpty(userId))
				? InsertUser(mallId, rakutenOrder, accessor)
				: UpdateUser(mallId, userId, rakutenOrder, accessor);
			userId = user.UserId;

			// 注文商品取得
			var rakutenOrderItems = new List<RakutenOrderItem>();
			foreach (var rakutenItem in rakutenOrder.PackageModelList[0].ItemModelList)
			{
				// 注文商品が商品マスタに存在しているかチェック
				var product = GetProduct(this.ShopId, Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? rakutenItem.SkuModelList[0].VariantId : rakutenItem.ItemNumber, this.MallSetting);

				// 商品マスタに商品が存在しない場合、エラーとする
				if (product == null)
				{
					Constants.DISP_ERROR_MESSAGE = "商品情報取得エラー：商品バリエーションID[" + rakutenItem.ItemNumber + "]の商品を取得できませんでした。商品情報をご確認ください。\r\n注文メールは詳細1をご確認ください。";
					throw new RakutenApiCoopException("商品バリエーションID" + rakutenItem.ItemNumber + "の商品情報を取得できませんでした。");
				}

				rakutenOrderItems.Add(new RakutenOrderItem(this.ShopId, product, this.OrderId, rakutenItem));
			}

			var orderModelRakuten = new OrderService().Get(rakutenOrder.OrderNumber, accessor);

			var isCanceledOrExpired = (rakutenOrder.OrderDatetime == null)
				|| (((DateTime)rakutenOrder.OrderDatetime).AddHours(Constants.RAKUTEN_RETRY_IMPORT_PERIOD) < DateTime.Now)
				|| Constants.RAKUTEN_CANCEL_ORDER_PROGRESS.Contains(StringUtility.ToEmpty(rakutenOrder.OrderProgress));

			// リアルタイム累計購入回数更新処理
			var realtimeOrder = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, new UserService().Get(userId, accessor).OrderCountOrderRealtime },
			};
			if (Constants.RAKUTEN_CANCEL_ORDER_PROGRESS.Contains(StringUtility.ToEmpty(rakutenOrder.OrderProgress)) && (this.Subject != Constants.RAKUTEN_ORDER_MAIL_SUBJECT))
			{
				if (orderModelRakuten == null)
				{
					MoveFolderRegisterLogs(
						accessor,
						Constants.PATH_RETRY,
						mallId,
						Constants.RAKUTEN_ORDER_CANCEL_MAIL_READ_MESSAGE,
						this.OrderId);
					return;
				}
				else if (orderModelRakuten.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
				{
					MoveFolderRegisterLogs(
						accessor,
						Constants.PATH_SUCCESS,
						mallId,
						Constants.RAKUTEN_ORDER_CANCEL_MAIL_READ_MESSAGE,
						this.OrderId);
					return;
				}

				CancelOrder(
					accessor,
					rakutenOrder,
					rakutenOrderItems,
					orderModelRakuten,
					realtimeOrder,
					userId,
					mallId,
					Constants.RAKUTEN_ORDER_CANCEL_MAIL_READ_MESSAGE);
				return;
			}

			var stockErrorMessage = string.Empty;
			if (orderModelRakuten != null)
			{
				UpdateStatusOrder(
					rakutenOrder,
					isRetryImportOrder,
					isCanceledOrExpired,
					rakutenOrderItems,
					orderModelRakuten,
					realtimeOrder,
					accessor);
			}
			else
			{
				stockErrorMessage = RegisterOrder(
					rakutenOrder,
					mallId,
					user,
					rakutenOrderItems,
					isCanceledOrExpired,
					accessor);

				if (isCanceledOrExpired == false)
				{
					OrderCommon.UpdateRealTimeOrderCount(
						realtimeOrder,
						Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER,
						accessor);
				}
			}

			// 海外注文かどうかチェック
			CheckOverseaOrder(
				mallId,
				rakutenOrder.OrderNumber,
				IsOverseaAddress(user.Zip, rakutenOrder.OrdererModel.Prefecture),
				IsOverseaAddress(
					string.Format("{0}-{1}", rakutenOrder.PackageModelList[0].SenderModel.ZipCode1, rakutenOrder.PackageModelList[0].SenderModel.ZipCode2),
					rakutenOrder.PackageModelList[0].SenderModel.Prefecture),
				accessor);

			// 更新履歴登録
			new UpdateHistoryService().InsertForOrder(rakutenOrder.OrderNumber, Constants.FLG_LASTCHANGED_BATCH, accessor);
			new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_BATCH, accessor);

			// 在庫エラー処理
			if (string.IsNullOrEmpty(stockErrorMessage) == false)
			{
				// 在庫エラーフォルダへ移動、エラーメール送信
				File.Move(this.FilePath, Path.Combine(Constants.PATH_STOCK_ERROR + @"\", Path.GetFileName(this.FilePath)));
				ErrorMailSender("在庫の更新時にエラーが発生しました。\r\n" + stockErrorMessage + "\r\nファイル名：" + Path.Combine(Constants.PATH_STOCK_ERROR + @"\", Path.GetFileName(this.FilePath)));

				return;
			}

			var isImportExpires = (rakutenOrder.OrderDatetime == null)
				|| (((DateTime)rakutenOrder.OrderDatetime).AddHours(Constants.RAKUTEN_RETRY_IMPORT_PERIOD) < DateTime.Now);

			var pathFile = isRetryImportOrder
				? (isImportExpires
					? Constants.PATH_ERROR
					: Constants.PATH_RETRY)
				: Constants.PATH_SUCCESS;

			MoveFolderRegisterLogs(accessor, pathFile, mallId, Constants.RAKUTEN_ORDER_MAIL_READ_MESSAGE, this.OrderId);
		}

		/// <summary>
		/// 注文キャンセル
		/// </summary>
		/// <param name="accessor">Sqlアクセさ</param>
		/// <param name="rakutenOrder">楽天オーダー</param>
		/// <param name="rakutenOrderItems">オーダーアイテムリスト</param>
		/// <param name="orderRakuten">楽天オーダーモデル</param>
		/// <param name="realtimeOrder">リアルタイム累計購入回数</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="mallWatchCancelMesssage">モール監視用メッセージ</param>
		private void CancelOrder(
			SqlAccessor accessor,
			RakutenApiOrder rakutenOrder,
			List<RakutenOrderItem> rakutenOrderItems,
			OrderModel orderRakuten,
			Hashtable realtimeOrder,
			string userId,
			string mallId,
			string mallWatchCancelMesssage)
		{
			UpdateStatusOrder(rakutenOrder, true, true, rakutenOrderItems, orderRakuten, realtimeOrder, accessor);

			// 更新履歴登録
			new UpdateHistoryService().InsertForOrder(rakutenOrder.OrderNumber, Constants.FLG_LASTCHANGED_BATCH, accessor);
			new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_BATCH, accessor);

			MoveFolderRegisterLogs(
				accessor,
				Constants.PATH_SUCCESS,
				mallId,
				mallWatchCancelMesssage,
				this.OrderId);
		}

		/// <summary>
		/// ファイル移動・ログ登録
		/// </summary>
		/// <param name="accessor">Sqlアクセさ</param>
		/// <param name="pathFile">移動先パス</param>
		/// <param name="mallId">モールID</param>
		/// <param name="mallWatchMesssage">モール監視用メッセージ</param>
		/// <param name="rakutenOrder">Rakutenオーダー</param>
		private void MoveFolderRegisterLogs(
			SqlAccessor accessor,
			string pathFile,
			string mallId,
			string mallWatchMesssage,
			string rakutenOrder)
		{
			// 指定したフォルダに移動
			var destFilePath = Path.Combine(pathFile + @"\", Path.GetFileName(this.FilePath));
			if (this.FilePath == destFilePath) return;
			File.Move(this.FilePath, destFilePath);
			if(this.FilePath.Contains(Constants.PATH_RETRY)) return;

				// モール監視ログ登録（注文メール取込成功）
				Program.MallWatchingLogManager.Insert(
					accessor,
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
					mallId,
					(pathFile == Constants.PATH_ERROR)
						? Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR
						: Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					string.Format("{0}\r\n注文ID[{1}]", mallWatchMesssage, rakutenOrder),
					this.MailMessage);
		}

		/// <summary>
		/// 楽天注文情報からユーザー登録
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>ユーザーモデル</returns>
		private UserModel InsertUser(string mallId, RakutenApiOrder rakutenOrder, SqlAccessor accessor)
		{
			// 楽天注文情報からユーザーモデル作成
			var user = CreateUserModel(rakutenOrder, mallId, GetNewUserId());

			user.LoginId = "";
			user.Password = "";
			user.MailFlg = Constants.FLG_USER_MAILFLG_UNKNOWN;
			user.MemberRankId = "";
			user.RecommendUid = "";
			user.RemoteAddr = "";
			user.CompanyName = "";
			user.CompanyPostName = "";

			// ユーザー登録処理
			FileLogger.WriteInfo("[SQL START]ユーザー登録");
			var userService = new UserService();
			var result = userService.InsertWithUserExtend(
				user,
				App.Common.Constants.FLG_LASTCHANGED_BATCH,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			if (result == false) throw new ApplicationException("ユーザー登録に失敗しました。");
			FileLogger.WriteInfo("[SQL END]ユーザー登録");

			return user;
		}

		/// <summary>
		/// 楽天注文情報からユーザー更新
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>ユーザーモデル</returns>
		private UserModel UpdateUser(string mallId, string userId, RakutenApiOrder rakutenOrder, SqlAccessor accessor)
		{
			// 楽天注文情報からユーザーモデル作成
			var user = CreateUserModel(rakutenOrder, mallId, userId);

			// ユーザー更新処理
			var registerResult = new UserService().UpdateWithUserExtend(user, UpdateHistoryAction.DoNotInsert, accessor);
			if (registerResult == false) throw new RakutenApiCoopException("注文者のユーザ情報更新に失敗しました。");

			return user;
		}

		/// <summary>
		/// 楽天注文情報から注文登録
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="rakutenOrder">楽天受注情報</param>
		/// <param name="rakutenOrderItems">楽天注文商品情報</param>
		/// <param name="isCanceledOrExpired">キャンセルまたはインポート期限切れ</param>
		/// <param name="accessor">SQLアクセサー</param>
		private void InsertOrder(string userId, RakutenApiOrder rakutenOrder, List<RakutenOrderItem> rakutenOrderItems, bool isCanceledOrExpired, SqlAccessor accessor)
		{
			// 楽天注文情報から注文モデル作成
			var order = new RakutenOrder(this.ShopId, rakutenOrder, rakutenOrderItems);

			order.OrderStatus = isCanceledOrExpired
				? Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED
				: Constants.RAKUTEN_RETRY_IMPORT_ORDER_PROGRESS.Contains(StringUtility.ToEmpty(rakutenOrder.OrderProgress))
					? Constants.FLG_ORDER_ORDER_STATUS_TEMP
					: Constants.FLG_ORDER_ORDER_STATUS_ORDERED;

			// リアルタイム累計購入回数取得
			var user = new UserService().Get(userId, accessor);
			var orderCount = ((user == null) ? 0 : user.OrderCountOrderRealtime);

			FileLogger.WriteInfo("[SQL START]w2_Order");
			using (var sqlStatement = new SqlStatement("Order", "InsertOrder"))
			{
				var input = new Hashtable()
				{
					{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
					{ Constants.FIELD_ORDER_ORDER_GROUP_ID, "" },
					{ Constants.FIELD_ORDER_ORDER_NO, "" },
					{ Constants.FIELD_ORDER_USER_ID, userId },
					{ Constants.FIELD_ORDER_SHOP_ID, this.ShopId },
					{ Constants.FIELD_ORDER_ORDER_KBN, order.OrderKbn },
					{ Constants.FIELD_ORDER_MALL_ID, (string)this.MallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] },
					{ Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, order.OrderPaymentKbn },
					{ Constants.FIELD_ORDER_ORDER_DATE, order.OrderDate },
					{ Constants.FIELD_ORDER_ORDER_STATUS, order.OrderStatus },
					{ Constants.FIELD_ORDER_ORDER_ITEM_COUNT, order.OrderItemCount },
					{ Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT, order.OrderProductCount },
					{ Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, order.OrderPriceSubtotal },
					{ Constants.FIELD_ORDER_ORDER_PRICE_PACK, new decimal(0) },
					{ Constants.FIELD_ORDER_ORDER_PRICE_TAX, order.OrderPriceTax },
					{ Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX, order.OrderPriceSubtotalTax },
					{ Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING, order.OrderPriceShipping },
					{ Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE, order.OrderPriceExchange },
					{ Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, order.OrderPriceRegulation },
					{ Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, order.OrderPriceTotal },
					{ Constants.FIELD_ORDER_CARD_KBN, "" },
					{ Constants.FIELD_ORDER_CARD_TRAN_ID, "" },
					{ Constants.FIELD_ORDER_SHIPPING_ID, Constants.MALL_DEFAULT_SHIPPING_ID },
					{ Constants.FIELD_ORDER_SETTLEMENT_AMOUNT, order.OrderPriceTotal },
					{ Constants.FIELD_ORDER_LAST_BILLED_AMOUNT, order.OrderPriceTotal },
					{ Constants.FIELD_ORDER_SETTLEMENT_CURRENCY, order.SettlementCurrency },
					{ Constants.FIELD_ORDER_CARD_INSTRUMENTS, order.CardInstruments },
					{ Constants.FIELD_ORDER_CAREER_ID, "" },
					{ Constants.FIELD_ORDER_MEMO, order.Memo },
					{ Constants.FIELD_ORDER_WRAPPING_MEMO, "" },
					{ Constants.FIELD_ORDER_PAYMENT_MEMO, order.PaymentMemo },
					{ Constants.FIELD_ORDER_RELATION_MEMO, order.RelationMemo },
					{ Constants.FIELD_ORDER_REGULATION_MEMO, order.RegulationMemo },
					{ Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG, Constants.FLG_PRODUCT_TAX_INCLUDED_PRETAX },
					{ Constants.FIELD_ORDER_ORDER_TAX_RATE, "0" },
					{ Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE, Constants.TAX_ROUNDTYPE },
					{ Constants.FIELD_ORDER_SHIPPING_TAX_RATE, order.ShippingTaxRate },
					{ Constants.FIELD_ORDER_PAYMENT_TAX_RATE, order.PaymentTaxRate },
					{ Constants.FIELD_ORDER_ORDER_COUNT_ORDER, orderCount + 1 },
				};

				input[Constants.FIELD_ORDER_ORDER_CANCEL_DATE]
					= (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
						? DateTime.Now
						: (DateTime?)null;

				var inserted = sqlStatement.ExecStatement(accessor, input);
				if (inserted <= 0)
				{
					throw new ApplicationException("注文情報の登録に失敗しました。");
				}
			}

			// 翌日配送「あす楽」を希望の注文の場合、注文拡張ステータスを更新
			if (rakutenOrder.AsurakuFlag.ToString() == "1")
			{
				if (String.IsNullOrEmpty(Constants.RAKUTEN_SHIPPING_TOMORROW_ORDER_EXTEND_STATUS_FIELD) == false)
				{
					var extendStatusNo = int.Parse(Constants.RAKUTEN_SHIPPING_TOMORROW_ORDER_EXTEND_STATUS_FIELD.Replace(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME, ""));
					new OrderService().UpdateOrderExtendStatus(
						rakutenOrder.OrderNumber,
						extendStatusNo,
						Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
						DateTime.Now,
						Constants.BATCH_USER,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}
			}
			FileLogger.WriteInfo("[SQL END]w2_Order");

			// 税率毎価格情報登録
			InsertOrderPriceByTaxRate(order, accessor);
		}

		/// <summary>
		/// 楽天APIレスポンスから税率毎価格情報を登録
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private static void InsertOrderPriceByTaxRate(RakutenOrder order, SqlAccessor sqlAccessor)
		{
			FileLogger.WriteInfo("[SQL START]楽天APIレスポンスから税率情報の登録");
			var orderPriceByTaxRateService = new OrderPriceByTaxRateService();
			foreach (var orderOrderPriceByTaxRate in order.OrderPriceByTaxRates)
			{
				orderOrderPriceByTaxRate.DateCreated = DateTime.Now;
				orderOrderPriceByTaxRate.DateChanged = DateTime.Now;
				orderPriceByTaxRateService.Insert(orderOrderPriceByTaxRate, sqlAccessor);
			}
			FileLogger.WriteInfo("[SQL END]楽天APIレスポンスから税率情報の登録");
		}

		/// <summary>
		/// 楽天APIレスポンスから注文者情報を登録
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="user">注文者のユーザモデル</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private static void InsertOrderOwner(string orderId, UserModel user, SqlAccessor sqlAccessor)
		{
			// 注文者情報を更新
			FileLogger.WriteInfo("[SQL START]楽天APIレスポンスから注文者情報の登録");
			using (var sqlStatement = new SqlStatement("Order", "InsertOrderOwner"))
			{
				var input = new Hashtable()
				{
					{ Constants.FIELD_ORDEROWNER_ORDER_ID, orderId },
					{ Constants.FIELD_ORDEROWNER_OWNER_KBN, user.UserKbn },
					{ Constants.FIELD_ORDEROWNER_OWNER_NAME, user.Name },
					{ Constants.FIELD_ORDEROWNER_OWNER_NAME1, user.Name1 },
					{ Constants.FIELD_ORDEROWNER_OWNER_NAME2, user.Name2 },
					{ Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA, user.NameKana },
					{ Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1, user.NameKana1 },
					{ Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2, user.NameKana2 },
					{ Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, user.MailAddr },
					{ Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2, user.MailAddr2 },
					{ Constants.FIELD_ORDEROWNER_OWNER_ZIP, user.Zip },
					{ Constants.FIELD_ORDEROWNER_OWNER_ADDR1, user.Addr1 },
					{ Constants.FIELD_ORDEROWNER_OWNER_ADDR2, user.Addr2 },
					{ Constants.FIELD_ORDEROWNER_OWNER_ADDR3, user.Addr3 },
					{ Constants.FIELD_ORDEROWNER_OWNER_ADDR4, user.Addr4 },
					{ Constants.FIELD_ORDEROWNER_OWNER_TEL1, user.Tel1 },
					{ Constants.FIELD_ORDEROWNER_OWNER_SEX, user.Sex },
					{ Constants.FIELD_ORDEROWNER_OWNER_BIRTH, user.Birth },
				};

				var inserted = sqlStatement.ExecStatement(sqlAccessor, input);
				if (inserted <= 0) throw new RakutenApiCoopException("楽天APIレスポンスから注文者情報の登録に失敗しました。");
			}
			FileLogger.WriteInfo("[SQL END]楽天APIレスポンスから注文者情報の登録");
		}

		/// <summary>
		/// 楽天APIレスポンスから配送先情報を登録
		/// </summary>
		/// <param name="rakutenOrder">楽天APIレスポンスから注文情報</param>
		/// <param name="owner">注文者情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void InsertOrderShipping(RakutenApiOrder rakutenOrder, UserModel owner, SqlAccessor accessor)
		{
			// 配送先が２つ以上有る場合、エラーになる
			if (rakutenOrder.PackageModelList.Length > 1)
			{
				Constants.DISP_ERROR_KBN = Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR;
				Constants.DISP_ERROR_MESSAGE = string.Format("楽天受注API連携：注文[{0}]は複数の送付先を持っていますが、w2Commerceには複数配送先を対応していません。", rakutenOrder.OrderNumber);
				throw new RakutenApiCoopException("複数の送付先を含むメールです。");
			}

			// 楽天朱蒙情報から配送先モデル作成
			var orderShipping = new RakutenOrderShipping(rakutenOrder, owner, this.ShopId);

			// 配送先情報を更新
			FileLogger.WriteInfo("[SQL START]配送先情報の登録");
			using (var statement = new SqlStatement("Order", "InsertOrderShipping"))
			{
				var input = new Hashtable()
				{
					{ Constants.FIELD_ORDERSHIPPING_ORDER_ID, rakutenOrder.OrderNumber },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, orderShipping.ShippingName },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, orderShipping.ShippingName1 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, orderShipping.ShippingName2 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA, orderShipping.ShippingNameKana },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1, orderShipping.ShippingNameKana1 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2, orderShipping.ShippingNameKana2 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, orderShipping.ShippingZip },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, orderShipping.ShippingAddr1 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, orderShipping.ShippingAddr2 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, orderShipping.ShippingAddr3 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, orderShipping.ShippingAddr4 },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, orderShipping.ShippingTel1 },
					{ Constants.FIELD_ORDERSHIPPING_EXTERNAL_SHIPPING_COOPERATION_ID, orderShipping.ExternalShippingCooperationId },
					{ Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID, orderShipping.DeliveryCompanyId },
					{ Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG, orderShipping.AnotherShippingFlg },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, orderShipping.ShippingTime },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, orderShipping.ShippingDate },
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, orderShipping.ShippingMethod },
					{ Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, orderShipping.ScheduledShippingDate },
				};

				var inserted = statement.ExecStatement(accessor, input);
				if (inserted <= 0) throw new RakutenApiCoopException("配送先情報の登録に失敗しました。");
			}

			// 楽天側配送時間帯取込の注文メモを更新
			if (orderShipping.ShippingTime == "999")
			{
				var memoTmp = new StringBuilder();
				var order = new RakutenOrder(rakutenOrder);
				memoTmp = memoTmp.Append(order.Memo).Append("[配送時間帯取込エラー]\r\nマッピング文言を更新してください。");

				new OrderService().Modify(
					order.OrderId,
					model =>
					{
						model.OrderId = order.OrderId;
						model.Memo = memoTmp.ToString();
					},
					UpdateHistoryAction.Insert,
					accessor);
			}

			FileLogger.WriteInfo("[SQL END]配送先情報の登録");
		}

		/// <summary>
		/// 楽天APIレスポンスから注文商品情報を登録
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="rakutenOrderItems">楽天注文商品情報</param>
		/// <param name="isCanceledOrExpired">キャンセルまたはインポート期限切れ</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>在庫情報更新エラーメッセージ</returns>
		private string InsertOrderItem(string mallId, RakutenApiOrder rakutenOrder, List<RakutenOrderItem> rakutenOrderItems, bool isCanceledOrExpired, SqlAccessor accessor)
		{
			var stockErrorMessage = new StringBuilder();
			var orderItemCount = 1;
			var stockError = false;

			FileLogger.WriteInfo("[SQL START]注文商品の登録");
			foreach (var orderItem in rakutenOrderItems)
			{
				stockErrorMessage.Remove(0, stockErrorMessage.Length);

				using (var sqlStatement = new SqlStatement("Order", "InsertOrderItem"))
				{
					var input = new Hashtable()
					{
						{ Constants.FIELD_ORDERITEM_SHOP_ID, this.ShopId },
						{ Constants.FIELD_ORDERITEM_ORDER_ID, rakutenOrder.OrderNumber },
						{ Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, orderItemCount },
						{ Constants.FIELD_ORDERITEM_PRODUCT_ID, orderItem.ProductId },
						{ Constants.FIELD_ORDERITEM_VARIATION_ID, orderItem.VariationId },
						{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, orderItem.ProductName },
						{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE, orderItem.ProductPrice },
						{ Constants.FIELD_ORDERITEM_PRODUCT_POINT, new decimal(0) },
						{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG, orderItem.ProductTaxIncludedFlg },
						{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, orderItem.ProductTaxRate },
						{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE, Constants.TAX_ROUNDTYPE },
						{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX, orderItem.ProductPricePretax },
						{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP, new decimal(0) },
						{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, orderItem.ItemQuantity },
						{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE, orderItem.ItemQuantity },
						{ Constants.FIELD_ORDERITEM_ITEM_PRICE, orderItem.ProductPrice * orderItem.ItemQuantity },
						{ Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE, orderItem.ProductPrice * orderItem.ItemQuantity },
						{ Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX, orderItem.ProductPriceTax * orderItem.ItemQuantity },
					};

					var inserted = sqlStatement.ExecStatement(accessor, input);
					if (inserted <= 0)
					{
						throw new ApplicationException("注文商品情報の登録に失敗しました。");
					}
				}
				orderItemCount++;
				FileLogger.WriteInfo("[SQL END]w2_OrderItems");

				// 在庫情報更新
				FileLogger.WriteInfo("[SQL START]2_ProductStock");
				var dvProductSrockManagementKbn = new DataView();
				using (var sqlStatement = new SqlStatement("Product", "GetProductSrockManagementKbn"))
				{
					var input = new Hashtable()
					{
						{ Constants.FIELD_PRODUCT_SHOP_ID, this.ShopId },
						{ Constants.FIELD_PRODUCT_PRODUCT_ID, orderItem.ProductId },
					};

					dvProductSrockManagementKbn = sqlStatement.SelectSingleStatement(accessor, input);
				}

				// 商品マスタに商品が存在しない場合、在庫更新エラーとする
				if (dvProductSrockManagementKbn.Count == 0)
				{
					FileLogger.WriteError("商品番号" + orderItem.ProductId + "の商品情報を取得できませんでした。");
					stockError = true;
					stockErrorMessage
						.Append("注文ID[")
						.Append(rakutenOrder.OrderNumber)
						.Append("] 商品バリエーションID[")
						.Append(StringUtility.ToEmpty(orderItem.VariationId))
						.Append("]の商品情報をご確認ください。");

					// モール監視ログ登録（在庫更新エラー）
					Program.MallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
						mallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
						"在庫更新エラー：商品バリエーションID[" + StringUtility.ToEmpty(orderItem.VariationId) + "]の商品を取得できませんでした。商品をご登録ください。\r\n注文メールは詳細1をご確認ください。\r\n注文ID[" + rakutenOrder.OrderNumber + "]", this.MailMessage);
				}
				// 該当商品の在庫管理方法が「在庫管理しない」以外の場合、在庫更新を行う（Not Transaction）
				else if ((string)dvProductSrockManagementKbn[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED
					&& (isCanceledOrExpired == false))
				{
					using (var sqlStatement = new SqlStatement("Product", "UpdateProductStock"))
					using (var sqlStatement2 = new SqlStatement("Product", "InsertProductStockHistory"))
					{
						//------------------------------------------------------
						// 商品在庫マスタ更新
						//------------------------------------------------------
						var input = new Hashtable()
						{
							{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, this.ShopId },
							{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, orderItem.ProductId },
							{ Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, orderItem.VariationId },
							{ Constants.FIELD_PRODUCTSTOCK_STOCK, orderItem.ItemQuantity },
						};

						var updated = sqlStatement.ExecStatement(accessor, input);
						if (updated <= 0)
						{
							FileLogger.WriteError("商品番号" + orderItem.ProductId + "の在庫を更新できませんでした。商品番号が誤っているか、在庫数が取得できなかった可能性があります。");
							stockError = true;
						}

						//------------------------------------------------------
						// 商品在庫履歴マスタ更新
						//------------------------------------------------------
						Hashtable input2 = new Hashtable()
						{
							{ Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, this.ShopId },
							{ Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, rakutenOrder.OrderNumber },
							{ Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, orderItem.ProductId },
							{ Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, orderItem.VariationId },
							{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, orderItem.ItemQuantity * -1 },
						};

						var updated2 = sqlStatement2.ExecStatement(accessor, input2);
						if (updated2 <= 0)
						{
							FileLogger.WriteError("商品番号" + orderItem.ProductId + "の在庫履歴作成に失敗しました。商品番号が誤っているか、在庫数が取得できなかった可能性があります。");
							stockError = true;
						}

						//------------------------------------------------------
						// エラーメール文言
						//------------------------------------------------------
						if (stockError)
						{
							stockErrorMessage.Append("注文ID[").Append(rakutenOrder.OrderNumber).Append("] 商品バリエーションID[").Append(StringUtility.ToEmpty(orderItem.VariationId)).Append("]の在庫情報をご確認ください。");

							// モール監視ログ登録（在庫更新エラー）
							Program.MallWatchingLogManager.Insert(
								accessor,
								Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
								mallId,
								Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
								"在庫更新エラー：商品バリエーションID[" + orderItem.VariationId + "]が間違っているか、在庫数が取得出来なかった可能性があります。在庫情報をご確認ください。\r\n注文メールは詳細1をご確認ください。\r\n注文ID[" + rakutenOrder.OrderNumber + "]", this.MailMessage);
						}
					}
				}
				FileLogger.WriteInfo("[SQL END]2_ProductStock");
			}
			FileLogger.WriteInfo("[SQL END]注文商品の登録");

			return stockErrorMessage.ToString();
		}

		/// <summary>
		/// 楽天注文情報からユーザーモデル作成
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="mallId">モールID</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーモデル</returns>
		private static UserModel CreateUserModel(RakutenApiOrder rakutenOrder, string mallId, string userId)
		{
			var rakutenUser = new RakutenUser(rakutenOrder.OrdererModel, rakutenOrder.CarrierCode);
			var userService = new UserService().Get(userId);

			var user = new UserModel()
			{
				UserId = userId,
				UserKbn = rakutenUser.UserKbn,
				MallId = mallId,
				Name = rakutenUser.Name,
				Name1 = rakutenUser.Name1,
				Name2 = rakutenUser.Name2,
				NameKana = rakutenUser.NameKana,
				NameKana1 = rakutenUser.NameKana1,
				NameKana2 = rakutenUser.NameKana2,
				MailAddr = rakutenUser.MailAddr,
				MailAddr2 = rakutenUser.MailAddr2,
				Zip = rakutenUser.Zip,
				Zip1 = rakutenUser.Zip1,
				Zip2 = rakutenUser.Zip2,
				Addr = rakutenUser.Addr,
				Addr1 = rakutenUser.Addr1,
				Addr2 = rakutenUser.Addr2,
				Addr3 = rakutenUser.Addr3,
				Addr4 = rakutenUser.Addr4,
				Tel1 = rakutenUser.Tel1,
				Tel1_1 = rakutenUser.Tel1_1,
				Tel1_2 = rakutenUser.Tel1_2,
				Tel1_3 = rakutenUser.Tel1_3,
				Sex = rakutenUser.Sex,
				Birth = rakutenUser.Birth,
				BirthYear = rakutenUser.BirthYear,
				BirthMonth = rakutenUser.BirthMonth,
				BirthDay = rakutenUser.BirthDay,
				LastChanged = Constants.BATCH_USER,
				OrderCountOrderRealtime = (userService != null) ? userService.OrderCountOrderRealtime : 0,
			};

			return user;
		}

		/// <summary>
		/// 海外注文のチェック
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="isOverseaOrderOwnerAddress">注文者は海外住所かどうか</param>
		/// <param name="isOverseaOrderShippingAddress">配送先は海外住所かどうか</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private static void CheckOverseaOrder(string mallId, string orderId, bool isOverseaOrderOwnerAddress, bool isOverseaOrderShippingAddress, SqlAccessor sqlAccessor)
		{
			if (isOverseaOrderOwnerAddress || isOverseaOrderShippingAddress)
			{
				if (Constants.RAKUTEN_OVERSEA_ORDER_MAIL_ENABLED
					&& (String.IsNullOrEmpty(Constants.RAKUTEN_OVERSEAS_ORDER_EXTEND_STATUS_FIELD) == false))
				{
					// 海外注文取り込み利用 AND 海外住所の場合
					// 注文拡張ステータスを更新
					var extendStatusNo = int.Parse(Regex.Replace(Constants.RAKUTEN_OVERSEAS_ORDER_EXTEND_STATUS_FIELD, @"[^\d]", ""));
					FileLogger.WriteInfo("[SQL START]海外注文の更新");
					var updated =
						new OrderService().UpdateOrderExtendStatus(
							orderId,
							extendStatusNo,
							Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
							DateTime.Now,
							Constants.BATCH_USER,
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor);
					if (updated <= 0) throw new ApplicationException("海外注文の更新に失敗しました。");
					FileLogger.WriteInfo("[SQL END]海外注文の更新");
				}
				else
				{
					// 海外注文非対応の場合、警告モール連携監視ログを登録
					Program.MallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_RAKUTEN_API_ORDER_COOP,
						mallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
						string.Format("楽天受注API連携：注文[{0}]は海外注文の可能性があります。\r\n運用で海外注文が非対応の場合、個別でご対応してください。", orderId),
						StringUtility.ToEmpty(Constants.DISP_ERROR_MESSAGE));
				}
			}
		}

		/// <summary>
		/// 海外住所かどうかチェック
		/// </summary>
		/// <param name="zip">郵便番号</param>
		/// <param name="prefecture">都道府県</param>
		/// <returns>true：海外住所、false：日本国内の住所</returns>
		private static bool IsOverseaAddress(string zip, string prefecture)
		{
			// 海外住所のチェック
			if ((zip == "000-0000") || (Properties.Resources.sKen.IndexOf(prefecture) == -1))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 楽天ペイ受注APIレスポンスのユニットエラーチェック
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="response">楽天ペイ受注APIレスポンス</param>
		/// <param name="shopId">ショップID</param>
		private static void SendErrorMail(string orderId, RakutenApiOrderResponse response, string shopId)
		{
			if ((response == null) || (response.MessageModelList == null) || (response.MessageModelList.Length == 0)) return;
			if (response.MessageModelList.Any(message => (message.MessageType == Constants.RAKUTEN_PAY_API_MESSAGE_TYPE_ERROR)) == false) return;

			var mailMessage = new StringBuilder();
			response.MessageModelList
				.Where(message => (message.MessageType == Constants.RAKUTEN_PAY_API_MESSAGE_TYPE_ERROR))
				.ToList()
				.ForEach(
					message =>
						mailMessage.AppendLine(
							string.Format(
								"エラー{0}（{1}）により注文番号[{2}]の注文情報取得に失敗しました。",
								message.MessageCode,
								message.Message,
								orderId)));
			SendErrorMail(mailMessage, shopId);
		}

		/// <summary>
		/// エラーメール送信
		/// </summary>
		/// <param name="mailMessage">送信メール</param>
		/// <param name="shopId">ショップID</param>
		private static void SendErrorMail(StringBuilder mailMessage, string shopId)
		{
			if (mailMessage.Length == 0) return;

			// モール連携監視用メッセージ
			Constants.DISP_ERROR_KBN = Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING;
			Constants.DISP_ERROR_MESSAGE = mailMessage.ToString();

			try
			{
				var input = new Hashtable();
				input.Add("message", mailMessage.ToString());
				var mailSendUtility = new MailSendUtility(shopId, Constants.CONST_MAIL_ID_RAKUTEN_COOP_ERROR, string.Empty, input, true, Constants.MailSendMethod.Auto);
				mailSendUtility.SendMail();
			}
			catch (Exception ex)
			{
				// DBでメールテンプレート取得できないなどの場合、設定値の宛先などでメールを送信
				AppLogger.WriteError(ex);
				using (var sender = new MailSendUtility(Constants.MailSendMethod.Auto))
				{
					// 送信元、送信先設定
					sender.SetSubject(Constants.MAIL_SUBJECTHEAD);
					sender.SetFrom(Constants.MAIL_FROM.Address);
					Constants.MAIL_TO_LIST.ForEach(mail => sender.AddTo(mail.Address));
					Constants.MAIL_CC_LIST.ForEach(mail => sender.AddCC(mail.Address));
					Constants.MAIL_BCC_LIST.ForEach(mail => sender.AddBcc(mail.Address));

					// 本文設定
					sender.SetBody(mailMessage.ToString());

					if (sender.SendMail() == false)
					{
						// 送信エラー時はログ書き込み
						AppLogger.WriteError("メール送信できませんでした。", sender.MailSendException);
					}
				}
			}
		}

		/// <summary>
		/// Update Status Order
		/// </summary>
		/// <param name="rakutenOrder">Rakuten Order</param>
		/// <param name="isRetryImportOrder">Is Retry Import Order</param>
		/// <param name="isCanceledOrExpired">キャンセルまたはインポート期限切れ</param>
		/// <param name="rakutenOrderItems">Rakuten Order Items</param>
		/// <param name="orderRakuten">Order Rakuten</param>
		/// <param name="realtimeOrder">Realtime Order</param>
		/// <param name="accessor">Accessor</param>
		private void UpdateStatusOrder(
			RakutenApiOrder rakutenOrder,
			bool isRetryImportOrder,
			bool isCanceledOrExpired,
			List<RakutenOrderItem> rakutenOrderItems,
			OrderModel orderRakuten,
			Hashtable realtimeOrder,
			SqlAccessor accessor)
		{
			if (isCanceledOrExpired)
			{
				new OrderService().Modify(
					orderRakuten.OrderId,
					(model) =>
					{
						model.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED;
						model.DateChanged = DateTime.Now;
						model.LastChanged = Constants.BATCH_USER;
						model.OrderCancelDate = DateTime.Now;
					},
					UpdateHistoryAction.DoNotInsert,
					accessor);

				new ProductStockService().UpdateProductStockCancel(
					rakutenOrderItems.ToArray(),
					Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL,
					Constants.BATCH_USER,
					accessor);

				OrderCommon.UpdateRealTimeOrderCount(
					realtimeOrder,
					Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_CANCEL,
					accessor);
				return;
			}

			if (isRetryImportOrder) return;

			if (Constants.RAKUTEN_IMPORT_ORDER_PROGRESS.Contains(StringUtility.ToEmpty(rakutenOrder.OrderProgress)))
			{
				new OrderService().UpdateOrderStatus(
					orderRakuten.OrderId,
					Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
					DateTime.Now, Constants.BATCH_USER,
					UpdateHistoryAction.Insert,
					accessor);
			}
		}

		/// <summary>
		/// Register Order
		/// </summary>
		/// <param name="rakutenOrder">Rakuten Order</param>
		/// <param name="mallId">Mall Id</param>
		/// <param name="user">User</param>
		/// <param name="rakutenOrderItems">Rakuten Order Items</param>
		/// <param name="isCanceledOrExpired">キャンセルまたはインポート期限切れ</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Error Message</returns>
		private string RegisterOrder(
			RakutenApiOrder rakutenOrder,
			string mallId,
			UserModel user,
			List<RakutenOrderItem> rakutenOrderItems,
			bool isCanceledOrExpired,
			SqlAccessor accessor)
		{
			// 注文情報登録
			InsertOrder(user.UserId, rakutenOrder, rakutenOrderItems, isCanceledOrExpired, accessor);

			// 注文者情報登録
			InsertOrderOwner(rakutenOrder.OrderNumber, user, accessor);

			// 配送先情報登録
			InsertOrderShipping(rakutenOrder, user, accessor);

			// 注文商品情報登録
			var stockErrorMessage = InsertOrderItem(mallId, rakutenOrder, rakutenOrderItems, isCanceledOrExpired, accessor);
			return stockErrorMessage;
		}

		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>対象のファイルパス</summary>
		public string FilePath { get; set; }
		/// <summary>メール文章</summary>
		public string MailMessage { get; set; }
		/// <summary>メール件名"</summary>
		public string Subject { get; set; }
		/// <summary>受注番号</summary>
		public string OrderId { get; set; }
		/// <summary>モール設定情報</summary>
		public DataRowView MallSetting { get; set; }
		#endregion
	}
}
