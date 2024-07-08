<%--
=========================================================================================================
  Module      : メールマガジン解除完了画面(MailMagazineCancelComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineCancelComplete.aspx.cs" Inherits="Form_User_MailMagazineCancelComplete" Title="メールマガジン解除完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user mailmagazine-cancel-input">
	<div class="user-unit">
		<h2>受付完了</h2>
		<p class="msg">
			メールマガジンを解除したメールアドレスは「<%: this.MailAddress %>」です。<br />
			今後とも、「<%: ShopMessage.GetMessage("ShopName") %>」をどうぞ宜しくお願い申し上げます。<br />
			<br />
			<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
		</p>
	</div>
	<div class="user-footer user-unit">
		<div class="button-next">
			<asp:LinkButton ID="lbTopPage" CssClass="btn" runat="server" OnClick="lbTopPage_Click">
				トップページへ
			</asp:LinkButton>
		</div>
	</div>
</section>
</asp:Content>