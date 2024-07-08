using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test.HogePaymentLib.Cancel
{
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class CancelResponse
	{
		[XmlElement("resultcode")]
		public string ResultCode;
	}
}
