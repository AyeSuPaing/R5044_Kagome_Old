<%--
=========================================================================================================
  Module      : スマートフォン用注文完了画面(OrderComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/SmartPhone/Form/Common/BodyRecommendAtOrderComplete.ascx" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderComplete.aspx.cs" Inherits="Form_Order_OrderComplete" Title="注文完了ページ" %>
<%@ Register TagPrefix="uc" TagName="BodyOrderConfirmRecommend" Src="~/SmartPhone/Form/Common/BodyOrderConfirmRecommend.ascx" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content3" ContentPlaceHolderID="AffiliateTagHead" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagHead"
					Location="head"
					Datas="<%# this.OrderList %>"
					IsAlreadyDisplayed="<%# this.IsAleadyDisplayed %>"
					runat="server"/>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="AffiliateTagBodyTop" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyTop"
					Location="body_top"
					Datas="<%# this.OrderList %>"
					IsAlreadyDisplayed="<%# this.IsAleadyDisplayed %>"
					runat="server"/>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="AffiliateTagBodyBottom" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyBottom"
					Location="body_bottom"
					Datas="<%# this.OrderList %>"
					IsAlreadyDisplayed="<%# this.IsAleadyDisplayed %>"
					runat="server"/>
</asp:Content>
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
<section class="wrap-order order-complete">

<div class="step">
	<% if (Constants.CART_LIST_LP_OPTION == false) { %>
	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-step05.jpg" alt="ご注文完了" width="320" />
	<% } else { %>
	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-lp-step03.jpg" alt="ご注文完了" width="320" />
	<% } %>
</div>

<!--アフィリエイトタグ出力-->
<asp:Repeater ID="rAffiliateTag" runat="server" Visible="<%# Constants.SETTING_PRODUCTION_ENVIRONMENT %>">
	<ItemTemplate>
		<%# Container.DataItem %>
	</ItemTemplate>
</asp:Repeater>
<div class="order-unit">
	<h2>ご注文ありがとうございます</h2>

	<asp:Repeater ID="rOrderHistory" runat="server">
	<HeaderTemplate>
		<div class="order-history-id">
	</HeaderTemplate>
	<ItemTemplate>
		<h3>カート番号<%# Container.ItemIndex + 1 %>：<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_ID)) %></h3>
		<%-- コンビニ決済用 --%>
		<div class="cvs-list" visible='<%# NeedToShowPaymentMessageHtml((string)GetKeyValue(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PAYMENT_KBN)) %>' runat="server">
			<%# GetKeyValue(this.OrderInfos[Container.ItemIndex], Constants.PAYMENT_MESSAGE_HTML) %>
		</div>
		<w2c:FacebookConversionAPI
			EventName="Purchase"
			EventId="<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_ID) %>"
			CustomDataOrderId="<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_ID) %>"
			UserId="<%#: this.LoginUserId %>"
			runat="server" />
	</ItemTemplate>
	<FooterTemplate>
		</div>
	</FooterTemplate>
	</asp:Repeater>

	<div class="msg">
		注文内容を記載したEメールをお送りしました。届かない場合など御座いましたらご連絡ください。
		<%if (this.IsLoggedIn == false){ %>
			<br />
			<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
			<br />
				今ご登録頂けると、<%= WebSanitizer.HtmlEncode(GetNumeric(PointOptionUtility.GetAddPoint((List<CartObject>)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_CART_LIST])))%>ポイントが加算されます。<br />
			<%} %>
			<br />
			<a href="<%= WebSanitizer.UrlAttrHtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION) %>" class="btn regist-member">
			新規会員登録はこちらから</a>
		<%} %>
		<br />
		<br />
		今後とも当店のご利用を心よりお待ち申し上げております。
		<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
			<div id="hGmoInreviewContent" runat="server"></div>
		<% } %>
	</div>
	<% if(Constants.RECOMMEND_OPTION_ENABLED){ %>
	<uc:BodyOrderConfirmRecommend runat="server" OrderList="<%# this.OrderList %>" />
	<% } %>
	<uc:BodyRecommend runat="server" OrderList="<%# this.OrderList %>" />
		<%-- 外部決済キャンセルは注文確認画面へ戻る --%>
	<% if ((this.CartList.Items.Count != 0) && (rErrorMesseges.Visible == false)) { %>
		注文が完了していないカートがあります。続けて注文を行う場合はこちら。<br />
		<% if (this.CartList.Items.Count(item => item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY) != 0) { %>
			Paidyのご利用は承認されませんでした。恐れ入りますが他のお支払方法をご選択ください。<br />
		<%} %>
		<asp:LinkButton runat="server" OnClick="lbRetryOrder_Click">続けて注文を行う</asp:LinkButton>
	<%} %>

	<%-- ▼エラー情報▼ --%>
	<asp:Repeater ID="rErrorMesseges" runat="server">
	<HeaderTemplate>
		<p class="attention">
		※一部の注文にエラーが発生致しました。
	</HeaderTemplate>
	<ItemTemplate>
		<%# WebSanitizer.HtmlEncode(Container.DataItem) %><br />
	</ItemTemplate>
	<FooterTemplate>
		</p>
	</FooterTemplate>
	</asp:Repeater>
	<asp:LinkButton ID="lbRetryOrder" runat="server" OnClick="lbRetryOrder_Click">失敗した注文をやり直す</asp:LinkButton>
	<%-- ▲エラー情報▲ --%>
</div>

<asp:PlaceHolder ID="pfDocomoPayment" runat="server">
<div class="order-unit">
	<%-- ドコモケータイ払い注文（目立たせたいため完了情報よりも上に配置） --%>
	ドコモケータイ払いの決済は、携帯電話で行っていただく必要があります。<br />
		決済を行う携帯電話のメールアドレスを入力し、送信ボタンを押してください。<br />
		決済処理は、メールに記載されている内容にしたがって進めてください。<br />
		ドメイン指定受信を設定されている方は、必ず「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopMailDomain")) %>」を受信できるように設定してください。<br />
		<asp:TextBox ID="tbMobileMailAddr" MaxLength="240" Width="150" runat="server"></asp:TextBox>@docomo.ne.jp
		<asp:Button ID="bSendDocomoPaymentMail" runat="server" OnClick="bSendDocomoPaymentMail_Click" Text="送信" /><br />
		<%= this.DocomoPaymentErrorMessage %>
</div>
</asp:PlaceHolder>
<%-- ▲ドコモケータイ払い用決済情報▲ --%>
	
	<%-- DSK後払いで与信がHOLDの場合 --%>
	<div class="order-unit">
		<div runat="server" Visible="<%# this.IsDskDeferredAuthResultHold %>" class="msg">
			<p>
				DSK後払いをご利用いただきありがとうございます。現在は与信審査中ですのでしばらくお待ちください。<br>
				審査結果によってはご利用いただけない場合がございます。<br>
				その場合には別の決済方法に変更させていただく場合がございます。
			</p>
		</div>
	</div>
	<%-- スコア後払いで与信がHOLDの場合 --%>
	<div runat="server" Visible="<%# this.IsVeritransDeferredAuthResultHold %>" class="msg">
		<p>
			<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_VERITRANS_CVSDEFAUTH_ERROR) %>
		</p>
	</div>

	<%-- スコア後払いで与信がHOLDの場合 --%>
	<div runat="server" Visible="<%# this.IsScoreDeferredAuthResultHold %>" class="msg">
		<p>
			<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SCORE_CVSDEFAUTH_ERROR) %>
		</p>
	</div>
	
	<%-- GMOアトカラの与信結果が審査中の場合 --%>
	<div runat="server" Visible="<%# this.IsGmoAtokaraAuthResultHold %>" class="msg">
		<p>
			アトカラをご利用いただきありがとうございます。現在は与信審査中ですのでしばらくお待ちください。
		</p>
	</div>

	<%-- ▼GoogleAnalyticsタグ出力▼ --%>
	<% if ((Constants.GOOGLEANALYTICS_ENABLED) && (Constants.SETTING_PRODUCTION_ENVIRONMENT)) { %>
	<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/cookie-utils.js"></script>
	<asp:Repeater ID="rGoogleAnalytics" runat="server">
		<HeaderTemplate>
			<%-- GA4用 --%>
			<script async src="https://www.googletagmanager.com/gtag/js?id=<%= (string.IsNullOrEmpty(Constants.GOOGLEANALYTICS_PROFILE_ID) ? Constants.GOOGLEANALYTICS_MEASUREMENT_ID : Constants.GOOGLEANALYTICS_PROFILE_ID) %>"></script>
			<script type="text/javascript">
				window.dataLayer = window.dataLayer || [];
				function gtag() { dataLayer.push(arguments); }
				gtag('js', new Date());
				<% if (string.IsNullOrEmpty(Constants.GOOGLEANALYTICS_PROFILE_ID) == false) { %>
				gtag('config', '<%= Constants.GOOGLEANALYTICS_PROFILE_ID %>');
				<% } %>
				gtag('config', '<%= Constants.GOOGLEANALYTICS_MEASUREMENT_ID %>');
			</script>
		</HeaderTemplate>
		<ItemTemplate>
			<script type='text/javascript'>
				<%-- GoogleAnalyticsタグ制御用注文IDが存在する？--%>
				var cookieKey = '<%# Constants.COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID + (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_ID] %>';
				if (!getCookie(cookieKey)) {
					<%-- GA4トランザクション計測タグ --%>
					gtag('event', 'purchase', {
						"transaction_id": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_ID] %>",
						"value": '<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL].ToPriceString() %>',
						"currency": "<%#: Constants.CONST_KEY_CURRENCY_CODE %>",
						"shipping": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING].ToPriceString() %>",
						"items": [
							<asp:Repeater DataSource='<%# ((Hashtable)Container.DataItem)["order_items"] %>' runat="server">
								<ItemTemplate>
									{
										"item_id": "<%#:((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_VARIATION_ID] %>",
										"item_name": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_PRODUCT_NAME] %>",
										"category": "<%#: GetProductBrandName((Hashtable)Container.DataItem) + GetProductCategoryName((Hashtable)Container.DataItem) %>",
										"quantity": <%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] %>,
										"price": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToPriceString() %>",
									},
								</ItemTemplate>
							</asp:Repeater>
						]
					});

					setCookie(cookieKey, null);
				}
			</script>
		</ItemTemplate>
	</asp:Repeater>
	<%} %>
	<%-- ▲GoogleAnalyticsログ出力（UniversalAnalytics版）▲ --%>
	<div class="cart-footer">
		<div class="button-next">
			<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>" class="btn">トップページへ</a>
		</div>
	</div>

<%-- CRITEOタグ（引数：注文情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.OrderList %>" />
</section>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
</asp:Content>
