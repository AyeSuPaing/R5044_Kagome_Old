<%--
=========================================================================================================
  Module      : カート一覧画面(CartList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyAnnounceFreeShipping" Src="~/Form/Common/BodyAnnounceFreeShipping.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommendFreeShipping" Src="~/Form/Common/BodyRecommendFreeShipping.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register Src="~/Form/Common/PayPalScriptsForm.ascx" TagPrefix="uc" TagName="PayPalScriptsForm" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/CartList.aspx.cs" Inherits="Form_Order_CartList" Title="ショッピングカートページ" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<%-- ▽▽Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない▽▽ --%>
<script runat="server">
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
</script>
<%-- △△Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない△△ --%>
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
<script type="text/javascript">

	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			try {
				<%-- Ajax動いてカート内容操作後にウィジェット消える場合のエラーを回避 --%>
				showAmazonPayCv2Button(
					'#AmazonPayCv2Button',
					'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
					<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
					'<%= this.AmazonRequest.Payload %>',
					'<%= this.AmazonRequest.Signature %>',
					'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>');
				if ($('#AmazonPayButton').length) showButton();
			}
			catch (e) { }
		}
	}
</script>
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<uc:AffiliateTag ID="AffiliateTagFree" Location="free" runat="server"/>

<p id="CartFlow"><img src="../../Contents/ImagesPkg/order/cart_step00.gif" alt="カート内容確認 " width="781" height="58" /></p>

<div class="btmbtn above cartstep">
<h2 class="ttlB">
	カート内容確認
</h2>
	<% if (this.Process.IsSubscriptionBoxError == false) { %>
<ul style="display:<%= (this.CartList.Items.Count != 0) ? "block" : "none" %>">
	<%-- UPDATE PANELの外のイベントを呼び出す --%>
	<%-- ▼PayPalログインここから▼ --%>
	<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
	<%if (this.DispPayPalShortCut) {%>
	<li style="float: left; width: 200px; margin: 0; position: relative; z-index: 1">
		<%
			ucPaypalScriptsForm.LogoDesign = "Cart";
			ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
			ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
		%>
		<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
		<div id="paypal-button"></div>
		<div style="font-size: 9pt; text-align: center;margin: 3px">
		<%if (SessionManager.PayPalCooperationInfo != null) {%>
			<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済
		<%} %>
		<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
		</div>
	</li>
	<% } %>
	<% } %>
	<%-- ▲PayPalログインここまで▲ --%>
	<li>
		<%if ( this.CanUseAmazonPaymentForFront()) { %>
		<%-- ▼▼Amazonお支払いボタンウィジェット▼▼ --%>
			<div id="AmazonPayButton" style="display:inline" title="Amazonアカウントでお支払いの場合はコチラから"></div>
			<div style="display: inline-block; margin-bottom: -20px">
				<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
				<div id="AmazonPayCv2Button"></div>
				<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
			</div>
			<%-- ▲▲Amazonお支払いボタンウィジェット▲▲ --%>
		<% } %>
	</li>
	<li><asp:Linkbutton Visible="<%# this.IsDisplayTwoClickButton %>" ID="lbTwoClickButton1" OnClick="lbTwoClickButton_Click" class="btn btn-success" runat="server">２クリック購入</asp:Linkbutton></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>" class="btn btn-success">ご購入手続き</a></li>
</ul>
	<% } %>
</div>

<%if (this.CartList.Items.Count != 0) {%>
	下の内容をご確認のうえ、「ご購入手続き」ボタンを押してください。
<%} else { %>
	カートに商品がありません。
<%} %>

<%-- 次へイベント用リンクボタン（イベント作成用。通常はUpdatePanel内部に設置する） --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" runat="server"></asp:LinkButton>

<div id="CartList">
<% if (string.IsNullOrEmpty(this.DispErrorMessage) == false) { %>
<span style="color:red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.DispErrorMessage) %></span>
<% } %>
<p class="sum"><img src="../../Contents/ImagesPkg/cartlist/ttl_sum.gif" alt="総合計" width="48" height="16" /><strong><%: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %></strong></p>

<%if (this.CartList.Items.Count != 0) {%>
<asp:Repeater id="rCartList" Runat="server" OnItemCommand="rCartList_ItemCommand" OnItemDataBound="rCartList_ItemDataBound">
<HeaderTemplate>
	<w2c:FacebookConversionAPI ID="FacebookConversionAPI1"
								EventName="AddToCart"
								CartList="<%# this.CartList %>"
								UserId="<%#: this.LoginUserId %>"
								runat="server" />
</HeaderTemplate>
<ItemTemplate>
	<asp:PlaceHolder ID="phSubscriptionBoxErrorMsg" runat="server" Visible="<%# string.IsNullOrEmpty(((CartObject)Container.DataItem).SubscriptionBoxErrorMsg) == false %>">
		<span style="color:red"><%# ((CartObject)Container.DataItem).SubscriptionBoxErrorMsg %></span>
	</asp:PlaceHolder>
	<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# ((CartObject)Container.DataItem).SubscriptionBoxCourseId %>" />
	<%if (Constants.ProductOrderLimitKbn.ProductOrderLimitOn == Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY) { %>
	<div runat="server" visible="<%# ((CartObject)Container.DataItem).HasNotFirstTimeOrderIdList %>" style="text-align:left;position:relative;bottom:30px;">
	</div>
	<%} %>

	<%if (Constants.CARTCOPY_OPTION_ENABLED){ %>
	<%-- ▼カート削除完了メッセージ▼ --%>
	<div runat="server" visible="<%# ((CartObject)Container.DataItem).IsCartDeleteCompleteMesseges %>" style="text-align:left;position:relative;bottom:30px;">
		<span>カートの削除が完了しました。</span>
	</div>
	<%-- ▲カート削除完了メッセージ▲ --%>
	<%} %>
	
<%if (this.HasOrderCombinedReturned) { %>
	<%-- ▼注文同梱解除メッセージ▼ --%>
	<div runat="server" style="text-align:left;position:relative;bottom:30px;">
		<span>注文同梱を解除しました。</span>
	</div>
	<%-- ▲注文同梱解除メッセージ▲ --%>
<% } %>

	<div class="productList">
	<div class="background">

	<%if (Constants.CARTCOPY_OPTION_ENABLED && (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false)) { %>
	<%-- ▼カートコピー完了メッセージ▼ --%>
	<div runat="server" visible="<%# ((CartObject)Container.DataItem).IsCartCopyCompleteMesseges %>" style="text-align:right">
		<span>カートのコピーが完了しました。</span>
	</div>
	<%-- ▲カートコピー完了メッセージ▲ --%>
	<%} %>

	<h3>カート番号 <%# Container.ItemIndex + 1 %><%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）")) %>のご注文内容<%if (Constants.CARTCOPY_OPTION_ENABLED){ %><span style="float:right"><asp:LinkButton ID="lbCopyCart" runat="server" Text="カートコピー" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbCopyCart_Click" style="color:white;" Visible="<%# Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false %>" />&nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lbDeleteCart" runat="server" Text="カート削除" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbDeleteCart_Click" style="color:white" /></span><%} %></h3>
	<div class="list" style="background-color: #f1f1f1;">
	<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)　{ %>
	<p class="ttl" style="display: flex;">
		<span style="padding-left: 50px; padding-right: 180px;">商品名</span>
		<span style="padding-left: 100px; padding-right: 200px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount %>" runat="server">頒布会コース名</span>
		<span style="padding-left: 40px;padding-right: 10px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">単価（税込）</span>
		<span id="optionPriceSpan" style="padding-left: 10px;" runat="server"><span runat="server" Visible="<%# HasProductOptionPrice((CartObject)Container.DataItem)%>">オプション価格（税込）</span></span>
		<span style="padding-left: 40px;padding-right: 25px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBox == false %>" runat="server">注文数</span>
		<span style="padding-left: 15px;padding-right: 13px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBox %>" runat="server">初回商品数</span>
		<span style="padding-left: 10px;padding-right: 20px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">消費税率</span>
		<span style="padding-left: 10px;padding-right: 24px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">小計（税込）</span>
	</p>
		<% } else { %>
		<p class="ttl">
			<span style="padding-left: 120px; padding-right: 105px;">商品名</span>
			<span style="padding-left: 120px; padding-right: 203px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount %>" runat="server">頒布会コース名</span>
			<span style="padding-left: 40px;padding-right: 24px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">単価（税込）</span>
			<span style="padding-left: 30px;padding-right: 24px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBox == false %>" runat="server">注文数</span>
			<span style="padding-left: 15px;padding-right: 13px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBox %>" runat="server">初回商品数</span>
			<span style="padding-left: 10px;padding-right: 18px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">消費税率</span>
			<span style="padding-left: 10px;padding-right: 24px;" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">小計（税込）</span>
		</p>
		<% } %>
	<asp:Repeater id="rCart" runat="server" DataSource='<%# (CartObject)Container.DataItem %>' OnItemCommand="CallCartList_ItemCommand">
	<ItemTemplate>
		<%-- 通常商品 --%>
		<div class="product" style="background-color: white;" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
		<%-- 隠し値 --%>
		<input type="hidden" id="hfProductName" value="<%# ((CartProduct)Container.DataItem).ProductName %>" disabled />
		<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
		<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
		<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
		<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# (((CartProduct)Container.DataItem).IsFixedPurchase) %>" />
		<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
		<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
		<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
		<asp:HiddenField ID="hfUnallocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' />
		<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# ((CartProduct)Container.DataItem).SubscriptionBoxCourseId %>" />
		<asp:HiddenField ID="hfSubscriptionBoxCourseName" runat="server" Value="<%# GetSubscriptionBoxDisplayName(((CartProduct)Container.DataItem).SubscriptionBoxCourseId) %>" />
		<asp:HiddenField ID="hfIsSubscriptionBox" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSubscriptionBox %>" />
		<div>
		<dl class="name">
		<dt>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
			<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
			<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
		</dt>
		<dd style="padding-top: 25px">
			<span>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
				<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
				<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<p class=\"message\">" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</p>" : "" %></span></dd>
		<dd visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
			<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
				<ItemTemplate>
					<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
					<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
				</ItemTemplate>
			</asp:Repeater>
		</dd>
		<dd>
			<asp:Repeater ID="rSetPromotion" DataSource="<%# GetSetPromotionByProduct((CartProduct)Container.DataItem) %>" runat="server">
				<ItemTemplate>
					<span class="setpromotion" visible='<%# ((SetPromotionModel)Container.DataItem).Url != "" %>' runat="server">
						「<a href="<%# WebSanitizer.HtmlEncode(Constants.PATH_ROOT + ((SetPromotionModel)Container.DataItem).Url) %>"><%# WebSanitizer.HtmlEncode(((SetPromotionModel)Container.DataItem).SetpromotionDispName) %></a>」対象商品
					</span>
					<span class="setpromotion" visible='<%# ((SetPromotionModel)Container.DataItem).Url == "" %>' runat="server">
						「<%# WebSanitizer.HtmlEncode(((SetPromotionModel)Container.DataItem).SetpromotionDispName) %>」対象商品
					</span>
				</ItemTemplate>
			</asp:Repeater>
		</dd>
		<dd visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
			<span style="color:red;">※配送料無料適用外商品です</span>
		</dd>
		</dl>
			<p class="price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
			<p class="quantity" style="padding-left: 110px; padding-right: 208px;" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() %>" runat="server"><%#: ((CartProduct)Container.DataItem).GetSubscriptionDisplayName() %></p>
			<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED ){ %>
			<p id="myOptionOne" style="text-align: left; width:142px; float: left; padding-top: 25px;" runat="server" Visible="<%#(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Items.Sum(co => co.TotalOptionPrice) != 0) || ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Items.Any(co => co.ProductOptionSettingList.HasOptionPrice)%>" >
				<span Visible="<%# (((CartProduct)Container.DataItem).ProductOptionSettingList.HasOptionPrice == false) %>" runat="server">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;―</span>
				<span Visible="<%# (((CartProduct)Container.DataItem).ProductOptionSettingList.HasOptionPrice) %>" runat="server"><%#: string.Format("{0}({1})", CurrencyManager.ToPrice(((CartProduct)Container.DataItem).TotalOptionPrice), this.ProductPriceTextPrefix) %></span>
			</p>
			<% } %>
		<p class="quantity">
			<span visible='<%# IsChangeProductCount((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem, (CartProduct)Container.DataItem) %>' runat="server">
			<asp:TextBox ID="tbProductCount" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' MaxLength="3"></asp:TextBox>
			</span>
			<span visible='<%# IsChangeProductCount((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem, (CartProduct)Container.DataItem) == false %>' runat="server">
				<%#  StringUtility.ToNumeric(((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet)%>
			</span>
		</p>
		<p class="taxRate" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: TaxCalculationUtility.GetTaxRateForDIsplay(((CartProduct)Container.DataItem).TaxRate) %>%</p>
		<p class="subtotal" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server">
			<%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price
										* ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet
										+ ((CartProduct)Container.DataItem).ProductOptionSettingList.SelectedOptionTotalPrice
										* ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet) %>
			(<%#: this.ProductPriceTextPrefix %>)
		</p>
		<p class="delete">
			<asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" Runat="server" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) == false %>">削除</asp:LinkButton>
			<asp:LinkButton ID="lbDeleteNecessaryProduct" OnClientClick="return delete_product_check_for_subscriptionBox(this);" Runat="server" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) %>" OnClick="lbDeleteCart_Click" CommandArgument="<%# ((IDataItemContainer)((RepeaterItem)Container.Parent.Parent)).DisplayIndex %>">削除</asp:LinkButton>
		</p>
		</div>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		<small class="fred pdg_leftA" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
			<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
		</small>
		</div><!--product-->
		
		<%-- セット商品 --%>
		<div class="product" style="background-color: white;" visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
		<%-- 隠し値 --%>
		<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSetItem %>" />
		<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# GetProductSetId((CartProduct)Container.DataItem) %>" />
		<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# GetProductSetNo((CartProduct)Container.DataItem) %>" />
		<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSetItemNo %>" />
		<div>
		<asp:Repeater id="rProductSet" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" OnItemCommand="rCartList_ItemCommand" runat="server">
		<HeaderTemplate>
			<table cellpadding="0" cellspacing="0" width="950" summary="ショッピングカート">
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
			<td class="name">
			<dl>
			<dt>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
			</dt>
			<dd>
				<span>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %> x <%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) %></a>
					<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) + " x " + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) : ""%>
					<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<br/><p class=\"message\">" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</p>" : "" %>
				</span>
			</dd>
			<dd visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
				<span style="color:red;">※配送料無料適用外商品です</span>
			</dd>
			</dl>
				<p class="price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</p></td>
			<td Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() %>" runat="server">
				<p class="quantity" style="padding-left: 40px; padding-right: 100px;"><%#: ((CartProduct)Container.DataItem).GetSubscriptionDisplayName() %></p>
			</td>
			<td Visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" rowspan="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items.Count : 1 %>" class="quantity" runat="server">
				<asp:TextBox ID="tbProductSetCount" Runat="server" Text='<%# GetProductSetCount((CartProduct)Container.DataItem) %>' MaxLength="3" CssClass="orderCount"></asp:TextBox></td>
			<td class="taxRate" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server">
				<%#: TaxCalculationUtility.GetTaxRateForDIsplay(((CartProduct)Container.DataItem).TaxRate) %>%</td>
			<td Visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == 1) || (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) %>" rowspan="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items.Count : 1 %>" class="subtotal" runat="server">
				<%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal((CartProduct)Container.DataItem)) %> (<%#: this.ProductPriceTextPrefix %>)</td>
			<td Visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" rowspan="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items.Count : 1 %>" class="delete" runat="server">
				<asp:LinkButton ID="lbDeleteProductSet" CommandName="DeleteProductSet" CommandArgument='' Runat="server">削除</asp:LinkButton></td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
		</asp:Repeater>
		</div>
		<small class="fred pdg_leftA" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
			<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
		</small>
		</div><!--product-->
		
	</ItemTemplate>
	</asp:Repeater>
	<!-- ▽セットプロモーション商品▽ -->
	<asp:Repeater ID="rCartSetPromotion" DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
	<ItemTemplate>
		<asp:HiddenField ID="hfCartSetPromotionNo" runat="server" Value="<%# ((CartSetPromotion)Container.DataItem).CartSetPromotionNo %>" />
		<div class="product" style="background-color: white;">
			<div>
				<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" OnItemCommand="rCartList_ItemCommand" runat="server">
				<HeaderTemplate>
					<table cellpadding="0" cellspacing="0" summary="ショッピングカート">
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td class="name">
							<dl>
							<dt>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
									<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
							</dt>
							<dd>
								<span>
									<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
										<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName)%></a>
									<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
								</span>
							</dd>
							<dd visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
							<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
								<ItemTemplate>
									<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
									<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
								</ItemTemplate>
							</asp:Repeater>
							</dd>
							<dd visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								<span style="color:red;">※配送料無料適用外商品です</span>
							</dd>
							</dl>
						</td>
						<td>
						<p class="price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
						</td>
						<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED){ %>
						<td>
						<p runat="server" Visible="<%# HasProductOptionPrice((CartObject)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem) %>" style="padding-top: 25px; width: 180px;">
							<span Visible="<%# (((CartProduct)Container.DataItem).ProductOptionSettingList.HasOptionPrice == false) %>" runat="server">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;―</span>
							<span Visible="<%# (((CartProduct)Container.DataItem).ProductOptionSettingList.HasOptionPrice) %>" runat="server"><%#: string.Format("{0}({1})", CurrencyManager.ToPrice(((CartProduct)Container.DataItem).TotalOptionPrice), this.ProductPriceTextPrefix) %></span>
						</p>
						</td>
						<% } %>
						<td class="name" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() %>" runat="server">
							<p class="quantity" style="padding-left: 40px; padding-right: 100px;"><%#: ((CartProduct)Container.DataItem).GetSubscriptionDisplayName() %></p>
						</td>
						<td class="quantity" style="padding-top:25px; width:100px;">
							<span visible="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem).IsGift == false %>" runat="server">
								<asp:TextBox ID="tbSetPromotionItemCount" Runat="server" Text="<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo] %>" MaxLength="3" CssClass="orderCount"></asp:TextBox><br />
								<small class="fred" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex, ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
									<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex, ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
								</small>
							</span>
							<span visible="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem).IsGift %>" runat="server">
								<%# StringUtility.ToNumeric(((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]) %>
							</span>
						</td>
						<td class="quantity" style="font-weight: bold; width: 70px;" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) && (HasProductOptionPrice((CartObject)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem) == false) %>" runat="server">
							<%# WebSanitizer.HtmlEncode(TaxCalculationUtility.GetTaxRateForDIsplay(((CartProduct)Container.DataItem).TaxRate)) %>%
						</td>
						<td class="quantityOption" style="padding-top: 25px; padding-left:45px; font-weight: bold; white-space: nowrap;" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) && HasProductOptionPrice((CartObject)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem) %>" runat="server">
							<%# WebSanitizer.HtmlEncode(TaxCalculationUtility.GetTaxRateForDIsplay(((CartProduct)Container.DataItem).TaxRate)) %>%
						</td>
						<td style="padding-top:15px;" visible="<%# (Container.ItemIndex == 0) && (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) && (HasProductOptionPrice((CartObject)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem) == false) %>" rowspan="<%# ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).Items.Count %>" class="subtotal" runat="server">
							<span visible="<%# ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).IsDiscountTypeProductDiscount %>" runat="server">
								<strike><%#: CurrencyManager.ToPrice(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).UndiscountedProductSubtotal) %> (税込)</strike><br />
							</span>
							<%#: CurrencyManager.ToPrice(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).ProductDiscountAmount) %> (税込)<br />
							<%# WebSanitizer.HtmlEncode(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).SetpromotionDispName) %>
						</td>
						<td style="padding-top: 15px; white-space: nowrap; padding-left: 35px; width: 112px; text-align: center; font-weight: bold;" visible="<%# (Container.ItemIndex == 0) && (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) && HasProductOptionPrice((CartObject)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem) %>" rowspan="<%# ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).Items.Count %>" runat="server">
							<span visible="<%# ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).IsDiscountTypeProductDiscount %>" runat="server">
								<strike><%#: CurrencyManager.ToPrice(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).UndiscountedProductSubtotal) %> (税込)</strike><br />
							</span>
							<%#: CurrencyManager.ToPrice(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).ProductDiscountAmount) %> (税込)<br />
							<%# WebSanitizer.HtmlEncode(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).SetpromotionDispName) %>
						</td>
						<td class="delete" style="padding-left: 150px;">
							<asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" CommandArgument='' Runat="server">削除</asp:LinkButton>
							<%-- 隠し値 --%>
							<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
							<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
							<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
							<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# (((CartProduct)Container.DataItem).IsFixedPurchase) || (((CartProduct)Container.DataItem).IsSubscriptionBox) %>" />
							<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
							<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
							<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
							<asp:HiddenField ID="hfAllocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo] %>' />
							<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# ((CartProduct)Container.DataItem).SubscriptionBoxCourseId %>" />
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
					</table>
				</FooterTemplate>
				</asp:Repeater>
			</div>
		</div>
	</ItemTemplate>
	</asp:Repeater>
	<!-- △セットプロモーション商品△ -->

	<%-- ▽ノベルティ▽ --%>
	<asp:Repeater ID="rNoveltyList" runat="server" DataSource="<%# GetCartNovelty(((CartObject)Container.DataItem).CartId) %>" Visible="<%# GetCartNovelty(((CartObject)Container.DataItem).CartId).Length != 0 %>">
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
			<div class="novelty clearFix">
			<h4 class="title">
				<%# WebSanitizer.HtmlEncode(((CartNovelty)Container.DataItem).NoveltyDispName) %>を追加してください。
			</h4>
			<p runat="server" visible="<%#((CartNovelty)Container.DataItem).GrantItemList.Length == 0 %>">
				ただいま付与できるノベルティはございません。
			</p>
			<asp:Repeater ID="rNoveltyItem" runat="server" DataSource="<%# ((CartNovelty)Container.DataItem).GrantItemList %>" OnItemCommand="rCartList_ItemCommand">
				<ItemTemplate>
					<div class="plist">
						<p class="image">
							<w2c:ProductImage ProductMaster="<%# ((CartNoveltyGrantItem)Container.DataItem).ProductInfo %>" IsVariation="true" ImageSize="M" runat="server" />
						</p>
						<p class="name"><%# WebSanitizer.HtmlEncode(((CartNoveltyGrantItem)Container.DataItem).JointName) %></p>
						<p class="price"><%#: CurrencyManager.ToPrice(((CartNoveltyGrantItem)Container.DataItem).Price) %>(<%#: this.ProductPriceTextPrefix %>)</p>
						<p class="add">
							<asp:LinkButton ID="lbAddNovelty" runat="server" CommandName="AddNovelty" CommandArgument='<%#  string.Format("{0},{1}", ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>' class="btn btn-mini">カートに追加</asp:LinkButton>
						</p>
						<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" /></p>
					</div><!--product-->
				</ItemTemplate>
			</asp:Repeater>
			</div><!--novelty-->
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
	</asp:Repeater>
	<%-- △ノベルティ△ --%>

	</div><!--list-->

	<uc:BodyAnnounceFreeShipping runat="server" TargetCart="<%# ((CartObject)Container.DataItem) %>" />

	<div class="cartOrder">
	<div class="subcartOrder">

	<div class="pointBox" visible="<%# Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn %>" runat="server">
	<div class="box">
	<p><img src="../../Contents/ImagesPkg/common/ttl_point.gif" alt="ポイントを使う" width="262" height="23" /></p>
	<div class="boxbtm">
	<div>
	<div>
	<dl runat="server" visible="<%# ((CartObject)Container.DataItem).CanUsePointForPurchase %>">
	<dt>今回合計 <%# WebSanitizer.HtmlEncode(GetNumeric(this.LoginUserPointUsable))%> ポイントまでご利用いただけます
	<span>※1<%= Constants.CONST_UNIT_POINT_PT %> = <%: CurrencyManager.ToPrice(1m) %></span>
	</dt>
	<dd><asp:TextBox ID="tbOrderPointUse" Runat="server" Text="<%# GetUsePoint((CartObject)Container.DataItem) %>" MaxLength="6"></asp:TextBox>&nbsp;&nbsp;<%= Constants.CONST_UNIT_POINT_PT %></dd>
	</dl>
		<dl runat="server" visible="<%# (((CartObject)Container.DataItem).CanUsePointForPurchase == false) %>">
			<p>
				あと「<%#: GetPriceCanPurchaseUsePoint(((CartObject)Container.DataItem).PurchasePriceTotal) %>」の購入でポイントをご利用いただけます。
			</p>
			<p runat="server" visible="<%# (this.LoginUserPointUsable > 0) %>">
				※利用可能ポイント「<%#: GetNumeric(this.LoginUserPointUsable) %>pt」
			</p>
		</dl>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<% if (this.CanUseAllPointFlg && this.IsLoggedIn) { %>
	<asp:CheckBox ID="cbUseAllPointFlg" Text="定期注文で利用可能なポイント<br>すべてを継続使用する" Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>"
		OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
		CssClass="cbUseAllPointFlg" Style="margin-left: 1.4em; text-indent: -1.6em;" AutoPostBack="True" runat="server"/>
	<span Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>" runat="server">※注文後はマイページ＞定期購入情報より<br/>変更できます。</span>
	<% } %>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<span class="fred" visible="<%# this.ErrorMessages.HasMessages(Container.ItemIndex, CartErrorMessages.ErrorKbn.Point) %>" runat="server">
		<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(Container.ItemIndex, CartErrorMessages.ErrorKbn.Point)) %>
	</span>
	</div><!--boxbtm-->
	</div><!--box-->
	</div><!--pointBox-->
	<div class="couponBox" visible="<%# Constants.W2MP_COUPON_OPTION_ENABLED %>" runat="server">
	<div class="box">
	<p><img src="../../Contents/ImagesPkg/common/ttl_coupon.gif" alt="クーポンを使う" width="262" height="23" /></p>
	<div id="divCouponInputMethod" runat="server"
		style="font-size: 10px; padding: 10px 10px 0px 10px; font-family: 'Lucida Grande','メイリオ',Meiryo,'Hiragino Kaku Gothic ProN', sans-serif; color: #333;">
		<asp:RadioButtonList runat="server" AutoPostBack="true" ID="rblCouponInputMethod"
			OnSelectedIndexChanged="rblCouponInputMethod_SelectedIndexChanged" OnDataBinding="rblCouponInputMethod_DataBinding"
			DataSource="<%# GetCouponInputMethod() %>" DataTextField="Text" DataValueField="Value" RepeatColumns="2" RepeatDirection="Horizontal"></asp:RadioButtonList>
	</div>
	<div class="boxbtm">
	<div>
	<div id="hgcCouponSelect" runat="server">
		<asp:DropDownList CssClass="input_border" style="width: 240px" ID="ddlCouponList" runat="server" DataTextField="Text" DataValueField="Value" OnTextChanged="ddlCouponList_TextChanged" AutoPostBack="true"></asp:DropDownList>
	</div>
	<div>
	<dl>
	<dl id="hgcCouponCodeInputArea" runat="server">
	<dt><span>クーポンコード</span></dt>
	<dd><asp:TextBox ID="tbCouponCode" runat="server" Text="<%# GetCouponCode(((CartObject)Container.DataItem).Coupon) %>" MaxLength="30" autocomplete="off"></asp:TextBox></dd>
	</dl>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<span class="fred" visible="<%# this.ErrorMessages.HasMessages(Container.ItemIndex, CartErrorMessages.ErrorKbn.Coupon) %>" runat="server">
		<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(Container.ItemIndex, CartErrorMessages.ErrorKbn.Coupon)) %>
	</span>
	<asp:LinkButton runat="server" ID="lbShowCouponBox" Text="クーポンBOX" 
		style="color: #ffffff !important; background-color: #000 !important;
		border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
		padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
		border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap;"
		OnClick="lbShowCouponBox_Click" ></asp:LinkButton>
	</div><!--boxbtm-->
	</div><!--box-->
	<div runat="server" id="hgcCouponBox" style="z-index: 1; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75);"
		Visible='<%# ((CartObject)Container.DataItem).CouponBoxVisible %>'>
	<div id="hgcCouponList" style="width: 800px; height: 500px; top: 50%; left: 50%; text-align: center; border: 2px solid #aaa; background: #fff; position: fixed; z-index: 2; margin:-250px 0 0 -400px;">
	<h2 style="height: 20px; color: #fff; background-color: #000; font-size: 16px; padding: 3px 0px; border-bottom: solid 1px #ccc; ">クーポンBOX</h2>
	<div style="height: 400px; overflow: auto;">
	<asp:Repeater ID="rCouponList" ItemType="UserCouponDetailInfo" Runat="server" DataSource="<%# GetUsableCoupons((CartObject)Container.DataItem) %>">
		<HeaderTemplate>
		<table>
			<tr>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:150px;">クーポンコード</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:230px;">クーポン名</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:170px;">割引金額<br />/割引率</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:70px;">利用可能回数</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:350px;">有効期限</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:100px;"></th>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:150px; background-color: white;">
					<%#: Item.CouponCode %><br />
					<asp:HiddenField runat="server" ID="hfCouponBoxCouponCode" Value="<%# Item.CouponCode %>" />
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:230px; background-color: white;"
					title="<%#: Item.CouponDispDiscription %>">
					<%#: Item.CouponDispName %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px; background-color: white;">
					<%# WebSanitizer.HtmlEncodeChangeToBr(GetCouponDiscountString(Item)) %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px; background-color: white;">
					<%#: GetCouponCount(Item) %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:350px; background-color: white;">
					<%#: DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:100px; background-color: white;">
					<asp:LinkButton runat="server" id="lbCouponSelect" OnClick="lbCouponSelect_Click" style="color: #ffffff !important; background-color: #000 !important;
					border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
					padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
					border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap;">このクーポンを使う</asp:LinkButton>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:Repeater>
	</div>
	<div style="width: 100%; height: 50px; display: block; z-index: 3">
		<asp:LinkButton ID="lbCouponBoxClose" OnClick="lbCouponBoxClose_Click" runat="server"
			style="padding: 8px 12px; font-size: 14px; color: #333; text-decoration: none; border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);
			display: inline-block; line-height: 18px; color: #333333; text-align: center; vertical-align: middle; border-radius: 5px; cursor: pointer; background-color: #f5f5f5;
			border: 1px solid #cccccc; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); text-decoration: none; background-image: none; margin: 5px auto">クーポンを利用しない
		</asp:LinkButton>
	</div>
	</div>
	</div>
	</div><!--couponBox-->

	<div class="priceList">
	<div>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotal) %></dd>
	</dl>
	<%if (this.ProductIncludedTaxFlg == false) { %>
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt>消費税額</dt>
			<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %></dd>
		</dl>
	<%} %>
	<%-- セットプロモーション(商品割引) --%>
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">
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
	<dd class='<%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).MemberRankDiscount * ((((CartObject)Container.DataItem).MemberRankDiscount < 0) ? -1 : 1)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期会員割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount * ((((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount < 0) ? -1 : 1)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
	<span runat="server" visible="<%# (((CartObject)Container.DataItem).HasFixedPurchase) %>">
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期購入割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "minus" : "" %>'><%#: (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseDiscount * ((((CartObject)Container.DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %></dd>
	</dl>
	</span>
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
	<dt>配送料金</dt>
	<dd runat="server" style='<%# ((((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg) || (((CartObject)Container.DataItem).IsDisplayShippingPriceUnsettled)) ? "display:none;" : ""%>'>
		<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %></dd>
	<dd runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg == false) ? "display:none;" : ""%>'>
		<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).ShippingPriceSeparateEstimateMessage)%></dd>
	<dd runat="server" style='<%#  (((CartObject)(Container).DataItem).ShippingPriceSeparateEstimateFlg == false) && (((CartObject)(Container).DataItem).IsDisplayShippingPriceUnsettled == false) ? "display:none;" : "color:red"%>'>
		配送先入力後に確定となります。</dd>
	</dl>
		<span runat="server" Visible="<%# ((((CartObject)Container.DataItem).Payment) != null) %>">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt>決済手数料</dt>
			<dd><%#: (((CartObject)Container.DataItem).Payment != null) ? CurrencyManager.ToPrice(((CartObject)Container.DataItem).Payment.PriceExchange) : "" %></dd>
		</dl>
		</span>
		<%-- セットプロモーション(配送料割引) --%>
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
	</div>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	<div>
	<dl class="result">
	<dt>合計(税込)</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %></dd>
	</dl>
	</div>
	</div><!--priceList-->
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div><!--subcartOrder-->
	</div><!--cartOrder-->
	</div><!--background-->
	</div><!--productList-->
	
	<%-- 隠し値：カートID --%>
	<asp:HiddenField ID="hfCartId" runat="server" Value="<%# ((CartObject)Container.DataItem).CartId %>" />
	<%-- 隠し再計算ボタン --%>
	<asp:LinkButton id="lbRecalculateCart" runat="server" CommandArgument="<%# Container.ItemIndex %>" onclick="lbRecalculate_Click"></asp:LinkButton>
</ItemTemplate>
</asp:Repeater>
<%} %>

<%if (this.CartList.Items.Count != 0) {%>
<p class="sum"><img src="../../Contents/ImagesPkg/cartlist/ttl_sum.gif" alt="総合計" width="48" height="16" /><strong><%: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %></strong></p>
<%} %>

<div class="btmbtn below">
<ul>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>" class="btn btn-large btn-gry">買い物を続ける</a></li>
	<%if (this.CartList.Items.Count != 0) {%>
	<%-- UPDATE PANELの外のイベントを呼び出す --%>
		<% if (this.Process.IsSubscriptionBoxError == false) { %>
		<li><asp:Linkbutton Visible="<%# this.IsDisplayTwoClickButton %>" ID="lbTwoClickButton2" OnClick="lbTwoClickButton_Click" class="btn btn-success" runat="server">２クリック購入</asp:Linkbutton></li>
		<li><a href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>" class="btn btn-large btn-success">ご購入手続き</a></li>
		<% } %>
	<%} %>
</ul>
<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
</div><!--btmbtn-->
</div>

</div><!--CartList-->

</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
<div>
	<!-- シルバーエッグ連携時使用 -->
	<uc:BodyProductRecommendByRecommendEngine runat="server" RecommendCode="pc413" RecommendTitle="おすすめ商品一覧" MaxDispCount="6" RecommendProductId="<%# GetCartProductsForSilveregg() %>" DispCategoryId="" NotDispCategoryId="" NotDispRecommendProductId="" />
</div>
<%-- CRITEOタグ（引数：カート情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.CartList %>" />
	
<% if(Constants.AMAZON_PAYMENT_CV2_ENABLED){ %>
	<%--▼▼ AmazonPay(CV2)スクリプト ▼▼--%>
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
	<%-- ▲▲AmazonPay(CV2)スクリプト▲▲ --%>
<% } else { %>
	<%--▼▼Amazonウィジェット用スクリプト▼▼--%>
	<script type="text/javascript">
		
		window.onAmazonLoginReady = function () {
			amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
		};
		window.onAmazonPaymentsReady = function () {
			if ($('#AmazonPayButton').length) showButton();
		};

		<%-- Amazonボタン表示ウィジェット --%>
		function showButton() {
			var authRequest;
			OffAmazonPayments.Button("AmazonPayButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
				type: "PwA",
				color: "Gold",
				size: "medium",
				authorization: function () {
					loginOptions = {
						scope: "payments:widget payments:shipping_address profile",
						popup: true
					};
					authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_ORDER_CALLBACK) %>");
				},
				onError: function (error) {
					alert(error.getErrorMessage());
				}
			});
		};
	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
	<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
<% } %>
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
<script>
	var subscriptionBoxMessage = '「@@ 1 @@」は「@@ 2 @@」の必須商品です。\n削除すると、「@@ 2 @@」の申し込みがキャンセルされます。\nよろしいですか？';

	// 必須商品チェック(頒布会)
	function delete_product_check_for_subscriptionBox(value) {
		var subscriptionBoxProductName = $(value).parent().parent().parent().find("[id$='hfProductName']").val();
		var subscriptionBoxProductId = $(value).parent().parent().parent().find("[id$='hfProductId']").val();
		var subscriptionBoxName = $(value).parent().parent().parent().find("[id$='hfSubscriptionBoxCourseName']").val();
		var subscriptionBoxId = $(value).parent().parent().parent().find("[id$='hfSubscriptionBoxCourseId']").val();
		subscriptionBoxMessage = subscriptionBoxMessage.replace('@@ 1 @@', subscriptionBoxProductName);
		subscriptionBoxMessage = subscriptionBoxMessage.replace('@@ 2 @@', subscriptionBoxName);
		subscriptionBoxMessage = subscriptionBoxMessage.replace('@@ 2 @@', subscriptionBoxName);
		return confirm(subscriptionBoxMessage);
	}
</script>
</asp:Content>
