<%--
=========================================================================================================
  Module      : User Invoice List Screen (UserInvoiceList.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserInvoiceList.aspx.cs" Inherits="Form_User_UserInvoiceList" Title="電子発票管理ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<style type="text/css">
		.updateDelete
		{
			width: 120px;
		}
		.updateDelete a
		{
			margin-bottom: 3px;
		}
	</style>
	<div id="dvUserFltContents">
		<h2>電子発票管理</h2>
		<div id="dvUserShippingList" class="unit">
			<%-- メッセージ --%>
			<strong>
				<span>
					<% if (this.IsDelete) { %>
					お電子発票オプション情報を削除致しました。
					<% } %>
				</span>
			</strong>
			<h4>よくご利用になるお電子発票オプションを登録する事ができます。</h4>
			<table cellspacing="0" style="border-top-style: none">
				<tr>
					<td class="insert">
						<asp:LinkButton ID="lbInsert" runat="server" OnClick="lbInsert_Click" class="btn btn-large">電子発票オプションの追加</asp:LinkButton>
					</td>
				</tr>
			</table>
			<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
			<asp:Repeater ID="rUserInvoiceList" runat="server" ItemType="w2.Domain.TwUserInvoice.TwUserInvoiceModel" OnItemCommand="rUserInvoiceList_ItemCommand">
				<HeaderTemplate>
					<table cellspacing="0">
						<tr>
							<th class="twInvoiceName">電子発票情報名</th>
							<th class="twUniformInvoice">電子発票種別</th>
							<th class="twCarryType">コード</th>
							<th class="updateDelete">&nbsp;&nbsp;</th>
						</tr>
				</HeaderTemplate>
				<ItemTemplate>
						<tr>
							<td class="twInvoiceName">
								<%#: Item.TwInvoiceName %>
							</td>
							<td class="twUniformInvoice">
								<%#: ValueText.GetValueText(
									Constants.TABLE_TWUSERINVOICE,
									Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE,
									Item.TwUniformInvoice) %>
							</td>
							<td class="twCarryType">
								<%# GetDisplayCode(Item) %>
							</td>
							<td class="updateDelete">
								<asp:LinkButton ID="lbUpdate" runat="server" CommandName="Update" CommandArgument="<%# Item.TwInvoiceNo %>" class="btn btn-mini">編集する</asp:LinkButton>
								<asp:LinkButton ID="lbDelete" runat="server" CommandName="Delete" CommandArgument="<%# Item.TwInvoiceNo %>" OnClientClick="return confirm('削除しますか？');" class="btn btn-mini">削除する</asp:LinkButton>
							</td>
						</tr>
				</ItemTemplate>
				<FooterTemplate>
					</table>
				</FooterTemplate>
			</asp:Repeater>
			<%-- エラーメッセージ --%>
			<% if (StringUtility.ToEmpty(this.ErrorMessage) != string.Empty) { %>
			<%: this.ErrorMessage %>
			<% } %>
			<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
		</div>
	</div>
</asp:Content>
