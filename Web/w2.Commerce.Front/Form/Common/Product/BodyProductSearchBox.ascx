<%--
=========================================================================================================
  Module      : 商品検索ボックス出力コントローラ(BodyProductSearchBox.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductSearchBox.ascx.cs" Inherits="Form_Common_Product_BodyProductSearchBox" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<%
	// 検索テキストボックスEnterで検索させる（UpdatePanelで括っておかないと非同期処理時に検索が効かなくなる）
	this.WtbSearchWord.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
	this.WtbMinPrice.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
	this.WtbMaxPrice.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
%>
</ContentTemplate>
</asp:UpdatePanel>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvProductSearch">
	<dl>
		<dt><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/label_productsearch.jpg" alt="商品の検索" /></dt>
		<dd><asp:DropDownList ID="ddlCategories" runat="server"></asp:DropDownList></dd>
		<dd class="btn_search_mini"><asp:TextBox ID="tbSearchWord" runat="server" CssClass="text" MaxLength="250"></asp:TextBox></dd>
		<dd class="text_search_price_mini"><asp:TextBox ID="tbMinPrice" runat="server" CssClass="text" Width="50" MaxLength="7"></asp:TextBox>円 ～ 
		<asp:TextBox ID="tbMaxPrice" runat="server" CssClass="text" Width="50" MaxLength="7"></asp:TextBox>円</dd>
		<dd><asp:CheckBox ID="cbStockOnly" Text="在庫あり" runat="server" /></dd>
		<dd class="btn_search_mini"><asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_Click"><span>検索</span></asp:LinkButton></dd>
		<dd>→ <a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/AdvancedSearch.aspx" +  (this.BrandId=="" ? "" : "?bid=" + this.BrandId)) %>">詳しく検索</a></dd>
	</dl>
</div>
<%-- △編集可能領域△ --%>