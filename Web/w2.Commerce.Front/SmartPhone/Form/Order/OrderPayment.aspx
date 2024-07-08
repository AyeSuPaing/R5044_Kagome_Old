<%--
=========================================================================================================
  Module      : スマートフォン用注文お支払い方法選択画面(OrderPayment.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderPayment.aspx.cs" Inherits="Form_Order_OrderPayment" Title="支払方法選択ページ" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/SmartPhone/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionSmsDef" Src="~/Form/Common/Order/PaymentDescriptionSmsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionTriLinkAfterPay" Src="~/Form/Common/Order/PaymentDescriptionTriLinkAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutControl" Src="~/Form/Common/Order/PaidyCheckoutControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionAtone" Src="~/Form/Common/Order/PaymentDescriptionAtone.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionNPAfterPay" Src="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx" %>
<%@ Register tagPrefix="uc" TagName="PaymentDescriptionLinePay" Src="~/Form/Common/Order/PaymentDescriptionLinePay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPay" Src="~/Form/Common/Order/PaymentDescriptionPayPay.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">	
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- 次へイベント用リンクボタン --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" runat="server"></asp:LinkButton>
<%-- 戻るイベント用リンクボタン --%>
<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server"></asp:LinkButton>

<section class="wrap-order order-payment">

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<div class="step">
	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-step03.jpg" alt="お支払情報の選択" width="320" />
</div>

<%-- エラーメッセージ（デフォルト注文方法用） --%>
<span style="color:red;text-align:center;display:block"><asp:Literal ID="lOrderErrorMessage" runat="server"></asp:Literal></span>
<span style="color: red; text-align: left; display: block"><asp:Literal ID="lPaymentErrorMessage" runat="server"></asp:Literal></span>

<%-- ▼エラー情報▼ --%>
<% if (this.ErrorMessages.HasMessages(-1, -1)) { %>
<span style="color:red"><%: this.ErrorMessages.Get(-1, -1) %></span>
<% } %>
<%-- ▲エラー情報▲ --%>

<%-- ▼PayPalログインここから▼ --%>
<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
	<%
		ucPaypalScriptsForm.LogoDesign = "Payment";
		ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
		ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
	%>
	<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
	<div id="paypal-button"></div>
	<%if (SessionManager.PayPalCooperationInfo != null) {%>
		<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済<br/>
	<%} %>
	<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
<%} %>
<%-- ▲PayPalログインここまで▲ --%>

<asp:Repeater id="rCartList" runat="server">
<ItemTemplate>
<div class="order-unit payment">
	<%-- ▼お支払い情報▼ --%>
	<h2><%# this.CartList.Items.Count > 1 ? "カート番号" + (Container.ItemIndex + 1).ToString() + "の" : "" %>お支払い方法</h2>
		
		<asp:CheckBox ID="cbUseSamePaymentAddrAsCart1" visible="<%# (Container.ItemIndex != 0) %>" Checked="<%# ((CartObject)Container.DataItem).Payment.IsSamePaymentAsCart1 %>" Text="カート番号「1」と同じお支払い方法を指定する" OnCheckedChanged="cbUseSamePaymentAddrAsCart1_OrderPayment_OnCheckedChanged" AutoPostBack="true" CssClass="select-same-payment" runat="server" />

		<%-- 注文商品 --%>
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
					<li class="product-price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %></li>
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
		<span style="color:red" runat="server" visible="<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(this.DispLimitedPaymentMessages[Container.ItemIndex])) == false) %>">
			<%# StringUtility.ToEmpty(this.DispLimitedPaymentMessages[Container.ItemIndex]) %>
		</span>
		<%-- ▲商品リスト▲ --%>
		</dd>
		</dl>

		<%--▼▼ クレジット Token保持用（カート1と同じ決済の場合） ▼▼--%>
		<asp:HiddenField ID="hfCreditTokenSameAs1" Value="<%# ((CartObject)Container.DataItem).Payment.CreditTokenSameAs1 %>" runat="server" />
		<%--▲▲ クレジット Token保持用（カート1と同じ決済の場合） ▲▲--%>
		<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
		<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
		<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL) { %>
		<dl class="order-form payment-list">
			<dt><asp:DropDownList ID="ddlPayment" runat="server" DataSource="<%# this.ValidPayments[Container.ItemIndex] %>" visible="<%# (Container.ItemIndex == 0) %>" ItemType="w2.Domain.Payment.PaymentModel" OnSelectedIndexChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" DataTextField="PaymentName" DataValueField="PaymentId" /></dt>
		</dl>
		<% } %>
		<asp:Repeater ID="rPayment" runat="server" DataSource="<%# this.ValidPayments[Container.ItemIndex] %>" ItemType="w2.Domain.Payment.PaymentModel" visible="<%# (Container.ItemIndex == 0) %>">
			<HeaderTemplate>
				<dl class="order-form payment-list">
			</HeaderTemplate>
			<ItemTemplate>
				<asp:HiddenField ID="hfShopId" Value='<%# Item.ShopId %>' runat="server" />
				<asp:HiddenField ID="hfPaymentId" Value='<%# Item.PaymentId %>' runat="server" />
				<asp:HiddenField ID="hfPaymentName" Value='<%# Item.PaymentName %>' runat="server" />
				<asp:HiddenField
					ID="hfCreditBincode"
					Value="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_BINCODE) %>"
					runat="server" />
				<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB) { %>
				<dt class="title">
					<w2c:RadioButtonGroup ID="rbgPayment" Checked="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.PaymentId == Item.PaymentId %>" GroupName='<%# "Payment_" + ((RepeaterItem)Container.Parent.Parent).ItemIndex %>' Text="<%# WebSanitizer.HtmlEncode(Item.PaymentName) %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" runat="server" />
				</dt>
				<% } %>
				<dd id="ddCredit" class="credit-card" runat="server">
					<div class="inner">
					<%-- クレジット --%>
					<div class="box-center" runat="server" visible="<%# OrderCommon.GetRegistedCreditCardSelectable(this.IsLoggedIn, this.CreditCardList.Count - 1)%>">
						<asp:DropDownList ID="ddlUserCreditCard" runat="server" DataSource="<%# this.CreditCardList %>" SelectedValue="<%# GetListItemValue(this.CreditCardList ,((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardBranchNo) %>" OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged" AutoPostBack="true" DataTextField="text" DataValueField="value" ></asp:DropDownList>
					</div>

					<%-- ▽新規カード▽ --%>
					<div id="divCreditCardInputForm" class="new" runat="server" visible="<%# IsNewCreditCard(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>">
					
						<% if (this.IsCreditCardLinkPayment() == false) { %>
						<%--▼▼ クレジット Token保持用 ▼▼--%>
						<asp:HiddenField ID="hfCreditToken" Value="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditToken %>" runat="server" />
						<%--▲▲ クレジット Token保持用 ▲▲--%>

						<%--▼▼ カード情報取得用 ▼▼--%>
						<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%# CreateGetCardInfoJsScriptForCreditTokenForCart(((RepeaterItem)Container.Parent.Parent), Container) %>" />
						<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
						<%--▲▲ カード情報取得用 ▲▲--%>
						
						<ul>
							<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
							<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
								<!-- Rakuten Card Form -->
								<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForRakutenToken_Click" style="display:none" runat="server">再入力</asp:LinkButton>
								<uc:RakutenCreditCard
									IsOrder="true"
									CartIndex="<%# ((RepeaterItem)Container.Parent.Parent).ItemIndex + 1 %>"
									InstallmentCodeList="<%# this.CreditInstallmentsList %>"
									SelectedInstallmentCode = "<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>"
									SelectedExpireMonth="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_MONTH) %>"
									SelectedExpireYear="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_YEAR) %>"
									AuthorName="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_AUTHOR_NAME) %>"
									CreditCardNo4="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardNo4 %>"
									CreditCompany="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_COMPANY) %>"
									SecurityCode="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.RakutenCvvToken %>"
									runat="server"
									ID="ucRakutenCreditCard" />
							<% } else { %>
							<div id="divCreditCardNoToken" visible='<%# (HasCreditToken(Container) == false) %>' runat="server">
							<%if (OrderCommon.CreditCompanySelectable) {%>
							<li class="card-company">
								<h4>カード会社</h4>
								<div><asp:DropDownList id="ddlCreditCardCompany" runat="server" DataSource="<%# this.CreditCompanyList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_COMPANY) %>" CssClass="input_widthG input_border"></asp:DropDownList></div>
							</li>
							<% } %>
							<li class="card-nums">
								<p class="attention">
									<asp:CustomValidator ID="cvCreditCardNo1" runat="Server"
										ControlToValidate="tbCreditCardNo1"
										ValidationGroup="OrderPayment"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										CssClass="error_inline" />
									<span id="sErrorMessage" runat="server" />
								</p>
								<h4>カード番号<span class="require">※</span></h4>
								<div>
									<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" CssClass="tel" MaxLength="16" Text="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_CARD_NO) %>" autocomplete="off"></w2c:ExtendedTextBox>
								</div>
								<p>
									カードの表記のとおりご入力ください。<br />
									例：<br />
									1234567890123456（ハイフンなし）
								</p>
							</li>
								
							<li class="card-exp">
								<h4>有効期限</h4>
								<div>
									<asp:DropDownList id="ddlCreditExpireMonth" runat="server" DataSource="<%# this.CreditExpireMonth %>" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_MONTH) %>"></asp:DropDownList>
										/
									<asp:DropDownList id="ddlCreditExpireYear" runat="server" DataSource="<%# this.CreditExpireYear %>" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_YEAR) %>"></asp:DropDownList>
										 (月/年)
								</div>
							</li>
							<li class="card-name">
								<h4>カード名義人<span class="require">※</span></h4>
								<div>
									<p class="attention">
										<asp:CustomValidator ID="cvCreditAuthorName" runat="Server"
											ControlToValidate="tbCreditAuthorName"
											ValidationGroup="OrderPayment"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											CssClass="error_inline" />
									</p>
									<w2c:ExtendedTextBox id="tbCreditAuthorName" Type="text" runat="server" MaxLength="50" Text="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_AUTHOR_NAME) %>" autocomplete="off"></w2c:ExtendedTextBox>
								<p>例：「TAROU YAMADA」</p>
								</div>
							</li>

							<li class="card-sequrity" visible="<%# OrderCommon.CreditSecurityCodeEnable %>" runat="server">
								<h4>セキュリティコード<span class="require">※</span></h4>
								<div>
									<p class="attention">
										<asp:CustomValidator ID="cvCreditSecurityCode" runat="Server"
											ControlToValidate="tbCreditSecurityCode"
											ValidationGroup="OrderPayment"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											CssClass="error_inline" />
									</p>
									<w2c:ExtendedTextBox id="tbCreditSecurityCode" Type="tel" runat="server" MaxLength="4" Text="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_SECURITY_CODE) %>" autocomplete="off"></w2c:ExtendedTextBox>
									<p>
										<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/card-sequrity-code.gif" alt="セキュリティコードとは" width="280" />
									</p>
								</div>
							</li>
								</div>
							<% } %>
							<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

							<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
							<% if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) { %>
								<div id="divCreditCardForTokenAcquired" Visible='<%# HasCreditToken(Container) %>' runat="server">
								<%if (OrderCommon.CreditCompanySelectable) {%>
								<li>
								<h4>カード会社</h4>
								<div><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardCompanyName %>" runat="server"></asp:Literal><br /></div>
								</li>
								<%} %>
								<li>
								<h4>カード番号 <asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton></h4>
								<div>
								<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardNo4 %>" runat="server"></asp:Literal><br /></p>
								</div>
								</li>
								<li>
								<h4>有効期限</h4>
								<div><asp:Literal ID="lExpirationMonthForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditExpireMonth %>" runat="server"></asp:Literal>
								&nbsp;/&nbsp;
								<asp:Literal ID="lExpirationYearForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditExpireYear %>" runat="server"></asp:Literal> (月/年)</div>
								</li>
								<li>
								<h4>カード名義人</h4>
								<div><asp:Literal ID="lCreditAuthorNameForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditAuthorName %>" runat="server"></asp:Literal><br /></div>
								</li>
								</div>
								<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>

								<li class="card-time" id="Div3" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
									<h4>支払い回数</h4>
									<div>
										<asp:DropDownList id="dllCreditInstallments" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>"></asp:DropDownList>
										<p>AMEX/DINERSは一括のみとなります。</p>
									</div>

								</li>
							<% } %>
							<% } else { %>
								<div>注文完了後に遷移する外部サイトでカード番号を入力してください。</div>
							<% } %>
							
							</ul>

							<div class="box-center">
							<asp:CheckBox ID="cbRegistCreditCard" runat="server" Checked="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.UserCreditCardRegistFlg %>" Visible="<%# OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, this.CreditCardList.Count - 1) %>" Text="このカードを登録する" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" AutoPostBack="true" />
							</div>

							<div id="divUserCreditCardName" class="card-save" Visible="<%# (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) %>" runat="server">
								<p class="msg">クレジットカードを保存する場合は、以下をご入力ください。</p>
								<h4>登録名<span class="require">※</span></h4>
								<asp:TextBox ID="tbUserCreditCardName" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.UserCreditCardName %>" MaxLength="100" Width="100%" CssClass="input_widthD input_border" runat="server" />
								<% if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) {%>
								<div>
									<p class="attention">
									<asp:CustomValidator ID="cvUserCreditCardName" runat="Server"
										ControlToValidate="tbUserCreditCardName"
										ValidationGroup="OrderPayment"
										ValidateEmptyText="true"
										SetFocusOnError="true" />
									</p>
								</div>
								<% } %>
							</div>

						</div>
						<%-- △新規カード△ --%>

						<%-- ▽登録済みカード▽ --%>
						<div id="divCreditCardDisp" visible="<%# IsNewCreditCard(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) == false %>" runat="server">
							<ul>
								<%if (OrderCommon.CreditCompanySelectable) {%>
								<li>
									<h4>カード会社</h4>
									<div>
										<asp:Literal ID="lCreditCardCompanyName" runat="server"></asp:Literal>
									</div>
								</li>
								<%} %>
								<li>
									<h4>カード番号</h4>
									<div>
										XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal>
									</div>
								</li>
								<li>
									<h4>有効期限</h4>
									<div>
										<asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>&nbsp;/&nbsp;<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)
									</div>
								</li>
								<li>
									<h4>カード名義人</h4>
									<div>
										<asp:Literal ID="lCreditAuthorName" runat="server"></asp:Literal>
									</div>
								</li>
								<li visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
									<h4>支払い回数</h4>
									<div><asp:DropDownList id="dllCreditInstallments2" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>"></asp:DropDownList>
									<p class="attention">※AMEX/DINERSは一括のみとなります。</p></div>
								</li>
							</ul>
						</div>
						<%-- △登録済みカード△ --%>
					</div>
				</dd>

				<%-- 代金引換 --%>
				<dd id="ddCollect" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) %>" runat="server">
				</dd>

				<%-- コンビニ(前払い) --%>
				<dd id="ddCvsPre" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE) %>" runat="server">

					<%-- コンビニ(前払い)：電算システム --%>
					<div id="Div4" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Dsk) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p><asp:DropDownList ID="ddlDskCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetDskConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>

					<%-- コンビニ(前払い)：SBPS --%>
					<div id="Div1" class="inner" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.SBPS) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p><asp:DropDownList ID="ddlSBPSCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetSBPSConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>
					
					<%-- コンビニ(前払い)：ヤマトKWC --%>
					<div class="inner" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p><asp:DropDownList ID="ddlYamatoKwcCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetYamatoKwcConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>

					<%-- コンビニ(前払い)：Gmo --%>
					<div class="inner" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p><asp:DropDownList ID="ddlGmoCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetGmoConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>
					<%-- コンビニ(前払い)：Rakuten --%>
					<div class="inner" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p>
							<asp:DropDownList
								ID="ddlRakutenCvsType"
								DataSource='<%# this.CvsTypeList %>'
								DataTextField="Text"
								DataValueField="Value"
								SelectedValue='<%# GetRakutenConvenienceType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>'
								CssClass="input_widthC input_border"
								runat="server" />
						</p>
					</div>
					<%-- コンビニ(前払い)：Zeus --%>
					<div class="inner" visible="<%# OrderCommon.IsPaymentCvsTypeZeus %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p>
							<asp:DropDownList
								ID="ddlZeusCvsType"
								DataSource='<%# this.CvsTypeList %>'
								DataTextField="Text"
								DataValueField="Value"
								SelectedValue='<%# GetZeusConvenienceType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>'
								CssClass="input_widthC input_border"
								runat="server" />
						</p>
					</div>
					<%-- コンビニ(前払い)：Paygent --%>
					<div class="inner" visible="<%# OrderCommon.IsPaymentCvsTypePaygent %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p>
							<asp:DropDownList
								ID="ddlPaygentCvsType"
								DataSource='<%# this.CvsTypeList %>'
								DataTextField="Text"
								DataValueField="Value"
								SelectedValue='<%# GetPaygentConvenienceType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>'
								CssClass="input_widthC input_border"
								runat="server" />
						</p>
					</div>
				</dd>
				<%-- コンビニ(前払い)ここまで --%>

				<%-- コンビニ(後払い) --%>
				<dd id="ddCvsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
					<uc:PaymentDescriptionCvsDef runat="server" ID="ucPaymentDescriptionCvsDef" />
				</dd>

				</dd>
				<%-- ヤマト後払いSMS認証連携 --%>
				<dd id="ddSmsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF) %>" runat="server">
					<uc:PaymentDescriptionSmsDef runat="server" id="ucPaymentDescriptionSmsDef" />
				</dd>

				<%-- 後付款(TriLink後払い) --%>
				<dd id="ddTriLinkAfterPayPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY) %>" runat="server">
					<uc:PaymentDescriptionTriLinkAfterPay runat="server" id="ucPaymentDescriptionTryLinkAfterPay" />
				</dd>

				<%-- 銀行振込（前払い） --%>
				<dd id="ddBankPre" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE) %>" runat="server">
				</dd>

				<%-- 銀行振込（後払い） --%>
				<dd id="ddBankDef" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF) %>" runat="server">
				</dd>

				<%-- 郵便振込（前払い） --%>
				<dd id="ddPostPre" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE) %>" runat="server">
				</dd>

				<%-- 郵便振込（後払い） --%>
				<dd id="ddPostDef" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF) %>" runat="server">
				</dd>

				<%-- ドコモケータイ払い --%>
				<dd id="ddDocomoPayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG) %>" runat="server">
					<div>
						<strong>【注意事項】</strong>
					</div>
					<div>
						決済には「i-mode対応」の携帯電話が必要です。<br />
						携帯電話のメールのドメイン指定受信を設定されている方は、必ず「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopMailDomain")) %>」を受信できるように設定してください。<br />
						1回の購入金額が<%: CurrencyManager.ToPrice(10000m) %>を超えてしまう場合はケータイ払いサービスをご利用いただけません。<br />
						i-mode」はＮＴＴドコモの商権、または登録商標です。<br />
					</div>
				</dd>

				<%-- S!まとめて支払い --%>
				<dd id="ddSMatometePayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG) %>" runat="server">
				</dd>

				<%-- まとめてau支払い --%>
				<dd id="ddAuMatometePayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG) %>" runat="server">
				</dd>

				<%-- ソフトバンク・ワイモバイルまとめて支払い(SBPS) --%>
				<dd id="ddSoftBankKeitaiSBPSPayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS) %>" runat="server">
				</dd>

				<%-- auかんたん決済(SBPS) --%>
				<dd id="ddAuKantanSBPSPayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS) %>" runat="server">
				</dd>

				<%-- ドコモケータイ払い(SBPS) --%>
				<dd id="ddDocomoKeitaiSBPSPayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS) %>" runat="server">
				</dd>

				<%-- S!まとめて支払い(SBPS) --%>
				<dd id="ddSMatometeSBPSPayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS) %>" runat="server">
				</dd>

				<%-- PayPal(SBPS) --%>
				<dd id="ddPaypalSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS) %>" runat="server">
					PayPal支払い
				</dd>

				<%--リクルートかんたん支払い(SBPS) --%>
				<dd id="ddRecruitSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS) %>" runat="server">
					リクルートかんたん支払い
				</dd>

				<%--楽天ペイ(SBPS) --%>
				<dd id="ddRakutenIdSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS) %>" runat="server">
				</dd>

				<%-- PayPal --%>
				<dd id="ddPayPal" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) %>" runat="server">
					<%if (SessionManager.PayPalCooperationInfo != null) {%>
						ご利用のPayPal アカウント：<br/>
						<b><%: SessionManager.PayPalCooperationInfo.AccountEMail %></b>
					<%} else {%>
						ご利用にはPayPalログインが必要です。
					<%} %>
					<uc:PaymentDescriptionPayPal runat="server" id="PaymentDescriptionPayPal" />
				</dd>

				<%-- Paidy --%>
				<dd id="ddPaidy" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY) %>" runat="server">
					<uc:PaidyCheckoutControl ID="ucPaidyCheckoutControl" runat="server" />
				</dd>

				<!-- NP後払い -->
				<dd id="ddNpAfterPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY) %>" runat="server">
					<uc:PaymentDescriptionNPAfterPay runat="server" id="PaymentDescriptionNPAfterPay" />
				</dd>

				<%-- LinePay --%>
				<dd id="ddLinePay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY) %>" runat="server">
					<uc:PaymentDescriptionLinePay runat="server" id="PaymentDescriptionLinePay" />
				</dd>
			
				<!-- PayPay -->
				<dd id="ddPayPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) %>" runat="server">
					<uc:PaymentDescriptionPayPay runat="server" id="PaymentDescriptionPayPay" />
				</dd>

				<%-- 決済なし --%>
				<dd id="ddNoPayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT) %>" runat="server">
				</dd>

				<%-- atone翌月払い --%>
				<dd id="ddAtonePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) %>" runat="server">
					<uc:PaymentDescriptionAtone runat="server" id="PaymentDescriptionAtone" />
				</dd>
				
				<%-- aftee翌月払い --%>
				<dd id="ddAfteePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) %>" runat="server">
				</dd>

				<%-- Ec Pay --%>
				<dd id="ddEcPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>" runat="server">
					<div class="inner" runat="server">
						<p class="title"><strong>支払い方法</strong></p>
						<p>
							<asp:DropDownList id="ddlEcPayment" runat="server"
								CssClass="input_border"
								DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE) %>"
								SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
									&& (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
									? this.CartList.Items[0].Payment.ExternalPaymentType
									: Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT %>"
								DataTextField="text"
								DataValueField="value"
								AutoPostBack="true"
								OnSelectedIndexChanged="ddlEcPayment_SelectedIndexChanged" />
								<br />
								<asp:CheckBox ID="cbEcPayCreditInstallment"
									runat="server"
									Checked="<%# ((this.CartList.Items[0].Payment != null) && this.CartList.Items[0].Payment.IsPaymentEcPayWithCreditInstallment) %>"
									Visible="<%# IsDisplayCreditInstallment(this.CartList.Items[0].Payment) && (string.IsNullOrEmpty(Constants.ECPAY_PAYMENT_CREDIT_INSTALLMENT) == false) %>"
									Text="分割払い" />
						</p>
					</div>
				</dd>

				<%-- （DSK）後払い --%>
				<dd id="ddDskDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF) %>" runat="server">
					コンビニ後払い（DSK）
				</dd>
				<%-- NewebPay Payment --%>
				<dd id="ddNewebPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>" runat="server">
					<strong>支払い方法<br /></strong>
					<asp:DropDownList id="ddlNewebPayment"
						runat="server"
						CssClass="input_border"
						DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb") %>'
						SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
							&& (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
								? this.CartList.Items[0].Payment.ExternalPaymentType
								: Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT %>"
						DataTextField="text"
						DataValueField="value"
						AutoPostBack="true"
						OnSelectedIndexChanged="ddlNewebPayment_SelectedIndexChanged" />
					<div id="dvCreditInstallment" visible="<%# IsDisplayCreditInstallment(this.CartList.Items[0].Payment) %>" runat="server">
					<strong>支払い回数<br /></strong>
					<asp:DropDownList id="ddlCreditInstallment"
						runat="server"
						DataSource="<%# this.NewebPayInstallmentsList %>"
						SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
							&& (string.IsNullOrEmpty(this.CartList.Items[0].Payment.NewebPayCreditInstallmentsCode) == false))
								? this.CartList.Items[0].Payment.NewebPayCreditInstallmentsCode
								: Constants.FLG_ORDER_CARD_INSTALLMENTS_CODE_ONCE %>"
						DataTextField="Text"
						DataValueField="Value"
						CssClass="input_border" />
					</div>
				</dd>
				<%-- ID決済(wechatpay、aripay、キャリア決済) --%>
				<dd id="ddCarrierbillingBokuPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU) %>" runat="server">
					ID決済(wechatpay、aripay、キャリア決済)
				</dd>
				
				<%-- GMOアトカラ --%>
				<dd id="ddGmoAtokara" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA) %>" runat="server">
					アトカラ
				</dd>
			</ItemTemplate>
			<FooterTemplate>
				</dl>
			</FooterTemplate>
			</asp:Repeater>
	<div id="sErrorMessage2" class="attention" EnableViewState="False" runat="server"></div>
	<%-- ▲お支払い情報▲ --%>

	<asp:LinkButton id="lbRecalculateCart" runat="server" CommandArgument="<%# Container.ItemIndex %>" onclick="lbRecalculate_Click"></asp:LinkButton>

	<%-- ▼領収書情報▼ --%>
	<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
	<dl class="order-form receipt-form" id="divReceipt">
		<dt>領収書情報</dt>
		<dd id="divDisplayCanNotInputMessage" runat="server" visible="false" class="attention">指定したお支払い方法は、領収書の発行ができません。</dd>
		<dd id="divReceiptInfoInputForm" runat="server">
			<strong>領収書希望有無を選択してください。</strong>
			<dd><asp:DropDownList id="ddlReceiptFlg" runat="server" DataTextField="text" DataValueField="value" DataSource="<%# this.DdlReceiptFlgListItems[Container.ItemIndex] %>"
				SelectedValue="<%# GetSelectedValueOfReceiptFlg(Container.ItemIndex, ((CartObject)Container.DataItem).IsUseSameReceiptInfoAsCart1, ((CartObject)Container.DataItem).ReceiptFlg) %>"
				OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" CssClass="input_border" />
			</dd>
			<div id="divReceiptAddressProviso" runat="server">
			<dd>宛名<span class="attention">※</span></dd>
			<dd>
				<asp:TextBox id="tbReceiptAddress" runat="server" Text="<%# ((CartObject)Container.DataItem).ReceiptAddress %>" MaxLength="100" Width="450" />
				<p><asp:CustomValidator ID="cvReceiptAddress" runat="Server"
					ControlToValidate="tbReceiptAddress"
					ValidationGroup="ReceiptRegisterModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"/></p>
			</dd>
			<dd>但し書き<span class="attention">※</span></dd>
			<dd>
				<asp:TextBox id="tbReceiptProviso" runat="server" Text="<%# ((CartObject)Container.DataItem).ReceiptProviso %>" MaxLength="100" Width="450" />
				<p><asp:CustomValidator ID="cvReceiptProviso" runat="Server"
					ControlToValidate="tbReceiptProviso"
					ValidationGroup="ReceiptRegisterModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"/></p>
			</dd>
			</div><!--divReceiptAddressProviso-->
		</dd><!--divReceiptInfoInputForm-->
	</dl><!--divReceipt-->
	<% } %>
	<%-- ▲領収書情報▲ --%>

</div>

</ItemTemplate>
</asp:Repeater>

</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
<div class="cart-footer">
	<div class="button-next">
		<a onclick="<%= this.NextOnClick %>" href="<%: this.NextEvent %>" class="btn">ご注文内容の確認へ進む</a>
		<a id="lbNextToConfirm" href="<%: this.NextEvent %>" style="display:none;"></a>
	</div>
</div>
</section>
	
<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>
<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var buyer = <%= PaidyUtility.CreatedBuyerDataObjectForPaidyPayment(this.CartList) %>;
	var hfPaidyTokenIdControlId = "<%= this.WhfPaidyTokenId.ClientID %>";
	var hfPaidyPaySelectedControlId = "<%= this.WhfPaidyPaySelected.ClientID %>";
	var lbNextProcess = "lbNextToConfirm";
	var isHistoryPage = false;
</script>
<uc:PaidyCheckoutScript ID="ucPaidyCheckoutScript" runat="server" />
<%--▲▲ Paidy用スクリプト ▲▲--%>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
<uc:Loading id="ucLoading" UpdatePanelReload="True" runat="server" />
</asp:Content>
