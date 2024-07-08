<%--
=========================================================================================================
  Module      : スマートフォン用ログイン画面(Login.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Login.aspx.cs" Inherits="Form_Login" Title="ログインページ" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
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
<section class="wrap-order">
	<div class="order-unit login">
		<h2 id="h2Login" runat="server">ログインページ</h2>
		<h2 id="h2SocialLogin" Visible="False" runat="server">ソーシャルログイン連携ページ</h2>
		<div id="dvMessages" class="msg-alert" runat="server"><%= WebSanitizer.HtmlEncodeChangeToBr(this.ErrorMessage) %></div>
		<dl class="order-form">
			<dt>
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>
					<%: ReplaceTag("@@User.mail_addr.name@@") %>
				<%} else { %>
					<%: ReplaceTag("@@User.login_id.name@@") %>
				<%} %>
			</dt>
			<dd>
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>
				<asp:TextBox ID="tbLoginIdInMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></asp:TextBox>
				<%} else { %>
				<asp:TextBox ID="tbLoginId" Type="email" Runat="server" CssClass="loginId" MaxLength="15"></asp:TextBox>
				<%} %>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.password.name@@") %>
			</dt>
			<dd>
				<% tbPassword.Attributes["MaxLength"] = ReplaceTag("@@User.password.length_max@@"); %>
				<asp:TextBox ID="tbPassword" Runat="server" TextMode="Password" autocomplete="off" CssClass="loginPass"></asp:TextBox>
			</dd>
		</dl>
		<p class="msg">
			<small id="dLoginErrorMessage" class="attention" runat="server"></small>
		</p>
		<p class="memory-pass"><asp:CheckBox ID="cbAutoCompleteLoginIdFlg" runat="server" Text="次回からの入力を省略" /></p>
		<div class="order-footer">
			<div class="button-next">
				<asp:LinkButton ID="lbLogin" runat="server" onclick="lbLogin_Click" OnClientClick="return onClientClickLogin();" CssClass="btn">ログイン</asp:LinkButton>
			</div>
			<p><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PASSWORD_REMINDER_INPUT) %>">&raquo; パスワードを忘れた方はこちら</a></p>
		</div>
		<% if (Constants.COMMON_SOCIAL_LOGIN_ENABLED && SessionManager.TemporaryStoreSocialLogin == null && string.IsNullOrEmpty(SessionManager.LineProviderUserId)) { %>
		<h2>ソーシャルログイン</h2>
		<div>
			<ul style="list-style-type:none;text-align:center;padding:0px;margin:1em 0 0 0;">
				<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
					<%-- Apple --%>
					<li class="social-login-margin">
						<a class="social-login apple-color"
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
					<li class="social-login-margin">
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
					<li class="social-login-margin">
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
					<li class="social-login-margin">
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
					<li class="social-login-margin">
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
				<%-- LINE --%>
				<% if (Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
					<li class="social-login-margin">
						<div class="social-login line-color">
							<div class="social-login line-active-color">
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
						<p style="margin-top: 3px;">※LINE連携時に友だち追加します</p>
					</li>
				<% } %>
				<%-- AmazonPay --%>
				<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED) { %>
					<li class="social-login-margin">
						<%-- ▼▼Amazonログインボタンウィジェット▼▼ --%>
						<div style="width: 296px; display:inline-block;">
							<div id="AmazonLoginButton"></div>
							<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
							<div id="AmazonLoginCv2Button"></div>
							<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
						</div>
						<%-- ▲▲Amazonログインボタンウィジェット▲▲ --%>
					</li>
				<% } %>
				<%-- PayPal --%>
				<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
					<li class="social-login-margin">
						<%
							ucPaypalScriptsForm.LogoDesign = "Login";
							ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
							ucPaypalScriptsForm.GetShippingAddress = true;
						%>
						<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
						<div id="paypal-button" style="width: 296px; margin: auto;"></div>
					<% if (SessionManager.PayPalCooperationInfo != null) {%>
							<p style="margin-top: 3px;">※<%: SessionManager.PayPalCooperationInfo.AccountEMail %> 連携済</p>
						<% } else {%>
							<p style="margin-top: 3px;">※PayPalで新規登録/ログインします</p>
						<%} %>
						<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click" />
					</li>
				<% } %>
				<%-- 楽天Connect --%>
				<% if (Constants.RAKUTEN_LOGIN_ENABLED) { %>
					<li class="social-login-margin">
						<p style="text-align: center">
							<asp:LinkButton ID="lbRakutenIdConnectRequestAuthForUserRegister" runat="server" OnClick="lbRakutenIdConnectRequestAuth_Click">
								<img src="https://static.id.rakuten.co.jp/static/btn-japanese-2x/idconnect_01-login_r@2x.png"
									style="width: 296px; height: 40px" />
							</asp:LinkButton>
							<p style="margin-top:3px">
								楽天会員のお客様は、<br/>
								楽天IDに登録している情報を利用して、<br/>
								「新規会員登録/ログイン」が可能です。
							</p>
						</p>
					</li>
				<% } %>
			</ul>
		</div>
		<% } %>
	</div>

	<div class="order-unit">
		<h2>新規会員登録</h2>
		<p class="msg">
			会員登録がお済みでない方は「会員登録する」をタップしてください。登録は無料になります。
		</p>
		<div class="order-footer">
			<div class="button-next">
				<a class="btn" href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION + "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + Server.UrlEncode(Request[Constants.REQUEST_KEY_NEXT_URL])) %>">会員登録する</a>
			</div>
			<div class="button-next">
				<%if (Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED) {%>
				<a class="btn" href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT %>">かんたん会員登録する</a>
				<%} %>
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
		<script type="text/javascript">
			window.onAmazonLoginReady = function () {
				amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
				amazon.Login.setUseCookie(true);
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
							popup: false
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
</section>
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<%--▼▼ O-MOTION用スクリプト ▼▼--%>
<script type="text/javascript">
    var setOmotionClientIdJs;
    var webMethodUrl;
    $(function () {
        setOmotionClientIdJs = "<%= CreateSetOmotionClientIdJsScript().Replace("\"", "\\\"") %>";
        webMethodUrl = "<%= Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_LOGIN %>";
    });
</script>
<uc:UserLoginScript runat="server" ID="ucUserLoginScript" />
<%--▲▲ O-MOTION用スクリプト ▲▲--%>
</asp:Content>
