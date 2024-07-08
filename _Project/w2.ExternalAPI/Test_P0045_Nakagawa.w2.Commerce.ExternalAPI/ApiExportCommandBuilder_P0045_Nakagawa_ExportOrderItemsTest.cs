using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.ExternalAPI.Common.Logging;
using w2.Common;

namespace Test_P0045_Nakagawa.w2.Commerce.ExternalAPI
{
	[TestClass]
	public class ApiExportCommandBuilder_P0045_Nakagawa_ExportOrderItemsTest
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
			Constants.STRING_SQL_CONNECTION = "server=w2server02;database=w2ReleaseV5.8;uid=sa;pwd=w2Sa";
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

		[TestMethod]
		public void TestMethod1()
		{
		}
	}
}
