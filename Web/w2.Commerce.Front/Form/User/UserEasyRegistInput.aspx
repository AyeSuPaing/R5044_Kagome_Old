<%--
=========================================================================================================
  Module      : かんたん会員登録入力画面(UserEasyRegistInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="UserEasyRegistInput.aspx.cs" Inherits="Form_User_UserEasyRegistInput" Title="かんたん会員登録入力ページ" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.App.Common.User.SocialLogin.Util" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Register Src="~/Form/Common/PaypalScriptsForm.ascx" TagPrefix="uc" TagName="PaypalScriptsForm" %>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="dvUserContents">

	<h2>会員情報の入力</h2>

	<div id="dvUserRegistInput" class="unit">
		
		<%-- メッセージ --%>
		<div class="dvContentsInfo" >
			<p>
				<%: ShopMessage.GetMessage("ShopName") %>会員へのお申込みにあたっては、以下の項目にご入力が必要です。<br />
				会員規約を必ずお読みの上、「登録する」ボタンを押して下さい。</p>
		</div>
		<% if ((Constants.SOCIAL_LOGIN_ENABLED || Constants.AMAZON_LOGIN_OPTION_ENABLED || Constants.PAYPAL_LOGINPAYMENT_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) && (SessionManager.SocialLogin == null)) { %>
			<%-- ソーシャルログイン連携 --%>
			<div class="dvSocialLoginCooperation">
				<h3>ソーシャルログイン連携</h3>
				<ul style="display:flex;margin-bottom: 10px;flex-wrap:wrap">
					<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
						<%-- Apple --%>
						<li style="margin: 5px;">
							<a class="social-login-registinput apple-color"
								href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
									w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
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
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
										true,
										Request.Url.Authority) %>">
								<div class="social-icon-width">
									<img class="facebook-icon-registinput"
										src="<%= Constants.PATH_ROOT %>
										Contents\ImagesPkg\socialLogin\logo_facebook.png" />
								</div>
								<div class="social-login-label-registinput">Facebookでログイン</div>
							</a>
						</li>
						<%-- Twitter --%>
						<li style="margin: 5px;">
							<a class="social-login-registinput twitter-color"
								href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
										w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
										true,
										Request.Url.Authority) %>">
								<div class="social-icon-width">
									<img class="twittericon-registinput"
										src="<%= Constants.PATH_ROOT %>
										Contents\ImagesPkg\socialLogin\logo_x.png" />
								</div>
								<div class="twitter-label-registinput">Xでログイン</div>
							</a>
						</li>
						<%-- Yahoo --%>
						<li style="margin: 5px;">
							<a class="social-login-registinput yahoo-color"
								href="<%= w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
										w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
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
											Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
											Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
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
					<% if (Constants.SOCIAL_LOGIN_ENABLED || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
						<%-- LINE --%>
						<li style="margin: 5px;">
							<div class="social-login-registinput line-color">
								<div class="social-login-registinput line-hover-color line-active-color">
									<a href="<%= LineConnect(
										w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION
											? Constants.PAGE_FRONT_DEFAULT
											: Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
										Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK,
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
					<%-- Amazon --%>
					<% if (Constants.AMAZON_LOGIN_OPTION_ENABLED) { %>
						<li style="margin: 5px;">
							<%--▼▼Amazonログインボタンウィジェット▼▼--%>
							<div id="AmazonLoginButton"></div>
							<div style="width:280px;display:inline-block;">
								<%--▼▼ Amazonログイン(CV2)ボタン ▼▼--%>
								<div id="AmazonLoginCv2Button"></div>
								<%--▲▲ Amazonログイン(CV2)ボタン ▲▲--%>
							</div>
							<%--▲▲Amazonログインボタンウィジェット▲▲--%>
						</li>
					<% } %>
					<%-- PayPal --%>
					<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) { %>
						<li style="margin: 5px;">
							<%-- ▼PayPalここから▼ --%>
							<%
								ucPaypalScriptsForm.LogoDesign = "Login";
								ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
								ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
							%>
							<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
							<div id="paypal-button" style="width: 280px;"></div>
							<div id="paypal-button2" style="width: 280px;">
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
			<table>
				<% if (this.IsVisible_UserName) { %>
				<tr>
					<%-- 氏名 --%>
					<th><%: ReplaceTag("@@User.name.name@@") %><span class="necessary">*</span><span id="efo_sign_name"/></th>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
									<% SetMaxLength(this.WtbUserName1, "@@User.name1.length_max@@"); %>
									<span class="fname">姓</span><asp:TextBox ID="tbUserName1" runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(this.WtbUserName2, "@@User.name2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox ID="tbUserName2" runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：山田</span></td>
								<td><span class="notes">太郎</span></td>
							</tr>
						</table>
						<asp:CustomValidator
							ID="cvUserName1"
							runat="server"
							ControlToValidate="tbUserName1"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserName2"
							runat="server"
							ControlToValidate="tbUserName2"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserNameKana) { %>
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
									<span class="fname">姓</span><asp:TextBox ID="tbUserNameKana1" runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(this.WtbUserNameKana2, "@@User.name_kana2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox ID="tbUserNameKana2" runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角ひらがな入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：やまだ</span></td>
								<td><span class="notes">たろう</span></td>
							</tr>
						</table>
						<small>
						<asp:CustomValidator
							ID="cvUserNameKana1"
							runat="server"
							ControlToValidate="tbUserNameKana1"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserNameKana2"
							runat="server"
							ControlToValidate="tbUserNameKana2"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						</small>
					</td>
				</tr>
				<% } %>
				<% if (Constants.PRODUCTREVIEW_ENABLED) { %>
				<% if (this.IsVisible_UserNickName) { %>
				<tr>
					<%-- ニックネーム --%>
					<th><%: ReplaceTag("@@User.nickname.name@@") %></th>
					<td>
						<asp:TextBox ID="tbUserNickName" runat="server" MaxLength="20" CssClass="nickname"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserNickName"
							runat="server"
							ControlToValidate="tbUserNickName"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% } %>
					<%
						for (int iLoop = 1900; iLoop <= 1940; iLoop++)
						{
							this.WddlUserBirthYear.Items.Remove(new ListItem(iLoop.ToString())) ;
						}
					 %>
				<% if (this.IsVisible_UserBirth) { %>
				<tr>
					<%-- 生年月日 --%>
					<th>
						<%: ReplaceTag("@@User.birth.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_birth"/>
					</th>
					<td>
						<asp:DropDownList ID="ddlUserBirthYear" runat="server" CssClass="year" onchange="changeDropDownDays()"></asp:DropDownList>年
						<asp:DropDownList ID="ddlUserBirthMonth" runat="server" CssClass="month" onchange="changeDropDownDays()"></asp:DropDownList>月
						<asp:DropDownList ID="ddlUserBirthDay" runat="server" CssClass="date"></asp:DropDownList>日
						<asp:CustomValidator
							ID="cvUserBirthYear"
							runat="Server"
							ControlToValidate="ddlUserBirthYear"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserBirthMonth"
							runat="Server"
							ControlToValidate="ddlUserBirthMonth"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserBirthDay"
							runat="Server"
							ControlToValidate="ddlUserBirthDay"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserSex) { %>
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
							runat="server"
							ControlToValidate="rblUserSex"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<%-- PCメールアドレス --%>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_mail_addr"/>
					</th>
					<td>
						<asp:TextBox ID="tbUserMailAddr" runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserMailAddr"
							runat="server"
							ControlToValidate="tbUserMailAddr"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserMailAddrForCheck"
							runat="server"
							ControlToValidate="tbUserMailAddr"
							ValidationGroup="UserEasyRegist"
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
						<asp:TextBox ID="tbUserMailAddrConf" runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserMailAddrConf"
							runat="server"
							ControlToValidate="tbUserMailAddrConf"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsVisible_UserMailAddr2 && (Constants.CROSS_POINT_OPTION_ENABLED == false)) { %>
				<tr>
					<%-- モバイルメールアドレス --%>
					<th>
						<%: ReplaceTag("@@User.mail_addr2.name@@") %>
						<span class="necessary" style="display:<%= (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) ? "" : "none" %>">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbUserMailAddr2" runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserMailAddr2"
							runat="server"
							ValidationGroup="UserRegist"
							ControlToValidate="tbUserMailAddr2"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserMailAddr2Conf && (Constants.CROSS_POINT_OPTION_ENABLED == false)) { %>
				<tr>
					<%-- モバイルメールアドレス（確認用） --%>
					<th><%: ReplaceTag("@@User.mail_addr2.name@@") %>（確認用）<span class="necessary" style="display:<%= (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) ? "" : "none" %>">*</span></th>
					<td>
						<asp:TextBox ID="tbUserMailAddr2Conf" runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserMailAddr2Conf"
							runat="server"
							ControlToValidate="tbUserMailAddr2Conf"
							ValidationGroup="UserRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (Constants.GLOBAL_OPTION_ENABLE && this.IsVisible_UserCountry) { %>
				<tr>
					<%-- 国 --%>
					<th>
						<%: ReplaceTag("@@User.country.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:DropDownList ID="ddlUserCountry" runat="server" CssClass="district" AutoPostBack="true" OnSelectedIndexChanged="ddlUserAddrCountry_SelectedIndexChanged" />
						<span id="countryAlertMessage" class="notes" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
						<asp:CustomValidator
							ID="cvUserCountry"
							runat="server"
							ControlToValidate="ddlUserCountry"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (Constants.GLOBAL_OPTION_ENABLE == false) { %>
				<% if (this.IsVisible_AmazonAddressWidget) { %>
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
				<% if (this.IsVisible_UserZip) { %>
				<tr>
					<%-- 郵便番号 --%>
					<th>
						<%: ReplaceTag("@@User.zip.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_zip"/>
					</th>
					<td valign="middle">
						<asp:TextBox ID="tbUserZip" MaxLength="8" OnTextChanged="lbSearchAddr_Click" runat="server" />
						<asp:LinkButton ID="lbSearchAddr" runat="server" OnClick="lbSearchAddr_Click" class="btn btn-mini" OnClientClick="return false;">
							住所検索</asp:LinkButton><br/>
						<%--検索結果レイヤー--%>
						<uc:Layer ID="ucLayer" runat="server" />
						<asp:CustomValidator
							ID="cvUserZip1"
							runat="server"
							ControlToValidate="tbUserZip"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline zip_input_error_message" />
						<span style="color :Red" id="addrSearchErrorMessage" class="shortZipInputErrorMessage">
							<%: this.ZipInputErrorMessage %></span>
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserAddr1) { %>
				<tr>
					<%-- 都道府県 --%>
					<th>
						<%: ReplaceTag("@@User.addr1.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_ship_addr1"/>
					</th>
					<td>
						<asp:DropDownList ID="ddlUserAddr1" runat="server" CssClass="district"></asp:DropDownList>
						<asp:CustomValidator
							ID="cvUserAddr1"
							runat="server"
							ControlToValidate="ddlUserAddr1"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserAddr2) { %>
				<tr>
					<%-- 市区町村 --%>
					<th>
						<%: ReplaceTag("@@User.addr2.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_ship_addr2"/>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserAddr2, "@@User.addr2.length_max@@"); %>
						<asp:TextBox ID="tbUserAddr2" runat="server" CssClass="addr"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserAddr2"
							runat="server"
							ControlToValidate="tbUserAddr2"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserAddr3) { %>
				<tr>
					<%-- 番地 --%>
					<th>
						<%: ReplaceTag("@@User.addr3.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_ship_addr3"/>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserAddr3, "@@User.addr3.length_max@@"); %>
						<asp:TextBox ID="tbUserAddr3" runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserAddr3"
							runat="server"
							ControlToValidate="tbUserAddr3"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (Constants.DISPLAY_ADDR4_ENABLED) { %>
				<% if (this.IsVisible_UserAddr4) { %>
				<tr>
					<%-- ビル・マンション名 --%>
					<th>
						<%: ReplaceTag("@@User.addr4.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserAddr4, "@@User.addr4.length_max@@"); %>
						<asp:TextBox ID="tbUserAddr4" runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserAddr4"
							runat="server"
							ControlToValidate="tbUserAddr4"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% } %>
				<% } %>
				<% } %>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<% if (this.IsVisible_UserCompanyName) { %>
				<tr>
					<%-- 企業名 --%>
					<th>
						<%: ReplaceTag("@@User.company_name.name@@")%>
						<span class="necessary"></span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserCompanyName, "@@User.company_name.length_max@@"); %>
						<asp:TextBox ID="tbUserCompanyName" runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserCompanyName"
							runat="server"
							ControlToValidate="tbUserCompanyName"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserCompanyPostName) { %>
				<tr>
					<%-- 部署名 --%>
					<th>
						<%: ReplaceTag("@@User.company_post_name.name@@")%>
						<span class="necessary"></span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserCompanyPostName, "@@User.company_post_name.length_max@@"); %>
						<asp:TextBox ID="tbUserCompanyPostName" runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserCompanyPostName"
							runat="server"
							ControlToValidate="tbUserCompanyPostName"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% } %>
				<% if (this.IsVisible_UserTel1) { %>
				<tr>
					<%-- 電話番号 --%>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_tel1"/>
					</th>
					<td>
						<asp:TextBox ID="tbUserTel1_1" runat="server" MaxLength="13" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvUserTel1')" />
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
							runat="server"
							ControlToValidate="tbUserTel1_1"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline shortTel" />
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
							<span><%: GetVerificationCodeNote() %></span>
							<asp:Label ID="lbAuthenticationMessage" runat="server" />
						</span>
						<asp:CustomValidator
							ID="cvAuthenticationCode"
							runat="Server"
							ControlToValidate="tbAuthenticationCode"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% } %>
				<% if (this.IsVisible_UserTel2) { %>
				<tr>
					<%-- 電話番号（予備） --%>
					<th>
						<%: ReplaceTag("@@User.tel2.name@@") %>
					</th>
					<td>
						<asp:TextBox ID="tbUserTel1_2" MaxLength="13" runat="server" CssClass="shortTel" />
						<asp:CustomValidator
							ID="cvUserTel2_1"
							runat="server"
							ControlToValidate="tbUserTel1_2"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsVisible_UserMailFlg) { %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_flg.name@@") %>
					</th>
					<td class="checkBox">
						<asp:CheckBox ID="cbUserMailFlg" Text=" 希望する " CssClass="checkBox" runat="server" />
					</td>
				</tr>
				<% } %>
			</table>
			<% if (this.IsDisplayPersonalAuthentication) { %>
			<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
			<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
			<% } %>
			<%-- ソーシャルログイン用 --%>
			<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
			<asp:HiddenField ID="hfSocialLoginJson" runat="server" />
			<% } %>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで --%>
		</div>
		<%if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) || this.IsVisible_UserPassword) { %>
		<div class="dvLoginInfo">
			<h3>ログイン情報</h3>
			<table>
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
				<tr>
					<th>
						<%-- ログインID --%>
						<%: ReplaceTag("@@User.login_id.name@@") %><span class="necessary">*</span><span id="efo_sign_login_id"/></th>
					<td>
						<% SetMaxLength(this.WtbUserLoginId, "@@User.login_id.length_max@@"); %>
						<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) tbUserLoginId.Attributes["Type"] = "email"; %>
						<asp:textbox ID="tbUserLoginId" width="120" runat="server"></asp:textbox>
						<span class="notes">※6～15桁</span>
						<asp:customvalidator
							ID="cvUserLoginId"
							runat="server"
							ControlToValidate="tbUserLoginId"
							ValidationGroup="UserEasyRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
				<%if (this.IsVisible_UserPassword){ %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.password.name@@") %>
						<span class="necessary">*</span><span id="efo_sign_password"/>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserPassword, "@@User.password.length_max@@"); %>
						<asp:Textbox ID="tbUserPassword" textmode="Password" autocomplete="off" cssclass="password" runat="server"></asp:Textbox>
						<span class="notes">※半角英数字混合7～15文字</span>
						<asp:CustomValidator
							ID="cvUserPassword"
							runat="server"
							ControlToValidate="tbUserPassword"
							ValidationGroup="UserEasyRegist"
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
						<asp:Textbox ID="tbUserPasswordConf" textmode="Password" autocomplete="off" cssclass="password" runat="server"></asp:Textbox>
						<span class="notes">※半角英数字混合7～15文字</span>
						<asp:CustomValidator
							ID="cvUserPasswordConf"
							runat="server"
							ControlToValidate="tbUserPasswordConf"
							ValidationGroup="UserEasyRegist"
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
	</div>
	<div ID="dvUserRegistRegulation" class="unit" style="margin-top:20px;">
		<h3>会員規約について</h3>
		<div class="dvRegulation">
			<uc:UserRegistRegulationMessage runat="server" />
		</div>
		<div style="text-align:center;margin-top:1.5em">
			<asp:CheckBox ID="cbUserAcceptedRegulation" Text="会員規約に同意する" runat="server" />
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
	<%-- キャプチャ認証 --%>
	<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbRegister.ClientID %>" />
	<div class="dvUserBtnBox">
		<p>
			<span><a href="javascript:history.back();" class="btn btn-large">戻る</a></span>
			<span><asp:LinkButton ID="lbRegister" ValidationGroup="UserEasyRegist" OnClientClick="return exec_submit();" OnClick="lbRegister_Click" class="btn btn-large btn-inverse" runat="server">登録する</asp:LinkButton></span>
		</p>
	</div>
</div>

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
		clickSearchZipCode(
			'<%= this.WtbUserZip.ClientID %>',
			'<%= this.WtbUserZip1.ClientID %>',
			'<%= this.WtbUserZip2.ClientID %>',
			'<%= this.WlbSearchAddr.ClientID %>',
			'<%= this.WlbSearchAddr.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage');

		// Check zip code input on text box change
		textboxChangeSearchZipCode(
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
						if (data.Error) $addressBookErrorMessage.html(data.Error);
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

		<%--Amazonボタン表示ウィジェット--%>
		function showButton() {
			var authRequest;
			OffAmazonPayments.Button("AmazonLoginButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
				type: "LwA",
				color: "Gold",
				size: "large",
				authorization: function () {
					loginOptions = { scope: "payments:shipping_address payments:widget profile", popup: true };
					authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_USER_EASY_REGISTER_CALLBACK) %>");
				},
				onError: function (error) {
					alert(error.getErrorMessage());
					location.href = "<%=Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN %>";
				}
			});
			$('#OffAmazonPaymentsWidgets0').css({ 'height': '44px', 'width': '218px' });
		}

		<%--Amazon住所取得関数--%>
		function getAmazonAddress(callback) {
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT%>/GetAmazonAddress",
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
