<%--
=========================================================================================================
  Module      : 商品のカラー検索パーツ (ProductColorSearchBox.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/ProductColorSearchBox.ascx.cs"  Inherits="Form_Common_Product_ProductColorSearchBox" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="商品のカラー検索パーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<script runat="server">
	public new void Page_Init(Object sender, EventArgs e)
	{
		base.Page_Init(sender, e);
<%-- ▽編集可能領域：プロパティ設定▽ --%>
<%-- △編集可能領域△ --%>
	}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="search-category unit" runat="server" Visible="<%# this.IsVisible %>">
<h3 class="title">カラーから探す</h3>

<nav style="text-align:center; padding: 0px 0px 2px 2px;">
	<asp:Repeater ID="rColorSearch" runat="server" ItemType="w2.App.Common.Product.ProductColor">
		<HeaderTemplate><br /><div class="categoryList" style="display: inline-block; text-align:left;"></HeaderTemplate>
		<ItemTemplate>
			<asp:LinkButton runat="server" id="lbSearch" onclick="lbSearch_Click" CommandArgument='<%# Item.Id %>'>
				<img src='<%#: Item.Url %>' Width="50" Height="50" style="padding: 5px 5px 4px 4px;" />
			</asp:LinkButton>
		</ItemTemplate>
		<FooterTemplate></div></FooterTemplate>
	</asp:Repeater>
</nav>
</section>
<%-- △編集可能領域△ --%>