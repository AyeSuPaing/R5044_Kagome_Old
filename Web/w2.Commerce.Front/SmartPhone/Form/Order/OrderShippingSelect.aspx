<%--
=========================================================================================================
  Module      : 注文配送選択画面(OrderShippingSelect.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderShippingSelect.aspx.cs" Inherits="Form_Order_OrderShippingSelect" MaintainScrollPositionOnPostback="true" Title="注文配送選択ページ" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
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
<style>
	:root {
		--change-amount-control-size: 40px;
	}

	.cartInfoToggle {
		margin: 10px 0;
		background: #000;
		color: #fff;
	}

	.changeQuantity {
		height: calc(var(--change-amount-control-size) - 3px);
		width: var(--change-amount-control-size);
		border: 1px solid #ccc;
		background:#f2f2f2;
		line-height: var(--change-amount-control-size);
		font-size: 12px;
		font-weight: bolder;
		display: inline-block;
		vertical-align: middle;
		text-align: center;
		float: right;
		cursor: pointer;
	}

	.bulkChangeQuantity {
		float: right;
		height: 30px;
		margin: 0 5px;
		padding: 4px 15px 4px;
		border-radius: 5px;
		background-color: #000;
		color: #fff !important;
		line-height: 30px;
		font-size: 13px;
		vertical-align: middle;
	}

	.user-product-quantity {
		height: var(--change-amount-control-size);
		max-width: var(--change-amount-control-size);
		text-align: center;
		font-size: 12px;
		border: 1px solid #ddd;
		border-radius: 4px;
		display: inline-block;
		vertical-align: middle;
		float: right;
	}

	.btnDelete {
		color: white !important;
		margin: 0 5px;
		background: black;
		padding: 5px 20px;
		border-radius: 5px;
	}

	.copyWrapInfoButton {
		text-decoration: underline !important;
	}
</style>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-order order-shipping">
	<div class="step">
		<img src="<%: Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-step02.jpg" alt="注文者情報と配送先" width="320" />
	</div>
	<%-- 次へイベント用リンクボタン --%>
	<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" Runat="Server" />
	<%-- 戻るイベント用リンクボタン --%>
	<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" Runat="Server" />
	<%-- エラーメッセージ（デフォルト注文方法用） --%>
	<span style="color:red;text-align:center;display:block"><asp:Literal ID="lInvalidGiftCartErrorMessage" Runat="Server" /></span>
	<span style="color:red;text-align:center;display:block"><asp:Literal ID="lOrderErrorMessage" Runat="Server" /></span>
	<asp:UpdatePanel ID="upUpdatePanel" Runat="Server">
		<ContentTemplate>
			<asp:HiddenField ID="hfOpenCartIndex" Runat="server" Value="0" />
			<small id="hcErrorMessage" enableviewstate="false" class="fred" Runat="Server"></small>
			<% this.CartItemIndexTmp = -1; %>
			<asp:Repeater ID="rCartList" Runat="Server">
				<ItemTemplate>
					<div class="order-unit">
						<%-- ▼注文者情報▼ --%>
						<div class="owner" visible='<%# Container.ItemIndex == 0 %>' Runat="Server">
							<h2>注文者情報</h2>
							<% if (this.IsEasyUser) { %>
							<p style="margin: 5px; padding: 5px; text-align: center; background-color: #ffff80; border: 1px solid #D4440D; border-color:#E5A500; color: #CC7600;">ご購入手続きに必要な会員情報が不足しています。</p>
							<% } %>
							<dl class="order-form product">
							<%-- 注文商品 --%>
							<dt>注文商品</dt>
								<dd>
								<%-- ▼商品リスト▼ --%>
								<asp:Repeater id="rCart" DataSource="<%# GetCartObject(Container.DataItem).Items %>" Runat="server">
								<HeaderTemplate>
									<table class="cart-table">
									<tbody>
								</HeaderTemplate>
								<ItemTemplate>
									<tr class="<%# (((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1) ? "last" : "" %>">
									<td class="product-image">
									<a href='<%#: GetCartProduct(Container.DataItem).CreateProductDetailUrl() %>' runat="server" Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
										<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
									<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false %>" />
									</td>
									<td class="product-info">
										<ul>
											<li class="product-name">
												<a href='<%#: GetCartProduct(Container.DataItem).CreateProductDetailUrl() %>' runat="server" Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
												<%#: GetCartProduct(Container.DataItem).ProductJointName %></a>
												<%#: (GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false) ? GetCartProduct(Container.DataItem).ProductJointName : "" %>
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
								<dt><%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span><span id="efo_sign_name"></span></dt>
								<dd class="name">
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerName1"
											Runat="Server"
											ControlToValidate="tbOwnerName1"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
										<asp:CustomValidator
											ID="cvOwnerName2"
											Runat="Server"
											ControlToValidate="tbOwnerName2"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerName1" Text="<%#: this.CartList.Owner.Name1 %>" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>' />
									<w2c:ExtendedTextBox ID="tbOwnerName2" Text="<%#: this.CartList.Owner.Name2 %>" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>' />
								</dd>
								<%-- 注文者：氏名（かな） --%>
								<% if (isOwnerAddrCountryJp) { %>
								<dt>
									<%: ReplaceTag("@@User.name_kana.name@@") %>
									<span class="require">※</span><span id="efo_sign_kana"></span>
								</dt>
								<dd class="<%: ReplaceTag("@@User.name_kana.type@@") %>">
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerNameKana1"
											Runat="Server"
											ControlToValidate="tbOwnerNameKana1"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
										<asp:CustomValidator
											ID="cvOwnerNameKana2"
											Runat="Server"
											ControlToValidate="tbOwnerNameKana2"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerNameKana1" Text="<%#: this.CartList.Owner.NameKana1 %>" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>' />
									<w2c:ExtendedTextBox ID="tbOwnerNameKana2" Text="<%#: this.CartList.Owner.NameKana2 %>" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>' />
								</dd>
								<% } %>
								<%-- 注文者：生年月日 --%>
								<dt>
									<%: ReplaceTag("@@User.birth.name@@") %>
									<span class="require">※</span><span id="efo_sign_birth"></span>
								</dt>
								<dd class="birth">
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerBirth"
											Runat="Server"
											ControlToValidate="ddlOwnerBirthDay"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											EnableClientScript="false" />
									</p>
									<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue='<%#: this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Year.ToString() : "" %>' CssClass="year" Runat="Server" />
									年 
									<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue='<%#: this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Month.ToString() : "" %>' CssClass="month" Runat="Server" />
									月 
									<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue='<%#: this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Day.ToString() : "" %>' CssClass="date" Runat="Server" />
									日
								</dd>
								<%-- 注文者：性別 --%>
								<dt>
									<%: ReplaceTag("@@User.sex.name@@") %>
									<span class="require">※</span><span id="efo_sign_sex"></span>
								</dt>
								<dd class="sex">
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerSex"
											Runat="Server"
											ControlToValidate="rblOwnerSex"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											EnableClientScript="false" />
									</p>
									<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%#: GetCorrectSexForDataBind(this.CartList.Owner.Sex) %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" Runat="Server" />
								</dd>
								<%-- 注文者：メールアドレス --%>
								<dt>
									<%: ReplaceTag("@@User.mail_addr.name@@") %>
									<span class="require">※</span><span id="efo_sign_mail_addr"></span>
								</dt>
								<dd class="mail">
									<p class="msg">お手数ですが、確認のため２度入力してください。</p>
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerMailAddrForCheck"
											Runat="Server"
											ControlToValidate="tbOwnerMailAddr"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
										<asp:CustomValidator
											ID="cvOwnerMailAddr"
											Runat="Server"
											ControlToValidate="tbOwnerMailAddr"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
										<asp:CustomValidator
											ID="cvOwnerMailAddrConf"
											Runat="Server"
											ControlToValidate="tbOwnerMailAddrConf"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerMailAddr" Text="<%#: this.CartList.Owner.MailAddr %>" Type="email" MaxLength="256" Runat="Server" CssClass="mail-domain-suggest" />
									<w2c:ExtendedTextBox ID="tbOwnerMailAddrConf" Text="<%#: this.CartList.Owner.MailAddr %>" Type="email" MaxLength="256" Runat="Server" CssClass="mail-domain-suggest" />
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
											Runat="Server"
											ControlToValidate="tbOwnerMailAddr2"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
										<asp:CustomValidator
											ID="cvOwnerMailAddr2Conf"
											Runat="Server"
											ControlToValidate="tbOwnerMailAddr2Conf"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerMailAddr2" Text="<%#: this.CartList.Owner.MailAddr2 %>" Type="email" MaxLength="256" Runat="Server" CssClass="mail-domain-suggest" />
									<w2c:ExtendedTextBox ID="tbOwnerMailAddr2Conf" Text="<%#: this.CartList.Owner.MailAddr2 %>" Type="email" MaxLength="256" Runat="Server" CssClass="mail-domain-suggest" />
								</dd>
								<% } %>
								<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
								<%-- 注文者：国 --%>
								<dt>
									<%: ReplaceTag("@@User.country.name@@", ownerAddrCountryIsoCode) %>
									<span class="require">※</span>
								</dt>
								<dd>
									<asp:DropDownList ID="ddlOwnerCountry" Runat="Server" AutoPostBack="true" SelectedValue="<%#: this.CartList.Owner.AddrCountryIsoCode %>" DataSource="<%# this.UserCountryDisplayList %>" OnSelectedIndexChanged="ddlOwnerCountry_SelectedIndexChanged" DataTextField="Text" DataValueField="Value" /><br/>
									<asp:CustomValidator
										ID="cvOwnerCountry"
										Runat="Server"
										ControlToValidate="ddlOwnerCountry"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										EnableClientScript="false"
										CssClass="error_inline" />
									<span id="countryAlertMessage" class="msg" Runat="Server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
								</dd>
								<% } %>
								<%-- 注文者：郵便番号 --%>
								<% if (isOwnerAddrCountryJp) { %>
								<dt>
									<%: ReplaceTag("@@User.zip.name@@") %>
									<span class="require">※</span><span id="efo_sign_zip"></span>
								</dt>
								<dd class="zip">
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerZip1"
											Runat="Server"
											ControlToValidate="tbOwnerZip"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
										<span id="sOwnerZipError" Runat="Server" class="attention shortZipInputErrorMessage"></span>
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerZip" Type="tel" Text="<%#: this.CartList.Owner.Zip %>" MaxLength="8" Runat="Server" OnTextChanged="lbSearchOwnergAddr_Click" />
									<br />
									<asp:LinkButton ID="lbSearchOwnergAddr" Runat="Server" onclick="lbSearchOwnergAddr_Click" CssClass="btn-add-search" OnClientClick="return false;" Text="郵便番号から住所を入力" />
									<%--検索結果レイヤー--%>
									<uc:Layer ID="ucLayer" Runat="Server" />
								</dd>
								<% } %>
								<dt>
									<%: ReplaceTag("@@User.addr.name@@") %>
									<span class="require">※</span><% if (isOwnerAddrCountryJp) { %><span id="efo_sign_address"></span><% } %></dt>
								<dd class="address">
									<% if (isOwnerAddrCountryJp) { %>
									<%-- 注文者：都道府県 --%>
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerAddr1"
											Runat="Server"
											ControlToValidate="ddlOwnerAddr1"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<asp:DropDownList ID="ddlOwnerAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%#: this.CartList.Owner.Addr1 %>" Runat="Server" />
									<% } %>
									<%-- 注文者：市区町村 --%>
									<% if (isOwnerAddrCountryTw) { %>
									<asp:DropDownList Runat="Server" ID="ddlOwnerAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged" />
									<br />
									<% } else { %>
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerAddr2"
											Runat="Server"
											ControlToValidate="tbOwnerAddr2"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerAddr2" placeholder='市区町村' Text="<%#: this.CartList.Owner.Addr2 %>" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' Runat="Server" />
									<% } %>
									<%-- 注文者：番地 --%>
									<% if (isOwnerAddrCountryTw) { %>
									<asp:DropDownList Runat="Server" ID="ddlOwnerAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95" />
									<% } else { %>
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerAddr3"
											Runat="Server"
											ControlToValidate="tbOwnerAddr3"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerAddr3" placeholder='番地' Text="<%#: this.CartList.Owner.Addr3 %>" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' Runat="Server" />
									<% } %>
									<%-- 注文者：ビル・マンション名 --%>
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerAddr4"
											Runat="Server"
											ControlToValidate="tbOwnerAddr4"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerAddr4" placeholder='建物名' Text="<%#: this.CartList.Owner.Addr4 %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' Runat="Server" />
									<%-- 注文者：州 --%>
									<% if (isOwnerAddrCountryJp == false) { %>
									<% if (isOwnerAddrCountryUs) { %>
									<asp:DropDownList ID="ddlOwnerAddr5" DataSource="<%# this.UserStateList %>" DataTextField="Text" DataValueField="Value" Runat="Server" />
									<asp:CustomValidator
										ID="cvOwnerAddr5Ddl"
										Runat="Server"
										ControlToValidate="ddlOwnerAddr5"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										EnableClientScript="false"
										CssClass="error_inline" />
									<% } else if (isOwnerAddrCountryTw) { %>
									<w2c:ExtendedTextBox ID="tbOwnerAddrTw" placeholder='省' Text="<%#: this.CartList.Owner.Addr5 %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' Runat="Server" />
									<% } else { %>
									<w2c:ExtendedTextBox ID="tbOwnerAddr5" placeholder='州' Text="<%#: this.CartList.Owner.Addr5 %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' Runat="Server" />
									<% } %>
									<% } %>
								</dd>
								<%-- 注文者：郵便番号（海外向け） --%>
								<% if (isOwnerAddrCountryJp == false) { %>
								<dt>
									<%: ReplaceTag("@@User.zip.name@@", ownerAddrCountryIsoCode) %>
									<% if (isOwnerAddrZipNecessary) { %>&nbsp;<span class="require">※</span><% } %></dt>
								<dd>
									<asp:TextBox ID="tbOwnerZipGlobal" Text="<%#: this.CartList.Owner.Zip %>" MaxLength="20" Runat="Server" Type="tel" />
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerZipGlobal"
											Runat="Server"
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
										Runat="Server" />
								</dd>
								<% } %>
								<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
								<%-- 注文者：企業名 --%>
								<dt><%: ReplaceTag("@@User.company_name.name@@")%></dt>
								<dd class="company-name">
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerCompanyName"
											Runat="Server"
											ControlToValidate="tbOwnerCompanyName"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' Text="<%#: this.CartList.Owner.CompanyName %>" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' Runat="Server" />
								</dd>
								<%-- 注文者：部署名 --%>
								<dt><%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
								<dd class="company-post">
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerCompanyPostName"
											Runat="Server"
											ControlToValidate="tbOwnerCompanyPostName"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"　/>
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' Text="<%#: this.CartList.Owner.CompanyPostName %>" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' Runat="Server" />
								</dd>
								<% } %>
								<%-- 注文者：電話番号1 --%>
								<% if (isOwnerAddrCountryJp) { %>
								<dt>
									<%: ReplaceTag("@@User.tel1.name@@") %>
									<span class="require">※</span><span id="efo_sign_tel1"></span>
								</dt>
								<dd class="tel">
									<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
									<!-- mobile phone if use GMO payment -->
									<p class="warning"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE) %></p>
									<% } %>
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerTel1_1"
											Runat="Server"
											ControlToValidate="tbOwnerTel1"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerTel1" Text="<%#: this.CartList.Owner.Tel1 %>" MaxLength="13" Type="tel" style="width:100%;" Runat="Server" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
									<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
									<asp:LinkButton
										ID="lbGetAuthenticationCode"
										CssClass="btn-add-get"
										Runat="Server"
										Text="認証コードの取得"
										OnClick="lbGetAuthenticationCode_Click"
										OnClientClick="return checkTelNoInput();" />
									<asp:Label ID="lbAuthenticationStatus" Runat="Server" />
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
										Runat="Server" />
									<span class="notes">
										<% if (this.HasAuthenticationCode) { %>
										<asp:Label ID="lbHasAuthentication" CssClass="authentication_success" Runat="Server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
										<% } %>
										<span><%: GetVerificationCodeNote(ownerAddrCountryIsoCode) %></span>
										<asp:Label ID="lbAuthenticationMessage" Runat="Server" />
									</span>
									<br />
									<asp:CustomValidator
										ID="cvAuthenticationCode"
										Runat="Server"
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
											Runat="Server"
											ControlToValidate="tbOwnerTel2"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="False"
											SetFocusOnError="true" />
									</p>
									<w2c:ExtendedTextBox ID="tbOwnerTel2" Text="<%#: this.CartList.Owner.Tel2 %>" MaxLength="13" Type="tel" style="width:100%;" Runat="Server" CssClass="shortTel" />
								</dd>
								<% } else { %>
								<%-- 注文者：電話番号1（海外向け） --%>
								<dt>
									<%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %>
									&nbsp;<span class="require">※</span>
								</dt>
								<dd class="tel">
									<w2c:ExtendedTextBox ID="tbOwnerTel1Global" Text="<%#: this.CartList.Owner.Tel1 %>" Width="100%" Type="tel" MaxLength="30" Runat="Server" onchange="resetAuthenticationCodeInput('cvOwnerTel1Global')" />
									<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
									<asp:LinkButton
										ID="lbGetAuthenticationCodeGlobal"
										class="btn-add-get"
										Runat="Server"
										Text="認証コードの取得"
										OnClick="lbGetAuthenticationCode_Click"
										OnClientClick="return checkTelNoInput();" />
									<asp:Label ID="lbAuthenticationStatusGlobal" Runat="Server" />
									<% } %>
									<p class="attention">
										<asp:CustomValidator
											ID="cvOwnerTel1Global"
											Runat="Server"
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
										Runat="Server" />
									<span class="notes">
										<% if (this.HasAuthenticationCode) { %>
										<asp:Label ID="lbHasAuthenticationGlobal" CssClass="authentication_success" Runat="Server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
										<% } %>
										<span><%: GetVerificationCodeNote(ownerAddrCountryIsoCode) %></span>
										<asp:Label ID="lbAuthenticationMessageGlobal" Runat="Server" />
									</span>
									<br />
									<asp:CustomValidator
										ID="cvAuthenticationCodeGlobal"
										Runat="Server"
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
									<w2c:ExtendedTextBox ID="tbOwnerTel2Global" Text="<%#: this.CartList.Owner.Tel2 %>" Type="tel" MaxLength="30" Runat="Server" />
									<asp:CustomValidator
										ID="cvOwnerTel2Global"
										Runat="Server"
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
									<asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" Runat="Server" />
								</dd>
							</dl>
							<% this.CartItemIndexTmp = -1; %>
						</div>
						<%-- ▲注文者情報▲ --%>

						<%-- ▼配送先情報▼ --%>
						<div class="shipping cartBox">
							<%
								this.CartItemIndexTmp++;
								this.CartShippingItemIndexTmp = -1;
							%>
							<h2 Visible="<%# GetCartObject(Container.DataItem).IsDigitalContentsOnly == false %>" Runat="Server"><%# this.CartList.Items.Count > 1 ? "カート番号" + (Container.ItemIndex + 1).ToString() + "の" : "" %>お届け先の住所</h2>
							<span class="invalidInputError" style="color: red; display: block; padding-left: 11px; padding-top: 5px;"></span>
							<asp:Repeater ID="rCartShippings" Runat="Server">
								<ItemTemplate>
									<% this.CartShippingItemIndexTmp++; %>
									<div class="orderBoxLarge">
										<div class="cartNo cartInfoToggle" style="user-select: none; padding: 0.5em; <%# GetCartShipping(Container.DataItem).CartObject.IsGift ? string.Empty : "display: none;" %>" visible="<%# DisplayFixedPurchaseShipping(Container) == false %>" onclick="initCartInfoToggle(<%# Container.ItemIndex %>)">
											<span id="toggleIcon">▼</span>
											<span runat="server" class="hiddenIndex" style="display: none;" gift="<%#: GetCartShipping(Container.DataItem).CartObject.IsGift %>"><%#: Container.ItemIndex + 1 %></span>
											&ensp;配送先<%# Container.ItemIndex + 1 %>
										</div>
										<asp:LinkButton ID="lbDeleteShipping"
											Runat="Server"
											Text="削除"
											CommandArgument="<%# Container.ItemIndex %>"
											OnClick="lbDeleteShipping_Click"
											Visible="<%# (DisplayFixedPurchaseShipping(Container) == false) && (((IList)((Repeater)Container.Parent).DataSource).Count > 1) %>"
											class="btnDelete" />
										<div class="<%#: GetCartShipping(Container.DataItem).CartObject.IsGift ? "toggleBlock" : string.Empty %>">
											<%-- Cart sender --%>
											<div id="divSenderInputForm" class="order-form" Runat="Server" Visible='<%# (DisplayFixedPurchaseShipping(Container) == false) && GetCartShipping(Container.DataItem).CartObject.IsGift %>'>
												<dl>
													<dt>送り主</dt>
												</dl>
												<dl>
													<dd>
														<asp:CheckBox ID="cbSameSenderAsShipping1"
															Checked='<%# GetCartShipping(Container.DataItem).IsSameSenderAsShipping1 %>'
															Visible='<%# Container.ItemIndex != 0 %>'
															Text="配送先1と同じ送り主を指定する"
															AutoPostBack="true"
															OnCheckedChanged="cbSameSenderAsShipping1_OnCheckedChanged"
															Runat="Server" />
														<div id="hcShippingSender" visible='<%# GetCartShipping(Container.DataItem).IsSameSenderAsShipping1 == false %>' Runat="Server">
															<asp:RadioButtonList ID="rblSenderSelector"
																AutoPostBack="true"
																RepeatDirection="Horizontal"
																CssClass="radioBtn sender-selector"
																OnSelectedIndexChanged="rblSenderSelector_OnSelectedIndexChanged"
																SelectedValue="<%#: GetCartShipping(Container.DataItem).SenderAddrKbn %>"
																Runat="Server">
																<asp:ListItem Text="注文者を送り主とする" Value="Owner" />
																<asp:ListItem Text="新規入力する" Value="New" />
															</asp:RadioButtonList>

															<%-- ▽送り主：入力フォーム▽ --%>
															<div id="divSenderInputFormInner" class="userList" visible="<%# GetSendFromOwner(GetCartShipping(Container.DataItem)) == false %>" Runat="Server" style="padding-top: 0;">
																<%
																	var senderAddrCountryIsoCode = GetSenderAddrCountryIsoCode(this.CartItemIndexTmp, this.CartShippingItemIndexTmp);
																	var isSenderAddrCountryJp = IsCountryJp(senderAddrCountryIsoCode);
																	var isSenderAddrCountryUs = IsCountryUs(senderAddrCountryIsoCode);
																	var isSenderAddrCountryTw = IsCountryTw(senderAddrCountryIsoCode);
																	var isSenderAddrZipNecessary = IsAddrZipcodeNecessary(senderAddrCountryIsoCode);
																%>
																<dl>
																	<dt><%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span></dt>
																	<dd class="name">
																		<w2c:ExtendedTextBox ID="tbSenderName1" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>' />
																		<w2c:ExtendedTextBox ID="tbSenderName2" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>' />
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderName1"
																				Runat="Server"
																				ControlToValidate="tbSenderName1"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				SetFocusOnError="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage" />
																			<asp:CustomValidator
																				ID="cvSenderName2"
																				Runat="Server"
																				ControlToValidate="tbSenderName2"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				SetFocusOnError="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage" />
																		</p>
																	</dd>
																	<% if (isSenderAddrCountryJp) { %>
																	<dt><%: ReplaceTag("@@User.name_kana.name@@") %><span class="require">※</span></dt>
																	<dd class="name">
																		<w2c:ExtendedTextBox ID="tbSenderNameKana1" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>' />
																		<w2c:ExtendedTextBox ID="tbSenderNameKana2" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' Runat="Server" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>' />
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderNameKana1"
																				Runat="Server"
																				ControlToValidate="tbSenderNameKana1"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																				ValidateEmptyText="true"
																				SetFocusOnError="true"
																				ValidationGroup="OrderShipping" />
																			<asp:CustomValidator
																				ID="cvenderNameKana2"
																				Runat="Server"
																				ControlToValidate="tbSenderNameKana2"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				SetFocusOnError="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage" />
																		</p>
																	</dd>
																	<% } %>
																	<%-- 送り主：国 --%>
																	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																	<dt><%: ReplaceTag("@@User.country.name@@", senderAddrCountryIsoCode) %><span class="require">※</span></dt>
																	<dd>
																		<asp:DropDownList ID="ddlSenderCountry"
																			Runat="Server"
																			DataSource="<%# this.UserCountryDisplayList %>"
																			AutoPostBack="true"
																			OnSelectedIndexChanged="ddlSenderCountry_SelectedIndexChanged"
																			DataTextField="Text"
																			DataValueField="Value"
																			SelectedValue="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE) %>" />
																		<asp:CustomValidator
																			ID="cvSenderCountry"
																			Runat="Server"
																			ControlToValidate="ddlSenderCountry"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage" />
																	</dd>
																	<% } %>
																	<%-- 送り主：郵便番号 --%>
																	<% if (isSenderAddrCountryJp) { %>
																	<dt><%: ReplaceTag("@@User.addr.name@@") %><span class="require">※</span></dt>
																	<dd class="zip">
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderZip1"
																				Runat="Server"
																				ControlToValidate="tbSenderZip"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				SetFocusOnError="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage" />
																		</p>
																		<w2c:ExtendedTextBox ID="tbSenderZip" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ZIP) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' Runat="Server" />
																		<small id="sSenderZipError" Runat="Server" class="fred sSenderZipError"></small>
																		<asp:LinkButton ID="lbSearchSenderAddr"
																			Runat="Server"
																			Text="郵便番号から住所を入力"
																			OnClick="lbSearchSenderAddr_Click"
																			CssClass="btn-add-search" />
																	</dd>
																	<% } %>
																	<%-- 送り主：都道府県 --%>
																	<dt><%: ReplaceTag("@@User.addr1.name@@", senderAddrCountryIsoCode) %><span class="fred">※</span></dt>
																	<dd class="address">
																		<% if (isSenderAddrCountryJp) { %>
																		<%-- 配送先：都道府県 --%>
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderAddr1"
																				Runat="Server"
																				ControlToValidate="ddlSenderAddr1"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																				SetFocusOnError="true" />
																		</p>
																		<asp:DropDownList ID="ddlSenderAddr1"
																			DataSource="<%# this.Addr1List %>"
																			DataTextField="Text"
																			DataValueField="Value"
																			SelectedValue="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1) %>"
																			Runat="Server" />
																		<% } %>
																		<%-- 配送先：市区町村 --%>
																		<% if (isSenderAddrCountryTw) { %>
																		<asp:DropDownList ID="ddlSenderAddr2"
																			Runat="Server"
																			DataSource="<%# this.UserTwCityList %>"
																			AutoPostBack="true"
																			DataTextField="Text"
																			DataValueField="Value"
																			OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged" />
																		<br />
																		<% } else { %>
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderAddr2"
																				Runat="Server"
																				ControlToValidate="tbSenderAddr2"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																				SetFocusOnError="true" />
																		</p>
																		<w2c:ExtendedTextBox ID="tbSenderAddr2" placeholder='市区町村' Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2) %>" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' Runat="Server" />
																		<% } %>
																		<%-- 配送先：番地 --%>
																		<% if (isSenderAddrCountryTw) { %>
																		<asp:DropDownList Runat="Server" ID="ddlSenderAddr3" DataTextField="Key" DataValueField="Value" Width="95" />
																		<% } else { %>
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderAddr3"
																				Runat="Server"
																				ControlToValidate="tbSenderAddr3"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																				SetFocusOnError="true" />
																		</p>
																		<w2c:ExtendedTextBox ID="tbSenderAddr3" placeholder='番地' Text='<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3) %>' MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' Runat="Server" />
																		<% } %>
																		<%-- 配送先：ビル・マンション名 --%>
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderAddr4"
																				Runat="Server"
																				ControlToValidate="tbSenderAddr4"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																				SetFocusOnError="true" />
																		</p>
																		<w2c:ExtendedTextBox ID="tbSenderAddr4" placeholder='建物名' Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4) %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' Runat="Server" />
																		<% if (isSenderAddrCountryJp == false) { %>
																		<%-- 配送先：州 --%>
																		<% if (isSenderAddrCountryUs) { %>
																		<asp:DropDownList Runat="Server"
																			ID="ddlSenderAddr5"
																			DataSource="<%# this.UserStateList %>"
																			Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), CartShipping.FIELD_ORDERSHIPPING_SENDER_ADDR5_US) %>" />
																		<asp:CustomValidator
																			ID="cvSenderAddr5Ddl"
																			Runat="Server"
																			ControlToValidate="ddlSenderAddr5"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage" />
																		<% } else { %>
																		<w2c:ExtendedTextBox Runat="Server" ID="tbSenderAddr5" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5) %>" placeholder="省" />
																		<asp:CustomValidator
																			ID="cvSenderAddr5"
																			Runat="Server"
																			ControlToValidate="tbSenderAddr5"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																		<% } %>
																		<% } %>
																	</dd>
																	<%-- 送り主：電話番号 --%>
																	<% if (isSenderAddrCountryJp) { %>
																	<dt>
																		<%: ReplaceTag("@@User.tel1.name@@") %>
																		&nbsp;<span class="fred">※</span>
																	</dt>
																	<dd class="tel">
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderTel1"
																				Runat="Server"
																				ControlToValidate="tbSenderTel1"
																				ValidationGroup="OrderShipping"
																				ValidateEmptyText="true"
																				SetFocusOnError="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																				CssClass="error_inline" />
																		</p>
																		<w2c:ExtendedTextBox ID="tbSenderTel1" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_TEL1) %>" Type="tel" style="width:100%;" MaxLength="13" Runat="Server" /><br />
																	</dd>
																	<% } else { %>
																	<%-- 送り主：電話番号1（海外向け） --%>
																	<dt>
																		<%: ReplaceTag("@@User.tel1.name@@", senderAddrCountryIsoCode) %>
																		&nbsp;<span class="fred">※</span>
																	</dt>
																	<dd class="tel">
																		<p class="attention">
																			<asp:CustomValidator
																				ID="cvSenderTel1Global"
																				Runat="Server"
																				ControlToValidate="tbSenderTel1Global"
																				ValidationGroup="OrderShippingGlobal"
																				ValidateEmptyText="true"
																				SetFocusOnError="true"
																				ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																				CssClass="error_inline" />
																		</p>
																		<w2c:ExtendedTextBox ID="tbSenderTel1Global" Runat="Server" Width="100%" Type="tel" MaxLength="30" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_TEL1) %>" />
																	</dd>
																	<% } %>
																</dl>
															</div>
															<%-- △送り主：入力フォーム△ --%>
														</div>
													</dd>
												</dl>
											</div>

											<div id="divShippingInputForm" Runat="Server">
												<dl class="order-form">
													<dt>配送先</dt>
													<dd>
														<asp:DropDownList ID="ddlShippingKbnList"
															DataSource="<%# this.UserShippingList %>"
															DataTextField="text"
															DataValueField="value"
															SelectedValue="<%#: GetCartShipping(Container.DataItem).ShippingAddrKbn %>"
															OnSelectedIndexChanged="ddlShippingKbnList_OnSelectedIndexChanged"
															AutoPostBack="true"
															Runat="Server" />
														<span style="color:red;display:block;"><asp:Literal ID="lShippingCountryErrorMessage" Runat="Server" /></span>
														<span id='<%# "spErrorConvenienceStore" + Container.ItemIndex.ToString() %>' style="color:red;display:block;"></span>
														<%-- ▽配送先表示▽ --%>
														<div id="divShippingDisp" visible="<%# GetShipToOwner(GetCartShipping(Container.DataItem)) %>" Runat="Server">
															<% var isShippingAddrCountryJp = IsCountryJp(this.CountryIsoCode); %>
															<dl class="order-form">
																<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
																<dd>
																	<asp:Literal ID="lShippingName1" Runat="Server" /><asp:Literal ID="lShippingName2" Runat="Server" />&nbsp;様
																	<% if (isShippingAddrCountryJp) { %>（<asp:Literal ID="lShippingNameKana1" Runat="Server" /><asp:Literal ID="lShippingNameKana2" Runat="Server" />&nbsp;さま）<% } %></dd>
																<dt>
																	<%: ReplaceTag("@@User.addr.name@@") %>
																</dt>
																<dd>
																	<% if (isShippingAddrCountryJp) { %>〒<asp:Literal ID="lShippingZip" Runat="Server" /><br /><% } %>
																	<asp:Literal ID="lShippingAddr1" Runat="Server" /> <asp:Literal ID="lShippingAddr2" Runat="Server" /><br />
																	<asp:Literal ID="lShippingAddr3" Runat="Server" /> <asp:Literal ID="lShippingAddr4" Runat="Server" />
																	<asp:Literal ID="lShippingAddr5" Runat="Server" /><br />
																	<% if (isShippingAddrCountryJp == false) { %><asp:Literal ID="lShippingZipGlobal" Runat="Server" /><br /><% } %>
																	<asp:Literal ID="lShippingCountryName" Runat="Server" />
																</dd>
																<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
																<dt><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
																<dd>
																	<asp:Literal ID="lShippingCompanyName" Runat="Server" />&nbsp<asp:Literal ID="lShippingCompanyPostName" Runat="Server" />
																</dd>
																<% } %>
																<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
																<dd>
																	<asp:Literal ID="lShippingTel1" Runat="Server" />
																</dd>
															</dl>
														</div>
														<%-- △配送先表示△ --%>

														<%-- ▽配送先入力フォーム▽ --%>
														<div id="divShippingInputFormInner" class="shipping-input" visible="<%# GetShipToOwner(GetCartShipping(Container.DataItem)) == false %>" Runat="Server">
															<div id="divShippingVisibleConvenienceStore" Runat="Server">
															<%
																var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartItemIndexTmp, this.CartShippingItemIndexTmp);
																var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
																var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
																var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
																var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
															%>
															<dl class="order-form">
																<%-- 配送先：氏名 --%>
																<dt><%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span><span id="<%# "efo_sign_ship_name" + Container.ItemIndex %>"></span></dt>
																<dd class="name">
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingName1"
																			Runat="Server"
																			ControlToValidate="tbShippingName1"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true" />
																		<asp:CustomValidator
																			ID="cvShippingName2"
																			Runat="Server"
																			ControlToValidate="tbShippingName2"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingName1" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' Runat="Server"></w2c:ExtendedTextBox>
																	<w2c:ExtendedTextBox ID="tbShippingName2" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' Runat="Server"></w2c:ExtendedTextBox>
																</dd>
																<%-- 配送先：氏名（かな） --%>
																<% if (isShippingAddrCountryJp) { %>
																<dt>
																	<%: ReplaceTag("@@User.name_kana.name@@") %>
																	<span class="require">※</span><span id="<%# "efo_sign_ship_kana" + Container.ItemIndex %>"></span>
																</dt>
																<dd class="name-kana">
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingNameKana1"
																			Runat="Server"
																			ControlToValidate="tbShippingNameKana1"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ValidationGroup="OrderShipping" />
																		<asp:CustomValidator
																			ID="cvShippingNameKana2"
																			Runat="Server"
																			ControlToValidate="tbShippingNameKana2"
																			ValidationGroup="OrderShipping"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			ValidateEmptyText="true"
																			SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingNameKana1" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) %>" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' Runat="Server" />
																	<w2c:ExtendedTextBox ID="tbShippingNameKana2" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) %>" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' Runat="Server" />
																</dd>
																<% } %>
																<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																<dt>
																	<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
																	<span class="require">※</span>
																</dt>
																<dd>
																	<asp:DropDownList
																		ID="ddlShippingCountry"
																		Runat="Server"
																		AutoPostBack="true"
																		DataSource="<%# this.ShippingAvailableCountryDisplayList %>"
																		OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"
																		DataTextField="Text"
																		DataValueField="Value"
																		SelectedValue="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE) %>" />
																	<asp:CustomValidator
																		ID="cvShippingCountry"
																		Runat="Server"
																		ControlToValidate="ddlShippingCountry"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</dd>
																<% } %>
																<%-- 配送先：郵便番号 --%>
																<% if (isShippingAddrCountryJp) { %>
																<dt>
																	<%: ReplaceTag("@@User.zip.name@@") %>
																	<span class="require">※</span><span id="<%# "efo_sign_ship_zip" + Container.ItemIndex %>"></span>
																</dt>
																<dd class="zip">
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingZip1"
																			Runat="Server"
																			ControlToValidate="tbShippingZip"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage" />
																		<span id="sShippingZipError" Runat="Server" class="shortZipInputErrorMessage"></span>
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingZip" Type="tel" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" OnTextChanged="lbSearchShippingAddr_Click" MaxLength="8" Runat="Server" />
																	<br />
																	<asp:LinkButton ID="lbSearchShippingAddr"
																		Runat="Server"
																		OnClick="lbSearchShippingAddr_Click"
																		Text="郵便番号から住所を入力"
																		CssClass="btn-add-search"
																		OnClientClick="return false;" />
																</dd>
																<% } %>
																<dt>
																	<%: ReplaceTag("@@User.addr.name@@") %>
																	<span class="require">※</span><% if (isShippingAddrCountryJp) { %><span id="<%# "efo_sign_ship_address" + Container.ItemIndex %>"></span><% } %></dt>
																	<dd class="address">
																	<% if (isShippingAddrCountryJp) { %>
																		<%-- 配送先：都道府県 --%>
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingAddr1"
																			Runat="Server"
																			ControlToValidate="ddlShippingAddr1"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			SetFocusOnError="true" />
																	</p>
																	<asp:DropDownList
																		ID="ddlShippingAddr1"
																		DataSource="<%# this.Addr1List %>"
																		DataTextField="Text"
																		DataValueField="Value"
																		SelectedValue="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>"
																		Runat="Server" />
																	<% } %>
																	<%-- 配送先：市区町村 --%>
																	<% if (isShippingAddrCountryTw) { %>
																	<asp:DropDownList
																		ID="ddlShippingAddr2"
																		Runat="Server"
																		DataSource="<%# this.UserTwCityList %>"
																		AutoPostBack="true"
																		DataTextField="Text"
																		DataValueField="Value"
																		OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged" />
																	<br />
																	<% } else { %>
																	<p class="attention">
																	<asp:CustomValidator
																		ID="cvShippingAddr2"
																		Runat="Server"
																		ControlToValidate="tbShippingAddr2"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingAddr2" placeholder='市区町村' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %>" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' Runat="Server" />
																	<% } %>
																	<%-- 配送先：番地 --%>
																	<% if (isShippingAddrCountryTw) { %>
																	<asp:DropDownList Runat="Server" ID="ddlShippingAddr3" DataTextField="Key" DataValueField="Value" Width="95" />
																	<% } else { %>
																	<p class="attention">
																	<asp:CustomValidator
																		ID="cvShippingAddr3"
																		Runat="Server"
																		ControlToValidate="tbShippingAddr3"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingAddr3" placeholder='番地' Text='<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %>' MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' Runat="Server" />
																	<% } %>
																	<%-- 配送先：ビル・マンション名 --%>
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingAddr4"
																			Runat="Server"
																			ControlToValidate="tbShippingAddr4"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingAddr4" placeholder='建物名' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' Runat="Server" />
																	<% if (isShippingAddrCountryJp == false) { %>
																	<%-- 配送先：州 --%>
																	<% if (isShippingAddrCountryUs) { %>
																	<asp:DropDownList ID="ddlShippingAddr5" DataSource="<%# this.UserStateList %>" DataTextField="Text" DataValueField="Value" Runat="Server" />
																	<asp:CustomValidator
																		ID="cvShippingAddr5Ddl"
																		Runat="Server"
																		ControlToValidate="ddlShippingAddr5"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																	<% } else if (isShippingAddrCountryTw) { %>
																	<w2c:ExtendedTextBox ID="tbShippingAddrTw" placeholder='省' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' Runat="Server" />
																	<% } else { %>
																	<w2c:ExtendedTextBox ID="tbShippingAddr5" placeholder='州' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' Runat="Server" />
																	<% } %>
																	<% } %>
																</dd>
																<%-- 配送先：郵便番号（海外向け） --%>
																<% if (isShippingAddrCountryJp == false) { %>
																<dt>
																	<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
																	<% if (isShippingAddrZipNecessary) { %>&nbsp;<span class="require">※</span><% } %></dt>
																<dd>
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingZipGlobal"
																			Runat="Server"
																			ControlToValidate="tbShippingZipGlobal"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			SetFocusOnError="true" />
																	</p>
																	<asp:TextBox ID="tbShippingZipGlobal" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" MaxLength="20" Runat="Server" Type="tel" />
																	<asp:LinkButton
																		ID="lbSearchAddrShippingFromZipGlobal"
																		OnClick="lbSearchAddrShippingFromZipGlobal_Click"
																		Style="display:none;"
																		Runat="Server" />
																</dd>
																<% } %>
																<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
																<%-- 配送先：企業名 --%>
																<dt><%: ReplaceTag("@@User.company_name.name@@")%></dt>
																<dd class="company-name">
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingCompanyName"
																			Runat="Server"
																			ControlToValidate="tbShippingCompanyName"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME) %>" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' Runat="Server" />
																</dd>
																<%-- 配送先：部署名 --%>
																<dt><%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
																<dd class="company-post">
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingCompanyPostName"
																			Runat="Server"
																			ControlToValidate="tbShippingCompanyPostName"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' Runat="Server" />
																</dd>
																<% } %>
																<%-- 配送先：電話番号 --%>
																<% if (isShippingAddrCountryJp) { %>
																<dt>
																	<%: ReplaceTag("@@User.tel1.name@@") %>
																	<span class="require">※</span><span id="<%# "efo_sign_ship_tel1" + Container.ItemIndex %>"></span>
																</dt>
																<dd class="tel">
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvShippingTel1_1"
																			Runat="Server"
																			ControlToValidate="tbShippingTel1"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingTel1" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" MaxLength="13" Type="tel" style="width:100%;" Runat="Server" CssClass="shortTel" />
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
																			Runat="Server"
																			ControlToValidate="tbShippingTel1Global"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			SetFocusOnError="true" />
																	</p>
																	<w2c:ExtendedTextBox ID="tbShippingTel1Global" Type="tel" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" MaxLength="30" Runat="Server" />
																</dd>
																<% } %>
																</div>
															<dd id="divSaveToUserShipping" visible="<%# this.IsLoggedIn %>" Runat="Server">
															<%-- ポストバックCustomValidatorの状態がクリアされてしまうため、JavaScirptで表示非表示を制御する --%>
															<p>
																<asp:RadioButtonList ID="rblSaveToUserShipping" OnSelectedIndexChanged="rblSaveToUserShipping_OnSelectedIndexChanged" AutoPostBack="true" SelectedValue='<%#: GetCartShipping(Container.DataItem).UserShippingRegistFlg ? "1" : "0" %>' RepeatLayout="Flow" CssClass="radioBtn" Runat="Server">
																	<asp:ListItem Text="配送先情報を保存しない" Value="0" />
																	<asp:ListItem Text="配送先情報を保存する" Value="1" />
																</asp:RadioButtonList>
															</p>
															<dd id="dlUserShippingName" visible="false" Runat="Server"><span id="efo_sign_ship_addr_name"></span>
																<p>配送先名をご入力ください</p>
																<p class="attention">
																	<asp:CustomValidator
																		ID="cvUserShippingName"
																		Runat="Server"
																		ControlToValidate="tbUserShippingName"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		SetFocusOnError="true" />
																</p>
																<w2c:ExtendedTextBox ID="tbUserShippingName" Text="<%#: GetCartShipping(Container.DataItem).UserShippingName %>" MaxLength="15" Runat="Server" />
															</dd>
															</dd>
														</div>
														<%-- △配送先入力フォーム△ --%>
													</dd>
												</dl>

												<dl class="order-form shipping-require">
													<dt visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">配送指定</dt>
													<dd visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server" class="userList">
														配送方法を選択して下さい。
														<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[((RepeaterItem)Container.Parent.Parent).ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" Runat="Server" />
													</dd>
													<div id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) && CanDisplayDeliveryCompany(((RepeaterItem)Container.Parent.Parent).ItemIndex)) %>" Runat="Server" class="userList">
														<dd>配送サービスを選択して下さい。
															<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" Runat="Server" />
														</dd>
													</div>
													<dd id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">
														<dl id="dlShipppingDateTime">
															<dt id="dtShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">配送希望日</dt>
															<dd id="ddShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">
																<asp:DropDownList
																	ID="ddlShippingDate"
																	Runat="Server"
																	CssClass="input_border"
																	DataTextField="text"
																	DataValueField="value"
																	DataSource="<%# GetShippingDateList(GetCartShipping(Container.DataItem), this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex]) %>"
																	OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnCheckedChanged"
																	AutoPostBack="true" />
																<br />
																<asp:Label ID="lShippingDateErrorMessage" CssClass="attention" Visible="false" Runat="Server" />
															</dd>
															<div id="divShippingTime" Runat="Server">
																<dt id="dtShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">配送希望時間帯</dt>
																<dd id="ddShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">
																	<asp:DropDownList id="ddlShippingTime" Runat="Server" DataSource="<%# GetShippingTimeList(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%#: GetShippingTime(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" />
																</dd>
															</div>
														</dl>
													</dd>
												</dl>
												<%-- ▽のし情報▽--%>
												<div visible="<%# IsDisplayGiftWrappingPaperAndBag(Container.DataItem, ((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server" class="order-form">
													<dl>
														<dt>
															のし・包装情報&nbsp;&nbsp;
															<asp:LinkButton ID="lbCopyWrappingInfoToOtherShippings"
																OnClick="lbCopyWrappingInfoToOtherShippings_Click"
																Runat="Server"
																Text="他の配送先にコピー"
																CssClass="copyWrapInfoButton" />
														</dt>
													</dl>
													<dl visible='<%# GetWrappingPaperFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' runat="server" class="userList" style="padding: 0 21px;">
														<dd style="border-bottom: 1px solid #000;">のし</dd>
														<dd>種類&nbsp;&nbsp;<asp:DropDownList ID="ddlWrappingPaperType" DataSource='<%# GetWrappingPaperTypes(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' SelectedValue='<%#: GetCartShipping(Container.DataItem).WrappingPaperType %>' DataTextField="text" DataValueField="value" runat="server" /></dd>
														<dd>差出人&nbsp;&nbsp;<asp:TextBox ID="tbWrappingPaperName" Text="<%#: GetCartShipping(Container.DataItem).WrappingPaperName %>" MaxLength="200" Width="200" runat="server" /></dd>
													</dl>
													<dl visible='<%# GetWrappingBagFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' runat="server" class="userList" style="padding: 0 21px;">
														<dd style="border-bottom: 1px solid #000;">包装</dd>
														<dd>種類&nbsp;&nbsp;<asp:DropDownList ID="ddlWrappingBagType" DataSource='<%# GetWrappingBagTypes(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' SelectedValue='<%#: GetCartShipping(Container.DataItem).WrappingBagType %>' DataTextField="text" DataValueField="value" runat="server" />
														</dd>
													</dl>
												</div>
												<asp:Repeater ID="rAllocatedProducts" Runat="Server">
													<HeaderTemplate>
														<%-- △のし情報△ --%>
														<%-- Cart product --%>
														<div class="userProduct order-form" style="border: none;">
															<dl>
																<dt>商品</dt>
															</dl>
															<table class="cart-table">
																<tbody>
													</HeaderTemplate>
													<ItemTemplate>
														<tr class="cart-unit-product">
															<td class="product-image">
																<a href='<%#: GetProductCount(Container.DataItem).Product.CreateProductDetailUrl() %>' Runat="Server" Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() %>">
																	<w2c:ProductImage ProductMaster="<%# GetProductCount(Container.DataItem).Product %>"
																		ImageSize="M"
																		Runat="Server" />
																</a>
																<w2c:ProductImage ProductMaster="<%# GetProductCount(Container.DataItem).Product %>"
																	ImageSize="M"
																	Runat="Server"
																	Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() == false %>" />
															</td>
															<td class="product-info">
																<ul>
																	<li class="product-name">
																		<a href='<%#: GetProductCount(Container.DataItem).Product.CreateProductDetailUrl() %>' Runat="Server" Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() %>">
																			<%#: GetProductCount(Container.DataItem).Product.ProductJointName %>
																		</a>
																		<%#: GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() == false ? GetProductCount(Container.DataItem).Product.ProductJointName : "" %>
																		<%#: GetProductCount(Container.DataItem).Product.GetProductTag("tag_cart_product_message").Length != 0 ? "<p class=\"product-msg\">" + GetProductCount(Container.DataItem).Product.GetProductTag("tag_cart_product_message") + "</p>" : "" %>
																	</li>
																	<li class="product-price" Visible="<%# GetProductCount(Container.DataItem).Product.IsSubscriptionBoxFixedAmount() == false %>" Runat="Server">
																		<%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(GetProductCount(Container.DataItem).Product.Price * GetProductCount(Container.DataItem).Count)) %> (<%#: this.ProductPriceTextPrefix %>)
																	</li>
																	<li visible='<%# GetProductCount(Container.DataItem).Product.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' Runat="Server">
																		<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetProductCount(Container.DataItem).Product.ProductOptionSettingList %>' Runat="Server">
																			<ItemTemplate>
																				<%#: ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																				<%# string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false ? "<br />" : string.Empty %>
																			</ItemTemplate>
																		</asp:Repeater>
																	</li>
																</ul>
															</td>
															<td class="product-control" style="max-width: 125px; min-width: 125px; background: none;">
																<div class="amout">
																	<span Runat="Server" visible="<%# FindCart(Container.DataItem).IsGift %>">
																		<asp:LinkButton Runat="Server"
																			Text="+"
																			Enabled="<%# CanChangeQuantity(Container.DataItem, true) %>"
																			CssClass="changeQuantity"
																			OnClick="lbRecalculateCart_Click"
																			Style="text-decoration: none"
																			CommandArgument="plus" />
																		<w2c:ExtendedTextBox ID="tbProductCount"
																			Type="tel"
																			Runat="Server"
																			Text='<%#: GetProductCount(Container.DataItem).Count %>'
																			CssClass="user-product-quantity"
																			OnTextChanged="lbRecalculateCart_Click"
																			onKeyPress="return isNumberKey(event)"
																			AutoPostBack="true"
																			MaxLength="3" />
																		<asp:LinkButton Runat="Server"
																			Text="-"
																			Enabled="<%# CanChangeQuantity(Container.DataItem, false) %>"
																			CssClass="changeQuantity"
																			OnClick="lbRecalculateCart_Click"
																			Style="text-decoration: none"
																			CommandArgument="subtract" />
																		<asp:LinkButton ID="lbRecalculateCart" OnClick="lbRecalculateCart_Click" class="lbRecalculateCart" Runat="Server" />
																	</span>
																</div>
															</td>
														</tr>
													</ItemTemplate>
													<FooterTemplate>
																</tbody>
															</table>
															<small id="hcErrorMessage" enableviewstate="false" class="fred" Runat="Server" />
														</div>
													</FooterTemplate>
												</asp:Repeater>
												<span id="sInvoices" Runat="Server" visible="false">
													<dl class="order-form shipping-require">
														<div id="divUniformInvoiceType" Runat="Server">
															<dt>発票種類</dt>
															<dd>
																<asp:DropDownList
																	ID="ddlUniformInvoiceType"
																	Runat="Server"
																	CssClass="input_border"
																	DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE) %>"
																	DataTextField="text"
																	DataValueField="value"
																	OnSelectedIndexChanged="ddlUniformInvoiceType_SelectedIndexChanged"
																	AutoPostBack="true" />
																<asp:DropDownList
																	ID="ddlUniformInvoiceTypeOption"
																	Runat="Server"
																	CssClass="input_border"
																	DataTextField="text"
																	DataValueField="value"
																	OnSelectedIndexChanged="ddlUniformInvoiceTypeOption_SelectedIndexChanged"
																	AutoPostBack="true"
																	Visible="false" />
															</dd>
															<dl id="dlUniformInvoiceOption1_8" Runat="Server" visible="false">
																<dd>統一編号</dd>
																<dd>
																	<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Width="220" Runat="Server" MaxLength="8" />
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvUniformInvoiceOption1_8"
																			Runat="Server"
																			ControlToValidate="tbUniformInvoiceOption1_8"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			EnableClientScript="false"
																			CssClass="error_inline" />
																	</p>
																	<asp:Label ID="lbUniformInvoiceOption1_8" Runat="Server" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Visible="false" />
																</dd>
																<dd>会社名</dd>
																<dd>
																	<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption2 %>" Width="220" Runat="Server" MaxLength="20" />
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvUniformInvoiceOption2"
																			Runat="Server"
																			ControlToValidate="tbUniformInvoiceOption2"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			EnableClientScript="false"
																			CssClass="error_inline" />
																	</p>
																	<asp:Label ID="lbtbUniformInvoiceOption2" Runat="Server" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption2 %>" Visible="false" />
																</dd>
															</dl>
															<dl id="dlUniformInvoiceOption1_3" Runat="Server" visible="false">
																<dd>寄付先コード</dd>
																<dd>
																	<asp:TextBox ID="tbUniformInvoiceOption1_3" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Width="220" Runat="Server" MaxLength="7" />
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvUniformInvoiceOption1_3"
																			Runat="Server"
																			ControlToValidate="tbUniformInvoiceOption1_3"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			EnableClientScript="false"
																			CssClass="error_inline" />
																	</p>
																	<asp:Label ID="lbUniformInvoiceOption1_3" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Runat="Server" Visible="false" />
																</dd>
															</dl>
															<dl id="dlUniformInvoiceTypeRegist" Runat="Server" visible="false">
																<dd>
																	<asp:CheckBox ID="cbSaveToUserInvoice" Checked="<%# GetCartShipping(Container.DataItem).UserInvoiceRegistFlg %>" Runat="Server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
																</dd>
																<dd id="dlUniformInvoiceTypeRegistInput" Runat="Server" visible="false">
																	電子発票情報名 <span class="require">※</span><br />
																	<asp:TextBox ID="tbUniformInvoiceTypeName" MaxLength="30" Runat="Server" />
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvUniformInvoiceTypeName"
																			Runat="Server"
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
														<div id="divInvoiceCarryType" Runat="Server">
															<dt>共通性載具</dt>
															<dd>
																<asp:DropDownList
																	ID="ddlInvoiceCarryType"
																	Runat="Server"
																	CssClass="input_border"
																	DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE) %>"
																	DataTextField="text"
																	DataValueField="value"
																	OnSelectedIndexChanged="ddlInvoiceCarryType_SelectedIndexChanged"
																	AutoPostBack="true" />
																<asp:DropDownList
																	ID="ddlInvoiceCarryTypeOption"
																	Runat="Server"
																	CssClass="input_border"
																	DataTextField="text"
																	DataValueField="value"
																	OnSelectedIndexChanged="ddlInvoiceCarryTypeOption_SelectedIndexChanged"
																	AutoPostBack="true"
																	Visible="false" />
															</dd>
															<dd id="divCarryTypeOption" runat ="server">
																<div id="divCarryTypeOption_8" Runat="Server" visible="false">
																	<asp:TextBox ID="tbCarryTypeOption_8" Width="220" Runat="Server" Text="<%#: GetCartShipping(Container.DataItem).CarryTypeOptionValue %>" placeholder="例:/AB201+9(限8個字)" MaxLength="8" />
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvCarryTypeOption_8"
																			Runat="Server"
																			ControlToValidate="tbCarryTypeOption_8"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			EnableClientScript="false"
																			CssClass="error_inline" />
																	</p>
																</div>
																<div id="divCarryTypeOption_16" Runat="Server" visible="false">
																	<asp:TextBox ID="tbCarryTypeOption_16" Width="220" Text="<%#: GetCartShipping(Container.DataItem).CarryTypeOptionValue %>" Runat="Server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
																	<p class="attention">
																	<asp:CustomValidator
																		ID="cvCarryTypeOption_16"
																		Runat="Server"
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
															<dl id="dlCarryTypeOptionRegist" Runat="Server" visible="false">
																<dd>
																	<asp:CheckBox ID="cbCarryTypeOptionRegist" Runat="Server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
																</dd>
																<dd id="divCarryTypeOptionName" Runat="Server" visible="false">
																	電子発票情報名 <span class="require">※</span>
																	<asp:TextBox ID="tbCarryTypeOptionName" Text="<%#: GetCartShipping(Container.DataItem).InvoiceName %>" Runat="Server" MaxLength="30" />
																	<p class="attention">
																		<asp:CustomValidator
																			ID="cvCarryTypeOptionName"
																			Runat="Server"
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
																<asp:Label Runat="Server" ID="lbCarryTypeOption" Visible="false" />
															</dd>
														</div>
													</dl>
												</span>
											</div>
											<%-- ▼定期購入配送パターン▼ --%>
											<div class="fixed" visible="<%# DisplayFixedPurchaseShipping(Container) %>" Runat="Server"><span id="efo_sign_fixed_purchase"></span>
												<h2 visible="<%# DisplayFixedPurchaseShipping(Container) %>" Runat="Server">定期購入 配送パターンの指定</h2>
												<div id='<%# "efo_sign_fixed_purchase" + Container.ItemIndex %>'></div>
												<%-- ▽デフォルトチェックの設定▽--%>
												<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
												<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container.Parent.Parent).ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container.Parent.Parent).ItemIndex, 2, 3, 1, 4) %>
												<%-- △ - - - - - - - - - - - △--%>
												<dl class="order-form" visible="<%# DisplayFixedPurchaseShipping(Container) %>" Runat="Server">
													<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" Runat="Server">
														<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date"
															Text="月間隔日付指定"
															Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 1) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true"
															Runat="Server" /><span id='<%# "efo_sign_fixed_purchase_month" + Container.ItemIndex %>'></span>
														<div id="ddFixedPurchaseMonthlyPurchase_Date" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" Runat="Server">
															<asp:DropDownList ID="ddlFixedPurchaseMonth"
																DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true) %>"
																DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" Runat="Server" />
																ヶ月ごと
															<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
																DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true,false,true) %>"
																DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" Runat="Server" />
																日に届ける
														</div>
														<p class="attention">
															<asp:CustomValidator
																ID="cvFixedPurchaseMonth"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseMonth"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
															<asp:CustomValidator
																ID="cvFixedPurchaseMonthlyDate"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseMonthlyDate"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
														</p>
													</dd>
													<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" Runat="Server">
														<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay"
															Text="月間隔・週・曜日指定"
															Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 2) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true" Runat="Server" /><span id='<%# "efo_sign_fixed_purchase_week_and_day" + Container.ItemIndex %>'></span>
														<div id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" Runat="Server">
															<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
																DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true, true) %>"
																DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" Runat="Server" />
															ヶ月ごと
															<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
																DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
																DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" Runat="Server" />
															<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
																DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
																DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" Runat="Server" />
																に届ける
														</div>
														<p class="attention">
															<asp:CustomValidator
																ID="cvFixedPurchaseIntervalMonths"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseIntervalMonths"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
															<asp:CustomValidator
																ID="cvFixedPurchaseWeekOfMonth"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseWeekOfMonth"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
															<asp:CustomValidator
																ID="cvFixedPurchaseDayOfWeek"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseDayOfWeek"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
														</p>
													</dd>
													<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" Runat="Server">
														<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays"
															Text="配送日間隔指定"
															Checked="<%# (GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 3) && (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? (GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false).Length > 0) : (GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false).Length > 1))) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true"
															Runat="Server" /><span id='<%# "efo_sign_fixed_purchase_interval_days" + Container.ItemIndex %>'></span>
														<div id="ddFixedPurchaseRegularPurchase_IntervalDays" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" Runat="Server">
															<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
																DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false) %>'
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																Runat="Server" />
																日ごとに届ける
														</div>
														<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%#: this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex].FixedPurchaseShippingDaysRequired %>" Runat="Server" />
														<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%#: this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex].FixedPurchaseMinimumShippingSpan %>" Runat="Server" />
														<p class="attention">
															<asp:CustomValidator
																ID="cvFixedPurchaseIntervalDays"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseIntervalDays"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
														</p>
													</dd>
													<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" Runat="Server">
														<asp:RadioButton
															ID="rbFixedPurchaseEveryNWeek"
															Text="週間隔・曜日指定"
															Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 4) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true"
															Runat="Server" /><span id="<%# "efo_sign_fixed_purchase_week" + Container.ItemIndex %>"></span>
														<div id="ddFixedPurchaseEveryNWeek" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" Runat="Server">
															<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
																DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true) %>"
																DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
																Runat="Server" />
															週間ごと
															<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
																DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false) %>"
																DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
																Runat="Server" />
															に届ける
														</div>
														<p class="attention">
															<asp:CustomValidator
																ID="cvFixedPurchaseEveryNWeek"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
															<asp:CustomValidator
																ID="cvFixedPurchaseEveryNWeekDayOfWeek"
																Runat="Server"
																ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true" />
														</p>
													</dd>
												</dl>
												<small><p class="attention" Runat="Server" visible="<%# GetAllFixedPurchaseKbnEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex) == false %>">同時に定期購入できない商品が含まれております。</p></small>
												<dl class="order-form">
													<dt id="dtFirstShippingDate" visible="true" Runat="Server">初回配送予定日</dt>
													<dd visible="true" Runat="Server">
														<asp:Label ID="lblFirstShippingDate" Runat="Server" />
														<asp:Label ID="lblFirstShippingDateNoteMessage" visible="false" Runat="Server">
															<br>配送予定日は変更となる可能性がありますことをご了承ください。
														</asp:Label>
														<asp:Literal ID="lFirstShippingDateDayOfWeekNoteMessage" visible="false" Runat="Server">
															<br>曜日指定は次回配送日より適用されます。
														</asp:Literal>
													</dd>
													<dt id="dtNextShippingDate" visible="true" Runat="Server">2回目の配送日を選択</dt>
													<dd visible="true" Runat="Server">
														<asp:Label ID="lblNextShippingDate" visible="false" Runat="Server" />&nbsp;
														<asp:DropDownList ID="ddlNextShippingDate" visible="false" OnDataBound="ddlNextShippingDate_OnDataBound" Runat="Server" />
													</dd>
												</dl>
												<dl>
													メール便の場合は数日ずれる可能性があります。
												</dl>
												<p class="attention"><span ID="sErrorMessage" Runat="Server"></span></p>
											</div>
											<%-- ▲定期購入配送パターン▲ --%>
										</div>
								</ItemTemplate>
							</asp:Repeater>
							<div class="cart-footer" Runat="Server" Visible='<%# CanAddShipping(GetCartObject(Container.DataItem)) %>'>
								<div class="button-next">
									<asp:LinkButton ID="lbAddShipping"
										Runat="Server"
										Visible='<%# CanAddShipping(GetCartObject(Container.DataItem)) %>'
										OnClick="lbAddShipping_Click"
										class="btn btn-mid btn-inverse add-shipping-button"
										Text="新しい配送先を追加" />
								</div>
							</div>
							<dl class="order-form memo">
								<%-- 注文メモ --%>
								<dt>注文メモ</dt>
								<dd>
									<asp:Repeater ID="rMemos" Runat="Server" DataSource="<%# GetCartObject(Container.DataItem).OrderMemos %>" Visible="<%# GetCartObject(Container.DataItem).OrderMemos.Count != 0 %>">
										<HeaderTemplate>
											<dl>
										</HeaderTemplate>
										<ItemTemplate>
											<dt><%#: Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME) %></dt>
											<dd>
												<p class="attention"><span id="sErrorMessageMemo" Runat="Server"></span></p>
												<w2c:ExtendedTextBox ID="tbMemo" Runat="Server" Text="<%#: Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%#: Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine" />
												<div>
													<%-- IDに"OtherValidator"を含めることで案件毎に追加したtextareaなどでチェック可能 --%>
													<asp:CustomValidator ID="OtherValidator" Runat="Server"
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
									Checked="<%# GetCartObject(Container.DataItem).ReflectMemoToFixedPurchase %>"
									visible="<%# (GetCartObject(Container.DataItem).OrderMemos.Count != 0) && GetCartObject(Container.DataItem).ReflectMemoToFixedPurchaseVisible %>"
									Text="2回目以降の注文メモにも追加する"
									CssClass="checkBox"
									Runat="Server" />
							</dl>
							<div style=" <%# IsDisplayCartDetail(Container.DataItem) ? string.Empty: "display: none;" %>">
								<h2>カート番号<%# Container.ItemIndex + 1 %><%#: DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）") %>のご注文内容</h2>
								<div class="userProduct" style="border: none;">
									<asp:Repeater ID="rCartProduct" Runat="Server">
										<HeaderTemplate>
											<table class="cart-table">
											<tbody>
										</HeaderTemplate>
										<ItemTemplate>
											<tr runat="server" visible='<%# (FindCart(Container.DataItem).IsGift && (Container.ItemIndex == 0)) %>'>
												<td colspan="3">
													※「一括変更」ボタンで、各配送先の該当商品の個数を指定した数に変更できます。
												</td>
											</tr>
											<tr class="cart-unit-product" Runat="Server">
												<td class="product-image">
													<a href='<%#: GetCartProduct(Container.DataItem).CreateProductDetailUrl() %>' Runat="Server" Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
														<w2c:ProductImage ProductMaster="<%# (GetCartProduct(Container.DataItem)) %>"
															ImageSize="M"
															Runat="Server" />
													</a>
													<w2c:ProductImage ProductMaster="<%# (GetCartProduct(Container.DataItem)) %>"
														ImageSize="M"
														Runat="Server"
														Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false %>" />
												</td>
												<td class="product-info">
													<ul>
														<li class="product-name">
															<a href='<%#: GetCartProduct(Container.DataItem).CreateProductDetailUrl() %>' Runat="Server" Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																<%#: GetCartProduct(Container.DataItem).ProductJointName %>
															</a>
															<%#: GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false ? GetCartProduct(Container.DataItem).ProductJointName : "" %>
															<%#: string.IsNullOrEmpty((GetCartProduct(Container.DataItem)).GetProductTag("tag_cart_product_message")) == false ? "<p class=\"product-msg\">" + GetCartProduct(Container.DataItem).GetProductTag("tag_cart_product_message") + "</p>" : "" %>
														</li>
														<li class="product-price" Visible="<%# GetCartProduct(Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" Runat="Server">
															<%#: CurrencyManager.ToPrice((GetCartProduct(Container.DataItem)).Price) %> (<%#: this.ProductPriceTextPrefix %>)
														</li>
														<li visible='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' Runat="Server">
															<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList %>' Runat="Server">
																<ItemTemplate>
																	<%#: ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																	<%# string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false ? "<br />" : string.Empty %>
																</ItemTemplate>
															</asp:Repeater>
														</li>
													</ul>
												</td>
												<td class="product-control" style="max-width: 125px; min-width: 125px; background: none;">
													<div class="amout">
														<span Runat="Server" style="vertical-align:text-bottom;">
															<p><%#: StringUtility.ToNumeric((GetCartProduct(Container.DataItem)).Count) %></p>
														</span>
													</div>
												</td>
											</tr>
											<tr>
												<td colspan="3">
													<span Runat="Server" style="margin: 5px 0; float: left;" visible="<%# FindCart(Container.DataItem).IsGift %>">
														<asp:LinkButton
															ID="lbProductCountBulkChange"
															runat="server"
															CssClass="btn-inverse bulkChangeQuantity"
															Text="一括変更"
															OnClick="lbProductCountBulkChange_OnClick" />
														<asp:LinkButton Runat="Server"
															Text="+"
															CssClass="changeQuantity"
															OnClick="lbCartSummaryProductCountChange_OnClick"
															Style="text-decoration: none"
															CommandArgument="plus" />
														<asp:TextBox
															ID="tbCartSummaryProductCount"
															Runat="Server"
															CssClass="user-product-quantity"
															MaxLength="3"
															onKeyPress="return isNumberKey(event)"
															AutoPostBack="False"
															Text="0" />
														<asp:LinkButton Runat="Server"
															Text="-"
															Enabled="<%# GetCartProduct(Container.DataItem).Count > 0 %>"
															CssClass="changeQuantity"
															OnClick="lbCartSummaryProductCountChange_OnClick"
															Style="text-decoration: none"
															CommandArgument="subtract" />
													</span>
												</td>
											</tr>
										</ItemTemplate>
										<FooterTemplate>
											</tbody>
											</table>
										</FooterTemplate>
									</asp:Repeater>
									<small id="hcErrorMessage" enableviewstate="false" class="fred" Runat="Server" />
								</div>
								<asp:Repeater ID="rCartDetail" Runat="Server">
									<ItemTemplate>
										<div class="cart-unit">
											<div class="cart-unit-footer">
												<%-- 総配送先数 --%>
												<dl>
													<dt>総配送先数</dt>
													<dd><%#: GetCartObject(Container.DataItem).Shippings.Count %></dd>
												</dl>
												<%-- 小計 --%>
												<dl>
													<dt>小計（<%#: this.ProductPriceTextPrefix %>）</dt>
													<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotal) %></dd>
												</dl>
												<% if (this.ProductIncludedTaxFlg == false) { %>
												<dl>
													<dt>消費税額</dt>
													<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotalTax) %></dd>
												</dl>
												<% } %>
												<%-- セットプロモーション(商品割引) --%>
												<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" Visible="<%# GetCartObject(Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" Runat="Server">
												<HeaderTemplate>
												</HeaderTemplate>
												<ItemTemplate>
												<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" Runat="Server">
													<dt>
														<%#: ((CartSetPromotion)Container.DataItem).SetpromotionDispName %>
													</dt>
													<dd>
														<%# ((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %>
													</dd>
												</dl>
												</ItemTemplate>
												<FooterTemplate>
												</FooterTemplate>
												</asp:Repeater>

												<% if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn) { %>
												<dl>
													<dt>会員ランク割引額</dt>
													<dd>
														<%# GetCartObject(Container.DataItem).MemberRankDiscount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).MemberRankDiscount * (GetCartObject(Container.DataItem).MemberRankDiscount < 0 ? -1 : 1)) %>
													</dd>
												</dl>
												<% } %>

												<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn) { %>
												<dl>
													<dt>定期会員割引額</dt>
													<dd>
														<%# GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount * (GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount < 0 ? -1 : 1)) %>
													</dd>
												</dl>
												<% } %>

												<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
												<div Runat="Server" visible="<%# GetCartObject(Container.DataItem).HasFixedPurchase %>">
													<dl>
														<dt>定期購入割引額</dt>
														<dd>
															<%#: GetCartObject(Container.DataItem).FixedPurchaseDiscount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).FixedPurchaseDiscount * (GetCartObject(Container.DataItem).FixedPurchaseDiscount < 0 ? -1 : 1)) %>
														</dd>
													</dl>
												</div>
												<% } %>

												<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
													<dl>
														<dt>クーポン割引額</dt>
														<dd>
															<%#: GetCouponName(GetCartObject(Container.DataItem)) %>
															<%# GetCartObject(Container.DataItem).UseCouponPrice > 0 ? "-" : string.Empty %>
															<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).UseCouponPrice * (GetCartObject(Container.DataItem).UseCouponPrice < 0 ? -1 : 1)) %>
														</dd>
													</dl>
												<% } %>

												<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
												<dl>
													<dt>ポイント利用額</dt>
													<dd>
														<%# GetCartObject(Container.DataItem).UsePointPrice > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).UsePointPrice * (GetCartObject(Container.DataItem).UsePointPrice < 0 ? -1 : 1)) %>
													</dd>
												</dl>
												<% } %>
												<%-- 配送料金 --%>
												<dl>
													<dt>配送料金</dt>
													<dd Runat="Server" style='<%# GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg ? "display:none;" : string.Empty %>'>
														<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceShipping) %>
													</dd>
													<dd Runat="Server" style='<%# GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg == false ? "display:none;" : string.Empty %>'>
														<%#: GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateMessage %>
													</dd>
												</dl>
												<%-- 決済手数料 --%>
												<span Runat="Server" Visible="<%# GetCartObject(Container.DataItem).Payment != null %>">
													<dl>
													<dt>決済手数料</dt>
													<dd><%#: GetCartObject(Container.DataItem).Payment != null ? CurrencyManager.ToPrice(GetCartObject(Container.DataItem).Payment.PriceExchange) : string.Empty %></dd>
												</dl>
												</span>

												<%-- セットプロモーション(配送料割引) --%>
												<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" Runat="Server">
												<HeaderTemplate>
												</HeaderTemplate>
												<ItemTemplate>
												<dl visible='<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>' Runat="Server">
												<dt>
													<%#: ((CartSetPromotion)Container.DataItem).SetpromotionDispName %>(送料割引)
												</dt>
												<dd>
													<%# ((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %>
												</dd>
												</dl>
												</ItemTemplate>
												<FooterTemplate>
												</FooterTemplate>
												</asp:Repeater>
												<dl>
													<dt>合計(税込)</dt>
													<dd>
														<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceTotal) %>
													</dd>
												</dl>
											</div>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
					</div>
				</ItemTemplate>
			</asp:Repeater>
		<div class="cart-footer">
			<div class="button-next">
				<a href="<%: this.NextEvent %>" class="btn"><%: this.IsNextConfirmPage ? "ご注文内容確認へ" : "お支払方法入力へ" %></a>
			</div>
			<div class="button-prev">
				<a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST %>" class="btn">戻る</a>
			</div>
		</div>
		<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
		<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" Runat="Server" />
		<asp:HiddenField ID="hfResetAuthenticationCode" Runat="Server" />
		<% } %>
		</ContentTemplate>
		<Triggers>
			<asp:PostBackTrigger ControlID="rCartList"/>
		</Triggers>
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
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		bindTwAddressSearch();
		<% } %>
		initCartInfoToggle($('#<%= this.WhfOpenCartIndex.ClientID %>').val());
		<% var serializer = new JavaScriptSerializer(); %>
		<% if (this.IsEfoOptionEnabled) { %>
		var customValidatorControlDisabledInformationList = <%: serializer.Serialize(this.CustomValidatorControlDisabledInformationList) %>
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

		<% foreach (RepeaterItem riShipping in ((Repeater)ri.FindControl("rCartShippings")).Items) { %>
		execAutoKanaWithKanaType(
			$('#<%= ((TextBox)riShipping.FindControl("tbSenderName1")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbSenderNameKana1")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbSenderName2")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbSenderNameKana2")).ClientID %>'));
		execAutoKanaWithKanaType(
			$('#<%= ((TextBox)riShipping.FindControl("tbShippingName1")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbShippingNameKana1")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbShippingName2")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbShippingNameKana2")).ClientID %>'));
		<% } %>
		<% } %>
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		execAutoChangeKanaWithKanaType(
			$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana1")).ClientID %>'),
			$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana2")).ClientID %>'));

		<% foreach (RepeaterItem riShipping in ((Repeater)ri.FindControl("rCartShippings")).Items) { %>
		execAutoChangeKanaWithKanaType(
			$('#<%= ((TextBox)riShipping.FindControl("tbSenderNameKana1")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbSenderNameKana2")).ClientID %>'));
		execAutoChangeKanaWithKanaType(
			$('#<%= ((TextBox)riShipping.FindControl("tbShippingNameKana1")).ClientID %>'),
			$('#<%= ((TextBox)riShipping.FindControl("tbShippingNameKana2")).ClientID %>'));
		<% } %>
		<% } %>
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

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search owner zip global
		textboxChangeSearchGlobalZip(
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZipGlobal").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrOwnerFromZipGlobal").UniqueID %>');
		<% } %>

		<% foreach (RepeaterItem riShipping in ((Repeater)ri.FindControl("rCartShippings")).Items) { %>
		// Check sender zip code input on click
		clickSearchZipCodeInRepeater(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip2").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchSenderAddr").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchSenderAddr").UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (riShipping.FindControl("sSenderZipError")).ClientID %>',
			"sender");

		// Check sender zip code input on text box change
		textboxChangeSearchZipCodeInRepeater(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip2").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip1").UniqueID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZip2").UniqueID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchSenderAddr").ClientID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (riShipping.FindControl("sSenderZipError")).ClientID %>',
			"sender");

		// Check shipping zip code input on click
		clickSearchZipCodeInRepeaterForSp(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip2").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchShippingAddr").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchShippingAddr").UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (riShipping.FindControl("sShippingZipError")).ClientID %>',
			"shipping",
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>');

		// Check shipping zip code input on text box change
		textboxChangeSearchZipCodeInRepeaterForSp(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip2").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip1").UniqueID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip2").UniqueID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchShippingAddr").ClientID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (riShipping.FindControl("sShippingZipError")).ClientID %>',
			"shipping");

		if (multiAddrsearchTriggerType == "shipping")
		{
			bindTargetForAddr1 = "<%= ((DropDownList)riShipping.FindControl("ddlShippingAddr1")).ClientID %>";
			bindTargetForAddr2 = "<%= ((TextBox)riShipping.FindControl("tbShippingAddr2")).ClientID %>";
			bindTargetForAddr3 = "<%= ((TextBox)riShipping.FindControl("tbShippingAddr3")).ClientID %>";
		}
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
			<% } %>
		} else if (multiAddrsearchTriggerType == "shipping") {
			<% foreach (RepeaterItem ri in rCartList.Items) { %>
			$('#' + bindTargetForAddr1).val(selectedAddr.find('.addr').text());
			$('#' + bindTargetForAddr2).val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
			$('#' + bindTargetForAddr3).focus();
			<% } %>
		}

		closePopupAndLayer();
	}

	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
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
			'<%: GetCorrectSexForDataBindDefault() %>',
			<%= this.WddlOwnerCountry.SelectedIndex %>,
			'<%= this.WddlOwnerAddr1.ClientID %>',
			<%= this.WddlOwnerAddr1.SelectedIndex %>
		);
	});

	// Initialize cart info toggle
	function initCartInfoToggle(openIndex) {
		var cartBoxs = $(".cartBox");
		cartBoxs.each(function () {
			var toggleBlocks = $(this).find(".toggleBlock");
			toggleBlocks.each(function (index, element) {
				if (index != openIndex) {
					var toggleButton = $(element.closest(".orderBoxLarge")).find("#toggleIcon");
					if (toggleButton.length != 0) {
						toggleButton[0].innerHTML = "▶";
						$(this).css("display", "none");
					}
				} else {
					var toggleButton = $(element.closest(".orderBoxLarge")).find("#toggleIcon");
					if (toggleButton.length != 0) {
						toggleButton[0].innerHTML = "▼";
						$(this).css("display", "");
					}
				}
			});
		});

		$('#<%= this.WhfOpenCartIndex.ClientID %>').val(openIndex);
	}
//-->
</script>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
</asp:Content>
