<%--
=========================================================================================================
  Module      : メールマガジン解除入力画面(MailMagazineCancelInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineCancelInput.aspx.cs" Inherits="Form_User_MailMagazineCancelInput" Title="メールマガジン解除入力ページ" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="dvUserContents">
		<h2>メールマガジン解除</h2>
	<div id="dvMailMagazineCancelInput" class="unit">
		<p>
			メールマガジンを解除される方は、下記のフォームに必要事項をご入力の上、「解除する」ボタンをクリックして下さい。<br /><br /><ins><span class="necessary">*</span>は必須入力となります。</ins></p>
		<div class="dvMailMagazineCancel">
			<table cellspacing="0">
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %><span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbMailAddr" Runat="server" CssClass="mailAddr mail-domain-suggest" MaxLength="256" Type="email"></asp:TextBox>
					</td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="javascript:history.back();" class="btn btn-large">
					キャンセル</a></span>
				<span><asp:LinkButton ID="lbCancel" runat="server" OnClick="lbCancelClick" class="btn btn-large btn-inverse">
					解除する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>