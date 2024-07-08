<%--
=========================================================================================================
  Module      : スマートフォン用注文配送先入力画面(OrderShipping.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderShipping.aspx.cs" Inherits="Form_Order_OrderShipping" MaintainScrollPositionOnPostback="true" Title="配送先情報入力ページ" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="EcPayScript" Src="~/Form/Common/ECPay/EcPayScript.ascx" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
<style>
	.convenience-store-item {
		margin: 2px;
		padding: 4px;
	}

	.convenience-store-button {
		display: block;
		padding: 1em;
		background-color: #000;
		margin: .5em 0;
		width: 50%;
		text-align: center;
		color: #fff !important;
	}
</style>
<% } %>
<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
<uc:EcPayScript runat="server" ID="ucECPayScript" />
<% } %>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-order order-shipping">

<div class="step">
	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-step02.jpg" alt="注文者情報と配送先" width="320" />
</div>

<%-- 次へイベント用リンクボタン --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" runat="server"></asp:LinkButton>
<%-- 戻るイベント用リンクボタン --%>
<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server"></asp:LinkButton>

<%-- エラーメッセージ（デフォルト注文方法用） --%>
<span style="color:red;text-align:center;display:block"><asp:Literal ID="lOrderErrorMessage" runat="server"></asp:Literal></span>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<% this.CartItemIndexTmp = -1; %>

<asp:Repeater id="rCartList" Runat="server">
<ItemTemplate>

<div class="order-unit">

<%-- ▼注文者情報▼ --%>
<div class="owner" visible='<%# Container.ItemIndex == 0 %>' runat="server">
	<h2>注文者情報</h2>
	<%if (this.IsEasyUser) {%>
	<p style="margin:5px;padding:5px;text-align:center;background-color:#ffff80;border:1px solid #D4440D;border-color:#E5A500;color:#CC7600;">ご購入手続きに必要な会員情報が不足しています。</p>
	<%} %>
<dl class="order-form product">
	<%-- 注文商品 --%>
	<dt>注文商品</dt>
	<dd>
	<%-- ▼商品リスト▼ --%>
	<asp:Repeater id="rCart" DataSource="<%# ((CartObject)Container.DataItem).Items %>" Runat="server">
	<HeaderTemplate>
		<table class="cart-table">
		<tbody>
	</HeaderTemplate>
	<ItemTemplate>
		<tr class="<%# (((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1) ? "last" : "" %>">
		<td class="product-image">
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
			<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
		<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
		</td>
		<td class="product-info">
			<ul>
				<li class="product-name">
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
					<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
				</li>
				<li class="product-price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server">
					<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %>
				</li>
			</ul>
		</td>
		</tr>
	</ItemTemplate>
	<FooterTemplate>
		</tbody>
		</table>
	</FooterTemplate>
	</asp:Repeater>
	<%-- ▲商品リスト▲ --%>
	</dd>
</dl>
<%
	this.CartItemIndexTmp++;
	var ownerAddrCountryIsoCode = GetOwnerAddrCountryIsoCode(this.CartItemIndexTmp);
	var isOwnerAddrCountryJp = IsCountryJp(ownerAddrCountryIsoCode);
	var isOwnerAddrCountryUs = IsCountryUs(ownerAddrCountryIsoCode);
	var isOwnerAddrCountryTw = IsCountryTw(ownerAddrCountryIsoCode);
	var isOwnerAddrZipNecessary = IsAddrZipcodeNecessary(ownerAddrCountryIsoCode);
%>
	<dl class="order-form">
		<%-- 注文者：氏名 --%>
		<dt><%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span><span id="efo_sign_name"/></dt>
		<dd class="name">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerName1"
				runat="Server"
				ControlToValidate="tbOwnerName1"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			<asp:CustomValidator
				ID="cvOwnerName2"
				runat="Server"
				ControlToValidate="tbOwnerName2"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerName1" Text="<%# this.CartList.Owner.Name1 %>" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>'></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox ID="tbOwnerName2" Text="<%# this.CartList.Owner.Name2 %>" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>'></w2c:ExtendedTextBox>
		</dd>
		<%-- 注文者：氏名（かな） --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			<span class="require">※</span><span id="efo_sign_kana"/>
		</dt>
		<dd class="<%= ReplaceTag("@@User.name_kana.type@@") %>">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerNameKana1"
				runat="Server"
				ControlToValidate="tbOwnerNameKana1"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			<asp:CustomValidator
				ID="cvOwnerNameKana2"
				runat="Server"
				ControlToValidate="tbOwnerNameKana2"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerNameKana1" Text="<%# this.CartList.Owner.NameKana1 %>" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>'></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox ID="tbOwnerNameKana2" Text="<%# this.CartList.Owner.NameKana2 %>" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>'></w2c:ExtendedTextBox>
		</dd>
		<% } %>
		<%-- 注文者：生年月日 --%>
		<dt>
			<%: ReplaceTag("@@User.birth.name@@") %>
			<span class="require">※</span><span id="efo_sign_birth"/>
		</dt>
		<dd class="birth">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerBirth"
				runat="Server"
				ControlToValidate="ddlOwnerBirthDay"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false" />
			</p>
			<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Year.ToString() : "" %>' CssClass="year" runat="server"></asp:DropDownList>
			年 
			<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Month.ToString() : "" %>' CssClass="month" runat="server"></asp:DropDownList>
			月 
			<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Day.ToString() : "" %>' CssClass="date" runat="server"></asp:DropDownList>
			日
		</dd>
		<%-- 注文者：性別 --%>
		<dt>
			<%: ReplaceTag("@@User.sex.name@@") %>
			<span class="require">※</span><span id="efo_sign_sex"/>
		</dt>
		<dd class="sex">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerSex"
				runat="Server"
				ControlToValidate="rblOwnerSex"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false" />
			</p>
			<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%# GetCorrectSexForDataBind(this.CartList.Owner.Sex) %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" />
		</dd>
		<%-- 注文者：メールアドレス --%>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
			<span class="require">※</span><span id="efo_sign_mail_addr"/>
		</dt>
		<dd class="mail">
			<p class="msg">お手数ですが、確認のため２度入力してください。</p>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerMailAddrForCheck" 
				runat="Server"
				ControlToValidate="tbOwnerMailAddr"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				/>
			<asp:CustomValidator
				ID="cvOwnerMailAddr"
				runat="Server"
				ControlToValidate="tbOwnerMailAddr"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			<asp:CustomValidator
				ID="cvOwnerMailAddrConf"
				runat="Server"
				ControlToValidate="tbOwnerMailAddrConf"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerMailAddr" Text="<%# this.CartList.Owner.MailAddr %>" Type="email" MaxLength="256" runat="server" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox ID="tbOwnerMailAddrConf" Text="<%# this.CartList.Owner.MailAddr %>" Type="email" MaxLength="256" runat="server" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
		</dd>
		<%-- 注文者：メールアドレス2 --%>
		<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
		<dt>
			<%: ReplaceTag("@@User.mail_addr2.name@@") %>
		</dt>
		<dd class="mobile">
			<p class="msg">お手数ですが、確認のため２度入力してください。</p>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerMailAddr2"
				runat="Server"
				ControlToValidate="tbOwnerMailAddr2"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			<asp:CustomValidator
				ID="cvOwnerMailAddr2Conf"
				runat="Server"
				ControlToValidate="tbOwnerMailAddr2Conf"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerMailAddr2" Text="<%# this.CartList.Owner.MailAddr2 %>" Type="email" MaxLength="256" runat="server" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox ID="tbOwnerMailAddr2Conf" Text="<%# this.CartList.Owner.MailAddr2 %>" Type="email" MaxLength="256" runat="server" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
		</dd>
		<% } %>
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<%-- 注文者：国 --%>
		<dt>
			<%: ReplaceTag("@@User.country.name@@", ownerAddrCountryIsoCode) %>
			<span class="require">※</span>
		</dt>
		<dd>
			<asp:DropDownList id="ddlOwnerCountry" runat="server" AutoPostBack="true" SelectedValue="<%# this.CartList.Owner.AddrCountryIsoCode %>" DataSource="<%# this.UserCountryDisplayList %>" OnSelectedIndexChanged="ddlOwnerCountry_SelectedIndexChanged" DataTextField="Text" DataValueField="Value"/><br/>
			<asp:CustomValidator
				ID="cvOwnerCountry"
				runat="Server"
				ControlToValidate="ddlOwnerCountry"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false"
				CssClass="error_inline" />
			<span id="countryAlertMessage" class="msg" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
		</dd>
		<% } %>
		<%-- 注文者：郵便番号 --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@") %>
			<span class="require">※</span><span id="efo_sign_zip"/>
		</dt>
		<dd class="zip">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerZip1"
				runat="Server"
				ControlToValidate="tbOwnerZip"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			<span id="sOwnerZipError" runat="server" class="attention shortZipInputErrorMessage"></span>
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerZip" Type="tel" Text="<%#: this.CartList.Owner.Zip %>" MaxLength="8" runat="server" OnTextChanged="lbSearchOwnergAddr_Click" />
			<br />
			<asp:LinkButton ID="lbSearchOwnergAddr" runat="server" onclick="lbSearchOwnergAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">郵便番号から住所を入力</asp:LinkButton>
			<%--検索結果レイヤー--%>
			<uc:Layer ID="ucLayer" runat="server" />
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>
			<span class="require">※</span><% if (isOwnerAddrCountryJp) { %><span id="efo_sign_address"/><% } %>
		</dt>
		<dd class="address">
			<% if (isOwnerAddrCountryJp) { %>
			<%-- 注文者：都道府県 --%>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerAddr1"
				runat="Server"
				ControlToValidate="ddlOwnerAddr1"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<asp:DropDownList ID="ddlOwnerAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# this.CartList.Owner.Addr1 %>" runat="server"></asp:DropDownList>
			<% } %>
			<%-- 注文者：市区町村 --%>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged"></asp:DropDownList>
				<br />
			<% } else { %>
				<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerAddr2"
					runat="Server"
					ControlToValidate="tbOwnerAddr2"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerAddr2" placeholder='市区町村' Text="<%# this.CartList.Owner.Addr2 %>" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<% } %>
			<%-- 注文者：番地 --%>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
			<% } else { %>
				<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerAddr3"
					runat="Server"
					ControlToValidate="tbOwnerAddr3"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerAddr3" placeholder='番地' Text="<%# this.CartList.Owner.Addr3 %>" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server" ></w2c:ExtendedTextBox>
			<% } %>
			<%-- 注文者：ビル・マンション名 --%>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerAddr4"
				runat="Server"
				ControlToValidate="tbOwnerAddr4"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerAddr4" placeholder='建物名' Text="<%# this.CartList.Owner.Addr4 %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<%-- 注文者：州 --%>
			<% if (isOwnerAddrCountryJp == false) { %>
				<% if (isOwnerAddrCountryUs) { %>
			<asp:DropDownList ID="ddlOwnerAddr5" DataSource="<%# this.UserStateList %>" DataTextField="Text" DataValueField="Value" runat="server"></asp:DropDownList>
					<asp:CustomValidator
						ID="cvOwnerAddr5Ddl"
						runat="Server"
						ControlToValidate="ddlOwnerAddr5"
						ValidationGroup="OrderShippingGlobal"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false"
						CssClass="error_inline" />
			<% } else if (isOwnerAddrCountryTw) { %>
			<w2c:ExtendedTextBox ID="tbOwnerAddrTw" placeholder='省' Text="<%# this.CartList.Owner.Addr5 %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<% } else { %>
			<w2c:ExtendedTextBox ID="tbOwnerAddr5" placeholder='州' Text="<%# this.CartList.Owner.Addr5 %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<% } %>
			<% } %>
		</dd>
		<%-- 注文者：郵便番号（海外向け） --%>
		<% if (isOwnerAddrCountryJp == false) { %>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrZipNecessary) { %>&nbsp;<span class="require">※</span><% } %>
		</dt>
		<dd>
			<asp:TextBox ID="tbOwnerZipGlobal" Text="<%# this.CartList.Owner.Zip %>" MaxLength="20" runat="server" Type="tel"></asp:TextBox>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerZipGlobal"
				runat="Server"
				ControlToValidate="tbOwnerZipGlobal"
				ValidationGroup="OrderShippingGlobal"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				CssClass="error_inline" />
			</p>
			<asp:LinkButton
				ID="lbSearchAddrOwnerFromZipGlobal"
				OnClick="lbSearchAddrOwnerFromZipGlobal_Click"
				Style="display:none;"
				runat="server" />
		</dd>
		<% } %>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<%-- 注文者：企業名 --%>
			<dt><%: ReplaceTag("@@User.company_name.name@@")%></dt>
			<dd class="company-name">
				<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerCompanyName"
					runat="Server"
					ControlToValidate="tbOwnerCompanyName"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' Text="<%# this.CartList.Owner.CompanyName %>" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
			<%-- 注文者：部署名 --%>
			<dt><%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
			<dd class="company-post">
				<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerCompanyPostName"
					runat="Server"
					ControlToValidate="tbOwnerCompanyPostName"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"　/>
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' Text="<%# this.CartList.Owner.CompanyPostName %>" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
		<%} %>
		<%-- 注文者：電話番号1 --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
			<span class="require">※</span><span id="efo_sign_tel1"/>
		</dt>
		<dd class="tel">
			<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
				<!-- mobile phone if use GMO payment -->
				<p class="warning">
					<%#: WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE) %>
				</p>
			<% } %>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerTel1_1"
				runat="Server"
				ControlToValidate="tbOwnerTel1"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerTel1" Text="<%#: this.CartList.Owner.Tel1 %>" MaxLength="13" Type="tel" style="width:100%;" runat="server" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
			<asp:LinkButton
				ID="lbGetAuthenticationCode"
				CssClass="btn-add-get"
				runat="server"
				Text="認証コードの取得"
				OnClick="lbGetAuthenticationCode_Click"
				OnClientClick="return checkTelNoInput();" />
				<asp:Label ID="lbAuthenticationStatus" runat="server" />
			<% } %>
		</dd>
		<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
		</dt>
		<dd>
			<asp:TextBox
				ID="tbAuthenticationCode"
				Width="30%"
				Enabled="false"
				MaxLength='<%# GetMaxLength("@@User.authentication_code.length_max@@") %>'
				runat="server" />
			<span class="notes">
				<% if (this.HasAuthenticationCode) { %>
				<asp:Label ID="lbHasAuthentication" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
				<% } %>
				<span><%: GetVerificationCodeNote(ownerAddrCountryIsoCode) %></span>
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
		<%-- 注文者：電話番号2 --%>
		<dt>
			<%: ReplaceTag("@@User.tel2.name@@") %>
		</dt>
		<dd class="tel">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerTel2"
				runat="Server"
				ControlToValidate="tbOwnerTel2"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="False"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerTel2" Text="<%# this.CartList.Owner.Tel2 %>" MaxLength="13" Type="tel" style="width:100%;" runat="server" CssClass="shortTel" />
		</dd>
		<% } else { %>
		<%-- 注文者：電話番号1（海外向け） --%>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="require">※</span>
		</dt>
		<dd class="tel">
			<w2c:ExtendedTextBox ID="tbOwnerTel1Global" Text="<%# this.CartList.Owner.Tel1 %>" Width="100%" Type="tel" MaxLength="30" runat="server" onchange="resetAuthenticationCodeInput('cvOwnerTel1Global')" />
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
			<asp:LinkButton
				ID="lbGetAuthenticationCodeGlobal"
				class="btn-add-get"
				runat="server"
				Text="認証コードの取得"
				OnClick="lbGetAuthenticationCode_Click"
				OnClientClick="return checkTelNoInput();" />
			<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
			<% } %>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerTel1Global"
				runat="Server"
				ControlToValidate="tbOwnerTel1Global"
				ValidationGroup="OrderShippingGlobal"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
		</dd>
		<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
		</dt>
		<dd>
			<asp:TextBox
				ID="tbAuthenticationCodeGlobal"
				Width="30%"
				Enabled="false"
				MaxLength='<%# GetMaxLength("@@User.authentication_code.length_max@@") %>'
				runat="server" />
			<span class="notes">
				<% if (this.HasAuthenticationCode) { %>
				<asp:Label ID="lbHasAuthenticationGlobal" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
				<% } %>
				<span><%: GetVerificationCodeNote(ownerAddrCountryIsoCode) %></span>
				<asp:Label ID="lbAuthenticationMessageGlobal" runat="server" />
			</span>
			<br />
			<asp:CustomValidator
				ID="cvAuthenticationCodeGlobal"
				runat="Server"
				ControlToValidate="tbAuthenticationCodeGlobal"
				ValidationGroup="OrderShippingGlobal"
				ValidateEmptyText="false"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
		</dd>
		<% } %>
		<%-- 注文者：電話番号2（海外向け） --%>
		<dt>
		<%: ReplaceTag("@@User.tel2.name@@", ownerAddrCountryIsoCode) %>
		</dt>
		<dd>
			<w2c:ExtendedTextBox ID="tbOwnerTel2Global" Text="<%# this.CartList.Owner.Tel2 %>" Type="tel" MaxLength="30" runat="server"></w2c:ExtendedTextBox>
			<asp:CustomValidator
				ID="cvOwnerTel2Global"
				runat="Server"
				ControlToValidate="tbOwnerTel2Global"
				ValidationGroup="OrderShippingGlobal"
				ValidateEmptyText="False"
				SetFocusOnError="true" />
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.mail_flg.name@@") %>
		</dt>
		<dd>
			<asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" runat="server" />
		</dd>
	</dl>
<% this.CartItemIndexTmp = -1; %>
</div>
<%-- ▲注文者情報▲ --%>

<%if (Constants.GIFTORDER_OPTION_ENABLED == false) { %>
<%-- ▼配送先情報▼ --%>
<div class="shipping">
<%
	this.CartItemIndexTmp++;
%>
	<h2 Visible="<%# (((CartObject)Container.DataItem).IsDigitalContentsOnly == false) %>" runat="server"><%# this.CartList.Items.Count > 1 ? "カート番号" + (Container.ItemIndex + 1).ToString() + "の" : "" %>お届け先の住所</h2>
	
		<asp:HiddenField id="hcShowShippingInputForm" value="<%# CanInputShippingTo(Container.ItemIndex) %>" runat="server" />

		<div id="divShipToCart1Address" Visible="<%# CanInputShippingTo(Container.ItemIndex) && (Container.ItemIndex != 0) %>" runat="server" class="shipping-select">
			<asp:CheckBox id="cbShipToCart1Address" Text="カート番号1の配送先を使用" OnCheckedChanged="cbShipToCart1Address_OnCheckedChanged" AutoPostBack="true" Checked="<%# ((CartObject)Container.DataItem).Shippings[0].IsSameShippingAsCart1 %>" runat="server" />
			<span style="color:red;display:block;"><asp:Literal ID="lShipToCart1AddressInvalidMessage" runat="server" /></span>
		</div>

		<div id="divShippingInputForm" runat="server">

		<dl class="order-form">
			<dt>配送先</dt>
			<dd>
				<asp:DropDownList ID="ddlShippingKbnList" DataSource="<%# GetShippingKbnList(Container.ItemIndex) %>" DataTextField="text" DataValueField="value" SelectedValue="<%# ((CartObject)Container.DataItem).Shippings[0].ShippingAddrKbn %>" OnSelectedIndexChanged="ddlShippingKbnList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
				<span style="color:red;display:block;"><asp:Literal ID="lStorePickUpInvalidMessage" runat="server" /></span>
				<span style="color:red;display:block;"><asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal></span>
				<span id='<%# "spErrorConvenienceStore" + Container.ItemIndex.ToString() %>' style="color:red;display:block;"></span>

				<%-- ▽コンビニ受取り▽ --%>
				<div id="divShippingInputFormConvenience" class="<%# Container.ItemIndex %>" runat="server">
					<ul>
						<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
						<dd runat="server" id="ddShippingReceivingStoreType">
							<li>
								<asp:DropDownList
									ID="ddlShippingReceivingStoreType"
									DataSource="<%# ShippingReceivingStoreType() %>"
									DataTextField="text" DataValueField="value"
									DataMember="<%# Container.ItemIndex %>"
									AutoPostBack="true"
									runat="server"
									OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged" />
								<br />
							</li>
						</dd>
						<% } %>
						<dd style="color:red;display:block;"><asp:Literal ID="lConvenienceStoreErrorMessage" runat="server"></asp:Literal></dd>
						購入金額<%# CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE)%>以上、または<%# StringUtility.ToEmpty(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[0]) %>kg以上の商品は指定しないでください</br>
						<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
						コンビニがセブンイレブンの場合<%# StringUtility.ToEmpty(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[1]) %>kg以上です。
						<% } %>
						<div id="divButtonOpenConvenienceStoreMapPopup" runat="server">
							<li class="convenience-store-item" runat="server" Visible='<%# Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false %>'>
								<a href="javascript:openConvenienceStoreMapPopup(<%# Container.ItemIndex %>);" class="btn btn-success convenience-store-button">Family/OK/Hi-Life</a>
							</li>
							<li runat="server" Visible='<%# Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED %>'>
								<asp:LinkButton
									ID="lbOpenEcPay"
									runat="server"
									class="btn btn-success convenience-store-button"
									OnClick="lbOpenEcPay_Click"
									CommandArgument="<%# Container.ItemIndex %>"
									Text="  電子マップ  " />
							</li>
						</div></br>
						<dd id="ddCvsShopId">
							<li class="convenience-store-item" id="liCvsShopId">
								<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopId@@") %></strong><br />
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopId" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%#: GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>" />
							</li>
						</dd>
						<dd id="ddCvsShopName">
							<li class="convenience-store-item" id="liCvsShopName">
								<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopName@@") %></strong><br />
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopName" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" />
							</li>
						</dd>
						<dd id="ddCvsShopAddress">
							<li class="convenience-store-item" id="liCvsShopAddress">
								<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopAddress@@") %></strong><br />
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>" />
							</li>
						</dd>
						<dd id="ddCvsShopTel">
							<li class="convenience-store-item" id="liCvsShopTel">
								<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopTel@@") %></strong><br />
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" />
							</li>
						</dd>
							<li class="convenience-store-item">
								<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG) %>" />
							</li>
						<dt>＜コンビニ受取の際の注意事項＞</br>
							注文者情報は必ず正しい「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>」と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」を入力してください。（ショートメールが受け取れる電話番号を入力する必要があります）
							コンビニで商品を受け取る際、店舗ではお客様の「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>」と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」下3桁を確認します。
						</dt>
					</ul>
				</div>
				<%-- △コンビニ受取り△ --%>

				<%-- ▽配送先表示▽ --%>
				<div id="divShippingDisp" visible="<%# GetShipToOwner(((CartObject)Container.DataItem).Shippings[0]) %>" runat="server">
					<% var isShippingAddrCountryJp = IsCountryJp(this.CountryIsoCode); %>
					<dl class="order-form">
					<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
					<dd>
						<asp:Literal ID="lShippingName1" runat="server"></asp:Literal><asp:Literal ID="lShippingName2" runat="server"></asp:Literal>&nbsp;様
						<%if (isShippingAddrCountryJp) {%>（<asp:Literal ID="lShippingNameKana1" runat="server"></asp:Literal><asp:Literal ID="lShippingNameKana2" runat="server"></asp:Literal>&nbsp;さま）<%} %>
					</dd>
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>
					</dt>
					<dd>
						<%if (isShippingAddrCountryJp) {%>〒<asp:Literal ID="lShippingZip" runat="server"></asp:Literal><br /><%} %>
						<asp:Literal ID="lShippingAddr1" runat="server"></asp:Literal> <asp:Literal ID="lShippingAddr2" runat="server"></asp:Literal><br />
						<asp:Literal ID="lShippingAddr3" runat="server"></asp:Literal> <asp:Literal ID="lShippingAddr4" runat="server"></asp:Literal> 
						<asp:Literal ID="lShippingAddr5" runat="server"></asp:Literal><br />
						<%if (isShippingAddrCountryJp == false) {%><asp:Literal ID="lShippingZipGlobal" runat="server"></asp:Literal><br /><%} %>
						<asp:Literal ID="lShippingCountryName" runat="server"></asp:Literal>
					</dd>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
					<dt><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
					<dd>
						<asp:Literal ID="lShippingCompanyName" runat="server"></asp:Literal>&nbsp<asp:Literal ID="lShippingCompanyPostName" runat="server"></asp:Literal>
					</dd>
					<%} %>
					<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
					<dd>
						<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
					</dd>
					</dl>
				</div>
				<%-- △配送先表示△ --%>

				<%-- ▽配送先入力フォーム▽ --%>
				<div id="divShippingInputFormInner" class="shipping-input" visible="<%# GetShipToOwner(((CartObject)Container.DataItem).Shippings[0]) == false %>" runat="server">
				<div id="divShippingVisibleConvenienceStore" runat="server">
				<%
					var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartItemIndexTmp);
					var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
					var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
					var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
					var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
				%>
				<dl class="order-form">
					<%-- 配送先：氏名 --%>
					<dt><%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span><span id="<%# "efo_sign_ship_name" + Container.ItemIndex %>"/></dt>
					<dd class="name">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingName1"
							runat="Server"
							ControlToValidate="tbShippingName1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						<asp:CustomValidator
							ID="cvShippingName2"
							runat="Server"
							ControlToValidate="tbShippingName2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingName1" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<w2c:ExtendedTextBox ID="tbShippingName2" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%-- 配送先：氏名（かな） --%>
					<% if (isShippingAddrCountryJp) { %>
					<dt>
						<%: ReplaceTag("@@User.name_kana.name@@") %>
						<span class="require">※</span><span id="<%# "efo_sign_ship_kana" + Container.ItemIndex %>"/>
					</dt>
					<dd class="name-kana">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingNameKana1"
							runat="Server"
							ControlToValidate="tbShippingNameKana1"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ValidationGroup="OrderShipping" />
						<asp:CustomValidator
							ID="cvShippingNameKana2"
							runat="Server"
							ControlToValidate="tbShippingNameKana2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingNameKana1" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) %>" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<w2c:ExtendedTextBox ID="tbShippingNameKana2" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) %>" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<% } %>
					<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
					<dt>
						<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
						<span class="require">※</span>
					</dt>
					<dd>
						<asp:DropDownList id="ddlShippingCountry" runat="server" AutoPostBack="true" DataSource="<%# this.ShippingAvailableCountryDisplayList %>" OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
						<asp:CustomValidator
							ID="cvShippingCountry"
							runat="Server"
							ControlToValidate="ddlShippingCountry"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
					</dd>
					<% } %>
					<%-- 配送先：郵便番号 --%>
					<% if (isShippingAddrCountryJp) { %>
					<dt>
						<%: ReplaceTag("@@User.zip.name@@") %>
						<span class="require">※</span><span id="<%# "efo_sign_ship_zip" + Container.ItemIndex %>"/>
					</dt>
					<dd class="zip">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingZip1"
							runat="Server"
							ControlToValidate="tbShippingZip"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"/>
						<span id="sShippingZipError" runat="server" class="shortZipInputErrorMessage"></span>
						</p>
						<w2c:ExtendedTextBox ID="tbShippingZip" Type="tel" Text="<%#: GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" OnTextChanged="lbSearchShippingAddr_Click" MaxLength="8" runat="server" />
						<br />
						<asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">
						郵便番号から住所を入力</asp:LinkButton>
					</dd>
					<% } %>
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>
						<span class="require">※</span><% if (isShippingAddrCountryJp) { %><span id="<%# "efo_sign_ship_address" + Container.ItemIndex %>"/><% } %>
					</dt>
					<dd class="address">
						<% if (isShippingAddrCountryJp) { %>
							<%-- 配送先：都道府県 --%>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingAddr1"
							runat="Server"
							ControlToValidate="ddlShippingAddr1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<asp:DropDownList ID="ddlShippingAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1) %>" runat="server"></asp:DropDownList>
						<% } %>
						<%-- 配送先：市区町村 --%>
						<% if (isShippingAddrCountryTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
							<br />
						<% } else { %>
							<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr2"
								runat="Server"
								ControlToValidate="tbShippingAddr2"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true" />
							</p>
							<w2c:ExtendedTextBox ID="tbShippingAddr2" placeholder='市区町村' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2) %>" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<%-- 配送先：番地 --%>
						<% if (isShippingAddrCountryTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField="Key" DataValueField="Value" Width="95" ></asp:DropDownList>
						<% } else { %>
							<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr3"
								runat="Server"
								ControlToValidate="tbShippingAddr3"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true" />
							</p>
							<w2c:ExtendedTextBox ID="tbShippingAddr3" placeholder='番地' Text='<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3) %>' MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<%-- 配送先：ビル・マンション名 --%>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingAddr4"
							runat="Server"
							ControlToValidate="tbShippingAddr4"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingAddr4" placeholder='建物名' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4) %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% if (isShippingAddrCountryJp == false) { %>
						<%-- 配送先：州 --%>
							<% if (isShippingAddrCountryUs) { %>
						<asp:DropDownList ID="ddlShippingAddr5" DataSource="<%# this.UserStateList %>" DataTextField="Text" DataValueField="Value" runat="server"></asp:DropDownList>
								<asp:CustomValidator
									ID="cvShippingAddr5Ddl"
									runat="Server"
									ControlToValidate="ddlShippingAddr5"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									EnableClientScript="false"
									CssClass="error_inline" />
						<% } else if (isShippingAddrCountryTw) { %>
						<w2c:ExtendedTextBox ID="tbShippingAddrTw" placeholder='省' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5) %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } else { %>
						<w2c:ExtendedTextBox ID="tbShippingAddr5" placeholder='州' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5) %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<% } %>
					</dd>
					<%-- 配送先：郵便番号（海外向け） --%>
					<% if (isShippingAddrCountryJp == false) { %>
					<dt>
						<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
						<% if (isShippingAddrZipNecessary) { %>&nbsp;<span class="require">※</span><% } %>
					</dt>
					<dd>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingZipGlobal"
							runat="Server"
							ControlToValidate="tbShippingZipGlobal"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<asp:TextBox ID="tbShippingZipGlobal" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" MaxLength="20" runat="server" Type="tel"></asp:TextBox>
						<asp:LinkButton
							ID="lbSearchAddrShippingFromZipGlobal"
							OnClick="lbSearchAddrShippingFromZipGlobal_Click"
							Style="display:none;"
							runat="server" />
					</dd>
					<% } %>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
					<%-- 配送先：企業名 --%>
					<dt><%: ReplaceTag("@@User.company_name.name@@")%></dt>
					<dd class="company-name">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingCompanyName"
							runat="Server"
							ControlToValidate="tbShippingCompanyName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME) %>" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%-- 配送先：部署名 --%>
					<dt><%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
					<dd class="company-post">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingCompanyPostName"
							runat="Server"
							ControlToValidate="tbShippingCompanyPostName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%} %>
					<%-- 配送先：電話番号 --%>
					<% if (isShippingAddrCountryJp) { %>
					<dt>
						<%: ReplaceTag("@@User.tel1.name@@") %>
						<span class="require">※</span><span id="<%# "efo_sign_ship_tel1" + Container.ItemIndex %>"/>
					</dt>
					<dd class="tel">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingTel1_1"
							runat="Server"
							ControlToValidate="tbShippingTel1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingTel1" Text="<%#: GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" MaxLength="13" Type="tel" style="width:100%;" runat="server" CssClass="shortTel" />
					</dd>
					<% } else { %>
					<%-- 注文者：電話番号2（海外向け） --%>
					<dt>
						<%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %>
						<span class="require">※</span>
					</dt>
					<dd class="tel">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingTel1Global"
							runat="Server"
							ControlToValidate="tbShippingTel1Global"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingTel1Global" Type="tel" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" MaxLength="30" runat="server"></w2c:ExtendedTextBox>
					</dd>
					<% } %>
					</div>
					<dd id="divSaveToUserShipping" visible="<%# this.IsLoggedIn %>" runat="server">
					<%-- ポストバックCustomValidatorの状態がクリアされてしまうため、JavaScirptで表示非表示を制御する --%>
					<p>
						<asp:RadioButtonList ID="rblSaveToUserShipping" OnSelectedIndexChanged="rblSaveToUserShipping_OnSelectedIndexChanged" AutoPostBack="true" SelectedValue='<%# ((CartObject)Container.DataItem).Shippings[0].UserShippingRegistFlg ? "1" : "0" %>' RepeatLayout="Flow" CssClass="radioBtn" runat="server">
						<asp:ListItem Text="配送先情報を保存しない" Value="0"></asp:ListItem>
						<asp:ListItem Text="配送先情報を保存する" Value="1"></asp:ListItem>
						</asp:RadioButtonList>
					</p>

					<dd id="dlUserShippingName" visible="false" runat="server"><span id="efo_sign_ship_addr_name"/>
						<p>配送先名をご入力ください</p>
						<p class="attention">
						<asp:CustomValidator
							ID="cvUserShippingName"
							runat="Server"
							ControlToValidate="tbUserShippingName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbUserShippingName" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UserShippingName %>" MaxLength="15" runat="server"></w2c:ExtendedTextBox>
					</dd>
					</dd>
				</div>
				<%-- △配送先入力フォーム△ --%>
			</dd>
		</dl>

		</div>

	<span id="sInvoices" runat="server" visible="false">
		<dl class="order-form shipping-require">
			<div id="divUniformInvoiceType" runat="server">
				<dt>発票種類</dt>
				<dd>
					<asp:DropDownList ID="ddlUniformInvoiceType" runat="server"
						CssClass="input_border"
						DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE) %>"
						DataTextField="text"
						DataValueField="value"
						OnSelectedIndexChanged="ddlUniformInvoiceType_SelectedIndexChanged"
						AutoPostBack="true">
					</asp:DropDownList>
					<asp:DropDownList ID="ddlUniformInvoiceTypeOption" runat="server"
						CssClass="input_border"
						DataTextField="text"
						DataValueField="value"
						OnSelectedIndexChanged="ddlUniformInvoiceTypeOption_SelectedIndexChanged"
						AutoPostBack="true"
						Visible="false">
					</asp:DropDownList>
				</dd>
				<dl id="dlUniformInvoiceOption1_8" runat="server" visible="false">
					<dd>統一編号</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="8"/>
						<p class="attention">
						<asp:CustomValidator
							ID="cvUniformInvoiceOption1_8" runat="server"
							ControlToValidate="tbUniformInvoiceOption1_8"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<asp:Label ID="lbUniformInvoiceOption1_8" runat="server" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" Visible="false"></asp:Label>
					</dd>
					<dd>会社名</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption2 %>" Width="220" runat="server" MaxLength="20"/>
						<p class="attention">
						<asp:CustomValidator
							ID="cvUniformInvoiceOption2" runat="server"
							ControlToValidate="tbUniformInvoiceOption2"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<asp:Label ID="lbtbUniformInvoiceOption2" runat="server" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption2 %>" Visible="false"></asp:Label>
					</dd>
				</dl>
				<dl id="dlUniformInvoiceOption1_3" runat="server" visible="false">
					<dd>寄付先コード</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption1_3" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="7" />
						<p class="attention">
						<asp:CustomValidator
							ID="cvUniformInvoiceOption1_3" runat="server"
							ControlToValidate="tbUniformInvoiceOption1_3"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<asp:Label ID="lbUniformInvoiceOption1_3" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" runat="server" Visible="false"></asp:Label>
					</dd>
				</dl>
				<dl id="dlUniformInvoiceTypeRegist" runat="server" visible="false">
					<dd>
						<asp:CheckBox ID="cbSaveToUserInvoice" Checked="<%# ((CartObject)Container.DataItem).Shippings[0].UserInvoiceRegistFlg %>" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
					</dd>
					<dd id="dlUniformInvoiceTypeRegistInput" runat="server" visible="false">
						電子発票情報名 <span class="require">※</span><br />
						<asp:TextBox ID="tbUniformInvoiceTypeName" MaxLength="30" runat="server"></asp:TextBox>
						<p class="attention">
						<asp:CustomValidator
							ID="cvUniformInvoiceTypeName" runat="server"
							ControlToValidate="tbUniformInvoiceTypeName"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
					</dd>
				</dl>
			</div>
			<div id="divInvoiceCarryType" runat="server">
				<dt>共通性載具</dt>
				<dd>
					<asp:DropDownList ID="ddlInvoiceCarryType" runat="server"
						CssClass="input_border"
						DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE) %>"
						DataTextField="text"
						DataValueField="value"
						OnSelectedIndexChanged="ddlInvoiceCarryType_SelectedIndexChanged"
						AutoPostBack="true"></asp:DropDownList>
					<asp:DropDownList ID="ddlInvoiceCarryTypeOption" runat="server"
						CssClass="input_border"
						DataTextField="text"
						DataValueField="value"
						OnSelectedIndexChanged="ddlInvoiceCarryTypeOption_SelectedIndexChanged"
						AutoPostBack="true"
						Visible="false">
					</asp:DropDownList>
				</dd>
				<dd id="divCarryTypeOption" runat ="server">
					<div id="divCarryTypeOption_8" runat="server" visible="false">
						<asp:TextBox ID="tbCarryTypeOption_8" Width="220" runat="server" Text="<%# ((CartObject)Container.DataItem).Shippings[0].CarryTypeOptionValue %>" placeholder="例:/AB201+9(限8個字)" MaxLength="8" />
						<p class="attention">
						<asp:CustomValidator
							ID="cvCarryTypeOption_8" runat="server"
							ControlToValidate="tbCarryTypeOption_8"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
					</div>
					<div id="divCarryTypeOption_16" runat="server" visible="false">
						<asp:TextBox ID="tbCarryTypeOption_16" Width="220" Text="<%# ((CartObject)Container.DataItem).Shippings[0].CarryTypeOptionValue %>" runat="server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
						<p class="attention">
						<asp:CustomValidator
							ID="cvCarryTypeOption_16" runat="server"
							ControlToValidate="tbCarryTypeOption_16"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
					</div>
				</dd>
				<dl id="dlCarryTypeOptionRegist" runat="server" visible="false">
					<dd>
						<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
					</dd>
					<dd id="divCarryTypeOptionName" runat="server" visible="false">
						電子発票情報名 <span class="require">※</span>
						<asp:TextBox ID="tbCarryTypeOptionName" Text="<%# ((CartObject)Container.DataItem).Shippings[0].InvoiceName %>" runat="server" MaxLength="30"></asp:TextBox>
						<p class="attention">
						<asp:CustomValidator
							ID="cvCarryTypeOptionName" runat="server"
							ControlToValidate="tbCarryTypeOptionName"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
					</dd>
				</dl>
				<dd>
					<asp:Label runat="server" ID="lbCarryTypeOption" Visible="false"></asp:Label>
				</dd>
			</div>
		</dl>
	</span>
		<dl class="order-form shipping-require">
			<dt visible="<%# CanInputShippingTo(Container.ItemIndex) %>" runat="server">配送指定</dt>
			<div id="dvStorePickUp" visible="false" runat="server">
				<div runat="server" class="userList">
					受け取り店舗を選択してください。
					<dl runat="server">
						<%-- 地域での絞り込みドロップダウン --%>
						<dt runat="server">地域：</dt>
						<dd>
							<asp:DropDownList
								ID="ddlRealShopArea"
								DataSource="<%# this.RealShopAreaList %>"
								OnSelectedIndexChanged="ddlRealShopNarrowDown_OnSelectedIndexChanged"
								DataTextField="AreaName"
								DataValueField="AreaId"
								AutoPostBack="true"
								OnDataBound="ddlRealShopArea_DataBound"
								runat="server" />
						</dd>
						<%-- 都道府県での絞り込みドロップダウン --%>
						<dt runat="server">都道府県：</dt>
						<dd>
							<asp:DropDownList
								ID="ddlRealShopAddr1List"
								DataSource="<%# this.Addr1List %>"
								OnSelectedIndexChanged="ddlRealShopNarrowDown_OnSelectedIndexChanged"
								DataTextField="Text"
								DataValueField="Value"
								AutoPostBack="true"
								runat="server" />
						</dd>
					</dl>
					<dl id="dlRealShopName" runat="server">
						<dt runat="server">店舗名：</dt>
						<dd>
							<asp:DropDownList
								ID="ddlRealShopName"
								DataSource="<%# this.RealShopNameList %>"
								OnSelectedIndexChanged="ddlRealShopNameList_OnSelectedIndexChanged"
								DataTextField="Name"
								DataValueField="RealShopId"
								AutoPostBack="true"
								Width="150"
								runat="server">
							</asp:DropDownList>
							<asp:CustomValidator
								ID="cvRealShopName"
								runat="Server"
								ControlToValidate="ddlRealShopName"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false"
								CssClass="error_inline" />
						</dd>
					</dl>
					<dl id="dlRealShopAddress" runat="server" visible="false">
						<dt runat="server">住所：</dt>
						<dd>
							<p>
								〒<asp:Literal ID="lRealShopZip" runat="server" />
								<br />
								<asp:Literal ID="lRealShopAddr1" runat="server" />
								<asp:Literal ID="lRealShopAddr2" runat="server" />
								<br />
								<asp:Literal ID="lRealShopAddr3" runat="server" />
								<br />
								<asp:Literal ID="lRealShopAddr4" runat="server" />
								<br />
								<asp:Literal ID="lRealShopAddr5" runat="server" />
							</p>
						</dd>
					</dl>
					<dl id="dlRealShopOpenningHours" runat="server" visible="false">
						<dt runat="server">営業時間：</dt>
						<dd>
							<asp:Literal ID="lRealShopOpenningHours" runat="server" />
						</dd>
					</dl>
					<dl id="dlRealShopTel" runat="server" visible="false">
						<dt runat="server">電話番号：</dt>
						<dd>
							<asp:Literal ID="lRealShopTel" runat="server" />
						</dd>
					</dl>
				</div>
			</div>
			<dd id="dvShippingMethod" visible="<%# CanInputShippingTo(Container.ItemIndex) && IsShippingStorePickup(Container.ItemIndex) == false %>" runat="server" class="userList">
				配送方法を選択して下さい。
				<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[Container.ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" runat="server"></asp:DropDownList>
			</dd>
			<div id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(Container.ItemIndex) && CanDisplayDeliveryCompany(Container.ItemIndex)) %>" runat="server" class="userList">
				<dd>配送サービスを選択して下さい。
					<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(Container.ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" runat="server"/>
				</dd>
			</div>
			<dd id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(Container.ItemIndex) %>" runat="server">
				<dl id="dlShipppingDateTime">
					<dt id="dtShippingDate" visible="<%# CanInputDateSet(Container.ItemIndex) %>" runat="server">配送希望日</dt>
					<dd id="ddShippingDate" visible="<%# CanInputDateSet(Container.ItemIndex) %>" runat="server">
						<asp:DropDownList id="ddlShippingDate" CssClass="input_border" runat="server" DataTextField="text" DataValueField="value" OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnCheckedChanged" AutoPostBack="true"></asp:DropDownList>
						<br />
						<asp:Label ID="lShippingDateErrorMessage" CssClass="attention" Visible="false" runat="server"></asp:Label>
					</dd>
					<div id="divShippingTime" runat="server">
						<dt id="dtShippingTime" visible="<%# CanInputTimeSet(Container.ItemIndex) %>" runat="server">配送希望時間帯</dt>
						<dd id="ddShippingTime" visible="<%# CanInputTimeSet(Container.ItemIndex) %>" runat="server">
							<asp:DropDownList id="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(Container.ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingTime(Container.ItemIndex) %>"></asp:DropDownList>
						</dd>
					</div>
				</dl>
			</dd>
		</dl>

		<dl class="order-form memo">
		<%-- 注文メモ --%>
		<dt>注文メモ</dt>
		<dd>
		<asp:Repeater ID="rMemos" runat="server" DataSource="<%# ((CartObject)Container.DataItem).OrderMemos %>" Visible="<%# ((CartObject)Container.DataItem).OrderMemos.Count != 0 %>">
		<HeaderTemplate>
			<dl>
		</HeaderTemplate>
		<ItemTemplate>
			<dt><%# WebSanitizer.HtmlEncode(Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME)) %></dt>
			<dd>
				<p class="attention"><span id="sErrorMessageMemo" runat="server"></span></p>
				<w2c:ExtendedTextBox ID="tbMemo"  runat="server" Text="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine"></w2c:ExtendedTextBox>
				<div>
				<%-- IDに"OtherValidator"を含めることで案件毎に追加したtextareaなどでチェック可能 --%>
				<asp:CustomValidator ID="OtherValidator" runat="Server"
					ControlToValidate="tbMemo"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
				</div>
			</dd>
		</ItemTemplate>
		<FooterTemplate>
			</dl>
		</FooterTemplate>
		</asp:Repeater>
		</dd>
		<asp:CheckBox ID="cbOnlyReflectMemoToFirstOrder"
			Checked="<%# ((CartObject)Container.DataItem).ReflectMemoToFixedPurchase %>"
			visible="<%# ((CartObject)Container.DataItem).OrderMemos.Count != 0 && ((CartObject)Container.DataItem).ReflectMemoToFixedPurchaseVisible %>"
			Text="2回目以降の注文メモにも追加する"
			CssClass="checkBox"
			runat="server" />
		</dl>
		<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server" Visible="<%# IsDisplayOrderExtend() %>" >
			<HeaderTemplate>
				<dl class="order-form">
					<dt>注文確認事項</dt>
					<dd>
						<dl>
			</HeaderTemplate>
			<ItemTemplate>
				<%-- 項目名 --%>
				<dt>
					<%#: Item.SettingModel.SettingName %>
					<span class="require"  runat="server" visible="<%# Item.SettingModel.IsNeecessary%>">※</span>
				</dt>
				<dd>
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
					<small><p class="attention"><asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label></p></small>
				</dd>
				<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
				<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
			</ItemTemplate>
			<FooterTemplate>
						</dl>
					</dd>
				</dl>
			</FooterTemplate>
		</asp:Repeater>
		<%-- ▼定期購入配送パターン▼ --%>
		<div class="fixed" visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server"><span id="efo_sign_fixed_purchase"/>
		<h2 visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server">定期購入 配送パターンの指定</h2>
		<div id='<%# "efo_sign_fixed_purchase" + Container.ItemIndex %>'></div>
		<%-- ▽デフォルトチェックの設定▽--%>
		<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
		<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 2, 3, 1, 4) %>
		<%-- △ - - - - - - - - - - - △--%>
		<dl class="order-form" visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server">
			<dd id="Div4" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
					Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 1) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_month" + Container.ItemIndex %>' />
				<div id="ddFixedPurchaseMonthlyPurchase_Date" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">
				<asp:DropDownList ID="ddlFixedPurchaseMonth"
					DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
					ヶ月ごと
				<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
					DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true,false,true) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
					日に届ける
				</div>
				<p class="attention">
				<asp:CustomValidator
					ID="cvFixedPurchaseMonth"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseMonth" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvFixedPurchaseMonthlyDate"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseMonthlyDate" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				</p>
			</dd>
			<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
					Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 2) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_week_and_day" + Container.ItemIndex %>' />
				<div id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">
				<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
					DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true, true) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server" />
				ヶ月ごと
				<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
					DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
				<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
					DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
					に届ける
				</div>
				<p class="attention">
				<asp:CustomValidator
					ID="cvFixedPurchaseIntervalMonths"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseIntervalMonths"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvFixedPurchaseWeekOfMonth"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseWeekOfMonth" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvFixedPurchaseDayOfWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseDayOfWeek" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				</p>
			</dd>
			<dd id="Div6" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
					Text="配送日間隔指定" Checked="<%# (GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 3) && (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? (GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false).Length > 0) : (GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false).Length > 1))) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_interval_days" + Container.ItemIndex %>' />
				<div id="ddFixedPurchaseRegularPurchase_IntervalDays" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
						DataSource='<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false) %>'
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
					</asp:DropDownList>
						日ごとに届ける
				</div>
				<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%# this.ShopShippingList[Container.ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
				<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%# this.ShopShippingList[Container.ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
				<p class="attention">
				<asp:CustomValidator
					ID="cvFixedPurchaseIntervalDays"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseIntervalDays" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				</p>
			</dd>
			<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
					Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 4) %>"
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id="<%# "efo_sign_fixed_purchase_week" + Container.ItemIndex %>" />
				<div id="ddFixedPurchaseEveryNWeek" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">　
					<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
						DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					週間ごと
					<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
						DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, false) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					に届ける
				</div>
				<p class="attention">
				<asp:CustomValidator
					ID="cvFixedPurchaseEveryNWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"/>
				<asp:CustomValidator
					ID="cvFixedPurchaseEveryNWeekDayOfWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"/>
				</p>
			</dd>
		</dl>
		<small><p class="attention" runat="server" visible="<%# GetAllFixedPurchaseKbnEnabled(Container.ItemIndex) == false %>">同時に定期購入できない商品が含まれております。</p></small>
		<dl class="order-form">
			<dt id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</dt>
			<dd visible="true" runat="server">
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
			<dd visible="true" runat="server">
				<asp:Label ID="lblNextShippingDate" visible="false" runat="server"></asp:Label>&nbsp;
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
		<p class="attention"><span ID="sErrorMessage" runat="server"></span></p>
		</div>
		<%-- ▲定期購入配送パターン▲ --%>
	</div>
</div>
<% } %>
</ItemTemplate>
</asp:Repeater>
		
<div class="cart-footer">
	<div class="button-next">
		<a onclick="<%= this.NextOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>" class="btn"><%: (this.IsNextConfirmPage) ? "ご注文内容確認へ" : "お支払方法入力へ" %></a>
	</div>
	<div class="button-prev">
		<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>" class="btn">戻る</a>
	</div>
</div>
<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
<% } %>
</ContentTemplate>
</asp:UpdatePanel>
</section>

<script type="text/javascript">
<!--
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
		<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
		bindTwAddressSearch();
		<% } %>
		<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
		HandleVisibility();
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
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			execAutoKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbOwnerName1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerName2")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana2")).ClientID %>'));
			execAutoKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingName1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingName2")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
		<%} %>
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			execAutoChangeKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana2")).ClientID %>'));
			execAutoChangeKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
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
			clickSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").UniqueID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sOwnerZipError")).ClientID %>',
				"owner",
				'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
				'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>');

			// Check owner zip code input on text box change
			textboxChangeSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").UniqueID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").UniqueID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").ClientID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sOwnerZipError")).ClientID %>',
				"owner");

			// Check shipping zip code input on click
			clickSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").UniqueID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sShippingZipError")).ClientID %>',
				"shipping",
				'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
				'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>');

			// Check shipping zip code input on text box change
			textboxChangeSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").UniqueID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").UniqueID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").ClientID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sShippingZipError")).ClientID %>',
				"shipping");

			if (multiAddrsearchTriggerType == "shipping")
			{
				bindTargetForAddr1 = "<%= ((DropDownList)ri.FindControl("ddlShippingAddr1")).ClientID %>";
				bindTargetForAddr2 = "<%= ((TextBox)ri.FindControl("tbShippingAddr2")).ClientID %>";
				bindTargetForAddr3 = "<%= ((TextBox)ri.FindControl("tbShippingAddr3")).ClientID %>";
			}

			<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
			// Textbox change search owner zip global
			textboxChangeSearchGlobalZip(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZipGlobal").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrOwnerFromZipGlobal").UniqueID %>');

			// Textbox change search shipping zip global
			textboxChangeSearchGlobalZip(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZipGlobal").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrShippingFromZipGlobal").UniqueID %>');
			<% } %>
		<% } %>
	}

	$(document).on('click', '.search-result-layer-close', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function () {
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
		} else if (multiAddrsearchTriggerType == "shipping") {
			<% foreach (RepeaterItem ri in rCartList.Items) { %>
				$('#' + bindTargetForAddr1).val(selectedAddr.find('.addr').text());
				$('#' + bindTargetForAddr2).val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
				$('#' + bindTargetForAddr3).focus();
			<%} %>
		}

		closePopupAndLayer();
	}

	<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	var selectedCartIndex = 0;

	function HandleVisibility() {
		<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
				var cartCount = <%= this.CartList.Items.Count %>;
				for (var i = 0; i < cartCount ; i++) {
					var elements = document.getElementsByClassName(i)[0];
					if(typeof elements != 'undefined')
					{
						if((elements.querySelector('[id$="hfCvsShopId"]') != null)
							&& (elements.querySelector('[id$="hfCvsShopId"]').value == ''))
						{
							$(elements.querySelector('[id$="ddCvsShopId"]')).hide();
							$(elements.querySelector('[id$="ddCvsShopName"]')).hide();
							$(elements.querySelector('[id$="ddCvsShopAddress"]')).hide();
							$(elements.querySelector('[id$="ddCvsShopTel"]')).hide();
						}
						else
						{
							$(elements.querySelector('[id$="ddCvsShopId"]')).show();
							$(elements.querySelector('[id$="ddCvsShopName"]')).show();
							$(elements.querySelector('[id$="ddCvsShopAddress"]')).show();
							$(elements.querySelector('[id$="ddCvsShopTel"]')).show();
						}
					}
				}
		<% } %>
	}

	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup(cartIndex) {
		selectedCartIndex = cartIndex;

		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl(true) %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var elements = document.getElementsByClassName(selectedCartIndex)[0];

		// For display
		elements.querySelector('[id$="liCvsShopId"] > span').innerHTML = cvsspot;
		elements.querySelector('[id$="liCvsShopName"] > span').innerHTML = name;
		elements.querySelector('[id$="liCvsShopAddress"] > span').innerHTML = addr;
		elements.querySelector('[id$="liCvsShopTel"] > span').innerHTML = tel;

		// For get value
		elements.querySelector('[id$="hfCvsShopId"]').value = cvsspot;
		elements.querySelector('[id$="hfCvsShopName"]').value = name;
		elements.querySelector('[id$="hfCvsShopAddress"]').value = addr;
		elements.querySelector('[id$="hfCvsShopTel"]').value = tel;

		elements.querySelector('[id$="ddCvsShopId"]').style.removeProperty('display');
		elements.querySelector('[id$="ddCvsShopName"]').style.removeProperty('display');
		elements.querySelector('[id$="ddCvsShopAddress"]').style.removeProperty('display');
		elements.querySelector('[id$="ddCvsShopTel"]').style.removeProperty('display');

		var element = document.getElementById('spErrorConvenienceStore' + selectedCartIndex);
		element.innerHTML = '';
	}

	<%-- Check Before Next Page --%>
	function CheckBeforeNextPage() {
		var hasError = false;
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		var shippingKbn = $('#<%= ((DropDownList)ri.FindControl("ddlShippingKbnList")).ClientID %>').val();
		if (shippingKbn == '<%= CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE %>') {
			var shopId = $('#<%= ((HiddenField)ri.FindControl("hfCvsShopId")).ClientID %>').val();

			var element = document.getElementById('spErrorConvenienceStore' + '<%= ri.ItemIndex %>');
			if (shopId == '') {
				element.innerHTML = 'コンビニが選択されていません';

				hasError = true;
			}
			else {
				element.innerHTML = '';
			}
		}

		<%} %>

		return (hasError == false);
	}
	<% } %>

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		<% foreach (RepeaterItem item in rCartList.Items) { %>
			<% if (((DropDownList)item.FindControl("ddlShippingAddr3") != null) && ((TextBox)item.FindControl("tbShippingZipGlobal") != null)) { %>
			$('#<%= ((DropDownList)item.FindControl("ddlShippingAddr3")).ClientID %>').change(function (e) {
				$('#<%= ((TextBox)item.FindControl("tbShippingZipGlobal")).ClientID %>').val(
					$('#<%= ((DropDownList)item.FindControl("ddlShippingAddr3")).ClientID %>').val().split('|')[0]);
			});
			<% } %>
			<% if (((DropDownList)item.FindControl("ddlOwnerAddr3") != null) && ((TextBox)item.FindControl("tbOwnerZipGlobal") != null)) { %>
			$('#<%= ((DropDownList)item.FindControl("ddlOwnerAddr3")).ClientID %>').change(function (e) {
				$('#<%= ((TextBox)item.FindControl("tbOwnerZipGlobal")).ClientID %>').val(
					$('#<%= ((DropDownList)item.FindControl("ddlOwnerAddr3")).ClientID %>').val().split('|')[0]);
			});
			<% } %>
		<% } %>
	}
	<% } %>

	<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
	// Set authentication message
	function setAuthenticationMessage() {
		var isOwnerAddrCountryJp = true;
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		isOwnerAddrCountryJp = document.getElementById('<%= this.WddlOwnerCountry.ClientID %>').value == '<%= Constants.COUNTRY_ISO_CODE_JP %>';
		<% } %>
		var authenticationStatusId = isOwnerAddrCountryJp
			? '<%= this.WlbAuthenticationStatus.ClientID %>'
			: '<%= this.WlbAuthenticationStatusGlobal.ClientID %>';
		var authenticationMessageId = isOwnerAddrCountryJp
			? '<%= this.WlbAuthenticationMessage.ClientID %>'
			: '<%= this.WlbAuthenticationMessageGlobal.ClientID %>';
		var phoneNumber = document.getElementById(isOwnerAddrCountryJp
			? '<%= this.WtbOwnerTel1.ClientID %>'
			: '<%= this.WtbOwnerTel1Global.ClientID %>').value;

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
			'<%= this.WtbOwnerTel1_1.ClientID %>',
			'<%= this.WtbOwnerTel1_2.ClientID %>',
			'<%= this.WtbOwnerTel1_3.ClientID %>',
			'<%= this.WtbOwnerTel1.ClientID %>',
			'<%= this.WtbOwnerTel1Global.ClientID %>');
		return result;
	}
	<% } %>

	// ドロップダウンリスト、ラジオボタンが初期値に戻らないようにページ読込完了時に値を再設定する（注文者情報）
	$(window).bind('load', function () {
		UpdateDdlAndRbl(
			'<%= this.WddlOwnerBirthYear.ClientID %>',
			'<%= this.WddlOwnerBirthMonth.ClientID %>',
			'<%= this.WddlOwnerBirthDay.ClientID %>',
			'<%= this.WrblOwnerSex.ClientID %>',
			'<%= this.WddlOwnerCountry.ClientID %>',
			<%= this.WddlOwnerBirthYear.SelectedIndex %>,
			<%= this.WddlOwnerBirthMonth.SelectedIndex %>,
			<%= this.WddlOwnerBirthDay.SelectedIndex %>,
			'<%= GetCorrectSexForDataBindDefault() %>',
			<%= this.WddlOwnerCountry.SelectedIndex %>,
			'<%= this.WddlOwnerAddr1.ClientID %>',
			<%= this.WddlOwnerAddr1.SelectedIndex %>
		);
	});
//-->
</script>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
</asp:Content>
