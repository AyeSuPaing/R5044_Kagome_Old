using System;
using System.Data;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	///  注文ID及びステータスの指定は無しで、5分前から1日前の間に更新された注文配送先情報を取得する汎用コマンドビルダ
	/// </summary>
    public class ApiExportCommandBuilder_SimpleCommandLibrary_E_0024 : ApiExportCommandBuilder
   {
	   #region #Export 出力処理
	   /// <summary>出力処理</summary>
	   protected override object[] Export(IDataRecord record)
	   {
		   return record.ToArray();
	   }
	   #endregion

	   #region #Init 初期化処理
	   /// <summary>初期化処理</summary>
	   public override DataTable GetDataTable()
	   {
		   //APIコマンド作る
		   GetOrderShippings cmd = new GetOrderShippings();

		   GetOrderShippingsArg arg = new GetOrderShippingsArg
		   {
			   // 注文情報作成日　開始時間、終了時間を指定
			   UpdatedTimeSpan = new PastAbsoluteTimeSpan(-5, DateTime.Now.AddDays(-1).AddMinutes(-5)),
			   // 注文ステータス
			   OrderStatus = "",

			   // 注文ID
			   OrderId = ""
		   };

		   // コマンド実行
		   GetOrderShippingsResult result = (GetOrderShippingsResult)cmd.Do(arg);

		   return result.ResultTable;
	   }
	   #endregion
   }
}
