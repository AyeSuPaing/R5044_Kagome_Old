<%--
=========================================================================================================
  Module      : 注文確認画面(OrderConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/Form/Common/BodyRecommend.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderConfirm.aspx.cs" Inherits="Form_Order_OrderConfirm" Title="注文確認ページ" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="AtonePaymentScript" Src="~/Form/Common/AtonePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="AfteePaymentScript" Src="~/Form/Common/AfteePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScriptForPaygent.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paygent.Paidy.Checkout" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<link href="<%: this.ResolveClientUrl("~/Css/modal.css?" + Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED) %>" rel="stylesheet" type="text/css" media="all" />
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
<%
	// 注文完了ボタン押下した際のJavascript処理追加
	this.CompleteButtonList.ForEach(button =>
	{
		button.OnClientClick = (this.HideOrderButtonWithClick) ? "return exec_submit(true)" : "return exec_submit(false)";

		if (this.CartList.Items.Any(item => item.Payment.IsPaymentPaygentPaidy)
			&& (this.CartList.Items.Count == 1))
		{
			button.OnClientClick = "PaidyPayProcess(); return false;";
		}
	});
	if (this.IsDispCorrespondenceSpecifiedCommericalTransactions)
	{
		this.CompleteButtonList[0].OnClientClick = "return false;";
		lbComplete3.OnClientClick = (this.HideOrderButtonWithClick) ? "return exec_submit(true)" : "return exec_submit(false)";

		if (this.CartList.Items.Any(item => item.Payment.IsPaymentPaygentPaidy)
			&& (this.CartList.Items.Count == 1))
		{
			lbComplete3.OnClientClick = "PaidyPayProcess(); return false;";
		}
	}
%>
<%-- 注文ボタン押下した際の処理 --%>
<script type="text/javascript">
	<!--
	var submitted = false;
	var isLastItemCart = false;
	var isPageConfirm = false;
	var isMyPage = null;
	var completeButton = null;
	var paymentNeedSubmitted = false;

	function exec_submit(clearSubmitButton)
	{
		completeButton = document.getElementById('<%= lbCompleteAfterComfirmPayment.ClientID %>');

		if (submitted === false) {
			<% if(Constants.PRODUCT_ORDER_LIMIT_ENABLED){ %>
			var confirmMessage = '<%= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
			<% if (this.HasOrderHistorySimilarShipping) { %>
			if (confirm(confirmMessage) === false) return false;
			<% } %>
			<% } %>
		}

		<% if(Constants.PAYMENT_ATONEOPTION_ENABLED && this.IsUseAtonePaymentAndNotYetCardTranId) { %>
		GetAtoneAuthority();
		<% } %>
		<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED && this.IsUseAfteePaymentAndNotYetCardTranId) { %>
		GetAfteeAuthority();
		<% } %>
		if (submitted) return false;

		submitted = true;

		return true;
	}

	function ClickSelect(cbDefaultInvoice) {
		if (cbDefaultInvoice.checked) {
			cbDefaultInvoice.checked = false;
		}
		else {
			cbDefaultInvoice.checked = true;
		}
	}

	function TwoClickSelect(control, duplicateClick) {
		var cbDefaultInvoice = control.childNodes[0];
		if (cbDefaultInvoice.checked) {
			cbDefaultInvoice.checked = false;
		}
		else {
			cbDefaultInvoice.checked = true;
		}
		if (duplicateClick) {
			__doPostBack(cbDefaultInvoice.UniqueId, '');
		}
	}
//-->
</script>
<% if(Constants.PAYMENT_ATONEOPTION_ENABLED && this.IsUseAtonePaymentAndNotYetCardTranId) { %>
<asp:HiddenField runat="server" ID="hfAtoneToken" />
	<script type="text/javascript">
		$('#<%= this.WhfAtoneToken.ClientID %>').val('<%= this.IsLoggedIn
			? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
			: string.Empty %>');

		// Set token
		function SetAtoneTokenFromChildPage(token) {
			$('#<%= this.WhfAtoneToken.ClientID %>').val(token);
		}

		// Get Current Token
		function GetCurrentAtoneToken() {
			return $('#<%= this.WhfAtoneToken.ClientID %>').val();
		}

		// Get Index Cart
		function GetIndexCartHavingPaymentAtoneOrAftee(isAtone, callBack)
		{
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({ isAtone: isAtone }),
				async: false,
				success: callBack
			});
		}

		// Atone Authority
		function GetAtoneAuthority() {
			GetIndexCartHavingPaymentAtoneOrAftee(true, function (response) {
				var data = JSON.parse(response.d);

				if (data.indexs.length > 0) {

					for (var index = 0; index < data.indexs.length; index++) {
						AtoneAuthories(data.indexs[index]);
						isLastItemCart = (index == (data.indexs.length - 1));
						break;
					}
					submitted = true;
				}
				else {
					submitted = false;
				}
			});
		}

		// ExecuteOrder
		function ExecuteOrder() {
			var buttonComplete = document.getElementById('<%= lbComplete1.ClientID %>');
			buttonComplete.click();
		}

		// Atone Go To Error Page
		function AtoneGoToErrorPage() {
			window.location = '<%=Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR %>';
		}
	</script>
	<% ucAtonePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_ORDER_CONFIRM)); %>
	<uc:AtonePaymentScript ID="ucAtonePaymentScript" runat="server"/>
<% } %>

<asp:HiddenField ID="hfPaidyPaymentId" runat="server" />
<asp:HiddenField ID="hfPaidySelect" runat="server" />
<asp:HiddenField ID="hfPaidyStatus" runat="server" />

<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED && this.IsUseAfteePaymentAndNotYetCardTranId) { %>
<asp:HiddenField runat="server" ID="hfAfteeToken" />
	<script type="text/javascript">
		$('#<%= this.WhfAfteeToken.ClientID %>').val('<%= this.IsLoggedIn
			? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]
			: string.Empty %>');

		// Set token
		function SetAfteeTokenFromChildPage(token) {
			$('#<%= this.WhfAfteeToken.ClientID %>').val(token);
		}

		// Get Current Token
		function GetCurrentAfteeToken() {
			return $('#<%= this.WhfAfteeToken.ClientID %>').val();
		}

		// Get Index Cart
		function GetIndexCartHavingPaymentAtoneOrAftee(isAtone, callBack) {
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({ isAtone: isAtone }),
				async: false,
				success: callBack
			});
		}

		// Aftee Authority
		function GetAfteeAuthority() {
			GetIndexCartHavingPaymentAtoneOrAftee(false, function (response) {
				var data = JSON.parse(response.d);
				if (data.indexs.length > 0) {

					for (var index = 0; index < data.indexs.length; index++) {
						AfteeAuthories(data.indexs[index]);
						isLastItemCart = (index == (data.indexs.length - 1));
						break;
					}
					submitted = true;
				}
				else {
					submitted = false;
				}
			});
		}

		// ExecuteOrder
		function ExecuteOrder() {
			var buttonComplete = document.getElementById('<%= lbComplete1.ClientID %>');
			buttonComplete.click();
		}

	</script>
	<% ucAfteePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_ORDER_CONFIRM)); %>
	<uc:AfteePaymentScript ID="ucAfteePaymentScript" runat="server"/>
<% } %>
<% if(this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
	<section id="modalArea" class="modalArea">
		<div id="modalBg" class="modalBg"></div>
		<div class="modalWrapper">
			<div class="modalContents">
				<%= ShopMessage.GetMessageByPaymentId(this.BindingCartList[0].Payment.PaymentId) %>
				<br/><asp:LinkButton id="lbComplete3" runat="server" onclick="lbComplete_Click" class="btn btn-success">注文を確定する</asp:LinkButton>
			</div>
			<div id="closeModal" class="closeModal">
				×
			</div>
		</div>
	</section>
<% } %>
<p id="CartFlow"><img src="../../Contents/ImagesPkg/order/cart_step03.gif" alt="ご注文内容確認 " width="781" height="58" /></p>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<div class="btmbtn above cartstep">
	<h2 class="ttlA">ご注文内容確認</h2>
	<ul>
		<li>
			<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal == false) { %>
				<asp:LinkButton id="lbComplete1" runat="server" onclick="lbComplete_Click" class="btn btn-success">注文を確定する</asp:LinkButton>
			<% } %>
		</li>
	</ul>
</div>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblDeliveryPatternAlert" runat="server" visible="false">配送パターンを選択してください</asp:Label>
</div>
<div style="text-align: left">
	<asp:Label id="lblPaymentAlert" runat="server">
		注文同梱後の金額が各決済方法の上限額を超えました。<br />お手数ですが、カートに戻って別の注文と同梱、または同梱せずに注文実行してください。
	</asp:Label>
</div>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblNotFirstTimeFixedPurchaseAlert" runat="server" visible="false"></asp:Label>
</div>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblPaypayErrorMessage" runat="server" visible="false" />
</div>
<% if (this.IsChangedProductPriceByOrderCombine) { %>
<div style="color: red; font-weight: bold">
	注文同梱により商品価格が変更になりました。
</div>
<% } %>
<% if (this.IsChangedFixedPurchaseByOrderCombine) { %>
<div style="color: red; font-weight: bold">
	注文同梱により既存の定期購入情報が変更されます。
</div>
<% } %>
<div style="color: red; font-weight: bold;">
	<asp:Literal id="lOrderCombineMessage" visible="false" runat="server" />
</div>
<div style="color: red; font-weight: bold;">
	<asp:Literal id="lSubscriptionOrderCombineMessage" visible="true" runat="server" />
</div>

	<div id="CartList">
	
<%-- ▼PayPalログインここから▼ --%>
<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
<%if (SessionManager.IsPayPalOrderfailed) {%>
	<%
		ucPaypalScriptsForm.LogoDesign = "Payment";
		ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
	%>
	<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
	<div id="paypal-button" style="height: 25px"></div>
	<%if (SessionManager.PayPalCooperationInfo != null) {%>
		<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済<br/>
	<%} %>
	<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
	<% SessionManager.IsPayPalOrderfailed = false; %>
<%} %>
<%} %>
<%-- ▲PayPalログインここまで▲ --%>

<asp:Repeater id="rCartList" Runat="server" OnItemCommand="rCartList_ItemCommand">
<ItemTemplate>
	<div style="color: red; font-weight: bold" Visible="<%# IsGlobalShippingPriceCalcError((CartObject)Container.DataItem) %>" runat="server">
		カート内の商品に配送料金が設定されていません。
		カートから削除してください。
	</div>
	<div class="main">
	<div class="submain">
	
	<%-- ▼注文内容▼ --%>
	<div class="column">
	<div id="Div1" visible="<%# Container.ItemIndex == 0 %>" runat="server">
	<h2><img src="../../Contents/ImagesPkg/order/sttl_cash_confirm.gif" alt="注文情報" width="64" height="16" /></h2>
	<!--<p class="pdg_bottomA">以下の内容をご確認のうえ、「注文する」ボタンを<br />クリックしてください。</p>-->
	</div>
	
	<div class="orderBox">
	<h3>
		カート番号<%# Container.ItemIndex + 1 %>
		<%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）"))%>
	</h3>
	<div class="bottom">
	<div id="Div2" class="box" visible="<%# Container.ItemIndex == 0 %>" runat="server">
	<em>本人情報確認</em>
	<div>
	<dl>
	<%-- 氏名 --%>
	<dt>
		<%: ReplaceTag("@@User.name.name@@") %>：
	</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Name1) %><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Name2) %>&nbsp;様</dd>
	<% if ((this.IsAmazonCv2Guest == false) || (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED) || IsTargetToExtendedAmazonAddressManagerOption()) { %>
	<%-- 氏名（かな） --%>
	<div <%# ((string.IsNullOrEmpty(((CartObject)Container.DataItem).Owner.NameKana) == false)) ? "" : "style=\"display:none;\"" %>>
		<dt <%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? "" : "style=\"display:none;\"" %>>
			<%: ReplaceTag("@@User.name_kana.name@@") %>：
		</dt>
		<dd <%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? "" : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.NameKana1) %><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.NameKana2) %>&nbsp;さま</dd>
	</div>
	<dt>
		<%: ReplaceTag("@@User.mail_addr.name@@") %>：
	</dt>
	<% } %>
	<dd><%# ((((CartObject)Container.DataItem).Owner.MailAddr) != "") ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.MailAddr) : "-&nbsp;" %></dd>
	<% if ((this.IsAmazonCv2Guest == false) || (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED) || IsTargetToExtendedAmazonAddressManagerOption()) { %>
	<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
	<dt>
		<%: ReplaceTag("@@User.mail_addr2.name@@") %>：
	</dt>
	<dd><%# ((((CartObject)Container.DataItem).Owner.MailAddr2) != "") ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.MailAddr2):"-&nbsp;" %></dd><br />
	<% } %>
	<dt>
		<%: ReplaceTag("@@User.addr.name@@") %>：
	</dt>
	<dd>
		<p>
			<%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? WebSanitizer.HtmlEncode("〒" + ((CartObject)Container.DataItem).Owner.Zip) + "<br />" : "" %>
			<%#: ((CartObject)Container.DataItem).Owner.Addr1 %> <%#: ((CartObject)Container.DataItem).Owner.Addr2 %><br />
			<%#: ((CartObject)Container.DataItem).Owner.Addr3 %> <%#: ((CartObject)Container.DataItem).Owner.Addr4 %><br />
			<%#: ((CartObject)Container.DataItem).Owner.Addr5 %> <%# (((CartObject)Container.DataItem).Owner.IsAddrJp == false) ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Zip) + "<br />" : "" %>
			<%#: ((CartObject)Container.DataItem).Owner.AddrCountryName %><br />
		</p>
	</dd>
	<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
	<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
		<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.CompanyName) %><br />
		<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.CompanyPostName) %></dd>
	<%} %>
	<%-- 電話番号 --%>
	<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Tel1) %></dd>
	<dt><%: ReplaceTag("@@User.tel2.name@@") %>：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Tel2) %>&nbsp;</dd>
	<% } %>
	<dt><%: ReplaceTag("@@User.mail_flg.name@@") %></dt>
	<dd><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, ((CartObject)Container.DataItem).Owner.MailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG))%><br />&nbsp;</dd>
	</dl>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	
	<div id="hgcChangeUserInfoBtn" runat="server">
	<p class="btn_change"><asp:LinkButton ID="LinkButton1" CommandName="GotoShipping" runat="server" class="btn btn-mini"><span>変更する</span></asp:LinkButton></p>
	</div>
	</div><!--box-->


	<asp:Repeater id="rCartShippings" DataSource='<%# Eval("Shippings") %>' OnItemCommand="rCartShippings_ItemCommand" runat="server">
	<ItemTemplate>
	<div class="box">
	<em>配送情報
	<span id="Span1" visible="<%# FindCart(Container.DataItem).IsGift %>" runat="server"><%# Container.ItemIndex + 1 %></span>
	</em>

	<div>
	<dl>
	<div runat="server" visible="<%# (((CartShipping)Container.DataItem).ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
		<span style="color:red;display:block;"><asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal></span></br>
		<dt>店舗ID：</dt>
		<dd><%#: ((CartShipping)Container.DataItem).ConvenienceStoreId %>&nbsp;</dd>
		<dt>店舗名称：</dt>
		<dd><%#: ((CartShipping)Container.DataItem).Name1 %>&nbsp;</dd>
		<dt>店舗住所：</dt>
		<dd><%#: ((CartShipping)Container.DataItem).Addr4 %>&nbsp;</dd>
		<dt>店舗電話番号：</dt>
		<dd><%#: ((CartShipping)Container.DataItem).Tel1 %>&nbsp;</dd>
	</div>
	<span visible="<%# IsStorePickupDisplayed(Container.DataItem) %>" runat="server">
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>：
		</dt>
		<dd>
			<p>
				<%# ((bool)Eval("IsShippingAddrJp")) ? WebSanitizer.HtmlEncode("〒" + Eval("Zip")) + "<br />" : ""  %>
				<%#: Eval("Addr1") %> <%#: Eval("Addr2") %><br />
				<%#: Eval("Addr3") %> <%#: Eval("Addr4") %> <%#: Eval("Addr5") %><br />
				<%# ((bool)Eval("IsShippingAddrJp") == false) ? WebSanitizer.HtmlEncode(Eval("Zip")) + "<br />" : ""  %>
				<%#: Eval("ShippingCountryName") %>
			</p>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
			<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(Eval("CompanyName"))%>&nbsp<%# WebSanitizer.HtmlEncode(Eval("CompanyPostName"))%></dd>
		<%} %>
		<%-- 氏名 --%>
		<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
		<dd><%# WebSanitizer.HtmlEncode(Eval("Name1")) %><%# WebSanitizer.HtmlEncode(Eval("Name2")) %>&nbsp;様</dd>
		<%-- 氏名（かな） --%>
		<div <%# (string.IsNullOrEmpty((string)Eval("NameKana")) == false) ? "" : "style=\"display:none;\"" %>>
			<dt <%# ((bool)Eval("IsShippingAddrJp")) ? "" : "style=\"display:none;\"" %>><%: ReplaceTag("@@User.name_kana.name@@") %>：</dt>
			<dd <%# ((bool)Eval("IsShippingAddrJp")) ? "" : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(Eval("NameKana1")) %><%# WebSanitizer.HtmlEncode(Eval("NameKana2")) %>&nbsp;さま</dd>
		</div>
		<%-- 電話番号 --%>
		<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
		<dd><%# WebSanitizer.HtmlEncode(Eval("Tel1")) %></dd>
		<span id="Span2" visible="<%# FindCart(Container.DataItem).IsGift %>" class="sender" runat="server">
		<dt>送り主：</dt>
		<dd>
			<p>
				<%# ((bool)Eval("IsSenderAddrJp")) ? "〒" + Eval("SenderZip") + "<br />" : ""  %>
				<%#: Eval("SenderAddr1")%><%#: Eval("SenderAddr2")%><br />
				<%#: Eval("SenderAddr3")%><%#: Eval("SenderAddr4")%><%#: Eval("SenderAddr5")%><br />
				<%# ((bool)Eval("IsSenderAddrJp") == false) ? WebSanitizer.HtmlEncode(Eval("SenderZip")) + "<br />"  : ""  %>
				<%#: Eval("SenderCountryName")%>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<br />
				<%# WebSanitizer.HtmlEncode(Eval("SenderCompanyName"))%>&nbsp<%# WebSanitizer.HtmlEncode(Eval("SenderCompanyPostName"))%>
				<%} %>
			</p>
		</dd>
		<dd><%# WebSanitizer.HtmlEncode(Eval("SenderName1"))%><%# WebSanitizer.HtmlEncode(Eval("SenderName2"))%>&nbsp;様</dd>
		<dd <%# ((bool)Eval("IsSenderAddrJp")) ? "" : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(Eval("SenderNameKana1"))%><%# WebSanitizer.HtmlEncode(Eval("SenderNameKana2"))%>&nbsp;さま</dd>
		<dd <%# ((bool)Eval("IsSenderAddrJp")) ? "" : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(Eval("SenderTel1"))%></dd>
		</span>
	</span>
	<span visible="<%# IsStorePickupDisplayed(Container.DataItem, true) %>" runat="server">
		<dt>
			受取店舗：
		</dt>
		<dd>
			<p><%#: Eval("RealShopName")%></p>
		</dd>
		<dt>
			受取店舗住所：
		</dt>
		<dd>
			<p>
				<%#: "〒" + Eval("Zip") %>
				<br />
				<%#: Eval("Addr1") %> <%#: Eval("Addr2") %>
				<br />
				<%#: Eval("Addr3") %>
				<br />
				<%#: Eval("Addr4") %>
				<br />
				<%#: Eval("Addr5") %>
			</p>
		</dd>
		<dt>
			営業時間：
		</dt>
		<dd>
			<p><%#: Eval("RealShopOpenningHours")%></p>
		</dd>
		<dt>
			店舗電話番号：
		</dt>
		<dd>
			<p><%#: Eval("Tel1")%></p>
		</dd>
	</span>
	<span visible="<%# (FindCart(Container.DataItem).IsDigitalContentsOnly == false) %>" runat="server">
		<% if (this.IsLoggedIn && Constants.TWOCLICK_OPTION_ENABLE && ((Constants.GIFTORDER_OPTION_ENABLED == false || Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED) && (this.IsAmazonLoggedIn == false))) {%>
		<br />
		<div><asp:CheckBox id="cbDefaultShipping" GroupName='<%# "DefaultShippingSetting_" + Container.ItemIndex %>' Text=" 通常の配送先に設定する" CssClass="radioBtn" runat="server"  OnCheckedChanged="cbDefaultShipping_OnCheckedChanged" AutoPostBack="true"/></div>
		<%} %>
		<div id="hgcChangeShippingInfoBtn" runat="server">
		<p class="btn_change"><asp:LinkButton ID="lbGotoShipping" CommandName="GotoShipping" CommandArgument="Shipping" runat="server" class="btn btn-mini"><span>変更する</span></asp:LinkButton><br /></p>
		</div>
	</span>
	<span id="hcProducts" visible="<%# FindCart(Container.DataItem).IsGift %>" runat="server">
		<dt>商品：</dt>
		<dd>
		<asp:Repeater ID="rProductCount" DataSource="<%# ((CartShipping)Container.DataItem).ProductCounts %>" runat="server">
		<ItemTemplate>
			<dd><strong>
				<%# WebSanitizer.HtmlEncode(((CartProduct)Eval("Product")).ProductJointName) %></strong>
				<%# (((CartProduct)Eval("Product")).GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartProduct)Eval("Product")).GetProductTag("tag_cart_product_message")) + "</small>" : ""%>
			<p id="P1" visible='<%# ((CartProduct)Eval("Product")).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
			<asp:Repeater ID="rProductOptionSettings" DataSource='<%#((CartProduct)Eval("Product")).ProductOptionSettingList %>' runat="server">
				<ItemTemplate>
				<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<strong>" : "" %>
				<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
				<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "</strong>" : "" %>
				</ItemTemplate>
			</asp:Repeater>
			</p>
			<p>&nbsp;&nbsp;&nbsp;&nbsp; <%#: CurrencyManager.ToPrice(((CartProduct)Eval("Product")).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;<%# WebSanitizer.HtmlEncode(Eval("Count")) %></p></dd>
		</ItemTemplate>
		</asp:Repeater>
		</dd>
	</span>
	<span visible="<%# IsStorePickupDisplayed(Container.DataItem) %>" runat="server">
		<dt>配送方法：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, ((CartShipping)Container.DataItem).ShippingMethod)) %>
		</dd>
		<dt visible ="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">配送サービス：</dt>
		<dd visible ="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">
			<%#: GetDeliveryCompanyName(((CartShipping)Container.DataItem).DeliveryCompanyId) %>
		</dd>
		<dt id="Dt1" visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server">
			配送希望日：</dt>
		<dd id="Dd1" visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingDate((CartShipping)Container.DataItem)) %></dd>
		<dt id="Dt2" visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server">
			配送希望時間帯：</dt>
		<dd id="Dd2" visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingTime((CartShipping)Container.DataItem)) %></dd>
	</span>
	<span visible="<%# IsStorePickupDisplayed(Container.DataItem, true) %>" runat="server">
		<dt>配送方法：</dt>
		<dd>店舗受取</dd>
	</span>
	<span>
		<dt id="Dt3" visible='<%# ((CartShipping)Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim() != ""  %>' runat="server">
			注文メモ：</dt>
		<dd id="Dd3" visible='<%# ((CartShipping)Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim() != ""  %>' runat="server">
			<%# WebSanitizer.HtmlEncodeChangeToBr(((CartShipping)Container.DataItem).CartObject.GetOrderMemosForOrderConfirm()) %>
		</dd>
		<span runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(((CartShipping)Container.DataItem).ShippingCountryIsoCode) %>">
			<span>
				<dt>
					発票種類：
					<div runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)%>">
						<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@") %>：<br />
						<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@") %>：
					</div>
					<div runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_DONATE)%>">
						<%# ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@") %>：
					</div>
				</dt>
				<dd>
					<%# GetInformationInvoice((CartShipping)Container.DataItem) %>
				</dd>
				<span runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) %>">
					<dt>
						共通性載具：
						<div visible="<%# string.IsNullOrEmpty(((CartShipping)Container.DataItem).CarryType) == false %>" runat="server">
							載具コード：
						</div>
					</dt>
					<dd><%# GetInformationInvoice((CartShipping)Container.DataItem, true) %></dd>
				</span>
			</span>
			<asp:CheckBox id="cbDefaultInvoice"
				GroupName='<%# "DefaultInvoiceSetting_" + Container.ItemIndex %>'
				class="radioBtn DefaultInvoice"
				Text ="通常の電子発票情報に設定する"
				OnCheckedChanged="cbDefaultInvoice_CheckedChanged"
				runat="server"
				AutoPostBack="true"
				Visible="<%# this.IsLoggedIn %>" />
			<br />
		</span>
		<br />
		<p><%#: ((CartShipping)Container.DataItem).CartObject.ReflectMemoToFixedPurchase ? "※2回目以降の注文メモにも追加する" : "" %></p>

		<span id="Span3" visible='<%# (bool)Eval("WrappingPaperValidFlg") && ((CartObject)Eval("CartObject")).IsGift %>' runat="server">
		<dt>のし種類：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(string.IsNullOrEmpty(((CartShipping)Container.DataItem).WrappingPaperType) ? "なし" : ((CartShipping)Container.DataItem).WrappingPaperType) %>
		</dd>
		<dt>のし差出人：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(string.IsNullOrEmpty(((CartShipping)Container.DataItem).WrappingPaperName) ? "なし" : ((CartShipping)Container.DataItem).WrappingPaperName) %>
		</dd>
		</span>
		<span id="Span4" visible='<%# (bool)Eval("WrappingBagValidFlg") && ((CartObject)Eval("CartObject")).IsGift %>' runat="server">
		<dt>包装種類：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(string.IsNullOrEmpty(((CartShipping)Container.DataItem).WrappingBagType) ? "なし" : ((CartShipping)Container.DataItem).WrappingBagType )%>
		</dd>
		</span>
		<asp:Repeater ID="rOrderExtendDisplay"
					ItemType="OrderExtendItemInput"
					DataSource="<%# GetDisplayOrderExtendItemInputs(GetCartIndexFromControl(Container)) %>"
					runat="server"
					Visible="<%# IsDisplayOrderExtend() %>" >
			<HeaderTemplate>
				<br />
				<span>
					<em>注文確認事項</em>
			</HeaderTemplate>
			<ItemTemplate>
				<div style="display: inline-block;">
					<dt><%#: Item.SettingModel.SettingName %>：</dt>
					<dd>
						<%#: (string.IsNullOrEmpty(Item.InputValue)) ? "指定なし" : Item.InputText %>
					</dd>
				</div>
			</ItemTemplate>
			<FooterTemplate>
				</span>
			</FooterTemplate>
		</asp:Repeater>
	</span>
	</dl>
	</div>
	<div visible="<%# (((FindCart(Container.DataItem).HasFixedPurchase) || (FindCart(Container.DataItem).IsBeforeCombineCartHasFixedPurchase))
		&& (this.IsShowDeliveryPatternInputArea(FindCart(Container.DataItem)) == false)) %>" runat="server">
	<em id="Em1" visible="<%# FindCart(Container.DataItem).HasFixedPurchase %>" runat="server">定期配送情報</em>
	<div id="Div1" visible="<%# FindCart(Container.DataItem).HasFixedPurchase %>" runat="server">
		<dl>
		<dt>配送パターン：</dt>
		<dd><%# WebSanitizer.HtmlEncode(((CartShipping)Container.DataItem).GetFixedPurchaseShippingPatternString()) %></dd>
		<dt>初回配送予定：</dt>
		<dd><%#: GetFirstShippingDate((CartShipping)Container.DataItem) %></dd>
		<dt>今後の配送予定：</dt>
		<dd><%#: DateTimeUtility.ToStringFromRegion(((CartShipping)Container.DataItem).NextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dd>
		<dt></dt>
		<dd><%#: DateTimeUtility.ToStringFromRegion(((CartShipping)Container.DataItem).NextNextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)%></dd>
		<p>※定期解約されるまで継続して指定した配送パターンでお届けします。</p><br />
		<dt id="Dt4" visible='<%# ((CartShipping)Container.DataItem).SpecifyShippingTimeFlg %>' runat="server">配送希望時間帯：</dt>
		<dd id="Dd4" visible='<%# ((CartShipping)Container.DataItem).SpecifyShippingTimeFlg %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingTime((CartShipping)Container.DataItem)) %></dd>
		</dl>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	</div>
	<% if(this.IsOrderCombined == false){ %>
		<div id="hgcChangeFixedPurchaseShippingInfoBtn" visible="<%# (((CartShipping)Container.DataItem).ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF) %>" runat="server">
			<p class="btn_change"><asp:LinkButton ID="lbGotoShipping2" CommandName="GotoShipping" CommandArgument="Shipping2" runat="server" class="btn btn-mini"><span>変更する</span></asp:LinkButton></p>
		</div>
	<% } %>
	</div><!--box-->	
	</ItemTemplate>
	</asp:Repeater>

	<div class="box" visible="<%# (this.IsShowDeliveryPatternInputArea((CartObject)Container.DataItem)) %>" runat="server">
		<div class="fixed">
		<%-- ▽デフォルトチェックの設定▽--%>
		<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
		<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 3, 2, 1) : SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 2, 3, 1) %>
		<%-- 定期購入 + 通常注文の注文同梱向け、定期購入配送パターン入力欄 --%>
		<em>定期購入 配送パターンの指定</em>
		<div>
			<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
					Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 1) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
				<div id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">　
					<asp:DropDownList ID="ddlFixedPurchaseMonth"
						DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
						runat="server">
					</asp:DropDownList>
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
						DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
					</asp:DropDownList>
					日に届ける
					<small><asp:CustomValidator runat="Server" 
						ControlToValidate="ddlFixedPurchaseMonth" 
						ValidationGroup="OrderShipping" 
						ValidateEmptyText="true" 
						SetFocusOnError="true" 
						CssClass="error_inline" />
					</small>
					<small><asp:CustomValidator runat="Server" 
						ControlToValidate="ddlFixedPurchaseMonthlyDate" 
						ValidationGroup="OrderShipping" 
						ValidateEmptyText="true" 
						SetFocusOnError="true" 
						CssClass="error_inline" />
					</small>
				</div>
			</div>
			<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
					Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 2) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
				<div id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">　
					<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
						DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server" />
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
						DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
					</asp:DropDownList>
					<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
						DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
					</asp:DropDownList>
					に届ける
					<small><asp:CustomValidator runat="Server"
						ControlToValidate="ddlFixedPurchaseIntervalMonths"
						ValidationGroup="OrderShipping"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						CssClass="error_inline" />
					</small>
					<small><asp:CustomValidator runat="Server" 
						ControlToValidate="ddlFixedPurchaseWeekOfMonth" 
						ValidationGroup="OrderShipping" 
						ValidateEmptyText="true" 
						SetFocusOnError="true" 
						CssClass="error_inline" />
					</small>
					<small><asp:CustomValidator runat="Server" 
						ControlToValidate="ddlFixedPurchaseDayOfWeek" 
						ValidationGroup="OrderShipping" 
						ValidateEmptyText="true" 
						SetFocusOnError="true" 
						CssClass="error_inline" />
					</small>
				</div>
			</div>
			<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
					Text="配送日間隔指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 3) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
				<div id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">　
					<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
						DataSource='<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false) %>'
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
					</asp:DropDownList>
					日ごとに届ける
					<small><asp:CustomValidator runat="Server" 
						ControlToValidate="ddlFixedPurchaseIntervalDays" 
						ValidationGroup="OrderShipping" 
						ValidateEmptyText="true" 
						SetFocusOnError="true" 
						CssClass="error_inline" />
					</small>
				</div>
			</div>
			<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
					Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 4) %>"
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
				<div id="ddFixedPurchaseEveryNWeek" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">
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
				<small><asp:CustomValidator
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
				</small>
			</div>
			<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
			<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
			<hr>
			<div id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</div>
			<div visible="true" runat="server" style="padding-left: 20px;">
				<asp:Label ID="lblFirstShippingDate" runat="server"></asp:Label>
				<asp:DropDownList
					ID="ddlFirstShippingDate"
					visible="false"
					OnDataBound="ddlFirstShippingDate_OnDataBound"
					AutoPostBack="True"
					OnSelectedIndexChanged="ddlFirstShippingDate_ItemSelected"
					runat="server" />
			</div>
			<div>
				<asp:Label ID="lblFirstShippingDateNoteMessage" visible="false" Text="配送予定日は変更となる可能性がありますことをご了承ください。" runat="server" />
			</div>
			<br>
			<div id="dtNextShippingDate" visible="true" runat="server">2回目の配送日を選択</div>
			<div visible="true" runat="server" style="padding-left: 20px;">
				<asp:Label ID="lblNextShippingDate" visible="false" runat="server"></asp:Label>
				<asp:DropDownList
					ID="ddlNextShippingDate"
					visible="false"
					OnDataBound="ddlNextShippingDate_OnDataBound"
					OnSelectedIndexChanged="ddlNextShippingDate_OnSelectedIndexChanged"
					AutoPostBack="True"
					runat="server" />
			</div>
			<div>
				メール便の場合は数日ずれる可能性があります。
			</div>
		</div>
		</div>
	</div>

	<div class="last">
	<div class="box">
	<em>決済情報</em>
	<div>
	<dl>
	<dt>お支払い：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.PaymentName) %></dd>
	<dt visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">支払い方法：</dt>
	<dd visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">
		<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
	</dd>
	<dt visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">支払い方法：</dt>
	<dd visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">
		<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb", ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
	</dd>
	<dt id="Dt4" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server">支払先コンビニ名</dt>
	<dd id="Dd4" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server"><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetPaymentCvsName()) %></dd>
	<dt id="dtCvsDef" visible="<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF %>" runat="server">
		<uc:PaymentDescriptionCvsDef runat="server" ID="ucPaymentDescriptionCvsDef"  />
	</dt>
	<dt id="Dt5" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardCompany) != "" %>' runat="server">カード会社：</dt>
	<dd id="Dd5" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardCompany) != "" %>' runat="server"><%#: ((CartObject)Container.DataItem).Payment.CreditCardCompanyName %></dd>
	<dt id="Dt6" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">カード番号：</dt>
	<dd id="Dd6" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">XXXXXXXXXXXX<%# WebSanitizer.HtmlEncode(GetCreditCardDispString(((CartObject)Container.DataItem).Payment)) %></dd>
	<dt id="Dt7" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">有効期限：</dt>
	<dd id="Dd7" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server"><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireMonth) %>/<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireYear) %> (月/年)</dd>
	<dt id="Dt8" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">支払い回数：</dt>
	<dd id="Dd8" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, ((CartObject)Container.DataItem).Payment.CreditInstallmentsCode))%></dd>
	<dt id="Dt9" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">カード名義：</dt>
	<dd id="Dd9" visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server"><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditAuthorName) %></dd>
	<dt id="Dt10" visible='<%# ((CartObject)Container.DataItem).Payment.UserCreditCardRegistable %>' runat="server">登録：</dt>
	<dd id="Dd10" visible='<%# ((CartObject)Container.DataItem).Payment.UserCreditCardRegistable %>' runat="server"><%# WebSanitizer.HtmlEncode((((CartObject)Container.DataItem).Payment.UserCreditCardRegistFlg) ? "する" : (((CartObject)Container.DataItem).Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW) ? "済" :"しない") %>
		<%# WebSanitizer.HtmlEncode((((CartObject)Container.DataItem).Payment.UserCreditCardRegistFlg) ? ("（" + ((CartObject)Container.DataItem).Payment.UserCreditCardName + "）") : "") %>
	</dd>
	</dl>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<% if (this.IsLoggedIn && Constants.TWOCLICK_OPTION_ENABLE && CheckPaymentCanSaveDefaultValue(this.CartList.Items[0].Payment.PaymentId)) { %>
	<asp:CheckBox id="cbDefaultPayment" GroupName="DefaultPaymentSetting" Text=" 通常の支払方法に設定する" CssClass="radioBtn" runat="server" OnCheckedChanged="cbDefaultPayment_OnCheckedChanged" AutoPostBack="true"/>
	<% } %>
	<%-- ▼領収書情報▼ --%>
	<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" /></p>
	<div>
		<em>領収書情報</em>
		<div>
			<dl>
				<dt>領収書希望：</dt>
				<dd><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG, ((CartObject)Container.DataItem).ReceiptFlg) %></dd>
				<div runat="server" Visible="<%# ((CartObject)Container.DataItem).ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
					<dt>宛名：</dt>
					<dd><%#: ((CartObject)Container.DataItem).ReceiptAddress %></dd>
					<dt>但し書き：</dt>
					<dd><%#: ((CartObject)Container.DataItem).ReceiptProviso %></dd>
				</div>
			</dl>
		</div>
	</div>
	<% } %>
	<%-- ▲領収書情報▲ --%>
	<div id="hgcChangePaymentInfoBtn" runat="server">
	<p class="btn_change"><asp:LinkButton ID="LinkButton2" CommandName="GotoPayment" runat="server" class="btn btn-mini"><span>変更する</span></asp:LinkButton></p>
	</div>
	</div><!--box-->
	</div><!--last-->
	</div><!--bottom-->
	</div><!--orderBox-->
	</div><!--column-->
	<%-- ▲注文内容▲ --%>

	<%-- ▼カート情報▼ --%>
	<div class="shoppingCart">
	<div id="Div3" visible="<%# Container.ItemIndex == 0 %>" runat="server">
	<h2><img src="../../Contents/ImagesPkg/common/ttl_shopping_cart.gif" alt="ショッピングカート" width="141" height="16" /></h2>
	<div class="sumBox mrg_topA">
	<div class="subSumBoxB">
	<p><img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" />
		<strong><%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %></strong></p>
	</div>
	</div><!--sum-->
	</div>
	
	<div class="subCartList">
	<div class="bottom">
	<h3>
		カート番号<%# Container.ItemIndex + 1 %>
		<%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）"))%>
	</h3>
	<div class="block">
	<asp:Repeater ID="rCart" DataSource="<%# ((CartObject)Container.DataItem).Items %>" OnItemDataBound="rCart_OnItemDataBound" runat="server">
	<ItemTemplate>
		<%-- 通常商品 --%>
		<div class="singleProduct" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
		<div>
		<dl>
		<dt>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
			<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
		</dt>
		<dd>
			<strong>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
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
		<p style="padding-top: 2px;" Visible="<%# DisplaySubscriptionBoxFixedAmountCourse(((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) %>" runat="server">
			頒布会コース名：&nbsp;<%#: ((CartProduct)Container.DataItem).GetSubscriptionDisplayName() %>
		</p>
		<p style="padding-top: 2px;" Visible="<%# DisplaySubscriptionBoxFixedAmountCourse(((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) %>" runat="server">
			定額：&nbsp;<%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).SubscriptionBoxFixedAmount) %>(<%#: this.ProductPriceTextPrefix %>)
		</p>
		<p runat="server" ID="pPrice" style="padding-top: 2px; text-decoration: line-through" Visible="False">
			<span runat="server" ID="sPrice">
				<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %>
			</span>
		</p>
		<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px">
			<span runat="server" ID="sSubscriptionBoxCampaignPrice"><%#:ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %></span>(<%#: this.ProductPriceTextPrefix %>)
		</p>
		<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="specifiedcommercialtransactionsOrderConfirmSellTimeName" runat="server">販売期間：<br/></div>
		<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="specifiedcommercialtransactionsOrderConfirmSellTime" runat="server">
			<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>
			<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
		</div>
		<%--頒布会の選択可能期間--%>
		<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsOrderConfirmSellTimeName" runat="server" >販売期間</div>
		<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsOrderConfirmSellTime" runat="server" >
			<%# WebSanitizer.HtmlEncodeChangeToBr((CartProduct.GetSubscriptionBoxSelectTermBr(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)))%>
		</div>
		<p runat="server" ID="pSubscriptionBoxCampaignPeriod" Visible="False" style="color: red; padding-top: 2px">キャンペーン期間：<br>
			<span ID="sSubscriptionBoxCampaignPeriodSince" class="specifiedcommercialtransactionsCampaignTime" runat="server"></span>～<br>
			<span ID="sSubscriptionBoxCampaignPeriodUntil" class="specifiedcommercialtransactionsCampaignTime" runat="server"></span>
		</p>
		<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" class="specifiedcommercialtransactionsOrderConfirmSellTimeName" runat="server">タイムセール期間:<br/></div>
		<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" class="specifiedcommercialtransactionsOrderConfirmSellTime" runat="server">
			<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>
		</div>
		<p><%# WebSanitizer.HtmlEncodeChangeToBr(((CartProduct)Container.DataItem).ReturnExchangeMessage) %></p>
		<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
			※配送料無料適用外商品です
		</p>
		</dd>
		</dl>
		</div>
		</div><!--singleProduct-->
		<%-- セット商品 --%>
		<div class="multiProduct" visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
		<asp:Repeater id="rProductSet" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" runat="server">
		<ItemTemplate>
			<div>
			<dl>
			<dt>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
			</dt>
			<dd>
				<strong>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
					<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
				</strong>
				<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</small>" : "" %>
			<p Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) %></p></dd>
			<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
				※配送料無料適用外商品です
			</p>
			</dl>
			</div>
			<table id="Table1" visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == ((CartProduct)Container.DataItem).ProductSet.Items.Count) %>" width="297" cellpadding="0" cellspacing="0" class="clr" runat="server">
			<tr>
			<th width="38">セット：</th>
			<th width="50"><%# GetProductSetCount((CartProduct)Container.DataItem) %></th>
			<th width="146" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal((CartProduct)Container.DataItem)) %> (<%#: this.ProductPriceTextPrefix %>)</th>
			<td width="61"></td>
			</tr>
			</table>
		</ItemTemplate>
		</asp:Repeater>
		</div><!--multiProduct-->
	</ItemTemplate>
	</asp:Repeater>
	<%-- セットプロモーション商品 --%>
	<asp:Repeater ID="rCartSetPromotion" DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
	<ItemTemplate>
		<div class="multiProduct">
			<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" runat="server">
			<ItemTemplate>
				<div>
					<dl>
						<dt>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
							<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
						</dt>
						<dd>
							<strong>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
								<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
							</strong>
							<p visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
							<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
								<ItemTemplate>
								<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<strong>" : "" %>
								<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
								<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "</strong>" : "" %>
								</ItemTemplate>
							</asp:Repeater>
							</p>
							<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]) %></p>
							<p runat="server" ID="pPrice" style="padding-top: 2px; text-decoration: line-through" Visible="False">
								<span runat="server" ID="sPrice">
									<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %>
								</span>
							</p>
							<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px">
								<span runat="server" ID="sSubscriptionBoxCampaignPrice">
									<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %>
								</span>
							</p>
							<p><%# WebSanitizer.HtmlEncodeChangeToBr(((CartProduct)Container.DataItem).ReturnExchangeMessage) %></p>
							<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								※配送料無料適用外商品です
							</p>
						</dd>
					</dl>
				</div>
			</ItemTemplate>
			</asp:Repeater>
			<dl class="setpromotion" Visible="<%# ((CartSetPromotion)Container.DataItem).IsAllItemsSubscriptionBoxFixedAmount == false %>" runat="server">
				<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
				<dd>
					<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
						<strike><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (税込)</strike><br />
					</span>
					<span class="specifiedcommercialtransactionsCampaignTimeSpan">
						<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (税込)
					</span>
				</dd>
				<br />
				<div class="SpecifiedcommercialtransactionsCampaignTimePadding100" Visible="<%# (((CartSetPromotion)Container.DataItem).Items[0].IsDisplaySell) %>" runat="server">販売期間：</div>
				<div class="SpecifiedcommercialtransactionsCampaignTimePadding110" Visible="<%# (((CartSetPromotion)Container.DataItem).Items[0].IsDisplaySell) %>"  runat="server">
					<%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Items[0].SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>
					<%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Items[0].SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
				</div>
				<div class="SpecifiedcommercialtransactionsCampaignTimePadding100" Visible="<%# ((CartSetPromotion)Container.DataItem).Items[0].IsDispSaleTerm %>" runat="server">タイムセール期間:</div>
				<div class="SpecifiedcommercialtransactionsCampaignTimePadding110" Visible="<%# ((CartSetPromotion)Container.DataItem).Items[0].IsDispSaleTerm %>" runat="server">
					<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartSetPromotion)Container.DataItem).Items[0].ProductSaleId)) %>
				</div>
				<div class="SpecifiedcommercialtransactionsCampaignTimePadding100" Visible="<%# ((CartSetPromotion)Container.DataItem).IsDispSetPromotionTerm %>" runat="server">セットプロモーション期間:</div>
				<div class="SpecifiedcommercialtransactionsCampaignTimePadding110" Visible="<%# ((CartSetPromotion)Container.DataItem).IsDispSetPromotionTerm %>" runat="server">
					<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetSetPromotionTermBr(((CartSetPromotion)Container.DataItem).SetPromotionId)) %>
				</div>
			</dl>
		</div>
	</ItemTemplate>
	</asp:Repeater>

	<div class="priceList">
	<div>
	<dl class="bgc">
	<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotal) %></dd>
	</dl>
	<%if (this.ProductIncludedTaxFlg == false) { %>
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt>消費税</dt>
			<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %></dd>
		</dl>
	<%} %>
	<%-- セットプロモーション割引額(商品割引) --%>
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" Visible="<%# ((CartObject)Container.DataItem).IsAllItemsSubscriptionBoxFixedAmount == false %>" runat="server">
	<ItemTemplate>
		<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
		<dd class='<%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>会員ランク割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).MemberRankDiscount * ((((CartObject)Container.DataItem).MemberRankDiscount < 0) ? -1 : 1)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn) { %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期会員割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount* ((((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount < 0) ? -1 : 1)) %></dd>
	</dl>
	<% } %>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<div runat="server" visible="<%# (((CartObject)Container.DataItem).HasFixedPurchase) %>">
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期購入割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "minus" : "" %>'><%#: (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseDiscount * ((((CartObject)Container.DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %></dd>
	</dl>
	</div>
	<%} %>
	<%if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>クーポン割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).UseCouponPrice > 0) ? "minus" : "" %>'>
		<%#: GetCouponName(((CartObject)Container.DataItem)) %>
		<%# (((CartObject)Container.DataItem).UseCouponPrice > 0) ? "-" : "" %>
		<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UseCouponPrice * ((((CartObject)Container.DataItem).UseCouponPrice < 0) ? -1 : 1)) %>
	</dd>
	</dl>
	<%} %>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>ポイント利用額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).UsePointPrice > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).UsePointPrice > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UsePointPrice * ((((CartObject)Container.DataItem).UsePointPrice < 0) ? -1 : 1)) %></dd>
	</dl>
	<%} %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>配送料金<span visible="<%# ((CartObject)Container.DataItem).IsGift %>" runat="server">合計</span></dt>
	<dd runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg) ? "display:none;" : ""%>'>
		<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %></dd>
	<dd runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg == false) ? "display:none;" : ""%>'>
		<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).ShippingPriceSeparateEstimateMessage)%></dd>
	<small style="color:red;" visible="<%# ((CartObject)Container.DataItem).IsDisplayFreeShiipingFeeText %>" runat="server">
		※配送料無料適用外の商品が含まれるため、<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %>の配送料が請求されます
	</small>
	</dl>
	<%-- セットプロモーション割引額(配送料割引) --%>
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
	<ItemTemplate>
		<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>(送料割引)</dt>
			<dd class='<%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>決済手数料</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).Payment.PriceExchange) %></dd>
	</dl>
	<%-- セットプロモーション割引額(決済手数料料割引) --%>
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
	<ItemTemplate>
		<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypePaymentChargeFree %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>(決済手数料割引)</dt>
			<dd class='<%# (((CartSetPromotion)Container.DataItem).PaymentChargeDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).PaymentChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).PaymentChargeDiscountAmount) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	<dl class='<%= : (this.DispNum++ % 2 == 0) ? "" : "bgc" %>' visible="<%# (((CartObject)Container.DataItem).PriceRegulation != 0) %>" runat="server">
	<dt>調整金額</dt>
	<dd class='<%#: (((CartObject)Container.DataItem).PriceRegulation < 0) ? "minus" : "" %>'>
		<%#: (((CartObject)Container.DataItem).PriceRegulation < 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(Math.Abs(((CartObject)Container.DataItem).PriceRegulation)) %></dd>
	</dl>
	</div>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	<div>
	<dl class="result">
	<dt>合計(税込)</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %></dd>
	<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
	<dt>決済金額(税込)</dt>
	<dd><%#:GetSettlementAmount(((CartObject)Container.DataItem)) %></dd>
	<small style="color: red"><%#: string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMOUNT_VARIES_WITH_RATE),((CartObject)Container.DataItem).SettlementCurrency) %></small>
	<% } %>
	</dl>
	<small class="InternationalShippingAttention" runat="server" visible="<%# IsDisplayProductTaxExcludedMessage((CartObject)Container.DataItem) %>">※国外配送をご希望の場合関税・商品消費税は料金に含まれず、商品到着後、現地にて税をお支払いいただくこととなりますのでご注意ください。</small>
	</div>
	<div id="hgcChangeCartInfoBtn" runat="server">
	<p><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>" class="btn btn-mini" style="text-decoration:none;">変更する</a></p>
	</div>
	</div><!--priceList-->

	</div><!--block-->
	</div><!--bottom-->
	</div><!--subCartList-->
	
	<div id="Div6" visible="<%# ((CartObjectList)((Repeater)Container.Parent).DataSource).Items.Count == Container.ItemIndex + 1 %>" runat="server">
	<div class="sumBox">
	<div class="subSumBox">
	<p><img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" />
		<strong><%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %></strong></p>
	</div>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
	<dl>
	<dt id="Dt11" Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server">初回購入獲得ポイント</dt>
	<dd id="Dd11" Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server"><%# WebSanitizer.HtmlEncode(GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalFirstBuyPoint)) %>pt</dd>
	<dt>購入後獲得ポイント</dt>
	<dd><%# WebSanitizer.HtmlEncode(GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalBuyPoint)) %>pt</dd>
	</dl>
	<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
	<%} %>
	</div><!--sumBox-->

	</div>

	<!-- レコメンド設定 -->
	<uc:BodyRecommend runat="server" Cart="<%# (CartObject)Container.DataItem %>" Visible="<%# this.IsOrderCombined == false %>" />

	<!-- 定期注文購入金額 -->
	<uc:BodyFixedPurchaseOrderPrice runat="server" Cart="<%# (CartObject)Container.DataItem %>" Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>" />

	
	</div><!--shoppingCart-->
	<%-- ▲カート情報▲ --%>

	<br class="clr" />
	</div><!--submain-->
	</div><!--main-->

</ItemTemplate>
</asp:Repeater>

<div style="text-align:left;padding:10px 0; width: 780px; margin:0 auto" id="hgcCompleteMessage" runat="server">
	<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal == false) { %>
	以下の内容をご確認のうえ、「注文を確定する」ボタンをクリックしてください。
	<% } %>
	<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
	<br/><%= ShopMessage.GetMessageByPaymentId(this.BindingCartList[0].Payment.PaymentId) %> 
	<% } %>
</div>
<div style="text-align: right">
	<asp:Label id="lblOrderCombineAlert" runat="server">「カートへ戻る」ボタンを押下すると、同梱が解除されます。</asp:Label>
</div>

<div class="btmbtn below">
	<ul>
	<li><asp:LinkButton id="lbCart" runat="server" OnClick="lbCart_Click" class="btn btn-large btn-org-gry">カートへ戻る</asp:LinkButton></li>
	<li>
		<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal) { %>
		<div style="text-align:left; display: inline-block;color: red;">
			カート内の商品が変更されました。<br/>
			お手数ですが再度Amazon Payでの購入手続きに進んでください。<br/><br/>
		</div>
		<div style="width: 200px">
			<div id="AmazonPayCv2Button"></div>
		</div>
		<% } else { %>
			<asp:LinkButton id="lbComplete2" runat="server" onclick="lbComplete_Click" class="btn btn-large btn-success">注文を確定する</asp:LinkButton>
		<% } %>
	</li>
	<li style="display:none;">
		<asp:LinkButton ID="lbCompleteAfterComfirmPayment" runat="server" onclick="lbComplete_Click"></asp:LinkButton>
	</li>
</ul>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</div>
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
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<uc:Loading id="ucLoading" UpdatePanelReload="True" runat="server" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<script type="text/javascript" charset="utf-8">
	showAmazonPayCv2Button(
		'#AmazonPayCv2Button',
		'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
		<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
		'<%= this.AmazonRequest.Payload %>',
		'<%= this.AmazonRequest.Signature %>',
		'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>');
</script>
<% } %>
<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
<script type="text/javascript">
	$(function () {
		$('#<%: lbComplete1.ClientID %>').click(function(){
			$('#modalArea').fadeIn();
		});
		$('#closeModal , #modalBg').click(function(){
			$('#modalArea').fadeOut();
		});
	});
</script>
<% } %>
<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var body = <%= new PaidyCheckout(this.CartList.Items.FirstOrDefault(item => item.Payment.IsPaymentPaygentPaidy)).CreateParameterForPaidyCheckout() %>;
	var hfPaidyPaySelectedControlId = "<%= (this.WhfPaidySelect.ClientID) %>";
	var hfPaidyPaymentIdControlId = "<%= (this.WhfPaidyPaymentId.ClientID) %>";
	var hfPaidyStatusControlId = "<%= (this.WhfPaidyStatus.ClientID) %>";
	var isHistoryPage = false;
	var lbNextProcess = "<%= lbCompleteAfterComfirmPayment.ClientID %>";
</script>
<uc:PaidyCheckoutScript runat="server" />
</asp:Content>
