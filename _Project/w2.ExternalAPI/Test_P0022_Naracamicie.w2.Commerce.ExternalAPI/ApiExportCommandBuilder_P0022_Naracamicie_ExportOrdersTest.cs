using P0022_Naracamicie.w2.Commerce.ExternalAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using w2.ExternalAPI.Common.Logging;
using w2.ExternalAPI.Common;
using w2.Common;
namespace Test_P0022_Naracamicie.w2.Commerce.ExternalAPI
{
	
	
	/// <summary>
	///ApiExportCommandBuilder_P0022_Naracamicie_ExportOrdersTest のテスト クラスです。すべての
	///ApiExportCommandBuilder_P0022_Naracamicie_ExportOrdersTest 単体テストをここに含めます
	///</summary>
	[TestClass()]
	public class ApiExportCommandBuilder_P0022_Naracamicie_ExportOrdersTest
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
			ApiLogger.SetLogger(new NullLogger());
			Constants.STRING_SQL_CONNECTION = "server=w2DB1;database=w2ReleaseV5.7;uid=sa;pwd=w2Sa";
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
		///GetOrderItem のテスト
		///</summary>
		[TestMethod()]
		public void GetOrderItemTest()
		{
			ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders target = new ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders();
			string orderId = "2012111900003";
			target.Properties = new BatchProperties("Timespan=360");
			DataTable actual;
			actual = target.GetOrderItem(orderId);
			Assert.AreEqual(10, actual.Rows.Count);
		}

		/// <summary>
		///isW2Gift のテスト
		///</summary>
		[TestMethod()]
		public void isW2GiftTest()
		{
			ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders target = new ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders();
			string orderId = "2012081600005";
			target.Properties = new BatchProperties("Timespan=360");
			DataTable orderItem = target.GetOrderItem(orderId);
			
			bool expected = true;
			bool actual;
			actual = target.isW2Gift(orderItem);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetGiftPrice のテスト
		///</summary>
		[TestMethod()]
		public void GetGiftPriceTest()
		{
			ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders target = new ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders();
			string orderId = "2012111900003";
			target.Properties = new BatchProperties("Timespan=360");
			DataTable orderItem = target.GetOrderItem(orderId);
			Decimal expected = 315; 
			Decimal actual;
			actual = target.GetGiftPrice(orderItem);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetGiftPrice のテスト
		///</summary>
		[TestMethod()]
		public void GetPriceOtherTest()
		{
			ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders target = new ApiExportCommandBuilder_P0022_Naracamicie_ExportOrders();
			string orderId = "DEV2013011000028";
			// Timespanは大き目の10年くらいとってます
			target.Properties = new BatchProperties("PaymentStatus=;OrderStatus=ODR;IntgFlag=4;IntgWorkingFlag=5;Timespan=3600;OrderID=" + orderId);
			DataTable order = target.GetDataTable();
			using (DataTableReader reader = new DataTableReader(order))
			{
				reader.Read();
				var obj = target.Do(reader);

				Decimal expected = -3120;
				Decimal actual;
				actual = (Decimal)obj[12]; // PriceOtherは12
				Assert.AreEqual(expected, actual);
			}
		}
	}
}
