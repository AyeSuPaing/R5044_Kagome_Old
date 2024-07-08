using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test.HogePaymentLib.Auth
{
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class AuthRequest
	{
		[XmlElement("orderid")]
		public string OrderId;

		[XmlElement("amount")]
		public decimal Amount;

		[XmlElement("saleflag")]
		public bool SaleFlag;
	}
}
