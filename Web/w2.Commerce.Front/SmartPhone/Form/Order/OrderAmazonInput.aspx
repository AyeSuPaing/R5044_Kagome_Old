<%--
=========================================================================================================
  Module      : Amazonペイメント画面(OrderAmazonInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderAmazonInput.aspx.cs" Inherits="Form_Order_OrderAmazonInput" Title="配送先・支払方法選択画面"%>
<%@ Import Namespace="System.Security.Policy" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
	<div class="order-unit">
		<asp:Repeater runat="server" ID="rCartList" DataSource="<%# this.CartList %>" >
		<ItemTemplate>

	<asp:UpdatePanel ID="upOwnerInfo" Visible="<%# this.IsOrderOwnerInputEnabled %>" runat="server">
	<ContentTemplate>
		<%-- ▼注文者情報▼ --%>
		<div class="owner">
			<h2>注文者情報</h2>
			<%if (this.IsEasyUser) {%>
			<p style="margin:5px;padding:5px;text-align:center;background-color:#ffff80;border:1px solid #D4440D;border-color:#E5A500;color:#CC7600;">ご購入手続きに必要な会員情報が不足しています。</p>
			<%} %>
			<dl class="order-form">
				<%-- 氏名 --%>
				<dt>
					<%: ReplaceTag("@@User.name.name@@") %>
					<span class="require">※</span><span id="efo_sign_name"/>
				</dt>
				<dd class="name">
					<p class="attention">
					<asp:CustomValidator ID="cvOwnerName1" runat="Server"
						ControlToValidate="tbOwnerName1"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
					<asp:CustomValidator ID="cvOwnerName2" runat="Server"
						ControlToValidate="tbOwnerName2"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
					</p>
					<% tbOwnerName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
					<% tbOwnerName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
					<w2c:ExtendedTextBox ID="tbOwnerName1" Text="<%# this.CartList.Owner.Name1 %>" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					<w2c:ExtendedTextBox ID="tbOwnerName2" Text="<%# this.CartList.Owner.Name2 %>" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
				</dd>
				<%-- 氏名（かな） --%>
				<dt>
					<%: ReplaceTag("@@User.name_kana.name@@") %>
					<% if (IsTargetToExtendedAmazonAddressManagerOption() == false) { %><span class="require">※</span><span id="efo_sign_kana"/><% } %>
				</dt>
				<dd class="name-kana">
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
					<% tbOwnerNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name_kana1.name@@"); %>
					<% tbOwnerNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name_kana2.name@@"); %>
					<w2c:ExtendedTextBox ID="tbOwnerNameKana1" Text="<%# this.CartList.Owner.NameKana1 %>" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					<w2c:ExtendedTextBox ID="tbOwnerNameKana2" Text="<%# this.CartList.Owner.NameKana2 %>" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
				</dd>
				<%-- 生年月日 --%>
				<dt>
					<%: ReplaceTag("@@User.birth.name@@") %>
					<% if (IsTargetToExtendedAmazonAddressManagerOption() == false) { %><span class="require">※</span><span id="efo_sign_birth"/><% } %>
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
					<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Year.ToString() : "1970" %>' CssClass="year" runat="server"></asp:DropDownList>
					年 
					<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Month.ToString() : "1" %>' CssClass="month" runat="server"></asp:DropDownList>
					月 
					<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Day.ToString() : "1" %>' CssClass="date" runat="server"></asp:DropDownList>
					日
				</dd>
				<%-- 性別 --%>
				<dt>
					<%: ReplaceTag("@@User.sex.name@@") %>
					<span class="require">※</span><span id="efo_sign_sex"/>
				</dt>
				<dd class="sex">
					<p class="attention">
					<asp:CustomValidator ID="cvOwnerSex" runat="Server"
						ControlToValidate="rblOwnerSex"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						EnableClientScript="false" />
					</p>
					<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%# GetCorrectSexForDataBind(this.CartList.Owner.Sex) %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" />
				</dd>
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
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false" />
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
				<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
				<dt><%: ReplaceTag("@@User.mail_addr2.name@@") %></dt>
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
				<%-- 郵便番号 --%>
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
						SetFocusOnError="true" />
					<span id="sOwnerZipError" runat="server" class="attention shortZipInputErrorMessage"></span>
					</p>
					<w2c:ExtendedTextBox ID="tbOwnerZip" Type="tel" Text="<%# this.CartList.Owner.Zip %>" OnTextChanged="lbSearchOwnergAddr_Click" MaxLength="8" runat="server" />
					<br />
					<asp:LinkButton ID="lbSearchOwnergAddr" runat="server" onclick="lbSearchOwnergAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">郵便番号から住所を入力</asp:LinkButton>
					<%--検索結果レイヤー--%>
					<uc:Layer ID="ucLayer" runat="server" />
				</dd>
				<dt>住所<span class="require">※</span><span id="efo_sign_address"/></dt>
				<dd class="address">
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
					<%-- 市区町村 --%>
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
					<%-- 番地 --%>
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
					<%-- ビル・マンション名 --%>
					<p class="attention">
					<asp:CustomValidator ID="CustomValidator28" runat="Server"
						ControlToValidate="tbOwnerAddr4"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
					</p>
					<w2c:ExtendedTextBox ID="tbOwnerAddr4" placeholder='建物名' Text="<%# this.CartList.Owner.Addr4 %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
				</dd>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
					<%-- 企業名 --%>
					<dt><%: ReplaceTag("@@User.company_name.name@@")%></dt>
					<dd class="company-name">
						<p class="attention">
						<asp:CustomValidator runat="Server"
							ControlToValidate="tbOwnerCompanyName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbOwnerCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' Text="<%# this.CartList.Owner.CompanyName %>" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%-- 部署名 --%>
					<dt><%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
					<dd class="company-post">
						<p class="attention">
						<asp:CustomValidator runat="Server"
							ControlToValidate="tbOwnerCompanyPostName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"　/>
						</p>
						<w2c:ExtendedTextBox ID="tbOwnerCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' Text="<%# this.CartList.Owner.CompanyPostName %>" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
				<%} %>
				<%-- 電話番号 --%>
				<dt>
					<%: ReplaceTag("@@User.tel1.name@@") %>
					<span class="require">※</span><span id="efo_sign_tel1"/>
				</dt>
				<dd class="tel">
					<p class="attention">
					<asp:CustomValidator
						ID="cvOwnerTel1_1"
						runat="Server"
						ControlToValidate="tbOwnerTel1"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
					</p>
					<w2c:ExtendedTextBox ID="tbOwnerTel1" Text="<%#: this.CartList.Owner.Tel1 %>" Type="tel" MaxLength="13" style="width:100%;" CssClass="shortTel" runat="server" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
					<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
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
				<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
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
				<%-- 電話番号（予備） --%>
				<dt><%: ReplaceTag("@@User.tel2.name@@") %></dt>
				<dd class="tel">
					<p class="attention">
					<asp:CustomValidator
						ID="cvOwnerTel2_1"
						runat="Server"
						ControlToValidate="tbOwnerTel2"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="False"
						SetFocusOnError="true" />
					</p>
					<w2c:ExtendedTextBox ID="tbOwnerTel2" Text="<%# this.CartList.Owner.Tel2 %>" Type="tel" MaxLength="13" style="width:100%;" CssClass="shortTel" runat="server" ></w2c:ExtendedTextBox>
				</dd>
				<dt>
					<%: ReplaceTag("@@User.mail_flg.name@@") %>
				</dt>
				<dd>
					<asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" runat="server" />
				</dd>

				<% if (IsTargetToExtendedAmazonAddressManagerOption()) { %>
				<asp:UpdatePanel id="upUserRegistRegulationForAmazonPay" UpdateMode="Conditional" runat="server">
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
					<div id="dvUserRegisterRegulationVisible" runat="server">
						<div id="dvUserBox">
							<div id="dvUserRegistRegulation" class="order-unit">
								<%-- メッセージ --%>
								<h3>会員規約について</h3>
								<div class="dvContentsInfo">
									<p>「<%: ShopMessage.GetMessage("ShopName") %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
									</p>
								</div>

								<div class="regulation">
									<uc:UserRegistRegulationMessage ID="UserRegistRegulationMessage1" runat="server" />
								</div>
							</div>
						</div>
					</div>
					</ContentTemplate>
				</asp:UpdatePanel>
				<% } %>
			</dl>
		</div>
		<%-- ▲注文者情報▲ --%>
		<% if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) { %>
		<asp:LinkButton ID="lbCheckAuthenticationCode" name="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
		<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
		<% } %>
	</ContentTemplate>
	</asp:UpdatePanel>
		<div id="ownerAddressBookContainer">
		<dl class="order-form" visible="<%# this.IsOrderOwnerInputEnabled == false %>" runat="server">
			<dt>注文者情報</dt>
			<%--▼▼ Amazon Pay(CV2)注文者情報 ▼▼--%>
			<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
			<dd visible="<%# this.IsOrderOwnerInputEnabled == false %>" runat="server">
				<dl>
					<%-- 氏名 --%>
					<dt>
						<%: ReplaceTag("@@User.name.name@@") %>：
					</dt>
					<dd>
						<p><%: this.AmazonCheckoutSession.Buyer.Name %></p>
					</dd>
					<%-- メールアドレス --%>
					<dt>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>：
					</dt>
					<dd>
						<p><%: this.AmazonCheckoutSession.Buyer.Email %></p>
					</dd>
				</dl>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.mail_flg.name@@") %>
			</dt>
			<dd>
				<asp:CheckBox ID="cbGuestOwnerMailFlg2" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" runat="server" />
			</dd>
				
			<%-- ▼Amazon Pay会員登録▼ --%>
			<% if (this.AmazonPayRegisterVisible) { %>
			<dd>
				<asp:UpdatePanel ID="upAmazonPayRegisterUpdatePanel" runat="server" UpdateMode="Conditional" Visible="<%# this.IsAmazonLoggedIn && (this.IsUserRegistedForAmazon == false) && (this.ExistsUserWithSameAmazonEmailAddress == false) %>">
					<ContentTemplate>
						<dd>
							<asp:CheckBox ID="cbUserRegisterForExternalSettlement" Checked="true" Text=" Amazonお届け先住所で会員登録する " OnCheckedChanged="cbUserRegisterForExternalSettlement_OnCheckedChanged" CssClass="checkBox" runat="server" AutoPostBack="true" />
						</dd>
						<div id="dvUserBoxVisible" runat="server">
							<div class="order-unit">
								<%-- メッセージ --%>
								<h3>会員規約について</h3>
								<div class="dvContentsInfo">
									<p>「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
									</p>
								</div>
								<div class="regulation">
									<uc:UserRegistRegulationMessage runat="server" />
								</div>
							</div>
						</div>
					</ContentTemplate>
				</asp:UpdatePanel>
			</dd>
			<% } %>
			<%-- ▲Amazon Pay会員登録▲ --%>

			<% } else { %>
			<%--▲▲ Amazon Pay(CV2)注文者情報 ▲▲--%>
			<dd class="name">
				<%--▼▼Amazonアドレス帳ウィジェット(注文者情報)▼▼--%>
				<div id="ownerAddressBookWidgetDiv"></div>
				<div id="ownerAddressBookErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
				<%--▲▲Amazonアドレス帳ウィジェット(注文者情報)▲▲--%>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.mail_flg.name@@") %>
			</dt>
			<dd>
				<asp:CheckBox ID="cbGuestOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" runat="server" />
			</dd>
			<% } %>
		</dl>
	</div>
	<%--▼▼AmazonリファレンスID格納▼▼--%>
	<asp:HiddenField runat="server" ID="hfAmazonOrderRefID" />
	<%--▲▲AmazonリファレンスID格納▲▲--%>
		<dl class="order-form product">
			<%-- 注文商品 --%>
			<dt>注文商品</dt>
			<dd>
			<%-- ▼商品リスト▼ --%>
				<asp:Repeater id="repProduct" DataSource="<%# ((CartObject)Container.DataItem).Items %>" Runat="server">
				<HeaderTemplate>
					<table class="cart-table">
					<tbody>
				</HeaderTemplate>
				<ItemTemplate>
					<tr class="<%# (((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1) ? "last" : "" %>">
					<td class="product-image">
					<a id="A7" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<w2c:ProductImage ID="ProductImage7" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
					<w2c:ProductImage ID="ProductImage8" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
					</td>
					<td class="product-info">
						<ul>
							<li class="product-name">
								<a id="A8" href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
								<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
							</li>
							<li class="product-price"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</li>
							<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								※配送料無料適用外商品です
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

	<div id="divShipToOwnerAddress" Visible="<%# (this.IsLoggedIn == false) && CanInputShippingTo(Container.ItemIndex) && (Constants.AMAZON_PAYMENT_CV2_ENABLED == false) %>" runat="server">
		<asp:CheckBox ID="cbShipToOwnerAddress" Text="注文者情報の住所へ配送する" Checked="<%# ((CartObject)Container.DataItem).Shippings[0].IsSameShippingAsCart1 %>" onclick="$('#shippingAddressBookContainer').toggle();" CssClass="checkBox" runat="server" />
	</div>
	
	<div id="shippingAddressBookContainer" style='margin-top: 10px;'>
		<dl class="order-form">
			<dt>配送先情報</dt>
			<%--▼▼ Amazon Pay(CV2)配送先情報 ▼▼--%>
			<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
			<dd>
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
				<a href="#" id="changeAddress" class="btn btn-large">配送先住所を変更</a>
				<p id="pShippingZipError" runat="server" style="color:red;padding:5px"></p>
			</dd>
			<% } else { %>
			<%--▲▲ Amazon Pay(CV2)配送先情報 ▲▲--%>
			<dd class="name">
				<%--▼▼Amazonアドレス帳ウィジェット(配送先情報)▼▼--%>
				<div id="shippingAddressBookWidgetDiv"></div>
				<div id="shippingAddressBookErrorMessage" ClientIDMode="Static" style="color:red;padding:5px" runat="server"></div>
				<%--▲▲Amazonアドレス帳ウィジェット(配送先情報)▲▲--%>
			</dd>
			<% } %>
		</dl>
	</div>

	<asp:UpdatePanel runat="server">
	<ContentTemplate>
		<dl class="order-form shipping-require">
			<div visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server">
				<dt>配送方法を選択して下さい。</dt>
				<dd>
					<dl>
					<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[((RepeaterItem)Container).ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" runat="server"></asp:DropDownList>
					</dl>
				</dd>
			</div>
			<div id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(((RepeaterItem)Container).ItemIndex) && CanDisplayDeliveryCompany(((RepeaterItem)Container).ItemIndex)) %>" runat="server">
				<br />
				<dt>配送サービスを選択して下さい。</dt>
				<dd><asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(((RepeaterItem)Container).ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" runat="server"/></dd>
			</div>
			<br />
			<dt visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送指定</dt>
			<dd id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">
				<dl id="dlShipppingDateTime">
				<dt id="dtShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送希望日</dt>
				<dd id="ddShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">
					<asp:DropDownList ID="ddlShippingDate" CssClass="input_border" runat="server" DataSource="<%# GetShippingDateList(((CartObject)((RepeaterItem)Container).DataItem), this.ShopShippingList[((RepeaterItem)Container).ItemIndex]) %>" DataTextField="text" DataValueField="value" SelectedValue="<%# GetShippingDate((CartObject)((RepeaterItem)Container).DataItem, this.ShopShippingList[((RepeaterItem)Container).ItemIndex]) %>"
						OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnCheckedChanged" AutoPostBack="true"></asp:DropDownList>
					<br />
					<asp:Label ID="lbShippingDateErrorMessage" style="color:red;line-height:1.5;" runat="server" />
					</dd>
				<dt id="dtShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送希望時間帯</dt>
				<dd id="ddShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">
					<asp:DropDownList id="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(((RepeaterItem)Container).ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingTime(((RepeaterItem)Container).ItemIndex) %>"></asp:DropDownList>
					</dd>
				</dl>
			</dd>
		</dl>

		<dl class="order-form memo">
		<%-- 注文メモ --%>
		<dt>注文メモ</dt>
		<dd>
	<asp:Repeater ID="rMemos" runat="server" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos %>" Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos.Count != 0 %>">
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
	<div class="fixed" visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase %>" runat="server"><span id="efo_sign_fixed_purchase"/>
	<h2 visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">定期購入 配送パターンの指定</h2>
	<div id='<%# "efo_sign_fixed_purchase" + ((RepeaterItem)Container).ItemIndex %>'></div>
	<%-- ▽デフォルトチェックの設定▽--%>
		<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
		<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 2, 3, 1, 4) %>
	<%-- △ - - - - - - - - - - - △--%>
		<dl class="order-form" visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">
		<dd id="Div4" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
				Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 1) %>" 
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_month" + ((RepeaterItem)Container).ItemIndex %>' />
			<div id="ddFixedPurchaseMonthlyPurchase_Date" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">
			<asp:DropDownList ID="ddlFixedPurchaseMonth"
				DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
			</asp:DropDownList>
				ヶ月ごと
			<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
				DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, false, true) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
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
		<dd id="Dd3" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
				Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 2) %>" 
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_week_and_day" + ((RepeaterItem)Container).ItemIndex %>' />
			<div id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">
			<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
				DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, true) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server" />
			ヶ月ごと
			<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
				DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
			</asp:DropDownList>
			<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
				DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
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
		<dd id="Div6" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
				Text="配送日間隔指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 3) %>" 
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_interval_days" + ((RepeaterItem)Container).ItemIndex %>' />
			<div id="ddFixedPurchaseRegularPurchase_IntervalDays" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">
				<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
					DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, false) %>'
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
					日ごとに届ける
			</div>
			<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
			<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
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
		<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
				Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 4) %>"
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%#: "efo_sign_fixed_purchase_week" + ((RepeaterItem)Container).ItemIndex %>' />
			<div id="ddFixedPurchaseEveryNWeek" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">
				<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
					DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container).ItemIndex, true) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
				週間ごと
				<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
					DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container).ItemIndex, false) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
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
				SetFocusOnError="true"
				CssClass="error_inline"/>
			<asp:CustomValidator
				ID="cvFixedPurchaseEveryNWeekDayOfWeek"
				runat="Server"
				ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				CssClass="error_inline"/>
			</p>
		</dd>
	</dl>
	<dl class="order-form">
		<dt id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</dt>
		<dd id="Dd4" visible="true" runat="server">
			<asp:DropDownList
				ID="ddlFirstShippingDate"
				visible="false"
				OnDataBound="ddlFirstShippingDate_OnDataBound"
				AutoPostBack="True"
				OnSelectedIndexChanged="ddlFirstShippingDate_ItemSelected"
				runat="server" />
			<asp:Label ID="lblFirstShippingDate" runat="server"></asp:Label>
			<asp:Label ID="lblFirstShippingDateNoteMessage" visible="false" runat="server">
				<br>配送予定日は変更となる可能性がありますことをご了承ください。
			</asp:Label>
			<asp:Literal ID="lFirstShippingDateDayOfWeekNoteMessage" visible="false" runat="server">
				<br>曜日指定は次回配送日より適用されます。
			</asp:Literal>
		</dd>
		<dt id="dtNextShippingDate" visible="true" runat="server">2回目の配送日を選択</dt>
		<dd id="Dd5" visible="true" runat="server">
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

	</ContentTemplate>
	</asp:UpdatePanel>

		<dl class="order-form">
			<dt>お支払い情報</dt>
			<%--▼▼ Amazon Pay(CV2)お支払方法 ▼▼--%>
			<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
			<dd>
			<div>
				<img src="../../../Contents/ImagesPkg/Amazon/logo-amzn_pay.png" height="20px" align="top">&nbsp;&nbsp;
				<%= (this.AmazonCheckoutSession.PaymentPreferences.Count > 0) ? this.AmazonCheckoutSession.PaymentPreferences[0].PaymentDescriptor : "" %>
				<a href="#" id="changePayment" class="btn btn-large">お支払い方法を変更</a>
			</div>
			</dd>
			<% } else { %>
			<%--▲▲ Amazon Pay(CV2)お支払方法 ▲▲--%>
			<dd class="name">
				<%--▼▼Amazon決済ウィジェット▼▼--%>
				<div id="walletWidgetDiv"></div>
				<%--▲▲Amazon決済ウィジェット▲▲ --%>
			<% if (this.CartList.Items[0].HasFixedPurchase) { %>
			<div style="margin: 10px 0;">下記のお支払い契約に同意してください。</div>
			<%--▼▼Amazon支払契約同意ウィジェット▼▼--%>
			<div id="consentWidgetDiv"></div>
			<%--▲▲Amazon支払契約同意ウィジェット▲▲--%>
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
			</dd>
		</dl>
		<%-- ▼領収書情報▼ --%>
		<asp:UpdatePanel ID="upReceiptInfo" runat="server">
		<ContentTemplate>
		<% if (Constants.RECEIPT_OPTION_ENABLED && (Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) == false)) { %>
		<dl class="order-form receipt-form" id="divReceipt">
			<dt>領収書情報</dt>
			<dd id="divReceiptInfoInputForm" runat="server">
				<strong>領収書希望有無を選択してください。</strong>
				<dd><asp:DropDownList id="ddlReceiptFlg" runat="server" DataTextField="text" DataValueField="value" DataSource="<%# this.ReceiptFlgListItems %>"
						SelectedValue="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptFlg %>"
						OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" />
				</dd>
				<div id="divReceiptAddressProviso" runat="server">
					<dt>宛名<span class="attention">※</span></dt>
					<dd>
					<asp:TextBox id="tbReceiptAddress" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptAddress %>" MaxLength="100" Width="450" />
					<p><asp:CustomValidator runat="Server"
						ControlToValidate="tbReceiptAddress"
						ValidationGroup="ReceiptRegisterModify"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false" /></p>
					</dd>
					<dt>但し書き<span class="attention">※</span></dt>
					<dd>
					<asp:TextBox id="tbReceiptProviso" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptProviso %>" MaxLength="100" Width="450" />
					<p><asp:CustomValidator runat="Server"
						ControlToValidate="tbReceiptProviso"
						ValidationGroup="ReceiptRegisterModify"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false" /></p>
					</dd>
				</div><!--divReceiptAddressProviso-->
			</dd><!--divReceiptInfoInputForm-->
		</dl><!--divReceipt-->
		<% } %>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- ▲領収書情報▲ --%>
		</ItemTemplate>
		</asp:Repeater>
	</div>
	
	<div class="cart-footer">
		<div class="button-next">
			<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:LinkButton runat="server" ID="lbNext" CssClass="btn btn-large btn-success" OnClick="lbNext_Click"><%= IsChangeMessageForNextButton() ? "会員登録してご注文内容の確認へ" : "確認画面へ" %></asp:LinkButton>
			</ContentTemplate>
			</asp:UpdatePanel>
		</div>
		<div class="button-prev">
			<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>" class="btn">戻る</a>
		</div>
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

	<%-- HACK：SmartPhoneの場合は画面レンダリング後でないとウィジェットが描画されない --%>
	$(function () {
		$('#shippingAddressBookContainer').css('display', '<%= (this.IsLoggedIn == false) && this.CartList.Items[0].Shippings[0].IsSameShippingAsCart1 ? "none" : "block" %>')
	});

	window.onAmazonLoginReady = function () {
		amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
		amazon.Login.setUseCookie(true);
	};
	window.onAmazonPaymentsReady = function () {
		showAddressBookWidget();
	};

	<%-- Amazonアドレス帳表示ウィジェット --%>
	function showAddressBookWidget() {
		<%-- Amazon注文者情報 --%>
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
					if (data.Error) $ownerAddressBookErrorMessage.html(data.Error);
				});
			},
			design: { designMode: 'smartphoneCollapsible' },
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
					if (data.Error) $shippingAddressBookErrorMessage.html(data.Error);
				});
			},
			design: { designMode: 'smartphoneCollapsible' },
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
			design: { designMode: 'smartphoneCollapsible' },
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
			design: { designMode: 'smartphoneCollapsible' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		}).bind("consentWidgetDiv");
	}

	<%-- Amazon住所取得関数 --%>
	function getAmazonAddress(orderType, addressType, callback) {
		$.ajax({
			type: "POST",
			url: "<%=Constants.PATH_ROOT + "SmartPhone/" +  Constants.PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT%>/GetAmazonAddress",
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
			'<%= this.lbAuthenticationMessage.ClientID %>',
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
<script type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<script type="text/javascript" charset="utf-8">
	amazon.Pay.bindChangeAction('#changeAddress',
		{
			amazonCheckoutSessionId: '<%= this.AmazonCheckoutSession.CheckoutSessionId %>',
			changeAction: 'changeAddress'
		});
	amazon.Pay.bindChangeAction('#changePayment',
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
</asp:Content>
