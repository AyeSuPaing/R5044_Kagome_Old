<%--
=========================================================================================================
  Module      : スマートフォン用ソーシャルログイン連携画面処理(SocialLoginCooperation.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/SocialLoginCooperation.aspx.cs" Inherits="Form_User_SocialLoginCooperation" Title="ソーシャルログイン連携ページ" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<% if (Constants.COMMON_SOCIAL_LOGIN_ENABLED) { %>
<section class="wrap-user social-login-cooperation">
<div class="user-unit">
	<h2>ソーシャルログイン連携</h2>
	<p class="msg">各種サービスとの連携設定を行えます。</p>
	<p class="msg">※連携をすべて解除する場合は、事前に「登録情報の変更」より<%: ReplaceTag("@@User.password.name@@") %>の設定を行ってください。</p>
	<div id="dvSocialLoginCooperation" class="unit" style="padding:0 1.0em 1.0em 1.0em;">
		<p style="color: #D8000C;background-color: #FFBABA;padding:1em;margin:1em;border: 1px solid;" visible="<%# string.IsNullOrEmpty(this.WlErrorMessage.Text) == false %>" runat="server"><asp:Literal ID="lErrorMessage" runat="server"></asp:Literal></p>
		<ul style="list-style:none;padding:0px;">
			<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
			<%-- Apple --%>
			<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em 1em 1em 0em;flex-wrap:wrap;">
					<img class="social-login-icon-apple"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_apple_black.png" />
					<% if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple)) { %>
						<div class="social-login-box social-login-box-connected">
							<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Apple</span>
							<br />連携済
						</div>
						<asp:LinkButton id="lbDisconnectApple" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple %>" CssClass="btn btn-small" style="color: #333333;background-color:#f5f5f5;width:125px;display:inline-block;padding:1em 3em;">解除する</asp:LinkButton>
					<%} else {%>
						<div class="social-login-box">
							<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Apple</span>
							<br />未連携
						</div>
						<asp:LinkButton id="lbConnectApple" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple %>" CssClass="btn btn-small" style="width:125px;background-color: #000000;color: #FFFFFF;display: inline-block;padding:1em 3em;">Appleと連携する</asp:LinkButton>
					<% } %>
				</div>
			</li>
				<%-- Facebook --%>
			<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em 1em 1em 0em;flex-wrap:wrap;">
					<img class="social-login-icon social-login-icon-facebook"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_facebook_primary.png" />
					<%if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter)) {%>
					<div class="social-login-box social-login-box-connected">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Facebook</span>
						<br />連携済
					</div>
					<asp:LinkButton id="lbDisconnectFacebook" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook %>" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
					<div class="social-login-box">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Facebook</span>
						<br />未連携
					</div>
					<asp:LinkButton id="lbConnectFacebook" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook %>" CssClass="btn btn-small" style="width:125px;background-color: #1877f2;color: #FFFFFF;display: inline-block;padding:1em 3em;">Facebookと連携する</asp:LinkButton>
				<%}%>
				</div>
			</li>
			<%-- Twitter --%>
			<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em;flex-wrap:wrap;">
					<img class="social-login-icon"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_x_black.png" />
				<%if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter)) {%>
					<div class="social-login-box social-login-box-connected">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">X</span>
						<br />連携済
					</div>
					<asp:LinkButton id="lbDisconnectTwitter" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter %>" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
					<div class="social-login-box">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">X</span>
						<br />未連携
					</div>
					<asp:LinkButton id="lbConnectTwitter" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter %>" CssClass="btn btn-small" style="width:125px;background-color: #000000;color: #FFFFFF;display: inline-block;padding:1em 3em;">Xと連携する</asp:LinkButton>
				<%}%>
				</div>
			</li>
			<%-- Yahoo --%>
			<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em;flex-wrap:wrap;">
					<img class="social-login-icon-yahoo"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_yahoo_red.png" />
				<%if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo)) {%>
					<div class="social-login-box social-login-box-connected">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Yahoo!</span>
						<br />連携済
					</div>
					<asp:LinkButton id="lbDisconnectYahoo" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo %>" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
				<%} else {%>
					<div class="social-login-box">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Yahoo!</span>
						<br />未連携
					</div>
					<asp:LinkButton id="lbConnectYahoo" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo %>" CssClass="btn btn-small" style="width:176px;background-color: #FF0033;color: #FFFFFF;display: inline-block;padding:1em 1em;">Yahoo! Japanと連携する</asp:LinkButton>
				<%}%>
				</div>
			</li>
			<%-- Google --%>
				<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em;flex-wrap:wrap;">
						<img class="social-login-icon"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_google.png" />
						<%if (Providers.Contains(w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus)) {%>
					<div class="social-login-box social-login-box-connected">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Google</span>
						<br />連携済
					</div>
							<asp:LinkButton id="lbDisconnectGoogle" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus %>" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
						<%} else {%>
					<div class="social-login-box">
						<span style="color: #000000;font-size: 13px;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Google</span>
						<br />未連携
					</div>
							<asp:LinkButton id="lbConnectGoogle" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus %>" CssClass="btn btn-small" style="width:125px;background-color: #FFFFFF;color: #000000;display: inline-block;padding:1em 3em;">Googleと連携する</asp:LinkButton>
						<%}%>
					</div>
				</li>
			<% } %>
			<% if (Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
			<%-- LINE --%>
			<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em;flex-wrap: wrap;">
					<img class="social-login-icon"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_line_circular.png" />
					<%if (IsCooperatedLine) {%>
					<div class="social-login-box social-login-box-connected">
						<span style="color: #000000;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">LINE</span>
						<br />連携済
					</div>
						<% if (this.IsLineDirectAutoLoginUser == false) { %>
							<asp:LinkButton id="lbDisconnectLine" runat="server" OnClick="lbDisconnect_Click" OnClientClick="return confirm('本当によろしいですか？');" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Line %>" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
						<% } else { %>
							<p>LINE連携を解除する場合は、事前に「登録情報の変更」よりメールアドレスの設定を行ってください。</p>
						<% } %>
					<%} else {%>
					<div class="social-login-box">
						<span style="color: #000000;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">LINE</span>
						<br />未連携
					</div>
						<asp:LinkButton id="lbConnectLine" runat="server" OnClick="lbConnect_Click" CommandArgument="<%#: w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Line %>" CssClass="btn btn-small" style="width:124px;background-color: #06C755;color: #FFFFFF;display: inline-block;padding:1em 3em;">LINEと連携する</asp:LinkButton>
					<%}%>
					<p style="padding-top:1em;">※LINE連携時に友だち追加します</p>
				</div>
			</li>
			<% } %>
			<%-- Amazon --%>
			<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED) { %>
				<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em 1em 1em 1em;flex-wrap:wrap;">
				<img class="social-login-icon"
						src="<%= Constants.PATH_ROOT %>
						Contents\ImagesPkg\socialLogin\logo_amazon.png" />
					<%if (IsCooperatedAmazon) {%>
					<div class="social-login-box-box-amazon social-login-box-connected">
						<span style="color: #000000;text-decoration:none;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Amazon</span>
						<br />連携済
					</div>
							<asp:LinkButton id="lbDisconnectAmazon" runat="server" OnClick="lbDisconnectAmazon_Click" OnClientClick="return disconnectAmazon()" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
						<%} else {%>
					<div class="social-login-box-box-amazon">
						<span style="color: #000000;text-decoration:none;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Amazon</span>
						<br />未連携
					</div>
							<%-- ▼▼Amazonログインボタンウィジェット▼▼ --%>
							<div id="AmazonLoginButton" style="margin-bottom:5px;text-align: center;"></div>
							<div style="width:210px;display:inline-block;">
								<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
								<div id="AmazonLoginCv2Button" style="display: inline-block; width: 100%;"></div>
								<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
							</div>
							<p style="padding-top:1em;"><%= (Constants.GLOBAL_OPTION_ENABLE && (this.IsLoginUserAddrJp == false)) ? "Amazonと連携する場合は、登録情報の変更から国をJapanに設定してください。" : "" %></p>
							<%-- ▲▲Amazonログインボタンウィジェット▲▲ --%>
						<%}%>
					</div>
				</li>
			<% } %>
			<%-- PayPal --%>
			<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) { %>
				<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
					<div style="display:flex;justify-content:center;margin:1em 0em 1em 1em;flex-wrap:wrap;">
						<img class="social-login-icon-paypal"
								src="<%= Constants.PATH_ROOT %>
								Contents\ImagesPkg\socialLogin\logo_paypal_box.png" />
						<%-- ▼PayPalログインここから▼ --%>
						<%
							ucPaypalScriptsForm.LogoDesign = "Login";
							ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
						%>
						<% if (SessionManager.PayPalCooperationInfo != null) {%>
							<div class="social-login-box social-login-box-connected">
								<span style="color: #000000;text-decoration:none;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Paypal</span>
								<br />連携済
							</div>
							<asp:LinkButton id="lbDisconnectPayPal" runat="server" OnClick="lbDisconnectPayPal_Click" OnClientClick="return confirm('本当によろしいですか？');" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;">解除する</asp:LinkButton>
						<% } else {%>
					<div class="social-login-box">
						<span style="color: #000000;text-decoration:none;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">PayPal</span>
						<br />未連携
					</div>
							<div id="paypal-button" style="width: 205px;display:inline-block;"></div>
							<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
						<%} %>
						<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
						<%-- ▲PayPalログインここまで▲ --%>
					</div>
				</li>
			<% } %>
			<%-- 楽天Connect --%>
			<% if (Constants.RAKUTEN_LOGIN_ENABLED) { %>
			<li style="padding:0 0 1em 0;border-bottom:1px solid #dcdcdc;">
				<div style="display:flex;justify-content:center;margin:1em;flex-wrap:wrap;">
					<img class="social-login-icon-paypal"
							src="<%= Constants.PATH_ROOT %>
							Contents\ImagesPkg\socialLogin\logo_rakuten.png" />
					
					<% if (this.IsRakutenIdConnectLinked == false){ %>
					<div class="social-login-box " style="width: 57px; height:10px;padding: 0.5em 1em;text-align: center;text-decoration: none;display: inline-block;">
						<span style="color: #000000;text-decoration:none;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">楽天</span>
						<br />未連携
					</div>
						<p><asp:LinkButton ID="lbRakutenIdConnectLinked" runat="server" OnClick="lbRakutenIdConnectLinked_Click">
							<img src="https://checkout.rakuten.co.jp/p/common/img/btn_idconnect_03.gif" style="width: 205px; object-fit: cover;"/></asp:LinkButton></p>
					<% } %>
					<% if (this.IsRakutenIdConnectLinked){ %>
						<div class="social-login-box social-login-box-connected">
							<span style="color: #000000;text-decoration:none;font-family: 'ヒラギノ角ゴ Pro W3', 'Hiragino Kaku Gothic Pro', 'メイリオ', Meiryo, Osaka, 'ＭＳ Ｐゴシック', 'MS PGothic', sans-serif;">Rakuten</span>
							<br />連携済
						</div>
						<asp:LinkButton ID="lbRakutenIdConnectNotLinked" runat="server" OnClick="lbRakutenIdConnectNotLinked_Click" CssClass="btn btn-small" style="  color: #333333;  background-color: #f5f5f5;width:125px;display: inline-block;padding:1em 3em;" Enabled="<%# SessionManager.IsRakutenIdConnectRegisterUser == true %>">解除する</asp:LinkButton>
					<% } %>
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

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
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
				size: "Large",
				authorization: function () {
					loginOptions = {
						scope: "payments:shipping_address payments:widget profile",
						popup: false
					};
					authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_COOPERATION_CALLBACK) %>");
				},
				onError: function (error) {
					alert(error.getErrorMessage());
				}
			});
			<% if (Constants.GLOBAL_OPTION_ENABLE && (this.IsLoginUserAddrJp == false)) { %>
			$('#OffAmazonPaymentsWidgets0').removeAttr("onclick");
			$('#OffAmazonPaymentsWidgets0').css({
				opacity: 0.5
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
	<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
		<% } %>
<% } %>
</asp:Content>
