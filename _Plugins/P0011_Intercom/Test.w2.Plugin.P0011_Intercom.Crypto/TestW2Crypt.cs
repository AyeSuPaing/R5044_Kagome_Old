using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.Crypto;

namespace Test.w2.Crypto
{
	/// <summary>
	/// UnitTest1 の概要の説明
	/// </summary>
	[TestClass]
	public class TestW2Crypt
	{
		public TestW2Crypt()
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

		[TestMethod]
		public void TestMethod1()
		{
			string targetval1 = "暗号化文字";

			string password = "暗号化パスワード";

			w2Crypt cry = new w2Crypt();

			//暗号化
			string encStr = cry.Encrypt(targetval1, password);

			//複合化
			string decStr = cry.Decrypt(encStr, password);
			
			//Assert
			Assert.AreEqual(targetval1, decStr);


			Assert.AreEqual("N200003526",cry.Decrypt(cry.Encrypt("N200003526", "aaaa"),"aaaa"));
			Assert.AreEqual("https://localhost/P0011_Intercom/Web/w2.Commerce.Front/Form/Entrance.aspx", cry.Decrypt(cry.Encrypt("https://localhost/P0011_Intercom/Web/w2.Commerce.Front/Form/Entrance.aspx", "aaaa"), "aaaa"));
			Assert.AreEqual("20110824131653282", cry.Decrypt(cry.Encrypt("20110824131653282", "aaaa"), "aaaa"));
			Assert.AreEqual("vaertgvaevbgaegvagvaegvbetb", cry.Decrypt(cry.Encrypt("vaertgvaevbgaegvagvaegvbetb", "aaaa"), "aaaa"));
			Assert.AreEqual("nwyejngvbaergveabvaefgvaqnthwy", cry.Decrypt(cry.Encrypt("nwyejngvbaergveabvaefgvaqnthwy", "aaaa"), "aaaa"));
			Assert.AreEqual("テストmおあｍｂヴぃおあｐ", cry.Decrypt(cry.Encrypt("テストmおあｍｂヴぃおあｐ", "aaaa"), "aaaa"));
		}
	}
}
