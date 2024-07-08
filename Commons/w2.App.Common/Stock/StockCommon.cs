/*
=========================================================================================================
  Module      : 外部連携用在庫関連処理パーシャルクラス(StockCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
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
using w2.Common.Logger;

namespace w2.App.Common.Stock
{
	/// <summary>
	/// 外部連携用在庫関連処理
	/// </summary>
	public partial class StockCommon
	{
		public const string DEFAULT_MAILTEMPLATE_ARRIVAL = "00000110";	// 再入荷通知のデフォルトメールテンプレート
		public const string LAST_CHANGED_API = "API";

		#region +SetStockQuantity 在庫数を更新する(絶対値)
		/// <summary>
		/// 在庫数を更新する(絶対値)
		/// </summary>
		/// <param name="productID">対象商品ID</param>
		/// <param name="variationID">対象商品バリエーションID</param>
		/// <param name="stock">更新する在庫数（絶対値）</param>
		public void SetStockQuantity(string productID, string variationID, int stock)
		{
			SetStockQuantity(productID, variationID, stock, false );
		}
		#endregion

		#region +SetStockQuantity 同期フラグを指定して在庫数を更新する(絶対値)
		/// <summary>
		/// 在庫数を更新する(絶対値)
		/// </summary>
		/// <param name="productID">対象商品ID</param>
		/// <param name="variationID">対象商品バリエーションID</param>
		/// <param name="stock">更新する在庫数（絶対値）</param>
		/// <param name="sync_flg">在庫履歴テーブルに追加する際の同期フラグ</param>
		/// <remarks>在庫安全基準値を下回った場合は在庫数0で更新し、在庫履歴テーブルにその旨を登録する。</remarks>
		public void SetStockQuantity(string productID, string variationID, int stock, bool sync_flg)
		{
			int resultCount = 0; // 実行件数
	
			// 可読性上げるためにあえてトランザクション内の処理は分割しない
			// 明示的にかくことで　トランザクションを意識！
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				// 在庫情報の更新と、履歴の追加を同一トランザクション内で行う
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				// 在庫情報取得
				DataView dv = GetStock(productID, variationID);

				// 本メソッド引数に指定された在庫数の絶対値からトランザクション内で取得した在庫数を元に相対値を算出
				// 絶対値-現時点在庫数=相対値
				int relativeStock = stock - Convert.ToInt32(dv[0][Constants.FIELD_PRODUCTSTOCK_STOCK]);
			
				// 在庫情報更新
				using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "UpdateProductStock"))
				{
					Hashtable input = new Hashtable();
					//更新パラメータセット
					input.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productID);
					input.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationID);
					input.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, LAST_CHANGED_API);
					input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT, Convert.ToInt32(dv[0][Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]));
					input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, relativeStock);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, 0);
					resultCount = sqlStatement.ExecStatement(sqlAccessor, input);
				}

				// 実行結果が0件はエラー
				if (resultCount == 0)
				{
					sqlAccessor.RollbackTransaction();
					sqlAccessor.CloseConnection();
					StringBuilder errorMessage = new StringBuilder();
					errorMessage.Append("在庫情報の更新に失敗しました。更新結果件数：" + resultCount.ToString(CultureInfo.InvariantCulture) + "件").Append("\r\n");
					errorMessage.Append("下記更新パラメータ").Append("\r\n");
					errorMessage.Append("product_id:" + productID).Append("\r\n");
					errorMessage.Append("variation_id:" + variationID).Append("\r\n");
					errorMessage.Append("stock:" + stock);
					throw new Exception(errorMessage.ToString());
				}

				if (relativeStock != 0) // 在庫履歴追加は在庫が変動したときのみとする。
				{
					// 在庫履歴追加
					using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "InsertProductStockHistoryWithSyncFlg"))
					{
						Hashtable input = new Hashtable();
						// 更新パラメータセット
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, "");
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, 0);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productID);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationID);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_OPERATION);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, relativeStock);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, 0);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, "");
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, LAST_CHANGED_API);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG, sync_flg);
						sqlStatement.ExecStatement(sqlAccessor, input);
					}
				}
				// コミット
				sqlAccessor.CommitTransaction();
				// 接続Close
				sqlAccessor.CloseConnection();
			}
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +AddStockQuantity 在庫数を更新する(相対値)
		/// <summary>
		/// 在庫数を更新する(相対値)
		/// </summary>
		/// <param name="productID">対象商品ID</param>
		/// <param name="variationID">対象商品バリエーションID</param>
		/// <param name="stock">更新する在庫数（相対値）</param>
		public void AddStockQuantity(string productID, string variationID, int stock)
		{
			AddStockQuantity(productID, variationID, stock,false);
		}
		#endregion

		#region +AddStockQuantity 同期フラグを指定して在庫数を更新する(相対値)
		/// <summary>
		/// 在庫数を更新する(相対値)
		/// </summary>
		/// <param name="productID">対象商品ID</param>
		/// <param name="variationID">対象商品バリエーションID</param>
		/// <param name="stock">更新する在庫数（相対値）</param>
		/// <param name="sync_flg">同期フラグ</param>
		public void AddStockQuantity(string productID, string variationID, int stock, bool sync_flg)
		{
			// 対象在庫情報取得
			DataView dataView = GetStock(productID, variationID);
			// 在庫情報更新
			UpdateProductStock(
				"",
				"0",
				productID,
				variationID,
				LAST_CHANGED_API,
				Convert.ToInt32(dataView[0][Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]),
				stock,
				0,
				0,
				0,
				0,
				Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_OPERATION,
				"",
				true,
				sync_flg);
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +UpdateProductStock 在庫情報を相対値で更新して履歴データも追加する（商品単位）
		/// <summary>
		/// 在庫情報を相対値で更新して履歴データも追加する（商品単位）
		/// 更新件数が0件の場合はエラーをThrow
		/// </summary>
		/// <param name="orderID">注文ID</param>
		/// <param name="shopID">店舗ID</param>
		/// <param name="productID">商品ID</param>
		/// <param name="variationID">バリエーションID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="stockAlert">商品在庫安全基準</param>
		/// <param name="addStock">商品在庫数</param>
		/// <param name="addRealStock">実在庫数</param>
		/// <param name="addRealStockB">実在庫数B</param>
		/// <param name="addRealStockC">実在庫数C</param>
		/// <param name="addRealStockReserved">引当済実在庫数</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="updateMemo">在庫更新メモ</param>
		/// <returns>実行件数</returns>
		public int UpdateProductStock(
			string orderID,
			string shopID,
			string productID,
			string variationID,
			string lastChanged,
			int stockAlert,
			int addStock,
			int addRealStock,
			int addRealStockB,
			int addRealStockC,
			int addRealStockReserved,
			string actionStatus,
			string updateMemo
			)
		{
			return UpdateProductStock(orderID, 
				shopID, 
				productID, 
				variationID, 
				lastChanged, 
				stockAlert, 
				addStock, 
				addRealStock, 
				addRealStockB, 
				addRealStockC, 
				addRealStockReserved, 
				actionStatus, 
				updateMemo, 
				true);
		}
		/// <summary>
		/// 在庫情報を相対値で更新して履歴データも追加する（商品単位）
		/// 更新件数が0件の場合はエラーをThrow
		/// 履歴データの作成は insertStockHistoryFlg が true の場合のみ
		/// </summary>
		/// <param name="orderID">注文ID</param>
		/// <param name="shopID">店舗ID</param>
		/// <param name="productID">商品ID</param>
		/// <param name="variationID">バリエーションID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="stockAlert">商品在庫安全基準</param>
		/// <param name="addStock">商品在庫数</param>
		/// <param name="addRealStock">実在庫数</param>
		/// <param name="addRealStockB">実在庫数B</param>
		/// <param name="addRealStockC">実在庫数C</param>
		/// <param name="addRealStockReserved">引当済実在庫数</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="updateMemo">在庫更新メモ</param>
		/// <param name="insertStockHistoryFlg">在庫履歴を作成するかどうか</param>
		/// <returns>実行件数</returns>
		public int UpdateProductStock(
			string orderID,
			string shopID,
			string productID,
			string variationID,
			string lastChanged,
			int stockAlert,
			int addStock,
			int addRealStock,
			int addRealStockB,
			int addRealStockC,
			int addRealStockReserved,
			string actionStatus,
			string updateMemo,
			bool insertStockHistoryFlg
			)
		{
			return UpdateProductStock(orderID,shopID,productID,variationID,lastChanged,stockAlert,addStock,
			    addRealStock, addRealStockB, addRealStockC, addRealStockReserved, actionStatus, updateMemo,insertStockHistoryFlg,false);
		}
		#endregion

		#region +UpdateProductStock 在庫情報を相対値で更新して履歴データも追加する（商品単位）
		/// <summary>
		/// 在庫情報を相対値で更新して履歴データも追加する（商品単位）
		/// 更新件数が0件の場合はエラーをThrow
		/// 管理画面からはこのメソッドをつかってね
		/// </summary>
		/// <param name="orderID">注文ID</param>
		/// <param name="shopID">店舗ID</param>
		/// <param name="productID">商品ID</param>
		/// <param name="variationID">バリエーションID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="stockAlert">商品在庫安全基準</param>
		/// <param name="addStock">商品在庫数</param>
		/// <param name="addRealStock">実在庫数</param>
		/// <param name="addRealStockB">実在庫数B</param>
		/// <param name="addRealStockC">実在庫数C</param>
		/// <param name="addRealStockReserved">引当済実在庫数</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="updateMemo">在庫更新メモ</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>実行件数</returns>
		public int UpdateProductStock(
			string orderID,
			string shopID,
			string productID,
			string variationID,
			string lastChanged,
			int stockAlert,
			int addStock,
			int addRealStock,
			int addRealStockB,
			int addRealStockC,
			int addRealStockReserved,
			string actionStatus,
			string updateMemo,
			SqlAccessor sqlAccessor)
		{
			int resultCount = 0; // 実行件数
			
			// トランザクション開始
			// 在庫情報の更新と、履歴の追加を同一トランザクション内で行う
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			// 在庫情報更新
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "UpdateProductStock"))
			{
				Hashtable input = new Hashtable();
				// 更新パラメータセット
				input.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopID);
				input.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productID);
				input.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationID);
				input.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, lastChanged);
				input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT, stockAlert);
				input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, addStock);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, addRealStock);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, addRealStockB);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, addRealStockC);
				input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, addRealStockReserved);
				resultCount = sqlStatement.ExecStatement(sqlAccessor, input);
			}

			// 実行結果が0件はエラー
			if (resultCount == 0)
			{
				sqlAccessor.RollbackTransaction();
				sqlAccessor.CloseConnection();
				StringBuilder errorMessage = new StringBuilder();
				errorMessage.Append("在庫情報の更新に失敗しました。更新結果件数：" + resultCount.ToString(CultureInfo.InvariantCulture) + "件").Append("\r\n");
				errorMessage.Append("下記更新パラメータ").Append("\r\n");
				errorMessage.Append("product_id:" + productID).Append("\r\n");
				errorMessage.Append("variation_id:" + variationID).Append("\r\n");
				errorMessage.Append("stock:" + addStock);
				throw new Exception(errorMessage.ToString());
			}

			// 在庫履歴追加
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "InsertProductStockHistoryWithSyncFlg"))
			{
				Hashtable input = new Hashtable();
				//更新パラメータセット
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, orderID);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, shopID);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productID);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationID);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, actionStatus);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, addStock);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, addRealStock);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, addRealStockB);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, addRealStockC);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, addRealStockReserved);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, updateMemo);
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, lastChanged);
				// 管理画面からの場合はSyncFlgをfalseで登録
				input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG, false);
				sqlStatement.ExecStatement(sqlAccessor, input);
			}
			return resultCount;
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +UpdateProductStock 在庫情報を相対値で更新して履歴データも追加する（商品単位）
		/// <summary>
		/// 同期フラグを指定して在庫情報を相対値で更新して履歴データも追加する（商品単位）
		/// 更新件数が0件の場合はエラーをThrow
		/// 履歴データの作成は insertStockHistoryFlg が true の場合のみ
		/// </summary>
		/// <param name="orderID">注文ID</param>
		/// <param name="shopID">店舗ID</param>
		/// <param name="productID">商品ID</param>
		/// <param name="variationID">バリエーションID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="stockAlert">商品在庫安全基準</param>
		/// <param name="addStock">商品在庫数</param>
		/// <param name="addRealStock">実在庫数</param>
		/// <param name="addRealStockB">実在庫数B</param>
		/// <param name="addRealStockC">実在庫数C</param>
		/// <param name="addRealStockReserved">引当済実在庫数</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="updateMemo">在庫更新メモ</param>
		/// <param name="insertStockHistoryFlg">在庫履歴を作成するかどうか</param>
		/// <param name="sync_flg">同期フラグ</param>
		/// <returns>実行件数</returns>
		public int UpdateProductStock(
			string orderID,
			string shopID,
			string productID,
			string variationID,
			string lastChanged,
			int stockAlert,
			int addStock,
			int addRealStock,
			int addRealStockB,
			int addRealStockC,
			int addRealStockReserved,
			string actionStatus,
			string updateMemo,
			bool insertStockHistoryFlg,
			bool sync_flg
			)
		{
			int resultCount = 0; // 実行件数

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				// 在庫情報の更新と、履歴の追加を同一トランザクション内で行う
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();
				
				// 在庫情報更新
				using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "UpdateProductStock"))
				{
					Hashtable input = new Hashtable();
					// 更新パラメータセット
					input.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopID);
					input.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productID);
					input.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationID);
					input.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, lastChanged);
					input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT, stockAlert);
					input.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, addStock);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, addRealStock);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, addRealStockB);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, addRealStockC);
					input.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, addRealStockReserved);
					resultCount = sqlStatement.ExecStatement(sqlAccessor, input);
				}

				// 実行結果が0件はエラー
				if (resultCount == 0)
				{
					sqlAccessor.RollbackTransaction();
					sqlAccessor.CloseConnection();
					StringBuilder errorMessage = new StringBuilder();
					errorMessage.Append("在庫情報の更新に失敗しました。更新結果件数：" + resultCount.ToString(CultureInfo.InvariantCulture) + "件").Append("\r\n");
					errorMessage.Append("下記更新パラメータ").Append("\r\n");
					errorMessage.Append("product_id:" + productID).Append("\r\n");
					errorMessage.Append("variation_id:" + variationID).Append("\r\n");
					errorMessage.Append("stock:" + addStock);
					throw new Exception(errorMessage.ToString());
				}

				// 在庫履歴追加
				if (insertStockHistoryFlg)
				{
					using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "InsertProductStockHistoryWithSyncFlg"))
					{
						Hashtable input = new Hashtable();
						// 更新パラメータセット
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, orderID);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, shopID);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productID);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationID);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, actionStatus);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, addStock);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, addRealStock);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, addRealStockB);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, addRealStockC);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, addRealStockReserved);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, updateMemo);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, lastChanged);
						input.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG, sync_flg);
						sqlStatement.ExecStatement(sqlAccessor, input);
					}
				}
				// コミット
				sqlAccessor.CommitTransaction();
				// 接続Close
				sqlAccessor.CloseConnection();
			}
			return resultCount;
			//他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +GetStock 在庫情報取得
		/// <summary>
		/// 在庫情報取得
		/// </summary>
		/// <param name="productID">対象商品ID</param>
		/// <param name="variationID">対象バリエーションID</param>
		/// <returns>パラメタに指定された条件で取得したProductStockテーブルのデータを持つDataView</returns>
		public DataView GetStock(string productID, string variationID)
		{
			DataView dataView;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "GetProductStock"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productID);
				input.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationID);
				dataView = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}

			// 在庫情報取得チェック
			if (dataView.Count == 0)
			{
				StringBuilder errorMessage = new StringBuilder();
				errorMessage.Append("在庫情報が取得できませんでした。この在庫情報は存在しない可能性があります。").Append("\r\n");
				errorMessage.Append("取得パラメータ").Append("\r\n");
				errorMessage.Append("product_id:'" + productID + "'").Append("\r\n");
				errorMessage.Append("variation_id:'" + variationID + "'");
				throw new Exception(errorMessage.ToString());
			}
			return dataView;
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +GetStock 在庫情報取得

		/// <summary>
		/// 在庫情報取得
		/// </summary>
		/// <param name="productID">対象商品ID</param>
		/// <param name="variationID">対象バリエーションID</param>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <returns>パラメタに指定された条件で取得したProductStockテーブルのデータを持つDataView</returns>
		public DataView GetStock(string productID, string variationID, SqlAccessor sqlAccessor)
		{
			DataView dataView;

			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "GetProductStock"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, 0);
				input.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productID);
				input.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationID);
				dataView = sqlStatement.SelectSingleStatement(sqlAccessor, input);
			}

			// 在庫情報取得チェック
			if (dataView.Count == 0)
			{
				StringBuilder errorMessage = new StringBuilder();
				errorMessage.Append("在庫情報が取得できませんでした。この在庫情報は存在しない可能性があります。").Append("\r\n");
				errorMessage.Append("product_id:'" + productID + "'").Append("\r\n");
				errorMessage.Append("variation_id:'" + variationID + "'");
				throw new Exception(errorMessage.ToString());
			}
			return dataView;
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +SendArrivalMail 顧客に在庫の増減をお知らせする
		/// <summary>
		/// 顧客に在庫の増減をお知らせする
		/// </summary>
		/// <param name="productID">お知らせする対象商品ID</param>
		/// <param name="variationID">お知らせする対象商品バリエーションID</param>
		/// <param name="stock">
		/// お知らせする商品の在庫の基準値（在庫数がこの値を超えている場合にお知らせする）
		/// 在庫安全基準値を使う場合はNullを指定し、useSafetyCriteriaパラメータをtrueで指定する
		/// </param>
		/// <param name="useSafetyCriteria">
		/// お知らせする商品の在庫の基準値に在庫安全基準値を使うかどうかのフラグ
		/// true:在庫安全基準値を使う
		/// false：在庫安全基準値は使わない、stockパラメータに指定されたものを基準値として使う
		/// </param>
		public void SendArrivalMail(string productID, string variationID, int? stock, bool useSafetyCriteria)
		{
			int stockBase = 0; // 在庫基準値
			int stockNow = 0; // 在庫数

			// 在庫情報取得
			DataView dv = GetStock(productID, variationID);

			// 在庫数を設定
			stockNow = Convert.ToInt32(dv[0][Constants.FIELD_PRODUCTSTOCK_STOCK]);

			// 在庫基準値を設定
			if (useSafetyCriteria)
			{
				// 在庫安全基準値を使う場合
				stockBase = Convert.ToInt32(dv[0][Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]);
			}
			else if (stock.HasValue)
			{
				stockBase = stock.Value;
			}

			// 在庫数が基準値を下回っているかか確認し、下回る場合は処理終了
			if (stockNow < stockBase) return;

			// 再入荷通知メール送信バッチが再入荷通知メールを送信できるようにUserProductArrivalMailテーブルのmail_send_statusを更新
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "UpdateUserProductArrivalMail"))
			{
				var input = new Hashtable();
				input.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, 0);
				input.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, productID);
				input.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, variationID);
				input.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_LAST_CHANGED, LAST_CHANGED_API);
				input.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN, Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL);
				input.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS, Constants.FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_SENDING);
				statement.ExecStatementWithOC(accessor, input);
			}

			// 再入荷通知メール送信バッチを起動
			// 引数作成
			StringBuilder parameters = new StringBuilder();
			parameters.Append(" -,").Append(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL).Append(",").Append(DEFAULT_MAILTEMPLATE_ARRIVAL);
			parameters.Append(" +,").Append(Constants.MailSendMethod.Auto); // メール送信方法：外部連携時、自動送信を指定する
			parameters.Append(" ").Append(0);
			parameters.Append(",").Append(productID);
			parameters.Append(",").Append(variationID);
			parameters.Append(",").Append("1"); // send_arrival_mail_flg
			parameters.Append(",").Append("0"); // send_release_mail_flg
			parameters.Append(",").Append("0"); // send_resale_mail_flg
			parameters.Append(",").Append(LAST_CHANGED_API);

			// プロセス実行
			ExecuteArrivalMailSendExeProsess(Constants.PHYSICALDIRPATH_ARRIVALMAILSEND_EXE, parameters.ToString());
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +ExecuteArrivalMailSendExeProsess 再入荷通知メール送信バッチのプロセス起動（非同期）
		/// <summary>
		/// 再入荷通知メール送信バッチのプロセス起動（非同期）
		/// </summary>
		/// <param name="arrivalMailSendExePath">再入荷通知メール送信バッチのexeファイルのフルパス</param>
		/// <param name="arg">再入荷通知メール送信バッチexeの起動時の引数</param>
		public void ExecuteArrivalMailSendExeProsess(string arrivalMailSendExePath, string arg)
		{
			Process process = Process.Start(arrivalMailSendExePath, arg);
		}
		#endregion

		#region +GetStockQuantitiesFrom 在庫数の増減を取得
		/// <summary>
		/// 在庫数の増減を取得
		/// </summary>
		/// <param name="timeSpan">過去における厳密期間</param>
		/// <returns>在庫の増減履歴情報</returns>
		public DataTable GetStockQuantitiesFrom(PastAbsoluteTimeSpan timeSpan)
		{
			timeSpan.ValidateMaxEndTime();

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "GetStockQuantitiesFrom"))
			{
				sqlStatement.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;

				Hashtable parameter = new Hashtable();
				parameter.Add("fromtime", timeSpan.BeginTime);
				parameter.Add("beforetime", timeSpan.EndTime);
				return sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +GetStockQuantitiesNow 現在の在庫数を取得
		/// <summary>
		/// 現在の在庫数を取得
		/// </summary>
		/// <returns>在庫情報</returns>
		public DataTable GetStockQuantitiesNow()
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "GetStockQuantitiesNow"))
			{
				return sqlStatement.SelectSingleStatement(sqlAccessor).Table;
			}
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +GetNotSyncChangeStockBySupplier サプライアを指定して、現在未同期の在庫変動数を取得する(termで指定された日前までを対象）
		/// <summary>
		/// サプライアを指定して、現在未同期の在庫変動数を取得する(termで指定された日前までを対象）
		/// </summary>
		/// <returns></returns>
		public DataTable GetNotSyncChangeStockBySupplier(string supplier,int term)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "GetNotSyncChangeStockBySupplier"))
			{
				Hashtable parameter = new Hashtable();
				parameter.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG, 0);
				parameter.Add(Constants.FIELD_PRODUCT_SUPPLIER_ID, supplier);
				parameter.Add("term", term * -1);
				return sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}
		}
		#endregion

		#region +UpdateStockHistorySyncFlg 在庫履歴の同期フラグを更新する
		/// <summary>
		/// 在庫履歴の同期フラグを更新する
		/// </summary>
		/// <returns></returns>
		public void UpdateStockHistorySyncFlg(string shopID, string productID, string variationID, int historyNO, bool syncFlg, SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock","UpdateStockHistorySyncFlg"))
			{
				Hashtable parameter = new Hashtable();
				parameter.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, shopID);
				parameter.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productID);
				parameter.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationID);
				parameter.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_HISTORY_NO, historyNO);
				parameter.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG, syncFlg);
				sqlStatement.ExecStatement(sqlAccessor, parameter);
			}
		}
		#endregion

		#region +GetStockBySupplier サプライアを指定して、在庫数を取得する(過去1日前の変動までを対象）
		/// <summary>
		/// サプライアを指定して、在庫数を取得する(termで指定された日前までを対象）
		/// </summary>
		/// <param name="supplier"></param>
		/// <param name="term">指定過去分日数</param>
		/// <remarks>在庫安全基準値を以下の在庫数の場合、在庫数を0として返却</remarks>
		/// <returns></returns>
		public DataTable GetStockBySupplier(string supplier,int term)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "GetStockQuantitiesNowBySupplier") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				Hashtable parameter = new Hashtable();
				parameter.Add(Constants.FIELD_PRODUCT_SUPPLIER_ID, supplier);
				parameter.Add("term", term * -1);
				return sqlStatement.SelectSingleStatement(sqlAccessor, parameter).Table;
			}
		}
		#endregion

		#region +SetStockQuantity 基幹からの在庫数からw2の注文個数を減算した在庫数で更新する（天賞堂向け）
		/// <summary>
		/// 基幹からの在庫数からw2の注文個数を減算した在庫数で更新する（天賞堂向け）
		/// </summary>
		/// <param name="productID">対象商品ID</param>
		/// <param name="variationID">対象商品バリエーションID</param>
		/// <param name="stock">基幹からの在庫数（絶対値）</param>
		public void SubtractOrderQuantityFromStock(string productID, string variationID, int stock)
		{
			var resultCount = 0; // 実行件数

			// 可読性上げるためにあえてトランザクション内の処理は分割しない
			// 明示的にかくことでトランザクションを意識！
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				// 在庫情報の更新と、履歴の追加を同一トランザクション内で行う
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 在庫情報更新
				using (var statement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "UpdateProductStockForTenshodo"))
				{
					var input = new Hashtable 
					{
						{Constants.FIELD_PRODUCTSTOCK_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID},
						{Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productID},
						{Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationID},
						{Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, LAST_CHANGED_API},
						{Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT, 0},
						{Constants.FIELD_PRODUCTSTOCK_STOCK, stock},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK, 0},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, 0},
					};
					resultCount = statement.ExecStatement(accessor, input);
					w2.Common.Logger.FileLogger.Write("ImportStock", " 在庫情報更新 resultCount：" + resultCount);
				}

				// 実行結果が0件はエラー
				if (resultCount == 0)
				{
					accessor.RollbackTransaction();
					accessor.CloseConnection();
					var errorMessage = new StringBuilder();
					errorMessage.Append("在庫情報の更新に失敗しました。更新結果件数：" + resultCount.ToString(CultureInfo.InvariantCulture) + "件").Append("\r\n");
					errorMessage.Append("下記更新パラメータ").Append("\r\n");
					errorMessage.Append("product_id:" + productID).Append("\r\n");
					errorMessage.Append("variation_id:" + variationID).Append("\r\n");
					errorMessage.Append("stock:" + stock);
					throw new Exception(errorMessage.ToString());
				}

				if (stock != 0) // 在庫履歴追加は在庫が変動したときのみとする。
				{
					// 在庫履歴追加
					using (var statement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "InsertProductStockHistoryWithSyncFlg"))
					{
						var input = new Hashtable
						{
							// 更新パラメータセット
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, ""},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productID},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationID},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_OPERATION},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, stock},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, ""},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, LAST_CHANGED_API},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG, false},
						};
						statement.ExecStatement(accessor, input);
						w2.Common.Logger.FileLogger.Write("ImportStock", " 在庫履歴追加");
					}
				}

				// コミット
				accessor.CommitTransaction();
				// 接続Close
				accessor.CloseConnection();
			}
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		#region +ExternalSetStockQuantity
		/// <summary>
		/// External set stock quantity (absolute value)
		/// </summary>
		/// <param name="productID">Product Id</param>
		/// <param name="variationID">Variation Id</param>
		/// <param name="stock">Stock</param>
		public void ExternalSetStockQuantity(string productID, string variationID, int stock)
		{
			int resultCount = 0;

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// Product stock information
				var productStock = GetStock(productID, variationID);

				var relativeStock = stock - Convert.ToInt32(productStock[0][Constants.FIELD_PRODUCTSTOCK_STOCK]);

				// Set stock
				using (var statement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "UpdateProductStock"))
				{
					var input = new Hashtable()
					{
						{Constants.FIELD_PRODUCTSTOCK_SHOP_ID, 0},
						{Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productID},
						{Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationID},
						{Constants.FIELD_PRODUCTSTOCK_STOCK, relativeStock},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK, 0},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0},
						{Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, 0},
						{Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT, Convert.ToInt32(productStock[0][Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT])},
						{Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, LAST_CHANGED_API}
					};

					resultCount = statement.ExecStatement(accessor, input);
				}

				if (resultCount == 0)
				{
					var errorMessage = new StringBuilder();
					errorMessage.Append("在庫情報の更新に失敗しました。").Append("\r\n");
					errorMessage.Append("下記更新パラメータ").Append("\r\n");
					errorMessage.Append("商品ID: " + productID).Append("\r\n");
					errorMessage.Append("バリエーションID: " + variationID).Append("\r\n");
					errorMessage.Append("在庫数: " + stock);
					FileLogger.WriteError(errorMessage.ToString());

					accessor.RollbackTransaction();
				}
				else
				{
					// Update stock history
					using (var statement = new SqlStatement(Resources.ResourceManager, "Stock", "Stock", "InsertProductStockHistoryWithExternalApi"))
					{
						var input = new Hashtable()
						{
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, string.Empty},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productID},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationID},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_EXTERNAL_API},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK, stock},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_B, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_C, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED, 0},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, string.Empty},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, LAST_CHANGED_API},
							{Constants.FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG, false}
						};

						statement.ExecStatement(accessor, input);
					}

					accessor.CommitTransaction();
				}
			}
		}
		#endregion
	}
}
