<%--
=========================================================================================================
  Module      : メールマガジン登録画面(BodyMailMagazineRegistration.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="BodyMailMagazineRegistrationControl" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<style>
	#dvMailMagazineRegistInputTitle, #dvUserModifyConfirmTitle .title {
		font-size: 15px;
		font-weight: bold;
		padding: 8px;
	}
</style>
<%-- ▽ミニカート（UpdatePanel）▽ --%>
<asp:UpdatePanel ID="upMailMagazine" runat="server">
<ContentTemplate>
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="dvUserBox" class="user-unit">
	<div id="dvMailMagazineRegistInput" class="unit" runat="server">
		<div id="dvMailMagazineRegistInputTitle">
			<p class="title">メールマガジン登録</p>
		</div>
		<p>
			メールマガジンを登録される方は、下記のフォームにメールアドレスをご入力の上、「登録する」ボタンをクリックして下さい。<br />
			<br />
			<ins><span class="necessary">*</span>は必須入力となります。</ins>
		</p>
		<div class="dvMailMagazineRegist">
			<table cellspacing="0">
				<tr>
					<th>メールアドレス<span class="necessary">*</span></th>
					<td>
						<asp:TextBox ID="tbUserMailAddr" runat="server" CssClass="mailAddr" MaxLength="256" Type="email" />
						<asp:CustomValidator ID="cvUserMailAddr" runat="Server"
							ControlToValidate="tbUserMailAddr"
							ValidationGroup="MailMagazineRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span>
					<asp:LinkButton ValidationGroup="MailMagazineRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">
					登録する</asp:LinkButton></span>
			</p>
		</div>
	</div>

	<div id="dvUserModifyConfirm" class="unit" runat="server">
		<div id="dvUserModifyConfirmTitle">
			<p class="title">登録完了</p>
		</div>

		<%-- メッセージ --%>
		<p class="completeInfo">
			<%: ShopMessage.GetMessage("ShopName") %>会員情報の変更を受け付けました。<br />
			今後とも<%: ShopMessage.GetMessage("ShopName") %>をどうぞ宜しくお願い申し上げます。
		</p>
		<p class="receptionInfo">
			<%: ShopMessage.GetMessage("ContactCenterInfo") %>
		</p>
	</div>

</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △ミニカート（UpdatePanel）△ --%>
<%-- △編集可能領域△ --%>