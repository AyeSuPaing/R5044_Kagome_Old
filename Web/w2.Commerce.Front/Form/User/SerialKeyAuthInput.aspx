<%--
=========================================================================================================
  Module      : シリアルキー認証画面(SerialKeyAuthInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/SerialKeyAuthInput.aspx.cs" Inherits="Form_User_SerialKeyAuthInput" Title="シリアルキー認証ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<div id="dvSerialKeyAuth">

		<h2>シリアルキー認証</h2>
		<p>
			デジタルコンテンツをダウンロードされる方は、注文番号とキーを入力の上、「送信する」ボタンをクリックして下さい。
			<br /><br /><ins><span class="necessary">*</span>は必須入力となります。</ins>
		</p>

		<div>
			<table cellspacing="0">
				<tr>
					<th>注文番号<span class="necessary">*</span></th>
					<td><asp:TextBox id="tbOrderId" Runat="server"></asp:TextBox>
					<asp:CustomValidator ControlToValidate="tbOrderId" ValidationGroup="SerialKeyAuthInput" runat="Server"
						ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>商品ID<span class="necessary">*</span></th>
					<td><asp:TextBox id="tbProductId" Runat="server"></asp:TextBox>
					<asp:CustomValidator ControlToValidate="tbProductId" ValidationGroup="SerialKeyAuthInput" runat="Server"
						ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>シリアルキー<span class="necessary">*</span></th>
					<td>
						<asp:TextBox id="tbSerialKey01" Runat="server" MaxLength="60"></asp:TextBox>
						<asp:CustomValidator ControlToValidate="tbSerialKey01" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<!--
						<asp:TextBox id="tbSerialKey02" Runat="server" CssClass="nameFirst"></asp:TextBox> - 
						<asp:TextBox id="tbSerialKey03" Runat="server" CssClass="nameFirst"></asp:TextBox><br />
						<asp:CustomValidator ControlToValidate="tbSerialKey02" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:CustomValidator ControlToValidate="tbSerialKey03" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:TextBox id="tbSerialKey04" Runat="server" CssClass="nameFirst"></asp:TextBox> - 
						<asp:TextBox id="tbSerialKey05" Runat="server" CssClass="nameFirst"></asp:TextBox> - 
						<asp:TextBox id="tbSerialKey06" Runat="server" CssClass="nameFirst"></asp:TextBox><br />
						<asp:CustomValidator ControlToValidate="tbSerialKey04" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:CustomValidator ControlToValidate="tbSerialKey05" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:CustomValidator ControlToValidate="tbSerialKey06" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:TextBox id="tbSerialKey07" Runat="server" CssClass="nameFirst"></asp:TextBox> - 
						<asp:TextBox id="tbSerialKey08" Runat="server" CssClass="nameFirst"></asp:TextBox> - 
						<asp:TextBox id="tbSerialKey09" Runat="server" CssClass="nameFirst"></asp:TextBox><br />
						<asp:CustomValidator ControlToValidate="tbSerialKey07" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:CustomValidator ControlToValidate="tbSerialKey08" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:CustomValidator ControlToValidate="tbSerialKey09" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:TextBox id="tbSerialKey10" Runat="server" CssClass="nameFirst"></asp:TextBox> - 
						<asp:TextBox id="tbSerialKey11" Runat="server" CssClass="nameFirst"></asp:TextBox> - 
						<asp:TextBox id="tbSerialKey12" Runat="server" CssClass="nameFirst"></asp:TextBox><br />
						<asp:CustomValidator ControlToValidate="tbSerialKey10" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:CustomValidator ControlToValidate="tbSerialKey11" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						<asp:CustomValidator ControlToValidate="tbSerialKey12" ValidationGroup="SerialKeyAuthInput" runat="Server"
							ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
						-->
					</td>
				</tr>
			</table>
		</div>

		<p style="color: red; font-weight:bold;"><br />※シリアルキー入力欄は最大12マスまで対応。顧客毎にデザイン埋め込みで調整します。</p>

		<div class="dvUserBtnBox">
			<p>
				<span><a href="javascript:history.back();">
					<img src="../../Contents/ImagesPkg/user/btn_cancel.gif" alt="キャンセル"></a></span>
				<span><asp:LinkButton OnClientClick="return exec_submit()" runat="server" OnClick="lbSend_Click">
					<img src="../../Contents/ImagesPkg/user/btn_sending.gif" alt="送信" /></asp:LinkButton></span>
			</p>
		</div>

	</div>
</div>
</asp:Content>
