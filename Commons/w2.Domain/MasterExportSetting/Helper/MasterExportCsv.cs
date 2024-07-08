/*
=========================================================================================================
  Module      : マスタCSV出力クラス(MasterExportCsv.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.ProductTaxCategory;

namespace w2.Domain.MasterExportSetting.Helper
{
	/// <summary>
	/// マスタCSV出力クラス
	/// </summary>
	public class MasterExportCsv
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="setting">マスタ出力設定</param>
		/// <param name="reader">SQLステートメントリーダー</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		public void Exec(
			MasterExportSettingModel setting,
			SqlStatementDataReader reader,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var masterExportSettingService = new MasterExportSettingService();
			var types = masterExportSettingService.GetMasterExportSettingTypes(setting.MasterKbn);
			if (setting.MasterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER)
			{
				// ユーザー属性
				var userAttributeTypes =
					masterExportSettingService.GetMasterExportSettingTypes(
						Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE);
				userAttributeTypes.Keys.Cast<string>().ToList().ForEach(key => types[key] = userAttributeTypes[key]);
			}

			var taxFieldNames = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
			foreach (var taxFieldName in taxFieldNames)
			{
				types[taxFieldName] = "price";
			}
			using (var streamWriter = new StreamWriter(outputStream, Encoding.GetEncoding(Constants.MASTEREXPORT_CSV_ENCODE)))
			{
				var splitFields = StringUtility.SplitCsvLine(setting.Fields);

				// ヘッダ情報出力
				if (reader.HasRows)
				{
					var headLine = MasterExportHelper.CreateCsvLine(splitFields);
					streamWriter.WriteLine(headLine);
				}

				// 1行ずつ作成して書き込む
				while (reader.Read())
				{
					// 行情報作成
					var lineData = new Hashtable();
					for (var i = 0; i < reader.FieldCount; i++)
					{
						var fieldName = reader.GetName(i);
						lineData[fieldName] = MasterExportHelper.ConvertData(setting, fieldName, reader[i]);

						// 日付変換
						if (types.ContainsKey(fieldName)
							&& ((string)types[fieldName] == "datetime")
							&& (string.IsNullOrEmpty(lineData[fieldName].ToString()) == false))
						{
							lineData[fieldName] = ((DateTime)lineData[fieldName]).ToString(formatDate);
						}

						//基軸通貨 小数点以下の有効桁数変換
						if (types.ContainsKey(fieldName)
							&& ((string)types[fieldName] == "price"))
						{
							var temp = StringUtility.ToEmpty(lineData[fieldName]);
							if (string.IsNullOrEmpty(temp) == false)
							{
								lineData[fieldName] = MasterExportHelper.ConvertPriceByKeyCurrency(
									decimal.Parse(temp),
									digitsByKeyCurrency,
									replacePrice);
							}
						}

						if (MasterExportHelper.IsGetTargetOfCombineFixedPurchaseData(fieldName, setting.MasterKbn))
						{
							var listFixedPurchaseParents = MasterExportHelper.GetParentOrderCombineFixedPurchase(StringUtility.ToEmpty(lineData[MasterExportHelper.ORDER_ID_FOR_GET_FIXED_PURCHASE_SHIPPING_DATES]));

							if (fieldName.Contains(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN))
							{
								lineData[fieldName] = MasterExportHelper.GetDataTimeOfFixedPurchaseParents(
									StringUtility.ToEmpty(lineData[fieldName]),
									fieldName,
									listFixedPurchaseParents);
							}
							else
							{
								lineData[fieldName] = MasterExportHelper.GetDataOfFixedPurchaseParents(
									StringUtility.ToEmpty(lineData[fieldName]),
									fieldName,
									listFixedPurchaseParents);
							}
						}
					}
					MasterExportHelper.ConvertDatas(setting, lineData);

					// 行CSV作成
					var csvLine = MasterExportHelper.CreateCsvLine(splitFields.Select(f => StringUtility.ToEmpty(lineData[f])));
					streamWriter.WriteLine(csvLine);
				}
			}
		}
	}
}
