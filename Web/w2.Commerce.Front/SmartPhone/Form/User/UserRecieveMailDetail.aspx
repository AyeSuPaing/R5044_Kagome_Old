<%--
=========================================================================================================
  Module      : 受信メール詳細画面(UserRecieveMailDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRecieveMailDetail.aspx.cs" Inherits="Form_User_UserRecieveMailDetail" Title="受信メール履歴" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user fixed-purchase-list">
<div class="user-unit">
	<h2>受信メール詳細</h2>
	<div class="content">
	<ul>
	<li>
		<h3>内容</h3>
		<dl class="user-form">
			<dt>
				受信日時
			</dt>
			<dd>
				<asp:Literal ID="lDateSendMail" runat="server" />
			</dd>
			<dt>
				送信元
			</dt>
			<dd>
				<asp:Literal ID="lMailFrom" runat="server" />
			</dd>
			<dt>
				送信先
			</dt>
			<dd>
				<asp:Literal ID="lMailTo" runat="server" />
			</dd>
			<dt>
				メール件名
			</dt>
			<dd>
				<asp:Literal ID="lMailSubject" runat="server" />
			</dd>
			<dt>
				メール本文
			</dt>
			<dd>
				<% if (this.IsHtml){ %>
					<input type= "button" onclick="javascript:open_window('<%: GetHtmlMailPopupUrl() %>	','contact','width=828,height=500,top=120,left=420,status=NO,scrollbars=yes');" value="表示" />
				<% } %>
				<asp:Literal ID="lMailBody" runat="server" />
			</dd>
		</dl>
	</li>
	</ul>
	<div class="user-footer">
		<div class="button-next">
			<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_RECIEVE_MAIL_LIST) %>" class="btn">戻る</a>
		</div>
	</div>

</div>

</section>
<script type="text/javascript">
	function open_window(link_file, window_name, window_type) {
		var new_win = window.open(link_file, window_name, window_type);
		new_win.focus();
	}
</script>
</asp:Content>