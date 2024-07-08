<%--
=========================================================================================================
  Module      : Smartphone User Invoice Confirmation Screen (UserInvoiceConfirm.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserInvoiceConfirm.aspx.cs" Inherits="Form_User_UserInvoiceConfirm" Title="電子発票確認ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<section class="wrap-user user-shipping-list">
		<div class="user-unit">
			<h2>入力内容の確認</h2>
			<div class="msg">
				<p>登録する住所に間違いがなければ、「登録する」ボタンを押してください。</p>
			</div>
			<dl class="user-form">
				<dt>電子発票情報名</dt>
				<dd>
					<%: this.TwUserInvoice.TwInvoiceName %>
				</dd>
				<dt>發票種類</dt>
				<dd>
					<%: ValueText.GetValueText(
						Constants.TABLE_TWUSERINVOICE,
						Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE,
						this.TwUserInvoice.TwUniformInvoice) %>
				</dd>
				<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) { %>
				<dt>共通性載具</dt>
				<dd>
					<%: ValueText.GetValueText(
						Constants.TABLE_TWUSERINVOICE,
						Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE,
						this.TwUserInvoice.TwCarryType) %>
					<br />
					<%: string.IsNullOrEmpty(this.TwUserInvoice.TwCarryTypeOption) ? string.Empty : this.TwUserInvoice.TwCarryTypeOption %>
				</dd>
				<% } %>
				<% if (string.IsNullOrEmpty(this.TwUserInvoice.TwUniformInvoiceOption1) == false) { %>
					<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) { %>
					<dt>統一編号</dt>
					<% } %>
					<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE) { %>
					<dt>寄付先コード</dt>
					<% } %>
					<dd><%: this.TwUserInvoice.TwUniformInvoiceOption1 %></dd>
				<% } %>
				<% if (string.IsNullOrEmpty(this.TwUserInvoice.TwUniformInvoiceOption2) == false) { %>
				<dt>会社名</dt>
				<dd><%: this.TwUserInvoice.TwUniformInvoiceOption2 %></dd>
				<% } %>
			</dl>
		</div>
		<div class="user-footer">
			<div class="button-next">
				<asp:LinkButton ID="lbRegist" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn">登録する</asp:LinkButton>
				<asp:LinkButton ID="lbModify" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn">更新する</asp:LinkButton>
			</div>
			<div class="button-prev">
				<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn">戻る</asp:LinkButton>
			</div>
		</div>
	</section>
</asp:Content>
