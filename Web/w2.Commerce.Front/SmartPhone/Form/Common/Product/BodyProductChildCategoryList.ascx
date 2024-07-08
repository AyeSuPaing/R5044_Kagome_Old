<%--
=========================================================================================================
Module      : 商品子カテゴリ一覧出力コントローラ(BodyProductChildCategoryList.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductChildCategoryList.ascx.cs" Inherits="Form_Common_Product_BodyProductChildCategoryList" %>
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
// カテゴリIDを指定します
this.CategoryId = "ROOT";

<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater ID="rCategoryList" runat="server">
<HeaderTemplate><ul></HeaderTemplate>
<ItemTemplate>
		<li>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>
				<%# WebSanitizer.HtmlEncode(Eval("CategoryName"))%>
			</a>
			<div></div>
		</li>
</ItemTemplate>
<FooterTemplate></ul></FooterTemplate>
</asp:Repeater>
<%-- △編集可能領域△ --%>