using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using FluentAssertions;
using w2.Common.Web;
using w2.CommonTests._Helper;

namespace w2.CommonTests.Web
{
	//[TestClass()]
	//[Ignore]
	//public class UrlValidatorTest : TestClassBase
	//{
	//	/// <summary>カレントホスト</summary>
	//	const string CURRENT_HOST = "www.w2.com";

	//	[DataTestMethod()]
	//	[DataRow(@"../", "相対指定（上位階層）")]
	//	[DataRow(@"/w2/", "相対指定（下位階層）")]
	//	[DataRow(@"w2w2", "相対指定（同階層）")]
	//	[DataRow(@"http://" + CURRENT_HOST, "絶対指定（スキーム有り）")]
	//	[DataRow(@"//" + CURRENT_HOST, "絶対指定（//開始）")]
	//	[DataRow(@"", "空文字（同階層と認識）")]
	//	public void IsSameHostUsedTest_IsTrue(string url, string msg)
	//	{
	//		UrlValidator.IsSameHostUsed(CURRENT_HOST, url).Should().BeTrue(msg);
	//	}

	//	[DataTestMethod()]
	//	[DataRow(@"http://www.google.co.jp/", "絶対指定（スキーム有り・別ホスト）")]
	//	[DataRow(@"//www.google.co.jp", "絶対指定（//開始・別ホスト）")]
	//	[DataRow(@"/\/www.google.co.jp", "オープンリダイレクトアタックの攻撃パターンの１つ（別ホスト）")]
	//	[DataRow(null, "nullはURL作成できないためfalse")]
	//	public void IsSameHostUsedTest_IsFalse(string url, string msg)
	//	{
	//		UrlValidator.IsSameHostUsed(CURRENT_HOST, url).Should().BeFalse(msg);
	//	}

	//	/// <summary>GetAltUrlIfOtherHostUsedTest向け 別URL</summary>
	//	const string ALT_URL = "http://yahoo.co.jp";

	//	[DataTestMethod()]
	//	[DataRow(@"/w2/", "同ホストURL")]
	//	public void GetAltUrlIfOtherHostUsedTest_IsSame(string url, string msg)
	//	{
	//		UrlValidator.GetAltUrlIfOtherHostUsed(CURRENT_HOST, url, ALT_URL).Should().Be(url);
	//	}

	//	[DataTestMethod()]
	//	[DataRow(@"//www.google.co.jp", "別ホストURL")]
	//	public void GetAltUrlIfOtherHostUsedTest_IsDifferent(string url, string msg)
	//	{
	//		UrlValidator.GetAltUrlIfOtherHostUsed(CURRENT_HOST, url, ALT_URL).Should().Be(ALT_URL);
	//	}
	//}
}

