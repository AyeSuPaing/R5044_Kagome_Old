/*
=========================================================================================================
  Module      : YAHOO API Yahoo注文取込進行クラス (YahooMallOrderDetailImportProcedure.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Mall.Yahoo.Dto;
using w2.App.Common.Mall.Yahoo.Foundation;
using w2.App.Common.Mall.Yahoo.Interfaces;
using w2.App.Common.MallCooperation;
using w2.App.Common.SendMail;
using w2.Common;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.MallCooperationSetting;
using w2.Domain.Order;

namespace w2.App.Common.Mall.Yahoo.Procedures
{
	/// <summary>
	/// Yahoo注文取込進行クラス
	/// </summary>
	public class YahooMallOrderDetailImportProcedure
	{
		private readonly IOrderService _orderService;
		private readonly IMallCooperationSettingService _mallCooperationSettingService;
		private readonly IYahooApiFacade _yahooApiFacade;
		private readonly IYahooMallOrderUpdateProcedure _yahooMallOrderUpdateProcedure;
		private readonly IYahooApiTokenProcedure _yahooApiTokenProcedure;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooMallOrderDetailImportProcedure()
		{
			_orderService = new OrderService();
			_mallCooperationSettingService = new MallCooperationSettingService();
			_yahooApiFacade = new YahooApiFacade();
			_yahooMallOrderUpdateProcedure = new YahooMallOrderUpdateProcedure();
			_yahooApiTokenProcedure = new YahooApiTokenProcedure();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderService"></param>
		/// <param name="mallCooperationSettingService"></param>
		/// <param name="yahooApiFacade"></param>
		/// <param name="yahooMallOrderUpdateProcedure"></param>
		/// <param name="yahooApiTokenProcedure"></param>
		public YahooMallOrderDetailImportProcedure(
			IOrderService orderService,
			IMallCooperationSettingService mallCooperationSettingService,
			IYahooApiFacade yahooApiFacade,
			IYahooMallOrderUpdateProcedure yahooMallOrderUpdateProcedure,
			IYahooApiTokenProcedure yahooApiTokenProcedure)
		{
			_orderService = orderService;
			_mallCooperationSettingService = mallCooperationSettingService;
			_yahooApiFacade = yahooApiFacade;
			_yahooMallOrderUpdateProcedure = yahooMallOrderUpdateProcedure;
			_yahooApiTokenProcedure = yahooApiTokenProcedure;
		}

		/// <summary>
		/// Yahooモール注文取込
		/// </summary>
		/// <param name="mallId">モールID</param>
		public void ImportYahooMallOrderDetails(string mallId)
		{
			// モール連携基本設定の取得
			var mallCooperationSetting =
				_mallCooperationSettingService.Get(Constants.CONST_DEFAULT_SHOP_ID, mallId: mallId);
			if (mallCooperationSetting == null)
			{
				var errMsg = "モール連携基本設定が存在しません。指定したモールIDを確認してください。";
				var errLog = $"{errMsg}mall_id={mallId}";
				WriteMallWatchingLog(mallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, errMsg);
				FileLogger.WriteError(errLog);
				NotifyError(mallId, errMsg);
				return;
			}

			// アクセストークンの取得
			var result = _yahooApiTokenProcedure.GetAccessTokenWithRefreshToken(
				mallId,
				dateTimeToCompare: DateTime.Now,
				mallCooperationSetting);
			if (result.RequiresRefreshOnBrowser())
			{
				var errMsg = "アクセストークンを取得できません。EC管理画面上でトークンを再取得が必要です。\r\nこのエラーが頻出する場合は、公開鍵による認証を行っていない可能性があります。";
				WriteMallWatchingLog(mallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, errMsg);
				NotifyError(mallId, errMsg);
				return;
			}
			if (result.IsSuccessful() == false)
			{
				var errMsg = "アクセストークンの取得に失敗しました。管理者へ問い合わせてください。";
				WriteMallWatchingLog(mallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, errMsg);
				NotifyError(mallId, errMsg);
				return;
			}
			var msg = "アクセストークンの取得に成功しました。";
			var log = $"{msg}mall_id={mallId}";
			FileLogger.WriteDebug(log);
			WriteMallWatchingLog(mallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, msg);

			// 注文取込
			var counter = ImportOrderDetails(
				result.AccessToken,
				mallCooperationSetting);
			NotifyResult(counter.GenerateEmailBodyInput(mallId));
			
			// ログ
			var message = $"モール注文の取り込みを完了しました。結果: {counter.GenerateResultMessage()}";
			FileLogger.WriteInfo(message);
			var logType = counter.HasAnyErrorOrWarning() == false
				? Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS
				: Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING;
			WriteMallWatchingLog(mallId, logType, message);

			WriteMallWatchingLog(mallId,
				counter.IsSuccessPublicKeyAuthorized
					? Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS
					: Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
				counter.IsSuccessPublicKeyAuthorized
					? $"公開鍵認証に成功しました。"
					: $"公開鍵認証に失敗しました。");
		}

		/// <summary>
		/// 認証情報の生成と公開鍵による暗号化
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="sellerId">セラーID</param>
		/// <param name="publicKey">公開鍵</param>
		/// <returns>公開鍵認証のための暗号化した認証情報</returns>
		private string GenerateEncryptedAuthValue(string mallId, string sellerId, string publicKey)
		{
			try
			{
				var authValue = new YahooApiPubkey(sellerId, publicKey).Encrypt();
				return authValue;
			}
			catch (Exception ex)
			{
				var errMsg = "公開鍵による認証に失敗しました。";
				var errLog = $"{errMsg}mall_id={mallId}";
				FileLogger.WriteError(errLog, ex);
				return "";
			}
		}

		/// <summary>
		/// メソッド内で更新ループ
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="mallCooperationSetting">モール設定</param>
		/// <returns>結果オブジェクト</returns>
		private YahooMallOrderDetailsImportResult ImportOrderDetails(
			string accessToken,
			MallCooperationSettingModel mallCooperationSetting)
		{
			// 取込対象の注文抽出
			var mallId = mallCooperationSetting.MallId;
			var orders = _orderService.GetOrdersForYahooOrderImport(mallId);
			var counter = new YahooMallOrderDetailsImportResult();
			counter.SetTotalCount(orders.Length);

			var isSuccessPublicKeyAuthorized = false;
			var publicKeyAuthorizedAt = new DateTime();

			// 取込
			foreach (var order in orders)
			{
				try
				{
					// 取り込み実行
					isSuccessPublicKeyAuthorized = ImportOrderDetail(order, accessToken, mallCooperationSetting);
					counter.IncrementSuccessCount();

					if (isSuccessPublicKeyAuthorized)
					{
						publicKeyAuthorizedAt = DateTime.Now;
					}
				}
				catch (YahooMallOrderException ex)
				{
					var err = $"注文の取り込みを中止しました。mallId={mallId},orderId={order[Constants.FIELD_ORDER_ORDER_ID]}";
					FileLogger.WriteWarn(err, ex);
					counter.IncrementWarningCount();
				}
				catch (Exception ex)
				{
					var err = $"注文の取り込みに失敗しました。mallId={mallId},orderId={order[Constants.FIELD_ORDER_ORDER_ID]}";
					FileLogger.WriteError(err, ex);
					counter.IncrementFailureCount();
				}
			}

			counter.SetPublicKeyAuthorized(isSuccessPublicKeyAuthorized);
			// 公開鍵認証結果に基づき、最終認証日時を更新
			if (isSuccessPublicKeyAuthorized)
			{
				_mallCooperationSettingService.UpdateYahooApiPublicKey(Constants.CONST_DEFAULT_SHOP_ID, mallId, publicKeyAuthorizedAt);
			}

			return counter;
		}

		/// <summary>
		/// 注文取込実行
		/// </summary>
		/// <param name="order">モールID</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="mallCooperationSetting">モール設定</param>
		/// <returns>公開鍵認証成功か</returns>
		private bool ImportOrderDetail(
			Dictionary<string, string> order,
			string accessToken,
			MallCooperationSettingModel mallCooperationSetting)
		{
			// 公開鍵認証準備 (認証情報の生成と公開鍵による暗号化)
			var mallId = mallCooperationSetting.MallId;
			var encryptedAuthValue = GenerateEncryptedAuthValue(
				mallId,
				mallCooperationSetting.YahooApiSellerId,
				mallCooperationSetting.YahooApiPublicKey);

			// 注文詳細API実行し、注文情報を取得
			var orderId = order[Constants.FIELD_ORDER_ORDER_ID];
			var valueList = orderId.Split('-');
			var targetMallId = valueList.Length > 1
				? orderId.Substring(0, orderId.LastIndexOf('-')) 
				: string.Empty;
			var mallOrderId = targetMallId == mallId
				? valueList[valueList.Length - 1]
				: orderId;
			
			var yahooMallOrder = _yahooApiFacade.GetYahooMallOrder(
				mallOrderId,
				accessToken,
				mallCooperationSetting.YahooApiSellerId,
				encryptedAuthValue,
				StringUtility.ToEmpty(mallCooperationSetting.YahooApiPublicKeyVersion));
			if (yahooMallOrder.IsSuccessful() == false)
			{
				FileLogger.WriteDebug($"注文詳細APIの実行に失敗しました。mallId={mallId},orderId={orderId}");
				_yahooMallOrderUpdateProcedure.RecordFailedResult(orderId);
				return false;
			}
			FileLogger.WriteDebug($"注文詳細APIの実行に成功しました。mallId={mallId},orderId={orderId}");

			// DB更新 
			var updateResult = _yahooMallOrderUpdateProcedure.Update(
				order: yahooMallOrder,
				tempUserId: order[Constants.FIELD_ORDER_USER_ID],
				mallId: mallId,
				orderId);
			if (updateResult == false)
			{
				FileLogger.WriteDebug($"注文詳細のDB取り込みに失敗しました。mallId={mallId},orderId={orderId}");
				_yahooMallOrderUpdateProcedure.RecordFailedResult(orderId);
				return yahooMallOrder.PublicKeyAuthResultCode == YahooApiPublicKeyAuthResponseStatus.Authorized;
			}
			FileLogger.WriteDebug($"注文詳細のDB取り込みに成功しました。mallId={mallId},orderId={orderId}");

			return yahooMallOrder.PublicKeyAuthResultCode == YahooApiPublicKeyAuthResponseStatus.Authorized;
		}

		/// <summary>
		/// エラー通知
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		private void NotifyError(string mallId, string errorMessage)
		{
			SendMailCommon.SendMailToOperator(
				Constants.CONST_MAIL_ID_MALL_ORDER_IMPORTER_ABNORMAL_TERMINATION,
				new Hashtable { { "error_message", errorMessage }, { "mall_id", mallId }, });
		}

		/// <summary>
		/// 結果通知
		/// </summary>
		/// <param name="input">入力値</param>
		private void NotifyResult(Hashtable input)
		{
			SendMailCommon.SendMailToOperator(Constants.CONST_MAIL_ID_MALL_ORDER_IMPORTER_TERMINATION, input);
		}

		/// <summary>
		/// モール監視ログ挿入
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="logKbn">ログ区分</param>
		/// <param name="message">メッセージ</param>
		/// <param name="accessor">アクセサ</param>
		private void WriteMallWatchingLog(string mallId, string logKbn, string message, SqlAccessor accessor = null)
		{
			if (accessor != null)
			{
				new MallWatchingLogManager().Insert(
					accessor,
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MALLORDERIMPORTER,
					mallId,
					logKbn,
					message);
			}
			else
			{
				new MallWatchingLogManager().Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MALLORDERIMPORTER,
					mallId,
					logKbn,
					message);
			}
		}
	}
}
