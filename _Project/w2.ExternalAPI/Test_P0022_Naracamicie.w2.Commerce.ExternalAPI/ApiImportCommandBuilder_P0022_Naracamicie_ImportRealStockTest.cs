using P0022_Naracamicie.w2.Commerce.ExternalAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Command.ApiCommand.RealStock;

namespace Test_P0022_Naracamicie.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// ApiImportCommandBuilder_P0022_Naracamicie_ImportRealStockTest の概要の説明
	/// </summary>
	[TestClass]
	public class ApiImportCommandBuilder_P0022_Naracamicie_ImportRealStockTest
	{
		public ApiImportCommandBuilder_P0022_Naracamicie_ImportRealStockTest()
		{
			//
			// TODO: コンストラクター ロジックをここに追加します
			//
		}

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
		// テストを作成する際には、次の追加属性を使用できます:
		//
		// クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// 各テストを実行する前に、TestInitialize を使用してコードを実行してください
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// 各テストを実行した後に、TestCleanup を使用してコードを実行してください
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		/// <summary>
		///GetArg のテスト
		///</summary>
		[TestMethod()]
		[DeploymentItem("P0022_Naracamicie.w2.Commerce.ExternalAPI.dll")]
		public void GetArgTest()
		{
			// 正しくCVNET（リアル店舗在庫管理システム）⇒ w2Commerce用フォーマットに変換されているか？
			ApiEntry apiEntry = new ApiEntry();
			DataTable table = new DataTable();
			table.Columns.AddRange(new DataColumn[5] { new DataColumn("0"), new DataColumn("1"), new DataColumn("2"), new DataColumn("3"), new DataColumn("4") });
			apiEntry.Data = table.NewRow();
			apiEntry.Data.ItemArray = new object[] { "011100", "102201409", "19", "00", "1" };
			SetRealStockQuantityArg expected = new SetRealStockQuantityArg();
			expected.RealShopID = "011100";
			expected.ProductID = "10-22-01-409";
			expected.VariationID = "10-22-01-409-00-19";
			expected.RealStock = 1;
			SetRealStockQuantityArg actual;
			actual = ApiImportCommandBuilder_P0022_Naracamicie_ImportRealStock.GetArg(apiEntry);
			Assert.AreEqual(expected.RealShopID, actual.RealShopID);
			Assert.AreEqual(expected.ProductID, actual.ProductID);
			Assert.AreEqual(expected.VariationID, actual.VariationID);
			Assert.AreEqual(expected.RealStock, actual.RealStock);
		}
	}
}
