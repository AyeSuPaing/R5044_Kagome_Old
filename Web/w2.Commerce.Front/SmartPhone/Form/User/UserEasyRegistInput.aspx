<%--
=========================================================================================================
  Module      : スマートフォン用かんたん会員登録入力画面(UserEasyRegistInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserEasyRegistInput.aspx.cs" Inherits="Form_User_UserEasyRegistInput" Title="かんたん会員新規登録入力ページ" %>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<section class="wrap-order user-regist-input">
<%-- お客様情報入力フォーム --%>
<div class="user-unit">
	<% if ((Constants.SOCIAL_LOGIN_ENABLED || Constants.AMAZON_LOGIN_OPTION_ENABLED || Constants.PAYPAL_LOGINPAYMENT_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) && (SessionManager.SocialLogin == null)) { %>
	<%-- ソーシャルログイン連携 --%>
	<div style="margin-bottom:2em;">
		<h2>ソーシャルログイン連携</h2>

		<ul style="list-style-type:none;text-align:center;padding:0px;margin:1em 0 0 0;">
			<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
				<%-- Apple --%>
				<li class="social-login-margin">
					<a class="social-login apple-color"
						href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
							w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
							Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
							Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
							true,
							Request.Url.Authority) %>">
						<div class="social-icon-width-apple">
							<img class="apple-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_apple.png" />
						</div>
						<div class="apple-label">Appleでサインイン</div>
					</a>
				</li>
				<%-- Facebook --%>
				<li class="social-login-margin">
					<a class="social-login facebook-color"
						href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								true,
								Request.Url.Authority) %>">
						<div class="social-icon-width">
							<img class="facebook-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_facebook.png" />
						</div>
						<div class="social-login-label">Facebookでログイン</div>
					</a>
				</li>
				<%-- Twitter --%>
				<li class="social-login-margin">
					<a class="social-login twitter-color"
						href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								true,
								Request.Url.Authority) %>">
						<div class="social-icon-width">
							<img class="twitter-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_x.png" />
						</div>
						<div class="twitter-label-registinput">Xでログイン</div>
					</a>
				</li>
				<%-- Yahoo --%>
				<li class="social-login-margin">
					<a class="social-login yahoo-color"
						href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								true,
								Request.Url.Authority) %>">
						<div class="social-icon-width">
							<img class="yahoo-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_yahoo.png" />
						</div>
						<div class="social-login-label">Yahoo! JAPAN IDでログイン</div>
					</a>
				</li>
				<%-- Google --%>
				<li class="social-login-margin">
					<a class="social-login google-color"
						href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								true,
								Request.Url.Authority) %>">
						<div class="social-icon-width">
							<img class="google-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_google.png" />
						</div>
						<div class="google-label">Sign in with Google</div>
					</a>
				</li>
			<% } %>
			<% if (Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
				<%-- LINE --%>
				<li class="social-login-margin">
					<div class="social-login line-color">
						<div class="social-login line-hover-color line-active-color">
							<a href="<%=LineConnect(
								w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION
									? Constants.PAGE_FRONT_DEFAULT
									: Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
								true,
								Request.Url.Authority) %>">
								<div class="social-icon-width">
									<img class="line-icon"
										src="<%= Constants.PATH_ROOT %>
										Contents\ImagesPkg\socialLogin\logo_line.png" />
								</div>
								<div class="social-login-label">LINEでログイン</div>
							</a>
						</div>
					</div>
					<p style="margin-top:5px">※LINE連携時に友だち追加します</p>
				</li>
			<% } %>
			<%-- AnazyonPay --%>
			<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED) { %>
				<li style="padding-bottom: 10px;">
					<%-- ▼▼Amazonログイン連携ボタンウィジェット▼▼ --%>
					<div id="AmazonLoginButton"></div>
					<div style="width: 296px; display: inline-block;">
						<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
						<div id="AmazonLoginCv2Button"></div>
						<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
					</div>
					<%-- ▲▲Amazonログイン連携ボタンウィジェット▲▲ --%>
				</li>
			<% } %>
			<%-- PayPal --%>
			<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) { %>
				<li class="social-login-margin">
					<%-- ▼PayPalログインここから▼ --%>
					<%
						ucPaypalScriptsForm.LogoDesign = "Login";
						ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
						ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
					%>
					<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
					<div id="paypal-button" style="width: 296px; margin: auto;"></div>
					<% if (SessionManager.PayPalCooperationInfo != null) {%>
						<p style="margin-top:3px">※<%: SessionManager.PayPalCooperationInfo.AccountEMail %> 連携済</p>
					<% } else {%>
						<p style="margin-top:3px">※PayPalで新規登録/ログインします</p>
					<%} %>
					<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click" />
					<%-- ▲PayPalログインここまで▲ --%>
				</li>
			<% } %>
		</ul>
	</div>
	<% } %>

	<asp:UpdatePanel runat="server">
		<ContentTemplate>
	<h2>会員登録フォーム</h2>
		<dl class="user-form">
			<% if (this.IsVisible_UserName) { %>
			<dt>
				<%-- 氏名 --%>
				<%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span><span id="efo_sign_name"/>
			</dt>
			<dd class="name">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserName1"
					runat="server"
					ControlToValidate="tbUserName1"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserName2"
					runat="server"
					ControlToValidate="tbUserName2"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
				<% tbUserName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserName1" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserName2" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			<% if (this.IsVisible_UserNameKana) { %>
			<dt>
				<%-- 氏名（かな） --%>
				<%: ReplaceTag("@@User.name_kana.name@@") %><span class="require">※</span><span id="efo_sign_kana"/>
			</dt>
			<dd class="<%= ReplaceTag("@@User.name_kana.type@@") %>">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserNameKana1"
					runat="server"
					ControlToValidate="tbUserNameKana1"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserNameKana2"
					runat="server"
					ControlToValidate="tbUserNameKana2"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name_kana1.name@@"); %>
				<% tbUserNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name_kana2.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserNameKana1" Runat="server" maxlength="20"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserNameKana2" Runat="server"  maxlength="20"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			<% if (Constants.PRODUCTREVIEW_ENABLED) { %>
			<% if (this.IsVisible_UserNickName) { %>
			<dt>
				<%-- ニックネーム --%>
				<%: ReplaceTag("@@User.nickname.name@@") %>
			</dt>
			<dd class="nickname">
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbUserNickName"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox id="tbUserNickName" autocapitalize="OFF" runat="server" MaxLength="20"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			<% } %>
			<% if (this.IsVisible_UserBirth) { %>
			<dt>
				<%-- 生年月日 --%>
				<%: ReplaceTag("@@User.birth.name@@") %><span class="require">※</span><span id="efo_sign_birth"/>
			</dt>
			<dd class="birth">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserBirthYear"
					runat="Server"
					ControlToValidate="ddlUserBirthYear"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserBirthMonth"
					runat="Server"
					ControlToValidate="ddlUserBirthMonth"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserBirthDay"
					runat="Server"
					ControlToValidate="ddlUserBirthDay"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				</p>
				<asp:DropDownList id="ddlUserBirthYear" runat="server" CssClass="year" onchange="changeDropDownDays()"></asp:DropDownList>年
				<asp:DropDownList id="ddlUserBirthMonth" runat="server" CssClass="month" onchange="changeDropDownDays()"></asp:DropDownList>月
				<asp:DropDownList id="ddlUserBirthDay" runat="server" CssClass="date"></asp:DropDownList>日
			</dd>
			<% } %>
			<% if (this.IsVisible_UserSex) { %>
			<dt>
				<%-- 性別 --%>
				<%: ReplaceTag("@@User.sex.name@@") %><span class="require">※</span><span id="efo_sign_sex"/>
			</dt>
			<dd class="sex">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserSex"
					runat="server"
					ValidationGroup="UserEasyRegist"
					ControlToValidate="rblUserSex"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				</p>
				<asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" CellSpacing="0" RepeatLayout="Flow"></asp:RadioButtonList>
			</dd>
			<% } %>
			<dt>
				<%: ReplaceTag("@@User.mail_addr.name@@") %><span class="require">※</span><span id="efo_sign_mail_addr"/>
			</dt>
			<dd class="mail">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserMailAddrForCheck"
					runat="Server"
					ControlToValidate="tbUserMailAddr"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserMailAddr"
					runat="Server"
					ValidationGroup="UserEasyRegist"
					ControlToValidate="tbUserMailAddr"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserMailAddrConf"
					runat="server"
					ControlToValidate="tbUserMailAddrConf"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<p class="msg">お手数ですが、確認のため２度入力してください</p>
				<w2c:ExtendedTextBox id="tbUserMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserMailAddrConf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
					※ログイン時に利用します。
				<% } %>
			</dd>
			<% if (this.IsVisible_UserMailAddr2 && this.IsVisible_UserMailAddr2Conf && (Constants.CROSS_POINT_OPTION_ENABLED == false)) { %>
			<dt>
				<%: ReplaceTag("@@User.mail_addr2.name@@") %><span class="require" style="display:<%= (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) ? "" : "none" %>">※</span>
			</dt>
			<dd class="mobile">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserMailAddr2"
					runat="server"
					ValidationGroup="UserEasyRegist"
					ControlToValidate="tbUserMailAddr2"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserMailAddr2Conf"
					runat="server"
					ControlToValidate="tbUserMailAddr2Conf"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<p class="msg">お手数ですが、確認のため２度入力してください</p>
				<w2c:ExtendedTextBox id="tbUserMailAddr2" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserMailAddr2Conf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
					※ログイン時に利用します。
				<% } %>
			</dd>
			<% } %>
			<% if (Constants.GLOBAL_OPTION_ENABLE && this.IsVisible_UserCountry) { %>
			<dt>
				<%: ReplaceTag("@@User.country.name@@") %><span class="require">※</span>
			</dt>
			<dd class="address">
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="ddlUserCountry"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<asp:DropDownList id="ddlUserCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUserAddrCountry_SelectedIndexChanged" /><br/>
				<span id="countryAlertMessage" class="msg" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
			</dd>
			<% } %>
			<% if (Constants.GLOBAL_OPTION_ENABLE == false) { %>
			<% if (this.IsVisible_AmazonAddressWidget) { %>
				<dt>
					<%: ReplaceTag("@@User.addr.name@@") %>
				</dt>
				<dd>
					<%--▼▼Amazonアドレス帳ウィジェット▼▼--%>
					<div id="addressBookWidgetDiv"></div>
					<div id="addressBookErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
					<%--▲▲Amazonアドレス帳ウィジェット▲▲--%>
					<%--▼▼AmazonリファレンスID格納▼▼--%>
					<asp:HiddenField runat="server" ID="hfAmazonOrderRefID" />
					<%--▲▲AmazonリファレンスID格納▲▲--%>
				</dd>
			<% } else { %>
			<% if (this.IsVisible_UserZip) { %>
			<dt>
				<%-- 郵便番号 --%>
				<%: ReplaceTag("@@User.zip.name@@") %><span class="require">※</span><span id="efo_sign_zip"/>
			</dt>
			<dd class="zip">
				<p class="attention" id="addrSearchErrorMessage">
				<asp:CustomValidator
					ID="cvUserZip1"
					runat="server"
					ValidationGroup="UserEasyRegist"
					ControlToValidate="tbUserZip"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
					<%: this.ZipInputErrorMessage %>
				</p>
				<w2c:ExtendedTextBox ID="tbUserZip" Type="tel" MaxLength="8" OnTextChanged="lbSearchAddr_Click" runat="server" />
				<br />
				<asp:LinkButton ID="lbSearchAddr" runat="server" OnClick="lbSearchAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">郵便番号から住所を探す</asp:LinkButton>
			</dd>
			<% } %>
			<% if (this.IsVisible_UserAddr1 || this.IsVisible_UserAddr2 || this.IsVisible_UserAddr3 || (Constants.DISPLAY_ADDR4_ENABLED && this.IsVisible_UserAddr4)) { %>
			<dt>
				<%: ReplaceTag("@@User.addr.name@@") %>
				<span class="require">※</span><span id="efo_sign_ship_address"/>
			</dt>
			<dd class="address">
				<p class="attention">
				<% if (this.IsVisible_UserAddr1) { %>
				<asp:CustomValidator
					ID="cvUserAddr1"
					runat="server"
					ControlToValidate="ddlUserAddr1"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
				<% if (this.IsVisible_UserAddr2) { %>
				<asp:CustomValidator
					ID="cvUserAddr2"
					runat="server"
					ControlToValidate="tbUserAddr2"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
				<% if (this.IsVisible_UserAddr3) { %>
				<asp:CustomValidator
					ID="cvUserAddr3"
					runat="server"
					ControlToValidate="tbUserAddr3"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
				<% if (Constants.DISPLAY_ADDR4_ENABLED && this.IsVisible_UserAddr4) { %>
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbUserAddr4"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% } %>
				<% if (this.IsVisible_UserAddr1) { %>
				<asp:DropDownList id="ddlUserAddr1" runat="server"></asp:DropDownList><br />
				<% } %>
				<% if (this.IsVisible_UserAddr2) { %>
				<%-- 市区町村 --%>
				<w2c:ExtendedTextBox id="tbUserAddr2" placeholder='市区町村' Runat="server" MaxLength="40"></w2c:ExtendedTextBox><br />
				<% } %>
				<% if (this.IsVisible_UserAddr3) { %>
				<%-- 番地 --%>
				<w2c:ExtendedTextBox id="tbUserAddr3" placeholder='番地' Runat="server" MaxLength="40"></w2c:ExtendedTextBox><br />
				<% } %>
				<% if (Constants.DISPLAY_ADDR4_ENABLED && this.IsVisible_UserAddr4) { %>
				<%-- ビル・マンション名 --%>
				<w2c:ExtendedTextBox id="tbUserAddr4" placeholder='建物名' Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
				<% } %>
			</dd>
			<% } %>
			<% } %>
			<% } %>
			<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<% if (this.IsVisible_UserCompanyName) { %>
			<dt>
				<%-- 企業名 --%>
				<%: ReplaceTag("@@User.company_name.name@@") %>
			</dt>
			<dd class="company-name">
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbUserCompanyName"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserCompanyName.Attributes["placeholder"] = ReplaceTag("@@User.company_name.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserCompanyName" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			<% if (this.IsVisible_UserCompanyPostName) { %>
			<dt>
				<%-- 部署名 --%>
				<%: ReplaceTag("@@User.company_post_name.name@@") %>
			</dt>
			<dd class="company-post">
				<p class="attention">
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbUserCompanyPostName"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserCompanyPostName.Attributes["placeholder"] = ReplaceTag("@@User.company_post_name.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserCompanyPostName" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			<% } %>
			<% if (this.IsVisible_UserTel1) { %>
			<dt>
				<%-- 電話番号 --%>
				<%: ReplaceTag("@@User.tel1.name@@") %><span class="require">※</span><span id="efo_sign_tel1"/>
			</dt>
			<dd class="tel">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserTel1"
					runat="server"
					ControlToValidate="tbUserTel1_1"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox id="tbUserTel1_1" style="width:100%;" Runat="server" MaxLength="13" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvUserTel1')" />
				<% if (this.IsDisplayPersonalAuthentication) { %>
				<asp:LinkButton
					ID="lbGetAuthenticationCode"
					CssClass="btn-add-get"
					runat="server"
					Text="認証コードの取得"
					OnClick="lbGetAuthenticationCode_Click"
					OnClientClick="return checkTelNoInput();" />
				<asp:Label ID="lbAuthenticationStatus" runat="server" />
				<% } %>
			</dd>
			<% if (this.IsDisplayPersonalAuthentication) { %>
			<dt>
				<%: ReplaceTag("@@User.authentication_code.name@@") %><span class="require">※</span>
			</dt>
			<dd>
				<% SetMaxLength(this.WtbAuthenticationCode, "@@User.authentication_code.length_max@@"); %>
				<asp:TextBox ID="tbAuthenticationCode" Width="30%" Enabled="false" runat="server" />
				<span class="notes">
					<% if (this.HasAuthenticationCode) { %>
					<asp:Label ID="lbHasAuthentication" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
					<% } %>
					<span><%: GetVerificationCodeNote() %></span>
					<asp:Label ID="lbAuthenticationMessage" runat="server" />
				</span>
				<br />
				<asp:CustomValidator
					ID="cvAuthenticationCode"
					runat="Server"
					ControlToValidate="tbAuthenticationCode"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="false"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
			</dd>
			<% } %>
			<% } %>
			<% if (this.IsVisible_UserTel2) { %>
			<dt>
				<%: ReplaceTag("@@User.tel2.name@@") %>
			</dt>
			<dd class="tel">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserTel2_1"
					runat="server"
					ControlToValidate="tbUserTel1_2"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="false"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbUserTel1_2" Type="tel" MaxLength="13" style="width:100%;" runat="server" CssClass="shortTel" />
			</dd>
			<% } %>
			<% if (this.IsVisible_UserMailFlg) { %>
			<dt>
				<%: ReplaceTag("@@User.mail_flg.name@@") %>
			</dt>
			<dd>
				<asp:CheckBox ID="cbUserMailFlg" Text="希望する" runat="server" />
			</dd>
			<% } %>
			<% if (this.IsDisplayPersonalAuthentication) { %>
			<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
			<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
			<% } %>
			<%-- ソーシャルログイン用 --%>
			<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
			<asp:HiddenField ID="hfSocialLoginJson" runat="server" />
			<% } %>
		</dl>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>

<%if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) || this.IsVisible_UserPassword) { %>
<div class="user-unit login">
	<h2>ログイン情報</h2>
		<dl class="user-form">
			<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
			<dt>
				<%: ReplaceTag("@@User.login_id.name@@") %>
				<span class="require">※</span><span id="efo_sign_login_id"/>
			</dt>
			<dd>
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserLoginId"
					runat="Server"
					ControlToValidate="tbUserLoginId"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserLoginId.Attributes["placeholder"] = ReplaceTag("@@User.login_id.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserLoginId" Type="email" MaxLength="15" Runat="server"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
			<%if (this.IsVisible_UserPassword){ %>
			<dt>
				<%: ReplaceTag("@@User.password.name@@") %>
				<span class="require">※</span><span id="efo_sign_password"/>
			</dt>
			<dd>
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserPassword"
					runat="Server"
					ControlToValidate="tbUserPassword"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserPasswordConf"
					runat="Server"
					ControlToValidate="tbUserPasswordConf"
					ValidationGroup="UserEasyRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<p class="msg">※半角英数字混合7文字〜15文字以内でお願いいたします</p>
				<w2c:ExtendedTextBox id="tbUserPassword" TextMode="Password" autocomplete="off" MaxLength="15" Runat="server"></w2c:ExtendedTextBox>
				<p class="msg">※お手数ですが、確認のため２度入力してください</p>
				<w2c:ExtendedTextBox id="tbUserPasswordConf" TextMode="Password" autocomplete="off" MaxLength="15" Runat="server"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
		</dl>
</div>
<% } %>

<div class="user-unit">
	<h2>会員規約</h2>
	<div style="margin:1em auto;padding:0px;border-style:solid;border-width:1px;border-color:#ccc;overflow-x:hidden;overflow:auto;height:150px;">
		<uc:UserRegistRegulationMessage runat="server" />
	</div>
	<div style="text-align:center;">
		<asp:CheckBox ID="cbUserAcceptedRegulation" Text="会員規約に同意する" runat="server" />
		<div>
			<asp:CustomValidator
				ID="cvUserAcceptedRegulation"
				runat="server"
				ValidationGroup="UserEasyRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidationUserAcceptedRegulation"
				ErrorMessage="会員規約に同意いただけない場合は、ご登録することができません。"
				CssClass="error_inline" />
		</div>
	</div>
</div>
<%-- キャプチャ認証 --%>
<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbRegister.ClientID %>" /><br />
<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbRegister" ValidationGroup="UserEasyRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbRegister_Click"  CssClass="btn">
		登録する
		</asp:LinkButton>
	</div>
</div>
</section>

<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/SocialPlusInputCompletion.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
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
		<% var serializer = new JavaScriptSerializer(); %>
		<% if (this.IsEfoOptionEnabled) { %>
		var customValidatorControlDisabledInformationList = <%= serializer.Serialize(this.CustomValidatorControlDisabledInformationList) %>
		bindRemoveCustomValidateErrorOnInputChangeValue(customValidatorControlDisabledInformationList);
		<% } else { %>
		var customValidatorControlInformationList = <%= serializer.Serialize(this.CustomValidatorControlInformationList) %>
		bindRemoveCustomValidateErrorWhenNoErrorDisplay(customValidatorControlInformationList);
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

		if (select != null) {
			var maxDay = new Date(year, month, 0).getDate();

			var optionsList = document.getElementById('<%= ddlUserBirthDay.ClientID %>').options;
			var daysCount = parseInt(optionsList[optionsList.length - 1].value);

			if (daysCount < maxDay) {
				for (var day = 0; day < (maxDay - daysCount); day++) {
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
	}

	<%-- ソーシャルログイン用 --%>
	<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
		var selectors = {
			mailAddress: '#<%= this.WtbUserMailAddr.ClientID %>',
			mailAddressConf: '#<%= this.WtbUserMailAddrConf.ClientID %>',
			tell1: '#<%= this.WtbUserTel1.ClientID %>',
			tell2: '#<%= this.WtbUserTel2.ClientID %>',
			tell3: '#<%= this.WtbUserTel3.ClientID %>',
			name1: '#<%= this.WtbUserName1.ClientID %>',
			name2: '#<%= this.WtbUserName2.ClientID %>',
			nameKanaCheck: '<%= ReplaceTag("@@User.name_kana.type@@") %>',
			nameKana1: '#<%= this.WtbUserNameKana1.ClientID %>',
			nameKana2: '#<%= this.WtbUserNameKana2.ClientID %>',
			productreviewEnabled: '<%= (Constants.SOCIAL_LOGIN_ENABLED)%>' ? true : false,
			nickName: '#<%= this.WtbUserNickName.ClientID %>',
			birthYear: '#<%= this.WddlUserBirthYear.ClientID %>',
			birthMonth: '#<%= this.WddlUserBirthMonth.ClientID %>',
			birthDay: '#<%= this.WddlUserBirthDay.ClientID %>',
			sex: '#<%= this.WrblUserSex.ClientID %> input:radio',
			zip1: '#<%= this.WtbUserZip1.ClientID %>',
			zip2: '#<%= this.WtbUserZip2.ClientID %>',
			address1: '#<%= this.WddlUserAddr1.ClientID %>',
			address2: '#<%= this.WtbUserAddr2.ClientID %>',
			address3: '#<%= this.WtbUserAddr3.ClientID %>',
			zip: '#<%= this.WtbUserZip.ClientID %>',
			tell: '#<%= this.WtbUserTel1_1.ClientID %>'
		}

		var data = $('#<%= this.WhfSocialLoginJson.ClientID %>').val();
		var json = data ? JSON.parse(data) : {};
		SocialPlusInputCompletion(json, selectors);
	<% } %>

	<%-- 会員規約に同意のクライアントチェック --%>
	function ClientValidationUserAcceptedRegulation(sender, e) {
		e.IsValid = $("#<%= this.WcbUserAcceptedRegulation.ClientID %>").is(':checked');
	}

	<% if (this.IsDisplayPersonalAuthentication) { %>
	// Set authentication message
	function setAuthenticationMessage() {
		var phoneNumber = document.getElementById('<%= this.WtbUserTel1_1.ClientID %>').value;
		setIntervalAuthenticationMessage(
			'<%= this.WlbAuthenticationStatus.ClientID %>',
			'<%= this.WlbAuthenticationMessage.ClientID %>',
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
			'');
		return result;
	}
	<% } %>
	//-->
</script>
<%--▼▼Amazonウィジェット用スクリプト▼▼--%>
<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED) { %>
	<% if(Constants.AMAZON_PAYMENT_CV2_ENABLED){ %>
	<%--▼▼ Amazon(CV2)スクリプト ▼▼--%>
	<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
	<script type="text/javascript" charset="utf-8">
		showAmazonSignInCv2Button(
			'#AmazonLoginCv2Button',
			'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
			<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
			'<%= this.AmazonRequest.Payload %>',
			'<%= this.AmazonRequest.Signature %>',
			'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>')
	</script>
	<%-- ▲▲Amazon(CV2)スクリプト▲▲ --%>
	<% } else { %>
	<script type="text/javascript">
		window.onAmazonLoginReady = function () {
			amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
			amazon.Login.setUseCookie(true);
		};
		window.onAmazonPaymentsReady = function () {
			if ($('#AmazonLoginButton').length) showButton();
			if ($('#addressBookWidgetDiv').length) showAddressBookWidget();
		};

		<%--Amazonアドレス帳表示ウィジェット--%>
		function showAddressBookWidget() {
			new OffAmazonPayments.Widgets.AddressBook({
				sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
				onOrderReferenceCreate: function (orderReference) {
					var orderReferenceId = orderReference.getAmazonOrderReferenceId();
					$('#<%= this.WhfAmazonOrderRefID.ClientID%>').val(orderReferenceId);
				},
				onAddressSelect: function (orderReference) {
					var $addressBookErrorMessage = $('#addressBookErrorMessage');
					$addressBookErrorMessage.empty();
					getAmazonAddress(function (response) {
						var data = JSON.parse(response.d);
						if (data.Error) {
							$addressBookErrorMessage.html(data.Error);
							return;
						}
						$("#<%= this.WtbUserTel1.ClientID %>").val(data.Input.Phone1);
						$("#<%= this.WtbUserTel2.ClientID %>").val(data.Input.Phone2);
						$("#<%= this.WtbUserTel3.ClientID %>").val(data.Input.Phone3);
					});
				},
				design: { designMode: 'smartphoneCollapsible' },
				onError: function (error) {
					alert(error.getErrorMessage());
					location.href = "<%=Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN %>";
				}
			}).bind("addressBookWidgetDiv");
		}

		<%--Amazonボタン表示ウィジェット--%>
		function showButton() {
			var authRequest;
			OffAmazonPayments.Button("AmazonLoginButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
				type: "LwA",
				color: "Gold",
				size: "large",
				authorization: function () {
					loginOptions = { scope: "payments:shipping_address payments:widget profile", popup: false };
					authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_USER_EASY_REGISTER_CALLBACK) %>");
				},
				onError: function (error) {
					alert(error.getErrorMessage());
					location.href = "<%=Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN %>";
				}
			});
			$('#OffAmazonPaymentsWidgets0').css({ 'height': '44px', 'width': '221px' });
		}

		<%--Amazon住所取得関数--%>
		function getAmazonAddress(callback) {
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT%>/GetAmazonAddress",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({
					amazonOrderReferenceId: $('#<%= this.WhfAmazonOrderRefID.ClientID %>').val()
				}),
				success: callback
			});
		}

	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
	<% } %>
	<% } %>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>

</asp:Content>
