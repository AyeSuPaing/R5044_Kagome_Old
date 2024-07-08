<!-- 翻訳タグ検証 -->
<%@ Control Language="C#" AutoEventWireup="true" %>
<script runat="server">
	protected void Page_Load(object sender, System.EventArgs e)
	{
		Literal1.Text = "翻訳タグは正常に動作しています。";
		Literal2.Text = "翻訳タグは正常に動作しています。";
		Literal3.Text = "翻訳タグは正常に動作しています。";
		Literal4.Text = "翻訳タグは正常に動作しています。";
		DataBind();
	}
	String GetMessage()
	{
		return "翻訳タグは正常に動作しています。";
	}
</script>
【原文】翻訳タグは正常に動作しています。<br />
■現在のユーザのリージョンデータによる判別<br />
-■HTMLエンコードなし<br />
<w2c:Translation runat="server">
	<asp:Literal ID="Literal1" runat="server" /><br />
	<%# GetMessage() %><br />
	<%#: GetMessage() %><br />
</w2c:Translation>
<br />
-■HTMLエンコードあり<br />
<w2c:Translation runat="server" HtmlEncode="True">
	<asp:Literal ID="Literal2" runat="server" /><br />
	<%# GetMessage() %><br />
	<%#: GetMessage() %><br />
</w2c:Translation>
<br />
<br />
■言語指定あり<br />
-■HTMLエンコードなし<br />
<w2c:Translation runat="server" Lang="en">
	<asp:Literal ID="Literal3" runat="server" /><br />
	<%# GetMessage() %><br />
	<%#: GetMessage() %><br />
</w2c:Translation>
<br />
-■HTMLエンコードあり<br />
<w2c:Translation runat="server" Lang="en" HtmlEncode="True">
	<asp:Literal ID="Literal4" runat="server" /><br />
	<%# GetMessage() %><br />
	<%#: GetMessage() %><br />
</w2c:Translation>
<br />

