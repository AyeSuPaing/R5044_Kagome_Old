<%--
=========================================================================================================
  Module      : スマートフォン用注文者決定画面(OrderOwnerDecision.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderOwnerDecision.aspx.cs" Inherits="Form_Order_OrderOwnerDecision" Title="注文者決定ページ" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register Src="~/Form/Common/UserLoginScript.ascx" TagPrefix="uc" TagName="UserLoginScript" %>
<%@ Register Src="~/Form/Common/SendAuthenticationCodeModal.ascx" TagPrefix="uc" TagName="SendAuthenticationCodeModal" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<%-- ▽▽Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない▽▽ --%>
<script runat="server">
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
</script>
<%-- △△Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない△△ --%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-order order-decision">

<div class="order-unit login">
	<h2>ログイン</h2>
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
			<% tbLoginIdInMailAddr.Attributes["placeholder"] = ReplaceTag("@@User.mail_addr.name@@"); %>
			<w2c:ExtendedTextBox ID="tbLoginIdInMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
		<% } else { %>
			<% tbLoginId.Attributes["placeholder"] = ReplaceTag("@@User.login_id.name@@"); %>
			<w2c:ExtendedTextBox ID="tbLoginId" Runat="server" MaxLength="15"></w2c:ExtendedTextBox>
		<%} %>
	</dd>
	<dt>
		<%: ReplaceTag("@@User.password.name@@") %>
	</dt>
	<dd>
		<% tbLoginId.Attributes["placeholder"] = ReplaceTag("@@User.password.name@@"); %>
		<w2c:ExtendedTextBox ID="tbPassword" Runat="server" TextMode="Password" autocomplete="off" MaxLength="15"></w2c:ExtendedTextBox>
	</dd>
	</dl>
	<p id="dLoginErrorMessage" class="attention" runat="server"></p>
	<p class="checked">
		<asp:CheckBox ID="cbAutoCompleteLoginIdFlg" runat="server" Text="次回からの入力を省略" />
	</p>
	<div class="order-footer">
		<div class="button-next">
		<asp:LinkButton ID="lbLogin" runat="server" onclick="lbLogin_Click" OnClientClick="return onClientClickLogin();" CssClass="btn">ログイン</asp:LinkButton>
		</div>
	</div>
</div>

<div class="order-unit not-login">
	<h2>会員でない方</h2>
	<div class="order-footer">
		<%if (this.CartList.HasFixedPurchase) { %>
		<p>定期購入商品は会員様のみがご購入頂けるようになっております。</p>
		<p>ログインIDをお持ちでないお客様はこちらから会員登録を行ってください。</p><br/>
		<% }else{ %>
		<p>会員登録がお済みで無い方はこちらから登録をお願いいたします。</p><br/>
		<% } %>
		<div class="button-next">
			<asp:LinkButton ID="lbUserRegist" runat="server" OnClick="lbUserRegist_Click" class="btn">新規会員登録</asp:LinkButton>
		</div>
		<div class="button-next">
			<%if (Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED) {%>
				<asp:LinkButton ID="lbUserEasyRegist" runat="server" OnClick="lbUserEasyRegist_Click" class="btn">かんたん会員登録する</asp:LinkButton>
			<% } %>
		</div>
	</div>
	<% if (this.CartList.HasFixedPurchase == false) { %>
	<h2>会員登録せずに購入</h2>
	<div class="order-footer">
		<div class="button-next">
		<asp:LinkButton ID="lbOrderShipping" runat="server" onclick="lbOrderShipping_Click" class="btn">
			ログインせずに購入する
		</asp:LinkButton>
		</div>
	</div>
	<% } %>
	<% if (Constants.COMMON_SOCIAL_LOGIN_ENABLED) { %>
	<div>
		<h2>ソーシャルログイン</h2>
		<div>
			<ul style="list-style:none;text-align:center;padding:0px;margin:1em 0 0 0;">
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
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
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
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
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
							href="<%= w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
									w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
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
				<% if (Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
					<%-- LINE --%>
					<li class="social-login-margin">
						<div class="social-login line-color">
							<div class="social-login line-active-color">
								<a href="<%= LineConnect(
									w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION
										? Constants.PAGE_FRONT_ORDER_SHIPPING
										: Constants.PAGE_FRONT_USER_REGIST_REGULATION,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
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
						<p style="margin-top:3px;">※LINE連携時に友だち追加します</p>
					</li>
				<% } %>
				<%-- Amazon --%>
				<% if (this.IsVisibleAmazonPayButton || this.IsVisibleAmazonLoginButton) { %>
					<li class="social-login-margin">
						<%-- ▼▼Amazonボタンウィジェット▼▼ --%>
						<div id="AmazonPayButton"></div>
						<div style="width: 296px; display: inline-block;">
							<%--▼▼ Amazon(CV2)ボタン ▼▼--%>
							<div id="AmazonCv2Button"></div>
							<%--▲▲ Amazon(CV2)ボタン ▲▲--%>
						</div>
						<%-- ▲▲Amazonボタンウィジェット▲▲ --%>
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
						<div style="margin-top: 3px;">
							<% if (SessionManager.PayPalCooperationInfo != null) {%>
								※<%: SessionManager.PayPalCooperationInfo.AccountEMail %> 連携済
							<% } else {%>
								※PayPalで新規登録/ログインします
							<%} %>
						</div>
						<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click" />
					</li>
				<% } %>
				<%-- 楽天Connect --%>
				<% if (Constants.RAKUTEN_LOGIN_ENABLED) { %>
					<li class="social-login-margin">
						<asp:LinkButton ID="lbRakutenIdConnectRequestAuthForUserRegister" runat="server" OnClick="lbRakutenIdConnectRequestAuth_Click">
							<img src="https://static.id.rakuten.co.jp/static/btn-japanese-2x/idconnect_01-login_r@2x.png"
								style="width: 296px; height: 40px" />
						</asp:LinkButton>
						<p style="margin-top: 3px">
							楽天会員のお客様は、<br/>
							楽天IDに登録している情報を利用して、<br/>
							「新規会員登録/ログイン」が可能です。
						</p>
					</li>
				<% } %>
			</ul>
		</div>
	</div>
	<% } %>
</div>
<uc:SendAuthenticationCodeModal runat="server" ID="ucSendAuthenticationCodeModal" />
<% if (this.IsVisibleAmazonPayButton || this.IsVisibleAmazonLoginButton) { %>
	<% if(Constants.AMAZON_PAYMENT_CV2_ENABLED){ %>
	<%--▼▼ Amazon(CV2)スクリプト ▼▼--%>
	<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
	<script type="text/javascript" charset="utf-8">
		showAmazonCv2Button(
			'#AmazonCv2Button',
			'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
			<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
			'<%= this.AmazonRequest.Payload %>',
			'<%= this.AmazonRequest.Signature %>',
			'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>',
			'<%= (this.IsVisibleAmazonPayButton) ? "PayAndShip" : ((this.IsVisibleAmazonLoginButton) ? "SignIn" : "") %>');
	</script>
	<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
	<% } else { %>
	<%--▼▼Amazonウィジェット用スクリプト▼▼--%>
	<script type="text/javascript">
		window.onAmazonLoginReady = function () {
			amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
			amazon.Login.setUseCookie(true);
		};
		window.onAmazonPaymentsReady = function () {
			if ($('#AmazonPayButton').length) showButton();
		};

		<%-- Amazonボタン表示ウィジェット --%>
		function showButton() {
			var authRequest;
			OffAmazonPayments.Button("AmazonPayButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
				type: "<%=(this.IsVisibleAmazonPayButton) ? "PwA" : ((this.IsVisibleAmazonLoginButton) ? "LwA" : "") %>",
				color: "Gold",
				size: "large",
				authorization: function () {
					loginOptions = {
						scope: 'payments:widget payments:shipping_address profile',
						popup: false,
						state: '<%= Request.RawUrl %>'
					};
					authRequest = amazon.Login.authorize(loginOptions, '<%= this.AmazonCallBackUrl %>');
				},
				onError: function (error) {
					alert(error.getErrorMessage());
				}
			});
			$('#OffAmazonPaymentsWidgets0').css({ 'width': '324px' });
		};
	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
	<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
	<% } %>
<% } %>
</section>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
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
