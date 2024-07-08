<%--
=========================================================================================================
Module      : 商品カテゴリツリー出力コントローラ(BodyProductCategoryTree.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductCategoryTree.ascx.cs" Inherits="Form_Common_Product_BodyProductCategoryTree" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
	base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<%-- デフォルト表示カテゴリー階層 --%>
<%-- 1～10のいずれかを設定する（デフォルトは1）--%>
this.DefaultDisplayCategoryDepth = 1;
<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (this.IsVisibleCategory) { %>
<section class="search-category unit">
<h3 class="title">カテゴリーから探す</h3>
<nav>
	<asp:Repeater ID="rCategoryList" runat="server">
	<HeaderTemplate><ul class="global-nav-2 clearfix"><!--</HeaderTemplate>
	<ItemTemplate>
		--><li id="ca<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryId) %>" class="<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryId) == this.CategoryId ? "visit" : "" %>"><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'><%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a></li><!--
	</ItemTemplate>
	<FooterTemplate>--></ul></FooterTemplate>
	</asp:Repeater>
</nav>
</section>
<% } %>
<%-- △編集可能領域△ --%>