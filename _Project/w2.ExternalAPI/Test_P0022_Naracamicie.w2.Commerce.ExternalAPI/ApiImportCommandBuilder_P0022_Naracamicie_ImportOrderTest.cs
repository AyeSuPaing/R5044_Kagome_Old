using P0022_Naracamicie.w2.Commerce.ExternalAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;

namespace Test_P0022_Naracamicie.w2.Commerce.ExternalAPI
{
    
    
    /// <summary>
    ///ApiImportCommandBuilder_P0022_Naracamicie_ImportOrderTest のテスト クラスです。すべての
    ///ApiImportCommandBuilder_P0022_Naracamicie_ImportOrderTest 単体テストをここに含めます
    ///</summary>
	[TestClass()]
	public class ApiImportCommandBuilder_P0022_Naracamicie_ImportOrderTest
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
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
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
		///GetArg のテスト
		///</summary>
		[TestMethod()]
		[DeploymentItem("P0022_Naracamicie.w2.Commerce.ExternalAPI.dll")]
		public void GetArgTest()
		{
			ApiEntry apiEntry = new ApiEntry();
			DataTable table = new DataTable();
			table.Columns.AddRange(new DataColumn[7] { new DataColumn("0"), new DataColumn("1"), new DataColumn("2"), new DataColumn("3"), new DataColumn("4"), new DataColumn("5"), new DataColumn("6") });
			apiEntry.Data = table.NewRow();
			apiEntry.Data.ItemArray = new object[] { "2012111900001", "", "565730000135", "true", "", "", "" };
			ShipOrderArg expected = null; 
			ShipOrderArg actual;
			actual = ApiImportCommandBuilder_P0022_Naracamicie_ImportOrder_Accessor.GetArg(apiEntry);
			Assert.AreEqual(expected, actual);
		}
	}
}
