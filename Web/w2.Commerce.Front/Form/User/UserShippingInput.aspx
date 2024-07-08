<%--
=========================================================================================================
  Module      : アドレス帳入力画面(UserShippingInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserShippingInput.aspx.cs" Inherits="Form_User_UserShippingInput" Title="アドレス帳入力ページ" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<div id="dvUserFltContents">
	<%-- メッセージ --%>
	<div id="dvHeaderUserShippingClumbs">
	<p>
		<img src="../../Contents/ImagesPkg/user/clumbs_usershipping_1.gif" alt="アドレス帳の入力" /></p>
	</div>

		<h2>アドレス帳の入力</h2>

	<div id="dvUserShippingInput" class="unit">
		<div class="dvContentsInfo">
			<p id="pRegistInfo" runat="server" visible="false">アドレス帳に新しいお届け先を登録します。<br/>下のフォームに入力し、「確認する」ボタンを押してください。
			登録する住所には、「配送先名」を登録する事ができます。（例：「実家」、「お店」など）</p>
			<p id="pModifyInfo" runat="server" visible="false">アドレス帳に登録されているお届け先を編集します。<br/>下のフォームに入力し、「確認する」ボタンを押してください。</p>
		</div>
		<div class="dvUserShippingInfo">
			<h3>アドレス帳情報</h3>
			<ins><span class="necessary">*</span>は必須入力となります。</ins>
			
			<%-- UPDATE PANEL開始 --%>
			<asp:UpdatePanel ID="upUpdatePanel" runat="server">
			<ContentTemplate>
			<table cellspacing="0">
				<tr>
					<th>配送先名<span class="necessary">*</span></th>
					<td>
						<asp:TextBox id="tbName" Runat="server" maxlength="30" CssClass="nameShipping"></asp:TextBox>
						<asp:CustomValidator
							ID="cvName"
							runat="Server"
							ControlToValidate="tbName"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<%-- 氏名 --%>
					<th><%: ReplaceTag("@@User.name.name@@") %><span class="necessary">*</span></th>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
									<% SetMaxLength(this.WtbShippingName1, "@@User.name1.length_max@@"); %>
									<span class="fname">姓</span><asp:TextBox id="tbShippingName1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(this.WtbShippingName2, "@@User.name2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox id="tbShippingName2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：山田</span></td>
								<td><span class="notes">太郎</span></td>
							</tr>
						</table>
						<asp:CustomValidator
							ID="cvShippingName1"
							runat="Server"
							ControlToValidate="tbShippingName1"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvShippingName2"
							runat="Server"
							ControlToValidate="tbShippingName2"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsShippingAddrJp) { %>
				<tr>
					<%-- 氏名（かな） --%>
					<th><%: ReplaceTag("@@User.name_kana.name@@") %><span class="necessary">*</span></th>
					<td>
						<table cellspacing="0">
							<tr>
								<td>
									<% SetMaxLength(this.WtbShippingNameKana1, "@@User.name_kana1.length_max@@"); %>
									<span class="fname">姓</span><asp:TextBox id="tbShippingNameKana1" Runat="server" CssClass="nameFirst"></asp:TextBox></td>
								<td>
									<% SetMaxLength(this.WtbShippingNameKana2, "@@User.name_kana2.length_max@@"); %>
									<span class="lname">名</span><asp:TextBox id="tbShippingNameKana2" Runat="server" CssClass="nameLast"></asp:TextBox><span class="notes">※全角ひらがな入力</span></td>
							</tr>
							<tr>
								<td><span class="notes">例：やまだ</span></td>
								<td><span class="notes">たろう</span></td>
							</tr>
						</table>
						<asp:CustomValidator
							ID="cvShippingNameKana1"
							runat="Server"
							ControlToValidate="tbShippingNameKana1"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvShippingNameKana2"
							runat="Server"
							ControlToValidate="tbShippingNameKana2"
							ValidationGroup="UserShippingRegist"
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
						<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
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
					</td>
				</tr>
				<% } %>
				<% if (this.IsShippingAddrJp) { %>
				<tr>
					<%-- 郵便番号 --%>
					<th>
						<%: ReplaceTag("@@User.zip.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbShippingZip" MaxLength="8" Type="tel" OnTextChanged="lbSearchShippingAddr_Click" runat="server" />
						<asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" class="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton><br/>
						<%--検索結果レイヤー--%>
						<uc:Layer ID="ucLayer" runat="server" />
						<asp:CustomValidator
							ID="cvShippingZip1"
							runat="Server"
							ControlToValidate="tbShippingZip"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline zip_input_error_message" />
						<%-- エラーメッセージ --%>
						<span style="color :Red" id="addrSearchErrorMessage" class="shortZipInputErrorMessage">
							<%: this.ZipInputErrorMessage %></span>
					</td>
				</tr>
				<tr>
					<%-- 都道府県 --%>
					<th><%: ReplaceTag("@@User.addr1.name@@") %><span class="necessary">*</span></th>
					<td>
						<asp:DropDownList id="ddlShippingAddr1" runat="server" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
						<asp:CustomValidator
							ID="cvShippingAddr1"
							runat="Server"
							ControlToValidate="ddlShippingAddr1"
							ValidationGroup="UserShippingRegist"
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
						<%: ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<% if (IsCountryTw(ddlShippingCountry.SelectedValue)) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr2" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
						<% } else { %>
							<% SetMaxLength(this.WtbShippingAddr2, "@@User.addr2.length_max@@"); %>
							<asp:TextBox id="tbShippingAddr2" Runat="server" CssClass="addr"></asp:TextBox><span class="notes">※全角入力</span>
							<asp:CustomValidator
								ID="cvShippingAddr2"
								runat="Server"
								ControlToValidate="tbShippingAddr2"
								ValidationGroup="UserShippingRegist"
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
						<%: ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>
						<% if (IsAddress3Necessary(this.ShippingAddrCountryIsoCode)){ %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<% if (IsCountryTw(ddlShippingCountry.SelectedValue)) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr3" AutoPostBack="true" DataTextField="Key" DataValueField="Value" Width="95"></asp:DropDownList>
						<% } else { %>
							<% SetMaxLength(this.WtbShippingAddr3, "@@User.addr3.length_max@@"); %>
							<asp:TextBox id="tbShippingAddr3" Runat="server" CssClass="addr2"></asp:TextBox>
							<asp:CustomValidator
								ID="cvShippingAddr3"
								runat="Server"
								ControlToValidate="tbShippingAddr3"
								ValidationGroup="UserShippingRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- ビル・マンション名 --%>
					<th>
						<%: ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode) %>
						<% if (this.IsShippingAddrJp == false) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<% SetMaxLength(this.WtbShippingAddr4, "@@User.addr4.length_max@@"); %>
						<asp:TextBox id="tbShippingAddr4" Runat="server" CssClass="addr2"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingAddr4"
							runat="Server"
							ControlToValidate="tbShippingAddr4"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsShippingAddrJp == false) { %>
				<tr>
					<%-- 郵便番号（海外向け） --%>
					<th>
						<%: ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>
						<% if (this.IsShippingAddrUs) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
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
								CssClass="error_inline" />
						<% } else { %>
						<asp:TextBox ID="tbShippingAddr5" runat="server"></asp:TextBox>
						<% } %>
					</td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
						<% if (this.IsShippingAddrZipNecessary) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<asp:TextBox ID="tbShippingZipGlobal" MaxLength="20" runat="server"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingZipGlobal"
							runat="Server"
							ControlToValidate="tbShippingZipGlobal"
							ValidationGroup="UserShippingRegistGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:LinkButton
							ID="lbSearchAddrFromShippingZipGlobal"
							runat="server"
							OnClick="lbSearchAddrFromShippingZipGlobal_Click"
							Style="display:none;" />
					</td>
				</tr>
				<% } %>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<tr>
					<%-- 企業名 --%>
					<th>
						<%: ReplaceTag("@@User.company_name.name@@") %>
						<span class="necessary"></span>
					</th>
					<td>
						<% SetMaxLength(this.WtbShippingCompanyName, "@@User.company_name.length_max@@"); %>
						<asp:TextBox id="tbShippingCompanyName" Runat="server" CssClass="addr2"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingCompanyName"
							runat="Server"
							ControlToValidate="tbShippingCompanyName"
							ValidationGroup="UserShippingRegist"
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
						<% SetMaxLength(this.WtbShippingCompanyPostName, "@@User.company_post_name.length_max@@"); %>
						<asp:TextBox id="tbShippingCompanyPostName" Runat="server" CssClass="addr2"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingCompanyPostName"
							runat="Server"
							ControlToValidate="tbShippingCompanyPostName"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<%} %>
				<tr>
					<%-- 電話番号 --%>
					<% if (this.IsShippingAddrJp) { %>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbShippingTel1" MaxLength="13" Type="tel" runat="server" CssClass="shortTel" />
						<asp:CustomValidator
							ID="cvShippingTel1_1"
							runat="Server"
							ControlToValidate="tbShippingTel1"
							ValidationGroup="UserShippingRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
					<% } else { %>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbShippingTel1Global" MaxLength="30" runat="server" Type="tel"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingTel1Global"
							runat="Server"
							ControlToValidate="tbShippingTel1Global"
							ValidationGroup="UserShippingRegistGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
					<% } %>
				</tr>
			</table>
			</ContentTemplate>
			</asp:UpdatePanel>
			<%-- UPDATE PANELここまで --%>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_LIST) %>" class="btn btn-large">戻る</a></span>
				<span><asp:LinkButton ID="lbConfirm" ValidationGroup="UserShippingRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">確認する</asp:LinkButton></span>
			</p>
		</div>
	</div>
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
		clickSearchZipCode(
			'<%= this.WtbShippingZip.ClientID %>',
			'<%= this.WtbShippingZip1.ClientID %>',
			'<%= this.WtbShippingZip2.ClientID %>',
			'<%= this.WlbSearchShippingAddr.ClientID %>',
			'<%= this.WlbSearchShippingAddr.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage');

		// Check zip code input on text box change
		textboxChangeSearchZipCode(
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