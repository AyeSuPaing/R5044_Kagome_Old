<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" ValidateRequest="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<script runat="server">
	void Main()
	{
		var to = this.Request["to"];
		var id = DateTime.Now.ToString("yyyyMMddHHmmssfff");
		var st = "200";
		var text = ConvertUCS2ToString(this.Request["text"]);

		// DN受信用Mockに投げる
		// 適宜環境に合わせてURLを調整
		var urlAccp = string.Format("http://localhost/R5044_Kagome.Develop/Web/w2.Commerce.Front/Payment/MacroKiosk/Receive.aspx?msgID={0}&msisdn={1}&status={2}",
			id, to, "ACCEPTED");

		var urlDeli = string.Format("http://localhost/R5044_Kagome.Develop/Web/w2.Commerce.Front/Payment/MacroKiosk/Receive.aspx?msgID={0}&msisdn={1}&status={2}",
			id, to, "DELIVERED");

		HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(urlAccp);
		webRequest.Method = "GET";
		webRequest.ContentType = "application/x-www-form-urlencoded";
		var res1 = webRequest.GetResponse();

		System.Threading.Thread.Sleep(200);

		HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(urlDeli);
		webRequest2.Method = "GET";
		webRequest2.ContentType = "application/x-www-form-urlencoded";
		var res2 = webRequest2.GetResponse();

		var res = string.Format("{0},{1},{2}", to, id, st);
		this.Response.Write(res);
	}

	/// <summary>
	/// Convert UCS2 to string
	/// </summary>
	/// <param name="hexaText">Hexa text</param>
	/// <returns>String</returns>
	string ConvertUCS2ToString(string hexaText)
	{
		hexaText = hexaText.Replace(" ", string.Empty);
		var byteLenght = hexaText.Length / 2;
		var arrayByte = new byte[byteLenght];
		
		using (var stringReader = new StringReader(hexaText))
		{
			for (int index = 0; index < byteLenght; index++)
			{
				arrayByte[index] = Convert.ToByte(
					new String(new char[2] { (char)stringReader.Read(), (char)stringReader.Read() }),
					16);
			}
		}

		var result = Encoding.BigEndianUnicode.GetString(arrayByte, 0, arrayByte.Length);
		return result.Replace(",", string.Empty);
	}
</script>
<% this.Main(); %>