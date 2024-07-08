<%--
=========================================================================================================
  Module      : ランディングカート確認画面(LandingCartConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/Form/Common/BodyRecommend.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/LandingOrderPage.master" AutoEventWireup="true" CodeFile="~/Landing/LandingCartConfirm.aspx.cs" Inherits="Landing_LandingCartConfirm" Title="注文確認画面" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Register TagPrefix="uc" TagName="AtonePaymentScript" Src="~/Form/Common/AtonePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="AfteePaymentScript" Src="~/Form/Common/AfteePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScriptForPaygent.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paygent.Paidy.Checkout" %>

<asp:Content ContentPlaceHolderID="AffiliateTagHead" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagHead"
					Location="head"
					Datas="<%# this.CartList %>"
					runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyTop" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyTop"
					Location="body_top"
					Datas="<%# this.CartList %>"
					runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyBottom" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyBottom"
					Location="body_bottom"
					Datas="<%# this.CartList %>"
					runat="server"/>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%: this.ResolveClientUrl("~/Css/modal.css?" + Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED) %>" rel="stylesheet" type="text/css" media="all" />
<%-- △編集可能領域△ --%>
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
	// 注文ボタン押下した際のJavascript処理追加
	if (this.WrCartList.Items.Count >= 1)
	{
		lbComplete1.OnClientClick
			= ((LinkButton)this.WrCartList.Items[this.WrCartList.Items.Count - 1].FindControl("lbComplete2")).OnClientClick
			= (this.HideOrderButtonWithClick) ? "return exec_submit(true)" : "return exec_submit(false)";

		if ((this.CartList.Items[this.WrCartList.Items.Count - 1]).Payment.IsPaymentPaygentPaidy)
		{
			lbComplete1.OnClientClick
				= ((LinkButton)this.WrCartList.Items[this.WrCartList.Items.Count - 1].FindControl("lbComplete2")).OnClientClick
				= "PaidyPayProcess(); return false;";
		}
	}
	if (this.IsDispCorrespondenceSpecifiedCommericalTransactions)
	{
		lbComplete1.OnClientClick = "return false;";
		lbComplete3.OnClientClick = (this.HideOrderButtonWithClick) ? "return exec_submit(true)" : "return exec_submit(false)";

		if ((this.CartList.Items[this.WrCartList.Items.Count - 1]).Payment.IsPaymentPaygentPaidy)
		{
			lbComplete3.OnClientClick = "PaidyPayProcess(); return false;";
		}
	}
%>
<%-- 注文ボタン押下した際の処理 --%>
<script type="text/javascript">
<!--
	var blSubmitted = false;
	var isLastItemCart = false;
	var isPageConfirm = false;
	var isMyPage = null;
	var completeButton = null;
	var paymentNeedSubmitted = false;

	function exec_submit(blClearSubmitButton)
	{
		completeButton = document.getElementById('<%= lbCompleteAfterComfirmPayment.ClientID %>');

		<% if(Constants.PAYMENT_ATONEOPTION_ENABLED && this.IsUseAtonePaymentAndNotYetCardTranId) { %>
		GetAtoneAuthority();
		<% } %>
		<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED && this.IsUseAfteePaymentAndNotYetCardTranId) { %>
		GetAfteeAuthority();
		<% } %>
		if (blSubmitted) return false;

		<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED && this.HasOrderHistorySimilarShipping) { %>
		var confirmMessage = '<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
		if (confirm(confirmMessage) == false) return false;
		<% } %>

		blSubmitted = true;

		return true;
	}

	window.onload = function () {
		<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
		var isUnselected = <%= IsUnselectedProductOption() ? "true" : "false" %>;
		if (isUnselected) {
			alert('<%= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_OPTION_UNSELECTED) %>');
			__doPostBack('ctl00$ContentPlaceHolder1$rCartList$ctl00$lbBack', '');
		}
		<% } %>
	};
//-->
</script>
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

		// Get Cart Index
		function GetIndexCartHavingPaymentAtoneOrAftee(isAtone, callBack) {
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
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
					blSubmitted = true;
				}
				else {
					blSubmitted = false;
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
					string.Format("{0}{1}",
						this.IsSmartPhone
							? "SmartPhone/"
							: string.Empty,
						Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)); %>
	<uc:AfteePaymentScript ID="ucAfteePaymentScript" runat="server"/>
<% } %>

<asp:HiddenField ID="hfPaidyPaymentId" runat="server" />
<asp:HiddenField ID="hfPaidySelect" runat="server" />
<asp:HiddenField ID="hfPaidyStatus" runat="server" />

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

		// Get Cart Index
		function GetIndexCartHavingPaymentAtoneOrAftee(isAtone, callBack) {
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
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
					blSubmitted = true;
				}
				else {
					blSubmitted = false;
				}
			});
		}

		// Execute Order
		function ExecuteOrder() {
			var buttonComplete = document.getElementById('<%= lbComplete1.ClientID %>');
			buttonComplete.click();
		}
	</script>
	<% ucAtonePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)); %>
	<uc:AtonePaymentScript ID="ucAtonePaymentScript" runat="server"/>
<% } %>
<% if(this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
<section id="modalArea" class="modalArea">
	<div id="modalBg" class="modalBg"></div>
	<div class="modalWrapper">
		<div class="modalContents">
			<%= ShopMessage.GetMessageByPaymentId(this.CartList.Items[0].Payment.PaymentId) %>
			<br/><asp:LinkButton id="lbComplete3" runat="server" onclick="lbComplete_Click" class="btn btn-success">注文を確定する</asp:LinkButton>
		</div>
		<div id="closeModal" class="closeModal">
			×
		</div>
	</div>
</section>
<% } %>
<% if (this.IsCartListLp) { %>
<p id="CartFlow"><img src="../Contents/ImagesPkg/order/cart_lp_step01.gif" alt="ご注文内容確認 " width="781" height="58" /></p>
<% } %>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div class="btmbtn above cartstep">
	<h2 class="ttlA">ご注文内容確認</h2>
	<ul>
		<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal == false) { %>
		<li><asp:LinkButton id="lbComplete1" runat="server" onclick="lbComplete_Click" class="btn btn-success">注文を確定する</asp:LinkButton></li>
		<li style="display:none;">
			<asp:LinkButton ID="lbCompleteAfterComfirmPayment" runat="server" onclick="lbComplete_Click"></asp:LinkButton>
		</li>
		<% } %>
	</ul>
</div>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblNotFirstTimeFixedPurchaseAlert" runat="server" visible="false"></asp:Label>
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
<div style="color: red; font-weight: bold">
	<asp:Label id="lblPaypayErrorMessage" runat="server" visible="false" />
</div>
<asp:Repeater id="rCartList" OnItemCommand="rCartList_OnItemCommand" Runat="server">
<ItemTemplate>
	<div id="Div1" style="color: red; font-weight: bold" Visible="<%# IsGlobalShippingPriceCalcError((CartObject)Container.DataItem) %>" runat="server">
		カート内の商品に配送料金が設定されていません。
		カートから削除してください。
	</div>
	<div class="main">
	<div class="submain">
	<%-- ▼注文内容▼ --%>
	<div class="column">
	<div visible="<%# Container.ItemIndex == 0 %>" runat="server">
	<h2><img src="../Contents/ImagesPkg/order/sttl_cash_confirm.gif" alt="注文情報" width="64" height="16" /></h2>
	<p class="pdg_bottomA">以下の内容をご確認のうえ、「注文を確定する」ボタンを<br />クリックしてください。</p>
	</div>
	
	<div class="orderBox">
	<h3>カート番号<%# Container.ItemIndex + 1 %></h3>
	<div class="bottom">
	<div class="box" visible="<%# Container.ItemIndex == 0 %>" runat="server">
	<em>本人情報確認</em>
	<div>
	<dl>
	<%-- 氏名 --%>
	<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Name1) %><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Name2) %>&nbsp;様</dd>
	<% if ((this.IsAmazonCv2Guest == false) || (Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED) || IsTargetToExtendedAmazonAddressManagerOption()) { %>
	<div <%# (string.IsNullOrEmpty(((CartObject)Container.DataItem).Owner.NameKana1 + ((CartObject)Container.DataItem).Owner.NameKana2) == false) ? "" : "style=\"display:none;\"" %>>
		<%-- 氏名（かな） --%>
		<dt <%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? "" : "style=\"display:none;\"" %>><%: ReplaceTag("@@User.name_kana.name@@") %>：</dt>
		<dd <%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? "" : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.NameKana1) %><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.NameKana2) %>&nbsp;さま</dd>
	</div>
	<% } %>
	<%-- PCメールアドレス --%>
	<dt><%: ReplaceTag("@@User.mail_addr.name@@") %>：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.MailAddr) %></dd>
	<% if ((this.IsAmazonCv2Guest == false) || (Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED) || IsTargetToExtendedAmazonAddressManagerOption()) { %>
	<dt>
		<%: ReplaceTag("@@User.addr.name@@") %>：
	</dt>
	<dd><p>
		<%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? "〒" + WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Zip) + "<br />" : "" %>
		<%#: ((CartObject)Container.DataItem).Owner.Addr1 %><%#: ((CartObject)Container.DataItem).Owner.Addr2 %><br />
		<%#: ((CartObject)Container.DataItem).Owner.Addr3 %><%#: ((CartObject)Container.DataItem).Owner.Addr4 %><br />
		<%#: ((CartObject)Container.DataItem).Owner.Addr5 %><%# (((CartObject)Container.DataItem).Owner.IsAddrJp == false) ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Zip) + "<br />" : "" %>
		<%#: ((CartObject)Container.DataItem).Owner.AddrCountryName %>
		</p>
	</dd>
	<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
	<!-- 企業名・法人名 -->
	<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
		<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.CompanyName) %>&nbsp<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.CompanyPostName) %></dd>
	<%} %>
	<%-- 電話番号 --%>
	<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
	<dd><%#: ((CartObject)Container.DataItem).Owner.Tel1 %></dd>
	<%-- 電話番号（予備） --%>
	<dt><%: ReplaceTag("@@User.tel2.name@@") %>：</dt>
	<dd><%#: ((CartObject)Container.DataItem).Owner.Tel2 %>&nbsp;</dd>
	<% } %>
	<dt>お知らせメールの<br />配信希望：</dt>
	<dd><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, ((CartObject)Container.DataItem).Owner.MailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG))%><br />&nbsp;</dd>
	<% if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE) { %>
	<dt>会員登録：</dt>
	<dd><%= (this.RegisterUser != null) ? "登録する" : "登録しない" %></dd>
	<% } %>
	</dl>
	<p class="clr"><img src="../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		<div>
			<p class="btn_change">
				<asp:LinkButton CommandName="GoBackLp" CommandArgument="<%# this.FocusingControlsOnOrderOwner %>" runat="server" class="btn btn-mini"><span>変更する</span></asp:LinkButton>
			</p>
		</div>
	</div>
	</div><!--box-->

	<div class="box">
	<em>配送先情報</em>
	<div>
		<div visible="<%# ((CartObject)Container.DataItem).GetShipping().IsShippingStorePickup %>" runat="server">
			<dl>
				<dt>受取店舗：</dt>
				<dd><%#: ((CartObject)Container.DataItem).GetShipping().RealShopName %></dd>
				<dt>受取店舗住所：</dt>
				<dd>
					<p>
						<%#: "〒" + ((CartObject)Container.DataItem).GetShipping().Zip %>
						<br />
						<%#: ((CartObject)Container.DataItem).GetShipping().Addr1 %>
						<%#: ((CartObject)Container.DataItem).GetShipping().Addr2 %>
						<br />
						<%#: ((CartObject)Container.DataItem).GetShipping().Addr3 %>
						<br />
						<%#: ((CartObject)Container.DataItem).GetShipping().Addr4 %>
						<br />
						<%#: ((CartObject)Container.DataItem).GetShipping().Addr5 %>
					</p>
				</dd>
				<dt>営業時間：</dt>
				<dd>
					<%#: ((CartObject)Container.DataItem).GetShipping().RealShopOpenningHours %>
				</dd>
				<dt>店舗電話番号：</dt>
				<dd><%#: ((CartObject)Container.DataItem).GetShipping().Tel1 %></dd>
			</dl>
		</div>
	<dl visible="<%# ((CartObject)Container.DataItem).GetShipping().IsShippingStorePickup == false %>" runat="server">
	<div runat="server" visible='<%# ((((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)) %>'>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>：
		</dt>
		<dd>
			<p>
				<%#: ((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp ? "〒" + ((CartObject)Container.DataItem).GetShipping().Zip : string.Empty %>
				<%#: ((CartObject)Container.DataItem).GetShipping().Addr1 %><%#: ((CartObject)Container.DataItem).GetShipping().Addr2 %><br />
				<%#: ((CartObject)Container.DataItem).GetShipping().Addr3 %><%#: ((CartObject)Container.DataItem).GetShipping().Addr4 %>
				<%#: ((CartObject)Container.DataItem).GetShipping().Addr5 %><br />
				<%# (((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp == false) ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().Zip) + "<br />" : string.Empty %>
				<%#: ((CartObject)Container.DataItem).GetShipping().ShippingCountryName %>
			</p>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
			<dt><%: ReplaceTag("@@User.company_name.name@@")%>
			<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
			<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().CompanyName) %>
				<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().CompanyPostName)%>&nbsp;
			</dd>
		<% } %>
		<%-- 氏名 --%>
		<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().Name1) %>
			<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().Name2) %>&nbsp;様
		</dd>
		<div <%# (string.IsNullOrEmpty(((CartObject)Container.DataItem).GetShipping().NameKana1 + ((CartObject)Container.DataItem).GetShipping().NameKana2) == false) ? string.Empty : "style=\"display:none;\"" %>>
			<%-- 氏名（かな） --%>
			<dt <%# (((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp) ? string.Empty : "style=\"display:none;\"" %>><%: ReplaceTag("@@User.name_kana.name@@") %>：</dt>
			<dd <%# (((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp) ? string.Empty : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().NameKana1) %>
				<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().NameKana2) %>&nbsp;さま
			</dd>
		</div>
		<%-- 電話番号 --%>
		<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
		<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().Tel1) %></dd>
	</div>
	<div runat="server" visible='<%# ((((CartObject)Container.DataItem).GetShipping().ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE) || (((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)) %>'>
		<dt>店舗ID：</dt>
		<dd><%#: ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreId %>&nbsp;</dd>
		<dt>店舗名称：</dt>
		<dd><%#: ((CartObject)Container.DataItem).GetShipping().Name1 %>&nbsp;</dd>
		<dt>店舗住所：</dt>
		<dd><%#: ((CartObject)Container.DataItem).GetShipping().Addr4 %>&nbsp;</dd>
		<dt>店舗電話番号：</dt>
		<dd><%#: ((CartObject)Container.DataItem).GetShipping().Tel1 %>&nbsp;</dd>
	</div>
		<dt>配送方法：</dt>
	<dd>
		<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, ((CartObject)Container.DataItem).Shippings[0].ShippingMethod) %>
	</dd>
	<dt visible ="<%# CanDisplayDeliveryCompany(Container.ItemIndex) %>" runat="server">配送サービス：</dt>
	<dd visible ="<%# CanDisplayDeliveryCompany(Container.ItemIndex) %>" runat="server">
		<%#: GetDeliveryCompanyName(((CartObject)Container.DataItem).Shippings[0].DeliveryCompanyId) %>
	</dd>
	<dt visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingDateFlg %>' runat="server">
		配送希望日：</dt>
	<dd visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingDateFlg %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingDate(((CartObject)Container.DataItem).GetShipping()))%></dd>
	<dt visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg && ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>' runat="server">
		配送希望時間帯：</dt>
	<dd visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg && ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingTime(((CartObject)Container.DataItem).GetShipping()))%></dd>
	<dt visible='<%# ((CartObject)Container.DataItem).GetOrderMemosForOrderConfirm().Trim() != "" %>' runat="server">注文メモ：</dt>
	<dd visible='<%# ((CartObject)Container.DataItem).GetOrderMemosForOrderConfirm().Trim() != "" %>' runat="server"><%# WebSanitizer.HtmlEncodeChangeToBr(((CartObject)Container.DataItem).GetOrderMemosForOrderConfirm()) %></dd>
	<span runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(((CartObject)Container.DataItem).GetShipping().ShippingCountryIsoCode) %>">
		<span>
			<dt>
				発票種類：
				<div runat="server" visible="<%# (((CartObject)Container.DataItem).Shippings[0].UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) %>">
					<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@") %>：<br />
					<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@") %>：
				</div>
				<div runat="server" visible="<%# (((CartObject)Container.DataItem).Shippings[0].UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_DONATE) %>">
					<%# ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@") %>：
				</div>
			</dt>
			<dd>
				<%# GetInformationInvoice(((CartObject)Container.DataItem).Shippings[0]) %>
			</dd>
			<span runat="server" visible="<%# (((CartObject)Container.DataItem).Shippings[0].UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) %>">
				<dt>
					共通性載具：
					<div visible="<%# string.IsNullOrEmpty(((CartObject)Container.DataItem).Shippings[0].CarryType) == false %>" runat="server">
						載具コード：
					</div>
				</dt>
				<dd><%# GetInformationInvoice(((CartObject)Container.DataItem).Shippings[0], true) %></dd>
			</span>
		</span>
		<br />
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
	</dl>
		<p><%#: (((CartObject)Container.DataItem).ReflectMemoToFixedPurchase) ? "※2回目以降の注文メモにも追加する" : "" %></p>
	</div>

	<em visible="<%# FindCart(((CartObject)Container.DataItem).GetShipping()).HasFixedPurchase %>" runat="server">定期配送情報</em>
	<div visible="<%# FindCart(((CartObject)Container.DataItem).GetShipping()).HasFixedPurchase %>" runat="server">
		<dl visible="<%# FindCart(((CartObject)Container.DataItem).GetShipping()).HasFixedPurchase %>" runat="server">
		<dt>配送パターン：</dt>
		<dd><%#: ((CartObject)Container.DataItem).GetShipping().GetFixedPurchaseShippingPatternString() %></dd>
		<dt>初回配送予定：</dt>
		<dd><%#: GetFirstShippingDate(((CartObject)Container.DataItem).GetShipping()) %></dd>
		<dt>今後の配送予定：</dt>
		<dd><%#: DateTimeUtility.ToStringFromRegion(((CartObject)Container.DataItem).GetShipping().NextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dd>
		<dt></dt>
		<dd><%#: DateTimeUtility.ToStringFromRegion(((CartObject)Container.DataItem).GetShipping().NextNextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dd>
		<p>※定期解約されるまで継続して指定した配送パターンでお届けします。</p><br />
		<dt visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg %>' runat="server">配送希望時間帯：</dt>
		<dd visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingTime(((CartObject)Container.DataItem).GetShipping())) %></dd>
		</dl>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<div>
		<p class="btn_change">
			<asp:LinkButton CommandName="GoBackLp" CommandArgument='<%# this.FocusingControlsOnOrderShipping %>' runat="server" class="btn btn-mini"><span>変更する</span></asp:LinkButton>
		</p>
	</div>
	</div><!--box-->

	<div class="last">
	<div class="box">
	<em>決済情報</em>
	<div>
	<dl>
	<dt>お支払い：</dt>
	<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.PaymentName) %></dd>
	<dt visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server">支払先コンビニ名</dt>
	<dd visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server"><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetPaymentCvsName()) %></dd>
	<dt visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardCompany) != "" %>' runat="server">カード会社：</dt>
	<dd visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardCompany) != "" %>' runat="server"><%#: ((CartObject)Container.DataItem).Payment.CreditCardCompanyName %></dd>
	<dt visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">カード番号：</dt>
	<dd visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">XXXXXXXXXXXX<%# WebSanitizer.HtmlEncode(GetCreditCardDispString(((CartObject)Container.DataItem).Payment)) %></dd>
	<dt visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">有効期限：</dt>
	<dd visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server"><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireMonth) %>/<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireYear) %>(月/年)</dd>
	<dt visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">支払い回数：</dt>
	<dd visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, ((CartObject)Container.DataItem).Payment.CreditInstallmentsCode))%></dd>
	<dt visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">カード名義：</dt>
	<dd visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server"><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditAuthorName) %></dd>
	<dt visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">支払い方法</dt>
	<dd visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">
		<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
	</dd>
	<dt visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">支払い方法：</dt>
	<dd visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">
		<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb", ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
	</dd>
	</dl>
	<p class="clr"><img src="../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	<p class="btn_change">
		<asp:LinkButton CommandName="GoBackLp" CommandArgument='<%# this.FocusingControlsOnOrderPayment %>' runat="server" class="btn btn-mini"><span>変更する</span></asp:LinkButton>
	</p>
	</div>

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
	</div><!--box-->
	</div><!--last-->

	</div><!--bottom-->
	</div><!--orderBox-->
	</div><!--column-->
	<%-- ▲注文内容▲ --%>

	<%-- ▼カート情報▼ --%>
	<div class="shoppingCart">
	<div visible="<%# Container.ItemIndex == 0 %>" runat="server">
	<h2><img src="../Contents/ImagesPkg/common/ttl_shopping_cart.gif" alt="ショッピングカート" width="141" height="16" /></h2>
	<div class="sumBox mrg_topA">
	<div class="subSumBoxB">
	<p><img src="../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" />
		<strong><%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %></strong></p>
	</div>
	</div><!--sum-->
	</div>
	
	<div class="subCartList">
	<div class="bottom">
	<h3>
		カート番号<%# Container.ItemIndex + 1 %></h3>
	<div class="block">
	<asp:Repeater ID="rCart" DataSource="<%# ((CartObject)Container.DataItem).Items %>" OnItemDataBound="rCart_OnItemDataBound" runat="server">
	<ItemTemplate>
		<%-- 通常商品 --%>
		<div class="singleProduct" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
		<div>
		<dl>
		<dt>
			<% if (this.IsCartListLp) { %>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
			<% } else { %>
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
			<% } %>
		</dt>
		<dd>
			<strong>
				<% if (this.IsCartListLp) { %>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
				<% } else { %>
					<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
				<% } %>
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
		<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet)%></p>
		<p runat="server" ID="pPrice" style="text-decoration: line-through" Visible="False"><span runat="server" ID="sPrice"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
		<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px"><span runat="server" ID="sSubscriptionBoxCampaignPrice"><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
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
		<p ID="pSubscriptionBoxCampaignPeriod" Visible="False" style="color: red; padding-top: 2px" runat="server">キャンペーン期間：<br/>
			<span ID="sSubscriptionBoxCampaignPeriodSince" class="specifiedcommercialtransactionsCampaignTime" runat="server"></span>～<br/>
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
						<% if (this.IsCartListLp) { %>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
							<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
						<% } else { %>
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
						<% } %>
					</dt>
					<dd>
						<strong>
							<% if (this.IsCartListLp) { %>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
							<% } else { %>
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
							<% } %>
						</strong>
						<p><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) %></p>
						<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
							※配送料無料適用外商品です
						</p>
					</dd>
				</dl>
			</div>
			<table visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == ((CartProduct)Container.DataItem).ProductSet.Items.Count) %>" width="297" cellpadding="0" cellspacing="0" class="clr" runat="server">
			<tr>
			<th width="38">セット：</th>
			<th width="50"><%# GetProductSetCount((CartProduct)Container.DataItem) %></th>
			<th width="146"><%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal((CartProduct)Container.DataItem)) %> (<%#: this.ProductPriceTextPrefix %>)</th>
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
							<% if (this.IsCartListLp) { %>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
							<% } else { %>
							<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
							<% } %>
						</dt>
						<dd>
							<strong>
								<% if (this.IsCartListLp) { %>
									<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
								<% } else { %>
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
								<% } %>
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
							<p class="specifiedcommercialtransactionsCampaignTime">数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%#: ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]%></p>
							<p class="specifiedcommercialtransactionsCampaignTime"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
							<p class="specifiedcommercialtransactionsCampaignTime"><%# WebSanitizer.HtmlEncodeChangeToBr(((CartProduct)Container.DataItem).ReturnExchangeMessage) %></p>
							<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								※配送料無料適用外商品です
							</p>
						</dd>
					</dl>
				</div>
			</ItemTemplate>
			</asp:Repeater>
			<dl class="setpromotion">
				<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
				<dd>
					<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
						<strike><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (<%#: this.ProductPriceTextPrefix %>)</strike><br />
					</span>
					<span class="specifiedcommercialtransactionsCampaignTimeSpan">
					<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (<%#: this.ProductPriceTextPrefix %>)
					</span>
				</dd>
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
			<dt>消費税額</dt>
			<dd><%# CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %></dd>
		</dl>
	<%} %>
	<%-- セットプロモーション割引額(商品割引) --%>
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
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
	<%} %>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<div runat="server" visible="<%# (((CartObject)Container.DataItem).HasFixedPurchase) %>">
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期購入割引額</dt>
	<dd class='<%# (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount > 0) ? "minus" : "" %>'><%# (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount* ((((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %></dd>
	</dl>
	</div>
	<% } %>
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
	<dt>配送料金</dt>
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
	</div>
	<p class="clr"><img src="../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	<div>
	<dl class="result">
	<dt>合計(税込)</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %></dd>
	<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
	<dt>決済金額(税込)</dt>
	<dd><%#: GetSettlementAmount(((CartObject)Container.DataItem)) %></dd>
	<small style="color: red"><%#: string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMOUNT_VARIES_WITH_RATE), ((CartObject)Container.DataItem).SettlementCurrency) %></small>
	<% } %>
	</dl>
	<small class="InternationalShippingAttention" runat="server" visible="<%# IsDisplayProductTaxExcludedMessage((CartObject)Container.DataItem) %>">※国外配送をご希望の場合消費税は料金に含まれず、商品到着後、お客様に直接お支払いいただくこととなりますのでご注意ください。</small>
	</div>
	<div>
		<p class="btn_change">
			<asp:LinkButton CommandName="GoBackLp" CommandArgument="<%# this.FocusingControlsOnCartList %>" CssClass="btn btn-mini" runat="server">変更する</asp:LinkButton>
		</p>
	</div>
	</div><!--priceList-->

	</div><!--block-->
	</div><!--bottom-->
	</div><!--subCartList-->
	
	<div visible="<%# ((CartObjectList)((Repeater)Container.Parent).DataSource).Items.Count == Container.ItemIndex + 1 %>" runat="server">
	<div class="sumBox">
	<div class="subSumBox">
	<p><img src="../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" />
		<strong><%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %></strong></p>
	</div>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
	<dl>
	<dt Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server">初回購入獲得ポイント</dt>
	<dd Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server"><%# WebSanitizer.HtmlEncode(GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalFirstBuyPoint)) %>pt</dd>
	<dt>購入後獲得ポイント</dt>
	<dd><%# WebSanitizer.HtmlEncode(GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalBuyPoint)) %>pt</dd>
	</dl>
	<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
	<%} %>
	</div><!--sumBox-->

	</div>

	<!-- レコメンド設定 -->
	<uc:BodyRecommend ID="BodyRecommend1" runat="server" Cart="<%# (CartObject)Container.DataItem %>" />
	
	<!-- 定期注文購入金額 -->
	<uc:BodyFixedPurchaseOrderPrice runat="server" Cart="<%# (CartObject)Container.DataItem %>" Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>" />

	</div><!--shoppingCart-->
	<%-- ▲カート情報▲ --%>

	<br class="clr" />
	</div><!--submain-->
	</div><!--main-->

<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal == false) { %>
<div style="text-align:left;padding:10px 0; margin: 0 auto; width: 780px" visible="<%# ((this.CartList.Items.Count > 1) ? ((this.CartList.Items.Count - Container.ItemIndex) == 1) : (Container.ItemIndex == 0)) %>" runat="server">
	以下の内容をご確認のうえ、「注文を確定する」ボタンをクリックしてください。
	<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
	<br/><%= ShopMessage.GetMessageByPaymentId(this.CartList.Items[0].Payment.PaymentId) %> 
	<% } %>
</div>
<% } %>

<div class="btmbtn below" visible="<%# ((this.CartList.Items.Count > 1) ? ((this.CartList.Items.Count - Container.ItemIndex) == 1) : (Container.ItemIndex == 0)) %>" runat="server">
<ul>
	<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal) { %>
	<li>
		<div style="text-align:left; display: inline-block;color: red;">
			カート内の商品が変更されました。<br/>
			お手数ですが再度Amazon Payでの購入手続きに進んでください。<br/><br/>
		</div>
		<div style="width: 200px">
			<div id="AmazonPayCv2Button"></div>
		</div>
	</li>
	<% } else {%>
	<li><asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server" class="btn btn-large">前のページに戻る</asp:LinkButton></li>
	<li><asp:LinkButton id="lbComplete2" runat="server" onclick="lbComplete_Click" class="btn btn-large btn-success">注文を確定する</asp:LinkButton>
	</li>
	<% } %>
	
</ul>
</div>

</ItemTemplate>
</asp:Repeater>

</div>
</ContentTemplate>
</asp:UpdatePanel>
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
		'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>')
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
