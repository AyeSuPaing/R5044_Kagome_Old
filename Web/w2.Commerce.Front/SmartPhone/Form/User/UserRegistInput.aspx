<%--
=========================================================================================================
  Module      : スマートフォン用会員登録入力画面(UserRegistInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendRegist" Src="~/SmartPhone/Form/Common/User/BodyUserExtendRegist.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistInput.aspx.cs" Inherits="Form_User_UserRegistInput" Title="会員新規登録入力ページ" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order user-regist-input">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<%-- ソーシャルログイン連携（連携ごとに条件を追加） --%>
<% if ((Constants.SOCIAL_LOGIN_ENABLED || (Constants.RAKUTEN_LOGIN_ENABLED && (this.IsAfterOrder == false)) || Constants.PAYPAL_LOGINPAYMENT_ENABLED || (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED) || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) && (SessionManager.SocialLogin == null) && (SessionManager.HasTemporaryUserId == false)) { %>
<div class="order-unit">
	<h2>ソーシャルログイン連携</h2>
	<ul style="list-style-type:none;text-align:center;padding:0px;margin:1em 0 0 0;">
		<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
			<%-- Apple --%>
			<li class="social-login-margin">
				<a class="social-login apple-color"
					href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
						w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
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
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
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
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
						true,
						Request.Url.Authority) %>">
					<div class="social-icon-width">
						<img class="twitter-icon"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_x.png" />
					</div>
					<div class="twitter-label">Xでログイン</div>
				</a>
			</li>
			<%-- Yahoo --%>
			<li class="social-login-margin">
				<a class="social-login yahoo-color"
					href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
						w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
						Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
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
									Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
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
		<% if ((Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) && String.IsNullOrEmpty(SessionManager.LineProviderUserId)) { %>
			<%-- LINE --%>
			<li class="social-login-margin">
				<div class="social-login line-color">
					<div class="social-login line-active-color">
						<a href="<%= LineConnect(
							w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION
								? Constants.PAGE_FRONT_DEFAULT
								: Constants.PAGE_FRONT_USER_REGIST_INPUT,
							Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
							Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
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
				<p style="margin-top:3px">※LINE連携時に友だち追加します</p>
			</li>
		<% } %>
		<%-- ▼PayPalログインここから▼ --%>
		<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) { %>
				<li class="social-login-margin">
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
			</li>
		<% } %>
		<%-- ▲PayPalログインここまで▲ --%>
		<%-- ▼楽天Connect▼ --%>
		<% if (Constants.RAKUTEN_LOGIN_ENABLED && (this.IsAfterOrder == false)) { %>
			<li class="social-login-margin">
				<p style="text-align: center;">
					<asp:LinkButton runat="server" ID="lbRakutenIdConnectRequestAuth" OnClick="lbRakutenIdConnectRequestAuth_OnClick">
						<img src="https://static.id.rakuten.co.jp/static/btn-japanese-2x/idconnect_02-new_r@2x.png"
							style="width: 296px; height: 40px" />
					</asp:LinkButton>
					<% if (this.IsRakutenIdConnectUserRegister) { %>
						<br/>※<%: SessionManager.RakutenIdConnectActionInfo.RakutenIdConnectUserInfoResponseData.Email %> 連携済<br/>
					<% } %>
				</p>
			</li>
		<% } %>
		<%-- ▲楽天Connect▲ --%>
		<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
		<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
			<li class="social-login-margin">
				<div style="width: 296px; display: inline-block;">
					<div id="AmazonLoginCv2Button"></div>
				</div>
			</li>
		<% } %>
		<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
	</ul>
</div>
<% } %>

<asp:UpdatePanel ID="upUpdatePanel" runat="server">
	<ContentTemplate>
	<%-- お客様情報入力フォーム --%>
	<div class="order-unit">
	<h2>会員登録フォーム</h2>
		<dl class="order-form">
			<dt>
				<%-- 氏名 --%>	
				<%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span><span id="efo_sign_name"/>
			</dt>
			<dd class="name">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserName1"
					runat="Server"
					ControlToValidate="tbUserName1"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserName2"
					runat="Server"
					ControlToValidate="tbUserName2"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
				<% tbUserName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserName1" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserName2" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
			</dd>
			<% if (this.IsUserAddrJp) { %>
			<dt>
				<%-- 氏名（かな） --%>
				<%: ReplaceTag("@@User.name_kana.name@@") %><span class="require">※</span><span id="efo_sign_kana"/>
			</dt>
			<dd class="<%= ReplaceTag("@@User.name_kana.type@@") %>">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserNameKana1"
					runat="Server"
					ControlToValidate="tbUserNameKana1"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserNameKana2"
					runat="Server"
					ControlToValidate="tbUserNameKana2"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
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
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox id="tbUserNickName" autocapitalize="OFF" runat="server" MaxLength="20"></w2c:ExtendedTextBox>
			</dd>
			<%} %>
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
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserBirthMonth"
					runat="Server"
					ControlToValidate="ddlUserBirthMonth"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserBirthDay"
					runat="Server"
					ControlToValidate="ddlUserBirthDay"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				</p>
				<asp:DropDownList id="ddlUserBirthYear" runat="server" CssClass="year" onchange="changeDropDownDays()"></asp:DropDownList>年
				<asp:DropDownList id="ddlUserBirthMonth" runat="server" CssClass="month" onchange="changeDropDownDays()"></asp:DropDownList>月
				<asp:DropDownList id="ddlUserBirthDay" runat="server" CssClass="date"></asp:DropDownList>日
			</dd>

			<dt>
				<%-- 性別 --%>
				<%: ReplaceTag("@@User.sex.name@@") %><span class="require">※</span><span id="efo_sign_sex"/>
			</dt>
			<dd class="sex">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserSex"
					runat="Server"
					ValidationGroup="UserRegist"
					ControlToValidate="rblUserSex"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false" />
				</p>
				<asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" CellSpacing="0" RepeatLayout="Flow"></asp:RadioButtonList>
			</dd>
		
			<dt>
				<%: ReplaceTag("@@User.mail_addr.name@@") %>
				<span class="require">※</span><span id="efo_sign_mail_addr"/>
			</dt>
			<dd class="mail">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserMailAddrForCheck"
					runat="Server"
					ControlToValidate="tbUserMailAddr"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				<asp:CustomValidator
					ID="cvUserMailAddr"
					runat="Server"
					ValidationGroup="UserRegist"
					ControlToValidate="tbUserMailAddr"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserMailAddrConf"
					runat="Server"
					ControlToValidate="tbUserMailAddrConf"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<p class="msg">お手数ですが、確認のため２度入力してください</p>
				<w2c:ExtendedTextBox id="tbUserMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserMailAddrConf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
					※ログイン時に利用します。
				<%} %>
			</dd>
			<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
			<dt>
				<%: ReplaceTag("@@User.mail_addr2.name@@") %><span class="require" style="display:<%= (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) ? "" : "none" %>">※</span>
			</dt>
			<dd class="mobile">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserMailAddr2"
					runat="Server"
					ValidationGroup="UserRegist"
					ControlToValidate="tbUserMailAddr2"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserMailAddr2Conf"
					runat="Server"
					ControlToValidate="tbUserMailAddr2Conf"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<p class="msg">お手数ですが、確認のため２度入力してください</p>
				<w2c:ExtendedTextBox id="tbUserMailAddr2" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox id="tbUserMailAddr2Conf" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
					※ログイン時に利用します。
				<%} %>
			</dd>
			<% } %>
			<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
			<dt>
				<%-- 国 --%>
				<%: ReplaceTag("@@User.country.name@@", this.UserAddrCountryIsoCode) %>
				<span class="require">※</span>
			</dt>
			<dd>
				<asp:DropDownList id="ddlUserCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUserAddrCountry_SelectedIndexChanged"/><br/>
				<asp:CustomValidator
					ID="cvUserCountry"
					runat="Server"
					ControlToValidate="ddlUserCountry"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"
					CssClass="error_inline" />
				<span id="countryAlertMessage" class="msg" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
			</dd>
			<% } %>
			<% if (this.IsAmazonLoggedIn && Constants.AMAZON_LOGIN_OPTION_ENABLED && (Constants.AMAZON_PAYMENT_CV2_ENABLED == false)) { %>
			   { %>
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
			<% if (this.IsUserAddrJp) { %>
			<dt>
			<%-- 郵便番号 --%>
			<%: ReplaceTag("@@User.zip.name@@") %><span class="require">※</span><span id="efo_sign_zip"/>
		</dt>
				<dd class="zip">
					<p class="attention" id="addrSearchErrorMessage">
						<asp:CustomValidator
							ID="cvUserZip1"
							runat="Server"
							ValidationGroup="UserRegist"
							ControlToValidate="tbUserZip"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						<%: this.ZipInputErrorMessage %>
					</p>
					<w2c:ExtendedTextBox id="tbUserZip" Type="tel" MaxLength="8" OnTextChanged="lbSearchAddr_Click" runat="server" />
					<br />
				<asp:LinkButton ID="lbSearchAddr" runat="server" OnClick="lbSearchAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">郵便番号から住所を探す</asp:LinkButton>
				<%--検索結果レイヤー--%>
				<uc:Layer ID="ucLayer" runat="server" />
			</dd>
			<% } %>
			<dt>
				<%: ReplaceTag("@@User.addr.name@@") %>
				<span class="require">※</span><span id="efo_sign_ship_address"/>
			</dt>
			<dd class="address">
				<% if (this.IsUserAddrJp) { %>
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserAddr1"
					runat="Server"
					ControlToValidate="ddlUserAddr1"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserAddr2"
					runat="Server"
					ControlToValidate="tbUserAddr2"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserAddr3"
					runat="Server"
					ControlToValidate="tbUserAddr3"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserAddr4"
					runat="Server"
					ControlToValidate="tbUserAddr4"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<asp:DropDownList id="ddlUserAddr1" runat="server"></asp:DropDownList><br />
				<% } %>
				<% if(IsCountryTw(this.UserAddrCountryIsoCode)) { %>
				<asp:DropDownList runat="server" ID="ddlUserAddr2" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlUserAddr2_SelectedIndexChanged"></asp:DropDownList>
				<br />
				<asp:DropDownList runat="server" ID="ddlUserAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
				<% } else { %>
				<%-- 市区町村 --%>
				<w2c:ExtendedTextBox id="tbUserAddr2" placeholder='市区町村' Runat="server" MaxLength="40"></w2c:ExtendedTextBox><br />
				<%-- 番地 --%>
				<w2c:ExtendedTextBox id="tbUserAddr3" placeholder='番地' Runat="server" MaxLength="40"></w2c:ExtendedTextBox><br />
				<% } %>
				<% if (Constants.DISPLAY_ADDR4_ENABLED) { %>
				<%-- ビル・マンション名 --%>
				<w2c:ExtendedTextBox id="tbUserAddr4" placeholder='建物名' Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
				<% } %>

				<% if (this.IsUserAddrJp == false) { %>
				<%-- 州 --%>
					<% if (this.IsUserAddrUs) { %>
				<asp:DropDownList runat="server" ID="ddlUserAddr5" ></asp:DropDownList>
						<asp:CustomValidator
							ID="cvUserAddr5"
							runat="Server"
							ControlToValidate="ddlUserAddr5"
							ValidationGroup="UserRegistGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
				<% } else if (IsCountryTw(this.UserAddrCountryIsoCode)) { %>
				<w2c:ExtendedTextBox id="tbUserAddrTw" placeholder='省' Runat="server" MaxLength="40" ></w2c:ExtendedTextBox>
				<% } else { %>
				<w2c:ExtendedTextBox id="tbUserAddr5" placeholder='州' Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
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
					ValidationGroup="UserRegistGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"
					CssClass="error_inline" />
				</p>
				<w2c:ExtendedTextBox id="tbUserZipGlobal" Type="tel" Runat="server" MaxLength="20"></w2c:ExtendedTextBox>
				<asp:LinkButton
					ID="lbSearchAddrFromZipGlobal"
					OnClick="lbSearchAddrFromZipGlobal_Click"
					Style="display:none;"
					runat="server" />
			</dd>
		<% } %>
		<% } %>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<dt>
				<%-- 企業名 --%>
				<%: ReplaceTag("@@User.company_name.name@@") %>
			</dt>
			<dd class="company-name">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserCompanyName"
					runat="Server"
					ControlToValidate="tbUserCompanyName"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserCompanyName.Attributes["placeholder"] = ReplaceTag("@@User.company_name.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserCompanyName" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
			</dd>
			<dt>
				<%-- 部署名 --%>
				<%: ReplaceTag("@@User.company_post_name.name@@") %>
			</dt>
			<dd class="company-post">
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserCompanyPostName"
					runat="Server"
					ControlToValidate="tbUserCompanyPostName"
					ValidationGroup="UserRegist"
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
			<span class="require">※</span><span id="efo_sign_tel1"/>
		</dt>
			<dd class="tel">
				<p class="attention">
					<asp:CustomValidator
						ID="cvUserTel1"
						runat="Server"
						ControlToValidate="tbUserTel1_1"
						ValidationGroup="UserRegist"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbUserTel1_1" Type="tel" MaxLength="13" style="width:100%;" runat="server" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvUserTel1')" />
				<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
					<p style="margin-top:3px">※<%= WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE) %></p>
				<% } %>
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
				<%: ReplaceTag("@@User.authentication_code.name@@") %>
				<span class="require">※</span>
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
					ValidationGroup="UserRegist"
					ValidateEmptyText="false"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
			</dd>
			<% } %>
			<dt>
				<%: ReplaceTag("@@User.tel2.name@@") %>
			</dt>
			<dd class="tel">
				<p class="attention">
					<asp:CustomValidator
						ID="cvUserTel2_1"
						runat="Server"
						ControlToValidate="tbUserTel1_2"
						ValidationGroup="UserRegist"
						ValidateEmptyText="false"
						SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbUserTel1_2" Type="tel" MaxLength="13" style="width:100%;" runat="server" CssClass="shortTel" />
			</dd>
			<% } else { %>
			<dt>
				<%-- 電話番号 --%>
				<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
				<span class="require">*</span>
			</dt>
			<dd class="tel">
				<w2c:ExtendedTextBox id="tbUserTel1Global" type="tel" Runat="server" MaxLength="30" Width="100%" onchange="resetAuthenticationCodeInput('cvUserTel1Global')" />
				<% if (this.IsDisplayPersonalAuthentication) { %>
				<asp:LinkButton
					ID="lbGetAuthenticationCodeGlobal"
					CssClass="btn-add-get"
					runat="server"
					Text="認証コードの取得"
					OnClick="lbGetAuthenticationCode_Click"
					OnClientClick="return checkTelNoInput();" />
				<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
				<% } %>
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserTel1Global"
					runat="Server"
					ControlToValidate="tbUserTel1Global"
					ValidationGroup="UserRegistGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
			</dd>
			<% if (this.IsDisplayPersonalAuthentication) { %>
			<dt>
				<%: ReplaceTag("@@User.authentication_code.name@@") %>
				<span class="require">※</span>
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
					ValidationGroup="UserRegistGlobal"
					ValidateEmptyText="false"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
			</dd>
			<% } %>
			<dt>
				<th><%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %></th>
				<span class="require"></span>
			</dt>
			<dd class="tel">
				<w2c:ExtendedTextBox id="tbUserTel2Global" type="tel" Runat="server" MaxLength="30"></w2c:ExtendedTextBox>
				<p class="attention">
				<asp:CustomValidator
					ID="cvUserTel2Global"
					runat="Server"
					ControlToValidate="tbUserTel2Global"
					ValidationGroup="UserRegistGlobal"
					ValidateEmptyText="false"
					SetFocusOnError="true" />
				</p>
			</dd>
			<% } %>
			<dt>
				<%: ReplaceTag("@@User.mail_flg.name@@") %>
			</dt>
			<dd>
				<asp:CheckBox ID="cbUserMailFlg" Text="希望する" runat="server" />
			</dd>
			<dt>
				<%-- ユーザ拡張項目 --%>
				その他
			</dt>
			<dd class="extend">
			<uc:BodyUserExtendRegist ID="ucBodyUserExtendRegist" HasInput="true" HasRegist="true" ExistingUser="<%# this.LoginUser %>" runat="server" />
			</dd>
		</dl>
		<%-- ソーシャルログイン用 --%>
		<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
		<asp:HiddenField ID="hfSocialLoginJson" runat="server" />
		<% } %>
		<% if (this.IsDisplayPersonalAuthentication) { %>
		<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
		<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
		<% } %>
</div>
	</ContentTemplate>
</asp:UpdatePanel>

	<%--GMO--%>
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
			<div class="order-unit">
				<% if (this.IsUserAddrJp) { %>
				<h2 id ="H1">
					<asp:CheckBox ID="IsBusinessOwner" Text=" GMO枠保証を希望する" CssClass="checkBox" runat="server" OnCheckedChanged="checkBusinessOwnerChangedEvent" AutoPostBack="True" Checked="true"/>

				</h2>
					<% if (this.WcbIsBusinessOwner.Checked){ %>
				<dl class="order-form">
							<dt>
								<%: ReplaceTag("@@User.OwnerName1.name@@") %>
								<span class="require">※</span>
							</dt>
							<dd>
								<asp:TextBox id="tbOwnerName1" Runat="server" MaxLength="21" Type="text" width="100%"></asp:TextBox>
								<asp:CustomValidator
									ID="cvpresidentNameFamily"
									runat="Server"
									ControlToValidate="tbOwnerName1"
									ValidationGroup="UserRegist"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</dd>

							<dt>
								<%: ReplaceTag("@@User.OwnerName2.name@@") %>
								<span class="require">※</span>
							</dt>
							<dd>
								<asp:TextBox id="tbOwnerName2" Runat="server" MaxLength="21" Type="text" width="100%"></asp:TextBox>
								<asp:CustomValidator
									ID="cvpresidentName"
									runat="Server"
									ControlToValidate="tbOwnerName2"
									ValidationGroup="UserRegist"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</dd>

							<dt>
								<%: ReplaceTag("@@User.OwnerNameKana1.name@@") %>
								<span class="require">※</span>
							</dt>
							<dd>
								<asp:TextBox id="tbOwnerNameKana1" Runat="server" MaxLength="25" Type="text" width="100%"></asp:TextBox>
								<asp:CustomValidator
									ID="cvpresidentNameFamilyKana"
									runat="Server"
									ControlToValidate="tbOwnerNameKana1"
									ValidationGroup="UserRegist"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</dd>

							<dt>
								<%: ReplaceTag("@@User.OwnerNameKana2.name@@") %>
								<span class="require">※</span>
							</dt>
							<dd>
								<asp:TextBox id="tbOwnerNameKana2" Runat="server" MaxLength="25" Type="text" width="100%"></asp:TextBox>
								<asp:CustomValidator
									ID="cvpresidentNameKana"
									runat="Server"
									ControlToValidate="tbOwnerNameKana2"
									ValidationGroup="UserRegist"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</dd>

							<dt>
								<%: ReplaceTag("@@User.OwnerBirth.name@@") %>
								<span class="require">※</span>
							</dt>
							<dd class="birth">
								<asp:DropDownList id="ddlOwnerBirthYear" runat="server" CssClass="year" onchange="changeGmoDropDownDays()"></asp:DropDownList>年
								<asp:DropDownList id="ddlOwnerBirthMonth" runat="server" CssClass="month" onchange="changeGmoDropDownDays()"></asp:DropDownList>月
								<asp:DropDownList id="ddlOwnerBirthDay" runat="server" CssClass="date"></asp:DropDownList>日
								<asp:CustomValidator
									ID="cvOwnerBirthYear"
									runat="Server"
									ControlToValidate="ddlOwnerBirthYear"
									ValidationGroup="UserRegist"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									EnableClientScript="false"
									CssClass="error_inline" />
								<asp:CustomValidator
									ID="cvOwnerBirthMonth"
									runat="Server"
									ControlToValidate="ddlOwnerBirthMonth"
									ValidationGroup="UserRegist"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									EnableClientScript="false"
									CssClass="error_inline" />
								<asp:CustomValidator
									ID="cvOwnerBirthDay"
									runat="Server"
									ControlToValidate="ddlOwnerBirthDay"
									ValidationGroup="UserRegist"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									EnableClientScript="false"
									CssClass="error_inline" />
							</dd>

						<dt>
							<%: ReplaceTag("@@User.RequestBudget.name@@") %>
							<span class="require">※</span>
						</dt>
						<dd>
							<asp:TextBox id="tbRequestBudget" Runat="server" MaxLength="8" Type="text" width="30%" style="text-align: right" />
							<p class="limit-unit">円</p>
							<asp:CustomValidator
								ID="cvreqUpperLimit"
								runat="Server"
								ControlToValidate="tbRequestBudget"
								ValidationGroup="UserRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						</dd>
					</dl>
					<% } %>
				<% } %>
			</div>
			<% } %>
		</ContentTemplate>
	</asp:UpdatePanel>

<%if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) || this.IsVisible_UserPassword) { %>
<div class="order-unit login">
	<h2 id ="logininfo">ログイン情報</h2>
		<dl class="order-form">
		<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
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
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<% tbUserLoginId.Attributes["placeholder"] = ReplaceTag("@@User.login_id.name@@"); %>
				<w2c:ExtendedTextBox id="tbUserLoginId" Type="email" MaxLength="15" Runat="server"></w2c:ExtendedTextBox>
			</dd>
		<%} %>
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
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvUserPasswordConf"
					runat="Server"
					ControlToValidate="tbUserPasswordConf"
					ValidationGroup="UserRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<div id="reTypeHiddenDiv2" runat="server"><p class="msg">※半角英数字混合7文字〜15文字以内でお願いいたします</p></div>
				<p><asp:Literal ID="lUserPassword" runat="server"></asp:Literal>
				<w2c:ExtendedTextBox id="tbUserPassword" TextMode="Password" autocomplete="off" MaxLength="15" Runat="server"></w2c:ExtendedTextBox><asp:Button ID="btnUserPasswordReType" runat="server" Text="再入力" Visible="False" OnClick="btnUserPasswordReType_Click" Width="128px"></asp:Button></p>
				<div id="reTypeHiddenDiv1" runat="server">
				<p class="msg">※お手数ですが、確認のため２度入力してください</p>
				<asp:Literal ID="lUserPasswordConf" runat="server"></asp:Literal>
				<w2c:ExtendedTextBox id="tbUserPasswordConf" TextMode="Password" autocomplete="off" MaxLength="15" Runat="server"></w2c:ExtendedTextBox>
				</div>
			</dd>
			<% } %>
		</dl>
</div>
<% } %>
<%-- キャプチャ認証 --%>
<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbConfirm.ClientID %>" /><br />
<div class="order-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbConfirm" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" CssClass="btn">
		確認する
		</asp:LinkButton>
	</div>
</div>
</section>

<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/SocialPlusInputCompletion.js"></script>
<script type="text/javascript">
<!--
	bindEvent();

	changeDropDownDays();
	<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
	changeGmoDropDownDays()
	<% } %>
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
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON%>',
			'#addrSearchErrorMessage',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>')

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
			for(var day = 0; day < (maxDay - daysCount); day++) {
				var appendDay = daysCount + day + 1;
				var option = document.createElement('option');
				option.setAttribute('value', appendDay);
				option.innerHTML = appendDay;
				select.appendChild(option);
			}
		} else {
			for(var dayDifference = 0; dayDifference < daysCount - maxDay; dayDifference++) {
				document.getElementById('<%= ddlUserBirthDay.ClientID %>').remove(daysCount - dayDifference);
			}
		}
	}

	<%-- 日付リスト変更 --%>
	function changeGmoDropDownDays() {
		var year = $("#<%= ddlOwnerBirthYear.ClientID %>").val();
		var month = $("#<%= ddlOwnerBirthMonth.ClientID %>").val();

		var select = document.getElementById('<%= ddlOwnerBirthDay.ClientID %>');
		
		var maxDay = new Date(year, month, 0).getDate();

		var optionsList = document.getElementById('<%= ddlOwnerBirthDay.ClientID %>').options;
		var daysCount = parseInt(optionsList[optionsList.length - 1].value);

		if (daysCount < maxDay) {
			for(var day = 0; day < (maxDay - daysCount); day++) {
				var appendDay = daysCount + day + 1;
				var option = document.createElement('option');
				option.setAttribute('value', appendDay);
				option.innerHTML = appendDay;
				select.appendChild(option);
			}
		} else {
			for(var dayDifference = 0; dayDifference < daysCount - maxDay; dayDifference++) {
				document.getElementById('<%= ddlOwnerBirthDay.ClientID %>').remove(daysCount - dayDifference);
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

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		$('#<%= this.WddlUserAddr3.ClientID %>').change(function (e) {
			$('#<%= this.WtbUserZipGlobal.ClientID %>').val(
				$('#<%= this.WddlUserAddr3.ClientID %>').val().split('|')[0]);
		});
	}
	<% } %>

	<% if (this.IsDisplayPersonalAuthentication) { %>
	// Set authentication message
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
	<%--▼▼Amazonウィジェット用スクリプト▼▼--%>
	<script type="text/javascript">
		
		window.onAmazonLoginReady = function () {
			amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
			amazon.Login.setUseCookie(true);
		};
		window.onAmazonPaymentsReady = function () {
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

		<%--Amazon住所取得関数--%>
		function getAmazonAddress(callback) {
			$.ajax({
				type: "POST",
				url: "<%=Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_USER_REGIST_INPUT%>/GetAmazonAddress",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({
					amazonOrderReferenceId: $('#<%= this.WhfAmazonOrderRefID.ClientID%>').val()
				}),
				success: callback
			});
		}
	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
	<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
	<% } %>
<% } %>
</asp:Content>
