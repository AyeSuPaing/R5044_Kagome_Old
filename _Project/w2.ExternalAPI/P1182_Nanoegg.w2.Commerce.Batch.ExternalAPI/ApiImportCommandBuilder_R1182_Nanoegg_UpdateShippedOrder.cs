/*
=========================================================================================================
Module      : R1182_Nanoegg専用出荷受注更新処理モジュール(ApiImportCommandBuilder_R1182_Nanoegg_UpdateShippedOrder.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Logging;
using w2.Common.Logger;

namespace R1182_Nanoegg.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// R1182_Nanoegg専用出荷受注更新処理クラス
	/// </summary>
	public class ApiImportCommandBuilder_R1182_Nanoegg_UpdateShippedOrder : ApiImportCommandBuilder
	{
		#region #Import インポート処理の実装
		/// <summary>
		/// インポート処理の実装
		/// </summary>
		/// <param name="apiEntry">処理対象の情報を持つApiEntry</param>
		protected override void Import(ApiEntry apiEntry)
		{
			try
			{
				//分割したデータを元にコマンド用引数クラス生成
				var shipOrderArg = new ShipOrderArg
				{
					OrderId = (string)apiEntry.Data[0],
					ShippingCheckNo = (string)apiEntry.Data[1],
					ShippedDate = DateTime.ParseExact(apiEntry.Data[2].ToString(), "yyyyMMdd", null),
					ApiMemo = "",
					// ギフト利用想定が無いので、固定で１をセットする
					ShippingNo = 1,
					// 出荷完了メールは送信しない
					DoesMail = false
				};

				//コマンド生成
				var shipOrder = new ShipOrder();
				//コマンド実行
				var shipOrderResult = (ShipOrderResult)shipOrder.Do(shipOrderArg);

				//コマンド成功可否
				if (shipOrderResult.ResultStatus == EnumResultStatus.Complete)
				{
					ApiLogger.Write(LogLevel.info, "[UpdateShippedOrder:Success]",
						string.Format("Order_id:{0}, ShippingCheckNo:{1}, ShippedDate{2}",
							shipOrderArg.OrderId,
							shipOrderArg.ShippingCheckNo,
							shipOrderArg.ShippedDate));
				}
				else
				{
					ApiLogger.Write(LogLevel.error, "[UpdateShippedOrder:Error]",
						string.Format("Order_id:{0}, ShippingCheckNo:{1}, ShippedDate{2}",
							shipOrderArg.OrderId,
							shipOrderArg.ShippingCheckNo,
							shipOrderArg.ShippedDate));
				}
			}
			catch (Exception ex)
			{
				// CSVの内容を取得(取得できる範囲でmessage作成)
				var message = string.Join("," ,
					Enumerable.Range(0, apiEntry.Data.ItemArray.Length)
						.Select(i => "csvRow[" + i + "]" + apiEntry.Data.ItemArray[i])
						.ToArray());
				
				// 失敗したケースのみデータをエラーログに出力
				FileLogger.WriteError("コマンド引数情報:" + GetType().Name + message, ex);
				
				// ExternalApiLogにログ内容Insert
				ApiLogger.Write(LogLevel.error,
				"コマンド引数情報:" + GetType().Name,
				message,
				ex);
				throw;
			}
		}
		#endregion
	}
}
