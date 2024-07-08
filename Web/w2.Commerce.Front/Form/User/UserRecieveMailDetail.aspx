<%--
=========================================================================================================
  Module      : 受信メール詳細画面(UserRecieveMailDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRecieveMailDetail.aspx.cs" Inherits="Form_User_UserRecieveMailDetail" Title="受信メール履歴" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<h2>受信メール詳細</h2>
	<div class="unit" id="nostyle">
		<table border="1" cellspacing="1" cellpadding="3" width="100%">
			<tr>
				<th colspan="2" style="text-align:center">内容</th>
			</tr>
			<tr>
				<th>受信日時</th>
				<td><asp:Literal ID="lDateSendMail" runat="server" /></td>
			</tr>
			<tr>
				<th>送信元</th>
				<td><asp:Literal ID="lMailFrom" runat="server" /></td>
			</tr>
			<tr>
				<th>送信先</th>
				<td><asp:Literal ID="lMailTo" runat="server" /></td>
			</tr>
			<tr>
				<th style="width:80px">メール件名</th>
				<td><asp:Literal ID="lMailSubject" runat="server" /></td>
			</tr>
			<tr>
				<th style="vertical-align :top">メール本文</th>
				<td id="tdNostyle">
					<% if (this.IsHtml){ %>
					<input type= "button" onclick="javascript:open_window('<%: GetHtmlMailPopupUrl() %>','contact','width=828,height=500,top=120,left=420,status=NO,scrollbars=yes');" value="表示" />
					<% } %>
					<asp:Literal ID="lMailBody" runat="server" />
				</td>
			</tr>
		</table>
	</div>
	<div class="dvUserBtnBox">
		<p><a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_RECIEVE_MAIL_LIST %>" class="btn btn-large">戻る</a></p>
	</div>
</div>
	
<script type="text/javascript">
	function open_window(link_file, window_name, window_type) {
		var new_win = window.open(link_file, window_name, window_type);
		new_win.focus();
	}
</script>
</asp:Content>