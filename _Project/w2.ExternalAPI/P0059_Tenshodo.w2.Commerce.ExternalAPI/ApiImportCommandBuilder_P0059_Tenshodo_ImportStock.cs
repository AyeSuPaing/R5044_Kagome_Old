/*
=========================================================================================================
  Module      : 天賞堂 在庫情報入力クラス(ApiImportCommandBuilder_P0059_Tenshodo_ImportStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using w2.App.Common.Stock;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.App.Common.Properties;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;
using w2.Common.Logger;

namespace P0059_Tenshodo.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// 天賞堂 在庫情報入力クラス
	/// </summary>
	public class ApiImportCommandBuilder_P0059_Tenshodo_ImportStock : ApiImportCommandBuilder
	{
		/// <summary>
		/// Import実行
		/// </summary>
		/// <param name="apiEntry"></param>
		protected override void Import(ApiEntry apiEntry)
		{
			FileLogger.Write("ImportStock", " Import実行");
			var stock = 0;

			// 分割データから必要な値とってくる
			var shopId = (string)apiEntry.Data[0];
			var productID = (string)apiEntry.Data[2];
			var variationID = (string)apiEntry.Data[2];
			int.TryParse((string)apiEntry.Data[3], out stock);

			FileLogger.Write("ImportStock", " shopId：" + shopId);
			FileLogger.Write("ImportStock", " productID：" + productID);
			FileLogger.Write("ImportStock", " variationID：" + variationID);
			FileLogger.Write("ImportStock", " stock：" + stock);

			// 注文数取得(最初だけ)
			if (OrderQuantity == null)
			{
				var orderCommon = new OrderCommon();
				var dv = orderCommon.GetOrderQuantity();

				// 在庫情報取得チェック
				if (dv.Count == 0)
				{
					FileLogger.Write("ImportStock", "在庫数計算の対象となる注文がありませんでした。注文個数を0として在庫数を計算します。");
					OrderQuantity = new DataTable();
				} else {
					OrderQuantity = dv.ToTable();
				}

				FileLogger.Write("ImportStock", "注文数取得(最初だけ)");
			}

			// 店舗コードが「0605」(オンラインストア)の場合のみ、在庫数計算してｗ２側在庫更新
			if (shopId == CustomConstants.ONLINE_STORE_CODE)
			{
				FileLogger.Write("ImportStock", "在庫数計算");
				// 在庫数計算
				var orderItemQuantity = OrderQuantity.AsEnumerable().Where(quantity => (string)quantity["product_id"] == productID && (string)quantity["variation_id"] == variationID)
																	.Select(quantity => (int)quantity["order_item_quantity"]);
				stock -= orderItemQuantity.FirstOrDefault();
				FileLogger.Write("ImportStock", "計算後stock：" + stock);

				// 在庫数（絶対値）更新
				// 在庫履歴の同期フラグは同期済の状態で作成
				var stockCommon = new StockCommon();
				stockCommon.SubtractOrderQuantityFromStock(
						productID,
						variationID,
						stock
						);
			}
		}

		// 注文個数保持用
		private static DataTable OrderQuantity { get; set; }
	}
}