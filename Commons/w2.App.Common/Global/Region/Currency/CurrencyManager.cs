/*
=========================================================================================================
  Module      : 通貨管理クラス (CurrecyManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Config;
using w2.App.Common.RefreshFileManager;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.Payment;

namespace w2.App.Common.Global.Region.Currency
{
	/// <summary>
	/// 通貨管理クラス
	/// </summary>
	public class CurrencyManager
	{
		/// <summary>キャッシュキー：為替レート</summary>
		private const string CACHE_KEY_EXCHANGERATE = "w2_CacheKey_ExchangeRate";

		/// <summary>日本国内の基軸通貨コード</summary>
		private const string CURRENCY_CODE_JPY = "JPY";

		/// <summary>台湾の基軸通貨コード</summary>
		private const string CURRENCY_CODE_TWD = "TWD";

		/// <summary>
		/// 基軸通貨価格取得 文字列型 通貨記号、セパレータを付与
		/// </summary>
		/// <param name="price">基軸通貨価格</param>
		/// <param name="withSymbol">通貨フォーマットの有無 true→通貨フォーマット適用あり(カンマ区切り、通貨記号あり) false→通貨フォーマット適用なし</param>
		/// <returns>基軸通貨価格</returns>
		public static string ToPriceByKeyCurrency(Object price, bool withSymbol = false)
		{
			var dec = ConvertPriceByKeyCurrency(price, DecimalUtility.Format.RoundDown);
			if (dec == null) return "";

			var result = (withSymbol)
				? StringUtility.ToPrice(
					dec.Value, Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId,
					Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleFormat)
				: dec.Value.ToString();
			return result;
		}

		/// <summary>
		/// 基軸通貨価格取得 Decimal型
		/// </summary>
		/// <param name="price">基軸通貨価格</param>
		/// <param name="format">Round→四捨五入、RoundDown→切り捨て、RoundUp→切り上げ</param>
		/// <returns>基軸通貨価格</returns>
		public static decimal? ConvertPriceByKeyCurrency(Object price, DecimalUtility.Format format = DecimalUtility.Format.RoundDown)
		{
			// 通貨によっては小数点のセパレータがコンマの場合があるため、コンマが含まれる場合はピリオドに変換
			// 計算時は小数点以下のセパレータはピリオドに統一
			var priceText = StringUtility.ToEmpty(price).Replace(
				CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalSeparator,
				".");

			decimal dec;
			if (decimal.TryParse(priceText, out dec) == false) return null;

			// 通貨コードの小数点以下の有効桁数より丸め処理
			dec = DecimalUtility.DecimalRound(dec, format, DigitsByKeyCurrency);
			return dec;
		}

		/// <summary>
		/// 表示通貨価格取得（レート計算あり）
		/// </summary>
		/// <param name="priceSrc">価格（数値型）</param>
		/// <returns>表示通貨価格</returns>
		public static string ToPrice(Object priceSrc)
		{
			var result = ToPriceExecutor(priceSrc, ToPrice);
			return result;
		}
		/// <summary>
		/// 表示通貨価格取得（レート計算あり）
		/// 通貨コード（元）：基軸通貨固定
		/// 通貨コード（先）：フロント表示通貨
		/// </summary>
		/// <param name="price">価格</param>
		/// <returns>表示通貨価格</returns>
		public static string ToPrice(decimal price)
		{
			var result = string.Empty;
			// クッキーから取得できない場合、または、非対応通貨の場合は基軸通貨で表示
			var region = RegionManager.GetInstance().Region;
			if ((region == null) || (GlobalConfigUtil.CheckCurrencyPossible(region.CurrencyCode) == false))
			{
				result = ToPriceByKeyCurrency(price, true);
				return result;
			}
			result = ToPrice(region.CurrencyCode, region.CurrencyLocaleId, price);
			return result;
		}
		/// <summary>
		/// 表示通貨価格取得（レート計算あり）
		/// 通貨コード（元）：基軸通貨固定
		/// </summary>
		/// <param name="dstCurrencyCode">通貨コード（先）</param>
		/// <param name="localeId">ロケールID</param>
		/// <param name="price">価格</param>
		/// <returns>表示通貨価格</returns>
		public static string ToPrice(string dstCurrencyCode, string localeId, decimal price)
		{
			// グローバルオプション無効の場合は設定ファイルを見ず、通貨変換を行う
			if (Constants.GLOBAL_OPTION_ENABLE == false) return StringUtility.ToPrice(price);

			var srcCurrencyCode = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;
			var result = ToPrice(srcCurrencyCode, dstCurrencyCode, localeId, price);
			return result;
		}
		/// <summary>
		/// 表示通貨価格取得（レート計算あり）
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCode">通貨コード（先）</param>
		/// <param name="localeId">ロケールID</param>
		/// <param name="price">価格</param>
		/// <returns>表示通貨価格</returns>
		private static string ToPrice(string srcCurrencyCode, string dstCurrencyCode, string localeId, decimal price)
		{
			var format = GlobalConfigUtil.GetCurrencyLocaleFormat(localeId);

			// 通貨コード（先）が指定なしの場合は、デフォルトカルチャの通貨形式
			if (string.IsNullOrEmpty(dstCurrencyCode)) return StringUtility.ToPrice(price);

			var rate = GetCachedExchangeRate(srcCurrencyCode, dstCurrencyCode);
			var calculatedPrice = price * rate;
			var result = StringUtility.ToPrice(calculatedPrice, localeId, format);
			return result;
		}

		/// <summary>
		/// 価格取得の実行メソッド
		/// decimalに変換後、指定のメソッドを実行します
		/// </summary>
		/// <param name="priceSrc">価格（数値型）</param>
		/// <param name="func">価格取得関数（引数：decimal、戻り値：string）</param>
		/// <returns>表示通貨価格</returns>
		private static string ToPriceExecutor(Object priceSrc, Func<decimal, string> func)
		{
			decimal price;
			if (decimal.TryParse(StringUtility.ToEmpty(priceSrc), out price))
			{
				return func(price);
			}
			return string.Empty;
		}

		/// <summary>
		/// 基軸通貨での価格変換（レート計算あり）
		/// </summary>
		/// <param name="price">価格</param>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <returns>基軸通貨での価格</returns>
		public static decimal ExchangePriceToKeyCurrency(decimal price, string srcCurrencyCode)
		{
			// グローバルオプション無効の場合または通貨コード元が指定されない場合は設定ファイルを見ず、そのまま戻す
			if ((Constants.GLOBAL_OPTION_ENABLE == false) || string.IsNullOrEmpty(srcCurrencyCode)) return price;

			var dstCurrencyCode = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;
			var rate = GetCachedExchangeRate(srcCurrencyCode, dstCurrencyCode);
			var calculatedPrice = Math.Round(price * rate, 2);

			return calculatedPrice;
		}

		/// <summary>
		/// キャッシュキー生成
		/// 通貨コードの元と先を結合したキャッシュキーを生成します。
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCode">通貨コード（先）</param>
		/// <returns>キャッシュキー</returns>
		private static string CreateCacheKey(string srcCurrencyCode, string dstCurrencyCode)
		{
			var result = srcCurrencyCode + dstCurrencyCode;
			return result;
		}

		/// <summary>
		/// キャッシュ済み為替レート取得
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCode">通貨コード（先）</param>
		/// <returns>為替レート</returns>
		private static decimal GetCachedExchangeRate(string srcCurrencyCode, string dstCurrencyCode)
		{
			var cache = CacheAllExchangeRate();
			var cacheKey = CreateCacheKey(srcCurrencyCode, dstCurrencyCode);
			var result = cache.ContainsKey(cacheKey) ? cache[cacheKey] : 1;
			return result;
		}

		/// <summary>
		/// 為替レート全件キャッシュ
		/// </summary>
		/// <returns>キャッシュ済み為替レートディクショナリ</returns>
		private static Dictionary<string, decimal> CacheAllExchangeRate()
		{
			var result = DataCacheControllerFacade.GetExchangeRateCacheController()
				.CacheData
				.ToDictionary(
					rate => CreateCacheKey(rate.SrcCurrencyCode, rate.DstCurrencyCode),
					rate => rate.ExchangeRate);
			return result;
		}

		/// <summary>
		/// 決済通貨を取得
		/// </summary>
		/// <param name="paymentId">決済種別</param>
		/// <returns>決済通貨</returns>
		public static string GetSettlementCurrency(string paymentId)
		{
			// グローバルオプションがOFFの場合は常にJPY
			if (Constants.GLOBAL_OPTION_ENABLE == false) return CURRENCY_CODE_JPY;

			switch (paymentId)
			{
				// クレジットカード
				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					return GetSettlementCurrencyByPaymentCardKbn();

				// 代引き
				case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT:
					return Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;

				// Paypal
				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
					return Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;

				// 台湾後払い
				case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
					return CURRENCY_CODE_TWD;

				// Convenience store
				case Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE:
					return CURRENCY_CODE_TWD;

				// Aftee must use taiwan dollar
				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					return CURRENCY_CODE_TWD;

				// 決済なし
				case Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT:
					return Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;

				// Ec Pay
				case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
					return CURRENCY_CODE_TWD;

				// LINE Pay
				case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
					return Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;

				// NewebPay
				case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
					return CURRENCY_CODE_TWD;

				default:
					return CURRENCY_CODE_JPY;
			}
		}

		/// <summary>
		/// 決済通貨を取得
		/// </summary>
		/// <returns>決済通貨</returns>
		private static string GetSettlementCurrencyByPaymentCardKbn()
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				// ２－１－Ｂ．カード・ゼウス決算処理
				case Constants.PaymentCard.Zeus:
					return CURRENCY_CODE_JPY;

				// ２－１－Ｆ．カード・GMO決算処理
				case Constants.PaymentCard.Gmo:
					return CURRENCY_CODE_JPY;

				// ２－１－Ｇ．カード・SBPSクレジット決算処理
				case Constants.PaymentCard.SBPS:
					return CURRENCY_CODE_JPY;

				// ２－１－Ｈ．カード・ヤマトKWCクレジット決算処理
				case Constants.PaymentCard.YamatoKwc:
					return CURRENCY_CODE_JPY;

				// ２－１－ｉ．カード・ZcomPaymentクレジット決算処理
				case Constants.PaymentCard.Zcom:
					return Constants.PAYMENT_SETTING_CREDIT_ZCOM_CURRENCYCODE;

				// ２－１－ｊ．カード・e-SCOTTクレジット決算処理
				case Constants.PaymentCard.EScott:
					return CURRENCY_CODE_JPY;

				// ２－１－k．カード・ベリトランスクレジット決算処理
				case Constants.PaymentCard.VeriTrans:
					return CURRENCY_CODE_JPY;

				default:
					return CURRENCY_CODE_JPY;
			}
		}

		/// <summary>
		/// 決済レート取得
		/// </summary>
		/// <param name="settlementCurrency">決済通貨</param>
		/// <returns>決済レート</returns>
		public static decimal GetSettlementRate(string settlementCurrency)
		{
			// グローバルオプションがOFFの場合は常に1
			if (Constants.GLOBAL_OPTION_ENABLE == false) return 1;

			var settlementRate = GetCachedExchangeRate(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code, settlementCurrency);
			return settlementRate;
		}

		/// <summary>
		/// 決済金額取得
		/// </summary>
		/// <param name="oldOrderId">元の注文ID</param>
		/// <param name="paymentKbn">決済区分</param>
		/// <param name="priceTotal">金額</param>
		/// <param name="settlementRate">決済レート</param>
		/// <param name="settlementCurrency">決済通貨</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>決済金額</returns>
		public static decimal GetSettlementAmount(
			string oldOrderId,
			string paymentKbn,
			decimal priceTotal,
			decimal settlementRate,
			string settlementCurrency,
			SqlAccessor accessor = null)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return priceTotal;

			// 支払金額と決済区分が同じ場合はDBの決済金額を使用
			var oldOrder = new OrderService().Get(oldOrderId, accessor);
			if (oldOrder.LastBilledAmount == priceTotal 
				&& oldOrder.OrderPaymentKbn == paymentKbn) return oldOrder.SettlementAmount;

			var settlementAmount = GetSettlementAmount(priceTotal, settlementRate, settlementCurrency);
			return settlementAmount;
		}

		/// <summary>
		/// 決済金額取得
		/// </summary>
		/// <param name="priceTotal">金額</param>
		/// <param name="settlementRate">決済レート</param>
		/// <param name="settlementCurrency">決済通貨</param>
		/// <returns>決済金額</returns>
		public static decimal GetSettlementAmount(decimal priceTotal, decimal settlementRate, string settlementCurrency)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return priceTotal;

			var currencyLocal = GlobalConfigUtil.GetLocalIdByCurrency(settlementCurrency).CurrencyLocales[0];

			// 表示通貨と共通化
			var settlementAmount = decimal.Parse(StringUtility.ToPrice(
				priceTotal * settlementRate,
				currencyLocal.LocaleId,
				currencyLocal.LocaleFormatWithoutSymbol));
			return settlementAmount;
		}

		/// <summary>
		/// 送金額を取得
		/// </summary>
		/// <param name="priceTotal">支払合計金額</param>
		/// <param name="settlementAmount">決済金額</param>
		/// <param name="settlementCurrency">決済通貨</param>
		/// <returns>送金額</returns>
		public static decimal GetSendingAmount(decimal priceTotal, decimal settlementAmount, string settlementCurrency)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return priceTotal;

			var sendingAmount = (settlementCurrency == Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code)
				? priceTotal
				: settlementAmount;
			return sendingAmount;
		}

		/// <summary>
		/// 決済通貨の表記法に変換
		/// </summary>
		/// <param name="settlementAmount">決済金額</param>
		/// <param name="settlementCurrency">決済通貨</param>
		/// <param name="isPaymentMemo">Is Payment Memo</param>
		/// <returns>決済金額</returns>
		public static string ToSettlementCurrencyNotation(
			decimal settlementAmount,
			string settlementCurrency,
			bool isPaymentMemo = false)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return "";
			var currencyLocal = GlobalConfigUtil.GetLocalIdByCurrency(settlementCurrency).CurrencyLocales[0];
			var region = RegionManager.GetInstance().Region;

			// クッキーから取得できない場合、または、非対応通貨の場合は基軸通貨
			string resionCurrencyCode;
			if ((region == null) || (GlobalConfigUtil.CheckCurrencyPossible(region.CurrencyCode) == false))
			{
				resionCurrencyCode = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;
			}
			else
			{
				resionCurrencyCode = region.CurrencyCode;
			}

			var localeFormat = ((settlementCurrency == CURRENCY_CODE_JPY)
				&& (resionCurrencyCode != CURRENCY_CODE_JPY)
				&& (isPaymentMemo == false))
					? currencyLocal.LocaleFormatJapanese
					: currencyLocal.LocaleFormat;
			var price = StringUtility.ToPrice(
				settlementAmount,
				currencyLocal.LocaleId,
				localeFormat);
			return price;
		}

		/// <summary>
		/// 決済種別から決済できない支払いを除去(DataView)
		/// </summary>
		/// <param name="validPayments">有効決済種別</param>
		/// <returns>決済できない支払いを除いた決済種別</returns>
		public static DataView RemovePaymentsByKeyCurrency(DataView validPayments)
		{
			// グローバルオプションOFFならばそのまま返す
			if (Constants.GLOBAL_OPTION_ENABLE == false) return validPayments;

			// 基軸通貨に対応している決済(クレジットカードと台湾後払いは全通貨対応)
			var result = validPayments
				.ToTable()
				.AsEnumerable()
				.Where(item =>
					(GetSettlementCurrency((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID]) == Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code)
					|| ((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					|| ((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID] == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					|| ((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID] == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
					|| ((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID] == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
					|| ((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID] == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					|| ((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID] == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					|| ((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID] == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))

				.AsDataView();
			return result;
		}

		/// <summary>
		/// 決済種別から決済できない支払いを除去(PaymentModel)
		/// </summary>
		/// <param name="payments">有効決済種別</param>
		/// <returns>決済できない支払いを除いた決済種別</returns>
		public static PaymentModel[] RemovePaymentsByKeyCurrency(PaymentModel[] payments)
		{
			// グローバルオプションOFFならばそのまま返す
			if (Constants.GLOBAL_OPTION_ENABLE == false) return payments;

			// 基軸通貨に対応している決済(クレジットカードと台湾後払いは全通貨対応)
			var result = payments
				.Where(model =>
					(GetSettlementCurrency(model.PaymentId) == Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code)
					|| (model.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					|| (model.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					|| (model.PaymentId == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
					|| (model.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
					|| (model.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					|| (model.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					|| (model.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
				.ToArray();
			return result;
		}

		/// <summary>
		/// Convert Price
		/// </summary>
		/// <param name="price">price</param>
		/// <returns>Value Convert Price</returns>
		public static decimal ConvertPrice(decimal price)
		{
			var region = RegionManager.GetInstance().Region;
			var result = ExchangePriceToKeyCurrencyTaiwan(price, region.CurrencyCode);

			return result;
		}

		/// <summary>
		/// Exchange Price To Key Currency Taiwan
		/// </summary>
		/// <param name="price">Price</param>
		/// <param name="dstCurrencyCode">Destination Currency Code</param>
		/// <returns>Price in key currency</returns>
		public static decimal ExchangePriceToKeyCurrencyTaiwan(decimal price, string dstCurrencyCode)
		{
			// グローバルオプション無効の場合または通貨コード元が指定されない場合は設定ファイルを見ず、そのまま戻す
			if ((Constants.GLOBAL_OPTION_ENABLE == false) || string.IsNullOrEmpty(dstCurrencyCode)) return price;

			var rate
				= GetCachedExchangeRate(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code, dstCurrencyCode);
			var calculatedPrice = Math.Round(price * rate, 2);

			return calculatedPrice;
		}

		/// <summary>基軸通貨 小数点以下の有効桁数</summary>
		public static int DigitsByKeyCurrency
		{
			get
			{
				return Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.CurrencyDecimalDigits 
					?? CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalDigits;
			}
		}
		/// <summary>
		/// 基軸通貨コードが日本国内のものかどうかを取得
		/// 日本国内→true, 日本国外→false
		/// </summary>
		public static bool IsJapanKeyCurrencyCode
		{
			get
			{
				return (string.Equals(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code, CURRENCY_CODE_JPY));
			}
		}
		/// <summary> 基軸通貨表記 </summary>
		public static string KeyCurrencyUnit
		{
			get
			{
				// 手動マージを減らすため、UnitStringが空の場合はデフォルトの値を設定する
				if (string.IsNullOrEmpty(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.UnitString))
				{
					switch (Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code)
					{
						case CURRENCY_CODE_JPY:
							return "¥";

						case CURRENCY_CODE_TWD:
							return "NT$";
					}
				}
				return Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.UnitString;
			}
		}

		/// <summary>
		/// Update exchange rate cache
		/// </summary>
		public static void UpdateExchangeRateCache()
		{
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ExchangeRate).CreateUpdateRefreshFile();
		}
	}
}