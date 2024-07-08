using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test.HogePaymentLib.Auth
{
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class AuthResponse
	{
		[XmlElement("resultcode")]
		public string ResultCode;

		[XmlElement("transactionid")]
		public string TransactionID;

		[XmlElement("transactionkey")]
		public string TransactionKey;
	}
}
