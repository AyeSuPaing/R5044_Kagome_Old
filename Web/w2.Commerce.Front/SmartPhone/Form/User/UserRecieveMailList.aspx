<%--
=========================================================================================================
  Module      : 受信メール履歴画面(UserRecieveMailList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRecieveMailList.aspx.cs" Inherits="Form_User_UserRecieveMailList" Title="受信メール履歴" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user fixed-purchase-list">
<div class="user-unit">
	<h2>受信メール履歴</h2>
	<p id="pInfo" class="msg">過去に受信したメールの履歴が表示されます</p>
	<!-- ページャ -->
	<div class="pager-wrap above"><%= this.PagerHtml %></div>
	<!-- 受信メール一覧 -->
	<asp:Repeater id="rUserRecieveMailList" runat="server" ItemType="w2.Domain.MailSendLog.MailSendLogModel">
	<HeaderTemplate>
	<div class="content">
	<ul>
	</HeaderTemplate>
	<ItemTemplate>
	<li>
		<h3>受信日時</h3>
		<h4 class="fixed-id">
			<%#: DateTimeUtility.ToStringFromRegion(Item.DateSendMail, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
		</h4>
		<dl class="user-form" onclick="window.location.href='<%#: WebSanitizer.HtmlEncode(CreateUrlToDetail(Item.LogNo))%>';">
			<dt>
				件名
			</dt>
			<dd>
				<%#: Item.MailSubject %>
			</dd>
		</dl>
	</li>
	</ItemTemplate>
	<FooterTemplate>
	</ul>
	</div>
	</FooterTemplate>

	</asp:Repeater>
	<%-- エラーメッセージ --%>
	<% if (StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
		<p><%= this.ErrorMessage %></p>
	<% } %>

	<!-- ページャ -->
	<div class="pager-wrap above"><%= this.PagerHtml %></div>
</div>
<div class="user-footer">
	<div class="button-next">
		<a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>