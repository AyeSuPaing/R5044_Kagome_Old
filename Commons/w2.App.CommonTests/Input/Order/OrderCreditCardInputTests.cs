using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using Moq;
using w2.App.Common;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.CommonTests._Helper;
using w2.Common.Wrapper;
using w2.CommonTests._Helper;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderCreditCardInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderCreditCardInputTests : AppTestClassBase
	{
		/// <summary>
		/// OrderCreditCardInputコンストラクタ実行
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderCreditCardInputTest()
		{
			// ・インスタンスを生成した際に例外をスローしないこと
			Action act = () => new OrderCreditCardInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderCreditCardInputインスタンスの生成"));
		}

		/// <summary>
		/// UserCreditCardInput作成
		/// ・生成したUserCreditCardInputインスタンスの各プロパティの値が、OrderCreditCardInputの内容と同一であること
		/// ・クレジットカード選択が可能の場合は会社コードが同一となる
		/// ・クレジットカード選択が不可能の場合は会社コードが「」となる
		/// ・登録フラグONの場合はユーザークレジットカード表示フラグがONとなる
		/// ・登録フラグOFFの場合はユーザークレジットカード表示フラグがOFFとなる
		/// </summary>
		[DataTestMethod()]
		[DataRow(Constants.PaymentCard.YamatoKwc, "CCode001", "CCode001", true, Constants.FLG_USERCREDITCARD_DISP_FLG_ON, "クレジット会社が選択可能")]
		[DataRow(Constants.PaymentCard.SBPS, "CCode001", "", true, Constants.FLG_USERCREDITCARD_DISP_FLG_ON, "クレジット会社が選択不可能")]
		[DataRow(Constants.PaymentCard.YamatoKwc, "CCode001", "CCode001", true, Constants.FLG_USERCREDITCARD_DISP_FLG_ON, "登録フラグON")]
		[DataRow(Constants.PaymentCard.YamatoKwc, "CCode001", "CCode001", false, Constants.FLG_USERCREDITCARD_DISP_FLG_OFF, "登録フラグOFF")]
		public void CeateUserCreditCardInputTest(
			Constants.PaymentCard paymentCardKbn,
					string companyCodeInput,
					string companyCodeResult,
					bool doRegister,
					string dispFlgResult,
					string msg)
		{
			var userId = "U001";
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_CARD_KBN), paymentCardKbn))
			{
				var orderCrecInput = new OrderCreditCardInput()
				{
					CompanyCode = companyCodeInput,
					CardNo = "0000111122223333",
					CardNo1 = "0000",
					CardNo2 = "1111",
					CardNo3 = "2222",
					CardNo4 = "3333",
					ExpireMonth = "11",
					ExpireYear = "26",
					AuthorName = "TEST",
					DoRegister = doRegister,
					RegisterCardName = "CDN",
					SecurityCode = "1234",
				};

				var userCreditCardInput = orderCrecInput.CeateUserCreditCardInput(userId);

				// ・生成したUserCreditCardInputインスタンスの各プロパティの値が、OrderCreditCardInputの内容と同一であること
				userCreditCardInput.UserId.Should().Be(userId, "値チェック：userId " + msg);
				userCreditCardInput.CardNo.Should().Be(orderCrecInput.CardNo, "値チェック：cardNo " + msg);
				userCreditCardInput.CardNo1.Should().Be(orderCrecInput.CardNo1, "値チェック：cardNo1 " + msg);
				userCreditCardInput.CardNo2.Should().Be(orderCrecInput.CardNo2, "値チェック：cardNo2 " + msg);
				userCreditCardInput.CardNo3.Should().Be(orderCrecInput.CardNo3, "値チェック：cardNo3 " + msg);
				userCreditCardInput.CardNo4.Should().Be(orderCrecInput.CardNo4, "値チェック：cardNo4 " + msg);
				userCreditCardInput.ExpirationMonth.Should().Be(orderCrecInput.ExpireMonth, "値チェック：expireMonth " + msg);
				userCreditCardInput.ExpirationYear.Should().Be(orderCrecInput.ExpireYear, "値チェック：expireMonth " + msg);
				userCreditCardInput.AuthorName.Should().Be(orderCrecInput.AuthorName, "値チェック：expireMonth " + msg);
				userCreditCardInput.CardDispName.Should().Be(orderCrecInput.RegisterCardName, "値チェック：registerCardName " + msg);
				userCreditCardInput.SecurityCode.Should().Be(orderCrecInput.SecurityCode, "値チェック：securityCode " + msg);
				userCreditCardInput.CreditToken.Should().Be(orderCrecInput.CreditToken, "値チェック：creditToken " + msg);
				// ・クレジットカード選択が可能の場合は会社コードが同一となる
				// ・クレジットカード選択が不可能の場合は会社コードが「」となる
				userCreditCardInput.CompanyCode.Should().Be(companyCodeResult, "値チェック：comapnyCode " + msg);
				// ・登録フラグONの場合はユーザークレジットカード表示フラグがONとなる
				// ・登録フラグOFFの場合はユーザークレジットカード表示フラグがOFFとなる
				userCreditCardInput.DispFlg.Should().Be(dispFlgResult, "値チェック：dispFlg " + msg);
			}
		}

		/// <summary>
		/// 表示用ユーザークレジットカードモデル生成
		/// ・生成したUserCreditCardModelインスタンスの各プロパティの値が、OrderCreditCardInputの内容と同一であること
		/// ・OrderCreditCardInputの登録フラグがtrueの場合、表示フラグがONとなること
		/// ・OrderCreditCardInputの登録フラグがfalseの場合、表示フラグがOFFとなること
		/// </summary>
		[DataTestMethod()]
		[DataRow(true, Constants.FLG_USERCREDITCARD_DISP_FLG_ON, "登録フラグON")]
		[DataRow(false, Constants.FLG_USERCREDITCARD_DISP_FLG_OFF, "登録フラグOFF")]
		public void CeateUserCreditCardModelForDsipTest(bool registFlgInput, string dispFlg, string msg)
		{
			var orderCrecInput = new OrderCreditCardInput();
			Action act = () => orderCrecInput.CeateUserCreditCardModelForDsip();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "空のクレジット情報のとき"));

			orderCrecInput = new OrderCreditCardInput()
			{
				CompanyCode = "CC",
				CardNo = "0000111122223333",
				CardNo1 = "0000",
				CardNo2 = "1111",
				CardNo3 = "2222",
				CardNo4 = "3333",
				ExpireMonth = "11",
				ExpireYear = "26",
				AuthorName = "TEST",
				DoRegister = registFlgInput,
				RegisterCardName = "CDN",
				SecurityCode = "1234",
			};

			var userCreditCardModelForDsip = orderCrecInput.CeateUserCreditCardModelForDsip();

			// ・生成したUserCreditCardModelインスタンスの各プロパティの値が、OrderCreditCardInputの内容と同一であること
			userCreditCardModelForDsip.LastFourDigit.Should().Be("0000111122223333", "LastFourDigit");
			userCreditCardModelForDsip.ExpirationMonth.Should().Be("11", "ExpirationMonth");
			userCreditCardModelForDsip.ExpirationYear.Should().Be("26", "ExpirationYear");
			userCreditCardModelForDsip.AuthorName.Should().Be("TEST", "AuthorName");
			userCreditCardModelForDsip.CardDispName.Should().Be("CDN", "CardDispName");
			// ・OrderCreditCardInputの登録フラグがtrueの場合、表示フラグがONとなること
			// ・OrderCreditCardInputの登録フラグがfalseの場合、表示フラグがOFFとなるこ
			userCreditCardModelForDsip.DispFlg.Should().Be(dispFlg, msg);
		}

		/// <summary>
		/// トークン有効期限切れをチェックする
		/// ・「クレジットトークン使用」かつ「有効期限が現在時刻より過去」→ true
		/// ・「クレジットトークン使用」かつ「有効期限が現在時刻より未来」→ false
		/// ・「クレジットトークンを使用しない」→ false
		/// ・トークン有効期限切れの場合、トークンオブジェクトをnullで上書きされていること
		/// </summary>
		[DataTestMethod()]
		[DataRow(ConfigurationSetting.ReadKbn.C300_Pc, "2020/08/15", "2020/08/16", true, "トークン使用かつ有効期限が過去日")]
		[DataRow(ConfigurationSetting.ReadKbn.C300_Pc, "2020/08/15", "2020/08/14", false, "トークン使用かつ有効期限が未来日")]
		[DataRow(ConfigurationSetting.ReadKbn.C300_ComerceManager, "2020/08/15", "2020/08/14", false, "トークンを使用しない")]
		public void IsTokenExpiredTest(ConfigurationSetting.ReadKbn readKbn, string expiredDate, string nowDate, bool isExpired, string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_CARD_KBN), Constants.PaymentCard.YamatoKwc))
			using (new TestConfigurator(Member.Of(() => Constants.CONFIGURATION_SETTING), new ConfigurationSetting()))
			{
				Constants.CONFIGURATION_SETTING.ReadKbnList.Clear();
				Constants.CONFIGURATION_SETTING.ReadKbnList.Add(readKbn);

				var dateTimeWrapperMock = new Mock<DateTimeWrapper>();
				dateTimeWrapperMock.Setup(s => s.Now).Returns(DateTime.Parse(nowDate));
				DateTimeWrapper.Instance = dateTimeWrapperMock.Object;

				var creditTokenInfoMock = new Mock<CartPayment.CreditTokenInfoBase>();
				creditTokenInfoMock.Setup(s => s.ExpireDate).Returns(DateTime.Parse(expiredDate));
				var orderCreditCardInput = new OrderCreditCardInput
				{
					CreditToken = creditTokenInfoMock.Object
				};

				var isTokenExpired = orderCreditCardInput.IsTokenExpired();

				// ・「クレジットトークン使用」かつ「有効期限が現在時刻より過去」→ true
				// ・「クレジットトークン使用」かつ「有効期限が現在時刻より未来」→ false
				// ・「クレジットトークンを使用しない」→ false
				isTokenExpired.Should().Be(isExpired, msg);
				// ・トークン有効期限切れの場合、トークンオブジェクトをnullで上書きされていること
				if (isExpired)
				{
					orderCreditCardInput.CreditToken.Should().BeNull("discard token：" + msg);
				}
			}
		}
	}
}
