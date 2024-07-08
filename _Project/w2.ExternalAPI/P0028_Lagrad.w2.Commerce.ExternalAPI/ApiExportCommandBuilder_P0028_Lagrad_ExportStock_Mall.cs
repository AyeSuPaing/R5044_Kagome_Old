/*
=========================================================================================================
  Module      : P0028_Lagrad用モール側のExportクラス(ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.App.Common.Stock;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Database.Common;
using w2.ExternalAPI.Common.Export;

namespace P0028_Lagrad.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// モール側Export用の在庫変動ファイル要コマンド
	/// </summary>
	public class ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall : ApiExportCommandBuilder
	{
		/// <summary>
		/// 同期フラグ更新対象構造体
		/// </summary>
		public struct UpdateSyncFlgTarget
		{
			public int HistoryNo;
			public string ShopID;
			public string ProductID;
			public string VariationID;
		}

		/// <summary>
		/// Export対象のデータ取得
		/// </summary>
		/// <returns></returns>
		public override DataTable GetDataTable()
		{	
			// コマンドライン引数から対象サプライアIDをとってくる
			string supplier = base.Properties["supplier"];
			int term = int.Parse(base.Properties["term"]);

			StockCommon stockCommon = new StockCommon();
			DataTable dt = stockCommon.GetNotSyncChangeStockBySupplier(supplier, term);

			// 後程在庫履歴の同期済フラグを立てる必要があるので、キーの情報を保持しておく
			updateSyncFlgTarget_ = new List<UpdateSyncFlgTarget>();
			foreach(DataRow dr in dt.Rows)
			{
				UpdateSyncFlgTarget updateSyncFlgTarget = new UpdateSyncFlgTarget();
				updateSyncFlgTarget.HistoryNo = int.Parse(StringUtility.ToEmpty(dr[Constants.FIELD_PRODUCTSTOCKHISTORY_HISTORY_NO]));
				updateSyncFlgTarget.ShopID = StringUtility.ToEmpty(dr[Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID]);
				updateSyncFlgTarget.ProductID = StringUtility.ToEmpty(dr[Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID]);
				updateSyncFlgTarget.VariationID = StringUtility.ToEmpty(dr[Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID]);
				updateSyncFlgTarget_.Add(updateSyncFlgTarget);
			}
			
			// データテーブルを集約・サマリ
			dt.Columns.Remove(Constants.FIELD_PRODUCTSTOCKHISTORY_HISTORY_NO);
			dt.Columns.Remove(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID);
			// 商品ID、バリエーションIDで集約
			DataView dv = new DataView(dt);
			DataTable distinctTable = dv.ToTable("DistinctTable", true,new string[]{Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID});
			distinctTable.Columns.Add("sum_add_stock");

			foreach(DataRow dr in distinctTable.Rows)
			{
				// 商品IDとバリエーションIDでサマル
				string exp = "SUM(" + Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK + ")";
				string fillter =
					string.Format(
					Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID + " = '{0}' and "+ Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID + " = '{1}' "
					,StringUtility.ToEmpty(dr[Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID])
					,StringUtility.ToEmpty(dr[Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID]));
				dr["sum_add_stock"] = dt.Compute(exp, fillter);
			}
			// 集約・サマリ値を持つデータテーブルを返却
			return distinctTable;
		}

		/// <summary>
		/// 出力実行
		/// </summary>
		/// <param name="record">対象レコード</param>
		/// <returns>出力値</returns>
		protected override object[] Export(IDataRecord record)
		{
			// そのまま
			return record.ToArray();
		}

		/// <summary>
		/// 終了時の処理
		/// </summary>
		protected override void End()
		{
			StockCommon stockCommon = new StockCommon();
			using(SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();
				// エラーを考慮し、ここでトランザクション制御を行う。
				// 一つでも更新に失敗した場合はロールバック
				try
				{
					// キーを元に在庫履歴の同期済フラグを立てる
					foreach (UpdateSyncFlgTarget updateSyncFlgTarget in updateSyncFlgTarget_)
					{
						// Hashtableを分解
						int historyNo = updateSyncFlgTarget.HistoryNo;
						string shopID = updateSyncFlgTarget.ShopID;
						string productID = updateSyncFlgTarget.ProductID;
						string variationID = updateSyncFlgTarget.VariationID;
						// 同期済みとして在庫履歴データを更新
						stockCommon.UpdateStockHistorySyncFlg(shopID, productID, variationID, historyNo, true, sqlAccessor);
					}
					sqlAccessor.CommitTransaction();
				}
				catch
				{
					// 一つでもえらったらロールバックかけてスロー
					try
					{
						sqlAccessor.RollbackTransaction();
					}
					catch 
					{
						// 上位でCatchさせるため何もしない
					}
					throw;
				}
			}
			base.End();
		}
		/// <summary>処理終了後に同期フラグを立てるために必要なDB上のKEY情報を保持</summary>
		private List<UpdateSyncFlgTarget> updateSyncFlgTarget_;
	}
}
