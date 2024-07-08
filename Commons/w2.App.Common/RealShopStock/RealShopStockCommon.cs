/*
=========================================================================================================
  Module      : 外部連携用リアル店舗在庫関連処理パーシャルクラス(RealShopStockCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using w2.App.Common.Properties;
using System.Text;
using w2.App.Common.Util;
using w2.Common.Sql;

namespace w2.App.Common.RealShopStock
{
	/// <summary>
	/// 外部連携用リアル店舗在庫関連処理
	/// </summary>
	public partial class RealShopStockCommon
	{
		public const string LAST_CHANGED_API = "API";

		/// <summary>
		/// リアル店舗在庫数更新（追加 OR 更新）
		/// </summary>
		/// <param name="realShopID">リアル店舗ID</param>
		/// <param name="productID">商品ID</param>
		/// <param name="variationID">バリエーションID</param>
		/// <param name="realStock">リアル店舗在庫数</param>
		public void SetRealStockQuantity(string realShopID, string productID, string variationID, int realStock)
		{
			int resultCount = 0; // 実行件数

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				//更新パラメータセット
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID, realShopID);
				input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID, productID);
				input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID, variationID);
				input.Add(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK, realStock);
				input.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, LAST_CHANGED_API);

				// リアル店舗在庫情報取得
				DataView realProductStock;
				using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "RealStock", "RealStock", "GetRealProductStock"))
				{
					realProductStock = sqlStatement.SelectSingleStatement(sqlAccessor, input);
				}

				// 既にリアル店舗在庫情報が存在する場合は、更新用ステートメントをセット
				string statementName = "InsertRealProductStock";
				if (realProductStock.Count == 1) { statementName = "UpdateRealProductStock"; }

				// 実行
				using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "RealStock", "RealStock", statementName))
				{
					resultCount = sqlStatement.ExecStatement(sqlAccessor, input);
				}

				// 実行結果が0件はエラー
				if (resultCount == 0)
				{
					sqlAccessor.RollbackTransaction();
					sqlAccessor.CloseConnection();
					StringBuilder errorMessage = new StringBuilder();
					errorMessage.Append("リアル店舗在庫情報の更新に失敗しました。更新結果件数：" + resultCount.ToString(CultureInfo.InvariantCulture) + "件").Append("\r\n");
					errorMessage.Append("下記更新パラメータ").Append("\r\n");
					errorMessage.Append(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID).Append(":").Append(realShopID).Append("\r\n");
					errorMessage.Append(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID).Append(":").Append(productID).Append("\r\n");
					errorMessage.Append(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID).Append(":").Append(variationID).Append("\r\n");
					errorMessage.Append(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK).Append(":").Append(realStock);
					throw new Exception(errorMessage.ToString());
				}

				// コミット
				sqlAccessor.CommitTransaction();
				// 接続Close
				sqlAccessor.CloseConnection();
			}
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}

		/// <summary>
		/// リアル店舗在庫情報削除（指定日時以前のデータを削除）
		/// </summary>
		/// <param name="beforeTime">指定日時</param>
		public void DeleteRealProductStockBeforeTime(DateTime beforeTime)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "RealStock", "RealStock", "DeleteRealProductStockBeforeTime"))
			{
				Hashtable input = new Hashtable();
				input.Add("beforetime", beforeTime);
				sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}
	}
}
