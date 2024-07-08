<%--
=========================================================================================================
  Module      : User Invoice Confirmation Screen (UserInvoiceConfirm.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserInvoiceConfirm.aspx.cs" Inherits="Form_User_UserInvoiceConfirm" Title="電子発票確認ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div id="dvUserFltContents">
		<%-- パンくず --%>
		<div id="dvHeaderUserShippingClumbs">
			<p>
				<img src="../../Contents/ImagesPkg/user/clumbs_userinvoice_2.gif" alt="入力内容の確認" />
			</p>
		</div>
		<h2>入力内容の確認</h2>
		<div id="dvUserShippingInput" class="unit">
			<div class="dvContentsInfo">
				<p>登録する電子発票に間違いがなければ、「<%: (lbRegist.Visible == true) ? "登録" : "更新"%>する」ボタンを押してください。</p>
			</div>
			<div class="dvUserShippingInfo">
				<h3>電子発票管理情報</h3>
				<table cellspacing="0">
					<tr>
						<th style="width: 190px">電子発票情報名</th>
						<td><%: this.TwUserInvoice.TwInvoiceName %></td>
					</tr>
					<tr>
						<th>發票種類</th>
						<td>
							<%: ValueText.GetValueText(
							Constants.TABLE_TWUSERINVOICE,
							Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE,
							this.TwUserInvoice.TwUniformInvoice) %>
						</td>
					</tr>
					<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) { %>
					<tr>
						<th>共通性載具</th>
						<td>
							<%: ValueText.GetValueText(
							Constants.TABLE_TWUSERINVOICE,
							Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE,
							this.TwUserInvoice.TwCarryType) %>
							<br />
							<%: string.IsNullOrEmpty(this.TwUserInvoice.TwCarryTypeOption) ? string.Empty : this.TwUserInvoice.TwCarryTypeOption %>
						</td>
					</tr>
					<% } %>
					<% if (string.IsNullOrEmpty(this.TwUserInvoice.TwUniformInvoiceOption1) == false) { %>
					<tr>
						<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) { %>
						<th>統一編号</th>
						<% } %>
						<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE) { %>
						<th>寄付先コード</th>
						<% } %>
						<td><%: this.TwUserInvoice.TwUniformInvoiceOption1 %></td>
					</tr>
					<% } %>
					<% if (string.IsNullOrEmpty(this.TwUserInvoice.TwUniformInvoiceOption2) == false) { %>
					<tr>
						<th>会社名</th>
						<td><%: this.TwUserInvoice.TwUniformInvoiceOption2 %></td>
					</tr>
					<% } %>
				</table>
			</div>
			<div class="dvUserBtnBox">
				<p>
					<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn btn-large">戻る</asp:LinkButton>
					<asp:LinkButton ID="lbRegist" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">登録する</asp:LinkButton>
					<asp:LinkButton ID="lbModify" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">更新する</asp:LinkButton>
				</p>
			</div>
		</div>
	</div>
</asp:Content>
