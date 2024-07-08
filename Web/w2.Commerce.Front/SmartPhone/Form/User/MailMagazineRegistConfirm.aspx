<%--
=========================================================================================================
  Module      : メールマガジン登録確認画面(MailMagazineRegistConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineRegistConfirm.aspx.cs" Inherits="Form_User_MailMagazineRegistConfirm" Title="メールマガジン登録確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user mailmagazine-regist-comfirm">
<div class="user-unit">
	<h2>入力内容の確認</h2>
	<%-- メッセージ --%>
	<p class="msg">
		お客様の入力された内容は以下の通りでよろしいでしょうか？<br />
		よろしければ「登録する」ボタンを押して下さい。
	</p>

	<dl class="user-form">
		<dt>
			<%: ReplaceTag("@@User.name.name@@") %>
		</dt>
		<dd><%: this.UserInput.Name1 %><%: this.UserInput.Name2 %></dd>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
		</dt>
		<dd><%: this.UserInput.NameKana1 %><%: this.UserInput.NameKana2 %></dd>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
		</dt>
		<dd><%: this.UserInput.MailAddr %></dd>
	</dl>
</div>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbSend" CssClass="btn" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click">
			登録する
		</asp:LinkButton>
	</div>
	<div class="button-prev">
		<asp:LinkButton ID="lbBack" CssClass="btn" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click">
			戻る
		</asp:LinkButton>
	</div>
</div>

</section>
</asp:Content>

