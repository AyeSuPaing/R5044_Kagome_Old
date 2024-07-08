<%--
=========================================================================================================
  Module      : 入荷通知メール登録ユーザコントロール(BodyProductArrivalMailRegister.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ProductArrivalMailRegisterUserControl" %>

<tr id="trArrivalMailForm" visible="false">
<td colspan="3" style="background-color : #f8f8f8" id="RegisterArrivalForm">
<% if (this.IsRegisterSucceeded == false) {%>
<section class="wrap-user user-product-arrival-mail-regist">
<div id="divInput" class="user-unit" runat="server">
	<div class="msg">
	こちらのアイテムが入荷した際、メールにてお知らせします。（通知期限：<%: DateTimeUtility.ToStringFromRegion(this.ExpiredDate, DateTimeUtility.FormatType.EndOfYearMonth1Letter) %>）
	<br />
	<span class="attention" visible="<%# (this.IsLoggedIn == false) %>" runat="server">
		※会員の方はログインしてから登録すると、登録状況を後で確認できます。
	</span>
	</div>
	<dl class="user-form">
		<dt>通知先アドレス</dt>
		<dd class="mail mail-arrival" visible="<%# this.IsLoggedIn && this.HasPcAddr %>" runat="server">
			<asp:CheckBox id="cbUserPcAddr" runat="server" Checked="<%# this.IsPcAddrRegistered || this.HasPcAddr %>" /><%: ReplaceTag("@@User.mail_addr.name@@") %><br />
			(<%#: this.PcAddr %>)
		</dd>
		<dd class="mobile mail-arrival" visible="<%# (Constants.MOBILEOPTION_ENABLED && this.IsLoggedIn && this.HasMbAddr) %>" runat="server">
			<asp:CheckBox id="cbUserMobileAddr" runat="server" Checked="<%# (Constants.MOBILEOPTION_ENABLED && (this.IsMbAddrRegistered || (this.HasPcAddr == false))) %>" /><%: ReplaceTag("@@User.mail_addr2.name@@") %><br />
			(<%#: this.MbAddr %>)
		</dd>
		
		<dd class="mail mail-arrival">
			<% if (StringUtility.ToEmpty(this.ErrorMessage) != "") {%><p class="attention"><%: this.ErrorMessage %></p><%} %>
			<asp:CheckBox id="cbMailAddr" runat="server" Checked="<%# (this.IsLoggedIn == false) %>" Text="その他アドレス" /><br />
			<w2c:ExtendedTextBox ID="tbMailAddr" type="textbox" runat="server" MaxLength="256" />
		</dd>
	</dl>

	<div class="user-footer-arrival">
		<div class="button-next">
			<asp:LinkButton ID="lbRegisterBtn" OnClick="lbRegister_Click" runat="server" CssClass="btn btn-arrival">登録する</asp:LinkButton>
		</div>
	</div>
</div>
</section>
<%} else {%>
<p> 登録が完了しました！
	<% if(this.HasPcMailKbnResult) {%>[PC]<%} %>
	<% if(this.HasMbMailKbnResult) {%>[モバイル]<%} %>
	<% if(this.HasOtherMailKbnResult) {%>[その他]<%} %>
</p>
<%} %>
</td>
</tr>
