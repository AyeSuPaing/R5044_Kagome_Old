<%--
=========================================================================================================
  Module      : メールマガジン登録入力画面(MailMagazineRegistInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineRegistInput.aspx.cs" Inherits="Form_User_MailMagazineRegistInput" Title="メールマガジン登録入力ページ" MaintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>

<section class="wrap-user mailmagazine-regist-input">
<div class="user-unit">
	<h2>メールマガジン登録</h2>
	<p class="msg">
		メールマガジンを登録される方は、下記のフォームに必要事項をご入力の上、「確認する」ボタンをクリックして下さい。<br />
		<span class="attention">※は必須入力となります。</span>
	</p>
	
	<dl class="user-form">
		<dt>
			<%-- 氏名 --%>
			<%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span>
		</dt>
		<dd class="name">
			<p class="attention">
				<asp:CustomValidator ID="cvUserName1" runat="Server"
					ControlToValidate="tbUserName1"
					ValidationGroup="MailMagazineRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				<asp:CustomValidator ID="cvUserName2" runat="Server"
					ControlToValidate="tbUserName2"
					ValidationGroup="MailMagazineRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
			</p>
			<% tbUserName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
			<% tbUserName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
			<% SetMaxLength(this.WtbUserName1, "@@User.name1.length_max@@"); %>
			<% SetMaxLength(this.WtbUserName2, "@@User.name2.length_max@@"); %>
			<asp:TextBox id="tbUserName1" Runat="server"></asp:TextBox>
			<asp:TextBox id="tbUserName2" Runat="server"></asp:TextBox>
		</dd>
		<dt>
			<%-- 氏名（かな） --%>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			<% if (this.IsJapanese) { %>
				<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="name-kana">
			<% if (this.IsJapanese) { %>
			<p class="attention">
				<asp:CustomValidator ID="cvUserNameKana1" runat="Server"
					ControlToValidate="tbUserNameKana1"
					ValidationGroup="MailMagazineRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				<asp:CustomValidator ID="cvUserNameKana2" runat="Server"
					ControlToValidate="tbUserNameKana2"
					ValidationGroup="MailMagazineRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
			</p>
			<% } %>
			<% tbUserNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name_kana1.name@@"); %>
			<% tbUserNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name_kana2.name@@"); %>
			<% SetMaxLength(this.WtbUserNameKana1, "@@User.name_kana1.length_max@@"); %>
			<% SetMaxLength(this.WtbUserNameKana2, "@@User.name_kana2.length_max@@"); %>
			<asp:TextBox id="tbUserNameKana1" Runat="server"></asp:TextBox>
			<asp:TextBox id="tbUserNameKana2" Runat="server"></asp:TextBox>
		</dd>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %><span class="require">※</span>
		</dt>
		<dd class="mail">
			<p class="msg">お手数ですが、確認のため２度入力してください</p>
			<p class="attention">
				<asp:CustomValidator ID="cvUserMailAddr" runat="Server"
					ControlToValidate="tbUserMailAddr"
					ValidationGroup="MailMagazineRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				<asp:CustomValidator ID="cvUserMailAddrConf" runat="Server"
					ControlToValidate="tbUserMailAddrConf"
					ValidationGroup="MailMagazineRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
			</p>

			<asp:TextBox ID="tbUserMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></asp:TextBox>
			<asp:TextBox id="tbUserMailAddrConf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></asp:TextBox>
		</dd>
	</dl>
	</div>

	<%-- キャプチャ認証 --%>
	<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbConfirm.ClientID %>" /><br />
	<div class="user-footer user-unit">
		<div class="button-next">
			<asp:LinkButton ID="lbConfirm" CssClass="btn" ValidationGroup="MailMagazineRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click">確認する</asp:LinkButton>
		</div>
		<div class="button-prev">
			<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_INPUT) %>" class="btn">
				リセット
			</a>
		</div>
		<p class="msg">メールマガジンを解除される方は、下記のボタンをクリックして、メールマガジン解除画面へ。</p>
		<div class="button-next">
			<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_CANCEL_INPUT) %>" class="btn">メールマガジン解除</a>
		</div>
	</div>

</section>


<script type="text/javascript">
<!--
	bindEvent();

	<%-- イベントをバインドする --%>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする --%>
	function bindExecAutoKana() {
		execAutoKanaWithKanaType(
			$("#<%= tbUserName1.ClientID %>"),
			$("#<%= tbUserNameKana1.ClientID %>"),
			$("#<%= tbUserName2.ClientID %>"),
			$("#<%= tbUserNameKana2.ClientID %>"));
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		execAutoChangeKanaWithKanaType(
			$("#<%= tbUserNameKana1.ClientID %>"),
			$("#<%= tbUserNameKana2.ClientID %>"));
	}
//-->
</script>

</asp:Content>
