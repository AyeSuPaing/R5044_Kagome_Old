<%--
=========================================================================================================
  Module      : ソーシャルログイン連携画面(SocialLoginCooperation.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="SocialLoginCooperation.aspx.cs" Inherits="Form_User_SocialLoginCooperation" Title="ソーシャルログイン連携ページ" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<% if (Constants.COMMON_SOCIAL_LOGIN_ENABLED) { %>
<div id="dvUserFltContents">
	<h2>ソーシャルログイン連携</h2>
	<div id="dvSocialLoginCooperation" class="unit">
		<p style="font-size:15px;line-height:1.5;margin-bottom:5px;">各種サービスとの連携設定を行えます。</p>
		<p class="msg">※連携をすべて解除する場合は、事前に「登録情報の変更」より<%: ReplaceTag("@@User.password.name@@") %>の設定を行ってください。</p>
		<p style="color: #D8000C;background-color: #FFBABA;padding:1em;margin:1em;border: 1px solid;" visible="<%# string.IsNullOrEmpty(this.WlErrorMessage.Text) == false %>" runat="server">
			<asp:Literal ID="lErrorMessage" runat="server"></asp:Literal>
		</p>
		<ul>
			<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
			<%-- Apple --%>
			<li>
				<div style="display:flex;justify-content:space-between;margin:1em;padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
					<img class="social-login-icon-apple"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_apple_black.png" />
					<span style="width: 78px; height:10px;color: #000000;padding: 1em 1.7em;text-align: center;text-decoration: none;display: inline-block;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Apple</span>
				<% if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple))
					{ %>
					<span style="width: 288px;display: inline-block;padding: 1em 1em;font-weight:bold;">ご利用状況：連携済</span>
					<asp:LinkButton id="lbDisconnectApple" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple %>" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
					<span style="width: 288px;display: inline-block;padding: 1em 1em;">ご利用状況：未連携</span>
					<asp:LinkButton id="lbConnectApple" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple %>" CssClass="btn btn-small" style="width:130px;background-color: #000000;color: #FFFFFF;display: inline-block;padding:1em 3em;">Appleと連携する</asp:LinkButton>
				<% } %>
				</div>
			</li>
			<%-- Facebook --%>
			<li>
				<div style="display:flex;justify-content:space-between;margin:1em;padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
					<img class="social-login-icon"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_facebook_primary.png" />
					<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align: center;text-decoration: none;display: inline-block;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Facebook</span>
				<% if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook))
				   { %>
					<span style="width: 288px;display: inline-block;padding: 1em 1em;font-weight:bold;">ご利用状況：連携済</span>
					<asp:LinkButton id="lbDisconnectFacebook" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook %>" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
					<span style="width: 288px;display: inline-block;padding: 1em 1em;">ご利用状況：未連携</span>
					<asp:LinkButton id="lbConnectFacebook" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook %>" CssClass="btn btn-small" style="width:130px;background-color: #1877f2;color: #FFFFFF;display: inline-block;padding:1em 3em;">Facebookと連携する</asp:LinkButton>
				<% } %>
				</div>
			</li>
			<%-- Twitter --%>
			<li>
				<div style="display:flex;justify-content:space-between;margin:1em;padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
					<img class="social-login-icon-x"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_x_black.png" />
					<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align: center;text-decoration: none;display: inline-block;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">X</span>
				<%if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter)) {%>
					<span style="width: 288px;display: inline-block;padding: 1em 1em;font-weight:bold;">ご利用状況：連携済</span>
					<asp:LinkButton id="lbDisconnectTwitter" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter %>" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
					<span style="width: 288px;display: inline-block;padding: 1em 1em;">ご利用状況：未連携</span>
					<asp:LinkButton id="lbConnectTwitter" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter %>" CssClass="btn btn-small" style="width:130px;background-color: #000000;color: #FFFFFF;display: inline-block;padding:1em 3em;">Xと連携する</asp:LinkButton>
				<% } %>
				</div>
			</li>
			<%-- Yahoo --%>
			<li>
				<div style="display:flex;justify-content:space-between;margin:1em;padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
					<img class="social-login-icon-yahoo"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_yahoo_red.png" />
					<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align:center;text-decoration:none;display:inline-block;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Yahoo!</span>
				<%if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo)) {%>
					<span style="width:288px;display:inline-block;padding:1em 1em;font-weight:bold">ご利用状況：連携済</span>
					<asp:LinkButton ID="lbDisconnectYahoo" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo %>" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
					<span style="width:288px;display:inline-block;padding:1em 1em;">ご利用状況：未連携</span>
					<asp:LinkButton ID="lbConnectYahoo" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo %>" CssClass="btn btn-small" style="width:130px;background-color: #FF0033;color: #FFFFFF;display: inline-block;padding:1em 3em;">Yahoo! Japanと連携する</asp:LinkButton>
				<% } %>
				</div>
			</li>
			<%-- Google --%>
			<li>
				<div style="display:flex;justify-content:space-between;margin:1em;padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
					<img class="social-login-icon"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_google.png" />
					<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align:center;text-decoration:none;display:inline-block;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Google</span>
					<%if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus)) {%>
						<span style="width:288px;display:inline-block;padding:1em 1em;font-weight:bold">ご利用状況：連携済</span>
						<asp:LinkButton ID="lbDisconnectGoogle" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus %>" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
					<%} else {%>
						<span style="width:288px;display:inline-block;padding:1em 1em;">ご利用状況：未連携</span>
						<asp:LinkButton ID="lbConnectGoogle" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus %>" CssClass="btn btn-small" style="width:130px;background-color: #FFFFFF;color: #000000;display: inline-block;padding:1em 3em;">Googleと連携する</asp:LinkButton>
					<% } %>
				</div>
			</li>
			<% } %>
			<%-- LINE --%>
			<% if (Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
			<li>
				<div style="border-bottom:1px solid #dcdcdc;margin:1em;padding:0 0 1em 0;">
					<div style="display:flex;">
						<img class="social-login-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_line_circular.png" />
						<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align:center;text-decoration:none;display:inline-block;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">LINE</span>
						<%if (this.IsCooperatedLine) { %>
							<span style="width: 280px;display: inline-block;padding: 1em 1em;font-weight:bold;">ご利用状況：連携済</span>
							<% if (this.IsLineDirectAutoLoginUser == false) { %>
								<asp:LinkButton id="lbDisconnectLine" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Line %>" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
							<% } else { %>
								<a class="btn btn-small disabled" style="padding:1em;">解除する</a>
								<p style="padding-top:1em;">LINE連携を解除する場合は、事前に「登録情報の変更」よりメールアドレスの設定を行ってください。</p>
							<% } %>
						<%} else {%>
							<span style="width: 280px;display: inline-block;padding: 1em 1em;">ご利用状況：未連携</span>
							<asp:LinkButton id="lbConnectLine" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Line %>" CssClass="btn btn-small" style="width:127px;background-color: #06C755;color: #FFFFFF;display: inline-block;padding:1em 3em;">LINEと連携する</asp:LinkButton>
						<% } %>
					</div>
					<div style="padding-top:1em;">※LINE連携時に友だち追加します</div>
				</div>
			</li>
			<% } %>
				<%-- Amazon --%>
			<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED) { %>
			<li>
				<div style="display:flex;justify-content:space-between;margin:1em;padding:0 0 1em 0;border-bottom:1px solid #dcdcdc; flex-wrap: wrap;">
				<img class="social-login-icon"
						src="<%= Constants.PATH_ROOT %>
						Contents\ImagesPkg\socialLogin\logo_amazon.png" />
				<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align:center;text-decoration:none;display:inline-block;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Amazon</span>
				<%if (IsCooperatedAmazon) {%>
				<span style="width: 280px;display: inline-block;padding: 1em 1em;font-weight:bold;">ご利用状況：連携済</span>
				<asp:LinkButton id="lbDisconnectAmazon" runat="server" OnClick="lbDisconnectAmazon_Click" OnClientClick="return disconnectAmazon()" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
				<div style="width: 280px;display: inline-block;padding: 1em 1em;">ご利用状況：未連携</div>
				<div>
					<%--▼▼Amazonログインボタンウィジェット▼▼--%>
					<div id="AmazonLoginButton" ></div>
					<div style="width:194px;display:inline-block;">
						<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
						<div id="AmazonLoginCv2Button"></div>
						<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
					</div>
					<%--▲▲Amazonログインボタンウィジェット▲▲--%>
				</div>
				<p style="padding-top:1em;"><%= (Constants.GLOBAL_OPTION_ENABLE && (this.IsLoginUserAddrJp == false)) ? "Amazonと連携する場合は、登録情報の変更から国をJapanに設定してください。" : "連携するには「Amazonアカウントでログイン」ボタンをクリックしてください。" %></p>
				<% }%>
				</div>
			</li>
			<% } %>
			<%-- PayPal --%>
			<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) { %>
			<li>
				<div style="border-bottom:1px solid #dcdcdc;margin:1em;padding:0 0 1em 0;">
					<div style="display:flex;">
						<img class="social-login-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_paypal_box.png" />
						<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align:center;text-decoration:none;display:inline-block;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Paypal</span>
						<% if (SessionManager.PayPalCooperationInfo != null) {%>
							<span style="width: 280px;display: inline-block;padding: 1em 1em;font-weight:bold;vertical-align: middle">ご利用状況：連携済<br/></span>
							<asp:LinkButton id="lbDisconnectPayPal" runat="server" OnClick="lbDisconnectPayPal_Click" OnClientClick="return confirm('本当によろしいですか？');" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
						<% } else {%>
							<span style="width: 280px;display: inline-block;padding: 1em 1em;">ご利用状況：未連携</span>
						<%-- ▼PayPalログインここから▼ --%>
						<%
							ucPaypalScriptsForm.LogoDesign = "Login";
							ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
						%>
						<div id="paypal-button" style="width: 190px;padding: 5px 0px 0px 0px"></div>
						<%-- ▲PayPalログインここまで▲ --%>
						<%} %>
						<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
						<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
					</div>
					<% if (SessionManager.PayPalCooperationInfo != null) {%>
						<div style="padding-top:1em;">（<%: "SessionManager.PayPalCooperationInfo.AccountEMail" %> に連携されています）</div>
					<% } else {%>
						<div style="padding-top:1em;">連携するにはPayPalボタンをクリックしてください。</div>
					<%} %>
				</div>
			</li>
			<% } %>
			<%-- 楽天Connect --%>
			<% if (Constants.RAKUTEN_LOGIN_ENABLED) { %>
			<li>
				<div style="border-bottom:1px solid #dcdcdc;margin:1em;padding:0 0 1em 0;">
					<div style="display:flex;">
						<img class="social-login-icon-paypal"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_rakuten.png" />
						<span style="width: 78px; height:10px;color: #000000;padding: 1em 2em;text-align:center;text-decoration:none;display:inline-block;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">楽天</span>
						<% if (this.IsRakutenIdConnectLinked == false){ %>
							<span style="width: 280px;display: inline-block;padding: 1em 1em;">ご利用状況：未連携</span>
							<p><asp:LinkButton ID="lbRakutenIdConnectLinked" runat="server" OnClick="lbRakutenIdConnectLinked_Click" Enabled="<%# this.IsRakutenIdConnectLinked == false %>">
								<img src="https://checkout.rakuten.co.jp/p/common/img/btn_idconnect_03.gif" style="width: 190px;"/></asp:LinkButton></p>
						<% } %>
						<% if (this.IsRakutenIdConnectLinked){ %>
							<span style="width: 362px;display: inline-block;padding: 1em 3em;font-weight:bold;">ご利用状況：連携済</span>
							<p><asp:LinkButton ID="lbRakutenIdConnectNotLinked" runat="server" OnClick="lbRakutenIdConnectNotLinked_Click" CssClass="btn btn-small" style="width:130px;display: inline-block;padding:1em 3em;" Enabled="<%# SessionManager.IsRakutenIdConnectRegisterUser == false %>">
								解除する</asp:LinkButton></p>
						<% } %>
					</div>
				<% if (this.IsDisplayRakutenIdConnectLinkedMessages){ %>
						<p style="padding-top:1em;">楽天会員IDとの紐づけが完了しました。楽天会員IDでログインが行えます。</p>
					<% } %>
					<% if (this.IsDisplayRakutenIdConnectNotLinkedMessages) { %>
						<p style="padding-top:1em;">楽天会員IDとの紐づけ解除が完了しました。</p>
					<% } %>
					<% if (SessionManager.IsRakutenIdConnectRegisterUser) { %>
						<p style="padding-top:1em;">楽天IDで新規登録したユーザーなので連携を解除することはできません。</p>
					<% } %>
				</div>
			</li>
			<% } %>
		</ul>
	</div>
</div>
<% } %>
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
	<%-- ▼▼Amazonウィジェット用スクリプト▼▼ --%>
	<script type="text/javascript">
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
				type: "LsA",
				color: "Black",
				size: "small",
				authorization: function () {
					loginOptions = {
						scope: "payments:shipping_address payments:widget profile",
						popup: true
					};
					authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_COOPERATION_CALLBACK) %>");
				},
				onError: function (error) {
					alert(error.getErrorMessage());
				}
			});
			<% if (Constants.GLOBAL_OPTION_ENABLE && (IsLoginUserAddrJp == false)) { %>
				$('#OffAmazonPaymentsWidgets0').removeAttr("onclick");
				$('#OffAmazonPaymentsWidgets0').css({
					opacity: 0.5,
					cursor: 'default'
				})
			<% } %>
		}

		<%--Amazon連携解除時処理--%>
		function disconnectAmazon() {
			if (confirm('本当によろしいですか？')) {
				amazon.Login.logout();
				return true;
			}
			return false;
		};
	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
		<% } %>
<% } %>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
</asp:Content>
