using System.Data;
using P0028_Lagrad.w2.Commerce.ExternalAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using w2.ExternalAPI.Common.Entry;

namespace Test.P0028_Lagrad.w2.Commerce.ExternalAPI
{
    
    
    /// <summary>
    ///ApiImportCommandBuilder_P0028_Lagrad_ImportStock_MallTest のテスト クラスです。すべての
    ///ApiImportCommandBuilder_P0028_Lagrad_ImportStock_MallTest 単体テストをここに含めます
    ///</summary>
	[TestClass()]
	public class ApiImportCommandBuilder_P0028_Lagrad_ImportStock_MallTest
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
		///Import のテスト
		///</summary>
		[TestMethod()]
		[DeploymentItem("P0028_Lagrad.w2.Commerce.ExternalAPI.dll")]
		public void ImportTest()
		{
			// テスト用データ削除
			TestUtil.DeleteStockHistory("0", "P0028TEST01", "P0028TEST01V01");
			TestUtil.DeleteStock("0", "P0028TEST01", "P0028TEST01V01");
			
			// テスト用データ登録
			TestUtil.InsertStock("0", "P0028TEST01", "P0028TEST01V01", 100, 20);
			
			ApiImportCommandBuilder_P0028_Lagrad_ImportStock_Mall_Accessor target 
				= new ApiImportCommandBuilder_P0028_Lagrad_ImportStock_Mall_Accessor();
			
			DataTable dt = new DataTable();
			dt.Columns.Add("product_id");
			dt.Columns.Add("variation_id");
			dt.Columns.Add("stock");

			DataRow dr = dt.NewRow();
			dr["product_id"] = "P0028TEST01";
			dr["variation_id"] = "P0028TEST01V01";
			dr["stock"] = "50";

			dt.Rows.Add(dr);

			ApiEntry apiEntry = new ApiEntry() { Data = dr};

			target.Import(apiEntry);
			
			// 更新後のDB確認
			string stockSql = "select * from w2_ProductStock where product_id = 'P0028TEST01' and variation_id = 'P0028TEST01V01' ";
			string stockHisSql = "select * from w2_ProductStockHistory where product_id = 'P0028TEST01' and variation_id = 'P0028TEST01V01' ";

			DataTable stockDT = TestUtil.GetDataTable(stockSql);
			DataTable stockHisDT = TestUtil.GetDataTable(stockHisSql);

			Assert.AreEqual(stockDT.Rows[0]["stock"].ToString(),"50");
			Assert.AreEqual(stockHisDT.Rows[0]["add_stock"].ToString(), "-50");

			Assert.AreEqual(stockHisDT.Rows[0]["sync_flg"], true);

		}
	}
}
