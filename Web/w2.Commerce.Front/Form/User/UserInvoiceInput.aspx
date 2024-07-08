<%--
=========================================================================================================
  Module      : User Invoice Input Screen (UserInvoiceInput.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserInvoiceInput.aspx.cs" Inherits="Form_User_UserInvoiceInput" Title="電子発票管理入力ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div id="dvUserFltContents">
		<%-- メッセージ --%>
		<div id="dvHeaderUserShippingClumbs">
			<p>
				<img src="../../Contents/ImagesPkg/user/clumbs_userinvoice_1.gif" alt="アドレス帳の入力" />
			</p>
		</div>
		<h2>電子発票管理情報の入力</h2>
		<div id="dvUserShippingInput" class="unit">
			<div class="dvContentsInfo">
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
			</div>
			<div class="dvUserShippingInfo">
				<h3>電子発票管理情報</h3>
				<ins><span class="necessary">*</span>は必須入力となります。</ins>
				<%-- UPDATE PANEL開始 --%>
				<asp:UpdatePanel ID="upUpdatePanel" runat="server">
					<ContentTemplate>
						<table cellspacing="0">
							<tr>
								<th style="width: 190px">電子発票情報名<span class="necessary">*</span></th>
								<td>
									<asp:TextBox ID="tbInvoiceName" runat="server" MaxLength="30"></asp:TextBox>
									<asp:CustomValidator
										ID="cvTwInvoiceName"
										runat="Server"
										ControlToValidate="tbInvoiceName"
										ValidationGroup="TwUserInvoice"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<tr>
								<th>發票種類<span class="necessary">*</span></th>
								<td>
									<asp:DropDownList ID="ddlTwUniformInvoice" runat="server"
										DataTextField="Text"
										DataValueField="Value"
										Width="150px"
										OnSelectedIndexChanged="ddlTwUniformInvoice_SelectedIndexChanged"
										AutoPostBack="true">
									</asp:DropDownList>
									<asp:CustomValidator
										ID="cvTwUniformInvoice"
										runat="Server"
										ControlToValidate="ddlTwUniformInvoice"
										ValidationGroup="TwUserInvoice"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<tr id="trCarryType" runat="server" visible="false">
								<th>共通性載具<span class="necessary">*</span></th>
								<td>
									<asp:DropDownList ID="ddlTwCarryType" runat="server"
										DataTextField="Text"
										DataValueField="Value"
										Width="150px"
										OnSelectedIndexChanged="ddlTwCarryType_SelectedIndexChanged"
										AutoPostBack="true">
									</asp:DropDownList>
									<asp:CustomValidator
										ID="cvTwCarryType"
										runat="Server"
										ControlToValidate="ddlTwCarryType"
										ValidationGroup="TwUserInvoice"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
									<div id="divCarryTypeOption" runat="server" visible="false">
										<br />
										<div id="divTwCarryTypeOption_8" runat="server" visible="false">
											<asp:TextBox ID="tbCarryTypeOption_8" Width="220" placeholder="例:/AB201+9(限8個字)" runat="server" MaxLength="8"></asp:TextBox>
											<asp:CustomValidator
												ID="cvTwCarryTypeOption_8"
												runat="Server"
												ControlToValidate="tbCarryTypeOption_8"
												ValidationGroup="TwUserInvoice"
												ValidateEmptyText="true"
												SetFocusOnError="true"
												ClientValidationFunction="ClientValidate"
												CssClass="error_inline" />
										</div>
										<div id="divTwCarryTypeOption_16" runat="server" visible="false">
											<asp:TextBox ID="tbCarryTypeOption_16" Width="220" placeholder="例:TP03000001234567(限16個字)" runat="server" MaxLength="16"></asp:TextBox>
											<asp:CustomValidator
												ID="cvTwCarryTypeOption_16"
												runat="Server"
												ControlToValidate="tbCarryTypeOption_16"
												ValidationGroup="TwUserInvoice"
												ValidateEmptyText="true"
												SetFocusOnError="true"
												ClientValidationFunction="ClientValidate"
												CssClass="error_inline" />
										</div>
									</div>
								</td>
							</tr>
							<tr id="trUniformInvoiceOption1" runat="server" visible="false">
								<th id="companyTittle" runat="server" visible="false">統一編号<span class="necessary">*</span></th>
								<th id="donateTittle" runat="server" visible="false">寄付先コード<span class="necessary">*</span></th>
								<td>
									<asp:TextBox ID="tbUniformInvoiceOption1_3" runat="server" MaxLength="7" Visible="false"></asp:TextBox>
									<asp:CustomValidator
										ID="cvTw_uniform_invoice_option1_3"
										runat="Server"
										ControlToValidate="tbUniformInvoiceOption1_3"
										ValidationGroup="TwUserInvoice"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
									<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" runat="server" MaxLength="8" Visible="false"></asp:TextBox>
									<asp:CustomValidator
										ID="cvTw_uniform_invoice_option1_8"
										runat="Server"
										ControlToValidate="tbUniformInvoiceOption1_8"
										ValidationGroup="TwUserInvoice"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<tr id="trUniformInvoiceOption2" runat="server" visible="false">
								<th>会社名<span class="necessary">*</span></th>
								<td>
									<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" runat="server" MaxLength="20"></asp:TextBox>
									<asp:CustomValidator
										ID="cvTw_uniform_invoice_option2"
										runat="Server"
										ControlToValidate="tbUniformInvoiceOption2"
										ValidationGroup="TwUserInvoice"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
						</table>
					</ContentTemplate>
				</asp:UpdatePanel>
				<%-- UPDATE PANELここまで --%>
			</div>
			<div class="dvUserBtnBox">
				<p>
					<span><a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST %>" class="btn btn-large">戻る</a></span>
					<span>
						<asp:LinkButton ID="lbConfirm" ValidationGroup="UserInvoiceRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">確認する</asp:LinkButton>
					</span>
				</p>
			</div>
		</div>
	</div>
</asp:Content>
