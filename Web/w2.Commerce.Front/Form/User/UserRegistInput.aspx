 <%--
=========================================================================================================
  Module      : 会員登録入力画面(UserRegistInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendRegist" Src="~/Form/Common/User/BodyUserExtendRegist.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistInput.aspx.cs" Inherits="Form_User_UserRegistInput" Title="会員新規登録入力ページ" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="dvUserContents">
	<%-- 会員登録系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p><img src="../../Contents/ImagesPkg/user/clumbs_regist_2.gif" alt="会員情報の入力" /></p>
	</div>

	<h2>会員情報の入力</h2>

	<div id="dvUserRegistInput" class="unit">
		
		<%-- メッセージ --%>
		<div class="dvContentsInfo">
			<p>
				<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>会員へのお申込みにあたっては、以下の項目にご入力が必要です。<br />
				下記の項目に入力の上、「確認する」ボタンを押して下さい。</p>
		</div>
		
		<% if ((Constants.SOCIAL_LOGIN_ENABLED || (Constants.RAKUTEN_LOGIN_ENABLED && (this.IsAfterOrder == false)) || Constants.PAYPAL_LOGINPAYMENT_ENABLED || (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED) || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) && (SessionManager.SocialLogin == null) && (SessionManager.HasTemporaryUserId == false)) { %>
		<%-- ソーシャルログイン連携 --%>
		<div class="dvSocialLoginCooperation">
			<h3>ソーシャルログイン連携</h3>
			<ul style="display: flex; margin-bottom:30px; flex-wrap: wrap;">
				<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
					<%-- Apple --%>
					<li style="margin: 5px;">
						<a class="social-login-registinput apple-color"
							href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								true,
								Request.Url.Authority) %>">
							<div class="social-icon-width">
								<img class="apple-icon-registinput"
									src="<%= Constants.PATH_ROOT %>
									Contents\ImagesPkg\socialLogin\logo_apple.png" />
							</div>
							<div class="apple-label">Appleでサインイン</div>
						</a>
					</li>
					<%-- Facebook --%>
					<li style="margin: 5px;">
						<a class="social-login-registinput facebook-color"
							href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								true,
								Request.Url.Authority) %>">
							<div class="social-icon-width">
								<img class="facebook-icon-registinput"
									src="<%= Constants.PATH_ROOT %>
									Contents\ImagesPkg\socialLogin\logo_facebook.png" />
							</div>
							<div class="social-login-label">Facebookでログイン</div>
						</a>
					</li>
					<%-- Twitter --%>
					<li style="margin: 5px;">
						<a class="social-login-registinput twitter-color"
							href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								true,
								Request.Url.Authority) %>">
							<div class="social-icon-width">
								<img class="twittericon-registinput"
									src="<%= Constants.PATH_ROOT %>
									Contents\ImagesPkg\socialLogin\logo_x.png" />
							</div>
							<div class="twitter-label">Xでログイン</div>
						</a>
					</li>
					<%-- Yahoo --%>
					<li style="margin: 5px;">
						<a class="social-login-registinput yahoo-color"
							href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
								true,
								Request.Url.Authority) %>">
							<div class="social-icon-width">
								<img class="yahoo-icon-registinput"
									src="<%= Constants.PATH_ROOT %>
									Contents\ImagesPkg\socialLogin\logo_yahoo.png" />
							</div>
							<div class="social-login-label-registinput">Yahoo! JAPAN IDでログイン</div>
						</a>
					</li>
					<%-- Google --%>
					<li style="margin: 5px;">
						<a class="social-login-registinput google-color"
							href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
										w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
										true,
								Request.Url.Authority) %>">
							<div class="social-icon-width">
								<img class="google-icon-registinput"
									src="<%= Constants.PATH_ROOT %>
									Contents\ImagesPkg\socialLogin\logo_google.png" />
							</div>
							<div class="google-label-registinput">Sign in with Google</div>
						</a>
					</li>
				<% } %>
				<% if ((Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) && String.IsNullOrEmpty(SessionManager.LineProviderUserId)) { %>
					<%-- LINE --%>
					<li style="margin: 5px;">
						<div class="social-login-registinput line-color">
							<div class="social-login-registinput line-hover-color line-active-color">
								<a href="<%= LineConnect(
									w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION
										? Constants.PAGE_FRONT_DEFAULT
										: Constants.PAGE_FRONT_USER_REGIST_INPUT,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK,
									true,
									Request.Url.Authority) %>">
									<div class="social-icon-width">
										<img class="line-icon-registinput"
											src="<%= Constants.PATH_ROOT %>
											Contents\ImagesPkg\socialLogin\logo_line.png" />
									</div>
									<div class="social-login-label-registinput">LINEでログイン</div>
								</a>
							</div>
						</div>
						<p style="margin-top:3px">※LINE連携時に友だち追加します</p>
					</li>
				<% } %>
				<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
				<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
					<li style="margin: 5px;">
						<div style="display: inline-block; width: 280px;">
							<div id="AmazonLoginCv2Button"></div>
						</div>
					</li>
				<% } %>
				<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
				<%-- PayPal --%>
				<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) { %>
					<%-- PayPal --%>
					<li style="margin: 5px;">
						<%-- ▼PayPalここから▼ --%>
						<%
							ucPaypalScriptsForm.LogoDesign = "Login";
							ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
							ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
						%>
						<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
						<div style="width: 280px;">
							<div id="paypal-button"></div>
							<% if (SessionManager.PayPalCooperationInfo != null) { %>
								<p style="margin-top:3px;">※<%: SessionManager.PayPalCooperationInfo.AccountEMail %> 連携済</p>
							<% } else { %>
								<p style="margin-top:3px;">※PayPalで新規登録/ログインします</p>
							<% } %>
						</div>
						<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click" />
						<%-- ▲PayPalここまで▲ --%>
					</li>
				<% } %>
				<%-- ▼楽天Connect▼ --%>
				<% if (Constants.RAKUTEN_LOGIN_ENABLED && (this.IsAfterOrder == false)) { %>
					<li style="margin: 5px;">
						<p style="text-align: center;">
							<asp:LinkButton runat="server" ID="lbRakutenIdConnectRequestAuth" OnClick="lbRakutenIdConnectRequestAuth_OnClick">
								<img src="https://static.id.rakuten.co.jp/static/btn-japanese-2x/idconnect_02-new_r_280.png" alt="楽天IDで新規登録"/>
							</asp:LinkButton>
							<% if (this.IsRakutenIdConnectUserRegister) { %>
								<br/>※<%: SessionManager.RakutenIdConnectActionInfo.RakutenIdConnectUserInfoResponseData.Email %> 連携済<br/>
							<% } %>
						</p>
					</li>
				<% } %>
				<%-- ▲楽天Connect▲ --%>
			</ul>
		</div>
		<% } %>

		<%-- お客様情報入力フォーム --%>
		<div class="dvUserInfo">	
			<h3>お客様情報</h3>
			<ins><span class="necessary">*</span>は必須入力となります。</ins>

		<%-- UPDATE PANEL開始 --%>
		<asp:UpdatePanel ID="upUpdatePanel" runat="server">
		<ContentTemplate>
			<table cellspacing="0">
				<tr>
					<%-- 氏名 --%>
					<th>
						<%: ReplaceTag("@@User.name.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_name"/>
					</th>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
									<% SetMaxLength(this.WtbUserName1, "@@User.name1.length_max@@"); %>
									<span class="fname">姓</span><asp:TextBox id="tbUserName1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(this.WtbUserName2, "@@User.name2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox id="tbUserName2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：山田</span></td>
								<td><span class="notes">太郎</span></td>
							</tr>
						</table>
						<asp:CustomValidator
							ID="cvUserName1"
							runat="Server"
							ControlToValidate="tbUserName1"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserName2"
							runat="Server"
							ControlToValidate="tbUserName2"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsUserAddrJp) { %>
				<tr>
					<%-- 氏名（かな） --%>
					<th>
						<%: ReplaceTag("@@User.name_kana.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_kana"/>
					</th>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
									<% SetMaxLength(this.WtbUserNameKana1, "@@User.name_kana1.length_max@@"); %>
									<span class="fname">姓</span><asp:TextBox id="tbUserNameKana1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(this.WtbUserNameKana2, "@@User.name_kana2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox id="tbUserNameKana2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角ひらがな入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：やまだ</span></td>
								<td><span class="notes">たろう</span></td>
							</tr>
						</table>
						<asp:CustomValidator
							ID="cvUserNameKana1"
							runat="Server"
							ControlToValidate="tbUserNameKana1"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserNameKana2"
							runat="Server"
							ControlToValidate="tbUserNameKana2"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<%if (Constants.PRODUCTREVIEW_ENABLED) { %>
				<tr>
					<%-- ニックネーム --%>
					<th>
						<%: ReplaceTag("@@User.nickname.name@@") %>
					</th>
					<td>
						<asp:TextBox id="tbUserNickName" runat="server" MaxLength="20" CssClass="nickname"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserNickName"
							runat="Server"
							ControlToValidate="tbUserNickName"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<%} %>
				<tr>
					<%-- 生年月日 --%>
					<th>
						<%: ReplaceTag("@@User.birth.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_birth"/>
					</th>
					<td>
						<asp:DropDownList id="ddlUserBirthYear" runat="server" CssClass="year" onchange="changeDropDownDays()"></asp:DropDownList>年
						<asp:DropDownList id="ddlUserBirthMonth" runat="server" CssClass="month" onchange="changeDropDownDays()"></asp:DropDownList>月
						<asp:DropDownList id="ddlUserBirthDay" runat="server" CssClass="date"></asp:DropDownList>日
						<asp:CustomValidator
							ID="cvUserBirthYear"
							runat="Server"
							ControlToValidate="ddlUserBirthYear"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserBirthMonth"
							runat="Server"
							ControlToValidate="ddlUserBirthMonth"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserBirthDay"
							runat="Server"
							ControlToValidate="ddlUserBirthDay"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- 性別 --%>
					<th>
						<%: ReplaceTag("@@User.sex.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_sex"/>
					</th>
					<td>
						<asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" CellSpacing="0" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
						<asp:CustomValidator
							ID="cvUserSex"
							runat="Server"
							ControlToValidate="rblUserSex"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- PCメールアドレス --%>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_mail_addr"/>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddr" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
						<span class="notes">※ログイン時に利用します</span>
						<%} %>
						<asp:CustomValidator
							ID="cvUserMailAddr"
							runat="Server"
							ControlToValidate="tbUserMailAddr"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserMailAddrForCheck"
							runat="Server"
							ControlToValidate="tbUserMailAddr"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- PCメールアドレス（確認用） --%>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>（確認用）
						<span class="necessary">*</span><span id="efo_sign_mail_addr_conf"/>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddrConf" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserMailAddrConf"
							runat="Server"
							ControlToValidate="tbUserMailAddrConf"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
				<tr>
					<%-- モバイルメールアドレス --%>
					<th>
						<%: ReplaceTag("@@User.mail_addr2.name@@") %>
						<span class="necessary" style="display:<%= (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) ? "" : "none" %>">*</span>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddr2" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserMailAddr2"
							runat="Server"
							ValidationGroup="UserRegist"
							ControlToValidate="tbUserMailAddr2"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- モバイルメールアドレス（確認用） --%>
					<th>
						<%: ReplaceTag("@@User.mail_addr2.name@@") %>（確認用）
						<span class="necessary" style="display:<%= (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) ? "" : "none" %>">*</span>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddr2Conf" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserMailAddr2Conf"
							runat="Server"
							ControlToValidate="tbUserMailAddr2Conf"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
				<tr>
					<%-- 国 --%>
					<th>
						<%: ReplaceTag("@@User.country.name@@", this.UserAddrCountryIsoCode) %>
					<span class="necessary">*</span>
					</th>
					<td>
						<asp:DropDownList id="ddlUserCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUserAddrCountry_SelectedIndexChanged"/>
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
						<span id="countryAlertMessage" class="notes" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
					</td>
				</tr>
				<% } %>
				<% if (this.IsAmazonLoggedIn && Constants.AMAZON_LOGIN_OPTION_ENABLED && (Constants.AMAZON_PAYMENT_CV2_ENABLED == false)) { %>
					<tr>
						<th>
							<%: ReplaceTag("@@User.addr.name@@") %>
						</th>
						<td>
							<%--▼▼Amazonアドレス帳ウィジェット▼▼--%>
							<div id="addressBookWidgetDiv" style="width:100%;height:300px;"></div>
							<div id="addressBookErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
							<%--▲▲Amazonアドレス帳ウィジェット▲▲--%>
							<%--▼▼AmazonリファレンスID格納▼▼--%>
							<asp:HiddenField runat="server" ID="hfAmazonOrderRefID" />
							<%--▲▲AmazonリファレンスID格納▲▲--%>
						</td>
					</tr>
				<% } else { %>
				<% if (this.IsUserAddrJp) { %>
				<tr>
					<%-- 郵便番号 --%>
					<th>
						<%: ReplaceTag("@@User.zip.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_zip"/>
					</th>
					<td valign="middle">
						<asp:TextBox ID="tbUserZip" MaxLength="8" Type="tel" OnTextChanged="lbSearchAddr_Click" runat="server" />
						<asp:LinkButton ID="lbSearchAddr" runat="server" OnClick="lbSearchAddr_Click" class="btn btn-mini" OnClientClick="return false;">
							住所検索</asp:LinkButton><br/>
						<%--検索結果レイヤー--%>
						<uc:Layer ID="ucLayer" runat="server" />
						<asp:CustomValidator
							ID="cvUserZip1"
							runat="Server"
							ValidationGroup="UserRegist"
							ControlToValidate="tbUserZip"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline zip_input_error_message" />
						<span style="color :Red" id="addrSearchErrorMessage" class="shortZipInputErrorMessage">
							<%: this.ZipInputErrorMessage %></span>
					</td>
				</tr>

				<tr>
					<%-- 都道府県 --%>
					<th>
						<%: ReplaceTag("@@User.addr1.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_addr1"/>
					</th>
					<td>
						<asp:DropDownList id="ddlUserAddr1" runat="server" CssClass="district"></asp:DropDownList>
						<asp:CustomValidator
							ID="cvUserAddr1"
							runat="Server"
							ControlToValidate="ddlUserAddr1"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<%-- 市区町村 --%>
					<th>
						<%: ReplaceTag("@@User.addr2.name@@", this.UserAddrCountryIsoCode) %>
						<span class="necessary">*</span><% if (this.IsUserAddrJp) { %><span id="efo_sign_addr2"/><% } %>
					</th>
					<td>
						<% if (IsCountryTw(this.UserAddrCountryIsoCode)) { %>
							<asp:DropDownList runat="server" ID="ddlUserAddr2" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlUserAddr2_SelectedIndexChanged"></asp:DropDownList>
						<% } else { %>
							<% SetMaxLength(this.WtbUserAddr2, "@@User.addr2.length_max@@"); %>
							<asp:TextBox id="tbUserAddr2" Runat="server" CssClass="addr"></asp:TextBox><span class="notes">※全角入力</span>
							<asp:CustomValidator
								ID="cvUserAddr2"
								runat="Server"
								ControlToValidate="tbUserAddr2"
								ValidationGroup="UserRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 番地 --%>
					<th>
						<%: ReplaceTag("@@User.addr3.name@@", this.UserAddrCountryIsoCode) %>
						<% if (IsAddress3Necessary(this.UserAddrCountryIsoCode)){ %><span class="necessary">*</span><span id="efo_sign_addr3"/><% } %>
					</th>
					<td>
						<% if (IsCountryTw(this.UserAddrCountryIsoCode)) { %>
							<asp:DropDownList runat="server" ID="ddlUserAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
						<% } else { %>
							<% SetMaxLength(this.WtbUserAddr3, "@@User.addr3.length_max@@"); %>
							<asp:TextBox id="tbUserAddr3" Runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
							<asp:CustomValidator
								ID="cvUserAddr3"
								runat="Server"
								ControlToValidate="tbUserAddr3"
								ValidationGroup="UserRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<% if (Constants.DISPLAY_ADDR4_ENABLED) { %>
				<tr>
					<%-- ビル・マンション名 --%>
					<th>
						<%: ReplaceTag("@@User.addr4.name@@", this.UserAddrCountryIsoCode) %>
						<% if (this.IsUserAddrJp == false) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserAddr4, "@@User.addr4.length_max@@"); %>
						<asp:TextBox id="tbUserAddr4" Runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserAddr4"
							runat="Server"
							ControlToValidate="tbUserAddr4"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsUserAddrJp == false) { %>
				<tr>
					<%-- 州 --%>
					<th><%: ReplaceTag("@@User.addr5.name@@", this.UserAddrCountryIsoCode) %>
						<% if (this.IsUserAddrUs) { %><span class="necessary">*</span><%} %>
					</th>
					<td>
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
								CssClass="error_inline" />
					<% } else { %>
						<asp:TextBox runat="server" ID="tbUserAddr5" ></asp:TextBox>
					<% } %>
					</td>
				</tr>
				<tr>
					<%-- 郵便番号（海外向け） --%>
					<th>
						<%: ReplaceTag("@@User.zip.name@@", this.UserAddrCountryIsoCode) %>
						<% if (this.IsUserAddrZipNecessary) { %><span class="necessary">*</span><% } %>
					</th>
					<td valign="middle">
						<asp:TextBox id="tbUserZipGlobal" Runat="server" MaxLength="20" Type="tel"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserZipGlobal"
							runat="Server"
							ControlToValidate="tbUserZipGlobal"
							ValidationGroup="UserRegistGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline"/>
						<asp:LinkButton
							ID="lbSearchAddrFromZipGlobal"
							OnClick="lbSearchAddrFromZipGlobal_Click"
							Style="display:none;"
							runat="server" />
					</td>
				</tr>
				<% } %>
				<% } %>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
				<tr>
					<%-- 企業名 --%>
					<th>
						<%: ReplaceTag("@@User.company_name.name@@") %>
						<span class="necessary"></span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserCompanyName, "@@User.company_name.length_max@@"); %>
						<asp:TextBox id="tbUserCompanyName" Runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserCompanyName"
							runat="Server"
							ControlToValidate="tbUserCompanyName"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- 部署名 --%>
					<th>
						<%: ReplaceTag("@@User.company_post_name.name@@") %>
						<span class="necessary"></span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserCompanyPostName, "@@User.company_post_name.length_max@@"); %>
						<asp:TextBox id="tbUserCompanyPostName" Runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserCompanyPostName"
							runat="Server"
							ControlToValidate="tbUserCompanyPostName"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<%} %>
				<% if (this.IsUserAddrJp) { %>
				<tr>
					<%-- 電話番号 --%>
					<th><%: ReplaceTag("@@User.tel1.name@@") %><span class="necessary">*</span><span id="efo_sign_tel1"/></th>
					<td>
						<asp:TextBox ID="tbUserTel1_1" MaxLength="13" Type="tel" runat="server" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvUserTel1')" />
						<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
							<span class="notes">※<%= WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE) %></span>
						<% } %>
						<% if (this.IsDisplayPersonalAuthentication) { %>
						<asp:LinkButton
							ID="lbGetAuthenticationCode"
							class="btn btn-mini"
							runat="server"
							Text="認証コードの取得"
							OnClick="lbGetAuthenticationCode_Click"
							OnClientClick="return checkTelNoInput();" />
						<br />
						<asp:Label ID="lbAuthenticationStatus" runat="server" />
						<% } %>
						<asp:CustomValidator
							ID="cvUserTel1"
							runat="Server"
							ControlToValidate="tbUserTel1_1"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsDisplayPersonalAuthentication) { %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.authentication_code.name@@") %><span class="necessary">*</span>
					</th>
					<td>
						<% SetMaxLength(this.WtbAuthenticationCode, "@@User.authentication_code.length_max@@"); %>
						<asp:TextBox ID="tbAuthenticationCode" CssClass="input_widthA" Enabled="false" runat="server" />
						<span class="notes">
							<% if (this.HasAuthenticationCode) { %>
							<asp:Label ID="lbHasAuthentication" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
							<% } %>
							<span><%: GetVerificationCodeNote(this.UserAddrCountryIsoCode) %></span>
							<asp:Label ID="lbAuthenticationMessage" runat="server" />
						</span>
						<asp:CustomValidator
							ID="cvAuthenticationCode"
							runat="Server"
							ControlToValidate="tbAuthenticationCode"
							ValidationGroup="UserRegist"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<%-- 電話番号（予備） --%>
					<th><%: ReplaceTag("@@User.tel2.name@@") %></th>
					<td>
						<asp:TextBox ID="tbUserTel1_2" MaxLength="13" Type="tel" runat="server" CssClass="shortTel" />
						<asp:CustomValidator
							ID="cvUserTel2_1"
							runat="Server"
							ControlToValidate="tbUserTel1_2"
							ValidationGroup="UserRegist"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } else { %>
				<tr>
					<%-- 電話番号 --%>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox id="tbUserTel1Global" Runat="server" MaxLength="16" Type="tel" onchange="resetAuthenticationCodeInput('cvUserTel1Global')" />
						<% if (this.IsDisplayPersonalAuthentication) { %>
						<asp:LinkButton
							ID="lbGetAuthenticationCodeGlobal"
							class="btn btn-mini"
							runat="server"
							Text="認証コードの取得"
							OnClick="lbGetAuthenticationCode_Click"
							OnClientClick="return checkTelNoInput();" />
						<br />
						<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
						<% } %>
						<asp:CustomValidator
							ID="cvUserTel1Global"
							runat="Server"
							ControlToValidate="tbUserTel1Global"
							ValidationGroup="UserRegistGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsDisplayPersonalAuthentication) { %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.authentication_code.name@@") %><span class="necessary">*</span>
					</th>
					<td>
						<% SetMaxLength(this.WtbAuthenticationCodeGlobal, "@@User.authentication_code.length_max@@"); %>
						<asp:TextBox ID="tbAuthenticationCodeGlobal" CssClass="input_widthA" Enabled="false" runat="server" />
						<span class="notes">
							<% if (this.HasAuthenticationCode) { %>
							<asp:Label ID="lbHasAuthenticationGlobal" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
							<% } %>
							<span><%: GetVerificationCodeNote(this.UserAddrCountryIsoCode) %></span>
							<asp:Label ID="lbAuthenticationMessageGlobal" runat="server" />
						</span>
						<asp:CustomValidator
							ID="cvAuthenticationCodeGlobal"
							runat="Server"
							ControlToValidate="tbAuthenticationCodeGlobal"
							ValidationGroup="UserRegistGlobal"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<%-- 電話番号（予備） --%>
					<th><%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %></th>
					<td>
						<asp:TextBox id="tbUserTel2Global" Runat="server" MaxLength="30" Type="tel"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserTel2Global"
							runat="Server"
							ControlToValidate="tbUserTel2Global"
							ValidationGroup="UserRegistGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_flg.name@@") %>
					</th>
					<td class="checkBox">
						<asp:CheckBox ID="cbUserMailFlg" Text=" 希望する " CssClass="checkBox" runat="server" />
					</td>
				</tr>
				<%-- ユーザー拡張項目　HasInput:true(入力画面)/false(確認画面)　HasRegist:true(新規登録)/false(登録編集) --%>
				<uc:BodyUserExtendRegist ID="ucBodyUserExtendRegist" HasInput="true" HasRegist="true" ExistingUser="<%# this.LoginUser %>" runat="server" />
			</table>
		<%-- ソーシャルログイン用 --%>
		<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
		<asp:HiddenField ID="hfSocialLoginJson" runat="server" />
		<% } %>
		<% if (this.IsDisplayPersonalAuthentication) { %>
		<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
		<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
		<% } %>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで --%>
		
		</div>
		<%--Form GMO--%>
		<div class="dvUserInfo">
			<asp:UpdatePanel ID="upGmoUpdatePanel" runat="server">
				<ContentTemplate>
					<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
						<% if (this.IsUserAddrJp) { %>
						<h3 id="H1">
							<asp:CheckBox ID="IsBusinessOwner" Text=" GMO枠保証を希望する" CssClass="checkBox" OnCheckedChanged="checkBusinessOwnerChangedEvent"  runat="server" AutoPostBack="True" Checked="true" />
						</h3>
						<% if (this.WcbIsBusinessOwner.Checked){ %>
							<table cellspacing="0">
								<tr>
									<th>
										<%: ReplaceTag("@@User.OwnerName1.name@@") %>
											<span class="necessary">*</span>
									</th>
									<td>
										<asp:TextBox id="tbOwnerName1" Runat="server" MaxLength="21" Type="text" width="250"></asp:TextBox>
										<asp:CustomValidator
											ID="cvpresidentNameFamily"
											runat="Server"
											ControlToValidate="tbOwnerName1"
											ValidationGroup="UserRegist"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											CssClass="error_inline" />
									</td>
								</tr>

								<tr>
									<th>
										<%: ReplaceTag("@@User.OwnerName2.name@@") %>
											<span class="necessary">*</span>
									</th>
									<td>
										<asp:TextBox id="tbOwnerName2" Runat="server" MaxLength="21" Type="text" width="250"></asp:TextBox>
										<asp:CustomValidator
											ID="cvpresidentName"
											runat="Server"
											ControlToValidate="tbOwnerName2"
											ValidationGroup="UserRegist"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											CssClass="error_inline" />
									</td>
								</tr>

								<tr>
									<th>
										<%: ReplaceTag("@@User.OwnerNameKana1.name@@") %>
											<span class="necessary">*</span>
									</th>
									<td>
										<asp:TextBox id="tbOwnerNameKana1" Runat="server" MaxLength="25" Type="text" width="250"></asp:TextBox>
										<asp:CustomValidator
											ID="cvpresidentNameFamilyKana"
											runat="Server"
											ControlToValidate="tbOwnerNameKana1"
											ValidationGroup="UserRegist"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											CssClass="error_inline" />
									</td>
								</tr>

								<tr>
									<th>
										<%: ReplaceTag("@@User.OwnerNameKana2.name@@") %>
											<span class="necessary">*</span>
									</th>
									<td>
										<asp:TextBox id="tbOwnerNameKana2" Runat="server" MaxLength="25" Type="text" width="250"></asp:TextBox>
										<asp:CustomValidator
											ID="cvpresidentNameKana"
											runat="Server"
											ControlToValidate="tbOwnerNameKana2"
											ValidationGroup="UserRegist"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											CssClass="error_inline" />
									</td>
								</tr>

								<tr>
									<%-- 生年月日 --%>
									<th>
										<%: ReplaceTag("@@User.OwnerBirth.name@@") %>
											<span class="necessary">*</span>
									</th>
									<td>
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
									</td>
								</tr>

								<tr>
									<th>
										<%: ReplaceTag("@@User.RequestBudget.name@@") %>
									</th>
									<td>
										<asp:TextBox id="tbRequestBudget" Runat="server" MaxLength="8" Type="text" width="80" style="text-align: right" />
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
									</td>
								</tr>
							</table>
							<% } %>
						<% } %>
					<% } %>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>

		<%if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) || this.IsVisible_UserPassword) { %>
		<div class="dvLoginInfo">
			<h3 id ="logininfo">ログイン情報</h3>
			<table cellspacing="0">
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
				<tr>
					<th>
						<%-- ログインID --%>
						<%: ReplaceTag("@@User.login_id.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_login_id"/>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserLoginId, "@@User.login_id.length_max@@"); %>
						<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) tbUserLoginId.Attributes["Type"] = "email"; %>
						<asp:TextBox id="tbUserLoginId" Width="120" Runat="server"></asp:TextBox><span class="notes">※6～15桁</span>
						<asp:CustomValidator ID="cvUserLoginId" runat="Server"
							ControlToValidate="tbUserLoginId"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<%} %>
				<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
				<%if (this.IsVisible_UserPassword){ %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.password.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_password" />
					</th>
					<td>
						<% SetMaxLength(this.WtbUserPassword, "@@User.password.length_max@@"); %>
						<asp:Literal ID="lUserPassword" runat="server"></asp:Literal>
						<asp:TextBox id="tbUserPassword" TextMode="Password" autocomplete="off" CssClass="password" Runat="server"></asp:TextBox><span class="notes">※半角英数字混合7～15文字</span>
						<asp:Button ID="btnUserPasswordReType" runat="server" Text="再入力" Visible="False" OnClick="btnUserPasswordReType_Click"></asp:Button>
						<asp:CustomValidator
							ID="cvUserPassword"
							runat="Server"
							ControlToValidate="tbUserPassword"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.password.name@@") %>(確認用)
						<span class="necessary">*</span><span id="efo_sign_password_conf"/>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserPasswordConf, "@@User.password.length_max@@"); %>
						<asp:Literal ID="lUserPasswordConf" runat="server"></asp:Literal>
						<asp:TextBox id="tbUserPasswordConf" TextMode="Password" autocomplete="off" CssClass="password" Runat="server"></asp:TextBox><span class="notes">※半角英数字混合7～15文字</span>
						<asp:CustomValidator
							ID="cvUserPasswordConf"
							runat="Server"
							ControlToValidate="tbUserPasswordConf"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
			</table>
		</div>
		<% } %>

		<br />
		<%-- キャプチャ認証 --%>
		<%--<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbConfirm.ClientID %>" />--%>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION) %>" class="btn btn-large">戻る</a></span>
				<span><asp:LinkButton ID="lbConfirm" ValidationGroup="UserRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">確認する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/SocialPlusInputCompletion.js"></script>
<script type="text/javascript">
<!--
	bindEvent();

	changeDropDownDays();
	<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
	changeGmoDropDownDays();
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
		clickSearchZipCode(
			'<%= this.WtbUserZip.ClientID %>',
			'<%= this.WtbUserZip1.ClientID %>',
			'<%= this.WtbUserZip2.ClientID %>',
			'<%= this.WlbSearchAddr.ClientID %>',
			'<%= this.WlbSearchAddr.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON%>',
			'#addrSearchErrorMessage')

		// Check zip code input on text box change
		textboxChangeSearchZipCode(
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
	//-->

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
		};
		window.onAmazonPaymentsReady = function () {
			if ($('#addressBookWidgetDiv').length) showAddressBookWidget();
		};

		<%-- Amazonアドレス帳表示ウィジェット --%>
		function showAddressBookWidget() {
			new OffAmazonPayments.Widgets.AddressBook({
				sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
				onOrderReferenceCreate: function (orderReference) {
					var x = orderReference.getAmazonOrderReferenceId();
					$('#<%= this.WhfAmazonOrderRefID.ClientID %>').val(x);
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
				design: { designMode: 'responsive' },
				onError: function (error) {
					alert(error.getErrorMessage());
					location.href = "<%=Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN %>";
				}
			}).bind("addressBookWidgetDiv");
		}

		<%-- Amazon住所取得関数 --%>
		function getAmazonAddress(callback) {
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT%>/GetAmazonAddress",
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
		<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
	<% } %>
<% } %>

</asp:Content>
