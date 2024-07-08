using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using P0028_Lagrad.w2.Commerce.ExternalAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using w2.ExternalAPI.Common;

namespace Test.P0028_Lagrad.w2.Commerce.ExternalAPI
{
	//********************************************************************************
	// P0028TEST01の商品をつくってから実施
	// サプライアIDはP0028であること
	// バリエーションはP0028TEST01V01、P0028TEST01V02を作成すること
	//********************************************************************************

    /// <summary>
    ///ApiExportCommandBuilder_P0028_Lagrad_ExportStock_MallTest のテスト クラスです。すべての
    ///ApiExportCommandBuilder_P0028_Lagrad_ExportStock_MallTest 単体テストをここに含めます
	/// モール側在庫Export（在庫変動数）コマンドテスト
    ///</summary>
	[TestClass()]
	public class ApiExportCommandBuilder_P0028_Lagrad_ExportStock_MallTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///現在のテストの実行についての情報および機能を
		///提供するテスト コンテキストを取得または設定します。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 追加のテスト属性
		// 
		//テストを作成するときに、次の追加属性を使用することができます:
		//
		//クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			TestUtil.init();
		}
		//
		//クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//各テストを実行する前にコードを実行するには、TestInitialize を使用
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//各テストを実行した後にコードを実行するには、TestCleanup を使用
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///GetDataTable のテスト
		///</summary>
		[TestMethod()]
		public void GetDataTableTest()
		{
			// テスト用データ削除
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V02");

			// テスト用データ登録
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01","02",-3,false);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -5, false);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V02", "02", -10, false);

			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall target = 
				new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall();
			target.Properties = new BatchProperties("supplier=P0028;term=1");
			DataTable actual;
			actual = target.GetDataTable();

			// 商品・バリエーションでサマリしているはずなので2件返却
			Assert.AreEqual("2", actual.Rows.Count.ToString());
			
			foreach(DataRow dr in  actual.Rows)
			{
				string msg = string.Empty;

				foreach(DataColumn dc in actual.Columns)
				{
					msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
				}
				Console.WriteLine(msg);
			}
		}

		/// <summary>
		///GetDataTable(指定期間超過） のテスト
		///</summary>
		[TestMethod()]
		public void GetDataTableTestOverCreateDate()
		{
			// テスト用データ削除
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V02");

			// テスト用データ登録
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -3, false,DateTime.Now.AddHours(-23 ));
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -5, false, DateTime.Now.AddHours(-23));
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V02", "02", -10, false, DateTime.Now.AddHours(-25));

			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall target = new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall();
			target.Properties = new BatchProperties("supplier=P0028;term=1");
			DataTable actual;
			actual = target.GetDataTable();

			// 商品・バリエーションでサマリしているが、対象外となるものがあるので1件だけ
			Assert.AreEqual("1", actual.Rows.Count.ToString());

			foreach (DataRow dr in actual.Rows)
			{
				string msg = string.Empty;

				foreach (DataColumn dc in actual.Columns)
				{
					msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
				}
				Console.WriteLine(msg);
			}

		}

		/// <summary>
		///GetDataTableNoData のテスト
		/// 同期済みの履歴が引っかからないことを確認する
		///</summary>
		[TestMethod()]
		public void GetDataTableNoData()
		{
			// テスト用データ削除
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V02");

			// テスト用データ登録
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -3, true);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -5, true);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V02", "02", -10, true);

			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall target = new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall();
			target.Properties = new BatchProperties("supplier=P0028;term=1");
			DataTable actual;
			actual = target.GetDataTable();
	
			// 商品・バリエーションでサマリしているはずなので2件返却
			Assert.AreEqual("0", actual.Rows.Count.ToString());

			foreach (DataRow dr in actual.Rows)
			{
				string msg = string.Empty;

				foreach (DataColumn dc in actual.Columns)
				{
					msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
				}
				Console.WriteLine(msg);
			}
		}

		/// <summary>
		///End のテスト
		///</summary>
		[TestMethod()]
		[DeploymentItem("P0028_Lagrad.w2.Commerce.ExternalAPI.dll")]
		public void EndTest()
		{
			// テスト用データ削除
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V02");

			// テスト用データ登録
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -3, false);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -5, false);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V02", "02", -10, false);

			// テスト用のデータとってくる
			string stockHisSql = "select * from w2_ProductStockHistory where product_id = 'P0028TEST01'";
			DataTable stockHisDT = TestUtil.GetDataTable(stockHisSql);
			Console.WriteLine("----------------------処理前----------------------");

			foreach (DataRow dr in stockHisDT.Rows)
			{
				string msg = string.Empty;

				foreach (DataColumn dc in stockHisDT.Columns)
				{
					msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
				}
				Console.WriteLine(msg);
			}

			// テスト用のデータを元に更新用ハッシュリスト生成
			List<ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall.UpdateSyncFlgTarget> list = new List<ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall.UpdateSyncFlgTarget>();

			foreach(DataRow dr in stockHisDT.Rows)
			{
				ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall.UpdateSyncFlgTarget updateSyncFlgTarget = new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall.UpdateSyncFlgTarget();

				updateSyncFlgTarget.HistoryNo = int.Parse(dr["history_no"].ToString());
				updateSyncFlgTarget.ShopID = dr["shop_id"].ToString();
				updateSyncFlgTarget.ProductID = dr["product_id"].ToString();
				updateSyncFlgTarget.VariationID = dr["variation_id"].ToString();

				list.Add(updateSyncFlgTarget);
			}

			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall_Accessor target = new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall_Accessor();
			target.updateSyncFlgTarget_ = list;
			target.End();
			// 処理後のデータとってくる
			DataTable aftDT = TestUtil.GetDataTable(stockHisSql);

			//処理後のデータ検証
			foreach(DataRow dr in aftDT.Rows)
			{
				Assert.AreEqual(dr["sync_flg"],true);
			}
			Console.WriteLine("----------------------処理後----------------------");
			foreach (DataRow dr in aftDT.Rows)
			{
				string msg = string.Empty;

				foreach (DataColumn dc in aftDT.Columns)
				{
					msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
				}
				Console.WriteLine(msg);
			}
		}

		/// <summary>
		///GetDataTableSetUpdData のテスト
		/// GetTableでEndメソッドにて更新するための情報が作られているか確認
		///</summary>
		[TestMethod()]
		public void GetDataTableSetUpdData()
		{
			// テスト用データ削除
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V02");

			// テスト用データ登録
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -3, false);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V01", "02", -5, false);
			TestUtil.InsertStockHistory("0", "P0028TEST01", "P0028TEST01V02", "02", -10, false);

			DataTable actual;

			using (ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall target = new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall())
			{
				target.Properties = new BatchProperties("supplier=P0028;term=1");
				// Disposeされると消えるのでこっぴとく
				actual = target.GetDataTable();
				// 商品・バリエーションでサマリしているはずなので2件返却
				Assert.AreEqual("2", actual.Rows.Count.ToString());
				Console.WriteLine("---------------------GetDataTableの結果-----------------------");

				foreach (DataRow dr in actual.Rows)
				{
					string msg = string.Empty;

					foreach (DataColumn dc in actual.Columns)
					{
						msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
					}
					Console.WriteLine(msg);
				}
			}

			string stockHisSql = "select * from w2_ProductStockHistory where product_id = 'P0028TEST01'";
			// 処理後のデータとってくる
			DataTable aftDT = TestUtil.GetDataTable(stockHisSql);

			//処理後のデータ検証
			foreach (DataRow dr in aftDT.Rows)
			{
				Assert.AreEqual(dr["sync_flg"], true);
			}
			Console.WriteLine("----------------------END(Dispose)後のDB----------------------");

			foreach (DataRow dr in aftDT.Rows)
			{
				string msg = string.Empty;

				foreach (DataColumn dc in aftDT.Columns)
				{
					msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
				}
				Console.WriteLine(msg);
			}
		}

		/// <summary>
		///Export のテスト
		///</summary>
		[TestMethod()]
		[DeploymentItem("P0028_Lagrad.w2.Commerce.ExternalAPI.dll")]
		public void ExportTest()
		{
			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall_Accessor target = new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Mall_Accessor(); 
			
			DataTable dt = new DataTable();
			dt.Columns.Add("col1");
			dt.Columns.Add("col2");
			dt.Columns.Add("col3");
			DataRow dr = dt.NewRow();
			dr["col1"] = "1";
			dr["col2"] = "2";
			dr["col3"] = "3";
			dt.Rows.Add(dr);

			using (DataTableReader dtR = new DataTableReader(dt))
			{
				dtR.Read();
				IDataRecord record = dtR;
				object[] expected = new object[] { "1", "2", "3" };
				object[] actual;
				actual = target.Export(record);
				Assert.AreEqual(expected.Length, actual.Length);
				Assert.AreEqual(expected[0], actual[0]);
				Assert.AreEqual(expected[1], actual[1]);
				Assert.AreEqual(expected[2], actual[2]);
			}
		}
	}
}
