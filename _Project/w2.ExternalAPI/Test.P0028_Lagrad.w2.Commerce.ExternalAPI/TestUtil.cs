using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common;
using System.Data.SqlClient;
using Constants = w2.Common.Constants;
using w2.ExternalAPI.Common.Logging;

namespace Test.P0028_Lagrad.w2.Commerce.ExternalAPI
{
	public class TestUtil
	{
		public static void init()
		{
			//------------------------------------------------------
			// SQL接続文字列設定
			//------------------------------------------------------
			Constants.STRING_SQL_CONNECTION = Properties.Settings.Default.SqlConnection;

			//------------------------------------------------------
			// アプリケーション設定読み込み
			//------------------------------------------------------
			Constants.APPLICATION_NAME = "P0028外部連携";

			//------------------------------------------------------
			// ログファイル
			//------------------------------------------------------
			Constants.PHYSICALDIRPATH_LOGFILE = @"C:\Logs\R5044_Kagome.Develop\";

			ApiLogger.SetLogger(new NullLogger());
				
		}

		public static void UpdateStockHistor(int historyNO,string shopID,string productID,string variationID,bool syncFlg, SqlConnection conn,SqlTransaction tran)
		{
			string sql =
				@"UPDATE w2_ProductStockHistory
           SET sync_flg = @sync_flg
           WHERE history_no = @history_no and shop_id = @shop_id AND product_id = @product_id AND variation_id = @variation_id ";

			using (SqlCommand cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@history_no", historyNO);
				cmd.Parameters.AddWithValue("@shop_id", shopID);
				cmd.Parameters.AddWithValue("@product_id", productID);
				cmd.Parameters.AddWithValue("@variation_id", variationID);
				cmd.Parameters.AddWithValue("@sync_flg", syncFlg);
				cmd.Transaction = tran;
				cmd.ExecuteNonQuery();

			}
		}

		public static void InsertStockHistory(string shopID,string productID,string variationID,string actionStatus,int addStock,bool syncFlg)
		{
			string sql =
				@"INSERT INTO w2_ProductStockHistory
           ([shop_id]
           ,[product_id]
           ,[variation_id]
           ,[action_status]
           ,[add_stock]
           ,[last_changed]
		   ,[sync_flg])
     VALUES
           (@shop_id
           ,@product_id
           ,@variation_id
           ,@action_status
           ,@add_stock
           ,@last_changed
           ,@sync_flg
           )";

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.SqlConnection))
			using (SqlCommand cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@shop_id", shopID);
				cmd.Parameters.AddWithValue("@product_id", productID);
				cmd.Parameters.AddWithValue("@variation_id", variationID);
				cmd.Parameters.AddWithValue("@action_status", actionStatus);
				cmd.Parameters.AddWithValue("@add_stock", addStock);
				cmd.Parameters.AddWithValue("@last_changed", "TEST_P0028");
				cmd.Parameters.AddWithValue("@sync_flg", syncFlg);

				cmd.Connection.Open();

				cmd.ExecuteNonQuery();

			}

		}

		public static void InsertStockHistory(string shopID, string productID, string variationID, string actionStatus, int addStock, bool syncFlg,DateTime dateCreated)
		{
			string sql =
				@"INSERT INTO w2_ProductStockHistory
           ([shop_id]
           ,[product_id]
           ,[variation_id]
           ,[action_status]
           ,[add_stock]
           ,[last_changed]
		   ,[sync_flg]
		   ,[date_created])
     VALUES
           (@shop_id
           ,@product_id
           ,@variation_id
           ,@action_status
           ,@add_stock
           ,@last_changed
           ,@sync_flg
           ,@date_created
           )";

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.SqlConnection))
			using (SqlCommand cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@shop_id", shopID);
				cmd.Parameters.AddWithValue("@product_id", productID);
				cmd.Parameters.AddWithValue("@variation_id", variationID);
				cmd.Parameters.AddWithValue("@action_status", actionStatus);
				cmd.Parameters.AddWithValue("@add_stock", addStock);
				cmd.Parameters.AddWithValue("@last_changed", "TEST_P0028");
				cmd.Parameters.AddWithValue("@sync_flg", syncFlg);
				cmd.Parameters.AddWithValue("@date_created", dateCreated);

				cmd.Connection.Open();

				cmd.ExecuteNonQuery();

			}

		}

		public static void DeleteStockHistory(string shopID, string productID, string variationID)
		{
			string sql =
				@" DELETE FROM w2_ProductStockHistory
           WHERE shop_id = @shop_id AND product_id = @product_id AND variation_id = @variation_id";

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.SqlConnection))
			using (SqlCommand cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@shop_id", shopID);
				cmd.Parameters.AddWithValue("@product_id", productID);
				cmd.Parameters.AddWithValue("@variation_id", variationID);

				cmd.Connection.Open();

				cmd.ExecuteNonQuery();
			
			}
		}

		public static void InsertStock(string shopID, string productID, string variationID, int stock, int stockAlert)
		{
			string sql =
				@"INSERT INTO w2_ProductStock
           ([shop_id]
           ,[product_id]
           ,[variation_id]
           ,[stock]
           ,[stock_alert]
           ,[last_changed])
     VALUES
           (@shop_id
           ,@product_id
           ,@variation_id
           ,@stock
           ,@stock_alert
           ,@last_changed
           )";

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.SqlConnection))
			using (SqlCommand cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@shop_id", shopID);
				cmd.Parameters.AddWithValue("@product_id", productID);
				cmd.Parameters.AddWithValue("@variation_id", variationID);
				cmd.Parameters.AddWithValue("@stock", stock);
				cmd.Parameters.AddWithValue("@stock_alert", stockAlert);
				cmd.Parameters.AddWithValue("@last_changed", "TEST_P0028");

				cmd.Connection.Open();

				cmd.ExecuteNonQuery();

			
			}

		}

		public static void InsertStock(string shopID, string productID, string variationID, 
			int stock, int stockAlert,DateTime dateChanged)
		{
			string sql =
				@"INSERT INTO w2_ProductStock
           ([shop_id]
           ,[product_id]
           ,[variation_id]
           ,[stock]
           ,[stock_alert]
           ,[last_changed]
           ,[date_changed])
     VALUES
           (@shop_id
           ,@product_id
           ,@variation_id
           ,@stock
           ,@stock_alert
           ,@last_changed
           ,@date_changed
           )";

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.SqlConnection))
			using (SqlCommand cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@shop_id", shopID);
				cmd.Parameters.AddWithValue("@product_id", productID);
				cmd.Parameters.AddWithValue("@variation_id", variationID);
				cmd.Parameters.AddWithValue("@stock", stock);
				cmd.Parameters.AddWithValue("@stock_alert", stockAlert);
				cmd.Parameters.AddWithValue("@last_changed", "TEST_P0028");
				cmd.Parameters.AddWithValue("@date_changed", dateChanged);

				cmd.Connection.Open();

				cmd.ExecuteNonQuery();

			}

		}
		
		public static void DeleteStock(string shopID, string productID, string variationID)
		{
			string sql =
				@" DELETE FROM w2_ProductStock
           WHERE shop_id = @shop_id AND product_id = @product_id AND variation_id = @variation_id";

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.SqlConnection))
			using (SqlCommand cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@shop_id", shopID);
				cmd.Parameters.AddWithValue("@product_id", productID);
				cmd.Parameters.AddWithValue("@variation_id", variationID);

				cmd.Connection.Open();

				cmd.ExecuteNonQuery();

			}
		}

		public static DataTable GetDataTable(string sql)
		{
			DataTable dt = new DataTable();

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.SqlConnection))
			using (SqlDataAdapter adp = new SqlDataAdapter(sql,conn))
			{
				adp.Fill(dt);
			}

			return dt;
		}

	}
}
