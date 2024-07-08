<%--
=========================================================================================================
  Module      : スマートフォン用アドレス帳一覧画面(UserShippingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserShippingList.aspx.cs" Inherits="Form_User_UserShippingList" Title="アドレス帳一覧ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-shipping-list">
<div class="user-unit">
	<h2>アドレス帳一覧</h2>
	<%-- メッセージ --%>
	<div class="msg">
		<% if (this.IsDelete){%>
			<p class="attention">お届け先情報を削除致しました。</p>
		<%} %>
		<p>よくご利用になるお届け先を登録する事ができます。</p>

		<%-- エラーメッセージ --%>
		<% if(StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
			<p><%: this.ErrorMessage %></p>
		<% } %>
	</div>

	<div class="button">
		<asp:LinkButton id="lbInsert" CssClass="btn" Text="追加する" runat="server" OnClick="lbInsert_Click" />
	</div>

	<div class="pager-wrap above"><%= this.PagerHtml %></div>

	<asp:Repeater id="rUserShippingList" ItemType="w2.Domain.UserShipping.UserShippingModel" runat="server" OnItemCommand="rUserShippingList_ItemCommand">
		<HeaderTemplate>
			<div class="shipping-list">
		</HeaderTemplate>
		<ItemTemplate>
			<ul>
			<li class="name"><%# WebSanitizer.HtmlEncode(Item.Name) %></li>
			<li>
				<div runat="server" Visible="<%# IsCountryJp(Item.ShippingCountryIsoCode) %>">
					<%# "〒" + WebSanitizer.HtmlEncode(Item.ShippingZip) %><br />
					<%#: Item.ShippingAddr1 %>
					<%#: Item.ShippingAddr2 %><br />
					<%#: Item.ShippingAddr3 %><br />
					<%# (string.IsNullOrEmpty(Item.ShippingAddr4) == false) ? WebSanitizer.HtmlEncode(Item.ShippingAddr4) + "<br />" : string.Empty %>
					<%# (string.IsNullOrEmpty(Item.ShippingAddr5) == false) ? WebSanitizer.HtmlEncode(Item.ShippingAddr5) + "<br />" : string.Empty %>
					<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false) ? WebSanitizer.HtmlEncode(Item.ShippingZip) + "<br />" : string.Empty %>
					<%#: Item.ShippingCountryName %><br />
					<%#: Item.ShippingName %>&nbsp;様
					<%#: "（" + Item.ShippingNameKana + " さま）" %>
				</div>
				<div runat="server" Visible="<%# IsCountryTw(Item.ShippingCountryIsoCode) %>">
					<%#: Item.ShippingName %>&nbsp;様<br />
					<div runat="server" visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>">
						<%#: Item.ShippingAddr2 %><br />
						<%#: Item.ShippingAddr3 %><br />
						<%#: Item.ShippingAddr4 %>
						<%# (string.IsNullOrEmpty(Item.ShippingAddr5) == false) ? WebSanitizer.HtmlEncode(Item.ShippingAddr5) + "<br />" : string.Empty %>
						<%#: Item.ShippingZip %><br />
					</div>
					<div runat="server" visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON %>">
						<%# (string.IsNullOrEmpty(Item.ShippingAddr4) == false) ? WebSanitizer.HtmlEncode(Item.ShippingAddr4) + "<br />" : string.Empty %>
					</div>
					<%#: Item.ShippingCountryName %><br />
				</div>
				<div runat="server" Visible="<%# ((IsCountryJp(Item.ShippingCountryIsoCode) == false) && (IsCountryTw(Item.ShippingCountryIsoCode) == false)) %>">
					<%#: Item.ShippingName %>&nbsp;様<br />
					<%#: Item.ShippingAddr2 %><br />
					<%# (string.IsNullOrEmpty(Item.ShippingAddr3) == false) ? WebSanitizer.HtmlEncode(Item.ShippingAddr3) + "<br />" : string.Empty %>
					<%#: Item.ShippingAddr4 %>,
					<%#: Item.ShippingAddr5 %>&nbsp;
					<%#: Item.ShippingZip %><br />
					<%#: Item.ShippingCountryName %><br />
				</div>
			</li>
			<li class="control">
				<asp:LinkButton id="lbUpdate" CssClass="btn" runat="server" CommandName="Update" CommandArgument="<%# Item.ShippingNo %>" Visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>">編集する</asp:LinkButton>
				<asp:LinkButton id="lbDelete" CssClass="btn" runat="server" CommandName="Delete" CommandArgument="<%# Item.ShippingNo %>" OnClientClick="return confirm('削除しますか？');" >削除する</asp:LinkButton>
			</li>
			</ul>
		</ItemTemplate>
		<FooterTemplate>
			</div>
		</FooterTemplate>
	</asp:Repeater>

	<div class="pager-wrap below"><%= this.PagerHtml %></div>

</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>