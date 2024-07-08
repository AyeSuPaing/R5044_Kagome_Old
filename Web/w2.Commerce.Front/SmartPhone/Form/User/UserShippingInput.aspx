<%--
=========================================================================================================
  Module      : スマートフォン用アドレス帳入力画面(UserShippingInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserShippingInput.aspx.cs" Inherits="Form_User_UserShippingInput" Title="アドレス帳入力ページ" MaintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-shipping-list">
<div class="user-unit">
	<h2>アドレス帳の入力</h2>
	<div class="msg">
		<p id="pRegistInfo" runat="server" visible="false">新しく追加されるお届け先を入力してください。</p>
		<p id="pModifyInfo" runat="server" visible="false">アドレス帳に登録されているお届け先を編集します。</p>
		<p class="attention">※は必須入力となります。</p>
	</div>

	<dl class="user-form">
		<dt>配送先名<span class="require">※</span></dt>
		<dd class="name">
			<p class="attention">
				<asp:CustomValidator
					ID="cvName"
					runat="Server"
					ControlToValidate="tbName"
					ValidationGroup="UserShippingRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbName" Runat="server" maxlength="30"></w2c:ExtendedTextBox>
		</dd>
		<dt>
			<%-- 氏名 --%>
			<%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span>
		</dt>
		<dd class="name">
			<p class="attention">
				<asp:CustomValidator
					ID="cvShippingName1"
					runat="Server"
					ControlToValidate="tbShippingName1"
					ValidationGroup="UserShippingRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvShippingName2"
					runat="Server"
					ControlToValidate="tbShippingName2"
					ValidationGroup="UserShippingRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbShippingName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
			<% tbShippingName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
			<w2c:ExtendedTextBox id="tbShippingName1" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox id="tbShippingName2" Runat="server" maxlength="10"></w2c:ExtendedTextBox>
		</dd>
		<% if (this.IsShippingAddrJp) { %>
		<dt>
			<%-- 氏名（かな） --%>
			<%: ReplaceTag("@@User.name_kana.name@@") %><span class="require">※</span>
		</dt>
		<dd class="name-kana">
			<p class="attention">
				<asp:CustomValidator
					ID="cvShippingNameKana1"
					runat="Server"
					ControlToValidate="tbShippingNameKana1"
					ValidationGroup="UserShippingRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvShippingNameKana2"
					runat="Server"
					ControlToValidate="tbShippingNameKana2"
					ValidationGroup="UserShippingRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbShippingNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name_kana1.name@@"); %>
			<% tbShippingNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name_kana2.name@@"); %>
			<w2c:ExtendedTextBox id="tbShippingNameKana1" Runat="server" maxlength="20"></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox id="tbShippingNameKana2" Runat="server"  maxlength="20"></w2c:ExtendedTextBox>
		</dd>
		<% } %>
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<dt>
			<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
			<span class="require">※</span>
		</dt>
		<dd>
			<asp:DropDownList runat="server" ID="ddlShippingCountry" AutoPostBack="true" OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"></asp:DropDownList>
			<asp:CustomValidator
				ID="cvShippingCountry"
				runat="Server"
				ControlToValidate="ddlShippingCountry"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false"
				CssClass="error_inline" />
		</dd>
		<% } %>
		<% if (this.IsShippingAddrJp) { %>
		<dt>
			<%-- 郵便番号 --%>
			<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
			<span class="require">※</span>
		</dt>
		<dd class="zip">
			<p class="attention" id="addrSearchErrorMessage">
				<asp:CustomValidator
					ID="cvShippingZip1"
					runat="Server"
					ControlToValidate="tbShippingZip"
					ValidationGroup="UserShippingRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<%-- エラーメッセージ --%>
				<% if(StringUtility.ToEmpty(this.ZipInputErrorMessage) != "") {%><%: this.ZipInputErrorMessage %><% } %>
			</p>
			<w2c:ExtendedTextBox ID="tbShippingZip" Type="tel" MaxLength="8" OnTextChanged="lbSearchShippingAddr_Click" runat="server" />
			<asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">郵便番号から住所を入力</asp:LinkButton>
			<%--検索結果レイヤー--%>
			<uc:Layer ID="ucLayer" runat="server" />
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@", this.ShippingAddrCountryIsoCode) %>
			<span class="require">※</span>
		</dt>
		<dd class="address">
			<p class="attention">
			<% if (this.IsShippingAddrJp) { %>
			<asp:CustomValidator
				ID="cvShippingAddr1"
				runat="Server"
				ControlToValidate="ddlShippingAddr1"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			<% } %>
			<asp:CustomValidator
				ID="cvShippingAddr2"
				runat="Server"
				ControlToValidate="tbShippingAddr2"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			<asp:CustomValidator
				ID="cvShippingAddr3"
				runat="Server"
				ControlToValidate="tbShippingAddr3"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			<asp:CustomValidator
				ID="cvShippingAddr4"
				runat="Server"
				ControlToValidate="tbShippingAddr4"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<% if (this.IsShippingAddrJp) { %>
			<%-- 都道府県 --%>
			<asp:DropDownList id="ddlShippingAddr1" runat="server" DataTextField="Text" DataValueField="Value"></asp:DropDownList><br />
			<% } %>
			<% if(this.IsShippingAddrTw) { %>
			<asp:DropDownList runat="server" ID="ddlShippingAddr2" AutoPostBack="true" DataTextField="Text" DataValueField="Value" Width="95" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
			<br />
			<asp:DropDownList runat="server" ID="ddlShippingAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
			<% } else { %>
			<%-- 市区町村 --%>
			<% tbShippingAddr2.Attributes["placeholder"] = ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode); %>
			<w2c:ExtendedTextBox id="tbShippingAddr2" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
			<%-- 番地 --%>
			<% tbShippingAddr3.Attributes["placeholder"] = ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode); %>
			<w2c:ExtendedTextBox id="tbShippingAddr3" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
			<% } %>
			<%-- ビル・マンション名 --%>
			<% tbShippingAddr4.Attributes["placeholder"] = ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode); %>
			<w2c:ExtendedTextBox id="tbShippingAddr4" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>

			<% if (this.IsShippingAddrJp == false) { %>
				<%-- 州 --%>
				<% if (this.IsShippingAddrUs) { %>
				<asp:DropDownList ID="ddlShippingAddr5" runat="server"></asp:DropDownList>
					<asp:CustomValidator
						ID="cvShippingAddr5"
						runat="Server"
						ControlToValidate="ddlShippingAddr5"
						ValidationGroup="UserShippingRegistGlobal"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false"
						CssClass="error_inline" />
				<% } else { %>
				<% tbShippingAddr5.Attributes["placeholder"] = ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode); %>
				<w2c:ExtendedTextBox id="tbShippingAddr5" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
				<% } %>
			<% } %>
		</dd>
		<%-- 郵便番号（海外向け） --%>
		<% if (this.IsShippingAddrJp == false) { %>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
			<% if (this.IsShippingAddrZipNecessary) { %><span class="require">※</span><% } %>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator
					ID="cvShippingZipGlobal"
					runat="Server"
					ControlToValidate="tbShippingZipGlobal"
					ValidationGroup="UserShippingRegistGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbShippingZipGlobal" type="tel" Runat="server" MaxLength="20"></w2c:ExtendedTextBox>
			<asp:LinkButton
				ID="lbSearchAddrFromShippingZipGlobal"
				OnClick="lbSearchAddrFromShippingZipGlobal_Click"
				Style="display:none;"
				runat="server" />
		</dd>
		<% } %>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<dt>
			<%-- 企業名 --%>
			<%: ReplaceTag("@@User.company_name.name@@")%>
		</dt>
		<dd class="company-name">
			<p class="attention">
			<asp:CustomValidator
				ID="cvShippingCompanyName"
				runat="Server"
				ControlToValidate="tbShippingCompanyName"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<% tbShippingCompanyName.Attributes["placeholder"] = ReplaceTag("@@User.company_name.name@@"); %>
			<w2c:ExtendedTextBox id="tbShippingCompanyName" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
		</dd>
		<dt>
			<%-- 部署名 --%>
			<%: ReplaceTag("@@User.company_post_name.name@@")%>
		</dt>
		<dd class="company-post">
			<p class="attention">
			<asp:CustomValidator
				ID="cvShippingCompanyPostName"
				runat="Server"
				ControlToValidate="tbShippingCompanyPostName"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
				<% tbShippingCompanyPostName.Attributes["placeholder"] = ReplaceTag("@@User.company_post_name.name@@"); %>
				<w2c:ExtendedTextBox id="tbShippingCompanyPostName" Runat="server" MaxLength="40"></w2c:ExtendedTextBox>
			</dd>
		<%} %>
		<% if (this.IsShippingAddrJp) { %>
		<dt>
			<%-- 電話番号 --%>
			<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %><span class="require">※</span>
		</dt>
		<dd class="tel">
			<p class="attention">
			<asp:CustomValidator
				ID="cvShippingTel1_1"
				runat="Server"
				ControlToValidate="tbShippingTel1"
				ValidationGroup="UserShippingRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbShippingTel1" Type="tel" MaxLength="13" style="width:100%;" runat="server" CssClass="shortTel" />
		</dd>
		<% } else { %>
		<dt>
			<%-- 電話番号 --%>
			<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
			<span class="require">※</span>
		</dt>
		<dd class="tel">
			<p class="attention">
				<asp:CustomValidator
					ID="cvShippingTel1Global"
					runat="Server"
					ControlToValidate="tbShippingTel1Global"
					ValidationGroup="UserShippingRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbShippingTel1Global" type="tel" Runat="server" MaxLength="30"></w2c:ExtendedTextBox>
		</dd>
		<% } %>
	</dl>
</div>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbConfirm" ValidationGroup="UserShippingRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" CssClass="btn">確認する</asp:LinkButton>
	</div>
</div>

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
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする --%>
	function bindExecAutoKana() {
		execAutoKanaWithKanaType(
			$("#<%= tbShippingName1.ClientID %>"),
			$("#<%= tbShippingNameKana1.ClientID %>"),
			$("#<%= tbShippingName2.ClientID %>"),
			$("#<%= tbShippingNameKana2.ClientID %>"));
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		execAutoChangeKanaWithKanaType(
			$("#<%= tbShippingNameKana1.ClientID %>"),
			$("#<%= tbShippingNameKana2.ClientID %>"));
	}

	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		// Check zip code input on click
		clickSearchZipCodeForSp(
			'<%= this.WtbShippingZip.ClientID %>',
			'<%= this.WtbShippingZip1.ClientID %>',
			'<%= this.WtbShippingZip2.ClientID %>',
			'<%= this.WlbSearchShippingAddr.ClientID %>',
			'<%= this.WlbSearchShippingAddr.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>');

		// Check zip code input on text box change
		textboxChangeSearchZipCodeForSp(
			'<%= this.WtbShippingZip.ClientID %>',
			'<%= this.WtbShippingZip1.ClientID %>',
			'<%= this.WtbShippingZip2.ClientID %>',
			'<%= this.WtbShippingZip1.UniqueID %>',
			'<%= this.WtbShippingZip2.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage');

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search zip global
		textboxChangeSearchGlobalZip(
			'<%= this.WtbShippingZipGlobal.ClientID %>',
			'<%= this.WlbSearchAddrFromShippingZipGlobal.UniqueID %>');
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
		$("#<%= ddlShippingAddr1.ClientID %>").val(selectedAddr.find('.addr').text());
		$("#<%= tbShippingAddr2.ClientID %>").val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
		$("#<%= tbShippingAddr3.ClientID %>").focus();

		closePopupAndLayer();
	}

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		$('#<%= this.WddlShippingAddr3.ClientID %>').change(function (e) {
			$('#<%= this.WtbShippingZipGlobal.ClientID %>').val(
				$('#<%= this.WddlShippingAddr3.ClientID %>').val().split('|')[0]);
		});
	}
	<% } %>
//-->
</script>

</asp:Content>