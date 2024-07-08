<%--
=========================================================================================================
  Module      : Smartphone User Invoice Input Screen (UserInvoiceInput.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserInvoiceInput.aspx.cs" Inherits="Form_User_UserInvoiceInput" Title="電子発票管理入力ページ" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<section class="wrap-user user-shipping-list">
		<div class="user-unit">
			<h2>電子発票管理情報の入力</h2>
			<div class="msg">
				<p id="pRegistInfo" runat="server" visible="false">
					電子発票管理に新しい電子発票情報を登録します。<br />
					下のフォームに入力し、「確認する」ボタンを押してください。
					電子発票情報名には、「任意な名称」を登録する事ができます。
					(例:「会社用」、「自分ショッピング用」など)
				</p>
				<p id="pModifyInfo" runat="server" visible="false">
					電子発票に登録されているお電子発票を編集します。<br />
					下のフォームに入力し、「確認する」ボタンを押してください。
				</p>
				<p class="attention">※は必須入力となります。</p>
			</div>
			<dl class="user-form">
				<dt>電子発票情報名<span class="require">※</span></dt>
				<dd class="name">
					<w2c:ExtendedTextBox id="tbInvoiceName" Runat="server" maxlength="30"></w2c:ExtendedTextBox>
					<p class="attention">
						<asp:CustomValidator
							ID="cvTwInvoiceName"
							runat="Server"
							ControlToValidate="tbInvoiceName"
							ValidationGroup="TwUserInvoice"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
					</p>
				</dd>
				<dt>發票種類<span class="require">※</span></dt>
				<dd>
					<asp:DropDownList runat="server" ID="ddlTwUniformInvoice" AutoPostBack="true" Width="150px" OnSelectedIndexChanged="ddlTwUniformInvoice_SelectedIndexChanged"></asp:DropDownList>
					<p class="attention">
						<asp:CustomValidator
							ID="cvTwUniformInvoice"
							runat="Server"
							ControlToValidate="ddlTwUniformInvoice"
							ValidationGroup="TwUserInvoice"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
					</p>
				</dd>
			</dl>
			<dl class="user-form" id="trCarryType" runat="server" visible="false">
				<dt>共通性載具<span class="require">※</span></dt>
				<dd>
					<asp:DropDownList runat="server" ID="ddlTwCarryType" AutoPostBack="true" Width="150px" OnSelectedIndexChanged="ddlTwCarryType_SelectedIndexChanged"></asp:DropDownList>
					<p class="attention">
						<asp:CustomValidator
							ID="cvTwCarryType"
							runat="Server"
							ControlToValidate="ddlTwCarryType"
							ValidationGroup="TwUserInvoice"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
					</p>
					<div id="divCarryTypeOption" runat="server" visible="false">
						<br />
						<div id="divTwCarryTypeOption_8" runat="server" visible="false">
							<w2c:ExtendedTextBox id="tbCarryTypeOption_8" Width="220" placeholder="例:/AB201+9(限8個字)" Runat="server" maxlength="8"></w2c:ExtendedTextBox>
							<p class="attention">
								<asp:CustomValidator
									ID="cvTwCarryTypeOption_8"
									runat="Server"
									ControlToValidate="tbCarryTypeOption_8"
									ValidationGroup="TwUserInvoice"
									ValidateEmptyText="true"
									SetFocusOnError="true" />
							</p>
						</div>
						<div id="divTwCarryTypeOption_16" runat="server" visible="false">
							<w2c:ExtendedTextBox id="tbCarryTypeOption_16" Width="220" placeholder="例:TP03000001234567(限16個字)" Runat="server" maxlength="16"></w2c:ExtendedTextBox>
							<p class="attention">
								<asp:CustomValidator
									ID="cvTwCarryTypeOption_16"
									runat="Server"
									ControlToValidate="tbCarryTypeOption_16"
									ValidationGroup="TwUserInvoice"
									ValidateEmptyText="true"
									SetFocusOnError="true" />
							</p>
						</div>
					</div>
				</dd>
			</dl>
			<dl class="user-form" id="trUniformInvoiceOption1" runat="server" visible="false">
				<dt id="companyTittle" visible="false" runat="server">統一編号<span class="require">※</span></dt>
				<dt id="donateTittle" visible="false" runat="server">寄付先コード<span class="require">※</span></dt>
				<dd>
					<w2c:ExtendedTextBox id="tbUniformInvoiceOption1_3" Runat="server" maxlength="7" Visible="false"></w2c:ExtendedTextBox>
					<p class="attention">
						<asp:CustomValidator
							ID="cvTw_uniform_invoice_option1_3"
							runat="Server"
							ControlToValidate="tbUniformInvoiceOption1_3"
							ValidationGroup="TwUserInvoice"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
					</p>
					<w2c:ExtendedTextBox id="tbUniformInvoiceOption1_8" placeholder="例:12345678" Runat="server" maxlength="8" Visible="false"></w2c:ExtendedTextBox>
					<p class="attention">
						<asp:CustomValidator
							ID="cvTw_uniform_invoice_option1_8"
							runat="Server"
							ControlToValidate="tbUniformInvoiceOption1_8"
							ValidationGroup="TwUserInvoice"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
					</p>
				</dd>
			</dl>
			<dl class="user-form" id="trUniformInvoiceOption2" runat="server" visible="false">
				<dt>会社名<span class="require">※</span></dt>
				<dd>
					<w2c:ExtendedTextBox id="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Runat="server" maxlength="20"></w2c:ExtendedTextBox>
					<p class="attention">
						<asp:CustomValidator
							ID="cvTw_uniform_invoice_option2"
							runat="Server"
							ControlToValidate="tbUniformInvoiceOption2"
							ValidationGroup="TwUserInvoice"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
					</p>
				</dd>
			</dl>
		</div>
		<div class="user-footer">
			<div class="button-next">
				<asp:LinkButton ID="lbConfirm" ValidationGroup="TwUserInvoice" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" CssClass="btn">確認する</asp:LinkButton>
			</div>
			<div class="button-prev">
				<a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST %>" class="btn">戻る</a>
			</div>
		</div>
	</section>
</asp:Content>
