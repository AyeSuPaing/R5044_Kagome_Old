/*
=========================================================================================================
  Module      : マスタ出力ヘルパ(MasterExportHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.SerialKey.Helper;
using w2.Domain.ShortUrl.Helper;

namespace w2.Domain.MasterExportSetting.Helper
{
	/// <summary>
	/// マスタ出力ヘルパ
	/// </summary>
	public class MasterExportHelper
	{
		/// <summary>Order id for get data for order combine</summary>
		public const string ORDER_ID_FOR_GET_FIXED_PURCHASE_SHIPPING_DATES = "order_id_for_get_fixed_purchase_shipping_dates";
		/// <summary>日付形式：「yyyy/MM/dd」のパターン※サーバータイム付けない</summary>
		public const string SHORT_DATE2_LETTER_NONE_SERVER_TIME = "yyyy/MM/dd";
		/// <summary>日付形式：「yyyy/MM/dd HH:mm:ss」のパターン※サーバータイム付けない</summary>
		public const string SHORT_DATE_HOUR_MINUTE_SECOND_NONE_SERVER_TIME = "yyyy/MM/dd HH:mm:ss";

		/// <summary>
		/// CSV行作成（改行なし）
		/// </summary>
		/// <param name="datas">データ配列</param>
		/// <returns>CSV行</returns>
		public static string CreateCsvLine(IEnumerable<string> datas)
		{
			return string.Join(",", datas.Select(data => string.Format("\"{0}\"", StringUtility.EscapeCsvColumn(data))));
		}

		/// <summary>
		/// データコンバート
		/// </summary>
		/// <param name="setting">マスタ出力設定</param>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="source">データ</param>
		/// <returns></returns>
		public static object ConvertData(MasterExportSettingModel setting, string fieldName, object source)
		{
			switch (fieldName)
			{
				// シリアルキーは復号化する
				case Constants.FIELD_SERIALKEY_SERIAL_KEY:
					return SerialKeyUtility.DecryptSerialKey((string)source);

				// 空白結合済みシリアルキーは別メソッドで復号化する
				case "serial_keys":
					return string.Join(",",
						StringUtility.ToEmpty(source).Trim().Split().Select(SerialKeyUtility.DecryptSerialKey));

				// プロトコルとドメイン名を付与する
				case Constants.FIELD_SHORTURL_SHORT_URL:
				case Constants.FIELD_SHORTURL_LONG_URL:
					if (setting.MasterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL)
					{
						source = UrlUtility.AddProtocolAndDomain(StringUtility.ToEmpty(source));
					}
					break;
			}
			return source;
		}

		/// <summary>
		/// データコンバート
		/// </summary>
		/// <param name="setting">マスタ出力設定</param>
		/// <param name="lineData">行データ</param>
		public static void ConvertDatas(MasterExportSettingModel setting, Hashtable lineData)
		{
			// モール出品設定マスタの場合、モール連携設定にて設定済みの出品情報のみCSV出力する（出力値）
			if (setting.MasterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_MALLEXHIBITSCONFIG)
			{
				for (var exhibitsCount = 1; exhibitsCount <= Constants.CONST_MALLEXHIBITS_COUNT; exhibitsCount++)
				{
					var fieldName = Constants.CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG + exhibitsCount;
					if (lineData.ContainsKey(fieldName) == false) continue;
					if (lineData[fieldName] == DBNull.Value)
					{
						lineData[fieldName] = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
					}
				}
			}
		}

		/// <summary>
		/// Is Get Combine Fixed Purchase Data
		/// </summary>
		/// <param name="field">Field</param>
		/// <param name="masterKbn">Master Kbn</param>
		/// <returns>True if has get combine fixed purchase data</returns>
		public static bool IsGetTargetOfCombineFixedPurchaseData(string field, string masterKbn)
		{
			var masterExportSettingService = new MasterExportSettingService();
			masterKbn = masterExportSettingService.GetMasterKbnException(masterKbn);
			var isValidMasterKbn = ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER)
				|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM));

			return isValidMasterKbn
				&& (field.Contains(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE)
					|| field.Contains(Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE)
					|| field.Contains(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN));
		}

		/// <summary>
		/// Get Parents Order Combine Fixed Purchase
		/// </summary>
		/// <param name="orderId">OrderId</param>
		/// <returns>Parents fixed purchase</returns>
		public static List<FixedPurchaseModel> GetParentOrderCombineFixedPurchase(string orderId)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Order", "GetParentOrderCombineFixedPurchase"))
			{
				var input = new Hashtable()
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId }
			};
				var data = statement.SelectSingleStatementWithOC(accessor, input);

				return data.Cast<DataRowView>().Select(dataRow => new FixedPurchaseModel(dataRow)).ToList();
			}
		}

		/// <summary>
		/// Get Data Of Fixed Purchase Parents
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="field">Field</param>
		/// <param name="listFixedPurchaseParent">List Fixed Purchase Parent</param>
		/// <returns>Data</returns>
		public static string GetDataOfFixedPurchaseParents(
			string value,
			string field,
			List<FixedPurchaseModel> listFixedPurchaseParent)
		{
			if (string.IsNullOrEmpty(value)) return string.Empty;

			var listTempResult = new List<string>() { ConvertStringToDateFormat(value) };
			listTempResult.AddRange(listFixedPurchaseParent.Select(fixedPurchase
				=> ConvertStringToDateFormat(StringUtility.ToEmpty(fixedPurchase.DataSource[field]))).ToArray());
			listTempResult.Sort();
			return string.Join(",", listTempResult);
		}

		/// <summary>
		/// Get DataTime Of Fixed Purchase Parents
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="field">Field</param>
		/// <param name="listFixedPurchaseParent">List Fixed Purchase Parent</param>
		/// <returns>Data</returns>
		public static string GetDataTimeOfFixedPurchaseParents(
			string value,
			string field,
			List<FixedPurchaseModel> listFixedPurchaseParent)
		{
			if (string.IsNullOrEmpty(value)) return string.Empty;

			var listTempResult = new List<string>() { ConvertStringToDateTimeFormat(value) };
			listTempResult.AddRange(listFixedPurchaseParent.Select(fixedPurchase
				=> ConvertStringToDateTimeFormat(StringUtility.ToEmpty(fixedPurchase.DataSource[field]))).ToArray());
			listTempResult.Sort();
			return string.Join(",", listTempResult);
		}

		/// <summary>
		/// Convert String To Date Format
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>Value with date format</returns>
		public static string ConvertStringToDateFormat(string value)
		{
			return Convert.ToDateTime(value).ToString(SHORT_DATE2_LETTER_NONE_SERVER_TIME);
		}

		/// <summary>
		/// Convert String To DateTime Format
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>Value with date format</returns>
		public static string ConvertStringToDateTimeFormat(string value)
		{
			return Convert.ToDateTime(value).ToString(SHORT_DATE_HOUR_MINUTE_SECOND_NONE_SERVER_TIME);
		}


		/// <summary>
		/// 基軸通貨価格取得 Decimal型
		/// </summary>
		/// <param name="price">基軸通貨価格</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <param name="format">Round→四捨五入、RoundDown→切り捨て、RoundUp→切り上げ</param>
		/// <returns>基軸通貨価格</returns>
		public static decimal? ConvertPriceByKeyCurrency(Object price, int digitsByKeyCurrency, string replacePrice, DecimalUtility.Format format = DecimalUtility.Format.RoundDown)
		{
			// 通貨によっては小数点のセパレータがコンマの場合があるため、コンマが含まれる場合はピリオドに変換
			// 計算時は小数点以下のセパレータはピリオドに統一
			var priceText = StringUtility.ToEmpty(price).Replace(replacePrice, ".");

			decimal dec;
			if (decimal.TryParse(priceText, out dec) == false) return null;

			// 通貨コードの小数点以下の有効桁数より丸め処理
			dec = DecimalUtility.DecimalRound(dec, format, digitsByKeyCurrency);
			return dec;
		}
	}
}