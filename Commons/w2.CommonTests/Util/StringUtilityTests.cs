using Microsoft.VisualStudio.TestTools.UnitTesting;
//using FluentAssertions;
using System;
using w2.CommonTests._Helper;
using w2.Common.Util;

namespace w2.CommonTests.Util.Tests
{
	//[TestClass()]
	//[Ignore]
	//public class StringUtilityTests : TestClassBase
	//{
	//	[DataTestMethod()]
	//	[DataRow(null, "", "null => 空置換")]
	//	[DataRow("", "", "空→空置換")]
	//	[DataRow(123, "123", "数値の文字列置換")]
	//	[DataRow(true, "True", "boolの文字列置換")]
	//	public void ToEmptyTest_空文字変換テスト(object src, string dst, string msg)
	//	{
	//		StringUtility.ToEmpty(src).Should().Be(dst, msg);
	//	}

	//	/// <summary>
	//	///ToValue のテスト
	//	///</summary>
	//	[DataTestMethod()]
	//	[DataRow(null, "123", "123", "null => 文字列置換")]
	//	[DataRow("", "123", "", "空→空のまま")]
	//	public void ToValueTest_置換テスト(object src, string rep, string dst, string msg)
	//	{
	//		StringUtility.ToValue(src, rep).Should().Be(dst, msg);
	//	}

	//	/// <summary>
	//	///RemoveUnavailableControlCode のテスト
	//	///</summary>
	//	[DataTestMethod()]
	//	public void RemoveUnavailableControlCodeTest()
	//	{
	//		var source = "";
	//		for (var i = 0; i < (int)0x20; i++)
	//		{
	//			source += (char)i;
	//		}
	//		source += (char)0x7f;

	//		var result = StringUtility.RemoveUnavailableControlCode(source);
	//		result.Should().Be("\t\n\r");// タブ、改行、復帰、は許可
	//	}

	//	/// <summary>
	//	/// RemoveUnavailableControlCode のテスト
	//	///</summary>
	//	[DataTestMethod()]
	//	[DataRow("0123456798", 4, new[] { "0123", "4567", "98" }, "正常系")]
	//	[DataRow("012", 4, new[] { "012" }, "lenght未満")]
	//	[DataRow("", 4, new String [0], "空文字")]
	//	public void SplitByLengthTest(string src, int length, string[] exp, string msg)
	//	{
	//		var results = StringUtility.SplitByLength(src, length);
	//		results.Length.Should().Be(exp.Length);
	//		results.Should().BeEquivalentTo(exp,msg);
	//	}
	//}
}
