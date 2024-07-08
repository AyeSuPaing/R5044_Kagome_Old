using System;
using System.Data;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 注文ID及びステータスの指定は無しで、35分前から5前の間に作成された注文情報を取得する汎用コマンドビルダ
	/// </summary>
    public class ApiExportCommandBuilder_SimpleCommandLibrary_E_0010 : ApiExportCommandBuilder
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
		   GetOrders cmd = new GetOrders();

		   GetOrdersArg arg = new GetOrdersArg
		   {
			   // 注文情報作成日　開始時間、終了時間を指定
			   CreatedTimeSpan = new PastAbsoluteTimeSpan(-5, DateTime.Now.AddMinutes(-35)),
			   // 注文情報更新日は指定なし
			   // 注文ステータス
			   OrderStatus = "",

			   // 注文ID
			   OrderId = ""
		   };

		   // コマンド実行
		   GetOrdersResult result = (GetOrdersResult)cmd.Do(arg);

		   return result.ResultTable;
	   }
	   #endregion
   }
}
