<%--
=========================================================================================================
  Module      : メールマガジン登録完了画面(MailMagazineRegistComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineRegistComplete.aspx.cs" Inherits="Form_User_MailMagazineRegistComplete" Title="メールマガジン登録完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user mailmagazine-regist-complete">
<div class="user-unit">
	<h2>登録完了</h2>
	<p class="msg">
		メールマガジンを登録したメールアドレスは「<%: this.UserMailAddr%>」です。<br />
		今後とも、「<%: ShopMessage.GetMessage("ShopName") %>」をどうぞ宜しくお願い申し上げます。<br />
		<br />
		<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
	</p>
</div>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbTopPage" CssClass="btn" runat="server" OnClick="lbTopPage_Click">
			トップページへ
		</asp:LinkButton>
	</div>
</div>

</section>
<w2c:FacebookConversionAPI
	EventName="CompleteRegistration"
	UserId="<%#: this.LoginUserId %>"
	CustomDataContentName="Content name"
	CustomDataValue="500.000"
	CustomDataCurrency="JPY"
	CustomDataStatus="Status"
	runat="server" />
</asp:Content>