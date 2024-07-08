<%--
=========================================================================================================
  Module      : 注文者決定画面(OrderOwnerDecision.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderOwnerDecision.aspx.cs" Inherits="Form_Order_OrderOwnerDecision" Title="注文者決定ページ" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Line.LineDirectConnect" %>
<%@ Register Src="~/Form/Common/MailDomains.ascx" TagPrefix="uc" TagName="MailDomains" %>
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
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvUserBox" class="clearFix">
	<div id="dvUserContents">
		<h2>注文者の決定</h2>
		<div id="dvLogin" class="unit clearFix">
			<div id="dvLoginWrap">
				<div class="dvLoginLogin">
					<h3>会員の方</h3> 
					<p >ログインIDをお持ちの方は、こちらからログインを行ってください。</p>
					<div id="LoginBox">
					<div class="top">
					<div class="bottom">
					<div>
					<dl>
					<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>
					<dt>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
					</dt>
					<dd><asp:TextBox ID="tbLoginIdInMailAddr" runat="server" CssClass="input_widthC input_border mail-domain-suggest" MaxLength="256" Type="email"></asp:TextBox></dd>
					<%} else { %>
					<dt>
						<%: ReplaceTag("@@User.login_id.name@@") %>
					</dt>
					<dd><asp:TextBox ID="tbLoginId" runat="server" CssClass="input_widthC input_border" MaxLength="15" Type="email"></asp:TextBox></dd>
					<%} %>
					</dl>
					<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
					</div>
					<div>
					<dl>
					<dt>
						<%: ReplaceTag("@@User.password.name@@") %>
					</dt>
					<dd><asp:TextBox ID="tbPassword" TextMode="Password" autocomplete="off" runat="server" CssClass="input_widthC input_border" MaxLength="15"></asp:TextBox></dd>
					</dl>
					<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
					</div>
					<p><small id="dLoginErrorMessage" class="fred" runat="server"></small></p>
					<span><asp:CheckBox ID="cbAutoCompleteLoginIdFlg" runat="server" Text="ログインIDを記憶する" /></span>
					<div class="alignR mb15"><asp:LinkButton ID="lbLogin" runat="server" onclick="lbLogin_Click" OnClientClick="return onClientClickLogin();" class="btn btn-large btn-success">ログイン</asp:LinkButton></div>
					<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_PASSWORD_REMINDER_INPUT) %>">パスワードを忘れた方はこちら</a></span>
					</div><!--bottom-->
					</div><!--top-->
					</div><!--LoginBox-->
				</div><!--dvLoginLogin-->
			</div><!--dvLoginWrap-->
			<div class="dvLoginRegist">
				<h3>会員でない方</h3>
				<div>
				<%if (this.CartList.HasFixedPurchase) { %>
				<p>定期購入商品は会員様のみがご購入頂けるようになっております。</p>
				<p>ログインIDをお持ちでないお客様はこちらから会員登録を行ってください。</p>
				<% }else{ %>
				<p>会員登録がお済みで無い方はこちらから登録をお願いいたします。</p>
				<% } %>
				<div class="alignC" style="width:450px"><asp:LinkButton ID="lbUserRegist" runat="server" OnClick="lbUserRegist_Click" class="btn btn-large btn-inverse">新規会員登録</asp:LinkButton></div>
				</div>
				<p style="margin:3px 0 4px;"></p>
				<div class="alignC" style="width:450px">
					<%if (Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED) {%>
						<asp:LinkButton ID="lbUserEasyRegist" runat="server" OnClick="lbUserEasyRegist_Click" class="btn-org btn-large btn-org-blk">かんたん会員登録する</asp:LinkButton>
					<%} %>
				</div>
				<%if (this.CartList.HasFixedPurchase == false) { %>
				<div style="margin-top:30px;">
				<h3>会員登録せずに購入</h3>
				<p>会員でないお客様はこちらから購入者情報をご記入ください。</p>
				<div class="alignC" style="width:450px"><asp:LinkButton ID="lbOrderShipping" runat="server" 
						onclick="lbOrderShipping_Click" class="btn btn-large btn-success">ご購入手続き</asp:LinkButton></div>
				</div>
				<%} %>
				<% if (Constants.COMMON_SOCIAL_LOGIN_ENABLED) { %>
					<br class="clr" />
					<div style="margin-top:20px;" class="dvSocialLoginCooperation">
						<h3>ソーシャルログイン</h3>
						<p style="margin-bottom: 10px;">ソーシャルログイン連携によるログインは、こちらから行ってください。</p>
						<ul>
							<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
								<%-- Apple --%>
								<li style="margin-bottom: 10px; float: none;">
									<a class="social-login-apple apple-color"
										href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
											w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
											Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
											Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
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
								<li style="margin-bottom: 10px; float: none;">
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
								<li style="margin-bottom: 10px; float: none;">
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
								<li style="margin-bottom: 10px; float: none;">
									<a class="social-login google-color"
										href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK,
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
											<a href="<%=LineConnect(
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
									<p style="margin: 3px 0 0 0;">※LINE連携時に友だち追加します</p>
								</li>
							<% } %>
							<%-- Amazon --%>
							<% if (this.IsVisibleAmazonPayButton || this.IsVisibleAmazonLoginButton) { %>
								<li style="margin-bottom: 10px; float: none;">
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
								<li style="margin-bottom: 10px; float: none;">
									<%
										ucPaypalScriptsForm.LogoDesign = "Login";
										ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
										ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
									%>
									<div style="width: 296px;">
										<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
										<div id="paypal-button"></div>
										<% if (SessionManager.PayPalCooperationInfo != null) {%>
											<p style="margin: 3px 0 0 0;">※<%: SessionManager.PayPalCooperationInfo.AccountEMail %> 連携済</p>
										<% } else {%>
											<p style="margin: 3px 0 0 0;">※PayPalで新規登録/ログインします</p>
										<%} %>
									</div>
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
				<% } %>
			</div>
		</div>
	</div>
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
	};
	window.onAmazonPaymentsReady = function () {
		if ($('#AmazonPayButton').length) showButton();
	};

	<%-- Amazonボタン表示ウィジェット --%>
	function showButton() {
		var authRequest;
		OffAmazonPayments.Button("AmazonPayButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
			type: "<%= (this.IsVisibleAmazonPayButton) ? "PwA" : ((this.IsVisibleAmazonLoginButton) ? "LwA" : "") %>",
			color: "Gold",
			size: "large",
			authorization: function () {
				loginOptions = {
					scope: "payments:widget payments:shipping_address profile",
					popup: true,
					state: '<%= Request.RawUrl %>'
				};
				authRequest = amazon.Login.authorize(loginOptions, '<%= this.AmazonCallBackUrl %>');
			},
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		});
		$('#OffAmazonPaymentsWidgets0').css({ 'height': '44px', 'width': '218px' });
	};
</script>
<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
	<% } %>
<% } %>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
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
