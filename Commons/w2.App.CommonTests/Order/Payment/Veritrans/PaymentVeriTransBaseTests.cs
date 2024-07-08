using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.CommonTests._Helper;

namespace w2.App.CommonTests.Order.Payment.Veritrans
{
	/// <summary>
	/// PaymentVeriTransBaseのテスト
	/// </summary>
	[TestClass()]
	public class PaymentVeriTransBaseTests : AppTestClassBase
	{
		/// <summary>
		/// JPO 支払情報作成のテスト
		/// </summary>
		[DataTestMethod()]
		[DataRow("01", "10", "一括")]
		[DataRow("02", "61C02", "02 回")]
		[DataRow("03", "61C03", "03 回")]
		[DataRow("05", "61C05", "05 回")]
		[DataRow("06", "61C06", "06 回")]
		[DataRow("10", "61C10", "10 回")]
		[DataRow("12", "61C12", "12 回")]
		[DataRow("99", "80", "リボ")]
		public void CreateJpoTest(string src, string jpo, string msg)
		{
			((string)new PrivateObject(new PaymentVeritransCredit()).Invoke("CreateJpo", new object[] { src })).Should().Be(jpo, msg);
			((string)new PrivateObject(new PaymentVeritransCredit3DS()).Invoke("CreateJpo", new object[] { src })).Should().Be(jpo, msg);
		}
	}
}
