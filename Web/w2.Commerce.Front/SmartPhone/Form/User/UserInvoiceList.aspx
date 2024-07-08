<%--
=========================================================================================================
  Module      : Smartphone User Invoice List Screen (UserInvoiceList.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserInvoiceList.aspx.cs" Inherits="Form_User_UserInvoiceList" Title="電子発票管理ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<section class="wrap-user user-shipping-list">
		<div class="user-unit">
			<h2>電子発票管理</h2>
			<%-- メッセージ --%>
			<div class="msg">
				<% if (this.IsDelete) { %>
					<p class="attention">お電子発票オプション情報を削除致しました。</p>
				<% } %>
				<p>よくご利用になるお電子発票オプションを登録する事ができます。</p>
				<%-- エラーメッセージ --%>
				<% if (StringUtility.ToEmpty(this.ErrorMessage) != string.Empty) { %>
					<p><%: this.ErrorMessage %></p>
				<% } %>
			</div>
			<div class="button">
				<asp:LinkButton id="lbInsert" CssClass="btn" runat="server" OnClick="lbInsert_Click">電子発票オプションの追加</asp:LinkButton>
			</div>
			<div class="pager-wrap above"><%= this.PagerHtml %></div>
			<asp:Repeater id="rUserInvoiceList" ItemType="w2.Domain.TwUserInvoice.TwUserInvoiceModel" runat="server" OnItemCommand="rUserInvoiceList_ItemCommand">
				<HeaderTemplate>
					<div class="shipping-list">
				</HeaderTemplate>
				<ItemTemplate>
						<ul>
							<li class="twInvoiceName"><%#: Item.TwInvoiceName %></li>
							<li class="twUniformInvoice">
								<%#: ValueText.GetValueText(
									Constants.TABLE_TWUSERINVOICE,
									Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE,
									Item.TwUniformInvoice) %>
							</li>
							<li class="twCarryType">
								<%# GetDisplayCode(Item) %>
							</li>
							<li class="control">
								<asp:LinkButton id="lbUpdate" CssClass="btn" runat="server" CommandName="Update" CommandArgument="<%# Item.TwInvoiceNo %>">編集する</asp:LinkButton>
								<asp:LinkButton id="lbDelete" CssClass="btn" runat="server" CommandName="Delete" CommandArgument="<%# Item.TwInvoiceNo %>" OnClientClick="return confirm('削除しますか？');" >削除する</asp:LinkButton>
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
				<a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="btn">マイページトップへ</a>
			</div>
		</div>
	</section>
</asp:Content>
