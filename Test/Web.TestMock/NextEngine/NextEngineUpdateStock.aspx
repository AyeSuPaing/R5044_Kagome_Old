<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" ValidateRequest="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<script runat="server">
	void Main()
	{
		// ストアアカウント
		var storeAccount = "samplestore";
		// 認証キー
		var storeKey = "aaa";
		// 商品コード SKU
		var code = "nextEngine01";
		// 在庫数
		var stock = "998";
		// 実行日時
		var ts = DateTime.Now.ToString("yyyyMMddHHmm");

		var querry = string.Format("StoreAccount={0}&Code={1}&Stock={2}&ts={3}", storeAccount, code, stock, ts);

		// 認証情報
		var sig = ConvertMd5Hash(querry + storeKey);

		var url = "http://localhost/R5044_Kagome.Develop/WebApi/w2.Commerce.WebApi/Mall/NextEngine/NextEngineUpdateStock.ashx?" + querry + "&.sig=" + sig;
		var webRequest = (HttpWebRequest)WebRequest.Create(url);
		webRequest.Method = "GET";
		webRequest.ContentType = "application/x-www-form-urlencoded";
		var res = webRequest.GetResponse();

		var st = res.GetResponseStream();
		var sr = new StreamReader(st, Encoding.GetEncoding("EUC-JP"));
		var xml = sr.ReadToEnd();

		this.Response.Clear();
		this.Response.ContentType = "text/Text";
		this.Response.ContentEncoding = Encoding.GetEncoding("EUC-JP");
		this.Response.Write(xml);
	}

	private string ConvertMd5Hash(string value)
	{
		var byteData = Encoding.ASCII.GetBytes(value);
		var md5 = new MD5CryptoServiceProvider();
		var byteHash = md5.ComputeHash(byteData);
		var hash = new System.Text.StringBuilder();
		foreach (var b in byteHash)
		{
			hash.Append(b.ToString("x2"));
		}

		var result = hash.ToString();
		return result;
	}
</script>
<% this.Main(); %>