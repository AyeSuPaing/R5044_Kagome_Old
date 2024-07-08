using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using FluentAssertions;
using w2.Common;

namespace w2.CommonTests._Helper
{
	/// <summary>
	/// TestConfiguratorのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class TestConfiguratorTests
	{
		/// <summary>
		/// テスト設定クラスのテスト
		/// ・usingで破棄後に元の定数値に戻っていること
		/// </summary>
		//[DataTestMethod()]
		//public void TestConfiguratorTest()
		//{
		//	Constants.APPLICATION_NAME = "BEFORE";	// string
		//	Constants.SETTING_PRODUCTION_ENVIRONMENT = true;	// bool
		//	Constants.SERVER_SMTP_PORT = 10;	// int
		//	Constants.SERVER_SMTP_AUTH_TYPE = SmtpAuthType.Normal;  // SmtpAuthType(enum)
		//	Constants.KBN_LOGOUTPUT_SETTINGS = new List<string>{ "A", "B", "C" };	// List<string>

		//	using (new TestConfigurator(Member.Of(() => Constants.APPLICATION_NAME), "AFTER"))
		//	using (new TestConfigurator(Member.Of(() => Constants.SETTING_PRODUCTION_ENVIRONMENT), false))
		//	using (new TestConfigurator(Member.Of(() => Constants.SERVER_SMTP_PORT), 20))
		//	using (new TestConfigurator(Member.Of(() => Constants.SERVER_SMTP_AUTH_TYPE), SmtpAuthType.SmtpAuth))
		//	using (new TestConfigurator(Member.Of(() => Constants.KBN_LOGOUTPUT_SETTINGS), new List<string> { "D", "E", "F" }))
		//	{
		//		Constants.APPLICATION_NAME.Should().Be("AFTER");
		//		Constants.SETTING_PRODUCTION_ENVIRONMENT.Should().Be(false);
		//		Constants.SERVER_SMTP_PORT.Should().Be(20);
		//		Constants.SERVER_SMTP_AUTH_TYPE.Should().Be(SmtpAuthType.SmtpAuth);
		//		Constants.KBN_LOGOUTPUT_SETTINGS[0].Should().Be("D");
		//		Constants.KBN_LOGOUTPUT_SETTINGS[1].Should().Be("E");
		//		Constants.KBN_LOGOUTPUT_SETTINGS[2].Should().Be("F");
		//	}

		//	Constants.APPLICATION_NAME.Should().Be("BEFORE");
		//	Constants.SETTING_PRODUCTION_ENVIRONMENT.Should().Be(true);
		//	Constants.SERVER_SMTP_PORT.Should().Be(10);
		//	Constants.SERVER_SMTP_AUTH_TYPE.Should().Be(SmtpAuthType.Normal);
		//	Constants.KBN_LOGOUTPUT_SETTINGS[0].Should().Be("A");
		//	Constants.KBN_LOGOUTPUT_SETTINGS[1].Should().Be("B");
		//	Constants.KBN_LOGOUTPUT_SETTINGS[2].Should().Be("C");
		//}
	}
}
