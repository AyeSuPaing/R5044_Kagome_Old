/*
=========================================================================================================
  Module      : Feiler リアル店舗在庫情報入力クラス(ApiImportCommandBuilder_P0025_Feiler_ImportRealStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.ExternalAPI.Common.Command.ApiCommand.RealStock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;
using w2.App.Common.RealShopStock;

namespace P0025_Feiler.w2.Commerce.ExternalAPI
{
	public class ApiImportCommandBuilder_P0025_Feiler_ImportRealStock : ApiImportCommandBuilder
	{
		static DateTime m_beforeTime;

		/// <summary>
		/// 実行前処理
		/// </summary>
		public override void PreDo()
		{
			// 実行前日時をセット
			m_beforeTime = DateTime.Now.AddSeconds(-5);
		}

		#region #Import インポート処理の実装
		/// <summary>
		/// インポート処理の実装
		/// </summary>
		/// <param name="apiEntry">処理対象の情報を持つApiEntry</param>
		protected override void Import(ApiEntry apiEntry)
		{
			//分割したデータを元にコマンド用引数クラス生成
			SetRealStockQuantityArg setRealStockQuantityArg = GetArg(apiEntry);

			// コマンド実行
			SetRealStockQuantity setRealStockQuantity = new SetRealStockQuantity();
			SetRealStockQuantityResult setStockQuantityResult = (SetRealStockQuantityResult)setRealStockQuantity.Do(setRealStockQuantityArg);
		}

		/// <summary>
		/// 実行後処理
		/// </summary>
		public override void PostDo()
		{
			// 取り込まれなかったリアル店舗在庫情報を削除
			RealShopStockCommon realStockCommon = new RealShopStockCommon();
			realStockCommon.DeleteRealProductStockBeforeTime(m_beforeTime);
		}

		/// <summary>
		/// 引数設定
		/// </summary>
		/// <param name="apiEntry">処理対象の情報を持つApiEntry</param>
		/// <returns></returns>
		public static SetRealStockQuantityArg GetArg(ApiEntry apiEntry)
		{
			// ■変換内容
			// 商品ID：column2 + column3 + column4
			// 商品バリエーションID：商品ID
			string productId = String.Format("{0}{1}{2}",
				apiEntry.Data[1].ToString(),
				apiEntry.Data[2].ToString(),
				apiEntry.Data[3].ToString());

			SetRealStockQuantityArg setRealStockQuantityArg = new SetRealStockQuantityArg
			{
				RealShopID = apiEntry.Data[0].ToString(),
				ProductID = productId,
				VariationID = productId,
				RealStock = Convert.ToInt32(apiEntry.Data[4])
			};

			return setRealStockQuantityArg;
		}
		#endregion
	}
}
