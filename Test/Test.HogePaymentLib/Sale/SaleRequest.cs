using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test.HogePaymentLib.Sale
{
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class SaleRequest
	{
		[XmlElement("transactionid")]
		public string TransactionId;

		[XmlElement("transactionkey")]
		public string TransactionKey;

		[XmlElement("amount")]
		public decimal Amount;
	}
}
