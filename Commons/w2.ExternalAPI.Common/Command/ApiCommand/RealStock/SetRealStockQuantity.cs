/*
=========================================================================================================
  Module      : SetRealStockQuantityコマンドクラス(SetRealStockQuantity.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.RealShopStock;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.RealStock
{
	#region Command
	/// <summary>
	/// SetStockQuantityコマンドクラス
	/// </summary>
	public class SetRealStockQuantity : ApiCommandBase
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
			SetRealStockQuantityArg setRealStockQuantityArg = (SetRealStockQuantityArg)apiCommandArg;

			//引数情報をログに
			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
				string.Format("RealShopID:'{0}',ProductID:'{1}',VariationID:'{2}',RealStock:'{3}'",
				setRealStockQuantityArg.RealShopID ?? "Null",
				setRealStockQuantityArg.ProductID ?? "Null",
				setRealStockQuantityArg.VariationID ?? "Null",
				setRealStockQuantityArg.RealStock.ToString()
				));

			//API呼び出し
			RealShopStockCommon realStockCommon = new RealShopStockCommon();
			realStockCommon.SetRealStockQuantity(
				setRealStockQuantityArg.RealShopID,
				setRealStockQuantityArg.ProductID,
				setRealStockQuantityArg.VariationID,
				setRealStockQuantityArg.RealStock);

			//正常終了列挙体セット
			SetRealStockQuantityResult setStockQuantityResult = new SetRealStockQuantityResult(EnumResultStatus.Complete);

			return setStockQuantityResult;

		}
		#endregion

	}
	#endregion

	#region Arg
	/// <summary>
	/// SetRealStockQuantityコマンド引数クラス
	/// </summary>
	public class SetRealStockQuantityArg : ApiCommandArgBase
	{
		/// <summary>リアル店舗ID</summary>
		public string RealShopID { get; set; }
		/// <summary>商品ID</summary>
		public string ProductID { get; set; }
		/// <summary>商品バリエーションID</summary>
		public string VariationID { get; set; }
		/// <summary>リアル店舗在庫数</summary>
		public int RealStock { get; set; }
	}

	#endregion

	#region Result
	/// <summary>
	/// SetRealStockQuantityコマンド実行結果クラス
	/// </summary>
	public class SetRealStockQuantityResult : ApiCommandResult
	{
		public SetRealStockQuantityResult(EnumResultStatus enumResultStatus)
			: base(enumResultStatus)
		{
		}

		public int MailCount { get; set; }
	}

	#endregion
}
