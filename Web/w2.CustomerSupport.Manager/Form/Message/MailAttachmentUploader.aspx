<%--
=========================================================================================================
  Module      : メール添付アップロードページ(MailAttachmentUploader.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailAttachmentUploader.aspx.cs" Inherits="Form_Message_MailAttachmentUploader" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<br />
	<table class="box_border" cellspacing="1" cellpadding="0" width="500" border="0">
	<tr>
		<td align="center">
			<table class="list_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
				<tr>
					<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
					<td align="center">
						<br />
						<asp:FileUpload id="fuAttachment" Width="480" runat="server" /><br /><br />
						<asp:Button ID="btnRegisterAttachment" Text="  登録する  " runat="server" OnClick="btnRegisterAttachment_Click" />
					</td>
				</tr>
				<tr>
					<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
			</table>
		</td>
	</tr>
	</table>

	<br />

	<asp:HiddenField id="hfAttachmentFileNo" runat="server" />
	<asp:HiddenField id="hfAttachmentFileName" runat="server" />

	<div id="divUpload" visible="false" runat="server">
	<script type="text/javascript">
	<!--
		if (window.opener && (window.opener.closed == false) && window.opener.set_upload_file) {
			window.opener.set_upload_file(
				document.getElementById('<%= hfAttachmentFileName.ClientID %>').value,
				document.getElementById('<%= hfAttachmentFileNo.ClientID %>').value);
			window.close();
		}
		//-->
	</script>
	</div>
</asp:Content>
