<%--
=========================================================================================================
  Module      : 会員登録変更入力画面(UserModifyInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendModify" Src="~/SmartPhone/Form/Common/User/BodyUserExtendModify.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserModifyInput.aspx.cs" Inherits="Form_User_UserModifyInput" title="登録情報変更入力ページ"  MaintainScrollPositionOnPostBack="true" %>
<%@ Import Namespace="w2.App.Common.Amazon.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<section class="wrap-user user-mofify-input">
<asp:UpdatePanel runat="server">
	<ContentTemplate>
<div class="user-unit">
	<h2>会員情報の確認・変更</h2>
	<div class="msg">
		<%if (this.IsEasyUser) {%>
		<p style="margin:5px;padding:5px;text-align:center;background-color:#ffff80;border:1px solid #D4440D;border-color:#E5A500;color:#CC7600;">ご購入手続きに必要な会員情報が不足しています。</p>
		<%} %>
		<p>登録情報の変更をご希望の方は、下記のフォームに必要事項をご入力の上「入力内容を確認する」ボタンをクリックしてください</p>
		<p class="attention">※は必須項目となります</p>
	</div>
	<dl class="user-form">
		<dt>
			<%-- 氏名 --%>
			<%: ReplaceTag("@@User.name.name@@") %>
			<% if (this.IsUserNameNecessary) { %>
				<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="name">
			<p class="attention">
				<% if (this.IsUserNameNecessary) { %>
				<asp:CustomValidator
					ID="cvUserName1"
					runat="Server"
					ControlToValidate="tbUserName1"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserName2"
					runat="Server"
					ControlToValidate="tbUserName2"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
			</p>
			<% tbUserName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
			<% tbUserName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
			<w2c:ExtendedTextBox id="tbUserName1" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox id="tbUserName2" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
		</dd>
		<% if (this.IsUserAddrJp) { %>
		<dt>
			<%-- 氏名（かな） --%>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			<% if (this.IsUserNameKanaNecessary) { %>
				<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="name-kana">
			<p class="attention">
				<% if (this.IsUserNameKanaNecessary) { %>
				<asp:CustomValidator
					ID="cvUserNameKana1"
					runat="Server"
					ControlToValidate="tbUserNameKana1"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserNameKana2"
					runat="Server"
					ControlToValidate="tbUserNameKana2"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
			</p>
			<% tbUserNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name_kana1.name@@"); %>
			<% tbUserNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name_kana2.name@@"); %>
			<w2c:ExtendedTextBox id="tbUserNameKana1" Runat="server" maxlength="20"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox id="tbUserNameKana2" Runat="server"  maxlength="20"></w2c:ExtendedTextBox>
		</dd>
		<% } %>
		<%if (Constants.PRODUCTREVIEW_ENABLED) { %>
		<dt>
			<%-- ニックネーム --%>
			<%: ReplaceTag("@@User.nickname.name@@") %>
		</dt>
		<dd class="nickname">
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserNickName"
					runat="Server"
					ControlToValidate="tbUserNickName"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbUserNickName" autocapitalize="OFF" runat="server" MaxLength="20" CssClass="nickname"></w2c:ExtendedTextBox><br />
		</dd>
		<%} %>
		<dt>
			<%-- 生年月日 --%>
			<%: ReplaceTag("@@User.birth.name@@") %>
			<% if (this.IsUserBirthNecessary) { %>
				<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="birth">
			<p class="attention">
				<% if (this.IsUserBirthNecessary) { %>
				<asp:CustomValidator
					ID="cvUserBirthYear"
					runat="Server"
					ControlToValidate="ddlUserBirthYear"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserBirthMonth"
					runat="Server"
					ControlToValidate="ddlUserBirthMonth"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserBirthDay"
					runat="Server"
					ControlToValidate="ddlUserBirthDay"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<% } %>
			</p>
			<asp:DropDownList id="ddlUserBirthYear" runat="server" CssClass="year" onchange="changeDropDownDays()"></asp:DropDownList>年
			<asp:DropDownList id="ddlUserBirthMonth" runat="server" CssClass="month" onchange="changeDropDownDays()"></asp:DropDownList>月
			<asp:DropDownList id="ddlUserBirthDay" runat="server" CssClass="date"></asp:DropDownList>日<br />
		</dd>
		<dt>
			<%-- 性別 --%>
			<%: ReplaceTag("@@User.sex.name@@") %>
			<% if (this.IsUserSexNecessary) { %>
			<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="sex">
			<p class="attention">
				<% if (this.IsUserSexNecessary) { %>
				<asp:CustomValidator
					ID="cvUserSex"
					runat="Server"
					ValidationGroup="UserModify"
					ControlToValidate="rblUserSex"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<% } %>
			</p>
			<asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" CellSpacing="0" RepeatLayout="Flow"></asp:RadioButtonList>
		</dd>
		<dt>
			<%-- PCメールアドレス --%>
			<%: ReplaceTag("@@User.mail_addr.name@@") %><%if (this.IsPcSiteOrOfflineUser) {%><span class="require">※</span><%} %>
		</dt>
		<dd class="mail">
			<p class="msg">お手数ですが、確認のため２度入力してください。</p>
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserMailAddrForCheck"
					runat="Server"
					ControlToValidate="tbUserMailAddr"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserMailAddr"
					runat="Server"
					ValidationGroup="UserModify"
					ControlToValidate="tbUserMailAddr"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserMailAddrConf"
					runat="Server"
					ControlToValidate="tbUserMailAddrConf"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbUserMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox id="tbUserMailAddrConf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox><br />
			<% cvUserMailAddrConf.ValidateEmptyText = this.IsPcSiteOrOfflineUser; %>
			<%if (this.IsPcSiteOrOfflineUser && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
				<p class="msg">※ログイン時に利用します</p>
			<%} %>
		</dd>
		<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
		<dt>
			<%-- モバイルメールアドレス --%>
			<%: ReplaceTag("@@User.mail_addr2.name@@") %><%if ((this.IsPcSiteOrOfflineUser == false) || (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)) {%><span class="require">※</span><%} %>
		</dt>
		<dd class="mobile">
			<p class="msg">お手数ですが、確認のため２度入力してください。</p>
			<p class="attention">
				<% cvUserMailAddr2.ValidateEmptyText = (this.IsPcSiteOrOfflineUser == false); %>
				<asp:CustomValidator
					ID="cvUserMailAddr2ForCheck"
					runat="Server"
					ControlToValidate="tbUserMailAddr2"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserMailAddr2"
					runat="Server"
					ControlToValidate="tbUserMailAddr2"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% cvUserMailAddr2Conf.ValidateEmptyText = (this.IsPcSiteOrOfflineUser == false); %>
				<asp:CustomValidator
					ID="cvUserMailAddr2Conf"
					runat="Server"
					ControlToValidate="tbUserMailAddr2Conf"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbUserMailAddr2" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox id="tbUserMailAddr2Conf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<%if ((this.IsPcSiteOrOfflineUser == false) && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
			<p class="msg">※ログイン時に利用します</p>
			<%} %>
		</dd>
		<% } %>
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<dt>
			<%: ReplaceTag("@@User.country.name@@", this.UserAddrCountryIsoCode) %>
		</dt>
		<dd>
			<asp:DropDownList ID="ddlUserCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUserAddrCountry_SelectedIndexChanged"></asp:DropDownList><br/>
			<asp:CustomValidator
				ID="cvUserCountry"
				runat="Server"
				ControlToValidate="ddlUserCountry"
				ValidationGroup="UserModify"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false"
				CssClass="error_inline" />
			<span id="countryAlertMessage" class="msg" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
		</dd>
		<% } %>
		<% if (this.IsUserAddrJp) { %>
		<dt>
			<%-- 郵便番号 --%>
			<%: ReplaceTag("@@User.zip.name@@") %>
			<% if (this.IsUserZipNecessary) { %>
			<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="zip">
			<p class="attention" id="addrSearchErrorMessage">
				<% if (this.IsUserZipNecessary) { %>
				<asp:CustomValidator
					ID="cvUserZip1"
					runat="Server"
					ControlToValidate="tbUserZip"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<span><%: this.ZipInputErrorMessage %></span>
				<% } %>
			</p>
			<w2c:ExtendedTextBox ID="tbUserZip" Type="tel" MaxLength="8" OnTextChanged="lbSearchAddr_Click" runat="server" />
			<asp:LinkButton ID="lbSearchAddr" runat="server" OnClick="lbSearchAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">郵便番号から住所を入力</asp:LinkButton>
			<%--検索結果レイヤー--%>
			<uc:Layer ID="ucLayer" runat="server" />
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>
			<% if (this.IsUserAddr1Necessary
					|| this.IsUserAddr2Necessary 
					|| this.IsUserAddr3Necessary)
				{ %>
			<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="address">
			<p class="attention">
				<% if (this.IsUserAddr1Necessary) { %>
				<asp:CustomValidator
					ID="cvUserAddr1"
					runat="Server"
					ControlToValidate="ddlUserAddr1"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
				<% if(IsCountryTw(this.UserAddrCountryIsoCode) == false) { %>
					<% if (this.IsUserAddr2Necessary) { %>
					<asp:CustomValidator
						ID="cvUserAddr2"
						runat="Server"
						ControlToValidate="tbUserAddr2"
						ValidationGroup="UserModify"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
					<% } %>
					<% if (this.IsUserAddr3Necessary) { %>
					<asp:CustomValidator
						ID="cvUserAddr3"
						runat="Server"
						ControlToValidate="tbUserAddr3"
						ValidationGroup="UserModify"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
					<% } %>
				<% } %>
				<asp:CustomValidator
					ID="cvUserAddr4"
					runat="Server"
					ControlToValidate="tbUserAddr4"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<%-- 都道府県 --%>
			<% if (this.IsUserAddrJp) { %>
			<asp:DropDownList id="ddlUserAddr1" runat="server"></asp:DropDownList>
			<% } %>
			<% if(IsCountryTw(this.UserAddrCountryIsoCode)) { %>
				<asp:DropDownList runat="server" ID="ddlUserAddr2" AutoPostBack="true" DataTextField="Text" DataValueField="Value" Width="95" OnSelectedIndexChanged="ddlUserAddr2_SelectedIndexChanged"></asp:DropDownList>
				<br />
				<asp:DropDownList runat="server" ID="ddlUserAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
			<% } else { %>
				<w2c:ExtendedTextBox id="tbUserAddr2" Runat="server" MaxLength="40" placeholder='市区町村'></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserAddr3" Runat="server" MaxLength="40" placeholder='番地'></w2c:ExtendedTextBox>
			<% } %>
			<w2c:ExtendedTextBox id="tbUserAddr4"  Runat="server" MaxLength="40" placeholder='建物名'></w2c:ExtendedTextBox>

			<% if (this.IsUserAddrJp == false) { %>
				<% if (this.IsUserAddrUs) { %>
				<asp:DropDownList ID="ddlUserAddr5" runat="server"></asp:DropDownList>
					<asp:CustomValidator
						ID="cvUserAddr5"
						runat="Server"
						ControlToValidate="ddlUserAddr5"
						ValidationGroup="UserModifyGlobal"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false"
						CssClass="error_inline" />
				<% } else if (IsCountryTw(this.UserAddrCountryIsoCode)) { %>
				<w2c:ExtendedTextBox id="tbUserAddrTw" Runat="server" MaxLength="40" placeholder='省'></w2c:ExtendedTextBox>
				<% } else { %>
				<w2c:ExtendedTextBox id="tbUserAddr5" Runat="server" MaxLength="40" placeholder='州'></w2c:ExtendedTextBox>
				<% } %>
			<% } %>
		</dd>
		<%-- 郵便番号（海外向け） --%>
		<% if (this.IsUserAddrJp == false) { %>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@", this.UserAddrCountryIsoCode) %>
			<% if (this.IsUserAddrZipNecessary) { %><span class="require">※</span><% } %>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserZipGlobal"
					runat="Server"
					ControlToValidate="tbUserZipGlobal"
					ValidationGroup="UserModifyGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbUserZipGlobal" Type="tel" Runat="server" MaxLength="20"></w2c:ExtendedTextBox>
			<asp:LinkButton
				ID="lbSearchAddrFromZipGlobal"
				OnClick="lbSearchAddrFromZipGlobal_Click"
				Style="display:none;"
				runat="server" />
		</dd>
		<% } %>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
		<dt>
			<%: ReplaceTag("@@User.company_name.name@@") %>
		</dt>
		<dd class="company-name">
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserCompanyName"
					runat="Server"
					ControlToValidate="tbUserCompanyName"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbUserCompanyName.Attributes["placeholder"] = ReplaceTag("@@User.company_name.name@@"); %>
			<w2c:ExtendedTextBox id="tbUserCompanyName" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
		</dd>
		<dt>
			<%: ReplaceTag("@@User.company_post_name.name@@") %>
		</dt>
		<dd class="company-post">
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserCompanyPostName"
					runat="Server"
					ControlToValidate="tbUserCompanyPostName"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbUserCompanyPostName.Attributes["placeholder"] = ReplaceTag("@@User.company_post_name.name@@"); %>
			<w2c:ExtendedTextBox id="tbUserCompanyPostName" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
		</dd>
		<% } %>
		<% if (this.IsUserAddrJp) { %>
		<dt>
			<%-- 電話番号 --%>
			<%: ReplaceTag("@@User.tel1.name@@") %>
			<% if (this.IsUserTel1Necessary) { %>
			<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="tel">
			<p class="attention">
				<% if (this.IsUserTel1Necessary) { %>
				<asp:CustomValidator
					ID="cvUserTel1"
					runat="Server"
					ControlToValidate="tbUserTel1_1"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
			</p>
			<w2c:ExtendedTextBox ID="tbUserTel1_1" Type="tel" MaxLength="13" style="width:100%;" runat="server" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvUserTel1')" />
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
			<asp:LinkButton
				ID="lbGetAuthenticationCode"
				CssClass="btn-add-get"
				Enabled="false"
				runat="server"
				Text="認証コードの取得"
				OnClick="lbGetAuthenticationCode_Click"
				OnClientClick="return checkTelNoInput();" />
			<asp:Label ID="lbAuthenticationStatus" runat="server" />
			<% } %>
		</dd>
		<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
		</dt>
		<dd>
			<% SetMaxLength(this.WtbAuthenticationCode, "@@User.authentication_code.length_max@@"); %>
			<asp:TextBox ID="tbAuthenticationCode" Width="30%" Enabled="false" runat="server" />
			<span class="notes">
				<% if (this.HasAuthenticationCode) { %>
				<asp:Label ID="lbHasAuthentication" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
				<% } %>
				<span><%: GetVerificationCodeNote(this.UserAddrCountryIsoCode) %></span>
				<asp:Label ID="lbAuthenticationMessage" runat="server" />
			</span>
			<br />
			<asp:CustomValidator
				ID="cvAuthenticationCode"
				runat="Server"
				ControlToValidate="tbAuthenticationCode"
				ValidationGroup="UserModifyGlobal"
				ValidateEmptyText="false"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
		</dd>
		<% } %>
		<% } else { %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
			<% if (this.IsUserTel1Necessary) { %>
			<span class="require">※</span>
			<% } %>
		</dt>
		<dd class="tel">
			<p class="attention">
				<% if (this.IsUserTel1Necessary) { %>
				<asp:CustomValidator
					ID="cvUserTel1Global"
					runat="Server"
					ControlToValidate="tbUserTel1Global"
					ValidationGroup="UserModifyGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
			</p>
			<w2c:ExtendedTextBox id="tbUserTel1Global" type="tel" Runat="server" MaxLength="30" CssClass="tel" Width="100%" onchange="resetAuthenticationCodeInput('cvUserTel1Global')" />
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
			<asp:LinkButton
				ID="lbGetAuthenticationCodeGlobal"
				CssClass="btn-add-get"
				Enabled="false"
				runat="server"
				Text="認証コードの取得"
				OnClick="lbGetAuthenticationCode_Click"
				OnClientClick="return checkTelNoInput();" />
			<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
			<% } %>
		</dd>
		<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
		</dt>
		<dd>
			<% SetMaxLength(this.WtbAuthenticationCodeGlobal, "@@User.authentication_code.length_max@@"); %>
			<asp:TextBox ID="tbAuthenticationCodeGlobal" Width="30%" Enabled="false" runat="server" />
			<span class="notes">
				<% if (this.HasAuthenticationCode) { %>
				<asp:Label ID="lbHasAuthenticationGlobal" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
				<% } %>
				<span><%: GetVerificationCodeNote(this.UserAddrCountryIsoCode) %></span>
				<asp:Label ID="lbAuthenticationMessageGlobal" runat="server" />
			</span>
			<br />
			<asp:CustomValidator
				ID="cvAuthenticationCodeGlobal"
				runat="Server"
				ControlToValidate="tbAuthenticationCodeGlobal"
				ValidationGroup="UserModify"
				ValidateEmptyText="false"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
		</dd>
		<% } %>
		<% } %>
		<% if (this.IsUserAddrJp) { %>
		<dt>
			<%: ReplaceTag("@@User.tel2.name@@") %>
		</dt>
		<dd class="tel">
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserTel2_1"
					runat="Server"
					ControlToValidate="tbUserTel1_2"
					ValidationGroup="UserModify"
					ValidateEmptyText="false"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbUserTel1_2" Type="tel" MaxLength="13" style="width:100%;" runat="server" CssClass="shortTel" />
		</dd>
		<% } else { %>
		<dt>
			<%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserTel2Global"
					runat="Server"
					ControlToValidate="tbUserTel2Global"
					ValidationGroup="UserModifyGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbUserTel2Global" type="tel" Runat="server" MaxLength="30" CssClass="tel"></w2c:ExtendedTextBox>
			
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.mail_flg.name@@") %>
		</dt>
		<dd>
			<asp:CheckBox ID="cbUserMailFlg" Text="希望する" runat="server" />
		</dd>
		<% if (this.ExistsUserExtend) { %>
		<dt>
			<%-- ユーザ拡張項目 --%>
			アンケート
		</dt>
		<dd class="extend">
			<uc:BodyUserExtendModify ID="ucBodyUserExtendModify" runat="server" HasInput="true" HasRegist="false" />
		</dd>
		<% } %>
	</dl>
</div>
	<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
	<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
	<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
	<% } %>
	</ContentTemplate>
</asp:UpdatePanel>
<div class="user-unit login" style="display:<%= Constants.RAKUTEN_LOGIN_ENABLED && SessionManager.IsRakutenIdConnectRegisterUser ? "none" : "" %>">
	<h2>ログイン情報</h2>
	<p class="msg">パスワードは変更する場合のみ入力してください。</p>
	<dl class="user-form">
	<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
		<dt>
			<%: ReplaceTag("@@User.login_id.name@@") %><span class="require">※</span>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserLoginId"
					runat="Server"
					ControlToValidate="tbUserLoginId"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbUserLoginId.Attributes["placeholder"] = ReplaceTag("@@User.login_id.name@@"); %>
			<w2c:ExtendedTextBox id="tbUserLoginId" MaxLength="15" Runat="server"></w2c:ExtendedTextBox>
			<p class="msg">※6～12桁(半角英数字）</p>
		</dd>
		<%} %>
		<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
		<%if (this.IsVisible_UserPassword){ %>
		<dt>
			変更前<%: ReplaceTag("@@User.password.name@@") %><span class="require">※</span>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserPasswordBefore"
					runat="Server"
					ControlToValidate="tbUserPasswordBefore"
					ValidationGroup="UserModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbUserPasswordBefore.Attributes["placeholder"] = "変更前" + ReplaceTag("@@User.password.name@@"); %>
			<w2c:ExtendedTextBox id="tbUserPasswordBefore" TextMode="Password" autocomplete="off" MaxLength="15" Runat="server"></w2c:ExtendedTextBox><br />
			<p class="msg">※7～15文字(半角英数字混合）</p>
		</dd>
		<% } %>
		<dt>
			変更後<%: ReplaceTag("@@User.password.name@@") %><span class="require">※</span>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserPassword"
					runat="Server"
					ControlToValidate="tbUserPassword"
					ValidationGroup="UserModify"
					ValidateEmptyText="false"
					SetFocusOnError="true" />
			</p>
			<% tbUserPassword.Attributes["placeholder"] = "変更後" + ReplaceTag("@@User.password.name@@"); %>
			<w2c:ExtendedTextBox id="tbUserPassword" TextMode="Password" autocomplete="off" MaxLength="15" Runat="server"></w2c:ExtendedTextBox><br />
			<p class="msg">※7～15文字(半角英数字混合）</p>
		</dd>
		<dt>
			変更後<%: ReplaceTag("@@User.password.name@@") %>(確認用)<span class="require">※</span>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator
					ID="cvUserPasswordConf"
					runat="Server"
					ControlToValidate="tbUserPasswordConf"
					ValidationGroup="UserModify"
					ValidateEmptyText="false"
					SetFocusOnError="true" />
			</p>
			<% tbUserPasswordConf.Attributes["placeholder"] = "変更後" + ReplaceTag("@@User.password.name@@") + "(確認用)"; %>
			<w2c:ExtendedTextBox id="tbUserPasswordConf" TextMode="Password" autocomplete="off" MaxLength="15" Runat="server"></w2c:ExtendedTextBox><br />
			<p class="msg">※7～15文字(半角英数字混合）</p>
		</dd>
	</dl>
</div>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbConfirm" ValidationGroup="UserModify" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" CssClass="btn">入力内容を確認する</asp:LinkButton>
	</div>
</div>
</section>

<script type="text/javascript">
<!--
	bindEvent();

	changeDropDownDays();

	<%-- UpdataPanelの更新時のみ処理を行う --%>
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			bindEvent();
		}
	}

	<%-- イベントをバインドする --%>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
		bindZipCodeSearch();
		<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
		bindTwAddressSearch();
		<% } %>
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

	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		// Check zip code input on click
		clickSearchZipCodeForSp(
			'<%= this.WtbUserZip.ClientID %>',
			'<%= this.WtbUserZip1.ClientID %>',
			'<%= this.WtbUserZip2.ClientID %>',
			'<%= this.WlbSearchAddr.ClientID %>',
			'<%= this.WlbSearchAddr.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>');

		// Check zip code input on text box change
		textboxChangeSearchZipCodeForSp(
			'<%= this.WtbUserZip.ClientID %>',
			'<%= this.WtbUserZip1.ClientID %>',
			'<%= this.WtbUserZip2.ClientID %>',
			'<%= this.WtbUserZip1.UniqueID %>',
			'<%= this.WtbUserZip2.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage');

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search zip global
		textboxChangeSearchGlobalZip(
			'<%= this.WtbUserZipGlobal.ClientID %>',
			'<%= this.WlbSearchAddrFromZipGlobal.UniqueID %>');
		<% } %>
	}

	$(document).on('click', '.search-result-layer-close', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function () {
		bindSelectedAddr($('li.search-result-layer-addr').index(this));
	});

	<%-- 複数住所検索結果からの選択値を入力フォームにバインドする --%>
	function bindSelectedAddr(selectedIndex) {
		var selectedAddr = $('.search-result-layer-addrs li').eq(selectedIndex);
		$("#<%= ddlUserAddr1.ClientID %>").val(selectedAddr.find('.addr').text());
		$("#<%= tbUserAddr2.ClientID %>").val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
		$("#<%= tbUserAddr3.ClientID %>").focus();

		closePopupAndLayer();
	}

	<%-- 日付リスト変更 --%>
	function changeDropDownDays() {
		var year = $("#<%= ddlUserBirthYear.ClientID %>").val();
		var month = $("#<%= ddlUserBirthMonth.ClientID %>").val();

		var select = document.getElementById('<%= ddlUserBirthDay.ClientID %>');

		var maxDay = new Date(year, month, 0).getDate();

		var optionsList = document.getElementById('<%= ddlUserBirthDay.ClientID %>').options;
		var daysCount = parseInt(optionsList[optionsList.length - 1].value);

		if (daysCount < maxDay) {
			for (var day = 0; day < (maxDay - daysCount) ; day++) {
				var appendDay = daysCount + day + 1;
				var option = document.createElement('option');
				option.setAttribute('value', appendDay);
				option.innerHTML = appendDay;
				select.appendChild(option);
			}
		} else {
			for (var dayDifference = 0; dayDifference < daysCount - maxDay; dayDifference++) {
				document.getElementById('<%= ddlUserBirthDay.ClientID %>').remove(daysCount - dayDifference);
			}
		}
	}

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		$('#<%= this.WddlUserAddr3.ClientID %>').change(function (e) {
			$('#<%= this.WtbUserZipGlobal.ClientID %>').val(
				$('#<%= this.WddlUserAddr3.ClientID %>').val().split('|')[0]);
		});
	}
	<% } %>

	<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
	// Set authentication message
	var setIntervalId;
	function setAuthenticationMessage() {
		var isUserAddrJp = true;
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		isUserAddrJp = document.getElementById('<%= this.WddlUserCountry.ClientID %>').value == '<%= Constants.COUNTRY_ISO_CODE_JP %>';
		<% } %>
		var authenticationStatusId = isUserAddrJp
			? '<%= this.WlbAuthenticationStatus.ClientID %>'
			: '<%= this.WlbAuthenticationStatusGlobal.ClientID %>';
		var authenticationMessageId = isUserAddrJp
			? '<%= this.WlbAuthenticationMessage.ClientID %>'
			: '<%= this.WlbAuthenticationMessageGlobal.ClientID %>';
		var phoneNumber = document.getElementById(isUserAddrJp ? '<%= this.WtbUserTel1_1.ClientID %>' : '<%= this.WtbUserTel1Global.ClientID %>').value;

		setIntervalAuthenticationMessage(
			authenticationStatusId,
			authenticationMessageId,
			phoneNumber,
			'<%= Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_CODE_DIGITS %>',
			'<%= Constants.PERSONAL_AUTHENTICATION_OF_USERR_EGISTRATION_AUTH_CODE_EXPIRATION_TIME %>')
	}

	// Check tel no input
	function checkTelNoInput() {
		var result = checkTelNo(
			'<%= this.WtbUserTel1.ClientID %>',
			'<%= this.WtbUserTel2.ClientID %>',
			'<%= this.WtbUserTel3.ClientID %>',
			'<%= this.WtbUserTel1_1.ClientID %>',
			'<%= this.WtbUserTel1Global.ClientID %>');
		return result;
	}
	<% } %>
//-->
</script>

</asp:Content>