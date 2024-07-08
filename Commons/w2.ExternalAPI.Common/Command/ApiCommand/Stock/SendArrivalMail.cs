/*
=========================================================================================================
  Module      : SendArrivalMailコマンドクラス(SendArrivalMail.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Stock;

using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Stock
{
	#region Command
 	/// <summary>
	/// SendArrivalMailコマンドクラス
 	/// </summary>
	public class SendArrivalMail : ApiCommandBase
	{
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			//Arg取得
			SendArrivalMailArg sendArrivalMailArg = (SendArrivalMailArg) apiCommandArg;

			//引数情報をログに
			ApiLogger.Write(LogLevel.info,"コマンド引数情報:" + GetType().Name,
				string.Format("ProductID:'{0}',VariationID:'{1}',Stock:'{2}',UseSafetyCriteria:'{3}'",
				sendArrivalMailArg.ProductID ?? "Null",
				sendArrivalMailArg.VariationID ?? "Null",
				sendArrivalMailArg.Stock.ToString(),
				sendArrivalMailArg.UseSafetyCriteria.ToString()
				));

			//API呼び出し
			StockCommon stockCommon = new StockCommon();
			stockCommon.SendArrivalMail(
					sendArrivalMailArg.ProductID
					, sendArrivalMailArg.VariationID
					, sendArrivalMailArg.Stock
					,sendArrivalMailArg.UseSafetyCriteria 
					);

				//正常終了列挙体セット
				SendArrivalMailResult sendArrivalMailResult = new SendArrivalMailResult(EnumResultStatus.Complete);
			
			return sendArrivalMailResult;
		}
		#endregion

	}
	#endregion

	#region Arg
	/// <summary>
	/// SendArrivalMailコマンド用引数クラス
	/// </summary>
	public class SendArrivalMailArg : ApiCommandArgBase
	{
		/// <summary>商品ID</summary>
		public string ProductID { get; set; }
		/// <summary>商品バリエーションID</summary>
		public string VariationID { get; set; }
		/// <summary>在庫数</summary>
		public Nullable<int> Stock { get; set; }
		/// <summary>
		/// 基準値を指定する代わりに在庫安全基準値を使用する。
		/// ただし、Stockが指定されていればUseSafetyCriteriaの値にかかわらず常にStockの値が優先される。
		/// </summary>
		public bool UseSafetyCriteria { get; set; }
		
	}
	#endregion

	#region Result
	/// <summary>
	/// SendArrivalMailコマンド用実行毛化クラス
	/// </summary>
	public class SendArrivalMailResult : ApiCommandResult
	{
		public SendArrivalMailResult(EnumResultStatus enumResultStatus) : base(enumResultStatus)
		{
		}
	}
	#endregion
}
