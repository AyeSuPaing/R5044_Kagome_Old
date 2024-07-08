/*
=========================================================================================================
  Module      : ShipOrderコマンドクラス(ShipOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Order
{
	#region Command
	/// <summary>
	/// ShipOrderコマンドクラス
	/// </summary>
	public class ShipOrder : ApiCommandBase
	{
		#region コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			// Arg取得
			ShipOrderArg shipOrderArg  = (ShipOrderArg)apiCommandArg;

			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
							string.Format("OrderId:'{0}',ShippingNo:'{1}',ShippingCheckNo:'{2}',DoesMail:'{3}',ApiMemo:'{4}',IsOverwriteMemo:'{5}',ShippedDate:'{6}'",
										  shipOrderArg.OrderId ?? "Null",
										  shipOrderArg.ShippingNo.ToString(),
										  shipOrderArg.ShippingCheckNo ?? "Null",
										  shipOrderArg.DoesMail.ToString(),
										  shipOrderArg.ApiMemo ?? "Null",
										  shipOrderArg.IsOverwriteMemo.ToString(),
										  shipOrderArg.ShippedDate.HasValue ? shipOrderArg.ShippedDate.ToString() : "Null"
								));

			OrderCommon.ShipOrder(
				shipOrderArg.OrderId,
				shipOrderArg.ShippingNo,
				shipOrderArg.ShippingCheckNo,
				shipOrderArg.DoesMail,
				shipOrderArg.ApiMemo,
				shipOrderArg.IsOverwriteMemo,
				shipOrderArg.ShippedDate.HasValue ? shipOrderArg.ShippedDate.Value : DateTime.Today,
				UpdateHistoryAction.Insert);

			// 正常終了列挙体セット
			ShipOrderResult shipOrderResult = new ShipOrderResult(EnumResultStatus.Complete);
	
			return shipOrderResult;
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// ShipOrderコマンド用引数クラス
	/// </summary>
	public class ShipOrderArg : ApiCommandArgBase
	{
		#region プロパティ
		/// <summary>注文IDプロパティ</summary>
		public string OrderId { get; set; }
		/// <summary>配送先枝番プロパティ</summary>
		public int ShippingNo { get; set; }
		/// <summary>配送伝票番号プロパティ</summary>
		public string ShippingCheckNo { get; set; }
		/// <summary>注文キャンセルメール送信フラグプロパティ</summary>
		public bool DoesMail { get; set; }
		/// <summary>外部連携メモプロパティ</summary>
		public string ApiMemo { get; set; }
		/// <summary>
		/// 外部連携メモ上書きプロパティ
		/// True:上書き
		/// False：追記
		/// </summary>
		public bool IsOverwriteMemo { get; set; }
		/// <summary>出荷完了日</summary>
		public DateTime? ShippedDate { get; set; }
		#endregion
	}
	
	#endregion

	#region Result
	/// <summary>
	/// ShipOrderコマンド用実行結果クラス
	/// </summary>
	public class ShipOrderResult : ApiCommandResult
	{
		public ShipOrderResult(EnumResultStatus enumResultStatus) : base(enumResultStatus)
		{
		}
	}
	#endregion
	
}
