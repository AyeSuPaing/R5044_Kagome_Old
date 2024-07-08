using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test.HogePaymentLib.Cancel
{
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class CancelRequest
	{
		[XmlElement("transactionid")]
		public string TransactionId;

		[XmlElement("transactionkey")]
		public string TransactionKey;

		[XmlElement("amount")]
		public decimal Amount;
	}
}
