<%--
=========================================================================================================
  Module      : アドレス帳一覧画面(UserShippingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserShippingList.aspx.cs" Inherits="Form_User_UserShippingList" Title="アドレス帳一覧ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">

		<h2>アドレス帳一覧</h2>

	<div id="dvUserShippingList" class="unit">
		<%-- メッセージ --%>
		<strong>
			<span>
				<% if (this.IsDelete){%>
					お届け先情報を削除致しました。
				<%} %>
			</span>
		</strong>
		<h4>よくご利用になるお届け先を登録する事ができます。</h4>
		<table cellspacing="0" style="border-top-style:none">
			<tr>
				<td class="insert">
					<asp:LinkButton id="lbInsert" runat="server" OnClick="lbInsert_Click" class="btn btn-large">アドレス帳の追加</asp:LinkButton>
				</td>
			</tr>
		</table>
		<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
		<asp:Repeater id="rUserShippingList" runat="server" ItemType="w2.Domain.UserShipping.UserShippingModel" OnItemCommand="rUserShippingList_ItemCommand">
			<HeaderTemplate>
				<table cellspacing="0">
					<tr>
						<th class="productPatternNum">配送先名</th>
						<th class="productName">お届け先</th>
						<th class="updatedelete">&nbsp;&nbsp;</th>
					</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="shippingName">
						<%# WebSanitizer.HtmlEncode(Item.Name) %>
					</td>
					<td class="shippingAddr">
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
					</td>
					<td class="updatedelete">
						<asp:LinkButton id="lbUpdate" runat="server" CommandName="Update" CommandArgument="<%# Item.ShippingNo %>" class="btn btn-mini" Visible="<%# Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>">編集する</asp:LinkButton>
						<asp:LinkButton id="lbDelete" runat="server" CommandName="Delete" CommandArgument="<%# Item.ShippingNo %>" OnClientClick="return confirm('削除しますか？');" class="btn btn-mini">削除する</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>

		<%-- エラーメッセージ --%>
		<% if(StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
			<%: this.ErrorMessage %>
		<% } %>

		<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
	</div>
</div>
</asp:Content>