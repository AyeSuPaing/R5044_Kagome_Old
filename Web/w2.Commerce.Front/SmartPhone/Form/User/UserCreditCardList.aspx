<%--
=========================================================================================================
  Module      : スマートフォン用ユーザクレジットカード一覧画面(UserCreditCardList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCreditCardList.aspx.cs" Inherits="Form_User_UserCreditCardList" Title="登録クレジット一覧ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order user-credit-card-list">
<div class="user-unit">
	<h2>登録クレジットカード一覧</h2>
	<div class="msg">
		<%-- メッセージ --%>
		<asp:Literal ID="lDeleteMessage" Runat="server" Text="登録クレジットカードを削除致しました。" Visible="false"></asp:Literal>
		<asp:Literal ID="lErrorMessage" Runat="server"></asp:Literal>
		ご利用になるクレジットカードを登録する事ができます。（最大3件まで）<br />
		尚、一度ご登録頂いた情報の変更はできません。ご変更される場合は、削除後に、再度追加登録してください。<br />
	</div>
	<div class="button">
		<asp:LinkButton id="lbInsert" CssClass="btn" Text="追加する" runat="server" OnClick="lbInsert_Click" />
	</div>

	<asp:Repeater id="rUserCreditCardList" ItemType="w2.App.Common.Order.UserCreditCard" runat="server" OnItemCommand="rUserCreditCardList_ItemCommand">
		<HeaderTemplate>
		<div class="credit-card-list-wrap">
		</HeaderTemplate>
		<ItemTemplate>
		<div class="credit-card-list">
			<dl>
				<dt>登録名</dt>
				<dd><%#: Item.CardDispName %></dd>
				<%if (OrderCommon.CreditCompanySelectable) {%>
				<dt>カード会社</dt>
				<dd><%#: Item.CompanyName %></dd>
				<%} %>
				<dt>カード番号</dt>
				<dd>XXXXXXXXXXXX<%#: Item.LastFourDigit %></dd>
				<dt>有効期限</dt>
				<dd><%#: Item.ExpirationMonth %>/<%# Item.ExpirationYear %> (月/年)</dd>
				<dt>カード名義</dt>
				<dd><%# Item.AuthorName %></dd>
			</dl>
			<div class="button">
				<asp:LinkButton id="lbUpdate" CssClass="btn" runat="server" CommandName="Update" CommandArgument="<%# Item.BranchNo %>" >編集する</asp:LinkButton><br>
				<asp:LinkButton id="lbDelete" CssClass="btn" runat="server" CommandName="Delete" CommandArgument="<%# Item.BranchNo %>" OnClientClick="return confirm('削除しますか？');" >削除する</asp:LinkButton>
			</div>
		</div>
		</ItemTemplate>
		<FooterTemplate>
		</div>
		</FooterTemplate>
	</asp:Repeater>

</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>