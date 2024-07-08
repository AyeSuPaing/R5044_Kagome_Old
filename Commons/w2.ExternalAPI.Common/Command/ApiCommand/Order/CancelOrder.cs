/*
=========================================================================================================
  Module      : CancelOrderコマンドクラス(CancelOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.ExternalAPI.Common.Logging;


namespace w2.ExternalAPI.Common.Command.ApiCommand.Order
{
	#region Command
	/// <summary>
	/// CancelOrderコマンドクラス
	/// </summary>
	public class CancelOrder : ApiCommandBase
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
			CancelOrderArg cancelOrderArg = (CancelOrderArg) apiCommandArg;

			// 引数情報をログに
			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
			                string.Format("OrderId:'{0}',DoesMail:'{1}',ApiMemo:'{2}',IsOverwriteMemo:'{3}'",
										  cancelOrderArg.OrderId ?? "Null",
			                              cancelOrderArg.DoesMail.ToString(),
										  cancelOrderArg.ApiMemo ?? "Null",
			                              cancelOrderArg.IsOverwriteMemo.ToString()
			                	));
		
			OrderCommon.CancelOrder(
				cancelOrderArg.OrderId,
				cancelOrderArg.DoesMail,
				cancelOrderArg.ApiMemo,
				cancelOrderArg.IsOverwriteMemo,
				UpdateHistoryAction.Insert);

			// 正常終了列挙体セット
			CancelOrderResult cancelOrderResult = new CancelOrderResult(EnumResultStatus.Complete);

			return cancelOrderResult;
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// CancelOrderコマンド用引数クラス
	/// </summary>
	public class CancelOrderArg : ApiCommandArgBase
	{
		#region プロパティ
		/// <summary>注文IDプロパティ</summary>
		public string OrderId { get; set; }
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
		#endregion
	}
	#endregion

	#region Result
	/// <summary>
	/// CancelOrderコマンド用実行結果クラス
	/// </summary>
	public class CancelOrderResult : ApiCommandResult
	{
		public CancelOrderResult(EnumResultStatus enumResultStatus) : base(enumResultStatus)
		{
		}
	}
	#endregion

}
