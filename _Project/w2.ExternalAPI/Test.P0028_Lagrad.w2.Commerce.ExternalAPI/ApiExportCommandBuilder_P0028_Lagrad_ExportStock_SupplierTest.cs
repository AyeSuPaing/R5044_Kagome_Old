using P0028_Lagrad.w2.Commerce.ExternalAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using w2.ExternalAPI.Common;

namespace Test.P0028_Lagrad.w2.Commerce.ExternalAPI
{
    
    
    /// <summary>
    ///ApiExportCommandBuilder_P0028_Lagrad_ExportStock_SupplierTest のテスト クラスです。すべての
    ///ApiExportCommandBuilder_P0028_Lagrad_ExportStock_SupplierTest 単体テストをここに含めます
	/// 店舗側在庫Export（在庫絶対数）コマンドテスト
    ///</summary>
	[TestClass()]
	public class ApiExportCommandBuilder_P0028_Lagrad_ExportStock_SupplierTest
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
			TestUtil.DeleteStock("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStock("0", "P0028TEST01", "P0028TEST01V02");

			// テスト用データ登録
			TestUtil.InsertStock("0", "P0028TEST01", "P0028TEST01V01", 100, 20);
			TestUtil.InsertStock("0", "P0028TEST01", "P0028TEST01V02", 200, 30);

			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier target 
				= new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier();

			target.Properties = new BatchProperties("supplier=P0028;term=1");

			DataTable actual;
			actual = target.GetDataTable();
			Assert.AreEqual(2, actual.Rows.Count);

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
		///GetDataTable(在庫安全基準値以下の在庫の場合） のテスト
		///</summary>
		[TestMethod()]
		public void GetDataTableTestUnderStockAlert()
		{
			// テスト用データ削除
			TestUtil.DeleteStock("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStock("0", "P0028TEST01", "P0028TEST01V02");
			
			// テスト用データ登録
			TestUtil.InsertStock("0", "P0028TEST01", "P0028TEST01V01", 20, 20);
			TestUtil.InsertStock("0", "P0028TEST01", "P0028TEST01V02", 31, 30);
			
			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier target
				= new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier();

			target.Properties = new BatchProperties("supplier=P0028;term=1");

			DataTable actual;
			actual = target.GetDataTable();
			Assert.AreEqual(2, actual.Rows.Count);

			foreach (DataRow dr in actual.Rows)
			{
				string msg = string.Empty;

				foreach (DataColumn dc in actual.Columns)
				{
					msg += dc.ColumnName + ":" + dr[dc.ColumnName] + "   ";
				}

				Console.WriteLine(msg);

				if (dr["VariationID"].ToString() == "P0028TEST01V01")
				{
					Assert.AreEqual(0, dr["Stock"]);
				}
				else if (dr["VariationID"].ToString() == "P0028TEST01V02")
				{
					Assert.AreEqual(31, dr["Stock"]);
				}

				
			}
		}

		/// <summary>
		///GetDataTable(指定期間超過） のテスト
		///</summary>
		[TestMethod()]
		public void GetDataTableTestOverCreateDate()
		{
			// テスト用データ削除
			TestUtil.DeleteStock("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStock("0", "P0028TEST01", "P0028TEST01V02");

			// テスト用データ登録
			TestUtil.InsertStock("0", "P0028TEST01", "P0028TEST01V01", 100, 20, DateTime.Now.AddHours(-23));
			TestUtil.InsertStock("0", "P0028TEST01", "P0028TEST01V02", 200, 30, DateTime.Now.AddHours(-25));

			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier target
				= new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier();

			target.Properties = new BatchProperties("supplier=P0028;term=1");

			DataTable actual;
			actual = target.GetDataTable();
			// 指定基準超過のものがあるので1件だけ
			Assert.AreEqual(1, actual.Rows.Count);

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
		///Export のテスト
		///</summary>
		[TestMethod()]
		[DeploymentItem("P0028_Lagrad.w2.Commerce.ExternalAPI.dll")]
		public void ExportTest()
		{
			ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier_Accessor target =
				new ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier_Accessor();
			
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

				object[] expected = new object[] {"1", "2", "3"};
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
