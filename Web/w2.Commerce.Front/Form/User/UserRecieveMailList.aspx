<%--
=========================================================================================================
  Module      : 受信メール履歴画面(UserRecieveMailList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRecieveMailList.aspx.cs" Inherits="Form_User_UserRecieveMailList" Title="受信メール履歴" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<h2>受信メール履歴</h2>
	<div id="dvFavoriteList" class="unit">
		<h4>過去に受信したメールの履歴が表示されます。</h4>
		<!-- ページャ -->
		<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
		<!-- 受信メール一覧 -->
		<asp:Repeater id="rUserRecieveMailList" runat="server" ItemType="w2.Domain.MailSendLog.MailSendLogModel">
		<HeaderTemplate>
			<table cellspacing="0">
				<tr>
					<th>受信日時</th>
					<th>件名</th>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr style="cursor: pointer;" onclick="window.location.href='<%#: CreateUrlToDetail(Item.LogNo)%>';">
			<td width="30%">
				<%#: DateTimeUtility.ToStringFromRegion(Item.DateSendMail, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
			</td>
			<td width="70%">
				<%#: Item.MailSubject %>
			</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
		</asp:Repeater>

		<!-- エラーメッセージ -->
		<% if (StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
			<p><%= this.ErrorMessage %></p>
		<% } %>

		<!-- ページャ -->
		<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
	</div>
</div>

</asp:Content>