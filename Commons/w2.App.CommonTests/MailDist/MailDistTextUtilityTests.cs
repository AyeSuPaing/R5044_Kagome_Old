using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using w2.App.CommonTests._Helper;

namespace w2.App.Common.MailDist.Tests
{
	[TestClass()]
	[Ignore]
	public class MailDistTextUtilityTests : AppTestClassBase
	{
		[DataTestMethod()]
		[DataRow(true, "</a>", "HTML")]
		[DataRow(false, "\n", "NOT HTML")]
		public void GetSeparatePatternTest(bool isHtml, string exp, string msg)
		{
			MailDistTextUtility.GetSeparatePattern(isHtml).Should().Be(exp, msg);
		}
	}
}
