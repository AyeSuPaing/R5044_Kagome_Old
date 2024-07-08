<%--
=========================================================================================================
  Module      : トップメッセージ印刷ページ(PrintMessage.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintMessage.aspx.cs" Inherits="Form_Top_PrintMessage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		【メディア・受発信】<%: ValueText.GetValueText(
			Constants.TABLE_CSMESSAGE,
			Constants.FIELD_CSMESSAGE_MEDIA_KBN,
			Message.MediaKbn) %>
		&nbsp;<%: ValueText.GetValueText(
			Constants.TABLE_CSMESSAGE,
			Constants.FIELD_CSMESSAGE_DIRECTION_KBN,
			Message.DirectionKbn) %><br />
		<% if (Message.InquiryReplyDate.HasValue) {%>
			【問合せ日時】<%: DateTimeUtility.ToStringForManager(
				Message.InquiryReplyDate,
				DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %><br />
		<%} %>
		【メッセージステータス】<%: Message.EX_MessageStatusName %><br />
		<% if (Message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL) {%>
			【From】<%: Message.EX_Mail.MailFrom %><br />
			【To】<%: Message.EX_Mail.MailTo %><br />
			<% if (string.IsNullOrEmpty(Message.EX_Mail.MailCc) == false) {%>
				【Cc】<%: Message.EX_Mail.MailCc %><br />
			<%} %>
			<% if (string.IsNullOrEmpty(Message.EX_Mail.MailBcc) == false) {%>
				【Bcc】<%: Message.EX_Mail.MailBcc %><br />
			<%} %>
			【Subject】<%: Message.EX_Mail.MailSubject %><br />
			<% if (Message.EX_Mail.SendDatetime.HasValue) {%>
					【Time】<%:WebSanitizer.HtmlEncode(
						DateTimeUtility.ToStringForManager(
							this.Message.EX_Mail.SendDatetime,
							DateTimeUtility.FormatType.ShortDateHourMinute2Letter)) %><br />
			<% } else if (Message.EX_Mail.ReceiveDatetime.HasValue) {%>
					【Time】<%:WebSanitizer.HtmlEncode(
						DateTimeUtility.ToStringForManager(
							this.Message.EX_Mail.ReceiveDatetime,
							DateTimeUtility.FormatType.ShortDateHourMinute2Letter)) %><br />
			<% } %>
			【Body】<br /><%= WebSanitizer.HtmlEncodeChangeToBr(Message.EX_Mail.MailBody) %><br />
			<% if (Message.EX_Mail.EX_MailAttachments.Length > 0) {%>
				【添付】
				<% foreach (var attachment in Message.EX_Mail.EX_MailAttachments) {%>
					<%: attachment.FileName %><br />
				<%} %>
			<%} %>
		<%} %>
		<% else {%>
			【氏名】<%: Message.UserName1 %><%: Message.UserName2 %><br />
			【かな】<%: Message.UserNameKana1 %><%: Message.UserNameKana2 %><br />
			【メールアドレス】<%: Message.UserMailAddr %><br />
			【電話番号】<%: Message.UserTel1 %><br />
			【件名】<%: Message.InquiryTitle %><br />
		    【内容】<br /><%= WebSanitizer.HtmlEncodeChangeToBr(Message.InquiryText) %><br />
			【回答】<br /><%= WebSanitizer.HtmlEncodeChangeToBr(Message.ReplyText) %><br />
		<%} %>
	</div>
	</form>
</body>
</html>
