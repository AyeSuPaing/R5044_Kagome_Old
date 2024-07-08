using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using w2.CommonTests;
using w2.CommonTests._Helper;

namespace w2.Domain.FixedPurchase.Helper.Tests
{
	[TestClass()]
	[Ignore]
	public class FixedPurchaseShippingDateCalculatorTests : TestClassBase
	{
		[DataTestMethod()]
		[DataRow(
			Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE,
			"1,25",
			"2011/05/30",
			0,
			NextShippingCalculationMode.Monthly,
			"2011/06/25",
			"日付指定の場合")]
		public void CalculateFollowingShippingDateTest(
			string fixedPurchaseKbn,
			string fixedPurchaseSetting1,
			string baseDateString,
			int slideDay,
			NextShippingCalculationMode calculationMode,
			string expectedString)
		{
			new FixedPurchaseShippingDateCalculator().CalculateFollowingShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting1,
				DateTime.Parse(baseDateString),
				slideDay,
				calculationMode).Should().Be(DateTime.Parse(expectedString));
		}
	}
}
