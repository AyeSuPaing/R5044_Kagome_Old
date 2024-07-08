/*
=========================================================================================================
  Module      : CompleteShipmentコマンドクラス(CompleteShipment.cs)
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
	/// CompleteShipmentコマンドクラス
	/// </summary>
	public class CompleteShipment : ApiCommandBase
	{
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			// Arg取得
			CompleteShipmentArg completeShipmentArg = (CompleteShipmentArg)apiCommandArg;

			// 引数情報をログに
			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
							string.Format("OrderId:'{0}',ShippingNo:'{1}',ShippingCheckNo:'{2}',DoesMail:'{3}',ApiMemo:'{4}',IsOverwriteMemo:'{5}',DeliveringDate:'{6}'",
										  completeShipmentArg.OrderId ?? "Null",
										  completeShipmentArg.ShippingNo.ToString(),
										  completeShipmentArg.ShippingCheckNo ?? "Null",
										  completeShipmentArg.DoesMail.ToString(),
										  completeShipmentArg.ApiMemo ?? "Null",
										  completeShipmentArg.IsOverwriteMemo.ToString(),
										  completeShipmentArg.DeliveringDate.HasValue ? completeShipmentArg.DeliveringDate.Value.ToString() : "Null"
								));

			OrderCommon.CompleteShipment(
				completeShipmentArg.OrderId,
				completeShipmentArg.ShippingNo,
				completeShipmentArg.ShippingCheckNo,
				completeShipmentArg.DoesMail,
				completeShipmentArg.ApiMemo,
				completeShipmentArg.IsOverwriteMemo,
				completeShipmentArg.DeliveringDate.HasValue ? completeShipmentArg.DeliveringDate.Value : DateTime.Today,
				UpdateHistoryAction.Insert);

			// 正常終了列挙体セット
			CompleteShipmentResult completeShipmentResult = new CompleteShipmentResult(EnumResultStatus.Complete);
			

			return completeShipmentResult;
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// CompleteShipmentコマンド用引数クラス
	/// </summary>
	public class CompleteShipmentArg : ApiCommandArgBase
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
		/// <summary> 配送完了日 </summary>
		public DateTime? DeliveringDate { get; set; }

		#endregion
	}
	#endregion

	#region Result
	/// <summary>
	/// CompleteShipmentコマンド用実行結果クラス
	/// </summary>
	public class CompleteShipmentResult : ApiCommandResult
	{
		public CompleteShipmentResult(EnumResultStatus enumResultStatus) : base(enumResultStatus)
		{
		}
	}
	#endregion


}
