<%--
=========================================================================================================
Module      : DN受信 (Receive.aspx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%-- Paymentフォルダ内はメンテ除外になっているため、ここに入れる --%>
<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" ValidateRequest="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	void Main()
	{
		// 必要に応じて出力先のディレクトリ
		string dirpath =  Constants.MACROKIOSK_DN_OUTPUT_DIR_PATH;
		if (!Directory.Exists(dirpath))
		{
			Directory.CreateDirectory(dirpath);
		}

		string filepath = Path.Combine(dirpath, string.Format(@"{0}.txt", DateTime.Now.ToString("rev_yyyyMMddHHmmssfff")));
		using (var fout = new StreamWriter(filepath, false))
		{
			//fout.WriteLine("受信しました。");
			var output = new List<string>();

			foreach (string pk in this.Request.QueryString.Keys)
			{
				output.Add(string.Format("{0}：{1}", pk, this.Request.QueryString[pk]));
			}
			fout.WriteLine(string.Join(",", output.ToArray()));
		}
	}
</script>
<% this.Main(); %>