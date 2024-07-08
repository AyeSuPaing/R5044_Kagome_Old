using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Web;
using System.IO;
using System.Net;
using w2.Plugin.P0011_Intercom.Util;
using w2.Plugin;
using w2.Plugin.P0011_Intercom;

namespace Test.w2.Plugin.P0011_Intercom
{


	/// <summary>
	///ConcretePluginTest のテスト クラスです。すべての
	///ConcretePluginTest 単体テストをここに含めます
	///</summary>
	[TestClass()]
	public class TestIntercomPlugin
	{
		private TestContext testContextInstance;

		//テスト用のファイル出力パス
		private string testFilePath_ = @"c:\WebServiceTest\IntercomReceive_";

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


		[TestMethod]
		public void OnRegisteredのテスト()
		{
			//ファイルあったら消しておく
			string proc = "UserRegist";

			if (File.Exists(testFilePath_ + proc + ".txt"))
			{
				File.Delete(testFilePath_ + proc + ".txt");
			}

			IntercomPlugin target = new IntercomPlugin();
			
			//テスト用のハッシュ作成
			Hashtable ht = new Hashtable();
			ht.Add("user_id", "0000000001");


			//テスト用のHTTPコンテキスト作成
			HttpContext context = new HttpContext(new HttpRequest("test1.txt", "hsttp://test", ""), new HttpResponse(null));
			context.Request.Cookies.Add(new HttpCookie("テスト項目3", "3"));
			HttpContext.Current = context;

			//init
			target.Initialize(Createtestiplugin(ht, context));
			target.OnRegistered();

			//Assertion
			Assert.IsTrue(File.Exists(testFilePath_ + proc + ".txt"));

		}

		[TestMethod]
		public void OnWithdrawedのテスト()
		{
			//ファイルあったら消しておく
			string proc = "UserDelete";

			if (File.Exists(testFilePath_ + proc + ".txt"))
			{
				File.Delete(testFilePath_ + proc + ".txt");
			}

			IntercomPlugin target = new IntercomPlugin();
			//ConcretePlugin_Accessor sa = new ConcretePlugin_Accessor();
			IntercomPlugin sa = new IntercomPlugin();

			//テスト用のハッシュ作成
			Hashtable ht = new Hashtable();
			ht.Add("user_id", "0000000001");


			//テスト用のHTTPコンテキスト作成
			HttpContext context = new HttpContext(new HttpRequest("test1.txt", "hsttp://test", ""), new HttpResponse(null));
			context.Request.Cookies.Add(new HttpCookie("テスト項目3", "3"));

			HttpContext.Current = context;

			//init
			sa.Initialize(Createtestiplugin(ht, context));
			sa.OnWithdrawed();




			Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

			string str = "";

			using (FileStream fs = new FileStream(@"C:\inetpub\wwwroot\P0011_Intercom\Plugins\w2.Plugin.P0011_Intercom.Test\bin\Debug\P0011_IntercomPluginConfig.xml",
				FileMode.Open, FileAccess.Read))
			{
				StreamReader sr = new StreamReader(fs);
				str = sr.ReadToEnd();
				sr.Close();
			}

			using (FileStream fs = new FileStream(@"C:\inetpub\wwwroot\P0011_Intercom\Plugins\w2.Plugin.P0011_Intercom.Test\bin\Debug\P0011_IntercomPluginConfig.xml",
				FileMode.Create, FileAccess.Write))
			{

				StreamWriter sw = new StreamWriter(fs);
				sw.Write(str.Replace("co.jp", "ne.jp"));
				sw.Close();
			}

			System.Threading.Thread.Sleep(1000);

			sa.OnWithdrawed();

			using (FileStream fs = new FileStream(@"C:\inetpub\wwwroot\P0011_Intercom\Plugins\w2.Plugin.P0011_Intercom.Test\bin\Debug\P0011_IntercomPluginConfig.xml",
			FileMode.Open, FileAccess.Read))
			{
				StreamReader sr = new StreamReader(fs);
				str = sr.ReadToEnd();
				sr.Close();
			}

			using (FileStream fs = new FileStream(@"C:\inetpub\wwwroot\P0011_Intercom\Plugins\w2.Plugin.P0011_Intercom.Test\bin\Debug\P0011_IntercomPluginConfig.xml",
				FileMode.Create, FileAccess.Write))
			{

				StreamWriter sw = new StreamWriter(fs);
				sw.Write(str.Replace("ne.jp", "co.jp"));
				sw.Close();
			}

			//Assertion
			//Assert.IsTrue(File.Exists(testFilePath_ + proc + ".txt"));

		}

		[TestMethod]
		public void OnCompletedのテスト()
		{
			//ファイルあったら消しておく
			string proc = "SerialDelete";

			if (File.Exists(testFilePath_ + proc + ".txt"))
			{
				File.Delete(testFilePath_ + proc + ".txt");
			}

			IntercomPlugin target = new IntercomPlugin();
		
			//テスト用のハッシュ作成
			Hashtable ht = new Hashtable();
			ht.Add("userid", "0000000001");


			//テスト用のHTTPコンテキスト作成
			HttpContext context = new HttpContext(new HttpRequest("test1.txt", "hsttp://test", ""), new HttpResponse(null));
			context.Request.Cookies.Add(new HttpCookie("productid", "w2test0093"));
			context.Request.Cookies.Add(new HttpCookie("serialkey", "//+6j3TJjEjWn3ckuae9wg=="));


			//init
			target.Initialize(Createtestiplugin(ht, context));
			target.OnCompleted();

			//Assertion
			//テスト用のwebサービスでc:\senddata.txtを作成するので検証
			Assert.IsTrue(File.Exists(testFilePath_ + proc + ".txt"));

		}


		[TestMethod]
		public void OnValidatingのテスト()
		{
			//ファイルあったら消しておく
			string proc = "SerialCheck";

			if (File.Exists(testFilePath_ + proc + ".txt"))
			{
				File.Delete(testFilePath_ + proc + ".txt");
			}

			IntercomPlugin target = new IntercomPlugin();
	
			//テスト用のハッシュ作成
			Hashtable ht = new Hashtable();
			ht.Add("userid", "0000000001");

			//テスト用のHTTPコンテキスト作成
			HttpContext context = new HttpContext(new HttpRequest("test1.txt", "hsttp://test", ""), new HttpResponse(null));
			context.Request.Cookies.Add(new HttpCookie("productid", "w2test0093"));
			context.Request.Cookies.Add(new HttpCookie("serialkey", "//+6j3TJjEjWn3ckuae9wg=="));

			//init
			target.Initialize(Createtestiplugin(ht, context));
			target.OnValidating();

			//Assertion
			//テスト用のwebサービスでc:\senddata.txtを作成するので検証
			Assert.IsTrue(File.Exists(testFilePath_ + proc + ".txt"));
		}

		[TestMethod]
		public void OnOnLoggedInのテスト()
		{
			//ファイルあったら消しておく
			string proc = "LoggedIn";

			if (File.Exists(testFilePath_ + proc + ".txt"))
			{
				File.Delete(testFilePath_ + proc + ".txt");
			}

			IntercomPlugin target = new IntercomPlugin();
		
			//テスト用のハッシュ作成
			Hashtable ht = new Hashtable();
			ht.Add("user_id", "0000000001");


			//テスト用のHTTPコンテキスト作成
			HttpContext context = new HttpContext(new HttpRequest("test1.txt", "hsttp://test", ""), new HttpResponse(null));
			context.Request.Cookies.Add(new HttpCookie("テスト項目3", "3"));
			HttpContext.Current = context;

			//init
			target.Initialize(Createtestiplugin(ht, context));
			target.OnLoggedIn();

			//Assertion
			//Assert.IsTrue(File.Exists(testFilePath_ + proc + ".txt"));

		}

		//テスト用のtestipluginファクトリ
		private IPluginHost Createtestiplugin(Hashtable data, HttpContext context)
		{
			IPluginHost ipht;
			ipht = new testipluginhost(data, context);

			return ipht;
		}

		private class testipluginhost : IPluginHost
		{

			private Hashtable data_;
			private System.Web.HttpContext context_;

			public testipluginhost(Hashtable data, HttpContext context)
			{
				data_ = data;
				context_ = context;
			}

			#region IPluginHost メンバー

			public void WriteErrorLog(string message)
			{
				Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
				using (StreamWriter writer = new StreamWriter(@"C:\WebServiceTest\pluginerr.txt", false, sjisEnc))
				{
					writer.WriteLine(message);
				}
			}

			public void WriteErrorLog(Exception ex, string message = "")
			{
				Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
				using (StreamWriter writer = new StreamWriter(@"C:\WebServiceTest\pluginerr.txt", false, sjisEnc))
				{
					writer.WriteLine(ex.Message + ex.StackTrace);
				}
			}

			public void WriteInfoLog(string message)
			{
				Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
				using (StreamWriter writer = new StreamWriter(@"C:\WebServiceTest\pluginInfo.txt", false, sjisEnc))
				{
					writer.WriteLine(message);
				}
			}

			public void WriteInfoLog(Exception ex, string message = "")
			{
				Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
				using (StreamWriter writer = new StreamWriter(@"C:\WebServiceTest\pluginInfo.txt", false, sjisEnc))
				{
					writer.WriteLine(ex.Message + ex.StackTrace);
				}
			}

			public bool SendMail(string subject, string body)
			{
				throw new NotImplementedException();
			}

			public bool SendMail(string subject, string body, string from, List<string> toList, List<string> ccList = null, List<string> bccList = null)
			{
				Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
				using (StreamWriter writer = new StreamWriter(@"C:\WebServiceTest\pluginMail.txt", false, sjisEnc))
				{
					writer.WriteLine("subject:" + subject);
					writer.WriteLine("body:" + body);
					writer.WriteLine("from:" + from);
					if (toList != null)
					{
						foreach (string to in toList)
						{
							writer.WriteLine("toList:" + to);
						}
					}

					if (ccList != null)
					{
						foreach (string cc in ccList)
						{
							writer.WriteLine("ccList:" + cc);
						}
					}

					if (bccList != null)
					{
						foreach (string bcc in bccList)
						{
							writer.WriteLine("bccList:" + bcc);
						}
					}

				}

				return true;
			}

			public Hashtable Data
			{
				get { return data_; }
			}

			public System.Web.HttpContext Context
			{
				get { return context_; }
			}

			#endregion
		}
	}
}
