<%--
=========================================================================================================
  Module      : ユーザクレジットカード一覧画面(UserCreditCardList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCreditCardList.aspx.cs" Inherits="Form_User_UserCreditCardList" Title="登録クレジット一覧ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
		<h2>登録クレジットカード一覧</h2>
	<div id="dvUserCreditCardList" class="unit">
		<%-- メッセージ --%>
		<strong>
			<span><asp:Literal ID="lDeleteMessage" Runat="server" Text="登録クレジットカードを削除致しました。" Visible="false"></asp:Literal></span>
		</strong>
		<h4>ご利用になるクレジットカードを登録する事ができます。</h4>
		<table cellspacing="0" style="border-top-style:none">
			<tr>
				<td class="insert">
					<asp:LinkButton id="lbInsert" runat="server" OnClick="lbInsert_Click" class="btn btn-large">クレジットカードの追加</asp:LinkButton>
				</td>
			</tr>
		</table>
		<asp:Repeater id="rUserCreditCardList" ItemType="w2.App.Common.Order.UserCreditCard" runat="server" OnItemCommand="rUserCreditCardList_ItemCommand">
			<HeaderTemplate>
				<table cellspacing="0">
					<tr>
						<th class="productPatternNum">クレジットカード登録名</th>
						<th class="productName">登録カード詳細</th>
						<th class="delete">&nbsp;&nbsp;</th>
					</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="CreditcardName">
						<%#: Item.CardDispName %>
					</td>
					<td class="CreditCardDetail">
						<%if (OrderCommon.CreditCompanySelectable) {%>
						<ul>
							<li class="itemname">カード会社</li>
							<li class="separator">：</li>
							<li class="iteminfo"><%#: Item.CompanyName %></li>
						</ul>
						<%} %>
						<ul>
							<li class="itemname">カード番号</li>
							<li class="separator">：</li>
							<% if (OrderCommon.IsUserPayTg) { %>
								<li class="iteminfo">XXXXXXXXXXXX<%#: Item.LastFourDigit.Replace('*','X') %></li>
							<% } else { %>
								<li class="iteminfo">XXXXXXXXXXXX<%#: Item.LastFourDigit %></li>
							<% } %>
						</ul>
						<ul>
							<li class="itemname">有効期限</li>
							<li class="separator">：</li>
							<li class="iteminfo"><%#: Item.ExpirationMonth %>/<%#: Item.ExpirationYear %> (月/年)</li>
						</ul>
						<ul>
							<li class="itemname">カード名義</li>
							<li class="separator">：</li>
							<li class="iteminfo"><%#: Item.AuthorName %></li>
						</ul>
					</td>
					<td class="updatedelete">
						<asp:LinkButton id="lbUpdate" runat="server" CommandName="Update" CommandArgument="<%# Item.BranchNo %>" class="btn btn-mini">編集する</asp:LinkButton><br>
						<asp:LinkButton id="lbDelete" runat="server" CommandName="Delete" CommandArgument="<%# Item.BranchNo %>" OnClientClick="return confirm('削除しますか？');" class="btn btn-mini">削除する</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>
		<asp:Literal ID="lErrorMessage" Runat="server"></asp:Literal>
	</div>
</div>
</asp:Content>