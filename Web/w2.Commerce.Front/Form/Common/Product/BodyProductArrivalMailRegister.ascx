<%--
=========================================================================================================
  Module      : 入荷通知メール登録ユーザコントロール(BodyProductArrivalMailRegister.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ProductArrivalMailRegisterUserControl" %>

<% if (this.IsRegisterSucceeded == false) {%>
<div id="divProductArrivalMail">
こちらのアイテムが入荷した際、メールにてお知らせします。<br />
（通知期限：<%: DateTimeUtility.ToStringFromRegion(this.ExpiredDate, DateTimeUtility.FormatType.EndOfYearMonth1Letter) %>）
<br />
<span visible="<%# (this.IsLoggedIn == false) %>" runat="server" style="color: Red;">※会員の方はログインしてから登録すると、登録状況を後で確認できます。</span>
<table>
	<tr>
		<th rowspan="4">通知先アドレス<span class="necessary">*</span></th>
	</tr>
	<tr visible="<%# (this.IsLoggedIn && this.HasPcAddr) %>" runat="server">
		<td><asp:CheckBox id="cbUserPcAddr" runat="server" Checked="<%# (this.IsPcAddrRegistered || this.HasPcAddr) %>" /><%: ReplaceTag("@@User.mail_addr.name@@") %><br />
			(<%#: this.PcAddr %>)</td>
	</tr>
	<tr visible="<%# (Constants.MOBILEOPTION_ENABLED && this.IsLoggedIn && this.HasMbAddr) %>" runat="server">
		<td><asp:CheckBox id="cbUserMobileAddr" runat="server" Checked="<%# (Constants.MOBILEOPTION_ENABLED && (this.IsMbAddrRegistered || (this.HasPcAddr == false))) %>" /><%: ReplaceTag("@@User.mail_addr2.name@@") %><br />
			(<%#: this.MbAddr %>)</td>
	</tr>
	<tr>
		<td>
			<asp:CheckBox id="cbMailAddr" runat="server" Checked="<%# (this.IsLoggedIn == false) %>" Text="その他" /><br />
			<asp:TextBox ID="tbMailAddr" runat="server" CssClass="mailAddr" MaxLength="256" Width="200" /><br />
			<% if (StringUtility.ToEmpty(this.ErrorMessage) != "") {%>
				<br /><span class="error_inline"><%: this.ErrorMessage %></span>
			<%} %>
		</td>
	</tr>
</table>
<p class="btnClose">
	<asp:LinkButton ID="lbRegisterBtn" OnClick="lbRegister_Click" runat="server" class="btn btn-inverse">登録する</asp:LinkButton>
</p>
</div>
<%} else {%>
<p> 登録が完了しました！
	<% if(this.HasPcMailKbnResult) {%>[PC]<%} %>
	<% if(this.HasMbMailKbnResult) {%>[モバイル]<%} %>
	<% if(this.HasOtherMailKbnResult) {%>[その他]<%} %>
</p>
<%} %>