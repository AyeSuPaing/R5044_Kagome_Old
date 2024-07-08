<%--
=========================================================================================================
  Module      : メールマガジン登録確認画面(MailMagazineRegistConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineRegistConfirm.aspx.cs" Inherits="Form_User_MailMagazineRegistConfirm" Title="メールマガジン登録確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<%-- メルマガ登録系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p><img src="../../Contents/ImagesPkg/user/clumbs_mail_regist_2.gif" alt="入力内容の入力" /></p>
	</div>

		<h2>入力内容の確認</h2>

	<div id="dvMailMagazineRegistConfirm" class="unit">
		<%-- メッセージ --%>
		<div class="dvContentsInfo">
			<p>
				お客様の入力された内容は以下の通りでよろしいでしょうか？<br />
				よろしければ「送信する」ボタンを押して下さい。</p>
		</div>
		<div class="dvUserInfo">
			<h3>お客様情報</h3>
			<table cellspacing="0">
				<tr>
					<th>
						<%: ReplaceTag("@@User.name.name@@") %>
					</th>
					<td><%: this.UserInput.Name1 %><%: this.UserInput.Name2 %></td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.name_kana.name@@") %>
					</th>
					<td><%: this.UserInput.NameKana1 %><%: this.UserInput.NameKana2 %></td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
					</th>
					<td><%: this.UserInput.MailAddr %></td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn btn-large">
					戻る</asp:LinkButton></span>
				<span><asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">
					送信する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>