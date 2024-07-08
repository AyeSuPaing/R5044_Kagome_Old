<%--
=========================================================================================================
  Module      : メールマガジン解除完了画面(MailMagazineCancelComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineCancelComplete.aspx.cs" Inherits="Form_User_MailMagazineCancelComplete" Title="メールマガジン解除完了ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">

		<h2>受付完了</h2>

	<div id="dvMailMagazineCancelComplete" class="unit">
		<p class="completeInfo">メールマガジンを解除したメールアドレスは「<span><%: this.MailAddress %></span>」です。<br />
		今後とも、「<%: ShopMessage.GetMessage("ShopName") %>」をどうぞ宜しくお願い申し上げます。<br />
		</p>
		<p class="receptionInfo">
			<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
		</p>
		<div class="dvUserBtnBox">
			<p><span><asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click" class="btn btn-large btn-inverse">
				トップページへ</asp:LinkButton></span></p>
		</div>
	</div>
</div>
</asp:Content>
