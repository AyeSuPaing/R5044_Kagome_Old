/*
=========================================================================================================
  Module      : 楽天注文プロセスクラス (RakutenProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
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
using w2.App.Common.Order;
using w2.Commerce.MallBatch.MailOrderGetter.Process.Base;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
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
		public RakutenProcess(string shopId, string filePath, string mailMessage, DataRowView mallSetting)
			: this()
		{
			this.ShopId = shopId;
			this.FilePath = filePath;
			this.MailMessage = mailMessage;
			this.OrderId = GetOrderIdFromMail();
			this.MallSetting = mallSetting;
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

			// 楽天受注APIから受注情報を取得
			var rakutenOrderApiResponse = new RakutenOrderApi(this.MallSetting).GetRakutenOrderInfo(new[] { this.OrderId });

			var mallId = (string)this.MallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID];
			SendErrorMail(this.OrderId, rakutenOrderApiResponse, this.ShopId);

			// 受注情報を返却
			if ((rakutenOrderApiResponse.errorCode != Constants.RAKUTEN_API_RESULT_ERROR_CODE_NORMAL)
				|| (rakutenOrderApiResponse.unitError != null))
			{
				throw new RakutenApiCoopException(RakutenApiCoopException.ORDER_GET_ERROR);
			}

			var rakutenOrder = rakutenOrderApiResponse.orderModel[0];

			// 登録済みユーザーチェック
			var userId = CheckRegisteredUser(accessor, mallId, rakutenOrder.ordererModel.emailAddress);

			// 新規ユーザー⇒情報登録、登録済みユーザー⇒ユーザー情報更新
			var user = (string.IsNullOrEmpty(userId))
				? InsertUser(mallId, rakutenOrder, accessor)
				: UpdateUser(mallId, userId, rakutenOrder, accessor);
			userId = user.UserId;

			// 注文商品取得
			var rakutenOrderItems = new List<RakutenOrderItem>();
			foreach (var rakutenItem in rakutenOrder.packageModel[0].itemModel)
			{
				// 注文商品が商品マスタに存在しているかチェック
				var productId = GetProductId(this.ShopId, rakutenItem.itemNumber);

				// 商品マスタに商品が存在しない場合、エラーとする
				if (string.IsNullOrEmpty(productId))
				{
					Constants.DISP_ERROR_MESSAGE = "商品情報取得エラー：商品バリエーションID[" + rakutenItem.itemNumber
						+ "]の商品を取得できませんでした。商品情報をご確認ください。\r\n注文メールは詳細1をご確認ください。";
					throw new RakutenApiCoopException("商品バリエーションID" + rakutenItem.itemNumber + "の商品情報を取得できませんでした。");
				}

				rakutenOrderItems.Add(new RakutenOrderItem(this.ShopId, productId, rakutenItem));
			}

			// 注文情報登録
			InsertOrder(userId, rakutenOrder, rakutenOrderItems, accessor);

			// 注文者情報登録
			InsertOrderOwner(this.OrderId, user, accessor);

			// 配送先情報登録
			InsertOrderShipping(rakutenOrder, user, accessor);

			// 注文商品情報登録
			var stockErrorMessage = InsertOrderItem(mallId, rakutenOrder, rakutenOrderItems, accessor);

			// 海外注文かどうかチェック
			var sender = rakutenOrder.packageModel[0].senderModel;
			CheckOverseaOrder(
				mallId,
				this.OrderId,
				IsOverseaAddress(user.Zip, rakutenOrder.ordererModel.prefecture),
				IsOverseaAddress(
					string.Format("{0}-{1}", sender.zipCode1, sender.zipCode2),
					sender.prefecture),
				accessor);

			// 更新履歴登録
			new UpdateHistoryService().InsertForOrder(this.OrderId, Constants.FLG_LASTCHANGED_BATCH, accessor);
			new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_BATCH, accessor);

			// リアルタイム累計購入回数更新処理
			var order = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
				{Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME,  new UserService().Get(userId, accessor).OrderCountOrderRealtime},
			};
			OrderCommon.UpdateRealTimeOrderCount(order, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER, accessor);

			// 在庫エラー処理
			if (string.IsNullOrEmpty(stockErrorMessage) == false)
			{
				// 在庫エラーフォルダへ移動、エラーメール送信
				File.Move(this.FilePath, Path.Combine(Constants.PATH_STOCK_ERROR, Path.GetFileName(this.FilePath)));
				ErrorMailSender("在庫の更新時にエラーが発生しました。\r\n"
					+ stockErrorMessage
					+ "\r\nファイル名："
					+ Path.Combine(Constants.PATH_STOCK_ERROR, Path.GetFileName(this.FilePath)));

				return;
			}

			// 成功フォルダへ移動
			File.Move(this.FilePath, Path.Combine(Constants.PATH_SUCCESS, Path.GetFileName(this.FilePath)));

			// モール監視ログ登録（注文メール取込成功）
			Program.MallWatchingLogManager.Insert(
				accessor,
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
				mallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
				"注文メールの取込を1件完了しました。\r\n取込済みの注文メールは詳細1にてご確認頂けます。\r\n注文ID[" + this.OrderId + "]",
				this.MailMessage);

		}

		/// <summary>
		/// 楽天注文情報からユーザー登録
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>ユーザーモデル</returns>
		private UserModel InsertUser(string mallId, orderModel rakutenOrder, SqlAccessor accessor)
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
		private UserModel UpdateUser(string mallId, string userId, orderModel rakutenOrder, SqlAccessor accessor)
		{
			// 楽天注文情報からユーザーモデル作成
			var user = CreateUserModel(rakutenOrder, mallId, userId);

			// ユーザー更新処理
			var updateResult = new UserService().UpdateWithUserExtend(user, UpdateHistoryAction.DoNotInsert, accessor);
			if (updateResult == false) throw new RakutenApiCoopException("注文者のユーザ情報更新に失敗しました。");

			return user;
		}

		/// <summary>
		/// 楽天注文情報から注文登録
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="rakutenOrder">楽天受注情報</param>
		/// <param name="rakutenOrderItems">楽天注文商品情報</param>
		/// <param name="accessor">SQLアクセサー</param>
		private void InsertOrder(
			string userId,
			orderModel rakutenOrder,
			List<RakutenOrderItem> rakutenOrderItems,
			SqlAccessor accessor)
		{
			// 楽天注文情報から注文モデル作成
			var order = new RakutenOrder(this.ShopId, rakutenOrder, rakutenOrderItems);

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
					{ Constants.FIELD_ORDER_ORDER_STATUS, Constants.FLG_ORDER_ORDER_STATUS_ORDERED },
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

				var inserted = sqlStatement.ExecStatement(accessor, input);
				if (inserted <= 0)
				{
					throw new ApplicationException("注文情報の登録に失敗しました。");
				}
			}

			// 翌日配送「あす楽」を希望の注文の場合、注文拡張ステータスを更新
			if (rakutenOrder.asurakuFlg == "1")
			{
				if (string.IsNullOrEmpty(Constants.RAKUTEN_SHIPPING_TOMORROW_ORDER_EXTEND_STATUS_FIELD) == false)
				{
					var extendStatusNo = int.Parse(Constants.RAKUTEN_SHIPPING_TOMORROW_ORDER_EXTEND_STATUS_FIELD.Replace(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME, ""));
					new OrderService().UpdateOrderExtendStatus(
						rakutenOrder.orderNumber,
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
		private void InsertOrderShipping(orderModel rakutenOrder, UserModel owner, SqlAccessor accessor)
		{
			// 配送先が２つ以上有る場合、エラーになる
			if (rakutenOrder.packageModel.Length > 1)
			{
				Constants.DISP_ERROR_KBN = Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR;
				Constants.DISP_ERROR_MESSAGE = string.Format(
					"楽天受注API連携：注文[{0}]は複数の送付先を持っていますが、w2Commerceには複数配送先を対応していません。",
					rakutenOrder.orderNumber);
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
					{ Constants.FIELD_ORDERSHIPPING_ORDER_ID, this.OrderId },
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
			FileLogger.WriteInfo("[SQL END]配送先情報の登録");
		}

		/// <summary>
		/// 楽天APIレスポンスから注文商品情報を登録
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="rakutenOrderItems">楽天注文商品情報</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>在庫情報更新エラーメッセージ</returns>
		private string InsertOrderItem(
			string mallId,
			orderModel rakutenOrder,
			List<RakutenOrderItem> rakutenOrderItems,
			SqlAccessor accessor)
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
						{ Constants.FIELD_ORDERITEM_ORDER_ID, rakutenOrder.orderNumber },
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
				DataView dvProductSrockManagementKbn;
				using (var statement = new SqlStatement("Product", "GetProductSrockManagementKbn"))
				{
					var input = new Hashtable()
					{
						{ Constants.FIELD_PRODUCT_SHOP_ID, this.ShopId },
						{ Constants.FIELD_PRODUCT_PRODUCT_ID, orderItem.ProductId },
					};

					dvProductSrockManagementKbn = statement.SelectSingleStatement(accessor, input);
				}

				// 商品マスタに商品が存在しない場合、在庫更新エラーとする
				if (dvProductSrockManagementKbn.Count == 0)
				{
					FileLogger.WriteError("商品番号" + orderItem.ProductId + "の商品情報を取得できませんでした。");
					stockError = true;
					stockErrorMessage
						.Append("注文ID[")
						.Append(rakutenOrder.orderNumber)
						.Append("] 商品バリエーションID[")
						.Append(StringUtility.ToEmpty(orderItem.VariationId))
						.Append("]の商品情報をご確認ください。");

					// モール監視ログ登録（在庫更新エラー）
					Program.MallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
						mallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
						string.Format(
							"在庫更新エラー：商品バリエーションID[{0}]の商品を取得できませんでした。商品をご登録ください。\r\n注文メールは詳細1をご確認ください。\r\n注文ID[{1}]",
							StringUtility.ToEmpty(orderItem.VariationId),
							rakutenOrder.orderNumber),
						this.MailMessage);
				}
				// 該当商品の在庫管理方法が「在庫管理しない」以外の場合、在庫更新を行う（Not Transaction）
				else if ((string)dvProductSrockManagementKbn[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
				{
					using (var sqlStatement = new SqlStatement("Product", "UpdateProductStock"))
					using (var sqlStatement2 = new SqlStatement("Product", "InsertProductStockHistory"))
					{
						// 商品在庫マスタ更新
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

						// 商品在庫履歴マスタ更新
						var input2 = new Hashtable()
						{
							{ Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, this.ShopId },
							{ Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, rakutenOrder.orderNumber },
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

						// エラーメール文言
						if (stockError)
						{
							stockErrorMessage.AppendFormat(
								"注文ID[{0}] 商品バリエーションID[{1}]の在庫情報をご確認ください。",
								rakutenOrder.orderNumber,
								StringUtility.ToEmpty(orderItem.VariationId));

							// モール監視ログ登録（在庫更新エラー）
							Program.MallWatchingLogManager.Insert(
								accessor,
								Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
								mallId,
								Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
								string.Format(
									"在庫更新エラー：商品バリエーションID[{0}]が間違っているか、在庫数が取得出来なかった可能性があります。在庫情報をご確認ください。\r\n注文メールは詳細1をご確認ください。\r\n注文ID[{1}]",
									orderItem.VariationId,
									rakutenOrder.orderNumber),
								this.MailMessage);
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
		private static UserModel CreateUserModel(orderModel rakutenOrder, string mallId, string userId)
		{
			var rakutenUser = new RakutenUser(rakutenOrder.ordererModel, rakutenOrder.carrierCode);

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
		/// <param name="accessor">SQLアクセサ</param>
		private static void CheckOverseaOrder(
			string mallId,
			string orderId,
			bool isOverseaOrderOwnerAddress,
			bool isOverseaOrderShippingAddress,
			SqlAccessor accessor)
		{
			if ((isOverseaOrderOwnerAddress == false) && (isOverseaOrderShippingAddress == false)) return;

			if (Constants.RAKUTEN_OVERSEA_ORDER_MAIL_ENABLED
				&& (string.IsNullOrEmpty(Constants.RAKUTEN_OVERSEAS_ORDER_EXTEND_STATUS_FIELD) == false))
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
						accessor);
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
		/// APIレスポンスのユニットエラーチェック
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="getOrderResponse">楽天APIレスポンス</param>
		/// <param name="mallId">モールID</param>
		/// <param name="shopId">ショップID</param>
		private static void SendErrorMail(string orderId, baseResponseModel getOrderResponse, string shopId)
		{
			if (getOrderResponse == null) return;

			var mailMessage = new StringBuilder();
			// 受注情報取得時のエラー確認
			if (getOrderResponse.errorCode != Constants.RAKUTEN_API_RESULT_ERROR_CODE_NORMAL)
			{
				mailMessage.AppendLine(
					string.Format(
						"エラー{0}（{1}）により注文番号[{2}]の注文情報取得に失敗しました。",
						getOrderResponse.errorCode,
						getOrderResponse.message,
						orderId));
			}
			if (getOrderResponse.unitError != null)
			{
				getOrderResponse.unitError.Where(x => x.errorCode != Constants.RAKUTEN_API_RESULT_ERROR_CODE_NORMAL).ToList()
					.ForEach(x => mailMessage.AppendLine(
						string.Format(
							"エラー{0}（{1}）により注文番号[{2}]の注文情報取得に失敗しました。",
							x.errorCode,
							x.message,
							x.orderKey)));
			}

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
				var mailSendUtility = new MailSendUtility(
					shopId,
					Constants.CONST_MAIL_ID_RAKUTEN_COOP_ERROR,
					string.Empty,
					input,
					true,
					Constants.MailSendMethod.Auto);
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
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>対象のファイルパス</summary>
		public string FilePath { get; set; }
		/// <summary>メール文章</summary>
		public string MailMessage { get; set; }
		/// <summary>受注番号</summary>
		public string OrderId { get; set; }
		/// <summary>モール設定情報</summary>
		public DataRowView MallSetting { get; set; }
		#endregion
	}
}