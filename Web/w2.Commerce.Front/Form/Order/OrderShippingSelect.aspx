<%--
=========================================================================================================
  Module      : 注文配送選択画面(OrderShippingSelect.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Page Title="注文配送選択ページ" Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderShippingSelect.aspx.cs" Inherits="Form_Order_OrderShippingSelect" MaintainScrollPositionOnPostback="true" %>
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
<style>
	.orderBox {
		border-bottom: #ADC3D0 solid 1px;
	}

	.orderBoxLarge {
		border: none;
	}

	.orderBoxLarge h4 {
		padding: 8px 11px;
		margin-top: 6px;
		margin-bottom: 6px;
	}

	.changeQuantity {
		min-width:20px;
		min-height:20px;
		background:#f2f2f2;
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
		margin: 0 5px;
		padding: 4px 15px 4px;
		border-radius: 5px;
		background-color: #000;
		color: #fff !important;
		font-size: 13px;
		vertical-align: middle;
	}

	.user-product-quantity {
		height: 18px;
		width: 30px;
		padding: 0;
		text-align: center;
		font-size: 12px;
		border: 1px solid #ddd;
		border-radius: 4px;
		display: inline-block;
		vertical-align: middle;
		float: right;
	}

	.cartInfoToggle {
		margin-left: 10px;
		max-width: 100px;
		margin-right: 18px;
	}

	.add-shipping-button {
		margin: 0 0 5px 21px;
	}

	.sender-selector {
		margin-left: 17px;
	}

	.priceList dl {
		width: 100%;
	}
</style>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<p id="CartFlow"><img src="../../Contents/ImagesPkg/order/cart_step01.gif" alt="お届け先情報入力" width="781" height="58" /></p>
<div class="btmbtn above cartstep">
	<h2 class="ttlA">お届け先情報入力</h2>
	<ul>
		<li><a href="<%: this.NextEvent %>" class="btn btn-success"><%: this.IsNextConfirmPage ? "ご注文内容確認へ" : "お支払方法入力へ" %></a></li>
	</ul>
</div>

<%-- エラーメッセージ（デフォルト注文方法用） --%>
<span style="color:red;text-align:center;display:block;"><asp:Literal ID="lInvalidGiftCartErrorMessage" Runat="Server" /></span>
<span style="color:red;text-align:center;display:block;"><asp:Literal ID="lOrderErrorMessage" Runat="Server" /></span>
<div id="CartList">
	<div class="main clearFix" style="margin-bottom:0;">
		<div class="submain">
			<div class="column">
				<h2><img src="../../Contents/ImagesPkg/order/sttl_user.gif" alt="注文者情報" width="80" height="16" /></h2>
				<% if (this.IsEasyUser) { %>
				<p style="margin: 5px; padding: 5px; text-align: center; background-color: #ffff80; border: 1px solid #D4440D; border-color:#E5A500; color: #CC7600;">ご購入手続きに必要な会員情報が不足しています。</p>
				<% } %>
				<p>以下の項目をご入力ください。<br />
				<% if (this.IsLoggedIn) { %>
				（入力した注文者情報で会員情報が更新されます。）<br />
				<% } %></p>
				<span class="fred">※</span>&nbsp;は必須入力です。<br />
			</div><!--column-->
			<div class="columnRight">
				<h2><img src="../../Contents/ImagesPkg/order/sttl_esd.gif" alt="配送先情報" width="80" height="16" /></h2>
				<p>「注文者情報」で入力した住所宛にお届けする場合は、以下の「注文者情報の住所へ配送する」を選択してください。<br /><span class="fred">※</span>&nbsp;は必須入力です。</p>
			</div><!--columnRight-->
			<br class="clr" />
		</div><!--submain-->
	</div><!--main-->

	<%-- 次へイベント用リンクボタン --%>
	<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" Runat="Server" />
	<%-- 戻るイベント用リンクボタン --%>
	<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" Runat="Server" />

	<%-- UPDATE PANEL開始 --%>
	<asp:UpdatePanel ID="upUpdatePanel" Runat="Server">
		<ContentTemplate>
			<asp:HiddenField ID="hfOpenCartIndex" Runat="server" Value="0" />
			<small id="hcErrorMessage" enableviewstate="false" class="fred" Runat="Server"></small>
			<% this.CartItemIndexTmp = -1; %>
			<div class="main" style="margin-top: 0;">
				<div class="submain">
					<asp:Repeater ID="rCartList" Runat="Server">
						<ItemTemplate>
							<%-- ▼注文者情報▼ --%>
							<div id="divOwnerColumn" class="column" visible='<%# Container.ItemIndex == 0 %>' Runat="Server">
								<%
									this.CartItemIndexTmp++;
									var ownerAddrCountryIsoCode = GetOwnerAddrCountryIsoCode(this.CartItemIndexTmp);
									var isOwnerAddrCountryJp = IsCountryJp(ownerAddrCountryIsoCode);
									var isOwnerAddrCountryUs = IsCountryUs(ownerAddrCountryIsoCode);
									var isOwnerAddrCountryTw = IsCountryTw(ownerAddrCountryIsoCode);
									var isOwnerAddrZipNecessary = IsAddrZipcodeNecessary(ownerAddrCountryIsoCode);
								%>
								<div class="userBox">
									<div class="top">
										<div class="bottom">
											<dl>
												<%-- 注文者：氏名 --%>
												<dt>
													<%: ReplaceTag("@@User.name.name@@", ownerAddrCountryIsoCode) %>
													&nbsp;<span class="fred">※</span><span id="efo_sign_name"></span>
												</dt>
												<dd>
													姓&nbsp;&nbsp;<asp:TextBox ID="tbOwnerName1" Text="<%#: this.CartList.Owner.Name1 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' Runat="Server" />&nbsp;&nbsp;
													名&nbsp;&nbsp;<asp:TextBox ID="tbOwnerName2" Text="<%#: this.CartList.Owner.Name2 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' Runat="Server" /><br />
													<small>
														<asp:CustomValidator
															ID="cvOwnerName1"
															Runat="Server"
															ControlToValidate="tbOwnerName1"
															ValidationGroup="OrderShipping"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															ClientValidationFunction="ClientValidate"
															CssClass="error_inline" />
														<asp:CustomValidator
															ID="cvOwnerName2"
															Runat="Server"
															ControlToValidate="tbOwnerName2"
															ValidationGroup="OrderShipping"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															ClientValidationFunction="ClientValidate"
															CssClass="error_inline" />
													</small>
												</dd>
												<%-- 注文者：氏名（かな） --%>
												<% if (isOwnerAddrCountryJp) { %>
												<dt>
													<%: ReplaceTag("@@User.name_kana.name@@", ownerAddrCountryIsoCode) %>
													&nbsp;<span class="fred">※</span><span id="efo_sign_kana"></span>
												</dt>
												<dd class="<%: ReplaceTag("@@User.name_kana.type@@") %>">
													姓&nbsp;&nbsp;<asp:TextBox ID="tbOwnerNameKana1" Text="<%#: this.CartList.Owner.NameKana1 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' Runat="Server" />&nbsp;&nbsp;
													名&nbsp;&nbsp;<asp:TextBox ID="tbOwnerNameKana2" Text="<%#: this.CartList.Owner.NameKana2 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' Runat="Server" /><br />
													<small>
														<asp:CustomValidator
															ID="cvOwnerNameKana1"
															Runat="Server"
															ControlToValidate="tbOwnerNameKana1"
															ValidationGroup="OrderShipping"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															ClientValidationFunction="ClientValidate"
															CssClass="error_inline" />
														<asp:CustomValidator
															ID="cvOwnerNameKana2"
															Runat="Server"
															ControlToValidate="tbOwnerNameKana2"
															ValidationGroup="OrderShipping"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															ClientValidationFunction="ClientValidate"
															CssClass="error_inline" />
													</small>
												</dd>
											<% } %>
											<%-- 注文者：生年月日 --%>
											<dt>
												<%: ReplaceTag("@@User.birth.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<% if (this.IsLoggedIn) { %><span class="fred">※</span><span id="efo_sign_birth"></span><% } %>
											</dt>
											<dd>
												<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue='<%#: this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.BirthYear : "" %>' CssClass="input_border" Runat="Server" />&nbsp;&nbsp;年&nbsp;&nbsp;
												<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue='<%#: this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Month.ToString() : "" %>' CssClass="input_widthA input_border" Runat="Server" />&nbsp;&nbsp;月&nbsp;&nbsp;
												<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue='<%#: this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Day.ToString() : "" %>' CssClass="input_widthA input_border" Runat="Server" />&nbsp;&nbsp;日
												<small>
													<asp:CustomValidator
														ID="cvOwnerBirth"
														Runat="Server"
														ControlToValidate="ddlOwnerBirthDay"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														EnableClientScript="false"
														CssClass="error_inline" />
												</small>
											</dd>
											<%-- 注文者：性別 --%>
											<dt>
												<%: ReplaceTag("@@User.sex.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<% if (this.IsLoggedIn) { %><span class="fred">※</span><% } %>
											</dt>
											<dd class="input_align">
												<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%#: GetCorrectSexForDataBind(this.CartList.Owner.Sex) %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" CellSpacing="5" RepeatLayout="Flow" CssClass="input_radio" Runat="Server" />
												<small>
													<asp:CustomValidator
														ID="cvOwnerSex"
														Runat="Server"
														ControlToValidate="rblOwnerSex"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														EnableClientScript="false"
														CssClass="error_inline" />
												</small>
											</dd>
											<%-- 注文者：PCメールアドレス --%>
											<dt>
												<%: ReplaceTag("@@User.mail_addr.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<span class="fred">※</span><span id="efo_sign_mail_addr"></span>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerMailAddr" Text="<%#: this.CartList.Owner.MailAddr %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" Runat="Server" Type="email" /><br />
												<small>
													<asp:CustomValidator
														ID="cvOwnerMailAddr"
														Runat="Server"
														ControlToValidate="tbOwnerMailAddr"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
													<asp:CustomValidator
														ID="cvOwnerMailAddrForCheck"
														Runat="Server"
														ControlToValidate="tbOwnerMailAddr"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														CssClass="error_inline" />
												</small>
											</dd>
											<%-- 注文者：PCメールアドレス（確認用） --%>
											<dt>
												<%: ReplaceTag("@@User.mail_addr.name@@", ownerAddrCountryIsoCode) %>（確認用）
												&nbsp;<span class="fred">※</span><span id="efo_sign_mail_addr_conf"></span>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerMailAddrConf" Text="<%#: this.CartList.Owner.MailAddr %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" Runat="Server" Type="email" /><br />
												<small>
													<asp:CustomValidator Runat="Server"
														ID="cvOwnerMailAddrConf"
														ControlToValidate="tbOwnerMailAddrConf"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
											<%-- 注文者：モバイルメールアドレス --%>
											<dt>
												<%: ReplaceTag("@@User.mail_addr2.name@@", ownerAddrCountryIsoCode) %>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerMailAddr2" Text="<%#: this.CartList.Owner.MailAddr2 %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" Runat="Server" Type="email" /><br />
												<small>
												<asp:CustomValidator Runat="Server"
													ID="cvOwnerMailAddr2"
													ControlToValidate="tbOwnerMailAddr2"
													ValidationGroup="OrderShipping"
													ValidateEmptyText="true"
													SetFocusOnError="true"
													ClientValidationFunction="ClientValidate"
													CssClass="error_inline" />
												</small>
											</dd>
											<%-- 注文者：モバイルメールアドレス（確認用） --%>
											<dt>
												<%: ReplaceTag("@@User.mail_addr2.name@@", ownerAddrCountryIsoCode) %>（確認用）
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerMailAddr2Conf" Text="<%#: this.CartList.Owner.MailAddr2 %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" Runat="Server" Type="email" /><br />
												<small>
												<asp:CustomValidator Runat="Server"
													ID="cvOwnerMailAddr2Conf"
													ControlToValidate="tbOwnerMailAddr2Conf"
													ValidationGroup="OrderShipping"
													ValidateEmptyText="true"
													SetFocusOnError="true"
													ClientValidationFunction="ClientValidate"
													CssClass="error_inline" />
												</small>
											</dd>
											<% } %>
											<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
											<%-- 注文者：国 --%>
											<dt>
												<%: ReplaceTag("@@User.country.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<span class="fred">※</span>
											</dt>
											<dd>
												<asp:DropDownList ID="ddlOwnerCountry" Runat="Server" AutoPostBack="true" SelectedValue="<%#: this.CartList.Owner.AddrCountryIsoCode %>" DataSource="<%# this.UserCountryDisplayList %>" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerCountry_SelectedIndexChanged" />
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
												<span id="countryAlertMessage" class="notes" Runat="Server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
											</dd>
											<% } %>
											<%-- 注文者：郵便番号 --%>
											<% if (isOwnerAddrCountryJp) { %>
											<dt>
												<%: ReplaceTag("@@User.zip.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<span class="fred">※</span><span id="efo_sign_zip"></span>
											</dt>
											<dd>
												<p class="pdg_topC">
													<asp:TextBox ID="tbOwnerZip" OnTextChanged="lbSearchOwnergAddr_Click" Text="<%#: this.CartList.Owner.Zip %>" CssClass="input_widthC input_border" Type="tel" MaxLength="8" Runat="Server" />
												</p>
												<span class="btn_add_sea"><asp:LinkButton ID="lbSearchOwnergAddr" Runat="Server" onclick="lbSearchOwnergAddr_Click" class="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span>
												<%--検索結果レイヤー--%>
												<uc:Layer ID="ucLayerForOwner" Runat="Server" />
												<p class="clr">
													<small class="fred">
														<asp:CustomValidator
															ID="cvOwnerZip1"
															Runat="Server"
															ControlToValidate="tbOwnerZip"
															ValidationGroup="OrderShipping"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															ClientValidationFunction="ClientValidate"
															CssClass="error_inline cvOwnerZipShortInput" />
													</small>
													<small id="sOwnerZipError" Runat="Server" class="fred sOwnerZipError"></small>
												</p>
											</dd>
											<%-- 注文者：都道府県 --%>
											<dt>
												<%: ReplaceTag("@@User.addr1.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<span class="fred">※</span><span id="efo_sign_addr1"></span>
											</dt>
											<dd>
												<asp:DropDownList ID="ddlOwnerAddr1"
													Runat="Server"
													DataTextField="Text"
													DataValueField="Value"
													DataSource="<%# this.Addr1List %>"
													SelectedValue="<%#: this.CartList.Owner.Addr1 %>" />
												<small>
													<asp:CustomValidator
														ID="cvOwnerAddr1"
														Runat="Server"
														ControlToValidate="ddlOwnerAddr1"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<% } %>

											<%-- 注文者：市区町村 --%>
											<dt>
												<%: ReplaceTag("@@User.addr2.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<span class="fred">※</span><% if (isOwnerAddrCountryJp) { %><span id="efo_sign_addr2"></span><% } %>
											</dt>
											<dd>
												<% if (isOwnerAddrCountryTw) { %>
												<asp:DropDownList Runat="Server" ID="ddlOwnerAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged" />
												<% } else { %>
												<asp:TextBox ID="tbOwnerAddr2" Text="<%#: this.CartList.Owner.Addr2 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' Runat="Server" /><br />
												<small>
												<asp:CustomValidator
													ID="cvOwnerAddr2"
													Runat="Server"
													ControlToValidate="tbOwnerAddr2"
													ValidationGroup="OrderShipping"
													ValidateEmptyText="true"
													SetFocusOnError="true"
													ClientValidationFunction="ClientValidate"
													CssClass="error_inline" />
												</small>
											<% } %>
											</dd>
											<%-- 注文者：番地 --%>
											<dt>
												<%: ReplaceTag("@@User.addr3.name@@", ownerAddrCountryIsoCode) %>
												<% if (IsAddress3Necessary(ownerAddrCountryIsoCode)) { %>&nbsp;<span class="fred">※</span><span id="efo_sign_addr3"></span><% } %>
											</dt>
											<dd>
												<% if (isOwnerAddrCountryTw) { %>
												<asp:DropDownList Runat="Server" ID="ddlOwnerAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95" />
												<% } else { %>
												<asp:TextBox ID="tbOwnerAddr3" Text="<%#: this.CartList.Owner.Addr3 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' Runat="Server" /><br />
												<small>
													<asp:CustomValidator ID="cvOwnerAddr3"
														Runat="Server"
														SetFocusOnError="true"
														ValidateEmptyText="true"
														ValidationGroup="OrderShipping"
														ControlToValidate="tbOwnerAddr3"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
												<% } %>
											</dd>
											<%-- 注文者：ビル・マンション名 --%>
											<dt>
												<%: ReplaceTag("@@User.addr4.name@@", ownerAddrCountryIsoCode) %>
												<% if (isOwnerAddrCountryJp == false) { %><span class="fred">*</span><% } %>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerAddr4" Text="<%#: this.CartList.Owner.Addr4 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' Runat="Server" /><br />
												<small>
													<asp:CustomValidator ID="cvOwnerAddr4"
														Runat="Server"
														SetFocusOnError="true"
														ValidateEmptyText="true"
														ValidationGroup="OrderShipping"
														ControlToValidate="tbOwnerAddr4"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<%-- 注文者：州 --%>
											<% if (isOwnerAddrCountryJp == false) { %>
											<dt>
												<%: ReplaceTag("@@User.addr5.name@@", ownerAddrCountryIsoCode) %>
												<% if (isOwnerAddrCountryUs) { %>&nbsp;<span class="fred">*</span><% } %>
											</dt>
											<dd>
												<% if (isOwnerAddrCountryUs) { %>
												<asp:DropDownList Runat="Server" ID="ddlOwnerAddr5" DataSource="<%# this.UserStateList %>" />
												<asp:CustomValidator ID="cvOwnerAddr5Ddl"
													Runat="Server"
													SetFocusOnError="true"
													ValidateEmptyText="true"
													ControlToValidate="ddlOwnerAddr5"
													ValidationGroup="OrderShippingGlobal"
													ClientValidationFunction="ClientValidate"
													CssClass="error_inline" />
												<% } else { %>
												<asp:TextBox Runat="Server" ID="tbOwnerAddr5" Text="<%#: this.CartList.Owner.Addr5 %>" />
												<asp:CustomValidator ID="cvOwnerAddr5"
													Runat="Server"
													ControlToValidate="tbOwnerAddr5"
													ValidationGroup="OrderShippingGlobal"
													ValidateEmptyText="true"
													SetFocusOnError="true"
													ClientValidationFunction="ClientValidate"
													CssClass="error_inline" />
												<% } %>
											</dd>
											<%-- 注文者：郵便番号（海外向け） --%>
											<dt>
												<%: ReplaceTag("@@User.zip.name@@", ownerAddrCountryIsoCode) %>
												<% if (isOwnerAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerZipGlobal" Text="<%#: this.CartList.Owner.Zip %>" MaxLength="20" Runat="Server" Type="tel" />
												<asp:CustomValidator ID="cvOwnerZipGlobal"
													Runat="Server"
													ControlToValidate="tbOwnerZipGlobal"
													ValidationGroup="OrderShippingGlobal"
													ValidateEmptyText="true"
													SetFocusOnError="true"
													ClientValidationFunction="ClientValidate"
													CssClass="error_inline" />
												<asp:LinkButton ID="lbSearchAddrOwnerFromZipGlobal"
													OnClick="lbSearchAddrOwnerFromZipGlobal_Click"
													Style="display: none;"
													Runat="Server" />
											</dd>
											<% } %>

											<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
											<%-- 注文者：企業名 --%>
											<dt>
												<%: ReplaceTag("@@User.company_name.name@@") %>
												&nbsp;<span class="fred"></span>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerCompanyName" Text="<%#: this.CartList.Owner.CompanyName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' Runat="Server" /><br />
												<small>
													<asp:CustomValidator ID="cvOwnerCompanyName"
														Runat="Server"
														ControlToValidate="tbOwnerCompanyName"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<%-- 注文者：部署名 --%>
											<dt>
												<%: ReplaceTag("@@User.company_post_name.name@@") %>
												&nbsp;<span class="fred"></span>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerCompanyPostName" Text="<%#: this.CartList.Owner.CompanyPostName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' Runat="Server" /><br />
												<small>
													<asp:CustomValidator ID="cvOwnerCompanyPostName"
														Runat="Server"
														ControlToValidate="tbOwnerCompanyPostName"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<% } %>

											<%-- 注文者：電話番号1 --%>
											<% if (isOwnerAddrCountryJp) { %>
											<dt>
												<%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<span class="fred">※</span><span id="efo_sign_tel1"></span>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerTel1" Text="<%#: this.CartList.Owner.Tel1 %>" CssClass="input_widthC input_border shortTel" MaxLength="13" Type="tel" Runat="Server" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
												<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
												<span class="btn_add_sea">
													<asp:LinkButton ID="lbGetAuthenticationCode"
														class="btn btn-mini"
														Runat="Server"
														Text="認証コードの取得"
														OnClick="lbGetAuthenticationCode_Click"
														OnClientClick="return checkTelNoInput();" />
												</span>
												<p><asp:Label ID="lbAuthenticationStatus" Runat="Server" /></p>
												<% } %>
												<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
												<!-- mobile phone if use GMO payment -->
												<small>
													<span class="warning_inline"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE) %></span>
												</small>
												<% } %>
												<small>
													<asp:CustomValidator ID="cvOwnerTel1_1"
														Runat="Server"
														ControlToValidate="tbOwnerTel1"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
											<dt>
												<%: ReplaceTag("@@User.authentication_code.name@@") %>
											</dt>
											<dd>
												<asp:TextBox ID="tbAuthenticationCode"
													CssClass="input_widthA input_border"
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
												<asp:CustomValidator ID="cvAuthenticationCode"
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
												<%: ReplaceTag("@@User.tel2.name@@", ownerAddrCountryIsoCode) %>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerTel2" Text="<%#: this.CartList.Owner.Tel2 %>" CssClass="input_widthD input_border shortTel" MaxLength="13" Type="tel" Runat="Server" /><br />
												<small>
													<asp:CustomValidator ID="cvOwnerTel2_1"
														Runat="Server"
														ControlToValidate="tbOwnerTel2"
														ValidationGroup="OrderShipping"
														ValidateEmptyText="false"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<% } %>
											<% if (isOwnerAddrCountryJp == false) { %>
											<%-- 注文者：電話番号1（海外向け） --%>
											<dt>
												<%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %>
												&nbsp;<span class="fred">※</span>
											</dt>
											<dd>
												<asp:TextBox ID="tbOwnerTel1Global" Text="<%#: this.CartList.Owner.Tel1 %>" MaxLength="30" Runat="Server" Type="tel" onchange="resetAuthenticationCodeInput('cvOwnerTel1Global')" />
												<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
												<span class="btn_add_sea">
													<asp:LinkButton ID="lbGetAuthenticationCodeGlobal"
														class="btn btn-mini"
														Runat="Server"
														Text="認証コードの取得"
														OnClick="lbGetAuthenticationCode_Click"
														OnClientClick="return checkTelNoInput();" />
													<asp:Label ID="lbAuthenticationStatusGlobal" Runat="Server" />
												</span>
												<% } %>
												<small>
													<asp:CustomValidator ID="cvOwnerTel1Global"
														Runat="Server"
														ControlToValidate="tbOwnerTel1Global"
														ValidationGroup="OrderShippingGlobal"
														ValidateEmptyText="true"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
											<dt>
												<%: ReplaceTag("@@User.authentication_code.name@@") %>
											</dt>
											<dd>
												<asp:TextBox ID="tbAuthenticationCodeGlobal"
													CssClass="input_widthA input_border"
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
												<asp:CustomValidator ID="cvAuthenticationCodeGlobal"
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
												<asp:TextBox ID="tbOwnerTel2Global" Text="<%#: this.CartList.Owner.Tel2 %>" MaxLength="30" Runat="Server" Type="tel" />
												<small>
													<asp:CustomValidator ID="cvOwnerTel2Global"
														Runat="Server"
														ControlToValidate="tbOwnerTel2Global"
														ValidationGroup="OrderShippingGlobal"
														ValidateEmptyText="false"
														SetFocusOnError="true"
														ClientValidationFunction="ClientValidate"
														CssClass="error_inline" />
												</small>
											</dd>
											<% } %>
											<dt>
												<%: ReplaceTag("@@User.mail_flg.name@@") %>
											</dt>
											<dd><asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text=" 配信する" CssClass="checkBox" Runat="Server" /></dd>
											</dl>
										</div><!--bottom-->
									</div><!--top-->
								</div><!--userBox-->
							<% this.CartItemIndexTmp = -1; %>
							</div><!--column-->
							<%-- ▲注文者情報▲ --%>

							<%-- ▼配送先情報▼ --%>
							<div class="columnRight" visible='<%# Container.ItemIndex == 0 %>'>
								<div class="orderBox cartBox">
									<h3>
										<div class="cartNo">カート番号<%#: Container.ItemIndex + 1 %><%#: DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）") %></div>
										<div class="cartLink"><a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST %>">カートへ戻る</a></div>
									</h3>
									<span class="invalidInputError" style="color: red; display: block; padding-left: 11px;"></span>
									<%
										this.CartItemIndexTmp++;
										this.CartShippingItemIndexTmp = -1;
									%>
									<asp:Repeater ID="rCartShippings" Runat="Server">
										<ItemTemplate>
											<% this.CartShippingItemIndexTmp++; %>
											<div class="orderBoxLarge" style="margin-bottom: 5px;">
												<div class="cartNo cartInfoToggle" style="cursor: pointer; user-select: none; <%#: GetCartShipping(Container.DataItem).CartObject.IsGift ? string.Empty : "display: none;" %>" onclick="initCartInfoToggle(<%# Container.ItemIndex %>)">
													<span id="toggleIcon" style="float: left; padding-right: 10px;">▼</span>
													<span runat="server" class="hiddenIndex" style="display: none;" gift="<%#: GetCartShipping(Container.DataItem).CartObject.IsGift %>"><%#: Container.ItemIndex + 1 %></span>
													<p style="float: left; padding-right: 10px;">&ensp;配送先<%#: Container.ItemIndex + 1 %></p>
												</div>
												<asp:LinkButton ID="lbDeleteShipping"
													Runat="Server"
													Text="削除"
													CommandArgument="<%#: Container.ItemIndex %>"
													OnClick="lbDeleteShipping_Click"
													Visible="<%# (DisplayFixedPurchaseShipping(Container) == false) && (((IList)((Repeater)Container.Parent).DataSource).Count > 1) %>"
													class="btnDelete" />
												<div class="clr"></div>
												<div class="<%#: GetCartShipping(Container.DataItem).CartObject.IsGift ? "toggleBlock" : string.Empty %>">
													<asp:Repeater ID="rAllocatedProductsTop" Runat="Server">
														<HeaderTemplate>
															<%-- △のし情報△ --%>
															<h4>商品</h4>
															<%-- Cart product --%>
															<div class="userProduct" style="border: none;">
														</HeaderTemplate>
														<ItemTemplate>
															<div class="<%#: ((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1 ? "last" : "" %>" style="width: auto;">
																<dl>
																	<dt>
																		<a Runat="Server"
																			href='<%#: GetProductCount(Container.DataItem).Product.CreateProductDetailUrl() %>'
																			Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() %>">
																			<w2c:ProductImage ProductMaster="<%# GetProductCount(Container.DataItem).Product %>" ImageSize="S" Runat="Server" />
																		</a>
																		<w2c:ProductImage ProductMaster="<%# GetProductCount(Container.DataItem).Product %>"
																			ImageSize="S"
																			Runat="Server"
																			Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() == false %>" />
																	</dt>
																	<dd>
																	<span style="float: left; padding-top: 0; font-weight: bold;">
																		<span>
																			<a Runat="Server"
																				href='<%#: GetProductCount(Container.DataItem).Product.CreateProductDetailUrl() %>'
																				Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() %>"
																				style="display: block; max-width: 200px;">
																				<%#: GetProductCount(Container.DataItem).Product.ProductJointName %>
																			</a>
																			<span visible="<%# FindCart(Container.DataItem).IsGift %>" Runat="Server">
																				<p class="quantity"><%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(GetProductCount(Container.DataItem).Product.Price * GetProductCount(Container.DataItem).Count)) %></p>
																			</span>
																		</span>
																	</span>
																	<p class="clr"></p>
																	<%#: GetProductCount(Container.DataItem).Product.GetProductTag("tag_cart_product_message").Length != 0 ? "<small>" + GetProductCount(Container.DataItem).Product.GetProductTag("tag_cart_product_message") + "</small>" : "" %>
																	<p visible='<%# GetProductCount(Container.DataItem).Product.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' Runat="Server" style="float: left;">
																		<b>
																		<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetProductCount(Container.DataItem).Product.ProductOptionSettingList %>' Runat="Server">
																			<ItemTemplate>
																				<%#: ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																				<%# string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false ? "<br />" : string.Empty %>
																			</ItemTemplate>
																		</asp:Repeater>
																		</b>
																	</p>
																	</dd>
																</dl>
																<p class="clr"></p>
															</div>
														</ItemTemplate>
														<FooterTemplate>
																<small id="hcErrorMessage" enableviewstate="false" class="fred" Runat="Server"></small>
															</div>
														</FooterTemplate>
													</asp:Repeater>
													<%-- Cart sender --%>
													<div Runat="Server" Visible='<%# GetCartShipping(Container.DataItem).CartObject.IsGift %>'>
													<h4>送り主</h4>
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
																<%-- 送り主：氏名 --%>
																<dt>
																	<%: ReplaceTag("@@User.name.name@@", senderAddrCountryIsoCode) %>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	姓&nbsp;&nbsp;<asp:TextBox ID="tbSenderName1" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' Runat="Server" />&nbsp;&nbsp;
																	名&nbsp;&nbsp;<asp:TextBox ID="tbSenderName2" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator ID="cvSenderName1"
																			Runat="Server"
																			ControlToValidate="tbSenderName1"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																		<asp:CustomValidator ID="cvSenderName2"
																			Runat="Server"
																			ControlToValidate="tbSenderName2"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<%-- 送り主：氏名（かな） --%>
																<% if (isSenderAddrCountryJp) { %>
																<dt>
																	<%: ReplaceTag("@@User.name_kana.name@@", senderAddrCountryIsoCode)%>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	姓&nbsp;&nbsp;<asp:TextBox ID="tbSenderNameKana1" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' Runat="Server" />&nbsp;&nbsp;
																	名&nbsp;&nbsp;<asp:TextBox ID="tbSenderNameKana2" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator ID="cvSenderNameKana1"
																			Runat="Server"
																			ControlToValidate="tbSenderNameKana1"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ValidationGroup="OrderShipping"
																			CssClass="error_inline" />
																		<asp:CustomValidator ID="cvenderNameKana2"
																			Runat="Server"
																			ControlToValidate="tbSenderNameKana2"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<% } %>
																<%-- 送り主：国 --%>
																<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																<dt>
																	<%: ReplaceTag("@@User.country.name@@", senderAddrCountryIsoCode) %>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	<asp:DropDownList ID="ddlSenderCountry"
																		Runat="Server"
																		DataSource="<%# this.UserCountryDisplayList %>"
																		AutoPostBack="true"
																		OnSelectedIndexChanged="ddlSenderCountry_SelectedIndexChanged"
																		DataTextField="Text"
																		DataValueField="Value"
																		SelectedValue="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE) %>" />
																	<small>
																		<asp:CustomValidator ID="cvSenderCountry"
																			Runat="Server"
																			ControlToValidate="ddlSenderCountry"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<% } %>
																<%-- 送り主：郵便番号 --%>
																<% if (isSenderAddrCountryJp) { %>
																<dt>
																	<%: ReplaceTag("@@User.zip.name@@", senderAddrCountryIsoCode) %>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	<span class="pdg_topC">
																		<asp:TextBox ID="tbSenderZip" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ZIP) %>" OnTextChanged="lbSearchSenderAddr_Click" CssClass="input_widthC input_border" MaxLength="8" Runat="Server" />
																	</span>
																	<span class="btn_add_sea"><asp:LinkButton ID="lbSearchSenderAddr" Runat="Server" OnClick="lbSearchSenderAddr_Click" CssClass="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span><br class="clr" />
																	<%--検索結果レイヤー--%>
																	<uc:Layer ID="ucLayerForSender" Runat="Server" />
																	<small>
																		<asp:CustomValidator ID="cvSenderZip1"
																			Runat="Server"
																			ControlToValidate="tbSenderZip"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline cvSenderZipShortInput" />
																	</small>
																	<small id="sSenderZipError" Runat="Server" class="fred sSenderZipError"></small>
																</dd>
																<%-- 送り主：都道府県 --%>
																<dt>
																	<%: ReplaceTag("@@User.addr1.name@@", senderAddrCountryIsoCode) %>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	<asp:DropDownList ID="ddlSenderAddr1"
																		DataSource="<%# this.Addr1List %>"
																		DataTextField="Text"
																		DataValueField="Value"
																		SelectedValue="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1) %>"
																		Runat="Server" />
																	<small>
																		<asp:CustomValidator ID="cvSenderAddr1"
																			Runat="Server"
																			ControlToValidate="ddlSenderAddr1"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<% } %>
																<%-- 送り主：市区町村 --%>
																<dt>
																	<%: ReplaceTag("@@User.addr2.name@@", senderAddrCountryIsoCode) %>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	<% if (isSenderAddrCountryTw) { %>
																	<asp:DropDownList Runat="Server" ID="ddlSenderAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged" />
																	<% } else { %>
																	<asp:TextBox ID="tbSenderAddr2" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator ID="cvSenderAddr2"
																			Runat="Server"
																			ControlToValidate="tbSenderAddr2"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																	<% } %>
																</dd>
																<%-- 送り主：番地 --%>
																<dt>
																	<%: ReplaceTag("@@User.addr3.name@@", senderAddrCountryIsoCode) %>
																	<% if (IsAddress3Necessary(senderAddrCountryIsoCode)) { %>&nbsp;<span class="fred">※</span><% } %>
																</dt>
																<dd>
																	<% if (isSenderAddrCountryTw) { %>
																	<asp:DropDownList Runat="Server" ID="ddlSenderAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95" />
																	<% } else { %>
																	<asp:TextBox ID="tbSenderAddr3" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator ID="cvSenderAddr3"
																			Runat="Server"
																			ControlToValidate="tbSenderAddr3"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																	<% } %>
																</dd>
																<%-- 送り主：ビル・マンション名 --%>
																<dt>
																	<%: ReplaceTag("@@User.addr4.name@@", senderAddrCountryIsoCode) %>
																	<% if (isSenderAddrCountryJp == false) { %>&nbsp;<span class="fred">※</span><% } %>
																</dt>
																<dd>
																	<asp:TextBox ID="tbSenderAddr4" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator ID="cvSenderAddr4"
																			Runat="Server"
																			ControlToValidate="tbSenderAddr4"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<%-- 送り主：州 --%>
																<% if (isSenderAddrCountryJp == false) { %>
																<dt>
																	<%: ReplaceTag("@@User.addr5.name@@", senderAddrCountryIsoCode) %>
																	<% if (isSenderAddrCountryUs) { %>&nbsp;<span class="fred">※</span><% } %>
																</dt>
																<dd>
																	<% if (isSenderAddrCountryUs) { %>
																	<asp:DropDownList Runat="Server" ID="ddlSenderAddr5" DataSource="<%# this.UserStateList %>" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), CartShipping.FIELD_ORDERSHIPPING_SENDER_ADDR5_US) %>" />
																	<asp:CustomValidator ID="cvSenderAddr5Ddl"
																		Runat="Server"
																		ControlToValidate="ddlSenderAddr5"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																	<% } else { %>
																	<asp:TextBox Runat="Server" ID="tbSenderAddr5" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5) %>" />
																	<small>
																		<asp:CustomValidator ID="cvSenderAddr5"
																			Runat="Server"
																			ControlToValidate="tbSenderAddr5"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																	<% } %>
																</dd>
																<%-- 送り主：郵便番号（海外向け） --%>
																<dt>
																	<%: ReplaceTag("@@User.zip.name@@", senderAddrCountryIsoCode) %>
																	<% if (isSenderAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
																</dt>
																<dd>
																	<asp:TextBox Runat="Server" ID="tbSenderZipGlobal" MaxLength="20" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_ZIP) %>" />
																	<small>
																		<asp:CustomValidator ID="cvSenderZipGlobal"
																			Runat="Server"
																			ControlToValidate="tbSenderZipGlobal"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																	<asp:LinkButton ID="lbSearchAddrSenderFromZipGlobal"
																		OnClick="lbSearchAddrSenderFromZipGlobal_Click"
																		Style="display:none;"
																		Runat="Server" />
																</dd>
																<% } %>
																<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
																<%-- 送り主：企業名 --%>
																<dt>
																	<%: ReplaceTag("@@User.company_name.name@@") %>
																	&nbsp;<span class="fred"></span>
																</dt>
																<dd>
																	<asp:TextBox ID="tbSenderCompanyName" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator ID="cvSenderCompanyName"
																			Runat="Server"
																			ControlToValidate="tbSenderCompanyName"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<%-- 送り主：部署名 --%>
																<dt>
																	<%: ReplaceTag("@@User.company_post_name.name@@")%>
																	&nbsp;<span class="fred"></span>
																</dt>
																<dd>
																	<asp:TextBox ID="tbSenderCompanyPostName" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator ID="cvSenderCompanyPostName"
																			Runat="Server"
																			ControlToValidate="tbSenderCompanyPostName"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<% } %>
																<%-- 送り主：電話番号 --%>
																<% if (isSenderAddrCountryJp) { %>
																<dt>
																	<%: ReplaceTag("@@User.tel1.name@@") %>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	<asp:TextBox ID="tbSenderTel1" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_TEL1) %>" CssClass="input_widthE input_border shortTel" MaxLength="13" Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator
																			ID="cvSenderTel1_1"
																			Runat="Server"
																			ControlToValidate="tbSenderTel1"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<% } else { %>
																<%-- 送り主：電話番号1（海外向け） --%>
																<dt>
																	<%: ReplaceTag("@@User.tel1.name@@", senderAddrCountryIsoCode) %>
																	&nbsp;<span class="fred">※</span>
																</dt>
																<dd>
																	<asp:TextBox ID="tbSenderTel1Global" Runat="Server" MaxLength="30" Text="<%#: GetSenderValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SENDER_TEL1) %>" />
																	<small>
																		<asp:CustomValidator ID="cvSenderTel1Global"
																			Runat="Server"
																			ControlToValidate="tbSenderTel1Global"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
																<% } %>
															</dl>
														</div>
														<%-- △送り主：入力フォーム△ --%>
													</div>
													</div>
													<%-- Cart shipping --%>
													<h4>配送先</h4>
													<div class="cartNo">
														<div class="userList" style="padding-bottom: 0; padding-top: 0;">
															配送先を選択して下さい。<br />
															<asp:DropDownList ID="ddlShippingKbnList"
																DataSource="<%# this.UserShippingList %>"
																DataTextField="text"
																DataValueField="value"
																AutoPostBack="true"
																SelectedValue='<%#: GetCartShipping(Container.DataItem).ShippingAddrKbn %>'
																OnSelectedIndexChanged="ddlShippingKbnList_OnSelectedIndexChanged"
																Runat="Server" />
														</div>
														<div id="divShippingDisp" class="userList" visible="<%# GetShipToNew(GetCartShipping(Container.DataItem)) == false %>" Runat="Server" style="padding-top: 0;">
															<% var isShippingAddrCountryJp = IsCountryJp(this.CountryIsoCode); %>
															<dl>
																<dt><%#: ReplaceTag("@@User.name.name@@") %></dt>
																<dd>
																	<asp:Literal ID="lShippingName1" Runat="Server" /><asp:Literal ID="lShippingName2" Runat="Server" />&nbsp;様
																	<% if (isShippingAddrCountryJp) { %>
																	（<asp:Literal ID="lShippingNameKana1" Runat="Server" /><asp:Literal ID="lShippingNameKana2" Runat="Server" />&nbsp;さま）
																	<% } %>
																</dd>
																<dt><%: ReplaceTag("@@User.addr.name@@") %></dt>
																<dd>
																	<% if (isShippingAddrCountryJp) { %>〒<asp:Literal ID="lShippingZip" Runat="Server" /><br /><% } %>
																	<asp:Literal ID="lShippingAddr1" Runat="Server" /> <asp:Literal ID="lShippingAddr2" Runat="Server" /><br />
																	<asp:Literal ID="lShippingAddr3" Runat="Server" /> <asp:Literal ID="lShippingAddr4" Runat="Server" />
																	<asp:Literal ID="lShippingAddr5" Runat="Server" /><br />
																	<% if (isShippingAddrCountryJp == false) { %><asp:Literal ID="lShippingZipGlobal" Runat="Server" /><br /><% } %>
																	<asp:Literal ID="lShippingCountryName" Runat="Server" /><br />
																	<small id="sOwnerZipError" Runat="Server" class="fred shortZipInputErrorMessage"></small>
																</dd>
																<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
																<dt>
																	<%: ReplaceTag("@@User.company_name.name@@")%>・
																	<%: ReplaceTag("@@User.company_post_name.name@@")%>
																</dt>
																<dd><asp:Literal ID="lShippingCompanyName" Runat="Server" />&nbsp<asp:Literal ID="lShippingCompanyPostName" Runat="Server" /></dd>
																<% } %>
																<%-- 電話番号 --%>
																<dt><%#: ReplaceTag("@@User.tel1.name@@") %></dt>
																<dd><asp:Literal ID="lShippingTel1" Runat="Server" /></dd>
															</dl>
														</div>
														<div id="divShippingInputFormInner" class="userList" visible="<%# GetShipToNew(GetCartShipping(Container.DataItem)) %>" Runat="Server" style="padding-top: 0;">
															<%
																var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartItemIndexTmp, this.CartShippingItemIndexTmp);
																var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
																var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
																var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
																var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
															%>
															<dl>
																<%-- 配送先：氏名 --%>
																<dt>
																	<%: ReplaceTag("@@User.name.name@@") %>
																	&nbsp;<span class="fred">※</span>
																</dt>
															<dd>
																姓&nbsp;&nbsp;<asp:TextBox ID="tbShippingName1" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' Runat="Server" />&nbsp;&nbsp;
																名&nbsp;&nbsp;<asp:TextBox ID="tbShippingName2" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' Runat="Server" /><br />
																<small>
																	<asp:CustomValidator ID="cvShippingName1"
																		Runat="Server"
																		ControlToValidate="tbShippingName1"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																	<asp:CustomValidator ID="cvShippingName2"
																		Runat="Server"
																		ControlToValidate="tbShippingName2"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
															</dd>
															<%-- 配送先：氏名（かな） --%>
															<% if (isShippingAddrCountryJp) { %>
															<dt>
																<%: ReplaceTag("@@User.name_kana.name@@")%>
																&nbsp;<span class="fred">※</span>
															</dt>
															<dd>
																姓&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana1" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' Runat="Server" />&nbsp;&nbsp;
																名&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana2" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' Runat="Server" /><br />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingNameKana1"
																		Runat="Server"
																		ControlToValidate="tbShippingNameKana1"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ValidationGroup="OrderShipping"
																		CssClass="error_inline" />
																	<asp:CustomValidator
																		ID="cvShippingNameKana2"
																		Runat="Server"
																		ControlToValidate="tbShippingNameKana2"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
															</dd>
															<% } %>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<%-- 配送先：国 --%>
															<dt>
																<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
																&nbsp;<span class="fred">※</span>
															</dt>
															<dd>
																<asp:DropDownList ID="ddlShippingCountry"
																	Runat="Server"
																	DataSource="<%# this.ShippingAvailableCountryDisplayList %>"
																	OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"
																	AutoPostBack="true"
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
																&nbsp;<span class="fred">※</span>
															</dt>
															<dd>
																<span class="pdg_topC">
																	<asp:TextBox ID="tbShippingZip" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" CssClass="input_widthC input_border" OnTextChanged="lbSearchShippingAddr_Click" MaxLength="8" Runat="Server" />
																</span>
																<span class="btn_add_sea"><asp:LinkButton ID="lbSearchShippingAddr" Runat="Server" OnClick="lbSearchShippingAddr_Click" CssClass="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span><br class="clr" />
																<%--検索結果レイヤー--%>
																<uc:Layer ID="ucLayerForShipping" Runat="Server" />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingZip1"
																		Runat="Server"
																		ControlToValidate="tbShippingZip"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline cvShippingZipShortInput" />
																</small>
																<small id="sShippingZipError" Runat="Server" class="fred sShippingZipError"></small>
															</dd>
															<%-- 配送先：都道府県 --%>
															<dt>
																<%: ReplaceTag("@@User.addr1.name@@") %>
																&nbsp;<span class="fred">※</span>
															</dt>
															<dd>
																<asp:DropDownList ID="ddlShippingAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>" Runat="Server" />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingAddr1"
																		Runat="Server"
																		ControlToValidate="ddlShippingAddr1"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
															</dd>
															<% } %>
															<%-- 配送先：市区町村 --%>
															<dt>
																<%: ReplaceTag("@@User.addr2.name@@", shippingAddrCountryIsoCode) %>
																&nbsp;<span class="fred">※</span>
															</dt>
															<dd>
															<% if (isShippingAddrCountryTw) { %>
																<asp:DropDownList Runat="Server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged" />
															<% } else { %>
																<asp:TextBox ID="tbShippingAddr2"
																	Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %>"
																	CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>'
																	Runat="Server" /><br />
																<small>
																<asp:CustomValidator
																	ID="cvShippingAddr2"
																	Runat="Server"
																	ControlToValidate="tbShippingAddr2"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																	CssClass="error_inline" /></small>
															<% } %>
															</dd>
															<%-- 配送先：番地 --%>
															<dt>
																<%: ReplaceTag("@@User.addr3.name@@", shippingAddrCountryIsoCode) %>
																<% if (IsAddress3Necessary(shippingAddrCountryIsoCode)) { %>&nbsp;<span class="fred">※</span><% } %>
															</dt>
															<dd>
															<% if (isShippingAddrCountryTw) { %>
																<asp:DropDownList Runat="Server" ID="ddlShippingAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95" />
															<% } else { %>
																<asp:TextBox ID="tbShippingAddr3" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' Runat="Server" /><br />
																<small>
																<asp:CustomValidator
																	ID="cvShippingAddr3"
																	Runat="Server"
																	ControlToValidate="tbShippingAddr3"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																	CssClass="error_inline" /></small>
															<% } %>
															</dd>
															<%-- 配送先：ビル・マンション名 --%>
															<dt>
																<%: ReplaceTag("@@User.addr4.name@@", shippingAddrCountryIsoCode) %>
																<% if (isShippingAddrCountryJp == false) { %>&nbsp;<span class="fred">※</span><% } %>
															</dt>
															<dd><asp:TextBox ID="tbShippingAddr4" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' Runat="Server" /><br />
															<small>
															<asp:CustomValidator
																ID="cvShippingAddr4"
																Runat="Server"
																ControlToValidate="tbShippingAddr4"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true"
																ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																CssClass="error_inline" /></small>
															</dd>
															<% if (isShippingAddrCountryJp == false) { %>
															<%-- 配送先：州 --%>
															<dt>
																<%: ReplaceTag("@@User.addr5.name@@", shippingAddrCountryIsoCode) %>
																<% if (isShippingAddrCountryUs) { %>&nbsp;<span class="fred">※</span><% } %>
															</dt>
															<dd>
																<% if (isShippingAddrCountryUs) { %>
																<asp:DropDownList Runat="Server" ID="ddlShippingAddr5" DataSource="<%# this.UserStateList %>" />
																<asp:CustomValidator
																	ID="cvShippingAddr5Ddl"
																	Runat="Server"
																	ControlToValidate="ddlShippingAddr5"
																	ValidationGroup="OrderShippingGlobal"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																	CssClass="error_inline" />
																<% } else { %>
																<asp:TextBox Runat="Server" ID="tbShippingAddr5" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>" />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingAddr5"
																		Runat="Server"
																		ControlToValidate="tbShippingAddr5"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
																<% } %>
															</dd>
															<%-- 配送先：郵便番号（海外向け） --%>
															<dt>
																<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
																<% if (isShippingAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
															</dt>
															<dd>
																<asp:TextBox Runat="Server" ID="tbShippingZipGlobal" MaxLength="20" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingZipGlobal"
																		Runat="Server"
																		ControlToValidate="tbShippingZipGlobal"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
																<asp:LinkButton
																	ID="lbSearchAddrShippingFromZipGlobal"
																	OnClick="lbSearchAddrShippingFromZipGlobal_Click"
																	Style="display: none;"
																	Runat="Server" />
															</dd>
															<% } %>
															<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
															<%-- 配送先：企業名 --%>
															<dt>
																<%: ReplaceTag("@@User.company_name.name@@")%>
																&nbsp;<span class="fred"></span>
															</dt>
															<dd>
																<asp:TextBox ID="tbShippingCompanyName" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' Runat="Server" /><br />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingCompanyName"
																		Runat="Server"
																		ControlToValidate="tbShippingCompanyName"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
															</dd>
															<%-- 配送先：部署名 --%>
															<dt>
																<%: ReplaceTag("@@User.company_post_name.name@@")%>
																&nbsp;<span class="fred"></span>
															</dt>
															<dd>
																<asp:TextBox ID="tbShippingCompanyPostName" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' Runat="Server" /><br />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingCompanyPostName"
																		Runat="Server"
																		ControlToValidate="tbShippingCompanyPostName"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
															</dd>
															<% } %>

															<%-- 配送先：電話番号 --%>
															<% if (isShippingAddrCountryJp) { %>
															<dt>
																<%: ReplaceTag("@@User.tel1.name@@") %>
																&nbsp;<span class="fred">※</span>
															</dt>
															<dd>
																<asp:TextBox ID="tbShippingTel1" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" CssClass="input_widthE input_border shortTel" MaxLength="13" Runat="Server" />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingTel1_1"
																		Runat="Server"
																		ControlToValidate="tbShippingTel1"
																		ValidationGroup="OrderShipping"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
															</dd>
															<% } else { %>
															<%-- 配送先：電話番号1（海外向け） --%>
															<dt>
																<%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %>
																&nbsp;<span class="fred">※</span>
															</dt>
															<dd>
																<asp:TextBox Runat="Server" ID="tbShippingTel1Global" MaxLength="30" Text="<%#: GetShippingValue(GetCartShipping(Container.DataItem), Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" />
																<small>
																	<asp:CustomValidator
																		ID="cvShippingTel1Global"
																		Runat="Server"
																		ControlToValidate="tbShippingTel1Global"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																		CssClass="error_inline" />
																</small>
															</dd>
															<% } %>
															</dl>
															<br class="clr" />

															<div class="subbox" visible="<%# this.IsLoggedIn %>" Runat="Server" style="margin: 0;">
																<p>
																	<%-- ポストバックCustomValidatorの状態がクリアされてしまうため、JaavScirptで表示非表示を制御する --%>
																	<asp:RadioButtonList ID="rblSaveToUserShipping" OnSelectedIndexChanged="rblSaveToUserShipping_OnSelectedIndexChanged" AutoPostBack="true" SelectedValue='<%#: GetCartShipping(Container.DataItem).UserShippingRegistFlg ? "1" : "0" %>' RepeatLayout="Flow" CssClass="radioBtn" Runat="Server">
																		<asp:ListItem Text="配送先情報を保存しない" Value="0" />
																		<asp:ListItem Text="配送先情報を保存する" Value="1" />
																	</asp:RadioButtonList>
																</p>
															</div>
															<!--subbox-->
															<dl id="dlUserShippingName" visible="<%# GetCartShipping(Container.DataItem).UserShippingRegistFlg %>" Runat="Server">
																<dd>配送先を保存する場合は、以下をご入力ください。</dd>
																<dt>配送先名&nbsp;<span class="fred">※</span></dt>
																<dd class="last">
																	<asp:TextBox ID="tbUserShippingName" Text="<%#: GetCartShipping(Container.DataItem).UserShippingName %>" MaxLength="30" CssClass="input_widthE input_border" Runat="Server" /><br />
																	<small>
																		<asp:CustomValidator
																			Runat="Server"
																			ControlToValidate="tbUserShippingName"
																			ValidationGroup="OrderShipping"
																			ValidateEmptyText="true"
																			SetFocusOnError="true"
																			ClientValidationFunction="ClientValidateForOrderShippingSelectPage"
																			CssClass="error_inline" />
																	</small>
																</dd>
															</dl>
														</div>
													</div>
													<%-- ▽のし情報▽--%>
													<div visible="<%# IsDisplayGiftWrappingPaperAndBag(Container.DataItem, ((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server" class="orderBoxLarge">
														<h4>
															のし・包装情報&nbsp;&nbsp;
															<asp:LinkButton ID="lbCopyWrappingInfoToOtherShippings"
																OnClick="lbCopyWrappingInfoToOtherShippings_Click"
																Runat="Server"
																Text="他の配送先にコピー" />
														</h4>
														<dl visible='<%# GetWrappingPaperFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' runat="server" class="userList" style="padding: 0 21px;">
															<dt>のし</dt>
															<dd>種類&nbsp;&nbsp;<asp:DropDownList ID="ddlWrappingPaperType" DataSource='<%# GetWrappingPaperTypes(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' SelectedValue='<%#: GetCartShipping(Container.DataItem).WrappingPaperType %>' DataTextField="text" DataValueField="value" runat="server" /></dd>
															<dd>差出人&nbsp;&nbsp;<asp:TextBox ID="tbWrappingPaperName" Text="<%#: GetCartShipping(Container.DataItem).WrappingPaperName %>" MaxLength="200" Width="200" runat="server" /></dd>
														</dl>
														<dl visible='<%# GetWrappingBagFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' runat="server" class="userList" style="padding: 0 21px;">
															<dt>包装</dt>
															<dd>種類&nbsp;&nbsp;<asp:DropDownList ID="ddlWrappingBagType" DataSource='<%# GetWrappingBagTypes(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' SelectedValue='<%#: GetCartShipping(Container.DataItem).WrappingBagType %>' DataTextField="text" DataValueField="value" runat="server" />
															</dd>
														</dl>
													</div>
													<asp:Repeater ID="rAllocatedProducts" Runat="Server">
														<HeaderTemplate>
															<%-- △のし情報△ --%>
															<h4>商品</h4>
															<%-- Cart product --%>
															<div class="userProduct" style="border: none;">
														</HeaderTemplate>
														<ItemTemplate>
																<div class="<%#: ((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1 ? "last" : "" %>" style="width: auto;">
																	<dl>
																		<dt>
																			<a Runat="Server"
																				href='<%#: GetProductCount(Container.DataItem).Product.CreateProductDetailUrl() %>'
																				Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() %>">
																				<w2c:ProductImage ProductMaster="<%# GetProductCount(Container.DataItem).Product %>" ImageSize="S" Runat="Server" />
																			</a>
																			<w2c:ProductImage ProductMaster="<%# GetProductCount(Container.DataItem).Product %>"
																				ImageSize="S"
																				Runat="Server"
																				Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() == false %>" />
																		</dt>
																		<dd>
																		<span style="float: left; padding-top: 0; font-weight: bold;">
																			<span>
																				<a Runat="Server"
																					href='<%#: GetProductCount(Container.DataItem).Product.CreateProductDetailUrl() %>'
																					Visible="<%# GetProductCount(Container.DataItem).Product.IsProductDetailLinkValid() %>"
																					style="display: block; max-width: 200px;">
																					<%#: GetProductCount(Container.DataItem).Product.ProductJointName %>
																				</a>
																				<span visible="<%# FindCart(Container.DataItem).IsGift %>" Runat="Server">
																					<p class="quantity"><%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(GetProductCount(Container.DataItem).Product.Price * GetProductCount(Container.DataItem).Count)) %></p>
																				</span>
																			</span>
																		</span>
																		<span Runat="Server" style="float: right;" visible="<%# FindCart(Container.DataItem).IsGift %>">
																			<asp:LinkButton Runat="Server"
																				Text="+"
																				Enabled="<%# CanChangeQuantity(Container.DataItem, true) %>"
																				CssClass="changeQuantity"
																				OnClick="lbRecalculateCart_Click"
																				Style="text-decoration: none"
																				CommandArgument="plus" />
																			<asp:TextBox
																				ID="tbProductCount"
																				Runat="Server"
																				CssClass="user-product-quantity"
																				MaxLength="3"
																				OnTextChanged="lbRecalculateCart_Click"
																				onKeyPress="return isNumberKey(event)"
																				AutoPostBack="true"
																				Text="<%#: GetProductCount(Container.DataItem).Count %>" />
																			<asp:LinkButton Runat="Server"
																				Text="-"
																				Enabled="<%# CanChangeQuantity(Container.DataItem, false) %>"
																				CssClass="changeQuantity"
																				OnClick="lbRecalculateCart_Click"
																				Style="text-decoration: none"
																				CommandArgument="subtract" />
																			<asp:LinkButton ID="lbRecalculateCart" OnClick="lbRecalculateCart_Click" class="lbRecalculateCart" Runat="Server" />
																		</span>
																		<p class="clr"></p>
																		<%#: GetProductCount(Container.DataItem).Product.GetProductTag("tag_cart_product_message").Length != 0 ? "<small>" + GetProductCount(Container.DataItem).Product.GetProductTag("tag_cart_product_message") + "</small>" : "" %>
																		<p visible='<%# GetProductCount(Container.DataItem).Product.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' Runat="Server" style="float: left;">
																			<b>
																			<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetProductCount(Container.DataItem).Product.ProductOptionSettingList %>' Runat="Server">
																				<ItemTemplate>
																					<%#: ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																					<%# string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false ? "<br />" : string.Empty %>
																				</ItemTemplate>
																			</asp:Repeater>
																			</b>
																		</p>
																	</dl>
																	<p class="clr"></p>
																</div>
															</ItemTemplate>
														<FooterTemplate>
																<small id="hcErrorMessage" enableviewstate="false" class="fred" Runat="Server"></small>
															</div>
														</FooterTemplate>
													</asp:Repeater>
													<div class="orderBoxLarge" style="margin-bottom: 0;">
														<h4>配送指定</h4>
													</div>
													<%-- Cart delivery method --%>
													<div visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server" class="userList">
														配送方法を選択して下さい。
														<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[((RepeaterItem)Container.Parent.Parent).ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" Runat="Server" />
													</div>
													<div id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) && CanDisplayDeliveryCompany(((RepeaterItem)Container.Parent.Parent).ItemIndex)) %>" Runat="Server" class="userList">
														配送サービスを選択して下さい。
														<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" Runat="Server" />
													</div>
													<div id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server" class="userList" style='<%# (HasFixedPurchase(Container) && (DisplayFixedPurchaseShipping(Container) == false)) ? "padding-bottom: 0px" : string.Empty %>'>
														配送希望日時を選択して下さい。
														<dl id="dlShipppingDateTime" Runat="Server">
															<dd></dd>
															<dt id="dtShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">配送希望日</dt>
															<dd id="ddShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">
																<asp:DropDownList id="ddlShippingDate"
																	CssClass="input_border"
																	Runat="Server"
																	DataTextField="text"
																	DataValueField="value"
																	DataSource="<%# GetShippingDateList(GetCartShipping(Container.DataItem), this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex]) %>"
																	OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnCheckedChanged"
																	AutoPostBack="true" />
																<br />
																<asp:Label ID="lShippingDateErrorMessage" CssClass="fred" Runat="Server" />
															</dd>
															<div id="divShippingTime" Runat="Server">
															<dt id="dtShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server">配送希望時間帯</dt>
															<dd id="ddShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" Runat="Server" class="last">
																<asp:DropDownList ID="ddlShippingTime" Runat="Server" DataSource="<%# GetShippingTimeList(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%#: GetShippingTime(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" />
															</dd>
															</div>
														</dl>
													</div>
													<div class="clr"></div>
													<%-- ▽Invoice▽ --%>
													<div style="min-height: 134px;" id="sInvoices" class="" Runat="Server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(GetCartShipping(Container.DataItem).ShippingCountryIsoCode) %>">
														<h4>発票種類</h4>
														<dl id="divUniformInvoiceType" Runat="Server" class="userList">
															<dl>
																<dd>
																	<asp:DropDownList ID="ddlUniformInvoiceType" Runat="Server"
																		CssClass="input_border"
																		DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE) %>"
																		DataTextField="text"
																		DataValueField="value"
																		OnSelectedIndexChanged="ddlUniformInvoiceType_SelectedIndexChanged"
																		AutoPostBack="true" />
																	<asp:DropDownList ID="ddlUniformInvoiceTypeOption" Runat="Server"
																		CssClass="input_border"
																		DataTextField="text"
																		DataValueField="value"
																		OnSelectedIndexChanged="ddlUniformInvoiceTypeOption_SelectedIndexChanged"
																		AutoPostBack="true"
																		Visible="false" />
																</dd>
															</dl>
															<dl id="dlUniformInvoiceOption1_8" Runat="Server" visible="false">
																<br />
																<dd>統一編号</dd>
																<dd>
																	<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Width="220" Runat="Server" MaxLength="8" />
																	<asp:CustomValidator
																		ID="cvUniformInvoiceOption1_8" Runat="Server"
																		ControlToValidate="tbUniformInvoiceOption1_8"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		ClientValidationFunction="ClientValidate"
																		SetFocusOnError="true"
																		CssClass="error_inline" />
																	<asp:Label ID="lbUniformInvoiceOption1_8" Runat="Server" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Visible="false" />
																</dd>
																<br />
																<dd>会社名</dd>
																<dd>
																	<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption2 %>" Width="220" Runat="Server" MaxLength="20" />
																	<asp:CustomValidator
																		ID="cvUniformInvoiceOption2" Runat="Server"
																		ControlToValidate="tbUniformInvoiceOption2"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		ClientValidationFunction="ClientValidate"
																		SetFocusOnError="true"
																		CssClass="error_inline" />
																	<asp:Label ID="lbtbUniformInvoiceOption2" Runat="Server" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption2 %>" Visible="false" />
																</dd>
															</dl>

															<dl id="dlUniformInvoiceOption1_3" Runat="Server" visible="false">
																<br />
																<dd>寄付先コード</dd>
																<dd>
																	<asp:TextBox ID="tbUniformInvoiceOption1_3" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Width="220" Runat="Server" MaxLength="7" />
																	<asp:CustomValidator
																		ID="cvUniformInvoiceOption1_3" Runat="Server"
																		ControlToValidate="tbUniformInvoiceOption1_3"
																		ValidationGroup="OrderShippingGlobal"
																		ValidateEmptyText="true"
																		ClientValidationFunction="ClientValidate"
																		SetFocusOnError="true"
																		CssClass="error_inline" />
																	<asp:Label ID="lbUniformInvoiceOption1_3" Text="<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>" Runat="Server" Visible="false" />
																</dd>
															</dl>
															<dl id="dlUniformInvoiceTypeRegist" Runat="Server" visible="false">
																<dd>
																	<asp:CheckBox ID="cbSaveToUserInvoice" Checked="<%# GetCartShipping(Container.DataItem).UserInvoiceRegistFlg %>" Runat="Server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
																</dd>
																<dl id="dlUniformInvoiceTypeRegistInput" Runat="Server" visible="false">
																	<dd>
																		電子発票情報名<span class="fred">※</span>
																		<asp:TextBox ID="tbUniformInvoiceTypeName" Text="<%#: GetCartShipping(Container.DataItem).InvoiceName %>" MaxLength="30" Runat="Server" />
																		<asp:CustomValidator
																			ID="cvUniformInvoiceTypeName" Runat="Server"
																			ControlToValidate="tbUniformInvoiceTypeName"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			CssClass="error_inline" />
																	</dd>
																</dl>
															</dl>
														</dl>
														<h4>共通性載具</h4>
														<dl id="divInvoiceCarryType" Runat="Server" class="userList">
															<dl>
																<dd>
																	<asp:DropDownList ID="ddlInvoiceCarryType" Runat="Server"
																		CssClass="input_border"
																		DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE) %>"
																		DataTextField="text"
																		DataValueField="value"
																		OnSelectedIndexChanged="ddlInvoiceCarryType_SelectedIndexChanged"
																		AutoPostBack="true"
																		Width="250" />
																</dd>
																<dd>
																	<asp:DropDownList ID="ddlInvoiceCarryTypeOption" Runat="Server"
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
																		<asp:CustomValidator
																			ID="cvCarryTypeOption_8"
																			Runat="Server"
																			ControlToValidate="tbCarryTypeOption_8"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			CssClass="error_inline" />
																	</div>
																	<div id="divCarryTypeOption_16" Runat="Server" visible="false">
																		<asp:TextBox ID="tbCarryTypeOption_16" Width="220" Text="<%#: GetCartShipping(Container.DataItem).CarryTypeOptionValue %>" Runat="Server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
																		<asp:CustomValidator
																			ID="cvCarryTypeOption_16"
																			Runat="Server"
																			ControlToValidate="tbCarryTypeOption_16"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			CssClass="error_inline" />
																	</div>
																</dd>
																<dd>
																	<asp:Label Runat="Server" ID="lbCarryTypeOptionText" Visible="false" Text="載具コード" />
																</dd>
																<dd>
																	<asp:Label Runat="Server" ID="lbCarryTypeOption" Visible="false" />
																</dd>
																<dl id="dlCarryTypeOptionRegist" Runat="Server" visible="false">
																	<dd>
																		<asp:CheckBox ID="cbCarryTypeOptionRegist" Runat="Server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
																	</dd>
																	<dd id="divCarryTypeOptionName" Runat="Server" visible="false">
																		電子発票情報名<span class="fred">※</span>
																		<asp:TextBox ID="tbCarryTypeOptionName" Text="<%#: GetCartShipping(Container.DataItem).InvoiceName %>" Runat="Server" MaxLength="30" />
																		<asp:CustomValidator
																			ID="cvCarryTypeOptionName" Runat="Server"
																			ControlToValidate="tbCarryTypeOptionName"
																			ValidationGroup="OrderShippingGlobal"
																			ValidateEmptyText="true"
																			ClientValidationFunction="ClientValidate"
																			SetFocusOnError="true"
																			CssClass="error_inline" />
																	</dd>
																</dl>
															</dl>
														</dl>
													</div>
													<%-- △Invoice△ --%>
												</div>
											</div>
											<h4 visible="<%# DisplayFixedPurchaseShipping(Container) %>" Runat="Server">定期購入 配送パターンの指定</h4>
											<%-- ▽デフォルトチェックの設定▽--%>
											<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
											<%#: Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container.Parent.Parent).ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container.Parent.Parent).ItemIndex, 2, 3, 1, 4) %>
											<%-- △ - - - - - - - - - - - △--%>
											<div visible="<%# DisplayFixedPurchaseShipping(Container) %>" Runat="Server" style='<%# DisplayFixedPurchaseShipping(Container) ? string.Empty : "margin-top: 0px;padding-top: 0px;" %>'>
												<div class="userList list" style="padding-bottom: 0" visible="<%# DisplayFixedPurchaseShipping(Container) %>" Runat="Server">「定期購入」はご希望の配送パターン・配送時間を指定して定期的に商品をお届けするサービスです。下記の配送パターンからお選び下さい。</div>

												<div id="<%#: "efo_sign_fixed_purchase" + Container.ItemIndex %>" class="userList list" style="padding-top: 0;" />
												<dl style="margin-top: 10px;" visible="<%# DisplayFixedPurchaseShipping(Container) %>" Runat="Server">
													<dt visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" Runat="Server">
														<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date"
															Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 1) %>"
															GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" Runat="Server" /><span id="<%#: "efo_sign_fixed_purchase_month" + Container.ItemIndex %>"></span></dt>
													<dd id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" Runat="Server">
														<asp:DropDownList ID="ddlFixedPurchaseMonth"
															DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true) %>"
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
															Runat="Server" />
														ヶ月ごと
														<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
															DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true, false, true) %>'
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
															Runat="Server" />
														日に届ける
													</dd>
													<small>
														<asp:CustomValidator
															ID="cvFixedPurchaseMonth"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseMonth"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
													</small>
													<small>
														<asp:CustomValidator
															ID="cvFixedPurchaseMonthlyDate"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseMonthlyDate"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
													</small>
													<dt visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" Runat="Server">
														<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay"
															Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 2) %>"
															GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" Runat="Server" /><span id="<%#: "efo_sign_fixed_purchase_week_and_day" + Container.ItemIndex %>"></span></dt>
													<dd id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" Runat="Server">
														<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
															DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true, true) %>"
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" Runat="Server" />
														ヶ月ごと
														<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
															DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
															Runat="Server" />
														<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
															DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
															Runat="Server" />
														に届ける
													</dd>
													<small>
														<asp:CustomValidator
															ID="cvFixedPurchaseIntervalMonths"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseIntervalMonths"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
													</small>
													<small>
														<asp:CustomValidator
															ID="cvFixedPurchaseWeekOfMonth"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseWeekOfMonth"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
													</small>
													<small>
														<asp:CustomValidator
															ID="cvFixedPurchaseDayOfWeek"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseDayOfWeek"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
														</small>
													<dt visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" Runat="Server">
														<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays"
															Text="配送日間隔指定" Checked="<%# (GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 3) && (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? (GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false).Length > 0) : (GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false).Length > 1))) %>"
															GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" Runat="Server" />
														<span id="<%#: "efo_sign_fixed_purchase_interval_days" + Container.ItemIndex %>"></span>
													</dt>
													<dd id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" Runat="Server">
														<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
															DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false) %>'
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
															Runat="Server" />
														日ごとに届ける
													</dd>
													<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%#: this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex].FixedPurchaseShippingDaysRequired %>" Runat="Server" />
													<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%#: this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex].FixedPurchaseMinimumShippingSpan %>" Runat="Server" />
													<small>
														<asp:CustomValidator ID="cvFixedPurchaseIntervalDays"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseIntervalDays"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
													</small>
													<dt visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" Runat="Server">
														<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
															Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 4) %>"
															GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" Runat="Server" /><span id="<%#: "efo_sign_fixed_purchase_week" + Container.ItemIndex %>"></span></dt>
													<dd id="ddFixedPurchaseEveryNWeek" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" Runat="Server">
														<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
															DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true) %>"
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
															Runat="Server" />
														週間ごと
														<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
															DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false) %>"
															DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
															OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
															AutoPostBack="true"
															Runat="Server" />
														に届ける
													</dd>
													<small>
														<asp:CustomValidator
															ID="cvFixedPurchaseEveryNWeek"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
													</small>
													<small>
														<asp:CustomValidator
															ID="cvFixedPurchaseEveryNWeekDayOfWeek"
															Runat="Server"
															ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
															ValidationGroup="OrderShipping"
															ClientValidationFunction="ClientValidate"
															ValidateEmptyText="true"
															SetFocusOnError="true"
															CssClass="error_inline" />
													</small>
												</dl>
												<small><p class="attention" Runat="Server" visible="<%# GetAllFixedPurchaseKbnEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex) == false %>">同時に定期購入できない商品が含まれております。</p></small>
												<small ID="sErrorMessage" class="fred" Runat="Server"></small>
												<br /><hr />
												<dl>
													<dt id="dtFirstShippingDate" visible="true" Runat="Server">初回配送予定日</dt>
													<dd visible="true" Runat="Server" style="padding-left: 20px;">
														<asp:Label ID="lblFirstShippingDate" Runat="Server" />
														<asp:Label ID="lblFirstShippingDateNoteMessage" visible="false" Runat="Server">
															<br>配送予定日は変更となる可能性がありますことをご了承ください。
														</asp:Label>
														<asp:Literal ID="lFirstShippingDateDayOfWeekNoteMessage" visible="false" Runat="Server">
															<br>曜日指定は次回配送日より適用されます。
														</asp:Literal>
													</dd>
													<dt id="dtNextShippingDate" visible="true" Runat="Server">2回目の配送日を選択</dt>
													<dd visible="true" Runat="Server" style="padding-left: 20px;">
														<asp:Label ID="lblNextShippingDate" visible="false" Runat="Server" />
														<asp:DropDownList ID="ddlNextShippingDate" visible="false" OnDataBound="ddlNextShippingDate_OnDataBound" Runat="Server" />
													</dd>
												</dl>
												<dl>
													メール便の場合は数日ずれる可能性があります。
												</dl>
											</div>
										</ItemTemplate>
									</asp:Repeater>
									<asp:LinkButton ID="lbAddShipping"
										Runat="Server"
										Visible='<%# CanAddShipping(GetCartObject(Container.DataItem)) %>'
										OnClick="lbAddShipping_Click"
										class="btn btn-mid btn-inverse add-shipping-button"
										Text="新しい配送先を追加" />
									<div class="fred userProduct" id="hcErrorMessages" enableviewstate="false" Runat="Server" visible="<%# DisplayFixedPurchaseShipping(Container) == false %>"></div>
										<asp:Repeater ID="rMemos" Runat="Server" DataSource="<%# GetCartObject(Container.DataItem).OrderMemos %>" Visible="<%# GetCartObject(Container.DataItem).OrderMemos.Count != 0 %>">
											<HeaderTemplate>
												<h4>注文メモ</h4>
												<div class="list">
											</HeaderTemplate>
											<ItemTemplate>
												<span style="font-weight: bold;"><%#: Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME) %></span>
												<p><asp:TextBox ID="tbMemo" Runat="Server" Text="<%#: Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%#: Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine" /><br /></p><br />
												<small id="sErrorMessageMemo" Runat="Server" class="fred" ></small>
												<%-- IDに"OtherValidator"を含めることで案件毎に追加したtextareaなどでチェック可能 --%>
												<asp:CustomValidator ID="OtherValidator" Runat="Server"
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
										<asp:CheckBox ID="cbOnlyReflectMemoToFirstOrder"
											Checked="<%# GetCartObject(Container.DataItem).ReflectMemoToFixedPurchase %>"
											visible="<%# (GetCartObject(Container.DataItem).OrderMemos.Count != 0) && GetCartObject(Container.DataItem).ReflectMemoToFixedPurchaseVisible %>"
											Text="2回目以降の注文メモにも追加する"
											CssClass="checkBox"
											Runat="Server" />
									</div><!--orderBox-->
									<div class="subCartList orderBox" style="margin-top: 0; border: 0 !important; <%# IsDisplayCartDetail(Container.DataItem) ? string.Empty: "display: none;" %>">
										<div style="border: 1px solid #C2CFD7;">
											<h4>カート番号<%#: Container.ItemIndex + 1 %><%#: DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）") %>のご注文内容</h4>
											<div class="block">
												<div class="orderBox" style="border: none; background: none;">
													<div class="userProduct" style="border: none;">
													<asp:Repeater ID="rCartProduct" runat="server">
														<ItemTemplate>
														<span runat="server" visible="<%# (FindCart(Container.DataItem).IsGift && (Container.ItemIndex == 0)) %>">
															※「一括変更」ボタンで、各配送先の該当商品の個数を指定した数に変更できます。
														</span>
														<div class="<%#: ((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1 ? "last" : string.Empty %>" style="width: auto; border: none;">
															<dl>
																<dt>
																	<a Runat="Server"
																		href='<%#: GetCartProduct(Container.DataItem).CreateProductDetailUrl() %>'
																		Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																		<w2c:ProductImage ProductMaster="<%# (GetCartProduct(Container.DataItem)) %>" ImageSize="S" Runat="Server" />
																	</a>
																	<w2c:ProductImage ProductMaster="<%# (GetCartProduct(Container.DataItem)) %>"
																		ImageSize="S"
																		Runat="Server"
																		Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false %>" />
																</dt>
																<dd>
																<span style="padding-top: 0; font-weight:bold;">
																	<span>
																		<a Runat="Server"
																			href='<%#: GetCartProduct(Container.DataItem).CreateProductDetailUrl() %>'
																			Visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>"
																			style="display: block; max-width: 200px;">
																			<%#: GetCartProduct(Container.DataItem).ProductJointName %>
																		</a>
																		<span Runat="Server">
																			<p class="quantity">数量：<%#: GetCartProduct(Container.DataItem).Count %></p>
																			<p class="quantity"><%#: CurrencyManager.ToPrice((GetCartProduct(Container.DataItem)).Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
																		</span>
																	</span>
																</span>
																<%#: string.IsNullOrEmpty((GetCartProduct(Container.DataItem)).GetProductTag("tag_cart_product_message")) == false ? "<small>" + GetProductCount(Container.DataItem).Product.GetProductTag("tag_cart_product_message") + "</small>" : "" %>
																<p visible='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' Runat="Server">
																	<b>
																	<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList %>' Runat="Server">
																		<ItemTemplate>
																			<%#: ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																			<%# string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false ? "<br />" : string.Empty %>
																		</ItemTemplate>
																	</asp:Repeater>
																	</b>
																</p>
															</dl>
															<span runat="server" style="margin: 5px 0; float: left;" visible="<%# FindCart(Container.DataItem).IsGift %>">
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
															<p class="clr"></p>
														</div>
														</ItemTemplate>
													</asp:Repeater>
													</div>
												</div>
												<asp:Repeater ID="rCartDetail" Runat="Server">
												<ItemTemplate>
												<div class="priceList">
													<div>
														<%-- 総配送先数 --%>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt>総配送先数</dt>
															<dd><%#: GetCartObject(Container.DataItem).Shippings.Count %></dd>
														</dl>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
															<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotal) %></dd>
														</dl>
														<% if (this.ProductIncludedTaxFlg == false) { %>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt>消費税額</dt>
															<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotalTax) %></dd>
														</dl>
														<% } %>
														<%-- セットプロモーション割引額(商品割引) --%>
														<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" Visible="<%# GetCartObject(Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">
														<ItemTemplate>
														<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt><%#: ((CartSetPromotion)Container.DataItem).SetpromotionDispName %></dt>
															<dd class='<%#: ((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0 ? "minus" : string.Empty %>'><%# ((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %></dd>
														</dl>
														</span>
														</ItemTemplate>
														</asp:Repeater>
														<% if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn) { %>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt>会員ランク割引額</dt>
															<dd class='<%#: GetCartObject(Container.DataItem).MemberRankDiscount > 0 ? "minus" : string.Empty %>'><%# GetCartObject(Container.DataItem).MemberRankDiscount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).MemberRankDiscount * (GetCartObject(Container.DataItem).MemberRankDiscount < 0 ? -1 : 1)) %></dd>
														</dl>
														<% } %>
														<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn) { %>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt>定期会員割引額</dt>
															<dd class='<%#: GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount > 0 ? "minus" : string.Empty %>'><%# GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount > 0 ? "-" : "" %><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount * (GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount < 0 ? -1 : 1)) %></dd>
														</dl>
														<% } %>
														<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt>クーポン割引額</dt>
															<dd class='<%#: GetCartObject(Container.DataItem).UseCouponPrice > 0 ? "minus" : string.Empty %>'>
																<%#: GetCouponName(GetCartObject(Container.DataItem)) %>
																<%# GetCartObject(Container.DataItem).UseCouponPrice > 0 ? "-" : string.Empty %>
																<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).UseCouponPrice * (GetCartObject(Container.DataItem).UseCouponPrice < 0 ? -1 : 1)) %>
															</dd>
														</dl>
														<% } %>
														<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
															<dt>ポイント利用額</dt>
															<dd class='<%#: GetCartObject(Container.DataItem).UsePointPrice > 0 ? "minus" : string.Empty %>'><%# GetCartObject(Container.DataItem).UsePointPrice > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice((GetCartObject(Container.DataItem)).UsePointPrice * ((GetCartObject(Container.DataItem)).UsePointPrice < 0 ? -1 : 1)) %></dd>
														</dl>
														<% } %>
														<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
														<div runat="server" visible="<%# GetCartObject(Container.DataItem).HasFixedPurchase %>">
															<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
																<dt>定期購入割引額</dt>
																<dd class='<%#: GetCartObject(Container.DataItem).FixedPurchaseDiscount > 0 ? "minus" : string.Empty %>'><%#: GetCartObject(Container.DataItem).FixedPurchaseDiscount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice((GetCartObject(Container.DataItem)).FixedPurchaseDiscount * ((GetCartObject(Container.DataItem)).FixedPurchaseDiscount < 0 ? -1 : 1)) %></dd>
															</dl>
														</div>
														<% } %>
														<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
														<dt>配送料金</dt>
														<dd runat="server" style='<%#: GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg ? "display: none;" : string.Empty %>'>
															<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceShipping) %></dd>
														<dd runat="server" style='<%#: GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg == false ? "display: none;" : string.Empty %>'>
															<%#: GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateMessage %></dd>
														</dl>
														</dl>
														<span runat="server" Visible="<%# GetCartObject(Container.DataItem).Payment != null %>">
														<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
															<dt>決済手数料</dt>
															<dd><%#: (GetCartObject(Container.DataItem).Payment != null) ? CurrencyManager.ToPrice(GetCartObject(Container.DataItem).Payment.PriceExchange) : "" %></dd>
														</dl>
														</span>
														<%-- セットプロモーション割引額(配送料割引) --%>
														<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
														<ItemTemplate>
															<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
															<dl class='<%: this.DispNum++ % 2 == 0 ? string.Empty : "bgc" %>'>
																<dt><%#: ((CartSetPromotion)Container.DataItem).SetpromotionDispName %>(送料割引)</dt>
																<dd class='<%#: ((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0 ? "minus" : string.Empty %>'><%# ((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0 ? "-" : string.Empty %><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %></dd>
															</dl>
															</span>
														</ItemTemplate>
														</asp:Repeater>
													</div>
													<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" /></p>
													<div>
														<dl class="result">
															<dt>合計(税込)</dt>
															<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceTotal) %></dd>
														</dl>
													</div>
												</div><!--priceList-->
												</ItemTemplate>
												</asp:Repeater>
											</div><!--block-->
										</div><!--bottom-->
									</div><!--subCartList-->
								</div><!--columnRight-->
								<%-- ▲配送先情報▲ --%>
						</ItemTemplate>
					</asp:Repeater>
					<br class="clr" />
				</div><!--submain-->
			</div><!--main-->

			<%-- UpdatePanel外のイベントを実行したいためこのような呼び出し方となっている --%>
			<div class="btmbtn below">
				<ul>
					<li><a onclick="<%: this.BackOnClick %>" href="<%: this.BackEvent %>" class="btn btn-large btn-org-gry">前のページに戻る</a></li>
					<li><a href="<%: this.NextEvent %>" class="btn btn-large btn-success"><%: this.IsNextConfirmPage ? "ご注文内容確認へ" : "お支払方法入力へ" %></a></li>
				</ul>
			</div>

			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
			<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none;" Runat="Server" />
			<asp:HiddenField ID="hfResetAuthenticationCode" Runat="Server" />
			<% } %>
		</ContentTemplate>
		<Triggers>
			<asp:PostBackTrigger ControlID="rCartList"/>
		</Triggers>
	</asp:UpdatePanel>
	<%-- UPDATE PANELここまで --%>
</div>

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
		<% foreach (RepeaterItem ri in this.WrCartList.Items) { %>
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

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search owner zip global
		textboxChangeSearchGlobalZip(
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZipGlobal").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrOwnerFromZipGlobal").UniqueID %>');
		// Textbox change search shippping zip global
		textboxChangeSearchGlobalZip(
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZipGlobal").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrShippingFromZipGlobal").UniqueID %>');
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
		clickSearchZipCodeInRepeater(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip2").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchShippingAddr").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchShippingAddr").UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (riShipping.FindControl("sShippingZipError")).ClientID %>',
			"shipping");

		// Check shipping zip code input on text box change
		textboxChangeSearchZipCodeInRepeater(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip1").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip2").ClientID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip1").UniqueID %>',
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZip2").UniqueID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchShippingAddr").ClientID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'<%= '#' + (riShipping.FindControl("sShippingZipError")).ClientID %>',
			"shipping");

		if (multiAddrsearchTriggerType == "shipping") {
			bindTargetForAddr1 = "<%= ((DropDownList)riShipping.FindControl("ddlShippingAddr1")).ClientID %>";
			bindTargetForAddr2 = "<%= ((TextBox)riShipping.FindControl("tbShippingAddr2")).ClientID %>";
			bindTargetForAddr3 = "<%= ((TextBox)riShipping.FindControl("tbShippingAddr3")).ClientID %>";
		}

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search shipping zip
		textboxChangeSearchGlobalZip(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbShippingZipGlobal").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchAddrShippingFromZipGlobal").UniqueID %>');

		// Textbox change search sender zip global
		textboxChangeSearchGlobalZip(
			'<%= GetWrappedTextBoxFromRepeater(riShipping, "tbSenderZipGlobal").ClientID %>',
			'<%= GetWrappedLinkButtonFromRepeater(riShipping, "lbSearchAddrSenderFromZipGlobal").UniqueID %>');
		<% } %>
		<% } %>
		<% } %>
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
		isOwnerAddrCountryJp = (document.getElementById('<%= this.WddlOwnerCountry.ClientID %>').value == '<%= Constants.COUNTRY_ISO_CODE_JP %>');
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

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
</asp:Content>
