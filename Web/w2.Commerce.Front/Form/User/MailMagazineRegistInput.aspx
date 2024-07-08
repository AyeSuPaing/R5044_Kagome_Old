<%--
=========================================================================================================
  Module      : メールマガジン登録入力画面(MailMagazineRegistInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/MailMagazineRegistInput.aspx.cs" Inherits="Form_User_MailMagazineRegistInput" Title="メールマガジン登録入力ページ" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="dvUserContents">
	<%-- メルマガ登録系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p><img src="../../Contents/ImagesPkg/user/clumbs_mail_regist_1.gif" alt="メールアドレスの入力" /></p>
	</div>
		<h2>メールマガジン登録</h2>

	<div id="dvMailMagazineRegistInput" class="unit">
		<p>メールマガジンを登録される方は、下記のフォームに必要事項をご入力の上、「確認する」ボタンをクリックして下さい。<br /><br /><ins><span class="necessary">*</span>は必須入力となります。</ins></p>
		<div class="dvMailMagazineRegist">
			<table cellspacing="0">
				<tr>
					<%-- 氏名 --%>
					<th><%: ReplaceTag("@@User.name.name@@") %><span class="necessary">*</span></th>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
									<% SetMaxLength(WtbUserName1, "@@User.name1.length_max@@"); %>
									<span class="fname">姓</span><asp:TextBox id="tbUserName1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(WtbUserName2, "@@User.name2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox id="tbUserName2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：山田</span></td>
								<td><span class="notes">太郎</span></td>
							</tr>
						</table>
						<asp:CustomValidator ID="cvUserName1" runat="Server"
							ControlToValidate="tbUserName1"
							ValidationGroup="MailMagazineRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator ID="cvUserName2" runat="Server"
							ControlToValidate="tbUserName2"
							ValidationGroup="MailMagazineRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- 氏名（かな） --%>
					<th><%: ReplaceTag("@@User.name_kana.name@@") %>
						<% if (this.IsJapanese) { %>
							<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
									<% SetMaxLength(WtbUserNameKana1, "@@User.name_kana1.length_max@@"); %>
									<span class="fname">姓</span><asp:TextBox id="tbUserNameKana1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(WtbUserNameKana2, "@@User.name_kana2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox id="tbUserNameKana2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角ひらがな入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：やまだ</span></td>
								<td><span class="notes">たろう</span></td>
							</tr>
						</table>
						<% if (this.IsJapanese) { %>
						<asp:CustomValidator ID="cvUserNameKana1" runat="Server"
							ControlToValidate="tbUserNameKana1"
							ValidationGroup="MailMagazineRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator ID="cvUserNameKana2" runat="Server"
							ControlToValidate="tbUserNameKana2"
							ValidationGroup="MailMagazineRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td><asp:TextBox ID="tbUserMailAddr" Runat="server" CssClass="mailAddr mail-domain-suggest" MaxLength="256" Type="email"></asp:TextBox>
					<asp:CustomValidator ID="cvUserMailAddr" runat="Server"
						ControlToValidate="tbUserMailAddr"
						ValidationGroup="MailMagazineRegist"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>（確認用）
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddrConf" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<asp:CustomValidator ID="cvUserMailAddrConf" runat="Server"
							ControlToValidate="tbUserMailAddrConf"
							ValidationGroup="MailMagazineRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
			</table>
		</div>
		<%-- キャプチャ認証 --%>
		<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbConfirm.ClientID %>" />
		<div class="dvUserBtnBox">
			<p>
				<span><asp:LinkButton ID="lbConfirm" ValidationGroup="MailMagazineRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">
					確認する</asp:LinkButton></span>
			</p>
		</div>
	</div>

		<h2>メールマガジン解除</h2>

	<div class="unit">
		<p>メールマガジンを解除される方は、下記のボタンをクリックして、メールマガジン解除画面へ。</p>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_CANCEL_INPUT) %>" class="btn btn-large btn-inverse">
					メールマガジン解除</a>
				</span>
			</p>
		</div>
	</div>

</div>

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
