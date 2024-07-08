<%--
=========================================================================================================
  Module      : 会員登録変更入力画面(UserModifyInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendModify" Src="~/Form/Common/User/BodyUserExtendModify.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserModifyInput.aspx.cs" Inherits="Form_User_UserModifyInput" Title="登録情報変更入力ページ" %>
<%@ Import Namespace="w2.App.Common.Amazon.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<div id="dvUserFltContents">
	<%-- 会員情報変更系パンくず --%>
	<div id="dvHeaderModifyClumbs">
		<p><img src="../../Contents/ImagesPkg/user/clumbs_modify_1.gif" alt="登録情報の変更" /></p>
	</div>

		<h2>登録情報の変更</h2>

	<div id="dvUserModifyInput" class="unit">
			
		<%-- メッセージ --%>
		<div class="dvContentsInfo">
			<p>登録情報の変更を希望の方は、下記のフォームに必要事項をご入力の上、<br />「入力内容確認」ボタンをクリックして下さい。</p>
		</div>

		<div class="dvUserInfo">
			<h3>お客様情報</h3>
			<%if (this.IsEasyUser) {%>
			<p style="margin:5px;padding:5px;text-align:center;background-color:#ffff80;border:1px solid #D4440D;border-color:#E5A500;color:#CC7600;">ご購入手続きに必要な会員情報が不足しています。</p>
			<%} %>
			<ins><span class="necessary">*</span>は必須入力となります。</ins>
			
			<%-- UPDATE PANEL開始 --%>
			<asp:UpdatePanel ID="upUpdatePanel" runat="server">
			<ContentTemplate>
			<table cellspacing="0">
				<tr>
					<%-- 氏名 --%>
					<th><%: ReplaceTag("@@User.name.name@@") %>
						<% if (this.IsUserNameNecessary) { %>
						<span class="necessary">*</span>
						<% } %>
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
						<% if (this.IsUserNameNecessary) { %>
						<asp:CustomValidator
							ID="cvUserName1"
							runat="Server"
							ControlToValidate="tbUserName1"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserName2"
							runat="Server"
							ControlToValidate="tbUserName2"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<% if (this.IsUserAddrJp) { %>
				<tr>
					<%-- 氏名（かな） --%>
					<th><%: ReplaceTag("@@User.name_kana.name@@") %>
						<% if (this.IsUserNameKanaNecessary) { %>
						<span class="necessary">*</span>
						<% } %>
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
						<% if (this.IsUserNameKanaNecessary) { %>
						<asp:CustomValidator
							ID="cvUserNameKana1"
							runat="Server"
							ControlToValidate="tbUserNameKana1"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserNameKana2"
							runat="Server"
							ControlToValidate="tbUserNameKana2"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<% } %>
				<%if (Constants.PRODUCTREVIEW_ENABLED) { %>
				<tr>
					<%-- ニックネーム --%>
					<th><%: ReplaceTag("@@User.nickname.name@@") %></th>
					<td>
						<asp:TextBox id="tbUserNickName" runat="server" MaxLength="20" CssClass="nickname"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserNickName"
							runat="Server"
							ControlToValidate="tbUserNickName"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<%} %>
				<tr>
					<%-- 生年月日 --%>
					<th><%: ReplaceTag("@@User.birth.name@@") %>
						<% if (this.IsUserBirthNecessary) { %>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<asp:DropDownList id="ddlUserBirthYear" runat="server" CssClass="year" onchange="changeDropDownDays()"></asp:DropDownList>年
						<asp:DropDownList id="ddlUserBirthMonth" runat="server" CssClass="month" onchange="changeDropDownDays()"></asp:DropDownList>月
						<asp:DropDownList id="ddlUserBirthDay" runat="server" CssClass="date"></asp:DropDownList>日
						<% if (this.IsUserBirthNecessary) { %>
						<asp:CustomValidator
							ID="cvUserBirthYear"
							runat="Server"
							ControlToValidate="ddlUserBirthYear"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserBirthMonth"
							runat="Server"
							ControlToValidate="ddlUserBirthMonth"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserBirthDay"
							runat="Server"
							ControlToValidate="ddlUserBirthDay"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 性別 --%>
					<th><%: ReplaceTag("@@User.sex.name@@") %>
						<% if (this.IsUserSexNecessary) { %>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" CellSpacing="0"  RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
						<% if (this.IsUserSexNecessary) { %>
						<asp:CustomValidator
							ID="cvUserSex"
							runat="Server"
							ControlToValidate="rblUserSex"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- PCメールアドレス --%>
					<th><%: ReplaceTag("@@User.mail_addr.name@@") %>
						<%if (this.IsPcSiteOrOfflineUser) {%>
						<span class="necessary">*</span>
						<%} %>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddr" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<%if (this.IsPcSiteOrOfflineUser && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
						<span class="notes">※ログイン時に利用します</span>
						<%} %>
						<% cvUserMailAddr.ValidateEmptyText = this.IsPcSiteOrOfflineUser; %>
						<asp:CustomValidator
							ID="cvUserMailAddr"
							runat="Server"
							ControlToValidate="tbUserMailAddr"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserMailAddrForCheck"
							runat="Server"
							ControlToValidate="tbUserMailAddr"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- PCメールアドレス(確認用) --%>
					<th><%: ReplaceTag("@@User.mail_addr.name@@") %>(確認用)
						<%if (this.IsPcSiteOrOfflineUser) {%>
						<span class="necessary">*</span>
						<%} %>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddrConf" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<% cvUserMailAddrConf.ValidateEmptyText = this.IsPcSiteOrOfflineUser; %>
						<asp:CustomValidator
							ID="cvUserMailAddrConf"
							runat="Server"
							ControlToValidate="tbUserMailAddrConf"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
				<tr>
					<%-- モバイルメールアドレス --%>
					<th><%: ReplaceTag("@@User.mail_addr2.name@@") %>
						<%if ((this.IsPcSiteOrOfflineUser == false) || (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)) {%>
						<span class="necessary">*</span>
						<%} %>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddr2" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<%if ((this.IsPcSiteOrOfflineUser == false) && Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
						<span class="notes">※ログイン時に利用します</span>
						<%} %>
						<% cvUserMailAddr2.ValidateEmptyText = (this.IsPcSiteOrOfflineUser == false); %>
						<asp:CustomValidator
							ID="cvUserMailAddr2"
							runat="Server"
							ControlToValidate="tbUserMailAddr2"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvUserMailAddr2ForCheck"
							runat="Server"
							ControlToValidate="tbUserMailAddr2"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- モバイルメールアドレス(確認用) --%>
					<th><%: ReplaceTag("@@User.mail_addr2.name@@") %>(確認用)
						<%if ((this.IsPcSiteOrOfflineUser == false) || (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)) {%>
						<span class="necessary">*</span>
						<%} %>
					</th>
					<td>
						<asp:TextBox id="tbUserMailAddr2Conf" Runat="server" MaxLength="256" CssClass="mailAddr mail-domain-suggest" Type="email"></asp:TextBox>
						<% cvUserMailAddr2Conf.ValidateEmptyText = (this.IsPcSiteOrOfflineUser == false); %>
						<asp:CustomValidator
							ID="cvUserMailAddr2Conf"
							runat="Server"
							ControlToValidate="tbUserMailAddr2Conf"
							ValidationGroup="UserModify"
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
						<% if (this.IsUserCountryNecessary) { %>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<asp:DropDownList ID="ddlUserCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUserAddrCountry_SelectedIndexChanged"></asp:DropDownList></br>
						<asp:CustomValidator
							ID="cvUserCountry"
							runat="Server"
							ControlToValidate="ddlUserCountry"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						<span id="countryAlertMessage" class="notes" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
					</td>
				</tr>
				<% } %>
				<% if (this.IsUserAddrJp) { %>
				<tr>
					<%-- 郵便番号 --%>
					<th>
						<%: ReplaceTag("@@User.zip.name@@") %>
						<% if (this.IsUserZipNecessary) { %>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<asp:TextBox ID="tbUserZip" MaxLength="8" Type="tel" OnTextChanged="lbSearchAddr_Click" runat="server" />
						<asp:LinkButton ID="lbSearchAddr" runat="server" OnClick="lbSearchAddr_Click" class="btn btn-mini" OnClientClick="return false;">
							住所検索</asp:LinkButton><br/>
						<%--検索結果レイヤー--%>
						<uc:Layer ID="ucLayer" runat="server" />
						<% if (this.IsUserZipNecessary) { %>
						<asp:CustomValidator
							ID="cvUserZip1"
							runat="Server"
							ControlToValidate="tbUserZip"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline zip_input_error_message" />
						<span style="color :Red" id="addrSearchErrorMessage" class="shortZipInputErrorMessage">
							<%: this.ZipInputErrorMessage %></span>
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 都道府県 --%>
					<th>
						<%: ReplaceTag("@@User.addr1.name@@") %>
						<% if (this.IsUserAddr1Necessary) { %>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<asp:DropDownList id="ddlUserAddr1" runat="server" CssClass="district"></asp:DropDownList>
						<% if (this.IsUserAddr1Necessary) { %>
						<asp:CustomValidator
							ID="cvUserAddr1"
							runat="Server"
							ControlToValidate="ddlUserAddr1"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<% } %>
				<tr>
					<%-- 市区町村 --%>
					<th>
						<%: ReplaceTag("@@User.addr2.name@@", this.UserAddrCountryIsoCode) %>
						<% if (this.IsUserAddr2Necessary) { %>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<% if (IsCountryTw(this.UserAddrCountryIsoCode)) { %>
							<asp:DropDownList runat="server" ID="ddlUserAddr2" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlUserAddr2_SelectedIndexChanged"></asp:DropDownList>
						<% } else { %>
							<% SetMaxLength(this.WtbUserAddr2, "@@User.addr2.length_max@@"); %>
								<asp:TextBox id="tbUserAddr2" Runat="server" CssClass="addr"></asp:TextBox><span class="notes">※全角入力</span>
								<% if (this.IsUserAddr2Necessary) { %>
								<asp:CustomValidator
									ID="cvUserAddr2"
									runat="Server"
									ControlToValidate="tbUserAddr2"
									ValidationGroup="UserModify"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
								<% } %>
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 番地 --%>
					<th>
						<%: ReplaceTag("@@User.addr3.name@@", this.UserAddrCountryIsoCode) %>
						<% if (IsAddress3Necessary(this.UserAddrCountryIsoCode) && this.IsUserAddr3Necessary) { %>
							<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<% if (IsCountryTw(this.UserAddrCountryIsoCode)) { %>
							<asp:DropDownList runat="server" ID="ddlUserAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
						<% } else { %>
							<% SetMaxLength(this.WtbUserAddr3, "@@User.addr3.length_max@@"); %>
							<asp:TextBox id="tbUserAddr3" Runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
							<% if (this.IsUserAddr3Necessary) { %>
							<asp:CustomValidator
								ID="cvUserAddr3"
								runat="Server"
								ControlToValidate="tbUserAddr3"
								ValidationGroup="UserModify"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
							<% } %>
						<% } %>
					</td>
				</tr>
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
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsUserAddrJp == false) { %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.addr5.name@@", this.UserAddrCountryIsoCode) %>
						<% if (this.IsUserAddrUs) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<% if (this.IsUserAddrUs) { %>
						<asp:DropDownList ID="ddlUserAddr5" runat="server"></asp:DropDownList>
							<asp:CustomValidator
								ID="cvUserAddr5"
								runat="Server"
								ControlToValidate="ddlUserAddr5"
								ValidationGroup="UserModifyGlobal"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						<% } else { %>
						<asp:TextBox ID="tbUserAddr5" runat="server"></asp:TextBox>
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 郵便番号（海外向け） --%>
					<th>
						<%: ReplaceTag("@@User.zip.name@@", this.UserAddrCountryIsoCode) %>
						<% if (this.IsUserAddrZipNecessary) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<asp:TextBox ID="tbUserZipGlobal" runat="server" MaxLength="20"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserZipGlobal"
							runat="Server"
							ControlToValidate="tbUserZipGlobal"
							ValidationGroup="UserModifyGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:LinkButton
							ID="lbSearchAddrFromZipGlobal"
							OnClick="lbSearchAddrFromZipGlobal_Click"
							Style="display:none;"
							Runat="server" />
					</td>
				</tr>
				<% } %>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<tr>
					<%-- 企業名 --%>
					<th>
						<%: ReplaceTag("@@User.company_name.name@@")%>
						<span class="necessary"></span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserCompanyName, "@@User.company_name.length_max@@"); %>
						<asp:TextBox id="tbUserCompanyName" Runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserCompanyName"
							runat="Server"
							ControlToValidate="tbUserCompanyName"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- 部署名 --%>
					<th>
						<%: ReplaceTag("@@User.company_post_name.name@@")%>
						<span class="necessary"></span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserCompanyPostName, "@@User.company_post_name.length_max@@"); %>
						<asp:TextBox id="tbUserCompanyPostName" Runat="server" CssClass="addr2"></asp:TextBox><span class="notes">※全角入力</span>
						<asp:CustomValidator
							ID="cvUserCompanyPostName"
							runat="Server"
							ControlToValidate="tbUserCompanyPostName"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<%} %>
				<tr>
					<%-- 電話番号 --%>
					<% if (this.IsUserAddrJp) { %>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@") %>
						<%if (this.IsUserTel1Necessary) { %>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<asp:TextBox ID="tbUserTel1_1" MaxLength="13" Type="tel" runat="server" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvUserTel1')" />
						<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
						<asp:LinkButton
							ID="lbGetAuthenticationCode"
							class="btn btn-mini"
							Enabled="false"
							runat="server"
							Text="認証コードの取得"
							OnClick="lbGetAuthenticationCode_Click"
							OnClientClick="return checkTelNoInput();" />
						<br />
						<asp:Label ID="lbAuthenticationStatus" runat="server" />
						<% } %>
						<% if (this.IsUserTel1Necessary) { %>
						<asp:CustomValidator
							ID="cvUserTel1"
							runat="Server"
							ControlToValidate="tbUserTel1_1"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% } %>
					</td>
					<% } else { %>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %>
						<%if (this.IsUserTel1Necessary) {%>
						<span class="necessary">*</span>
						<% } %>
					</th>
					<td>
						<asp:TextBox ID="tbUserTel1Global" runat="server" MaxLength="30" onchange="resetAuthenticationCodeInput('cvUserTel1Global')" />
						<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
						<asp:LinkButton
							ID="lbGetAuthenticationCodeGlobal"
							class="btn btn-mini"
							Enabled="false"
							runat="server"
							Text="認証コードの取得"
							OnClick="lbGetAuthenticationCode_Click"
							OnClientClick="return checkTelNoInput();" />
						<br />
						<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
						<% } %>
						<% if (this.IsUserTel1Necessary) { %>
						<asp:CustomValidator
							ID="cvUserTel1Global"
							runat="Server"
							ControlToValidate="tbUserTel1Global"
							ValidationGroup="UserModifyGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% } %>
					</td>
					<% } %>
				</tr>
				<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
					<% if (this.IsUserAddrJp) { %>
					<tr>
						<th>
							<%: ReplaceTag("@@User.authentication_code.name@@") %>
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
								ValidationGroup="UserModify"
								ValidateEmptyText="false"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						</td>
					</tr>
					<% } else { %>
					<tr>
						<th>
							<%: ReplaceTag("@@User.authentication_code.name@@") %>
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
								ValidationGroup="UserModifyGlobal"
								ValidateEmptyText="false"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						</td>
					</tr>
					<% } %>
				<% } %>
				<tr>
					<% if (this.IsUserAddrJp) { %>
					<th>
						<%: ReplaceTag("@@User.tel2.name@@") %>
					</th>
					<td>
						<asp:TextBox ID="tbUserTel1_2" MaxLength="13" Type="tel" runat="server" CssClass="shortTel" />
						<asp:CustomValidator
							ID="cvUserTel2_1"
							runat="Server"
							ControlToValidate="tbUserTel1_2"
							ValidationGroup="UserModify"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
					<% } else { %>
					<th><%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %></th>
					<td>
						<asp:TextBox ID="tbUserTel2Global" runat="server"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUserTel2Global"
							runat="Server"
							ControlToValidate="tbUserTel2Global"
							ValidationGroup="UserModifyGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
					<% } %>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_flg.name@@") %>
					</th>
					<td class="checkBox">
						<asp:CheckBox ID="cbUserMailFlg" Text=" 希望する " CssClass="checkBox" runat="server" />
					</td>
				</tr>
				<%-- ユーザー拡張項目　HasInput:true(入力画面)/false(確認画面)　HasRegist:true(新規登録)/false(登録編集) --%>
				<uc:BodyUserExtendModify ID="ucBodyUserExtendModify" runat="server" HasInput="true" HasRegist="false" />
			</table>
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
			<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
			<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
			<% } %>
			</ContentTemplate>
			</asp:UpdatePanel>
			<%-- UPDATE PANELここまで --%>

		</div>
		<div class="dvLoginInfo" style="display:<%= Constants.RAKUTEN_LOGIN_ENABLED && SessionManager.IsRakutenIdConnectRegisterUser ? "none" : "" %>">
			<h3>ログイン情報</h3>
			<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
			<table cellspacing="0">
				<tr>
					<%-- ログインID --%>
					<th><%: ReplaceTag("@@User.login_id.name@@") %><span class="necessary">*</span>
					</th>
					<td>
						<% SetMaxLength(this.WtbUserLoginId, "@@User.login_id.length_max@@"); %>
						<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) tbUserLoginId.Attributes["Type"] = "email"; %>
						<asp:TextBox id="tbUserLoginId" Runat="server" CssClass="loginId"></asp:TextBox><span class="notes">※6～15桁</span>
						<asp:CustomValidator
							ID="cvUserLoginId"
							runat="Server"
							ControlToValidate="tbUserLoginId"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
			</table>
			<%} %>
			<p>パスワードを変更する場合のみ入力してください。</p>
			<table cellspacing="0">
				<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
				<%if (this.IsVisible_UserPassword){ %>
				<tr>
					<th>変更前<%: ReplaceTag("@@User.password.name@@") %><span class="necessary">*</span></th>
					<td>
						<% SetMaxLength(this.WtbUserPasswordBefore, "@@User.password.length_max@@"); %>
						<asp:TextBox id="tbUserPasswordBefore" Runat="server" TextMode="Password" autocomplete="off" CssClass="loginPass"></asp:TextBox><span class="notes">※半角英数字混合7～15文字</span>
						<asp:CustomValidator
							ID="cvUserPasswordBefore"
							runat="Server"
							ControlToValidate="tbUserPasswordBefore"
							ValidationGroup="UserModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<th>変更後<%: ReplaceTag("@@User.password.name@@") %><span class="necessary">*</span></th>
					<td>
						<% SetMaxLength(this.WtbUserPassword, "@@User.password.length_max@@"); %>
						<asp:TextBox id="tbUserPassword" Runat="server" TextMode="Password" autocomplete="off" CssClass="loginPass"></asp:TextBox><span class="notes">※半角英数字混合7～15文字</span>
						<asp:CustomValidator
							ID="cvUserPassword"
							runat="Server"
							ControlToValidate="tbUserPassword"
							ValidationGroup="UserModify"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>変更後<%: ReplaceTag("@@User.password.name@@") %>(確認用)<span class="necessary">*</span></th>
					<td>
						<% SetMaxLength(this.WtbUserPasswordConf, "@@User.password.length_max@@"); %>
						<asp:TextBox id="tbUserPasswordConf" Runat="server" TextMode="Password" autocomplete="off" CssClass="loginPass"></asp:TextBox><span class="notes">※半角英数字混合7～15文字</span>
						<asp:CustomValidator
							ID="cvUserPasswordConf"
							runat="Server"
							ControlToValidate="tbUserPasswordConf"
							ValidationGroup="UserModify"
							ValidateEmptyText="false"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn btn-large">
					戻る</a></span>
				<span><asp:LinkButton ID="lbConfirm" ValidationGroup="UserModify" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">
					確認する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>

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
		<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
		bindTwAddressSearch();
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
			for (var day = 0; day < (maxDay - daysCount) ; day++) {
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

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		$('#<%= this.WddlUserAddr3.ClientID %>').change(function (e) {
			$('#<%= this.WtbUserZipGlobal.ClientID %>').val(
				$('#<%= this.WddlUserAddr3.ClientID %>').val().split('|')[0]);
		});
	}
	<% } %>

	<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
	// Set authentication message
	var setIntervalId;
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

</asp:Content>