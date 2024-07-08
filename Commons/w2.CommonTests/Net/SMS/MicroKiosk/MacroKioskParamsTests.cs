using Microsoft.VisualStudio.TestTools.UnitTesting;
//using FluentAssertions;
using System.Web;
using w2.CommonTests._Helper;

namespace w2.CommonTests.Net.SMS.MicroKiosk.Tests
{
	//[TestClass()]
	//[Ignore]
	//public class MacroKioskParamsTests : TestClassBase
	//{
	//	[DataTestMethod()]
	//	public void CreatePostStringTest()
	//	{
	//		var para = new w2.Common.Net.SMS.MicroKiosk.MacroKioskParams();
	//		para.SetUser("TEST").SetPass("papss").SetTo("81123456").SetFrom("w2 Solution").SetText("テスト");
	//		para.Title = "おしらせ";
	//		var ps = para.CreatePostString();

	//		var result = string.Format(
	//			"user={0}&pass={1}&type={2}&to={3}&from={4}&text={5}&servid={6}&title={7}",
	//			HttpUtility.UrlEncode("TEST"),
	//			HttpUtility.UrlEncode("papss"),
	//			HttpUtility.UrlEncode("5"),
	//			HttpUtility.UrlEncode("81123456"),
	//			HttpUtility.UrlEncode("w2 Solution"),
	//			HttpUtility.UrlEncode("30C630B930C8"),
	//			HttpUtility.UrlEncode("MES01"),
	//			HttpUtility.UrlEncode("304A30573089305B"));

	//		ps.Should().Be(result);

	//		// 文字列のUSC2変換は下記提供のユーティリティで確認できます
	//		// http://utilities.etracker.cc/converter/converter.aspx
	//	}

	//	// 通信が発生するためいったんコメントアウト
	//	//[DataTestMethod]
	//	//public void TestConn()
	//	//{
	//	//	var para = new MacroKioskParams();
	//	//	para.SetUser("TEST256").SetPass("1I)qletw").SetTo("8109012178545").SetFrom("w2 Solution").SetText("テスト");
	//	//	var connector = new HttpApiConnector();
	//	//	var url = "";

	//	//	// マクロキオスクへつなげちゃうと課金されるので必要な時のみつかってください
	//	//	//url = "http://www.etracker.cc/bulksms/mesapi.aspx";
	//	//	// モック使う場合はビルドしてね
	//	//	url = "http://localhost/V5/Test/Web.TestMock/MacroKiosk/mesapi.aspx";

	//	//	var res = connector.Do(url, Encoding.ASCII, para, "", "");
	//	//	Console.WriteLine(res);

	//	//	res.Split(',').Length.Should().Be(3, "パラメタ３つ");
	//	//	res.Split(',')[0].Should().Be("8109012178545", "1つめのパラメタ");
	//	//	res.Split(',')[0].Should().Be("200", "3つめのパラメタ");
	//	//}
	//}
}
