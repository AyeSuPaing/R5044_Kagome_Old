<%--
=========================================================================================================
  Module      : ログイン画面(Login.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Login.aspx.cs" Inherits="Form_Login" Title="ログインページ" %>
<%@ Import Namespace="w2.App.Common.Line.LineDirectConnect" %>
<%@ Register Src="~/Form/Common/PaypalScriptsForm.ascx" TagPrefix="uc" TagName="PaypalScriptsForm" %>
<%@ Register Src="~/Form/Common/MailDomains.ascx" TagPrefix="uc" TagName="MailDomains" %>
<%@ Register Src="~/Form/Common/UserLoginScript.ascx" TagPrefix="uc" TagName="UserLoginScript" %>
<%@ Register Src="~/Form/Common/SendAuthenticationCodeModal.ascx" TagPrefix="uc" TagName="SendAuthenticationCodeModal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script runat="server">
	public new void Page_Load(Object sender, EventArgs e)
	{
		base.Page_Load(sender, e);
		if (Constants.COMMON_SOCIAL_LOGIN_ENABLED && SessionManager.TemporaryStoreSocialLogin != null)
		{
			this.Page.Title = "ソーシャルログインページ";
		}
	}
</script>
<div id="dvUserContents">
	<h2 id="h2Login" runat="server">ログインページ</h2>
	<h2 id="h2SocialLogin" Visible="False" runat="server">ソーシャルログイン連携ページ</h2>
	<div id="dvLogin" class="unit clearFix">
			<div id="dvMessages" runat="server" class="contentsInfo"><p><%= WebSanitizer.HtmlEncodeChangeToBr(this.ErrorMessage) %></p></div>
		<div id="dvLoginWrap">
			<div class="dvLoginLogin">
				<h3>ログイン</h3>
				<p id="pLogin" runat="server">会員登録がお済みの方は以下よりログイン下さい。</p>
				<p id="pSocialLogin" Visible="False" runat="server">会員登録がお済みの方はログインでアカウント連携ができます。</p>
				<ul>
					<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>
					<li>
						<%: ReplaceTag("@@User.mail_addr.name@@") %><br />
						<asp:TextBox ID="tbLoginIdInMailAddr" Runat="server" CssClass="loginIdInMailAddr mail-domain-suggest" MaxLength="256" Type="email"></asp:TextBox>
					</li>
					<%} else { %>
					<li><%: ReplaceTag("@@User.login_id.name@@") %><br />
						<asp:TextBox ID="tbLoginId" Runat="server" CssClass="loginId" MaxLength="15"></asp:TextBox></li>
					<%} %>
					<li><%: ReplaceTag("@@User.password.name@@") %><br />
						<% tbPassword.Attributes["MaxLength"] = ReplaceTag("@@User.password.length_max@@"); %>
						<asp:TextBox ID="tbPassword" Runat="server" TextMode="Password" autocomplete="off" CssClass="loginPass"></asp:TextBox></li>
					<li>
						<small id="dLoginErrorMessage" class="fred" runat="server"></small>
					</li>
					<li><asp:CheckBox ID="cbAutoCompleteLoginIdFlg" runat="server" Text="ログインIDを記憶する" /><br />（共有パソコンの場合は解除がオススメです）</li>
				</ul>
				<p>
				<asp:LinkButton ID="lbLogin" runat="server" onclick="lbLogin_Click" OnClientClick="return onClientClickLogin();" class="btn-org btn-large btn-org-blk">ログイン</asp:LinkButton>
				</p>
			</div>
			<div class="dvLoginReminder">
				<h3>パスワードを忘れた方</h3>
				<p><a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_PASSWORD_REMINDER_INPUT %>" class="btn-org btn-large btn-org-blk">パスワードを忘れた方はこちら</a></p>
			</div>
		</div>
		<div class="dvLoginRegist">
			<h3>会員登録</h3>

			<p>会員登録がお済みで無い方はこちらから登録をお願いいたします。</p>
			<p><a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION + "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + Server.UrlEncode(Request[Constants.REQUEST_KEY_NEXT_URL]) %>" class="btn-org btn-large btn-org-blk">会員登録をする</a></p>
			<%if (Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED) {%>
			<p><a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT %>" class="btn-org btn-large btn-org-blk">かんたん会員登録をする</a></p>
			<%} %>
			<% if (Constants.COMMON_SOCIAL_LOGIN_ENABLED && SessionManager.TemporaryStoreSocialLogin == null && string.IsNullOrEmpty(SessionManager.LineProviderUserId)) { %>
				<div style="list-style-type: none; margin-top: 30px;">
					<h3>ソーシャルログイン</h3>
					<div style="padding: 0 1em;">
						<ul>
							<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
								<%-- Apple --%>
								<li style="margin-bottom: 10px; float: none;">
									<a class="social-login-apple apple-color"
										href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
											w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
											Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
											Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
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
								<li style="margin-bottom: 10px; float: none;">
									<a class="social-login facebook-color"
										href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
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
								<li style="margin-bottom: 10px; float: none;">
									<a class="social-login twitter-color"
										href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
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
								<li style="margin-bottom: 10px; float: none;">
									<a class="social-login yahoo-color"
										href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
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
								<li style="margin-bottom: 10px; float: none;">
									<a class="social-login google-color"
										href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
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
								<li style="margin-bottom: 10px; float: none;">
									<div class="social-login line-color">
										<div class="social-login line-hover-color line-active-color">
											<a href="<%= LineConnect(
												w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION
													? Constants.PAGE_FRONT_DEFAULT
													: Constants.PAGE_FRONT_LOGIN,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK,
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
									<p style="margin: 3px 0 0 0;">※LINE連携時に友だち追加します</p>
								</li>
							<% } %>
							<%-- AmazonPay --%>
							<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED) { %>
								<li style="margin-bottom: 10px; float: none;">
									<%--▼▼Amazonログインボタンウィジェット▼▼--%>
									<div id="AmazonLoginButton"></div>
									<div style="width:296px; display: inline-block;">
										<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
										<div id="AmazonLoginCv2Button"></div>
										<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
									</div>
									<%--▲▲Amazonログインボタンウィジェット▲▲--%>
								</li>
							<% } %>
							<%-- PayPal --%>
							<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
								<li style="margin-bottom: 10px; float: none;">
									<%
										ucPaypalScriptsForm.LogoDesign = "Login";
										ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
										ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
									%>
									<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
									<div id="paypal-button" style="width: 296px;"></div>
									<% if (SessionManager.PayPalCooperationInfo != null) {%>
										<p style="margin: 3px 0 0 0;">※<%: SessionManager.PayPalCooperationInfo.AccountEMail %> 連携済</p>
									<% } else {%>
										<p style="margin: 3px 0 0 0;">※PayPalで新規登録/ログインします</p>
									<%} %>
									<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click" />
								</li>
							<% } %>
							<%-- 楽天Connect --%>
							<% if (Constants.RAKUTEN_LOGIN_ENABLED) { %>
								<li style="margin-bottom: 10px; float: none;">
									<asp:LinkButton ID="lbRakutenIdConnectRequestAuthForUserRegister" runat="server" OnClick="lbRakutenIdConnectRequestAuth_Click">
										<img src="https://static.id.rakuten.co.jp/static/btn-japanese-2x/idconnect_01-login_r@2x.png" style="width: 296px; height: 40px" />
									</asp:LinkButton>
									<p style="margin: 3px 0 0 0; width: 460px;">
										楽天会員のお客様は、楽天IDに登録している情報を利用して、<br/>
										「新規会員登録/ログイン」が可能です。
									</p>
								</li>
							<% } %>
						</ul>
					</div>
				</div>
			<% } %>
		</div>
	</div>
</div>
<uc:SendAuthenticationCodeModal runat="server" ID="ucSendAuthenticationCodeModal" />
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
	<script type='text/javascript'>
		window.onAmazonLoginReady = function () {
			amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
		};
		window.onAmazonPaymentsReady = function () {
			if ($('#AmazonLoginButton').length) showButton();
		};

		<%--Amazonボタン表示ウィジェット--%>
		function showButton() {
			var authRequest;
			OffAmazonPayments.Button("AmazonLoginButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
				type: "LwA",
				color: "Gold",
				size: "large",
				authorization: function () {
					loginOptions = {
						scope: "payments:shipping_address payments:widget profile",
						popup: true
					};
					authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_LOGIN_CALLBACK, this.NextUrl)%>");
				},
				onError: function (error) {
					alert(error.getErrorMessage());
				}
			});
			$('#OffAmazonPaymentsWidgets0').css({ 'height': '44px', 'width': '237px' });
		}
	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
	<% } %>
<% } %>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<%--▼▼ O-MOTION用スクリプト ▼▼--%>
<script type="text/javascript">
	var setOmotionClientIdJs;
	var webMethodUrl;
	$(function () {
		setOmotionClientIdJs = "<%= CreateSetOmotionClientIdJsScript().Replace("\"", "\\\"") %>";
		webMethodUrl = "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN %>";
	});
</script>
<uc:UserLoginScript runat="server" ID="ucUserLoginScript" />
<%--▲▲ O-MOTION用スクリプト ▲▲--%>
</asp:Content>
