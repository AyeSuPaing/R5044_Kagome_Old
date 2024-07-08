/*
=========================================================================================================
  Module      : マスタEXCEL出力クラス(MasterExportExcel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using w2.Common.Util;

namespace w2.Domain.MasterExportSetting.Helper
{
	/// <summary>
	/// マスタEXCEL出力クラス
	/// </summary>
	public class MasterExportExcel
	{
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="source">ソース</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		public void Exec(
			MasterExportSettingModel setting,
			ExcelTemplateSetting excelTemplateSetting,
			DataView source,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var splitFields = StringUtility.SplitCsvLine(setting.Fields);

			// シリアルキー項目が存在するかチェック
			if (splitFields.Contains(Constants.FIELD_SERIALKEY_SERIAL_KEY) || splitFields.Contains("serial_keys"))
			{
				foreach (DataRowView line in source)
				{
					for (var i = 0; i < splitFields.Length; i++)
					{
						line[i] = MasterExportHelper.ConvertData(setting, splitFields[i], line[i]);
					}
				}
			}

			var masterExportSettingService = new MasterExportSettingService();
			var types = masterExportSettingService.GetMasterExportSettingTypes(setting.MasterKbn);
			for (var colIndex = 0; colIndex < splitFields.Length; colIndex++)
			{
				if (types.ContainsKey(splitFields[colIndex]) && ((string)types[splitFields[colIndex]] == "price"))
				{
					for (var rowIndex = 0; rowIndex < source.Count; rowIndex++)
					{
						var temp = StringUtility.ToEmpty(source[rowIndex][colIndex]);
						if (string.IsNullOrEmpty(temp) == false)
						{
							source[rowIndex][colIndex] = MasterExportHelper.ConvertPriceByKeyCurrency(decimal.Parse(temp), digitsByKeyCurrency, replacePrice);
						}
					}
				}

				if (MasterExportHelper.IsGetTargetOfCombineFixedPurchaseData(StringUtility.ToEmpty(splitFields[colIndex]), setting.MasterKbn))
				{
					var fieldName = StringUtility.ToEmpty(splitFields[colIndex]);
					var fixedPurchaseDic = new Dictionary<int, string>();
					for (var rowIndex = 0; rowIndex < source.Count; rowIndex++)
					{
						var value = StringUtility.ToEmpty(source[rowIndex][colIndex]);
						if (string.IsNullOrEmpty(value) == false)
						{
							var listFixedPurchaseParents = MasterExportHelper.GetParentOrderCombineFixedPurchase(StringUtility.ToEmpty(source[rowIndex][MasterExportHelper.ORDER_ID_FOR_GET_FIXED_PURCHASE_SHIPPING_DATES]));

							fixedPurchaseDic.Add(
								rowIndex,
								MasterExportHelper.GetDataOfFixedPurchaseParents(
									value,
									fieldName,
									listFixedPurchaseParents));
						}
					}

					if ((source[0][colIndex] is string) == false)
					{
						source.Table.Columns.RemoveAt(colIndex);
						var column = source.Table.Columns.Add(fieldName, typeof(string));
						column.SetOrdinal(colIndex);
					}

					foreach (var row in fixedPurchaseDic)
					{
						source[row.Key][colIndex] = row.Value;
					}
				}
			}

			var index = source.Table.Columns.IndexOf(MasterExportHelper.ORDER_ID_FOR_GET_FIXED_PURCHASE_SHIPPING_DATES);
			if (index != -1)
			{
				source.Table.Columns.RemoveAt(index);
			}

			var fields = setting.Fields.Split(',');
			for (var i = 0; i < fields.Length; i++)
			{
				source.Table.Columns[i].ColumnName =
					(fields.Take(i).Contains(fields[i]) == false)
						? fields[i]
						: fields[i] + fields.Take(i).Count(name => (name == fields[i]));
			}

			try
			{
				// エクセルファイル出力処理
				var meteMasterExport = new ExportToExcel(excelTemplateSetting);
				meteMasterExport.Create(source, outputStream, formatDate);
			}
			catch (Exception ex)
			{
				w2.Common.Logger.AppLogger.WriteError(ex);
			}
		}
	}
}