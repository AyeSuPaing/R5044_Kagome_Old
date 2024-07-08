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
<div class="dvCategorySearch">

<%-- １階層目 --%>
<asp:Repeater ID="rCategoryList" runat="server">
<HeaderTemplate>
<div class="categoryList">
<a href="javascript:void(0);" class="title toggle active">カテゴリーから探す</a>
<ul>
</HeaderTemplate>
<ItemTemplate>
	<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>
		<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
	<%-- ２階層目 --%>
	<asp:Repeater ID="Repeater1" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
	<HeaderTemplate><ul></HeaderTemplate>
	<ItemTemplate>
	<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
		<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
		<%-- ３階層目 --%>
		<asp:Repeater ID="Repeater2" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
		<HeaderTemplate><ul></HeaderTemplate>
		<ItemTemplate>
			<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
				<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
			<%-- ４階層目 --%>
			<asp:Repeater ID="Repeater3" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
			<HeaderTemplate><ul></HeaderTemplate>
			<ItemTemplate>
				<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
					<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
				<%-- ５階層目 --%>
				<asp:Repeater ID="Repeater4" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
				<HeaderTemplate><ul></HeaderTemplate>
				<ItemTemplate>
					<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
						<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
					<%-- ６階層目 --%>
					<asp:Repeater ID="Repeater5" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
					<HeaderTemplate><ul></HeaderTemplate>
					<ItemTemplate>
						<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
							<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
						<%-- ７階層目 --%>
						<asp:Repeater ID="Repeater6" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
						<HeaderTemplate><ul></HeaderTemplate>
						<ItemTemplate>
							<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
								<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
							<%-- ８階層目 --%>
							<asp:Repeater ID="Repeater7" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
							<HeaderTemplate><ul></HeaderTemplate>
							<ItemTemplate>
								<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
									<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
								<%-- ９階層目 --%>
								<asp:Repeater ID="Repeater8" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
								<HeaderTemplate><ul></HeaderTemplate>
								<ItemTemplate>
									<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
										<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a>
									<%-- １０階層目 --%>
									<asp:Repeater ID="Repeater9" DataSource='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((ProductCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
									<HeaderTemplate><ul></HeaderTemplate>
									<ItemTemplate>
										<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval("ShopId"), Eval("CategoryId"), Eval("CategoryUrl"), (string)Eval("CategoryName"))) %>'>┗
											<%# WebSanitizer.HtmlEncode(((ProductCategoryTreeNode)Container.DataItem).CategoryName) %></a></li>
									</ItemTemplate>
									<FooterTemplate></ul></FooterTemplate>
									</asp:Repeater>
									</li>
								</ItemTemplate>
								<FooterTemplate></ul></FooterTemplate>
								</asp:Repeater>
								</li>
							</ItemTemplate>
							<FooterTemplate></ul></FooterTemplate>
							</asp:Repeater>
							</li>
						</ItemTemplate>
						<FooterTemplate></ul></FooterTemplate>
						</asp:Repeater>
						</li>
					</ItemTemplate>
					<FooterTemplate></ul></FooterTemplate>
					</asp:Repeater>
					</li>
				</ItemTemplate>
				<FooterTemplate></ul></FooterTemplate>
				</asp:Repeater>
				</li>
			</ItemTemplate>
			<FooterTemplate></ul></FooterTemplate>
			</asp:Repeater>
			</li>
		</ItemTemplate>
		<FooterTemplate></ul></FooterTemplate>
		</asp:Repeater>
		</li>
	</ItemTemplate>
	<FooterTemplate></ul></FooterTemplate>
	</asp:Repeater>
	</li>
</ItemTemplate>
<FooterTemplate>
</ul>
</div>
</FooterTemplate>
</asp:Repeater>

<div class="categoryList">
	<a href="javascript:void(0);" class="title toggle">ブランドから探す</a>
	<ul>
		<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "DefaultBrandTop.aspx?bid=brand1") %>">brandname1</a></li>
		<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "DefaultBrandTop.aspx?bid=brand2") %>">brandname2</a></li>
	</ul>
</div>

<div class="categoryList">
	<a href="javascript:void(0);" class="title toggle">価格から探す</a>
	<ul>
		<% if (this.CurrentCurrencyCode == "JPY") { %>
		<li><a href="<%: CreatePriceLinkUrl(true, 0m, 3000m) %>">～ 3,000円</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 3000m, 5000m) %>">3,000円 ～ 5,000円</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 5000m, 8000m) %>">5,000円 ～ 8,000円</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 8000m, 10000m) %>">8,000円 ～ 10,000円</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 10000m, 15000m) %>">10,000円 ～ 15,000円</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 15000m, 20000m) %>">15,000円 ～ 20,000円</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 20000m, 30000m) %>">20,000円 ～ 30,000円</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 30000m) %>">30,000円 ～</a></li>
		<%} else if (this.CurrentCurrencyCode == "USD") { %>
		<li><a href="<%: CreatePriceLinkUrl(true, 0m, 30m) %>">～ $30.00</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 30m, 50m) %>">$30.00 ～ $50.00</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 50m, 80m) %>">$50.00 ～ $80.00</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 80m, 100m) %>">$80.00 ～ $100.00</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 100m, 150m) %>">$100.00 ～ $150.00</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 150m, 200m) %>">$150.00 ～ $200.00</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 200m, 300m) %>">$200.00 ～ $300.00</a></li>
		<li><a href="<%: CreatePriceLinkUrl(true, 300m) %>">$300.00 ～</a></li>
		<%} else { %>
		<li><a href="<%: CreatePriceLinkUrl(0, 3000) %>">～ <%: CurrencyManager.ToPrice(3000m) %></a></li>
		<li><a href="<%: CreatePriceLinkUrl(3000, 5000) %>"><%: CurrencyManager.ToPrice(3000m) %> ～ <%: CurrencyManager.ToPrice(5000m) %></a></li>
		<li><a href="<%: CreatePriceLinkUrl(5000, 8000) %>"><%: CurrencyManager.ToPrice(5000m) %> ～ <%: CurrencyManager.ToPrice(8000m) %></a></li>
		<li><a href="<%: CreatePriceLinkUrl(8000, 10000) %>"><%: CurrencyManager.ToPrice(8000m) %> ～ <%: CurrencyManager.ToPrice(10000m) %></a></li>
		<li><a href="<%: CreatePriceLinkUrl(10000, 15000) %>"><%: CurrencyManager.ToPrice(10000m) %> ～ <%: CurrencyManager.ToPrice(15000m) %></a></li>
		<li><a href="<%: CreatePriceLinkUrl(15000, 20000) %>"><%: CurrencyManager.ToPrice(15000m) %> ～ <%: CurrencyManager.ToPrice(20000m) %></a></li>
		<li><a href="<%: CreatePriceLinkUrl(20000, 30000) %>"><%: CurrencyManager.ToPrice(20000m) %> ～ <%: CurrencyManager.ToPrice(30000m) %></a></li>
		<li><a href="<%: CreatePriceLinkUrl(30000) %>"><%: CurrencyManager.ToPrice(30000m) %> ～</a></li>
		<%} %>
	</ul>
</div>
<div>
<%-- △編集可能領域△ --%>