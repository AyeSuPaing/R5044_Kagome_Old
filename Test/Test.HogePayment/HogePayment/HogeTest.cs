using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.HogePaymentLib;
using Test.HogePaymentLib.Auth;
using Test.HogePaymentLib.Cancel;
using Test.HogePaymentLib.Sale;

namespace Test.HogePayment
{
	[TestClass()]
	public class HogeTest
	{
		public HogeSetting CreateSetting()
		{
			var setting = new HogeSetting("http://localhost/V5.14/Test/Web.PaymentTest/w2Hoge/Auth.aspx",
				"http://localhost/V5.14/Test/Web.PaymentTest/w2Hoge/Sale.aspx",
				"http://localhost/V5.14/Test/Web.PaymentTest/w2Hoge/Cancel.aspx");

			return setting;
		}

		[TestMethod()]
		public void AuthTest()
		{
			var payment = new PaymentHoge(CreateSetting());
			var request = new AuthRequest();
			request.OrderId = "1234567890";
			request.Amount = 1280;

			var res = payment.Auth(request);
			Console.WriteLine(HogeHelper.Serialize(res));
			Assert.AreEqual(res.ResultCode, "OK");

			request.Amount = 1000;
			var res2 = payment.Auth(request);
			Console.WriteLine(HogeHelper.Serialize(res2));
			Assert.AreEqual(res2.ResultCode, "NG");
		}

		[TestMethod()]
		public void SaleTest()
		{

			var payment = new PaymentHoge(CreateSetting());
			var request = new SaleRequest();
			request.TransactionId = "1234567890";
			request.TransactionKey = "keyzzxxzz";
			request.Amount = 2280;

			var res = payment.Sale(request);
			Console.WriteLine(HogeHelper.Serialize(res));
			Assert.AreEqual(res.ResultCode, "OK");

			request.Amount = 2000;
			var res2 = payment.Sale(request);
			Console.WriteLine(HogeHelper.Serialize(res2));
			Assert.AreEqual(res2.ResultCode, "NG");
		}

		[TestMethod()]
		public void CancelTest()
		{
			var payment = new PaymentHoge(CreateSetting());
			var request = new CancelRequest();
			request.TransactionId = "1234567890";
			request.TransactionKey = "keyzzxxzz";
			request.Amount = 3280;

			var res = payment.Cancel(request);
			Console.WriteLine(HogeHelper.Serialize(res));
			Assert.AreEqual(res.ResultCode, "OK");

			request.Amount = 3000;
			var res2 = payment.Cancel(request);
			Console.WriteLine(HogeHelper.Serialize(res2));
			Assert.AreEqual(res2.ResultCode, "NG");
		}
	}
}
