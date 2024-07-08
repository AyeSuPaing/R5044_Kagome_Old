<%--
=========================================================================================================
  Module      : 注文配送先入力画面(OrderShipping.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="EcPayScript" Src="~/Form/Common/ECPay/EcPayScript.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderShipping.aspx.cs" Inherits="Form_Order_OrderShipping" Title="配送先情報入力ページ" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
<% if(Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
<uc:EcPayScript runat="server" ID="ucECPayScript" />
<% } %>
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
<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
<style>
	.convenience-store-item {
		margin: 2px;
		padding: 4px;
	}

	.convenience-store-button {
		padding: 12px 17px;
		font-size: 16px;
		text-decoration: none !important;
	}
</style>
<% } %>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<p id="CartFlow"><img src="../../Contents/ImagesPkg/order/cart_step01.gif" alt="お届け先情報入力" width="781" height="58" /></p>
<div class="btmbtn above cartstep">
	<h2 class="ttlA">
	<%if (Constants.GIFTORDER_OPTION_ENABLED){ %>
	お届け先情報入力
	<%} else { %>
	お届け先情報入力
	<%} %>
	</h2>
	<ul>
		<li><a onclick="<%= this.NextOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>" class="btn btn-success"><%: (this.IsNextConfirmPage) ? "ご注文内容確認へ" : "お支払方法入力へ" %></a></li>
	</ul>
</div>

<%-- エラーメッセージ（デフォルト注文方法用） --%>
<span style="color:red;text-align:center;display:block;"><asp:Literal ID="lOrderErrorMessage" runat="server"></asp:Literal></span>
	
<div id="CartList">

<div class="main clearFix" style="margin-bottom:0;">
<div class="submain">
<div class="column">
<h2><img src="../../Contents/ImagesPkg/order/sttl_user.gif" alt="注文者情報" width="80" height="16" /></h2>
<%if (this.IsEasyUser) {%>
<p style="margin:5px;padding:5px;text-align:center;background-color:#ffff80;border:1px solid #D4440D;border-color:#E5A500;color:#CC7600;">ご購入手続きに必要な会員情報が不足しています。</p>
<%} %>
<p>以下の項目をご入力ください。<br />
<%if (this.IsLoggedIn) {%>
（入力した注文者情報で会員情報が更新されます。）<br />
<%} %></p>
<span class="fred">※</span>&nbsp;は必須入力です。<br /></div><!--column-->

<%if (Constants.GIFTORDER_OPTION_ENABLED == false) { %>
<div class="columnRight">
<h2><img src="../../Contents/ImagesPkg/order/sttl_esd.gif" alt="配送先情報" width="80" height="16" /></h2>
<p>「注文者情報」で入力した住所宛にお届けする場合は、以下の「注文者情報の住所へ配送する」を選択してください。<br /><span class="fred">※</span>&nbsp;は必須入力です。</p>
</div><!--columnRight-->
<%} %>

<br class="clr" />
</div><!--submain-->
</div><!--main-->

<%-- 次へイベント用リンクボタン --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" runat="server"></asp:LinkButton>
<%-- 戻るイベント用リンクボタン --%>
<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server"></asp:LinkButton>

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<% this.CartItemIndexTmp = -1; %>

<div class="main" style="margin-top:0;">
<div class="submain">
<asp:Repeater id="rCartList" Runat="server">
<ItemTemplate>
<%-- ▼注文者情報▼ --%>
<div id="divOwnerColumn" class="column" visible='<%# Container.ItemIndex == 0 %>' runat="server">
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
		<%-- 注文者：氏名（かな） --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_kana"/>
		</dt>
		<dd class="<%= ReplaceTag("@@User.name_kana.type@@") %>">
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
		<%} %>
		<%-- 注文者：生年月日 --%>
		<dt>
			<%: ReplaceTag("@@User.birth.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<%if (this.IsLoggedIn) {%><span class="fred">※</span><span id="efo_sign_birth"/><% } %>
		</dt>
		<dd>
		<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue='<%# (this.CartList.Owner.Birth.HasValue ) ? this.CartList.Owner.BirthYear.ToString() : "" %>' CssClass="input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;年&nbsp;&nbsp;
		<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue='<%# (this.CartList.Owner.Birth.HasValue ) ? this.CartList.Owner.Birth.Value.Month.ToString() : "" %>' CssClass="input_widthA input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;月&nbsp;&nbsp;
		<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue='<%# (this.CartList.Owner.Birth.HasValue) ? this.CartList.Owner.Birth.Value.Day.ToString() : "" %>' CssClass="input_widthA input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;日
		<small>
		<asp:CustomValidator
			ID="cvOwnerBirth"
			runat="Server"
			ControlToValidate="ddlOwnerBirthDay"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			EnableClientScript="false"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：性別 --%>
		<dt>
			<%: ReplaceTag("@@User.sex.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<%if (this.IsLoggedIn) {%><span class="fred">※</span><% } %>
		</dt>
		<dd class="input_align">
		<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%# GetCorrectSexForDataBind(this.CartList.Owner.Sex) %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" CellSpacing="5" RepeatLayout="Flow" CssClass="input_radio" runat="server" />
		<small>
		<asp:CustomValidator
			ID="cvOwnerSex"
			runat="Server"
			ControlToValidate="rblOwnerSex"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			EnableClientScript="false"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：PCメールアドレス --%>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_mail_addr"/>
		</dt>
		<dd><asp:TextBox ID="tbOwnerMailAddr" Text="<%# this.CartList.Owner.MailAddr %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" runat="server" Type="email"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerMailAddr"
			runat="Server"
			ControlToValidate="tbOwnerMailAddr"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
		<asp:CustomValidator
			ID="cvOwnerMailAddrForCheck"
			runat="Server"
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
		<%-- 注文者：モバイルメールアドレス --%>
		<dt>
			<%: ReplaceTag("@@User.mail_addr2.name@@", ownerAddrCountryIsoCode) %>
		</dt>
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
		<%-- 注文者：モバイルメールアドレス（確認用） --%>
		<dt>
			<%: ReplaceTag("@@User.mail_addr2.name@@", ownerAddrCountryIsoCode) %>（確認用）
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
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<%-- 注文者：国 --%>
		<dt>
			<%: ReplaceTag("@@User.country.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span>
		</dt>
		<dd>
			<asp:DropDownList id="ddlOwnerCountry" runat="server" AutoPostBack="true" SelectedValue="<%# this.CartList.Owner.AddrCountryIsoCode %>" DataSource="<%# this.UserCountryDisplayList %>" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerCountry_SelectedIndexChanged"/><br/>
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
			<span id="countryAlertMessage" class="notes" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
		</dd>
		<% } %>
		<%-- 注文者：郵便番号 --%>
		<% if (isOwnerAddrCountryJp) {%>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_zip"/>
		</dt>
		<dd>
		<p class="pdg_topC">
		<asp:TextBox ID="tbOwnerZip" OnTextChanged="lbSearchOwnergAddr_Click" Text="<%# this.CartList.Owner.Zip %>" CssClass="input_widthC input_border" Type="tel" MaxLength="8" runat="server" /></p>
		<span class="btn_add_sea"><asp:LinkButton ID="lbSearchOwnergAddr" runat="server" onclick="lbSearchOwnergAddr_Click" class="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span>
		<%--検索結果レイヤー--%>
		<uc:Layer ID="ucLayerForOwner" runat="server" />
		<p class="clr">
		<small class="fred">
		<asp:CustomValidator
			ID="cvOwnerZip1"
			runat="Server"
			ControlToValidate="tbOwnerZip"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline cvOwnerZipShortInput" /></small>
		<small id="sOwnerZipError" runat="server" class="fred sOwnerZipError"></small>
		</p></dd>
		<%-- 注文者：都道府県 --%>
		<dt>
			<%: ReplaceTag("@@User.addr1.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_addr1"/>
		</dt>
		<dd><asp:DropDownList ID="ddlOwnerAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# this.CartList.Owner.Addr1 %>" runat="server"></asp:DropDownList>
		<small>
		<asp:CustomValidator
			ID="cvOwnerAddr1"
			runat="Server"
			ControlToValidate="ddlOwnerAddr1"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } %>

		<%-- 注文者：市区町村 --%>
		<dt>
			<%: ReplaceTag("@@User.addr2.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><% if (isOwnerAddrCountryJp) { %><span id="efo_sign_addr2"/><% } %>
		</dt>
		<dd>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged"></asp:DropDownList>
			<% } else { %>
				<asp:TextBox ID="tbOwnerAddr2" Text="<%# this.CartList.Owner.Addr2 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerAddr2"
			runat="Server"
			ControlToValidate="tbOwnerAddr2"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		<% } %>
		</dd>
		<%-- 注文者：番地 --%>
		<dt>
			<%: ReplaceTag("@@User.addr3.name@@", ownerAddrCountryIsoCode) %>
			<% if (IsAddress3Necessary(ownerAddrCountryIsoCode)){ %>&nbsp;<span class="fred">※</span><span id="efo_sign_addr3"/><% } %>
		</dt>
		<dd>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
			<% } else { %>
				<asp:TextBox ID="tbOwnerAddr3" Text="<%# this.CartList.Owner.Addr3 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerAddr3"
			runat="Server"
			ControlToValidate="tbOwnerAddr3"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		<% } %>
		</dd>
		<%-- 注文者：ビル・マンション名 --%>
		<dt>
			<%: ReplaceTag("@@User.addr4.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrCountryJp == false) { %><span class="fred">*</span><% } %>
		</dt>
		<dd><asp:TextBox ID="tbOwnerAddr4" Text="<%# this.CartList.Owner.Addr4 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerAddr4"
			runat="Server"
			ControlToValidate="tbOwnerAddr4"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：州  --%>
		<% if (isOwnerAddrCountryJp == false) { %>
		<dt>
			<%: ReplaceTag("@@User.addr5.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrCountryUs) { %>&nbsp;<span class="fred">*</span><% } %>
		</dt>
		<dd>
			<% if (isOwnerAddrCountryUs) { %>
			<asp:DropDownList runat="server" ID="ddlOwnerAddr5" DataSource="<%# this.UserStateList %>"></asp:DropDownList>
			<asp:CustomValidator
				ID="cvOwnerAddr5Ddl"
				runat="Server"
				ControlToValidate="ddlOwnerAddr5"
				ValidationGroup="OrderShippingGlobal"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
			<% } else { %>
			<asp:TextBox runat="server" ID="tbOwnerAddr5" Text="<%# this.CartList.Owner.Addr5 %>" ></asp:TextBox>
			<asp:CustomValidator
				ID="cvOwnerAddr5"
				runat="Server"
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
			<asp:TextBox ID="tbOwnerZipGlobal" Text="<%# this.CartList.Owner.Zip %>" MaxLength="20" runat="server" Type="tel"></asp:TextBox>
			<asp:CustomValidator
				ID="cvOwnerZipGlobal"
				runat="Server"
					ControlToValidate="tbOwnerZipGlobal"
					ValidationGroup="OrderShippingGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
			<asp:LinkButton
				ID="lbSearchAddrOwnerFromZipGlobal"
				OnClick="lbSearchAddrOwnerFromZipGlobal_Click"
				Style="display:none;"
				Runat="server" />
		</dd>
		<% } %>

		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<%-- 注文者：企業名 --%>
		<dt>
			<%: ReplaceTag("@@User.company_name.name@@") %>
			&nbsp;<span class="fred"></span>
		</dt>
		<dd><asp:TextBox ID="tbOwnerCompanyName" Text="<%# this.CartList.Owner.CompanyName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerCompanyName"
			runat="Server"
			ControlToValidate="tbOwnerCompanyName"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：部署名 --%>
		<dt>
			<%: ReplaceTag("@@User.company_post_name.name@@") %>
			&nbsp;<span class="fred"></span>
		</dt>
		<dd><asp:TextBox ID="tbOwnerCompanyPostName" Text="<%# this.CartList.Owner.CompanyPostName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerCompanyPostName"
			runat="Server"
			ControlToValidate="tbOwnerCompanyPostName"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%} %>

		<%-- 注文者：電話番号1 --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_tel1"/>
		</dt>
		<dd>
			<asp:TextBox ID="tbOwnerTel1" Text="<%#: this.CartList.Owner.Tel1 %>" CssClass="input_widthC input_border shortTel" MaxLength="13" Type="tel" runat="server" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
			<span class="btn_add_sea">
				<asp:LinkButton
					ID="lbGetAuthenticationCode"
					class="btn btn-mini"
					runat="server"
					Text="認証コードの取得"
					OnClick="lbGetAuthenticationCode_Click"
					OnClientClick="return checkTelNoInput();" />
			</span>
			<p><asp:Label ID="lbAuthenticationStatus" runat="server" /></p>
			<% } %>
		<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
			<!-- mobile phone if use GMO payment -->
			<small>
				<span class="warning_inline"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_INPUT_GMO_KB_MOBILE_PHONE) %></span>
			</small>
		<% } %>
		<small>
		<asp:CustomValidator
			ID="cvOwnerTel1_1"
			runat="Server"
			ControlToValidate="tbOwnerTel1"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
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
			<%: ReplaceTag("@@User.tel2.name@@", ownerAddrCountryIsoCode) %>
		</dt>
		<dd>
			<asp:TextBox ID="tbOwnerTel2" Text="<%#: this.CartList.Owner.Tel2 %>" CssClass="input_widthD input_border shortTel" MaxLength="13" Type="tel" runat="server" /><br />
			<small>
				<asp:CustomValidator
					ID="cvOwnerTel2_1"
					runat="Server"
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
			<asp:TextBox ID="tbOwnerTel1Global" Text="<%# this.CartList.Owner.Tel1 %>" MaxLength="30" runat="server" Type="tel" onchange="resetAuthenticationCodeInput('cvOwnerTel1Global')" />
			<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
			<span class="btn_add_sea">
				<asp:LinkButton
					ID="lbGetAuthenticationCodeGlobal"
					class="btn btn-mini"
					runat="server"
					Text="認証コードの取得"
					OnClick="lbGetAuthenticationCode_Click"
					OnClientClick="return checkTelNoInput();" />
				<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
			</span>
			<% } %>
			<small>
				<asp:CustomValidator
					ID="cvOwnerTel1Global"
					runat="Server"
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
			<asp:TextBox
				ID="tbAuthenticationCodeGlobal"
				CssClass="input_widthA input_border"
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
			<asp:TextBox ID="tbOwnerTel2Global" Text="<%# this.CartList.Owner.Tel2 %>" MaxLength="30" runat="server" Type="tel"></asp:TextBox>
			<small>
				<asp:CustomValidator
					ID="cvOwnerTel2Global"
					runat="Server"
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
		<dd><asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text=" 配信する" CssClass="checkBox" runat="server" /></dd>
		</dl>
	</div><!--bottom-->
	</div><!--top-->
	</div><!--userBox-->
<% this.CartItemIndexTmp = -1; %>
</div><!--column-->
<%-- ▲注文者情報▲ --%>

<%-- ▼配送先情報▼ --%>
<%if (Constants.GIFTORDER_OPTION_ENABLED == false) { %>
<div class="columnRight" visible='<%# Container.ItemIndex == 0 %>'>
	<div class="orderBox">
	<h3>
		<div class="cartNo">カート番号<%# Container.ItemIndex + 1 %><%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）"))%></div>
		<div class="cartLink"><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>">カートへ戻る</a></div>
	</h3>
	<div class="bottom">
	<%
		this.CartItemIndexTmp++;
	%>
	<div class="userProduct">
		<asp:Repeater id="rCart" DataSource="<%# ((CartObject)Container.DataItem).Items %>" Runat="server">
		<ItemTemplate>
			<div class="<%# (((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1) ? "last" : "" %>">
			<dl>
			<dt>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="S" runat="server" /></a>
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="S" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
			</dt>
			<dd>
				<strong>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
					<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
				</strong>
				<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</small>" : "" %>
			<p id="P1" visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
				<b>
				<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
					<ItemTemplate>
						<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
						<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
					</ItemTemplate>
				</asp:Repeater>
				</b>
			</p>
			<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
				<p>
					<%#: ((CartProduct)Container.DataItem).ProductOptionSettingList.HasOptionPrice
						? ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem)
						: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %>
				</p>
			<% } %>
			</dl>
			<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
			</div>
		</ItemTemplate>
		</asp:Repeater>
	</div><!--userProduct-->

	<asp:HiddenField id="hcShowShippingInputForm" value="<%# CanInputShippingTo(Container.ItemIndex) %>" runat="server" />

	<div id="divShipToCart1Address" class="userList" Visible="<%# CanInputShippingTo(Container.ItemIndex) && (Container.ItemIndex != 0) %>" runat="server">
		<asp:CheckBox id="cbShipToCart1Address" Text="カート１の配送先へ配送する" OnCheckedChanged="cbShipToCart1Address_OnCheckedChanged" AutoPostBack="true" Checked="<%# ((CartObject)Container.DataItem).Shippings[0].IsSameShippingAsCart1 %>" CssClass="checkBox" runat="server" />
		<span style="color:red;display:block;"><asp:Literal ID="lShipToCart1AddressInvalidMessage" runat="server" /></span>
	</div>

	<div id="divShippingInputForm" class="userList" runat="server">

		配送先を選択して下さい。<br /><br />
		<asp:DropDownList ID="ddlShippingKbnList" DataSource="<%# GetShippingKbnList(Container.ItemIndex) %>" DataTextField="text" DataValueField="value" SelectedValue="<%# ((CartObject)Container.DataItem).Shippings[0].ShippingAddrKbn %>" OnSelectedIndexChanged="ddlShippingKbnList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList><br />
		<span style="color:red;display:block;"><asp:Literal ID="lStorePickUpInvalidMessage" runat="server" /></span>
		<span style="color:red;display:block;"><asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal></span><br />
		<span id='<%# "spErrorConvenienceStore" + Container.ItemIndex.ToString() %>' style="color:red;display:block;"></span>
		<div id="divShippingInputFormConvenience" class="<%# Container.ItemIndex %>" runat="server">
			<dl>
				<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
				<dd runat="server" id="ddShippingReceivingStoreType">
					<asp:DropDownList ID="ddlShippingReceivingStoreType" DataSource="<%# ShippingReceivingStoreType() %>" DataTextField="text" DataValueField="value" DataMember="<%# Container.ItemIndex %>" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged"></asp:DropDownList><br />
				</dd>
				<% } %>
				<dd style="color:red;display:block;"><asp:Literal ID="lConvenienceStoreErrorMessage" runat="server"></asp:Literal></dd>
				<dd>購入金額<%# CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE)%>以上、または<%#: StringUtility.ToEmpty(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[0]) %>kg以上の商品は指定しないでください</dd>
				<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
					<dd>コンビニがセブンイレブンの場合<%#: StringUtility.ToEmpty(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[1]) %>kg以上です。</dd>
				<% } %>
				<div id="divButtonOpenConvenienceStoreMapPopup" runat="server">
					<dd runat="server" visible='<%# (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false) %>'>
						<a href="javascript:openConvenienceStoreMapPopup(<%# Container.ItemIndex %>);" class="btn btn-success convenience-store-button">Family/OK/Hi-Life</a>
					</dd>
					<dd runat="server" visible='<%# Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED %>'>
						<asp:LinkButton
							ID="lbOpenEcPay"
							runat="server"
							class="btn btn-success convenience-store-button"
							OnClick="lbOpenEcPay_Click"
							CommandArgument="<%# Container.ItemIndex %>"
							Text="  電子マップ  " />
					</dd>
				</div>
				<dd>
					<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG) %>" />
				</dd>
				<dd>
					<dt id="ddCvsShopId">
						<%: ReplaceTag("@@DispText.shipping_convenience_store.shopId@@") %><br />
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopId" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%#: GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>" />
					</dt>
					<dt id="ddCvsShopName">
						<%: ReplaceTag("@@DispText.shipping_convenience_store.shopName@@") %><br />
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopName" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" />
					</dt>
					<dt id="ddCvsShopAddress">
						<%: ReplaceTag("@@DispText.shipping_convenience_store.shopAddress@@") %><br />
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>" />
					</dt>
					<dt id="ddCvsShopTel">
						<%: ReplaceTag("@@DispText.shipping_convenience_store.shopTel@@") %><br />
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" />
					</dt>
				</dd>
				<dt>＜コンビニ受取の際の注意事項＞</br>
					注文者情報は必ず正しい「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>」と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」を入力してください。（ショートメールが受け取れる電話番号を入力する必要があります）
					コンビニで商品を受け取る際、店舗ではお客様の「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」下3桁を確認します。
				</dt> </br>
			</dl>
		</div>
		<%-- ▽配送先表示▽ --%>
		<div id="divShippingDisp" visible="<%# GetShipToOwner(((CartObject)Container.DataItem).Shippings[0]) %>" runat="server">
			<%
				var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartItemIndexTmp);
				var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
				var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
				var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
			%>
			<dl>
				<%-- 配送先：氏名 --%>
				<dt>
					<%: ReplaceTag("@@User.name.name@@", shippingAddrCountryIsoCode) %>
				</dt>
				<dd>
				<asp:Literal ID="lShippingName1" runat="server"></asp:Literal><asp:Literal ID="lShippingName2" runat="server"></asp:Literal>&nbsp;様
				<%if (IsCountryJp(this.CountryIsoCode)) {%>（<asp:Literal ID="lShippingNameKana1" runat="server"></asp:Literal><asp:Literal ID="lShippingNameKana2" runat="server"></asp:Literal>&nbsp;さま）<%} %>
					<br />
				</dd>
				<%-- 配送先：住所 --%>
				<dt>
					<%: ReplaceTag("@@User.addr.name@@") %>
				</dt>
				<dd>
				<%if (IsCountryJp(this.CountryIsoCode)) {%>〒<asp:Literal ID="lShippingZip" runat="server"></asp:Literal><br /><%} %>
					<asp:Literal ID="lShippingAddr1" runat="server"></asp:Literal> <asp:Literal ID="lShippingAddr2" runat="server"></asp:Literal><br />
					<asp:Literal ID="lShippingAddr3" runat="server"></asp:Literal> <asp:Literal ID="lShippingAddr4" runat="server"></asp:Literal> 
					<asp:Literal ID="lShippingAddr5" runat="server"></asp:Literal><br />
					<%if (IsCountryJp(this.CountryIsoCode) == false) {%><asp:Literal ID="lShippingZipGlobal" runat="server"></asp:Literal><br /><%} %>
					<asp:Literal ID="lShippingCountryName" runat="server"></asp:Literal>
				</dd>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<%-- 配送先：企業名・部署名 --%>
				<dt><%: ReplaceTag("@@User.company_name.name@@") %>・<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
				<dd>
				<asp:Literal ID="lShippingCompanyName" runat="server"></asp:Literal>&nbsp<asp:Literal ID="lShippingCompanyPostName" runat="server"></asp:Literal>
				</dd>
				<%} %>
				<%-- 配送先：電話番号 --%>
				<dt><%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %></dt>
				<dd>
				<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
				</dd>
			</dl>
		</div>
		<%-- △配送先表示△ --%>

		<%-- ▽配送先入力フォーム▽ --%>
		<div id="divShippingInputFormInner" visible="<%# GetShipToOwner(((CartObject)Container.DataItem).Shippings[0]) == false %>" class="<%# Container.ItemIndex %>" runat="server">
			<div id="divShippingVisibleConvenienceStore" class="<%# Container.ItemIndex %>" runat="server">
			<%
				var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartItemIndexTmp);
				var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
				var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
				var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
				var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
			%>
			<dl>
				<%-- 配送先：氏名 --%>
				<dt>
					<%: ReplaceTag("@@User.name.name@@", shippingAddrCountryIsoCode) %>
					&nbsp;<span class="fred">※</span><span id="<%# "efo_sign_ship_name" + Container.ItemIndex %>"/>
				</dt>
				<dd>
				姓&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="tbShippingName1" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
				名&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="tbShippingName2" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvShippingName1"
					runat="Server"
					ControlToValidate="tbShippingName1"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
				<asp:CustomValidator
					ID="cvShippingName2"
					runat="Server"
					ControlToValidate="tbShippingName2"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				</dd>
				<%-- 配送先：氏名（かな） --%>
				<% if (isShippingAddrCountryJp) { %>
				<dt>
					<%: ReplaceTag("@@User.name_kana.name@@", shippingAddrCountryIsoCode) %>
					&nbsp;<span class="fred">※</span><span id="<%# "efo_sign_ship_kana" + Container.ItemIndex %>"/>
				</dt>
				<dd>
				姓&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana1"  Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
				名&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana2"  Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvShippingNameKana1"
					runat="Server"
					ControlToValidate="tbShippingNameKana1"
					ClientValidationFunction="ClientValidate"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ValidationGroup="OrderShipping"
					CssClass="error_inline" />
				<asp:CustomValidator
					ID="cvShippingNameKana2"
					runat="Server"
					ControlToValidate="tbShippingNameKana2"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				</dd>
				<% } %>
				<%-- 配送先：国  --%>
				<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
				<dt>
					<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
				</dt>
				<dd>
					<asp:DropDownList id="ddlShippingCountry" runat="server" AutoPostBack="true" DataSource="<%# this.ShippingAvailableCountryDisplayList %>" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"></asp:DropDownList>
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
					<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
					&nbsp;<span class="fred">※</span><span id="<%# "efo_sign_ship_zip" + Container.ItemIndex %>"/>
				</dt>
				<dd>
				<p class="pdg_topC">
				<asp:TextBox ID="tbShippingZip" OnTextChanged="lbSearchShippingAddr_Click" Text="<%#: GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" CssClass="input_widthC input_border" Type="tel" MaxLength="8" runat="server" />
				<span class="btn_add_sea"><asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" class="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span>
				<p class="clr">
				<small class="fred">
				<asp:CustomValidator
					ID="cvShippingZip1"
					runat="Server"
					ControlToValidate="tbShippingZip"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline cvShippingZipShortInput" /></small>
				<small id="sShippingZipError" runat="server" class="fred sShippingZipError"></small>
				</p></dd>
				<% } %>
				<% if (isShippingAddrCountryJp) { %>
				<%-- 配送先：都道府県 --%>
				<dt>
					<%: ReplaceTag("@@User.addr1.name@@", shippingAddrCountryIsoCode) %>
					&nbsp;<span class="fred">※</span><span id="<%# "efo_sign_ship_addr1" + Container.ItemIndex %>"/>
				</dt>
				<dd><asp:DropDownList ID="ddlShippingAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1) %>" runat="server"></asp:DropDownList>
				<small>
				<asp:CustomValidator
					ID="cvShippingAddr1"
					runat="Server"
					ControlToValidate="ddlShippingAddr1"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				</dd>
				<% } %>
				<%-- 配送先：市区町村 --%>
				<dt>
					<%: ReplaceTag("@@User.addr2.name@@", shippingAddrCountryIsoCode) %>
					&nbsp;<span class="fred">※</span><% if (isShippingAddrCountryJp) { %><span id="<%# "efo_sign_ship_addr2" + Container.ItemIndex %>"/><% } %>
				</dt>
				<dd>
					<% if (isShippingAddrCountryTw) { %>
						<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
					<% } else { %>
						<asp:TextBox ID="tbShippingAddr2" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvShippingAddr2"
					runat="Server"
					ControlToValidate="tbShippingAddr2"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				<% } %>
				</dd>
				<%-- 配送先：番地 --%>
				<dt>
					<%: ReplaceTag("@@User.addr3.name@@", shippingAddrCountryIsoCode) %>
					<% if (IsAddress3Necessary(shippingAddrCountryIsoCode)){ %>&nbsp;<span class="fred">※</span><span id="<%# "efo_sign_ship_addr3" + Container.ItemIndex %>"/><% } %>
				</dt>
				<dd>
					<% if (isShippingAddrCountryTw) { %>
						<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField="Key" DataValueField="Value" Width="95" ></asp:DropDownList>
					<% } else { %>
						<asp:TextBox ID="tbShippingAddr3" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvShippingAddr3"
					runat="Server"
					ControlToValidate="tbShippingAddr3"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				<% } %>
				</dd>
				<%-- 配送先：ビル・マンション名 --%>
				<dt>
					<%: ReplaceTag("@@User.addr4.name@@", shippingAddrCountryIsoCode) %>
					<% if (isShippingAddrCountryJp == false) { %>&nbsp;<span class="fred">※</span><% } %>
				</dt>
				<dd><asp:TextBox ID="tbShippingAddr4" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvShippingAddr4"
					runat="Server"
					ControlToValidate="tbShippingAddr4"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				</dd>
				<%-- 配送先：州 --%>
				<% if (isShippingAddrCountryJp == false) { %>
				<dt>
					<%: ReplaceTag("@@User.addr5.name@@", shippingAddrCountryIsoCode) %>
					<% if (isShippingAddrCountryUs) { %>&nbsp;<span class="fred">※</span><% } %>
				</dt>
				<dd>
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
						CssClass="error_inline" />
					<% } else { %>
					<asp:TextBox ID="tbShippingAddr5" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox>
					<small>
					<asp:CustomValidator
						ID="cvShippingAddr5"
						runat="Server"
						ControlToValidate="tbShippingAddr5"
						ValidationGroup="OrderShippingGlobal"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
					</small>
					<% } %>
				</dd>
				<% } %>
				<%-- 配送先：郵便番号（海外向け） --%>
				<% if (isShippingAddrCountryJp == false) { %>
				<dt>
					<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
					<% if (isShippingAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
				</dt>
				<dd>
					<asp:TextBox ID="tbShippingZipGlobal" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" MaxLength="20" runat="server" Type="tel"></asp:TextBox>
					<small>
					<asp:CustomValidator
						ID="cvShippingZipGlobal"
						runat="Server"
						ControlToValidate="tbShippingZipGlobal"
						ValidationGroup="OrderShippingGlobal"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" /></small>
					<asp:LinkButton
						ID="lbSearchAddrShippingFromZipGlobal"
						OnClick="lbSearchAddrShippingFromZipGlobal_Click"
						Style="display:none;"
						Runat="server" />
				</dd>
				<% } %>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<%-- 配送先：企業名 --%>
				<dt>
					<%: ReplaceTag("@@User.company_name.name@@")%>
					&nbsp;<span class="fred"></span>
				</dt>
				<dd><asp:TextBox ID="tbShippingCompanyName" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvShippingCompanyName"
					runat="Server"
					ControlToValidate="tbShippingCompanyName"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				</dd>
				<%-- 配送先：部署名 --%>
				<dt>
					<%: ReplaceTag("@@User.company_post_name.name@@")%>
					&nbsp;<span class="fred"></span>
				</dt>
				<dd><asp:TextBox ID="tbShippingCompanyPostName" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbShippingCompanyPostName"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				</dd>
				<%} %>
				<%-- 配送先：電話番号 --%>
				<% if (isShippingAddrCountryJp) { %>
				<dt>
					<%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %>
					&nbsp;<span class="fred">※</span><span id="<%# "efo_sign_ship_tel1" + Container.ItemIndex %>"/>
				</dt>
				<dd><asp:TextBox ID="tbShippingTel1" Text="<%#: GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" CssClass="input_widthD input_border shortTel" Type="tel" MaxLength="13" runat="server" />
				<small>
				<asp:CustomValidator
					ID="cvShippingTel1_1"
					runat="Server"
					ControlToValidate="tbShippingTel1"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
				</dd>
				<% } else { %>
				<%-- 配送先：電話番号1（海外向け） --%>
				<dt>
					<%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %>
					&nbsp;<span class="fred">※</span>
				</dt>
				<dd>
					<asp:TextBox ID="tbShippingTel1Global" Text="<%# GetShippingValue((CartObject)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" MaxLength="30" runat="server" Type="tel"></asp:TextBox>
					<small>
					<asp:CustomValidator
						ID="cvShippingTel1Global"
						runat="Server"
						ControlToValidate="tbShippingTel1Global"
						ValidationGroup="OrderShippingGlobal"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" /></small>
				</dd>
				<% } %>
			</dl>
			</div>
			<dl id="divSaveToUserShipping" class="subbox" visible="<%# this.IsLoggedIn %>" runat="server">
				<p>
				<%-- ポストバックCustomValidatorの状態がクリアされてしまうため、JaavScirptで表示非表示を制御する --%>
				<asp:RadioButtonList ID="rblSaveToUserShipping" OnSelectedIndexChanged="rblSaveToUserShipping_OnSelectedIndexChanged" AutoPostBack="true" SelectedValue='<%# ((CartObject)Container.DataItem).Shippings[0].UserShippingRegistFlg ? "1" : "0" %>' RepeatLayout="Flow" CssClass="radioBtn" runat="server">
				<asp:ListItem Text="配送先情報を保存しない" Value="0"></asp:ListItem>
				<asp:ListItem Text="配送先情報を保存する" Value="1"></asp:ListItem>
				</asp:RadioButtonList>
				</p>
				<img src="../../Contents/ImagesPkg/common/btm_sub_boxA.gif" alt="bottom" width="298" height="4" />
			<!--subbox-->
			<dl id="dlUserShippingName" visible="false" runat="server">
					<dt><span>配送先を保存する場合は、以下をご入力ください。</span></dt>
					<dt>配送先名&nbsp;<span class="fred">※</span><span id="<%# "efo_sign_ship_addr_name" + Container.ItemIndex %>"/></dt>
					<dd class="last"><asp:TextBox ID="tbUserShippingName" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UserShippingName %>" MaxLength="30" CssClass="input_widthD input_border" runat="server"></asp:TextBox><br />
					<asp:CustomValidator
						ID="cvUserShippingName"
						runat="Server"
						ControlToValidate="tbUserShippingName"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" /></small>
					</dd>
			</dl>
			</dl>
		</div>
	</div><!--userList-->

	<span id="sInvoices" runat="server" visible="false">
		<div id="divUniformInvoiceType" runat="server">
			<h4>発票種類</h4>
			<div class="userList">
				<dl>
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
				</dl>
				<dl id="dlUniformInvoiceOption1_8" runat="server" visible="false">
					<br />
					<dd>統一編号</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="8"/>
						<asp:CustomValidator
							ID="cvUniformInvoiceOption1_8" runat="server"
							ControlToValidate="tbUniformInvoiceOption1_8"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							CssClass="error_inline" />
						<asp:Label ID="lbUniformInvoiceOption1_8" runat="server" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" Visible="false"></asp:Label>
					</dd>
					<br />
					<dd>会社名</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption2 %>" Width="220" runat="server" MaxLength="20"/>
						<asp:CustomValidator
							ID="cvUniformInvoiceOption2" runat="server"
							ControlToValidate="tbUniformInvoiceOption2"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							CssClass="error_inline" />
						<asp:Label ID="lbtbUniformInvoiceOption2" runat="server" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption2 %>" Visible="false"></asp:Label>
					</dd>
				</dl>

				<dl id="dlUniformInvoiceOption1_3" runat="server" visible="false">
					<br />
					<dd>寄付先コード</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption1_3" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="7" />
						<asp:CustomValidator
							ID="cvUniformInvoiceOption1_3" runat="server"
							ControlToValidate="tbUniformInvoiceOption1_3"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							CssClass="error_inline" />
						<asp:Label ID="lbUniformInvoiceOption1_3" Text="<%# ((CartObject)Container.DataItem).Shippings[0].UniformInvoiceOption1 %>" runat="server" Visible="false"></asp:Label>
					</dd>
				</dl>
				<dl id="dlUniformInvoiceTypeRegist" runat="server" visible="false">
					<dd>
						<asp:CheckBox ID="cbSaveToUserInvoice" Checked="<%# ((CartObject)Container.DataItem).Shippings[0].UserInvoiceRegistFlg %>" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
					</dd>
					<div id="dlUniformInvoiceTypeRegistInput" runat="server" visible="false">
						電子発票情報名 <span class="fred">※</span><br />
						<asp:TextBox ID="tbUniformInvoiceTypeName" Text="<%# ((CartObject)Container.DataItem).Shippings[0].InvoiceName %>" MaxLength="30" runat="server"></asp:TextBox>
						<asp:CustomValidator
							ID="cvUniformInvoiceTypeName" runat="server"
							ControlToValidate="tbUniformInvoiceTypeName"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							CssClass="error_inline" />
					</div>
				</dl>
			</div>
		</div>
		<div id="divInvoiceCarryType" runat="server">
			<h4>共通性載具</h4>
			<div class="userList">
				<dl>
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
					<br />
					<div id="divCarryTypeOption" runat ="server">
						<div id="divCarryTypeOption_8" runat="server" visible="false">
							<asp:TextBox ID="tbCarryTypeOption_8" Width="220" runat="server" Text="<%# ((CartObject)Container.DataItem).Shippings[0].CarryTypeOptionValue %>" placeholder="例:/AB201+9(限8個字)" MaxLength="8" />
							<asp:CustomValidator
								ID="cvCarryTypeOption_8"
								runat="server"
								ControlToValidate="tbCarryTypeOption_8"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								CssClass="error_inline" />
						</div>
						<div id="divCarryTypeOption_16" runat="server" visible="false">
							<asp:TextBox ID="tbCarryTypeOption_16" Width="220" Text="<%# ((CartObject)Container.DataItem).Shippings[0].CarryTypeOptionValue %>" runat="server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
							<asp:CustomValidator
								ID="cvCarryTypeOption_16"
								runat="server"
								ControlToValidate="tbCarryTypeOption_16"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								CssClass="error_inline" />
						</div>
					</div>
					<dl id="dlCarryTypeOptionRegist" runat="server" visible="false">
						<dd>
							<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
						</dd>
						<div id="divCarryTypeOptionName" runat="server" visible="false">
							電子発票情報名 <span class="fred">※</span><br />
							<asp:TextBox ID="tbCarryTypeOptionName" Text="<%# ((CartObject)Container.DataItem).Shippings[0].InvoiceName %>" runat="server" MaxLength="30"></asp:TextBox>
							<asp:CustomValidator
								ID="cvCarryTypeOptionName" runat="server"
								ControlToValidate="tbCarryTypeOptionName"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								CssClass="error_inline" />
						</div>
					</dl>
					<asp:Label runat="server" ID="lbCarryTypeOption" Visible="false"></asp:Label>
				</dl>
			</div>
		</div>
	</span>
		<h4 id="h4DeliveryOptions" visible="<%# CanInputShippingTo(Container.ItemIndex) %>" runat="server">配送指定</h4>
		<div id="dvStorePickup" Visible="<%# this.CanInputShippingStorePickup && (Container.ItemIndex == 0) %>" runat="server">
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
		<div id="dvShippingMethod" visible="<%# CanInputShippingTo(Container.ItemIndex) && IsShippingStorePickup(Container.ItemIndex) == false  %>" runat="server" class="userList">
			配送方法を選択して下さい。
			<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[Container.ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" runat="server"></asp:DropDownList>
		</div>
		<div id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(Container.ItemIndex) && CanDisplayDeliveryCompany(Container.ItemIndex)) %>" runat="server" class="userList">
			配送サービスを選択して下さい。
			<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(Container.ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" runat="server"/>
		</div>
		<div id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(Container.ItemIndex) %>" runat="server" class="userList" style='<%# HasFixedPurchase(Container) && (DisplayFixedPurchaseShipping(Container) == false) ? "padding-bottom: 0px" : "" %>'>
			配送希望日時を選択して下さい。
			<dl id="dlShipppingDateTime" runat="server">
				<dd></dd>
				<dt id="dtShippingDate" visible="<%# CanInputDateSet(Container.ItemIndex) %>" runat="server">配送希望日</dt>
				<dd id="ddShippingDate" visible="<%# CanInputDateSet(Container.ItemIndex) %>" runat="server">
					<asp:DropDownList id="ddlShippingDate" CssClass="input_border" runat="server" DataTextField="text" DataValueField="value"
						OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnCheckedChanged" AutoPostBack="true"></asp:DropDownList>
					<br />
					<asp:Label ID="lShippingDateErrorMessage" CssClass="fred" runat="server"></asp:Label>
				</dd>
				<div id="divShippingTime" runat="server">
				<dt id="dtShippingTime" visible="<%# CanInputTimeSet(Container.ItemIndex) %>" runat="server">配送希望時間帯</dt>
				<dd id="ddShippingTime" visible="<%# CanInputTimeSet(Container.ItemIndex) %>" runat="server" class="last">
					<asp:DropDownList id="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(Container.ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingTime(Container.ItemIndex) %>"></asp:DropDownList>
				</dd>
				</div>
			</dl>
		</div>

		<h4 style="margin-top:15px" visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server">定期購入 配送パターンの指定</h4>
		<%-- ▽デフォルトチェックの設定▽--%>
		<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
		<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 2, 3, 1, 4) %>
		<%-- △ - - - - - - - - - - - △--%>
		<div visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server" class="orderBox" style='<%# DisplayFixedPurchaseShipping(Container) ? "" : "margin-top: 0px;padding-top: 0px" %>'>
			<div class="userList list" style="padding-bottom: 0" visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server">「定期購入」はご希望の配送パターン・配送時間を指定して定期的に商品をお届けするサービスです。下記の配送パターンからお選び下さい。</div>

			<div id="<%# "efo_sign_fixed_purchase" + Container.ItemIndex %>" class="userList list" style="padding-top: 0" />
			<dl style="margin-top: 10px;" visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server">
				<dt id="Dt1" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
						Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 1) %>" 
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id="<%# "efo_sign_fixed_purchase_month" + Container.ItemIndex %>" /></dt>
				<dd id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">　
					<asp:DropDownList ID="ddlFixedPurchaseMonth"
						DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
						runat="server">
					</asp:DropDownList>
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
						DataSource='<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true, false, true) %>'
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
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
					ID="cvFixedPurchaseMonthlyDate"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseMonthlyDate"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					CssClass="error_inline"/>
				</small>
				<dt id="Dt2" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
						Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 2) %>" 
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id="<%# "efo_sign_fixed_purchase_week_and_day" + Container.ItemIndex %>" /></dt>
				<dd id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%#  GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">　
				<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
						DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server" />
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
						DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
						runat="server">
					</asp:DropDownList>
					<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
						DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
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
				<dt id="Dt3" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
						Text="配送日間隔指定" Checked="<%# (GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 3) && (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? (GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false).Length > 0) : (GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false).Length > 1))) %>" 
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
					<span id="<%# "efo_sign_fixed_purchase_interval_days" + Container.ItemIndex %>" />
				</dt>
				<dd id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">　
					<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
						DataSource='<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false) %>'
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
						runat="server">
					</asp:DropDownList>
					日ごとに届ける
				</dd>
				<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%# this.ShopShippingList[Container.ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
				<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%# this.ShopShippingList[Container.ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
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
				<dt id="Dt4" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
						Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 4) %>"
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id="<%# "efo_sign_fixed_purchase_week" + Container.ItemIndex %>" /></dt>
				<dd id="ddFixedPurchaseEveryNWeek" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">　
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
			<small><p class="attention" runat="server" visible="<%# GetAllFixedPurchaseKbnEnabled(Container.ItemIndex) == false %>">同時に定期購入できない商品が含まれております。</p></small>
			<small ID="sErrorMessage" class="fred" runat="server"></small>
			<br /><hr />
			<dl>
				<dt id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</dt>
				<dd visible="true" runat="server" style="padding-left: 20px;">
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
				<dd visible="true" runat="server" style="padding-left: 20px;">
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

		<asp:Repeater ID="rMemos" runat="server" DataSource="<%# ((CartObject)Container.DataItem).OrderMemos %>" Visible="<%# ((CartObject)Container.DataItem).OrderMemos.Count != 0 %>">
		<HeaderTemplate>
			<h4>注文メモ</h4>
			<div class="list">
		</HeaderTemplate>
		<ItemTemplate>
			<strong><%# WebSanitizer.HtmlEncode(Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME)) %></strong>
			<p><asp:TextBox ID="tbMemo"  runat="server" Text="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine"></asp:TextBox><br /></p><br />
			<small id="sErrorMessageMemo" runat="server" class="fred" ></small>
			<%-- IDに"OtherValidator"を含めることで案件毎に追加したtextareaなどでチェック可能 --%>
			<asp:CustomValidator ID="OtherValidator" runat="Server"
				ControlToValidate="tbMemo"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"/>
		</ItemTemplate>
		<FooterTemplate>
			</div>
		</FooterTemplate>
		</asp:Repeater>
		<asp:CheckBox ID="cbOnlyReflectMemoToFirstOrder"
			Checked="<%# ((CartObject)Container.DataItem).ReflectMemoToFixedPurchase %>"
			visible="<%# ((CartObject)Container.DataItem).OrderMemos.Count != 0 && ((CartObject)Container.DataItem).ReflectMemoToFixedPurchaseVisible %>"
			Text="2回目以降の注文メモにも追加する"
			CssClass="checkBox"
			runat="server" />
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
	</div><!--bottom-->
	</div><!--orderBox-->
</div><!--columnRight-->
<%} %>
<%-- ▲配送先情報▲ --%>

</ItemTemplate>
</asp:Repeater>

<br class="clr" />
</div><!--submain-->
</div><!--main-->

<%-- UpdatePanel外のイベントを実行したいためこのような呼び出し方となっている --%>
<div class="btmbtn below">
<ul>
	<li><a onclick="<%= this.BackOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.BackEvent) %>" class="btn btn-large btn-org-gry">前のページに戻る</a></li>
	<li><a onclick="<%= this.NextOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>" class="btn btn-large btn-success"><%: (this.IsNextConfirmPage) ? "ご注文内容確認へ" : "お支払方法入力へ" %></a></li>
</ul>
</div>

	<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && this.IsLoggedIn) { %>
	<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
	<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
	<% } %>
</ContentTemplate>
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

			// Check shipping zip code input on click
			clickSearchZipCodeInRepeater(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").UniqueID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sShippingZipError")).ClientID %>',
				"shipping");

			// Check shipping zip code input on text box change
			textboxChangeSearchZipCodeInRepeater(
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

			// Textbox change search shippping zip global
			textboxChangeSearchGlobalZip(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZipGlobal").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrShippingFromZipGlobal").UniqueID %>');
			<% } %>
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
	}

	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup(cartIndex) {
		selectedCartIndex = cartIndex;

		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var elements = document.getElementsByClassName(selectedCartIndex)[0];

		// For display
		elements.querySelector('[id$="ddCvsShopId"] > span').innerHTML = cvsspot;
		elements.querySelector('[id$="ddCvsShopName"] > span').innerHTML = name;
		elements.querySelector('[id$="ddCvsShopAddress"] > span').innerHTML = addr;
		elements.querySelector('[id$="ddCvsShopTel"] > span').innerHTML = tel;

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

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
</asp:Content>
