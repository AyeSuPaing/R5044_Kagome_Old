<%--
=========================================================================================================
  Module      : Amazonペイメント画面(OrderAmazonInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderAmazonInput.aspx.cs" Inherits="Form_Order_OrderAmazonInput" Title="配送先・支払方法選択画面"%>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
<asp:Repeater runat="server" ID="rCartList" DataSource="<%# this.CartList %>">
	<ItemTemplate>
		<div class="main" style="background-image: none;">
			<h2 style="margin-bottom:18px;"><img src="../../Contents/ImagesPkg/order/sttl_user.gif" alt="注文者情報" width="80" height="16" /></h2>
			<asp:UpdatePanel ID="upOwnerInfo" Visible="<%# this.IsOrderOwnerInputEnabled %>" runat="server">
			<ContentTemplate>
			<p>以下の項目をご入力ください。<br />
			</p>
			<span class="fred">※</span>&nbsp;は必須入力です。<br />
			<%-- ▼注文者情報▼ --%>
			<div id="divOwnerColumn" style="display:flex;justify-content: space-between;" runat="server">
				<div style="width: 340px;">
					<div class="userBox">
					<div class="top">
					<div class="bottom">
						<dl>
						<%-- 氏名 --%>
						<dt>
							<%= ReplaceTag("@@User.name.name@@") %>
							&nbsp;<span class="fred">※</span><span id="efo_sign_name"/>
						</dt>
						<dd>
						姓&nbsp;&nbsp;<asp:TextBox ID="tbOwnerName1" Text="<%# this.CartList.Owner.Name1 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
						名&nbsp;&nbsp;<asp:TextBox ID="tbOwnerName2" Text="<%# this.CartList.Owner.Name2 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></asp:TextBox><br />
						<small>
						<asp:CustomValidator
							ID="cvOwnerName1"
							runat="Server"
							ControlToValidate="tbOwnerName1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvOwnerName2"
							runat="Server"
							ControlToValidate="tbOwnerName2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%-- 氏名（かな） --%>
						<dt>
							<%: ReplaceTag("@@User.name_kana.name@@") %>
							&nbsp;<% if (IsTargetToExtendedAmazonAddressManagerOption() == false) { %><span class="fred">※</span><span id="efo_sign_kana"/><% } %>
						</dt>
						<dd class="<%: ReplaceTag("@@User.name_kana.type@@") %>">
						姓&nbsp;&nbsp;<asp:TextBox ID="tbOwnerNameKana1" Text="<%# this.CartList.Owner.NameKana1 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
						名&nbsp;&nbsp;<asp:TextBox ID="tbOwnerNameKana2" Text="<%# this.CartList.Owner.NameKana2 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></asp:TextBox><br />
						<small>
						<asp:CustomValidator
							ID="cvOwnerNameKana1"
							runat="Server"
							ControlToValidate="tbOwnerNameKana1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator 
							ID="cvOwnerNameKana2"
							runat="Server"
							ControlToValidate="tbOwnerNameKana2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%-- 生年月日 --%>
						<dt>
							<%: ReplaceTag("@@User.birth.name@@") %>
							&nbsp;<%if (IsTargetToExtendedAmazonAddressManagerOption() == false) {%><span class="fred">※</span><span id="efo_sign_birth"/><% } %>
						</dt>
						<dd>
						<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue='<%# (this.CartList.Owner.Birth.HasValue ) ? this.CartList.Owner.BirthYear.ToString() : "1970" %>' CssClass="input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;年&nbsp;&nbsp;
						<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue='<%# (this.CartList.Owner.Birth.HasValue ) ? this.CartList.Owner.Birth.Value.Month.ToString() : "1" %>' CssClass="input_widthA input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;月&nbsp;&nbsp;
						<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue='<%# (this.CartList.Owner.Birth.HasValue) ? this.CartList.Owner.Birth.Value.Day.ToString() : "1" %>' CssClass="input_widthA input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;日
						<small>
						<asp:CustomValidator
							ID="cvOwnerBirth" runat="Server"
							ControlToValidate="ddlOwnerBirthDay"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</small>
						</dd>
						<%-- 性別 --%>
						<dt>
							<%: ReplaceTag("@@User.sex.name@@") %>
							&nbsp;<%if (this.IsLoggedIn) {%><span class="fred">※</span><span id="efo_sign_sex"/><% } %>
						</dt>
						<dd class="input_align">
						<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%# GetCorrectSexForDataBind(this.CartList.Owner.Sex) %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" CellSpacing="5" RepeatLayout="Flow" CssClass="input_radio" runat="server" />
						<small>
						<asp:CustomValidator
							ID="cvOwnerSex" runat="Server"
							ControlToValidate="rblOwnerSex"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" /></small>
						</dd>
						<%-- PCメールアドレス --%>
						<dt>
							<%: ReplaceTag("@@User.mail_addr.name@@") %>
							&nbsp;<span class="fred">※</span><span id="efo_sign_mail_addr"/>
						</dt>
						<dd><asp:TextBox ID="tbOwnerMailAddr" Text="<%# this.CartList.Owner.MailAddr %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" runat="server" Type="email"></asp:TextBox><br />
						<small>
						<asp:CustomValidator runat="Server"
							ID="cvOwnerMailAddr"
							ControlToValidate="tbOwnerMailAddr"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator runat="Server"
							ID="cvOwnerMailAddrForCheck" 
							ControlToValidate="tbOwnerMailAddr"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							CssClass="error_inline" />
						</small>
						</dd>
						<%-- PCメールアドレス（確認用） --%>
						<dt>
							<%: ReplaceTag("@@User.mail_addr.name@@") %>（確認用）
							&nbsp;<span class="fred">※</span><span id="efo_sign_mail_addr_conf"/>
						</dt>
						<dd><asp:TextBox ID="tbOwnerMailAddrConf" Text="<%# this.CartList.Owner.MailAddr %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" runat="server" Type="email"></asp:TextBox><br />
						<small>
						<asp:CustomValidator runat="Server"
							ID="cvOwnerMailAddrConf"
							ControlToValidate="tbOwnerMailAddrConf"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
						<%-- モバイルメールアドレス --%>
						<dt><%: ReplaceTag("@@User.mail_addr2.name@@")%></dt>
						<dd><asp:TextBox ID="tbOwnerMailAddr2" Text="<%# this.CartList.Owner.MailAddr2 %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" runat="server" Type="email"></asp:TextBox><br />
						<small>
						<asp:CustomValidator runat="Server"
							ID="cvOwnerMailAddr2"
							ControlToValidate="tbOwnerMailAddr2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%-- モバイルメールアドレス（確認用） --%>
						<dt>
							<%: ReplaceTag("@@User.mail_addr2.name@@")%>（確認用）
						</dt>
						<dd><asp:TextBox ID="tbOwnerMailAddr2Conf" Text="<%# this.CartList.Owner.MailAddr2 %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" runat="server" Type="email"></asp:TextBox><br />
						<small>
						<asp:CustomValidator runat="Server"
							ID="cvOwnerMailAddr2Conf"
							ControlToValidate="tbOwnerMailAddr2Conf"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<% } %>
						</dl>
					</div><!--bottom-->
					</div><!--top-->
					</div><!--userBox-->
				</div>
				<div style="width: 340px;">
					<div class="userBox">
					<div class="top">
					<div class="bottom">
						<dl>
						<%-- 郵便番号 --%>
						<dt>
							<%: ReplaceTag("@@User.zip.name@@") %>
							&nbsp;<span class="fred">※</span><span id="efo_sign_zip"/>
						</dt>
						<dd>
						<p class="pdg_topC">
							<asp:TextBox ID="tbOwnerZip" OnTextChanged="lbSearchOwnergAddr_Click" Text="<%#: this.CartList.Owner.Zip %>" CssClass="input_widthC input_border" Type="tel" MaxLength="8" runat="server" />
						</p>
						<span class="btn_add_sea"><asp:LinkButton ID="lbSearchOwnergAddr" runat="server" onclick="lbSearchOwnergAddr_Click" class="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span>
						<%--検索結果レイヤー--%>
						<uc:Layer ID="ucLayerForOwner" runat="server" />
						<p class="clr">
						<small class="fred">
						<asp:CustomValidator
							ID="cvOwnerZip1" runat="Server"
							ControlToValidate="tbOwnerZip"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline zip_input_error_message" />
						</small>
						<small id="sOwnerZipError" runat="server" class="fred shortZipInputErrorMessage"></small>
						</p></dd>
						<%-- 都道府県 --%>
						<dt>
							<%: ReplaceTag("@@User.addr1.name@@") %>
							&nbsp;<span class="fred">※</span><span id="efo_sign_addr1"/>
						</dt>
						<dd><asp:DropDownList ID="ddlOwnerAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# this.CartList.Owner.Addr1 %>" runat="server"></asp:DropDownList>
						<small>
						<asp:CustomValidator
							ID="cvOwnerAddr1" runat="Server"
							ControlToValidate="ddlOwnerAddr1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%-- 市区町村 --%>
						<dt>
							<%: ReplaceTag("@@User.addr2.name@@") %>
							&nbsp;<span class="fred">※</span><span id="efo_sign_addr2"/>
						</dt>
						<dd><asp:TextBox ID="tbOwnerAddr2" Text="<%# this.CartList.Owner.Addr2 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></asp:TextBox><br />
						<small>
						<asp:CustomValidator
							ID="cvOwnerAddr2" runat="Server"
							ControlToValidate="tbOwnerAddr2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%-- 番地 --%>
						<dt>
							<%: ReplaceTag("@@User.addr3.name@@") %>
							&nbsp;<span class="fred">※</span><span id="efo_sign_addr3"/>
						</dt>
						<dd><asp:TextBox ID="tbOwnerAddr3" Text="<%# this.CartList.Owner.Addr3 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></asp:TextBox><br />
						<small>
						<asp:CustomValidator
							ID="cvOwnerAddr3" runat="Server"
							ControlToValidate="tbOwnerAddr3"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%-- ビル・マンション名 --%>
						<dt>
							<%: ReplaceTag("@@User.addr4.name@@") %>
							&nbsp;<span class="fred"></span>
						</dt>
						<dd><asp:TextBox ID="tbOwnerAddr4" Text="<%# this.CartList.Owner.Addr4 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox><br />
						<small>
						<asp:CustomValidator ID="CustomValidator10" runat="Server"
							ControlToValidate="tbOwnerAddr4"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
						<%-- 企業名 --%>
						<dt>
							<%: ReplaceTag("@@User.company_name.name@@")%>
							&nbsp;<span class="fred"></span>
						</dt>
						<dd><asp:TextBox ID="tbOwnerCompanyName" Text="<%# this.CartList.Owner.CompanyName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></asp:TextBox><br />
						<small>
						<asp:CustomValidator ID="CustomValidator11" runat="Server"
							ControlToValidate="tbOwnerCompanyName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%-- 部署名 --%>
						<dt>
							<%: ReplaceTag("@@User.company_post_name.name@@")%>
							&nbsp;<span class="fred"></span>
						</dt>
						<dd><asp:TextBox ID="tbOwnerCompanyPostName" Text="<%# this.CartList.Owner.CompanyPostName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></asp:TextBox><br />
						<small>
						<asp:CustomValidator ID="CustomValidator12" runat="Server"
							ControlToValidate="tbOwnerCompanyPostName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" /></small>
						</dd>
						<%} %>
						<%-- 電話番号 --%>
						<dt>
							<%: ReplaceTag("@@User.tel1.name@@") %>
							&nbsp;<span class="fred">※</span><span id="efo_sign_tel1"/>
						</dt>
						<dd>
							<asp:TextBox ID="tbOwnerTel1" Text="<%#: this.CartList.Owner.Tel1 %>" CssClass="input_widthC input_border shortTel" MaxLength="13" Type="tel" runat="server" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
							<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
							<span class="btn_add_sea">
								<asp:LinkButton
									ID="lbGetAuthenticationCode"
									class="btn btn-mini"
									runat="server"
									Text="認証コードの取得"
									OnClick="lbGetAuthenticationCode_Click"
									OnClientClick="return checkTelNoInput();" />
							</span>
							<br />
							<asp:Label ID="lbAuthenticationStatus" runat="server" />
							<% } %>
							<br />
						<small>
						<asp:CustomValidator
							ID="cvOwnerTel1_1" runat="Server"
							ControlToValidate="tbOwnerTel1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						</small>
						</dd>
						<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
						<dt>
							<%: ReplaceTag("@@User.authentication_code.name@@") %>
						</dt>
						<dd>
							<asp:TextBox
								ID="tbAuthenticationCode"
								CssClass="input_widthA input_border"
								Enabled="false"
								MaxLength='<%# GetMaxLength("@@User.authentication_code.length_max@@") %>'
								runat="server" />
							<span class="notes">
								<% if (this.HasAuthenticationCode) { %>
								<asp:Label ID="lbHasAuthentication" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
								<% } %>
								<span><%: GetVerificationCodeNote() %></span>
								<asp:Label ID="lbAuthenticationMessage" runat="server" />
							</span>
							<br />
							<asp:CustomValidator
								ID="cvAuthenticationCode"
								runat="Server"
								ControlToValidate="tbAuthenticationCode"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="false"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						</dd>
						<% } %>
						<dt>
							<%: ReplaceTag("@@User.tel2.name@@") %>
						</dt>
						<dd>
							<asp:TextBox ID="tbOwnerTel2" Text="<%#: this.CartList.Owner.Tel2 %>" CssClass="input_widthD input_border shortTel" MaxLength="13" runat="server" /><br />
							<small>
							<asp:CustomValidator
								ID="cvOwnerTel2_1" runat="Server"
									ControlToValidate="tbOwnerTel2"
									ValidationGroup="OrderShipping"
									ValidateEmptyText="false"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</small>
						</dd>
						<dt>
							<%: ReplaceTag("@@User.mail_flg.name@@") %>
						</dt>
						<dd><asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text=" 配信する" CssClass="checkBox" runat="server" /></dd>
						<% if (IsTargetToExtendedAmazonAddressManagerOption()) { %>
						<asp:UpdatePanel id="UpdatePanel1" UpdateMode="Conditional" runat="server">
							<ContentTemplate>
							<dt>
								会員登録
							</dt>
							<dd>
								<asp:CheckBox
									ID="cbUserRegister"
									Checked="true"
									Enabled="true"
									Text=" 注文者情報で会員登録をする "
									OnCheckedChanged="cbUserRegister_OnCheckedChanged"
									CssClass="checkBox"
									runat="server"
									AutoPostBack="true" />
							</dd>
							</ContentTemplate>
						</asp:UpdatePanel>
						<% } %>
						</dl>

					</div><!--bottom-->
					</div><!--top-->
					</div><!--userBox-->
				</div>
			</div><!--column-->
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
			<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
			<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
			<% } %>
			<% if (IsTargetToExtendedAmazonAddressManagerOption()) { %>
			<asp:UpdatePanel id="upUserRegistRegulationForAmazonPay" UpdateMode="Conditional" runat="server">
				<ContentTemplate>
				<div id="dvUserRegisterRegulationVisible" runat="server">
					<div style="width: 780px">
						<div class="userBox-userRegistRegulation">
						<div class="top">
						<div class="bottom">
							<div id="dvUserBox">
								<div id="dvUserRegistRegulation" class="unit">
									<%-- メッセージ --%>
									<h3>会員規約について</h3>
									<div class="dvContentsInfo">
										<p>「<%: ShopMessage.GetMessage("ShopName") %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
										</p>
									</div>

									<div class="dvRegulation">
										<uc:UserRegistRegulationMessage ID="UserRegistRegulationMessage1" runat="server" />
									</div>
								</div>
							</div>
						</div>
						</div>
						</div>
					</div>
				</div>
				</ContentTemplate>
			</asp:UpdatePanel>
			<% } %>
			<%-- ▲注文者情報▲ --%>
			</ContentTemplate>
			</asp:UpdatePanel>
			<%-- ゲストの場合のみ注文者情報にAmazonのウィジェットを表示する --%>
			<div id="Div2" visible="<%# this.IsOrderOwnerInputEnabled == false %>" class="column" runat="server">
				<%--▼▼ Amazon Pay(CV2)注文者情報 ▼▼--%>
				<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
				<div class="userBox">
				<div class="top">
				<div class="bottom">
					<%-- 氏名 --%>
					<dt>
						<%: ReplaceTag("@@User.name.name@@") %>
					</dt>
					<dd>
						<%: this.CartList.Owner.Name %>
					<%-- PCメールアドレス --%>
					<dt>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
					</dt>
					<dd>
						<%: this.CartList.Owner.MailAddr %>
					</dd>
					<dt>
						<%: ReplaceTag("@@User.mail_flg.name@@") %>
					</dt>
					<dd>
						<asp:CheckBox ID="cbGuestOwnerMailFlg2" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" runat="server" />
					</dd>
					<div visible="<%# this.IsLoggedIn == false %>" runat="server">
						<dd>
							<%-- ▼Amazon Pay会員登録▼ --%>
							<% if (this.AmazonPayRegisterVisible && (this.IsLoggedIn == false)) { %>
							<asp:UpdatePanel ID="upAmazonPayRegisterUpdatePanel" UpdateMode="Conditional" Visible="<%# this.IsAmazonLoggedIn %>" runat="server">
								<ContentTemplate>
									<br/>
									<div class="checkBox">
										<asp:CheckBox
											ID="cbUserRegisterForExternalSettlement"
											Checked="true"
											Text='<%#: IsTargetToExtendedAmazonAddressManagerOption() ? " 注文者情報で会員登録をする " : " Amazonお届け先住所で会員登録する " %>'
											OnCheckedChanged="cbUserRegisterForExternalSettlement_OnCheckedChanged"
											AutoPostBack="true"
											CssClass="checkBox"
											runat="server" />
									</div>
									<div id="dvUserBoxVisible" runat="server">
										<div id="CartList">
											<div id="dvUserBox">
												<div id="dvUserRegistRegulation" class="unit">
													<%-- メッセージ --%>
													<h3>会員規約について</h3>
													<div class="dvContentsInfo">
														<p>「<%: ShopMessage.GetMessage("ShopName") %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
														</p>
													</div>
													<div class="dvRegulation">
														<uc:UserRegistRegulationMessage ID="UserRegistRegulationMessage" runat="server" />
													</div>
												</div>
											</div>
										</div>
									</div>
								</ContentTemplate>
							</asp:UpdatePanel>
							<% } %>
							<%-- ▲Amazon Pay会員登録▲ --%>
						</dd>
					</div>
				</div>
				</div>
				</div>
				<% } else { %>
				<%--▲▲ Amazon Pay(CV2)注文者情報 ▲▲--%>
				<div id="ownerAddressBookContainer" style="width:780px;">
					<%-- ▼▼Amazonアドレス帳ウィジェット(注文者情報)▼▼ --%>
					<div id="ownerAddressBookWidgetDiv" style="width:780px;height:300px;"></div>
					<div id="ownerAddressBookErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
					<%-- ▲▲Amazonアドレス帳ウィジェット(注文者情報)▲▲ --%>
				</div>
				<dt>
					<%: ReplaceTag("@@User.mail_flg.name@@") %>
				</dt>
				<dd><asp:CheckBox ID="cbGuestOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text=" 配信する" CssClass="checkBox" runat="server" /></dd>
				<% } %>
			</div>
			<%-- ▼▼AmazonリファレンスID格納▼▼ --%>
			<asp:HiddenField runat="server" ID="hfAmazonOrderRefID" />
			<%-- ▲▲AmazonリファレンスID格納▲▲ --%>
		</div>
		<br class="clr" />
		<div class="main">
		<div class="submain">
			<div class="column">
				<h2><img src="../../Contents/ImagesPkg/order/sttl_esd.gif" alt="配送先情報" width="80" height="16" /></h2>

				<div id="divShipToOwnerAddress" Visible="<%# (this.IsLoggedIn == false) && CanInputShippingTo(Container.ItemIndex) && (Constants.AMAZON_PAYMENT_CV2_ENABLED == false) %>" runat="server">
					<asp:CheckBox ID="cbShipToOwnerAddress" Text="注文者情報の住所へ配送する" Checked="<%# ((CartObject)Container.DataItem).Shippings[0].IsSameShippingAsCart1 %>" onclick="$('#shippingAddressBookContainer').toggle();" CssClass="checkBox" runat="server" />
				</div>
				<%--▼▼ Amazon Pay(CV2)配送先情報 ▼▼--%>
				<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
				<div style="width: 340px;">
				<div class="userBox">
				<div class="top">
				<div class="bottom">
					<dl>
										
					<%-- 住所 --%>
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>：
					</dt>
					<dd>
						<p>
							〒<%: StringUtility.ToEmpty(this.AmazonCheckoutSession.ShippingAddress.PostalCode) %><br />
							<%: StringUtility.ToEmpty(this.AmazonCheckoutSession.ShippingAddress.StateOrRegion) %><br />
							<%: StringUtility.ToEmpty(this.AmazonCheckoutSession.ShippingAddress.AddressLine1) %><br />
							<%: StringUtility.ToEmpty(this.AmazonCheckoutSession.ShippingAddress.AddressLine2) %><br />
							<%: StringUtility.ToEmpty(this.AmazonCheckoutSession.ShippingAddress.AddressLine3) %>
						</p>
					</dd>
					<dt>
						<%: ReplaceTag("@@User.name.name@@") %>：
					</dt>
					<dd>
						<p><%: StringUtility.ToEmpty(this.AmazonCheckoutSession.ShippingAddress.Name) %></p>
					</dd>
					</dl>
					<br />
					<asp:LinkButton ID="lbChangeAddress" ClientIDMode="Static" class="btn btn-mini" style="text-decoration:none; margin:10px 0;" runat="server" OnClientClick="return false;">配送先住所を変更</asp:LinkButton><br />
					<p id="pShippingZipError" runat="server" class="fred shortZipInputErrorMessage"></p>
				</div><!--bottom-->
				</div><!--top-->
				</div><!--userBox-->
				</div>
				<% } else { %>
				<%--▲▲ Amazon Pay(CV2)配送先情報 ▲▲--%>
				<div id="shippingAddressBookContainer" style='margin-top: 10px;<%= (this.IsLoggedIn == false) && this.CartList.Items[0].GetShipping().IsSameShippingAsCart1 ? "display:none;" : "" %>'>
					<%-- ▼▼Amazonアドレス帳ウィジェット(配送先情報)▼▼ --%>
					<div id="shippingAddressBookWidgetDiv" style="width:340px;height:300px;"></div>
					<div id="shippingAddressBookErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
					<%-- ▲▲Amazonアドレス帳ウィジェット(配送先情報)▲▲ --%>
				</div>
				<% } %>
				<div class="orderBox">
					<div class="bottom">
						<asp:UpdatePanel runat="server">
						<ContentTemplate>
							<h4 visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送指定</h4>
							<div visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server" class="userList">
							<div>
							配送方法を選択して下さい。
							<br />
								<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[((RepeaterItem)Container).ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" runat="server"></asp:DropDownList>
							</div>
							<div id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(((RepeaterItem)Container).ItemIndex) && CanDisplayDeliveryCompany(((RepeaterItem)Container).ItemIndex)) %>" runat="server">
							<br />
							配送サービスを選択して下さい。
							<br />
							<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(((RepeaterItem)Container).ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" runat="server"/>
							</div>
							<br />
								<div id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server" style='<%# HasFixedPurchase((RepeaterItem)Container) && (DisplayFixedPurchaseShipping((RepeaterItem)Container) == false) ? "padding-bottom: 0px" : "" %>'>
							配送希望日時を選択して下さい。
							<dl id="dlShipppingDateTime" runat="server">
								<dd></dd>
										<dt id="dtShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送希望日</dt>
										<dd id="ddShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">
											<asp:DropDownList ID="ddlShippingDate" CssClass="input_border" runat="server" DataSource="<%# GetShippingDateList(((CartObject)((RepeaterItem)Container).DataItem), this.ShopShippingList[((RepeaterItem)Container).ItemIndex]) %>" DataTextField="text" DataValueField="value" SelectedValue="<%# GetShippingDate((CartObject)((RepeaterItem)Container).DataItem, this.ShopShippingList[((RepeaterItem)Container).ItemIndex]) %>"
												OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnCheckedChanged" AutoPostBack="true"></asp:DropDownList>
											<br />
											<asp:Label ID="lbShippingDateErrorMessage" style="color:red;line-height:1.5;" runat="server" />
										</dd>
										<dt id="dtShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送希望時間帯</dt>
										<dd id="ddShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server" class="last">
											<asp:DropDownList ID="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(((RepeaterItem)Container).ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingTime(((RepeaterItem)Container).ItemIndex) %>"></asp:DropDownList>
										</dd>
									</dl>
								</div>
							</div>
							<h4 id="H2" style="margin-top:15px" visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">定期購入 配送パターンの指定</h4>
							<%-- ▽デフォルトチェックの設定▽--%>
							<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
							<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 2, 3, 1, 4) %>
							<%-- △ - - - - - - - - - - - △--%>
							<div id="Div3" visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase %>" runat="server" class="list" style='<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) ? "" : "margin-top: 0px;padding-top: 0px" %>'><span id="efo_sign_fixed_purchase"></span>
								<span visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">「定期購入」はご希望の配送パターン・配送時間を指定して定期的に商品をお届けするサービスです。下記の配送パターンからお選び下さい。</span>

								<div id='<%# "efo_sign_fixed_purchase" + ((RepeaterItem)Container).ItemIndex %>'></div>
								<dl style="margin-top: 10px;" visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">
									<dt id="Dt1" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
										<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
											Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 1) %>" 
											GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_month" + ((RepeaterItem)Container).ItemIndex %>' /></dt>
									<dd id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">
										<asp:DropDownList ID="ddlFixedPurchaseMonth"
											DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true) %>"
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
											runat="server">
										</asp:DropDownList>
										ヶ月ごと
										<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
											DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, false, true) %>"
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
											runat="server">
										</asp:DropDownList>
										日に届ける
									</dd>
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseMonth"
										runat="Server"
										ControlToValidate="ddlFixedPurchaseMonth" 
										ValidationGroup="OrderShipping" 
										ValidateEmptyText="true" 
										SetFocusOnError="true" 
										CssClass="error_inline"/>
									</small>
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseMonthlyDate" runat="Server"
										ControlToValidate="ddlFixedPurchaseMonthlyDate" 
										ValidationGroup="OrderShipping" 
										ValidateEmptyText="true" 
										SetFocusOnError="true" 
										CssClass="error_inline"/>
									</small>
									<dt id="Dt2" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
										<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
											Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 2) %>" 
											GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_week_and_day" + ((RepeaterItem)Container).ItemIndex %>' /></dt>
									<dd id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">　
										<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
											DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, true) %>"
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server" />
										ヶ月ごと
										<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
											DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
											runat="server">
										</asp:DropDownList>
										<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
											DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
											runat="server">
										</asp:DropDownList>
										に届ける
									</dd>
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseIntervalMonths"
										runat="Server"
										ControlToValidate="ddlFixedPurchaseIntervalMonths"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										CssClass="error_inline"/>
									</small>
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseWeekOfMonth"
										runat="Server"
										ControlToValidate="ddlFixedPurchaseWeekOfMonth" 
										ValidationGroup="OrderShipping" 
										ValidateEmptyText="true" 
										SetFocusOnError="true" 
										CssClass="error_inline"/>
									</small>
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseDayOfWeek"
										runat="Server"
										ControlToValidate="ddlFixedPurchaseDayOfWeek" 
										ValidationGroup="OrderShipping" 
										ValidateEmptyText="true" 
										SetFocusOnError="true" 
										CssClass="error_inline"/>
									</small>
									<dt id="Dt3" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
										<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
											Text="配送日間隔指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 3) %>" 
											GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_interval_days" + ((RepeaterItem)Container).ItemIndex %>' /></dt>
									<dd id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">　
										<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
											DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, false) %>'
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
											runat="server">
									</asp:DropDownList>
									</dd>
									<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
									<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseIntervalDays"
										runat="Server"
										ControlToValidate="ddlFixedPurchaseIntervalDays" 
										ValidationGroup="OrderShipping" 
										ValidateEmptyText="true" 
										SetFocusOnError="true" 
										CssClass="error_inline"/>
									</small>
									<dt id="Dt4" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
										<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
											Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 4) %>"
											GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%#: "efo_sign_fixed_purchase_week" + ((RepeaterItem)Container).ItemIndex %>' /></dt>
									<dd id="ddFixedPurchaseEveryNWeek" runat="server" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>">
										<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
											DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container).ItemIndex, true) %>"
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
											runat="server">
										</asp:DropDownList>
										週間ごと
										<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
											DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container).ItemIndex, false) %>"
											DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
											OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
											runat="server">
										</asp:DropDownList>
										に届ける
									</dd>
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseEveryNWeek"
										runat="Server"
										ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										CssClass="error_inline"/>
									</small>
									<small>
									<asp:CustomValidator
										ID="cvFixedPurchaseEveryNWeekDayOfWeek"
										runat="Server"
										ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										CssClass="error_inline"/>
									</small>
								</dl>
								<small ID="sErrorMessage" class="fred" runat="server"></small>
								<br /><hr />
								<dl>
									<dt id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</dt>
									<dd id="Dd1" visible="true" runat="server" style="padding-left: 20px;">
										<asp:Label ID="lblFirstShippingDate" runat="server"></asp:Label>
										<asp:DropDownList
											ID="ddlFirstShippingDate"
											visible="false"
											OnDataBound="ddlFirstShippingDate_OnDataBound"
											AutoPostBack="True"
											OnSelectedIndexChanged="ddlFirstShippingDate_ItemSelected"
											runat="server" />
										<asp:Label ID="lblFirstShippingDateNoteMessage" visible="false" runat="server">
											<br>配送予定日は変更となる可能性がありますことをご了承ください。
										</asp:Label>
										<asp:Literal ID="lFirstShippingDateDayOfWeekNoteMessage" visible="false" runat="server">
											<br>曜日指定は次回配送日より適用されます。
										</asp:Literal>
									</dd>
									<dt id="dtNextShippingDate" visible="true" runat="server">2回目の配送日を選択</dt>
									<dd id="Dd2" visible="true" runat="server" style="padding-left: 20px;">
										<asp:Label ID="lblNextShippingDate" visible="false" runat="server"></asp:Label>
										<asp:DropDownList
											ID="ddlNextShippingDate"
											visible="false"
											OnDataBound="ddlNextShippingDate_OnDataBound"
											OnSelectedIndexChanged="ddlNextShippingDate_OnSelectedIndexChanged"
											AutoPostBack="True"
											runat="server" />
									</dd>
								</dl>
								<dl>
									メール便の場合は数日ずれる可能性があります。
								</dl>
						</div>

						<!--userList-->
						<asp:Repeater ID="rMemos" runat="server" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos %>" Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos.Count != 0 %>">
							<HeaderTemplate>
								<h4>注文メモ</h4>
								<div class="list">
							</HeaderTemplate>
							<ItemTemplate>
								<strong><%# WebSanitizer.HtmlEncode(Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME)) %></strong>
								<p>
									<asp:TextBox ID="tbMemo" runat="server" Text="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine"></asp:TextBox><br />
								</p>
								<br />
								<small id="sErrorMessageMemo" runat="server" class="fred"></small>
								<%-- IDに"OtherValidator"を含めることで案件毎に追加したtextareaなどでチェック可能 --%>
								<asp:CustomValidator
									ID="OtherValidator"
									runat="Server"
									ControlToValidate="tbMemo"
									ValidationGroup="OrderShipping"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate" />
							</ItemTemplate>
							<FooterTemplate>
								</div>
							</FooterTemplate>
						</asp:Repeater>
						<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server" Visible="<%# IsDisplayOrderExtend() %>" >
							<HeaderTemplate>
								<h4>注文確認事項</h4>
								<div class="list">
							</HeaderTemplate>
							<ItemTemplate>
								<%-- 項目名 --%>
								<p>
									<strong><%#: Item.SettingModel.SettingName %></strong>
									<span class="fred"  runat="server" visible="<%# Item.SettingModel.IsNeecessary%>">※</span>
									<%-- 概要 --%>
									<div><%# Item.SettingModel.OutlineHtmlEncode %></div>
									<%-- TEXT --%>
									<div runat="server" visible="<%# Item.SettingModel.IsInputTypeText%>">
										<asp:TextBox runat="server" ID="tbSelect" Width="250px" MaxLength="100"></asp:TextBox>
									</div>
									<%-- DDL --%>
									<div runat="server" visible="<%# Item.SettingModel.IsInputTypeDropDown %>">
										<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
									</div>
									<%-- RADIO --%>
									<div runat="server" visible="<%# Item.SettingModel.IsInputTypeRadio %>">
										<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
									</div>
									<%-- CHECK --%>
									<div runat="server" visible="<%# Item.SettingModel.IsInputTypeCheckBox %>">
										<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
									</div>
									<%-- 検証文言 --%>
									<small><asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label></small>
									<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
									<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
								</p>
							</ItemTemplate>
							<FooterTemplate>
								</div>
							</FooterTemplate>
						</asp:Repeater>
						</ContentTemplate>
						</asp:UpdatePanel>
					</div>
				</div>
				<br />
				<br />
	</div>
	<%-- ▼カート情報▼ --%>
	<asp:UpdatePanel ID="upShoppingCart" runat="server">
	<ContentTemplate>
	<div class="shoppingCart">
		<div id="Div7" runat="server">
			<h2>
				<img src="../../Contents/ImagesPkg/common/ttl_shopping_cart.gif" alt="ショッピングカート" width="141" height="16" /></h2>
			<div class="sumBox mrg_topA">
				<div class="subSumBoxB">
					<p>
						<img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" /><strong><%: CurrencyManager.ToPrice(this.CartList.PriceCartListTotalWithOutPaymentPrice) %></strong>
					</p>
				</div>
			</div>
			<!--sum-->
		</div>

		<div class="subCartList">
			<div class="bottom">
				<h3>
					<div class="cartNo">
						カート番号
					</div>
					<div class="cartLink"><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>">カートへ戻る</a></div>
				</h3>
				<div class="block">

					<asp:Repeater ID="rCart" DataSource="<%# this.CartList.Items.FirstOrDefault() %>" runat="server">
						<ItemTemplate>
							<%-- 通常商品 --%>
							<div id="Div1" class="singleProduct" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
								<div>
									<dl>
										<dt>
											<a id="A1" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
												<w2c:ProductImage ID="ProductImage1" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
											</a>
											<w2c:ProductImage ID="ProductImage2" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
										</dt>
										<dd>
											<strong>
												<a id="A2" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
													<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
												<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
											</strong>
											<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</small>" : "" %>
											<p id="P1" visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
												<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
													<ItemTemplate>
														<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<strong>" : "" %>
														<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
														<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "</strong>" : "" %>
													</ItemTemplate>
												</asp:Repeater>
											</p>
											<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet) %></p>
											<p><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %> (<%#: this.ProductPriceTextPrefix %>)</p>
											<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
												※配送料無料適用外商品です
											</p>
										</dd>
									</dl>
								</div>
							</div>
							<!--singleProduct-->
							<%-- セット商品 --%>
							<div id="Div9" class="multiProduct" visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
								<asp:Repeater ID="rProductSet" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" runat="server">
									<ItemTemplate>
										<div>
											<dl>
												<dt>
													<a id="A3" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
														<w2c:ProductImage ID="ProductImage3" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
													</a>
													<w2c:ProductImage ID="ProductImage4" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
												</dt>
												<dd>
													<strong>
														<a id="A4" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
															<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
														<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
													</strong>
													<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</small>" : "" %>
													<p><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) %></p>
													<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
														※配送料無料適用外商品です
													</p>
												</dd>
											</dl>
										</div>
										<table id="Table1" visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == ((CartProduct)Container.DataItem).ProductSet.Items.Count) %>" width="297" cellpadding="0" cellspacing="0" class="clr" runat="server">
											<tr>
												<th width="38">セット：</th>
												<th width="50"><%# GetProductSetCount((CartProduct)Container.DataItem) %></th>
												<th width="146"><%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal((CartProduct)Container.DataItem)) %> (<%#: this.ProductPriceTextPrefix %>)</th>
												<td width="61"></td>
											</tr>
										</table>
									</ItemTemplate>
								</asp:Repeater>
							</div>
							<!--multiProduct-->
						</ItemTemplate>
					</asp:Repeater>
					<%-- セットプロモーション商品 --%>
					<asp:Repeater ID="rCartSetPromotion" DataSource="<%# this.CartList.Items.FirstOrDefault().SetPromotions %>" runat="server">
						<ItemTemplate>
							<div class="multiProduct">
								<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" runat="server">
									<ItemTemplate>
										<div>
											<dl>
												<dt>
													<a id="A5" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
														<w2c:ProductImage ID="ProductImage5" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
													</a>
													<w2c:ProductImage ID="ProductImage6" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
												</dt>
												<dd>
													<strong>
														<a id="A6" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
															<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
														<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
													</strong>
													<p id="P2" visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
														<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
															<ItemTemplate>
																<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<strong>" : "" %>
																<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
																<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "</strong>" : "" %>
															</ItemTemplate>
														</asp:Repeater>
													</p>
													<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]) %></p>
													<p><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
													<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
														※配送料無料適用外商品です
													</p>
												</dd>
											</dl>
										</div>
									</ItemTemplate>
								</asp:Repeater>
								<dl class="setpromotion">
									<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
									<dd>
										<span id="Span3" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
											<strike><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (<%#: this.ProductPriceTextPrefix %>)</strike>
											<br />
										</span>
											<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (<%#: this.ProductPriceTextPrefix %>)
									</dd>
								</dl>
							</div>
						</ItemTemplate>
					</asp:Repeater>

					<div class="priceList">
						<div>
							<dl class="bgc">
								<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
								<dd><%#: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().PriceSubtotal) %></dd>
							</dl>
							<%if (this.ProductIncludedTaxFlg == false) { %>
								<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
									<dt>消費税</dt>
									<dd><%#: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().PriceSubtotalTax) %></dd>
								</dl>
							<%} %>
							<%-- セットプロモーション割引額(商品割引) --%>
							<asp:Repeater ID="Repeater1" DataSource="<%# this.CartList.Items.FirstOrDefault().SetPromotions %>" runat="server">
								<ItemTemplate>
									<span id="Span4" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
										<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
											<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
											<dd class='<%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %></dd>
										</dl>
									</span>
								</ItemTemplate>
							</asp:Repeater>
							<%if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn) { %>
							<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
								<dt>会員ランク割引額</dt>
								<dd class='<%# (this.CartList.Items.FirstOrDefault().MemberRankDiscount > 0) ? "minus" : "" %>'><%# (this.CartList.Items.FirstOrDefault().MemberRankDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().MemberRankDiscount * ((this.CartList.Items.FirstOrDefault().MemberRankDiscount < 0) ? -1 : 1)) %></dd>
							</dl>
							<%} %>
							<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
							<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
								<dt>ポイント利用額</dt>
								<dd class='<%# (this.CartList.Items.FirstOrDefault().UsePointPrice > 0) ? "minus" : "" %>'><%# (this.CartList.Items.FirstOrDefault().UsePointPrice > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().UsePointPrice * ((this.CartList.Items.FirstOrDefault().UsePointPrice < 0) ? -1 : 1)) %></dd>
							</dl>
							<%} %>
							<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
							<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
								<dt>クーポン割引額</dt>
								<dd class='<%# (this.CartList.Items.FirstOrDefault().UseCouponPrice > 0) ? "minus" : "" %>'>
									<%#: GetCouponName(((CartObject)((RepeaterItem)Container).DataItem)) %>
									<%# (this.CartList.Items.FirstOrDefault().UseCouponPrice > 0) ? "-" : "" %>
									<%#: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().UseCouponPrice * ((this.CartList.Items.FirstOrDefault().UseCouponPrice < 0) ? -1 : 1)) %>
								</dd>
							</dl>
							<%} %>
							<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
								<dt>配送料金</dt>
								<dd id="Dd5" runat="server" style='<%# (this.CartList.Items.FirstOrDefault().ShippingPriceSeparateEstimateFlg) ? "display:none;": ""%>'><%: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().PriceShipping) %></dd>
								<dd id="Dd6" runat="server" style='<%# (this.CartList.Items.FirstOrDefault().ShippingPriceSeparateEstimateFlg == false) ? "display:none;": ""%>'>
									<%# WebSanitizer.HtmlEncode(this.CartList.Items.FirstOrDefault().ShippingPriceSeparateEstimateMessage)%></dd>
								<small style="color:red;" visible="<%# this.CartList.Items.FirstOrDefault().IsDisplayFreeShiipingFeeText %>" runat="server">
									※配送料無料適用外の商品が含まれるため、<%#: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().PriceShipping) %>の配送料が請求されます
								</small>
							</dl>
							<%-- セットプロモーション割引額(配送料割引) --%>
							<asp:Repeater ID="Repeater2" DataSource="<%# this.CartList.Items.FirstOrDefault().SetPromotions %>" runat="server">
								<ItemTemplate>
									<spna visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
										<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
											<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>(送料割引)</dt>
											<dd class='<%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %></dd>
										</dl>
									</spna>
								</ItemTemplate>
							</asp:Repeater>
						</div>
						<p class="clr">
							<img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" />
						</p>
						<div>
							<dl class="result">
								<dt>合計(税込)</dt>
								<dd><%: CurrencyManager.ToPrice(this.CartList.Items.FirstOrDefault().PriceCartTotalWithoutPaymentPrice) %></dd>
							</dl>
						</div>
					</div>
					<!--priceList-->

				</div>
				<!--block-->
			</div>
			<!--bottom-->
		</div>
		<!--subCartList-->

		<div id="Div12" runat="server">
			<div class="sumBox">
				<div class="subSumBox">
					<p>
						<img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" /><strong><%: CurrencyManager.ToPrice(this.CartList.PriceCartListTotalWithOutPaymentPrice) %></strong>
					</p>
				</div>
			</div>
			<!--sumBox-->
		</div>

	</div>
	<!--shoppingCart-->
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="lbPostBack" EventName="Click"/>
	</Triggers>
	</asp:UpdatePanel>
	<asp:LinkButton ID="lbPostBack" ClientIDMode="Static" runat="server"/>
	<%-- ▲カート情報▲ --%>
	</div>
	<br class="clr" />
	</div>
	<div class="main" style="background-image: none;">
		<div class="column">
			<%--▼▼ Amazon Pay(CV2)お支払い方法 ▼▼--%>
			<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
			<div class="bottom">
			<h2><img src="../../Contents/ImagesPkg/order/sttl_cash.gif" alt="お支払い情報" width="95" height="16" /></h2>
			<div style="font-size: 14px;" class="text-center m-3">
				<img src="../../Contents/ImagesPkg/Amazon/logo-amzn_pay.png" height="20px" align="top">&nbsp;&nbsp;
				<%= (this.AmazonCheckoutSession.PaymentPreferences.Count > 0) ? this.AmazonCheckoutSession.PaymentPreferences[0].PaymentDescriptor : "" %>
				<asp:LinkButton ID="lbChangePayment" ClientIDMode="Static" class="btn btn-mini" style="text-decoration:none; margin:10px 0;" runat="server" OnClientClick="return false;">お支払い方法を変更</asp:LinkButton>
			</div>
			</div>
			<% } else { %>
			<%--▲▲ Amazon Pay(CV2)お支払い方法 ▲▲--%>
			<h2><img src="../../Contents/ImagesPkg/order/sttl_cash.gif" alt="お支払い情報" width="95" height="16" /></h2>
			<%-- ▼▼Amazon決済ウィジェット▼▼ --%>
			<div id="walletWidgetDiv" style="width:780px;height:300px;"></div>
			<%-- ▲▲Amazon決済ウィジェット▲▲ --%>
				<% if (this.CartList.Items[0].HasFixedPurchase) { %>
				<div style="margin: 10px 0;">下記のお支払い契約に同意してください。</div>
				<%-- ▼▼Amazon支払契約同意ウィジェット▼▼ --%>
				<div id="consentWidgetDiv" style="width:780px;height:105px;margin-top: 0.5em;"></div>
				<%-- ▲▲Amazon支払契約同意ウィジェット▲▲ --%>
				<%-- ▼▼Amazon支払契約ID格納▼▼ --%>
				<asp:HiddenField runat="server" ID="hfAmazonBillingAgreementId" />
				<%-- ▲▲Amazon支払契約ID格納▲▲ --%>
				<% } %>
			<% } %>
			<asp:UpdatePanel ID="upConstraintErrorMessageContainer" runat="server">
			<ContentTemplate>
				<div id="constraintErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
			</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	<br class="clr" />
	</div>
	<%-- ▼領収書情報▼ --%>
	<asp:UpdatePanel ID="upReceiptInfo" runat="server">
	<ContentTemplate>
	<% if (Constants.RECEIPT_OPTION_ENABLED && (Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) == false)) { %>
	<div class="order-amazonpay-receipt" id="divReceipt">
	<div class="input">
		<h4>領収書情報</h4>
		<div id="divReceiptInfoInputForm" runat="server" class="receipt-info">
			<strong>領収書希望有無を選択してください。</strong>
			<dd><asp:DropDownList id="ddlReceiptFlg" runat="server" DataTextField="text" DataValueField="value" DataSource="<%# this.ReceiptFlgListItems %>"
				SelectedValue="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptFlg %>"
				OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" />
			</dd>
			<div id="divReceiptAddressProviso" runat="server">
			<dt>宛名<span class="fred">※</span></dt>
			<dd>
				<asp:TextBox id="tbReceiptAddress" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptAddress %>" MaxLength="100" Width="600" />
				<p><asp:CustomValidator runat="Server"
					ControlToValidate="tbReceiptAddress"
					ValidationGroup="ReceiptRegisterModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"/></p>
			</dd>
			<dt>但し書き<span class="fred">※</span></dt>
			<dd class="last">
				<asp:TextBox id="tbReceiptProviso" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptProviso %>" MaxLength="100" Width="600" />
				<p><asp:CustomValidator runat="Server"
					ControlToValidate="tbReceiptProviso"
					ValidationGroup="ReceiptRegisterModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"/></p>
			</dd>
			</div><!--divReceiptAddressProviso-->
		</div><!--divReceiptInfoInputForm-->
	</div><!--input-->
	</div><!--divReceipt-->
	<% } %>
	</ContentTemplate>
	</asp:UpdatePanel>
	<%-- ▲領収書情報▲ --%>
</ItemTemplate>
</asp:Repeater>
<div class="btmbtn below">
	<ul>
		<asp:UpdatePanel runat="server">
		<ContentTemplate>
		<li>
			<asp:LinkButton runat="server" ID="lbNext" CssClass="btn btn-large btn-success" OnClick="lbNext_Click"><%= IsChangeMessageForNextButton() ? "会員登録してご注文内容の確認へ" : "確認画面へ" %></asp:LinkButton>
		</li>
		</ContentTemplate>
		</asp:UpdatePanel>
	</ul>
</div>
<script>
	bindEvent();

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
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		execAutoKanaWithKanaType(
			$('#<%= ((TextBox)ri.FindControl("tbOwnerName1")).ClientID %>'),
			$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana1")).ClientID %>'),
			$('#<%= ((TextBox)ri.FindControl("tbOwnerName2")).ClientID %>'),
			$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana2")).ClientID %>'));
		<%} %>
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			execAutoChangeKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana2")).ClientID %>'));
		<%} %>
	}

	var bindTargetForAddr1 = "";
	var bindTargetForAddr2 = "";
	var bindTargetForAddr3 = "";
	var multiAddrsearchTriggerType = "";
	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		// Check owner zip code input on click
		clickSearchZipCodeInRepeater(
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (ri.FindControl("sOwnerZipError")).ClientID %>',
			"owner");

		// Check owner zip code input on text box change
		textboxChangeSearchZipCodeInRepeater(
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").UniqueID %>',
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").UniqueID %>',
			'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").ClientID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (ri.FindControl("sOwnerZipError")).ClientID %>',
			"owner");
		<%} %>
	}

	$(document).on('click', '.search-result-layer-close', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function (e) {
		bindSelectedAddr($('li.search-result-layer-addr').index(this), multiAddrsearchTriggerType);
	});

	<%-- 複数住所検索結果からの選択値を入力フォームにバインドする --%>
	function bindSelectedAddr(selectedIndex, multiAddrsearchTriggerType) {
		var selectedAddr = $('.search-result-layer-addrs li').eq(selectedIndex);
		if (multiAddrsearchTriggerType == "owner") {
			<% foreach (RepeaterItem ri in rCartList.Items) { %>
			$('#<%= ((DropDownList)ri.FindControl("ddlOwnerAddr1")).ClientID %>').val(selectedAddr.find('.addr').text());
			$('#<%= ((TextBox)ri.FindControl("tbOwnerAddr2")).ClientID %>').val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
			$('#<%= ((TextBox)ri.FindControl("tbOwnerAddr3")).ClientID %>').focus();
			<%} %>
		}

		closePopupAndLayer();
	}
</script>

<%-- ▼▼Amazonウィジェット用スクリプト▼▼ --%>
<script type="text/javascript">

	window.onAmazonLoginReady = function () {
		amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
	};
	window.onAmazonPaymentsReady = function () {
		showAddressBookWidget();
	};

	<%-- Amazonアドレス帳表示ウィジェット --%>
	function showAddressBookWidget() {
		<%-- Amazon注文者情報 --%>
		<% if (this.IsLoggedIn == false) { %>
		new OffAmazonPayments.Widgets.AddressBook({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			<% if (this.CartList.Items[0].HasFixedPurchase) { %>
			agreementType: 'BillingAgreement',
			<% } %>
			onReady: function (arg) {
				<% if (this.CartList.Items[0].HasFixedPurchase) { %>
				var billingAgreementId = arg.getAmazonBillingAgreementId();
				$('#<%# this.WhfAmazonBillingAgreementId.ClientID %>').val(billingAgreementId);
				<% } else { %>
				var orderReferenceId = arg.getAmazonOrderReferenceId();
				$('#<%# this.WhfAmazonOrderRefID.ClientID %>').val(orderReferenceId);
				<% } %>
			},
			onAddressSelect: function (orderReference) {
				var $ownerAddressBookErrorMessage = $('#ownerAddressBookErrorMessage');
				$ownerAddressBookErrorMessage.empty();
				getAmazonAddress('<%=(this.CartList.Items[0].HasFixedPurchase) ? w2.App.Common.Amazon.AmazonConstants.OrderType.AutoPay : w2.App.Common.Amazon.AmazonConstants.OrderType.OneTime %>', '<%= w2.App.Common.Amazon.AmazonConstants.AddressType.Owner %>', function (response) {
					var data = JSON.parse(response.d);
					if ($('#<%= WcbShipToOwnerAddress.ClientID %>').prop('checked') && (typeof data.RequestPostBack !== "undefined")) location.href = $("#lbPostBack").attr('href');
					if (data.Error) $ownerAddressBookErrorMessage.html(data.Error);
				});
			},
			design: { designMode: 'responsive' },
			onError: function (error) {
				var message = error.getErrorMessage();
				switch (error.getErrorCode()) {
				case 'UnknownError':
					message = 'エラーが発生しました。\r\n時間を空けて再度お試しください。';
					break;
				}
				alert(message);
			}
		}).bind("ownerAddressBookWidgetDiv");
		<% } %>

		<%-- Amazon配送先情報 --%>
		new OffAmazonPayments.Widgets.AddressBook({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			<% if (this.CartList.Items[0].HasFixedPurchase) { %>
			agreementType: 'BillingAgreement',
			<% } %>
			onReady: function (arg) {
				<% if (this.CartList.Items[0].HasFixedPurchase) { %>
				var billingAgreementId = arg.getAmazonBillingAgreementId();
				$('#<%# this.WhfAmazonBillingAgreementId.ClientID %>').val(billingAgreementId);
				<% } else { %>
				var orderReferenceId = arg.getAmazonOrderReferenceId();
				$('#<%# this.WhfAmazonOrderRefID.ClientID %>').val(orderReferenceId);
				<% } %>
				showWalletWidget(arg);
				showConsentWidget(arg);
			},
			onAddressSelect: function (orderReference) {
				var $shippingAddressBookErrorMessage = $('#shippingAddressBookErrorMessage');
				$shippingAddressBookErrorMessage.empty();
				getAmazonAddress('<%=(this.CartList.Items[0].HasFixedPurchase) ? w2.App.Common.Amazon.AmazonConstants.OrderType.AutoPay : w2.App.Common.Amazon.AmazonConstants.OrderType.OneTime %>', '<%= w2.App.Common.Amazon.AmazonConstants.AddressType.Shipping %>', function (response) {
					var data = JSON.parse(response.d);
					if (typeof data.RequestPostBack !== "undefined") location.href = $("#lbPostBack").attr('href');
					if (data.Error) $shippingAddressBookErrorMessage.html(data.Error);
				});
			},
			design: { designMode: 'responsive' },
			onError: function (error) {
				var message = error.getErrorMessage();
				switch (error.getErrorCode()) {
				case 'UnknownError':
					message = 'エラーが発生しました。\r\n時間を空けて再度お試しください。';
					break;
				}
				alert(message);
			}
		}).bind("shippingAddressBookWidgetDiv");
	}

	<%-- Amazon決済方法表示ウィジェット --%>
	function showWalletWidget(arg) {
		new OffAmazonPayments.Widgets.Wallet({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			<% if (this.CartList.Items[0].HasFixedPurchase) { %>
			agreementType: 'BillingAgreement',
			amazonBillingAgreementId: arg.getAmazonBillingAgreementId(),
			<% } %>
			design: { designMode: 'responsive' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		}).bind("walletWidgetDiv");
	}

	<%-- Amazon支払契約同意ウィジェット --%>
	function showConsentWidget(arg) {
		new OffAmazonPayments.Widgets.Consent({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			amazonBillingAgreementId: arg.getAmazonBillingAgreementId(),
			onConsent: function (billingAgreementConsentStatus) {
				buyerBillingAgreementConsentStatus = billingAgreementConsentStatus.getConsentStatus();
				if (buyerBillingAgreementConsentStatus) {
					$('#constraintErrorMessage').empty();
				}
			},
			design: { designMode: 'responsive' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		}).bind("consentWidgetDiv");
	}

	<%-- Amazon住所取得関数 --%>
	function getAmazonAddress(orderType, addressType, callback) {
		$.ajax({
			type: "POST",
			url: "<%=Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT%>/GetAmazonAddress",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				orderReferenceIdOrBillingAgreementId: $('#<%=(this.CartList.Items[0].HasFixedPurchase) ? this.WhfAmazonBillingAgreementId.ClientID : this.WhfAmazonOrderRefID.ClientID %>').val(),
				orderType: orderType,
				addressType: addressType
			}),
			success: callback
		});
	}

	<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
	// Set authentication message
	function setAuthenticationMessage() {
		var phoneNumber = document.getElementById('<%= this.WtbOwnerTel1.ClientID %>').value;

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
			'<%= this.WtbOwnerTel1_1.ClientID %>',
			'<%= this.WtbOwnerTel1_2.ClientID %>',
			'<%= this.WtbOwnerTel1_3.ClientID %>',
			'<%= this.WtbOwnerTel1.ClientID %>',
			'');
		return result;
	}
	<% } %>
</script>
<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<script type="text/javascript" charset="utf-8">
	amazon.Pay.bindChangeAction('#lbChangeAddress',
		{
			amazonCheckoutSessionId: '<%= this.AmazonCheckoutSession.CheckoutSessionId %>',
			changeAction: 'changeAddress'
		});
	amazon.Pay.bindChangeAction('#lbChangePayment',
		{
			amazonCheckoutSessionId: '<%= this.AmazonCheckoutSession.CheckoutSessionId %>',
			changeAction: 'changePayment'
		});
</script>
	<% } %>
<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
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
</asp:Content>
