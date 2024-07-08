<%--
=========================================================================================================
  Module      : メールマガジン登録完了画面(MailMagazineRegistComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineRegistComplete.aspx.cs" Inherits="Form_User_MailMagazineRegistComplete" Title="メールマガジン登録完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<%-- メルマガ登録系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p><img src="../../Contents/ImagesPkg/user/clumbs_mail_regist_3.gif" alt="登録完了" /></p>
	</div>

		<h2>登録完了</h2>

	<div id="dvMailMagazineRegistComplete" class="unit">
		<p class="completeInfo">メールマガジンを登録したメールアドレスは「<span><%: this.UserMailAddr%></span>」です。<br />
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
<w2c:FacebookConversionAPI
	EventName="CompleteRegistration"
	UserId="<%#: this.LoginUserId %>"
	CustomDataContentName="Content name"
	CustomDataValue="500.000"
	CustomDataCurrency="JPY"
	CustomDataStatus="Status"
	runat="server" />
</asp:Content>