/*
=========================================================================================================
  Module      : ナラカミ リアル店舗在庫情報入力クラス(ApiImportCommandBuilder_P0022_Naracamicie_ImportRealStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.ExternalAPI.Common.Command.ApiCommand.RealStock;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;
using System.Globalization;
using w2.App.Common.RealShopStock;

namespace P0022_Naracamicie.w2.Commerce.ExternalAPI
{
	public class ApiImportCommandBuilder_P0022_Naracamicie_ImportRealStock : ApiImportCommandBuilder
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
			// 商品ID＆商品バリエーションIDを商品マスタのフォーマットに変換
			// ■変換内容
			// 商品ID：column2の1～2文字目 + "-" + column2の3～4文字目 + "-" + column2の5～6文字目 + "-" + column2の7～9文字目
			// 商品バリエーションID：商品ID + "-" + column4 + "-" + column3
			StringBuilder productId = new StringBuilder();
			productId.Append(apiEntry.Data[1].ToString().Substring(0, 2));
			productId.Append("-").Append(apiEntry.Data[1].ToString().Substring(2, 2));
			productId.Append("-").Append(apiEntry.Data[1].ToString().Substring(4, 2));
			productId.Append("-").Append(apiEntry.Data[1].ToString().Substring(6, 3));
			StringBuilder variationId = new StringBuilder();
			variationId.Append(productId.ToString());
			variationId.Append("-").Append(apiEntry.Data[3].ToString());
			variationId.Append("-").Append(apiEntry.Data[2].ToString());

			SetRealStockQuantityArg setRealStockQuantityArg = new SetRealStockQuantityArg
			{
				RealShopID = apiEntry.Data[0].ToString(),
				ProductID = productId.ToString(),
				VariationID = variationId.ToString(),
				RealStock = Convert.ToInt32(apiEntry.Data[4])
			};
			return setRealStockQuantityArg;
		}
		#endregion
	}
}
