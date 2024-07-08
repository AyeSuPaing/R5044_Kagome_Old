<%--
=========================================================================================================
  Module      : 注文配送先配送先選択画面(OrderShippingSelectShipping.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="配送先選択ページ" Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderShippingSelectShipping.aspx.cs" Inherits="Form_Order_OrderShippingSelectShipping" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
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

<%-- ▽編集可能領域：コンテンツ▽ --%>
<p id="CartFlow"></p>
<h2 class="ttlA">
<span class="btn_nxt_item"><a href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>"><small>次へ進む</small></a></span>
<span class="btn_back btn_back_item"><a onclick="<%= this.BackOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.BackEvent) %>"><small>戻る</small></a></span>
<em><img src="../../Contents/ImagesPkg/order/ttl_user_esd02.gif" alt="配送先情報選択" width="127" height="18" /></em>
</h2>

<p>配送先情報を入力してください。
<%if (this.CartList.HasGift) { %>
ギフト注文の配送先を増やす場合は「新しい配送先を追加 」リンクをクリックしてください。<br />
<%} %>
<span class="fred">※</span>&nbsp;は必須入力です。
</p>
<br class="clr" />

<%-- 次へイベント用リンクボタン --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" runat="server"></asp:LinkButton>
<%-- 戻るイベント用リンクボタン --%>
<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server"></asp:LinkButton>

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<% this.CartItemIndexTmp = -1; %>

<asp:Repeater id="rCartList" Runat="server">
<ItemTemplate>
<%-- ▼配送先情報▼ --%>
<div class="orderBoxLarge">
<h3>
	<div class="cartNo">カート番号<%# Container.ItemIndex + 1 %><%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）"))%></div>
	<div class="cartLink"><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>">カートへ戻る</a></div>
</h3>
	
<%-- ▽未分類商品一覧▽ ※商品一覧はポストバックで更新されるためDataSourceは裏で指定 --%>
<asp:Repeater id="rCartProducts" Runat="server">
<HeaderTemplate>
<h4>商品 </h4>
<div class="userProduct">
</HeaderTemplate>
<ItemTemplate>
<div class="<%# (((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1) ? "last" : "" %>">
<dl>
<dt><w2c:ProductImage ID="ProductImage1" ProductMaster="<%# ((CartShipping.ProductCount)Container.DataItem).Product %>" ImageSize="S" runat="server" /></dt>
<dd style="float:left">
<strong><%# WebSanitizer.HtmlEncode(((CartShipping.ProductCount)Container.DataItem).Product.ProductJointName) %>
</strong>

<p id="P1" visible='<%# ((CartShipping.ProductCount)Container.DataItem).Product.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
	<b>
	<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartShipping.ProductCount)Container.DataItem).Product.ProductOptionSettingList %>' runat="server">
	<ItemTemplate>
	<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
	<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
	</ItemTemplate>
	</asp:Repeater>
	</b>
</p>

</dd>
</dl>
<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
</div>
</ItemTemplate>
<FooterTemplate>
</div><!--userProduct-->
</FooterTemplate>
</asp:Repeater>
<%-- △未分類商品一覧△ --%>

<asp:HiddenField ID="hfCartIndex" Value="<%# Container.ItemIndex %>" runat="server" />

<%
	this.CartItemIndexTmp++;
	this.CartShippingItemIndexTmp = -1;
%>

<%-- ▽配送先ループ▽※編集中のダミーShippingをバインドすることがあるためFindCartは利用できない --%>
<asp:Repeater id="rCartShippings" runat="server">
<ItemTemplate>
<%
	this.CartShippingItemIndexTmp++;
%>

<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
<h4 visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server">
<div class="cartNo">配送先<%# Container.ItemIndex + 1 %>
&nbsp;<asp:LinkButton ID="lbDeleteShipping" class="btnDelete" Text="削除" Visible="<%# this.CartList.Items[((RepeaterItem)Container.Parent.Parent).ItemIndex].IsGift && (Container.ItemIndex > 0) %>" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbDeleteShipping_Click" runat="server"></asp:LinkButton>&nbsp;
<div class="cartLink">&nbsp;
</div>
</h4>

<%-- ▽送り主▽ --%>
<div id="divShippingInputForm" class="userListFloat orderBoxLarge list" visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server">
<h5>送り主</h5>

<asp:CheckBox id="cbSameSenderAsShipping1" Checked='<%# ((CartShipping)Container.DataItem).IsSameSenderAsShipping1 %>' Visible='<%# Container.ItemIndex != 0 %>' OnCheckedChanged='cbSameSenderAsShipping1_OnCheckedChanged' Text="配送先1と同じ送り主を指定する" AutoPostBack="true" runat="server" />
<div id="hcShippingSender" visible='<%# ((CartShipping)Container.DataItem).IsSameSenderAsShipping1 == false %>' runat="server">

<asp:RadioButtonList ID="rblSenderSelector" SelectedValue="<%# ((CartShipping)Container.DataItem).SenderAddrKbn %>" Visible="<%# ((CartShipping)Container.DataItem).CartObject.IsGift %>" OnSelectedIndexChanged="rblSenderSelector_OnSelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal" CssClass="radioBtn" runat="server">
<asp:ListItem Text="注文者を送り主とする" Value="Owner"></asp:ListItem>
<asp:ListItem Text="新規入力する" Value="New"></asp:ListItem>
</asp:RadioButtonList>

<%-- ▽送り主：表示▽ --%>
<div id="divSenderDisp" visible="<%# GetSendFromOwner(((CartShipping)Container.DataItem)) %>" runat="server">
<%
	var ownerAddrCountryIsoCode = this.CartList.Owner.AddrCountryIsoCode;
	var isOwnerAddrCountryJp = IsCountryJp(ownerAddrCountryIsoCode);
	var isOwnerAddrCountryUs = IsCountryUs(ownerAddrCountryIsoCode);
	var isOwnerAddrZipNecessary = IsAddrZipcodeNecessary(ownerAddrCountryIsoCode);
%>
<dl>
<%-- 氏名 --%>
<dt><%: ReplaceTag("@@User.name.name@@", ownerAddrCountryIsoCode) %></dt>
<dd>
<%: this.CartList.Owner.Name1 %> <%: this.CartList.Owner.Name2 %>&nbsp;様（
<% if (this.CartList.Owner.IsAddrJp) { %>
<%: this.CartList.Owner.NameKana1 %> <%: this.CartList.Owner.NameKana2 %>&nbsp;さま）<br />
<% } %>
</dd>
<dt>
	<%: ReplaceTag("@@User.addr.name@@") %>
</dt>
<dd>
<% if (this.CartList.Owner.IsAddrJp) { %><%: this.CartList.Owner.Zip %><br /><% } %>
<%: this.CartList.Owner.Addr1　%> <%: this.CartList.Owner.Addr2 %><br />
<%: this.CartList.Owner.Addr3　%> <%: this.CartList.Owner.Addr4 %><br />
<%: this.CartList.Owner.Addr5　%> <% if (this.CartList.Owner.IsAddrJp == false) { %><%: this.CartList.Owner.Zip %><% } %>
<%: this.CartList.Owner.AddrCountryName %>
</dd>
<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
<%-- 企業名・部署名 --%>
<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
	<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
<dd>
<%: this.CartList.Owner.CompanyName %>&nbsp<%: this.CartList.Owner.CompanyPostName %>
</dd>
<%} %>
<%-- 電話番号 --%>
<dt><%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %></dt>
<dd>
<%: this.CartList.Owner.Tel1 %>
</dd>
</dl>
</div>
<%-- △送り主：表示△ --%>

<%-- ▽送り主：入力フォーム▽ --%>
<div id="divSenderInputFormInner" visible="<%# GetSendFromOwner((CartShipping)Container.DataItem) == false %>" runat="server">
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
姓&nbsp;&nbsp;<asp:TextBox ID="tbSenderName1" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_NAME1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
名&nbsp;&nbsp;<asp:TextBox ID="tbSenderName2" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_NAME2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></asp:TextBox><br />
<small>
<asp:CustomValidator
	ID="cvSenderName1"
	runat="Server"
	ControlToValidate="tbSenderName1"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" />
<asp:CustomValidator
	ID="cvSenderName2"
	runat="Server"
	ControlToValidate="tbSenderName2"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
<%-- 送り主：氏名（かな） --%>
<% if (isSenderAddrCountryJp) { %>
<dt>
	<%: ReplaceTag("@@User.name_kana.name@@", senderAddrCountryIsoCode)%>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
姓&nbsp;&nbsp;<asp:TextBox ID="tbSenderNameKana1"  Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
名&nbsp;&nbsp;<asp:TextBox ID="tbSenderNameKana2"  Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></asp:TextBox><br />
<small>
<asp:CustomValidator
	ID="cvSenderNameKana1"
	runat="Server"
	ControlToValidate="tbSenderNameKana1"
	ClientValidationFunction="ClientValidate"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ValidationGroup="OrderShipping"
	CssClass="error_inline" />
<asp:CustomValidator
	ID="cvenderNameKana2"
	runat="Server"
	ControlToValidate="tbSenderNameKana2"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
<% } %>
<%-- 送り主：国 --%>
<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
<dt>
	<%: ReplaceTag("@@User.country.name@@", senderAddrCountryIsoCode) %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
	<asp:DropDownList runat="server" ID="ddlSenderCountry" DataSource="<%# this.UserCountryDisplayList %>" AutoPostBack="true" OnSelectedIndexChanged="ddlSenderCountry_SelectedIndexChanged" DataTextField="Text" DataValueField="Value"
		SelectedValue="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE) %>"></asp:DropDownList>
	<asp:CustomValidator
		ID="cvSenderCountry"
		runat="Server"
		ControlToValidate="ddlSenderCountry"
		ValidationGroup="OrderShipping"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		ClientValidationFunction="ClientValidate"
		EnableClientScript="false"
		CssClass="error_inline" />
</dd>
<% } %>
<%-- 送り主：郵便番号 --%>
<% if (isSenderAddrCountryJp) { %>
<dt>
	<%: ReplaceTag("@@User.zip.name@@", senderAddrCountryIsoCode) %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
<span class="left">
<asp:TextBox ID="tbSenderZip" Text="<%#: GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_ZIP) %>" OnTextChanged="lbSearchSenderAddr_Click" CssClass="input_widthC input_border" MaxLength="8" runat="server" />
</span>
&nbsp;&nbsp;<span class="btn_add_sea right"><asp:LinkButton ID="lbSearchSenderAddr" runat="server" OnClick="lbSearchSenderAddr_Click" OnClientClick="return false;"><small>住所検索</small></asp:LinkButton></span><br class="clr" />
<%--検索結果レイヤー--%>
<uc:Layer ID="ucLayerForSender" runat="server" />
<small>
<asp:CustomValidator
	ID="cvSenderZip1"
	runat="Server"
	ControlToValidate="tbSenderZip"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline cvSenderZipShortInput" /></small>
<small id="sSenderZipError" runat="server" class="fred sSenderZipError"></small>
</dd>
<%-- 送り主：都道府県 --%>
<dt>
	<%: ReplaceTag("@@User.addr1.name@@", senderAddrCountryIsoCode) %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd><asp:DropDownList ID="ddlSenderAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1) %>" runat="server"></asp:DropDownList>
<small>
<asp:CustomValidator
	ID="cvSenderAddr1"
	runat="Server"
	ControlToValidate="ddlSenderAddr1"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
<% } %>
<%-- 送り主：市区町村 --%>
<dt>
	<%: ReplaceTag("@@User.addr2.name@@", senderAddrCountryIsoCode) %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
<% if (isSenderAddrCountryTw) { %>
	<asp:DropDownList runat="server" ID="ddlSenderAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged"></asp:DropDownList>
<% } else { %>
	<asp:TextBox ID="tbSenderAddr2" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></asp:TextBox><br />
	<small>
	<asp:CustomValidator
		ID="cvSenderAddr2"
		runat="Server"
		ControlToValidate="tbSenderAddr2"
		ValidationGroup="OrderShipping"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		ClientValidationFunction="ClientValidate"
		CssClass="error_inline" /></small>
<% } %>
</dd>
<%-- 送り主：番地 --%>
<dt>
	<%:ReplaceTag("@@User.addr3.name@@", senderAddrCountryIsoCode) %>
	<% if (IsAddress3Necessary(senderAddrCountryIsoCode)){ %>&nbsp;<span class="fred">※</span><% } %>
</dt>
<dd>
<% if (isSenderAddrCountryTw) { %>
	<asp:DropDownList runat="server" ID="ddlSenderAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
<% } else { %>
	<asp:TextBox ID="tbSenderAddr3" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></asp:TextBox><br />
	<small>
	<asp:CustomValidator
		ID="cvSenderAddr3"
		runat="Server"
		ControlToValidate="tbSenderAddr3"
		ValidationGroup="OrderShipping"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		ClientValidationFunction="ClientValidate"
		CssClass="error_inline" /></small>
<% } %>
</dd>
<%-- 送り主：ビル・マンション名 --%>
<dt>
	<%: ReplaceTag("@@User.addr4.name@@", senderAddrCountryIsoCode) %>
	<% if (isSenderAddrCountryJp == false) { %>&nbsp;<span class="fred">※</span><% } %>
</dt>
<dd><asp:TextBox ID="tbSenderAddr4" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox><br />
<small>
<asp:CustomValidator
	ID="cvSenderAddr4"
	runat="Server"
	ControlToValidate="tbSenderAddr4"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
<%-- 送り主：州 --%>
<% if (isSenderAddrCountryJp == false) { %>
<dt>
	<%: ReplaceTag("@@User.addr5.name@@", senderAddrCountryIsoCode) %>
	<% if (isSenderAddrCountryUs) { %>&nbsp;<span class="fred">※</span><% } %>
</dt>
<dd>
	<% if (isSenderAddrCountryUs) { %>
	<asp:DropDownList runat="server" ID="ddlSenderAddr5" DataSource="<%# this.UserStateList %>" Text="<%# GetSenderValue((CartShipping)Container.DataItem, CartShipping.FIELD_ORDERSHIPPING_SENDER_ADDR5_US) %>"></asp:DropDownList>
		<asp:CustomValidator
			ID="cvSenderAddr5Ddl"
			runat="Server"
			ControlToValidate="ddlSenderAddr5"
			ValidationGroup="OrderShippingGlobal"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
	<% } else { %>
	<asp:TextBox runat="server" ID="tbSenderAddr5" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5) %>"></asp:TextBox>
	<small>
	<asp:CustomValidator
		ID="cvSenderAddr5"
		runat="Server"
		ControlToValidate="tbSenderAddr5"
		ValidationGroup="OrderShippingGlobal"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		ClientValidationFunction="ClientValidate"
		CssClass="error_inline" /></small>
	<% } %>
</dd>
<%-- 送り主：郵便番号（海外向け） --%>
<dt>
	<%: ReplaceTag("@@User.zip.name@@", senderAddrCountryIsoCode) %>
	<% if (isSenderAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
</dt>
<dd>
	<asp:TextBox runat="server" ID="tbSenderZipGlobal" MaxLength="20" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_ZIP) %>"></asp:TextBox>
	<small>
	<asp:CustomValidator
		ID="cvSenderZipGlobal"
		runat="Server"
		ControlToValidate="tbSenderZipGlobal"
		ValidationGroup="OrderShippingGlobal"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		ClientValidationFunction="ClientValidate"
		CssClass="error_inline" /></small>
	<asp:LinkButton
		ID="lbSearchAddrSenderFromZipGlobal"
		OnClick="lbSearchAddrSenderFromZipGlobal_Click"
		Style="display:none;"
		runat="server" />
</dd>
<% } %>
<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
<%-- 送り主：企業名 --%>
<dt>
	<%: ReplaceTag("@@User.company_name.name@@") %>
	&nbsp;<span class="fred"></span>
</dt>
<dd><asp:TextBox ID="tbSenderCompanyName" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></asp:TextBox><br />
<small>
<asp:CustomValidator
	ID="cvSenderCompanyName"
	runat="Server"
	ControlToValidate="tbSenderCompanyName"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
<%-- 送り主：部署名 --%>
<dt>
	<%: ReplaceTag("@@User.company_post_name.name@@")%>
	&nbsp;<span class="fred"></span>
</dt>
<dd><asp:TextBox ID="tbSenderCompanyPostName" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></asp:TextBox><br />
<small>
<asp:CustomValidator
	ID="cvSenderCompanyPostName"
	runat="Server"
	ControlToValidate="tbSenderCompanyPostName"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
<%}%>
<%-- 送り主：電話番号 --%>
<% if (isSenderAddrCountryJp) { %>
<dt>
	<%: ReplaceTag("@@User.tel1.name@@") %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
	<asp:TextBox ID="tbSenderTel1" Text="<%#: GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_TEL1) %>" CssClass="input_widthE input_border shortTel" MaxLength="13" runat="server" /><br />
<small>
<asp:CustomValidator
	ID="cvSenderTel1_1"
	runat="Server"
	ControlToValidate="tbSenderTel1"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
<% } else { %>
<%-- 送り主：電話番号1（海外向け） --%>
<dt>
	<%: ReplaceTag("@@User.tel1.name@@", senderAddrCountryIsoCode) %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
	<asp:TextBox ID="tbSenderTel1Global" runat="server" MaxLength="30" Text="<%# GetSenderValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SENDER_TEL1) %>"></asp:TextBox>
	<small>
	<asp:CustomValidator
		ID="cvSenderTel1Global"
		runat="Server"
		ControlToValidate="tbSenderTel1Global"
		ValidationGroup="OrderShippingGlobal"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		ClientValidationFunction="ClientValidate"
		CssClass="error_inline" /></small>
</dd>
<% } %>
</dl>
</div>
<%-- △送り主：入力フォーム△ --%>
</div>

</div><!--userList-->
<%-- △送り主△ --%>

<%-- ▽配送先▽ --%>
<div id="divShippingInputForm2" class="userListFloat orderBoxLarge list" visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server">
<h5>配送先</h5>

配送先を選択して下さい。<br />
<asp:DropDownList ID="ddlShippingKbnList" DataSource="<%# this.UserShippingList %>" DataTextField="text" DataValueField="value" SelectedValue='<%# ((CartShipping)Container.DataItem).ShippingAddrKbn %>' OnSelectedIndexChanged="ddlShippingKbnList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
<br /><br />
<span style="color:red;display:block;"><asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal></span>
<%-- ▽配送先表示▽ --%>
<div id="divShippingDisp" visible="<%# (GetShipToNew(((CartShipping)Container.DataItem)) == false) %>" runat="server">
<% var isShippingAddrCountryJp = IsCountryJp(this.CountryIsoCode); %>
<dl>
<dt>
	<%#: ReplaceTag("@@User.name.name@@") %>
</dt>
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
	<asp:Literal ID="lShippingCountryName" runat="server"></asp:Literal><br />
	<small id="sOwnerZipError" runat="server" class="fred shortZipInputErrorMessage"></small>
</dd>
<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
<dt>
	<%: ReplaceTag("@@User.company_name.name@@")%>・
	<%: ReplaceTag("@@User.company_post_name.name@@")%>
</dt>
<dd>
<asp:Literal ID="lShippingCompanyName" runat="server"></asp:Literal>&nbsp<asp:Literal ID="lShippingCompanyPostName" runat="server"></asp:Literal>
</dd>
<%} %>
<%-- 電話番号 --%>
<dt>
	<%#: ReplaceTag("@@User.tel1.name@@") %>
</dt>
<dd>
<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
</dd>

</dl>
</div>
<%-- △配送先表示△ --%>

<%-- ▽配送先入力フォーム▽ --%>
<div id="divShippingInputFormInner" visible="<%# GetShipToNew((CartShipping)Container.DataItem) %>" runat="server">
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
姓&nbsp;&nbsp;<asp:TextBox ID="tbShippingName1" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
名&nbsp;&nbsp;<asp:TextBox ID="tbShippingName2" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></asp:TextBox><br />
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
	<%: ReplaceTag("@@User.name_kana.name@@")%>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
姓&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana1"  Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
名&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana2"  Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></asp:TextBox><br />
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
<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
<%-- 配送先：国 --%>
<dt>
	<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
	<asp:DropDownList ID="ddlShippingCountry" runat="server" DataSource="<%# this.ShippingAvailableCountryDisplayList %>" OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged" AutoPostBack="true" DataTextField="Text" DataValueField="Value"
		SelectedValue="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE) %>"></asp:DropDownList>
	<asp:CustomValidator
		ID="cvShippingCountry"
		runat="Server"
		ControlToValidate="ddlShippingCountry"
		ValidationGroup="OrderShipping"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		EnableClientScript="false"
		ClientValidationFunction="ClientValidate"
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
<span class="left">
<asp:TextBox ID="tbShippingZip" Text="<%#: GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" CssClass="input_widthC input_border" OnTextChanged="lbSearchShippingAddr_Click" MaxLength="8" runat="server" />
</span>
&nbsp;&nbsp;<span class="btn_add_sea right"><asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" OnClientClick="return false;"><small>住所検索</small></asp:LinkButton></span><br class="clr" />
<%--検索結果レイヤー--%>
<uc:Layer ID="ucLayerForShipping" runat="server" />
<small>
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
</dd>
<%-- 配送先：都道府県 --%>
<dt>
	<%: ReplaceTag("@@User.addr1.name@@") %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd><asp:DropDownList ID="ddlShippingAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1) %>" runat="server"></asp:DropDownList>
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
	&nbsp;<span class="fred">※</span>
</dt>
<dd>
<% if (isShippingAddrCountryTw) { %>
	<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
<% } else { %>
	<asp:TextBox ID="tbShippingAddr2" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></asp:TextBox><br />
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
	<% if (IsAddress3Necessary(shippingAddrCountryIsoCode)){ %>&nbsp;<span class="fred">※</span><% } %>
</dt>
<dd>
<% if (isShippingAddrCountryTw) { %>
	<asp:DropDownList runat="server" ID="ddlShippingAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
<% } else { %>
	<asp:TextBox ID="tbShippingAddr3" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></asp:TextBox><br />
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
<dd><asp:TextBox ID="tbShippingAddr4" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox><br />
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
<% if (isShippingAddrCountryJp == false) { %>
<%-- 配送先：州 --%>
<dt>
	<%: ReplaceTag("@@User.addr5.name@@", shippingAddrCountryIsoCode) %>
	<% if (isShippingAddrCountryUs) { %>&nbsp;<span class="fred">※</span><% } %>
</dt>
<dd>
	<% if (isShippingAddrCountryUs) { %>
	<asp:DropDownList runat="server" ID="ddlShippingAddr5" DataSource="<%# this.UserStateList %>"></asp:DropDownList>
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
	<asp:TextBox runat="server" ID="tbShippingAddr5" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5) %>"></asp:TextBox>
	<small>
	<asp:CustomValidator
		ID="cvShippingAddr5"
		runat="Server"
		ControlToValidate="tbShippingAddr5"
		ValidationGroup="OrderShippingGlobal"
		ValidateEmptyText="true"
		SetFocusOnError="true"
		ClientValidationFunction="ClientValidate"
		CssClass="error_inline" /></small>
	<% } %>
</dd>
<%-- 配送先：郵便番号（海外向け） --%>
<dt>
	<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
	<% if (isShippingAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
</dt>
<dd>
	<asp:TextBox runat="server" ID="tbShippingZipGlobal" MaxLength="20" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ZIP) %>"></asp:TextBox>
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
<dd><asp:TextBox ID="tbShippingCompanyName" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></asp:TextBox><br />
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
<dd><asp:TextBox ID="tbShippingCompanyPostName" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>" CssClass="input_widthE input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></asp:TextBox><br />
<small>
<asp:CustomValidator
	ID="cvShippingCompanyPostName"
	runat="Server"
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
	<%: ReplaceTag("@@User.tel1.name@@") %>
	&nbsp;<span class="fred">※</span>
</dt>
<dd><asp:TextBox ID="tbShippingTel1" Text="<%# ((CartShipping)Container.DataItem).Tel1 %>" CssClass="input_widthE input_border shortTel" MaxLength="13" runat="server" />
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
	<asp:TextBox runat="server" ID="tbShippingTel1Global" MaxLength="30" Text="<%# GetShippingValue((CartShipping)Container.DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_TEL1) %>"></asp:TextBox>
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
<br class="clr" />

<div id="Div7" class="subbox" visible="<%# this.IsLoggedIn %>" runat="server">
<p>

<%-- ポストバックCustomValidatorの状態がクリアされてしまうため、JaavScirptで表示非表示を制御する --%>
<asp:RadioButtonList ID="rblSaveToUserShipping" OnSelectedIndexChanged="rblSaveToUserShipping_OnSelectedIndexChanged" AutoPostBack="true" SelectedValue='<%# ((CartShipping)Container.DataItem).UserShippingRegistFlg ? "1" : "0" %>' RepeatLayout="Flow" CssClass="radioBtn" runat="server">
<asp:ListItem Text="配送先情報を保存しない" Value="0"></asp:ListItem>
<asp:ListItem Text="配送先情報を保存する" Value="1"></asp:ListItem>
</asp:RadioButtonList>
</p>
</div>
<!--subbox-->

<dl id="dlUserShippingName" visible="<%# ((CartShipping)Container.DataItem).UserShippingRegistFlg %>" runat="server">
<dd>配送先を保存する場合は、以下をご入力ください。</dd>
<dt>配送先名&nbsp;<span class="fred">※</span></dt>
<dd class="last"><asp:TextBox ID="tbUserShippingName" Text="<%# ((CartShipping)Container.DataItem).UserShippingName %>" MaxLength="30" CssClass="input_widthE input_border" runat="server"></asp:TextBox><br />
<asp:CustomValidator ID="CustomValidator32" runat="Server"
	ControlToValidate="tbUserShippingName"
	ValidationGroup="OrderShipping"
	ValidateEmptyText="true"
	SetFocusOnError="true"
	ClientValidationFunction="ClientValidate"
	CssClass="error_inline" /></small>
</dd>
</dl>
</div>
</div><!--userList-->
<%-- △配送先△ --%>

</ItemTemplate>
</asp:Repeater>
<%-- △配送先ループ△ --%>
<div class="clr" runat="server">
<div visible='<%# ((CartObject)Container.DataItem).IsGift %>' class="addShipping" runat="server">
&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lbAddShipping" OnClick="lbAddShipping_Click" runat="server">新しい配送先を追加</asp:LinkButton>
</div>
<div class="fred userProduct" id="hcErrorMessages" enableviewstate="false" runat="server"></div>
</div>
</div><!--orderBoxLarge-->
<%-- ▲配送先情報▲ --%>

</ItemTemplate>
</asp:Repeater>

<%-- UpdatePanel外のイベントを実行したいためこのような呼び出し方となっている --%>
<p class="btmbtn btn_nxt_item right"><a href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>"><small>次へ進む</small></a></p>
<p class="btmbtn btn_back_item right"><a onclick="<%= this.BackOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.BackEvent) %>"><small>戻る</small></a></p>

<br class="clr" />


</ContentTemplate>
<Triggers>
<asp:PostBackTrigger ControlID="rCartList"/>
</Triggers>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

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
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする--%>
	function bindExecAutoKana() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
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
			<%} %>
		<%} %>
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			<% foreach (RepeaterItem riShipping in ((Repeater)ri.FindControl("rCartShippings")).Items) { %>
				execAutoChangeKanaWithKanaType(
					$('#<%= ((TextBox)riShipping.FindControl("tbSenderNameKana1")).ClientID %>'),
					$('#<%= ((TextBox)riShipping.FindControl("tbSenderNameKana2")).ClientID %>'));
				execAutoChangeKanaWithKanaType(
					$('#<%= ((TextBox)riShipping.FindControl("tbShippingNameKana1")).ClientID %>'),
					$('#<%= ((TextBox)riShipping.FindControl("tbShippingNameKana2")).ClientID %>'));
			<%} %>
		<%} %>
	}

	var bindTargetForAddr1 = "";
	var bindTargetForAddr2 = "";
	var bindTargetForAddr3 = "";
	var multiAddrsearchTriggerType = "";
	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
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
		<%} %>
	}

	$(document).on('click', '.search-result-layer-close, #layer', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function () {
		bindSelectedAddr($('li.search-result-layer-addr').index(this), multiAddrsearchTriggerType);
	});

	<%-- 複数住所検索結果からの選択値を入力フォームにバインドする --%>
	function bindSelectedAddr(selectedIndex, multiAddrsearchTriggerType) {
		var selectedAddr = $('.search-result-layer-addrs li').eq(selectedIndex);
		if (multiAddrsearchTriggerType == "sender") {
			<% foreach (RepeaterItem ri in rCartList.Items) { %>
			<% foreach (RepeaterItem riShipping in ((Repeater)ri.FindControl("rCartShippings")).Items) { %>
				$('#' + bindTargetForAddr1).val(selectedAddr.find('.addr').text());
				$('#' + bindTargetForAddr2).val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
				$('#' + bindTargetForAddr3).focus();
			<%} %>
			<%} %>
		} else if (multiAddrsearchTriggerType == "shipping") {
			<% foreach (RepeaterItem ri in rCartList.Items) { %>
			<% foreach (RepeaterItem riShipping in ((Repeater)ri.FindControl("rCartShippings")).Items) { %>
				$('#' + bindTargetForAddr1).val(selectedAddr.find('.addr').text());
				$('#' + bindTargetForAddr2).val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
				$('#' + bindTargetForAddr3).focus();
			<%} %>
			<%} %>
		}

		closePopupAndLayer();
	}

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		<% foreach (RepeaterItem item in rCartList.Items) { %>
		<% foreach (RepeaterItem itemShipping in ((Repeater)item.FindControl("rCartShippings")).Items) { %>
			<% if (((DropDownList)itemShipping.FindControl("ddlShippingAddr3") != null) && ((TextBox)itemShipping.FindControl("tbShippingZipGlobal") != null)) { %>
			$('#<%= ((DropDownList)itemShipping.FindControl("ddlShippingAddr3")).ClientID %>').change(function (e) {
				$('#<%= ((TextBox)itemShipping.FindControl("tbShippingZipGlobal")).ClientID %>').val(
					$('#<%= ((DropDownList)itemShipping.FindControl("ddlShippingAddr3")).ClientID %>').val().split('|')[0]);
			});
			<% } %>
			<% if (((DropDownList)itemShipping.FindControl("ddlOwnerAddr3") != null) && ((TextBox)itemShipping.FindControl("tbSenderZipGlobal") != null)) { %>
			$('#<%= ((DropDownList)itemShipping.FindControl("ddlOwnerAddr3")).ClientID %>').change(function (e) {
				$('#<%= ((TextBox)itemShipping.FindControl("tbSenderZipGlobal")).ClientID %>').val(
					$('#<%= ((DropDownList)itemShipping.FindControl("ddlOwnerAddr3")).ClientID %>').val().split('|')[0]);
			});
			<% } %>
		<% } %>
		<% } %>
	}
	<% } %>
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