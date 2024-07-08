<%--
=========================================================================================================
  Module      : メールマガジン解除入力画面(MailMagazineCancelInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineCancelInput.aspx.cs" Inherits="Form_User_MailMagazineCancelInput" Title="メールマガジン解除入力ページ" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<section class="wrap-user mailmagazine-cancel-input">
	<div class="user-unit">
		<h2>メールマガジン解除</h2>
		<p class="msg">
			メールマガジンを解除される方は、下記のフォームに必要事項をご入力の上、「解除する」ボタンをクリックして下さい。<br />
			<span class="attention">※は必須入力となります。</span>
		</p>

		<dl class="user-form">
			<dt>
				<%: ReplaceTag("@@User.mail_addr.name@@") %>
				<span class="require">※</span>
			</dt>
			<dd class="mail"><asp:TextBox ID="tbMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></asp:TextBox></dd>
		</dl>
	</div>
		
	<div class="user-footer user-unit">
		<div class="button-next">
			<asp:LinkButton ID="lbCancel" CssClass="btn" runat="server" OnClick="lbCancelClick">
				解除する
			</asp:LinkButton>
		</div>
		<div class="button-prev">
			<a href="javascript:history.back();" class="btn">
				キャンセル
			</a>
		</div>
	</div>
</section>
</asp:Content>